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
        private Controller.TrailController m_controller;

        public Controller.TrailController Controller
        {
            set { m_controller = value; }
        }


        public bool MultipleActivities
        {
            set { m_multiple = value; }
        }

        #region ILabelProvider Members

        public override Image GetImage(object element, TreeList.Column column)
        {
            Data.TrailResult row = TrailsPlugin.UI.Activity.ResultListControl.getTrailResultRow(element);

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
            Data.TrailResult row = TrailsPlugin.UI.Activity.ResultListControl.getTrailResultRow(element);

            //Some special for Summary
            if (TrailsPlugin.Data.Settings.ResultSummaryStdDev && row is SummaryTrailResult)
            {
                SummaryTrailResult row2 = row as SummaryTrailResult;
                if (row2.Results.Count > 1)
                {
                    switch (column.Id)
                    {
                        case TrailResultColumnIds.StartTime:
                            //Not interesting to average time when only one activity. Other multi may be interesting.
                            if (row2.Activities.Count <= 1){return null;}
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
                                return UnitUtil.Distance.ToString(a.Value, m_controller.ReferenceActivity, "") + " σ" + UnitUtil.Elevation.ToString(a.StdDev, m_controller.ReferenceActivity, "");
                            }
                        case TrailResultColumnIds.AvgPace:
                            {
                                SummaryValue<double> a = row2.AvgPaceStdDev();
                                return UnitUtil.Pace.ToString(a.Value, m_controller.ReferenceActivity, "") + " σ" + UnitUtil.Pace.ToString(a.StdDev, m_controller.ReferenceActivity, "");
                            }
                        case TrailResultColumnIds.AvgSpeed:
                            {
                                SummaryValue<double> a = row2.AvgSpeedStdDev();
                                return UnitUtil.Speed.ToString(a.Value, m_controller.ReferenceActivity, "") + " σ" + UnitUtil.Speed.ToString(a.StdDev, m_controller.ReferenceActivity, "");
                            }
                        case TrailResultColumnIds.AvgSpeedPace:
                            {
                                SummaryValue<double> a;
                                if (UnitUtil.PaceOrSpeed.IsPace(m_controller.ReferenceActivity))
                                {
                                    a = row2.AvgPaceStdDev();
                                    return UnitUtil.Pace.ToString(a.Value, m_controller.ReferenceActivity, "") + " σ" + UnitUtil.Pace.ToString(a.StdDev, m_controller.ReferenceActivity, "");
                                }
                                else
                                {
                                    a = row2.AvgSpeedStdDev();
                                    return UnitUtil.Speed.ToString(a.Value, m_controller.ReferenceActivity, "") + " σ" + UnitUtil.Speed.ToString(a.StdDev, m_controller.ReferenceActivity, "");
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
                        //        return UnitUtil.Pace.ToString(a.Value, m_controller.ReferenceActivity, "") + " σ" + UnitUtil.Pace.ToString(a.StdDev, m_controller.ReferenceActivity, "");
                        //    }
                        //case TrailResultColumnIds.Diff:
                        //    {
                        //        SummaryValue<double> a = row2.DiffStdDev();
                        //        return UnitUtil.Elevation.ToString(a.Value, m_controller.ReferenceActivity, "") + " σ" + UnitUtil.Elevation.ToString(a.StdDev, m_controller.ReferenceActivity, "");
                        //    }
                        default:
                            break;
                    }
                }
            }

            switch (column.Id)
            {
                case TrailResultColumnIds.Order:
                    return row.Order.ToString();
                case TrailResultColumnIds.ResultColor:
                    return null;
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
                        return UnitUtil.Distance.ToString(row.StartDistance, m_controller.ReferenceActivity, "");
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
                        return UnitUtil.Distance.ToString(row.Distance, m_controller.ReferenceActivity, "");
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
                    return UnitUtil.Speed.ToString(row.AvgSpeed, m_controller.ReferenceActivity, "");
                case TrailResultColumnIds.FastestSpeed:
                    return UnitUtil.Speed.ToString(row.FastestSpeed, m_controller.ReferenceActivity, "");
                case TrailResultColumnIds.AvgPace:
                    return UnitUtil.Pace.ToString(row.AvgSpeed, m_controller.ReferenceActivity, "");
                case TrailResultColumnIds.FastestPace:
                    return UnitUtil.Pace.ToString(row.FastestSpeed, m_controller.ReferenceActivity, "");
                case TrailResultColumnIds.AvgSpeedPace:
                    return UnitUtil.PaceOrSpeed.ToString(row.AvgSpeed, m_controller.ReferenceActivity, "");
                case TrailResultColumnIds.FastestSpeedPace:
                    return UnitUtil.PaceOrSpeed.ToString(row.FastestSpeed, m_controller.ReferenceActivity, "");
                case TrailResultColumnIds.Name:
                    return row.Name;
                case TrailResultColumnIds.PredictDistance:
                    return UnitUtil.Time.ToString(row.PredictDistance, "");
                case TrailResultColumnIds.IdealTime:
                    return UnitUtil.Time.ToString(row.IdealTime, "");
                case TrailResultColumnIds.GradeRunAdjustedTime:
                    return UnitUtil.Time.ToString(row.GradeRunAdjustedTime, "");
                case TrailResultColumnIds.GradeRunAdjustedPace:
                    return UnitUtil.Pace.ToString(row.GradeRunAdjustedSpeed, m_controller.ReferenceActivity, "");
                case TrailResultColumnIds.Diff:
                    return UnitUtil.Elevation.ToString(row.Diff, "");
                case TrailResultColumnIds.AscendingSpeed_VAM:
                    return UnitUtil.Elevation.ToString(row.AscendingSpeed_VAM, "");
                case TrailResultColumnIds.AveragePowerBalance:
                    if (row is SummaryTrailResult)
                    {
                        return null;
                    }
                    return (row.AveragePowerBalance / 100).ToString("0.0%");
                case TrailResultColumnIds.AverageTemperature:
                    if (row is SummaryTrailResult)
                    {
                        return null;
                    }
                    return (row.AverageTemperature).ToString("0.0");
                case TrailResultColumnIds.AverageGroundContactTime:
                    if (row is SummaryTrailResult)
                    {
                        return null;
                    }
                    return (row.AverageGroundContactTime).ToString("0");
                case TrailResultColumnIds.AverageVerticalOscillation:
                    if (row is SummaryTrailResult)
                    {
                        return null;
                    }
                    return (row.AverageVerticalOscillation).ToString("0.0");
                case TrailResultColumnIds.AverageSaturatedHemoglobin:
                    if (row is SummaryTrailResult)
                    {
                        return null;
                    }
                    return (row.AverageSaturatedHemoglobin / 100).ToString("0.0%");
                case TrailResultColumnIds.AverageTotalHemoglobinConcentration:
                    if (row is SummaryTrailResult)
                    {
                        return null;
                    }
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
