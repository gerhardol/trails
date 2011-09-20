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
        private TrailOrderStatus m_status;
        private IActivity m_resultActivity = null; //Used for ReferenceActivity results only

        private IList<IncompleteTrailResult> m_incompleteResults;
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

            //Preset status
            if (m_trail.HighScore > 0)
            {
                if(Integration.HighScore.HighScoreIntegrationEnabled)
                {
                    m_status = TrailOrderStatus.MatchNoCalc;
                }
            }
            else if (Trail.IsSplits)
            {
                //By default, always match
                m_status = TrailOrderStatus.MatchNoCalc;
            }
            else if (Trail.IsReference)
            {
                if (trail.ReferenceActivity != null && trail.ReferenceActivity.GPSRoute != null)
                {
                    // Let Reference always match, to trigger possible recalc after
                    m_status = TrailOrderStatus.MatchNoCalc;
                }
                else
                {
                    m_status = TrailOrderStatus.NotInBound;
                }
            }
            else if (Trail.TrailLocations.Count == 0)
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
                if (Status == TrailOrderStatus.NoInfo || 
                    m_trail.IsReference && Status == TrailOrderStatus.MatchNoCalc)
                {
                    if (m_trail.IsInBounds(m_controller.Activities) || 
                        m_trail.IsReference && m_trail.ReferenceActivity == null)
                    {
                        //Do not downgrade MatchNoCalc here
                        Status = TrailOrderStatus.InBoundNoCalc;
                    }
                    else
                    {
                        //Downgrade status
                        m_status = TrailOrderStatus.NotInBound;
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

        public void Reset()
        {
            m_resultsListWrapper = null;
        }
        public void Clear()
        {
            if (m_resultsListWrapper != null)
            {
                foreach (TrailResultWrapper t in m_resultsListWrapper)
                {
                    t.Result.Clear(false);
                }
            }
        }

        public IList<TrailResult> Results
        {
            get
            {
                return TrailResultWrapper.GetTrailResults(ResultTreeList);
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
        public void AddInBoundResult(System.Windows.Forms.ProgressBar progressBar)
        {
            CalcResults(progressBar);
            m_canAddInbound = false;
            if (null != progressBar)
            {
                progressBar.Value = 0;
                progressBar.Maximum = m_inBound.Count;
            }
            foreach (IActivity activity in m_inBound)
            {
                //No GPS check: Must have been done when adding to inbound list
                TrailResultWrapper result = new TrailResultWrapper(this, activity, m_resultsListWrapper.Count + 1);
                m_resultsListWrapper.Add(result);
                //add children
                result.getSplits();
                if (null != progressBar)
                {
                    progressBar.Value++;
                }
            }
        }

        public void CalcResults()
        {
            CalcResults(m_controller.Activities, m_trail.MaxRequiredMisses, m_trail.Bidirectional, null);
        }
        public void CalcResults(System.Windows.Forms.ProgressBar progressBar)
        {
            CalcResults(m_controller.Activities, m_trail.MaxRequiredMisses, m_trail.Bidirectional, progressBar);
        }
        public void CalcResults(IList<IActivity> activities, int MaxRequiredMisses, bool bidirectional, System.Windows.Forms.ProgressBar progressBar)
        {
            if (m_resultsListWrapper == null || m_trail.TrailChanged(m_resultActivity))
            {
                m_resultsListWrapper = new List<TrailResultWrapper>();
                m_incompleteResults = new List<IncompleteTrailResult>();

                m_resultActivity = m_trail.ReferenceActivity;

                if (null != progressBar && progressBar.Maximum < progressBar.Value + activities.Count)
                {
                    progressBar.Maximum = activities.Count + progressBar.Value;
                }
                if (m_trail.HighScore > 0)
                {
                    if (Integration.HighScore.HighScoreIntegrationEnabled)
                    {
                        int HighScoreProgressVal = 0;
                        int HighScoreProgressMax = 0;
                        if (null != progressBar)
                        {
                            //Set by HighScore before 2.0.327
                            HighScoreProgressVal = progressBar.Value;
                            HighScoreProgressMax = progressBar.Maximum;
                        }
                        IList<Integration.HighScore.HighScoreResult> res = Integration.HighScore.GetHighScoreForActivity(activities, progressBar);
                        if (res != null && res.Count > 0)
                        {
                            foreach (Integration.HighScore.HighScoreResult h in res)
                            {
                                this.Status = TrailOrderStatus.Match;
                                TrailResultWrapper result = new TrailResultWrapper(this, h.activity, h.selInfo, h.tooltip, m_resultsListWrapper.Count + 1);
                                m_resultsListWrapper.Add(result);
                            }
                        }
                        if (null != progressBar)
                        {
                            progressBar.Maximum = HighScoreProgressMax;
                            progressBar.Value = HighScoreProgressVal + activities.Count;
                        }
                    }
                }
                else
                {
                    IList<TrailGPSLocation> trailgps = null;
                    IList<IGPSBounds> locationBounds = new List<IGPSBounds>();
                    if (!m_trail.IsSplits)
                    {
                        trailgps = m_trail.TrailLocations;
                        int noNonReq = 0;
                        foreach (TrailGPSLocation l in trailgps)
                        {
                            if (!l.Required)
                            {
                                noNonReq++;
                            }
                            locationBounds.Add(TrailGPSLocation.getGPSBounds(new List<TrailGPSLocation> { l }, 10 * this.m_trail.Radius));
                        }
                        MaxRequiredMisses = Math.Min(trailgps.Count - noNonReq, MaxRequiredMisses);
                    }
                    foreach (IActivity activity in activities)
                    {
                        if (m_trail.IsSplits)
                        {
                            this.Status = TrailOrderStatus.Match;
                            TrailResultWrapper result = new TrailResultWrapper(this, activity, m_resultsListWrapper.Count + 1);
                            m_resultsListWrapper.Add(result);
                        }
                        else
                        {
                            if (trailgps.Count > 0)
                            {
                                bool inBound = false;
                                if (m_trail.IsInBounds(new List<IActivity> { activity }))
                                {
                                    inBound = true;
                                    if (!m_inBound.Contains(activity))
                                    {
                                        m_inBound.Add(activity);
                                    }
                                }
                                //TODO: optimize prune (Intersect()) if MaxReq is set, to see that at least one point matches
                                //As this is currently used when adding is used only for EditTrail, no concern
                                if ((inBound || MaxRequiredMisses > 0) && activity.GPSRoute != null)
                                {
                                    CalcInboundResults(activity, trailgps, locationBounds, MaxRequiredMisses, false, progressBar);
                                    if (bidirectional)
                                    {
                                        IList<TrailGPSLocation> trailgpsReverse = new List<TrailGPSLocation>();
                                        IList<IGPSBounds> locationBoundsReverse = new List<IGPSBounds>();
                                        for (int i = trailgps.Count - 1; i >= 0; i--)
                                        {
                                            trailgpsReverse.Add(trailgps[i]);
                                            locationBoundsReverse.Add(locationBounds[i]);
                                        }
                                        CalcInboundResults(activity, trailgpsReverse, locationBoundsReverse, MaxRequiredMisses, true, progressBar);
                                    }
                                }
                                //NotInBound is pruned prior to this
                            }
                        }
                        if (null != progressBar && progressBar.Value < progressBar.Maximum)
                        {
                            progressBar.Value++;
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

        private class pInfo
        {
            public pInfo(int index, float dist, float prevDist)
            {
                this.index = index;
                this.dist = dist;
                this.prevDist = prevDist;
            }
            public float checkPass(ActivityTrail trail, IActivity activity, TrailGPSLocation trailp, ref float closeDist)
            {
                double dist;
                float routeFactor = ActivityTrail.checkPass(trail.Trail.Radius,
                  routePoint(activity, this.index - 1), this.prevDist,
                  routePoint(activity, this.index), this.dist,
                  trailp,
                    /* dTrack[routeIndex].Value - dTrack[prevRouteIndex].Value, */
                  out dist);
                if (routeFactor > 0)
                {
                    closeDist = (float)dist;
                }
                return routeFactor;
            }
            public int index;
            public float dist;
            public float prevDist;
        }

        private void CalcInboundResults(IActivity activity, IList<TrailGPSLocation> trailgps, IList<IGPSBounds> locationBounds, int MaxRequiredMisses, bool reverse, System.Windows.Forms.ProgressBar progressBar)
        {
            IList<TrailResultPointMeta> resultPoints = new List<TrailResultPointMeta>();

            //Cache information about previous distances
            PointInfo prevPoint = new PointInfo(-1, 0);
            PointInfo prevStartPoint = new PointInfo(-1, 0);
            int prevActivityMatchIndex = -1; //Last index that match for this activity

            //Required points misses - undocumented feature
            int currRequiredMisses = 0;
            //Ignore short legs - undocumented feature
            IDistanceDataTrack dTrack = null;
            if (this.Trail.MinDistance > 0)
            {
                //Must use the distance track related to the GPS track here, not the (potentially optimized) InfoCache track
                dTrack = activity.GPSRoute.GetDistanceMetersTrack();
            }

            for (int routeIndex = 0; routeIndex < activity.GPSRoute.Count; routeIndex++)
            {
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
                if (trailgps.Count > 1 && resultPoints.Count == 1 &&
                    !resultPoints[0].restart &&
                    resultPoints[0].Time > DateTime.MinValue &&
                    routeIndex > 1 + Math.Max(resultPoints[0].index, Math.Max(resultPoints[0].matchInRadius,
                    resultPoints[0].matchPassBy)) &&
                    locationBounds[0].Contains((IGPSLocation)(activity.GPSRoute[routeIndex].Value)))
                {
                    PointInfo startPoint = new PointInfo(routeIndex,
                         distanceTrailToRoute(activity, trailgps, 0, routeIndex));
                    bool match = false;
                    if (startPoint.dist < this.m_trail.Radius)
                    {
                        match = true;
                    }
                    if (!match)
                    {
                        if (prevStartPoint.index != routeIndex - 1)
                        {
                            //refresh cache
                            prevStartPoint.index = routeIndex - 1;
                            prevStartPoint.dist = distanceTrailToRoute(activity, trailgps, 0, prevStartPoint.index);
                        }
                        double d;
                        if (0 < checkPass(this.Trail.Radius,
                    routePoint(activity, prevStartPoint.index), prevStartPoint.dist,
                    routePoint(activity, startPoint.index), startPoint.dist,
                    trailgps[TrailIndex(trailgps, 0)]/*, dTrack[routeIndex].Value - dTrack[prevStartPointIndex].Value*/,
                    out d))
                        {
                            match = true;
                        }
                    }
                    if (match)
                    {
                        //Start over if we pass first point before all were found
                        resultPoints.Clear();
                    }
                    prevStartPoint = startPoint;
                }

                /////////////////////////////////////
                //try matching

                float routeDist = float.MaxValue;
                int matchIndex = -1;
                float matchDist = float.MaxValue; //distance at matchtime
                DateTime? matchTime = null; //Time for match or DateTime.Min for inserted
                //latest match - not necessarily the best match but needed for 
                int lastMatchInRadiusIndex = -1;
                int lastMatchPassByIndex = -1;

                //Check if the point is in bounds, the distance to point is the heaviest calculation per point
                //location bounds is aproximate, should cover the apopriate "checkPass" area
                if (locationBounds[trailgps.Count==1 ? 0:resultPoints.Count].Contains((IGPSLocation)(activity.GPSRoute[routeIndex].Value)))
                {
                    routeDist = distanceTrailToRoute(activity, trailgps, resultPoints.Count, routeIndex);
                    if (routeIndex > 0 && (prevPoint.index < 0 || routeIndex - prevPoint.index != 1 || prevPoint.dist == float.MaxValue))
                    {
                        prevPoint.index = routeIndex - 1;
                        prevPoint.dist = distanceTrailToRoute(activity, trailgps, resultPoints.Count, prevPoint.index);
                    }

                    //////////////////////////////////////
                    //Find the best GPS point for this result
                    if (routeDist < this.Trail.Radius)
                    {
                        bool firstPoint = true;
                        float prevRouteDist = prevPoint.dist;
                        float distHysteresis = Math.Max(this.Trail.Radius / 30, 5);
                        Stack<pInfo> info = new Stack<pInfo>();
                        float matchFactor = -1;

                        //routeIndex is a match, but points following may be better
                        //Go through the points while we are in the radius or at last point
                        //(for last point we break, next may follow directly) 
                        while (true)
                        {
                            if (!firstPoint)
                            {
                                //avoid calculations...
                                routeDist = distanceTrailToRoute(activity, trailgps, resultPoints.Count, routeIndex);
                            }
                            firstPoint = false;

                            pInfo pointInfo = new pInfo(routeIndex, routeDist, prevRouteDist);
                            if (resultPoints.Count == 0)
                            {
                                //start point
                                //Just cycle through the points, to prepare for back track to first (matchIndex)
                                info.Push(pointInfo);
                            }
                            else
                            {
                                float routeFactor = -1;
                                float closeDist = pointInfo.dist;
                                if (routeIndex > 0)
                                {
                                    //Check closest point for first in the 
                                    routeFactor = pointInfo.checkPass(this, activity,
                                       trailgps[TrailIndex(trailgps, resultPoints.Count)], ref closeDist);
                                }

                                if (closeDist < matchDist)
                                {
                                    //Better, still closing in
                                    matchIndex = pointInfo.index;
                                    matchDist = closeDist;
                                    matchFactor = routeFactor;
                                }
                                if (isEndTrailPoint(trailgps, resultPoints.Count + 1) &&
                                    pointInfo.dist > matchDist + distHysteresis)
                                {
                                    //Leaving middle for last point - no more checks
                                    break;
                                }
                            }

                            if (routeDist >= this.Trail.Radius || routeIndex >= activity.GPSRoute.Count - 1)
                            {
                                if (routeDist >= this.Trail.Radius)
                                {
                                    //No longer in radius, we have the best match
                                    //As we peeked on next, we have to set back the index
                                    routeIndex--;
                                    routeDist = prevRouteDist;
                                }

                                if (resultPoints.Count == 0)
                                {
                                    foreach (pInfo p in info)
                                    {
                                        float routeFactor = -1;
                                        float closeDist = p.dist;
                                        if (routeIndex > 0)
                                        {
                                            routeFactor = pointInfo.checkPass(this, activity,
                                               trailgps[TrailIndex(trailgps, resultPoints.Count)], ref closeDist);
                                        }

                                        if (closeDist < matchDist)
                                        {
                                            //Better, still closing in
                                            matchIndex = p.index;
                                            matchDist = closeDist;
                                            matchFactor = routeFactor;
                                        }
                                        if (p.prevDist > matchDist + distHysteresis)
                                        {
                                            //Leaving middle for last point - no more checks
                                            break;
                                        }
                                    }
                                }
                                break;
                            }

                            prevRouteDist = routeDist;
                            routeIndex++;
                        }

                        //The last indexes matching in radius
                        lastMatchInRadiusIndex = routeIndex;
                        lastMatchPassByIndex = routeIndex + 1;

                        if (matchFactor > 0)
                        {
                            DateTime d1 = activity.GPSRoute.EntryDateTime(activity.GPSRoute[matchIndex - 1]);
                            DateTime d2 = activity.GPSRoute.EntryDateTime(activity.GPSRoute[matchIndex]);
                            matchTime = d1.Add(TimeSpan.FromSeconds(matchFactor * (d2 - d1).TotalSeconds));
                            if (matchFactor < 0.5)
                            {
                                matchIndex--;
                            }
                        }
                        else
                        {
                            matchTime = activity.GPSRoute.EntryDateTime(activity.GPSRoute[matchIndex]);
                        }
                    } //if (dist < radius)

                    ///////////
                    //Check for pass-by
                    //Setting a limit here like (routeDist < 10*this.Trail.Radius) will improve detection time very slightly
                    //This handling is very sensitive, especially for single point trails, the reason for the check
                    else if (trailgps.Count > 1 || resultPoints.Count == 0 || routeIndex > resultPoints[resultPoints.Count - 1].matchPassBy)
                    {
                        double d;
                        float factor = checkPass(this.Trail.Radius,
                            routePoint(activity, prevPoint.index), prevPoint.dist,
                            routePoint(activity, routeIndex), routeDist,
                            trailgps[TrailIndex(trailgps, resultPoints.Count)],
                            /*, dTrack[routeIndex].Value - dTrack[prevRouteIndex].Value*/
                            out d);
                        if (0 < factor)
                        {
                            DateTime d1 = activity.GPSRoute.EntryDateTime(activity.GPSRoute[prevPoint.index]);
                            DateTime d2 = activity.GPSRoute.EntryDateTime(activity.GPSRoute[routeIndex]);
                            matchTime = d1.Add(TimeSpan.FromSeconds(factor * (d2 - d1).TotalSeconds));

                            //if setting matchIndex to prev point, following matches can match but give loop issues
                            matchIndex = routeIndex;// prevPoint.index;
                            matchDist = (float)d;
                            lastMatchPassByIndex = routeIndex;
                        }
                    }
                }

                //////////////////////////////
                //All GPS points tested but search should maybe match
                bool automaticMatch = false;
                if (matchTime == null && routeIndex >= activity.GPSRoute.Count - 1)
                {
                    bool required = trailgps[TrailIndex(trailgps, resultPoints.Count)].Required;

                    ///////////////////
                    //Last point check for non required points - automatic match, so search can restart
                    if (!required)
                    {
                        automaticMatch = true;
                    }
                    else if (currRequiredMisses < MaxRequiredMisses)
                    {
                        automaticMatch = true;
                        //OK to miss this point. Set automatic match to start looking at prev match
                        currRequiredMisses++;
                    }
                    if (automaticMatch)
                    {
                        matchTime = DateTime.MinValue;
                        matchDist = this.Trail.Radius * 2;
                    }
                }

                ////////////////////////////
                //Ignore short legs
                if (this.Trail.MinDistance > 0 && resultPoints.Count > 0 &&
                    matchTime != null && !automaticMatch)
                {
                    int prevMatchIndex = getPrevMatchIndex(resultPoints);
                    if (dTrack != null && prevMatchIndex >= 0 &&
                    this.Trail.MinDistance > (dTrack[matchIndex].Value - dTrack[prevMatchIndex].Value))
                    {
                        //matchIndex = -1;
                        matchTime = null;
                    }
                }

                /////////////////////////////////////
                //Add found match to result 
                if (matchTime != null)
                {
                    resultPoints.Add(new TrailResultPointMeta((DateTime)matchTime,
                        trailgps[TrailIndex(trailgps, resultPoints.Count)].Name,
                        matchIndex, lastMatchInRadiusIndex, lastMatchPassByIndex, matchDist));

                    //Clear cache, dist unknown to next point
                    prevPoint.index = -1;

                    if (isEndTrailPoint(trailgps, resultPoints.Count))
                    {
                        //Check if this is a partial match, also used for automatic matches
                        bool isPartial = false;
                        int noOfMatches = 0;
                        foreach (TrailResultPointMeta i in resultPoints)
                        {
                            if (i.Time != DateTime.MinValue)
                            {
                                noOfMatches++;
                            }
                            else
                            {
                                isPartial = true;
                            }
                        }

                        //A result must have at least two matches, otherwise it is not possible to get distance etc
                        if (noOfMatches >= 2)
                        {
                            if (isPartial)
                            {
                                Status = TrailOrderStatus.MatchPartial;
                            }
                            else
                            {
                                Status = TrailOrderStatus.Match;
                            }
                            TrailResultInfo resultInfo = new TrailResultInfo(activity);
                            float distDiff = 0;
                            for (int i = 0; i < resultPoints.Count; i++)
                            {
                                TrailResultPointMeta point = resultPoints[i];
                                //Include the point if not restart
                                if (point.restart)
                                //Something like the following could be used to recover previous points, but 
                                //i+1 must be the first required point with one non required in between (the last non required is always "bad")
                                //i < resultPoints.Count - 1 && !resultPoints[i + 1].restart &&
                                //point.Time > DateTime.MinValue && point.Time <= resultPoints[i + 1].Time)
                                {
                                    point.Time = DateTime.MinValue;
                                }
                                resultInfo.Points.Add(point);
                                distDiff += point.trailDistDiff;
                            }
                            TrailResultWrapper result = new TrailResultWrapper(this, m_resultsListWrapper.Count + 1, resultInfo,
                                distDiff, reverse);
                            m_resultsListWrapper.Add(result);

                            //Save latest match info, routeIndex should not be lower than this
                            prevActivityMatchIndex = getPrevMatchIndex(resultPoints);
                        }
                        else if (!reverse && noOfMatches > 0)
                        {
                            //Reverse matches with one point will be the same as forward match, ignore
                            IncompleteTrailResult result = new IncompleteTrailResult(activity, resultPoints, reverse);
                            m_incompleteResults.Add(result);
                            prevActivityMatchIndex = getPrevMatchIndex(resultPoints);
                        }
                        else
                        {
                            //no matches at all, abort
                            prevActivityMatchIndex = activity.GPSRoute.Count;
                        }

                        //Reset matches for trail detection
                        currRequiredMisses = 0;
                        resultPoints.Clear();
                    }
                    else if (matchTime != DateTime.MinValue)
                    {
                        //Not end point, but at least one point match
                        Status = TrailOrderStatus.InBoundMatchPartial;
                    }
                }
                else
                {
                    //Cache previous values, used in passed-by checks
                    prevPoint.index = routeIndex;
                    prevPoint.dist = routeDist;
                }

                ////////////////////////////////////
                //Determine where to start from
                //The normal case at no match is to continue with next routePoint
                //At matches and end points, the match should maybe restart
                if (automaticMatch)
                {
                    //If this was an automatic match, start at prev match
                    routeIndex = Math.Max(prevActivityMatchIndex, getPrevMatchIndex(resultPoints));
                }
                else if (matchTime != null)
                {
                    if (resultPoints.Count == 0)
                    {
                        //End point, try this point again as start
                        routeIndex = matchIndex - 1;
                    }
                    else
                    {
                        //For single point trail, we need to start after current radius to not get immediate match
                        //For two point trails the result is unexpected if the first/last points completely overlap
                        //For trails with more points, assume that all points do not overlap
                        if (2 >= trailgps.Count)
                        {
                            //One of these must have been a match
                            int last = Math.Max(lastMatchInRadiusIndex, lastMatchPassByIndex);
                            routeIndex = last;
                        }
                        else
                        {
                            //Start search for next point after this match even if they overlap
                            routeIndex = matchIndex;
                        }
                    }
                }
                //Reached the end with no match (point or automatic)
                else if (routeIndex >= activity.GPSRoute.Count - 1 &&
                         resultPoints.Count > 0 &&
                        !resultPoints[resultPoints.Count - 1].restart)
                {
                    ////////////////////////////////////
                    //We have reached the end without a match
                    //If there are non-required previous, try dropping them and continue

                    //Last req index that match for this activity
                    int prevReqMatchIndex = -1;
                    bool matchNoReqToIgnore = false;
                    for (int i = resultPoints.Count - 1; i >= 0; i--)
                    {
                        if (trailgps[TrailIndex(trailgps, i)].Required)
                        {
                            prevReqMatchIndex = resultPoints[i].index;
                            break;
                        }
                        else if (!resultPoints[i].restart)
                        {
                            //Hide the non-required point
                            resultPoints[i].restart = true;
                            matchNoReqToIgnore = true;
                        }
                    }
                    if (matchNoReqToIgnore)
                    {
                        routeIndex = Math.Max(prevActivityMatchIndex, prevReqMatchIndex);
                        prevPoint.index = -1;
                    }
                }

                ///////////////////////////////////////
            } //foreach gps point

            ///////////////////////////////////////
            //Update "no result counters"
            {
                //InBoundMatchPartial updated for all incomplete results
                int noOfMatches = 0;
                foreach (TrailResultPointMeta i in resultPoints)
                {
                    if (i.Time != DateTime.MinValue)
                    {
                        noOfMatches++;
                    }
                }

                TrailOrderStatus? newStatus = null; 
                if (noOfMatches > 0 && !reverse || noOfMatches > 1)
                {
                    IncompleteTrailResult result = new IncompleteTrailResult(activity, resultPoints, reverse);
                    m_incompleteResults.Add(result);

                    newStatus = TrailOrderStatus.InBoundMatchPartial;
                }
                else if (m_status >= TrailOrderStatus.InBoundNoCalc)
                {
                    newStatus = TrailOrderStatus.InBound;
                }

                if (newStatus != null)
                {
                    if (!m_noResCount.ContainsKey((TrailOrderStatus)newStatus))
                    {
                        m_noResCount[(TrailOrderStatus)newStatus] = 0;
                    }
                    m_noResCount[(TrailOrderStatus)newStatus]++;
                }
            }
        }

        ////////////////////////////
        private static int getPrevMatchIndex(IList<TrailResultPointMeta> resultPoints)
        {
            int currMatches = resultPoints.Count;
            int prevMatchIndex = -1;
            for (int i = currMatches - 1; i >= 0; i--)
            {
                if (!resultPoints[i].restart && resultPoints[i].index > -1 && resultPoints[i].Time > DateTime.MinValue)
                {
                    prevMatchIndex = resultPoints[i].index;
                    break;
                }
            }
            return prevMatchIndex;
        }

        private static float checkPass(float radius, IGPSPoint r1, float dt1, IGPSPoint r2, float dt2, TrailGPSLocation trailp/*, float d12*/, out double d)
        {
            d = double.MaxValue;
            float factor = -1;
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
                || dt1 < radius * Math.Sqrt(2)
                && dt2 < radius * Math.Sqrt(2))
            {
                //Law of cosines - get a1, angle at r1, the first point
                double d12 = r1.DistanceMetersToPoint(r2);
                double a10 = (dt1 * dt1 + d12 * d12 - dt2 * dt2) / (2 * dt1 * d12);
                //Point is in circle if closest point is between r1&r2 and it is in circle (neither r1 nor r2 is)
                //This means the angle a1 must be +/- 90 degrees : cos(a1)>=0
                if (a10 > -0.001)
                {
                    //Rounding errors w GPS measurements
                    a10 = Math.Min(a10, 1);
                    a10 = Math.Max(a10, -1);
                    double a1 = Math.Acos(a10);
                    //Dist from r1 to closest point on d1-d2 (called x)
                    double d1x = Math.Abs(dt1 * Math.Cos(a1));
                    //Dist from t1 to x on line between d1-d2
                    double dtx = dt1 * Math.Sin(a1);
                    if (d1x < d12 && dtx < radius)
                    {
                        d = dtx;
                        factor = (float)(d1x / d12);
                        //Return factor, to return best aproximation from r1
                    }
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

        ///////////////////////////////////////////
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

        ///////////////
        public IList<TrailResult> IncompleteResults
        {
            get
            {
                IList<TrailResult> result = new List<TrailResult>();
                ((List<IncompleteTrailResult>)m_incompleteResults).Sort();
                foreach (IncompleteTrailResult t in m_incompleteResults)
                {
                    TrailResultInfo resultInfo = new TrailResultInfo(t.Activity);
                    float distDiff = 0;
                    for (int i = 0; i < t.Points.Count; i++)
                    {
                        TrailResultPointMeta point = t.Points[i];
                        //For incomplete, do not care if restart points were used or not
                        resultInfo.Points.Add(point);
                        distDiff += point.trailDistDiff;
                    }
                    //Set remaining points
                    for (int i = t.Points.Count; i < m_trail.TrailLocations.Count; i++)
                    {
                        resultInfo.Points.Add(new TrailResultPoint(DateTime.MinValue, m_trail.TrailLocations[i].Name));
                    }
                    TrailResult tr = new TrailResult(this, m_resultsListWrapper.Count + 1,
                        resultInfo, distDiff, t.Reverse);
                    result.Add(tr);
                }
                return result;
            }
        }

        /////////////////
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
                if (this.Trail.IsSplits != to2.Trail.IsSplits)
                {
                    return (this.Trail.IsSplits) ? 1 : -1;
                }
                else if (this.Trail.Generated != to2.Trail.Generated)
                {
                    return (this.Trail.Generated) ? 1 : -1;
                }
                else if (this.Trail.TrailLocations.Count != to2.Trail.TrailLocations.Count &&
                    (this.Trail.TrailLocations.Count==1 || to2.Trail.TrailLocations.Count==1))
                {
                    return (this.Trail.TrailLocations.Count < to2.Trail.TrailLocations.Count) ? 1 : -1;
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
                //(Splits could be before Ref but his will increase response time with many activities)
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

        internal class PointInfo
        {
            public PointInfo(int index, float dist)
            {
                this.index = index;
                this.dist = dist;
            }
            public int index;
            public float dist;
        }

        internal class IncompleteTrailResult : IComparable
        {
            //Save some info for incomplete matches, calculate TrailResults if needed
            public IncompleteTrailResult(IActivity activity, IList<TrailResultPointMeta> Points, bool reverse)
            {
                this.Activity = activity;
                this.Points = Points;
                this.Reverse = reverse;
            }
            public IList<TrailResultPointMeta> Points;
            public IActivity Activity;
            public bool Reverse;

            public int CompareTo(object obj)
            {
                if (!(obj is IncompleteTrailResult))
                {
                    return -1;
                }
                IncompleteTrailResult o2 = (IncompleteTrailResult)obj;
                if (this.Points.Count != o2.Points.Count)
                {
                    return this.Points.Count > o2.Points.Count ? 1 : -1;
                }
                if (this.Reverse != o2.Reverse)
                {
                    return this.Reverse ? -1 : 1;
                }
                return 0;
            }
        }
        internal class TrailResultPointMeta : TrailResultPoint
        {
            public TrailResultPointMeta(DateTime time, string name,
                int index, int lastMatchInRadius, int lastMatchPassBy, float trailDist) :
                base(time, name)
            {
                this.index = index;
                this.matchInRadius = lastMatchInRadius;
                this.matchPassBy = lastMatchPassBy;
                this.trailDistDiff = trailDist;
            }
            public int index;
            public int matchInRadius;
            public int matchPassBy;
            public bool restart;
            public float trailDistDiff;
        }

        public override string ToString()
        {
            return (new TrailDropdownLabelProvider()).GetText(this, null) + ": " + this.Status.ToString();
        }
    }

    /*******************************************************/

    public class TrailDropdownLabelProvider : TreeList.ILabelProvider
    {

        public System.Drawing.Image GetImage(object element, TreeList.Column column)
        {
            ActivityTrail t = (ActivityTrail)element;
            if (t.ActivityCount == 0)
            {
                return Properties.Resources.square_blue;
            }
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
            if (t.Trail.IsReference && null != t.Trail.ReferenceActivityNoCalc)
            {
                DateTime time = ActivityInfoCache.Instance.GetInfo(t.Trail.ReferenceActivityNoCalc).ActualTrackStart;
                if (DateTime.MinValue == time)
                {
                    time = t.Trail.ReferenceActivityNoCalc.StartTime;
                }
                name += " " + time.ToLocalTime().ToString();
            }

            if (t.Status == TrailOrderStatus.Match ||
                t.Status == TrailOrderStatus.MatchPartial)
            {
                name += " (" + t.Results.Count + ")";
            }
            else if (t.Status == TrailOrderStatus.MatchNoCalc)
            {
                if (t.Trail.IsSplits)
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
