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
using GpsRunningPlugin.Util;

namespace TrailsPlugin.Data {
    public class TrailResult : ITrailResult, IComparable
    {
        private IList<Data.TrailGPSLocation> m_trailgps;
		private IActivity m_activity;
		private int m_order;

        private int m_startIndex = -1;
		private int m_endIndex = -1;
        private DateTime? m_startTime = null;
        private DateTime? m_endTime = null;
        private float m_startDistance = float.NaN;
        //private float m_lastDistance = float.NaN;
        private float m_distDiff; //to give quality of results
        private IList<int> m_indexes = new List<int>();
        private Color m_trailColor = getColor(nextTrailColor++);
        private TrailResult m_parentResult = null;
        private string m_toolTip = null;

        private IDistanceDataTrack m_distanceMetersTrack = null;
        private IDistanceDataTrack m_activityDistanceMetersTrack = null;
        private INumericTimeDataSeries m_elevationMetersTrack;
        private INumericTimeDataSeries m_cadencePerMinuteTrack;
        private INumericTimeDataSeries m_heartRatePerMinuteTrack;
        private INumericTimeDataSeries m_powerWattsTrack;
        private INumericTimeDataSeries m_speedTrack;
        private INumericTimeDataSeries m_paceTrack;
        private INumericTimeDataSeries m_gradeTrack;

        //Converted tracks, to display format
        private TrailResult m_cacheTrackRef = null;
        private IDistanceDataTrack m_distanceMetersTrack0 = null;
        private INumericTimeDataSeries m_elevationMetersTrack0;
        private INumericTimeDataSeries m_speedTrack0;
        private INumericTimeDataSeries m_paceTrack0;
        private INumericTimeDataSeries m_DiffTimeTrack0 = null;
        private INumericTimeDataSeries m_DiffDistTrack0 = null;

        private IGPSRoute m_gpsTrack;
        private IList<IGPSPoint> m_gpsPoints;
        
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
            foreach (int i in indexes)
            {
                m_indexes.Add(i);
                if (i >= 0)
                {
                    if (m_startIndex < 0)
                    {
                        m_startIndex = i;
                    }
                    m_endIndex = i;
                }
            }
            m_distDiff = distDiff;

            if (par == null)
            {
                if (!s_activities.ContainsKey(m_activity))
                {
                    s_activities.Add(m_activity, new trActivityInfo());
                    //aActivities[m_activity].activityColor = getColor(nextActivityColor++);
                }
                s_activities[m_activity].res.Add(this);
            }
        }

        public IList<TrailResult> getSplits()
        {
            IList<TrailResult> splits = new List<TrailResult>();
            if (m_trailgps.Count > 1)
            {
                for (int i = 1; i < m_indexes.Count; i++)
                {
                    int startIndex = m_indexes[i - 1];
                    int i2 = nextValidIndex(m_indexes, i);
                    int endIndex = -1;
                    if (i2 >= 0)
                    {
                        endIndex = m_indexes[i2];
                    }
                    if (m_trailgps.Count > i && startIndex >= 0 && endIndex >= 0 && m_trailgps.Count > i2)
                    {
                        Data.TrailGPSLocation tg1 = m_trailgps[i - 1];
                        Data.TrailGPSLocation tg2 =  m_trailgps[i2];
                        TrailResult tr = new TrailResult(this, new List<Data.TrailGPSLocation> { tg1, tg2 },
                            m_activity, i,
                            new List<int> { startIndex, endIndex },
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

        //Get the valid index (could be the first)
        private static int nextValidIndex(IList<int> aMatch, int start)
        {
            int res = -1;
            for (int i = start; i < aMatch.Count; i++)
            {
                if (aMatch[i] >= 0)
                {
                    res = i;
                    break;
                }
            }
            return res;
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
                if (GPSRoute == null || GPSRoute.Count == 0)
                {
                    return TimeSpan.FromSeconds(0);
                }
                return getElapsedWithoutPauses(this.GPSRoute, this.GPSRoute[this.GPSRoute.Count-1]);
            }
        }
        public double Distance
        {
            get
            {
                if (DistanceMetersTrack != null && DistanceMetersTrack.Count > 0)
                {
                    //TODO: calculate unpaused distance
                    return DistanceMetersTrack[DistanceMetersTrack.Count - 1].Value;
                }
                else
                {
                    return 0;
                }
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
                if (m_startTime == null)
                {
                    DateTime startTime = m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[m_startIndex]);
                    DateTime endTime   = m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[m_endIndex]);
                    //m_startTime = startTime;
                    //while (ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused((DateTime)m_startTime, Pauses))
                    //{
                    //    m_startTime = ((DateTime)m_startTime).Add(TimeSpan.FromSeconds(1));
                    //    if (endTime.CompareTo((DateTime)m_startTime) <= 0)
                    //    {
                    //        //Trail (or subtrail) is completely paused. Use all
                    //        m_startTime = startTime;
                    //        break;
                    //    }
                    //}
                    m_startTime = getUnpausedTime(startTime, Pauses, true);
                    if (endTime.CompareTo((DateTime)m_startTime) <= 0)
                    {
                        //Trail (or subtrail) is completely paused. Use all
                        m_startTime = startTime;
                    }
                }
                return (DateTime)m_startTime;
            }
        }
        public DateTime EndDateTime
        {
            get
            {
                if (m_endTime == null)
                {
                    DateTime startTime = m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[m_startIndex]);
                    DateTime endTime   = m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[m_endIndex]);
                    //m_endTime = endTime;
                    //while (ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused((DateTime)m_endTime, Pauses))
                    //{
                    //     m_endTime = ((DateTime)m_endTime).Add(TimeSpan.FromSeconds(-1));
                    //     if (startTime.CompareTo((DateTime)m_endTime) >= 0)
                    //     {
                    //         //Trail (or subtrail) is completely paused. Use all
                    //         m_endTime = endTime;
                    //         break;
                    //     }
                    //}
                    m_endTime = getUnpausedTime(endTime, Pauses, false);
                    if (startTime.CompareTo((DateTime)m_endTime) >= 0)
                    {
                        //Trail (or subtrail) is completely paused. Use all
                        m_endTime = endTime;
                    }
                }
                return (DateTime)m_endTime;
            }
        }
        private static DateTime getUnpausedTime(DateTime time, IValueRangeSeries<DateTime> pauses, bool next)
        {
            if (ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(time, pauses))
            {
                foreach (IValueRange<DateTime> pause in pauses)
                {
                    if (time.CompareTo(pause.Lower) > 0 &&
                        time.CompareTo(pause.Upper) < 0)
                    {
                        if (next)
                        {
                            time = (pause.Upper).Add(TimeSpan.FromSeconds(1));
                        }
                        else
                        {
                            time = pause.Lower.Add(TimeSpan.FromSeconds(-1));
                        }
                    }
                }
            }
            return time;
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
            //return t - StartDist;
            return DistanceMetersTrack.GetInterpolatedValue(getDateTimeFromDistActivity(t)).Value;
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
        public DateTime getDateTimeFromElapsedResult(INumericTimeDataSeries series, ITimeValueEntry<float> t)
        {
            return series.EntryDateTime(t);
            //return StartDateTime.AddSeconds(t.ElapsedSeconds);
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

        //private DateTime getTimeWithPauses(ITimeValueEntry<float> elapsed)
        //{
        //    //Note: Uses the Activity.StartTime, should be related to the track time
        //    return ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.AddTimeAndPauses(
        //        this.Activity.StartTime, TimeSpan.FromSeconds(elapsed.ElapsedSeconds), Pauses);
        //}

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
            includeStopped = TrailsPlugin.Plugin.GetApplication().SystemPreferences.AnalysisSettings.IncludeStopped;
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
                    if (m_parentResult != null)
                    {
                        return m_parentResult.Pauses;
                    }
                    m_pauses = new ValueRangeSeries<DateTime>();
                    IValueRangeSeries<DateTime> actPause;
                    if (includeStopped())
                    {
                        actPause = Info.Activity.TimerPauses;
                    }
                    else
                    {
                        actPause = Info.NonMovingTimes;
                    }
                    foreach (ValueRange<DateTime> t in actPause)
                    {
                        m_pauses.Add(t);
                    }

                    if (Settings.RestIsPause)
                    {
                        for (int i = 0; i<Info.Activity.Laps.Count; i++)
                        {
                            ILapInfo lap = Info.Activity.Laps[i];
                            if (lap.Rest)
                            {
                                DateTime endTime;
                                if (i < Info.Activity.Laps.Count - 1)
                                {
                                    endTime = Info.Activity.Laps[i + 1].StartTime;
                                    if (!Info.Activity.Laps[i + 1].Rest)
                                    {
                                        endTime -= TimeSpan.FromSeconds(1);
                                    }
                                }
                                else
                                {
                                    endTime = Info.EndTime;
                                }
                                m_pauses.Add(new ValueRange<DateTime>(lap.StartTime, endTime));
                            }
                        }
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
                //TrailsPlugin.Plugin.GetApplication().SystemPreferences.AnalysisSettings.HeartRateSmoothingSeconds = 120;
                return ActivityInfoCache.Instance.GetInfo(this.Activity);
                //ActivityInfo m_ActivityInfo = null;
                //if (m_ActivityInfo == null)
                //{
                //    m_ActivityInfo = ActivityInfoCache.Instance.GetInfo(this.Activity);
                //    //m_ActivityInfo.Activity.syst
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
                        if (i < m_activityDistanceMetersTrack.Count)
                        {
                            m_startDistance = m_activityDistanceMetersTrack[i].Value;
                        }
                        float? prevDist=null;
                        float distance=0;
                        while (i < m_activityDistanceMetersTrack.Count &&
                            0 <= this.EndDateTime.CompareTo(m_activityDistanceMetersTrack.EntryDateTime(m_activityDistanceMetersTrack[i])))
                        {
                            ITimeValueEntry<float> timeValue = m_activityDistanceMetersTrack[i];
                            DateTime time = m_activityDistanceMetersTrack.EntryDateTime(timeValue);
                            float actDist = timeValue.Value;
                            //float val = (float)getDistResultFromDistActivity(timeValue.Value);
                            if (!ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(time, Pauses))
                            {
                                if (prevDist != null)
                                {
                                    distance += actDist - (float)prevDist;
                                }
                                m_distanceMetersTrack.Add(time, distance);
                                prevDist = actDist;
                            }
                            else
                            {
                                prevDist = null;
                            }
                            i++;
                        }
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
                    if (i>=0 && i < m_activity.GPSRoute.Count)
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
                return (float)UnitUtil.Speed.ConvertFrom(this.Distance / this.Duration.TotalSeconds, m_activity);
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
                return (float)UnitUtil.Pace.ConvertFrom(this.Distance / this.Duration.TotalSeconds, m_activity); 
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

        //Reset values used in calculations
        public void Clear()
        {
            m_distanceMetersTrack0 = null;
            m_elevationMetersTrack0 = null;
            m_speedTrack0 = null;
            m_paceTrack0 = null;
            m_DiffTimeTrack0 = null;
            m_DiffDistTrack0 = null;

            m_pauses = null;
            m_startTime = null;
            m_endTime = null;
            
            m_distanceMetersTrack = null;
            m_activityDistanceMetersTrack = null;
            m_elevationMetersTrack = null;
            m_cadencePerMinuteTrack = null;
            m_heartRatePerMinuteTrack = null;
            m_powerWattsTrack = null;
            m_speedTrack = null;
            m_paceTrack = null;
            m_gradeTrack = null;

            m_gpsPoints = null;
            m_gpsTrack = null;
        }

        public INumericTimeDataSeries copyTrailTrack(INumericTimeDataSeries source)
        {
            INumericTimeDataSeries track = new NumericTimeDataSeries();
            track.AllowMultipleAtSameTime = true;
            if (source != null)
            {
                for (int i = 0; i < this.DistanceMetersTrack.Count; i++)
                {
                    DateTime time = getDateTimeFromElapsedResult(this.DistanceMetersTrack, this.DistanceMetersTrack[i]);
                    ITimeValueEntry<float> value = source.GetInterpolatedValue(time);
                    if (value != null &&
                        !ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(time, Pauses))
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
        //Unused, no cache
        public INumericTimeDataSeries HeartRatePerMinutePercentMaxTrack
        {
            get
            {
                IAthleteInfoEntry lastAthleteEntry = Plugin.GetApplication().Logbook.Athlete.InfoEntries.LastEntryAsOfDate(m_activity.StartTime);
                INumericTimeDataSeries track = new NumericTimeDataSeries();
                // Value is in BPM so convert to the % max HR if we have the info
                if (!float.IsNaN(lastAthleteEntry.MaximumHeartRatePerMinute))
                {
                    INumericTimeDataSeries tempResult = this.HeartRatePerMinuteTrack;
                    foreach (ITimeValueEntry<float> entry in tempResult)
                    {
                        float hr = (entry.Value / lastAthleteEntry.MaximumHeartRatePerMinute) * 100;
                        track.Add(tempResult.EntryDateTime(entry), hr);
                    }
                }
                return track;
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
		public INumericTimeDataSeries SpeedTrack
        {
			get {
				if (m_speedTrack == null) {
					m_speedTrack = new NumericTimeDataSeries();
                    m_speedTrack.AllowMultipleAtSameTime = true;
					for (int i = 0; i < this.DistanceMetersTrack.Count; i++)
                    {
                        DateTime time = getDateTimeFromElapsedResult(this.DistanceMetersTrack, this.DistanceMetersTrack[i]);
						ITimeValueEntry<float> value = Info.SmoothedSpeedTrack.GetInterpolatedValue(time);
						if (value != null)
                        {
                            float speed = value.Value;
                            m_speedTrack.Add(time, speed);
						}
					}
				}
				return m_speedTrack;
			}
		}
        
        //This section has caused a exception once
        public INumericTimeDataSeries PaceTrack
        {
			get {
				if (m_paceTrack == null) {
                    //PaceTrack could share a common base track (in m/s) with SpeedTrack,
                    //and be converted to pace/speed units when referenced
                    m_paceTrack = new NumericTimeDataSeries();
                    m_paceTrack.AllowMultipleAtSameTime = true;
                    for (int i = 0; i < this.DistanceMetersTrack.Count; i++)
                    {
                        DateTime time = getDateTimeFromElapsedResult(this.DistanceMetersTrack, this.DistanceMetersTrack[i]);
                        ITimeValueEntry<float> value = Info.SmoothedSpeedTrack.GetInterpolatedValue(time);
                        if (value != null)
                        {
                            float pace = value.Value;// (float)UnitUtil.Pace.ConvertFrom(value.Value, m_activity);
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

        private bool checkCacheRef(TrailResult refRes)
        {
            if (refRes == null || refRes != m_cacheTrackRef)
            {
                m_cacheTrackRef = refRes;
                //Clear();
                m_distanceMetersTrack0 = null;
                m_elevationMetersTrack0 = null;
                m_speedTrack0 = null;
                m_paceTrack0 = null;
                m_DiffTimeTrack0 = null;
                m_DiffDistTrack0 = null;

                return true;
            }
            return false;
        }

        public IDistanceDataTrack DistanceMetersTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_distanceMetersTrack0 == null)
            {
                m_distanceMetersTrack0 = new DistanceDataTrack();
                foreach (ITimeValueEntry<float> entry in this.DistanceMetersTrack)
                {
                    float val = (float)UnitUtil.Distance.ConvertFrom(entry.Value, refRes.Activity);
                    if (val != float.NaN)
                    {
                        m_distanceMetersTrack0.Add(m_distanceMetersTrack0.EntryDateTime(entry), val);
                    }
                }
            }
            return m_distanceMetersTrack0;
        }

        public INumericTimeDataSeries ElevationMetersTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_elevationMetersTrack0 == null)
            {
                m_elevationMetersTrack0 = new NumericTimeDataSeries();
                foreach (ITimeValueEntry<float> entry in this.ElevationMetersTrack)
                {
                    float val = (float)UnitUtil.Elevation.ConvertFrom(entry.Value, refRes.Activity);
                    if (val != float.NaN)
                    {
                        m_elevationMetersTrack0.Add(m_elevationMetersTrack0.EntryDateTime(entry), val);
                    }
                }
            }
            return m_elevationMetersTrack0;
        }

        public INumericTimeDataSeries SpeedTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_speedTrack0 == null)
            {
                m_speedTrack0 = new NumericTimeDataSeries();
                foreach (ITimeValueEntry<float> entry in this.SpeedTrack)
                {
                    float val = (float)UnitUtil.Speed.ConvertFrom(entry.Value, refRes.Activity);
                    if (val != float.NaN)
                    {
                        m_speedTrack0.Add(m_speedTrack0.EntryDateTime(entry), val);
                    }
                }
            }
            return m_speedTrack0;
        }

        public INumericTimeDataSeries PaceTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_paceTrack0 == null)
            {
                m_paceTrack0 = new NumericTimeDataSeries();
                foreach (ITimeValueEntry<float> entry in this.PaceTrack)
                {
                    float val = (float)UnitUtil.Pace.ConvertFrom(entry.Value, refRes.Activity);
                    if (val != float.NaN)
                    {
                        m_paceTrack0.Add(m_paceTrack0.EntryDateTime(entry), val);
                    }
                }
            }
            return m_paceTrack0;
        }

        public INumericTimeDataSeries DiffTimeTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_DiffTimeTrack0 == null)
            {
                m_DiffTimeTrack0 = new NumericTimeDataSeries();
                float oldElapsedSeconds = -1;
                float lastValue = 0;
                foreach (ITimeValueEntry<float> t in DistanceMetersTrack)
                {
                    try
                    {
                        if (refRes != null && t.ElapsedSeconds <= refRes.DistanceMetersTrack.TotalElapsedSeconds &&
                            t.ElapsedSeconds > oldElapsedSeconds)
                        {
                            DateTime d1 = this.getDateTimeFromElapsedResult(this.DistanceMetersTrack, t);
                            DateTime d2 = refRes.DistanceMetersTrack.GetTimeAtDistanceMeters(t.Value);
                            lastValue = (float)(-t.ElapsedSeconds + d2.Subtract(refRes.DistanceMetersTrack.StartTime).TotalSeconds);
                            if (!ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(d1, Pauses))
                            {
                                m_DiffTimeTrack0.Add(d1, lastValue);
                            }
                            oldElapsedSeconds = t.ElapsedSeconds;
                        }
                    }
                    catch { }
                }

                //Add a point last in the track, to show the complete dist in the chart
                //Alternatively use speed to extrapolate difference
                if (DistanceMetersTrack.Count > 0)
                {
                    ITimeValueEntry<float> t = DistanceMetersTrack[DistanceMetersTrack.Count - 1];
                    if (oldElapsedSeconds < t.ElapsedSeconds)
                    {
                        try
                        {
                            DateTime d1 = this.getDateTimeFromElapsedResult(this.DistanceMetersTrack, t);
                            if (!ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(d1, Pauses))
                            {
                                m_DiffTimeTrack0.Add(d1, lastValue);
                            }
                        }
                        catch { }
                    }
                }
            }
            return m_DiffTimeTrack0;
        }

        public INumericTimeDataSeries DiffDistTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_DiffDistTrack0 == null)
            {
                m_DiffDistTrack0 = new NumericTimeDataSeries();
                float oldElapsedSeconds = -1;
                float lastValue = 0;
                foreach (ITimeValueEntry<float> t in DistanceMetersTrack)
                {
                    try
                    {
                        if (refRes != null && t.ElapsedSeconds <= refRes.DistanceMetersTrack.TotalElapsedSeconds &&
                            t.ElapsedSeconds > oldElapsedSeconds)
                        {
                            DateTime d1 = this.getDateTimeFromElapsedResult(this.DistanceMetersTrack, t);
                            DateTime d2 = refRes.getDateTimeFromElapsedResult(refRes.DistanceMetersTrack, t);
                            lastValue = (float)UnitUtil.Distance.ConvertFrom(t.Value - refRes.DistanceMetersTrack.GetInterpolatedValue(d2).Value, refRes.Activity);
                            if (!ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(d1, Pauses))
                            {
                                m_DiffDistTrack0.Add(d1, lastValue);
                            }
                            oldElapsedSeconds = t.ElapsedSeconds;
                        }
                    }
                    catch { }
                }
                //Add a point last in the track, to show the complete dist in the chart
                //Alternatively use speed to extrapolate difference
                if (DistanceMetersTrack.Count > 0)
                {
                    ITimeValueEntry<float> t = DistanceMetersTrack[DistanceMetersTrack.Count - 1];
                    if (oldElapsedSeconds < t.ElapsedSeconds)
                    {
                        try
                        {
                            DateTime d1 = this.getDateTimeFromElapsedResult(this.DistanceMetersTrack, t);
                            if (!ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(d1, Pauses))
                            {
                                m_DiffDistTrack0.Add(d1, lastValue);
                            }
                        }
                        catch { }
                    }
                }
            }
            return m_DiffDistTrack0;
        }

        private void getGps()
        {
            m_gpsTrack = new GPSRoute();
            m_gpsPoints = new List<IGPSPoint>();
            if (Activity.GPSRoute != null)
            {
                for (int i = m_startIndex; i <= m_endIndex; i++)
                {
                    DateTime time = m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[i]);
                    IGPSPoint point = m_activity.GPSRoute[i].Value;
                    if (!ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(time, Pauses))
                    {
                        m_gpsPoints.Add(point);
                        m_gpsTrack.Add(time, point);
                    }
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
                if (!m_colorOverridden && s_activities.Count > 1 && this.ParentResult != null)
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
        private static IDictionary<IActivity, trActivityInfo> s_activities = new Dictionary<IActivity, trActivityInfo>();
        public static IList<TrailResult> TrailResultList(IActivity activity)
        {
            trActivityInfo t = new trActivityInfo();
            s_activities.TryGetValue(activity, out t);
            return t.res;
        }
        public static void Reset()
        {
            nextTrailColor = 1;
            //nextActivityColor = 1;
            s_activities.Clear();
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
            get { return UnitUtil.Distance.ToString(Distance, ""); }
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
            get { return (ElevChg > 0 ? "+" : "") + UnitUtil.Elevation.ToString(ElevChg, ""); }
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
            //Note: should be exporting raw speedtrack
            //get { return PaceTrack; }
            get { return PaceTrack0(this); }
        }

        INumericTimeDataSeries ITrailResult.PowerWattsTrack
        {
            get { return PowerWattsTrack; }
        }

        INumericTimeDataSeries ITrailResult.SpeedTrack
        {
            //Note: should be exporting raw speedtrack
            //get { return SpeedTrack; }
            get { return SpeedTrack0(this); }
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
