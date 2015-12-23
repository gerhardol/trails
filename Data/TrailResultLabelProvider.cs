/*
Copyright (C) 2010 Gerhard Olsson

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
using System.Collections.Generic;
using ZoneFiveSoftware.Common.Visuals;
using System.Drawing;
using GpsRunningPlugin.Util;
using ZoneFiveSoftware.Common.Data.Fitness.CustomData;
using ZoneFiveSoftware.Common.Data.Fitness;

namespace TrailsPlugin.Data
{
    class TrailResultLabelProvider : TreeList.DefaultLabelProvider
    {

        private bool m_multiple = false;

        public bool MultipleActivities
        {
            set { m_multiple = value; }
        }

        #region ILabelProvider Members

        public override Image GetImage(object element, TreeList.Column column)
        {
            Data.TrailResult row = (element as TrailResultWrapper).Result;

            if (column.Id == TrailResultColumnIds.ResultColor)
            {
                Bitmap image = new Bitmap(column.Width, 15);
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        image.SetPixel(x, y, row.ResultColor.LineNormal);
                    }
                }
                return image;
            }
            else
            {
                return base.GetImage(row.Activity, column);
            }
        }

        public override string GetText(object element, TreeList.Column column)
        {
            Data.TrailResult row = (element as TrailResultWrapper).Result;

            //Some special for Summary
            if (row is SummaryTrailResult)
            {
                switch (column.Id)
                {
                    case TrailResultColumnIds.AveragePowerBalance:
                    case TrailResultColumnIds.AverageTemperature:
                    case TrailResultColumnIds.AverageGroundContactTime:
                    case TrailResultColumnIds.AverageVerticalOscillation:
                    case TrailResultColumnIds.AverageSaturatedHemoglobin:
                    case TrailResultColumnIds.AverageTotalHemoglobinConcentration:
                        //No implementation, ignore
                        return null;

                    default:
                        if (!Controller.TrailController.Instance.ExplicitSelection &&
                            TrailResultColumnIds.ClimbFields.Contains(column.Id) && !row.ClimbCalculated)
                        {
                            //Potentially many selections (of Ascent etc), no value set
                            return null;
                        }
                        break;
                }

                if (TrailsPlugin.Data.Settings.ResultSummaryStdDev)
                {
                    SummaryTrailResult row2 = row as SummaryTrailResult;
                    if (!row2.IsTotal && row2.Results.Count > 1)
                    {
                        switch (column.Id)
                        {
                            case TrailResultColumnIds.StartTime:
                                //Not interesting to average time when only one activity. Other multi may be interesting.
                                if (row2.Activities.Count <= 1) { return null; }
                                //Only time of day, averaged
                                return row.StartTime.ToLocalTime().ToString("T");
                            case TrailResultColumnIds.Duration:
                                {
                                    SummaryValue<TimeSpan> a = row2.DurationStdDev();
                                    return UnitUtil.Time.ToString(a.Value, "") + " σ" + UnitUtil.Time.ToString(a.StdDev, "");
                                }
                            case TrailResultColumnIds.Distance:
                                {
                                    SummaryValue<double> a = row2.DistanceStdDev();
                                    string d;
                                    if (row.PoolLengthInfo != null)
                                    {
                                        d = UnitUtil.Distance.ToString(a.Value, row.PoolLengthInfo.DistanceUnits, "F0");
                                    }
                                    else
                                    {
                                        d = UnitUtil.Distance.ToString(a.Value, Controller.TrailController.Instance.ReferenceActivity, "");
                                    }
                                    return d + " σ" + UnitUtil.Elevation.ToString(a.StdDev, Controller.TrailController.Instance.ReferenceActivity, "");
                                }
                            case TrailResultColumnIds.AvgPace:
                                {
                                    SummaryValue<double> a = row2.AvgPaceStdDev();
                                    return UnitUtil.Pace.ToString(a.Value, Controller.TrailController.Instance.ReferenceActivity, "") + " σ" + UnitUtil.Pace.ToString(a.StdDev, Controller.TrailController.Instance.ReferenceActivity, "");
                                }
                            case TrailResultColumnIds.AvgSpeed:
                                {
                                    SummaryValue<double> a = row2.AvgSpeedStdDev();
                                    return UnitUtil.Speed.ToString(a.Value, Controller.TrailController.Instance.ReferenceActivity, "") + " σ" + UnitUtil.Speed.ToString(a.StdDev, Controller.TrailController.Instance.ReferenceActivity, "");
                                }
                            case TrailResultColumnIds.AvgSpeedPace:
                                {
                                    SummaryValue<double> a;
                                    if (UnitUtil.PaceOrSpeed.IsPace(Controller.TrailController.Instance.ReferenceActivity))
                                    {
                                        a = row2.AvgPaceStdDev();
                                        return UnitUtil.Pace.ToString(a.Value, Controller.TrailController.Instance.ReferenceActivity, "") + " σ" + UnitUtil.Pace.ToString(a.StdDev, Controller.TrailController.Instance.ReferenceActivity, "");
                                    }
                                    else
                                    {
                                        a = row2.AvgSpeedStdDev();
                                        return UnitUtil.Speed.ToString(a.Value, Controller.TrailController.Instance.ReferenceActivity, "") + " σ" + UnitUtil.Speed.ToString(a.StdDev, Controller.TrailController.Instance.ReferenceActivity, "");
                                    }
                                }
                            //case TrailResultColumnIds.GradeRunAdjustedTime:
                            //    {
                            //        SummaryValue<double> a = row2.GradeRunAdjustedTimeStdDev();
                            //        return UnitUtil.Time.ToString(a.Value, "") + " σ" + UnitUtil.Time.ToString(a.StdDev, "");
                            //    }
                            //case TrailResultColumnIds.GradeRunAdjustedPace:
                            //    {
                            //        SummaryValue<TimeSpan> a = row2.GradeRunAdjustedPaceStdDev();
                            //        return UnitUtil.Pace.ToString(a.Value, Controller.TrailController.Instance.ReferenceActivity, "") + " σ" + UnitUtil.Pace.ToString(a.StdDev, Controller.TrailController.Instance.ReferenceActivity, "");
                            //    }
                            //case TrailResultColumnIds.Diff:
                            //    {
                            //        SummaryValue<double> a = row2.DiffStdDev();
                            //        return UnitUtil.Elevation.ToString(a.Value, Controller.TrailController.Instance.ReferenceActivity, "") + " σ" + UnitUtil.Elevation.ToString(a.StdDev, Controller.TrailController.Instance.ReferenceActivity, "");
                            //    }
                            default:
                                break;
                        }
                    }
                }
            }

            if (row is PausedChildTrailResult)
            {
                switch (column.Id)
                {
                    case TrailResultColumnIds.StartTime:
                    case TrailResultColumnIds.StartDistance:
                    case TrailResultColumnIds.EndTime:
                    case TrailResultColumnIds.Duration:
                        //Show with default formatting
                        break;
                    default:
                        //Not for pauses
                        return null;
                }
            }

            if (row is SubChildTrailResult)
            {
                switch (column.Id)
                {
                    //Ignore wildly inaccurate data, few points for Pool swimming, can be lower than Avg
                    //(not always good on lap level too)
                    case TrailResultColumnIds.FastestSpeed:
                    case TrailResultColumnIds.FastestPace:
                        return null;
                    default:
                        break;
                }
            }

            switch (column.Id)
            {
                case TrailResultColumnIds.ResultColor:
                    return null;

                //String output without formatting
                case TrailResultColumnIds.Order:
                case TrailResultColumnIds.Name:
                case TrailResultColumnIds.TrailName:
                    return base.GetText(row, column);

                case TrailResultColumnIds.StartTime:
                    if (row.Activity == null) return null;
                    string date = "";
                    if (m_multiple)
                    {
                        date = row.StartTime.ToLocalTime().ToShortDateString() + " ";
                    }
                    return date + row.StartTime.ToLocalTime().ToString("T");
                case TrailResultColumnIds.StartDistance:
                    if (row.PoolLengthInfo != null)
                    {
                        return UnitUtil.Distance.ToString(row.StartDistance, row.PoolLengthInfo.DistanceUnits, "F0u");
                    }
                    else
                    {
                        return UnitUtil.Distance.ToString(row.StartDistance, Controller.TrailController.Instance.ReferenceActivity, "");
                    }
                case TrailResultColumnIds.EndTime:
                    if (row.Activity == null) return null;
                    return row.EndTime.ToLocalTime().ToString("T");
                case TrailResultColumnIds.Duration:
                    return UnitUtil.Time.ToString(row.Duration, "");
                case TrailResultColumnIds.Distance:
                    if (row.PoolLengthInfo != null)
                    {
                        return UnitUtil.Distance.ToString(row.Distance, row.PoolLengthInfo.DistanceUnits, "F0u");
                    }
                    else
                    {
                        return UnitUtil.Distance.ToString(row.Distance, Controller.TrailController.Instance.ReferenceActivity, "");
                    }
                case TrailResultColumnIds.AvgCadence:
                    return UnitUtil.Cadence.ToString(row.AvgCadence);
                case TrailResultColumnIds.AvgHR:
                    return UnitUtil.HeartRate.ToString(row.AvgHR);
                case TrailResultColumnIds.MaxHR:
                    return UnitUtil.HeartRate.ToString(row.MaxHR);
                case TrailResultColumnIds.Ascent:
                    return UnitUtil.Elevation.ToString(row.Ascent);
                case TrailResultColumnIds.Descent:
                    return UnitUtil.Elevation.ToString(row.Descent);
                case TrailResultColumnIds.ElevChg:
                    return (row.ElevChg > 0 ? "+" : "") + UnitUtil.Elevation.ToString(row.ElevChg, "");
                case TrailResultColumnIds.AvgPower:
                    return UnitUtil.Power.ToString(row.AvgPower);
                case TrailResultColumnIds.AscAvgGrade:
                    return (row.AscAvgGrade).ToString("0.0%");
                case TrailResultColumnIds.AscMaxAvgGrade:
                    return (row.AscMaxAvgGrade).ToString("0.0%");
                case TrailResultColumnIds.DescAvgGrade:
                    return (row.DescAvgGrade).ToString("0.0%");
                case TrailResultColumnIds.AvgSpeed:
                    return UnitUtil.Speed.ToString(row.AvgSpeed, Controller.TrailController.Instance.ReferenceActivity, "");
                case TrailResultColumnIds.FastestSpeed:
                    return UnitUtil.Speed.ToString(row.FastestSpeed, Controller.TrailController.Instance.ReferenceActivity, "");
                case TrailResultColumnIds.AvgPace:
                    return UnitUtil.Pace.ToString(row.AvgSpeed, Controller.TrailController.Instance.ReferenceActivity, "");
                case TrailResultColumnIds.FastestPace:
                    return UnitUtil.Pace.ToString(row.FastestSpeed, Controller.TrailController.Instance.ReferenceActivity, "");
                case TrailResultColumnIds.AvgSpeedPace:
                    return UnitUtil.PaceOrSpeed.ToString(row.AvgSpeed, Controller.TrailController.Instance.ReferenceActivity, "");
                case TrailResultColumnIds.FastestSpeedPace:
                    return UnitUtil.PaceOrSpeed.ToString(row.FastestSpeed, Controller.TrailController.Instance.ReferenceActivity, "");
                case TrailResultColumnIds.PredictDistance:
                    return UnitUtil.Time.ToString(row.PredictDistance, "");
                case TrailResultColumnIds.IdealTime:
                    return UnitUtil.Time.ToString(row.IdealTime, "");
                case TrailResultColumnIds.GradeRunAdjustedTime:
                    return UnitUtil.Time.ToString(row.GradeRunAdjustedTime, "");
                case TrailResultColumnIds.GradeRunAdjustedPace:
                    return UnitUtil.Pace.ToString(row.GradeRunAdjustedSpeed, Controller.TrailController.Instance.ReferenceActivity, "");
                case TrailResultColumnIds.Diff:
                    return UnitUtil.Elevation.ToString(row.Diff, "");
                case TrailResultColumnIds.AscendingSpeed_VAM:
                    return UnitUtil.Elevation.ToString(row.AscendingSpeed_VAM, "");
                case TrailResultColumnIds.AveragePowerBalance:
                    return (row.AveragePowerBalance / 100).ToString("0.0%");
                case TrailResultColumnIds.AverageTemperature:
                    return (row.AverageTemperature).ToString("0.0");
                case TrailResultColumnIds.AverageGroundContactTime:
                    return (row.AverageGroundContactTime).ToString("0");
                case TrailResultColumnIds.AverageVerticalOscillation:
                    return (row.AverageVerticalOscillation).ToString("0.0");
                case TrailResultColumnIds.AverageSaturatedHemoglobin:
                    return (row.AverageSaturatedHemoglobin / 100).ToString("0.0%");
                case TrailResultColumnIds.AverageTotalHemoglobinConcentration:
                    return (row.AverageTotalHemoglobinConcentration).ToString("0.0");

                default:

                    if (TrailResultColumns.IsLapField(column.Id))
                    {
                        ILapInfo lap = row.LapInfo;
                        if (lap != null)
                        {
                            //The column Id is faked to not clash with the internal ids
                            TreeList.Column c = new TreeList.Column(TrailResultColumns.LapId(column.Id));
                            return base.GetText(lap, c);
                        }
                        //Not Splits trail
                        return null;
                    }
                    else if (TrailResultColumns.IsSwimLapField(column.Id))
                    {
                        IPoolLengthInfo lap = row.PoolLengthInfo;
                        if (lap != null)
                        {
                            //The column Id is faked to not clash with the internal ids
                            TreeList.Column c = new TreeList.Column(TrailResultColumns.SwimLapId(column.Id));
                            return base.GetText(lap, c);
                        }
                        //Not Splits trail
                        return null;
                    }
                    else if (TrailResultColumns.IsActivityField(column.Id))
                    {
                        if (row is ParentTrailResult)
                        {
                            if (row.Activity == null) return null;
                            if(column.Id == TrailResultColumnIds.MetaData_Source)
                            {
                                return row.Activity.Metadata.Source;
                            }
                            return base.GetText(row.Activity, column);
                        }
                        return null;
                    }
                    else
                    {
                        if (row.Activity == null) return null;
                        ICustomDataFieldDefinition cust = TrailResultColumns.CustomDef(column.Id);
                        if (cust != null)
                        {
                            if (row is ParentTrailResult)
                            {
                                return row.Activity.GetCustomDataValue(TrailResultColumns.CustomDef(column.Id)).ToString();
                            }
                            return null;
                        }
                    }

                    System.Diagnostics.Debug.Assert(false, string.Format("No label info for id {0}", column.Id));
                    return null;
            }
        }

        #endregion
    }
}
