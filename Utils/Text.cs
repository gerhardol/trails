using System;
using System.Collections.Generic;
using System.Text;

namespace TrailsPlugin.Utils {
	class Text {
		public static string ToString(System.TimeSpan time) {
			if (time.TotalHours < 1.0) {
				int i1 = time.Seconds;
				return System.String.Concat(time.Minutes, ":", i1.ToString("00"));
			}
			object[] objArr = new object[5];
			objArr[0] = System.Math.Floor(time.TotalHours);
			objArr[1] = ":";
			int i2 = time.Minutes;
			objArr[2] = i2.ToString("00");
			objArr[3] = ":";
			int i3 = time.Seconds;
			objArr[4] = i3.ToString("00");
			return System.String.Concat(objArr);
		}

		public static string ToString(ZoneFiveSoftware.Common.Data.GPS.IGPSPoint point, ZoneFiveSoftware.Common.Data.GPS.GPSLocation.Units units) {
			string s = "+4";
			switch (units) {
				case ZoneFiveSoftware.Common.Data.GPS.GPSLocation.Units.Decimal3:
					s = "+3";
					break;

				case ZoneFiveSoftware.Common.Data.GPS.GPSLocation.Units.Decimal4:
					s = "+4";
					break;

				case ZoneFiveSoftware.Common.Data.GPS.GPSLocation.Units.Minutes:
					s = "m-1";
					break;

				case ZoneFiveSoftware.Common.Data.GPS.GPSLocation.Units.MinutesSeconds:
					s = "m0";
					break;
			}
			return point.ToString(s);
		}
	}
}
