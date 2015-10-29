/*
Copyright (C) 2009 Brendan Doherty
Copyright (C) 2010-2015 Gerhard Olsson

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

using System;
using ZoneFiveSoftware.Common.Visuals;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.Measurement;
using System.Diagnostics;
using ZoneFiveSoftware.Common.Data.Fitness.CustomData;
using System.Reflection;

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
        public const string ResultColor = "ResultColor";
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
        public const string Diff = "Diff";
        public const string AscendingSpeed_VAM = "AscendingSpeed_VAM";
        public const string Name = "Name";

        public const string AveragePowerBalance = "AveragePowerBalance";
        public const string AverageTemperature = "AverageTemperature";
        public const string AverageGroundContactTime = "AverageGroundContactTime";
        public const string AverageVerticalOscillation = "AverageVerticalOscillation";
        public const string AverageSaturatedHemoglobin = "AverageSaturatedHemoglobin";
        public const string AverageTotalHemoglobinConcentration = "AverageTotalHemoglobinConcentration";

        public const string GradeRunAdjustedTime = "GradeRunAdjustedTime";
        public const string GradeRunAdjustedPace = "GradeRunAdjustedPace";
        internal static IList<string> GradeRunAdjustedFields = new List<string> { TrailResultColumnIds.GradeRunAdjustedTime, TrailResultColumnIds.GradeRunAdjustedPace };

        public const string PredictDistance = "PredictDistance";
        public const string IdealTime = "IdealTime";
        internal static IList<string> PerformancePredictorFields = new List<string> { TrailResultColumnIds.PredictDistance, TrailResultColumnIds.IdealTime };

        //derived from Activity, not in individual result
        //For historical reasons, do not have prefix
        public const string Location = "Location";
        public const string Category = "Category";
        public const string MetaData_Source = "MetaData_Source";
        internal static IList<string> ActivityFields = new List<string> { TrailResultColumnIds.Category, TrailResultColumnIds.Location, TrailResultColumnIds.MetaData_Source };

        //obsolete fields - maybe just in dev versions
        internal static IList<string> ObsoleteFields = new List<string> { "AvgGrade", "AscMaxGrade", "AvgPaceSpeed", "LapInfo_StartTime", "LapInfo_TotalDistanceMeters", "LapInfo_TotalTime" };

        //Splits
        //All lap fields must start with this prefix, to transform the Id back to standard (and to find them, which could be done with a separate structure)
        internal const string LapInfoPrefix = "LapInfo_";
        public const string LapInfo_AverageCadencePerMinute = LapInfoPrefix + "AverageCadencePerMinute";
        public const string LapInfo_AverageHeartRatePerMinute = LapInfoPrefix + "AverageHeartRatePerMinute";
        public const string LapInfo_AveragePowerWatts = LapInfoPrefix + "AveragePowerWatts";
        public const string LapInfo_ElevationChangeMeters = LapInfoPrefix + "ElevationChangeMeters";
        public const string LapInfo_Notes = LapInfoPrefix + "Notes";
        public const string LapInfo_Rest = LapInfoPrefix + "Rest";
        //public const string LapInfo_StartTime = LapInfoPrefix + "StartTime";
        public const string LapInfo_TotalCalories = LapInfoPrefix + "TotalCalories";
        //public const string LapInfo_TotalDistanceMeters = LapInfoPrefix + "TotalDistanceMeters";
        //public const string LapInfo_TotalTime = LapInfoPrefix + "TotalTime";

        //IPoolLengthInfo PoolLengths
        internal const string SwimLapInfoPrefix = "SwimLapInfo_";
        public const string SwimLapInfo_AverageStrokeDistance = SwimLapInfoPrefix + "AverageStrokeDistance";
        public const string SwimLapInfo_AverageStrokeRate = SwimLapInfoPrefix + "AverageStrokeRate";
        //public const string SwimLapInfo_DistanceUnits = SwimLapInfoPrefix + "DistanceUnits";
        public const string SwimLapInfo_Efficiency = SwimLapInfoPrefix + "Efficiency";
        //public const string SwimLapInfo_StartTime = SwimLapInfoPrefix + "StartTime";
        public const string SwimLapInfo_StrokeCount = SwimLapInfoPrefix + "StrokeCount";
        public const string SwimLapInfo_StrokeType = SwimLapInfoPrefix + "StrokeType";
        public const string SwimLapInfo_SWOLF = SwimLapInfoPrefix + "SWOLF";
        //public const string SwimLapInfo_TotalDistanceMeters = SwimLapInfoPrefix + "TotalDistanceMeters";
        //public const string SwimLapInfo_TotalTime = SwimLapInfoPrefix + "TotalTime";
    }

    public class TrailResultColumns
    {
        private IList<IListColumnDefinition> m_columnDefs = new List<IListColumnDefinition>();
        private IDictionary<string, IListColumnDefinition> m_columnDict = new Dictionary<string, IListColumnDefinition>();
        private IList<string> m_ignoredColumnDict = new List<string>();
        private static IDictionary<string, ICustomDataFieldDefinition> m_custColumnDict = new Dictionary<string, ICustomDataFieldDefinition>();

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
            if (this.m_columnDict.ContainsKey(id))
            {
                return this.m_columnDict[id];
            }
            else if (!m_ignoredColumnDict.Contains(id))
            {
                //It is OK to have lap fields defined, but this is not a Splits trail
                //Unknown column, not ignored
                Debug.Assert(false, string.Format("Unknown column {0}", id));
            }
            return null;
        }

        public static ICustomDataFieldDefinition CustomDef(string id)
        {
            if (m_custColumnDict.ContainsKey(id))
            {
                return m_custColumnDict[id];
            }
            return null;
        }

        public static bool IsLapField(string id)
        {
            if (id.StartsWith(TrailResultColumnIds.LapInfoPrefix))
            {
                return true;
            }
            return false;
        }

        public static bool IsSwimLapField(string id)
        {
            if (id.StartsWith(TrailResultColumnIds.SwimLapInfoPrefix))
            {
                return true;
            }
            return false;
        }

        public static bool IsActivityField(string id)
        {
            if (TrailResultColumnIds.ActivityFields.Contains(id))
            {
                return true;
            }
            return false;
        }

        public static string LapId(string id)
        {
            return id.Remove(0, TrailResultColumnIds.LapInfoPrefix.Length);
        }

        public static string SwimLapId(string id)
        {
            return id.Remove(0, TrailResultColumnIds.SwimLapInfoPrefix.Length);
        }

        public IList<IListColumnDefinition> ColumnDefs()
        {
            return m_columnDefs;
        }

        public TrailResultColumns(IActivity activity, int noRes, bool multAct, bool laps, bool swim)
        : this(activity, noRes, multAct, false, laps, swim)
        {
        }

        public TrailResultColumns(IActivity activity, int noRes, bool multAct)
        : this(activity, noRes, multAct, true, true, true)
        {
        }

        private TrailResultColumns(IActivity activity, int noRes, bool multAct, bool all, bool laps, bool swim)
        {
            string TrailsGroup = "";// Properties.Resources.ApplicationName;
            string ActivityGroup = CommonResources.Text.LabelActivity;
            //Order width is dynamic from no of activities
            int noRes2 = System.Math.Max(10, noRes);
            int w = 14 + (int)System.Math.Log10(noRes2) * 9;
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Order, "#", TrailsGroup, w, StringAlignment.Far));
            w = multAct ? 115 : 70;
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.StartTime, CommonResources.Text.LabelStartTime, TrailsGroup, w, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.StartDistance, CommonResources.Text.LabelStart + CommonResources.Text.LabelDistance + " (" + UnitUtil.Distance.LabelAbbrAct(activity) + ")", TrailsGroup, 60, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.EndTime, CommonResources.Text.LabelEndTime, TrailsGroup, 70, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Duration, CommonResources.Text.LabelDuration, TrailsGroup, 60, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Distance, CommonResources.Text.LabelDistance + " (" + UnitUtil.Distance.LabelAbbrAct(activity) + ")", TrailsGroup, 60, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgCadence, CommonResources.Text.LabelAvgCadence, TrailsGroup, 60, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgHR, CommonResources.Text.LabelAvgHR, TrailsGroup, 50, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.MaxHR, CommonResources.Text.LabelMaxHR, TrailsGroup, 50, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgPower, CommonResources.Text.LabelAvgPower + " (" + CommonResources.Text.LabelWatts + ")", TrailsGroup, 70, StringAlignment.Far));
            if (all || !swim)
            {
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Ascent, CommonResources.Text.LabelAscending + " (" + UnitUtil.Elevation.LabelAbbrAct(activity) + ")", TrailsGroup, 60, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Descent, CommonResources.Text.LabelDescending + " (" + UnitUtil.Elevation.LabelAbbrAct(activity) + ")", TrailsGroup, 60, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.ElevChg, CommonResources.Text.LabelElevationChange + " (" + UnitUtil.Elevation.LabelAbbrAct(activity) + ")", TrailsGroup, 70, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AscAvgGrade, CommonResources.Text.LabelAscending + " " + CommonResources.Text.LabelAvgGrade, TrailsGroup, 70, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AscMaxAvgGrade, CommonResources.Text.LabelMaxAvgGrade, TrailsGroup, 70, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.DescAvgGrade, CommonResources.Text.LabelDescending + " " + CommonResources.Text.LabelAvgGrade, TrailsGroup, 70, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AscendingSpeed_VAM, Properties.Resources.UI_Activity_List_AscendingSpeed_VAM + " (" + UnitUtil.Elevation.LabelAbbrAct(activity) + "/h)", TrailsGroup, 70, StringAlignment.Far));
            }
            else
            {
                m_ignoredColumnDict.Add(TrailResultColumnIds.Ascent);
                m_ignoredColumnDict.Add(TrailResultColumnIds.Descent);
                m_ignoredColumnDict.Add(TrailResultColumnIds.ElevChg);
                m_ignoredColumnDict.Add(TrailResultColumnIds.AscAvgGrade);
                m_ignoredColumnDict.Add(TrailResultColumnIds.AscMaxAvgGrade);
                m_ignoredColumnDict.Add(TrailResultColumnIds.DescAvgGrade);
                m_ignoredColumnDict.Add(TrailResultColumnIds.AscendingSpeed_VAM);
            }
            int speedIndex = m_columnDefs.Count;
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgSpeed, CommonResources.Text.LabelAvgSpeed + " (" + UnitUtil.Speed.LabelAbbrAct(activity) + ")", TrailsGroup, 70, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.FastestSpeed, CommonResources.Text.LabelFastestSpeed + " (" + UnitUtil.Speed.LabelAbbrAct(activity) + ")", TrailsGroup, 70, StringAlignment.Far));
            int paceIndex = m_columnDefs.Count;
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AvgPace, CommonResources.Text.LabelAvgPace + " (" + UnitUtil.Pace.LabelAbbrAct(activity) + ")", TrailsGroup, 70, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.FastestPace, CommonResources.Text.LabelFastestPace + " (" + UnitUtil.Pace.LabelAbbrAct(activity) + ")", TrailsGroup, 70, StringAlignment.Far));
            
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
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Name, CommonResources.Text.LabelName, ActivityGroup, 70, StringAlignment.Near));


            //Activity
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Location, CommonResources.Text.LabelLocation, ActivityGroup, 70, StringAlignment.Near));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Category, CommonResources.Text.LabelCategory, ActivityGroup, 70, StringAlignment.Near));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.MetaData_Source, Properties.Resources.Metadata_Source, ActivityGroup, 70, StringAlignment.Near));

            if (all || !swim && TrailsPlugin.Integration.PerformancePredictor.PerformancePredictorIntegrationEnabled)
            {
                string PerformancePredictorGroup = Properties.Resources.PerformancePredictorPluginName;
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.PredictDistance, CommonResources.Text.LabelTime + " (" + UnitUtil.Distance.ToString(Settings.PredictDistance, "u") + ")", PerformancePredictorGroup, 70, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.IdealTime, Properties.Resources.UI_Activity_List_IdealTime, PerformancePredictorGroup, 70, StringAlignment.Far));
            }
            else
            {
                m_ignoredColumnDict.Add(TrailResultColumnIds.PredictDistance);
                m_ignoredColumnDict.Add(TrailResultColumnIds.IdealTime);
            }
            if (all || !swim && Settings.RunningGradeAdjustMethod != Data.RunningGradeAdjustMethodEnum.None)
            {
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.GradeRunAdjustedTime, CommonResources.Text.LabelGrade + " " + CommonResources.Text.LabelDuration, TrailsGroup, 60, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.GradeRunAdjustedPace, CommonResources.Text.LabelGrade + " " + CommonResources.Text.LabelAvgPace + " (" + UnitUtil.Pace.LabelAbbrAct(activity) + ")", TrailsGroup, 70, StringAlignment.Far));
            }
            else
            {
                m_ignoredColumnDict.Add(TrailResultColumnIds.GradeRunAdjustedTime);
                m_ignoredColumnDict.Add(TrailResultColumnIds.GradeRunAdjustedPace);
            }
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.Diff, Properties.Resources.UI_Activity_List_DiffPresent + " (" + UnitUtil.Elevation.LabelAbbrAct(activity) + ")", TrailsGroup, 70, StringAlignment.Far));

            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AveragePowerBalance, CommonResources.Text.LabelPowerBalance, TrailsGroup, 70, StringAlignment.Far));
            m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AverageTemperature, CommonResources.Text.LabelTemperature + UnitUtil.Temperature.LabelAbbr2, TrailsGroup, 70, StringAlignment.Far));
            if (all || !swim)
            {
                //Hardcoded to cm, as in track?
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AverageGroundContactTime, CommonResources.Text.LabelGroundContactTime + " (ms)", TrailsGroup, 70, StringAlignment.Far));
                //No translation for ms?
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AverageVerticalOscillation, CommonResources.Text.LabelVerticalOscillation + " (" + Length.LabelAbbr(Length.Units.Centimeter) + ")", TrailsGroup, 70, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AverageSaturatedHemoglobin, CommonResources.Text.LabelSaturatedHemoglobinPercent, TrailsGroup, 70, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.AverageTotalHemoglobinConcentration, CommonResources.Text.LabelTotalHemoglobinConcentration, TrailsGroup, 70, StringAlignment.Far));
            }
            else
            {
                m_ignoredColumnDict.Add(TrailResultColumnIds.AverageGroundContactTime);
                m_ignoredColumnDict.Add(TrailResultColumnIds.AverageVerticalOscillation);
                m_ignoredColumnDict.Add(TrailResultColumnIds.AverageSaturatedHemoglobin);
                m_ignoredColumnDict.Add(TrailResultColumnIds.AverageTotalHemoglobinConcentration);
            }
            //Reset every refresh
            m_custColumnDict = new Dictionary<string, ICustomDataFieldDefinition>();
           
            foreach (ICustomDataFieldDefinition custDataDef in TrailsPlugin.Plugin.GetApplication().Logbook.CustomDataFieldDefinitions)
            {
                if (custDataDef.ObjectType.Type.Equals(typeof(IActivity)))
                {
                    IListColumnDefinition cust = new ListColumnDefinition(custDataDef.Id.ToString(), custDataDef.Name, Properties.Resources.List_CustomFields + " - " + custDataDef.GroupAggregation.ToString(), 70, StringAlignment.Far);
                    m_columnDefs.Add(cust);
                    m_custColumnDict[custDataDef.Id.ToString()] = custDataDef;
                }
            }

            if (all || laps)
            {
                string LapGroup = CommonResources.Text.LabelLap;
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.LapInfo_AverageCadencePerMinute, CommonResources.Text.LabelAvgCadence, LapGroup, 60, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.LapInfo_AverageHeartRatePerMinute, CommonResources.Text.LabelAvgHR, LapGroup, 60, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.LapInfo_AveragePowerWatts, CommonResources.Text.LabelAvgPower, LapGroup, 60, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.LapInfo_ElevationChangeMeters, CommonResources.Text.LabelElevationChange, LapGroup, 60, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.LapInfo_Notes, CommonResources.Text.LabelNotes, LapGroup, 60, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.LapInfo_Rest, Properties.Resources.List_RestLap, LapGroup, 60, StringAlignment.Far));
                //m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.LapInfo_StartTime, CommonResources.Text.LabelStartTime, LapGroup, 115, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.LapInfo_TotalCalories, CommonResources.Text.LabelCalories, LapGroup, 60, StringAlignment.Far));
                //m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.LapInfo_TotalDistanceMeters, CommonResources.Text.LabelDistance + " (" + UnitUtil.Distance.LabelAbbrAct(activity) + ")", LapGroup, 60, StringAlignment.Far));
                //m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.LapInfo_TotalTime, CommonResources.Text.LabelTime, LapGroup, 60, StringAlignment.Far));
            }
            else
            {
                m_ignoredColumnDict.Add(TrailResultColumnIds.LapInfo_AverageCadencePerMinute);
                m_ignoredColumnDict.Add(TrailResultColumnIds.LapInfo_AverageHeartRatePerMinute);
                m_ignoredColumnDict.Add(TrailResultColumnIds.LapInfo_AveragePowerWatts);
                m_ignoredColumnDict.Add(TrailResultColumnIds.LapInfo_ElevationChangeMeters);
                m_ignoredColumnDict.Add(TrailResultColumnIds.LapInfo_Notes);
                m_ignoredColumnDict.Add(TrailResultColumnIds.LapInfo_Rest);
                m_ignoredColumnDict.Add(TrailResultColumnIds.LapInfo_TotalCalories);
            }
            if (all || swim)
            {
                string SwimLapGroup = Properties.Resources.UI_Activity_List_SwimLap;
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.SwimLapInfo_AverageStrokeDistance, Properties.Resources.UI_Activity_List_AverageStrokeDistance, SwimLapGroup, 60, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.SwimLapInfo_AverageStrokeRate, Properties.Resources.UI_Activity_List_AverageStrokeRate, SwimLapGroup, 60, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.SwimLapInfo_Efficiency, Properties.Resources.UI_Activity_List_Efficiency, SwimLapGroup, 60, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.SwimLapInfo_StrokeCount, Properties.Resources.UI_Activity_List_StrokeCount, SwimLapGroup, 60, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.SwimLapInfo_StrokeType, Properties.Resources.UI_Activity_List_StrokeType, SwimLapGroup, 60, StringAlignment.Far));
                m_columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.SwimLapInfo_SWOLF, Properties.Resources.UI_Activity_List_SWOLF, SwimLapGroup, 60, StringAlignment.Far));
            }
            else
            {
                m_ignoredColumnDict.Add(TrailResultColumnIds.SwimLapInfo_AverageStrokeDistance);
                m_ignoredColumnDict.Add(TrailResultColumnIds.SwimLapInfo_AverageStrokeRate);
                m_ignoredColumnDict.Add(TrailResultColumnIds.SwimLapInfo_Efficiency);
                m_ignoredColumnDict.Add(TrailResultColumnIds.SwimLapInfo_StrokeCount);
                m_ignoredColumnDict.Add(TrailResultColumnIds.SwimLapInfo_StrokeType);
                m_ignoredColumnDict.Add(TrailResultColumnIds.SwimLapInfo_SWOLF);
            }

            //Dictionary with all fields
            foreach (IListColumnDefinition l in m_columnDefs)
            {
                this.m_columnDict[l.Id] = l;
            }
        }

        public static IList<IListColumnDefinition> PermanentMultiColumnDefs()
        {
            IList<IListColumnDefinition> columnDefs = new List<IListColumnDefinition>();
            columnDefs.Add(new ListColumnDefinition(TrailResultColumnIds.ResultColor, "", "", 25, StringAlignment.Near));

            return columnDefs;
        }

        public static int Compare(TrailResult x, TrailResult y)
        {
            if (x == null || x is SummaryTrailResult)
            {
                return 1;
            }
            if (y == null || y is SummaryTrailResult)
            {
                return -1;
            }

            int result = 0;
            const string GradeRunAdjustedPace_Field = "GradeRunAdjustedSpeed";
            foreach (string id0 in TrailsPlugin.Data.Settings.SummaryViewSortColumns)
            {
                bool reverseSortOrder = false;
                string id = id0;
                //Sort Pace and speed the same
                switch (id)
                {
                    case TrailResultColumnIds.AvgPace:
                    case TrailResultColumnIds.AvgSpeedPace:
                        id = TrailResultColumnIds.AvgSpeed;
                        break;
                    case TrailResultColumnIds.FastestPace:
                    case TrailResultColumnIds.FastestSpeedPace:
                        id = TrailResultColumnIds.FastestSpeed;
                        break;
                    case TrailResultColumnIds.GradeRunAdjustedPace:
                        reverseSortOrder = true;
                        id = GradeRunAdjustedPace_Field;
                        break;
                }

                //Limit "automatic" calculations in some situations
                if (TrailsPlugin.Controller.TrailController.Instance.AutomaticUpdate)
                {
                    if (id == TrailResultColumnIds.GradeRunAdjustedTime ||
                        id == TrailResultColumnIds.PredictDistance ||
                        id == TrailResultColumnIds.IdealTime)
                    {
                        id = TrailResultColumnIds.Duration;
                    }
                    if (id == GradeRunAdjustedPace_Field)
                    {
                        id = TrailResultColumnIds.AvgPace;
                    }
                }

                try
                {
                    Boolean fieldFound = false;
                    if (id == TrailResultColumnIds.MetaData_Source)
                    {
                        //Special handling, sub object
                        fieldFound = true;
                        if (x is ParentTrailResult && x.Activity != null && y.Activity != null)
                        {
                            result = x.Activity.Metadata.Source.CompareTo(y.Activity.Metadata.Source);
                        }
                    }

                    {
                        //Dont bother with reflection CompareTo, few types, just complicates TrailResult/Lap
                        //If not parent result, there is no difference
                        ICustomDataFieldDefinition cust = TrailResultColumns.CustomDef(id);
                        if (cust != null)
                        {
                            fieldFound = true;
                            if (x is ParentTrailResult && x.Activity != null && y.Activity != null)
                            {
                                object xoc = x.Activity.GetCustomDataValue(cust);
                                object yoc = y.Activity.GetCustomDataValue(cust);
                                if (xoc == null)
                                {
                                    result = 1;
                                }
                                else if (yoc == null)
                                {
                                    result = -1;
                                }
                                else if (cust.DataType.Id.Equals(new System.Guid("{6e0f7115-6aa3-49ea-a855-966ce17317a1}")))
                                {
                                    //numeric
                                    result = ((System.Double)xoc).CompareTo((System.Double)yoc);
                                }
                                else
                                {
                                    //date or string
                                    result = ((string)xoc).CompareTo((string)yoc);
                                }
                            }
                        }
                    }
                    if (!fieldFound)
                    {
                        //Use reflection to get values and compare routines
                        //This is very slightly slower than hardcoded access.
                        //Profiling on the simple Duration field, it takes 129 ms vs 150ms for 64000/57000(?) operations (Splits, 2400 activities, sorted twice)
                        //(Total sort is 300/400 ms though)
                        //An attempt to cache method gave 180ms 82000(?), this was removed
                        //For most fields, this is not measurable, getting data is much slower
                        object xo;
                        object yo;

                        if (IsLapField(id))
                        {
                            //Note: Some lap/poolinfo fields are read using TrailResult properties
                            id = LapId(id);
                            xo = x.LapInfo;
                            yo = y.LapInfo;
                        }
                        else if (IsSwimLapField(id))
                        {
                            id = SwimLapId(id);
                            xo = x.PoolLengthInfo;
                            yo = y.PoolLengthInfo;
                        }
                        else if (IsActivityField(id))
                        {
                            xo = x.Activity;
                            yo = y.Activity;
                        }
                        else
                        {
                            xo = x;
                            yo = y;
                        }

                        if (xo != null && yo != null)
                        {
                            //Only Properties, no fields searched
                            PropertyInfo xf = xo.GetType().GetProperty(id);
                            PropertyInfo yf = yo.GetType().GetProperty(id);
                            if (xf == null)
                            {
                                Debug.Assert(false, string.Format("No property info for id {0} for x {1}", id, xo));
                                result = 1;
                            }
                            else if (yf == null)
                            {
                                Debug.Assert(false, string.Format("No property info for id {0} for y {1}", id, yo));
                                result = -1;
                            }

                            object xv = xf.GetValue(xo, null);
                            object yv = yf.GetValue(yo, null);
                            if (xv == null)
                            {
                                //Debug.Assert(false, string.Format("No value for id {0} for x {1}", id, xo));
                                if (yv == null) result = 1;
                            }
                            else if (yv == null)
                            {
                                Debug.Assert(false, string.Format("No value for id {0} for y {1}", id, yo));
                                if (xv == null) result = -1;
                            }
                            else
                            {
                                //Get the CompareTo method using reflection
                                MethodInfo cmp = null;
                                //Specialized version of generic (not applicable for .Net2) 
                                // from http://stackoverflow.com/questions/4035719/getmethod-for-generic-method
                                foreach (MethodInfo methodInfo in xv.GetType().GetMember("CompareTo",
                                                                 MemberTypes.Method, BindingFlags.Public | BindingFlags.Instance))
                                {
                                    // Check that the parameter counts and types match, 
                                    // with 'loose' matching on generic parameters
                                    ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                                    if (parameterInfos.Length == 1)
                                    {
                                        if (parameterInfos[0].ParameterType.Equals(yv) || parameterInfos[0].ParameterType.Equals(typeof(object)))
                                        {
                                            cmp = methodInfo;
                                            break;
                                        }
                                    }
                                }
                                if (cmp != null)
                                {
                                    result = (int)cmp.Invoke(xv, new object[1] { yv });
                                }
                                else
                                {
                                    Debug.Assert(false, string.Format("No CompareTo for id {0} for x {1}", id, xo));
                                }
                            }
                        }
                        else if (xo == null)
                        {
                            result = 1;
                        }
                        else if (y == null)
                        {
                            result = -1;
                        }

                    }
                }

                catch (System.Exception e)
                {
                    Debug.Assert(false, string.Format("Exception when finding properties for id {0} for x {1}, y {2}: {3}", id, x, y, e));
                }

                if (result != 0)
                {
                    if (reverseSortOrder)
                    {
                        result *= -1;
                    }
                    break;
                }

            }
            if (result == 0)
            {
                //Fallback sorting
                result = x.Order.CompareTo(y.Order);
            }
            result *= (TrailsPlugin.Data.Settings.SummaryViewSortDirection == ListSortDirection.Ascending ? 1 : -1);
            return result;
        }
    }
}
