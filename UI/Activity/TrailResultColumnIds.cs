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
using System.Collections.Generic;
using System.Drawing;
using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Data.Fitness;
using TrailsPlugin;
#if ST_2_1
//IListItem
using ZoneFiveSoftware.SportTracks.UI;
using ZoneFiveSoftware.SportTracks.Data;
#endif

namespace TrailsPlugin.UI {
#if !ST_2_1
    public class ListColumnDefinition : IListColumnDefinition
    {
        public ListColumnDefinition(string id, string text, string groupName, int width, StringAlignment align)
        {
            this.align = align;
            this.groupName = groupName;
            this.id = id;
            this.width = width;
            this.text = text;
        }
        private StringAlignment align;
        public StringAlignment Align
        {
            get
            {
                return align;
            }
        }
        string groupName;
        public string GroupName
        {
            get
            {
                return groupName;
            }
        }
        string id;
        public string Id
        {
            get
            {
                return id;
            }
        }
        int width;
        public int Width
        {
            get
            {
                return width;
            }
        }
        string text;
        public string Text(string id)
        {
            return text;
        }
    }
#endif
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

#if ST_2_1
		public static IList<IListItem> ColumnDefs(IActivity activity)
        {
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
#else
        public static ICollection<IListColumnDefinition> ColumnDefs(IActivity activity)
        {
            Length.Units elevationUnit = PluginMain.GetApplication().SystemPreferences.ElevationUnits;
			Length.Units distanceUnit = PluginMain.GetApplication().SystemPreferences.DistanceUnits;
			if (activity != null) {	
				distanceUnit = activity.Category.DistanceUnits;
				elevationUnit = activity.Category.ElevationUnits;
			}

            IList<IListColumnDefinition> columnDefs = new List<IListColumnDefinition>();
            columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Order, "#", "", 30, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.StartTime, CommonResources.Text.LabelStartTime, "", 70, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.EndTime, CommonResources.Text.LabelEndTime, "", 70, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Duration, CommonResources.Text.LabelDuration, "", 60, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Distance, CommonResources.Text.LabelDistance + " (" + Length.LabelAbbr(distanceUnit) + ")", "", 60, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgCadence, CommonResources.Text.LabelAvgCadence, "", 60, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgHR, CommonResources.Text.LabelAvgHR, "", 50, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.MaxHR, CommonResources.Text.LabelMaxHR, "", 50, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.ElevChg, CommonResources.Text.LabelElevationChange + " (" + Length.LabelAbbr(elevationUnit) + ")", "", 70, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgPower, CommonResources.Text.LabelAvgPower + " (" + CommonResources.Text.LabelWatts + ")", "", 70, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgGrade, CommonResources.Text.LabelAvgGrade, "", 70, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgSpeed, CommonResources.Text.LabelAvgSpeed + " (" + Utils.Units.GetSpeedUnitLabelForActivity(activity) + ")", "", 70, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.MaxSpeed, Properties.Resources.Column_MaxSpeed+" (" + Utils.Units.GetSpeedUnitLabelForActivity(activity) + ")", "", 70, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgPace, CommonResources.Text.LabelAvgPace + " (" + Utils.Units.GetPaceUnitLabelForActivity(activity) + ")", "", 70, StringAlignment.Near));
            columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.MaxPace, Properties.Resources.Column_MaxPace+" (" + Utils.Units.GetPaceUnitLabelForActivity(activity) + ")", "", 70, StringAlignment.Near));
			return columnDefs;
		}
#endif
	}
}
