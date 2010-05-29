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
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.Measurement;

namespace TrailsPlugin.Utils {
	class Units {
        public static float ParseElevation(string p)
        {
            Length.Units unit = PluginMain.GetApplication().SystemPreferences.ElevationUnits;
            return (float)Length.ParseDistanceMeters(p, ref unit);
        }
        public static string ElevationToString(double value, string fmt)
        {
            Length.Units units = PluginMain.GetApplication().SystemPreferences.ElevationUnits;
            return LengthToString(value, units, fmt);
        }
        public static string DistanceToString(double value, string fmt)
        {
            Length.Units units = PluginMain.GetApplication().SystemPreferences.DistanceUnits;
            return LengthToString(value, units, fmt);
        }
        public static string LengthToString(double value, Length.Units units, string fmt)
        {
            return Length.ToString(Length.Convert(
                    value,
                    Length.Units.Meter,
                    units), units, "N" + Length.DefaultDecimalPrecision(units)+fmt);
        }

        public static float GetSpeed(double value, IActivity activity, Speed.Units kind)
        {
            //speed is in m/s
            double speed = value;
            Length.Units du;
            if (activity != null)
            {
#if ST_2_1
                du = activity.Category.DistanceUnits;
#else
                du = (kind == Speed.Units.Pace) ? activity.Category.PaceDistance.ValueUnits : activity.Category.SpeedDistance.ValueUnits;
                //scale, custom unit may be other than one
                speed = speed /
                    (float)((kind == Speed.Units.Pace) ? activity.Category.PaceDistance.Value : activity.Category.SpeedDistance.Value);
#endif
            }
            else
            {
                du = PluginMain.GetApplication().SystemPreferences.DistanceUnits;
            }
            //convert from (x*)m/s to (x*)<unit>/s
            speed = GetLength(speed, du);
            
            if (kind == Speed.Units.Speed)
            {
                //speed is <unit>/h
                speed = speed * Utils.Constants.SecondsPerHour;
            }
            else
            {
                //pace is <time (s)>/<unit>
                speed = 1/speed;
            }
            return (float)speed;
        }
        public static float GetElevation(double value, IActivity activity)
        {
            Length.Units du;
            if (activity != null)
            {
                du = activity.Category.ElevationUnits;
            }
            else
            {
                du = PluginMain.GetApplication().SystemPreferences.ElevationUnits;
            }
            return GetLength(value, du);
        }
        public static float GetLength(double value, Length.Units du)
        {
            return (float)Length.Convert(value, Length.Units.Meter, du);
        }
        public static float GetDistance(double value, IActivity activity)
        {
            Length.Units du;
            if (activity != null)
            {
                du = activity.Category.DistanceUnits;
            }
            else
            {
                du = PluginMain.GetApplication().SystemPreferences.DistanceUnits;
            }
            return GetLength(value,du);
        }

        public static string GetSpeedLabel(IActivity activity, Speed.Units kind)
        {
            string speedUnitLabel;
#if ST_2_1
            Length.Units du;
            if (activity != null)
            {
                du = activity.Category.DistanceUnits;
            }
            else
            {
                du = PluginMain.GetApplication().SystemPreferences.DistanceUnits;
            }
            if (kind == Speed.Units.Speed)
            {
                if (IsMetric(du))
                {
                    speedUnitLabel = CommonResources.Text.LabelKmPerHour;
                }
                else
                {
                    speedUnitLabel = CommonResources.Text.LabelMilePerHour;
                }
            }
            else
            {
                if (IsMetric(du))
                {
                    speedUnitLabel = CommonResources.Text.LabelMinPerKm;
                }
                else
                {
                    speedUnitLabel = CommonResources.Text.LabelMinPerMile;
                }
            }
#else
            Length du;
            if (activity != null)
            {
               du = activity.Category.SpeedDistance;
            }
            else
            {
                du = new Length(1, PluginMain.GetApplication().SystemPreferences.DistanceUnits);
            }
            speedUnitLabel = ZoneFiveSoftware.Common.Data.Measurement.Speed.Label(kind, du);
#endif
            return speedUnitLabel;
		}
        public static string GetSpeedLabel(IActivity activity)
        {
            return GetSpeedLabel(activity, Speed.Units.Speed);
        }
        public static string GetPaceLabel(IActivity activity)
        {
            return GetSpeedLabel(activity, Speed.Units.Pace);
        }

        public static string GetElevationLabel(IActivity activity)
        {
            Length.Units du;
            if (activity != null)
            {
                du = activity.Category.ElevationUnits;
            }
            else
            {
                du = PluginMain.GetApplication().SystemPreferences.ElevationUnits;
            }

            return Length.LabelAbbr(du);
        }

        public static string GetDistanceLabel(IActivity activity)
        {
            Length.Units du;
            if (activity != null)
            {
                du = activity.Category.DistanceUnits;
            }
            else
            {
                du = PluginMain.GetApplication().SystemPreferences.DistanceUnits;
            }
            return Length.LabelAbbr(du);
        }

#if ST_2_1
        public static bool IsMetric(Length.Units unit)
        {
			return (int)unit <= (int)Length.Units.Kilometer;
		}
#endif
	}
}
