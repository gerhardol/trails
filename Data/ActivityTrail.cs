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
using ZoneFiveSoftware.Common.Visuals;

namespace TrailsPlugin.Data
{
	public class ActivityTrail : IComparable
    {
        private Controller.TrailController m_controller;
		private Data.Trail m_trail;
        private IList<Data.TrailResultWrapper> m_resultsListWrapper = null;
        private IList<Data.TrailResult> m_resultsList = null;
        private TrailOrderStatus m_status;
        private IActivity m_resultActivity = null;
        //Counter for "no results"
        public IDictionary<TrailOrderStatus, int> m_noResCount = new Dictionary<TrailOrderStatus, int>();
        private IList<IActivity> m_inBound = new List<IActivity>();
        private bool m_canAddInbound = true;

        public ActivityTrail(Controller.TrailController controller, Data.Trail trail)
        {
            m_controller = controller; ;
            m_trail = trail;
            m_status = TrailOrderStatus.NoInfo;
            if (m_trail.Generated && !m_trail.IsReference)
            {
                m_canAddInbound = false;
            }
            if (m_trail.HighScore > 0)
            {
                if(Integration.HighScore.HighScoreIntegrationEnabled)
                {
                    m_status = TrailOrderStatus.MatchNoCalc;
                }
            }
            else if (Trail.MatchAll || Trail.IsReference)
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
        public int ActivityCount
        {
            get
            {
                return m_controller.Activities.Count;
            }
        }

        public TrailOrderStatus Status
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
                if (Status == TrailOrderStatus.NoInfo)
                {
                    if (m_trail.HighScore > 0)
                    {
                        if (Integration.HighScore.HighScoreIntegrationEnabled)
                        {
                            this.Status = TrailOrderStatus.MatchNoCalc;
                        }
                    }
                    else if (m_trail.IsInBounds(m_controller.Activities))
                    {
                        Status = TrailOrderStatus.InBoundNoCalc;
                    }
                    else
                    {
                        Status = TrailOrderStatus.NotInBound;
                    }
                }
                //Any activity in bounds?
                return Status <= TrailOrderStatus.InBound;
            }
        }
        public bool IsNoCalc
        {
            get
            {
                return (Status == TrailOrderStatus.MatchNoCalc || Status == TrailOrderStatus.InBoundNoCalc);
            }
        }
        public IList<TrailResult> Results
        {
            get
            {
                if (m_resultsList == null)
                {
                    //CalcResults();
                    m_resultsList = new List<TrailResult>();
                    foreach (TrailResultWrapper tr in ResultTreeList)
                    {
                        m_resultsList.Add(tr.Result);
                    }
                }
                return m_resultsList;
            }
        }

        public void Clear()
        {
            foreach (TrailResultWrapper t in ResultTreeList)
            {
                t.Result.Clear();
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
            ((List<TrailResultWrapper>)ResultTreeList).Sort();
            foreach (TrailResultWrapper tr in ResultTreeList)
            {
                tr.Sort();
            }
        }

        public bool CanAddInbound
        {
            get
            {
                return m_canAddInbound;
            }
        }
        public void AddInBoundResult()
        {
            CalcResults();
            m_canAddInbound = false;
            foreach (IActivity activity in m_inBound)
            {
                if (activity.GPSRoute != null && activity.GPSRoute.Count > 1)
                {
                    TrailResultWrapper result = new TrailResultWrapper(m_trail, activity, m_resultsListWrapper.Count + 1);
                    m_resultsListWrapper.Add(result);
                    //add children
                    result.getSplits();
                }
            }
        }

        public void CalcResults()
        {
            if (m_resultsListWrapper == null || m_trail.TrailChanged(m_resultActivity))
            {
                m_resultsListWrapper = new List<TrailResultWrapper>();
                m_resultActivity = m_trail.ReferenceActivity;

                IList<TrailGPSLocation> trailgps = m_trail.TrailLocations;
                if (m_trail.HighScore > 0)
                {
                    if (Integration.HighScore.HighScoreIntegrationEnabled)
                    {
                        IList<Integration.HighScore.HighScoreResult> res = Integration.HighScore.GetHighScoreForActivity(m_controller.Activities, null);
                        if (res != null && res.Count > 0)
                        {
                            foreach (Integration.HighScore.HighScoreResult h in Integration.HighScore.GetHighScoreForActivity(m_controller.Activities, null))
                            {
                                if (h.activity.GPSRoute != null && h.activity.GPSRoute.Count > 0)
                                {
                                    this.Status = TrailOrderStatus.Match;
                                    TrailResultWrapper result = new TrailResultWrapper(m_trail, h.activity, h.selInfo, h.tooltip, m_resultsListWrapper.Count + 1);
                                    m_resultsListWrapper.Add(result);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (IActivity activity in m_controller.Activities)
                    {
                        if (activity.GPSRoute != null && activity.GPSRoute.Count > 1)
                        {
                            if (m_trail.MatchAll)
                            {
                                this.Status = TrailOrderStatus.Match;
                                TrailResultWrapper result = new TrailResultWrapper(m_trail, activity, m_resultsListWrapper.Count + 1);
                                m_resultsListWrapper.Add(result);
                            }
                            else if (trailgps.Count > 0)
                            {
                                if (m_trail.IsInBounds(new List<IActivity>{activity}))
                                {
                                    m_inBound.Add(activity);
                                    IList<int> aMatch = new List<int>();
                                    int lastMatchInRadius = -1;
                                    int lastMatchPassBy = -1;
                                    float trailDistDiff = 0;
                                    int prevRouteIndex = -1;
                                    int prevMatchIndex = -1; //Last index that match for this activity
                                    float prevDistToPoint = 0;
                                    float prevDistToStartPoint = 0;
                                    int maxRequiredMisses = m_trail.MaxRequiredMisses;
                                    int currRequiredMisses = 0;
                                    IDistanceDataTrack dTrack = null;
                                    //Ignore short legs
                                    if (this.Trail.MinDistance > 0)
                                    {
                                        dTrack = activity.GPSRoute.GetDistanceMetersTrack();
                                    }

                                    for (int routeIndex = 0; routeIndex < activity.GPSRoute.Count; routeIndex++)
                                    {
                                        int matchIndex = -1;
                                        float routeDist = distanceTrailToRoute(activity, trailgps, aMatch.Count, routeIndex);
                                        float matchDist = float.MaxValue;

                                        //////////////////////////////////////
                                        //Shorten the trail if possible

                                        //TODO: Find a way to get shorter trails
                                        //The algorithm here will reduce A'1-B'1-A'2-B'2-C to A'2-B'2-C
                                        //but will fail to match A'1-B-A'2-C
                                        //One way could be to try again if start is match
                                        //For something like: A'1-B'1-A'2-C'1-B'2-A'3-C'2
                                        //Is A'1-B'1-A'2-C'1 or A'2-C'1-B'2-A'3-C'2 or start with A'3?
                                        //Add overlapping results?
                                        //if (matchIndex < 0 && aMatch.Count > 0 &&
                                        //    routeDist > 3 * this.m_trail.Radius)
                                        //{
                                        //    //Start over if we pass first point before all were found
                                        //    float distFromStartToPoint = distanceTrailToRoute(activity, trailgps, 0, routeIndex);
                                        //    if (distFromStartToPoint < this.m_trail.Radius)
                                        //    {
                                        //        aMatch.Clear();
                                        //        matchIndex = routeIndex;
                                        //        trailDistDiff = 0;
                                        //    }
                                        //}

                                        //Special case of the algorithm above, restarting if the first point is seen again.
                                        //So A1-A2-B1-C1 is reduced to A2-B1-C1
                                        if (trailgps.Count > 1 && aMatch.Count == 1 &&
                                            lastMatchInRadius > -1 && lastMatchPassBy > -1 &&
                                            routeIndex > lastMatchInRadius && routeIndex > lastMatchPassBy)
                                        {
                                            float routeDistStartPoint = distanceTrailToRoute(activity, trailgps, aMatch.Count, routeIndex);
                                            if (distanceTrailToRoute(activity, trailgps, 0, routeIndex) < this.m_trail.Radius ||
                                            0 < checkPass(routePoint(activity, prevRouteIndex), prevDistToStartPoint, routePoint(activity, routeIndex), routeDistStartPoint, trailgps[TrailIndex(trailgps, aMatch.Count)], this.Trail.Radius))
                                            {
                                                //Start over if we pass first point before all were found
                                                aMatch.Clear();
                                                trailDistDiff = 0;
                                            }
                                            prevDistToStartPoint = routeDistStartPoint;
                                        }
                                        else
                                        {
                                            prevDistToStartPoint = routeDist;
                                        }

                                        //////////////////////////////////////
                                        //Find the best GPS point for this result
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
                                            //Check for pass-by - ignore if prevous was match for single point trails
                                        else if (trailgps.Count > 1 || aMatch.Count>0 && prevRouteIndex+1<routeIndex)
                                        {
                                            float factor = checkPass(routePoint(activity, prevRouteIndex), prevDistToPoint, routePoint(activity, routeIndex), routeDist, trailgps[TrailIndex(trailgps, aMatch.Count)], this.Trail.Radius);
                                            if (0 < factor)
                                            {
                                                lastMatchPassBy = routeIndex;
                                                //An estimated point (including time) could be inserted in the track, use closest now
                                                if (prevDistToPoint < routeDist)
                                                {
                                                    matchIndex = prevRouteIndex;
                                                    matchDist = prevDistToPoint;
                                                }
                                            }
                                        }

                                        //////////////////////////////
                                        //Last point check for non required points - automatic match
                                        bool required = trailgps[TrailIndex(trailgps, aMatch.Count)].Required;
                                        bool automaticMatch = false;
                                        if (matchIndex < 0 && routeIndex >= activity.GPSRoute.Count - 1 &&
                                            (!required || currRequiredMisses < maxRequiredMisses))
                                        {
                                            automaticMatch = true;
                                            if (required)
                                            {
                                                currRequiredMisses++;
                                            }
                                        }

                                        //Ignore short legs
                                        if (this.Trail.MinDistance > 0 && matchIndex >= 0 && aMatch.Count > 0 &&
                                            aMatch[aMatch.Count - 1] >= 0 && dTrack != null &&
                                            this.Trail.MinDistance > (dTrack[matchIndex].Value - dTrack[aMatch[aMatch.Count - 1]].Value))
                                        {
                                            matchIndex = -1;
                                        }

                                        /////////////////////////////////////
                                        //Add found match to result 
                                        if ((matchIndex >= 0 &&
                                            //Allow match with same index only for first point
                                         (aMatch.Count == 0 || aMatch[aMatch.Count - 1] < matchIndex)) ||
                                            automaticMatch)
                                        {
                                            aMatch.Add(matchIndex);
                                            trailDistDiff += matchDist;

                                            //Check if this is a partial match
                                            bool isPartial = false;
                                            int noMatches = 0;
                                            foreach (int i in aMatch)
                                            {
                                                if (i >= 0)
                                                {
                                                    noMatches++;
                                                }
                                                else
                                                {
                                                    isPartial = true;
                                                }
                                            }
                                            if (isEndTrailPoint(trailgps, aMatch.Count))
                                            {
                                                if (noMatches >= 2)
                                                {
                                                    if (isPartial)
                                                    {
                                                        Status = TrailOrderStatus.MatchPartial;
                                                    }
                                                    else
                                                    {
                                                        Status = TrailOrderStatus.Match;

                                                    }
                                                    TrailResultWrapper result = new TrailResultWrapper(m_trail, activity, m_resultsListWrapper.Count + 1, aMatch, trailDistDiff);
                                                    m_resultsListWrapper.Add(result);
                                                }
                                                currRequiredMisses = 0;
                                                aMatch.Clear();
                                                trailDistDiff = 0;
                                                //Try this point again as start
                                                routeIndex = matchIndex - 1;
                                                routeDist = prevRouteIndex;
                                            }
                                            else
                                            {
                                                //Start search for next point after this match
                                                if (!automaticMatch)
                                                {
                                                    routeIndex = matchIndex;
                                                    //Not a result, but at least one point match
                                                    Status = TrailOrderStatus.InBoundMatchPartial;
                                                }

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
                                            if (automaticMatch)
                                            {
                                                //If this was an automatic match, start at prev match
                                                //Make sure to not restart if this was the end and  there were not enough matches
                                                if (currRequiredMisses <= maxRequiredMisses &&
                                                    (aMatch.Count > 0 || noMatches >= 2))
                                                {
                                                    routeIndex = prevMatchIndex;
                                                }
                                            }
                                            else
                                            {
                                                prevMatchIndex = routeIndex;
                                            }
                                            prevRouteIndex = -1;
                                            prevDistToPoint = float.MaxValue;
                                        }
                                        else
                                        {
                                            prevRouteIndex = routeIndex;
                                            prevDistToPoint = routeDist;
                                        }
                                    }
                                    //Update "no result counters"
                                    if (aMatch.Count > 0)
                                    {
                                        TrailOrderStatus t = TrailOrderStatus.InBoundMatchPartial;
                                        if (!m_noResCount.ContainsKey(t))
                                        {
                                            m_noResCount[t] = 0;
                                        }
                                        m_noResCount[t]++;
                                    }
                                    else if (m_status >= TrailOrderStatus.InBoundNoCalc)
                                    {
                                        TrailOrderStatus t = TrailOrderStatus.InBound;
                                        if (!m_noResCount.ContainsKey(t))
                                        {
                                            m_noResCount[t] = 0;
                                        }
                                        m_noResCount[t]++;
                                    }
                                }
                                //NotInBound is pruned prior to this
                                //else
                                //{
                                //    TrailOrderStatus t = TrailOrderStatus.NotInBound;
                                //    if (!noResCount.ContainsKey(t))
                                //    {
                                //        noResCount[t] = 0;
                                //    }
                                //    noResCount[t]++;
                                //}
                            }
                        }
                    }
                        //children
                    foreach (TrailResultWrapper tr in this.m_resultsListWrapper)
                    {
                        tr.getSplits();
                    }
                }
                if (m_resultsListWrapper.Count == 0 && m_status < TrailOrderStatus.InBound &&
                    (m_status == TrailOrderStatus.InBoundNoCalc || m_status == TrailOrderStatus.MatchNoCalc))
                {
                    //Downgrade status from "speculative match"
                    m_status = TrailOrderStatus.InBound;
                }
            }
        }

        public void Remove(IList<TrailResultWrapper> atr, bool invertSelection)
        {
            if (this.m_resultsListWrapper != null)
            {
                IList<TrailResultWrapper> selected = atr;
                if (invertSelection)
                {
                    selected = new List<TrailResultWrapper>();
                    foreach (TrailResultWrapper tr in this.m_resultsListWrapper)
                    {
                        if (!atr.Contains(tr))
                        {
                            selected.Add(tr);
                        }
                    }
                    //Exclude parents to selected
                    foreach (TrailResultWrapper tr in atr)
                    {
                        if (tr.Parent != null)
                        {
                            TrailResultWrapper tr2 = tr.Parent as TrailResultWrapper;
                            if (selected.Contains(tr2))
                            {
                                selected.Remove(tr2);
                            }
                        }
                    }

                }
                if (invertSelection && selected.Count == m_resultsListWrapper.Count)
                {
                    //Must keep at least one in "inverted"
                    return;
                }
                //Needed?
                //foreach (TrailResult trr in m_resultsList)
                //{
                //    trr.RemoveChildren(atr[0].Result);
                //}
                //This will not have any effect with invertSelection, as 'selected' contains
                //complement to parents only
                foreach (TrailResultWrapper trr in m_resultsListWrapper)
                {
                    trr.RemoveChildren(selected, invertSelection);
                }
                foreach (TrailResultWrapper tr in selected)
                {
                    if (Results.Contains(tr.Result))
                    {
                        Results.Remove(tr.Result);
                    }
                    if (ResultTreeList.Contains(tr))
                    {
                        ResultTreeList.Remove(tr);
                    }
                }
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
                double a0 = (d1 * d1 + d3 * d3 - d2 * d2) / (2 * d1 * d3);
                //Rounding errors w GPS measurements
                a0 = Math.Min(a0, 1);
                a0 = Math.Max(a0, -1);
                double a1 = Math.Acos(a0);
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

        //Some syntethic value to sort
        private float? sortValue;
        private float SortValue
        {
            get
            {
                if (sortValue == null)
                {
                    sortValue = 0;
                    foreach (Data.TrailResult tr in this.Results)
                    {
                        sortValue += tr.DistDiff;
                    }
                    sortValue = sortValue / (float)Math.Pow(this.Results.Count, 1.5);

                }
                return (float)sortValue;
            }
        }

        #region Implementation of IComparable
        public int CompareTo(object obj)
        {
            if (!(obj is ActivityTrail))
            {
                return 1;
            }
            ActivityTrail to2 = obj as ActivityTrail;
            if (Status != to2.Status)
            {
                return Status > to2.Status ? 1 : -1;
            }
            else if (Status == TrailOrderStatus.Match)
            {
                if (this.Trail.MatchAll != to2.Trail.MatchAll)
                {
                    return (this.Trail.MatchAll) ? 1 : -1;
                }
                else if (this.Trail.Generated != to2.Trail.Generated)
                {
                    return (this.Trail.Generated) ? 1 : -1; ;
                }
                //else if (activityTrail.Results.Count != to2.activityTrail.Results.Count)
                //{
                //    return (activityTrail.Results.Count < to2.activityTrail.Results.Count) ? 1 : -1;
                //}
                else
                {
                    return this.SortValue > to2.SortValue ? 1 : -1;
                }
            }
            else if (Status == TrailOrderStatus.MatchNoCalc)
            {
                //Sort generated trails as Reference, Splits, HighScore
                //If this is changed, consider changing checkCurrentTrailOrdered() so not the trail always follows the generated trail
                //(Splits could be before Ref but his will increase resonse time with many activities)
                if (this.Trail.IsReference)
                {
                    return -1;
                }
                else if (to2.Trail.IsReference)
                {
                    return 1;
                }
                else if (this.Trail.HighScore > 0)
                {
                    return 1;
                }
                else if (to2.Trail.HighScore > 0)
                {
                    return -1;
                }
            }

            //Sort remaining by name
            return this.Trail.Name.CompareTo(to2.Trail.Name);
        }
        #endregion
    }

    /*******************************************************/

    public class TrailDropdownLabelProvider : TreeList.ILabelProvider
    {

        public System.Drawing.Image GetImage(object element, TreeList.Column column)
        {
            ActivityTrail t = (ActivityTrail)element;
            switch (t.Status)
            {
                case TrailOrderStatus.Match:
                    return Properties.Resources.square_green;
                case TrailOrderStatus.MatchNoCalc:
                    return Properties.Resources.square_green_check;
                case TrailOrderStatus.MatchPartial:
                    return Properties.Resources.square_green_minus;
                case TrailOrderStatus.InBoundNoCalc:
                    return Properties.Resources.square_green_plus;
                case TrailOrderStatus.InBoundMatchPartial:
                    return Properties.Resources.square_red_plus;
                case TrailOrderStatus.InBound:
                    return Properties.Resources.square_red;
                case TrailOrderStatus.NotInBound:
                    return Properties.Resources.square_blue;
                default:
                    return null;
            }
        }
        public string GetText(object element, TreeList.Column column)
        {
            ActivityTrail t = (ActivityTrail)element;
            string name = t.Trail.Name;
            if (t.Status == TrailOrderStatus.Match ||
                t.Status == TrailOrderStatus.MatchPartial)
            {
                name += " (" + t.Results.Count + ")";
            }
            else if (t.Status == TrailOrderStatus.MatchNoCalc)
            {
                if (t.Trail.MatchAll)
                {
                    name += " (" + t.ActivityCount + ")";
                }
            }
            else if ((t.Status == TrailOrderStatus.InBoundMatchPartial) &&
                t.m_noResCount.ContainsKey(t.Status))
            {
                name += " (" + t.m_noResCount[t.Status];
                if (t.m_noResCount.ContainsKey(TrailOrderStatus.InBound))
                {
                    name += ", " + t.m_noResCount[TrailOrderStatus.InBound];
                }
                name += ")";
            }
            else if ((t.Status == TrailOrderStatus.InBound ||
                t.Status == TrailOrderStatus.NotInBound) &&
                t.m_noResCount.ContainsKey(t.Status))
            {
                name += " (" + t.m_noResCount[t.Status] + ")";
            }
            //Note: No result for InBoundNoCalc
            return name;
        }
    }

    public enum TrailOrderStatus
    {
        //<= InBound is inbound
        //InBoundNoCalc is better than InBound, as it may be Match
        Match, MatchPartial, MatchNoCalc, InBoundMatchPartial, InBoundNoCalc, InBound, NotInBound, NoInfo
    }
}
