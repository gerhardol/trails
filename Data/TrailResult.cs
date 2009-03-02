using System;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Algorithm;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;

namespace TrailsPlugin.Data {
	public class TrailResult {
		private IActivity m_activity;
		private int m_order;
		public TrailResult(IActivity activity, int order) {
			m_activity = activity;
			m_order = order;
		}
		public int startIndex;
		public int endIndex;
		public int Order {
			get {
				return m_order;
			}
		}

		public TimeSpan StartTime {
			get {
				return m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[startIndex]).TimeOfDay;
			}
		}
		public TimeSpan EndTime {
			get {
				return m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[endIndex]).TimeOfDay;
			}
		}
		public TimeSpan Duration {
			get {
				return TimeSpan.FromSeconds(
					m_activity.GPSRoute[endIndex].ElapsedSeconds
					- m_activity.GPSRoute[startIndex].ElapsedSeconds
				);
			}
		}
		public float Distance {
			get {
				float distance = 0;
				for (int i = startIndex; i < endIndex; i++) {
					distance += m_activity.GPSRoute[i].Value.DistanceMetersToPoint(
						m_activity.GPSRoute[i + 1].Value
					);
				}
				return distance/1000;
			}
		}

		public int AvgCadence {
			get {
				float cadence = 0;
				for (int i = startIndex; i < endIndex; i++) {
					cadence += m_activity.CadencePerMinuteTrack[i].Value;
				}
				return (int)(cadence / (endIndex - startIndex + 1));
			}
		}
	}
}
