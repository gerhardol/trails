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
        private ActivityTrail m_activityTrail;
		private IActivity m_activity;
		private int m_order;
        private string m_name;

        private TrailResult m_parentResult = null;
        private TrailResultInfo m_childrenInfo;

        private DateTime? m_startTime = null;
        private DateTime? m_endTime = null;
        private float m_startDistance = float.NaN;
        private float m_distDiff; //to give quality of results
        private Color m_trailColor = getColor(nextTrailColor++);
        private string m_toolTip = null;

        private IDistanceDataTrack m_distanceMetersTrack = null;
        private IDistanceDataTrack m_activityDistanceMetersTrack = null;
        private INumericTimeDataSeries m_elevationMetersTrack;
        private INumericTimeDataSeries m_cadencePerMinuteTrack;
        private INumericTimeDataSeries m_heartRatePerMinuteTrack;
        private INumericTimeDataSeries m_powerWattsTrack;
        private INumericTimeDataSeries m_speedTrack;
        private INumericTimeDataSeries m_gradeTrack;

        //Converted tracks, to display format, with smoothing
        //Should be in a separate class
        private TrailResult m_cacheTrackRef = null;
        private IDistanceDataTrack m_distanceMetersTrack0 = null;
        private INumericTimeDataSeries m_cadencePerMinuteTrack0;
        private INumericTimeDataSeries m_elevationMetersTrack0;
        private INumericTimeDataSeries m_gradeTrack0;
        private INumericTimeDataSeries m_heartRatePerMinuteTrack0;
        private INumericTimeDataSeries m_powerWattsTrack0;
        private INumericTimeDataSeries m_speedTrack0;
        private INumericTimeDataSeries m_paceTrack0;
        private INumericTimeDataSeries m_DiffTimeTrack0 = null;
        private INumericTimeDataSeries m_DiffDistTrack0 = null;
        IList<double> m_trailPointTimeOffset0;
        IList<double> m_trailPointDistOffset0;

        private IGPSRoute m_gpsTrack;
        private IList<IGPSPoint> m_gpsPoints;
        private static ActivityInfoOptions m_TrailActivityInfoOptions;

        public static ActivityInfoOptions TrailActivityInfoOptions
        {
            get
            {
                if (m_TrailActivityInfoOptions == null)
                {
                    m_TrailActivityInfoOptions = new ActivityInfoOptions(true);
                }
                return m_TrailActivityInfoOptions;
            }
            set
            {
                m_TrailActivityInfoOptions = value;
            }
        }

        public TrailResult(ActivityTrail activityTrail, int order, TrailResultInfo indexes, float distDiff)
            : this(activityTrail, null, order, indexes, distDiff)
        { }

        public TrailResult(ActivityTrail activityTrail, int order, TrailResultInfo indexes, float distDiff, string toolTip)
            : this(activityTrail, null, order, indexes, distDiff)
        { 
            m_toolTip = toolTip;
        }

        private TrailResult(ActivityTrail activityTrail, TrailResult par, int order, TrailResultInfo indexes, float distDiff)
        {
            createTrailResult(activityTrail, par, order, indexes, distDiff);
        }

        //Create from splits
        public TrailResult(ActivityTrail activityTrail, Trail trail, IActivity activity, int order)
        {
            TrailResultInfo indexes;
            Data.Trail.TrailGpsPointsFromSplits(activity, out indexes);
            createTrailResult(activityTrail, null, order, indexes, float.MaxValue);
        }

        private void createTrailResult(ActivityTrail activityTrail, TrailResult par, int order, TrailResultInfo indexes, float distDiff)
        {
            m_activityTrail = activityTrail;
            m_parentResult = par;
            m_activity = indexes.Activity;
            m_order = order;
            m_name = indexes.Name;
            m_childrenInfo = indexes.Copy();
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
            if (this.m_childrenInfo.Count > 1)
            {
                int i; //start time index
                for (i = 0; i < m_childrenInfo.Count - 1; i++)
                {
                    if (m_childrenInfo.Points[i].Time != DateTime.MinValue)
                    {
                        int j; //end time index
                        for (j = i + 1; j < m_childrenInfo.Points.Count; j++)
                        {
                            if (m_childrenInfo.Points[j].Time != DateTime.MinValue)
                            {
                                break;
                            }
                        }
                        if (this.m_childrenInfo.Count > i && 
                            this.m_childrenInfo.Count >= j)
                        {
                            if (m_childrenInfo.Points[j].Time != DateTime.MinValue)
                            {
                                TrailResultInfo t = m_childrenInfo.CopySlice(i,j);
                                TrailResult tr = new TrailResult(m_activityTrail, this, i+1, t, m_distDiff);
                                tr.m_parentResult = this;
                                //if (aActivities.Count > 1)
                                //{
                                //    nextTrailColor--;
                                //    tr.m_trailColor = this.m_trailColor;
                                //}
                                splits.Add(tr);
                            }
                        }
                        i = j-1;//Next index to try
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
            get { return (float)(m_distDiff / Math.Pow(m_childrenInfo.Count, 1.5)); }
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
                if (DistanceMetersTrack == null || DistanceMetersTrack.Count == 0)
                {
                    return Info.Time;
                }
                return getElapsedWithoutPauses(this.DistanceMetersTrack, this.DistanceMetersTrack[this.DistanceMetersTrack.Count - 1]);
            }
        }
        public double Distance
        {
            get
            {
                if (DistanceMetersTrack != null && DistanceMetersTrack.Count > 0)
                {
                    return DistanceMetersTrack[DistanceMetersTrack.Count - 1].Value;
                }
                else
                {
                    return Info.DistanceMeters;
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
                    DateTime startTime = m_childrenInfo.Points[0].Time;
                    DateTime endTime = m_childrenInfo.Points[m_childrenInfo.Points.Count - 1].Time;
                    m_startTime = getUnpausedTime(startTime, Pauses, true);
                    if (endTime.CompareTo((DateTime)m_startTime) <= 0)
                    {
                        //Trail (or subtrail) is completely paused. Use all
                        m_startTime = startTime;
                    }
                    if (m_startTime == DateTime.MinValue)
                    {
                        m_startTime = Activity.StartTime;
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
                    DateTime startTime = m_childrenInfo.Points[0].Time;
                    DateTime endTime = m_childrenInfo.Points[m_childrenInfo.Points.Count - 1].Time;
                    m_endTime = getUnpausedTime(endTime, Pauses, false);
                    if (startTime.CompareTo((DateTime)m_endTime) >= 0)
                    {
                        //Trail (or subtrail) is completely paused. Use all
                        m_endTime = endTime;
                    }
                    if (m_endTime == DateTime.MinValue)
                    {
                        m_endTime = Info.ActualTrackEnd;
                    }
                }
                return (DateTime)m_endTime;
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
            //Cannot use "t - StartDist" as there may be pauses
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
        public static DateTime getDateTimeFromElapsedActivityStatic(IGPSRoute gps, ITimeValueEntry<IGPSPoint> p)
        {
            //Added here, if there are tricks for pauses required
            return gps.EntryDateTime(p);
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
            return ActivityDistanceMetersTrack.GetInterpolatedValue(getDateTimeFromDistResult(t)).Value;
        }

        //Chart and Result must have the same understanding of Distance
        public static double DistanceConvertTo(double t, TrailResult refRes)
        {
            return UnitUtil.Distance.ConvertTo(t, refRes.Activity);
        }
        public static double DistanceConvertFrom(double t, TrailResult refRes)
        {
            return UnitUtil.Distance.ConvertFrom(t, refRes.Activity);
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
                    //Note: Assumes that subtrails are a part of parent - could be changed
                    if (m_parentResult != null)
                    {
                        return m_parentResult.Pauses;
                    }
                    m_pauses = new ValueRangeSeries<DateTime>();
                    IValueRangeSeries<DateTime> actPause;
                    if (includeStopped())
                    {
                        actPause = Activity.TimerPauses;
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
                        for (int i = 0; i<Activity.Laps.Count; i++)
                        {
                            ILapInfo lap = Activity.Laps[i];
                            if (lap.Rest)
                            {
                                DateTime endTime;
                                if (i < Activity.Laps.Count - 1)
                                {
                                    endTime = Activity.Laps[i + 1].StartTime;
                                    if (!Activity.Laps[i + 1].Rest)
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

        ActivityInfo m_ActivityInfo = null;
        ActivityInfo Info
        {
            get
            {
                //Caching is not needed, done by ST
                //return ActivityInfoCache.Instance.GetInfo(this.Activity);
                //Custom InfoCache, to control smoothing
                if (m_ActivityInfo == null)
                {
                    ActivityInfoCache c = new ActivityInfoCache();
                    ActivityInfoOptions t = new ActivityInfoOptions(false);
                    c.Options = t;
                    m_ActivityInfo = c.GetInfo(this.Activity);
                }
                return m_ActivityInfo;
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
                    float? prevDist = null;
                    float distance = 0;
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
                return m_name;
            }
        }

        public IList<DateTime> TimeTrailPoints
        {
            get
            {
                return m_childrenInfo.CopyTime();
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
				return HeartRatePerMinuteTrack0(m_cacheTrackRef).Max;
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
                return (float)(this.Distance / this.Duration.TotalSeconds);
            }
		}
		public float FastestSpeed {
			get {
                checkCacheRef(m_cacheTrackRef);
                return (float)UnitUtil.Speed.ConvertTo(this.SpeedTrack0(m_cacheTrackRef).Max, m_cacheTrackRef.Activity);
			}
		}
        //Smoothing could differ speed/pace, why this is separate
		public double FastestPace {
            get
            {
                checkCacheRef(m_cacheTrackRef);
                return UnitUtil.Pace.ConvertTo(this.PaceTrack0(m_cacheTrackRef).Min, m_cacheTrackRef.Activity);
			}
		}
        public double ElevChg
        {
			get
            {
                float value = 0;
                if (ElevationMetersTrack != null && ElevationMetersTrack.Count > 1)
                {
                    value = ElevationMetersTrack[ElevationMetersTrack.Count - 1].Value - ElevationMetersTrack[0].Value;
                }
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
        public void Clear(bool onlyDisplay)
        {
            m_distanceMetersTrack0 = null;
            m_elevationMetersTrack0 = null;
            m_gradeTrack0 = null;
            m_heartRatePerMinuteTrack0 = null;
            m_powerWattsTrack0 = null;
            m_cadencePerMinuteTrack0 = null;
            m_speedTrack0 = null;
            m_paceTrack0 = null;
            m_DiffTimeTrack0 = null;
            m_DiffDistTrack0 = null;
            m_trailPointTimeOffset0 = null;
            m_trailPointDistOffset0 = null;

            //smoothing control
            //m_TrailActivityInfoOptions = null;

            if (!onlyDisplay)
            {
                m_pauses = null;
                //m_startTime = null;
                //m_endTime = null;

                //m_ActivityInfo = null;

                m_distanceMetersTrack = null;
                m_activityDistanceMetersTrack = null;
                m_elevationMetersTrack = null;
                m_cadencePerMinuteTrack = null;
                m_heartRatePerMinuteTrack = null;
                m_powerWattsTrack = null;
                m_speedTrack = null;
                //m_paceTrack = null;
                m_gradeTrack = null;

                m_gpsPoints = null;
                m_gpsTrack = null;
            }
        }

        //copy the relevant part to a new track
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
        //Copy the raw unsmoothed track
        public INumericTimeDataSeries copySmoothTrack(INumericTimeDataSeries source, int smooth)
        {
            INumericTimeDataSeries track = new NumericTimeDataSeries();
            if (source != null)
            {
                float oldElapsedSeconds = -1;
                foreach (ITimeValueEntry<float> t in source)
                {
                    try
                    {
                        if (t.ElapsedSeconds > oldElapsedSeconds)
                        {
                            DateTime time = source.EntryDateTime(t);
                            track.Add(time, t.Value);
                            oldElapsedSeconds = t.ElapsedSeconds;
                        }
                    }
                    catch { }
                }
                float min; float max;
                track = ZoneFiveSoftware.Common.Data.Algorithm.NumericTimeDataSeries.Smooth(track, smooth, out min, out max);
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
        //public INumericTimeDataSeries HeartRatePerMinutePercentMaxTrack
        //{
        //    get
        //    {
        //        IAthleteInfoEntry lastAthleteEntry = Plugin.GetApplication().Logbook.Athlete.InfoEntries.LastEntryAsOfDate(m_activity.StartTime);
        //        INumericTimeDataSeries track = new NumericTimeDataSeries();
        //        // Value is in BPM so convert to the % max HR if we have the info
        //        if (!float.IsNaN(lastAthleteEntry.MaximumHeartRatePerMinute))
        //        {
        //            INumericTimeDataSeries tempResult = this.HeartRatePerMinuteTrack;
        //            foreach (ITimeValueEntry<float> entry in tempResult)
        //            {
        //                float hr = (entry.Value / lastAthleteEntry.MaximumHeartRatePerMinute) * 100;
        //                track.Add(tempResult.EntryDateTime(entry), hr);
        //            }
        //        }
        //        return track;
        //    }
        //}

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
                    DateTime prevTime = DateTime.MinValue;
					for (int i = 0; i < this.DistanceMetersTrack.Count; i++)
                    {
                        DateTime time = getDateTimeFromElapsedResult(this.DistanceMetersTrack, this.DistanceMetersTrack[i]);
						ITimeValueEntry<float> value = Info.SmoothedSpeedTrack.GetInterpolatedValue(time);
                        if (i>0 && prevTime.CompareTo(time) < 0)
                        {
                            float speed = value.Value;
                            m_speedTrack.Add(time, speed);
                        }
                        prevTime = time;
					}
                }
				return m_speedTrack;
			}
		}
        
        public INumericTimeDataSeries GradeTrack
        {
            get
            {
                if (m_gradeTrack == null)
                {
                    //TODO: Implement raw grade track
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
                if (m_cacheTrackRef == null)
                {
                    //A reference is needed to set for instance display format
                    m_cacheTrackRef = this;
                }
                Clear(true);

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
                    float val = (float)UnitUtil.Distance.ConvertFrom(entry.Value, m_cacheTrackRef.Activity);
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
                    float val = (float)UnitUtil.Elevation.ConvertFrom(entry.Value, m_cacheTrackRef.Activity);
                    if (val != float.NaN)
                    {
                        m_elevationMetersTrack0.Add(m_elevationMetersTrack0.EntryDateTime(entry), val);
                    }
                }
                float min;
                float max;
                m_elevationMetersTrack0 = ZoneFiveSoftware.Common.Data.Algorithm.NumericTimeDataSeries.Smooth(m_elevationMetersTrack0, TrailActivityInfoOptions.ElevationSmoothingSeconds, out min, out max);
            }
            return m_elevationMetersTrack0;
        }

        public INumericTimeDataSeries GradeTrack0(TrailResult refRes)
        {
            //checkCacheRef(refRes);
            if (m_gradeTrack0 == null)
            {
                m_gradeTrack0 = copySmoothTrack(this.GradeTrack, TrailActivityInfoOptions.ElevationSmoothingSeconds);
            }
            return m_gradeTrack0;
        }

        public INumericTimeDataSeries CadencePerMinuteTrack0(TrailResult refRes)
        {
            //checkCacheRef(refRes);
            if (m_cadencePerMinuteTrack0 == null)
            {
                m_cadencePerMinuteTrack0 = copySmoothTrack(this.CadencePerMinuteTrack, TrailActivityInfoOptions.CadenceSmoothingSeconds);
            }
            return m_cadencePerMinuteTrack0;
        }

        public INumericTimeDataSeries HeartRatePerMinuteTrack0(TrailResult refRes)
        {
            //checkCacheRef(refRes);
            if (m_heartRatePerMinuteTrack0 == null)
            {
                m_heartRatePerMinuteTrack0 = copySmoothTrack(this.HeartRatePerMinuteTrack, TrailActivityInfoOptions.HeartRateSmoothingSeconds);
            }
            return m_heartRatePerMinuteTrack0;
        }

        public INumericTimeDataSeries PowerWattsTrack0(TrailResult refRes)
        {
            //checkCacheRef(refRes);
            if (m_powerWattsTrack0 == null)
            {
                m_powerWattsTrack0 = copySmoothTrack(this.PowerWattsTrack, TrailActivityInfoOptions.PowerSmoothingSeconds);
            }
            return m_powerWattsTrack0;
        }

        public INumericTimeDataSeries SpeedTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_speedTrack0 == null)
            {
                m_speedTrack0 = new NumericTimeDataSeries();
                foreach (ITimeValueEntry<float> entry in this.SpeedTrack)
                {
                    float val = (float)UnitUtil.Speed.ConvertFrom(entry.Value, m_cacheTrackRef.Activity);
                    if (val != float.NaN)
                    {
                        m_speedTrack0.Add(m_speedTrack0.EntryDateTime(entry), val);
                    }
                }
                float min;
                float max;
                m_speedTrack0 = ZoneFiveSoftware.Common.Data.Algorithm.NumericTimeDataSeries.Smooth(m_speedTrack0, TrailActivityInfoOptions.SpeedSmoothingSeconds, out min, out max);
            }
            return m_speedTrack0;
        }

        public INumericTimeDataSeries PaceTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_paceTrack0 == null)
            {
                m_paceTrack0 = new NumericTimeDataSeries();
                foreach (ITimeValueEntry<float> entry in this.SpeedTrack)
                {
                    float val = (float)UnitUtil.Pace.ConvertFrom(entry.Value, m_cacheTrackRef.Activity);
                    if (val != float.NaN)
                    {
                        m_paceTrack0.Add(m_paceTrack0.EntryDateTime(entry), val);
                    }
                }
                float min;
                float max;
                m_paceTrack0 = ZoneFiveSoftware.Common.Data.Algorithm.NumericTimeDataSeries.Smooth(m_paceTrack0, TrailActivityInfoOptions.SpeedSmoothingSeconds, out min, out max);
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
                        if (m_cacheTrackRef != null && t.ElapsedSeconds <= m_cacheTrackRef.DistanceMetersTrack.TotalElapsedSeconds &&
                            t.ElapsedSeconds > oldElapsedSeconds)
                        {
                            DateTime d1 = this.getDateTimeFromElapsedResult(this.DistanceMetersTrack, t);
                            DateTime d2 = refRes.DistanceMetersTrack.GetTimeAtDistanceMeters(t.Value);
                            lastValue = (float)(-t.ElapsedSeconds + d2.Subtract(refRes.DistanceMetersTrack.StartTime).TotalSeconds);
                            if (!ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(d1, Pauses))
                            {
                                m_DiffTimeTrack0.Add(d1, lastValue);
                                oldElapsedSeconds = t.ElapsedSeconds;
                            }
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
                        if (m_cacheTrackRef != null && t.ElapsedSeconds <= m_cacheTrackRef.DistanceMetersTrack.TotalElapsedSeconds &&
                            t.ElapsedSeconds > oldElapsedSeconds)
                        {
                            DateTime d1 = this.getDateTimeFromElapsedResult(this.DistanceMetersTrack, t);
                            DateTime d2 = m_cacheTrackRef.getDateTimeFromElapsedResult(m_cacheTrackRef.DistanceMetersTrack, t);
                            lastValue = (float)UnitUtil.Distance.ConvertFrom(t.Value - m_cacheTrackRef.DistanceMetersTrack.GetInterpolatedValue(d2).Value, m_cacheTrackRef.Activity);
                            if (!ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(d1, Pauses))
                            {
                                m_DiffDistTrack0.Add(d1, lastValue);
                                oldElapsedSeconds = t.ElapsedSeconds;
                            }
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

        public IList<double> TrailPointTimeOffset0(TrailResult refRes)
        {
            //checkCacheRef(refRes);
            if (m_trailPointTimeOffset0 == null)
            {
                m_trailPointTimeOffset0 = new List<double>();
                m_trailPointTimeOffset0.Add(0);
                for (int k = 1; k < this.TimeTrailPoints.Count; k++)
                {
                    double val;
                    if (k < this.TimeTrailPoints.Count && this.TimeTrailPoints[k] != DateTime.MinValue)
                    {
                        val = this.TimeTrailPoints[k].Subtract(this.TimeTrailPoints[0]).TotalSeconds;
                    }
                    else
                    {
                        val = m_trailPointTimeOffset0[m_trailPointTimeOffset0.Count - 1];
                    }
                    m_trailPointTimeOffset0.Add(val);
                }
            }
            return m_trailPointTimeOffset0;
        }

        public IList<double> TrailPointDistOffset0(TrailResult refRes)
        {
            //checkCacheRef(refRes);
            if (m_trailPointDistOffset0 == null)
            {
                //Note: Beware of calling order. i.e. DistanceMetersTrack0(refRes) could clear m_trailPointDistOffset0
                IDistanceDataTrack distanceTrack = this.DistanceMetersTrack0(refRes);
                IList<double> timeOffset = this.TrailPointTimeOffset0(refRes);
                m_trailPointDistOffset0 = new List<double>();
                m_trailPointDistOffset0.Add(0);
                for (int k = 1; k < timeOffset.Count; k++)
                {
                    double val;
                    if (timeOffset[k] <= distanceTrack.TotalElapsedSeconds)
                    {
                        val = distanceTrack.GetInterpolatedValue(
                            distanceTrack.StartTime.AddSeconds(timeOffset[k])).Value;
                    }
                    else
                    {
                       val = m_trailPointDistOffset0[m_trailPointDistOffset0.Count - 1];
                    }
                    m_trailPointDistOffset0.Add(val);
                }
            }
            return m_trailPointDistOffset0;
        }

        private void getGps()
        {
            m_gpsTrack = new GPSRoute();
            m_gpsPoints = new List<IGPSPoint>();
            if (m_activity.GPSRoute != null && StartDateTime != DateTime.MinValue && EndDateTime != DateTime.MinValue)
            {
                for (int i = 0; i < m_activity.GPSRoute.Count; i++)
                {
                    DateTime time = m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[i]);
                    if (time.CompareTo(EndDateTime) > 0)
                    {
                        break;
                    }
                    if (time.CompareTo(StartDateTime) >= 0)
                    {
                        IGPSPoint point = m_activity.GPSRoute[i].Value;
                        if (!ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(time, Pauses))
                        {
                            m_gpsPoints.Add(point);
                            m_gpsTrack.Add(time, point);
                        }
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
            get { return (float)UnitUtil.Pace.ConvertFrom(AvgSpeed); }
        }

        float ITrailResult.AvgPower
        {
            get { return AvgPower; }
        }

        float ITrailResult.AvgSpeed
        {
            get { return (float)UnitUtil.Speed.ConvertFrom(AvgSpeed); }
        }

        INumericTimeDataSeries ITrailResult.CadencePerMinuteTrack
        {
            get { return CadencePerMinuteTrack0(m_cacheTrackRef); }
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
            get { return ElevationMetersTrack0(m_cacheTrackRef); }
        }

        TimeSpan ITrailResult.EndTime
        {
            get { return EndTime; }
        }

        double ITrailResult.FastestPace
        {
            get
            {
                checkCacheRef(m_cacheTrackRef); 
                return UnitUtil.Pace.ConvertFrom(FastestPace, m_cacheTrackRef.Activity);
            }
        }

        float ITrailResult.FastestSpeed
        {
            get
            {
                checkCacheRef(m_cacheTrackRef);
                return (float)UnitUtil.Speed.ConvertFrom(FastestSpeed, m_cacheTrackRef.Activity);
            }
        }

        INumericTimeDataSeries ITrailResult.GradeTrack
        {
            get { return GradeTrack0(m_cacheTrackRef); }
        }

        INumericTimeDataSeries ITrailResult.HeartRatePerMinuteTrack
        {
            get { return HeartRatePerMinuteTrack0(m_cacheTrackRef); }
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
            get { return PaceTrack0(m_cacheTrackRef); }
        }

        INumericTimeDataSeries ITrailResult.PowerWattsTrack
        {
            get { return PowerWattsTrack0(m_cacheTrackRef); }
        }

        INumericTimeDataSeries ITrailResult.SpeedTrack
        {
            get { return SpeedTrack0(m_cacheTrackRef); }
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
