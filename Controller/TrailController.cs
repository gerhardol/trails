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
        private IDictionary<IActivity, IGPSBounds> activityGps = new Dictionary<IActivity, IGPSBounds>();

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

                if (!keepSelectedTrail)
                {
                    this.m_currentActivityTrails.Clear();
                }
                this.Reset();
                //Make sure reference activity is 'reasonable' in case the reference trail is selected
                this.checkReferenceActivity(progressBar);

                //Calculate trails - at least InBounds, set aprpriate ActivityTrail
                this.ReCalcTrails(false, progressBar);
            }
        }

        public IGPSBounds GpsBoundsCache(IActivity activity)
        {
            if (!activityGps.ContainsKey(activity))
            {
                activity.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Activity_PropertyChanged);
                activityGps.Add(activity, GPSBounds.FromGPSRoute(activity.GPSRoute));
            }
            return activityGps[activity];
        }

        public void ClearGpsBoundsCache()
        {
            foreach (IActivity activity in activityGps.Keys)
            {
                //this.Activity_PropertyChanged(activity, null);
                foreach (ActivityTrail at in this.m_CurrentOrderedTrails)
                {
                    at.ClearResultsForActivity(activity);
                }
                activity.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(Activity_PropertyChanged);
            }
            activityGps.Clear();
        }

        void Activity_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //e is null at reset. For other this is called multiple times - only run once
            if (sender is IActivity && (e == null || e.PropertyName == "GPSRoute"))
            {
                IActivity activity = sender as IActivity;
                if(activity != null && activityGps.ContainsKey(activity))
                {
                    activityGps[activity] = null;
                    //activity.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(Activity_PropertyChanged);
                    foreach (ActivityTrail at in this.m_CurrentOrderedTrails)
                    {
                        at.ClearResultsForActivity(activity);
                    }
                }
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
            }
            return result;
        }

        private void SetCurrentActivityTrail(ActivityTrail activityTrail, bool manuallySet, System.Windows.Forms.ProgressBar progressBar)
        {
            SetCurrentActivityTrail(new List <ActivityTrail>{activityTrail}, manuallySet, progressBar);
        }

        public void SetCurrentActivityTrail(IList<ActivityTrail> activityTrails, bool manuallySet, System.Windows.Forms.ProgressBar progressBar)
        {
            //Create a copy of the trails to set - could be the lists cleared
            IList<ActivityTrail> activityTrails2 = new List<ActivityTrail>();
            foreach (ActivityTrail to in activityTrails)
            {
                activityTrails2.Add(to);
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
                //Assume fine if match also is not calculated
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
                       !string.IsNullOrEmpty(m_referenceActivity.Name))
                    {
                        //Prev does not match, try matching names
                        foreach (ActivityTrail at in this.OrderedTrails())
                        {
                            if (at.Trail.Name.StartsWith(this.m_referenceActivity.Name))
                            {
                                CheckSetCurrentList(new List<ActivityTrail> { at }, progressBar);
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

                //Last resort, use Reference Activity
                if (this.m_currentActivityTrails.Count == 0)
                {
                    this.CheckSetCurrentList(new List<ActivityTrail> { this.m_referenceActivityTrail }, progressBar);
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

                if (at.IsInBounds)
                {

                    if (reCalc ||
                        this.m_activities.Count * TrailData.AllTrails.Values.Count <=
                            Settings.MaxAutoCalcActivitiesTrails &&
                        at.Trail.IsAutoTryAll)
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

        public IActivity ReferenceActivity
        {
            //No special set, implicitly from switching activity or setting result
            get
            {
                return this.m_referenceActivity;
            }
        }

        public TrailResult ReferenceTrailResult
        {
            set
            {
                m_referenceTrailResult = value;
                if (value != null && this.m_referenceActivity != value.Activity)
                {
                    this.m_referenceActivityTrail.Init();
                    this.m_referenceActivity = value.Activity;
                    this.m_referenceActivityTrail.Trail.ReferenceActivity = this.m_referenceActivity;
                }
            }
            get
            {
                return this.m_referenceTrailResult;
            }
        }

        //check that the reference result is reasonable
        //Called after activities are updated, before trail is selected/calculated
        private IActivity checkReferenceActivity(System.Windows.Forms.ProgressBar progressBar)
        {
            IActivity prevRefActivity = this.m_referenceActivity;
            bool moreChecks = true;

            //Should always follow the trail result, if it exists
            if (m_referenceTrailResult != null)
            {
                if (this.m_activities.Contains(this.m_referenceTrailResult.Activity))
                {
                    m_referenceActivity = m_referenceTrailResult.Activity;
                    moreChecks = false;
                }
                else
                {
                    //Old result, not possible
                    m_referenceTrailResult = null;
                }
            }

            //If no longer in activities, reset (if set from result or not does not matter)
            if (moreChecks && m_referenceActivity != null)
            {
                if (!this.m_activities.Contains(this.m_referenceActivity))
                {
                    this.m_referenceActivity = null;
                }
            }

            //Last resort, use first activity
            //This also covers the trivial situation with only one activity
            if (m_referenceActivity == null && this.m_activities.Count > 0)
            {
                m_referenceActivity = this.m_activities[0];
            }

            //ref activity possibly changed - reset calculations
            if (this.m_referenceActivity != null && this.m_referenceActivity != prevRefActivity)
            {
                bool calculated = this.m_referenceActivityTrail.Status != TrailOrderStatus.MatchNoCalc;
                this.m_referenceActivityTrail.Init();
                this.m_referenceActivityTrail.Trail.ReferenceActivity = m_referenceActivity;
                if (calculated)
                {
                    this.m_referenceActivityTrail.CalcResults(progressBar);
                }
            }

            return m_referenceActivity;
        }

        private TrailResult checkReferenceTrailResult(System.Windows.Forms.ProgressBar progressBar)
        {
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

                    foreach (TrailResult tr in TrailResultWrapper.ParentResults(this.CurrentResultTreeList))
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

        public IList<ActivityTrail> OrderedTrails()
        {
            //Trail may be calculated in various situations, also when getting results,
            //It is therefore easier to keep resort here
            ((List<ActivityTrail>)m_CurrentOrderedTrails).Sort();
            return m_CurrentOrderedTrails;
        }

        public void Clear()
        {
            foreach (ActivityTrail at in m_CurrentOrderedTrails)
            {
                at.Clear(false);
            }
            //If the reference is no longer available, reset it when checking ref when using it
        }

        public void CurrentClear(bool onlyDisplay)
        {
            foreach (ActivityTrail t in this.CurrentActivityTrails)
            {
                t.Clear(onlyDisplay);
            }
        }

        private void NewTrail(Trail trail, System.Windows.Forms.ProgressBar progressBar)
        {
            ActivityTrail at = new ActivityTrail(this, trail);
            this.m_CurrentOrderedTrails.Add(at);

            this.SetCurrentActivityTrail(at, true, progressBar);
        }

        public bool AddTrail(Trail trail, System.Windows.Forms.ProgressBar progressBar)
        {
            if (TrailData.InsertTrail(trail))
            {
                NewTrail(trail, progressBar);
                return true;
			} 
            else
            {
				return false;
			}
		}

        public bool UpdateTrail(Trail trail, System.Windows.Forms.ProgressBar progressBar)
        {
            if (TrailData.UpdateTrail(trail))
            {
                foreach (ActivityTrail at in this.m_CurrentOrderedTrails)
                {
                    //Explicitly set - no checks for no of activities
                    if (at.Trail == trail)
                    {
                        at.Init();
                        at.CalcResults(progressBar);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
		}

		public bool DeleteCurrentTrail()
        {
            bool result = false;
            if (m_currentActivityTrails.Count > 0)
            {
                //Only the primary trail deleted
                ActivityTrail at = this.m_currentActivityTrails[0];
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
