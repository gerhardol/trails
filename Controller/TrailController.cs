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
		static public TrailController Instance {
			get {
				if (m_instance == null) {
					m_instance = new TrailController();
				}
				return m_instance;
			}
		}

		private TrailController() {
		}
			 
        private IList<IActivity> m_activities = new List<IActivity>();
        private ActivityTrail m_currentActivityTrail = null;
        private IList<ActivityTrail> m_currentActivityTrails = new List<ActivityTrail>();
        private Guid m_lastTrailId = Guid.Empty;
        private Data.TrailResult m_referenceTrailResult = null;
        private IActivity m_referenceActivity = null;
        private IActivity m_lastReferenceActivity = null;
        private IList<ActivityTrail> m_CurrentOrderedTrails = null;

        //Update activities, should keep trail and reference
        public IList<IActivity> Activities
        {
            get
            {
                return m_activities;
            }
            set
            {
                this.ActivitiesNoAuto = value;
                //TBD: Need to check trail and reference?
            }
        }

        //Set activities, do not force calculation
        public IList<IActivity> ActivitiesNoAuto
        {
            set
            {
                if (this.m_activities != value)
                {
                    if (value == null || value.Count == 1 && value[0] == null)
                    {
                        this.m_activities.Clear();
                    }
                    else
                    {
                        this.m_activities = value;
                    }
                    this.Reset(true);
                }
            }
        }

        //Reset calculations, without changing selected activities
        public void Reset(bool all)
        {
            if (m_CurrentOrderedTrails != null)
            {
                for (int i = 0; i < m_CurrentOrderedTrails.Count; i++)
                {
                    m_CurrentOrderedTrails[i].Reset();
                }
            }
            m_CurrentOrderedTrails = null;
            if (m_currentActivityTrail != null)
            {
                m_lastTrailId = m_currentActivityTrail.Trail.Id;
            }

            if (all)
            {
                m_currentActivityTrail = null;
                this.m_currentActivityTrails.Clear();
                m_lastReferenceActivity = m_referenceActivity;
                m_referenceActivity = null;
            }
        }

        //Used when no calculation is forced, ie displaying
        public bool CurrentActivityTrailIsDisplayed
        {
            get
            {
                if (CurrentActivityTrail != null)
                {
                    //Avoid setting if there are "too many" activities
                    //as this triggers building the result list, which can take a while to build
                    if (m_currentActivityTrail.IsNoCalc)
                    {
                        return false;
                    }
                }
                return m_currentActivityTrail != null;
            }
        }

        public IList<ActivityTrail> CurrentActivityTrail_Multi
        {
            get
            {
                if (CurrentActivityTrail != null)
                {
                    //Avoid setting if there are "too many" activities
                    //as this triggers building the result list, which can take a while to build
                    if (m_currentActivityTrail.IsNoCalc)
                    {
                        //TBD return null;
                    }
                }
                return m_currentActivityTrails;
            }
        }

        public Data.ActivityTrail CurrentActivityTrailNoChecks
        {
            get
            {
                return m_currentActivityTrail;
            }
        }

        public Data.ActivityTrail CurrentActivityTrail
        {
            set
            {
                SetCurrentActivityTrail(value, false, null);
            }
            get
            {
                checkCurrentTrailOrdered(true);
                return m_currentActivityTrail;
            }
        }

        public IList<TrailResultWrapper> CurrentResultTreeList
        {
            get
            {
                IList<TrailResultWrapper> result = new List<TrailResultWrapper>();
                foreach (ActivityTrail at in this.CurrentActivityTrail_Multi)
                {
                    foreach (TrailResultWrapper tw in at.ResultTreeList)
                    {
                        result.Add(tw);
                    }
                }
                return result;
            }
        }

        public void SetCurrentActivityTrail(Data.ActivityTrail value, bool addMode, System.Windows.Forms.ProgressBar progressBar)
        {
            if (!addMode)
            {
                m_currentActivityTrails.Clear();
            }

            foreach (ActivityTrail to in GetOrderedTrails(progressBar, false))
            {
                if (to.Trail != null && to.Trail.Id == value.Trail.Id)
                {
                    if (m_currentActivityTrails.Count == 0)
                    {
                        m_currentActivityTrail = to;
                    }
                    m_currentActivityTrails.Add(to);
                    //Trigger result recalculation if needed
                    if (progressBar != null)
                    {
                        progressBar.Value = 0;
                        progressBar.Maximum = this.m_activities.Count;
                    }
                    to.CalcResults(progressBar);
                    break;
                }
            }
        }

        private void checkCurrentTrailOrdered(bool checkRef)
        {
            if (this.m_activities.Count > 0)
            {
                if (m_currentActivityTrail == null && m_activities.Count > 0)
                {
                    //If last used trail had results, use it
                    foreach (ActivityTrail to in OrderedTrails)
                    {
                        //Avoid switch automatically to Reference, as this is the first in list and often to selected and then will stick
                        //If Splits or HighScore is selected, follow it
                        if (to.Trail.Id == m_lastTrailId && !to.Trail.IsReference)
                        {
                            if (to.Status <= TrailOrderStatus.InBoundMatchPartial)
                            {
                                this.SetCurrentActivityTrail(to, false, null);
                            }
                            break;
                        }
                    }
                }
                if (m_currentActivityTrail == null &&
                    OrderedTrails.Count > 0)
                {
                    if (checkRef)
                    {
                        checkReferenceActivity(false);
                    }
                    if (m_referenceActivity != null &&
                        m_referenceActivity.Name != "")
                    {
                        //Try matching names
                        foreach (ActivityTrail to in OrderedTrails)
                        {
                            if (to.Trail.Name == m_referenceActivity.Name)
                            {
                                if (to.IsInBounds)
                                {
                                    this.SetCurrentActivityTrail(to, false, null);
                                }
                                break;
                            }
                        }
                    }
                }
                if (m_currentActivityTrail == null &&
                    OrderedTrails.Count > 0)
                {
                    //The ordered list should have the best match first
                    //(but results may not be calculated)
                    this.SetCurrentActivityTrail(OrderedTrails[0], false, null);
                }
                if (m_currentActivityTrail != null)
                {
                    //Reset Ref trail
                    if (m_currentActivityTrail.Trail.IsReference &&
                        m_referenceActivity != m_currentActivityTrail.Trail.ReferenceActivityNoCalc)
                    {
                        m_currentActivityTrail.Trail.ReferenceActivityNoCalc = null;
                    }
                }
                if (m_currentActivityTrail != null)
                {
                    //Precalculate results if not too heavy
                    //Ref trail may require recalc, only recalc when requested
                    if ((checkRef || !m_currentActivityTrail.Trail.IsReference) &&
                        this.m_activities.Count <= TrailsPlugin.Data.Settings.MaxAutoCalcActitiesSingleTrail &&
                        m_currentActivityTrail.Status >= TrailOrderStatus.MatchNoCalc)
                    {
                        m_currentActivityTrail.CalcResults();
                    }
                }
            }
        }

        //The reference activity is normally related to the result
        public IActivity ReferenceActivity
        {
            //No special setter, same as result
            get
            {
                return checkReferenceActivity(true);
            }
        }

        public Data.TrailResult ReferenceTrailResultNoChecks
        {
            get
            {
                if (m_referenceTrailResult == null)
                {
                    return this.ReferenceTrailResult;
                }
                return m_referenceTrailResult;
            }
        }

        public Data.TrailResult ReferenceTrailResult
        {
            set
            {
                m_referenceTrailResult = value;
                if (value != null)
                {
                    m_referenceActivity = value.Activity;
                }
            }
            get
            {
                checkCurrentTrailOrdered(false);
                if (m_currentActivityTrail == null)
                {
                    m_referenceTrailResult = null;
                }
                else
                {
                    checkReferenceTrailResult(true);
                }
                return m_referenceTrailResult;
            }
        }

        public IActivity checkReferenceActivity(bool checkRef)
        {
            //The ref trail follows the activities if possible, set from other info otherwise
            if (m_referenceActivity == null)
            {
                if (m_lastReferenceActivity != null)
                {
                    //Check if last is still valid
                    foreach (IActivity activity in m_activities)
                    {
                        if (activity == m_lastReferenceActivity)
                        {
                            m_referenceActivity = m_lastReferenceActivity;
                            break;
                        }
                    }
                }
            }
            //One activity - use it
            if (m_referenceActivity == null &&
                    m_activities != null && m_activities.Count == 1)
            {
                m_referenceActivity = m_activities[0];
            }

            if (m_referenceActivity == null)
            {
                if (checkRef)
                {
                    checkCurrentTrailOrdered(false);
                }
                if (m_currentActivityTrail != null)
                {
                    if (!m_currentActivityTrail.Trail.IsReference)
                    {
                        //If trail is not reference, try the reference result
                        TrailResult tr = checkReferenceTrailResult(checkRef);
                        if (tr != null)
                        {
                            m_referenceActivity = tr.Activity;
                        }
                    }
                    else if ((checkRef || !m_currentActivityTrail.Trail.IsReference) &&
                        this.m_activities.Count > 0 &&
                        null != m_currentActivityTrail.Trail.ReferenceActivityNoCalc)
                    {
                        //If reference trail, set the ref from the trail
                        foreach (IActivity activity in this.m_activities)
                        {
                            if (activity == m_currentActivityTrail.Trail.ReferenceActivityNoCalc)
                            {
                                m_referenceActivity = activity;
                            }
                        }
                    }

                    if (m_referenceActivity == null && this.m_activities.Count > 0)
                    {
                        m_referenceActivity = this.m_activities[0];
                    }
                }
            }
            return m_referenceActivity;
        }

        private Data.TrailResult checkReferenceTrailResult(bool checkRef)
        {
            //Check that the ref is for current result
            if (m_referenceTrailResult != null &&
                (checkRef || !m_currentActivityTrail.Trail.IsReference) &&
                m_currentActivityTrail.Status <= TrailOrderStatus.MatchPartial)
            {
                if (TrailResultWrapper.SelectedItems(this.CurrentResultTreeList, 
                    new List<TrailResult>{m_referenceTrailResult}).Count==0)
                {
                    m_referenceTrailResult = null;
                }
            }

            //check that the ref is for current results
            if (m_currentActivityTrail != null)
            {
                if(m_currentActivityTrail.Status == TrailOrderStatus.Match &&
                    !TrailResultWrapper.AllResults(m_currentActivityTrail.ResultTreeList).Contains(m_referenceTrailResult))
                {
                    m_referenceTrailResult = null;
                }
            }

            //If the reference result is not set, try a result for current activity
            if (m_referenceTrailResult == null &&
                m_referenceActivity != null &&
                (checkRef || !m_currentActivityTrail.Trail.IsReference) &&
                m_currentActivityTrail.Status <= TrailOrderStatus.MatchPartial)
            {
                foreach (Data.TrailResult tr in TrailResultWrapper.ParentResults(m_currentActivityTrail.ResultTreeList))
                {
                    if (tr.Activity.Equals(m_referenceActivity))
                    {
                        m_referenceTrailResult = tr;
                        break;
                    }
                }
            }

            if (m_referenceTrailResult == null &&
                (checkRef || !m_currentActivityTrail.Trail.IsReference) &&
                m_currentActivityTrail.Status >= TrailOrderStatus.MatchNoCalc)
            {
                m_currentActivityTrail.CalcResults();
            }
            if (m_referenceTrailResult == null &&
                (checkRef || !m_currentActivityTrail.Trail.IsReference) &&
                m_currentActivityTrail.Status <= TrailOrderStatus.MatchPartial &&
                TrailResultWrapper.ParentResults(m_currentActivityTrail.ResultTreeList).Count > 0)
            {
                m_referenceTrailResult = TrailResultWrapper.ParentResults(m_currentActivityTrail.ResultTreeList)[0];
            }

            return m_referenceTrailResult;
        }

        public IList<ActivityTrail> GetOrderedTrails(System.Windows.Forms.ProgressBar progressBar, bool force)
        {
            if (m_CurrentOrderedTrails == null)
            {
                getTrails(progressBar, force);
            }
            ((List<ActivityTrail>)m_CurrentOrderedTrails).Sort();
            return m_CurrentOrderedTrails;
        }

        public IList<ActivityTrail> OrderedTrails
        {
            get
            {
                return GetOrderedTrails(null, false);
            }
        }

        private void getTrails(System.Windows.Forms.ProgressBar progressBar, bool force)
        {
            Data.TrailResult.Reset();

            m_CurrentOrderedTrails = new List<ActivityTrail>();
            foreach (Data.Trail trail in Data.TrailData.AllTrails.Values)
            {
                ActivityTrail to = new TrailsPlugin.Data.ActivityTrail(this, trail);
                if (to.IsInBounds)
                {
                    if (to.Status != TrailOrderStatus.MatchNoCalc &&
                        (force ||
                        this.m_activities.Count * Data.TrailData.AllTrails.Values.Count <=
                        TrailsPlugin.Data.Settings.MaxAutoCalcActivitiesTrails) &&
                        trail.IsAutoTryAll)
                    {
                        to.CalcResults(progressBar);
                    }
                    else if (progressBar != null)
                    {
                        progressBar.Value += this.m_activities.Count;
                    }
                }
                else if (progressBar != null)
                {
                    progressBar.Value += this.m_activities.Count;
                }
                m_CurrentOrderedTrails.Add(to);
            }
        }

        public void Clear()
        {
            if (m_CurrentOrderedTrails != null)
            {
                foreach (ActivityTrail t in m_CurrentOrderedTrails)
                {
                    t.Clear(false);
                }
            }
            //If the reference is no longer available, reset it when checking ref when using it
        }

        public void CurrentClear(bool onlyDisplay)
        {
            foreach (ActivityTrail t in this.CurrentActivityTrail_Multi)
            {
                t.Clear(onlyDisplay);
            }
        }

        private void NewTrail(Data.Trail trail)
        {
            this.SetCurrentActivityTrail(new TrailsPlugin.Data.ActivityTrail(this, trail), false, null);
            m_currentActivityTrail.CalcResults();
            if (m_CurrentOrderedTrails == null)
            {
                getTrails(null, false);
            }
            m_CurrentOrderedTrails.Add(m_currentActivityTrail);
            m_lastTrailId = trail.Id;
        }

        public bool AddTrail(Data.Trail trail)
        {
            if (Data.TrailData.InsertTrail(trail))
            {
                NewTrail(trail);
                return true;
			} 
            else
            {
				return false;
			}
		}

		public bool UpdateTrail(Data.Trail trail)
        {
            if (Data.TrailData.UpdateTrail(trail))
            {
                if (m_currentActivityTrail != null)
                {
                    m_CurrentOrderedTrails.Remove(m_currentActivityTrail);
                }
                NewTrail(trail);
                return true;
            }
            else
            {
                return false;
            }
		}

		public bool DeleteCurrentTrail() {
            if (m_currentActivityTrail != null &&
                Data.TrailData.DeleteTrail(m_currentActivityTrail.Trail))
            {
                if (m_currentActivityTrail != null)
                {
                    m_CurrentOrderedTrails.Remove(m_currentActivityTrail);
                }
                m_currentActivityTrail = null;
                this.m_currentActivityTrails.Clear();
                m_lastTrailId = Guid.Empty;
				return true;
			} else {
				return false;
			}			
		}
	}
}
