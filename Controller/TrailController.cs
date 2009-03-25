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
			 
		private IActivity m_currentActivity = null;
		private Data.ActivityTrail m_currentTrail = null;
		private string m_lastTrailId = null;
		private IList<Data.ActivityTrail> m_activityTrails = null;

		public IActivity CurrentActivity {
			get {
				return m_currentActivity;
			}
			set {
				if (m_currentActivity != value) {
					m_currentActivity = value;
					if (m_currentTrail != null) {
						m_lastTrailId = m_currentTrail.Trail.Id;
					}
					m_currentTrail = null;
					m_activityTrails = null;
				}
			}
		}

		public Data.ActivityTrail CurrentActivityTrail {
			set {
				m_currentTrail = value;
			}
			get {
				if (m_currentTrail == null && m_currentActivity != null) {
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
						foreach (Data.ActivityTrail t in trails) {
							if (t.Results.Count > 0) {
								m_currentTrail = t;
								break;
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


		public IList<Data.ActivityTrail> TrailsInBounds {
			get {
				if(m_activityTrails == null) {					
					if (m_currentActivity != null) {
						m_activityTrails = new List<Data.ActivityTrail>();
						IGPSBounds gpsBounds = GPSBounds.FromGPSRoute(m_currentActivity.GPSRoute);
						foreach (Data.Trail trail in PluginMain.Data.AllTrails.Values) {
							if (trail.IsInBounds(gpsBounds)) {
								m_activityTrails.Add(new Data.ActivityTrail(m_currentActivity, trail));
							}
						}
					}
				}
				return m_activityTrails;
			}
		}

		public IList<Data.ActivityTrail> TrailsWithResults {
			get {
				SortedList<long, Data.ActivityTrail> trails = new SortedList<long, Data.ActivityTrail>();
				if (m_currentActivity != null) {
					foreach (Data.ActivityTrail trail in this.TrailsInBounds) {
						IList<Data.TrailResult> results = trail.Results;
						if (results.Count > 0) {
							trails.Add(results[0].StartTime.Ticks, trail);
						}
					}
				}
				return trails.Values;
			}
		}

		public bool AddTrail(Data.Trail trail) {
			if (PluginMain.Data.InsertTrail(trail)) {
				m_activityTrails = null;
				m_currentTrail = new TrailsPlugin.Data.ActivityTrail(m_currentActivity, trail);
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
