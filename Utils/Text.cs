/******************************************************************************

    This file is part of TrailsPlugin.

    TrailsPlugin is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TrailsPlugin is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TrailsPlugin.  If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

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
