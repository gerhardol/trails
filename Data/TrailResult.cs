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
		private INumericTimeDataSeries m_distanceMetersTrack;
		private INumericTimeDataSeries m_elevationMetersTrack;
		private INumericTimeDataSeries m_powerWattsTrack;
		private int m_startIndex;
		private int m_endIndex;

		public TrailResult(IActivity activity, int order, int startIndex, int endIndex) {
			m_activity = activity;
			m_order = order;
			m_startIndex = startIndex;
			m_endIndex = endIndex;

			m_cadencePerMinuteTrack = new NumericTimeDataSeries();
			m_heartRatePerMinuteTrack = new NumericTimeDataSeries();
			m_distanceMetersTrack = new NumericTimeDataSeries();
			m_elevationMetersTrack = new NumericTimeDataSeries();
			m_powerWattsTrack = new NumericTimeDataSeries();
			for (int i = m_startIndex; i <= m_endIndex; i++) {
				DateTime time = m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[i]);
				if (m_activity.CadencePerMinuteTrack != null) {
					m_cadencePerMinuteTrack.Add(
						time,
						m_activity.CadencePerMinuteTrack.GetInterpolatedValue(time).Value
					);
				}
				if (m_activity.HeartRatePerMinuteTrack != null) {
					m_heartRatePerMinuteTrack.Add(
						time,
						m_activity.HeartRatePerMinuteTrack.GetInterpolatedValue(time).Value
					);
				}
				if (m_activity.DistanceMetersTrack != null) {
					m_distanceMetersTrack.Add(
						time,
						m_activity.DistanceMetersTrack.GetInterpolatedValue(time).Value
					);
				}
				if (m_activity.ElevationMetersTrack != null) {
					m_elevationMetersTrack.Add(
						time,
						m_activity.ElevationMetersTrack.GetInterpolatedValue(time).Value
					);
				}
				if (m_activity.PowerWattsTrack != null) {
					m_powerWattsTrack.Add(
						time,
						m_activity.PowerWattsTrack.GetInterpolatedValue(time).Value
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
				return m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[m_startIndex]).ToLocalTime().TimeOfDay;
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
				return distance/1000;
			}
		}

		public float AvgCadence {
			get {
				return m_cadencePerMinuteTrack.Avg;
			}
		}
		public float AvgHR {
			get {
				return m_heartRatePerMinuteTrack.Avg;
			}
		}
		public float MaxHR {
			get {
				return m_heartRatePerMinuteTrack.Max;
			}
		}
		public float ElevChg {
			get {
				return m_activity.GPSRoute[m_endIndex].Value.ElevationMeters - m_activity.GPSRoute[m_startIndex].Value.ElevationMeters;
			}
		}
	}
}
