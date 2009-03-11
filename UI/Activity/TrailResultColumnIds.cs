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

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.SportTracks.UI;
using System.Collections.Generic;
using System.Drawing;
using ZoneFiveSoftware.SportTracks.Data;

namespace TrailsPlugin.UI {
	public class TrailResultColumnIds {
		public const string Order = "Order";
		public const string StartTime = "StartTime";
		public const string EndTime = "EndTime";
		public const string Duration = "Duration";
		public const string Distance = "Distance";
		public const string AvgCadence = "AvgCadence";
		public const string AvgHR = "AvgHR";
		public const string MaxHR = "MaxHR";
		public const string ElevChg = "ElevChg";

		private static IList<IListItem> m_columnDefs = null;
		public static IList<IListItem> ColumnDefs() {
			if (m_columnDefs == null) {
				m_columnDefs = new List<IListItem>();
				m_columnDefs.Add(new ListItemInfo(TrailResultColumnIds.Order, "#", "", 30, StringAlignment.Near));
				m_columnDefs.Add(new ListItemInfo(TrailResultColumnIds.StartTime, "Start", "", 70, StringAlignment.Near));
				m_columnDefs.Add(new ListItemInfo(TrailResultColumnIds.EndTime, "End", "", 70, StringAlignment.Near));
				m_columnDefs.Add(new ListItemInfo(TrailResultColumnIds.Duration, "Duration", "", 60, StringAlignment.Near));
				m_columnDefs.Add(new ListItemInfo(TrailResultColumnIds.Distance, "Distance", "", 60, StringAlignment.Near));
				m_columnDefs.Add(new ListItemInfo(TrailResultColumnIds.AvgCadence, "Avg Cadence", "", 60, StringAlignment.Near));
				m_columnDefs.Add(new ListItemInfo(TrailResultColumnIds.AvgHR, "Avg HR", "", 50, StringAlignment.Near));
				m_columnDefs.Add(new ListItemInfo(TrailResultColumnIds.MaxHR, "Max HR", "", 50, StringAlignment.Near));
				m_columnDefs.Add(new ListItemInfo(TrailResultColumnIds.ElevChg, "Elev. Chg", "", 50, StringAlignment.Near));
			}
			return m_columnDefs;
		}
	}
}
