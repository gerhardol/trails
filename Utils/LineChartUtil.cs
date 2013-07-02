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
        Cadence,
        Elevation,
        HeartRateBPM,
        //HeartRatePercentMax,
        Power,
        Grade,
        Speed,
        Pace,
        SpeedPace,
        DiffTime,
        DiffDist,
        DiffDistTime,
        DeviceSpeed,
        DevicePace,
        DeviceSpeedPace,
        DeviceElevation,
        DeviceDiffDist,
        DiffHeartRateBPM, //NotUsedInTrails
        Time, //NotUsedInTrails
        Distance //NotUsedInTrails
    }

    public static class LineChartUtil
    {
        public static string SmoothOverTrailBordersString(SmoothOverTrailBorders t)
        {
            return "Chart Border Smooth: " + Data.Settings.SmoothOverTrailPoints.ToString(); //TODO: Translate
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
                        Debug.Assert(false);
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
            return LineChartTypesString(x);
        }

        public static string LineChartTypesString(LineChartTypes YAxisReferential)
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
                case LineChartTypes.DiffHeartRateBPM:
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
                case LineChartTypes.SpeedPace:
                case LineChartTypes.DeviceSpeedPace:
                    {
                        yAxisLabel = CommonResources.Text.LabelSpeed + CommonResources.Text.LabelPace;
                        break;
                    }
                case LineChartTypes.Grade:
                    {
                        yAxisLabel = CommonResources.Text.LabelGrade;
                        break;
                    }
                case LineChartTypes.Distance:
                case LineChartTypes.DiffDist:
                case LineChartTypes.DeviceDiffDist:
                    {
                        yAxisLabel = CommonResources.Text.LabelDistance;
                        break;
                    }
                case LineChartTypes.Time:
                case LineChartTypes.DiffTime:
                    {
                        yAxisLabel = CommonResources.Text.LabelTime;
                        break;
                    }
                case LineChartTypes.DiffDistTime:
                    {
                        yAxisLabel = CommonResources.Text.LabelDistance + CommonResources.Text.LabelTime;
                        break;
                    }
                default:
                    {
                        Debug.Assert(false);
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
                        Debug.Assert(false);
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
                        Debug.Assert(false);
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
                        Debug.Assert(false);
                        // Fail safe
                        track = new NumericTimeDataSeries();
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
                            Debug.Assert(false);
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
