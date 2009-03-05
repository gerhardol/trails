using ZoneFiveSoftware.Common.Data.GPS;

namespace TrailsPlugin.Utils {
	public class GPS {
		public static IGPSPoint LocationToPoint(IGPSLocation location) {
			return new GPSPoint(location.LatitudeDegrees, location.LongitudeDegrees, 0);
		}

		public static IGPSLocation PointToLocation(IGPSPoint point) {
			return new GPSLocation(point.LatitudeDegrees, point.LongitudeDegrees);
		}
	}
}
