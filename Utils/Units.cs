using System;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.Measurement;

namespace TrailsPlugin.Utils {
	class Units {
		public static string GetSpeedUnitLabelForActivity(IActivity activity) {
			string speedUnitLabel = CommonResources.Text.LabelKmPerHour;

			if (activity != null) {
				if (activity.Category.SpeedUnits == Speed.Units.Speed) {
					if (IsMetric(activity.Category.DistanceUnits)) {
						speedUnitLabel = CommonResources.Text.LabelKmPerHour;
					} else {
						speedUnitLabel = CommonResources.Text.LabelMilePerHour;
					}
				} else {
					if (IsMetric(activity.Category.DistanceUnits)) {
						speedUnitLabel = CommonResources.Text.LabelMinPerKm;
					} else {
						speedUnitLabel = CommonResources.Text.LabelMinPerMile;
					}
				}
			}
			return speedUnitLabel;
		}

		public static bool IsMetric(Length.Units unit) {
			return (int)unit <= (int)Length.Units.Kilometer;
		}

		public static bool IsStatute(Length.Units unit) {
			return !IsMetric(unit);
		}

		public static double SpeedToPace(double speed) {
			return Constants.MinutesPerHour / speed;
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
