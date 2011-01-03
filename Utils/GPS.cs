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

using System.Collections.Generic;
using ZoneFiveSoftware.Common.Data.GPS;

namespace TrailsPlugin.Utils {
	public class GPS {
		public static IGPSPoint LocationToPoint(IGPSLocation location) {
			return new GPSPoint(location.LatitudeDegrees, location.LongitudeDegrees, 0);
		}

		public static IGPSLocation PointToLocation(IGPSPoint point) {
			return new GPSLocation(point.LatitudeDegrees, point.LongitudeDegrees);
		}

#if !ST_2_1
        public static IGPSBounds GetBounds(IList<IList<IGPSPoint>> trks)
        {
            GPSBounds area = null;
            foreach (IList<IGPSPoint> trk in trks)
            {
                GPSBounds area2 = GPSBounds.FromGPSPoints(trk);
                if (area2 != null)
                {
                    if (area == null)
                    {
                        area = area2;
                    }
                    else
                    {
                        area = (GPSBounds)area.Union(area2);
                    }
                }
            }
            return area;
        }
#endif
    }
}
