﻿/*
Copyright (C) 2009 Brendan Doherty
Copyright (C) 2010-2014 Gerhard Olsson

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
using System.Windows.Forms;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;

namespace TrailsPlugin.Data
{
    public class ActivityTrail : IComparable
    {
        private Trail m_trail;
        private IList<TrailResultWrapper> m_results = null;
        private IList<TrailResultWrapper> m_defResults = null;
        private TrailOrderStatus m_status;

        private IList<IncompleteTrailResult> m_incompleteResults;
        private IList<TrailResultWrapper> m_filteredResults;
        //Counter for "no results"
        public IDictionary<TrailOrderStatus, int> m_noResCount = new Dictionary<TrailOrderStatus, int>();
        private IList<IActivity> m_inBound = new List<IActivity>();
        private bool m_canAddInbound = true;

        public ActivityTrail(Trail trail)
        {
            this.m_trail = trail;

            this.Init();
        }

        public void Init()
        {
            this.m_results = null;
            this.m_defResults = null;
            m_incompleteResults = new List<IncompleteTrailResult>();
            m_filteredResults = new List<TrailResultWrapper>();

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
            else if (this.m_trail.TrailType == Trail.CalcType.UniqueRoutes)
            {
                if (Integration.UniqueRoutes.UniqueRouteIntegrationEnabled)
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
            else if (this.m_trail.TrailType == Trail.CalcType.ElevationPoints)
            {
                //By default, never match
                this.m_status = TrailOrderStatus.NotInstalled;
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
            else if (this.m_trail.TrailLocations.Count == 0 || this.m_trail.Radius <= 0)
            {
                this.m_status = TrailOrderStatus.NoConfiguration;
            }
        }

        //Clear all calculated data for the result, do not affect trail calculations
        public void Clear(bool onlyDisplay)
        {
            if (this.m_results != null)
            {
                foreach (TrailResultWrapper t in this.m_results)
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
                TrailOrderStatus res = m_status;
                foreach (Trail t in this.Trail.AllChildren)
                {
                    ActivityTrail at = Controller.TrailController.Instance.GetActivityTrail(t);
                    if (at != null)
                    {
                        res = BestCalcStatus(res, at.Status);
                    }
                    else
                    {
                        System.Diagnostics.Debug.Assert(false, "Unexpectedly no ActivityTrail for " + t + " in " + this.Trail);
                    }
                }
                return res;
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
                    this.m_inBound = this.m_trail.InBoundActivities(Controller.TrailController.Instance.Activities);
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

        //The results for the normal, not including references
        public IList<TrailResultWrapper> Results
        {
            get
            {
                this.CalcResults(null);
                return this.m_results;
            }
        }

        public IList<TrailResultWrapper> DisplayResults
        {
            get
            {
                //First add the (possible) results related to the default activity
                IList<TrailResultWrapper> displayResults = new List<TrailResultWrapper>();
                if (this.m_defResults == null)
                {
                    this.m_defResults = new List<TrailResultWrapper>();
                    if (this.Trail.DefaultRefActivity != null &&
                        !Controller.TrailController.Instance.Activities.Contains(this.Trail.DefaultRefActivity))
                    {
                        foreach (TrailResultWrapper tw2 in this.CalcTrailCompleteResult(this.Trail.DefaultRefActivity))
                        {
                            this.m_defResults.Add(tw2);
                        }
                    }
                }
                foreach(TrailResultWrapper trw in this.m_defResults)
                {
                    displayResults.Add(trw);
                }

                //The "normal" results
                foreach (TrailResultWrapper trw in this.Results)
                {
                    displayResults.Add(trw);
                }
                return displayResults;
            }
        }

        public void ReAdd(TrailResultWrapper t)
        {
            bool found = false;
            if (this.m_results == null)
            {
                this.m_results = new List<TrailResultWrapper>();
            }
            foreach (TrailResultWrapper t2 in this.m_results)
            {
                if (t.Result.CompareTo(t2.Result) == 0)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                this.m_results.Add(t);
                //We do not know initial status, assume match...
                this.Status = TrailOrderStatus.Match;
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
            foreach (IActivity activity in m_inBound)
            {
                //No GPS check: Must have been done when adding to inbound list
                TrailResultInfo splitIndexes = Data.Trail.TrailResultInfoFromSplits(activity, false);
                TrailResultWrapper result = new TrailResultWrapper(new SplitsParentTrailResult(this, m_results.Count + 1, splitIndexes));
                m_results.Add(result);
                //Note: status is not updated, cannot be better
            }
            //No progress update
        }

        private IList<IActivity> ResultActivities
        {
            get
            {
                IList<IActivity> activities = new List<IActivity>();
                if (m_results != null)
                {
                    foreach (TrailResultWrapper trw in m_results)
                    {
                        IActivity act = trw.Result.Activity;
                        if (!activities.Contains(act))
                        {
                            activities.Add(act);
                        }
                    }
                }
                return activities;
            }
        }

        public void CalcResults(System.Windows.Forms.ProgressBar progressBar)
        {
            CalcResults(Controller.TrailController.Instance.Activities, m_trail.MaxRequiredMisses, m_trail.BiDirectional, progressBar);
        }

        public void CalcResults(IList<IActivity> activities, int MaxAllowedMisses, bool bidirectional, System.Windows.Forms.ProgressBar progressBar)
        {
            if (m_results == null)
            {
                //Avoid calculations with only one null activity
                //(assume this is a race condition, do not set m_resultsListWrapper)
                if (activities == null || activities.Count == 1 && activities[0] == null)
                {
                    return;
                }

                m_results = new List<TrailResultWrapper>();

                //Calculation depends on TrailType
                if (m_trail.TrailType == Trail.CalcType.HighScore)
                {
                    CalcHighScoreResults(activities, MaxAllowedMisses, bidirectional, progressBar);
                }
                else if (m_trail.TrailType == Trail.CalcType.UniqueRoutes)
                {
                    CalcUniqueRoutesResults(activities, MaxAllowedMisses, bidirectional, progressBar);
                }
                else if (this.m_trail.TrailType == Trail.CalcType.Splits)
                {
                    CalcSplitsResults(activities, MaxAllowedMisses, bidirectional, progressBar);
                }
                else //PositionTrails
                {
                    CalcPositionResults(activities, MaxAllowedMisses, bidirectional, progressBar);
                }

                if (this.m_trail.IsURFilter &&
        Controller.TrailController.Instance.ReferenceResult != null && Controller.TrailController.Instance.ReferenceResult.Result.GPSRoute != null)
                {
                    this.FilterURRouteSnippet(Controller.TrailController.Instance.ReferenceResult.Result.GPSRoute, progressBar);
                }

                if (m_results.Count == 0 && m_status < TrailOrderStatus.InBound && this.IsNoCalc)
                {
                    //Downgrade status from "speculative match"
                    this.m_status = TrailOrderStatus.InBound;
                }
            }

            if (null != progressBar && progressBar.Value < progressBar.Maximum)
            {
                progressBar.Value++;
            }
        }

        private void CalcHighScoreResults(IList<IActivity> activities, int MaxAllowedMisses, bool bidirectional, System.Windows.Forms.ProgressBar progressBar)
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
                    TrailResultWrapper parentWrapper = null;
                    foreach (Integration.HighScore.HighScoreResult h in hs)
                    {
                        TrailResultInfo indexes = Data.Trail.ResultInfoFromSelection(h.activity, h.selInfo);
                        if (indexes.Count > 1)
                        {
                            if (h.order == 1 || parentWrapper == null)
                            {
                                this.Status = TrailOrderStatus.Match;
                                HighScoreParentTrailResult parentResult = new HighScoreParentTrailResult(this, 1, indexes, h.tooltip);
                                parentWrapper = new TrailResultWrapper(parentResult);
                                m_results.Add(parentWrapper);
                            }
                            else
                            {
                                ChildTrailResult ctr = new HighScoreChildTrailResult((parentWrapper.Result as HighScoreParentTrailResult), h.order, indexes, h.tooltip);
                                TrailResultWrapper child = new TrailResultWrapper(parentWrapper, ctr);
                            }
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

        private void CalcUniqueRoutesResults(IList<IActivity> activities, int MaxAllowedMisses, bool bidirectional, System.Windows.Forms.ProgressBar progressBar)
        {
            try
            {
                if (Controller.TrailController.Instance.ReferenceActivity != null)
                {
                    IList<IActivity> searchActivities = null; //Default, UR settings
                    if (Controller.TrailController.Instance.Activities.Count != 1)
                    {
                        searchActivities = Controller.TrailController.Instance.Activities;
                    }
                    IList<IActivity> resultActivities = Integration.UniqueRoutes.GetUniqueRoutesForActivity(
                        Controller.TrailController.Instance.ReferenceActivity.GPSRoute, searchActivities, progressBar);
                    foreach (IActivity activity in resultActivities)
                    {
                        this.Status = TrailOrderStatus.Match;
                        TrailResultInfo splitIndexes = Data.Trail.TrailResultInfoFromSplits(activity, false);
                        TrailResultWrapper result = new TrailResultWrapper(new SplitsParentTrailResult(this, m_results.Count + 1, splitIndexes));
                        m_results.Add(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageDialog.Show(ex.Message, "Plugin error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalcSplitsResults(IList<IActivity> activities, int MaxAllowedMisses, bool bidirectional, System.Windows.Forms.ProgressBar progressBar)
        {
            IList<IActivity> handledActivities = new List<IActivity>();
            //Calculate the results for the selected activities
            if (Data.Settings.OverlappingResultShareSplitTime)
            {
                IList<IActivity> refActivities = new List<IActivity>();
                foreach (TrailResultWrapper tr in Controller.TrailController.Instance.SelectedResults)
                {
                    IActivity refAct = tr.Result.Activity;
                    if (activities.Contains(refAct) && !refActivities.Contains(refAct))
                    {
                        this.Status = TrailOrderStatus.Match;
                        TrailResultInfo splitIndexes = Data.Trail.TrailResultInfoFromSplits(refAct, false);
                        SplitsParentTrailResult str = new SplitsParentTrailResult(this, m_results.Count + 1, splitIndexes);
                        TrailResultWrapper refWrapper = new TrailResultWrapper(str);
                        m_results.Add(refWrapper);
                        handledActivities.Add(refAct);
                        refActivities.Add(refAct);

                        if (refAct == Controller.TrailController.Instance.ReferenceActivity)
                        {
                            Controller.TrailController.Instance.ReferenceResult = refWrapper;
                        }
                        foreach (IActivity activity in activities)
                        {
                            //Handle all activities, also if they are previously a ref
                            if (refAct != activity && refWrapper.Result.AnyOverlap(activity))
                            {
                                TrailResultInfo indexes = refWrapper.Result.SubResultInfo.CopyFromReference(activity);
                                SplitsParentTrailResult cstr = new SplitsParentTrailResult(this, m_results.Count + 1, indexes);
                                cstr.UpdateOverlap(str);
                                TrailResultWrapper result = new TrailResultWrapper(cstr);

                                m_results.Add(result);
                                handledActivities.Add(activity);
                            }
                        }
                    }
                }
            }
            //Remaining results
            foreach (IActivity activity in activities)
            {
                if (!handledActivities.Contains(activity))
                {
                    this.Status = TrailOrderStatus.Match;
                    TrailResultInfo splitIndexes = Data.Trail.TrailResultInfoFromSplits(activity, false);
                    SplitsParentTrailResult str = new SplitsParentTrailResult(this, m_results.Count + 1, splitIndexes);
                    TrailResultWrapper result = new TrailResultWrapper(str);
                    m_results.Add(result);
                }
            }
        }

        private void CalcPositionResults(IList<IActivity> activities, int MaxAllowedMisses, bool bidirectional, System.Windows.Forms.ProgressBar progressBar)
        {
            IList<TrailGPSLocation> trailgps = null;
            IList<IGPSBounds> locationBounds = new List<IGPSBounds>();
            if (m_trail.TrailType == Trail.CalcType.TrailPoints ||
                m_trail.TrailType == Trail.CalcType.ElevationPoints)
            {
                trailgps = m_trail.TrailLocations;
                int noNonReq = TrailGPSLocation.LocationBounds(trailgps, locationBounds, 10 * this.m_trail.Radius);
                MaxAllowedMisses = Math.Min(trailgps.Count - noNonReq, MaxAllowedMisses);
            }
            //Calculate InBound information if not already done
            if (this.Status != TrailOrderStatus.InBoundNoCalc)
            {
                bool tmp = this.IsInBounds;
            }
            foreach (IActivity activity in activities)
            {

                TrailOrderStatus activityStatus = TrailOrderStatus.NoInfo;
                if (trailgps != null && trailgps.Count > 0)
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
                            /*activityStatus != TrailOrderStatus.Match &&*/ activityStatus < TrailOrderStatus.InBound)
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
                    TrailResultInfo splitIndexes = Data.Trail.TrailResultInfoFromSplits(activity, false);
                    TrailResultWrapper result = new TrailResultWrapper(new SplitsParentTrailResult(this, m_results.Count + 1, splitIndexes));
                    m_results.Add(result);
                }
            }
            //Always set InBound count, used in some displays
            m_noResCount[TrailOrderStatus.InBound] = m_inBound.Count;

            if (Data.Settings.OverlappingResultShareSplitTime)
            {
                foreach (TrailResultWrapper tr in Controller.TrailController.Instance.SelectedResults)
                {
                    foreach (TrailResultWrapper trw in this.m_results)
                    {
                        //Handle all activities, also if they are previously a ref
                        if (tr.Result.Activity != trw.Result.Activity &&
                           //Not using AnyOverlap (avoid multiple matches), just check first
                           Math.Abs((tr.Result.StartTime - trw.Result.StartTime).TotalSeconds) < 9)
                        {
                            trw.Result.UpdateOverlap(tr.Result);
                            TrailResultInfo indexes = tr.Result.SubResultInfo.CopyFromReference(trw.Result.Activity);
                            trw.UpdateIndexes(indexes);
                        }
                    }
                }
            }
        }

        public void FilterURRouteSnippet(IGPSRoute route, System.Windows.Forms.ProgressBar progressBar)
        {
            IList<IActivity> urActivities = Integration.UniqueRoutes.GetSimilarRoutes(
                Controller.TrailController.Instance.ReferenceResult.Result.GPSRoute, this.ResultActivities, progressBar);

            IList<TrailResultWrapper> trws = new List<TrailResultWrapper>();
            foreach (TrailResultWrapper trw in m_results)
            {
                if (urActivities.Contains(trw.Result.Activity) ||
                    Controller.TrailController.Instance.ReferenceActivity == trw.Result.Activity)
                {
                    trws.Add(trw);
                }
                else
                {
                    this.m_filteredResults.Add(trw);
                }
            }
            m_results = trws;
        }

        private class pInfo
        {
            public pInfo(int index, float dist, float prevDist)
            {
                this.index = index;
                this.dist = dist;
                this.prevDist = prevDist;
            }

            public int index;
            public float dist;
            public float prevDist;
        }

        internal void MergeSubResults(TrailResultWrapper parent, IList<TrailResultWrapper> ctr, bool all)
        {
            //Get the indexes from the displayed results
            //Using the order of displayed results (results may have been excluded (pauses) or deleted)
            IList<TrailResultWrapper> displayedChildren = TrailResultWrapper.ChildrenTimeSorted(parent);

            IList<int> orders = new List<int>();
            foreach (TrailResultWrapper tr in ctr)
            {
                for (int childIndex = 0;
                    childIndex < displayedChildren.Count;
                    childIndex++)
                {
                    if (tr == displayedChildren[childIndex])
                    {
                        orders.Add(childIndex);
                        break;
                    }
                }
            }

            //The indexes may not have been added in order
            ((List<int>)orders).Sort();

            IList<TrailResultWrapper> mergeResults;
            if (all)
            {
                mergeResults = new List<TrailResultWrapper>();
                foreach (TrailResultWrapper tr in this.m_results)
                {
                    mergeResults.Add(tr);
                }
                if (Data.Settings.OverlappingResultShareSplitTime)
                {
                    IList<TrailResultWrapper> overlapResults = new List<TrailResultWrapper>();
                    foreach (TrailResultWrapper tr in this.m_results)
                    {
                        if (tr.Result.Overlaps.Count > 0 &&
                            !overlapResults.Contains(tr))
                        {
                            overlapResults.Add(tr);
                        }
                    }

                    //All (only) overlapping results are handled separetly
                    foreach (TrailResultWrapper tr in this.m_results)
                    {
                        foreach (TrailResult tr2 in tr.Result.Overlaps)
                        {
                            if (!overlapResults.Contains(tr2.Wrapper))
                            {
                                mergeResults.Remove(tr2.Wrapper);
                            }
                        }
                    }
                }
            }
            else
            {
                mergeResults = new List<TrailResultWrapper> { parent };
            }


            foreach (TrailResultWrapper trw in mergeResults)
            {
                //All trail types are not handled
                if (!(trw.Result is PositionParentTrailResult) && !(trw.Result is SplitsParentTrailResult))
                {
                    continue;
                }

                IList<TrailResultWrapper> mergeChildren = TrailResultWrapper.ChildrenTimeSorted(trw);
                if (mergeChildren == null || mergeChildren.Count == 0)
                {
                    continue;
                }
                int orderIndex = 0; //wrap around the index (mark only last if offset is the same)
                TrailResultInfo indexes = trw.Result.SubResultInfo.Copy();
                indexes.Points = new List<TrailResultPoint> { mergeChildren[0].Result.SubResultInfo.Points[0] };

                for (int childIndex = 1;
                     childIndex < mergeChildren.Count;
                     childIndex++)
                {
                    int i = orders[orderIndex % orders.Count] + (orderIndex / orders.Count) * (1 + orders[orders.Count - 1]);
                    TrailResultPoint t = mergeChildren[childIndex].Result.SubResultInfo.Points[0];
                    if (childIndex <= i)
                    {
                        indexes.Points[indexes.Points.Count - 1].Merge(t);
                    }
                    else
                    {
                        //First or new start
                        indexes.Points.Add(t);
                        orderIndex++;
                    }
                    //Set the order to the last merged result
                    indexes.Points[indexes.Points.Count - 1].Order = mergeChildren[childIndex].Result.Order;
                }
                //End point always included
                indexes.Points.Add(mergeChildren[mergeChildren.Count - 1].Result.SubResultInfo.Points[1]);

                int order = trw.Result.Order;
                trw.UpdateIndexes(indexes);

                IActivity refAct = trw.Result.Activity;
                foreach (TrailResult tr2 in trw.Result.Overlaps)
                {
                    TrailResultInfo indexes2 = trw.Result.SubResultInfo.CopyFromReference(tr2.Activity);
                    tr2.Wrapper.UpdateIndexes(indexes2);
                }
            }
        }

        private TrailOrderStatus CalcInboundResults(IActivity activity, IList<TrailGPSLocation> trailgps, IList<IGPSBounds> locationBounds, int MaxRequiredMisses, bool reverse, System.Windows.Forms.ProgressBar progressBar)
        {
            IList<TrailResultInfo> trailResults = new List<TrailResultInfo>();
            TrailOrderStatus status = CalcGpsTrail(activity, null, trailgps, locationBounds, this.Trail.Radius, this.Trail.MinDistance, MaxRequiredMisses, reverse, 0, this.Trail.IsCompleteActivity, trailResults, m_incompleteResults, progressBar);

            foreach (TrailResultInfo resultInfo in trailResults)
            {
                TrailResultWrapper result = new TrailResultWrapper(new PositionParentTrailResult(this, m_results.Count + 1, resultInfo, resultInfo.DistDiff, resultInfo.Reverse));
                m_results.Add(result);
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

        internal static IList<TrailResultPoint> CalcElevationPoints(IActivity activity, IValueRangeSeries<DateTime> pauses)
        {
            IList<TrailResultPoint> result = new List<TrailResultPoint>();
            foreach (TrailGPSLocation l in TrailData.ElevationPointsTrail.TrailLocations)
            {
                IList<TrailResultPoint> points = ActivityTrail.GetClosestMatches(activity, pauses, l);
                foreach (TrailResultPoint t in points)
                {
                    if (t.Time > DateTime.MinValue)
                    {
                        result.Add(t);
                    }
                }
            }
            SortResultPointsByTime s = new SortResultPointsByTime();
            ((List<TrailResultPoint>)result).Sort(s);
            return result;
        }

        private static IList<TrailResultPoint> GetClosestMatches(IActivity activity, IValueRangeSeries<DateTime> pauses, TrailGPSLocation trailgps)
        {
            //A fix to get best point. 
            //Done rather that destroying trail detection further
            IList<TrailResultInfo> trailResults = new List<TrailResultInfo>();
            IList<ActivityTrail.IncompleteTrailResult> incompleteResults = new List<ActivityTrail.IncompleteTrailResult>();

            //Force all results to be incomplete, to match all matches along a track (to avoid best match is thrown away)
            GetTrailResultInfo(activity, pauses, new List<TrailGPSLocation> { trailgps }, trailgps.Radius, false, 1, trailResults, incompleteResults);

            //Should never give any trailResults, max results set to 1
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
            return points;
        }

        internal static TrailResultPoint GetClosestMatch(IActivity activity, IValueRangeSeries<DateTime> pauses, IGPSLocation gps, float radius)
        {
            TrailGPSLocation trailgps = new TrailGPSLocation(gps, radius);
            return GetClosestMatch(activity, pauses, trailgps);
        }

        private static TrailResultPoint GetClosestMatch(IActivity activity, IValueRangeSeries<DateTime> pauses, TrailGPSLocation trailgps)
        {
            IList<TrailResultPoint> points = GetClosestMatches(activity, pauses, trailgps);
            ((List<TrailResultPoint>)points).Sort();
            if (points.Count == 0)
            {
                return null;
            }
            return points[0];
        }

        internal static TrailOrderStatus GetTrailResultInfo(IActivity activity, IValueRangeSeries<DateTime> pauses, IList<IGPSLocation> gpsLoc,
            float radius, bool bidirectional, IList<TrailResultInfo> trailResults)
        {
            IList<ActivityTrail.IncompleteTrailResult> incompleteResults = new List<ActivityTrail.IncompleteTrailResult>(); //unused
            IList<TrailGPSLocation> trailgps = Trail.TrailGpsPointsFromGps(gpsLoc, radius);

            TrailOrderStatus status = GetTrailResultInfo(activity, pauses, trailgps, radius, bidirectional, 0, trailResults, incompleteResults);

            //Sort best match first
            ((List<TrailResultInfo>)trailResults).Sort();

            return status;
        }

        private static TrailOrderStatus GetTrailResultInfo(IActivity activity, IValueRangeSeries<DateTime> pauses, IList<TrailGPSLocation> trailgps,
            float radius, bool bidirectional, int maxPoints, IList<TrailResultInfo> trailResults, IList<ActivityTrail.IncompleteTrailResult> incompleteResults)
        {
            TrailOrderStatus status = TrailOrderStatus.NoInfo;

            if (activity != null && activity.GPSRoute != null && activity.GPSRoute.Count > 0)
            {
                IList<ZoneFiveSoftware.Common.Data.GPS.IGPSBounds> locationBounds = new List<ZoneFiveSoftware.Common.Data.GPS.IGPSBounds>();
                TrailGPSLocation.LocationBounds(trailgps, locationBounds, 10 * radius);

                status = CalcGpsTrail(activity, pauses, trailgps, locationBounds,
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
                    TrailOrderStatus status2 = CalcGpsTrail(activity, pauses, trailgpsReverse, locationBoundsReverse,
                        radius, 0, 0, true, maxPoints, false, trailResults, incompleteResults, null);
                    status = BestCalcStatus(status, status2);
                }
            }

            return status;
        }

        public IList<TrailResultWrapper> CalcTrailCompleteResult(IActivity activity)
        {
            IList<TrailResultWrapper> res = new List<TrailResultWrapper>();
            if (activity != null && activity.GPSRoute != null && activity.GPSRoute.Count > 0)
            {
                IList<TrailResultInfo> trailResults = new List<TrailResultInfo>();
                IList<IncompleteTrailResult> incompleteResults = new List<IncompleteTrailResult>();
                IList<TrailGPSLocation> trailgps = m_trail.TrailLocations;
                IList<IGPSBounds> locationBounds = new List<IGPSBounds>();
                int noNonReq = TrailGPSLocation.LocationBounds(trailgps, locationBounds, 10 * this.m_trail.Radius);
                int MaxAllowedMisses = Math.Min(trailgps.Count - noNonReq, m_trail.MaxRequiredMisses);

                CalcGpsTrail(activity, null, this.Trail.TrailLocations, locationBounds,
                    this.Trail.Radius, this.Trail.MinDistance, 0, false, MaxAllowedMisses, true,
                    trailResults, incompleteResults, null);
                foreach (TrailResultInfo tri in trailResults)
                {
                    TrailResult tr = new PositionParentTrailResult(this, 0, tri, tri.DistDiff, tri.Reverse);
                    TrailResultWrapper result = new TrailResultWrapper((ParentTrailResult)tr);
                    res.Add(result);
                }
            }
            return res;
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
        private static TrailOrderStatus CalcGpsTrail(IActivity activity, IValueRangeSeries<DateTime> pauses, IList<TrailGPSLocation> trailgps, IList<IGPSBounds> locationBounds,
            float radius, float minDistance, int MaxRequiredMisses, bool reverse, int maxResultPoints, bool isComplete,
            IList<TrailResultInfo> trailResults, IList<IncompleteTrailResult> incompleteResults, System.Windows.Forms.ProgressBar progressBar)
        {
            CurrentResult currResult = new CurrentResult(trailgps);

            //Cache information about previous distances and index
            PointInfo prevPoint = new PointInfo(-1, 0);
            PointInfo prevStartPoint = new PointInfo(-1, 0);

            float distHysteresis;//Used as const
#if SQUARE_DISTANCE
            //See TrailGPSLocation for details about SQUARE_DISTANCE
            //This improves trail detection time slightly, but has currently some differences
            //In additition, the standard distance is useful for debugging 
            
            //Factors are not completely independent, but minimal effect
            radius = (TrailGPSLocation.DistanceToSquareScaling * radius) * (TrailGPSLocation.DistanceToSquareScaling * radius);
            {
                float distHysteresisMin = (TrailGPSLocation.DistanceToSquareScaling * 5) * (TrailGPSLocation.DistanceToSquareScaling * 5);
                const float distHysteresisFactor = 30*30;
                distHysteresis = Math.Max(radius / distHysteresisFactor, distHysteresisMin);
            }
            const float radiusFactor = 2*2;
            const float passByFactor = 10*10;
#else
            {
                const float distHysteresisMin = 5;
                const float distHysteresisFactor = 30;
                distHysteresis = Math.Max(radius / distHysteresisFactor, distHysteresisMin);
            }
            const float radiusFactor = 2;
            const float passByFactor = 10;
#endif

            //Ignore short legs - undocumented feature
            IDistanceDataTrack dTrack = null;
            if (minDistance > 0)
            {
                //Must use the distance track related to the GPS track here, not the (potentially optimized) InfoCache track
                dTrack = activity.GPSRoute.GetDistanceMetersTrack();
            }

            //Check every GPS point
            //This handling is very time critical, also use of methods affect considaerably
            //The methods have some refs as well, not separated from the flow

            //cache some data
            int activity_GPSRoute_Count = activity.GPSRoute.Count; //const, optimisation
            for (int routeIndex = 0; routeIndex < activity_GPSRoute_Count; routeIndex++)
            {

                //////////////////////////////////////
                //Shorten the trail if possible
                if (currResult.SingleFirstMatch >= 0 &&
                    locationBounds[currResult.SingleFirstMatch].Contains((IGPSLocation)(activity.GPSRoute[routeIndex].Value)))
                {
                    shortenTrail(activity, trailgps, locationBounds, radius, passByFactor, routeIndex, currResult, ref prevStartPoint);
                }

                /////////////////////////////////////
                //try matching
                TrailResultPointMeta matchPoint = null;
                if (locationBounds[currResult.NextTrailGpsIndex].Contains((IGPSLocation)(activity.GPSRoute[routeIndex].Value)))
                {
                    matchPoint = getMatch(activity, trailgps, locationBounds, radius, passByFactor, distHysteresis, ref routeIndex, currResult, ref prevPoint);
                }

                //////////////////////////////
                //All GPS points tested but search should maybe match
                //Not meaningful for one point trails
                if (routeIndex >= (activity_GPSRoute_Count - 1) && matchPoint == null)
                {
                    bool match = false;
                    if (trailgps.Count > 1)
                    {
                        bool required = trailgps[currResult.NextTrailGpsIndex].Required;
                        ///////////////////
                        //Last point check for non required points - automatic match, so search can restart
                        if (!required)
                        {
                            match = true;
                        }
                        else if (currResult.currRequiredMisses < MaxRequiredMisses)
                        {
                            //OK to miss this point. Set automatic match to start looking at prev match
                            currResult.currRequiredMisses++;
                            match = true;
                        }
                    }
                    else if (isComplete && currResult.CurrMatches == 1)
                    {
                        match = true;
                    }

                    if (match)
                    {
                        matchPoint = new TrailResultPointMeta(trailgps[currResult.NextTrailGpsIndex], radius * radiusFactor);
                    }
                }

                /////////////////////////////////////
                //Add found match to result 
                ///////////////////////////////////////
                if (matchPoint != null)
                {
                    //Ignore short legs (uses point-point distance, not "time"-"time")
                    if (minDistance > 0 && matchPoint.Time != DateTime.MinValue && currResult.CurrMatches > 0 && dTrack != null && currResult.LastMatchIndex >= 0 && matchPoint.index > 0 &&
                              minDistance > (dTrack[matchPoint.index].Value - dTrack[currResult.LastMatchIndex].Value))
                    {
                        matchPoint = null;
                    }
                    else if (pauses != null && matchPoint.Time != DateTime.MinValue && ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(matchPoint.Time, pauses))
                    {
                        //selection outside the interesting bounds
                        matchPoint = null;
                    }
                    else
                    {
                        int nextMatchIndex = updateResults(activity, currResult, trailResults, incompleteResults,
                            maxResultPoints, reverse, isComplete, matchPoint);
                        routeIndex = nextMatchIndex - 1; //routeIndex is always increased
                        //Clear cache, dist unknown to next point
                        prevPoint.index = -1;
                    }
                }
            } //foreach gps point

            ///////////////////////////////////////
            //Multiple "Complete" results does not make much sense, merge them and add start/end
            if (isComplete && trailResults != null && trailResults.Count > 0)
            {
                DateTime pDate = DateTime.MaxValue;
                TrailResultInfo resultInfo = new TrailResultInfo(activity, reverse);
                foreach (TrailResultInfo r in trailResults)
                {
                    foreach (TrailResultPointMeta t in r.Points)
                    {
                        if (t.Time != pDate)
                        {
                            resultInfo.Points.Add(t);
                        }
                        pDate = t.Time;
                    }
                }
                int i;
                TrailResultPointMeta p;

                i = 0;
                p = new TrailResultPointMeta(new TrailGPSLocation(activity.GPSRoute[i].Value), activity.GPSRoute.StartTime, i, i, i, 0);
                p.SetElevation(float.NaN); //No elevation point
                resultInfo.Points.Insert(0, p);

                i = activity.GPSRoute.Count - 1;
                p = new TrailResultPointMeta(new TrailGPSLocation(activity.GPSRoute[i].Value), activity.GPSRoute.EntryDateTime(activity.GPSRoute[i]), i, 1, i, 0);
                p.SetElevation(float.NaN);
                resultInfo.Points.Add(p);

                trailResults.Clear();
                trailResults.Add(resultInfo);
            }
            ///////////////////////////////////////
            //Possible last incomplete result
            //InBoundMatchPartial updated for all incomplete results
            if (currResult.CurrMatches > 0 && !reverse || currResult.CurrMatches > 1)
            {
                IncompleteTrailResult incompleteResult = new IncompleteTrailResult(activity, currResult.Points, reverse);
                incompleteResults.Add(incompleteResult);
            }
            return currResult.status;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private static void shortenTrail(IActivity activity, IList<TrailGPSLocation> trailgps, IList<IGPSBounds> locationBounds,
            float radius, float passByFactor, int routeIndex, CurrentResult currResult, ref PointInfo prevStartPoint)
        {
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

            //checked in main loop for efficiency
            //currResult.SingleFirstMatch >= 0  &&
            //locationBounds[currResult.SingleFirstMatch].Contains((IGPSLocation)(activity.GPSRoute[routeIndex].Value)) 
            if (trailgps.Count > 1 && routeIndex > (1 + currResult.Points[currResult.SingleFirstMatch].nextMatchOutsideRadius))
            {
                PointInfo startPoint = new PointInfo(routeIndex,
                     TrailGPSLocation.DistanceMetersToPointSimple(trailgps[currResult.SingleFirstMatch], activity.GPSRoute[routeIndex].Value));
                bool match = false;
                if (startPoint.dist < radius)
                {
                    match = true;
                }
                else if (startPoint.dist < radius * passByFactor)
                {
                    if (prevStartPoint.index != routeIndex - 1)
                    {
                        //refresh cache
                        prevStartPoint.index = routeIndex - 1;
                        prevStartPoint.dist = TrailGPSLocation.DistanceMetersToPointSimple(trailgps[currResult.SingleFirstMatch], activity.GPSRoute[prevStartPoint.index].Value);
                    }
                    float d;
                    if (0 < TrailGPSLocation.checkPass(radius,
                activity.GPSRoute[prevStartPoint.index].Value, prevStartPoint.dist,
                activity.GPSRoute[startPoint.index].Value, startPoint.dist,
                trailgps[currResult.SingleFirstMatch]/*, dTrack[routeIndex].Value - dTrack[prevStartPointIndex].Value*/,
                out d))
                    {
                        match = true;
                    }
                }
                if (match)
                {
                    //Start over if we pass first point before all were found
                    currResult.Clear(routeIndex);
                }
                prevStartPoint = startPoint;
            }
        }

        private static TrailResultPointMeta getMatch(IActivity activity, IList<TrailGPSLocation> trailgps, IList<IGPSBounds> locationBounds,
            float radius, float passByFactor, float distHysteresis, ref int routeIndex, CurrentResult currResult, ref PointInfo prevPoint)
        {
            TrailResultPointMeta matchPoint = null;

            //Check if the point is in bounds, the distance to point is the heaviest calculation per point
            //location bounds is aproximate, should cover the aproximate "checkPass" area

            //Check in main loop for efficiency
            //if (locationBounds[currResult.TrailGpsIndex].Contains((IGPSLocation)(activity.GPSRoute[routeIndex].Value)))

            {
                if (routeIndex > 0 && routeIndex - prevPoint.index != 1)
                {
                    prevPoint.index = routeIndex - 1;
                    prevPoint.dist = TrailGPSLocation.DistanceMetersToPointSimple(trailgps[currResult.NextTrailGpsIndex], activity.GPSRoute[prevPoint.index].Value);
                }
                float routeDist = TrailGPSLocation.DistanceMetersToPointSimple(trailgps[currResult.NextTrailGpsIndex], activity.GPSRoute[routeIndex].Value);

                //////////////////////////////////////
                //Find the best GPS point for this result
                if (routeDist < radius)
                {
                    bool firstPoint = true;
                    float prevRouteDist = prevPoint.dist;

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
                            routeDist = TrailGPSLocation.DistanceMetersToPointSimple(trailgps[currResult.NextTrailGpsIndex], activity.GPSRoute[routeIndex].Value);
                        }
                        else
                        {
                            firstPoint = false;
                        }

                        {
                            if (currResult.CurrMatches == 0)
                            {
                                //start point
                                //Just cycle through the points, to prepare for back track to first (matchIndex)
                                pInfo pointInfo = new pInfo(routeIndex, routeDist, prevRouteDist);
                                info.Push(pointInfo);
                            }
                            else
                            {
                                //Middle point: Try until out of radius
                                //End point: Abort when leaving center (no refinement if non required last)
                                float routeFactor = -1;
                                float closeDist = routeDist;
                                if (routeIndex > 0)
                                {
                                    //Check closest point
                                    float dist;
                                    routeFactor = TrailGPSLocation.checkPass(radius,
                                        activity.GPSRoute[routeIndex - 1].Value, prevRouteDist,
                                        activity.GPSRoute[routeIndex].Value, routeDist,
                                        trailgps[currResult.NextTrailGpsIndex], out dist);
                                    if (routeFactor > 0)
                                    {
                                        closeDist = (float)dist;
                                    }
                                }

                                if (closeDist < matchDist)
                                {
                                    //Better, still closing in
                                    matchIndex = routeIndex;
                                    matchDist = closeDist;
                                    matchFactor = routeFactor;
                                }
                                if (currResult.NextIsEndTrailPoint && ((routeFactor > 0) && (closeDist < distHysteresis * 3) || (routeDist > matchDist + distHysteresis)))
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
                            if (currResult.CurrMatches == 0)
                            {
                                foreach (pInfo p in info)
                                {
                                    float routeFactor = -1;
                                    float closeDist = p.dist;
                                    float dist;
                                    if (p.index > 0)
                                    {
                                        routeFactor = TrailGPSLocation.checkPass(radius,
                                            activity.GPSRoute[p.index - 1].Value, p.prevDist,
                                            activity.GPSRoute[p.index].Value, p.dist,
                                            trailgps[currResult.NextTrailGpsIndex], out dist);
                                        if (routeFactor > 0)
                                        {
                                            closeDist = (float)dist;
                                        }
                                    }

                                    if (closeDist < matchDist)
                                    {
                                        //Better, still closing in
                                        matchIndex = p.index;
                                        matchDist = closeDist;
                                        matchFactor = routeFactor;
                                    }
                                    if (((routeFactor > 0) && (closeDist < distHysteresis * 3)) || (p.prevDist > matchDist + distHysteresis))
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
                    int prevMatchIndex = matchIndex;
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

                    matchPoint = new TrailResultPointMeta(trailgps[currResult.NextTrailGpsIndex], matchTime,
                        matchIndex, prevMatchIndex, routeIndex + 1, matchDist);
                } //if (dist < radius)

                ///////////
                //Check for pass-by
                //This handling is very sensitive for single point trails, extra check required
                else if (routeDist < radius * passByFactor && prevPoint.index >= 0 && routeIndex > currResult.LastMatchOutsideRadius)
                {
                    float d;
                    float factor = TrailGPSLocation.checkPass(radius,
                        activity.GPSRoute[prevPoint.index].Value, prevPoint.dist,
                        activity.GPSRoute[routeIndex].Value, routeDist,
                        trailgps[currResult.NextTrailGpsIndex],
                        out d);
                    if (0 < factor)
                    {
                        DateTime d1 = activity.GPSRoute.EntryDateTime(activity.GPSRoute[prevPoint.index]);
                        DateTime d2 = activity.GPSRoute.EntryDateTime(activity.GPSRoute[routeIndex]);
                        DateTime matchTime = d1.Add(TimeSpan.FromSeconds(factor * (d2 - d1).TotalSeconds));

                        //if setting matchIndex to prev point, following matches can match but give loop issues
                        matchPoint = new TrailResultPointMeta(trailgps[currResult.NextTrailGpsIndex], matchTime,
                            routeIndex, routeIndex - 1, routeIndex, (float)d);
                    }
                }
                //Cache previous values, used in passed-by checks for this trail point
                prevPoint.index = routeIndex;
                prevPoint.dist = routeDist;
            }
            //No need to invalidate the cache, it must be checked anyway (invalidation when trail point is switched)
            //else
            //{
            //    prevPoint.index = -1;
            //}
            return matchPoint;
        }

        private static int updateResults(IActivity activity, CurrentResult currResult, IList<TrailResultInfo> trailResults, IList<IncompleteTrailResult> incompleteResults,
            int maxResultPoints, bool reverse, bool isComplete, TrailResultPointMeta matchPoint)
        {
            bool isEnd = currResult.NextIsEndTrailPoint;
            currResult.Add(matchPoint);

            //Lowest value for next start point, updated at OK matches
            //Next match must at least be at last OK match (but may be in same radius)
            //(If this was an automatic match (-1), set back the routeIndex)
            int nextMatchIndex = currResult.NextOkMatch();

            //////////////////////////////
            //End point
            if (isEnd ||
                //Abort at a certain number of result points
                (maxResultPoints > 0 && maxResultPoints >= currResult.CurrMatches))
            {
                //A result must have at least two matches, otherwise it is not possible to get distance etc
                if (currResult.CurrMatches >= 2 || currResult.CurrMatches == 1 && isComplete)
                {
                    if (currResult.CurrMatches < currResult.Points.Count)
                    {
                        currResult.status = BestCalcStatus(currResult.status, TrailOrderStatus.MatchPartial);
                    }
                    else
                    {
                        currResult.status = BestCalcStatus(currResult.status, TrailOrderStatus.Match);
                        //OK to reset misses only at full match
                        currResult.currRequiredMisses = 0;
                    }

                    TrailResultInfo resultInfo = new TrailResultInfo(activity, reverse);
                    for (int i = 0; i < currResult.Points.Count; i++)
                    {
                        TrailResultPointMeta point = currResult.Points[i];
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
                    //if (isComplete)
                    //{
                    //    int i;
                    //    TrailResultPointMeta p;

                    //    i = 0;
                    //    p = new TrailResultPointMeta(new TrailGPSLocation(activity.GPSRoute[i].Value), activity.GPSRoute.StartTime, i, i, i, 0);
                    //    p.SetElevation(""); //No elevation point
                    //    //resultInfo.Points.Insert(0, p);

                    //    i = activity.GPSRoute.Count - 1;
                    //    p = new TrailResultPointMeta(new TrailGPSLocation(activity.GPSRoute[i].Value), activity.GPSRoute.EntryDateTime(activity.GPSRoute[i]), i, 1, i, 0);
                    //    p.SetElevation("");
                    //    //resultInfo.Points.Add(p);

                    //    //Note: No update of prevMatchIndex, allow multiple loops
                    //}
                    trailResults.Add(resultInfo);
                }
                else
                {
                    //Match is incomplete
                    if (!reverse && currResult.CurrMatches >= 1)
                    {
                        //One point match: Only forward, reverse matches will be the same
                        IncompleteTrailResult result = new IncompleteTrailResult(activity, currResult.Points, reverse);
                        incompleteResults.Add(result);
                    }
                    if (maxResultPoints <= 0)
                    {
                        //No match, abort further matches
                        nextMatchIndex = activity.GPSRoute.Count;
                    }
                }

                //Reset matches for trail detection
                currResult.Clear();
            }
            else
            {
                // First/Intermediate point (no action)
            }

            return nextMatchIndex;
        }

        /*********************************************************************************/
        private class CurrentResult
        {
            public CurrentResult(IList<TrailGPSLocation> trailResults)
            {
                this.noOfTrailGps = trailResults.Count;
                if (this.noOfTrailGps == 1)
                {
                    checkSingleFirstMatch = true;
                }
            }
            public IList<TrailResultPointMeta> Points = new List<TrailResultPointMeta>();
            //Some calculated variables
            public int SingleFirstMatch = -1;
            private bool checkSingleFirstMatch = false;

            public int NextTrailGpsIndex = 0;
            private int noOfTrailGps;

            private int nextOkMatch = 0; //Next point to match OK
            private int firstMatchOutsideRadius = -1; //For first point - avoid loops
            public int LastMatchOutsideRadius = -1; //Previous match
            public int LastMatchIndex = -1;

            public int CurrMatches = 0; //Real matches (auto match excluded)
            public bool NextIsEndTrailPoint = false;

            //Required points misses - undocumented feature
            public int currRequiredMisses = 0;
            public TrailOrderStatus status = TrailOrderStatus.NoInfo;

            public void Add(TrailResultPointMeta t)
            {
                if (this.noOfTrailGps > 1)
                {
                    this.NextTrailGpsIndex = this.Points.Count + 1;
                    if (this.Points.Count + 2 >= this.noOfTrailGps)
                    {
                        this.NextIsEndTrailPoint = true;
                    }
                }
                else
                {
                    //Note: this.NextTrailGpsIndex is always 0
                    this.NextIsEndTrailPoint = true;
                }

                if (t.index > -1 && t.Time > DateTime.MinValue)
                {
                    if (this.SingleFirstMatch >= 0)
                    {
                        //No longer single index
                        this.SingleFirstMatch = -1;
                    }
                    else if (!this.checkSingleFirstMatch)
                    {
                        this.SingleFirstMatch = this.Points.Count;
                        this.checkSingleFirstMatch = true;
                    }
                    if (this.CurrMatches == 0)
                    {
                        this.firstMatchOutsideRadius = t.nextMatchOutsideRadius;
                    }
                    this.LastMatchIndex = t.index;
                    this.LastMatchOutsideRadius = t.nextMatchOutsideRadius;

                    if (t.nextOkMatch < 0)
                    {
                        this.nextOkMatch = 0;
                    }
                    else
                    {
                        this.nextOkMatch = t.nextOkMatch;
                    }

                    if (this.NextIsEndTrailPoint || this.noOfTrailGps == 1)
                    {
                        //To avoid "detection loops", the potential start for next trail must be after 
                        //the first possible match for the first trail point
                        //For one point trail, the same applies to the first match as for the end point (also incomplete)
                        this.nextOkMatch = Math.Max(this.nextOkMatch, this.firstMatchOutsideRadius);
                    }

                    //(Possibly) update status for intermediate/incomplete results
                    //At least one point match
                    status = BestCalcStatus(status, TrailOrderStatus.InBoundMatchPartial);

                    this.CurrMatches++;
                }
                this.Points.Add(t);
            }

            public int NextOkMatch()
            {
                return this.nextOkMatch;
            }

            //Clear when "shortening" trails
            public void Clear(int index)
            {
                this.Clear();
                this.firstMatchOutsideRadius = index;
                this.LastMatchOutsideRadius = index;
                this.LastMatchIndex = index;
                this.nextOkMatch = index;
            }

            public void Clear()
            {
                this.Points.Clear();

                this.CurrMatches = 0;
                this.NextTrailGpsIndex = 0;
                this.SingleFirstMatch = -1;
                if (this.noOfTrailGps > 1)
                {
                    this.checkSingleFirstMatch = false;
                }
                this.NextIsEndTrailPoint = false;
            }
        }

        ///////////////////////////////////////////
        public void Remove(TrailResultWrapper tr)
        {
            if (this.m_results != null)
            {
                foreach (TrailResultWrapper trr in m_results)
                {
                    trr.AllChildren.Remove(tr);
                    //May not be needed as Children are added when sorting
                    trr.Children.Remove(tr);
                }
                this.m_results.Remove(tr);
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
                    TrailResult tr = new PositionParentTrailResult(this, m_results.Count + 1,
                        resultInfo, resultInfo.DistDiff, t.Reverse);
                    result.Add(tr);
                }
                return result;
            }
        }

        public IList<TrailResultWrapper> FilteredResults
        {
            get
            {
                return this.m_filteredResults;
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
                    IList<TrailResultWrapper> visible = TrailResultWrapper.UnpausedResults(this.Results);
                    foreach (Data.TrailResultWrapper tr in visible)
                    {
                        sortValue += tr.Result.SortQuality;
                    }
                    sortValue = sortValue / (float)Math.Pow(visible.Count, 1.5);
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
            else if (this.Trail.TrailPriority != to2.Trail.TrailPriority)
            {
                return (this.Trail.TrailPriority < to2.Trail.TrailPriority) ? 1 : -1;
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
            public TrailResultPointMeta(TrailGPSLocation trailLoc, float diffDist) :
                this(trailLoc, DateTime.MinValue, -1, -1, -1, diffDist)
            {
            }

            public TrailResultPointMeta(TrailGPSLocation trailLoc, DateTime time,
                int index, int nextOkMatch, int nextMatchOutsideRadius, float diffDist) :
                base(trailLoc, time, diffDist)
            {
                this.index = index;
                this.nextOkMatch = nextOkMatch;
                this.nextMatchOutsideRadius = nextMatchOutsideRadius;
            }
            //indexes for this match
            public int index;
            public int nextOkMatch;
            public int nextMatchOutsideRadius;
        }

        public override string ToString()
        {
            return (new TrailDropdownLabelProvider()).GetText(this, null) + ": " + this.Status.ToString();
        }
    }

    public enum TrailOrderStatus
    {
        //<= InBound is inbound
        //InBoundNoCalc is better than InBound, as it may be Match
        //MatchPartial is when all match, but some with "wildcards"
        //InBoundMatchPartial is when there is at least one match, but not for all points
        Match, MatchPartial, MatchNoCalc, InBoundMatchPartial, InBoundNoCalc, InBound, NotInBound, NoConfiguration, NotInstalled, NoInfo
    }
}
