using System.Collections.Generic;
using ZoneFiveSoftware.Common.Data.Fitness;

namespace TrailsPlugin.Controller {
	class TrailsController {
		private static TrailsController m_instance = null;
		public static TrailsController Instance {
			get {
				if (m_instance == null) {
					m_instance = new TrailsController();
				}
				return m_instance;
			}
		}

		public IList<Data.Trail> AllTrails {
			get {
				// todo
				return new List<Data.Trail>();
			}
		}

		public IList<Data.Trail> TravelledTrails(IActivity activity) {
			// todo
			return new List<Data.Trail>();
		}

		public void InsertTrail(Data.Trail trail) {
			// todo
		}
		public void UpdateTrail(Data.Trail trail) {
			// todo
		}
		public void DeleteTrail(Data.Trail trail) {
			// todo
		}
	}
}
