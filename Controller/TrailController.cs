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

		private TrailController()
        {
		}
			 
        private IList<IActivity> m_activities = new List<IActivity>();
        private IList<ActivityTrail> m_currentActivityTrails = new List<ActivityTrail>();
        private IList<Guid> m_lastTrailIds = new List<Guid>();
        private TrailResult m_referenceTrailResult = null;
        private TrailResult m_lastReferenceTrailResult = null;
        private IActivity m_referenceActivity = null;
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
            m_lastTrailIds.Clear();
            foreach(ActivityTrail at in this.CurrentActivityTrail_Multi)
            {
                m_lastTrailIds.Add(at.Trail.Id);
            }

            if (all)
            {
                this.m_currentActivityTrails.Clear();
                this.m_lastReferenceTrailResult = this.m_referenceTrailResult;
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
                    if (m_currentActivityTrails[0].IsNoCalc)
                    {
                        return false;
                    }
                }
                return m_currentActivityTrails.Count > 0;
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
                    if (m_currentActivityTrails[0].IsNoCalc)
                    {
                        //TBD return null;
                    }
                }
                return m_currentActivityTrails;
            }
        }

        public ActivityTrail CurrentActivityTrail
        {
            set
            {
                SetCurrentActivityTrail(value, false, null);
            }
            get
            {
                checkCurrentTrailOrdered(true);
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

        private Trail CurrentTrail
        {
            get
            {
                checkCurrentTrailOrdered(true);
                if (m_currentActivityTrails.Count == 0)
                {
                    return null;
                }
                else
                {
                    return m_currentActivityTrails[0].Trail;
                }
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

        private TrailOrderStatus CurrentStatus()
        {
            TrailOrderStatus result = TrailOrderStatus.NoInfo;
            foreach (ActivityTrail at in this.CurrentActivityTrail_Multi)
            {
                if (at.Status < result)
                {
                    result = at.Status;
                }
            }
            return result;
        }

        private void CurrentCalcResults()
        {
            foreach (ActivityTrail at in this.CurrentActivityTrail_Multi)
            {
                at.CalcResults();
            }
        }

        public void SetCurrentActivityTrail(ActivityTrail value, bool addMode, System.Windows.Forms.ProgressBar progressBar)
        {
            if (!addMode)
            {
                m_currentActivityTrails.Clear();
            }

            foreach (ActivityTrail to in GetOrderedTrails(progressBar, false))
            {
                if (to.Trail != null && to.Trail.Id == value.Trail.Id)
                {
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
                if (m_currentActivityTrails.Count == 0 && m_activities.Count > 0)
                {
                    //If last used trail had results, use it
                    foreach (Guid g in m_lastTrailIds)
                    {
                        foreach (ActivityTrail to in OrderedTrails)
                        {
                            //Avoid switch automatically to Reference, as this is the first in list and often to selected and then will stick
                            //If Splits or HighScore is selected, follow it
                            if (g == to.Trail.Id && !to.Trail.IsReference)
                            {
                                if (to.Status <= TrailOrderStatus.InBoundMatchPartial)
                                {
                                    //m_currentActivityTrails is empty, no need to check m_lastTrailIds.count > 1
                                    this.SetCurrentActivityTrail(to, true, null);
                                }
                                break;
                            }
                        }
                    }
                }

                if (m_currentActivityTrails.Count == 0 &&
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

                if (m_currentActivityTrails.Count == 0 &&
                    OrderedTrails.Count > 0)
                {
                    //The ordered list should have the best match first
                    //(but results may not be calculated)
                    this.SetCurrentActivityTrail(OrderedTrails[0], false, null);
                }

                //Reset Ref trail
                foreach (ActivityTrail at in this.CurrentActivityTrail_Multi)
                {
                    if (at.Trail.IsReference &&
                        m_referenceActivity != at.Trail.ReferenceActivityNoCalc)
                    {
                        at.Trail.ReferenceActivityNoCalc = null;
                    }
                }


                foreach (ActivityTrail at in this.CurrentActivityTrail_Multi)
                {
                    //Precalculate results if not too heavy
                    //Ref trail may require recalc, only recalc when requested
                    if ((checkRef || !at.Trail.IsReference) &&
                        this.m_activities.Count <= Settings.MaxAutoCalcActitiesSingleTrail &&
                        at.Status >= TrailOrderStatus.MatchNoCalc)
                    {
                        at.CalcResults();
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

        public TrailResult ReferenceTrailResultNoChecks
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

        public TrailResult ReferenceTrailResult
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
                if (m_currentActivityTrails.Count == 0)
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
                if (this.m_lastReferenceTrailResult != null)
                {
                    //Check if last activity is still valid
                    foreach (IActivity activity in m_activities)
                    {
                        if (activity == m_lastReferenceTrailResult.Activity)
                        {
                            m_referenceActivity = m_lastReferenceTrailResult.Activity;
                            break;
                        }
                    }
                }
            }

            //If only one activity - use it
            if (m_referenceActivity == null && m_activities.Count == 1)
            {
                m_referenceActivity = m_activities[0];
            }

            if (m_referenceActivity == null)
            {
                if (checkRef)
                {
                    checkCurrentTrailOrdered(false);
                }
                foreach (ActivityTrail at in this.CurrentActivityTrail_Multi)
                {
                    if (!at.Trail.IsReference)
                    {
                        //If trail is not reference, try the reference result
                        TrailResult tr = checkReferenceTrailResult(checkRef);
                        if (tr != null)
                        {
                            m_referenceActivity = tr.Activity;
                        }
                    }
                    else if ((checkRef || !at.Trail.IsReference) &&
                        this.m_activities.Count > 0 &&
                        null != at.Trail.ReferenceActivityNoCalc)
                    {
                        //If reference trail, set the ref from the trail
                        foreach (IActivity activity in this.m_activities)
                        {
                            if (activity == at.Trail.ReferenceActivityNoCalc)
                            {
                                m_referenceActivity = activity;
                            }
                        }
                    }
                }
            }

            //Last resort, use first activity
            if (m_referenceActivity == null && this.m_activities.Count > 0)
            {
                m_referenceActivity = this.m_activities[0];
            }

            return m_referenceActivity;
        }

        private TrailResult checkReferenceTrailResult(bool checkRef)
        {
            //TBD: Rewrite IsReference handling
            if (m_currentActivityTrails.Count > 0)
            {
                //Check that the ref is for current result
                if ((checkRef || !this.CurrentTrail.IsReference) &&
                    this.CurrentStatus() <= TrailOrderStatus.MatchPartial)
                {
                    if (m_referenceTrailResult != null)
                    {
                        //Check if ref is in all results
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
                    //Use last as backup
                    if (m_referenceTrailResult == null && this.m_lastReferenceTrailResult != null)
                    {
                        IList<TrailResultWrapper> trs = TrailResultWrapper.SelectedItems(this.CurrentResultTreeList,
                        new List<TrailResult> { this.m_lastReferenceTrailResult });
                        if (trs.Count > 0)
                        {
                            m_referenceTrailResult = trs[0].Result;
                        }

                    }
                }

                //check that the ref is for current results
                //if (m_referenceTrailResult != null && this.CurrentStatus() == TrailOrderStatus.Match)
                //{
                //    //Calculation may be redone - Contains() will not match
                //    TrailResult newRef = null;
                //    foreach (TrailResult tr in TrailResultWrapper.AllResults(this.CurrentResultTreeList))
                //    {
                //        if (m_referenceTrailResult.CompareTo(tr) == 0)
                //        {
                //            newRef = tr;
                //            break;
                //        }
                //        //Try last ref as second option
                //        if (this.m_lastReferenceTrailResult != null && m_lastReferenceTrailResult.CompareTo(tr) == 0)
                //        {
                //            newRef = tr;
                //        }
                //    }
                //    m_referenceTrailResult = newRef;
                //}

                //If the reference result is not set, try a result for current activity
                if (m_referenceTrailResult == null &&
                    m_referenceActivity != null &&
                    (checkRef || !this.CurrentTrail.IsReference) &&
                    this.CurrentStatus() <= TrailOrderStatus.MatchPartial)
                {
                    foreach (TrailResult tr in TrailResultWrapper.ParentResults(this.CurrentResultTreeList))
                    {
                        if (tr.Activity.Equals(m_referenceActivity))
                        {
                            m_referenceTrailResult = tr;
                            break;
                        }
                    }
                }

                if (m_referenceTrailResult == null &&
                    (checkRef || !this.CurrentTrail.IsReference) &&
                    this.CurrentStatus() >= TrailOrderStatus.MatchNoCalc)
                {
                    this.CurrentCalcResults();
                }
                if (m_referenceTrailResult == null &&
                    (checkRef || !this.CurrentTrail.IsReference) &&
                    this.CurrentStatus() <= TrailOrderStatus.MatchPartial &&
                    TrailResultWrapper.ParentResults(this.CurrentResultTreeList).Count > 0)
                {
                    m_referenceTrailResult = TrailResultWrapper.ParentResults(this.CurrentResultTreeList)[0];
                }
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
            TrailResult.Reset();

            m_CurrentOrderedTrails = new List<ActivityTrail>();
            foreach (Trail trail in TrailData.AllTrails.Values)
            {
                ActivityTrail to = new ActivityTrail(this, trail);
                if (to.IsInBounds)
                {
                    if (to.Status != TrailOrderStatus.MatchNoCalc &&
                        (force ||
                        this.m_activities.Count * TrailData.AllTrails.Values.Count <=
                        Settings.MaxAutoCalcActivitiesTrails) &&
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

        private void NewTrail(Trail trail)
        {
            ActivityTrail t = new ActivityTrail(this, trail);
            this.SetCurrentActivityTrail(t, false, null);
            this.CurrentCalcResults();
            if (m_CurrentOrderedTrails == null)
            {
                getTrails(null, false);
            }
            m_CurrentOrderedTrails.Add(t);
        }

        public bool AddTrail(Trail trail)
        {
            if (TrailData.InsertTrail(trail))
            {
                NewTrail(trail);
                return true;
			} 
            else
            {
				return false;
			}
		}

		public bool UpdateTrail(Trail trail)
        {
            if (TrailData.UpdateTrail(trail))
            {
                //Assume the primary trail was edited
                if (m_currentActivityTrails.Count > 0)
                {
                    m_CurrentOrderedTrails.Remove(m_currentActivityTrails[0]);
                }
                NewTrail(trail);
                return true;
            }
            else
            {
                return false;
            }
		}

		public bool DeleteCurrentTrail()
        {
            if (m_currentActivityTrails.Count > 0 &&
                TrailData.DeleteTrail(this.m_currentActivityTrails[0].Trail))
            {
                m_CurrentOrderedTrails.Remove(m_currentActivityTrails[0]);
                m_currentActivityTrails[0] = null;
                this.m_currentActivityTrails.RemoveAt(0);
                this.m_lastTrailIds.RemoveAt(0);
				return true;
			} 
            else
            {
				return false;
			}			
		}
	}
}
