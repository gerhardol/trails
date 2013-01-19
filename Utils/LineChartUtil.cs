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
using TrailsPlugin.Data;

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

        private static float[] GetSingleSelection(XAxisValue XAxisReferential, TrailResult tr, TrailResult ReferenceTrailResult, IValueRange<DateTime> v)
        {
            DateTime d1 = v.Lower;
            DateTime d2 = v.Upper;
            if (XAxisReferential == XAxisValue.Time)
            {
                return GetSingleSelectionFromResult(tr, ReferenceTrailResult, d1, d2);
            }
            else
            {
                double t1 = tr.getDistResult(d1);
                double t2 = tr.getDistResult(d2);
                return GetSingleSelectionFromResult(tr, ReferenceTrailResult, t1, t2);
            }
        }
        
        private static float[] GetSingleSelection(XAxisValue XAxisReferential, TrailResult tr, TrailResult ReferenceTrailResult, IValueRange<double> v)
        {
            //Note: Selecting in Route gives unpaused distance, but this should be handled in the selection
            if (XAxisReferential == XAxisValue.Time)
            {
                DateTime d1 = DateTime.MinValue, d2 = DateTime.MinValue;
                d1 = tr.getDateTimeFromDistActivity(v.Lower);
                d2 = tr.getDateTimeFromDistActivity(v.Upper);
                return GetSingleSelectionFromResult(tr, ReferenceTrailResult, d1, d2);
            }
            else
            {
                double t1 = tr.getDistResultFromDistActivity(v.Lower);
                double t2 = tr.getDistResultFromDistActivity(v.Upper);
                return GetSingleSelectionFromResult(tr, ReferenceTrailResult, t1, t2);
            }
        }

        private static float[] GetSingleSelectionFromResult(TrailResult tr, TrailResult ReferenceTrailResult, DateTime d1, DateTime d2)
        {
            float x1 = float.MaxValue, x2 = float.MinValue;
            //Convert to distance display unit, Time is always in seconds
            x1 = (float)(tr.getTimeResult(d1));
            x2 = (float)(tr.getTimeResult(d2));
            return new float[] { x1, x2 };
        }

        private static float[] GetSingleSelectionFromResult(TrailResult tr, TrailResult ReferenceTrailResult, double t1, double t2)
        {
            float x1 = float.MaxValue, x2 = float.MinValue;
            //distance is for result, then to display units
            x1 = (float)TrackUtil.DistanceConvertFrom(t1, ReferenceTrailResult);
            x2 = (float)TrackUtil.DistanceConvertFrom(t2, ReferenceTrailResult);
            return new float[] { x1, x2 };
        }

        internal static IList<float[]> GetResultSelectionFromActivity(XAxisValue XAxisReferential, TrailResult tr, TrailResult ReferenceTrailResult, IItemTrackSelectionInfo sel)
        {
            IList<float[]> result = new List<float[]>();

            //Currently only one range but several regions in the chart can be selected
            //Only use one of the selections
            if (sel.MarkedTimes != null)
            {
                foreach (IValueRange<DateTime> v in sel.MarkedTimes)
                {
                    result.Add(GetSingleSelection(XAxisReferential, tr, ReferenceTrailResult, v));
                }
            }
            else if (sel.MarkedDistances != null)
            {
                foreach (IValueRange<double> v in sel.MarkedDistances)
                {
                    result.Add(GetSingleSelection(XAxisReferential, tr, ReferenceTrailResult, v));
                }
            }
            else if (sel.SelectedTime != null)
            {
                result.Add(GetSingleSelection(XAxisReferential, tr, ReferenceTrailResult, sel.SelectedTime));
            }
            else if (sel.SelectedDistance != null)
            {
                result.Add(GetSingleSelection(XAxisReferential, tr, ReferenceTrailResult, sel.SelectedDistance));
            }
            return result;
        }
    }
}
