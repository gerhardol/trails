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
using ZoneFiveSoftware.Common.Data.Measurement;
using System.Diagnostics;

#if ST_2_1
//IListItem
using ZoneFiveSoftware.SportTracks.UI;
using ZoneFiveSoftware.SportTracks.Data;
#endif
using GpsRunningPlugin.Util;

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

    public class TrailResultColumnIds
    {
        public const string Order = "Order";
        public const string Color = "Color";
        public const string StartTime = "StartTime";
        public const string StartDistance = "StartDistance";
        public const string EndTime = "EndTime";
        public const string Duration = "Duration";
        public const string Distance = "Distance";
        public const string AvgCadence = "AvgCadence";
        public const string AvgHR = "AvgHR";
        public const string MaxHR = "MaxHR";
        public const string Ascent = "Ascent";
        public const string Descent = "Descent";
        public const string ElevChg = "ElevChg";
        public const string AvgPower = "AvgPower";
        public const string AscAvgGrade = "AscAvgGrade";
        public const string AscMaxAvgGrade = "AscMaxAvgGrade";
        public const string DescAvgGrade = "DescAvgGrade";
        //Note: No DescMaxAvgGrade
        public const string AvgSpeed = "AvgSpeed";
        public const string FastestSpeed = "FastestSpeed";
        public const string AvgPace = "AvgPace";
        public const string FastestPace = "FastestPace";
        public const string AvgSpeedPace = "AvgSpeedPace";
        public const string FastestSpeedPace = "FastestSpeedPace";
        public const string Name = "Name";
        public const string Location = "Location";
        public const string Category = "Category";
        public const string PredictDistance = "PredictDistance";
        public const string IdealTime = "IdealTime";
        public const string GradeRunAdjustedTime = "GradeRunAdjustedTime";
        public const string GradeRunAdjustedPace = "GradeRunAdjustedPace";
        public const string Diff = "Diff";
        public const string VAM = "VAM";
        public const string AscendingSpeed_VAM = "AscendingSpeed_VAM";

        //Used by Settings at start
        public static string DefaultSortColumn()
        {
            return TrailResultColumnIds.StartTime;
        }

        public static IList<string> DefaultColumns()
        {
            IList<string> activityPageColumns = new List<string>();
            activityPageColumns.Add(TrailResultColumnIds.Order);
            activityPageColumns.Add(TrailResultColumnIds.StartTime);
            activityPageColumns.Add(TrailResultColumnIds.Duration);
            activityPageColumns.Add(TrailResultColumnIds.Distance);
            activityPageColumns.Add(TrailResultColumnIds.AvgSpeedPace);
            activityPageColumns.Add(TrailResultColumnIds.AvgHR);
            activityPageColumns.Add(TrailResultColumnIds.MaxHR);
            activityPageColumns.Add(TrailResultColumnIds.Ascent);
            activityPageColumns.Add(TrailResultColumnIds.GradeRunAdjustedPace);
            activityPageColumns.Add(TrailResultColumnIds.Name);
            return activityPageColumns;
        }
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
        public IListColumnDefinition ColumnDef(string id)
        {
            //compatibility
            if (id == "AvgGrade") { id = "AscAvgGrade"; }
            if (id == "AscMaxGrade") { id = "AscMaxAvgGrade"; }
            if (id == "AvgPaceSpeed") { id = "AvgSpeedPace"; }
            if (!this.m_columnDict.ContainsKey(id))
            {
                //Unknown column, not ignored
                Debug.Assert(false);
            }
            else
            {
                return this.m_columnDict[id];
            }
            return null;
        }

        public IList<IListColumnDefinition> ColumnDefs()
        {
            return m_columnDefs;
        }

        private IList<IListColumnDefinition> m_columnDefs = new List<IListColumnDefinition>();
        private IDictionary<string,IListColumnDefinition> m_columnDict = new Dictionary<string, IListColumnDefinition>();
        public TrailResultColumnIds(IActivity activity, int noRes, bool multAct, bool all)
        {
            int noRes2 = System.Math.Max(10, noRes);
            int w = 14 + (int)System.Math.Log10(noRes2) * 9;
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Order, "#", "", w, StringAlignment.Far));
            w = multAct ? 115 : 70;
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.StartTime, CommonResources.Text.LabelStartTime, "", w, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.StartDistance, CommonResources.Text.LabelStart + CommonResources.Text.LabelDistance + " (" + UnitUtil.Distance.LabelAbbrAct(activity) + ")", "", 60, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.EndTime, CommonResources.Text.LabelEndTime, "", 70, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Duration, CommonResources.Text.LabelDuration, "", 60, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Distance, CommonResources.Text.LabelDistance + " (" + UnitUtil.Distance.LabelAbbrAct(activity) + ")", "", 60, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgCadence, CommonResources.Text.LabelAvgCadence, "", 60, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgHR, CommonResources.Text.LabelAvgHR, "", 50, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.MaxHR, CommonResources.Text.LabelMaxHR, "", 50, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Ascent, CommonResources.Text.LabelAscending + " (" + UnitUtil.Elevation.LabelAbbrAct(activity) + ")", "", 60, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Descent, CommonResources.Text.LabelDescending + " (" + UnitUtil.Elevation.LabelAbbrAct(activity) + ")", "", 60, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.ElevChg, CommonResources.Text.LabelElevationChange + " (" + UnitUtil.Elevation.LabelAbbrAct(activity) + ")", "", 70, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgPower, CommonResources.Text.LabelAvgPower + " (" + CommonResources.Text.LabelWatts + ")", "", 70, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AscAvgGrade, CommonResources.Text.LabelAscending + " " + CommonResources.Text.LabelAvgGrade, "", 70, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AscMaxAvgGrade, CommonResources.Text.LabelMaxAvgGrade, "", 70, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.DescAvgGrade, CommonResources.Text.LabelDescending + " " + CommonResources.Text.LabelAvgGrade, "", 70, StringAlignment.Far));

            int speedIndex = m_columnDefs.Count;
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgSpeed, CommonResources.Text.LabelAvgSpeed + " (" + UnitUtil.Speed.LabelAbbrAct(activity) + ")", "", 70, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.FastestSpeed, CommonResources.Text.LabelFastestSpeed + " (" + UnitUtil.Speed.LabelAbbrAct(activity) + ")", "", 70, StringAlignment.Far));
            int paceIndex = m_columnDefs.Count;
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgPace, CommonResources.Text.LabelAvgPace + " (" + UnitUtil.Pace.LabelAbbrAct(activity) + ")", "", 70, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.FastestPace, CommonResources.Text.LabelFastestPace + " (" + UnitUtil.Pace.LabelAbbrAct(activity) + ")", "", 70, StringAlignment.Far));
            
            //SpeedPace columns are handled as Speed, except that the headline differs
            //(the headline is likely to narrow, but Speed should be visible)
            IListColumnDefinition col = m_columnDefs[speedIndex];
            string text = m_columnDefs[speedIndex].Text(m_columnDefs[speedIndex].Id) + " / " + m_columnDefs[paceIndex].Text(m_columnDefs[paceIndex].Id);
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgSpeedPace, text, col.GroupName, col.Width, col.Align));
            speedIndex++; paceIndex++;
            col = m_columnDefs[speedIndex];
            text = m_columnDefs[speedIndex].Text(m_columnDefs[speedIndex].Id) + " / " + m_columnDefs[paceIndex].Text(m_columnDefs[paceIndex].Id);
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.FastestSpeedPace, text, col.GroupName, col.Width, col.Align));

            //int index = speedIndex;
            //if(UnitUtil.PaceOrSpeed.IsPace(activity))
            //{
            //    index=paceIndex;
            //}
            //columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgSpeedPace, columnDefs[index].Text(columnDefs[index].Id), columnDefs[index].GroupName, columnDefs[index].Width, columnDefs[index].Align));
            //index++;
            //columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.FastestSpeedPace, columnDefs[index].Text(columnDefs[index].Id), columnDefs[index].GroupName, columnDefs[index].Width, columnDefs[index].Align));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Name, CommonResources.Text.LabelName, "", 70, StringAlignment.Near));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Location, CommonResources.Text.LabelLocation, "", 70, StringAlignment.Near));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Category, CommonResources.Text.LabelCategory, "", 70, StringAlignment.Near));
            if (all || TrailsPlugin.Integration.PerformancePredictor.PerformancePredictorIntegrationEnabled)
            {
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.PredictDistance, CommonResources.Text.LabelTime + " (" + UnitUtil.Distance.ToString(Settings.PredictDistance, "u") + ")", "", 70, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.IdealTime, Properties.Resources.UI_Activity_List_IdealTime, "", 70, StringAlignment.Far));
            }
            if (all || Settings.RunningGradeAdjustMethod != Data.RunningGradeAdjustMethodEnum.None)
            {
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.GradeRunAdjustedTime, CommonResources.Text.LabelGrade + " " + CommonResources.Text.LabelDuration, "", 60, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.GradeRunAdjustedPace, CommonResources.Text.LabelGrade + " " + CommonResources.Text.LabelAvgPace + " (" + UnitUtil.Pace.LabelAbbrAct(activity) + ")", "", 70, StringAlignment.Far));
            }
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Diff, Properties.Resources.UI_Activity_List_DiffPresent + " (" + UnitUtil.Elevation.LabelAbbrAct(activity) + ")", "", 70, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.VAM, Properties.Resources.UI_Activity_List_AscendingSpeed_VAM + " (" + UnitUtil.Elevation.LabelAbbrAct(activity) + "/h)", "", 70, StringAlignment.Far));
            
            foreach(IListColumnDefinition l in m_columnDefs)
            {
                this.m_columnDict[l.Id] = l;
            }
        }

        public static IList<IListColumnDefinition> PermanentMultiColumnDefs()
        {
            IList<IListColumnDefinition> columnDefs = new List<IListColumnDefinition>();
            columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Color, "", "", 25, StringAlignment.Near));

            return columnDefs;
        }

        public static int Compare(TrailResult x, TrailResult y)
        {
            if (x == null || x.Activity == null)
            {
                //Summary
                return 1;
            }
            if (y == null || y.Activity == null)
            {
                return -1;
            }
            int result;

            if (TrailsPlugin.Data.Settings.SummaryViewSortColumn == TrailResultColumnIds.Name)
            {
                result = x.Activity.Name.CompareTo(y.Activity.Name);
            }
            else if (TrailsPlugin.Data.Settings.SummaryViewSortColumn == TrailResultColumnIds.Location)
            {
                result = x.Activity.Location.CompareTo(y.Activity.Location);
            }
            else if (TrailsPlugin.Data.Settings.SummaryViewSortColumn == TrailResultColumnIds.Category)
            {
                result = x.Activity.Category.ToString().CompareTo(y.Activity.Category.ToString());
            }
            else
            {
                result = getCompareField(x, TrailsPlugin.Data.Settings.SummaryViewSortColumn).CompareTo(getCompareField(y, TrailsPlugin.Data.Settings.SummaryViewSortColumn));
            }
            //Seem the same, but check activity
            if (result == 0 && x.Activity != y.Activity)
            {
                result = x.Activity.ReferenceId.CompareTo(y.Activity.ReferenceId);
            }

            int dir = (TrailsPlugin.Data.Settings.SummaryViewSortDirection == ListSortDirection.Ascending ? 1 : -1);
            return dir * result;
        }

        //Helper function to get numerical value used in comparison
        private static double getCompareField(TrailResult x, string id)
        {
            //Should be using reflection....
            switch (id)
            {
                case TrailResultColumnIds.Color:
                    return x.ResultColor.LineNormal.ToArgb();
                case TrailResultColumnIds.Order:
                    //Order is set dynamically for parents, but used for children
                    return x.Order;
                case TrailResultColumnIds.StartTime:
                    return x.StartTime.Ticks;
                case TrailResultColumnIds.StartDistance:
                    return x.StartDist;
                case TrailResultColumnIds.EndTime:
                    return x.EndTime.Ticks;
                case TrailResultColumnIds.Duration:
                    return x.Duration.TotalSeconds;
                case TrailResultColumnIds.Distance:
                    return x.Distance;
                case TrailResultColumnIds.AvgCadence:
                    return x.AvgCadence;
                case TrailResultColumnIds.AscAvgGrade:
                    return x.AscAvgGrade;
                case TrailResultColumnIds.AscMaxAvgGrade:
                    return x.AscMaxAvgGrade;
                case TrailResultColumnIds.DescAvgGrade:
                    return x.DescAvgGrade;
                case TrailResultColumnIds.AvgHR:
                    return x.AvgHR;
                case TrailResultColumnIds.AvgPower:
                    return x.AvgPower;
                case TrailResultColumnIds.AvgPace:
                case TrailResultColumnIds.AvgSpeed:
                case TrailResultColumnIds.AvgSpeedPace:
                    return x.AvgSpeed;
                case TrailResultColumnIds.FastestPace:
                case TrailResultColumnIds.FastestSpeed:
                case TrailResultColumnIds.FastestSpeedPace:
                    return x.FastestSpeed;
                case TrailResultColumnIds.Ascent:
                    return x.Ascent;
                case TrailResultColumnIds.Descent:
                    return x.Descent;
                case TrailResultColumnIds.ElevChg:
                    return x.ElevChg;
                case TrailResultColumnIds.MaxHR:
                    return x.MaxHR;
                case TrailResultColumnIds.PredictDistance:
                    return x.PredictDistance;
                case TrailResultColumnIds.IdealTime:
                    return x.IdealTime.TotalSeconds;
                case TrailResultColumnIds.GradeRunAdjustedTime:
                    return x.GradeRunAdjustedTime.TotalSeconds;
                case TrailResultColumnIds.GradeRunAdjustedPace:
                    return x.GradeRunAdjustedSpeed;
                case TrailResultColumnIds.Diff:
                    return x.Diff;
                case TrailResultColumnIds.AscendingSpeed_VAM:
                    return x.VAM;
                default:
                    return x.Order;
            }
        }
    }
}
