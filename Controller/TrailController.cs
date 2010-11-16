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

namespace TrailsPlugin.Controller {
	class TrailController {
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
		private Data.ActivityTrail m_currentTrail = null;
		private string m_lastTrailId = null;
		private IList<Data.ActivityTrail> m_activityTrails = null;

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
                    if (m_currentTrail != null)
                    {
                        m_lastTrailId = m_currentTrail.Trail.Id;
                    }
                    m_currentTrail = null;
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
				m_currentTrail = value;
			}
			get {
				if (m_currentTrail == null && m_activities.Count>0) {
					IList<Data.ActivityTrail> trails = this.TrailsInBounds;
					foreach (Data.ActivityTrail t in trails) {
						if (t.Trail.Id == m_lastTrailId) {
							if (t.Results.Count > 0) {
								m_currentTrail = t;
							}							
							break;
						}
					}
					if (m_currentTrail == null) {
                        float bestMatch = float.PositiveInfinity;
						foreach (Data.ActivityTrail t in trails) {
                            if (t.Results.Count > 0)
                            {
                                float currMatch = 0;
                                foreach (Data.TrailResult r in t.Results)
                                {
                                    currMatch += r.DistDiff;
                                }
                                currMatch = currMatch / t.Results.Count;
                                if (currMatch < bestMatch)
                                {
                                    bestMatch = currMatch;
                                    m_currentTrail = t;
                                }
                                //	break;
                            }
						}

					}
					if (m_currentTrail == null && trails.Count > 0) {
						m_currentTrail = trails[0];
					}
				}
				return m_currentTrail;
			}
		}

        private IList<Data.ActivityTrail> m_CurrentOrderedTrails = null;
        public IList<Data.ActivityTrail> OrderedTrails
        {
            get
            {
                if (m_CurrentOrderedTrails == null || m_activityTrails == null)
                {
                }
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

        //wrapper for m_activityTrails, m_CurrentOrderedTrails - previously separate
        private void getTrails()
        {
            SortedList<double, Data.ActivityTrail> trailsUsed = new SortedList<double, Data.ActivityTrail>();
            SortedList<string, Data.ActivityTrail> trailsInBounds = new SortedList<string, Data.ActivityTrail>();
            SortedList<string, Data.ActivityTrail> trailsNotInBound = new SortedList<string, Data.ActivityTrail>();

            m_activityTrails = new List<Data.ActivityTrail>();
            foreach (Data.Trail trail in PluginMain.Data.AllTrails.Values)
            {
                Data.ActivityTrail at = new TrailsPlugin.Data.ActivityTrail(Activities, trail);
                if (trail.IsInBounds(Activities))
                {
                    m_activityTrails.Add(at);
                    if (at.Results.Count > 0)
                    {
                        double key = at.Results[0].StartTime.TotalSeconds;
                        while (trailsUsed.ContainsKey(key))
                        {
                            key++;
                        }
                        trailsUsed.Add(key, at);
                    }
                    else
                    {
                        trailsInBounds.Add(at.Trail.Name, at);
                    }
                }
                else
                {
                    trailsNotInBound.Add(at.Trail.Name, at);
                }
            }

            m_CurrentOrderedTrails = new List<Data.ActivityTrail>();
            foreach (Data.ActivityTrail t in trailsUsed.Values)
            {
                m_CurrentOrderedTrails.Add(t);
            }
            foreach (Data.ActivityTrail t in trailsInBounds.Values)
            {
                m_CurrentOrderedTrails.Add(t);
            }
            foreach (Data.ActivityTrail t in trailsNotInBound.Values)
            {
                m_CurrentOrderedTrails.Add(t);
            }
        }
		public bool AddTrail(Data.Trail trail) {
			if (PluginMain.Data.InsertTrail(trail)) {
				m_activityTrails = null;
				m_currentTrail = new TrailsPlugin.Data.ActivityTrail(m_activities, trail);
				m_lastTrailId = trail.Id;
				return true;
			} else {
				return false;
			}
		}


		public bool UpdateTrail(Data.Trail trail) {
			if (PluginMain.Data.UpdateTrail(trail)) {
				m_lastTrailId = trail.Id;
				m_currentTrail = null;
				m_activityTrails = null;
				return true;
			} else {
				return false;
			}
		}

		public bool DeleteCurrentTrail() {
			if (PluginMain.Data.DeleteTrail(m_currentTrail.Trail)) {
				m_activityTrails = null;
				m_currentTrail = null;
				m_lastTrailId = null;
				return true;
			} else {
				return false;
			}			
		}
	}
}
