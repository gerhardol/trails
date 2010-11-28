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
using System.ComponentModel;
using ZoneFiveSoftware.Common.Data.Fitness;

#if ST_2_1
//IListItem
using ZoneFiveSoftware.SportTracks.UI;
using ZoneFiveSoftware.SportTracks.Data;
#endif

namespace TrailsPlugin.Data {
#if ST_2_1
    public interface IListColumnDefinition : IListItem { }
#endif

    public class ListColumnDefinition :  IListColumnDefinition
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
        public override string ToString()
        {
            return text;
        }
    }

    public class TrailResultColumnIds {
        public const string Order = "Order";
        public const string Color = "Color";
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
        public const string FastestSpeed = "FastestSpeed";
        public const string AvgPace = "AvgPace";
        public const string FastestPace = "FastestPace";
        public const string Name = "Name";
        public const string Location = "Location";
        public const string Category = "Category";

#if ST_2_1
        public static ICollection<IListItem> ColumnDefs_ST2(IActivity activity, bool mult)
        {
            IList<IListItem> columnDefs_ST2 = new List<IListItem>();
            foreach (IListColumnDefinition columnDefs in ColumnDefs(activity, mult))
            {
                columnDefs_ST2.Add(columnDefs);
            }
            return columnDefs_ST2;
        }
#endif
        public static ICollection<IListColumnDefinition> ColumnDefs(IActivity activity, bool mult)
        {
            IList<IListColumnDefinition> columnDefs = new List<IListColumnDefinition>();
            columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Order, "#", "", 32, StringAlignment.Near));
            int w = mult ? 115 : 70;
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.StartTime, CommonResources.Text.LabelStartTime, "", w, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.EndTime, CommonResources.Text.LabelEndTime, "", 70, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Duration, CommonResources.Text.LabelDuration, "", 60, StringAlignment.Near));
            columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Distance, CommonResources.Text.LabelDistance + " (" + Utils.Units.GetDistanceLabel(activity) + ")", "", 60, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgCadence, CommonResources.Text.LabelAvgCadence, "", 60, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgHR, CommonResources.Text.LabelAvgHR, "", 50, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.MaxHR, CommonResources.Text.LabelMaxHR, "", 50, StringAlignment.Near));
            columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.ElevChg, CommonResources.Text.LabelElevationChange + " (" + Utils.Units.GetElevationLabel(activity) + ")", "", 70, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgPower, CommonResources.Text.LabelAvgPower + " (" + CommonResources.Text.LabelWatts + ")", "", 70, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgGrade, CommonResources.Text.LabelAvgGrade, "", 70, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgSpeed, CommonResources.Text.LabelAvgSpeed + " (" + Utils.Units.GetSpeedLabel(activity) + ")", "", 70, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.FastestSpeed, CommonResources.Text.LabelFastestSpeed+" (" + Utils.Units.GetSpeedLabel(activity) + ")", "", 70, StringAlignment.Near));
			columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgPace, CommonResources.Text.LabelAvgPace + " (" + Utils.Units.GetPaceLabel(activity) + ")", "", 70, StringAlignment.Near));
            columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.FastestPace, CommonResources.Text.LabelFastestPace + " (" + Utils.Units.GetPaceLabel(activity) + ")", "", 70, StringAlignment.Near));
            columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Name, CommonResources.Text.LabelName, "", 70, StringAlignment.Near));
            columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Location, CommonResources.Text.LabelLocation, "", 70, StringAlignment.Near));
            columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Category, CommonResources.Text.LabelCategory, "", 70, StringAlignment.Near));

            return columnDefs;
		}
        public static ICollection<IListColumnDefinition> PermanentMultiColumnDefs()
        {
            IList<IListColumnDefinition> columnDefs = new List<IListColumnDefinition>();
            columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Color, "", "", 25, StringAlignment.Near));

            return columnDefs;
        }

        public static int Compare(TrailResult x, TrailResult y)
        {
            int result = (TrailsPlugin.Data.Settings.SummaryViewSortDirection == ListSortDirection.Ascending ? 1 : -1);

            if (TrailsPlugin.Data.Settings.SummaryViewSortColumn == TrailResultColumnIds.Name)
            {
                result *= x.Activity.Name.CompareTo(y.Activity.Name);
            }
            else if (TrailsPlugin.Data.Settings.SummaryViewSortColumn == TrailResultColumnIds.Location)
            {
                result *= x.Activity.Location.CompareTo(y.Activity.Location);
            }
            else if (TrailsPlugin.Data.Settings.SummaryViewSortColumn == TrailResultColumnIds.Category)
            {
                result *= x.Activity.Category.ToString().CompareTo(y.Activity.Category.ToString());
            }
            else
            {
                result *= getCompareField(x, TrailsPlugin.Data.Settings.SummaryViewSortColumn).CompareTo(getCompareField(y, TrailsPlugin.Data.Settings.SummaryViewSortColumn));
            }            
            return result;
        }
        //Helper function to get numerical value used in comparison
        private static double getCompareField(TrailResult x, string id)
        {
            //Should be using reflection....
            double t;
            switch (id)
            {
                case TrailResultColumnIds.Color:
                    return x.TrailColor.ToArgb();
                case TrailResultColumnIds.Order:
                    return x.Order;
                case TrailResultColumnIds.StartTime:
                    return x.FirstTime.Ticks;
                case TrailResultColumnIds.EndTime:
                    return x.EndTime.Ticks;
                case TrailResultColumnIds.Duration:
                    return x.Duration.TotalSeconds;
                case TrailResultColumnIds.Distance:
                    double.TryParse(x.Distance, out t);
                    return t;
                case TrailResultColumnIds.AvgCadence:
                    return x.AvgCadence;
                case TrailResultColumnIds.AvgGrade:
                    return x.AvgGrade;
                case TrailResultColumnIds.AvgHR:
                    return x.AvgHR;
                case TrailResultColumnIds.AvgPace:
                    return -x.AvgPace;
                case TrailResultColumnIds.AvgPower:
                    return x.AvgPower;
                case TrailResultColumnIds.AvgSpeed:
                    return x.AvgSpeed;
                case TrailResultColumnIds.ElevChg:
                    double.TryParse(x.ElevChg, out t);
                    return t;
                case TrailResultColumnIds.FastestPace:
                    return -x.FastestPace;
                case TrailResultColumnIds.FastestSpeed:
                    return x.FastestSpeed;
                case TrailResultColumnIds.MaxHR:
                    return x.MaxHR;
                default:
                    return x.Order;
            }
        }
    }
}
