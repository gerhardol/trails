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

//Used in both Trails and Matrix plugin

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
