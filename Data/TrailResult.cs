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
				return distance / 1000;
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
		public float ElevChg {
			get {
				return m_activity.GPSRoute[m_endIndex].Value.ElevationMeters - m_activity.GPSRoute[m_startIndex].Value.ElevationMeters;
			}
		}
		public IDistanceDataTrack DistanceMetersTrack {
			get {
				if (m_distanceMetersTrack == null) {
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
				return m_distanceMetersTrack;
			}
		}
		public INumericTimeDataSeries ElevationMetersTrack {
			get {
				if (m_elevationMetersTrack == null) {
					m_elevationMetersTrack = new NumericTimeDataSeries();
					for (int i = m_startIndex; i <= m_endIndex; i++) {
						ITimeValueEntry<IGPSPoint> value = m_activity.GPSRoute[i];
						m_elevationMetersTrack.Add(
							m_startTime.AddSeconds(value.ElapsedSeconds),
							value.Value.ElevationMeters
						);
					}
				}
				return m_elevationMetersTrack;
			}
		}

		public IActivityCategory Category {
			get {
				return m_activity.Category;
			}
		}


		public INumericTimeDataSeries initTrack(INumericTimeDataSeries source) {
			INumericTimeDataSeries track = new NumericTimeDataSeries();
			if (source != null) {
				for (int i = m_startIndex; i <= m_endIndex; i++) {
					ITimeValueEntry<float> value = source[i];
					track.Add(
						m_startTime.AddSeconds(value.ElapsedSeconds),
						value.Value
					);
				}
			}
			return track;
		}

		public INumericTimeDataSeries CadencePerMinuteTrack {
			get {
				if (m_cadencePerMinuteTrack == null) {
					m_cadencePerMinuteTrack = this.initTrack(m_activity.CadencePerMinuteTrack);
				}
				return m_cadencePerMinuteTrack;
			}
		}
		public INumericTimeDataSeries HeartRatePerMinuteTrack {
			get {
				if (m_heartRatePerMinuteTrack == null) {
					m_heartRatePerMinuteTrack = this.initTrack(m_activity.HeartRatePerMinuteTrack);
				}
				return m_heartRatePerMinuteTrack;
			}
		}
		public INumericTimeDataSeries PowerWattsTrack {
			get {
				if (m_powerWattsTrack == null) {
					m_powerWattsTrack = this.initTrack(m_activity.PowerWattsTrack);
				}
				return m_powerWattsTrack;
			}
		}
		public INumericTimeDataSeries SpeedTrack {
			get {
				if (m_speedTrack == null) {
					
					m_speedTrack  = new NumericTimeDataSeries();
					ActivityInfo activityInfo = ActivityInfoCache.Instance.GetInfo(m_activity);

					m_speedTrack = this.initTrack(activityInfo.SmoothedSpeedTrack);
				}
				return m_speedTrack;
			}
		}
	}
}
