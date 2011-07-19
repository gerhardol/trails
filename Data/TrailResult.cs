/*
Copyright (C) 2009 Brendan Doherty
Copyright (C) 2011 Gerhard Olsson

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

        private TrailResult m_parentResult;
        private TrailResultInfo m_childrenInfo;

        private DateTime? m_startTime;
        private DateTime? m_endTime;
        private float m_startDistance = float.NaN;
        private float m_totalDistDiff; //to give quality of results
        private Color m_trailColor = getColor(nextTrailColor++);
        private string m_toolTip;

        private IValueRangeSeries<DateTime> m_pauses;
        private IDistanceDataTrack m_distanceMetersTrack;
        private IDistanceDataTrack m_activityDistanceMetersTrack;
        //private INumericTimeDataSeries m_elevationMetersTrack;
        //private INumericTimeDataSeries m_cadencePerMinuteTrack;
        //private INumericTimeDataSeries m_heartRatePerMinuteTrack;
        //private INumericTimeDataSeries m_powerWattsTrack;
        //private INumericTimeDataSeries m_speedTrack;
        //private INumericTimeDataSeries m_gradeTrack;

        //Converted tracks, to display format, with smoothing
        //Should be in a separate class
        private TrailResult m_cacheTrackRef;
        private IDistanceDataTrack m_distanceMetersTrack0;
        private INumericTimeDataSeries m_cadencePerMinuteTrack0;
        private INumericTimeDataSeries m_elevationMetersTrack0;
        private INumericTimeDataSeries m_gradeTrack0;
        private INumericTimeDataSeries m_heartRatePerMinuteTrack0;
        private INumericTimeDataSeries m_powerWattsTrack0;
        private INumericTimeDataSeries m_speedTrack0;
        private INumericTimeDataSeries m_paceTrack0;
        private INumericTimeDataSeries m_DiffTimeTrack0;
        private INumericTimeDataSeries m_DiffDistTrack0;
        IList<double> m_trailPointTime0;
        IList<double> m_trailPointDist0;

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
        {
        }

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
            m_totalDistDiff = distDiff;

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
                            this.m_childrenInfo.Count > j)
                        {
                            if (m_childrenInfo.Points[j].Time != DateTime.MinValue)
                            {
                                TrailResultInfo t = m_childrenInfo.CopySlice(i, j);
                                TrailResult tr = new TrailResult(m_activityTrail, this, i+1, t, m_totalDistDiff);
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
            get { return (float)(m_totalDistDiff / Math.Pow(m_childrenInfo.Count, 1.5)); }
        }
        public int Order
        {
			get {
				return m_order;
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
                return ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.TimeNotPaused(
                   StartDateTime, EndDateTime, Pauses);
            }
        }
        public double Distance
        {
            get
            {
                if (DistanceMetersTrack == null || DistanceMetersTrack.Count == 0)
                {
                    return Info.DistanceMeters;
                }
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

        //Start/end time must match the Distance track
        public DateTime StartDateTime
        {
            get
            {
                if (m_startTime == null)
                {
                    DateTime startTime = m_childrenInfo.Points[0].Time;
                    DateTime endTime = m_childrenInfo.Points[m_childrenInfo.Points.Count - 1].Time;
                    m_startTime = getFirstUnpausedTime(startTime, Pauses, true);
                    if (endTime.CompareTo((DateTime)m_startTime) <= 0)
                    {
                        //Trail (or subtrail) is completely paused. Use all
                        m_startTime = startTime;
                    }
                    if (m_startTime == DateTime.MinValue)
                    {
                        m_startTime = Info.ActualTrackStart;
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
                    m_endTime = getFirstUnpausedTime(endTime, Pauses, false);
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

        //StartDateTime used in tracks, truncating milliseconds
        //Truncating is no longer needed, just an alias
        private DateTime StartDateTime_sec
        {
            get
            {
                return StartDateTime;
            }
        }
        private DateTime EndDateTime_sec
        {
            get
            {
                return EndDateTime;
            }
        }

        //Start/End from trail points and pauses. Used to create the Distance track
        //Separate value not needed, just an alias
        private DateTime StartDateTime_0
        {
            get
            {
                return StartDateTime;
            }
        }
        private DateTime EndDateTime_0
        {
            get
            {
                return EndDateTime;
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


        /*************************************************/
        //DateTime vs elapsed result/activity, distance result/activity conversions
        //The correct tracks must be in DistanceMetersTrack and ActivityDistanceMetersTrack

        //Get result time and distance from activity references
        public float getDistResult(DateTime t)
        {
            return getDistFromTrackTime(DistanceMetersTrack, t);
        }
        public float getDistResultFromDistActivity(double t)
        {
            return getDistFromTrackTime(DistanceMetersTrack, getDateTimeFromDistActivity(t));
        }
        public DateTime getDateTimeFromDistActivity(double t)
        {
            return getDateTimeFromTrackDist(ActivityDistanceMetersTrack, (float)t);
        }
        public DateTime getDateTimeFromDistResult(double t)
        {
            DateTime time = getDateTimeFromTrackDist(DistanceMetersTrack, (float)t);
            if (time < this.StartDateTime_0)
            {
                time = StartDateTime_0;
            }
            if (time > this.EndDateTime_0)
            {
                time = EndDateTime_0;
            }
            return time;
        }
        //public float getDistActivityFromDistResult(double t)
        //{
        //    return getDistFromTrackTime(ActivityDistanceMetersTrack, getDateTimeFromDistResult(t));
        //}
        //public double getDistResultFromUnpausedDistActivity(double t)
        //{
        //    return getDistResult(getDateTimeFromUnpausedDistActivity(t));
        //}
        //public DateTime getDateTimeFromUnpausedDistActivity(double t)
        //{
        //    return ActivityUnpausedDistanceMetersTrack.GetTimeAtDistanceMeters(t);
        //}

        //Elapsed vs DateTime for result
        public double getElapsedResult(DateTime d)
        {
            return getElapsedTimeSpanResult(d).TotalSeconds;
        }

        private TimeSpan getElapsedTimeSpanResult(DateTime entryTime)
        {
            TimeSpan elapsed = ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.TimeNotPaused(
                this.DistanceMetersTrack.StartTime, entryTime, Pauses);
            return elapsed;
        }
        //private TimeSpan getElapsedTimeSpanResult(IDistanceDataTrack dataSeries, ITimeValueEntry<float> time)
        //{
        //    DateTime entryTime = dataSeries.EntryDateTime(time);//TODO: Use here is correct, but depends on source. To clarify
        //    return getElapsedTimeSpanResult(entryTime);
        //}
        //private TimeSpan getElapsedWithoutPauses(IGPSRoute dataSeries, ITimeValueEntry<IGPSPoint> time)
        //{
        //    DateTime entryTime = dataSeries.EntryDateTime(time);
        //    return getElapsedWithoutPauses(entryTime);
        //}
        //private TimeSpan getElapsedWithoutPauses(INumericTimeDataSeries dataSeries, ITimeValueEntry<float> time)
        //{
        //    DateTime entryTime = dataSeries.EntryDateTime(time);
        //    return getElapsedWithoutPauses(entryTime);
        //}

        //(Distance) Result to activity 
        public DateTime getDateTimeFromElapsedResult(float t)
        {
            DateTime time = ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.AddTimeAndPauses(DistanceMetersTrack.StartTime, TimeSpan.FromSeconds(t), Pauses);
            if (time < this.StartDateTime_0)
            {
                time = StartDateTime_0;
            }
            if (time > this.EndDateTime_0)
            {
                time = EndDateTime_0;
            }
            return time;
        }
        public DateTime getDateTimeFromElapsedActivity(float t)
        {
            return ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.AddTimeAndPauses(ActivityDistanceMetersTrack.StartTime, TimeSpan.FromSeconds(t), Pauses);
        }

        /**********************************/
        //Static methods
        private static float getDistFromTrackTime(IDistanceDataTrack distTrack, DateTime t)
        {
            int status;
            return getDistFromTrackTime(distTrack, t, out status);
        }
        private static float getDistFromTrackTime(IDistanceDataTrack distTrack, DateTime t, out int status)
        {
            //Ignore malformed activities and selection outside the result
            float res = 0;
            status = 1;
            ITimeValueEntry<float> entry = distTrack.GetInterpolatedValue(t);
            if (entry != null)
            {
                res = entry.Value;
                status = 0;
            }
            else if (distTrack.Count > 0 && t >= distTrack.StartTime.AddSeconds(2))
            {
                //Seem to be out of bounds
                //Any time after start is handled as end, as pauses may complicate calcs (default is 0, i.e. before)
                res = distTrack[distTrack.Count - 1].Value;
            }
            return res;
        }

        public static DateTime getDateTimeFromTrackDist(IDistanceDataTrack distTrack, float t)
        {
            DateTime res = DateTime.MinValue;
            if (t >= distTrack.Max && distTrack.Count > 0)
            {
                res = distTrack.StartTime.AddSeconds(distTrack.TotalElapsedSeconds);
            }
            else if (t <= distTrack.Min)
            {
                res = distTrack.StartTime;
            }
            else
            {
                try
                {
                    res = distTrack.GetTimeAtDistanceMeters(t);
                }
                catch { }
            }
            return res;
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

        private static DateTime getFirstUnpausedTime(DateTime time, IValueRangeSeries<DateTime> pauses, bool next)
        {
            if (ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(time, pauses))
            {
                foreach (IValueRange<DateTime> pause in pauses)
                {
                    if (time.CompareTo(pause.Lower) >= 0 &&
                        time.CompareTo(pause.Upper) <= 0)
                    {
                        if (next)
                        {
                            time = (pause.Upper).Add(TimeSpan.FromSeconds(1));
                        }
                        else if(pause.Lower > DateTime.MinValue)
                        {
                            time = (pause.Lower).Add(TimeSpan.FromSeconds(-1));
                        }
                        break;
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

        public IValueRangeSeries<DateTime> Pauses
        {
            get
            {
                if (m_pauses == null)
                {
                    //Note: Assumes that subtrails are a part of parent - could be changed
                    m_pauses = new ValueRangeSeries<DateTime>();
                    if (ParentResult != null)
                    {
                        foreach (ValueRange<DateTime> t in ParentResult.Pauses)
                        {
                            m_pauses.Add(t);
                        }
                    }
                    else
                    {
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
                            for (int i = 0; i < Activity.Laps.Count; i++)
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
                    //if (ParentResult != null)
                    //{
                    //    if (this.ParentResult.StartDateTime_0 < this.StartDateTime_0)
                    //    {
                    //        m_pauses.Add(new ValueRange<DateTime>(this.ParentResult.StartDateTime_0, this.StartDateTime_0));
                    //    }
                    //    if (this.EndDateTime_0 < this.ParentResult.EndDateTime_0)
                    //    {
                    //        m_pauses.Add(new ValueRange<DateTime>(this.EndDateTime_0, this.ParentResult.EndDateTime_0));
                    //    }
                    //}
                }
                return m_pauses;
            }
        }

        /*********************************************/
        //Distance

        private void getDistanceTrack()
        {
            if (null == m_distanceMetersTrack)
            { 
                m_distanceMetersTrack = new DistanceDataTrack();
                m_distanceMetersTrack.AllowMultipleAtSameTime = false;
                if (this.ParentResult != null)
                {
                    m_activityDistanceMetersTrack = this.ParentResult.ActivityDistanceMetersTrack;
                    //m_distanceMetersTrack could be created from parent too, but it is simpler and not heavier to use same code-path as parent
                }
                else
                {
                    if (includeStopped())
                    {
                        m_activityDistanceMetersTrack = Info.ActualDistanceMetersTrack;
                    }
                    else
                    {
                        m_activityDistanceMetersTrack = Info.MovingDistanceMetersTrack;
                    }
                    m_activityDistanceMetersTrack.AllowMultipleAtSameTime = false;
                    if (m_activityDistanceMetersTrack != null)
                    {
                        //insert points at borders
                      (new InsertValues<float>(this)).insertValues(m_activityDistanceMetersTrack);
                    }
                }
                if (m_activityDistanceMetersTrack != null)
                {
                    int i = 0;
                    float prevDist = float.NaN;
                    float distance = 0;
                    int oldElapsed = int.MinValue;
                    while (i < m_activityDistanceMetersTrack.Count &&
                        StartDateTime_0 > m_activityDistanceMetersTrack.EntryDateTime(m_activityDistanceMetersTrack[i]))
                    {
                        i++;
                    }
                    if (i < m_activityDistanceMetersTrack.Count)
                    {
                        m_startDistance = m_activityDistanceMetersTrack[i].Value;
                    }

                    while (i < m_activityDistanceMetersTrack.Count &&
                        EndDateTime_0 >= m_activityDistanceMetersTrack.EntryDateTime(m_activityDistanceMetersTrack[i]))
                    {
                        ITimeValueEntry<float> timeValue = m_activityDistanceMetersTrack[i];
                        uint elapsed = timeValue.ElapsedSeconds;
                        DateTime time = m_activityDistanceMetersTrack.EntryDateTime(timeValue);
                        if (elapsed > oldElapsed && 
                            !ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(time, Pauses))
                        {
                            float actDist = timeValue.Value;
                            if (!float.IsNaN(prevDist))
                            {
                                distance += actDist - prevDist;
                            }
                            m_distanceMetersTrack.Add(time, distance);
                            prevDist = actDist;
                            oldElapsed = (int)elapsed;
                        }
                        else
                        {
                            prevDist = float.NaN;
                        }
                        i++;
                    }
                }
            }
        }

        //Distance track for trail, adjusted with pauses
        public IDistanceDataTrack DistanceMetersTrack
        {
            get
            {
                getDistanceTrack();
                return m_distanceMetersTrack;
            }
        }

        //Distance track as ST sees it. Used in conversions
        public IDistanceDataTrack ActivityDistanceMetersTrack
        {
            get
            {
                getDistanceTrack();
                return m_activityDistanceMetersTrack;
            }
        }

        //The Activity related tracks were previously cached. Kept as an uniform method to get
        //The tracks, if the Info cache handling is changed
        private INumericTimeDataSeries CadencePerMinuteTrack
        {
            get
            {
                return Info.SmoothedCadenceTrack;
            }
        }

        private INumericTimeDataSeries ElevationMetersTrack
        {
            get
            {
                return Info.SmoothedElevationTrack;
            }
        }
        private INumericTimeDataSeries GradeTrack
        {
            get
            {
                return Info.SmoothedGradeTrack;
            }
        }

        private INumericTimeDataSeries HeartRatePerMinuteTrack
        {
            get
            {
                return Info.SmoothedHeartRateTrack;
            }
        }

        private INumericTimeDataSeries PowerWattsTrack
        {
            get
            {
                return Info.SmoothedPowerTrack;
            }
        }

        private INumericTimeDataSeries SpeedTrack
        {
            get
            {
                return Info.SmoothedSpeedTrack;
            }
        }

        public IList<DateTime> TrailPointDateTime
        {
            get
            {
                return m_childrenInfo.CopyTime();
            }
        }

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
        
		public float AvgCadence {
			get {
                return (float)UnitUtil.Power.ConvertTo(CadencePerMinuteTrack0(m_cacheTrackRef).Avg,
                    m_cacheTrackRef.Activity);
			}
		}
		public float AvgHR {
			get {
                return HeartRatePerMinuteTrack0(m_cacheTrackRef).Avg;
			}
		}
		public float MaxHR {
			get {
				return HeartRatePerMinuteTrack0(m_cacheTrackRef).Max;
			}
		}
		public float AvgPower {
			get {
                return (float)UnitUtil.Power.ConvertTo(PowerWattsTrack0(m_cacheTrackRef).Avg,
                    m_cacheTrackRef.Activity);
			}
		}
		public float AvgGrade {
			get {
                return GradeTrack0(m_cacheTrackRef).Avg;
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
                INumericTimeDataSeries elevationTrack = ElevationMetersTrack0(m_cacheTrackRef);
                if (elevationTrack != null && elevationTrack.Count > 1)
                {
                    value = (float)UnitUtil.Elevation.ConvertTo(
                        elevationTrack[elevationTrack.Count - 1].Value -
                        elevationTrack[0].Value,
                        m_cacheTrackRef.Activity);
                }
                return value;
			}
		}

		public IActivityCategory Category {
			get {
				return m_activity.Category;
			}
		}

        /**************************************************************************/
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
            m_trailPointTime0 = null;
            m_trailPointDist0 = null;

            //smoothing control
            //m_TrailActivityInfoOptions = null;

            if (!onlyDisplay)
            {
                m_pauses = null;
                m_startTime = null;
                m_endTime = null;

                //m_ActivityInfo = null;
                m_distanceMetersTrack = null;
                m_activityDistanceMetersTrack = null;
                //m_elevationMetersTrack = null;
                //m_cadencePerMinuteTrack = null;
                //m_heartRatePerMinuteTrack = null;
                //m_powerWattsTrack = null;
                //m_speedTrack = null;
                ////m_paceTrack = null;
                //m_gradeTrack = null;

                m_gpsPoints = null;
                m_gpsTrack = null;
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

        internal class InsertValues<T>
        {
            TrailResult result;
            public InsertValues(TrailResult t)
            {
                this.result = t;
            }
            public void insertValues(ITimeDataSeries<T> track, DateTime atime)
            {
                //atime -= TimeSpan.FromMilliseconds(atime.Millisecond);
                if (atime >= result.StartDateTime_0 && atime <= result.EndDateTime_0 &&
                    !ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(atime, result.Pauses))
                {
                    ITimeValueEntry<T> interpolatedP = track.GetInterpolatedValue(atime);
                    if (interpolatedP != null)
                    {
                        try
                        {
                            track.Add(atime, interpolatedP.Value);
                        }
                        catch { }
                    }
                }
            }
            public void insertValues(ITimeDataSeries<T> track)
            {
                //Insert points around pauses and points
                //This is needed to get the track match the cut-up activity
                //(otherwise for instance start point need to be added)

                //start/end should be included from points, but prepare for changes...
                insertValues(track, result.StartDateTime_0);
                insertValues(track, result.EndDateTime_0);
                foreach (IValueRange<DateTime> p in result.Pauses)
                {
                    if (p.Lower > DateTime.MinValue)
                    {
                        insertValues(track, p.Lower.AddSeconds(-1));
                    }
                    insertValues(track, p.Upper.AddSeconds(1));
                }
                foreach (TrailResultPoint t in result.m_childrenInfo.Points)
                {
                    DateTime time = t.Time;
                    if (time > DateTime.MinValue)
                    {
                        insertValues(track, time.Subtract(TimeSpan.FromSeconds(1)));
                        insertValues(track, time);
                    }
                }

            }
        }

        //copy the relevant part to a new track
        private INumericTimeDataSeries copyTrailTrack(INumericTimeDataSeries source)
        {
            return copySmoothTrack(source, true, 0, new Convert(ConvertNone), m_cacheTrackRef);
        }

        //Convert to/from internal format to display format
        public delegate double Convert(double value, IActivity activity);
        //Empty definition, when no conversion needed
        public static double ConvertNone(double p, IActivity activity)
        {
            return p;
        }

        //Copy a track and apply smoothing
        public INumericTimeDataSeries copySmoothTrack(INumericTimeDataSeries source, bool insert, int smooth, Convert convert, TrailResult refRes)
        {
            IActivity refActivity = null;
            if (refRes != null)
            {
                refActivity = refRes.Activity;
            }
            INumericTimeDataSeries track = new NumericTimeDataSeries();
            track.AllowMultipleAtSameTime = false;
            if (source != null)
            {
                int oldElapsed = int.MinValue;
                foreach (ITimeValueEntry<float> t in source)
                {
                    DateTime time = source.EntryDateTime(t);
                    if (StartDateTime <= time && time <= EndDateTime &&
                        !ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(time, Pauses))
                    {
                        uint elapsed = t.ElapsedSeconds;
                        if (elapsed > oldElapsed)
                        {
                            track.Add(time, (float)convert(t.Value, refActivity));
                            oldElapsed = (int)elapsed;
                        }
                    }
                    if (time > EndDateTime)
                    {
                        break;
                    }
                }
                if (insert)
                {
                    //Insert values around borders, to limit effects when track is chopped
                    (new InsertValues<float>(this)).insertValues(track);
                }
                if (smooth > 0)
                {
                    float min; float max;
                    track = ZoneFiveSoftware.Common.Data.Algorithm.NumericTimeDataSeries.Smooth(track, smooth, out min, out max);
                }
            }
            return track;
        }


        public IDistanceDataTrack DistanceMetersTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_distanceMetersTrack0 == null)
            {
                //Just copy the track. No smoothing or inserting of values
                m_distanceMetersTrack0 = new DistanceDataTrack();
                foreach (ITimeValueEntry<float> entry in this.DistanceMetersTrack)
                {
                    float val = (float)UnitUtil.Distance.ConvertFrom(entry.Value, m_cacheTrackRef.Activity);
                    if (!float.IsNaN(val))
                    {
                        DateTime time = DistanceMetersTrack.EntryDateTime(entry);
                        m_distanceMetersTrack0.Add(time, val);
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
                m_elevationMetersTrack0 = copySmoothTrack(this.ElevationMetersTrack, true, TrailActivityInfoOptions.ElevationSmoothingSeconds,
                    new Convert(UnitUtil.Elevation.ConvertFrom), refRes);
            }
            return m_elevationMetersTrack0;
        }

        public INumericTimeDataSeries GradeTrack0(TrailResult refRes)
        {
            //checkCacheRef(refRes);
            if (m_gradeTrack0 == null)
            {
                m_gradeTrack0 = copySmoothTrack(this.GradeTrack, true, TrailActivityInfoOptions.ElevationSmoothingSeconds,
                    new Convert(UnitUtil.Grade.ConvertFrom), refRes);
            }
            return m_gradeTrack0;
        }

        public INumericTimeDataSeries CadencePerMinuteTrack0(TrailResult refRes)
        {
            //checkCacheRef(refRes);
            if (m_cadencePerMinuteTrack0 == null)
            {
                m_cadencePerMinuteTrack0 = copySmoothTrack(this.CadencePerMinuteTrack, true, TrailActivityInfoOptions.CadenceSmoothingSeconds,
                    new Convert(UnitUtil.Cadence.ConvertFrom), refRes);
            }
            return m_cadencePerMinuteTrack0;
        }

        public INumericTimeDataSeries HeartRatePerMinuteTrack0(TrailResult refRes)
        {
            //checkCacheRef(refRes);
            if (m_heartRatePerMinuteTrack0 == null)
            {
                m_heartRatePerMinuteTrack0 = copySmoothTrack(this.HeartRatePerMinuteTrack, true, TrailActivityInfoOptions.HeartRateSmoothingSeconds,
                    new Convert(UnitUtil.HeartRate.ConvertFrom), refRes);
            }
            return m_heartRatePerMinuteTrack0;
        }

        public INumericTimeDataSeries PowerWattsTrack0(TrailResult refRes)
        {
            //checkCacheRef(refRes);
            if (m_powerWattsTrack0 == null)
            {
                m_powerWattsTrack0 = copySmoothTrack(this.PowerWattsTrack, true, TrailActivityInfoOptions.PowerSmoothingSeconds,
                    new Convert(UnitUtil.Power.ConvertFrom), refRes);
            }
            return m_powerWattsTrack0;
        }

        public INumericTimeDataSeries SpeedTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_speedTrack0 == null)
            {
                m_speedTrack0 = copySmoothTrack(this.SpeedTrack, false, TrailActivityInfoOptions.SpeedSmoothingSeconds,
                                new Convert(UnitUtil.Speed.ConvertFrom), refRes); 
            }
            return m_speedTrack0;
        }

        public INumericTimeDataSeries PaceTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_paceTrack0 == null)
            {
                m_paceTrack0 = copySmoothTrack(this.SpeedTrack, false, TrailActivityInfoOptions.SpeedSmoothingSeconds,
                                new Convert(UnitUtil.Pace.ConvertFrom), refRes);
            }
            return m_paceTrack0;
        }

        /********************************************************************/
        public INumericTimeDataSeries DiffTimeTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_DiffTimeTrack0 == null && DistanceMetersTrack.Count > 0)
            {
                m_DiffTimeTrack0 = new NumericTimeDataSeries();
                int oldElapsed = int.MinValue;
                float lastValue = 0;
                int dateTrailPointIndex = -1;
                float refOffset = 0;
                float refPrevElapsedSec = 0;
                float diffOffset = 0;
                foreach (ITimeValueEntry<float> t in DistanceMetersTrack)
                {
                    uint elapsed = t.ElapsedSeconds;
                    if (elapsed > oldElapsed && m_cacheTrackRef != null &&
                        elapsed <= m_cacheTrackRef.DistanceMetersTrack.TotalElapsedSeconds)
                    {
                        DateTime d1 = this.DistanceMetersTrack.EntryDateTime(t);
                        if (!ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(d1, Pauses))
                        {
                            while (Settings.ResyncDiffAtTrailPoints &&
                                (dateTrailPointIndex == -1 ||
                                dateTrailPointIndex < this.TrailPointDateTime.Count - 1 &&
                                d1 > this.TrailPointDateTime[dateTrailPointIndex + 1]))
                            {
                                dateTrailPointIndex++;
                                if (dateTrailPointIndex < this.TrailPointDateTime.Count - 1 &&
                                    dateTrailPointIndex < m_cacheTrackRef.TrailPointDateTime.Count - 1 &&
                                    this.TrailPointDateTime[dateTrailPointIndex] > DateTime.MinValue &&
                                    m_cacheTrackRef.TrailPointDateTime[dateTrailPointIndex] > DateTime.MinValue)
                                {
                                    refOffset = m_cacheTrackRef.getDistResult(m_cacheTrackRef.TrailPointDateTime[dateTrailPointIndex]) -
                                       this.getDistResult(this.TrailPointDateTime[dateTrailPointIndex]);
                                    if (Settings.AdjustResyncDiffAtTrailPoints)
                                    {
                                        //diffdist over time will normally "jump" at each trail point
                                        //I.e. if the reference is behind, the distance suddenly gained must be subtracted
                                        //Note: ST only uses int for time, use double anyway
                                        float oldElapsedSec = oldElapsed > 0 ? oldElapsed : 0;
                                        float refElapsedP = (float)m_cacheTrackRef.getElapsedResult(m_cacheTrackRef.TrailPointDateTime[dateTrailPointIndex]);
                                        float elapsedP = (float)this.getElapsedResult(this.TrailPointDateTime[dateTrailPointIndex]);
                                        diffOffset += (refElapsedP - refPrevElapsedSec - (elapsedP - oldElapsed));
                                    }
                                }
                            }
                        }

                        //elapsed (entry) is elapsed in the series, not elapsed seconds....
                        float elapsedSec = (float)this.getElapsedResult(d1);
                        float refElapsedSec;
                        if (m_cacheTrackRef == this)
                        {
                            //"inconsistency" from getDateTimeFromTrack() can happen if the ref stands still, getDateTimeFromTrack returns first elapsed
                            refElapsedSec = elapsedSec;
                        }
                        else
                        {
                            DateTime d2 = TrailResult.getDateTimeFromTrackDist(m_cacheTrackRef.DistanceMetersTrack, t.Value + refOffset);
                            refElapsedSec = (float)m_cacheTrackRef.getElapsedResult(d2);
                        }
                        lastValue = refElapsedSec - elapsedSec + diffOffset;
                        m_DiffTimeTrack0.Add(d1, lastValue);
                        oldElapsed = (int)elapsed;
                        refPrevElapsedSec = refElapsedSec;
                    }
                }
                //Add a point last in the track, to show the complete dist in the chart
                //Alternative would be to use speed to extrapolate difference
                if (oldElapsed < DistanceMetersTrack.TotalElapsedSeconds)
                {
                    DateTime d1 = this.DistanceMetersTrack.EntryDateTime(DistanceMetersTrack[DistanceMetersTrack.Count - 1]);
                    m_DiffTimeTrack0.Add(d1, lastValue);
                }
            }
            return m_DiffTimeTrack0;
        }

        private ITimeValueEntry<float> getValueEntryOffset(ITimeValueEntry<float> t, int refOffset)
        {
            uint refElapsed = t.ElapsedSeconds;
            if (refElapsed > -refOffset)
            {
                refElapsed = (uint)(refElapsed + refOffset);
            }
            return new TimeValueEntry<float>(refElapsed, t.Value);
        }

        public INumericTimeDataSeries DiffDistTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_DiffDistTrack0 == null && DistanceMetersTrack.Count > 0)
            {
                m_DiffDistTrack0 = new NumericTimeDataSeries();
                int oldElapsed = int.MinValue;
                float lastValue = 0;
                int dateTrailPointIndex = -1;
                int refOffset = 0;
                float diffOffset = 0;
                double prevDist = 0;
                double prevRefDist = 0;
                foreach (ITimeValueEntry<float> t in DistanceMetersTrack)
                {
                    uint elapsed = t.ElapsedSeconds;
                    if (elapsed > oldElapsed && elapsed <= m_cacheTrackRef.DistanceMetersTrack.TotalElapsedSeconds)
                    {
                        DateTime d1 = DistanceMetersTrack.EntryDateTime(t);
                        while (Settings.ResyncDiffAtTrailPoints &&
                            (dateTrailPointIndex == -1 ||
                            dateTrailPointIndex < this.TrailPointDateTime.Count - 1 &&
                            d1 > this.TrailPointDateTime[dateTrailPointIndex + 1]))
                        {
                            dateTrailPointIndex++;
                            if (dateTrailPointIndex < this.TrailPointDateTime.Count - 1 &&
                                dateTrailPointIndex < m_cacheTrackRef.TrailPointDateTime.Count - 1 &&
                                this.TrailPointDateTime[dateTrailPointIndex] > DateTime.MinValue &&
                                m_cacheTrackRef.TrailPointDateTime[dateTrailPointIndex] > DateTime.MinValue)
                            {
                                refOffset = (int)(m_cacheTrackRef.getElapsedResult(m_cacheTrackRef.TrailPointDateTime[dateTrailPointIndex]) -
                                   this.getElapsedResult(this.TrailPointDateTime[dateTrailPointIndex]));
                                //TODO: Configure, explain.
                                if (Settings.AdjustResyncDiffAtTrailPoints)
                                {
                                    //diffdist over time will normally "jump" at each trail point
                                    //I.e. if the reference is behind, the distance suddenly gained must be subtracted
                                    int status2;
                                    double refDistP = getDistFromTrackTime(m_cacheTrackRef.DistanceMetersTrack,
                                        m_cacheTrackRef.TrailPointDateTime[dateTrailPointIndex], out status2);
                                    if (status2 == 0)
                                    {
                                        double distP = getDistFromTrackTime(this.DistanceMetersTrack,
                                            this.TrailPointDateTime[dateTrailPointIndex], out status2);
                                        if (status2 == 0)
                                        {
                                            diffOffset += (float)(refDistP - prevRefDist - (distP - prevDist));
                                        }
                                    }
                                }
                            }
                        }

                        DateTime d2 = m_cacheTrackRef.DistanceMetersTrack.EntryDateTime(getValueEntryOffset(t, refOffset));
                        int status;
                        double refDist = getDistFromTrackTime(m_cacheTrackRef.DistanceMetersTrack, d2, out status);
                        if (status == 0)
                        {
                            //Only add if valid estimation
                            lastValue = (float)UnitUtil.Distance.ConvertFrom(t.Value - refDist + diffOffset, m_cacheTrackRef.Activity);
                            m_DiffDistTrack0.Add(d1, lastValue);
                            oldElapsed = (int)elapsed;
                            prevDist = t.Value;
                            prevRefDist = refDist;
                        }
                    }
                }
                //Add a point last in the track, to show the complete dist in the chart
                //Alternatively use speed to extrapolate difference
                if (oldElapsed < DistanceMetersTrack.TotalElapsedSeconds)
                {
                    DateTime d1 = DistanceMetersTrack.EntryDateTime(DistanceMetersTrack[DistanceMetersTrack.Count - 1]);
                    m_DiffDistTrack0.Add(d1, lastValue);
                }
            }
            return m_DiffDistTrack0;
        }


        /********************************************************************/
        IList<double> m_trailPointTimeOffset;
        IList<double> m_trailPointDistOffset;
        public IList<double> TrailPointTimeOffset0(TrailResult refRes)
        {
            TrailPointTime0(refRes);
            return m_trailPointTimeOffset;
        }
        public IList<double> TrailPointDistOffset01(TrailResult refRes)
        {
            TrailPointDist0(refRes);
            IList<double> trailPointDistOffset0 = new List<double>();
            foreach(float f in m_trailPointDistOffset)
            {
                trailPointDistOffset0.Add(UnitUtil.Distance.ConvertFrom(f, m_cacheTrackRef.Activity));
            }

            return trailPointDistOffset0;
        }
        public IList<double> TrailPointTime0(TrailResult refRes)
        {
            //checkCacheRef(refRes);
            if (m_trailPointTime0 == null || m_trailPointTimeOffset == null)
            {
                m_trailPointTime0 = new List<double>();
                m_trailPointTimeOffset = new List<double>();
                for (int k = 0; k < this.TrailPointDateTime.Count; k++)
                {
                    double val, val1;
                    if (this.TrailPointDateTime[k] > DateTime.MinValue)
                    {
                        //The used start time for the point
                        DateTime t1 = getFirstUnpausedTime(this.TrailPointDateTime[k], Pauses, true);
                        val = ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.TimeNotPaused(
                                StartDateTime, t1, Pauses).TotalSeconds;
                        //Offset time from detected to actual start
                        val1 = ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.TimeNotPaused(
                                this.TrailPointDateTime[k], t1, Activity.TimerPauses).TotalSeconds;
                    }
                    else
                    {
                        if (k > 0)
                        {
                            val = m_trailPointTime0[m_trailPointTime0.Count - 1];
                        }
                        else
                        {
                            val = 0;
                        }
                        val1 = 0;
                    }
                    m_trailPointTime0.Add(val);
                    m_trailPointTimeOffset.Add(val1);
                }
                for (int k = 0; k < m_trailPointTimeOffset.Count - 1; k++)
                {
                    if (m_trailPointTimeOffset[k] > m_trailPointTime0[k + 1] - m_trailPointTime0[k])
                    {
                        m_trailPointTimeOffset[k] = 0;
                    }
                }
            }
            return m_trailPointTime0;
        }

        public IList<double> TrailPointDist0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_trailPointDist0 == null || m_trailPointDistOffset == null)
            {
                m_trailPointDist0 = new List<double>();
                m_trailPointDistOffset = new List<double>();
                for (int k = 0; k < TrailPointDateTime.Count; k++)
                {
                    double val=0, val1=0;
                    if (TrailPointDateTime[k] > DateTime.MinValue)
                    {
                        ITimeValueEntry<float> interpolatedP = DistanceMetersTrack0(m_cacheTrackRef).GetInterpolatedValue(getFirstUnpausedTime(this.TrailPointDateTime[k], Pauses, true));
                        if (interpolatedP != null)
                        {
                            val = interpolatedP.Value;
                        }
                        else if (k > 0)
                        {
                            val = m_trailPointDist0[k - 1];
                        }

                        //Use "ST" distance track to get offset from detected start to used start
                        interpolatedP = ActivityDistanceMetersTrack.GetInterpolatedValue(getFirstUnpausedTime(this.TrailPointDateTime[k], Pauses, true));
                        ITimeValueEntry<float> interpolatedP2 = ActivityDistanceMetersTrack.GetInterpolatedValue(this.TrailPointDateTime[k]);
                        if (interpolatedP != null && interpolatedP2 != null)
                        {
                            val1 = interpolatedP.Value - interpolatedP2.Value;
                        }
                    }
                    else
                    {
                        if (k > 0)
                        {
                            val = m_trailPointDist0[k - 1];
                        }
                    }
                    m_trailPointDist0.Add(val);
                    m_trailPointDistOffset.Add(val1);
                }
                for (int k = 0; k < m_trailPointDistOffset.Count; k++)
                {
                    if (k < m_trailPointDistOffset.Count - 1 && m_trailPointDistOffset[k] > m_trailPointDist0[k + 1] - m_trailPointDist0[k])
                    {
                        m_trailPointDistOffset[k] = 0;
                    }
                }
            }
            return m_trailPointDist0;
        }

        /****************************************************************/
        private void getGps()
        {
            m_gpsTrack = new GPSRoute();
            m_gpsTrack.AllowMultipleAtSameTime = false;
            m_gpsPoints = null;
            if (m_activity.GPSRoute != null && m_activity.GPSRoute.Count > 0 &&
                StartDateTime_sec != DateTime.MinValue && EndDateTime_sec != DateTime.MinValue)
            {
                int i = 0;
                while (i < m_activity.GPSRoute.Count &&
                       StartDateTime_sec > m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[i]))
                {
                    i++;
                }
                while (i < m_activity.GPSRoute.Count &&
                       m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[i]) < this.EndDateTime_sec)
                {
                    DateTime time = m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[i]);
                    IGPSPoint point = m_activity.GPSRoute[i].Value;
                    if (!ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(time, Pauses))
                    {
                        m_gpsTrack.Add(time, point);
                    }
                    i++;
                }
                (new InsertValues<IGPSPoint>(this)).insertValues(m_gpsTrack);
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
            if (m_gpsTrack == null || m_gpsPoints == null)
            {
                IValueRangeSeries<DateTime> t = new ValueRangeSeries<DateTime>();
                t.Add(new ValueRange<DateTime>(StartDateTime, EndDateTime));
                IList<IList<IGPSPoint>> list = GpsPoints(t);
                if (list != null && list.Count > 0)
                {
                    m_gpsPoints = list[0];
                }
                else
                {
                    m_gpsPoints = new List<IGPSPoint>();
                }
            }
            return m_gpsPoints;
        }

        public IList<IList<IGPSPoint>> GpsPoints(Data.TrailsItemTrackSelectionInfo t)
        {
            //Before calling, the MarkedTimes should be set
            if (t.MarkedTimes != null && t.MarkedTimes.Count > 0)
            {
                if (t.MarkedTimes.Count == 1 &&
                    t.MarkedTimes[0].Lower == StartDateTime &&
                    t.MarkedTimes[0].Upper == EndDateTime)
                {
                    //Use cache
                    return new List<IList<IGPSPoint>> { this.GpsPoints() };
                }
                return GpsPoints(t.MarkedTimes);
            }
            //else if (t.MarkedDistances != null && t.MarkedDistances.Count > 0)
            //{
            //    return GpsPoints(t.MarkedDistances);
            //}
            return new List<IList<IGPSPoint>>();
        }

        private IList<IList<IGPSPoint>> GpsPoints(IValueRangeSeries<DateTime> t)
        {
            IList<IList<IGPSPoint>> result = new List<IList<IGPSPoint>>();

                foreach (IValueRange<DateTime> r in t)
                {
                    //IGPSRoute GpsTrack = Activity.GPSRoute;
                    IList<IGPSPoint> track = new List<IGPSPoint>();
                    foreach(ITimeValueEntry<IGPSPoint> entry in this.GPSRoute)
                    {
                        DateTime time = this.GPSRoute.EntryDateTime(entry);
                        if (r.Lower <= time && time <= r.Upper &&
                            !ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(time, Pauses))
                        {
                            track.Add(entry.Value);
                        }
                    }
                    //if (Activity.GPSRoute != null)
                    //{
                    //int i = 0;
                    //while (i < GpsTrack.Count &&
                    //    0 < r.Lower.CompareTo(GpsTrack.EntryDateTime(GpsTrack[i])))
                    //{
                    //    i++;
                    //}
                    //if (i < GpsTrack.Count &&
                    //    0 > r.Lower.CompareTo(GpsTrack.EntryDateTime(GpsTrack[i])))
                    //{
                    //    //Insert start point
                    //    ITimeValueEntry<IGPSPoint> interpolatedP = GpsTrack.GetInterpolatedValue(r.Lower);
                    //    if (interpolatedP != null)
                    //    {
                    //        track.Add(interpolatedP.Value);
                    //    }
                    //}
                    //ITimeValueEntry<IGPSPoint> entry = null;
                    //while (i < GpsTrack.Count &&
                    //    0 <= r.Upper.CompareTo(GpsTrack.EntryDateTime(GpsTrack[i])))
                    //{
                    //    entry = GpsTrack[i];
                    //    track.Add(entry.Value);
                    //    i++;
                    //}
                    //if (entry!= null &&
                    //    0 > r.Upper.CompareTo(GpsTrack.EntryDateTime(entry)))
                    //{
                    //    ITimeValueEntry<IGPSPoint> interpolatedP = GpsTrack.GetInterpolatedValue(r.Upper);
                    //    if (interpolatedP != null)
                    //    {
                    //        track.Add(interpolatedP.Value);
                    //    }
                    //}
                    result.Add(track);
                //}
            }

            return result;
        }

        //Note: IItemTrackSelectionInfo uses Activity distance
        //private IList<IList<IGPSPoint>> GpsPoints(IValueRangeSeries<double> t)
        //{
        //    IList<IList<IGPSPoint>> result = new List<IList<IGPSPoint>>();
        //    if (Activity.GPSRoute != null)
        //    {
        //        IGPSRoute GpsTrack = Activity.GPSRoute;
        //        IDistanceDataTrack DistanceMetersTrack = ActivityDistanceMetersTrack;

        //        foreach (IValueRange<double> r in t)
        //        {
        //            IList<IGPSPoint> track = new List<IGPSPoint>();
        //            int i = 0;
        //            while (i < ActivityDistanceMetersTrack.Count &&
        //                getDistResultFromDistActivity(r.Lower) > ActivityDistanceMetersTrack[i].Value)
        //            {
        //                i++;
        //            }
        //            DateTime time = getDateTimeFromDistActivity(r.Lower);
        //            if (i < GpsTrack.Count &&
        //                0 > time.CompareTo(GpsTrack.EntryDateTime(GpsTrack[i])))
        //            {
        //                //Insert start point
        //                ITimeValueEntry<IGPSPoint> interpolatedP = GpsTrack.GetInterpolatedValue(time);
        //                if (interpolatedP != null)
        //                {
        //                    track.Add(interpolatedP.Value);
        //                }
        //            }
        //            ITimeValueEntry<IGPSPoint> entry = null;
        //            while (i < ActivityDistanceMetersTrack.Count &&
        //                getDistResultFromDistActivity(r.Upper) >= ActivityDistanceMetersTrack[i].Value)
        //            {
        //                time = ActivityDistanceMetersTrack.EntryDateTime(ActivityDistanceMetersTrack[i]);
        //                entry = GpsTrack.GetInterpolatedValue(time);
        //                if (entry != null)
        //                {
        //                    track.Add(entry.Value);
        //                }
        //                i++;
        //            }
        //            time = getDateTimeFromDistActivity(r.Upper);
        //            if (entry != null &&
        //                0 > time.CompareTo(GpsTrack.EntryDateTime(entry)))
        //            {
        //                ITimeValueEntry<IGPSPoint> interpolatedP = GpsTrack.GetInterpolatedValue(time);
        //                if (interpolatedP != null)
        //                {
        //                    track.Add(interpolatedP.Value);
        //                }
        //            }
        //            result.Add(track);
        //        }
        //    }
        //    return result;
        //}

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

        ActivityInfo m_ActivityInfo = null;
        ActivityInfo Info
        {
            get
            {
                //Caching is not needed, done by ST
                //return ActivityInfoCache.Instance.GetInfo(this.Activity);
                //Custom InfoCache, to control smoothing
                if (this.ParentResult != null)
                {
                    return this.ParentResult.Info;
                }
                if (m_ActivityInfo == null)
                {
                    ActivityInfoCache c = new ActivityInfoCache();
                    ActivityInfoOptions t = new ActivityInfoOptions(false);
                    c.Options = t;
                    IActivity activity = this.Activity;
                    //if (this.Pauses != activity.TimerPauses)
                    //{
                    //    IActivity activity2 = Plugin.GetApplication().Logbook.Activities.AddCopy(activity);
                    //    activity = activity2;
                    //    activity.TimerPauses.Clear();
                    //    foreach (IValueRange<DateTime> p in this.Pauses)
                    //    {
                    //        activity.TimerPauses.Add(p);
                    //    }
                    //    activity.Category = Plugin.GetApplication().Logbook.ActivityCategories[1];
                    //    if (activity.Category.SubCategories.Count > 0)
                    //    {
                    //        activity.Category = activity.Category.SubCategories[0];
                    //    }
                    //}
                    m_ActivityInfo = c.GetInfo(activity);
                }
                return m_ActivityInfo;
            }
        }

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

        //Create a copy of this result as an activity
        //Incomplete right now, used to create trails from results
        public IActivity CopyToActivity()
        {
            IActivity activity = Plugin.GetApplication().Logbook.Activities.Add(this.StartDateTime);
            activity.Category = this.Activity.Category;
            activity.GPSRoute = this.GPSRoute;
            activity.TimeZoneUtcOffset = this.Activity.TimeZoneUtcOffset;
            activity.TimerPauses.Clear();
            foreach (IValueRange<DateTime> t in this.Pauses)
            {
                activity.TimerPauses.Add(t);
            }
            return activity;
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
            get { return copySmoothTrack(this.GradeTrack, true, TrailActivityInfoOptions.ElevationSmoothingSeconds,
                new Convert(UnitUtil.Grade.ConvertTo), m_cacheTrackRef);
            }
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
