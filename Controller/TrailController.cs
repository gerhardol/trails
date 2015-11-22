/*
Copyright (C) 2009 Brendan Doherty
Copyright (C) 2012 Gerhard Olsson

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
using TrailsPlugin.Data;

namespace TrailsPlugin.Controller 
{
    public class TrailController
    {
        static private TrailController m_instance;
        static public TrailController Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new TrailController();
                }
                return m_instance;
            }
        }

        private IList<IActivity> m_activities = new List<IActivity>();
        private IList<ActivityTrail> m_currentActivityTrails = new List<ActivityTrail>();
        private IList<ActivityTrail> m_prevSelectedTrails = new List<ActivityTrail>();
        private IList<ActivityTrail> m_prevActivityTrails = new List<ActivityTrail>();
        
        private TrailResult m_referenceTrailResult = null;
        private IActivity m_referenceActivity = null;

        private IList<ActivityTrail> m_CurrentOrderedTrails = new List<ActivityTrail>();
        private ActivityTrail m_referenceActivityTrail;

        private TrailController()
        {
            foreach (Trail trail in TrailData.AllTrails.Values)
            {
                ActivityTrail to = new ActivityTrail(this, trail);
                this.m_CurrentOrderedTrails.Add(to);
                if (trail.IsReference)
                {
                    this.m_referenceActivityTrail = to;
                }
            }
        }

        //Update activities, should keep trail and reference
        public IList<IActivity> Activities
        {
            get
            {
                return m_activities;
            }
            set
            {
                this.SetActivities(value, true, null);
            }
        }

        //Set activities, do not force calculation
        public void SetActivities(IList<IActivity> activities, bool keepSelectedTrail)
        {
            SetActivities(activities, keepSelectedTrail, null);
        }

        public void SetActivities(IList<IActivity> activities, bool keepSelectedTrail, System.Windows.Forms.ProgressBar progressBar)
        {
            if (this.m_activities != activities)
            {
                if (activities == null || activities.Count == 1 && activities[0] == null)
                {
                    this.m_activities.Clear();
                }
                else
                {
                    this.m_activities = activities;
                }
                if (this.PrimaryCurrentActivityTrail != null &&
                    this.PrimaryCurrentActivityTrail.Trail.DefaultRefActivity != null &&
                    !this.m_activities.Contains(this.PrimaryCurrentActivityTrail.Trail.DefaultRefActivity))
                {
                    this.m_activities.Add(this.PrimaryCurrentActivityTrail.Trail.DefaultRefActivity);
                }

                if (!keepSelectedTrail)
                {
                    this.m_currentActivityTrails.Clear();
                }
                this.Reset();
                //Make sure reference activity is 'reasonable' in case the reference trail is selected
                this.checkReferenceActivity(progressBar);

                //Calculate trails - at least InBounds, set apropriate ActivityTrail
                this.ReCalcTrails(false, progressBar);
            }
        }

        public IList<ActivityTrail> OrderedTrails()
        {
            //Trail may be calculated in various situations, also when getting results,
            //It is therefore easier to keep resort here
            ((List<ActivityTrail>)m_CurrentOrderedTrails).Sort();
            return m_CurrentOrderedTrails;
        }

        //Clear trail results
        public void Clear(bool onlyDisplay)
        {
            foreach (ActivityTrail t in this.OrderedTrails())
            {
                t.Clear(onlyDisplay);
            }
        }

        //Reset calculations, without changing selected activities
        public void Reset()
        {
            TrailResult.Reset();
            foreach (ActivityTrail at in m_CurrentOrderedTrails)
            {
                at.Init();
            }
            //activityGps handled separately
        }

        public ActivityTrail GetActivityTrail(Trail t)
        {
            ActivityTrail res = null;
            foreach (ActivityTrail at in m_CurrentOrderedTrails)
            {
                if (at.Trail == t)
                {
                    res = at;
                    break;
                }
            }
            return res;
        }

        public void CurrentReset(bool onlyDisplay)
        {
            foreach (ActivityTrail t in this.CurrentActivityTrails)
            {
                t.Init();
            }
        }

        //Used when no calculation is forced, ie displaying
        public bool CurrentActivityTrailIsSelected
        {
            get
            {
                return this.m_currentActivityTrails.Count > 0;
            }
        }

        public IList<ActivityTrail> CurrentActivityTrails
        {
            get
            {
                return m_currentActivityTrails;
            }
        }

        public ActivityTrail PrimaryCurrentActivityTrail
        {
            get
            {
                if (m_currentActivityTrails.Count == 0)
                {
                    return null;
                }
                else
                {
                    return m_currentActivityTrails[0];
                }
            }
        }

        public IList<TrailResultWrapper> CurrentResultTreeList
        {
            get
            {
                IList<TrailResultWrapper> result = new List<TrailResultWrapper>();
                foreach (ActivityTrail at in this.CurrentActivityTrails)
                {
                    foreach (TrailResultWrapper tw in at.ResultTreeList)
                    {
                        result.Add(tw);
                    }
                }
                
                return result;
            }
        }

        private TrailOrderStatus CurrentStatus()
        {
            TrailOrderStatus result = TrailOrderStatus.NoInfo;
            foreach (ActivityTrail at in this.CurrentActivityTrails)
            {
                if (at.Status < result)
                {
                    result = at.Status;
                }
                if (result == TrailOrderStatus.Match)
                {
                    break;
                }
            }
            return result;
        }

        private void SetCurrentActivityTrail(ActivityTrail activityTrail, bool manuallySet, System.Windows.Forms.ProgressBar progressBar)
        {
            SetCurrentActivityTrail(new List <ActivityTrail>{activityTrail}, manuallySet, progressBar);
        }

        public void SetCurrentActivityTrail(IList<ActivityTrail> activityTrails, bool manuallySet, System.Windows.Forms.ProgressBar progressBar)
        {
            //Create a copy of the trails to set - could be that activityTrails is cleared in next step
            IList<ActivityTrail> activityTrails2 = new List<ActivityTrail>();
            foreach (ActivityTrail to in activityTrails)
            {
                if (!activityTrails2.Contains(to))
                {
                    activityTrails2.Add(to);
                }
                foreach(Trail t2 in to.Trail.Children)
                {
                    ActivityTrail to2 = GetActivityTrail(t2);
                    if (!activityTrails2.Contains(to2))
                    {
                        activityTrails2.Add(to2);
                    }
                }
            }

            this.m_currentActivityTrails.Clear();
            this.m_prevActivityTrails.Clear();
            if (manuallySet)
            {
                this.m_prevSelectedTrails.Clear();
            }

            foreach (ActivityTrail to in activityTrails2)
            {
                this.m_currentActivityTrails.Add(to);
                if (!to.Trail.Generated)
                {
                    this.m_prevActivityTrails.Add(to);
                }
                if (manuallySet)
                {
                    this.m_prevSelectedTrails.Add(to);
                }

                //Trigger result calculation, a current trail is always calculated
                if (progressBar != null)
                {
                    progressBar.Maximum++;
                }
                to.CalcResults(progressBar);
            }

            //Reference may have been changed
            this.checkReferenceTrailResult(progressBar);
        }

        private bool CheckSetCurrentList(IList<ActivityTrail> activityTrails, System.Windows.Forms.ProgressBar progressBar)
        {
            //If last selected trail still has results, use all the trails
            bool useSelected = false;
            foreach (ActivityTrail at in activityTrails)
            {
                if (this.m_activities.Count <= Settings.MaxAutoCalcActitiesSingleTrail &&
                    at.IsNoCalc)
                {
                    //Try to get a better status, this is the trail the user wants
                    at.CalcResults(progressBar);
                }
                if (at.Status <= TrailOrderStatus.MatchPartial &&
                    at.ResultTreeList.Count <= Settings.MaxAutoCalcResults)
                {
                    useSelected = true;
                    break;
                }
            }
            if (useSelected)
            {
                this.SetCurrentActivityTrail(activityTrails, false, progressBar);
            }
            return useSelected;
        }

        private void CheckSetCurrentTrail(System.Windows.Forms.ProgressBar progressBar)
        {
            if (this.m_activities.Count > 0)
            {
                if (this.m_currentActivityTrails.Count > 0)
                {
                    //Even if the selection is kept, it may not be calculated
                    //Check that current selection is OK
                    CheckSetCurrentList(this.m_currentActivityTrails, progressBar);
                }

                if (this.m_currentActivityTrails.Count == 0)
                {
                    //If last selected trail still has results, use all the trails
                    CheckSetCurrentList(this.m_prevSelectedTrails, progressBar);
                }

                //Try last auto selected (that should not be generated)
                if (this.m_currentActivityTrails.Count == 0)
                {
                    CheckSetCurrentList(this.m_prevActivityTrails, progressBar);
                }

                //Try matching name for reference activity
                if (this.m_currentActivityTrails.Count == 0)
                {
                    if (this.m_referenceActivity != null &&
                       !string.IsNullOrEmpty(this.m_referenceActivity.Name))
                    {
                        //Prev does not match, try matching names
                        foreach (ActivityTrail at in this.OrderedTrails())
                        {
                            if (at.Trail.Name.StartsWith(this.m_referenceActivity.Name))
                            {
                                CheckSetCurrentList(new List<ActivityTrail> { at }, progressBar);
                                if (this.m_currentActivityTrails.Count > 0)
                                {
                                    //Use first match, ordered trails should have best first
                                    break;
                                }
                            }
                        }
                    }
                }

                if (this.m_currentActivityTrails.Count == 0)
                {
                    //Try to get the best match from non-auto trails
                    //The ordered list should be basically sorted, so the best candidates should be first
                    int recalc = 3;
                    foreach (ActivityTrail at in this.OrderedTrails())
                    {
                        if (at.Status != TrailOrderStatus.MatchNoCalc)
                        {
                            if (this.CheckSetCurrentList(new List<ActivityTrail> { at }, progressBar) ||
                                recalc-- <= 0)
                            {
                                break;
                            }
                        }
                    }
                }

                //Last resort, use Reference Activity or Splits
                if (this.m_currentActivityTrails.Count == 0)
                {
                    if (this.m_activities.Count > Settings.MaxAutoSelectSplits)
                    {
                        //Many activities, select Reference Activity
                        this.CheckSetCurrentList(new List<ActivityTrail> { this.m_referenceActivityTrail }, progressBar);
                    }
                    else
                    {
                        foreach (ActivityTrail at in this.OrderedTrails())
                        {
                            if (at.Trail.IsSplits)
                            {
                                this.CheckSetCurrentList(new List<ActivityTrail> { at }, progressBar);
                                break;
                            }
                        }
                    }
                }
            }
        }

        //Set at automatic updates, to possibly limit calculations
        private bool m_AutomaticUpdate = false;
        public bool AutomaticUpdate
        {
            get { return m_AutomaticUpdate; }
            set
            {
                if (value && this.CurrentResultTreeList.Count > TrailsPlugin.Data.Settings.MaxAutoCalcResults)
                {
                    //Avoid sort on some fields that are heavy to calculate at auto updates
                    m_AutomaticUpdate = true;
                }
                else
                {
                    m_AutomaticUpdate = false;
                }
            }
        }

        public void ReCalcTrails(bool reCalc, System.Windows.Forms.ProgressBar progressBar)
        {
            foreach (ActivityTrail at in this.m_CurrentOrderedTrails)
            {
                int val = 0;
                if (progressBar != null)
                {
                    val = progressBar.Value;
                }

                //Is InBounds is separated here to simplify profiling
                //There is not much point in calculating generated trails
                //(reference could maybe be interesting to see no of matches)
                if (at.IsInBounds && at.Status != TrailOrderStatus.MatchNoCalc && at.Trail.TrailType != Trail.CalcType.ElevationPoints)
                {

                    if (reCalc ||
                        this.m_activities.Count * TrailData.AllTrails.Values.Count <=
                            Settings.MaxAutoCalcActivitiesTrails &&
                        at.Trail.TrailPriority >= -1)
                    {
                        at.CalcResults(progressBar);
                    }
                } 
                
                //Increase the progress, regardless if updated or not
                if (progressBar != null && val < progressBar.Maximum)
                {
                    progressBar.Value = val + 1;
                }
            }
            //Check that current trail is OK,
            CheckSetCurrentTrail(progressBar);
        }

        public void RecalculateAllTrails()
        {                
            //all trails must be recalculated, there is no possibility to recalculate for one trail only
            //(almost: Clear results for a certain activity followed by CalcTrail then sets status)
            //As activities are edit in single view normally, recalc time is not an issue
            //(except if results have been 
            //(if a user auto edits, there could be seconds of slowdown).
            Controller.TrailController.Instance.Reset();
            //Make sure reference activity is 'reasonable' in case the reference trail is selected
            Controller.TrailController.Instance.checkReferenceActivity(null);

            //Calculate trails - at least InBounds, set apropriate ActivityTrail
            Controller.TrailController.Instance.ReCalcTrails(false, null);
        }

        public IActivity ReferenceActivity
        {
            //No special set, implicitly from switching activity or setting result
            get
            {
                if (this.m_referenceActivity==null || !this.m_activities.Contains(this.m_referenceActivity))
                {
                    if (m_referenceTrailResult!= null && this.m_referenceTrailResult.Activity!= null &&
                        this.m_activities.Contains(this.m_referenceTrailResult.Activity))
                    {
                        this.m_referenceActivity = this.m_referenceTrailResult.Activity;
                    }
                    else if (this.m_activities.Count > 0)
                    {
                        this.m_referenceActivity = this.m_activities[0];
                    }
                }
                return this.m_referenceActivity;
            }
        }

        public TrailResult ReferenceTrailResult
        {
            set
            {
                m_referenceTrailResult = value;
                //Check that the value is OK, as well as set activity and possibly recalc ref trail
                this.checkReferenceTrailResult(null);
            }
            get
            {
                return this.m_referenceTrailResult;
            }
        }

        //check that the reference result is reasonable
        //Called after activities are updated, before trail is selected/calculated
        internal IActivity checkReferenceActivity(System.Windows.Forms.ProgressBar progressBar)
        {
            bool moreChecks = true;

            //Should always follow the trail result, if it exists
            if (m_referenceTrailResult != null)
            {
                if (this.m_activities.Contains(this.m_referenceTrailResult.Activity))
                {
                    //The ref result is at least possible, set ref activity
                    this.m_referenceActivity = m_referenceTrailResult.Activity;
                    //Save last used activity
                    this.m_referenceTrailResult.m_activityTrail.Trail.ReferenceActivity = m_referenceActivity;
                    //skip a Contains check...
                    moreChecks = false;
                }
                else
                {
                    //result not possible
                    this.m_referenceTrailResult = null;
                }
            }

            //If refAct no longer in activities, reset (if set from result or not does not matter)
            if (moreChecks && m_referenceActivity != null)
            {
                if (!this.m_activities.Contains(this.m_referenceActivity))
                {
                    this.m_referenceActivity = null;
                }
            }

            if (m_referenceActivity == null && this.m_activities.Count > 0)
            {
                if (this.PrimaryCurrentActivityTrail != null &&
                    this.PrimaryCurrentActivityTrail.Trail.ReferenceActivity != null &&
                    this.m_activities.Contains(this.PrimaryCurrentActivityTrail.Trail.ReferenceActivity))
                {
                    //Switch back to a trail with prev activities (not used in most usage patterns, but from m_prevSelectedTrails)
                    this.m_referenceActivity = this.PrimaryCurrentActivityTrail.Trail.ReferenceActivity;
                }
                else if (this.PrimaryCurrentActivityTrail != null &&
                         this.PrimaryCurrentActivityTrail.Trail.DefaultRefActivity != null)
                {
                    //Use the reference activity, must already have been inserted
                    this.m_referenceActivity = this.PrimaryCurrentActivityTrail.Trail.DefaultRefActivity;
                }
                else if (this.PrimaryCurrentActivityTrail == null)
                {
                    foreach (ActivityTrail at in this.m_prevSelectedTrails)
                    {
                        if (at.Trail.ReferenceActivity != null &&
                            this.m_activities.Contains(at.Trail.ReferenceActivity))
                        {
                            //Switch back to a trail with prev activities (not used in most usage patterns)
                            this.m_referenceActivity = at.Trail.ReferenceActivity;
                            break;
                        }
                    }
                }

                else
                {
                    //Last resort, use first activity
                    //This also covers the trivial situation with only one activity
                    this.m_referenceActivity = this.m_activities[0];
                }
            }

            //ReferenceActivity trail: ref activity possibly changed - reset calculations
            if (this.m_referenceActivity != null &&
                this.m_referenceActivity != this.m_referenceActivityTrail.Trail.ReferenceActivity)
            {
                this.m_referenceActivityTrail.Trail.ReferenceActivity = m_referenceActivity;
                this.m_referenceActivityTrail.Init();
            }

            return m_referenceActivity;
        }

        private TrailResult checkReferenceTrailResult(System.Windows.Forms.ProgressBar progressBar)
        {
            System.Diagnostics.Debug.Assert(m_currentActivityTrails != null);
            if (m_currentActivityTrails.Count > 0)
            {
                //Check that the ref is for current calculation
                if (this.CurrentStatus() <= TrailOrderStatus.MatchPartial)
                {
                    if (m_referenceTrailResult != null)
                    {
                        //Check if ref is in all results (may have changed, Contains() will not work)
                        IList<TrailResultWrapper> trs = TrailResultWrapper.SelectedItems(this.CurrentResultTreeList,
                            new List<TrailResult> { m_referenceTrailResult });
                        System.Diagnostics.Debug.Assert(trs != null);
                        if (trs.Count > 0)
                        {
                            m_referenceTrailResult = trs[0].Result;
                        }
                        else
                        {
                            m_referenceTrailResult = null;
                        }
                    }
                }

                //If the reference result is not set, try a result for current reference activity,
                // secondly the first result
                //No forced calculations, should already be done
                if (this.m_referenceTrailResult == null &&
                    this.m_referenceActivity != null &&
                    this.CurrentStatus() <= TrailOrderStatus.MatchPartial)
                {
                    TrailResult firstResult = null;

                    foreach (TrailResult tr in TrailResultWrapper.Results(this.CurrentResultTreeList))
                    {
                        if (firstResult == null)
                        {
                            firstResult = tr;
                        }
                        if (this.m_referenceActivity.Equals(tr.Activity))
                        {
                            this.m_referenceTrailResult = tr;
                            break;
                        }
                    }
                    if (this.m_referenceTrailResult == null && firstResult != null)
                    {
                        this.m_referenceTrailResult = firstResult;
                    }
                }
            }

            //Reference activity may have to be updated
            checkReferenceActivity(progressBar);

            return m_referenceTrailResult;
        }

        /***********************************************************/

        private IList<TrailResult> m_selectedResults = null;
        public IList<TrailResult> SelectedResults
        {
            get
            {
                return this.m_selectedResults;
            }
            set
            {
                this.m_selectedResults = value;
            }
        }

        /***********************************************************/

        private void NewTrail(Trail trail, System.Windows.Forms.ProgressBar progressBar)
        {
            ActivityTrail at = new ActivityTrail(this, trail);
            this.m_CurrentOrderedTrails.Add(at);

            //Select this trail
            this.m_currentActivityTrails.Clear();
            this.m_currentActivityTrails.Add(at);
            this.SetCurrentActivityTrail(this.m_currentActivityTrails, true, progressBar);
        }

        public bool AddTrail(Trail trail, System.Windows.Forms.ProgressBar progressBar)
        {
            bool result = false;
            if (TrailData.InsertTrail(trail))
            {
                NewTrail(trail, progressBar);
                result = true;
            } 
            else
            {
                result = false;
            }
            return result;
        }

        public bool UpdateTrail(Trail trail, System.Windows.Forms.ProgressBar progressBar)
        {
            bool result = false;
            if (TrailData.UpdateTrail(trail))
            {
                foreach (ActivityTrail at in this.m_CurrentOrderedTrails)
                {
                    if (at.Trail.Id == trail.Id)
                    {
                        at.Trail = trail;
                        at.Init();
                        break;
                    }
                }
                this.SetCurrentActivityTrail(this.m_currentActivityTrails, true, progressBar);
                result = true;
            }
            return result;
        }

        public bool DeleteCurrentTrail()
        {
            return this.DeleteTrail(this.PrimaryCurrentActivityTrail);
        }

        private bool DeleteTrail(ActivityTrail at)
        {
            bool result = false;
            if (at != null)
            {
                //Only the primary trail deleted
                at.Init();
                if (TrailData.DeleteTrail(at.Trail))
                {
                    this.m_CurrentOrderedTrails.Remove(at);
                    this.m_currentActivityTrails.Remove(at);
                    this.m_prevSelectedTrails.Remove(at);
                    at = null;
                    result = true;
                }
            }
            return result;
        }
    }
}
