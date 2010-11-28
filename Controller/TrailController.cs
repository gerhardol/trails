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
		private TrailOrdered m_currentTrailOrdered = null;
		private string m_lastTrailId = null;
        private Data.TrailResult m_referenceTrailResult = null;
        private IActivity m_referenceActivity = null;

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
                    if (m_currentTrailOrdered != null)
                    {
                        m_lastTrailId = m_currentTrailOrdered.activityTrail.Trail.Id;
                    }
                    m_CurrentOrderedTrails = null;
                    m_currentTrailOrdered = null;
                    if (m_referenceActivity != null)
                    {
                        bool match = false;
                        foreach (IActivity activity in m_activities)
                        {
                            if (activity == m_referenceActivity)
                            {
                                match = true;
                                break;
                            }
                        }
                        if (!match)
                        {
                            m_referenceActivity = null;
                        }
                    }
                }
            }
        }

        public IActivity FirstActivity
        {
            get
            {
                if (m_activities != null && m_activities.Count > 0)
                {
                    return m_activities[0];
                }
                return null;
            }
        }
        public IActivity CurrentActivity {
            get
            {
                if (m_activities != null && m_activities.Count == 1)
                {
                    return m_activities[0];
                }
                return null;
			}
		}

        public TrailOrdered CurrentTrailOrdered
        {
            set
            {
                foreach (TrailOrdered to in OrderedTrails)
                {
                    if (to.activityTrail.Trail.Id == value.activityTrail.Trail.Id)
                    {
                        m_currentTrailOrdered = to;
                        //Trigger result recalculation if needed
                        setOrderedStatus(to, false, true);
                        break;
                    }
                }
            }
            get
            {
                checkCurrentTrailOrdered();
                if (m_currentTrailOrdered != null)
                {
                    setOrderedStatus(m_currentTrailOrdered);
                    //Avoid setting if there are "too many" activities
                    //as this triggers building the result list, which can take a while to build
                    if (!(m_currentTrailOrdered.status == TrailOrderStatus.MatchNoCalc ||
                         m_currentTrailOrdered.status == TrailOrderStatus.InBoundNoCalc))
                    {
                        return m_currentTrailOrdered;
                    }
                }
                    return null;
            }
        }
        public Data.ActivityTrail CurrentActivityTrail
        {
			get
            {
                ActivityTrail result = null;
                checkCurrentTrailOrdered();
                if (m_currentTrailOrdered != null)
                {
                    setOrderedStatus(m_currentTrailOrdered);
                    result = m_currentTrailOrdered.activityTrail;
                }
                return result;
            }
		}

        private void checkCurrentTrailOrdered()
        {
            if (m_currentTrailOrdered == null && m_activities.Count > 0)
            {
                //If last used trail had results, use it
                foreach (TrailOrdered to in OrderedTrails)
                {
                    if (to.activityTrail.Trail.Id == m_lastTrailId)
                    {
                        if (to.status <= TrailOrderStatus.InBoundNoCalc)
                        {
                            m_currentTrailOrdered = to;
                        }
                        break;
                    }
                }
            }
            if (m_currentTrailOrdered == null &&
                OrderedTrails.Count > 0)
            {
                //The ordered list should have the best match first
                //(but results may not be calculated)
                m_currentTrailOrdered = OrderedTrails[0];
            }
        }

        //The reference activity is normally related to the result
        public IActivity ReferenceActivity
        {
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
                m_referenceActivity = value.Activity; //No special setter
            }
            get
            {
                checkCurrentTrailOrdered();
                if (m_currentTrailOrdered == null)
                {
                    m_referenceTrailResult = null;
                }
                else
                {
                    //setOrderedStatus(m_currentTrailOrdered);
                    checkReferenceTrailResult(true);
                }
                return m_referenceTrailResult;
            }
        }

        public IActivity checkReferenceActivity(bool checkRef)
        {
            if (m_referenceActivity == null)
            {
                checkCurrentTrailOrdered();
                if (m_currentTrailOrdered != null)
                {
                    if (!m_currentTrailOrdered.activityTrail.Trail.IsReference)
                    {
                        TrailResult tr = checkReferenceTrailResult(checkRef);
                        if (tr != null)
                        {
                            m_referenceActivity = tr.Activity;
                        }
                    }
                    else if ((checkRef || !m_currentTrailOrdered.activityTrail.Trail.IsReference) &&
                        Activities.Count > 0 &&
                        null != m_currentTrailOrdered.activityTrail.Trail.ReferenceActivity)
                    {
                        foreach (IActivity activity in Activities)
                        {
                            if (activity == m_currentTrailOrdered.activityTrail.Trail.ReferenceActivity)
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
                    (checkRef || !m_currentTrailOrdered.activityTrail.Trail.IsReference) &&
                    m_currentTrailOrdered.status == TrailOrderStatus.Match)
                {
                    bool match = false;
                    foreach (Data.TrailResult tr in m_currentTrailOrdered.activityTrail.Results)
                    {
                        if (m_referenceTrailResult.Equals(tr) ||
                            m_referenceTrailResult.Equals(tr.ParentResult))
                        {
                            match = true;
                            break;
                        }
                    }
                    if (!match)
                    {
                        m_referenceTrailResult = null;
                    }
                }

                //If the reference result is not set, try a result for current activity
                if (m_referenceTrailResult == null &&
                    m_referenceActivity != null &&
                    (checkRef || !m_currentTrailOrdered.activityTrail.Trail.IsReference) &&
                    m_currentTrailOrdered.status == TrailOrderStatus.Match)
                {
                    foreach (Data.TrailResult tr in m_currentTrailOrdered.activityTrail.Results)
                    {
                        if (tr.Activity.Equals(m_referenceActivity))
                        {
                            m_referenceTrailResult = tr;
                            break;
                        }
                    }
                }
                if (m_referenceTrailResult == null &&
                    (checkRef || !m_currentTrailOrdered.activityTrail.Trail.IsReference) &&
                    m_currentTrailOrdered.status == TrailOrderStatus.Match &&
                    m_currentTrailOrdered.activityTrail.Results.Count > 0)
                {
                    m_referenceTrailResult = m_currentTrailOrdered.activityTrail.Results[0];
                }

            return m_referenceTrailResult;
        }

        private IList<TrailOrdered> m_CurrentOrderedTrails = null;
        public IList<TrailOrdered> OrderedTrails
        {
            get
            {
                if (m_CurrentOrderedTrails == null)
                {
                    getTrails();
                }
                else if (m_currentTrailOrdered != null)
                {
                    setOrderedStatus(m_currentTrailOrdered);
                }
                //Sort ordered trails, as it is used when choosing the initial trail
                ((List<TrailOrdered>)m_CurrentOrderedTrails).Sort();
                return m_CurrentOrderedTrails;
            }
        }

        private void getTrails()
        {
            Data.TrailResult.Reset();

            m_CurrentOrderedTrails = new List<TrailOrdered>();
            foreach (Data.Trail trail in PluginMain.Data.AllTrails.Values)
            {
                TrailOrdered to = new TrailOrdered(new TrailsPlugin.Data.ActivityTrail(Activities, trail), TrailOrderStatus.NoInfo);
                setOrderedStatus(to, true, false);
                m_CurrentOrderedTrails.Add(to);
            }
        }

        private void setOrderedStatus(TrailOrdered to)
        {
            setOrderedStatus(to, false, false);
        }
        private void setOrderedStatus(TrailOrdered to, bool isNew, bool isSelected)
        {
            //The status setting differs for selected 
            if (!isNew)
            {
                if (to.status == TrailOrderStatus.MatchNoCalc ||
                      to.status == TrailOrderStatus.InBoundNoCalc ||
                      to.status == TrailOrderStatus.NoInfo)
                {
                    if (isSelected ||
                        Activities.Count <= TrailsPlugin.Data.Settings.MaxAutoCalcResults)// &&
                        //!to.activityTrail.Trail.IsReference)
                    {
                        if (to.activityTrail.Results.Count > 0)
                        {
                            to.status = TrailOrderStatus.Match;
                        }
                        else
                        {
                            to.status = TrailOrderStatus.InBound;
                        }
                    }
                }
            }
            else
            {
                if (!isSelected && (to.activityTrail.Trail.MatchAll || to.activityTrail.Trail.IsReference))
                {
                    // Let Reference always match, to trigger possible recalc after
                    to.status = TrailOrderStatus.MatchNoCalc;
                }
                else if (to.activityTrail.IsInBounds)
                {
                    if (Activities.Count * PluginMain.Data.AllTrails.Values.Count <=
                        TrailsPlugin.Data.Settings.MaxAutoCalcActivitiesTrails)
                    {
                        if (to.activityTrail.Results.Count > 0)
                        {
                            to.status = TrailOrderStatus.Match;
                        }
                        else
                        {
                            to.status = TrailOrderStatus.InBound;
                        }
                    }
                    else
                    {
                        to.status = TrailOrderStatus.InBoundNoCalc;
                    }
                }
                else
                {
                    to.status = TrailOrderStatus.NotInBound;
                }
            }
        }

        public bool AddTrail(Data.Trail trail)
        {
			if (PluginMain.Data.InsertTrail(trail))
            {
                m_CurrentOrderedTrails = null;
				m_currentTrailOrdered = new TrailOrdered(new TrailsPlugin.Data.ActivityTrail(m_activities, trail), TrailOrderStatus.NoInfo);
				m_lastTrailId = trail.Id;
				return true;
			} 
            else
            {
				return false;
			}
		}


		public bool UpdateTrail(Data.Trail trail) {
			if (PluginMain.Data.UpdateTrail(trail)) {
                m_CurrentOrderedTrails = null;
				m_currentTrailOrdered = null;
                m_lastTrailId = trail.Id;
                return true;
			} else {
				return false;
			}
		}

		public bool DeleteCurrentTrail() {
            if (m_currentTrailOrdered != null &&
                PluginMain.Data.DeleteTrail(m_currentTrailOrdered.activityTrail.Trail))
            {
                m_CurrentOrderedTrails = null;
				m_currentTrailOrdered = null;
				m_lastTrailId = null;
				return true;
			} else {
				return false;
			}			
		}
	}
}
