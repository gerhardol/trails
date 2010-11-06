/*
Copyright (C) 2009 Brendan Doherty
Copyright (C) 2010 Gerhard Olsson

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

		public IList<TrailResult> Results {
			get {
				if (m_resultsList == null) {
					m_resultsList = new List<TrailResult>();
                    if (m_activity.GPSRoute != null && m_activity.GPSRoute.Count > 1 &&
                        this.m_trail.TrailLocations.Count > 0)
                    {
                        IList<int> aMatch = new List<int>();
                        int lastLeaveRadius = -1;
                        float trailDistDiff = 0;
                        int prevRouteIndex = -1;
                        float prevDistToPoint = 0;
						for (int routeIndex = 0; routeIndex < m_activity.GPSRoute.Count; routeIndex++)
                        {
                            int matchIndex = -1;
                            float matchDist = distanceTrailToRoute(aMatch.Count, routeIndex);
                            if (matchDist < this.Trail.Radius)
                            {
                                matchIndex = routeIndex;
                                float prevDistToPoint2 = matchDist;
                                //There is a match, but following points may be better
                                for (int routeIndex2 = routeIndex + 1; routeIndex2 < m_activity.GPSRoute.Count; routeIndex2++)
                                {
                                    float distToPoint2 = distanceTrailToRoute(aMatch.Count, routeIndex2);
                                    float distHysteresis = Math.Max(this.Trail.Radius/30,5);
                                    if (distToPoint2 >= this.Trail.Radius)
                                    {
                                        //No longer in radius, we have the best match
                                        break;
                                    }
                                    if (aMatch.Count > 0)
                                    {
                                        if (isEndTrailPoint(aMatch.Count+1))
                                        {
                                            if (distToPoint2 < matchDist + distHysteresis)
                                            {
                                                //New best match, some hysteris to follow to the end
                                                matchIndex = routeIndex2;
                                                matchDist = distToPoint2;
                                                //xxxif (distToPoint2 < matchDist)
                                                {
                                                    //special handling - can give overlap but less risk to loose potential trails
                                                    //continue with next trail point search from this point
                                                    lastLeaveRadius = matchIndex;
                                                }
                                            }
                                            if (distToPoint2 > matchDist + distHysteresis)
                                            {
                                                //Leaving middle for last point - abort
                                                break;
                                            }
                                        }
                                        //For points after the first: Find closest while in radius
                                        else if (distToPoint2 < matchDist)
                                        {
                                            //New best match
                                            matchIndex = routeIndex2;
                                            matchDist = distToPoint2;
                                            //continue with next trail point search from this point
                                            lastLeaveRadius = matchIndex;
                                        }
                                    }
                                    else
                                    {
                                        //For start point
                                        if (distToPoint2 + distHysteresis < matchDist
                                            || distToPoint2 < matchDist && distToPoint2 < prevDistToPoint2)
                                        {
                                            //Closing in - this is a potential point "closest before start leaving"
                                            matchIndex = routeIndex2;
                                            matchDist = distToPoint2;
                                        }
                                        //continue with next trail point search with next route point
                                        lastLeaveRadius = routeIndex2;
                                    }
                                    prevDistToPoint2 = distToPoint2;
                                }
                            }
                            else
                            {
                                float factor = checkPass(routePoint(prevRouteIndex), prevDistToPoint, routePoint(routeIndex), matchDist, this.m_trail.TrailLocations[TrailIndex(aMatch.Count)], this.Trail.Radius);
                                if (0 < factor)
                                {
                                    //An estimated point (including time) could be inserted in the track, use closest now
                                    if (prevDistToPoint < matchDist)
                                    {
                                        matchIndex = prevRouteIndex;
                                        matchDist = prevDistToPoint;
                                    }
                                    lastLeaveRadius = routeIndex;
                                }
                            }

                            if (matchIndex < 0 && routeIndex > lastLeaveRadius &&
                                aMatch.Count > 0 && matchDist > 3 * this.m_trail.Radius)
                            {
                                //Start over if we pass first point before all were found
                                //Note: No pass by or similar. The algorithm to "restart" could be enhanced...
                                float distFromStartToPoint = distanceTrailToRoute(0, routeIndex);
                                if (distFromStartToPoint < this.m_trail.Radius)
                                {
                                    aMatch.Clear();
                                    matchIndex = routeIndex;
                                    trailDistDiff = 0;
                                }
                            }
                            prevRouteIndex = routeIndex;
                            prevDistToPoint = matchDist;

                            if (matchIndex >= 0 &&
                                //Allow match with same index only if point is new (.e. not single point trails)
                                (aMatch.Count==0 || aMatch[aMatch.Count-1] < matchIndex))
                            {
                                aMatch.Add(matchIndex);
                                trailDistDiff += matchDist;
                                //For single point trail, we need to start after current radius
                                if (1 == this.m_trail.TrailLocations.Count && lastLeaveRadius >= 0)
                                {
                                    routeIndex = lastLeaveRadius;
                                }

                                if (isEndTrailPoint(aMatch.Count))
                                {
                                    TrailResult result = new TrailResult(m_activity, m_resultsList.Count + 1, aMatch, trailDistDiff);
                                    m_resultsList.Add(result);
                                    aMatch.Clear();
                                    trailDistDiff = 0;
                                }
                            }
                        }
					}
				}
				return m_resultsList;
			}
		}

        private static float checkPass(IGPSPoint r1, float d1, IGPSPoint r2, float d2, TrailGPSLocation trailp, float radius)
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
                double d = Math.Abs(d1 * Math.Cos(a1));
                //Point is in circle if closest point is between r1&r2 and it is in circle (neither r1 nor r2 is)
                if (d < d3 && d1 * Math.Sin(a1) < radius)
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
        private IGPSPoint routePoint(int index)
        {
            if (index < 0 || index >= m_activity.GPSRoute.Count)
            {
                return null;
            }
            return m_activity.GPSRoute[index].Value;
        }
        private int TrailIndex(int trailIndex)
        {
            //Single point trails (and wraparound if implemented) have special handling
            if (this.m_trail.TrailLocations.Count <= trailIndex)
            {
                trailIndex = 0;
            }
            return trailIndex;
        }
        private float distanceTrailToRoute(int trailIndex, int routeIndex)
        {
            return this.m_trail.TrailLocations[TrailIndex(trailIndex)].DistanceMetersToPoint(routePoint(routeIndex));
        }
        private bool isEndTrailPoint(int noOfTrailPoints)
        {
            //Special handling for single point trails (and wraparound if implemented)
            return (noOfTrailPoints >= 2 && 
                (1 == this.m_trail.TrailLocations.Count ||
                noOfTrailPoints == this.m_trail.TrailLocations.Count));
        }
    }
}
