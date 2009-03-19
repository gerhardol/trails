using System;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.Measurement;

namespace TrailsPlugin.Utils {
	class Units {
		public static string GetSpeedUnitLabelForActivity(IActivity activity) {
			string speedUnitLabel = CommonResources.Text.LabelKmPerHour;

			if (activity != null) {
				if (!IsMetric(activity.Category.DistanceUnits)) {
					speedUnitLabel = CommonResources.Text.LabelMilePerHour;
				}

			}
			return speedUnitLabel;
		}

		public static string GetPaceUnitLabelForActivity(IActivity activity) {
			string paceUnitLabel = CommonResources.Text.LabelMinPerKm;
			if (activity != null) {
				if (!IsMetric(activity.Category.DistanceUnits)) {
					paceUnitLabel = CommonResources.Text.LabelMinPerMile;
				}
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
