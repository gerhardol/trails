/*
Copyright (C) 2009 Brendan Doherty

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
		private string m_lastTrailId = null;
        private Data.TrailResult m_referenceTrailResult = null;
        private IActivity m_referenceActivity = null;
        private IActivity m_lastReferenceActivity = null;
        private IList<ActivityTrail> m_CurrentOrderedTrails = null;

        public IList<IActivity> Activities
        {
            get
            {
                return m_activities;
            }
            set
            {
                if (m_activities != value)
                {
                    m_activities = value;
                    this.Reset();
                }
            }
        }

        //Reset calculations, without changing selected activities
        public void Reset()
        {
            if (m_currentActivityTrail != null)
            {
                m_lastTrailId = m_currentActivityTrail.Trail.Id;
            }
            if (m_CurrentOrderedTrails != null)
            {
                for (int i = 0; i < m_CurrentOrderedTrails.Count; i++)
                {
                    m_CurrentOrderedTrails[i].Reset();
                    //m_CurrentOrderedTrails[i] = null;
                }
            }
            m_CurrentOrderedTrails = null;
            m_currentActivityTrail = null;
            m_lastReferenceActivity = m_referenceActivity;
            m_referenceActivity = null;
        }
        //Used when no calculation is forced, ie displaying
        public ActivityTrail CurrentActivityTrailDisplayed
        {
            get
            {
                if (CurrentActivityTrail != null)
                {
                    //Avoid setting if there are "too many" activities
                    //as this triggers building the result list, which can take a while to build
                    if (m_currentActivityTrail.IsNoCalc)
                    {
                        return null;
                    }
                }
                return m_currentActivityTrail;
            }
        }

        public Data.ActivityTrail CurrentActivityTrail
        {
            set
            {
                foreach (ActivityTrail to in OrderedTrails)
                {
                    if (to.Trail.Id == value.Trail.Id)
                    {
                        m_currentActivityTrail = to;
                        //Trigger result recalculation if needed
                        to.CalcResults();
                        break;
                    }
                }
            }
            get
            {
                checkCurrentTrailOrdered(true);
                return m_currentActivityTrail;
            }
		}

        private void checkCurrentTrailOrdered(bool checkRef)
        {
            if (Activities.Count > 0)
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
                                m_currentActivityTrail = to;
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
                                    m_currentActivityTrail = to;
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
                    m_currentActivityTrail = OrderedTrails[0];
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
                        Activities.Count * Data.TrailData.AllTrails.Values.Count <=
                        TrailsPlugin.Data.Settings.MaxAutoCalcActivitiesTrails)
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
                        Activities.Count > 0 &&
                        null != m_currentActivityTrail.Trail.ReferenceActivityNoCalc)
                    {
                        //If reference trail, set the ref from the trail
                        foreach (IActivity activity in Activities)
                        {
                            if (activity == m_currentActivityTrail.Trail.ReferenceActivityNoCalc)
                            {
                                m_referenceActivity = activity;
                            }
                        }
                    }

                    if (m_referenceActivity == null && Activities.Count > 0)
                    {
                        m_referenceActivity = Activities[0];
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
                if (TrailResultWrapper.SelectedItems(m_currentActivityTrail.ResultTreeList, 
                    new List<TrailResult>{m_referenceTrailResult}).Count==0)
                {
                    m_referenceTrailResult = null;
                }
            }

            //check that the ref is for current results
            if (m_currentActivityTrail != null)
            {
                if(!m_currentActivityTrail.Results.Contains(m_referenceTrailResult))
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
                foreach (Data.TrailResult tr in m_currentActivityTrail.Results)
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
                m_currentActivityTrail.Status <= TrailOrderStatus.MatchPartial &&
                m_currentActivityTrail.Results.Count > 0)
            {
                m_referenceTrailResult = m_currentActivityTrail.Results[0];
            }

            return m_referenceTrailResult;
        }

        public IList<ActivityTrail> OrderedTrails
        {
            get
            {
                if (m_CurrentOrderedTrails == null)
                {
                    getTrails();
                }
                ((List<ActivityTrail>)m_CurrentOrderedTrails).Sort();
                return m_CurrentOrderedTrails;
            }
        }

        private void getTrails()
        {
            Data.TrailResult.Reset();

            m_CurrentOrderedTrails = new List<ActivityTrail>();
            foreach (Data.Trail trail in Data.TrailData.AllTrails.Values)
            {
                ActivityTrail to = new TrailsPlugin.Data.ActivityTrail(this, trail);
                if (to.IsInBounds)
                {
                    if (to.Status != TrailOrderStatus.MatchNoCalc &&
                        Activities.Count * Data.TrailData.AllTrails.Values.Count <=
                        TrailsPlugin.Data.Settings.MaxAutoCalcActivitiesTrails)
                    {
                        to.CalcResults();
                    }
                }
                m_CurrentOrderedTrails.Add(to);
            }
        }

        private void NewTrail(Data.Trail trail)
        {
            m_currentActivityTrail = new TrailsPlugin.Data.ActivityTrail(this, trail);
            m_currentActivityTrail.CalcResults();
            if (m_CurrentOrderedTrails == null)
            {
                getTrails();
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
				m_lastTrailId = null;
				return true;
			} else {
				return false;
			}			
		}
	}
}
