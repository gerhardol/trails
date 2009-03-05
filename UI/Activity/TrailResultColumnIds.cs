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
