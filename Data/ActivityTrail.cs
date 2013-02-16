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
//Use simpler distance calculations in trail calculations, speeds up. Calculations are relative anyway.
#define SIMPLE_DISTANCE
//Instead of real distances, use "scaled squared" distance calculations. This requires that compare values and factors are adjusted too.
//The effect on performance is minimal though.
//#define SQUARE_DISTANCE

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

        private IList<IncompleteTrailResult> m_incompleteResults;
        //Counter for "no results"
        public IDictionary<TrailOrderStatus, int> m_noResCount = new Dictionary<TrailOrderStatus, int>();
        private IList<IActivity> m_inBound = new List<IActivity>();
        private bool m_canAddInbound = true;

        public ActivityTrail(Controller.TrailController controller, Data.Trail trail)
        {
            this.m_controller = controller;
            this.m_trail = trail;

            this.Init();
        }

        public void Init()
        {
            this.m_resultsListWrapper = null;
            this.m_incompleteResults = null;

            this.m_inBound.Clear();
            this.m_status = TrailOrderStatus.NoInfo;

            if (this.m_trail.Generated && !this.m_trail.IsReference)
            {
                this.m_canAddInbound = false;
            }
            else 
            {
                this.m_canAddInbound = true;
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
            else if (this.m_trail.TrailType == Trail.CalcType.Splits)
            {
                //By default, always match
                this.m_status = TrailOrderStatus.MatchNoCalc;
            }
            else if (this.m_trail.IsReference)
            {
                if (this.m_trail.ReferenceActivity != null && this.m_trail.ReferenceActivity.GPSRoute != null)
                {
                    // Let Reference always match, to trigger possible recalc after
                    this.m_status = TrailOrderStatus.MatchNoCalc;
                }
                else
                {
                    this.m_status = TrailOrderStatus.NotInBound;
                }
            }
            else if (this.m_trail.TrailLocations.Count == 0)
            {
                this.m_status = TrailOrderStatus.NotInBound;
            }
        }

        //Clear all calculated data for the result, do not affect trail calculations
        public void Clear(bool onlyDisplay)
        {
            if (this.m_resultsListWrapper != null)
            {
                foreach (TrailResultWrapper t in this.m_resultsListWrapper)
                {
                    t.Result.Clear(onlyDisplay);
                }
            }
        }

        public Data.Trail Trail
        {
            get
            {
                return m_trail;
            }
            set
            {
                m_trail = value;
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
                    this.m_inBound = this.m_trail.InBoundActivities(m_controller.Activities);
                    if (this.m_inBound.Count > 0 || 
                        m_trail.IsReference && m_trail.ReferenceActivity == null)
                    {
                        //Do not downgrade MatchNoCalc here
                        this.Status = TrailOrderStatus.InBoundNoCalc;
                        this.m_noResCount[this.Status] = this.m_inBound.Count;
                    }
                    else
                    {
                        //Downgrade status
                        this.Status = TrailOrderStatus.NotInBound;
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

        public IList<TrailResultWrapper> ResultTreeList
        {
            get
            {
                return ResultTreeListRows(null);
            }
        }

        public IList<TrailResultWrapper> ResultTreeListRows(System.Windows.Forms.ProgressBar progressBar)
        {
            this.CalcResults(progressBar);
            return this.m_resultsListWrapper;
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
            foreach (IActivity activity in m_inBound)
            {
                //No GPS check: Must have been done when adding to inbound list
                TrailResultWrapper result = new TrailResultWrapper(this, activity, m_resultsListWrapper.Count + 1);
                m_resultsListWrapper.Add(result);
                //add children
                result.getSplits();
                //Note: status is not updated, cannot be better
            }
            //No progress update
        }

        public void CalcResults(System.Windows.Forms.ProgressBar progressBar)
        {
            CalcResults(m_controller.Activities, m_trail.MaxRequiredMisses, m_trail.BiDirectional, progressBar);
        }

        public void CalcResults(IList<IActivity> activities, int MaxAllowedMisses, bool bidirectional, System.Windows.Forms.ProgressBar progressBar)
        {
            if (m_resultsListWrapper == null)
            {
                //Avoid calculaations with only one null activity
                //(assume this is a race condition, do not set m_resultsListWrapper)
                if (activities == null || activities.Count == 1 && activities[0] == null)
                {
                    return;
                }

                m_resultsListWrapper = new List<TrailResultWrapper>();
                m_incompleteResults = new List<IncompleteTrailResult>();

                //Calculation depends on TrailType
                if (m_trail.TrailType == Trail.CalcType.HighScore)
                {
                    if (Integration.HighScore.HighScoreIntegrationEnabled)
                    {
                        //Save values modified by HighScore
                        bool visible = false;
                        int HighScoreProgressVal = 0;
                        int HighScoreProgressMax = 0;
                        if (null != progressBar)
                        {
                            //Set by HighScore before 2.0.327
                            visible = progressBar.Visible;
                            HighScoreProgressVal = progressBar.Value;
                            HighScoreProgressMax = progressBar.Maximum;
                        }
                        IList<Integration.HighScore.HighScoreResult> hs = Integration.HighScore.GetHighScoreForActivity(activities, 10, null/*progressBar*/);
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
                            progressBar.Visible = visible;
                            progressBar.Maximum = HighScoreProgressMax;
                            if (progressBar.Value + 1 < HighScoreProgressVal)
                            {
                                progressBar.Value = HighScoreProgressVal + 1;
                            }
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
                        MaxAllowedMisses = Math.Min(trailgps.Count - noNonReq, MaxAllowedMisses);
                    }
                    //Calculate InBound information if not already done
                    if (this.Status != TrailOrderStatus.InBoundNoCalc)
                    {
                        bool tmp = this.IsInBounds;
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
                                bool inBound = this.m_inBound.Contains(activity);

                                //TODO: optimize prune (Intersect()) if MaxReq is set, to see that at least one point matches
                                //As this is currently used when adding is used only for EditTrail, no concern
                                if ((inBound || MaxAllowedMisses > 0) && activity.GPSRoute != null)
                                {
                                    //Status is at least inbound (even if this is a "downgrade")
                                    if (m_status == TrailOrderStatus.InBoundNoCalc)
                                    {
                                        m_status = TrailOrderStatus.InBound;
                                    }
                                    activityStatus = CalcInboundResults(activity, trailgps, locationBounds, MaxAllowedMisses, false, progressBar);
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
                                        activityStatus = CalcInboundResults(activity, trailgpsReverse, locationBoundsReverse, MaxAllowedMisses, true, progressBar);
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

            if (null != progressBar && progressBar.Value < progressBar.Maximum)
            {
                progressBar.Value++;
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
                float dist;
                float routeFactor = ActivityTrail.checkPass(radius,
                  activity.GPSRoute[this.index - 1].Value, this.prevDist,
                  activity.GPSRoute[this.index].Value, this.dist,
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
            TrailOrderStatus status = CalcTrail(activity, trailgps, locationBounds, this.Trail.Radius, this.Trail.MinDistance, MaxRequiredMisses, reverse, 0, this.Trail.IsCompleteActivity, trailResults, m_incompleteResults, progressBar);

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
            IList<TrailGPSLocation> trailgps = Trail.TrailGpsPointsFromGps(new List<IGPSLocation> { gps });
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
                radius, 0, 0, false, maxPoints, false, trailResults, incompleteResults, null);
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
                    radius, 0, 0, true, maxPoints, false, trailResults, incompleteResults, null);
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
            float radius, float minDistance, int MaxRequiredMisses, bool reverse, int maxResultPoints, bool isComplete,
            IList<TrailResultInfo> trailResults, IList<IncompleteTrailResult> incompleteResults, System.Windows.Forms.ProgressBar progressBar)
        {
            TrailOrderStatus status = TrailOrderStatus.NoInfo;
            IList<TrailResultPointMeta> currResultPoints = new List<TrailResultPointMeta>();

            //Cache information about previous distances
            PointInfo prevPoint = new PointInfo(-1, 0);
            PointInfo prevStartPoint = new PointInfo(-1, 0);
            int prevMatchIndex = -1; //Next match cannot be lower than this

#if SQUARE_DISTANCE
            //Use "squared" distance internally, to speed up calculations
            //Factors are not completely independent, but minimal effect
            radius = (TrailGPSLocation.DistanceToSquareScaling * radius) * (TrailGPSLocation.DistanceToSquareScaling * radius);
            float distHysteresisMin = (TrailGPSLocation.DistanceToSquareScaling * 5) * (TrailGPSLocation.DistanceToSquareScaling * 5);
            const float distHysteresisFactor = 30*30;
            const float radiusFactor = 2*2;
            const float passByFactor = 10*10;
#else
            const float distHysteresisMin = 5;
            const float distHysteresisFactor = 30;
            const float radiusFactor = 2;
            const float passByFactor = 10;
#endif
            //Required points misses - undocumented feature
            int currRequiredMisses = 0;
            //Ignore short legs - undocumented feature
            IDistanceDataTrack dTrack = null;
            if (minDistance > 0)
            {
                //Must use the distance track related to the GPS track here, not the (potentially optimized) InfoCache track
                dTrack = activity.GPSRoute.GetDistanceMetersTrack();
            }

            int trailGpsIndex = 0;
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
                    currResultPoints[0].Time > DateTime.MinValue &&
                    routeIndex > (1 + Math.Max(currResultPoints[0].index, Math.Max(currResultPoints[0].matchInRadius,
                    currResultPoints[0].matchPassBy))) &&
                    locationBounds[0].Contains((IGPSLocation)(activity.GPSRoute[routeIndex].Value)))
                {
                    PointInfo startPoint = new PointInfo(routeIndex,
                         distanceTrailToRoute(trailgps[0], activity, routeIndex));
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
                            prevStartPoint.dist = distanceTrailToRoute(trailgps[0], activity, prevStartPoint.index);
                        }
                        float d;
                        if (startPoint.dist < radius * passByFactor && 
                            0 < checkPass(radius,
                    activity.GPSRoute[prevStartPoint.index].Value, prevStartPoint.dist,
                    activity.GPSRoute[startPoint.index].Value, startPoint.dist,
                    trailgps[0]/*, dTrack[routeIndex].Value - dTrack[prevStartPointIndex].Value*/,
                    out d))
                        {
                            match = true;
                        }
                    }
                    if (match)
                    {
                        //Start over if we pass first point before all were found
                        currResultPoints.Clear();
                        trailGpsIndex = 0;
                    }
                    prevStartPoint = startPoint;
                }

                /////////////////////////////////////
                //try matching

                float routeDist = float.MaxValue;
                TrailResultPointMeta matchPoint = null;

                //Check if the point is in bounds, the distance to point is the heaviest calculation per point
                //location bounds is aproximate, should cover the aproximate "checkPass" area
                if (locationBounds[trailGpsIndex].Contains((IGPSLocation)(activity.GPSRoute[routeIndex].Value)))
                {
                    routeDist = distanceTrailToRoute(trailgps[trailGpsIndex], activity, routeIndex);
                    if (routeIndex > 0 && (prevPoint.index < 0 || routeIndex - prevPoint.index != 1 || prevPoint.dist == float.MaxValue))
                    {
                        prevPoint.index = routeIndex - 1;
                        prevPoint.dist = distanceTrailToRoute(trailgps[trailGpsIndex], activity, prevPoint.index);
                    }

                    //////////////////////////////////////
                    //Find the best GPS point for this result
                    if (routeDist < radius)
                    {
                        bool firstPoint = true;
                        float prevRouteDist = prevPoint.dist;
                        float distHysteresis = Math.Max(radius / distHysteresisFactor, distHysteresisMin);//Used as const

                        float matchFactor = -1;
                        float matchDist = float.MaxValue; //Set to max, to update correctly for first point
                        int matchIndex = routeIndex;

                        Stack<pInfo> info = new Stack<pInfo>();

                        //routeIndex is a match, but points following may be better
                        //Go through the points while we are in the radius or at last point
                        //(for last point we break, next may follow directly) 
                        while (true)
                        {
                            if (!firstPoint)
                            {
                                //avoid calculations...
                                routeDist = distanceTrailToRoute(trailgps[trailGpsIndex], activity, routeIndex);
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
                                    //Middle point: Try until out of radius
                                    //End point: Abort when leaving center
                                    float routeFactor = -1;
                                    float closeDist = pointInfo.dist;
                                    if (routeIndex > 0)
                                    {
                                        //Check closest point
                                        routeFactor = pointInfo.checkPass(activity,
                                           trailgps[trailGpsIndex], radius, ref closeDist);
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
                                    //As we peeked on next, we have to set back the index (as last point in radius is saved)
                                    routeIndex--;
                                    routeDist = prevRouteDist;
                                }

                                //First point: Go through backward, similar to endpoint
                                if (currResultPoints.Count == 0)
                                {
                                    foreach (pInfo p in info)
                                    {
                                        float routeFactor = -1;
                                        float closeDist = p.dist;
                                        if (p.index > 0)
                                        {
                                            routeFactor = p.checkPass(activity,
                                               trailgps[trailGpsIndex], radius, ref closeDist);
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

                        DateTime matchTime = activity.GPSRoute.EntryDateTime(activity.GPSRoute[matchIndex]);
                        if (matchFactor > 0)
                        {
                            DateTime d1 = activity.GPSRoute.EntryDateTime(activity.GPSRoute[matchIndex - 1]);
                            //matchIndex is always "next" point, adjust
                            matchTime = d1.Add(TimeSpan.FromSeconds(matchFactor * (matchTime - d1).TotalSeconds));
                            if (matchFactor < 0.5)
                            {
                                matchIndex--;
                            }
                            //Use the first as last match - next may match after
                            prevMatchIndex = matchIndex - 1;
                        }
                        else
                        {
                            prevMatchIndex = matchIndex;
                        }
                        matchPoint = new TrailResultPointMeta(trailgps[trailGpsIndex], matchTime,
                            matchIndex, routeIndex, routeIndex + 1, matchDist);
                    } //if (dist < radius)

                    ///////////
                    //Check for pass-by
                    //This handling is very sensitive for single point trails, extra check required
                    else if (routeDist < radius * passByFactor && prevPoint.index >= 0 &&
                        (trailgps.Count > 1 || currResultPoints.Count == 0 || routeIndex > currResultPoints[currResultPoints.Count - 1].matchPassBy))
                    {
                        float d;
                        float factor = checkPass(radius,
                            activity.GPSRoute[prevPoint.index].Value, prevPoint.dist,
                            activity.GPSRoute[routeIndex].Value, routeDist,
                            trailgps[trailGpsIndex],
                            out d);
                        if (0 < factor)
                        {
                            DateTime d1 = activity.GPSRoute.EntryDateTime(activity.GPSRoute[prevPoint.index]);
                            DateTime d2 = activity.GPSRoute.EntryDateTime(activity.GPSRoute[routeIndex]);
                            DateTime matchTime = d1.Add(TimeSpan.FromSeconds(factor * (d2 - d1).TotalSeconds));

                            //if setting matchIndex to prev point, following matches can match but give loop issues
                            matchPoint = new TrailResultPointMeta(trailgps[trailGpsIndex], matchTime,
                                routeIndex, -1, routeIndex, (float)d);
                            prevMatchIndex = routeIndex-1;
                        }
                    }
                }

                //////////////////////////////
                //All GPS points tested but search should maybe match
                //Not meaningful for one point trails
                if (matchPoint == null && routeIndex >= (activity.GPSRoute.Count - 1) && trailgps.Count > 1)
                {
                    bool required = trailgps[trailGpsIndex].Required;

                    ///////////////////
                    //Last point check for non required points - automatic match, so search can restart
                    if (!required)
                    {
                        matchPoint = new TrailResultPointMeta(trailgps[trailGpsIndex], DateTime.MinValue,
                            -1, -1, -1, radius * radiusFactor);
                    }
                    else if (currRequiredMisses < MaxRequiredMisses)
                    {
                        //OK to miss this point. Set automatic match to start looking at prev match
                        currRequiredMisses++;
                        matchPoint = new TrailResultPointMeta(trailgps[trailGpsIndex], DateTime.MinValue,
                            -1, -1, -1, radius * radiusFactor);
                    }
                }

                ////////////////////////////
                //Ignore short legs
                if (minDistance > 0 && currResultPoints.Count > 0 &&
                    matchPoint != null && matchPoint.Time != DateTime.MinValue)
                {
                    if (dTrack != null && prevMatchIndex >= 0 &&
                      minDistance > (dTrack[matchPoint.index].Value - dTrack[prevMatchIndex].Value))
                    {
                        matchPoint = null;
                    }
                }

                /////////////////////////////////////
                //Add found match to result 
                if (matchPoint != null)
                {
                    currResultPoints.Add(matchPoint);
                    trailGpsIndex = trailgps.Count == 1 ? 0 : currResultPoints.Count;

                    //Lowest value for next start point, updated at OK matches
                    if (matchPoint.Time != DateTime.MinValue)
                    {
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
                                //if (point.restart)
                                //Something like the following could be used to recover previous points, but 
                                //i+1 must be the first required point with one non required in between (the last non required is always "bad")
                                //i < resultPoints.Count - 1 && !resultPoints[i + 1].restart &&
                                //point.Time > DateTime.MinValue && point.Time <= resultPoints[i + 1].Time)
                                //{
                                //    point.Time = DateTime.MinValue;
                                //}
                                resultInfo.Points.Add(point);
                            }
                            if (isComplete)
                            {
                                int i = 0;
                                TrailResultPointMeta p = new TrailResultPointMeta(new TrailGPSLocation(activity.GPSRoute[i].Value), activity.GPSRoute.StartTime, i, i, i, 0);
                                resultInfo.Points.Insert(0, p);

                                i = activity.GPSRoute.Count - 1;
                                p = new TrailResultPointMeta(new TrailGPSLocation(activity.GPSRoute[i].Value), activity.GPSRoute.EntryDateTime(activity.GPSRoute[i]), i, i, i, 0);
                                resultInfo.Points.Add(p);

                                //Note: No update of prevMatchIndex, allow multiple loops
                            }
                            trailResults.Add(resultInfo);

                            //To avoid "detection loops", the potential start for next trail must be after 
                            //the first possible match for the first trail point
                            //There must be some matches in the current result points
                            prevMatchIndex = Math.Max(prevMatchIndex, getFirstMatchRadius(currResultPoints));

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
                            }
                            if (maxResultPoints <= 0)
                            {
                                //No match, abort further matches
                                prevMatchIndex = activity.GPSRoute.Count;
                            }
                        }

                        //Reset matches for trail detection
                        currResultPoints.Clear();
                        trailGpsIndex = 0;
                    }
                    else
                    {
                        //Intermediate point
                        if (trailgps.Count == 1)
                        {
                            //For one point trail, the same applies to the first match as for the end point (see above)
                            prevMatchIndex = Math.Max(prevMatchIndex, getFirstMatchRadius(currResultPoints));
                        }
                    }

                    //Next match must at least be at last OK match (but may be in same radius)
                    //(If this was an automatic match (-1), set back the routeIndex)
                    routeIndex = prevMatchIndex;

                    //Clear cache, dist unknown to next point
                    prevPoint.index = -1;
                }
                else
                {
                    //Cache previous values, used in passed-by checks for this trail point
                    prevPoint.index = routeIndex;
                    prevPoint.dist = routeDist;
                }

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
        //See TrailGPSLocation for details
        //This improves trail detection time with about 10%, but has currently some differences
        //In additition, the standard distance is useful for debugging 
 
        private static int getFirstMatchRadius(IList<TrailResultPointMeta> resultPoints)
        {
            int currMatches = resultPoints.Count;
            int prevMatchIndex = -1;
            for (int i = 0; i < currMatches; i++)
            {
                if (resultPoints[i].index > -1 && resultPoints[i].Time > DateTime.MinValue)
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

        private static float checkPass(float radius, IGPSPoint r1, float dt1, IGPSPoint r2, float dt2, TrailGPSLocation trailp, out float d)
        {
#if SQUARE_DISTANCE
            const int sqrt2 = 2;
#else
            //Optimise, accuracy is down to percent
            const float sqrt2 = 1.4142135623730950488016887242097F;
#endif
            d = float.MaxValue;
            float factor = -1;
            if (r1 == null || r2 == null || trailp == null) return factor;

            //Check if the line goes via the "circle" if the sign changes
            //Also need to check close points that fit in a 45 deg tilted "square" where sign may not change

            //Optimise for all conditions tested - property access takes some time
            float tplat = trailp.LatitudeDegrees;
            float tplon = trailp.LongitudeDegrees;
            float r1lat = r1.LatitudeDegrees;
            float r1lon = r1.LongitudeDegrees;
            float r2lat = r2.LatitudeDegrees;
            float r2lon = r2.LongitudeDegrees;
            if (r1lat > tplat && r2lat < tplat ||
                r1lat < tplat && r2lat > tplat ||
                r1lon > tplon && r2lon < tplon ||
                r1lon < tplon && r2lon > tplon ||
                dt1 < radius * sqrt2 && dt2 < radius * sqrt2)
            {
                //Law of cosines - get a1, angle at r1, the first point
#if SIMPLE_DISTANCE
                float d12 = TrailGPSLocation.DistanceMetersToPointGpsSimple(r1, r2);
#else
                float d12 = r1.DistanceMetersToPoint(r2);
#endif
#if SQUARE_DISTANCE
                float a10 = (float)((dt1 + d12 - dt2) / (2 * Math.Sqrt(dt1 * d12)));
#else
                float a10 = (dt1 * dt1 + d12 * d12 - dt2 * dt2) / (2 * dt1 * d12);
#endif

                //Point is in circle if closest point is between r1&r2 and it is in circle (neither r1 nor r2 is)
                //This means the angle a1 must be +/- 90 degrees : cos(a1)>=0
                if (a10 > -0.001)
                {
                    //Rounding errors w GPS measurements
                    a10 = Math.Min(a10, 1);
                    a10 = Math.Max(a10, -1);
                    float a1 = (float)Math.Acos(a10);
                    //Dist from r1 to closest point on d1-d2 (called x)
                    float d1x = (float)Math.Abs(dt1 * Math.Cos(a1));
                    //Dist from t1 to x on line between d1-d2
                    float dtx = dt1 * (float)Math.Sin(a1);
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

        private static float distanceTrailToRoute(TrailGPSLocation t, IActivity activity, int routeIndex)
        {
#if SIMPLE_DISTANCE
            return TrailGPSLocation.DistanceMetersToPointSimple(t, activity.GPSRoute[routeIndex].Value);
#else
            return t.DistanceMetersToPoint(activity.GPSRoute[routeIndex].Value);
#endif
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
                    if (TrailResultWrapper.Results(this.ResultTreeList).Contains(tr.Result))
                    {
                        TrailResultWrapper.Results(this.ResultTreeList).Remove(tr.Result);
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
                    foreach (Data.TrailResult tr in TrailResultWrapper.Results(this.ResultTreeList))
                    {
                        sortValue += tr.SortQuality;
                    }
                    sortValue = sortValue / (float)Math.Pow(TrailResultWrapper.Results(this.ResultTreeList).Count, 1.5);

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
            else if (this.Status == TrailOrderStatus.InBoundNoCalc)
            {
                return this.m_inBound.Count < to2.m_inBound.Count ? 1 : -1;
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
                    (this.Trail.TrailLocations.Count == 1 || to2.Trail.TrailLocations.Count == 1))
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
            }
            //indexes for this match
            public int index;
            public int matchInRadius;
            public int matchPassBy;
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
            if (t.Trail.IsReference && null != t.Trail.ReferenceActivity)
            {
                DateTime time = ActivityInfoCache.Instance.GetInfo(t.Trail.ReferenceActivity).ActualTrackStart;
                if (DateTime.MinValue == time)
                {
                    time = t.Trail.ReferenceActivity.StartTime;
                }
                name += " " + time.ToLocalTime().ToString();
            }

            if (t.Status == TrailOrderStatus.Match ||
                t.Status == TrailOrderStatus.MatchPartial)
            {
                name += " (" + TrailResultWrapper.Results(t.ResultTreeList).Count + ")";
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
                //Other results
            else if (t.m_noResCount.ContainsKey(t.Status))
            {
                name += " (" + t.m_noResCount[t.Status] + ")";
            }
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
