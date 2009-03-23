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
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.Measurement;

namespace TrailsPlugin.Data {
	public class TrailResult {
		private IActivity m_activity;
		private int m_order;
		private INumericTimeDataSeries m_cadencePerMinuteTrack;
		private INumericTimeDataSeries m_heartRatePerMinuteTrack;
		private IDistanceDataTrack m_distanceMetersTrack;
		private INumericTimeDataSeries m_elevationMetersTrack;
		private INumericTimeDataSeries m_powerWattsTrack;
		private INumericTimeDataSeries m_speedTrack;
		private INumericTimeDataSeries m_gradeTrack;
		private INumericTimeDataSeries m_paceTrack;
		private int m_startIndex;
		private int m_endIndex;
		private DateTime m_startTime;
		private float m_startDistance;

		public TrailResult(IActivity activity, int order, int startIndex, int endIndex) {
			m_activity = activity;
			m_order = order;
			m_startIndex = startIndex;
			m_endIndex = endIndex;

			m_startTime = m_activity.StartTime.AddSeconds(m_activity.GPSRoute[startIndex].ElapsedSeconds);
			m_startDistance = m_activity.GPSRoute.GetDistanceMetersTrack()[startIndex].Value;

			m_distanceMetersTrack = new DistanceDataTrack();
			IDistanceDataTrack track = m_activity.GPSRoute.GetDistanceMetersTrack();
			float startDistance = track[m_startIndex].Value;
			if (track != null) {
				for (int i = m_startIndex; i <= m_endIndex; i++) {
					ITimeValueEntry<float> value = track[i];
					m_distanceMetersTrack.Add(
						m_startTime.AddSeconds(value.ElapsedSeconds),
						value.Value - startDistance
					);
				}
			}
		}
		public int Order {
			get {
				return m_order;
			}
		}

		public TimeSpan StartTime {
			get {
				return m_startTime.ToLocalTime().TimeOfDay;
			}
		}
		public TimeSpan EndTime {
			get {
				return m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[m_endIndex]).ToLocalTime().TimeOfDay;
			}
		}
		public TimeSpan Duration {
			get {
				return TimeSpan.FromSeconds(
					m_activity.GPSRoute[m_endIndex].ElapsedSeconds
					- m_activity.GPSRoute[m_startIndex].ElapsedSeconds
				);
			}
		}
		public float Distance {
			get {
				float distance = 0;
				for (int i = m_startIndex; i < m_endIndex; i++) {
					distance += m_activity.GPSRoute[i].Value.DistanceMetersToPoint(
						m_activity.GPSRoute[i + 1].Value
					);
				}
				return (float)Length.Convert(distance, Length.Units.Meter, Utils.Units.MajorLengthUnit(m_activity.Category.DistanceUnits));
			}
		}

		public float AvgCadence {
			get {
				return CadencePerMinuteTrack.Avg;
			}
		}
		public float AvgHR {
			get {
				return HeartRatePerMinuteTrack.Avg;
			}
		}
		public float MaxHR {
			get {
				return HeartRatePerMinuteTrack.Max;
			}
		}
		public float AvgPower {
			get {
				return PowerWattsTrack.Avg;
			}
		}
		public float AvgGrade {
			get {
				return GradeTrack.Avg;
			}
		}
		public float AvgSpeed {
			get {
				return this.SpeedTrack.Avg;
			}
		}
		public float MaxSpeed {
			get {
				return this.SpeedTrack.Max;
			}
		}
		public double AvgPace {
			get {
				return this.PaceTrack.Avg;
			}
		}
		public double MaxPace {
			get {
				return this.PaceTrack.Max;
			}
		}
		public float ElevChg {
			get {
				return (float)Length.Convert(m_activity.GPSRoute[m_endIndex].Value.ElevationMeters - m_activity.GPSRoute[m_startIndex].Value.ElevationMeters,
					Length.Units.Meter,
					m_activity.Category.ElevationUnits
				);
			}
		}
		public IDistanceDataTrack DistanceMetersTrack {
			get {
				return m_distanceMetersTrack;
			}
		}
		public INumericTimeDataSeries ElevationMetersTrack {
			get {
				if (m_elevationMetersTrack == null) {
					ActivityInfo activityInfo = ActivityInfoCache.Instance.GetInfo(m_activity);
					m_elevationMetersTrack = this.copyTrailTrack(activityInfo.SmoothedElevationTrack);
				}
				return m_elevationMetersTrack;
			}
		}

		public IActivityCategory Category {
			get {
				return m_activity.Category;
			}
		}


		public INumericTimeDataSeries copyTrailTrack(INumericTimeDataSeries source) {
			INumericTimeDataSeries track = new NumericTimeDataSeries();
			if (source != null) {
				for(int i = 0; i < m_distanceMetersTrack.Count; i++) {
					DateTime time = m_startTime.AddSeconds(m_distanceMetersTrack[i].ElapsedSeconds);
					ITimeValueEntry<float> value = source.GetInterpolatedValue(time);
					if (value != null) {
						track.Add(
							time,
							value.Value
						);
					}
				}
			}
			return track;
		}

		public INumericTimeDataSeries CadencePerMinuteTrack {
			get {
				if (m_cadencePerMinuteTrack == null) {
					ActivityInfo activityInfo = ActivityInfoCache.Instance.GetInfo(m_activity);
					m_cadencePerMinuteTrack = this.copyTrailTrack(activityInfo.SmoothedCadenceTrack);
				}
				return m_cadencePerMinuteTrack;
			}
		}
		public INumericTimeDataSeries HeartRatePerMinuteTrack {
			get {
				if (m_heartRatePerMinuteTrack == null) {
					ActivityInfo activityInfo = ActivityInfoCache.Instance.GetInfo(m_activity);
					m_heartRatePerMinuteTrack = this.copyTrailTrack(activityInfo.SmoothedHeartRateTrack);
				}
				return m_heartRatePerMinuteTrack;
			}
		}
		public INumericTimeDataSeries PowerWattsTrack {
			get {
				if (m_powerWattsTrack == null) {
					ActivityInfo activityInfo = ActivityInfoCache.Instance.GetInfo(m_activity);
					m_powerWattsTrack = this.copyTrailTrack(activityInfo.SmoothedPowerTrack);
				}
				return m_powerWattsTrack;
			}
		}
		public INumericTimeDataSeries SpeedTrack {
			get {
				if (m_speedTrack == null) {
					m_speedTrack = new NumericTimeDataSeries();
					ActivityInfo activityInfo = ActivityInfoCache.Instance.GetInfo(m_activity);
					for (int i = 0; i < m_distanceMetersTrack.Count; i++) {
						DateTime time = m_startTime.AddSeconds(m_distanceMetersTrack[i].ElapsedSeconds);

						ITimeValueEntry<float> value = activityInfo.SmoothedSpeedTrack.GetInterpolatedValue(time);
						if (value != null) {
							m_speedTrack.Add(
								time,
								(float)Length.Convert(value.Value, Length.Units.Meter, Utils.Units.MajorLengthUnit(m_activity.Category.DistanceUnits)) * Utils.Constants.SecondsPerHour
							);
						}
					}					
				}
				return m_speedTrack;
			}
		}
		public INumericTimeDataSeries PaceTrack {
			get {
				if (m_paceTrack == null) {

					m_paceTrack = new NumericTimeDataSeries();
					if (SpeedTrack != null) {
						for (int i = 0; i < m_distanceMetersTrack.Count; i++) {
							DateTime time = m_startTime.AddSeconds(m_distanceMetersTrack[i].ElapsedSeconds);
							ITimeValueEntry<float> value = SpeedTrack.GetInterpolatedValue(time);
							if (value != null) {
								float pace = (float)Utils.Units.SpeedToPace(value.Value) * Utils.Constants.SecondsPerMinute;
								if (pace != float.NaN) {
									m_paceTrack.Add(
										time,
										pace
									);
								}
							}
						}
					}
				}
				return m_paceTrack;
			}
		}
		public INumericTimeDataSeries GradeTrack {
			get {
				if (m_gradeTrack == null) {
					ActivityInfo activityInfo = ActivityInfoCache.Instance.GetInfo(m_activity);
					m_gradeTrack = this.copyTrailTrack(activityInfo.SmoothedGradeTrack);
				}
				return m_gradeTrack;
			}
		}
	}
}
