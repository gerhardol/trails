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
				return m_instance;
			}
		}

		private IActivity m_currentActivity = null;
		private Data.ActivityTrail m_currentTrail = null;
		private string m_previousTrailId;
		private IList<Data.ActivityTrail> m_activityTrails = null;

		public IActivity CurrentActivity {
			get {
				return m_currentActivity;
			}
			set {
				if (m_currentActivity != value) {
					m_currentActivity = value;
					if (m_currentTrail != null) {
						m_previousTrailId = m_currentTrail.Id;
					}
					m_currentTrail = null;
				}
			}
		}

		public Data.ActivityTrail CurrentTrail {
			get {
				if (m_currentTrail == null) {

				}
				return m_currentTrail;
			}
		}


		public IList<Data.ActivityTrail> TrailsInBounds {
			get {
				IList<Data.ActivityTrail> trails = new List<Data.ActivityTrail>();
				if (m_currentActivity != null) {
					IGPSBounds gpsBounds = GPSBounds.FromGPSRoute(m_currentActivity.GPSRoute);
					foreach (Data.Trail trail in PluginMain.Data.AllTrails.Values) {
						if (trail.IsInBounds(gpsBounds)) {
							trails.Add(new Data.ActivityTrail(m_currentActivity, trail));
						}
					}
				}
				return trails;
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
	}
}
