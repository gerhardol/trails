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
using GpsRunningPlugin.Util;

namespace TrailsPlugin.Data
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

    public class LineChartUtil
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
                    {
                        yAxisLabel = CommonResources.Text.LabelSpeed;
                        break;
                    }
                case LineChartTypes.Pace:
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
                case LineChartTypes.DiffDist:
                case LineChartTypes.DeviceDiffDist:
                    {
                        yAxisLabel = CommonResources.Text.LabelDistance;
                        break;
                    }
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
                //case LineChartTypes.DeviceDiffDist:
                //{
                    //    yAxisLabel = CommonResources.Text.LabelDevice;
                    //    break;
                    //}
                default:
                    {
                        //Debug.Assert(false);
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
            axis.LabelColor = LineChartUtil.ChartColor[axisType].LineNormal;
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
                        axis.Label = CommonResources.Text.LabelSpeed + UnitUtil.Pace.LabelAbbrAct2(activity);
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

        public static readonly IDictionary<LineChartTypes, ChartColors> ChartColor = new Dictionary<LineChartTypes, ChartColors>()
            {
                //Aproximate same as ST colors
                {LineChartTypes.Speed, new ChartColors(Color.FromArgb(0x20, 0x4A, 0x87), Color.FromArgb(0xC6, 0xCD, 0xD8), Color.FromArgb(0x94, 0xA7, 0xC2))},
                {LineChartTypes.Pace, new ChartColors(Color.FromArgb(0x20, 0x4A, 0x87), Color.FromArgb(0xC6, 0xCD, 0xD8), Color.FromArgb(0x94, 0xA7, 0xC2))},
                {LineChartTypes.Elevation, new ChartColors(Color.FromArgb(0x8F, 0x59, 0x02), Color.FromArgb(0xE3, 0xD4, 0xBB), Color.FromArgb(0xC8, 0xAE, 0x83))},
                {LineChartTypes.Grade, new ChartColors(Color.FromArgb(0xC1, 0x7D, 0x11), Color.FromArgb(0xEE, 0xDC, 0xBF), Color.FromArgb(0xE0, 0xBF, 0x8A))},
                {LineChartTypes.HeartRateBPM, new ChartColors(/*Red*/ Color.FromArgb(0xCC, 0x00, 0x00), Color.FromArgb(0xF1, 0xBF, 0xBB), Color.FromArgb(0xE5, 0x84, 0x82))},
                {LineChartTypes.Cadence, new ChartColors(Color.FromArgb(0x4E, 0x9A, 0x06), Color.FromArgb(0xD3, 0xE3, 0xBC), Color.FromArgb(0xD3, 0xE3, 0xBC))},
                {LineChartTypes.Power, new ChartColors(Color.FromArgb(0x5C, 0x35, 0x66), Color.FromArgb(0xD7, 0xCB, 0xD3), Color.FromArgb(0xB0, 0x9D, 0xB2))},
                //Private
                {LineChartTypes.DiffTime, new ChartColors(/*DarkCyan*/ Color.FromArgb(0x00, 0x8B, 0x8B), Color.FromArgb(0x89, 0xE9, 0xFF), Color.FromArgb(0x4C, 0xDE, 0xFF))},
                {LineChartTypes.DiffDist, new ChartColors(/*Color.CornflowerBlue*/ Color.FromArgb(0x64, 0x95, 0xED), Color.FromArgb(0x89, 0xE9, 0xFF), Color.FromArgb(0x4C, 0xDE, 0xFF))},
                //Device, slightly red
                {LineChartTypes.DeviceSpeed, new ChartColors(Color.FromArgb(0x3E, 0x41, 0x7A), Color.FromArgb(0xC6, 0xCD, 0xD8), Color.FromArgb(0x94, 0xA7, 0xC2))},
                {LineChartTypes.DevicePace, new ChartColors(Color.FromArgb(0x3E, 0x41, 0x7A), Color.FromArgb(0xC6, 0xCD, 0xD8), Color.FromArgb(0x94, 0xA7, 0xC2))}, 
                {LineChartTypes.DeviceElevation, new ChartColors(Color.FromArgb(0xB7, 0x46, 0x02), Color.FromArgb(0xE3, 0xD4, 0xBB), Color.FromArgb(0xC8, 0xAE, 0x83))},
                {LineChartTypes.DeviceDiffDist, new ChartColors(Color.FromArgb(0x00, 0x8B, 0x8B), Color.FromArgb(0x89, 0xE9, 0xFF), Color.FromArgb(0x4C, 0xDE, 0xFF))},
            };
    }

    public class ChartColors
    {
        public ChartColors(Color LineNormal, Color FillNormal, Color FillSelected)
        {
            this.LineNormal = LineNormal;
            this.FillNormal = FillNormal;
            this.FillSelected = FillSelected;
        }
        public Color LineNormal;
        public Color LineSelected;
        public Color FillNormal;
        public Color FillSelected;
    }
}
