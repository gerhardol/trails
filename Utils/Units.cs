using System;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.Measurement;

namespace TrailsPlugin.Utils {
	class Units {
		public static string ToString(double value, Length.Units units) {
			return Length.ToString(Length.Convert(
					value,
					Length.Units.Meter,
					units), units, "N" + Length.DefaultDecimalPrecision(units));
		}

		public static string GetSpeedUnitLabelForActivity(IActivity activity) {
			Length.Units du = PluginMain.GetApplication().SystemPreferences.DistanceUnits;
			if (activity != null) {
				du = activity.Category.DistanceUnits;
			}

			string speedUnitLabel = CommonResources.Text.LabelKmPerHour;
			if (!IsMetric(du)) {
				speedUnitLabel = CommonResources.Text.LabelMilePerHour;
			}
			return speedUnitLabel;
		}

		public static string GetPaceUnitLabelForActivity(IActivity activity) {
			Length.Units du = PluginMain.GetApplication().SystemPreferences.DistanceUnits;
			if (activity != null) {
				du = activity.Category.DistanceUnits;
			}

			string paceUnitLabel = CommonResources.Text.LabelMinPerKm;
			if (!IsMetric(du)) {
				paceUnitLabel = CommonResources.Text.LabelMinPerMile;
			}
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
