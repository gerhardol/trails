using System.Collections.Generic;
using ZoneFiveSoftware.Common.Data.Fitness;
using System.Xml;
using System.Xml.Serialization;

namespace TrailsPlugin {
	public class TrailSettings {

		private SortedDictionary<string, Data.Trail> m_AllTrails = new SortedDictionary<string, Data.Trail>();

		private static TrailSettings m_instance = null;
		public static TrailSettings Instance {
			get {
				if (m_instance == null) {
					m_instance = new TrailSettings();
				}
				return m_instance;
			}
		}

		private TrailSettings() {
			loadSettings();
		}

		~TrailSettings() {			
		}

		private void loadSettings() {
		}

		public SortedDictionary<string, Data.Trail> AllTrails {
			get {
				return m_AllTrails;
			}
		}

		public IList<Data.Trail> TravelledTrails(IActivity activity) {
			// todo
			return new List<Data.Trail>();
		}

		public bool InsertTrail(Data.Trail trail) {
			if (m_AllTrails.ContainsKey(trail.name)) {
				return false;
			} else {
				m_AllTrails.Add(trail.name, trail);
				return true;
			}
		}
		public bool UpdateTrail(Data.Trail trail) {
			if (m_AllTrails.ContainsKey(trail.name)) {
				m_AllTrails[trail.name] = trail;
				return true;
			} else {				
				return false;
			}
		}
		public bool DeleteTrail(Data.Trail trail) {
			if (m_AllTrails.ContainsKey(trail.name)) {
				m_AllTrails.Remove(trail.name);
				return true;
			} else {
				return false;
			}
		}
	}
}
