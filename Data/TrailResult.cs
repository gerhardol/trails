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
using System.Drawing;
using System.Collections.Generic;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ITrailExport;

namespace TrailsPlugin.Data {
    public class TrailResult : ITrailResult, IComparable
    {
        private IList<Data.TrailGPSLocation> m_trailgps;
		private IActivity m_activity;
		private int m_order;
		private INumericTimeDataSeries m_cadencePerMinuteTrack;
		private INumericTimeDataSeries m_heartRatePerMinuteTrack;
        private IDistanceDataTrack m_distanceMetersTrack = null;
        private IDistanceDataTrack m_activityDistanceMetersTrack = null;
        //private IDistanceDataTrack m_activityUnpausedDistanceMetersTrack = null;
        private INumericTimeDataSeries m_elevationMetersTrack;
		private INumericTimeDataSeries m_powerWattsTrack;
        private INumericTimeDataSeries m_speedTrack;
        private INumericTimeDataSeries m_gradeTrack;
        private IGPSRoute m_gpsTrack;
        private IList<IGPSPoint> m_gpsPoints;
        private INumericTimeDataSeries m_paceTrack;
        private int m_startIndex;
		private int m_endIndex;
        private DateTime m_startTime;
        private float m_startDistance = float.NaN;
        //private float m_lastDistance = float.NaN;
        private float m_distDiff; //to give quality of results
        private IList<int> m_indexes = new List<int>();
        private Color m_trailColor = getColor(nextTrailColor++);
        private TrailResult m_parentResult = null;
        private string m_toolTip = null;

        public TrailResult(Trail trail, IActivity activity, int order, IList<int> indexes, float distDiff)
            : this(trail.TrailLocations, activity, order, indexes, distDiff)
        {}
        public TrailResult(IList<Data.TrailGPSLocation> trailgps, IActivity activity, int order, IList<int> indexes, float distDiff)
            : this(null, trailgps, activity, order, indexes, distDiff)
        { }
        public TrailResult(IActivity activity, int order, IList<int> indexes, float distDiff, string toolTip)
            : this(null, new List<Data.TrailGPSLocation>(), activity, order, indexes, distDiff)
        { 
            m_toolTip = toolTip;
        }

        private TrailResult(TrailResult par, IList<Data.TrailGPSLocation> trailgps, IActivity activity, int order, IList<int> indexes, float distDiff)
        {
            createTrailResult(par, trailgps, activity, order, indexes, distDiff);
        }

        //Create from splits
        public TrailResult(Trail trail, IActivity activity, int order)
        {
            IList<int> indexes;
            m_trailgps = Data.Trail.TrailGpsPointsFromSplits(activity, out indexes);
            createTrailResult(null, m_trailgps, activity, order, indexes, float.MaxValue);
        }

        private void createTrailResult(TrailResult par, IList<Data.TrailGPSLocation> trailgps, IActivity activity, int order, IList<int> indexes, float distDiff)
        {
            m_parentResult = par;
            m_trailgps = trailgps;
            m_activity = activity;
            m_order = order;
            m_startIndex = indexes[0];
            m_endIndex = indexes[indexes.Count - 1];
            foreach (int i in indexes)
            {
                m_indexes.Add(i);
            }
            m_distDiff = distDiff;

            m_startTime = m_activity.StartTime.AddSeconds(m_activity.GPSRoute[m_startIndex].ElapsedSeconds);
            if (par == null)
            {
                if (!aActivities.ContainsKey(m_activity))
                {
                    aActivities.Add(m_activity, new trActivityInfo());
                    //aActivities[m_activity].activityColor = getColor(nextActivityColor++);
                }
                aActivities[m_activity].res.Add(this);
            }
        }

        public IList<TrailResult> getSplits()
        {
            IList<TrailResult> splits = new List<TrailResult>();
            if (m_trailgps.Count > 1)
            {
                for (int i = 1; i < m_indexes.Count; i++)
                {
                    if (m_trailgps.Count > i)
                    {
                        Data.TrailGPSLocation tg1 = m_trailgps[i - 1];
                        Data.TrailGPSLocation tg2 = tg1;
                        if (m_trailgps.Count > i)
                        {
                            tg2 = m_trailgps[i];
                        }
                        TrailResult tr = new TrailResult(this, new List<Data.TrailGPSLocation> { tg1, tg2 },
                            m_activity, i,
                            new List<int> { m_indexes[i - 1], m_indexes[i] },
                            m_distDiff);
                        tr.m_parentResult = this;
                        //if (aActivities.Count > 1)
                        //{
                        //    nextTrailColor--;
                        //    tr.m_trailColor = this.m_trailColor;
                        //}
                        splits.Add(tr);
                    }
                }
            }
            return splits;
        }

        /**********************************************************/
        public IActivity Activity
        {
            get { return m_activity; }
        }
        //Distance error used to sort results
        public float DistDiff
        {
            get { return (float)(m_distDiff/Math.Pow(m_indexes.Count, 1.5)); }
        }
        public int Order
        {
			get {
				return m_order;
			}
		}
        public string ToolTip
        {
            get
            {
                if (m_toolTip != null)
                {
                    return m_toolTip;
                }
                else
                {
                    return Activity.StartTime + " " + Activity.Name;
                }
            }
        }

        /****************************************************************/
        public TimeSpan Duration
        {
            get
            {
                return getElapsedWithoutPauses(this.GPSRoute, this.GPSRoute[this.GPSRoute.Count-1]);
            }
        }
        public double Distance
        {
            get
            {
                return DistanceMetersTrack[DistanceMetersTrack.Count - 1].Value;
            }
        }

        public TimeSpan StartTime
        {
            get
            {
                return StartDateTime.ToLocalTime().TimeOfDay;
            }
        }
        public TimeSpan EndTime
        {
            get
            {
                return EndDateTime.ToLocalTime().TimeOfDay;
            }
        }
        public DateTime StartDateTime
        {
            get
            {
                return m_startTime;
            }
        }
        public DateTime EndDateTime
        {
            get
            {
                return m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[m_endIndex]);
            }
        }
        //All of result including pauses/stopped
        //This is how FilteredStatistics want the info
        public IValueRangeSeries<DateTime> getSelInfo()
        {
            IValueRangeSeries<DateTime> t = new ValueRangeSeries<DateTime>();
            t.Add(new ValueRange<DateTime>(this.StartDateTime, this.EndDateTime));
            return t;
        }
        public double StartDist
        {
            get
            {
                if (float.IsNaN(m_startDistance))
                {
                    getDistanceTrack();
                }
                return m_startDistance;
            }
        }
        //public double LastDist
        //{
        //    get
        //    {
        //        if (float.IsNaN(m_lastDistance))
        //        {
        //            getDistanceTrack();
        //        }
        //        return m_lastDistance;
        //    }
        //}

        //IItemTrackSelectionInfo Selection
        //{
        //    get
        //    {
        //        TrailsItemTrackSelectionInfo result = new TrailsItemTrackSelectionInfo();
        //        result.MarkedTimes = new ValueRangeSeries<DateTime>();
        //        ValueRange<DateTime> range = new ValueRange<DateTime>(this.StartDateTime, this.EndDateTime);
        //        result.MarkedTimes = new ValueRangeSeries<DateTime>();
        //        result.MarkedTimes.Add(range);

        //        return result;
        //    }
        //}


        /*************************************************/
        //DateTime vs elapsed result/activity, distance result/activity conversions

        //Get result time and distance from activity references
        public double getDistResult(DateTime t)
        {
            //Ignore malformed activities and selection outside the result
            double res = 0;
            try
            {
                res = DistanceMetersTrack.GetInterpolatedValue(t).Value;
            }
            catch { }
            return res;
        }
        public double getDistResultFromDistActivity(double t)
        {
            return t - StartDist;
        }
        //public double getDistResultFromUnpausedDistActivity(double t)
        //{
        //    return getDistResult(getDateTimeFromUnpausedDistActivity(t));
        //}
        public DateTime getDateTimeFromDistActivity(double t)
        {
            return ActivityDistanceMetersTrack.GetTimeAtDistanceMeters(t);
        }
        //public DateTime getDateTimeFromUnpausedDistActivity(double t)
        //{
        //    return ActivityUnpausedDistanceMetersTrack.GetTimeAtDistanceMeters(t);
        //}
        public static DateTime getDateTimeFromElapsedActivityStatic(IActivity act, ITimeValueEntry<IGPSPoint> p)
        {
            //Added here, if there are tricks for pauses required
            return act.GPSRoute.EntryDateTime(p);
        }

        //Elapsed vs DateTime for result
        public double getElapsedResult(DateTime d)
        {
            return d.Subtract(StartDateTime).TotalSeconds;
        }

        //Result to activity
        public DateTime getDateTimeFromElapsedResult(ITimeValueEntry<float> t)
        {
            return StartDateTime.AddSeconds(t.ElapsedSeconds);
        }
        public DateTime getDateTimeFromElapsedResult(float t)
        {
            return StartDateTime.AddSeconds(t);
        }
        public DateTime getDateTimeFromDistResult(double t)
        {
            DateTime res = DateTime.MinValue;
            try
            {
                res = DistanceMetersTrack.GetTimeAtDistanceMeters(t);
            }
            catch { }
            return res;
        }
        public double getDistActivityFromDistResult(double t)
        {
            return StartDist + t;
        }

        /***************************************************/
        private TimeSpan getElapsedWithoutPauses(DateTime entryTime)
        {
            TimeSpan elapsed = ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.TimeNotPaused(
                this.StartDateTime, entryTime, Pauses);
            return elapsed;
        }
        private TimeSpan getElapsedWithoutPauses(IGPSRoute dataSeries, ITimeValueEntry<IGPSPoint> time)
        {
            DateTime entryTime = dataSeries.EntryDateTime(time);
            return getElapsedWithoutPauses(entryTime);
        }
        //private TimeSpan getElapsedWithoutPauses(INumericTimeDataSeries dataSeries, ITimeValueEntry<float> time)
        //{
        //    DateTime entryTime = dataSeries.EntryDateTime(time);
        //    return getElapsedWithoutPauses(entryTime);
        //}
        private TimeSpan getElapsedWithoutPauses(IDistanceDataTrack dataSeries, ITimeValueEntry<float> time)
        {
            DateTime entryTime = dataSeries.EntryDateTime(time);
            return getElapsedWithoutPauses(entryTime);
        }

        private DateTime getTimeWithPauses(ITimeValueEntry<float> elapsed)
        {
            return ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.AddTimeAndPauses(
                this.Activity.StartTime, TimeSpan.FromSeconds(elapsed.ElapsedSeconds), Pauses);
        }

        /*********************************************/
        private bool includeStopped()
        {
            bool includeStopped = true;
#if ST_2_1
            // If UseEnteredData is set, exclude Stopped
            if (info.Activity.UseEnteredData == false && info.Time.Equals(info.ActualTrackTime))
            {
                includeStopped = true;
            }
#else
            includeStopped = TrailsPlugin.PluginMain.GetApplication().SystemPreferences.AnalysisSettings.IncludeStopped;
#endif
            return includeStopped;
        }

        IValueRangeSeries<DateTime> m_pauses = null;
        IValueRangeSeries<DateTime> Pauses
        {
            get
            {
                if (m_pauses == null)
                {
                    if (includeStopped())
                    {
                        m_pauses = Info.Activity.TimerPauses;
                    }
                    else
                    {
                        m_pauses = Info.NonMovingTimes;
                    }
                }
                return m_pauses;
            }
        }
        //ActivityInfo m_ActivityInfo = null;
        ActivityInfo Info
        {
            get
            {
                //Caching is not needed, done by ST
                return ActivityInfoCache.Instance.GetInfo(this.Activity);
                //if (m_ActivityInfo == null)
                //{
                //    m_ActivityInfo = ActivityInfoCache.Instance.GetInfo(this.Activity);
                //}
                //return m_ActivityInfo;
            }
        }



        /*********************************************/
        //Distance
        private void getDistanceTrack()
        {
            if (null == m_distanceMetersTrack)
            {
                m_distanceMetersTrack = new DistanceDataTrack();
                m_distanceMetersTrack.AllowMultipleAtSameTime = true;
                if (Activity.GPSRoute != null)
                {
                    //m_activityDistanceMetersTrack = m_activity.GPSRoute.GetDistanceMetersTrack();
                    //m_activityUnpausedDistanceMetersTrack = Info.ActualDistanceMetersTrack;
                    if (includeStopped())
                    {
                        m_activityDistanceMetersTrack = Info.ActualDistanceMetersTrack;
                    }
                    else
                    {
                        m_activityDistanceMetersTrack = Info.MovingDistanceMetersTrack;
                    }
                    if (m_activityDistanceMetersTrack != null)
                    {
                        int i = 0;
                        while (i < m_activityDistanceMetersTrack.Count &&
                            0 < this.StartDateTime.CompareTo(m_activityDistanceMetersTrack.EntryDateTime(m_activityDistanceMetersTrack[i])))
                        {
                            i++;
                        }
                        m_startDistance = m_activityDistanceMetersTrack[i].Value;
                        while (i < m_activityDistanceMetersTrack.Count &&
                            0 <= this.EndDateTime.CompareTo(m_activityDistanceMetersTrack.EntryDateTime(m_activityDistanceMetersTrack[i])))
                        {
                            ITimeValueEntry<float> time = m_activityDistanceMetersTrack[i];
                            m_distanceMetersTrack.Add(
                                m_activityDistanceMetersTrack.EntryDateTime(time),
                                (float)getDistResultFromDistActivity(time.Value)
                                );
                            i++;
                        }
                        //float m_lastDistance = m_activityDistanceMetersTrack[--i].Value;
                    }
                }
            }
        }
        public IDistanceDataTrack DistanceMetersTrack
        {
            get
            {
                getDistanceTrack();
                return m_distanceMetersTrack;
            }
        }
        public IDistanceDataTrack ActivityDistanceMetersTrack
        {
            get
            {
                getDistanceTrack();
                return m_activityDistanceMetersTrack;
            }
        }
        //public IDistanceDataTrack ActivityUnpausedDistanceMetersTrack
        //{
        //    get
        //    {
        //        getDistanceTrack();
        //        return m_activityUnpausedDistanceMetersTrack;
        //    }
        //}

        /*************************************************/
        public TrailResult ParentResult
        {
            get
            {
                return m_parentResult;
            }
        }
        public String Name
        {
            get
            {
                if (m_parentResult==null)
                {
                    return m_activity.Name;
                }
                else if (m_trailgps!=null && m_trailgps.Count>0)
                {
                    return m_trailgps[0].Name;
                }
                return "";
            }
        }

        public IList<DateTime> TimeTrailPoints
        {
            get
            {
                IList<DateTime> results = new List<DateTime>();
                foreach (int i in m_indexes)
                {
                    if (i < m_activity.GPSRoute.Count)
                    {
                        results.Add(m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[i]));
                    }
                }
                return results;
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
                return Utils.Units.GetSpeed(this.Distance / this.Duration.TotalSeconds, m_activity, Speed.Units.Speed);
            }
		}
		public float FastestSpeed {
			get {
				return this.SpeedTrack.Max;
			}
		}
		public double AvgPace {
			get {
                //Note: Using PaceTrack.Avg will give bad values if a track has slow parts
                return Utils.Units.GetSpeed(this.Distance / this.Duration.TotalSeconds, m_activity, Speed.Units.Pace); 
			}
		}
		public double FastestPace {
			get {
				return this.PaceTrack.Min;
			}
		}
        public double ElevChg
        {
			get
            {
				float value = m_activity.GPSRoute[m_endIndex].Value.ElevationMeters - m_activity.GPSRoute[m_startIndex].Value.ElevationMeters;
                return value;
			}
		}

        public INumericTimeDataSeries ElevationMetersTrack
        {
			get {
				if (m_elevationMetersTrack == null) {
					m_elevationMetersTrack = this.copyTrailTrack(Info.SmoothedElevationTrack);
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
            track.AllowMultipleAtSameTime = true;
            if (source != null)
            {
                for (int i = 0; i < this.DistanceMetersTrack.Count; i++)
                {
                    DateTime time = getDateTimeFromElapsedResult(this.DistanceMetersTrack[i]);
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
					m_cadencePerMinuteTrack = this.copyTrailTrack(Info.SmoothedCadenceTrack);
				}
				return m_cadencePerMinuteTrack;
			}
		}
		public INumericTimeDataSeries HeartRatePerMinuteTrack {
			get {
				if (m_heartRatePerMinuteTrack == null) {
					m_heartRatePerMinuteTrack = this.copyTrailTrack(Info.SmoothedHeartRateTrack);
				}
				return m_heartRatePerMinuteTrack;
			}
		}
		public INumericTimeDataSeries PowerWattsTrack {
			get {
				if (m_powerWattsTrack == null) {
					m_powerWattsTrack = this.copyTrailTrack(Info.SmoothedPowerTrack);
				}
				return m_powerWattsTrack;
			}
		}
		public INumericTimeDataSeries SpeedTrack {
			get {
				if (m_speedTrack == null) {
					m_speedTrack = new NumericTimeDataSeries();
                    m_speedTrack.AllowMultipleAtSameTime = true;
					for (int i = 0; i < this.DistanceMetersTrack.Count; i++)
                    {
                        DateTime time = getDateTimeFromElapsedResult(this.DistanceMetersTrack[i]);
						ITimeValueEntry<float> value = Info.SmoothedSpeedTrack.GetInterpolatedValue(time);
						if (value != null)
                        {
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
                    m_paceTrack.AllowMultipleAtSameTime = true;
                    for (int i = 0; i < this.DistanceMetersTrack.Count; i++)
                    {
                        DateTime time = getDateTimeFromElapsedResult(this.DistanceMetersTrack[i]);
                        ITimeValueEntry<float> value = Info.SmoothedSpeedTrack.GetInterpolatedValue(time);
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
                    m_gradeTrack = this.copyTrailTrack(Info.SmoothedGradeTrack);
                }
                return m_gradeTrack;
            }
        }
        public INumericTimeDataSeries DiffTimeTrack(TrailResult refRes)
        {
            INumericTimeDataSeries result = new NumericTimeDataSeries();
            result.AllowMultipleAtSameTime = true;
            foreach (ITimeValueEntry<float> t in DistanceMetersTrack)
            {
                if (t.ElapsedSeconds <= refRes.DistanceMetersTrack.TotalElapsedSeconds)
                {
                    DateTime d1 = this.getDateTimeFromElapsedResult(t.ElapsedSeconds);
                    DateTime d2 = refRes.DistanceMetersTrack.GetTimeAtDistanceMeters(t.Value);
                    result.Add(d1, (float)(-t.ElapsedSeconds + d2.Subtract(refRes.DistanceMetersTrack.StartTime).TotalSeconds));
                }
            }
            return result;
        }
        public INumericTimeDataSeries DiffDistTrack(TrailResult refRes)
        {
            INumericTimeDataSeries result = new NumericTimeDataSeries();
            foreach (ITimeValueEntry<float> t in DistanceMetersTrack)
            {
                if (t.ElapsedSeconds <= refRes.DistanceMetersTrack.TotalElapsedSeconds)
                {
                    DateTime d1 = this.getDateTimeFromElapsedResult(t.ElapsedSeconds);
                    DateTime d2 = refRes.getDateTimeFromElapsedResult(t.ElapsedSeconds);
                    result.Add(d1, t.Value - refRes.DistanceMetersTrack.GetInterpolatedValue(d2).Value);
                }
            }
            return result;
        }

        private void getGps()
        {
            m_gpsTrack = new GPSRoute();
            m_gpsPoints = new List<IGPSPoint>();
            if (Activity.GPSRoute != null)
            {
                for (int i = m_startIndex; i <= m_endIndex; i++)
                {
                    m_gpsPoints.Add(m_activity.GPSRoute[i].Value);
                    m_gpsTrack.Add(m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[i]), m_activity.GPSRoute[i].Value);
                }
            }
        }
        public IGPSRoute GPSRoute
        {
            get
            {
                if (m_gpsTrack == null)
                {
                    getGps();
                }
                return m_gpsTrack;
            }
        }
        public IList<IGPSPoint> GpsPoints()
        {
            if (m_gpsPoints == null)
            {
                getGps();
            }
            return m_gpsPoints;
        }

        public IList<IList<IGPSPoint>> GpsPoints(Data.TrailsItemTrackSelectionInfo t)
        {
            if (t.MarkedTimes != null && t.MarkedTimes.Count > 0)
            {
                return GpsPoints(t.MarkedTimes);
            }
            else if (t.MarkedDistances != null && t.MarkedDistances.Count > 0)
            {
                return GpsPoints(t.MarkedDistances);
            }
            return new List<IList<IGPSPoint>>();
        }

        private IList<IList<IGPSPoint>> GpsPoints(IValueRangeSeries<DateTime> t)
        {
            IList<IList<IGPSPoint>> result = new List<IList<IGPSPoint>>();

            if (Activity.GPSRoute != null)
            {
                foreach (IValueRange<DateTime> r in t)
                {
                    IGPSRoute GpsTrack = Activity.GPSRoute;
                    IList<IGPSPoint> track = new List<IGPSPoint>();
                    int i = 0;
                    while (i < GpsTrack.Count &&
                        0 < r.Lower.CompareTo(GpsTrack.EntryDateTime(GpsTrack[i])))
                    {
                        i++;
                    }
                    while (i < GpsTrack.Count &&
                        0 <= r.Upper.CompareTo(GpsTrack.EntryDateTime(GpsTrack[i])))
                    {
                        track.Add(GpsTrack[i].Value);
                        i++;
                    }
                    result.Add(track);
                }
            }

            return result;
        }

        //Note: IItemTrackSelectionInfo uses Activity distance
        private IList<IList<IGPSPoint>> GpsPoints(IValueRangeSeries<double> t)
        {
            IList<IList<IGPSPoint>> result = new List<IList<IGPSPoint>>();
            if (Activity.GPSRoute != null)
            {
                IGPSRoute GpsTrack = Activity.GPSRoute;
                IDistanceDataTrack DistanceMetersTrack = ActivityDistanceMetersTrack;

                foreach (IValueRange<double> r in t)
                {
                    IList<IGPSPoint> track = new List<IGPSPoint>();
                    int i = 0;
                    while (i < GpsTrack.Count &&
                        getDistResultFromDistActivity(r.Lower) > DistanceMetersTrack[i].Value)
                    {
                        i++;
                    }
                    while (i < GpsTrack.Count &&
                        getDistResultFromDistActivity(r.Upper) >= DistanceMetersTrack[i].Value)
                    {
                        track.Add(GpsTrack[i].Value);
                        i++;
                    }
                    result.Add(track);
                }
            }
            return result;
        }

        /*************************************************/
        #region Color
        private static int nextTrailColor = 1;
        //private static int nextActivityColor = 1;

        //public Color ActivityColor
        //{
        //    get
        //    {
        //        trActivityInfo t = new trActivityInfo();
        //        aActivities.TryGetValue(this.m_activity, out t);
        //        if (t == null)
        //        {
        //            return Color.Brown;
        //        }
        //        return t.activityColor;
        //    }
        //}

        private bool m_colorOverridden = false;
        public Color TrailColor
        {
            get
            {
                if (!m_colorOverridden && aActivities.Count > 1 && this.ParentResult != null)
                {
                    return this.ParentResult.m_trailColor;
                }
                else
                {
                    return this.m_trailColor;
                }
            }
            set
            {
                m_colorOverridden = true;
                m_trailColor = value;
            }
        }

        private static Color getColor(int color)
        {
            switch (color%10)
            {
                case 0: return Color.Blue;
                case 1: return Color.Red;
                case 2: return Color.Green;
                case 3: return Color.Orange;
                case 4: return Color.Plum;
                case 5: return Color.HotPink;
                case 6: return Color.Gold;
                case 7: return Color.Silver;
                case 8: return Color.YellowGreen;
                case 9: return Color.Turquoise;
            }
            return Color.Black;
        }

        //private Color newColor()
        //{
        //    int color = nextIndex;
        //    nextIndex = (nextIndex + 1) % 10;
        //    return getColor(color);
        //}
        #endregion

        #region Activity caches
        private class trActivityInfo
        {
            public IList<TrailResult> res = new List<TrailResult>();
            public Color activityColor = getColor(0);
        }
        private static IDictionary<IActivity, trActivityInfo> aActivities = new Dictionary<IActivity, trActivityInfo>();
        public static IList<TrailResult> TrailResultList(IActivity activity)
        {
            trActivityInfo t = new trActivityInfo();
            aActivities.TryGetValue(activity, out t);
            return t.res;
        }
        public static void Reset()
        {
            nextTrailColor = 1;
            //nextActivityColor = 1;
            aActivities.Clear();
        }
        #endregion

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
            get { return Utils.Units.DistanceToString(Distance, ""); }
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
            get { return (ElevChg > 0 ? "+" : "") + Utils.Units.ElevationToString(ElevChg, ""); }
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

        #region IComparable<Product> Members

        public int CompareTo(object obj)
        {
            if (obj != null && obj is TrailResult)
            {
                TrailResult other = obj as TrailResult;
                return TrailResultColumnIds.Compare(this, other);
            }
            else
            {
                return this.ToString().CompareTo(obj.ToString());
            }
        }
        public int CompareTo(TrailResult other)
        {
            return TrailResultColumnIds.Compare(this, other);
        }
        #endregion
    }
}
