/******************************************************************************

    This file is part of TrailsPlugin.

    TrailsPlugin is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TrailsPlugin is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TrailsPlugin.  If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

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
            return Length.ToString(Length.Convert(
                    value,
                    Length.Units.Meter,
                    units), units, "N" + Length.DefaultDecimalPrecision(units)+fmt);
        }
        public static string ToString(double value, Length.Units units)
        {
            return Length.ToString(Length.Convert(
                    value,
                    Length.Units.Meter,
                    units), units, "N" + Length.DefaultDecimalPrecision(units));
        }

		public static string GetSpeedUnitLabelForActivity(IActivity activity) {
			Length.Units du;
            if (activity != null)
            {
                du = activity.Category.DistanceUnits;
            }
            else
            {
                du = PluginMain.GetApplication().SystemPreferences.DistanceUnits;
            }
            string speedUnitLabel;
#if ST_2_1
            if (IsMetric(du))
            {
                speedUnitLabel = CommonResources.Text.LabelKmPerHour;
            }
            else
            {
                speedUnitLabel = CommonResources.Text.LabelMilePerHour;
            }
#else
            if (activity != null)
            {
                speedUnitLabel = ZoneFiveSoftware.Common.Data.Measurement.Speed.Label(
                    ZoneFiveSoftware.Common.Data.Measurement.Speed.Units.Speed,
                    activity.Category.SpeedDistance);
            }
            else
            {
                speedUnitLabel = ZoneFiveSoftware.Common.Data.Measurement.Speed.Label(
                    ZoneFiveSoftware.Common.Data.Measurement.Speed.Units.Speed,
                    new Length(1, du));
            }
#endif
            return speedUnitLabel;
		}

		public static string GetPaceUnitLabelForActivity(IActivity activity) {
            Length.Units du;
            if (activity != null)
            {
                du = activity.Category.DistanceUnits;
            }
            else
            {
                du = PluginMain.GetApplication().SystemPreferences.DistanceUnits;
            }

			string paceUnitLabel;
#if ST_2_1
            if (IsMetric(du))
            {
                paceUnitLabel = CommonResources.Text.LabelMinPerKm;
            }
            else
            {
                paceUnitLabel = CommonResources.Text.LabelMinPerMile;
            }
#else
            if (activity != null)
            {
                paceUnitLabel = ZoneFiveSoftware.Common.Data.Measurement.Speed.Label(
                    ZoneFiveSoftware.Common.Data.Measurement.Speed.Units.Pace,
                    activity.Category.PaceDistance);
            }
            else
            {
                paceUnitLabel = ZoneFiveSoftware.Common.Data.Measurement.Speed.Label(
                    ZoneFiveSoftware.Common.Data.Measurement.Speed.Units.Pace,
                    new Length(1, du));
            }
#endif
			return paceUnitLabel;
		}

		public static bool IsMetric(Length.Units unit) {
			return (int)unit <= (int)Length.Units.Kilometer;
		}

		public static bool IsStatute(Length.Units unit) {
			return !IsMetric(unit);
		}

		public static double SpeedToPace(double speed) {
			if (speed == 0) {
				return double.NaN;
			} else {
				return Constants.MinutesPerHour / speed;
			}
		}

		public static double PaceToSpeed(double pace) {
			return Constants.MinutesPerHour / pace;
		}

		public static Length.Units MajorLengthUnit(Length.Units unit) {
			if (IsMetric(unit)) {
				return Length.Units.Kilometer;
			} else {
				return Length.Units.Mile;
			}
		}
	}
}
