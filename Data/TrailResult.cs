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
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.Measurement;
using ITrailExport;

namespace TrailsPlugin.Data {
    public class TrailResult : ITrailResult
    {
		private IActivity m_activity;
		private int m_order;
		private INumericTimeDataSeries m_cadencePerMinuteTrack;
		private INumericTimeDataSeries m_heartRatePerMinuteTrack;
		private IDistanceDataTrack m_distanceMetersTrack;
		private INumericTimeDataSeries m_elevationMetersTrack;
		private INumericTimeDataSeries m_powerWattsTrack;
        private INumericTimeDataSeries m_speedTrack;
        private INumericTimeDataSeries m_gradeTrack;
        private IGPSRoute m_gpsTrack;
        private INumericTimeDataSeries m_paceTrack;
        private int m_startIndex;
		private int m_endIndex;
        private DateTime m_startTime;
        private float m_startDistance;
        private float m_distDiff; //to give quality of results

        public TrailResult(IActivity activity, int order, int startIndex, int endIndex): 
            this(activity, order, startIndex, endIndex, -1)
        {
        }
		public TrailResult(IActivity activity, int order, int startIndex, int endIndex, float distDiff) {

            m_activity = activity;
			m_order = order;
			m_startIndex = startIndex;
			m_endIndex = endIndex;
            m_distDiff = distDiff;

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
        public TrailResult(IActivity activity, int matches, float distDiff) { }

        public IActivity Activity
        {
            get { return m_activity; }
        }
        public float DistDiff
        {
            get { return m_distDiff; }
        }
        public int Order
        {
			get {
				return m_order;
			}
		}

        public TimeSpan StartTime
        {
            get
            {
                return m_startTime.ToLocalTime().TimeOfDay;
            }
        }
        public TimeSpan EndTime
        {
            get
            {
                return m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[m_endIndex]).ToLocalTime().TimeOfDay;
            }
        }
        public DateTime FirstTime
        {
            get
            {
                return m_startTime;
            }
        }
        public DateTime LastTime
        {
            get
            {
                return m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[m_endIndex]);
            }
        }
        public double FirstDist
        {
            get
            {
                return m_startDistance;
            }
        }
        public double LastDist
        {
            get
            {
                return m_activity.GPSRoute.GetDistanceMetersTrack()[m_endIndex].Value;
            }
        }
        public double getDistAt(DateTime t)
        {
            return m_activity.GPSRoute.GetDistanceMetersTrack().GetInterpolatedValue(t).Value -FirstDist;
        }
        public DateTime getTimeAt(double t)
        {
            return m_activity.GPSRoute.GetDistanceMetersTrack().GetTimeAtDistanceMeters(t);
        }
        public TimeSpan Duration
        {
			get {
				return TimeSpan.FromSeconds(
					m_activity.GPSRoute[m_endIndex].ElapsedSeconds
					- m_activity.GPSRoute[m_startIndex].ElapsedSeconds
				);
			}
		}
		public string Distance {
			get {
				float distance = 0;
				for (int i = m_startIndex; i < m_endIndex; i++) {
					distance += m_activity.GPSRoute[i].Value.DistanceMetersToPoint(
						m_activity.GPSRoute[i + 1].Value
					);
				}
                return Utils.Units.DistanceToString(distance, "");
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
		public float FastestSpeed {
			get {
				return this.SpeedTrack.Max;
			}
		}
		public double AvgPace {
			get {
				return this.PaceTrack.Avg;
			}
		}
		public double FastestPace {
			get {
				return this.PaceTrack.Min;
			}
		}
        public string ElevChg
        {
			get {
				float value = m_activity.GPSRoute[m_endIndex].Value.ElevationMeters - m_activity.GPSRoute[m_startIndex].Value.ElevationMeters;
				return (value > 0 ? "+" : "") + Utils.Units.ElevationToString(value, "");
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

        public INumericTimeDataSeries copyTrailTrack(INumericTimeDataSeries source)
        {
            INumericTimeDataSeries track = new NumericTimeDataSeries();
            if (source != null)
            {
                for (int i = 0; i < m_distanceMetersTrack.Count; i++)
                {
                    DateTime time = m_startTime.AddSeconds(m_distanceMetersTrack[i].ElapsedSeconds);
                    ITimeValueEntry<float> value = source.GetInterpolatedValue(time);
                    if (value != null)
                    {
                        track.Add(time, value.Value);
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
                            float speed = Utils.Units.GetSpeed(value.Value, m_activity, Speed.Units.Speed);
                            m_speedTrack.Add(time, speed);
						}
					}
				}
				return m_speedTrack;
			}
		}
		public INumericTimeDataSeries PaceTrack {
			get {
				if (m_paceTrack == null) {
                    //PaceTrack could share a common base track (in m/s) with SpeedTrack,
                    //and be converted to pace/speed units when referenced
                    m_paceTrack = new NumericTimeDataSeries();
                    ActivityInfo activityInfo = ActivityInfoCache.Instance.GetInfo(m_activity);
                    for (int i = 0; i < m_distanceMetersTrack.Count; i++)
                    {
                        DateTime time = m_startTime.AddSeconds(m_distanceMetersTrack[i].ElapsedSeconds);
                        ITimeValueEntry<float> value = activityInfo.SmoothedSpeedTrack.GetInterpolatedValue(time);
                        if (value != null)
                        {
                            float pace = Utils.Units.GetSpeed(value.Value, m_activity, Speed.Units.Pace);
                            if (pace != float.NaN)
                            {
                                m_paceTrack.Add(time, pace);
                            }
                        }
                    }
				}
				return m_paceTrack;
			}
		}
        public INumericTimeDataSeries GradeTrack
        {
            get
            {
                if (m_gradeTrack == null)
                {
                    ActivityInfo activityInfo = ActivityInfoCache.Instance.GetInfo(m_activity);
                    m_gradeTrack = this.copyTrailTrack(activityInfo.SmoothedGradeTrack);
                }
                return m_gradeTrack;
            }
        }
        public IGPSRoute GpsTrack
        {
            get
            {
                if (m_gpsTrack == null)
                {
                    m_gpsTrack = new GPSRoute();
                    for (int i = m_startIndex; i < m_endIndex; i++)
                    {
                        m_gpsTrack.Add(m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[i]), m_activity.GPSRoute[i].Value);
                    }
                }
                return m_gpsTrack;
            }
        }

        #region Implementation of ITrailResult

        float ITrailResult.AvgCadence
        {
            get { return AvgCadence; }
        }

        float ITrailResult.AvgGrade
        {
            get { return AvgGrade; }
        }

        float ITrailResult.AvgHR
        {
            get { return AvgHR; }
        }

        double ITrailResult.AvgPace
        {
            get { return AvgPace; }
        }

        float ITrailResult.AvgPower
        {
            get { return AvgPower; }
        }

        float ITrailResult.AvgSpeed
        {
            get { return AvgSpeed; }
        }

        INumericTimeDataSeries ITrailResult.CadencePerMinuteTrack
        {
            get { return CadencePerMinuteTrack; }
        }

        IActivityCategory ITrailResult.Category
        {
            get { return Category; }
        }

        INumericTimeDataSeries ITrailResult.CopyTrailTrack(INumericTimeDataSeries source)
        {
            return copyTrailTrack(source);
        }

        string ITrailResult.Distance
        {
            get { return Distance; }
        }

        IDistanceDataTrack ITrailResult.DistanceMetersTrack
        {
            get { return DistanceMetersTrack; }
        }

        TimeSpan ITrailResult.Duration
        {
            get { return Duration; }
        }

        string ITrailResult.ElevChg
        {
            get { return ElevChg; }
        }

        INumericTimeDataSeries ITrailResult.ElevationMetersTrack
        {
            get { return ElevationMetersTrack; }
        }

        TimeSpan ITrailResult.EndTime
        {
            get { return EndTime; }
        }

        double ITrailResult.FastestPace
        {
            get { return FastestPace; }
        }

        float ITrailResult.FastestSpeed
        {
            get { return FastestSpeed; }
        }

        INumericTimeDataSeries ITrailResult.GradeTrack
        {
            get { return GradeTrack; }
        }

        INumericTimeDataSeries ITrailResult.HeartRatePerMinuteTrack
        {
            get { return HeartRatePerMinuteTrack; }
        }

        float ITrailResult.MaxHR
        {
            get { return MaxHR; }
        }

        int ITrailResult.Order
        {
            get { return Order; }
        }

        INumericTimeDataSeries ITrailResult.PaceTrack
        {
            get { return PaceTrack; }
        }

        INumericTimeDataSeries ITrailResult.PowerWattsTrack
        {
            get { return PowerWattsTrack; }
        }

        INumericTimeDataSeries ITrailResult.SpeedTrack
        {
            get { return SpeedTrack; }
        }

        TimeSpan ITrailResult.StartTime
        {
            get { return StartTime; }
        }

        #endregion
    }
}
