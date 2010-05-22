/*
Copyright (C) 2009 Brendan Doherty

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;

namespace TrailsPlugin.Data {
	class ActivityTrail {
		private IActivity m_activity;
		private Data.Trail m_trail;
		private IList<Data.TrailResult> m_resultsList;

		public ActivityTrail(IActivity activity, Data.Trail trail) {
			m_activity = activity;
			m_trail = trail;

		}
		public IActivity Activity {
			get {
				return m_activity;
			}
		}
		public Data.Trail Trail {
			get {
				return m_trail;
			}
		}

		public bool IsInBounds {
			get {
				IGPSBounds gpsBounds = GPSBounds.FromGPSRoute(m_activity.GPSRoute);
				return m_trail.IsInBounds(gpsBounds);
			}
		}
        private float checkPass(IGPSPoint r1, float d1, IGPSPoint r2, float d2, TrailGPSLocation trailp, float radius)
        {
            float factor = 0;
            if (r1 == null || r2 == null || trailp == null) return factor;

            //Check if the line goes via the "circle" if the sign changes
            //Also need to check close points that fit in a 45 deg tilted "square" where sign may not change
            if (r1.LatitudeDegrees > trailp.LatitudeDegrees
                                && r2.LatitudeDegrees < trailp.LatitudeDegrees
                || r1.LatitudeDegrees < trailp.LatitudeDegrees
                                && r2.LatitudeDegrees > trailp.LatitudeDegrees
                || r1.LongitudeDegrees > trailp.LongitudeDegrees
                                && r2.LongitudeDegrees < trailp.LongitudeDegrees
                || r1.LongitudeDegrees < trailp.LongitudeDegrees
                                && r2.LongitudeDegrees > trailp.LongitudeDegrees
                || d1 < radius * Math.Sqrt(2)
                || d2 < radius * Math.Sqrt(2))
            {
                //Law of cosines - get angle at r1
                double d3 = r1.DistanceMetersToPoint(r2);
                double a1 = Math.Acos((d1 * d1 + d3 * d3 - d2 * d2) / (2 * d1 * d3));
                //Dist from r1 to closest point
                double d = d1*Math.Cos(a1);
                //Point is in circle if closest point is between r1&r2 and it is in circle (neither r1 nor r2 is)
                if (d < d3 && d1*Math.Sin(a1) < radius)
                {
                    factor = 1 - (float)(d / d3);
                    //Return factor, to return best aproximation something like below could be used
                    //The time should also be estimated 
                    //Lineary extend - not exact but works on shorter distances
                    //resultPoint = new GPSPoint(
                    //    r1.LatitudeDegrees + factor*(r2.LatitudeDegrees - r1.LatitudeDegrees),
                    //    r1.LongitudeDegrees + factor*(r2.LongitudeDegrees - r1.LongitudeDegrees),
                    //    r1.ElevationMeters + factor*(r2.ElevationMeters - r1.ElevationMeters));
                }
            }
            return factor;
        }
		public IList<TrailResult> Results {
			get {
				if (m_resultsList == null) {
					m_resultsList = new List<TrailResult>();
					if (m_activity.GPSRoute != null && m_activity.GPSRoute.Count > 1) {

						int trailIndex = 0;
						int startIndex = -1, endIndex = -1;
						bool stillInStartRadius = false;
                        IGPSPoint prevRoutePoint = null;
                        float prevDistToPoint = 0;
						for (int routeIndex = 0; routeIndex < m_activity.GPSRoute.Count; routeIndex++) {
							IGPSPoint routePoint = m_activity.GPSRoute[routeIndex].Value;

							if (stillInStartRadius) {
								float distToStart = this.m_trail.TrailLocations[0].DistanceMetersToPoint(routePoint);
								if (distToStart > this.Trail.Radius) {
									stillInStartRadius = false;
								}
							}

							if (!stillInStartRadius && trailIndex != 0) {
								float distFromStartToPoint = this.m_trail.TrailLocations[0].DistanceMetersToPoint(routePoint);
								if (distFromStartToPoint < this.m_trail.Radius) {
									trailIndex = 0;
								}
							}

                            int matchIndex = -1;
							float distToPoint = this.m_trail.TrailLocations[trailIndex].DistanceMetersToPoint(routePoint);
                            if (distToPoint < this.Trail.Radius)
                            {
                                matchIndex = routeIndex;
                                for (int routeIndex2 = routeIndex + 1; routeIndex2 < m_activity.GPSRoute.Count; routeIndex2++)
                                {
                                    //Get closest point, abort when track is moving away from middle
                                    IGPSPoint routePoint2 = m_activity.GPSRoute[routeIndex2].Value;
                                    float distToPoint2 = this.m_trail.TrailLocations[trailIndex].DistanceMetersToPoint(routePoint2);
                                    if (distToPoint2 + 0.5 > distToPoint)
                                    {
                                        matchIndex = routeIndex;
                                        break;
                                    }
                                    else
                                    {
                                        distToPoint = distToPoint2;
                                        routeIndex = routeIndex2;
                                    }
                                }
                            }
                            else
                            {
                                float factor = checkPass(prevRoutePoint, prevDistToPoint, routePoint, distToPoint, this.m_trail.TrailLocations[trailIndex], this.Trail.Radius);
                                if (0 < factor)
                                {
                                    //An estimated point (including time) could be inserted in the track
                                    matchIndex = prevDistToPoint > distToPoint ? routeIndex : routeIndex-1;
                                }
                            }

                            prevRoutePoint = routePoint;
                            prevDistToPoint = distToPoint;
                            if (matchIndex >= 0)
                            {
                                //This is a match, should not check following points for "passing by"
                                prevRoutePoint = null;

                                if (trailIndex == 0)
                                {
                                    // found the start						
                                    startIndex = matchIndex;
                                    trailIndex++;
                                    stillInStartRadius = true;

                                }
                                else if (trailIndex == this.m_trail.TrailLocations.Count - 1)
                                {
                                    // found the end
                                    endIndex = matchIndex;
                                    TrailResult result = new TrailResult(m_activity, m_resultsList.Count + 1, startIndex, endIndex);
                                    m_resultsList.Add(result);
                                    result = null;
                                    trailIndex = 0;
                                }
                                else
                                {
                                    // found a mid point
                                    trailIndex++;
                                }
                            }
						}
					}
				}
				return m_resultsList;
			}
		}
	}
}
