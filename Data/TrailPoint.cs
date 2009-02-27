using ZoneFiveSoftware.Common.Data.GPS;


namespace TrailsPlugin.Data {
	public class TrailPoint {
		private IGPSLocation m_gpsLocation;
		public TrailPoint(IGPSLocation gpsLocation) {
			m_gpsLocation = gpsLocation;
		}
		public string Latitude {
			get {
				return m_gpsLocation.LatitudeDegrees.ToString();
			}
		}
		public string Longitude {
			get {
				return m_gpsLocation.LongitudeDegrees.ToString();
			}
		}

	}
}
