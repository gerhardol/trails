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

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Chart;
using ZoneFiveSoftware.Common.Visuals.Fitness;

using TrailsPlugin.Data;
using GpsRunningPlugin.Util;
using System.Collections;

namespace TrailsPlugin.Utils
{
    public enum SmoothOverTrailBorders { All, Unchanged, None };
    public enum SyncGraphMode { None, Start, End, Average, Min, Max };

    public enum XAxisValue
    {
        Time,
        Distance
    }

    public enum LineChartTypes
    {
        Unknown,
        Speed,
        Pace,
        SpeedPace,
        Elevation,
        Grade,
        HeartRateBPM,
        //HeartRatePercentMax,
        Cadence,
        Power,
        DiffTime, //unused
        DiffDist, //unused
        DiffDistTime,
        PowerBalance, //LeftPower
        Temperature,
        GroundContactTime,
        VerticalOscillation,
        SaturatedHemoglobin,
        TotalHemoglobinConcentration,
        DeviceSpeed,
        DevicePace,
        DeviceSpeedPace,
        DeviceElevation,
        DeviceDiffDist,
        DiffHeartRateBPM, //NotUsedInTrails
        Time, //NotUsedInTrails
        Distance //NotUsedInTrails
    }

    public class ChartTypeDefinition : IListColumnDefinition
    {
        public ChartTypeDefinition(LineChartTypes chartKey)
        {
            this.chartKey = chartKey;
        }
        private LineChartTypes chartKey;

        public StringAlignment Align
        {
            get
            {
                return StringAlignment.Near;
            }
        }
        public string GroupName
        {
            get
            {
                string group = null;
                if (chartKey >= LineChartTypes.DeviceSpeed)
                { group = CommonResources.Text.LabelDevice; }
                else if (chartKey >= LineChartTypes.PowerBalance)
                { group = ""; }
                return group;
            }
        }
        public string Id
        {
            get
            {
                return this.chartKey.ToString();
            }
        }
        public int Width
        {
            get
            {
                return 0;
            }
        }
        public string Text(string id)
        {
            return this.ToString();
        }
        public override string ToString()
        {
            return LineChartUtil.LineChartTypesString(chartKey, false);
        }
    }

    public static class LineChartUtil
    {
        public static IList<LineChartTypes> ParseLineChartType(string[] s)
        {
            IList<LineChartTypes> r = new List<LineChartTypes>();
            foreach (String column in s)
            {
                try
                {
                    LineChartTypes t = (LineChartTypes)Enum.Parse(typeof(LineChartTypes), column, true);
                    //Compatibility w previous, where DifTime/DiffDist could be speced directly
                    if (t == LineChartTypes.DiffDist || t == LineChartTypes.DiffTime)
                    {
                        if (!r.Contains(LineChartTypes.DiffDistTime))
                        {
                            r.Add(LineChartTypes.DiffDistTime);
                        }
                    }
                    else
                    {
                        r.Add(t);
                    }
                }
                catch { }
            }
            return r;
        }

        public static IList<LineChartTypes> SortMultiGraphType(IList<LineChartTypes> list)
        {
            //The order of presentation is fixed and the same as enum order
            ((List<LineChartTypes>)list).Sort();
            return list;
        }

        public static IList<string> LineChartType_strings(IList<LineChartTypes> list)
        {
            IList<string> r = new List<string>();
            foreach (LineChartTypes l in list)
            {
                r.Add(l.ToString());
            }
            return r;
        }

        public static IList<IListColumnDefinition> MultiCharts()
        {
            return ChartDefs(Enum.GetValues(typeof(LineChartTypes)), true);
        }

        public static IList<IListColumnDefinition> MultiGraphs()
        {
            return ChartDefs((Array)new LineChartTypes[] { LineChartTypes.Cadence,
                LineChartTypes.SpeedPace, LineChartTypes.Speed, LineChartTypes.Pace,
                LineChartTypes.Elevation, LineChartTypes.Grade,
                LineChartTypes.HeartRateBPM, LineChartTypes.Power,
                LineChartTypes.DiffDistTime},
                false);
        }

        private static IList<IListColumnDefinition> ChartDefs(Array list, bool charts)
        {
            IList<IListColumnDefinition> r = new List<IListColumnDefinition>();
            foreach (LineChartTypes l in list)
            {
                if (l > LineChartTypes.Unknown && 
                    (charts && l < LineChartTypes.DiffHeartRateBPM ||
                     !charts && l < LineChartTypes.PowerBalance) &&
                    l != LineChartTypes.DiffDist && l != LineChartTypes.DiffTime)
                {
                    r.Add(new ChartTypeDefinition(l));
                }
            }
            return r;
        }

        public static IList<IListColumnDefinition> ChartDefs(IList<LineChartTypes> list)
        {
            return ChartDefs(((List<LineChartTypes>)list));
        }

        public static string SmoothOverTrailBordersString(SmoothOverTrailBorders t)
        {
            string s;
            switch (t)
            {
                case SmoothOverTrailBorders.All:
                    {
                        s = Properties.Resources.UI_Chart_SmoothOverTrailBorders_All;
                        break;
                    }
                case SmoothOverTrailBorders.Unchanged:
                    {
                        s = Properties.Resources.UI_Chart_SmoothOverTrailBorders_Unchanged;
                        break;
                    }
                case SmoothOverTrailBorders.None:
                    {
                        s = Properties.Resources.UI_Chart_SmoothOverTrailBorders_None;
                        break;
                    }
                default:
                    {
                        Debug.Assert(false, string.Format("Unexpecteded SmoothOverTrailBorders {0}", t));
                        s = t.ToString();
                        break;
                    }
            }
            return Properties.Resources.UI_Chart_ChartBorderSmooth + ": " + s;
        }

        //No simple way to dynamically translate enum
        //The integer (raw) value is stored as defaults too
        public static string XAxisValueString(XAxisValue XAxisReferential)
        {
            string xAxisLabel = "";
            switch (XAxisReferential)
            {
                case XAxisValue.Distance:
                    {
                        xAxisLabel = CommonResources.Text.LabelDistance;
                        break;
                    }
                case XAxisValue.Time:
                    {
                        xAxisLabel = CommonResources.Text.LabelTime;
                        break;
                    }
                    //TODO: DateTime
                default:
                    {
                        Debug.Assert(false, string.Format("Unexpecteded XAxisValue {0}", XAxisReferential));
                        break;
                    }
            }
            return xAxisLabel;
        }

        public static IList<LineChartTypes> DefaultLineChartTypes()
        {
            return new List<LineChartTypes>{
                LineChartTypes.SpeedPace, LineChartTypes.Elevation,
                LineChartTypes.HeartRateBPM, LineChartTypes.Cadence};
        }

        public static string ChartTypeString(LineChartTypes x)
        {
            return LineChartTypesString(x, true);
        }

        public static string LineChartTypesString(LineChartTypes YAxisReferential, bool yAxis)
        {
            string yAxisLabel = "";
            switch (YAxisReferential)
            {
                case LineChartTypes.Cadence:
                    {
                        yAxisLabel = CommonResources.Text.LabelCadence;
                        break;
                    }
                case LineChartTypes.Elevation:
                case LineChartTypes.DeviceElevation:
                    {
                        yAxisLabel = CommonResources.Text.LabelElevation;
                        break;
                    }
                case LineChartTypes.HeartRateBPM:
                case LineChartTypes.DiffHeartRateBPM: //Unused - need name changes
                    {
                        yAxisLabel = CommonResources.Text.LabelHeartRate;
                        break;
                    }
                //case LineChartTypes.HeartRatePercentMax:
                //    {
                //        yAxisLabel = CommonResources.Text.LabelHeartRate;
                //        break;
                //    }
                case LineChartTypes.Power:
                    {
                        yAxisLabel = CommonResources.Text.LabelPower;
                        break;
                    }
                case LineChartTypes.Speed:
                case LineChartTypes.DeviceSpeed:
                    {
                        yAxisLabel = CommonResources.Text.LabelSpeed;
                        break;
                    }
                case LineChartTypes.Pace:
                case LineChartTypes.DevicePace:
                    {
                        yAxisLabel = CommonResources.Text.LabelPace;
                        break;
                    }
                case LineChartTypes.SpeedPace: //Only chart selector
                case LineChartTypes.DeviceSpeedPace: //Only chart selector
                    {
                        yAxisLabel = CommonResources.Text.LabelSpeed + " / " + CommonResources.Text.LabelPace;
                        break;
                    }
                case LineChartTypes.Grade:
                    {
                        yAxisLabel = CommonResources.Text.LabelGrade;
                        break;
                    }
                case LineChartTypes.PowerBalance:
                    {
                        yAxisLabel = CommonResources.Text.LabelPowerBalance;
                        break;
                    }
                case LineChartTypes.Temperature:
                    {
                        yAxisLabel = CommonResources.Text.LabelTemperature;
                        break;
                    }
                case LineChartTypes.GroundContactTime:
                    {
                        yAxisLabel = CommonResources.Text.LabelGroundContactTime;
                        break;
                    }
                case LineChartTypes.VerticalOscillation:
                    {
                        yAxisLabel = CommonResources.Text.LabelVerticalOscillation;
                        break;
                    }
                case LineChartTypes.SaturatedHemoglobin:
                    {
                        yAxisLabel = CommonResources.Text.LabelSaturatedHemoglobinPercent;
                        break;
                    }
                case LineChartTypes.TotalHemoglobinConcentration:
                    {
                        yAxisLabel = CommonResources.Text.LabelTotalHemoglobinConcentration;
                        break;
                    }
                case LineChartTypes.Distance:
                case LineChartTypes.DiffDist: //Only for yaxis label
                    {
                        yAxisLabel = CommonResources.Text.LabelDistance;
                        break;
                    }
                case LineChartTypes.DeviceDiffDist:
                    {
                        if (yAxis)
                        {
                            yAxisLabel = CommonResources.Text.LabelDistance;
                        }
                        else
                        {
                            yAxisLabel = Properties.Resources.UI_Chart_Difference;
                        }
                        break;
                    }
                case LineChartTypes.Time:
                case LineChartTypes.DiffTime://Only for yaxis label
                    {
                        yAxisLabel = CommonResources.Text.LabelTime;
                        break;
                    }
                case LineChartTypes.DiffDistTime://Only chart selector
                    {
                        yAxisLabel = Properties.Resources.UI_Chart_Difference;
                        break;
                    }
                default:
                    {
                        Debug.Assert(false, string.Format("Unexpecteded LineChartTypes {0}", YAxisReferential));
                        break;
                    }
            }
            return yAxisLabel;
        }

        public static void SetupXAxisFormatter(XAxisValue axisType, IAxis axis, IActivity activity)
        {
            switch (axisType)
            {
                case XAxisValue.Distance:
                    {
                        axis.Formatter = new Formatter.General(UnitUtil.Distance.DefaultDecimalPrecision);
                        axis.Label = CommonResources.Text.LabelDistance +
                                                UnitUtil.Distance.LabelAbbrAct2(activity);
                        break;
                    }
                case XAxisValue.Time:
                    {

                        axis.Formatter = new Formatter.SecondsToTime();
                        axis.Label = CommonResources.Text.LabelTime;
                        break;
                    }
                default:
                    {
                        Debug.Assert(false, string.Format("Unexpecteded XAxisValue {0}", axisType));
                        break;
                    }
            }
        }

        public static void SetupYAxisFormatter(LineChartTypes axisType, IAxis axis, IActivity activity)
        {
            axis.SmartZoom = true;
            axis.LabelColor = ColorUtil.ChartColor[axisType].LineNormal;
            switch (axisType)
            {
                case LineChartTypes.Cadence:
                    {
                        axis.Formatter = new Formatter.General(UnitUtil.Cadence.DefaultDecimalPrecision);
                        axis.Label = CommonResources.Text.LabelCadence + UnitUtil.Cadence.LabelAbbr2;
                        break;
                    }
                case LineChartTypes.Grade:
                    {
                        axis.Formatter = new Formatter.General(UnitUtil.Grade.DefaultDecimalPrecision);
                        axis.Label = UnitUtil.Grade.LabelAxis;
                        break;
                    }
                case LineChartTypes.Elevation:
                case LineChartTypes.DeviceElevation:
                    {
                        axis.Formatter = new Formatter.General(UnitUtil.Elevation.DefaultDecimalPrecision);
                        axis.Label = CommonResources.Text.LabelElevation + UnitUtil.Elevation.LabelAbbrAct2(activity);
                        break;
                    }
                case LineChartTypes.HeartRateBPM:
                case LineChartTypes.DiffHeartRateBPM:
                    {
                        axis.Formatter = new Formatter.General(UnitUtil.HeartRate.DefaultDecimalPrecision);
                        axis.Label = CommonResources.Text.LabelHeartRate + UnitUtil.HeartRate.LabelAbbr2;
                        break;
                    }
                //case LineChartTypes.HeartRatePercentMax:
                //    {
                //        axis.Label = CommonResources.Text.LabelHeartRate + " (" +
                //                                CommonResources.Text.LabelPercentOfMax + ")";
                //        break;
                //    }
                case LineChartTypes.Power:
                    {
                        axis.Formatter = new Formatter.General(UnitUtil.Power.DefaultDecimalPrecision);
                        axis.Label = CommonResources.Text.LabelPower + UnitUtil.Power.LabelAbbr2;
                        break;
                    }
                case LineChartTypes.Speed:
                case LineChartTypes.DeviceSpeed:
                    {
                        axis.Formatter = new Formatter.General(UnitUtil.Speed.DefaultDecimalPrecision);
                        axis.Label = CommonResources.Text.LabelSpeed + UnitUtil.Speed.LabelAbbrAct2(activity);
                        break;
                    }
                case LineChartTypes.Pace:
                case LineChartTypes.DevicePace:
                    {
                        axis.Formatter = new Formatter.SecondsToTime();
                        axis.Label = CommonResources.Text.LabelPace + UnitUtil.Pace.LabelAbbrAct2(activity);
                        break;
                    }
                case LineChartTypes.PowerBalance:
                    {
                        axis.Formatter = new Formatter.Percent();
                        axis.Label = CommonResources.Text.LabelPowerBalance;
                        break;
                    }
                case LineChartTypes.Temperature:
                    {
                        axis.Formatter = new Formatter.General(UnitUtil.Temperature.DefaultDecimalPrecision);
                        axis.Label = CommonResources.Text.LabelTemperature + UnitUtil.Temperature.LabelAbbr2;
                        break;
                    }
                case LineChartTypes.GroundContactTime:
                    {
                        axis.Formatter = new Formatter.General(1);
                        axis.Label = CommonResources.Text.LabelGroundContactTime + " (ms)";
                        break;
                    }
                case LineChartTypes.VerticalOscillation:
                    {
                        axis.Formatter = new Formatter.General();
                        axis.Label = CommonResources.Text.LabelVerticalOscillation + " (" + Length.LabelAbbr(Length.Units.Centimeter) + ")";
                        break;
                    }
                case LineChartTypes.SaturatedHemoglobin:
                    {
                        axis.Formatter = new Formatter.Percent();
                        axis.Label = CommonResources.Text.LabelSaturatedHemoglobinPercent;
                        break;
                    }
                case LineChartTypes.TotalHemoglobinConcentration:
                    {
                        axis.Formatter = new Formatter.General();
                        axis.Label = CommonResources.Text.LabelTotalHemoglobinConcentration;
                        break;
                    }
                case LineChartTypes.DiffTime:
                case LineChartTypes.Time:
                    {
                        axis.Formatter = new Formatter.SecondsToTime();
                        axis.Label = CommonResources.Text.LabelTime;
                        break;
                    }
                case LineChartTypes.DiffDist:
                case LineChartTypes.DeviceDiffDist:
                    {
                        axis.Formatter = new Formatter.General(UnitUtil.Elevation.DefaultDecimalPrecision - 1);
                        axis.Label = CommonResources.Text.LabelDistance + UnitUtil.Elevation.LabelAbbrAct2(activity);
                        break;
                    }
                case LineChartTypes.Distance:
                    {
                        axis.Formatter = new Formatter.General(UnitUtil.Distance.DefaultDecimalPrecision);
                        axis.Label = CommonResources.Text.LabelDistance + UnitUtil.Distance.LabelAbbrAct2(activity);
                        break;
                    }
                default:
                    {
                        Debug.Assert(false, string.Format("Unexpecteded LineChartTypes {0}", axisType));
                        break;
                    }
            }
        }

        /************************************************/
        /// <summary>
        /// Some chartTypes share axis. Could be changeable in the chart
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static LineChartTypes ChartToAxis(LineChartTypes chart)
        {
            LineChartTypes axis = chart;

            if (chart == LineChartTypes.DeviceSpeed)
            {
                axis = LineChartTypes.Speed;
            }
            else if (chart == LineChartTypes.DevicePace)
            {
                axis = LineChartTypes.Pace;
            }
            else if (chart == LineChartTypes.DeviceElevation)
            {
                axis = LineChartTypes.Elevation;
            }
            else if (chart == LineChartTypes.DeviceDiffDist)
            {
                axis = LineChartTypes.DiffDist;
            }

            return axis;
        }

        public static INumericTimeDataSeries GetSmoothedActivityTrack(Data.TrailResult result, LineChartTypes lineChart, TrailResult refRes)
        {
            // Fail safe
            INumericTimeDataSeries track;

            switch (lineChart)
            {
                case LineChartTypes.Cadence:
                    {
                        track = result.CadencePerMinuteTrack0(refRes);
                        break;
                    }
                case LineChartTypes.Elevation:
                    {
                        track = result.ElevationMetersTrack0(refRes);
                        break;
                    }
                case LineChartTypes.HeartRateBPM:
                    {
                        track = result.HeartRatePerMinuteTrack0(refRes);
                        break;
                    }
                //case LineChartTypes.HeartRatePercentMax:
                //    {
                //        track = result.HeartRatePerMinutePercentMaxTrack;
                //        break;
                //    }
                case LineChartTypes.Power:
                    {
                        track = result.PowerWattsTrack0(refRes);
                        break;
                    }
                case LineChartTypes.Grade:
                    {
                        track = result.GradeTrack0(refRes);
                        break;
                    }

                case LineChartTypes.Speed:
                    {
                        track = result.SpeedTrack0(refRes);
                        break;
                    }

                case LineChartTypes.Pace:
                    {
                        track = result.PaceTrack0(refRes);
                        break;
                    }

                case LineChartTypes.PowerBalance:
                    {
                        track = result.PowerBalanceTrack0(refRes);
                        break;
                    }

                case LineChartTypes.Temperature:
                    {
                        track = result.TemperatureTrack0(refRes);
                        break;
                    }
                case LineChartTypes.GroundContactTime:
                    {
                        track = result.GroundContactTimeTrack0(refRes);
                        break;
                    }
                case LineChartTypes.VerticalOscillation:
                    {
                        track = result.VerticalOscillationTrack0(refRes);
                        break;
                    }
                case LineChartTypes.SaturatedHemoglobin:
                    {
                        track = result.SaturatedHemoglobinTrack0(refRes);
                        break;
                    }
                case LineChartTypes.TotalHemoglobinConcentration:
                    {
                        track = result.TotalHemoglobinConcentrationTrack0(refRes);
                        break;
                    }
                case LineChartTypes.DiffTime:
                    {
                        track = result.DiffTimeTrack0(refRes);
                        break;
                    }
                case LineChartTypes.DiffDist:
                    {
                        track = result.DiffDistTrack0(refRes);
                        break;
                    }
                case LineChartTypes.DeviceSpeed:
                case LineChartTypes.DevicePace:
                    {
                        track = result.DeviceSpeedPaceTrack0(refRes);
                        break;
                    }
                case LineChartTypes.DeviceElevation:
                    {
                        track = result.DeviceElevationTrack0(refRes);
                        break;
                    }
                case LineChartTypes.DeviceDiffDist:
                    {
                        track = result.DeviceDiffDistTrack0(refRes);
                        break;
                    }

                default:
                    {
                        Debug.Assert(false, string.Format("Unexpecteded LineChartTypes {0}", lineChart));
                        // Fail safe
                        track = new TrackUtil.NumericTimeDataSeries();
                        break;
                    }
            }
            return track;
        }

        public static float getSyncGraphOffset(INumericTimeDataSeries graphPoints, INumericTimeDataSeries refGraphPoints, SyncGraphMode syncGraph)
        {
            float syncGraphOffset = 0;
            if (graphPoints != refGraphPoints &&
                refGraphPoints != null && refGraphPoints.Count > 1 &&
                graphPoints != null && graphPoints.Count > 1)
            {
                switch (syncGraph)
                {
                    case SyncGraphMode.None:
                        break;
                    case SyncGraphMode.Start:
                        syncGraphOffset = refGraphPoints[0].Value - graphPoints[0].Value;
                        break;
                    case SyncGraphMode.End:
                        syncGraphOffset = refGraphPoints[refGraphPoints.Count - 1].Value - graphPoints[graphPoints.Count - 1].Value;
                        break;
                    case SyncGraphMode.Average:
                        syncGraphOffset = refGraphPoints.Avg - graphPoints.Avg;
                        break;
                    case SyncGraphMode.Min:
                        syncGraphOffset = refGraphPoints.Min - graphPoints.Min;
                        break;
                    case SyncGraphMode.Max:
                        syncGraphOffset = refGraphPoints.Max - graphPoints.Max;
                        break;
                    default:
                        {
                            Debug.Assert(false, string.Format("Unexpecteded SyncGraphMode {0}", syncGraph));
                            break;
                        }
                }
                if (float.IsNaN(syncGraphOffset) || float.IsInfinity(syncGraphOffset))
                {
                    syncGraphOffset = 0;
                }
            }
            return syncGraphOffset;
        }
    }
}
