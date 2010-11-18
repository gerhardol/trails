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
        private IList<IActivity> m_activities;
		private Data.Trail m_trail;
		private IList<Data.TrailResult> m_resultsList;

		public ActivityTrail(IList<IActivity> activities, Data.Trail trail) {
			m_activities = activities;
			m_trail = trail;
		}
		public Data.Trail Trail {
			get {
				return m_trail;
			}
		}

        public bool IsInBounds
        {
            get
            {
                //Any activity in bounds?
                return m_trail.IsInBounds(m_activities);
            }
        }

		public IList<TrailResult> Results {
			get {
                if (m_resultsList == null)
                {
                    m_resultsList = new List<TrailResult>();
                    foreach (IActivity activity in m_activities)
                    {
                        if (activity.GPSRoute != null && activity.GPSRoute.Count > 1)
                        {
                            //Show all activities on Route
                            if (m_trail.Name.Contains("MatchAll"))
                            {
                                //Match all
                                IList<int> allMatch = new List<int>();
                                if (null == activity.Laps || 0 == activity.Laps.Count)
                                {
                                    allMatch.Add(0);
                                }
                                else
                                {
                                    int i = 0;
                                    foreach (ILapInfo l in activity.Laps)
                                    {
                                        for (; i < activity.GPSRoute.Count; i++)
                                        {
                                            if (0 > l.StartTime.CompareTo(activity.GPSRoute.EntryDateTime(activity.GPSRoute[i])))
                                            {
                                                allMatch.Add(i);
                                                break;
                                            }
                                        }
                                    }
                                }
                                allMatch.Add(activity.GPSRoute.Count - 1);
                                TrailResult result = new TrailResult(activity, m_resultsList.Count + 1, allMatch, float.MaxValue);
                                m_resultsList.Add(result);
                            }

                            if (this.m_trail.TrailLocations.Count > 0)
                            {
                                IList<int> aMatch = new List<int>();
                                int lastMatchInRadius = -1;
                                float trailDistDiff = 0;
                                int prevRouteIndex = -1;
                                float prevDistToPoint = 0;
                                for (int routeIndex = 0; routeIndex < activity.GPSRoute.Count; routeIndex++)
                                {
                                    int matchIndex = -1;
                                    float routeDist = distanceTrailToRoute(activity, aMatch.Count, routeIndex);
                                    float matchDist = float.MaxValue;
                                    if (routeDist < this.Trail.Radius)
                                    {
                                        matchIndex = routeIndex;
                                        matchDist = routeDist;
                                        float prevDistToPoint2 = routeDist;
                                        float localMax = float.MaxValue;
                                        //There is a match, but following points may be better
                                        for (int routeIndex2 = routeIndex + 1; routeIndex2 < activity.GPSRoute.Count; routeIndex2++)
                                        {
                                            float distToPoint2 = distanceTrailToRoute(activity, aMatch.Count, routeIndex2);
                                            float distHysteresis = Math.Max(this.Trail.Radius / 30, 5);
                                            if (distToPoint2 >= this.Trail.Radius)
                                            {
                                                //No longer in radius, we have the best match
                                                break;
                                            }
                                            if (aMatch.Count == 0)
                                            {
                                                //start point
                                                if (distToPoint2 + distHysteresis < matchDist
                                                    || distToPoint2 < matchDist && distToPoint2 < prevDistToPoint2
                                                    || distToPoint2 + distHysteresis < localMax)
                                                {
                                                    //Closing in - this is a potential point "closest before start leaving"
                                                    matchIndex = routeIndex2;
                                                    matchDist = distToPoint2;
                                                    localMax = distToPoint2;
                                                }
                                            }
                                            else
                                            {
                                                if (isEndTrailPoint(aMatch.Count + 1))
                                                {
                                                    if (prevDistToPoint2 < matchDist &&
                                                        prevDistToPoint2 < distToPoint2)
                                                    {
                                                        //Leaving middle and last was best
                                                        matchIndex = routeIndex2 - 1;
                                                        matchDist = prevDistToPoint2;
                                                    }
                                                    else if (distToPoint2 + distHysteresis < matchDist)
                                                    {
                                                        //Better, with some hysteresis
                                                        matchIndex = routeIndex2;
                                                        matchDist = distToPoint2;
                                                    }
                                                    if (distToPoint2 > matchDist + distHysteresis)
                                                    {
                                                        //Leaving middle for last point - no more checks
                                                        break;
                                                    }
                                                }
                                                //For points after the first: Find closest while in radius
                                                else if (distToPoint2 < matchDist)
                                                {
                                                    //New best match
                                                    matchIndex = routeIndex2;
                                                    matchDist = distToPoint2;
                                                }
                                            }
                                            //still in radius
                                            lastMatchInRadius = routeIndex2;

                                            prevDistToPoint2 = distToPoint2;
                                            localMax = Math.Max(localMax, distToPoint2);
                                        }
                                    }
                                    else
                                    {
                                        float factor = checkPass(routePoint(activity, prevRouteIndex), prevDistToPoint, routePoint(activity, routeIndex), routeDist, this.m_trail.TrailLocations[TrailIndex(aMatch.Count)], this.Trail.Radius);
                                        if (0 < factor)
                                        {
                                            //An estimated point (including time) could be inserted in the track, use closest now
                                            if (prevDistToPoint < routeDist)
                                            {
                                                matchIndex = prevRouteIndex;
                                                matchDist = prevDistToPoint;
                                            }
                                        }
                                    }

                                    if (matchIndex < 0 && aMatch.Count > 0 &&
                                        routeDist > 3 * this.m_trail.Radius)
                                    {
                                        //Start over if we pass first point before all were found
                                        //Note: No pass by or similar. The algorithm to "restart" could be enhanced...
                                        float distFromStartToPoint = distanceTrailToRoute(activity, 0, routeIndex);
                                        if (distFromStartToPoint < this.m_trail.Radius)
                                        {
                                            aMatch.Clear();
                                            matchIndex = routeIndex;
                                            trailDistDiff = 0;
                                        }
                                    }

                                    if (matchIndex >= 0 &&
                                        //Allow match with same index only for first point
                                     (aMatch.Count == 0 || aMatch[aMatch.Count - 1] < matchIndex))
                                    {
                                        aMatch.Add(matchIndex);
                                        trailDistDiff += matchDist;

                                        if (isEndTrailPoint(aMatch.Count))
                                        {
                                            TrailResult result = new TrailResult(activity, m_resultsList.Count + 1, aMatch, trailDistDiff);
                                            m_resultsList.Add(result);

                                            aMatch.Clear();
                                            trailDistDiff = 0;
                                            //Try this point again as start
                                            routeIndex = matchIndex - 1;
                                            routeDist = prevRouteIndex;
                                        }
                                        else
                                        {
                                            //Start search for next point after this match
                                            routeIndex = matchIndex;

                                            //For single point trail, we need to start after current radius to not get immediate match
                                            //For two point trails the result is unexpected if the first/last points overlap
                                            //For trails with more points, assume that all points do not overlap
                                            if (2 >= this.m_trail.TrailLocations.Count &&
                                                lastMatchInRadius >= routeIndex)
                                            {
                                                routeIndex = lastMatchInRadius;
                                                routeDist = matchDist;
                                            }
                                        }
                                    }
                                    prevRouteIndex = routeIndex;
                                    prevDistToPoint = routeDist;
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
        private IGPSPoint routePoint(IActivity activity, int index)
        {
            if (index < 0 || index >= activity.GPSRoute.Count)
            {
                return null;
            }
            return activity.GPSRoute[index].Value;
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
        private float distanceTrailToRoute(IActivity activity, int trailIndex, int routeIndex)
        {
            return this.m_trail.TrailLocations[TrailIndex(trailIndex)].DistanceMetersToPoint(routePoint(activity, routeIndex));
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
