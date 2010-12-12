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
	public class ActivityTrail : IComparable
    {
        private IList<IActivity> m_activities;
		private Data.Trail m_trail;
        private IList<Data.TrailResultWrapper> m_resultsListWrapper;
        private IList<Data.TrailResult> m_resultsList = null;
        private TrailOrderStatus m_status;
        private IActivity m_resultActivity = null;

        public ActivityTrail(IList<IActivity> activities, Data.Trail trail)
        {
            m_activities = activities;
            m_trail = trail;
            m_status = TrailOrderStatus.NoInfo;
            if (Trail.MatchAll || Trail.IsReference)
            {
                // Let Reference always match, to trigger possible recalc after
                m_status = TrailOrderStatus.MatchNoCalc;
            }
            else if (!Trail.IsReference && Trail.TrailLocations.Count == 0)
            {
                m_status = TrailOrderStatus.NotInBound;
            }
        }

		public Data.Trail Trail {
			get {
				return m_trail;
			}
		}

        public TrailOrderStatus status
        {
            get
            {
                return m_status;
            }
            set
            {
                //Set "better" status only
                if (value < m_status)
                {
                    m_status = value;
                }
            }
        }
        public bool IsInBounds
        {
            get
            {
                if (status == TrailOrderStatus.NoInfo)
                {
                    if (m_trail.IsInBounds(m_activities))
                    {
                        status = TrailOrderStatus.InBoundNoCalc;
                    }
                    else
                    {
                        status = TrailOrderStatus.NotInBound;
                    }
                }
                //Any activity in bounds?
                return status <= TrailOrderStatus.InBound;
            }
        }
        public bool IsNoCalc
        {
            get
            {
                return (status == TrailOrderStatus.MatchNoCalc || status == TrailOrderStatus.InBoundNoCalc);
            }
        }
        public IList<TrailResult> Results
        {
            get
            {
                if (m_resultsList == null)
                {
                    CalcResults();
                    m_resultsList = new List<TrailResult>();
                    foreach (TrailResultWrapper tr in m_resultsListWrapper)
                    {
                        m_resultsList.Add(tr.Result);
                    }
                }
                return m_resultsList;
            }
        }
        public IList<TrailResultWrapper> ResultTreeList
        {
            get
            {
                CalcResults();
                return m_resultsListWrapper;
            }
        }
        public void Sort()
        {
            ((List<TrailResultWrapper>)m_resultsListWrapper).Sort();
            foreach (TrailResultWrapper tr in m_resultsListWrapper)
            {
                tr.Sort();
            }
        }
        public void CalcResults()
        {
            if (m_resultsListWrapper == null || m_trail.TrailChanged(m_resultActivity))
            {
                m_resultActivity = m_trail.ReferenceActivity;

                m_resultsListWrapper = new List<TrailResultWrapper>();
                IList<TrailGPSLocation> trailgps = m_trail.TrailLocations;
                foreach (IActivity activity in m_activities)
                {
                    if (activity.GPSRoute != null && activity.GPSRoute.Count > 1)
                    {
                        if (m_trail.MatchAll)
                        {
                            this.status = TrailOrderStatus.Match;
                            TrailResultWrapper result = new TrailResultWrapper(m_trail, activity, m_resultsListWrapper.Count + 1);
                            m_resultsListWrapper.Add(result);
                        }

                        if (IsInBounds && trailgps.Count > 0)
                        {
                            IList<int> aMatch = new List<int>();
                            int lastMatchInRadius = -1;
                            float trailDistDiff = 0;
                            int prevRouteIndex = -1;
                            float prevDistToPoint = 0;
                            for (int routeIndex = 0; routeIndex < activity.GPSRoute.Count; routeIndex++)
                            {
                                int matchIndex = -1;
                                float routeDist = distanceTrailToRoute(activity, trailgps, aMatch.Count, routeIndex);
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
                                        float distToPoint2 = distanceTrailToRoute(activity, trailgps, aMatch.Count, routeIndex2);
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
                                            if (isEndTrailPoint(trailgps, aMatch.Count + 1))
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
                                    float factor = checkPass(routePoint(activity, prevRouteIndex), prevDistToPoint, routePoint(activity, routeIndex), routeDist, trailgps[TrailIndex(trailgps, aMatch.Count)], this.Trail.Radius);
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
                                    float distFromStartToPoint = distanceTrailToRoute(activity, trailgps, 0, routeIndex);
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

                                    if (isEndTrailPoint(trailgps, aMatch.Count))
                                    {
                                        status = TrailOrderStatus.Match;
                                        TrailResultWrapper result = new TrailResultWrapper(m_trail, activity, m_resultsListWrapper.Count + 1, aMatch, trailDistDiff);
                                        m_resultsListWrapper.Add(result);

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
                                        if (2 >= trailgps.Count &&
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
            if (m_resultsListWrapper.Count == 0 && m_status != TrailOrderStatus.Match)
            {
                //Downgrade status
                m_status = TrailOrderStatus.InBound;
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
        private static IGPSPoint routePoint(IActivity activity, int index)
        {
            if (index < 0 || index >= activity.GPSRoute.Count)
            {
                return null;
            }
            return activity.GPSRoute[index].Value;
        }
        private static int TrailIndex(IList<TrailGPSLocation> trailgps, int trailIndex)
        {
            //Single point trails (and wraparound if implemented) have special handling
            if (trailgps.Count <= trailIndex)
            {
                trailIndex = 0;
            }
            return trailIndex;
        }
        private static float distanceTrailToRoute(IActivity activity, IList<TrailGPSLocation> trailgps, int trailIndex, int routeIndex)
        {
            return trailgps[TrailIndex(trailgps, trailIndex)].DistanceMetersToPoint(routePoint(activity, routeIndex));
        }
        private static bool isEndTrailPoint(IList<TrailGPSLocation> trailgps, int noOfTrailPoints)
        {
            //Special handling for single point trails (and wraparound if implemented)
            return (noOfTrailPoints >= 2 &&
                (1 == trailgps.Count ||
                noOfTrailPoints == trailgps.Count));
        }

        #region Implementation of IComparable
        public int CompareTo(object obj)
        {
            if (!(obj is ActivityTrail))
            {
                return 1;
            }
            ActivityTrail to2 = obj as ActivityTrail;
            if (status != to2.status)
            {
                return status > to2.status ? 1 : -1;
            }
            else if (status == TrailOrderStatus.Match)
            {
                if (this.Trail.MatchAll != to2.Trail.MatchAll)
                {
                    return (this.Trail.MatchAll) ? 1 : -1;
                }
                //else if (activityTrail.Results.Count != to2.activityTrail.Results.Count)
                //{
                //    return (activityTrail.Results.Count < to2.activityTrail.Results.Count) ? 1 : -1;
                //}
                else
                {
                    float e1 = 0;
                    foreach (Data.TrailResult tr in this.Results)
                    {
                        e1 += tr.DistDiff;
                    }
                    e1 = e1 / (float)Math.Pow(this.Results.Count, 1.5);
                    float e2 = 0;
                    foreach (Data.TrailResult tr in to2.Results)
                    {
                        e2 += tr.DistDiff;
                    }
                    e2 = e2 / (float)Math.Pow(to2.Results.Count, 1.5);
                    //No check if equal here
                    return e1 < e2 ? 1 : -1;
                }
            }
            //Sort remaining by name
            return this.Trail.Name.CompareTo(to2.Trail.Name);
        }
        #endregion
    }
    public enum TrailOrderStatus
    {
        //<= InBound is inbound
        //InBoundNoCalc is better than InBound, as it may be Match
        Match, MatchNoCalc, InBoundNoCalc, InBound, NotInBound, NoInfo
    }
}
