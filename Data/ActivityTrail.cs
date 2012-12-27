/*
Copyright (C) 2009 Brendan Doherty
Copyright (C) 2010-2012 Gerhard Olsson

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
            this.m_controller = controller;
            this.m_trail = trail;
            this.m_status = TrailOrderStatus.NoInfo;

            if (this.m_trail.Generated && !this.m_trail.IsReference)
            {
                this.m_canAddInbound = false;
            }

            //Preset status
            if (this.m_trail.TrailType == Trail.CalcType.HighScore)
            {
                if (Integration.HighScore.HighScoreIntegrationEnabled)
                {
                    this.m_status = TrailOrderStatus.MatchNoCalc;
                }
                else
                {
                    this.m_status = TrailOrderStatus.NotInstalled;
                }
            }
            else if (m_trail.TrailType == Trail.CalcType.Splits)
            {
                //By default, always match
                this.m_status = TrailOrderStatus.MatchNoCalc;
            }
            else if (Trail.IsReference)
            {
                if (trail.ReferenceActivity != null && trail.ReferenceActivity.GPSRoute != null)
                {
                    // Let Reference always match, to trigger possible recalc after
                    this.m_status = TrailOrderStatus.MatchNoCalc;
                }
                else
                {
                    this.m_status = TrailOrderStatus.NotInBound;
                }
            }
            else if (Trail.TrailLocations.Count == 0)
            {
                this.m_status = TrailOrderStatus.NotInBound;
            }
        }

		public Data.Trail Trail
        {
			get
            {
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

        private static TrailOrderStatus BestCalcStatus(TrailOrderStatus current, TrailOrderStatus upd)
        {
            //Set "better" status only
            if (upd < current)
            {
                return upd;
            }
            return current;
        }

        public TrailOrderStatus Status
        {
            get
            {
                return m_status;
            }
            set
            {
                m_status = BestCalcStatus(m_status, value);
            }
        }

        public bool IsInBounds
        {
            get
            {
                if (this.Status == TrailOrderStatus.NoInfo ||
                    m_trail.IsReference && this.Status == TrailOrderStatus.MatchNoCalc)
                {
                    if (m_trail.IsInBounds(m_controller.Activities) || 
                        m_trail.IsReference && m_trail.ReferenceActivity == null)
                    {
                        //Do not downgrade MatchNoCalc here
                        this.Status = TrailOrderStatus.InBoundNoCalc;
                    }
                    else
                    {
                        //Downgrade status
                        this.m_status = TrailOrderStatus.NotInBound;
                    }
                }
                //Any activity in bounds?
                return this.Status <= TrailOrderStatus.InBound;
            }
        }

        public bool IsNoCalc
        {
            get
            {
                return (this.Status == TrailOrderStatus.MatchNoCalc || this.Status == TrailOrderStatus.InBoundNoCalc);
            }
        }

        public void Reset()
        {
            m_resultsListWrapper = null;
            m_incompleteResults = null;
        }

        public void Clear(bool onlyDisplay)
        {
            if (m_resultsListWrapper != null)
            {
                foreach (TrailResultWrapper t in m_resultsListWrapper)
                {
                    t.Result.Clear(onlyDisplay);
                }
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

        public IList<TrailResultWrapper> ResultTreeListRows()
        {
            IList<TrailResultWrapper> results = new List<TrailResultWrapper>();
            foreach (TrailResultWrapper t in ResultTreeList)
            {
                results.Add(t);
            }
            return results;
        }

        //public void Sort()
        //{
        //    ((List<TrailResultWrapper>)ResultTreeList).Sort();
        //    int i = 1;
        //    foreach (TrailResultWrapper tr in ResultTreeList)
        //    {
        //        tr.Result.Order = i;
        //        i++;
        //        tr.Sort();
        //    }
        //}

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
            CalcResults(m_controller.Activities, m_trail.MaxRequiredMisses, m_trail.BiDirectional, null);
        }

        public void CalcResults(System.Windows.Forms.ProgressBar progressBar)
        {
            CalcResults(m_controller.Activities, m_trail.MaxRequiredMisses, m_trail.BiDirectional, progressBar);
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

                //Calculation depends on TrailType
                if (m_trail.TrailType == Trail.CalcType.HighScore)
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
                        IList<Integration.HighScore.HighScoreResult> hs = Integration.HighScore.GetHighScoreForActivity(activities, 10, progressBar);
                        if (hs != null && hs.Count > 0)
                        {
                            TrailResultWrapper parent = null;
                            foreach (Integration.HighScore.HighScoreResult h in hs)
                            {
                                this.Status = TrailOrderStatus.Match;
                                TrailResultWrapper result = new TrailResultWrapper(this, parent, h.activity, h.selInfo, h.tooltip, h.order/* m_resultsListWrapper.Count + 1*/);
                                if (h.order == 1)
                                {
                                    m_resultsListWrapper.Add(result);
                                    parent = result;
                                }
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
                    if (m_trail.TrailType == Trail.CalcType.TrailPoints)
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
                        if (m_trail.TrailType == Trail.CalcType.Splits)
                        {
                            this.Status = TrailOrderStatus.Match;
                            TrailResultWrapper result = new TrailResultWrapper(this, activity, m_resultsListWrapper.Count + 1);
                            m_resultsListWrapper.Add(result);
                        }
                        else
                        {
                            TrailOrderStatus activityStatus = TrailOrderStatus.NoInfo;
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
                                    //Status is at least inbound (even if this is a "downgrade")
                                    if (m_status == TrailOrderStatus.InBoundNoCalc)
                                    {
                                        m_status = TrailOrderStatus.InBound;
                                    }
                                    activityStatus = CalcInboundResults(activity, trailgps, locationBounds, MaxRequiredMisses, false, progressBar);
                                    //No need to check bidirectional for one point trails
                                    if (bidirectional && trailgps.Count > 1 &&
                                        activityStatus != TrailOrderStatus.Match && activityStatus < TrailOrderStatus.InBound)
                                    {
                                        IList<TrailGPSLocation> trailgpsReverse = new List<TrailGPSLocation>();
                                        IList<IGPSBounds> locationBoundsReverse = new List<IGPSBounds>();
                                        for (int i = trailgps.Count - 1; i >= 0; i--)
                                        {
                                            trailgpsReverse.Add(trailgps[i]);
                                            locationBoundsReverse.Add(locationBounds[i]);
                                        }
                                        activityStatus = CalcInboundResults(activity, trailgpsReverse, locationBoundsReverse, MaxRequiredMisses, true, progressBar);
                                    }
                                }
                                //NotInBound is pruned prior to this
                            }
                            //If there was no other match, try match name
                            if (activityStatus >= TrailOrderStatus.InBound && this.m_trail.IsNameMatch && !string.IsNullOrEmpty(activity.Name) &&
                                (activity.Name.StartsWith(this.m_trail.Name) || this.m_trail.Name.StartsWith(activity.Name)))
                            {
                                this.Status = TrailOrderStatus.Match;
                                //Splits result
                                TrailResultWrapper result = new TrailResultWrapper(this, activity, m_resultsListWrapper.Count + 1);
                                m_resultsListWrapper.Add(result);
                            }
                        }
                        
                        if (null != progressBar && progressBar.Value < progressBar.Maximum)
                        {
                            progressBar.Value++;
                        }
                    }
                    //Always set InBound count, used in some displays
                    m_noResCount[TrailOrderStatus.InBound] = m_inBound.Count;
                    foreach (TrailResultWrapper tr in this.m_resultsListWrapper)
                    {
                        tr.getSplits();
                    }
                }
                if (m_resultsListWrapper.Count == 0 && m_status < TrailOrderStatus.InBound && this.IsNoCalc)
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

            public float checkPass(IActivity activity, TrailGPSLocation trailp, float radius, ref float closeDist)
            {
                double dist;
                float routeFactor = ActivityTrail.checkPass(radius,
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

        private TrailOrderStatus CalcInboundResults(IActivity activity, IList<TrailGPSLocation> trailgps, IList<IGPSBounds> locationBounds, int MaxRequiredMisses, bool reverse, System.Windows.Forms.ProgressBar progressBar)
        {
            IList<TrailResultInfo> trailResults = new List<TrailResultInfo>();
            TrailOrderStatus status = CalcTrail(activity, trailgps, locationBounds, this.Trail.Radius, this.Trail.MinDistance, MaxRequiredMisses, reverse, 0, trailResults, m_incompleteResults, progressBar);

            foreach (TrailResultInfo resultInfo in trailResults)
            {
                TrailResultWrapper result = new TrailResultWrapper(this, m_resultsListWrapper.Count + 1, resultInfo);
                m_resultsListWrapper.Add(result);
            }

            if (status != TrailOrderStatus.Match)
            {
                if (!m_noResCount.ContainsKey(status))
                {
                    m_noResCount[status] = 0;
                }
                m_noResCount[status]++;
            }
            //Update accumulated status
            this.Status = status;

            return status;
        }

        internal static TrailResultPoint GetClosestMatch(IActivity activity, IGPSLocation gps, float radius)
        {
            //A fix to get best point. 
            //Done rather that destroying trail detection furthe
            IList<TrailResultInfo> trailResults = new List<TrailResultInfo>(); //Unused
            IList<ActivityTrail.IncompleteTrailResult> incompleteResults = new List<ActivityTrail.IncompleteTrailResult>();
            IList<TrailGPSLocation> trailgps = Trail.TrailGpsPointsFromGps(new List<ZoneFiveSoftware.Common.Data.GPS.IGPSLocation>{gps});
            //Force all results to be incomplete, to match all matches along a track (to avoid best match is thrown away)
            GetTrailResultInfo(activity, trailgps, radius, false, 1, trailResults, incompleteResults);

            IList<TrailResultPoint> points = new List<TrailResultPoint>();
            foreach (TrailResultInfo l in trailResults)
            {
                foreach (TrailResultPoint t in l.Points)
                {
                    points.Add(t);
                }
            }
            foreach (ActivityTrail.IncompleteTrailResult l in incompleteResults)
            {
                foreach (TrailResultPoint t in l.Points)
                {
                    if (t.Time != DateTime.MinValue)
                    {
                        points.Add(t);
                    }
                }
            }
            ((List<TrailResultPoint>)points).Sort();
            if (points.Count == 0)
            {
                return null;
            }
            return points[0];
        }

        internal static TrailOrderStatus GetTrailResultInfo(IActivity activity, IList<IGPSLocation> gpsLoc,
            float radius, bool bidirectional, IList<TrailResultInfo> trailResults)
        {
            IList<ActivityTrail.IncompleteTrailResult> incompleteResults = new List<ActivityTrail.IncompleteTrailResult>(); //unused
            IList<TrailGPSLocation> trailgps = Trail.TrailGpsPointsFromGps(gpsLoc);

            TrailOrderStatus status = GetTrailResultInfo(activity, trailgps, radius, bidirectional, 0, trailResults, incompleteResults);

            //Sort best match first
            ((List<TrailResultInfo>)trailResults).Sort();

            return status;
        }

        private static TrailOrderStatus GetTrailResultInfo(IActivity activity, IList<TrailGPSLocation> trailgps,
            float radius, bool bidirectional, int maxPoints, IList<TrailResultInfo> trailResults, IList<ActivityTrail.IncompleteTrailResult> incompleteResults)
        {
            TrailOrderStatus status = TrailOrderStatus.NoInfo;

            IList<ZoneFiveSoftware.Common.Data.GPS.IGPSBounds> locationBounds = new List<ZoneFiveSoftware.Common.Data.GPS.IGPSBounds>();
            foreach (TrailGPSLocation l in trailgps)
            {
                locationBounds.Add(TrailGPSLocation.getGPSBounds(new List<TrailGPSLocation> { l }, 10 * radius));
            }

            status = CalcTrail(activity, trailgps, locationBounds,
                radius, 0, 0, false, maxPoints, trailResults, incompleteResults, null);
            if (bidirectional && trailgps.Count > 1 && status != TrailOrderStatus.Match && status < TrailOrderStatus.InBound)
            {
                IList<TrailGPSLocation> trailgpsReverse = new List<TrailGPSLocation>();
                IList<IGPSBounds> locationBoundsReverse = new List<IGPSBounds>();
                for (int i = trailgps.Count - 1; i >= 0; i--)
                {
                    trailgpsReverse.Add(trailgps[i]);
                    locationBoundsReverse.Add(locationBounds[i]);
                }
                TrailOrderStatus status2 = CalcTrail(activity, trailgpsReverse, locationBoundsReverse,
                radius, 0, 0, true, maxPoints, trailResults, incompleteResults, null);
                status = BestCalcStatus(status, status2);
            }

            return status;
        }

        /// <summary>
        /// Calculate trail result points for one activity for a certain set of points (Trail)
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="trailgps"></param>
        /// <param name="locationBounds"></param>
        /// <param name="radius"></param>
        /// <param name="minDistance"></param>
        /// <param name="MaxRequiredMisses"></param>
        /// <param name="reverse"></param>
        /// <param name="trailResults"></param>
        /// <param name="incompleteResults"></param>
        /// <param name="progressBar"></param>
        /// <returns></returns>
        private static TrailOrderStatus CalcTrail(IActivity activity, IList<TrailGPSLocation> trailgps, IList<IGPSBounds> locationBounds, 
            float radius, float minDistance, int MaxRequiredMisses, bool reverse, int maxResultPoints,
            IList<TrailResultInfo> trailResults, IList<IncompleteTrailResult> incompleteResults, System.Windows.Forms.ProgressBar progressBar)
        {
            TrailOrderStatus status = TrailOrderStatus.NoInfo;
            IList<TrailResultPointMeta> currResultPoints = new List<TrailResultPointMeta>();

            //Cache information about previous distances
            PointInfo prevPoint = new PointInfo(-1, 0);
            PointInfo prevStartPoint = new PointInfo(-1, 0);
            int prevActivityMatchIndex = -1; //Next match cannot be lower than this
            
            //Required points misses - undocumented feature
            int currRequiredMisses = 0;
            //Ignore short legs - undocumented feature
            IDistanceDataTrack dTrack = null;
            if (minDistance > 0)
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
                if (trailgps.Count > 1 && currResultPoints.Count == 1 &&
                    !currResultPoints[0].restart &&
                    currResultPoints[0].Time > DateTime.MinValue &&
                    routeIndex > 1 + Math.Max(currResultPoints[0].index, Math.Max(currResultPoints[0].matchInRadius,
                    currResultPoints[0].matchPassBy)) &&
                    locationBounds[0].Contains((IGPSLocation)(activity.GPSRoute[routeIndex].Value)))
                {
                    PointInfo startPoint = new PointInfo(routeIndex,
                         distanceTrailToRoute(activity, trailgps, 0, routeIndex));
                    bool match = false;
                    if (startPoint.dist < radius)
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
                        if (0 < checkPass(radius,
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
                        currResultPoints.Clear();
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
                //location bounds is aproximate, should cover the aproximate "checkPass" area
                if (locationBounds[trailgps.Count==1 ? 0 : currResultPoints.Count].Contains((IGPSLocation)(activity.GPSRoute[routeIndex].Value)))
                {
                    routeDist = distanceTrailToRoute(activity, trailgps, currResultPoints.Count, routeIndex);
                    if (routeIndex > 0 && (prevPoint.index < 0 || routeIndex - prevPoint.index != 1 || prevPoint.dist == float.MaxValue))
                    {
                        prevPoint.index = routeIndex - 1;
                        prevPoint.dist = distanceTrailToRoute(activity, trailgps, currResultPoints.Count, prevPoint.index);
                    }

                    //////////////////////////////////////
                    //Find the best GPS point for this result
                    if (routeDist < radius)
                    {
                        bool firstPoint = true;
                        float prevRouteDist = prevPoint.dist;
                        float distHysteresis = Math.Max(radius / 30, 5);
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
                                routeDist = distanceTrailToRoute(activity, trailgps, currResultPoints.Count, routeIndex);
                            }
                            firstPoint = false;

                            {
                                pInfo pointInfo = new pInfo(routeIndex, routeDist, prevRouteDist);
                                if (currResultPoints.Count == 0)
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
                                        //Check closest point
                                        routeFactor = pointInfo.checkPass(activity,
                                           trailgps[TrailIndex(trailgps, currResultPoints.Count)], radius, ref closeDist);
                                    }

                                    if (closeDist < matchDist)
                                    {
                                        //Better, still closing in
                                        matchIndex = pointInfo.index;
                                        matchDist = closeDist;
                                        matchFactor = routeFactor;
                                    }
                                    if (isEndTrailPoint(trailgps, currResultPoints.Count + 1) &&
                                        routeFactor > 0 || pointInfo.dist > matchDist + distHysteresis)
                                    {
                                        //Leaving center for last point - no more checks
                                        break;
                                    }
                                }
                            }

                            if (routeDist >= radius || routeIndex >= activity.GPSRoute.Count - 1)
                            {
                                if (routeDist >= radius)
                                {
                                    //No longer in radius, we have the best match
                                    //As we peeked on next, we have to set back the index
                                    routeIndex--;
                                    routeDist = prevRouteDist;
                                }

                                if (currResultPoints.Count == 0)
                                {
                                    foreach (pInfo p in info)
                                    {
                                        float routeFactor = -1;
                                        float closeDist = p.dist;
                                        if (routeIndex > 0)
                                        {
                                            routeFactor = p.checkPass(activity,
                                               trailgps[TrailIndex(trailgps, currResultPoints.Count)], radius, ref closeDist);
                                        }

                                        if (closeDist < matchDist)
                                        {
                                            //Better, still closing in
                                            matchIndex = p.index;
                                            matchDist = closeDist;
                                            matchFactor = routeFactor;
                                        }
                                        if (routeFactor > 0 || p.prevDist > matchDist + distHysteresis)
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
                    //Setting a limit here like (routeDist < 10*radius) will improve detection time very slightly
                    //This handling is very sensitive, especially for single point trails, the reason for the check
                    else if (trailgps.Count > 1 || currResultPoints.Count == 0 || routeIndex > currResultPoints[currResultPoints.Count - 1].matchPassBy)
                    {
                        double d;
                        float factor = checkPass(radius,
                            routePoint(activity, prevPoint.index), prevPoint.dist,
                            routePoint(activity, routeIndex), routeDist,
                            trailgps[TrailIndex(trailgps, currResultPoints.Count)],
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
                //Not meaningful for one point trails
                if (matchTime == null && routeIndex >= activity.GPSRoute.Count - 1 && trailgps.Count > 1)
                {
                    bool required = trailgps[TrailIndex(trailgps, currResultPoints.Count)].Required;

                    ///////////////////
                    //Last point check for non required points - automatic match, so search can restart
                    if (!required)
                    {
                        matchTime = DateTime.MinValue;  //automaticMatch = true;
                    }
                    else if (currRequiredMisses < MaxRequiredMisses)
                    {
                        matchTime = DateTime.MinValue;  //automaticMatch = true;
                        //OK to miss this point. Set automatic match to start looking at prev match
                        currRequiredMisses++;
                    }
                    if (matchTime != null && matchTime == DateTime.MinValue)
                    {
                        matchDist = radius * 2;
                    }
                }

                ////////////////////////////
                //Ignore short legs
                if (minDistance > 0 && currResultPoints.Count > 0 &&
                    matchTime != null && matchTime != DateTime.MinValue)
                {
                    int prevMatchIndex = getPrevMatchIndex(currResultPoints);
                    if (dTrack != null && prevMatchIndex >= 0 &&
                    minDistance > (dTrack[matchIndex].Value - dTrack[prevMatchIndex].Value))
                    {
                        matchTime = null;
                    }
                }

                /////////////////////////////////////
                //Add found match to result 
                if (matchTime != null)
                {
                    currResultPoints.Add(new TrailResultPointMeta(trailgps[TrailIndex(trailgps, currResultPoints.Count)],
                        (DateTime)matchTime,
                        matchIndex, lastMatchInRadiusIndex, lastMatchPassByIndex, matchDist));

                    //Clear cache, dist unknown to next point
                    prevPoint.index = -1;

                    //Lowest value for next start point, updated at OK matches
                    if (matchTime != DateTime.MinValue)
                    {
                        //Save latest match info, next match cannot be lower than this
                        prevActivityMatchIndex = Math.Max(prevActivityMatchIndex, getPrevMatchIndex(currResultPoints));

                        //At least one point match
                        status = BestCalcStatus(status, TrailOrderStatus.InBoundMatchPartial);
                    }

                    //Check if this is a partial or incomplete trail match
                    int noOfMatches = 0;
                    foreach (TrailResultPointMeta i in currResultPoints)
                    {
                        if (i.Time != DateTime.MinValue)
                        {
                            noOfMatches++;
                        }
                    }

                    //////////////////////////////
                    //End point
                    if (isEndTrailPoint(trailgps, currResultPoints.Count) ||
                        //Abort at a certain number of result points
                        (maxResultPoints > 0 && maxResultPoints >= noOfMatches))
                    {
                        //A result must have at least two matches, otherwise it is not possible to get distance etc
                        if (noOfMatches >= 2)
                        {
                            if (noOfMatches < currResultPoints.Count)
                            {
                                status = BestCalcStatus(status, TrailOrderStatus.MatchPartial);
                            }
                            else
                            {
                                status = BestCalcStatus(status, TrailOrderStatus.Match);
                                //OK to reset misses only at full match
                                currRequiredMisses = 0;
                            }

                            TrailResultInfo resultInfo = new TrailResultInfo(activity, reverse);
                            for (int i = 0; i < currResultPoints.Count; i++)
                            {
                                TrailResultPointMeta point = currResultPoints[i];
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
                            }
                            trailResults.Add(resultInfo);

                            //To avoid "detection loops", the potential start for next trail must be after 
                            //the first possible match for the first trail point
                            //There must be some matches in the current result points
                            prevActivityMatchIndex = Math.Max(prevActivityMatchIndex, getFirstMatchRadius(currResultPoints));

                            //Actually, prevActivityMatchIndex could be set to getFirstMatchRadius() to allow overlapping results
                            //Will decrease trail detection time slightly
                        }
                        else
                        {
                            //Match is incomplete
                            if (!reverse && noOfMatches >= 1)
                            {
                                //One point match: Only forward, reverse matches will be the same
                                IncompleteTrailResult result = new IncompleteTrailResult(activity, currResultPoints, reverse);
                                incompleteResults.Add(result);
                                //prevActivityMatchIndex = getPrevMatchIndex(currResultPoints);
                            }
                            if (maxResultPoints <= 0)
                            {
                                //Abort further matches
                                prevActivityMatchIndex = activity.GPSRoute.Count;
                            }
                        }

                        //Reset matches for trail detection
                        currResultPoints.Clear();
                    }
                    else
                    {
                        //Intermediate point
                        if (trailgps.Count == 1)
                        {
                            //For one point trail, the same applies to the second match as for the end point
                            prevActivityMatchIndex = Math.Max(prevActivityMatchIndex, getFirstMatchRadius(currResultPoints));
                        }
                        else
                        {
                            //Next match may be in the same radius
                            routeIndex = matchIndex;
                        }
                    }
                    //If this was an automatic match, set back the routeIndex to last good match
                    if (matchTime == DateTime.MinValue)
                    {
                        routeIndex = prevActivityMatchIndex;
                    }
                }
                else
                {
                    //Cache previous values, used in passed-by checks for this trail point
                    prevPoint.index = routeIndex;
                    prevPoint.dist = routeDist;
                }

//TODO remove - need for restart handling?
#if OLD_TRAIL_RESTART
                ////////////////////////////////////
                //Determine where to start from
                //At matches, the index is determined from type of match
                //The normal case at no match is to continue with next routePoint
                //At end points, non, the match may restart
                if ((matchTime == null /*|| matchTime == DateTime.MinValue*/) &&
                    routeIndex >= activity.GPSRoute.Count - 1 &&
                         currResultPoints.Count > 0)// &&
                        //!currResultPoints[currResultPoints.Count - 1].restart) 
                {
                    ////////////////////////////////////
                    //We have reached the end without a match
                    //If there are non-required previous, try dropping them and continue

                    //Last req index that match for this activity
                    bool matchNoReqToIgnore = false;
                    for (int i = currResultPoints.Count - 1; i >= 0; i--)
                    {
                        if (!currResultPoints[i].restart && trailgps[TrailIndex(trailgps, i)].Required &&
                            currResultPoints[i].index >= 0)
                        {
                            //prevActivityMatchIndex = currResultPoints[i].index;
                            //reset routeIndex to last real match
                            routeIndex = prevActivityMatchIndex;
                            break;
                        }
                        else if (!currResultPoints[i].restart)
                        {
                            //Hide the non-required point
                            currResultPoints[i].restart = true;
                        }
                    }
                    if (matchNoReqToIgnore)
                    {
                        routeIndex = prevReqMatchIndex;
                        prevPoint.index = -1;
                    }
                }
#endif
                //Route index cannot be lower than latest match
                routeIndex = Math.Max(routeIndex, prevActivityMatchIndex);

                ///////////////////////////////////////
            } //foreach gps point

            ///////////////////////////////////////
            //Possible last incomple result
            {
                //InBoundMatchPartial updated for all incomplete results
                int noOfMatches = 0;
                foreach (TrailResultPointMeta i in currResultPoints)
                {
                    if (i.Time != DateTime.MinValue)
                    {
                        noOfMatches++;
                    }
                }

                if (noOfMatches > 0 && !reverse || noOfMatches > 1)
                {
                    IncompleteTrailResult incompleteResult = new IncompleteTrailResult(activity, currResultPoints, reverse);
                    incompleteResults.Add(incompleteResult);
                }
            }

            return status;
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

        private static int getFirstMatchRadius(IList<TrailResultPointMeta> resultPoints)
        {
            int currMatches = resultPoints.Count;
            int prevMatchIndex = -1;
            for (int i = 0; i < currMatches; i++)
            {
                if (!resultPoints[i].restart && resultPoints[i].index > -1 && resultPoints[i].Time > DateTime.MinValue)
                {
                    prevMatchIndex = Math.Max(resultPoints[i].matchInRadius, resultPoints[i].matchPassBy);
                    if (prevMatchIndex >= 0)
                    {
                        break;
                    }
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
                    if (TrailResultWrapper.ParentResults(this.ResultTreeList).Contains(tr.Result))
                    {
                        TrailResultWrapper.ParentResults(this.ResultTreeList).Remove(tr.Result);
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
                    TrailResultInfo resultInfo = new TrailResultInfo(t.Activity, t.Reverse);
                    for (int i = 0; i < t.Points.Count; i++)
                    {
                        TrailResultPointMeta point = t.Points[i];
                        //For incomplete, do not care if restart points were used or not
                        resultInfo.Points.Add(point);
                    }
                    //Set remaining points
                    for (int i = t.Points.Count; i < m_trail.TrailLocations.Count; i++)
                    {
                        resultInfo.Points.Add(new TrailResultPoint(m_trail.TrailLocations[i], DateTime.MinValue, TrailResultPoint.DiffDistMax));
                    }
                    TrailResult tr = new TrailResult(this, m_resultsListWrapper.Count + 1,
                        resultInfo, resultInfo.DistDiff, t.Reverse);
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
                    foreach (Data.TrailResult tr in TrailResultWrapper.ParentResults(this.ResultTreeList))
                    {
                        sortValue += tr.DistDiff;
                    }
                    sortValue = sortValue / (float)Math.Pow(TrailResultWrapper.ParentResults(this.ResultTreeList).Count, 1.5);

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
            if (this.Status != to2.Status)
            {
                return this.Status > to2.Status ? 1 : -1;
            }
            else if (this.Status == TrailOrderStatus.Match)
            {
                if (this.Trail.TrailType != to2.Trail.TrailType)
                {
                    return (this.Trail.TrailType > to2.Trail.TrailType) ? 1 : -1;
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
            else if (this.Status == TrailOrderStatus.MatchNoCalc)
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
                else
                {
                    return (this.Trail.TrailType > to2.Trail.TrailType) ? 1 : -1;
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
                this.Points = new List<TrailResultPointMeta>();
                foreach (TrailResultPointMeta t in Points)
                {
                    this.Points.Add(t);
                }
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
            public TrailResultPointMeta(TrailGPSLocation trailLoc, DateTime time,
                int index, int lastMatchInRadius, int lastMatchPassBy, float diffDist) :
                base(trailLoc, time, diffDist)
            {
                this.index = index;
                this.matchInRadius = lastMatchInRadius;
                this.matchPassBy = lastMatchPassBy;
                this.restart = false;
            }
            //indexes for this match
            public int index;
            public int matchInRadius;
            public int matchPassBy;
            //a point may be used to "restart" match once
            //TODO: No longer used?
            public bool restart;
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
                name += " (" + TrailResultWrapper.ParentResults(t.ResultTreeList).Count + ")";
            }
            else if (t.Status == TrailOrderStatus.MatchNoCalc)
            {
                if (t.Trail.TrailType == Trail.CalcType.Splits)
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
        //MatchPartial is when all match, but some with "wildcards"
        //InBoundMatchPartial is when there is at least one match, but not for all points
        Match, MatchPartial, MatchNoCalc, InBoundMatchPartial, InBoundNoCalc, InBound, NotInBound, NoInfo, NotInstalled
    }
}
