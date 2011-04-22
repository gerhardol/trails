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
        DiffHeartRateBPM, //NotUsedInTrails
        Time, //NotUsedInTrails
        Distance //NotUsedInTrails
    }

    public class LineChartUtil
        {
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
            return LineChartTypesString((LineChartTypes)x);
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
                    {
                        axis.Formatter = new Formatter.General(UnitUtil.Speed.DefaultDecimalPrecision);
                        axis.Label = CommonResources.Text.LabelSpeed + UnitUtil.Pace.LabelAbbrAct2(activity);
                        break;
                    }
                case LineChartTypes.Pace:
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

        public static readonly IDictionary<LineChartTypes, Color> ChartColor = new Dictionary<LineChartTypes, Color>()
            {
                //Aproximate same as ST colors
                {LineChartTypes.Speed, Color.FromArgb(32, 74, 135)},
                {LineChartTypes.Pace, Color.FromArgb(32, 74, 135)},
                {LineChartTypes.Elevation, Color.FromArgb(143, 89, 2)},
                {LineChartTypes.Grade, Color.FromArgb(193, 125, 17)},
                {LineChartTypes.HeartRateBPM, Color.Red},
                {LineChartTypes.Cadence, Color.FromArgb(78, 154, 6)},
                {LineChartTypes.Power, Color.FromArgb(92, 53, 102)},
                //Private
                {LineChartTypes.DiffTime, Color.DarkCyan},
                {LineChartTypes.DiffDist, Color.CornflowerBlue}
            };
    }
}
