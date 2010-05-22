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

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.SportTracks.UI;
using System.Collections.Generic;
using System.Drawing;
using ZoneFiveSoftware.SportTracks.Data;
using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Data.Fitness;

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
		public const string AvgPower = "AvgPower";
		public const string AvgGrade = "AvgGrade";
		public const string AvgSpeed = "AvgSpeed";
		public const string MaxSpeed = "MaxSpeed";
		public const string AvgPace = "AvgPace";
		public const string MaxPace = "MaxPace";

		public static IList<IListItem> ColumnDefs(IActivity activity) {
			Length.Units elevationUnit = PluginMain.GetApplication().SystemPreferences.ElevationUnits;
			Length.Units distanceUnit = PluginMain.GetApplication().SystemPreferences.DistanceUnits;
			if (activity != null) {	
				distanceUnit = activity.Category.DistanceUnits;
				elevationUnit = activity.Category.ElevationUnits;
			}

			IList<IListItem> columnDefs = new List<IListItem>();
			columnDefs.Add(new ListItemInfo(TrailResultColumnIds.Order, "#", "", 30, StringAlignment.Near));
			columnDefs.Add(new ListItemInfo(TrailResultColumnIds.StartTime, CommonResources.Text.LabelStartTime, "", 70, StringAlignment.Near));
			columnDefs.Add(new ListItemInfo(TrailResultColumnIds.EndTime, CommonResources.Text.LabelEndTime, "", 70, StringAlignment.Near));
			columnDefs.Add(new ListItemInfo(TrailResultColumnIds.Duration, CommonResources.Text.LabelDuration, "", 60, StringAlignment.Near));
			columnDefs.Add(new ListItemInfo(TrailResultColumnIds.Distance, CommonResources.Text.LabelDistance + " (" + Length.LabelAbbr(distanceUnit) + ")", "", 60, StringAlignment.Near));
			columnDefs.Add(new ListItemInfo(TrailResultColumnIds.AvgCadence, CommonResources.Text.LabelAvgCadence, "", 60, StringAlignment.Near));
			columnDefs.Add(new ListItemInfo(TrailResultColumnIds.AvgHR, CommonResources.Text.LabelAvgHR, "", 50, StringAlignment.Near));
			columnDefs.Add(new ListItemInfo(TrailResultColumnIds.MaxHR, CommonResources.Text.LabelMaxHR, "", 50, StringAlignment.Near));
			columnDefs.Add(new ListItemInfo(TrailResultColumnIds.ElevChg, CommonResources.Text.LabelElevationChange + " (" + Length.LabelAbbr(elevationUnit) + ")", "", 70, StringAlignment.Near));
			columnDefs.Add(new ListItemInfo(TrailResultColumnIds.AvgPower, CommonResources.Text.LabelAvgPower + " (" + CommonResources.Text.LabelWatts + ")", "", 70, StringAlignment.Near));
			columnDefs.Add(new ListItemInfo(TrailResultColumnIds.AvgGrade, CommonResources.Text.LabelAvgGrade, "", 70, StringAlignment.Near));
			columnDefs.Add(new ListItemInfo(TrailResultColumnIds.AvgSpeed, CommonResources.Text.LabelAvgSpeed + " (" + Utils.Units.GetSpeedUnitLabelForActivity(activity) + ")", "", 70, StringAlignment.Near));
			columnDefs.Add(new ListItemInfo(TrailResultColumnIds.MaxSpeed, "Max Speed (" + Utils.Units.GetSpeedUnitLabelForActivity(activity) + ")", "", 70, StringAlignment.Near));
			columnDefs.Add(new ListItemInfo(TrailResultColumnIds.AvgPace, CommonResources.Text.LabelAvgPace + " (" + Utils.Units.GetPaceUnitLabelForActivity(activity) + ")", "", 70, StringAlignment.Near));
			columnDefs.Add(new ListItemInfo(TrailResultColumnIds.MaxPace, "Max Pace (" + Utils.Units.GetPaceUnitLabelForActivity(activity) + ")", "", 70, StringAlignment.Near));

			return columnDefs;
		}
	}
}
