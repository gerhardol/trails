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

namespace TrailsPlugin.Controller 
{
    public enum TrailOrderStatus
    {
        Used, InBoundNoCalc, InBound, NotInBound, NoInfo
    }
    public class TrailOrdered : IComparable
    {
        public TrailOrdered(Data.ActivityTrail activityTrail, TrailOrderStatus status)
        { this.activityTrail = activityTrail; this.status = status; }
        public Data.ActivityTrail activityTrail;
        public TrailOrderStatus status;

        public int CompareTo(object obj)
        {
            if (!(obj is TrailOrdered))
            {
                return 1;
            }
            TrailOrdered to2 = obj as TrailOrdered;
            if(status != to2.status){
                return status > to2.status? 1: -1;
            }
            else if (status == TrailOrderStatus.Used)
            {
                if (activityTrail.Trail.MatchAll != to2.activityTrail.Trail.MatchAll)
                {
                    return (activityTrail.Trail.MatchAll) ? 1 : -1;
                }
                else if (activityTrail.Results.Count != to2.activityTrail.Results.Count)
                {
                    return (activityTrail.Results.Count < to2.activityTrail.Results.Count) ? 1 : -1;
                }
                else
                {
                    float e1 = 0;
                    foreach (Data.TrailResult tr in activityTrail.Results)
                    {
                        e1 += tr.DistDiff;
                    }
                    e1 = e1 / activityTrail.Results.Count;
                    float e2 = 0;
                    foreach (Data.TrailResult tr in activityTrail.Results)
                    {
                        e2 += tr.DistDiff;
                    }
                    e2 = e2 / activityTrail.Results.Count;
                    //No check if equal here
                    return e1 < e2 ? 1 : -1;
                }
            }
            return activityTrail.Trail.Name.CompareTo(to2.activityTrail.Trail.Name);
        }
    }

    public class TrailController
    {
        const int MaxAutoCalcActivities = 20;
        const int MaxAutoCalcResults = MaxAutoCalcActivities*10;
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
		private Data.ActivityTrail m_currentActivityTrail = null;
		private string m_lastTrailId = null;
		private IList<Data.ActivityTrail> m_activityTrails = null;
        private Data.TrailResult m_referenceTrailResult = null;

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
                    if (m_currentActivityTrail != null)
                    {
                        m_lastTrailId = m_currentActivityTrail.Trail.Id;
                    }
                    m_currentActivityTrail = null;
                    m_activityTrails = null;
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

		public Data.ActivityTrail CurrentActivityTrail {
			set {
				m_currentActivityTrail = value;
                m_referenceTrailResult = null;
			}
			get {
				if (m_currentActivityTrail == null && m_activities.Count>0) {
					IList<Data.ActivityTrail> trails = this.TrailsInBounds;
					foreach (Data.ActivityTrail t in trails) {
						if (t.Trail.Id == m_lastTrailId) {
							if (t.Results.Count > 0) {
								m_currentActivityTrail = t;
							}							
							break;
						}
					}
				}
                if (m_currentActivityTrail == null && Activities.Count < MaxAutoCalcResults &&
                    m_CurrentOrderedTrails != null && m_CurrentOrderedTrails.Count > 0)
                {
                    //The ordered list should have the best match first
                    //Avoid setting if there are "too many" activities
                    //as this triggers building the result list, which  can take a while to build
                    m_currentActivityTrail = m_CurrentOrderedTrails[0].activityTrail;
                }
                return m_currentActivityTrail;
            }
		}

        public Data.TrailResult ReferenceTrail
        {
            set
            {
                m_referenceTrailResult = value;
            }
            get
            {
                if (m_currentActivityTrail == null || m_currentActivityTrail.Results.Count == 0)
                {
                    return null;
                }
                if (m_referenceTrailResult == null)
                {
                    m_referenceTrailResult = m_currentActivityTrail.Results[0];
                }
                //Note: No check that the result exists, it could be a subsplit
                return m_referenceTrailResult;
            }
        }

        private IList<TrailOrdered> m_CurrentOrderedTrails = null;
        public IList<TrailOrdered> OrderedTrails
        {
            get
            {
                if (m_CurrentOrderedTrails == null || m_activityTrails == null)
                {
                    getTrails();
                }
                CheckOrderedTrails();
                return m_CurrentOrderedTrails;
            }
        }


		public IList<Data.ActivityTrail> TrailsInBounds {
			get {
				if(m_activityTrails == null) {
                    getTrails();
				}
				return m_activityTrails;
			}
		}
        private void CheckOrderedTrails()
        {
            if (m_currentActivityTrail != null && m_CurrentOrderedTrails.Count > 0)
            {
                foreach (TrailOrdered to in m_CurrentOrderedTrails)
                {
                    if (m_currentActivityTrail.Equals(to.activityTrail))
                    {
                        if (!(to.status == TrailOrderStatus.Used ||
                            to.status == TrailOrderStatus.InBound ||
                            to.status == TrailOrderStatus.NotInBound))
                        {
                            if (to.activityTrail.Results.Count > 0)
                            {
                                to.status = TrailOrderStatus.Used;
                            }
                            else if (to.activityTrail.IsInBounds)
                            {
                                to.status = TrailOrderStatus.InBound;
                            }
                            else
                            {
                                to.status = TrailOrderStatus.NotInBound;
                            }
                        }
                        break;
                    }
                }
            }
            ((List<TrailOrdered>)m_CurrentOrderedTrails).Sort();
        }

        //wrapper for m_activityTrails, m_CurrentOrderedTrails
        private void getTrails()
        {
            Data.TrailResult.Reset();
            m_activityTrails = new List<Data.ActivityTrail>();
            m_CurrentOrderedTrails = new List<TrailOrdered>();
            foreach (Data.Trail trail in PluginMain.Data.AllTrails.Values)
            {
                Data.ActivityTrail at = new TrailsPlugin.Data.ActivityTrail(Activities, trail);
                if (Activities.Count <= MaxAutoCalcActivities)
                {
                    if (trail.IsInBounds(Activities))
                    {
                        if (at.Results.Count > 0)
                        {
                            m_activityTrails.Add(at);
                            m_CurrentOrderedTrails.Add(new TrailOrdered(at, TrailOrderStatus.Used));
                        }
                        else
                        {
                            m_CurrentOrderedTrails.Add(new TrailOrdered(at, TrailOrderStatus.InBound));
                        }
                    }
                    else
                    {
                        m_CurrentOrderedTrails.Add(new TrailOrdered(at, TrailOrderStatus.NotInBound));
                    }
                }
                else
                {
                    if (at.IsInBounds)
                    {
                        m_CurrentOrderedTrails.Add(new TrailOrdered(at, TrailOrderStatus.InBoundNoCalc));
                    }
                    else
                    {
                        m_CurrentOrderedTrails.Add(new TrailOrdered(at, TrailOrderStatus.NotInBound));
                    }
                }
            }
            //Sort ordered trails, as it is used when choosing the initial trail
            CheckOrderedTrails();
        }

        public bool AddTrail(Data.Trail trail)
        {
			if (PluginMain.Data.InsertTrail(trail))
            {
				m_activityTrails = null;
				m_currentActivityTrail = new TrailsPlugin.Data.ActivityTrail(m_activities, trail);
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
				m_lastTrailId = trail.Id;
				m_currentActivityTrail = null;
				m_activityTrails = null;
				return true;
			} else {
				return false;
			}
		}

		public bool DeleteCurrentTrail() {
			if (PluginMain.Data.DeleteTrail(m_currentActivityTrail.Trail)) {
				m_activityTrails = null;
				m_currentActivityTrail = null;
				m_lastTrailId = null;
				return true;
			} else {
				return false;
			}			
		}
	}
}
