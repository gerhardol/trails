/*
Copyright (C) 2009 Brendan Doherty
Copyright (C) 2011-2013 Gerhard Olsson

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
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ITrailExport;
using TrailsPlugin.Utils;
using GpsRunningPlugin.Util;

namespace TrailsPlugin.Data
{
    public class TrailResult : IComparable
    {
        #region private variables
        public ActivityTrail m_activityTrail;
        private IActivity m_activity;
        protected int m_order;
        private string m_name;
        private bool m_reverse;

        protected TrailResultInfo m_subResultInfo;

        private bool? m_includeStopped;
        private DateTime? m_startTime;
        private DateTime? m_endTime;
        private float? m_predictDistance;
        private float m_startDistance = float.NaN;
        private float m_totalDistDiff; //to give quality of results
        private ChartColors m_trailColor = null;
        private string m_toolTip;
        //Temporary? (undocumented)
        public static bool m_diffOnDateTime = false;
        public static bool diffToSelf = false;
        static public bool PaceTrackIsGradeAdjustedPaceAvg = false;

        private IValueRangeSeries<DateTime> m_pauses;
        private IDistanceDataTrack m_distanceMetersTrack;
        private IDistanceDataTrack m_activityDistanceMetersTrack;
        //private INumericTimeDataSeries m_elevationMetersTrack;
        //private INumericTimeDataSeries m_cadencePerMinuteTrack;
        //private INumericTimeDataSeries m_heartRatePerMinuteTrack;
        //private INumericTimeDataSeries m_powerWattsTrack;
        //private INumericTimeDataSeries m_speedTrack;
        //private INumericTimeDataSeries m_gradeTrack;
        private INumericTimeDataSeries m_gradeRunAdjustedTime;
        private IDistanceDataTrack m_gradeRunAdjustedTimeAvg; //data is time, not elapsed/distance
        private IList<ginfo> m_grades = new List<ginfo>();

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
        private INumericTimeDataSeries m_deviceDiffDistTrack0;
        private INumericTimeDataSeries m_deviceSpeedPaceTrack0;
        private INumericTimeDataSeries m_deviceElevationTrack0;
        private INumericTimeDataSeries m_DiffTimeTrack0;
        private INumericTimeDataSeries m_DiffDistTrack0;
        IList<float> m_trailPointTime0;
        IList<float> m_trailPointDist0;
        private double? m_ascent;
        private double? m_descent;

        private IGPSRoute m_gpsTrack;
        private IList<IList<IGPSPoint>> m_gpsPoints;

        //common for all - UR uses activities and this is a cache for resultList too
        //This could well be moved to another cache
        private static IActivity m_cacheTrackActivity;
        private static IDictionary<IActivity, IItemTrackSelectionInfo[]> m_commonStretches;
        private static ActivityInfoOptions m_TrailActivityInfoOptions;
        //internal IList<IActivity> SameTimeActivities = null;
        #endregion

        /**********************************************************/
        #region contructors
        //Parent results
        protected TrailResult(ActivityTrail activityTrail, int order, TrailResultInfo indexes, float distDiff)
        {
            createTrailResult(activityTrail, order, indexes, distDiff);
        }
        protected TrailResult(ActivityTrail activityTrail, int order, TrailResultInfo indexes, float distDiff, bool reverse)
            : this(activityTrail, order, indexes, distDiff)
        {
            this.m_reverse = reverse;
        }
        protected TrailResult(ActivityTrail activityTrail, int order, TrailResultInfo indexes, float distDiff, string toolTip)
            : this(activityTrail, order, indexes, distDiff)
        {
            this.m_toolTip = toolTip;
        }

        //Summary result
        protected TrailResult()
        {
            //a summary result is not related to an activity trail
            createTrailResult(null, 0, new TrailResultInfo(null, false), float.NaN);
            m_toolTip = "";
            m_trailColor = ColorUtil.SummaryColor;
        }

        private void createTrailResult(ActivityTrail activityTrail, int order, TrailResultInfo indexes, float distDiff)
        {
            m_activityTrail = activityTrail;
            m_activity = indexes.Activity;
            m_order = order;
            if (this is ChildTrailResult || this.m_activity == null || string.IsNullOrEmpty(this.m_activity.Name))
            {
                m_name = indexes.Name;
            }
            else
            {
                this.m_name = this.m_activity.Name;
            }
            m_subResultInfo = indexes.Copy();
            m_totalDistDiff = distDiff;
        }

#endregion

        /**********************************************************/
        #region state
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
            m_deviceSpeedPaceTrack0 = null;
            m_deviceElevationTrack0 = null;
            m_deviceDiffDistTrack0 = null;
            m_DiffTimeTrack0 = null;
            m_DiffDistTrack0 = null;
            m_trailPointTime0 = null;
            m_trailPointDist0 = null;
            m_ascent = null;
            m_descent = null;
            m_predictDistance = null;

            //smoothing control
            //m_TrailActivityInfoOptions = null;

            if (!onlyDisplay)
            {
                m_startTime = null;
                m_endTime = null;
                m_includeStopped = null;

                m_distanceMetersTrack = null;
                m_gpsPoints = null;
                m_gpsTrack = null;
                m_gradeRunAdjustedTime = null;
                //No need to explicitly clear derived, like: m_gradeRunAdjustedTimeAvg = null;

                if (!(this is ChildTrailResult) || !(this as ChildTrailResult).PartOfParent)
                {
                    m_activityDistanceMetersTrack = null;
                    //m_ActivityInfo = null;
                    m_pauses = null;
                }
            }
        }

        public static void Reset()
        {
            nextTrailColor = 0;
        }
        #endregion

        /**********************************************************/
        #region basic fields

        public virtual IActivity Activity
        {
            get { return m_activity; }
        }
        //Distance error used to sort results
        public float SortQuality
        {
            get { return (float)(m_totalDistDiff / Math.Pow(m_subResultInfo.Count, 1.5)); }
        }

        public virtual float Diff
        {
            get
            {
                if (this is ChildTrailResult && this.m_subResultInfo.Count > 0 && this.m_subResultInfo.Points[0].Time != DateTime.MinValue)
                {
                    return this.m_subResultInfo.Points[0].DistDiff;
                }
                return m_totalDistDiff;
            }
        }

        public float PointDiff(int i)
        {
            if (i < m_subResultInfo.Count)
            {
                return this.m_subResultInfo.Points[i].DistDiff;
            }
            else
            {
                return float.NaN;
            }
        }

        public int Order
        {
            get
            {
                return m_order;
            }
           set
            {
                m_order = value;
            }
        }

        public bool Reverse
        {
            get
            {
                return m_reverse;
            }
        }

        /****************************************************************/

        protected TimeSpan? m_duration = null;
        internal bool DurationIsNull() { return this.m_duration == null; }
        public virtual TimeSpan Duration
        {
            get
            {
                if (m_duration == null)
                {
                    if (!(this is ChildTrailResult) &&
                        TrailsPlugin.Data.Settings.ResultSummaryIsDevice &&
                        this.Activity != null)
                    {
                        m_duration = this.Activity.TotalTimeEntered;
                    }
                    else
                    {
                        m_duration = this.getTimeSpanResult(EndTime);
                    }
                }
                return (TimeSpan)this.m_duration;
            }
        }

        public virtual double Distance
        {
            get
            {
                if (!(this is ChildTrailResult) &&
                    TrailsPlugin.Data.Settings.ResultSummaryIsDevice &&
                    this.Activity != null)
                {
                    return this.Activity.TotalDistanceMetersEntered;
                }
                if (DistanceMetersTrack == null || DistanceMetersTrack.Count == 0)
                {
                    return 0;
                }
                return DistanceMetersTrack.Max;
            }
        }

        public TimeSpan StartTimeOfDay
        {
            get
            {
                if (!(this is ChildTrailResult) &&
                    TrailsPlugin.Data.Settings.ResultSummaryIsDevice &&
                    this.Activity != null)
                {
                    return this.Activity.StartTime.ToLocalTime().TimeOfDay;
                }
                return StartTime.ToLocalTime().TimeOfDay;
            }
        }

        public TimeSpan EndTimeOfDay
        {
            get
            {
                return EndTime.ToLocalTime().TimeOfDay;
            }
        }

        //Start/end time must match the Distance track
        public DateTime StartTime
        {
            get
            {
                if (m_startTime == null)
                {
                    if (m_subResultInfo.Points.Count == 0)
                    {
                        m_startTime = DateTime.MinValue;
                    }
                    else
                    {
                        DateTime startTime = m_subResultInfo.Points[0].Time;
                        DateTime endTime = m_subResultInfo.Points[m_subResultInfo.Points.Count - 1].Time;
                        m_startTime = TrackUtil.getFirstUnpausedTime(startTime, Pauses, true);
                        if (endTime.CompareTo((DateTime)m_startTime) <= 0)
                        {
                            //Trail (or subtrail) is completely paused. Use all
                            m_startTime = startTime;
                        }
                        if (m_startTime == DateTime.MinValue)
                        {
                            if (this.Activity == null || this.Info == null)
                            {
                                m_startTime = DateTime.MinValue;
                            }
                            else
                            {
                                m_startTime = this.Info.ActualTrackStart;
                                if (m_startTime == DateTime.MinValue)
                                {
                                    m_startTime = this.Activity.StartTime;
                                }
                            }
                        }
                    }
                }
                return (DateTime)m_startTime;
            }
        }

        public DateTime EndTime
        {
            get
            {
                if (m_endTime == null)
                {
                    if (m_subResultInfo.Points.Count == 0)
                    {
                        m_endTime = DateTime.MinValue;
                    }
                    else
                    {
                        DateTime startTime = m_subResultInfo.Points[0].Time;
                        DateTime endTime = m_subResultInfo.Points[m_subResultInfo.Points.Count - 1].Time;
                        m_endTime = TrackUtil.getFirstUnpausedTime(endTime, Pauses, false);
                        if (startTime.CompareTo((DateTime)m_endTime) >= 0)
                        {
                            //Trail (or subtrail) is completely paused. Use all
                            m_endTime = endTime;
                        }
                        if (m_endTime == DateTime.MinValue)
                        {
                            if (this.Activity == null || this.Info == null)
                            {
                                m_endTime = DateTime.MinValue;
                            }
                            else
                            {
                                m_endTime = Info.ActualTrackEnd;
                                if (m_endTime == DateTime.MinValue)
                                {
                                    m_endTime = Info.EndTime;
                                }
                            }
                        }
                    }
                }
                return (DateTime)m_endTime;
            }
        }

        //All of result including pauses/stopped is how FilteredStatistics want the info
        public IValueRangeSeries<DateTime> getSelInfo(bool excludePauses)
        {
            IValueRangeSeries<DateTime> res = new ValueRangeSeries<DateTime> { new ValueRange<DateTime>(this.StartTime, this.EndTime) };
            if (excludePauses)
            {
                res = TrailsItemTrackSelectionInfo.excludePauses(res, this.Pauses);
            }
            return res;
        }

        public virtual double StartDist
        {
            get
            {
                if (Settings.StartDistOffsetFromStartPoint &&
                    this.Activity.GPSRoute != null && this.Activity.GPSRoute.Count > 0)
                {
                    //Get offset from start point, regardless if there is a pause
                    DateTime startTime = StartTime;
                    int i = -1;
                    for (int k = 0; k < this.TrailPointDateTime.Count; k++)
                    {
                        if (this.TrailPointDateTime[k] > DateTime.MinValue)
                        {
                            //The used start time for the point, disregarding pauses
                            startTime = TrackUtil.getFirstUnpausedTime(this.TrailPointDateTime[k], new ValueRangeSeries<DateTime>(), true);
                            i = k;
                            break;
                        }
                    }
                    //Alternative, get time on track
                    //startDistance = this.ActivityDistanceMetersTrack.GetInterpolatedValue(StartDateTime).Value -
                    //    this.ActivityDistanceMetersTrack.GetInterpolatedValue(startTime).Value;
                    float startDistance = -1000; //Negative to see it in list
                    if (i >= 0 && this.m_activityTrail != null && i < this.m_subResultInfo.Count)
                    {
                        ITimeValueEntry<IGPSPoint> entry = this.Activity.GPSRoute.GetInterpolatedValue(StartTime);
                        if (entry != null)
                        {
                            startDistance = this.m_subResultInfo.Points[i].DistanceMetersToPoint(entry.Value);
                        }
                    }
                    return startDistance;
                }
                else
                {
                    if (float.IsNaN(m_startDistance))
                    {
                        getDistanceTrack();
                        if (float.IsNaN(m_startDistance))
                        {
                            m_startDistance = 0;
                        }
                    }
                }
                return m_startDistance;
            }
        }
        #endregion

        /**********************************************************/
        #region conversions
        //DateTime vs elapsed vs time (duration) for result/activity, distance result/activity conversions
        //The correct tracks must be in DistanceMetersTrack and ActivityDistanceMetersTrack

        //Get result time and distance from activity references
        public float getDistActivity(DateTime t)
        {
            return TrackUtil.getValFromDateTime(ActivityDistanceMetersTrack, t);
        }
        public float getDistResult(DateTime t)
        {
            return TrackUtil.getValFromDateTime(DistanceMetersTrack, t);
        }
        public float getDistResultFromDistActivity(double t)
        {
            return this.getDistResult(getDateTimeFromDistActivity(t));
        }

        public DateTime getDateTimeFromDistActivity(double t)
        {
            return TrackUtil.getDateTimeFromTrackDist(ActivityDistanceMetersTrack, (float)t);
        }
        public DateTime getDateTimeFromDistResult(double t)
        {
            DateTime dateTime = TrackUtil.getDateTimeFromTrackDist(DistanceMetersTrack, (float)t);
            return adjustTimeToLimits(dateTime);
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

        //Note: All Elapsed is vs result DateTime, ActivityUnpausedDistance is always distance<->DateTime

        public double getTimeResult(DateTime d)
        {
            return getTimeSpanResult(d).TotalSeconds;
        }

        private TimeSpan getTimeSpanResult(DateTime entryTime)
        {
            TimeSpan time = ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.TimeNotPaused(
                this.StartTime, entryTime, Pauses);
            return time;
        }

        public double getTimeActivity(DateTime d)
        {
            return getTimeSpanActivity(d).TotalSeconds;
        }

        private TimeSpan getTimeSpanActivity(DateTime entryTime)
        {
            if (this.Info == null)
            {
                return TimeSpan.FromSeconds(0);
            }
            TimeSpan time = ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.TimeNotPaused(
                this.Info.ActualTrackStart, entryTime, Pauses);
            return time;
        }

        //Result to activity
        public DateTime getDateTimeFromTimeResult(float t)
        {
            DateTime dateTime = ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.AddTimeAndPauses(this.StartTime, TimeSpan.FromSeconds(t), Pauses);
            return adjustTimeToLimits(dateTime);
        }

        public DateTime getDateTimeFromTimeActivity(float t)
        {
            if (this.Info == null)
            {
                return DateTime.MinValue;
            }
            return ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.AddTimeAndPauses(this.Info.ActualTrackStart, TimeSpan.FromSeconds(t), Pauses);
        }

        private DateTime adjustTimeToLimits(DateTime dateTime)
        {
            if (dateTime < this.StartTime)
            {
                dateTime = StartTime;
            }
            if (dateTime > this.EndTime)
            {
                dateTime = EndTime;
            }
            return dateTime;
        }

        //As elevation source can be configured, keep it here
        public double? getElevation(DateTime d1)
        {
            //Could use activity info cache elevation too
            double? elevation = null;
            INumericTimeDataSeries track = this.ElevationMetersTrack0(this.m_cacheTrackRef);
            if (track != null && track.Count > 1 &&
                d1 >= track.StartTime && d1 <= track.EntryDateTime(track[track.Count - 1]))
            {
                elevation = track.GetInterpolatedValue(d1).Value;
            }
            else
            {
                track = ActivityInfoCache.Instance.GetInfo(this.Activity).SmoothedElevationTrack;
                if (track != null && track.Count > 1 &&
                    d1 >= track.StartTime && d1 <= track.EntryDateTime(track[track.Count - 1]))
                {
                    elevation = track.GetInterpolatedValue(d1).Value;
                }
            }
            return elevation;
        }
        #endregion

        /**********************************************************/
        #region basic tracks
        static bool isIncludeStoppedCategory(IActivityCategory category)
        {
            if (category == null || Settings.ExcludeStoppedCategory == null || Settings.ExcludeStoppedCategory.Length == 0)
            {
                return true;
            }
            else
            {
                foreach (String name in Settings.ExcludeStoppedCategory)
                {
                    if (name.Contains(category.Name))
                    {
                        return false;
                    }
                }
            }
            return isIncludeStoppedCategory(category.Parent);
        }

        private bool IncludeStopped
        {
            get
            {
                if (m_includeStopped == null)
                {
#if ST_2_1
            // If UseEnteredData is set, exclude Stopped
            if (info.Activity.UseEnteredData == false && info.Time.Equals(info.ActualTrackTime))
            {
                m_includeStopped = true;
            }
#else
                    m_includeStopped = TrailsPlugin.Plugin.GetApplication().SystemPreferences.AnalysisSettings.IncludeStopped;
#endif
                    if ((bool)m_includeStopped)
                    {
                        m_includeStopped = isIncludeStoppedCategory(this.Category);
                    }
                }
                return (bool)m_includeStopped;
            }
        }

        /*********************************************/

        public IValueRangeSeries<DateTime> ExternalPauses
        {
            get
            {
                IValueRangeSeries<DateTime> pauses = new ValueRangeSeries<DateTime>();
                pauses.Add(new ValueRange<DateTime>(DateTime.MinValue, this.StartTime.AddSeconds(-1)));
                foreach(IValueRange<DateTime> v in this.Pauses)
                {
                    pauses.Add(v);
                }
                pauses.Add(new ValueRange<DateTime>(this.EndTime.AddSeconds(1), DateTime.MaxValue));
                return pauses;
            }
        }

        public IValueRangeSeries<DateTime> Pauses
        {
            get
            {
                if (m_pauses == null)
                {
                    if (this is SummaryTrailResult)
                    {
                        Debug.Assert(false);
                        this.m_pauses = new ValueRangeSeries<DateTime>();
                    }
                    else if (this is ChildTrailResult && (this as ChildTrailResult).PartOfParent)
                    {
                        m_pauses = (this as ChildTrailResult).ParentResult.Pauses;
                    }
                    else
                    {
                        m_pauses = new ValueRangeSeries<DateTime>();
                        IValueRangeSeries<DateTime> actPause;
                        actPause = Info.NonMovingTimes;
                        foreach (ValueRange<DateTime> t in actPause)
                        {
                            m_pauses.Add(t);
                        }

                        if (Settings.RestIsPause)
                        {
                            //Check for active laps first, if all laps are inactive show anyway
                            bool isActive = false;
                            if (this.Activity != null && this.Activity.Laps != null)
                            {
                                foreach (ILapInfo lap in Activity.Laps)
                                {
                                    if (!lap.Rest)
                                    {
                                        isActive = true;
                                        break;
                                    }
                                }
                            }
                            if (isActive)
                            {
                                for (int i = 0; i < Activity.Laps.Count; i++)
                                {
                                    ILapInfo lap = Activity.Laps[i];
                                    if (lap.Rest)
                                    {
                                        //pauses are set on next second
                                        DateTime lower = lap.StartTime;
                                        if (i == 0)
                                        {
                                            lower = lower.AddSeconds(-1);
                                        }
                                        DateTime upper;
                                        if (i < Activity.Laps.Count - 1)
                                        {
                                            upper = Activity.Laps[i + 1].StartTime;
                                            if (!Activity.Laps[i + 1].Rest)
                                            {
                                                upper=upper.AddSeconds(-1);
                                            }
                                            //Fix: Lap start time is in seconds, precision could be lost
                                            DateTime upper2 = lower.Add(lap.TotalTime);
                                            if (upper.Millisecond == 0 && Math.Abs((upper2 - upper).TotalSeconds) < 2)
                                            {
                                                upper = upper2;
                                            }
                                        }
                                        else
                                        {
                                            upper = this.Info.EndTime.AddSeconds(1);
                                        }
                                        this.m_pauses.Add(new ValueRange<DateTime>(lower, upper));
                                    }
                                }
                            }
                        }
                        if (Settings.RestIsPause)
                        {
                            //Non required trail points
                            for (int i = 0; i < this.m_subResultInfo.Points.Count - 1; i++)
                            {
                                if (i < this.m_subResultInfo.Points.Count &&
                                    !this.m_subResultInfo.Points[i].Required &&
                                    this.TrailPointDateTime[i] > DateTime.MinValue)
                                {
                                    DateTime lower = this.TrailPointDateTime[i];
                                    DateTime upper = this.EndTime;
                                    while (i < this.TrailPointDateTime.Count &&
                                        i < this.m_subResultInfo.Points.Count &&
                                        (!this.m_subResultInfo.Points[i].Required ||
                                        this.TrailPointDateTime[i] == DateTime.MinValue))
                                    {
                                        i++;
                                    }
                                    if (i < this.TrailPointDateTime.Count &&
                                        this.TrailPointDateTime[i] > DateTime.MinValue)
                                    {
                                        upper = this.TrailPointDateTime[i];
                                    }
                                    m_pauses.Add(new ValueRange<DateTime>(lower, upper));
                                }
                            }
                            //IList<TrailGPSLocation> trailLocations = m_activityTrail.Trail.TrailLocations;
                            //if (m_activityTrail.Trail.IsSplits)
                            //{
                            //    trailLocations = Trail.TrailGpsPointsFromSplits(this.m_activity);
                            //}
                            //for (int i = 0; i < this.TrailPointDateTime.Count - 1; i++)
                            //{
                            //    if (i < trailLocations.Count &&
                            //        !trailLocations[i].Required &&
                            //        this.TrailPointDateTime[i] > DateTime.MinValue)
                            //    {
                            //        DateTime lower = this.TrailPointDateTime[i];
                            //        DateTime upper = this.EndTime;
                            //        while (i < this.TrailPointDateTime.Count &&
                            //            i < trailLocations.Count &&
                            //            (!trailLocations[i].Required ||
                            //            this.TrailPointDateTime[i] == DateTime.MinValue))
                            //        {
                            //            i++;
                            //        }
                            //        if (i < this.TrailPointDateTime.Count &&
                            //            this.TrailPointDateTime[i] > DateTime.MinValue)
                            //        {
                            //            upper = this.TrailPointDateTime[i];
                            //        }
                            //        m_pauses.Add(new ValueRange<DateTime>(lower, upper));
                            //    }
                            //}
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
                    //ExternalPauses
                    //m_pauses.Add(new ValueRange<DateTime>(DateTime.MinValue, this.StartTime.AddSeconds(-1)));
                    //m_pauses.Add(new ValueRange<DateTime>(this.EndTime.AddSeconds(1), DateTime.MaxValue));
                }
                return m_pauses;
            }
        }

        /*********************************************/
        //Distance
        private void getDistanceTrack()
        {
            if (null == m_distanceMetersTrack || null == m_activityDistanceMetersTrack || float.IsNaN(m_startDistance))
            {
                m_distanceMetersTrack = new DistanceDataTrack();
                m_distanceMetersTrack.AllowMultipleAtSameTime = false;
                if (this is SummaryTrailResult)
                {
                    Debug.Assert(false);
                    m_activityDistanceMetersTrack = m_distanceMetersTrack;
                    return;
                }
                if (this is ChildTrailResult && (this as ChildTrailResult).PartOfParent)
                {
                    m_activityDistanceMetersTrack = (this as ChildTrailResult).ParentResult.ActivityDistanceMetersTrack;
                    //m_distanceMetersTrack could be created from parent too (if second rounding is disregarded),
                    //but it is simpler and not heavier to use same code-path as parent
                }
                else
                {
                    m_activityDistanceMetersTrack = Info.MovingDistanceMetersTrack;
                    //There are occasional 0 or decreasing distance values, that disrupt insertion
                    if (this.m_activityDistanceMetersTrack.Count > 0)
                    {
                        for (int i = 1; i < this.m_activityDistanceMetersTrack.Count; i++)
                        {
                            if (this.m_activityDistanceMetersTrack[i].Value < this.m_activityDistanceMetersTrack[i - 1].Value)
                            {
                                this.m_activityDistanceMetersTrack.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                    m_activityDistanceMetersTrack.AllowMultipleAtSameTime = false;
                    if (m_activityDistanceMetersTrack != null && this.m_activityDistanceMetersTrack.Count > 0)
                    {
                        //insert points at borders in m_activityDistanceMetersTrack
                        //Less special handling when transversing the activity track
                        (new InsertValues<float>(this)).
                            insertValues(m_activityDistanceMetersTrack, m_activityDistanceMetersTrack);
                        //TrackUtil.ResortTrack<float>(m_activityDistanceMetersTrack);
                    }
                }
                if (m_activityDistanceMetersTrack != null && m_activityDistanceMetersTrack.Count > 0)
                {
                    int i = 0;
                    float distance = 0;
                    uint oldElapsed = 0;

                    while (i < m_activityDistanceMetersTrack.Count &&
                        this.StartTime >= m_activityDistanceMetersTrack.EntryDateTime(m_activityDistanceMetersTrack[i]))
                    {
                        i++;
                    }

                    //Add a point at result start, also if no Dist point is set
                    this.m_distanceMetersTrack.Add(this.StartTime, distance);
                    this.m_startDistance = TrackUtil.getValFromDateTimeIndex(this.m_activityDistanceMetersTrack, this.StartTime, i);

                    float prevDist = this.m_startDistance;
                    while (i < this.m_activityDistanceMetersTrack.Count &&
                        this.EndTime > this.m_activityDistanceMetersTrack.EntryDateTime(this.m_activityDistanceMetersTrack[i]))
                    {
                        ITimeValueEntry<float> timeValue = m_activityDistanceMetersTrack[i];
                        DateTime dateTime = m_activityDistanceMetersTrack.EntryDateTime(timeValue);
                        if (!ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(dateTime, this.Pauses))
                        {
                            uint elapsed = timeValue.ElapsedSeconds;
                            if (elapsed > oldElapsed || oldElapsed == 0)
                            {
                                float actDist = timeValue.Value;
                                if (!float.IsNaN(prevDist))
                                {
                                    //It is possible to get the offsets at boundaries, instead of inserting values
                                    distance += actDist - prevDist;
                                }
                                //Adding max one entry per truncated second
                                m_distanceMetersTrack.Add(dateTime, distance);
                                prevDist = actDist;
                                oldElapsed = elapsed;
                            }
                        }
                        else
                        {
                            prevDist = float.NaN;
                        }
                        i++;
                    }

                    //Set real last distance, override if elapsedSec is set
                    if (!float.IsNaN(prevDist))
                    {
                        float dist = TrackUtil.getValFromDateTimeIndex(this.m_activityDistanceMetersTrack, this.EndTime, i);
                        distance += dist - prevDist;
                        TrackUtil.trackAdd(this.m_distanceMetersTrack, this.EndTime, distance);
                    }
                }
            }
        }

        //Distance track for trail, adjusted with pauses
        internal IDistanceDataTrack DistanceMetersTrack
        {
            get
            {
                getDistanceTrack();
                return m_distanceMetersTrack;
            }
        }

        //Distance track as ST sees it. Used in conversions
        private IDistanceDataTrack ActivityDistanceMetersTrack
        {
            get
            {
                getDistanceTrack();
                return m_activityDistanceMetersTrack;
            }
        }
        #endregion

        /**********************************************************/
        #region fields

        public TrailResultInfo SubResultInfo
        {
            get
            {
                return m_subResultInfo;
            }
        }

        public IList<DateTime> TrailPointDateTime
        {
            get
            {
                return m_subResultInfo.CopyTime();
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
                else if (this.Activity != null)
                {
                    string s = string.Format("{0} {1} {2}", this.StartTime.ToLocalTime(), Activity.Name, Activity.Notes.Substring(0, Math.Min(Activity.Notes.Length, 40)));
                    if (m_diffOnDateTime)
                    {
                        s += " " + Activity.Metadata.Source;
                    }
                    return s;
                }
                return "";
            }
        }

        public virtual float AvgCadence
        {
            get
            {
                if (!(this is ChildTrailResult) &&
                    TrailsPlugin.Data.Settings.ResultSummaryIsDevice &&
                    this.Activity != null)
                {
                    return this.Activity.AverageCadencePerMinuteEntered;
                }
                INumericTimeDataSeries track = CadencePerMinuteTrack0(m_cacheTrackRef);
                if (track == null || track.Count == 0)
                {
                    if (this.Info == null)
                    {
                        return float.NaN;
                    }
                    return Info.AverageCadence;
                }
                return track.Avg;
            }
        }

        public virtual float AvgHR
        {
            get
            {
                if (!(this is ChildTrailResult) &&
                    TrailsPlugin.Data.Settings.ResultSummaryIsDevice &&
                    this.Activity != null)
                {
                    return this.Activity.AverageHeartRatePerMinuteEntered;
                }
                INumericTimeDataSeries track = HeartRatePerMinuteTrack0(m_cacheTrackRef);
                if (track == null || track.Count == 0)
                {
                    if (this.Info == null)
                    {
                        return float.NaN;
                    }
                    return Info.AverageHeartRate;
                }
                return track.Avg;
            }
        }

        public virtual float MaxHR
        {
            get
            {
                if (!(this is ChildTrailResult) &&
                    TrailsPlugin.Data.Settings.ResultSummaryIsDevice &&
                    this.Activity != null)
                {
                    return this.Activity.MaximumHeartRatePerMinuteEntered;
                }
                INumericTimeDataSeries track = HeartRatePerMinuteTrack0(m_cacheTrackRef);
                if (track == null || track.Count == 0)
                {
                    if (this.Info == null)
                    {
                        return float.NaN;
                    }
                    return Info.MaximumHeartRate;
                }
                return track.Max;
            }
        }

        public virtual float AvgPower
        {
            get
            {
                if (!(this is ChildTrailResult) &&
                    TrailsPlugin.Data.Settings.ResultSummaryIsDevice &&
                    this.Activity != null)
                {
                    return this.Activity.AveragePowerWattsEntered;
                }
                float result;
                INumericTimeDataSeries track = PowerWattsTrack0(m_cacheTrackRef);
                if (track == null || track.Count == 0)
                {
                    if (this.Info == null)
                    {
                        return float.NaN;
                    }
                    result = Info.AveragePower;
                }
                else
                {
                    result = track.Avg;
                }
                return (float)UnitUtil.Power.ConvertTo(result, m_cacheTrackRef.Activity);
            }
        }

        public virtual float AscAvgGrade
        {
            get
            {
                if (!(this is ChildTrailResult) &&
                    TrailsPlugin.Data.Settings.ResultSummaryIsDevice &&
                    this.Activity != null &&
                    this.Activity.TotalDistanceMetersEntered > 0)
                {
                    return this.Activity.TotalAscendMetersEntered / this.Activity.TotalDistanceMetersEntered;
                }
                //Just Ascending instead of track average, more useful
                return (float)(this.Ascent / this.Distance);
            }
        }

        public virtual float DescAvgGrade
        {
            get
            {
                if (!(this is ChildTrailResult) &&
                    TrailsPlugin.Data.Settings.ResultSummaryIsDevice &&
                    this.Activity != null &&
                    this.Activity.TotalDistanceMetersEntered > 0)
                {
                    return this.Activity.TotalDescendMetersEntered / this.Activity.TotalDistanceMetersEntered;
                }
                return (float)(this.Descent / this.Distance);
            }
        }

        public virtual float AvgSpeed
        {
            get
            {
                if (!(this is ChildTrailResult) &&
                    TrailsPlugin.Data.Settings.ResultSummaryIsDevice &&
                    this.Activity != null &&
                    this.Activity.TotalTimeEntered.TotalSeconds > 0)
                {
                    return (float)(this.Activity.TotalDistanceMetersEntered / this.Activity.TotalTimeEntered.TotalSeconds);
                }
                return (float)(this.Distance / this.Duration.TotalSeconds);
            }
        }

        public virtual float FastestSpeed
        {
            get
            {
                checkCacheRef(m_cacheTrackRef);
                return (float)UnitUtil.Speed.ConvertTo(this.SpeedTrack0(m_cacheTrackRef).Max, m_cacheTrackRef.Activity);
            }
        }

        //Smoothing could differ speed/pace, why this is separate
        public virtual double FastestPace
        {
            get
            {
                checkCacheRef(m_cacheTrackRef);
                return UnitUtil.Pace.ConvertTo(this.PaceTrack0(m_cacheTrackRef).Min, m_cacheTrackRef.Activity);
            }
        }

        public virtual double Ascent
        {
            get
            {
                if (!(this is ChildTrailResult) &&
                    TrailsPlugin.Data.Settings.ResultSummaryIsDevice &&
                    this.Activity != null)
                {
                    return this.Activity.TotalAscendMetersEntered;
                }
                if (m_ascent == null)
                {
                    GetTotalClimbValue();
                }
                return (double)m_ascent;
            }
        }

        public virtual double Descent
        {
            get
            {
                if (!(this is ChildTrailResult) &&
                    TrailsPlugin.Data.Settings.ResultSummaryIsDevice &&
                    this.Activity != null)
                {
                    return this.Activity.TotalDescendMetersEntered;
                }
                if (m_descent == null)
                {
                    GetTotalClimbValue();
                }
                return (double)m_descent;
            }
        }

        //Calculate ascent/descent for the trail
        public void GetTotalClimbValue()
        {
            //ActivityInfoCache.Instance.GetInfo(this.Activity).SmoothedGradeTrack;// 
            INumericTimeDataSeries track = this.GradeTrack0(m_cacheTrackRef);
            if (track == null || track.Count == 0)
            {
                if (this.Info == null)
                {
                    m_ascent = double.NaN;
                    m_descent = double.NaN;
                }
                else
                {
                    m_ascent = Info.TotalAscendingMeters(Plugin.GetApplication().DisplayOptions.SelectedClimbZone);
                    m_descent = Info.TotalDescendingMeters(Plugin.GetApplication().DisplayOptions.SelectedClimbZone);
                }
            }
            else
            {
                m_ascent = 0;
                m_descent = 0;
                //Note: Using Trails Info inflates the values slightly, also standard Info does a little (due to points added)
                //Attempt to adjust factor, may need tuning
                ActivityInfo info = this.Info;
                if (info != null)
                {
                    ZoneCategoryInfo categoryInfo = ZoneCategoryInfo.Calculate(info, Plugin.GetApplication().DisplayOptions.SelectedClimbZone, track, 0.8f);

                    // Remove totals row
                    categoryInfo.Zones.RemoveAt(categoryInfo.Zones.Count - 1);

                    foreach (ZoneInfo zoneInfo in categoryInfo.Zones)
                    {
                        if (zoneInfo.Zone.Low > 0 && zoneInfo.ElevationChangeMeters > 0)
                        {
                            m_ascent += zoneInfo.ElevationChangeMeters;
                        }
                        if (zoneInfo.Zone.High < 0 && zoneInfo.ElevationChangeMeters < 0)
                        {
                            m_descent += zoneInfo.ElevationChangeMeters;
                        }
                    }
                }
            }
        }

        public virtual double ElevChg
        {
            get
            {
                float value = 0;
                checkCacheRef(this.m_cacheTrackRef);
                INumericTimeDataSeries elevationTrack = CalcElevationMetersTrack0(this.m_cacheTrackRef);
                if (elevationTrack != null && elevationTrack.Count > 1)
                {
                    value = elevationTrack[elevationTrack.Count - 1].Value - elevationTrack[0].Value;
                    if (!UnitUtil.Elevation.isDefaultUnit(this.m_cacheTrackRef.Activity))
                    {
                        //Already in display unit, convert back
                        value = (float)UnitUtil.Elevation.ConvertTo(value, this.m_cacheTrackRef.Activity);
                    }
                }
                return value;
            }
        }

        public virtual double VAM //Ascending speed
        {
            get
            {
                return this.ElevChg / this.Duration.TotalHours;
            }
        }

        public IActivityCategory Category
        {
            get
            {
                if (m_activity == null)
                {
                    return null;
                }
                return m_activity.Category;
            }
        }

        public float PredictDistance
        {
            get
            {
                if (m_predictDistance == null)
                {
                    if (TrailsPlugin.Integration.PerformancePredictor.PerformancePredictorIntegrationEnabled)
                    {
                        IList<TrailsPlugin.Integration.PerformancePredictor.PerformancePredictorResult> t =
                            TrailsPlugin.Integration.PerformancePredictor.PerformancePredictorFields(
                            new List<IActivity> { this.Activity }, new List<double> { this.Duration.TotalSeconds }, new List<double> { this.Distance }, new List<double> { Settings.PredictDistance }, null);
                        if (t != null && t.Count > 0 && t[0] != null)
                        {
                            m_predictDistance = (float)t[0].predicted[0].new_time;
                        }
                    }
                    if (m_predictDistance == null)
                    {
                        m_predictDistance = float.NaN;
                    }
                }
                return (float)m_predictDistance;
            }
        }
#endregion

        /**********************************************************/
        #region tracks
        private bool checkCacheRef(TrailResult refRes)
        {
            if (refRes == null || refRes != this.m_cacheTrackRef)
            {
                //Clear cache where ref (possibly null) has been used
                Clear(true);
                m_cacheTrackRef = refRes;
                if (m_cacheTrackRef == null)
                {
                    //A reference is needed to set for instance display format
                    m_cacheTrackRef = this;
                }

                return true;
            }
            return false;
        }

        private static bool checkCacheAct(IActivity refAct)
        {
            if (refAct == null || refAct != m_cacheTrackActivity)
            {
                m_cacheTrackActivity = refAct;
                m_commonStretches = null;

                return true;
            }
            return false;
        }

        //copy the relevant part to a new track
        public INumericTimeDataSeries copyTrailTrack(INumericTimeDataSeries source)
        {
            return copySmoothTrack(source, false, 0, new Convert(ConvertNone), m_cacheTrackRef);
        }

        //Convert to/from internal format to display format
        public delegate double Convert(double value, IActivity activity);
        //Empty definition, when no conversion needed
        public static double ConvertNone(double p, IActivity activity)
        {
            return p;
        }

        private INumericTimeDataSeries SmoothTrack(INumericTimeDataSeries track, int smooth)
        {
            if (smooth > 0)
            {
                float min; float max;
                if (Settings.SmoothOverTrailPoints == SmoothOverTrailBorders.All || m_subResultInfo.Points.Count <= 2)
                {
                    track = ZoneFiveSoftware.Common.Data.Algorithm.NumericTimeDataSeries.Smooth(track, smooth, out min, out max);
                }
                else
                {
                    //Smooth individual trail sections individually
                    int pointIndex = 0;
                    INumericTimeDataSeries tTrack = new NumericTimeDataSeries();
                    DateTime pTime = DateTime.MinValue;
                    for (int i = 0; i < track.Count; i++)
                    {
                        DateTime t = track.EntryDateTime(track[i]);
                        int addOffset = 0;
                        if (t <= pTime || i >= track.Count - 1 || i == 0)
                        {
                            tTrack.Add(t, track[i].Value);
                        }
                        else
                        {
                            //When updating, do not change this point
                            addOffset = 1;
                        }

                       //End of section
                        if ((i >= track.Count - 1 || t > pTime) && tTrack.Count > 0 && i > 0)
                        {
                            tTrack = ZoneFiveSoftware.Common.Data.Algorithm.NumericTimeDataSeries.Smooth(tTrack, smooth, out min, out max);
                            for (int j = 0; j < tTrack.Count; j++)
                            {
                                int k = i - addOffset - tTrack.Count + 1 + j;
                                if (k < track.Count)
                                {
                                    track.SetValueAt(k, tTrack[j].Value);
                                }
                                else
                                { //TBD: This should not happen, but only affects some smoothing
                                }
                            }
                            tTrack = new NumericTimeDataSeries();
                            if (addOffset > 0)
                            {
                                //This point was not added in last smooth
                                tTrack.Add(t, track[i].Value);
                            }
                        }
                        //Increase point index
                        //Do not care about last trail point, last section always smoothed
                        int nIndex = pointIndex;
                        bool unchangedBorder = Settings.SmoothOverTrailPoints == SmoothOverTrailBorders.Unchanged;
                        while ((pointIndex < m_subResultInfo.Points.Count) &&
                            (pTime <= DateTime.MinValue || //No match with current, handle w next
                            t > pTime && nIndex == pointIndex || //No match with current, handle w next
                            //Advance if next (and all in between) is same as start
                                unchangedBorder && nIndex < pointIndex))
                        {
                            pointIndex++;
                            if (pointIndex < m_subResultInfo.Points.Count)
                            {
                                pTime = m_subResultInfo.Points[pointIndex].Time;
                                unchangedBorder &= m_subResultInfo.Points[nIndex].Required == m_subResultInfo.Points[pointIndex].Required;
                            }
                        }
                    }
                }
            }
            return track;
        }

        /// <summary>
        /// Copy a track, trim and apply smoothing
        /// </summary>
        /// <param name="source"></param>
        /// <param name="insert"></param>
        /// <param name="smooth"></param>
        /// <param name="convert"></param>
        /// <param name="refRes"></param>
        /// <returns></returns>

        public INumericTimeDataSeries copySmoothTrack(INumericTimeDataSeries source, bool insert, int smooth, Convert convert, TrailResult refRes)
        {
            return copySmoothTrack(source, insert, true, smooth, convert, refRes);
        }

        public INumericTimeDataSeries copySmoothTrack(INumericTimeDataSeries source, bool insert, bool trimToResult, int smooth, Convert convert, TrailResult refRes)
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
                DateTime startTime;
                DateTime endTime;
                IValueRangeSeries<DateTime> pauses;
                if (trimToResult)
                {
                    //ST only uses whole seconds in graphs
                    //Set start/end to whole second match, to get easier comparisions
                    startTime = this.StartTime.AddMilliseconds(-this.StartTime.Millisecond);
                    pauses = this.Pauses;
                    endTime = this.EndTime.AddMilliseconds(1000-this.EndTime.Millisecond);
                }
                else if (this.Activity != null && this.Info != null)
                {
                    //(normally)elevation handling, trim to activity
                    //startTime = this.Activity.StartTime;
                    startTime = this.Info.ActualTrackStart.AddMilliseconds(-this.Info.ActualTrackStart.Millisecond);
                    pauses = this.Activity.TimerPauses;
                    //endTime = ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.AddTimeAndPauses(startTime, this.Activity.TotalTimeEntered, pauses);
                    endTime = this.Info.ActualTrackEnd.AddMilliseconds(1000 - this.Info.ActualTrackEnd.Millisecond);
                }
                else
                {
                    //should not occur
                    startTime = DateTime.MinValue;
                    pauses = new ValueRangeSeries<DateTime>();
                    endTime = DateTime.MaxValue;
                }

                int oldElapsed = int.MinValue;
                foreach (ITimeValueEntry<float> t in source)
                {
                    DateTime dateTime = source.EntryDateTime(t);
                    if (startTime <= dateTime && dateTime <= endTime &&
                        !ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(dateTime, pauses))
                    {
                        uint elapsed = t.ElapsedSeconds;
                        if (elapsed > oldElapsed)
                        {
                            track.Add(dateTime, (float)convert(t.Value, refActivity));
                            oldElapsed = (int)elapsed;
                        }
                    }
                    if (dateTime >= this.EndTime)
                    {
                        break;
                    }
                }

                if (insert)
                {
                    //Insert values around borders, to limit effects when track is chopped
                    //Do this before other additions, so start is StartTime for track
                    InsertValues<float> iv;
                    if (trimToResult)
                    {
                        iv = new InsertValues<float>(this);
                    }
                    else
                    {
                        iv = new InsertValues<float>(startTime, endTime, pauses);
                    }
                    iv.insertValues(track, source);
                }
                else
                {
                    //ST bug: track could be out of order (due to insert at least)
                    TrackUtil.ResortTrack(track);
                }

                track = SmoothTrack(track, smooth);
            }
            return track;
        }


        public IDistanceDataTrack DistanceMetersTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_distanceMetersTrack0 == null)
            {
                //Just copy the track. No smoothing or inserting of values
                this.m_distanceMetersTrack0 = new DistanceDataTrack();
                foreach (ITimeValueEntry<float> entry in this.DistanceMetersTrack)
                {
                    float val = (float)UnitUtil.Distance.ConvertFrom(entry.Value, m_cacheTrackRef.Activity);
                    if (!float.IsNaN(val))
                    {
                        DateTime dateTime = this.DistanceMetersTrack.EntryDateTime(entry);
                        this.m_distanceMetersTrack0.Add(dateTime, val);
                    }
                }
            }
            return this.m_distanceMetersTrack0;
        }

        public INumericTimeDataSeries CalcElevationMetersTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            INumericTimeDataSeries etrack = null;
            if (TrailsPlugin.Data.Settings.UseDeviceElevationForCalc)
            {
                etrack = this.DeviceElevationTrack0(this.m_cacheTrackRef);
            }
            if (etrack == null || etrack.Count == 0)
            {
                return this.ElevationMetersTrack0(this.m_cacheTrackRef);
            }
            return etrack;
        }

        public INumericTimeDataSeries ElevationMetersTrack0()
        {
            return this.ElevationMetersTrack0(this.m_cacheTrackRef);
        }
        public INumericTimeDataSeries ElevationMetersTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_elevationMetersTrack0 == null)
            {
                m_elevationMetersTrack0 = copySmoothTrack(this.ElevationMetersTrack, true, TrailActivityInfoOptions.ElevationSmoothingSeconds,
                    new Convert(UnitUtil.Elevation.ConvertFrom), this.m_cacheTrackRef);
            }
            return m_elevationMetersTrack0;
        }

        public static bool CalculateGrade = false;
        public INumericTimeDataSeries GradeTrack0()
        {
            return this.copySmoothTrack(this.GradeTrack, true, TrailActivityInfoOptions.ElevationSmoothingSeconds,
                new Convert(UnitUtil.Grade.ConvertTo), this.m_cacheTrackRef);
            //TBD return this.GradeTrack0(this.m_cacheTrackRef);
        }
        public INumericTimeDataSeries GradeTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_gradeTrack0 == null)
            {
                m_gradeTrack0 = copySmoothTrack(this.GradeTrack, true, TrailActivityInfoOptions.ElevationSmoothingSeconds,
                        new Convert(UnitUtil.Grade.ConvertFrom), this.m_cacheTrackRef);

                if (CalculateGrade && this.Activity.CadencePerMinuteTrack == null)
                {
                    //Some temporary handling of Grade. The standard ST Grade smooth grade after finding grade which smooth too much
                    //Use smoothed elevation instead should give better representaion, but it seems like to need some combination
                    //Put in cadence track, to compare for now
                    //Use smoothed Elevation track, then calc grade
                    float pDist = 0;
                    float pEle = 0;
                    double pTime = 0;
                    bool first = true;
                    INumericTimeDataSeries track = this.CalcElevationMetersTrack0(this.m_cacheTrackRef);
                    m_cadencePerMinuteTrack0 = new NumericTimeDataSeries();
                    foreach (ITimeValueEntry<float> entry in track)
                    {
                        DateTime d = track.EntryDateTime(entry);
                        float dist = this.getDistResult(d);
                        float ele = entry.Value;
                        double time = this.getTimeResult(d);
                        if (!first)
                        {
                            first = false;
                            if (dist > pDist && time > pTime)
                            {
                                //TBD slow speed smooth
                                m_cadencePerMinuteTrack0.Add(d, 100 * (ele - pEle) / (dist - pDist));
                            }
                        }
                        first = false;
                        pDist = dist;
                        pEle = ele;
                        pTime = time;

                    }
                }
            }
            return m_gradeTrack0;
        }

        public INumericTimeDataSeries CadencePerMinuteTrack0()
        {
            return this.CadencePerMinuteTrack0(this.m_cacheTrackRef);
        }
        public INumericTimeDataSeries CadencePerMinuteTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_cadencePerMinuteTrack0 == null)
            {
                m_cadencePerMinuteTrack0 = copySmoothTrack(this.CadencePerMinuteTrack, true, TrailActivityInfoOptions.CadenceSmoothingSeconds,
                     new Convert(UnitUtil.Cadence.ConvertFrom), this.m_cacheTrackRef);
            }
            return m_cadencePerMinuteTrack0;
        }

        public INumericTimeDataSeries HeartRatePerMinuteTrack0()
        {
            return this.HeartRatePerMinuteTrack0(this.m_cacheTrackRef);
        }
        public INumericTimeDataSeries HeartRatePerMinuteTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_heartRatePerMinuteTrack0 == null)
            {
                m_heartRatePerMinuteTrack0 = copySmoothTrack(this.HeartRatePerMinuteTrack, true, TrailActivityInfoOptions.HeartRateSmoothingSeconds,
                    new Convert(UnitUtil.HeartRate.ConvertFrom), this.m_cacheTrackRef);
            }
            return m_heartRatePerMinuteTrack0;
        }

        public INumericTimeDataSeries PowerWattsTrack0()
        {
            return this.PowerWattsTrack0(this.m_cacheTrackRef);
        }
        public INumericTimeDataSeries PowerWattsTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_powerWattsTrack0 == null)
            {
                m_powerWattsTrack0 = copySmoothTrack(this.PowerWattsTrack, true, TrailActivityInfoOptions.PowerSmoothingSeconds,
                    new Convert(UnitUtil.Power.ConvertFrom), this.m_cacheTrackRef);
            }
            return m_powerWattsTrack0;
        }

        public INumericTimeDataSeries SpeedTrack0()
        {
            return this.SpeedTrack0(this.m_cacheTrackRef);
        }
        public INumericTimeDataSeries SpeedTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if ((this is ChildTrailResult) && (this as ChildTrailResult).PartOfParent)
            {
                m_speedTrack0 = copySmoothTrack((this as ChildTrailResult).ParentResult.SpeedTrack0(this.m_cacheTrackRef), false, 0, ConvertNone, this.m_cacheTrackRef);
                return m_speedTrack0;
            }
            if (m_speedTrack0 == null)
            {
                m_speedTrack0 = copySmoothTrack(this.SpeedTrack, true, TrailActivityInfoOptions.SpeedSmoothingSeconds,
                                 new Convert(UnitUtil.Speed.ConvertFrom), this.m_cacheTrackRef);
            }
            return m_speedTrack0;
        }

        public INumericTimeDataSeries PaceTrack0()
        {
            return this.PaceTrack0(this.m_cacheTrackRef);
        }
        public INumericTimeDataSeries PaceTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if ((this is ChildTrailResult) && (this as ChildTrailResult).PartOfParent)
            {
                m_paceTrack0 = copySmoothTrack((this as ChildTrailResult).ParentResult.PaceTrack0(this.m_cacheTrackRef), false, 0, ConvertNone, this.m_cacheTrackRef);
                return m_paceTrack0;
            }
            if (m_paceTrack0 == null)
            {
                INumericTimeDataSeries speedTrack;
                if (Settings.RunningGradeAdjustMethod == RunningGradeAdjustMethodEnum.None)
                {
                    speedTrack = this.SpeedTrack;
                }
                else
                {
                    //grade adjusted track
                    speedTrack = new NumericTimeDataSeries();
                    if (!TrailResult.PaceTrackIsGradeAdjustedPaceAvg)
                    {
                        //Show pace adjusted to flat ground
                        this.calcGradeRunAdjustedTime(this.m_cacheTrackRef);
                        if (this.m_grades.Count > 1)
                        {
                            foreach (ginfo t in m_grades)
                            {
                                if (!float.IsNaN(t.adjSpeed))
                                {
                                    speedTrack.Add(t.dateTime, t.adjSpeed);
                                }
                            }
                        }
                    }
                    else
                    {
                        //Show pace as it should have been with "even pace" (the adjusted pace will not give same "total time")
                        //Undocumented
                        INumericTimeDataSeries timeTrack = this.GradeRunAdjustedTimeTrack(this.m_cacheTrackRef);
                        if (this.m_grades.Count > 1)
                        {
                            //No data in first point - could use dummy
                            for (int i = 1; i < timeTrack.Count; i++)
                            {
                                ginfo t = this.m_grades[i];
                                speedTrack.Add(t.dateTime, t.dist / (timeTrack[i].Value - timeTrack[i - 1].Value));
                            }
                        }
                    }
                    if (speedTrack != null && speedTrack.Count > 2)
                    {
                        //Distance calculation is not always "stable" at start/end, partly due to rounding of seconds.
                        //Just one point with lets say 10m instead of 5m will offset the smoothing
                        //To minimize this, remove the points
                        const int minCheck = 10;
                        if (speedTrack[speedTrack.Count - 1].ElapsedSeconds - speedTrack[speedTrack.Count - 2].ElapsedSeconds < minCheck)
                        {
                            speedTrack.RemoveAt(speedTrack.Count - 1);
                        }
                        if (speedTrack[1].ElapsedSeconds - speedTrack[0].ElapsedSeconds < minCheck)
                        {
                            speedTrack.RemoveAt(0);
                        }
                    }
                }
                if (speedTrack != null && speedTrack.Count > 1)
                {
                    //Smooth speed track, as smoothing pace gives incorrect data (when fast is close to slow)
                    this.m_paceTrack0 = copySmoothTrack(speedTrack, true, TrailActivityInfoOptions.SpeedSmoothingSeconds,
                                    new Convert(ConvertNone), this.m_cacheTrackRef);
                    for (int i = 0; i < this.m_paceTrack0.Count; i++)
                    {
                        this.m_paceTrack0.SetValueAt(i, (float)UnitUtil.Pace.ConvertFrom(m_paceTrack0[i].Value, this.m_cacheTrackRef.Activity));
                    }
                }
                else
                {
                    this.m_paceTrack0 = new NumericTimeDataSeries();
                }
            }
            return this.m_paceTrack0;
        }

        public static IDictionary<IActivity, IItemTrackSelectionInfo[]> CommonStretches(IActivity refAct, IList<IActivity> acts, System.Windows.Forms.ProgressBar progressBar)
        {
            checkCacheAct(refAct);
            if (null == m_commonStretches)
            {
                m_commonStretches = new Dictionary<IActivity, IItemTrackSelectionInfo[]>();
            }
            IList<IActivity> acts2 = new List<IActivity>();
            foreach (IActivity act in acts)
            {
                if (!m_commonStretches.ContainsKey(act))
                {
                    acts2.Add(act);
                }
            }
            if (acts2.Count > 0)
            {
                IDictionary<IActivity, IItemTrackSelectionInfo[]> commonStretches =
                    TrailsPlugin.Integration.UniqueRoutes.GetCommonStretchesForActivity(refAct, acts2, null);
                foreach (IActivity act in acts2)
                {
                    m_commonStretches.Add(act, commonStretches[act]);
                }
            }
            //Return all...
            return m_commonStretches;
        }

        #endregion

        /**********************************************************/
        #region device tracks
        public INumericTimeDataSeries DeviceSpeedPaceTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_deviceSpeedPaceTrack0 == null)
            {
                if ((this is ChildTrailResult) && (this as ChildTrailResult).PartOfParent)
                {
                    m_deviceSpeedPaceTrack0 = copySmoothTrack((this as ChildTrailResult).ParentResult.DeviceSpeedPaceTrack0(this.m_cacheTrackRef), false, 0, ConvertNone, this.m_cacheTrackRef);
                    return m_deviceSpeedPaceTrack0;
                }
                bool isPace = this.m_cacheTrackRef.Activity.Category.SpeedUnits.Equals(Speed.Units.Pace);
                //About the same as copySmoothTrack()
                m_deviceSpeedPaceTrack0 = new NumericTimeDataSeries();
                if (this.Activity != null && this.Activity.DistanceMetersTrack != null && this.Activity.DistanceMetersTrack.Count > 0)
                {
                    ITimeValueEntry<float> prev = null;
                    IDistanceDataTrack source = this.Activity.DistanceMetersTrack;
                    InsertValues<float> iv = new InsertValues<float>(this);
                    iv.insertValues(source, source);
                    //TrackUtil.ResortTrack<float>(source);

                    foreach (ITimeValueEntry<float> t in source)
                    {
                        if (t != null)
                        {
                            DateTime dateTime = source.EntryDateTime(t);
                            if (prev != null &&
                                this.StartTime <= dateTime && dateTime <= this.EndTime &&
                                !ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(dateTime, this.Pauses))
                            {
                                //Ignore 0 time and infinity pace
                                if (t.ElapsedSeconds - prev.ElapsedSeconds > 0 && 
                                    (!isPace || (t.Value - prev.Value > 0)))
                                {
                                    float val = (t.Value - prev.Value) / (t.ElapsedSeconds - prev.ElapsedSeconds);
                                    if (val > 0)
                                    {
                                        m_deviceSpeedPaceTrack0.Add(dateTime, val);
                                    }
                                }
                            }
                            if (prev == null || t.ElapsedSeconds - prev.ElapsedSeconds > 0)
                            {
                                //Update previous only if more than one sec has passed
                                prev = t;
                            }
                            if (dateTime >= this.EndTime)
                            {
                                break;
                            }
                        }
                    }
                }
                m_deviceSpeedPaceTrack0 = SmoothTrack(m_deviceSpeedPaceTrack0, TrailActivityInfoOptions.SpeedSmoothingSeconds);
                //Smooth speed, convert after
                for (int i = 0; i < m_deviceSpeedPaceTrack0.Count; i++)
                {
                    m_deviceSpeedPaceTrack0.SetValueAt(i, (float)UnitUtil.PaceOrSpeed.ConvertFrom(isPace, m_deviceSpeedPaceTrack0[i].Value, this.m_cacheTrackRef.Activity));
                }
            }
            return m_deviceSpeedPaceTrack0;
        }

        private INumericTimeDataSeries DeviceElevationTrackFromActivity(IActivity activity, bool trim, int eleSmooth)
        {
            INumericTimeDataSeries deviceElevationTrack0 = null;
            if (activity != null)
            {
                INumericTimeDataSeries sourceTrack = null;
                if (activity.ElevationMetersTrack != null && activity.ElevationMetersTrack.Count > 1)
                {
                    //Separate elevation track, prefer it, assume Barometric
                    sourceTrack = activity.ElevationMetersTrack;
                }
                else if (activity.GPSRoute != null && activity.GPSRoute.Count > 1 && activity.GPSRoute.StartTime > DateTime.MinValue)
                {
                    foreach (string devName in Settings.BarometricDevices)
                    {
                        if (activity.Metadata.Source.Contains(devName))
                        {
                            sourceTrack = new NumericTimeDataSeries();
                            foreach (ITimeValueEntry<IGPSPoint> g in activity.GPSRoute)
                            {
                                sourceTrack.Add(activity.GPSRoute.EntryDateTime(g), g.Value.ElevationMeters);
                            }
                            break;
                        }
                    }
                }
                if (sourceTrack != null && sourceTrack.Count>1)
                {
                    const int maxTimeDiff = 60;
                    DateTime start2 = sourceTrack.StartTime.AddSeconds(-maxTimeDiff);
                    IValueRangeSeries<DateTime> pauses;
                    if (this.Activity != null)
                    {
                        pauses = this.Activity.TimerPauses;
                    }
                    else
                    {
                        pauses = new ValueRangeSeries<DateTime>();
                    }
                    DateTime end2 = ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.AddTimeAndPauses(start2, TimeSpan.FromSeconds(sourceTrack.TotalElapsedSeconds+2*maxTimeDiff), pauses);
                    if (start2 <= this.StartTime && this.EndTime <= end2)
                    {
                        deviceElevationTrack0 = copySmoothTrack(sourceTrack, true, trim, eleSmooth,
                               new Convert(UnitUtil.Elevation.ConvertFrom), this.m_cacheTrackRef);
                    }
                }
            }
            return deviceElevationTrack0;
        }

        //Unused align of a track, previously in DeviceElevationTrackFromActivity
        //private void AdjustToGraphSync(TrailResult refRes, INumericTimeDataSeries deviceElevationTrack0)
        //{
        //    float offset = LineChartUtil.getSyncGraphOffset(deviceElevationTrack0, this.ElevationMetersTrack0(refRes), UI.Activity.TrailLineChart.SyncGraph);
        //    for (int i = 0; i < deviceElevationTrack0.Count; i++)
        //    {
        //        deviceElevationTrack0.SetValueAt(i, deviceElevationTrack0[i].Value + offset);
        //    }
        //}

        public INumericTimeDataSeries DeviceElevationTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_deviceElevationTrack0 == null)
            {
                m_deviceElevationTrack0 = DeviceElevationTrack0(TrailActivityInfoOptions.ElevationSmoothingSeconds, true, TrailsPlugin.Data.Settings.UseTrailElevationAdjust);
            }
            return m_deviceElevationTrack0;
        }

        public INumericTimeDataSeries DeviceElevationTrack0()
        {
            return this.DeviceElevationTrack0(this.m_cacheTrackRef);
        }

        private INumericTimeDataSeries DeviceElevationTrack0(int eleSmooth, bool trimToResult, bool adjustElevation)
        {
            INumericTimeDataSeries deviceElevationTrack0 = null;
            //include the activity track with UseTrailElevationAdjust, then cut it - for adjustment
            bool trimSource = !TrailsPlugin.Data.Settings.UseTrailElevationAdjust && trimToResult;

            //Try to get separate elevation track or GPS elevation from barometric device
            deviceElevationTrack0 = this.DeviceElevationTrackFromActivity(this.Activity, trimSource, eleSmooth);

            if (deviceElevationTrack0 == null && TrailsPlugin.Data.Settings.DeviceElevationFromOther)
            {
                //Get device elevation from another activity in results
                foreach (TrailResultWrapper trw in Controller.TrailController.Instance.CurrentResultTreeList)
                {
                    IActivity activity = trw.Result.Activity;
                    if (activity != this.Activity)
                    {
                        deviceElevationTrack0 = this.DeviceElevationTrackFromActivity(activity, trimSource, eleSmooth);
                        if (deviceElevationTrack0 != null && deviceElevationTrack0.Count > 1)
                        {
                            break;
                        }
                    }
                }
                if (deviceElevationTrack0 == null)
                {
                    //TBD: Can "similar" activities be preferred? Currently use first found
                    foreach (IActivity activity in Plugin.GetApplication().Logbook.Activities)
                    {
                        if (activity != this.Activity)
                        {
                            deviceElevationTrack0 = this.DeviceElevationTrackFromActivity(activity, trimSource, eleSmooth);
                            if (deviceElevationTrack0 != null && deviceElevationTrack0.Count > 1)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            if (deviceElevationTrack0 == null)
            {
                deviceElevationTrack0 = new NumericTimeDataSeries();
            }
            else if (adjustElevation)
            {
                //Note that deviceElevationTrack0 is extended, to set elevationOffset as good as possible
                IList<DateEle> elevationOffset = new List<DateEle>();
                elevationOffset = getElevationOffsets(deviceElevationTrack0, this.m_subResultInfo.Points);

                if (elevationOffset.Count == 0)
                {
                    //Get the results on the complete activity, to match better
                    IList<TrailResultPoint> points = ActivityTrail.CalcElevationPoints(this.Activity, null);
                    elevationOffset = getElevationOffsets(deviceElevationTrack0, points);
                }

                if (trimToResult)
                {
                    //Trim, insert (as start/end changes), smooth already done
                    deviceElevationTrack0 = copySmoothTrack(deviceElevationTrack0, true, true, 0,
                        new Convert(ConvertNone), this.m_cacheTrackRef);
                }
                //Correct the elevation track from the offset (already in display unit), by time
                //(assume that all error is changed over time)
                if (elevationOffset.Count > 0)
                {
                    int eleIndex = 0;
                    if (elevationOffset.Count > 1)
                    {
                        //interpolate between points, extrapolate before first and after last
                        eleIndex = 1;
                    }
                    for (int i = 0; i < deviceElevationTrack0.Count; i++)
                    {
                        DateTime t = deviceElevationTrack0.EntryDateTime(deviceElevationTrack0[i]);
                        float e;
                        while (elevationOffset[eleIndex].Date < t)
                        {
                            if (eleIndex >= elevationOffset.Count - 1)
                            {
                                break;
                            }
                            else
                            {
                                eleIndex++;
                            }
                        }
                        if (eleIndex > 0)
                        {
                            //interpolate between points, extrapolate before first and after last
                            float k = (float)((t - elevationOffset[eleIndex - 1].Date).TotalSeconds / (elevationOffset[eleIndex].Date - elevationOffset[eleIndex - 1].Date).TotalSeconds);
                            e = ((1 - k) * elevationOffset[eleIndex - 1].elevation + k * elevationOffset[eleIndex].elevation);
                        }
                        else
                        {
                            //single point, no extrapolation
                            e = elevationOffset[eleIndex].elevation;
                        }
                        e += deviceElevationTrack0[i].Value;
                        deviceElevationTrack0.SetValueAt(i, e);
                    }
                }
            }
            return deviceElevationTrack0;
        }

        private IList<DateEle> getElevationOffsets(INumericTimeDataSeries deviceElevationTrack0, IList<TrailResultPoint> points)
        {
            //Find matches at elevationpoints for current trail
            IList<DateEle> elevationOffset = new List<DateEle>();
            IActivity refAct = null;
            if(this.m_cacheTrackRef != null)
            {
                refAct = this.m_cacheTrackRef.Activity;
            }
            foreach (TrailResultPoint t in points)
            {
                if (!float.IsNaN(t.ElevationMeters))
                {
                    DateTime d1 = t.Time;
                    if (d1 != DateTime.MinValue)
                    {
                        float ele = float.NaN;
                        const int extendSeconds = 60;
                        //deviceElevationTrack0 is not trimmed and therefore "maximum" size
                        if (deviceElevationTrack0.StartTime <= d1 && d1 <= deviceElevationTrack0.EntryDateTime(deviceElevationTrack0[deviceElevationTrack0.Count - 1]))
                        {
                            try
                            {
                                ele = deviceElevationTrack0.GetInterpolatedValue(d1).Value;
                            }
                            catch { }
                        }
                        else
                        {
                            //Allow some extending, if the elevation track is not matching completely (separate start for instance)
                            double offset;
                            int index = -1;
                            offset = (deviceElevationTrack0.StartTime - d1).TotalSeconds;
                            if (offset >= 0 && offset <= extendSeconds)
                            {
                                index = 0;
                            }
                            else
                            {
                                index = deviceElevationTrack0.Count - 1;
                                offset = (d1 - deviceElevationTrack0.EntryDateTime(deviceElevationTrack0[index])).TotalSeconds;
                                if (offset < 0 && offset > extendSeconds)
                                {
                                    index = -1;
                                }
                            }
                            if (index >= 0)
                            {
                                ele = deviceElevationTrack0[index].Value;
                            }
                        }
                        if (!float.IsNaN(ele))
                        {
                            //elevation track is already in display unit
                            float eleOffset = t.ElevationMeters;
                            if (!UnitUtil.Elevation.isDefaultUnit(refAct))
                            {
                                eleOffset = (float)UnitUtil.Elevation.ConvertFrom(eleOffset, refAct);
                            }
                            eleOffset -= ele;
                            DateEle e = new DateEle(d1, eleOffset);
                            elevationOffset.Add(e);
                        }
                    }
                }
            }
            return elevationOffset;
        }

        private class DateEle
        {
                internal DateEle(DateTime Date, float elevation)
                {
                    this.Date = Date;
                    this.elevation = elevation;
                }

                internal DateTime Date;
                internal float elevation;
        }

        //Set GPS elevation from the "calculated" elevation track
        internal bool SetDeviceElevation(bool adjustElevation)
        {
            bool result = false;
            //Get a track adjusted for the activity
            INumericTimeDataSeries eTrack = this.DeviceElevationTrack0(0, false, adjustElevation);
            if (this.Activity != null && eTrack != null && eTrack.Count > 1)
            {
                // possibly converted to display unit (for Imperal/feet users). Change back to SI unit
                if (!UnitUtil.Elevation.isDefaultUnit(this.Activity))
                {
                    for (int i = 0; i < eTrack.Count; i++)
                    {
                        eTrack.SetValueAt(i, (float)UnitUtil.Elevation.ConvertTo(eTrack[i].Value, this.Activity));
                    }
                }

                if (this.Activity.GPSRoute != null && this.Activity.GPSRoute.Count > 1)
                {
                    IGPSRoute gpsRoute = new GPSRoute();
                    foreach (ITimeValueEntry<IGPSPoint> g in this.Activity.GPSRoute)
                    {
                        DateTime d1 = this.Activity.GPSRoute.EntryDateTime(g);
                        ITimeValueEntry<float> val = eTrack.GetInterpolatedValue(d1);
                        float ele = float.NaN;
                        if (val != null)
                        {
                            ele = val.Value;
                        }
                        else
                        {
                            //The elevation track should include (almost) all points, just some extensions
                            if (d1 < eTrack.StartTime)
                            {
                                ele = eTrack[0].Value;
                            }
                            else if (d1 > eTrack.EntryDateTime(eTrack[eTrack.Count - 1]))
                            {
                                ele = eTrack[eTrack.Count - 1].Value;
                            }
                            else
                            {
                                //Should not happen, ignore point...
                            }
                        }

                        if (!float.IsNaN(ele))
                        {
                            //ele += UI.Activity.TrailLineChart.FixedSyncGraphMode;
                            IGPSPoint g2 = new GPSPoint(g.Value.LatitudeDegrees, g.Value.LongitudeDegrees, ele);
                            gpsRoute.Add(d1, g2);
                        }
                    }
                    this.Activity.GPSRoute = gpsRoute;
                }
                else
                {
                    //No GPS track, add as separate elevation track
                    this.Activity.ElevationMetersTrack = eTrack;
                }
                this.Clear(false);
                result = true;
            }
            return result;
        }

        //internal int SetDeviceElevationOffset()
        //{
        //    if (this.Activity != null && this.Activity.ElevationMetersTrack != null &&
        //        this.Activity.ElevationMetersTrack.Count > 0 && Settings.ChartType == LineChartTypes.Elevation &&
        //        UI.Activity.TrailLineChart.FixedSyncGraphMode != 0)
        //    {
        //        for (int i = 0; i < this.ElevationMetersTrack.Count; i++)
        //        {
        //            this.ElevationMetersTrack.SetValueAt(i, this.ElevationMetersTrack[i].Value - UI.Activity.TrailLineChart.FixedSyncGraphMode);
        //        }
        //    }
        //    return 0;
        //}

        //internal int SetExternalElevation(INumericTimeDataSeries eTrack)
        //{
        //    INumericTimeDataSeries elevationTrack0 = copySmoothTrack(eTrack, true, TrailActivityInfoOptions.ElevationSmoothingSeconds,
        //           new Convert(UnitUtil.Elevation.ConvertFrom), this.m_cacheTrackRef);
        //    float offset = this.ElevationMetersTrack.Avg - elevationTrack0.Avg;
        //    for (int i = 0; i < elevationTrack0.Count; i++)
        //    {
        //        elevationTrack0.SetValueAt(i, elevationTrack0[i].Value + offset);
        //    }

        //    INumericTimeDataSeries t = this.m_deviceElevationTrack0;
        //    if (t == null || t.Count==0)
        //    {
        //        //Save "original" data as device track
        //        t = this.m_elevationMetersTrack0;
        //    }
        //    this.Clear(false);
        //    this.m_deviceElevationTrack0 = t;
        //    this.m_elevationMetersTrack0 = elevationTrack0;

        //    return 0;
        //}

        public INumericTimeDataSeries DeviceDiffDistTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_deviceDiffDistTrack0 == null)
            {
                if ((this is ChildTrailResult) && (this as ChildTrailResult).PartOfParent)
                {
                    m_deviceDiffDistTrack0 = copySmoothTrack((this as ChildTrailResult).ParentResult.DeviceDiffDistTrack0(this.m_cacheTrackRef), false, 0, ConvertNone, this.m_cacheTrackRef);
                    return m_deviceDiffDistTrack0;
                }
                m_deviceDiffDistTrack0 = new NumericTimeDataSeries();
                if (this.Activity != null && this.Activity.DistanceMetersTrack != null && this.Activity.DistanceMetersTrack.Count > 0)
                {
                    float? start2 = null;
                    foreach (ITimeValueEntry<float> t in this.DistanceMetersTrack)
                    {
                        DateTime dateTime = this.DistanceMetersTrack.EntryDateTime(t);
                        ITimeValueEntry<float> t2 = this.Activity.DistanceMetersTrack.GetInterpolatedValue(dateTime);
                        if (t2 != null &&
                                !ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(dateTime, this.Pauses))
                        {
                            if (start2 == null)
                            {
                                start2 = t2.Value;
                            }
                            float val = (float)UnitUtil.Elevation.ConvertFrom(-t.Value + t2.Value - (float)start2, this.Activity);
                            m_deviceDiffDistTrack0.Add(dateTime, val);
                        }
                        else
                        { }
                    }
                }
            }
            return m_deviceDiffDistTrack0;
        }
#endregion

        /**********************************************************/
        #region GradeAdjust

        internal static void ResetRunningGradeCalcMethod()
        {
            Settings.RunningGradeAdjustMethod = RunningGradeAdjustMethodEnum.None;
        }

        internal static void IncreaseRunningGradeCalcMethod()
        {
            Settings.RunningGradeAdjustMethod++;
            if (Settings.RunningGradeAdjustMethod == RunningGradeAdjustMethodEnum.Last)
            {
                Settings.RunningGradeAdjustMethod = RunningGradeAdjustMethodEnum.None;
            }
            foreach (TrailResult t in TrailResultWrapper.IncludeSubResults(TrailsPlugin.Controller.TrailController.Instance.CurrentResultTreeList))
            {
                t.m_gradeRunAdjustedTime = null;
            }
        }

        private class ginfo
        {
            public DateTime dateTime;
            public float time;
            public float dist;
            public float totDist;
            public float q;
            public float adjTime = 0;
            public float adjSpeed; //The speed in a point - simplifies when cutting up a track

            public ginfo(DateTime dateTime, float time, float dist, float totDist, float q)
            {
                this.dateTime = dateTime;
                this.time = time;
                this.dist = dist;
                this.totDist = totDist;
                this.q = q;
                if (time > 0)
                {
                    this.adjSpeed = this.dist / this.time / this.q;
                }
                else
                {
                    this.adjSpeed = float.NaN;
                }
            }
        }

        public virtual TimeSpan GradeRunAdjustedTime
        {
            get
            {
                this.calcGradeRunAdjustedTime(this.m_cacheTrackRef);
                float res = 0;
                if (m_gradeRunAdjustedTime != null && this.m_gradeRunAdjustedTime.Count > 0)
                {
                    res = this.m_gradeRunAdjustedTime[this.m_gradeRunAdjustedTime.Count - 1].Value;
                }
                return TimeSpan.FromSeconds(res);
            }
        }

        public virtual float GradeRunAdjustedSpeed
        {
            get
            {
                this.calcGradeRunAdjustedTime(this.m_cacheTrackRef);
                float res;
                double time = this.GradeRunAdjustedTime.TotalSeconds;
                if (time > 0)
                {
                    res = (float)(this.Distance / time);
                }
                else
                {
                    res = float.NaN;
                }

                return res;
            }
        }

        //TBD optimise Minimize use of ElevationMetersTrack0() as well as GetInterpolatedValue() 
        private void calcGradeRunAdjustedTime(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_gradeRunAdjustedTime == null)
            {
                m_gradeRunAdjustedTime = new NumericTimeDataSeries();
                m_grades = new List<ginfo>();
                //Clear derived objects too
                m_gradeRunAdjustedTimeAvg = null;

                float prevTime = 0;
                float prevVal = 0;
                float prevEle = float.NaN;
                float prevDist = 0;
                INumericTimeDataSeries eleTrack = this.CalcElevationMetersTrack0(this.m_cacheTrackRef);
                bool convertEleToSI = !UnitUtil.Elevation.isDefaultUnit(this.Activity);

                foreach (ITimeValueEntry<float> t in this.DistanceMetersTrack)
                {
                    //TODO insert when sparse
                    DateTime dateTime = this.DistanceMetersTrack.EntryDateTime(t);
                    ITimeValueEntry<float> t2 = eleTrack.GetInterpolatedValue(dateTime);
                    float dist = t.Value;
                    float time = (float)this.getTimeResult(dateTime);
                    float q = 1; //diff to flat

                    if (t2 != null)
                    {
                        float ele = t2.Value;
                        if (convertEleToSI)
                        {
                            ele = (float)UnitUtil.Elevation.ConvertFrom(ele, this.Activity);//convert back...
                        }
                        //This point is valid, at least for next
                        if (!float.IsNaN(prevEle))
                        {
                            if (dist - prevDist > 0)
                            {
                                float g = (ele - prevEle) / (dist - prevDist); //grade
                                q = RunningGradeAdjustMethodClass.getGradeFactor(g, time, prevTime, dist, prevDist);
                            }
                        }
                        else
                        {
                            //Just save prevEle
                        }
                        prevEle = ele;
                    }
                    else
                    {
                        //No elevation, keep q=1
                        prevEle = float.NaN;
                    }
                    float adjTime = prevVal + q * (time - prevTime);
                    m_gradeRunAdjustedTime.Add(dateTime, adjTime);
                    prevVal = adjTime;
                    m_grades.Add(new ginfo(dateTime, time - prevTime, dist - prevDist, dist, q));

                    prevTime = time;
                    prevDist = dist;
                }
            }
        }

        private IDistanceDataTrack GradeRunAdjustedTimeTrack(TrailResult refRes)
        {
            this.calcGradeRunAdjustedTime(refRes);

            if (m_gradeRunAdjustedTimeAvg == null)
            {
                m_gradeRunAdjustedTimeAvg = new DistanceDataTrack();
                if (this.m_gradeRunAdjustedTime.Count > 0)
                {
                    //Iterate to get new time apropriate for "adjusted time"
                    float newTime = m_gradeRunAdjustedTime[m_gradeRunAdjustedTime.Count - 1].Value; //time derived from predictTime, should converge to resultTime
                    float predictTime = (float)this.Duration.TotalSeconds * 2 - newTime;//Time used to predict resultTime - seed with most likely
                    float[,] splitTime = Settings.AdjustDiffSplitTimes;

                    int i = 0; //limit iterations - should iterate within 3 tries
                    while (i==0 ||
                        Math.Abs(newTime - this.Duration.TotalSeconds) > 3 && i < 9 ||
                        Math.Abs(newTime - this.Duration.TotalSeconds) > 1 && i < 3)
                    {
                        float avgAdjSpeed = (float)(this.Distance / predictTime);
                        float avgAdjSpeedAdj = avgAdjSpeed;
                        int j = -1;
                        newTime = 0;

                        foreach (ginfo t in m_grades)
                        {
                            if (t.q > 0)
                            {
                                //Get avg speed for this part
                                while (splitTime != null && splitTime.Length > 0 &&
                                    j < splitTime.Length / 2 && (j < 0 || t.totDist > splitTime[j, 0]))
                                {
                                    j++;
                                    float prevTime = 0;
                                    float prevDist = 0;
                                    if (j > 0)
                                    {
                                        prevDist = splitTime[j - 1, 0];
                                        prevTime = splitTime[j - 1, 1];
                                    }
                                    float thisTime = 0;
                                    float thisDist = (float)this.Distance;
                                    if (j < splitTime.Length / 2)
                                    {
                                        thisDist = splitTime[j, 0];
                                        thisTime = splitTime[j, 1];
                                    }
                                    avgAdjSpeedAdj = (thisDist - prevDist) / ((thisDist - prevDist) / avgAdjSpeed + (thisTime - prevTime));
                                }
                                newTime += t.dist / avgAdjSpeedAdj / t.q;
                            }
                            //Add time, not elapsed to track
                            t.adjTime = newTime;
                        }
                        predictTime = (float)(this.Duration.TotalSeconds + (predictTime - newTime));
                        i++;
                    }
#if TRACE
                    if (i > 3)
                    { }
#endif
                    foreach (ginfo t in m_grades)
                    {
                        m_gradeRunAdjustedTimeAvg.Add(t.dateTime, t.adjTime);
#if TRACE
                        if (Math.Abs(t.adjTime - this.getTimeResult(t.dateTime)) > 4)
                        {
                        }
                        if (Math.Abs(t.adjTime - this.getTimeResult(t.dateTime)) <2)
                        {
                        }
#endif
                    }
                }
            }

            return m_gradeRunAdjustedTimeAvg;
        }
        
        #endregion

        /**********************************************************/
        #region diff

        private TrailResult getRefSub(TrailResult parRes)
        {
            TrailResult res = parRes;
            if ((this is ChildTrailResult) && (this as ChildTrailResult).PartOfParent &&
                parRes != null && parRes is ParentTrailResult && (parRes as ParentTrailResult).m_childrenResults != null)
            {
                //This is a subsplit, get the subsplit related to the ref
                foreach (TrailResult tr in (parRes as ParentTrailResult).m_childrenResults)
                {
                    if (this.m_order == tr.Order)
                    {
                        res = tr;
                        break;
                    }
                }
            }
            return res;
        }

        public INumericTimeDataSeries DiffTimeTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_DiffTimeTrack0 == null)
            {
                m_DiffTimeTrack0 = new NumericTimeDataSeries();
                TrailResult trRef = getRefSub(m_cacheTrackRef);
                if (this.DistanceMetersTrack.Count > 0 && trRef != null)
                {
                    int oldElapsed = int.MinValue;
                    float lastValue = 0;
                    int dateTrailPointIndex = -1;
                    float refOffset = 0;
                    float refPrevTime = 0;
                    float diffOffset = 0;

                    bool prevCommonStreches = false;
                    IValueRangeSeries<DateTime> commonStretches = null;
                    if (Settings.DiffUsingCommonStretches && trRef.Activity != null)
                    {
                        //TODO: Use reference stretch too
                        commonStretches = CommonStretches(trRef.Activity, new List<IActivity> { this.Activity }, null)[this.Activity][0].MarkedTimes;
                        m_DiffTimeTrack0.Add(StartTime, 0);
                    }

                    foreach (ITimeValueEntry<float> t in DistanceMetersTrack)
                    {
                        uint elapsed = t.ElapsedSeconds;
                        if (elapsed > oldElapsed)
                        {
                            DateTime d1 = this.DistanceMetersTrack.EntryDateTime(t);
                            if (!ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(d1, Pauses))
                            {
                                while (Settings.ResyncDiffAtTrailPoints &&
                                    this.TrailPointDateTime.Count == trRef.TrailPointDateTime.Count && //Splits etc
                                    (dateTrailPointIndex == -1 ||
                                    dateTrailPointIndex < this.TrailPointDateTime.Count - 1 &&
                                    d1 > this.TrailPointDateTime[dateTrailPointIndex + 1]))
                                {
                                    dateTrailPointIndex++;
                                    if (dateTrailPointIndex < this.TrailPointDateTime.Count - 1 &&
                                        dateTrailPointIndex < trRef.TrailPointDateTime.Count - 1 &&
                                        this.TrailPointDateTime[dateTrailPointIndex] > DateTime.MinValue &&
                                        trRef.TrailPointDateTime[dateTrailPointIndex] > DateTime.MinValue)
                                    {
                                        refOffset = trRef.getDistResult(trRef.TrailPointDateTime[dateTrailPointIndex]) -
                                           this.getDistResult(this.TrailPointDateTime[dateTrailPointIndex]);
                                        if (Settings.AdjustResyncDiffAtTrailPoints)
                                        {
                                            //diffdist over time will normally "jump" at each trail point
                                            //I.e. if the reference is behind, the distance suddenly gained must be subtracted
                                            //Note: ST only uses int for time, use double anyway
                                            //float oldElapsedSec = oldElapsed > 0 ? oldElapsed : 0;
                                            //TODO correct?
                                            //float oldTime = oldElapsed;
                                            float refTimeP = (float)trRef.getTimeResult(trRef.TrailPointDateTime[dateTrailPointIndex]);
                                            float thisTimeP = (float)this.getTimeResult(this.TrailPointDateTime[dateTrailPointIndex]);
                                            diffOffset += (refTimeP - refPrevTime - (thisTimeP - refPrevTime)); 
                                        }
                                    }
                                }

                                if (Settings.DiffUsingCommonStretches &&
                                    //IsPaused is in the series here...
                                !ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(d1, commonStretches))
                                {
                                    prevCommonStreches = true;
                                }
                                else
                                {
                                    //elapsed (entry) is elapsed in the series, not result/activity elapsed seconds
                                    float thisTime = (float)this.getTimeResult(d1);
                                    float? refTime = null;
                                    if (TrailResult.diffToSelf || trRef == this)
                                    {
                                        ////"inconsistency" from getDateTimeFromTrack() can happen if the ref stands still, getDateTimeFromTrack returns first elapsed
                                        //refTime = thisTime;
                                        //get diff from average
                                        if (Settings.RunningGradeAdjustMethod != RunningGradeAdjustMethodEnum.None ||
                                            TrailsPlugin.Data.Settings.AdjustDiffSplitTimes != null)
                                        {
                                            float dist = this.getDistResult(d1);
                                            ITimeValueEntry<float> val = this.GradeRunAdjustedTimeTrack(this.m_cacheTrackRef).GetInterpolatedValue(d1);
                                            if (val != null)
                                            {
                                                refTime = val.Value;
                                            }
                                        }
                                        else 
                                        {
                                            float dist = this.getDistResult(d1);
                                            refTime = dist / this.AvgSpeed;
                                        }
                                    }
                                    else
                                    {
                                        if (t.Value + refOffset <= trRef.DistanceMetersTrack.Max)
                                        {
                                            DateTime d2 = TrackUtil.getDateTimeFromTrackDist(trRef.DistanceMetersTrack, t.Value + refOffset);
                                            refTime = (float)trRef.getTimeResult(d2);
                                        }
                                    }
                                    if (refTime != null && !float.IsNaN((float)refTime))
                                    {
                                        if (Settings.DiffUsingCommonStretches && prevCommonStreches)
                                        {
                                            diffOffset = thisTime - (float)refTime;
                                            prevCommonStreches = false;
                                        }
                                        lastValue = (float)refTime - thisTime + diffOffset;
                                        m_DiffTimeTrack0.Add(d1, lastValue);
                                        oldElapsed = (int)elapsed;
                                        refPrevTime = (float)refTime;
                                    }
                                }
                            }
                        }
                    }
                    //Add a point last in the track, to show the complete dist in the chart
                    //Alternative use lastvalue
                    dateTrailPointIndex = this.TrailPointDateTime.Count - 1;
                    if (trRef != this &&
                        !ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(EndTime, Pauses) &&
                        (Settings.ResyncDiffAtTrailPoints ||
                        this.m_activityTrail != null && (this.m_activityTrail.Trail.IsReference || !this.m_activityTrail.Trail.Generated)) &&
                        m_cacheTrackRef == trRef && //Otherwise will cache be cleared for splits...
                        this.TrailPointDateTime.Count == trRef.TrailPointDateTime.Count && //Splits etc
                        dateTrailPointIndex > 0 &&
                        dateTrailPointIndex == trRef.TrailPointDateTime.Count - 1 &&
                                this.TrailPointDateTime[dateTrailPointIndex] > DateTime.MinValue &&
                                trRef.TrailPointDateTime[dateTrailPointIndex] > DateTime.MinValue)
                    {
                        float refTime = trRef.TrailPointTime0(trRef)[dateTrailPointIndex];
                        float trTime = this.TrailPointTime0(trRef)[dateTrailPointIndex];
                        if (!float.IsNaN(refTime) && !float.IsNaN(trTime))
                        {
                            lastValue = (float)(refTime - trTime + diffOffset);
                            //TBD Disable this add, not so interesting?
                            //m_DiffTimeTrack0.Add(EndTime, lastValue);
                        }
                    }
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

        private enum DiffMode { ActivityStart, AbsoluteTime } //TODO:, TimeOfDay }
        public INumericTimeDataSeries DiffDistTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_DiffDistTrack0 == null)
            {
                m_DiffDistTrack0 = new NumericTimeDataSeries();
                TrailResult trRef = getRefSub(m_cacheTrackRef);
                if (this.DistanceMetersTrack.Count > 0 && trRef != null)
                {
                    int oldElapsed = int.MinValue;
                    float lastValue = 0;
                    int dateTrailPointIndex = -1;
                    int refTimeOffset = 0;
                    float diffOffset = 0;
                    double prevDist = 0;
                    double prevRefDist = 0;

                    bool prevCommonStreches = false;
                    IValueRangeSeries<DateTime> commonStretches = null;
                    if (Settings.DiffUsingCommonStretches && trRef.Activity != null)
                    {
                        commonStretches = CommonStretches(trRef.Activity, new List<IActivity> { this.Activity }, null)[this.Activity][0].MarkedTimes;
                        m_DiffDistTrack0.Add(StartTime, 0);
                    }
                    DiffMode diffMode = DiffMode.ActivityStart;
                    if (m_diffOnDateTime && (
                        this.StartTime >= trRef.StartTime && this.StartTime <= trRef.EndTime ||
                        trRef.StartTime >= this.StartTime && trRef.StartTime <= this.EndTime))
                    {
                        //TODO: Implement
                        diffMode = DiffMode.AbsoluteTime;
                    }
                    foreach (ITimeValueEntry<float> t in this.DistanceMetersTrack)
                    {
                        uint elapsed = t.ElapsedSeconds;
                        float thisDist = t.Value;
                        if (elapsed > oldElapsed)
                        {
                            DateTime d1 = DistanceMetersTrack.EntryDateTime(t);
                            if (!ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(d1, Pauses))
                            {
                                //TODO ActivityStart
                                while (/*diffMode == DiffMode.ActivityStart && */Settings.ResyncDiffAtTrailPoints &&
                                    this.TrailPointDateTime.Count == trRef.TrailPointDateTime.Count && //Splits etc
                                    (dateTrailPointIndex == -1 ||
                                    dateTrailPointIndex < this.TrailPointDateTime.Count - 1 &&
                                    d1 > this.TrailPointDateTime[dateTrailPointIndex + 1]))
                                {
                                    dateTrailPointIndex++;
                                    if (dateTrailPointIndex < this.TrailPointDateTime.Count - 1 &&
                                        dateTrailPointIndex < trRef.TrailPointDateTime.Count - 1 &&
                                        this.TrailPointDateTime[dateTrailPointIndex] > DateTime.MinValue &&
                                        trRef.TrailPointDateTime[dateTrailPointIndex] > DateTime.MinValue)
                                    {
                                        refTimeOffset = (int)(trRef.getTimeResult(trRef.TrailPointDateTime[dateTrailPointIndex]) -
                                           this.getTimeResult(this.TrailPointDateTime[dateTrailPointIndex]));
                                        //TODO: Configure, explain (or remove)
                                        if (Settings.AdjustResyncDiffAtTrailPoints)
                                        {
                                            //diffdist over time will normally "jump" at each trail point
                                            //I.e. if the reference is behind, the distance suddenly gained must be subtracted
                                            int status2;
                                            double refDistP = TrackUtil.getValFromDateTime(trRef.DistanceMetersTrack,
                                                trRef.TrailPointDateTime[dateTrailPointIndex], out status2);
                                            if (status2 == 0)
                                            {
                                                //getDistResult
                                                double distP = TrackUtil.getValFromDateTime(this.DistanceMetersTrack,
                                                    this.TrailPointDateTime[dateTrailPointIndex], out status2);
                                                if (status2 == 0)
                                                {
                                                    diffOffset += (float)(refDistP - prevRefDist - (distP - prevDist));
                                                }
                                            }
                                        }
                                    }
                                }

                                if (Settings.DiffUsingCommonStretches &&
                                    //IsPaused is in the series here...
                                    !ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(d1, commonStretches))
                                {
                                    prevCommonStreches = true;
                                }
                                else
                                {
                                    //Only check if possible (ignore pauses when pruning)
                                    //if (t.ElapsedSeconds + refTimeOffset <= trRef.DistanceMetersTrack.TotalElapsedSeconds)
                                    {
                                        double? refDist = null;
                                        if (TrailResult.diffToSelf || trRef == this)
                                        {
                                            ////"inconsistency" from getDateTimeFromTrack() can happen if the ref stands still, getDateTimeFromTrack returns first elapsed
                                            //get diff from average
                                            if (Settings.RunningGradeAdjustMethod != RunningGradeAdjustMethodEnum.None || 
                                                TrailsPlugin.Data.Settings.AdjustDiffSplitTimes != null)
                                            {
                                                //ITimeValueEntry<float> val = this.GradeRunAdjustedTimeTrack(this.m_cacheTrackRef).GetInterpolatedValue(d1);
                                                //if (val != null && !float.IsNaN(val.Value))
                                                {
                                                    //TODO: correct check here?
                                                    //refDist = this.getDistResult(getDateTimeFromTimeResult(val.Value));
                                                    IDistanceDataTrack dt = this.GradeRunAdjustedTimeTrack(this.m_cacheTrackRef);
                                                    DateTime d2 = dt.GetTimeAtDistanceMeters(this.getTimeResult(d1));
                                                    refDist = this.getDistResult(d2);
                                                }
                                            }
                                            else
                                            {
                                                //get diff from average
                                                refDist = this.getTimeResult(d1) * this.AvgSpeed;
                                            }
                                        }
                                        else
                                        {
                                            DateTime d2;
                                            if (diffMode == DiffMode.ActivityStart)
                                            {
                                                d2 = trRef.DistanceMetersTrack.EntryDateTime(getValueEntryOffset(t, refTimeOffset));
                                            }
                                            else
                                            {
                                                d2 = d1;
                                            }
                                            int status;
                                            refDist = TrackUtil.getValFromDateTime(trRef.DistanceMetersTrack, d2, out status);
                                            if (status != 0)
                                            {
                                                refDist = null;
                                            }
                                        }
                                        //Only add if valid estimation
                                        if (refDist != null && !float.IsNaN((float)refDist))
                                        {
                                            if (Settings.DiffUsingCommonStretches && prevCommonStreches)
                                            {
                                                //TODO: Not implemented
                                                diffOffset = (float)refDist - thisDist;
                                                prevCommonStreches = false;
                                            }
                                            double diff = thisDist - (double)refDist + diffOffset;
                                            lastValue = (float)UnitUtil.Elevation.ConvertFrom(diff, trRef.Activity);
                                            m_DiffDistTrack0.Add(d1, lastValue);
                                            oldElapsed = (int)elapsed;
                                            prevDist = thisDist;
                                            prevRefDist = (double)refDist;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //Add a point last in the track, to show the complete dist in the chart
                    //Alternatively use lastvalue
                    dateTrailPointIndex = this.TrailPointDateTime.Count - 1;
                    if (trRef != this &&
                        !ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(EndTime, Pauses) &&
                        (Settings.ResyncDiffAtTrailPoints || 
                        this.m_activityTrail != null && (this.m_activityTrail.Trail.IsReference || !this.m_activityTrail.Trail.Generated)) &&
                        m_cacheTrackRef == trRef && //Otherwise will cache be cleared for splits...
                        this.TrailPointDateTime.Count == trRef.TrailPointDateTime.Count && //Splits etc
                        dateTrailPointIndex > 0 &&
                        dateTrailPointIndex == trRef.TrailPointDateTime.Count - 1 &&
                                this.TrailPointDateTime[dateTrailPointIndex] > DateTime.MinValue &&
                                trRef.TrailPointDateTime[dateTrailPointIndex] > DateTime.MinValue)
                    {
                        float refDist = trRef.TrailPointDist0(trRef)[dateTrailPointIndex];
                        float trDist = this.TrailPointDist0(trRef)[dateTrailPointIndex];
                        if (!float.IsNaN(refDist) && !float.IsNaN(trDist))
                        {
                            lastValue = (float)UnitUtil.Elevation.ConvertFrom(refDist -
                                trDist + diffOffset, trRef.Activity);
                            //TBD Disable this add, not so interesting?
                            //m_DiffDistTrack0.Add(EndTime, lastValue);
                        }
                    }
                }
            }
            return m_DiffDistTrack0;
        }


        /********************************************************************/
        //IList<float> m_trailPointTimeOffset;
        //IList<float> m_trailPointDistOffset;
        //public IList<float> TrailPointTimeOffset0(TrailResult refRes)
        //{
        //    TrailPointTime0(refRes);
        //    return m_trailPointTimeOffset;
        //}
        //public IList<float> TrailPointDistOffset0(TrailResult refRes)
        //{
        //    TrailPointDist0(refRes);
        //    return m_trailPointDistOffset;
        //}

        public IList<float> TrailPointTime0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_trailPointTime0 == null /*|| m_trailPointTimeOffset == null*/)
            {
                m_trailPointTime0 = new List<float>();
                //m_trailPointTimeOffset = new List<float>();
                for (int k = 0; k < this.TrailPointDateTime.Count; k++)
                {
                    float val = float.NaN;
                    //float val1 = float.NaN;
                    if (this.TrailPointDateTime[k] > DateTime.MinValue)
                    {
                        //The used start time for the point
                        DateTime t1 = TrackUtil.getFirstUnpausedTime(this.TrailPointDateTime[k], Pauses, true);
                        //Same as val = getTimeSpanResult(t1);
                        val = (float)ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.TimeNotPaused(
                                StartTime, t1, Pauses).TotalSeconds;
                        //Offset time from detected to actual start
                        //NonMovingTimes here instead?
                        //val1 = (float)ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.TimeNotPaused(
                        //        this.TrailPointDateTime[k], t1, Activity.TimerPauses).TotalSeconds;
                    }
                    m_trailPointTime0.Add(val);
                    //m_trailPointTimeOffset.Add(val1);
                }
                //for (int k = 0; k < m_trailPointTimeOffset.Count - 1; k++)
                //{
                //    if (!float.IsNaN(m_trailPointTimeOffset[k]) && !float.IsNaN(m_trailPointTime0[k + 1]) && !float.IsNaN(m_trailPointTime0[k]) &&
                //        m_trailPointTimeOffset[k] > m_trailPointTime0[k + 1] - m_trailPointTime0[k])
                //    {
                //        m_trailPointTimeOffset[k] = 0;
                //    }
                //}
            }
            return m_trailPointTime0;
        }

        public IList<float> TrailPointDist0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_trailPointDist0 == null /*|| m_trailPointDistOffset == null*/)
            {
                m_trailPointDist0 = new List<float>();
                //m_trailPointDistOffset = new List<float>();
                for (int k = 0; k < TrailPointDateTime.Count; k++)
                {
                    float val = float.NaN;
                    //float val1 = float.NaN;
                    if (TrailPointDateTime[k] > DateTime.MinValue)
                    {
                        ITimeValueEntry<float> interpolatedP = DistanceMetersTrack0(m_cacheTrackRef).GetInterpolatedValue(TrackUtil.getFirstUnpausedTime(this.TrailPointDateTime[k], Pauses, true));
                        if (interpolatedP != null)
                        {
                            //In display format
                            val = interpolatedP.Value;
                        }

                        ////Use "ST" distance track to get offset from detected point start (that could be paused) to used start
                        //interpolatedP = ActivityDistanceMetersTrack.GetInterpolatedValue(TrackUtil.getFirstUnpausedTime(this.TrailPointDateTime[k], Pauses, true));
                        //ITimeValueEntry<float> interpolatedP2 = ActivityDistanceMetersTrack.GetInterpolatedValue(this.TrailPointDateTime[k]);
                        //if (interpolatedP != null && interpolatedP2 != null)
                        //{
                        //    val1 = (float)UnitUtil.Distance.ConvertFrom(interpolatedP.Value - interpolatedP2.Value, m_cacheTrackRef.Activity);
                        //}
                    }

                    m_trailPointDist0.Add(val);
                    //m_trailPointDistOffset.Add(val1);
                }
                //for (int k = 0; k < m_trailPointDistOffset.Count; k++)
                //{
                //    if (k < m_trailPointDistOffset.Count - 1 &&
                //        !float.IsNaN(m_trailPointDistOffset[k]) && !float.IsNaN(m_trailPointDist0[k + 1]) && !float.IsNaN(m_trailPointDist0[k]) &&
                //        m_trailPointDistOffset[k] > m_trailPointDist0[k + 1] - m_trailPointDist0[k])
                //    {
                //        //TBD is this needed?
                //        m_trailPointDistOffset[k] = 0;
                //    }
                //}
            }
            return m_trailPointDist0;
        }
#endregion

        /**********************************************************/
        #region GPS

        private IGPSRoute getGps()
        {
            IGPSRoute gpsTrack = new GPSRoute();
            gpsTrack.AllowMultipleAtSameTime = false;
            if (m_activity != null && m_activity.GPSRoute != null && m_activity.GPSRoute.Count > 0 &&
                StartTime != DateTime.MinValue && EndTime != DateTime.MinValue)
            {
                int i = 0;
                while (i < m_activity.GPSRoute.Count)
                {
                    DateTime dateTime = m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[i]);
                    if (this.StartTime < dateTime)
                    {
                        break;
                    }
                    i++;
                }

                while (i < m_activity.GPSRoute.Count)
                {
                    DateTime dateTime = m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[i]);

                    if (!ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(dateTime, Pauses))
                    {
                        IGPSPoint point = m_activity.GPSRoute[i].Value;
                        gpsTrack.Add(dateTime, point);
                    }
                    if (dateTime >= this.EndTime)
                    {
                        break;
                    }
                    i++;
                }

                //Insert values at borders in m_gpsTrack
                (new InsertValues<IGPSPoint>(this)).insertValues(gpsTrack, m_activity.GPSRoute);
                //TrackUtil.ResortTrack<IGPSPoint>(gpsTrack);
            }
            else
            {
                //debug
            }
            return gpsTrack;
        }

        public IGPSRoute GPSRoute
        {
            get
            {
                if (m_gpsTrack == null)
                {
                    m_gpsTrack = getGps();
                    m_gpsPoints = null;
                }
                return m_gpsTrack;
            }
        }

        public IList<IList<IGPSPoint>> GpsPoints()
        {
            if (m_gpsPoints == null)
            {
                m_gpsPoints = this.GpsPoints(getSelInfo(true));
            }
            return m_gpsPoints;
        }

        public IList<IList<IGPSPoint>> GpsPoints(Data.TrailsItemTrackSelectionInfo t)
        {
            //Before calling, the MarkedTimes should be set
            if (t.MarkedTimes != null && t.MarkedTimes.Count > 0)
            {
                if (t.MarkedTimes.Count == 1 &&
                    t.MarkedTimes[0].Lower <= StartTime &&
                    t.MarkedTimes[0].Upper >= EndTime)
                {
                    //Use cache
                    return this.GpsPoints();
                }
                return this.GpsPoints(t.MarkedTimes);
            }
            return new List<IList<IGPSPoint>>();
        }
        private IList<IList<IGPSPoint>> GpsPoints(IValueRangeSeries<DateTime> t)
        {
            return TrailsItemTrackSelectionInfo.GpsPoints(this.GPSRoute, t);
        }
#endregion

        /**********************************************************/
        #region Color
        private static int nextTrailColor = 0;

        private bool m_colorOverridden = false;
        public ChartColors ResultColor
        {
            get
            {
                ChartColors result = null;

                if (!m_colorOverridden &&
                    (this is ChildTrailResult) && (this as ChildTrailResult).PartOfParent &&
                    Controller.TrailController.Instance.SelectedResults != null &&
                    Controller.TrailController.Instance.SelectedResults.Count > 1)
                {
                    bool useParentColor = false;
                    //Find if any other trailresult with this parent exists
                    foreach (TrailResult tr in Controller.TrailController.Instance.SelectedResults)
                    {
                        TrailResult parent = tr;
                        if (parent is ChildTrailResult)
                        {
                            parent = (parent as ChildTrailResult).ParentResult;
                        }
                        if ((this as ChildTrailResult).ParentResult != parent)
                        {
                            useParentColor = true;
                            break;
                        }
                    }

                    if (useParentColor)
                    {
                        result = (this as ChildTrailResult).ParentResult.ResultColor;
                    }
                }

                if (result == null)
                {
                    if (this.m_trailColor == null)
                    {
                        //Wait to create the color, to get consistent colors before resorting
                        this.m_trailColor = getColor(nextTrailColor++);
                    }
                    result = this.m_trailColor;
                }

                return result;
            }
            set
            {
                m_colorOverridden = true;
                m_trailColor = value;
            }
        }

        private static ChartColors getColor(int color)
        {
            return ColorUtil.ResultColor[nextTrailColor % ColorUtil.ResultColor.Count];
        }
        #endregion

        /**********************************************************/
        #region Activity caches

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

        //ActivityInfo m_ActivityInfo = null;
        ActivityInfo Info
        {
            get
            {
                //Custom InfoCache, to control smoothing (otherwise ST could cache)
                if (this is SummaryTrailResult)
                {
                    Debug.Assert(false);
                    return null;
                }
                return ActivityCache.GetActivityInfo(this.Activity, this.IncludeStopped);
            }
        }

        //The Activity related tracks were previously cached. Kept as an uniform method to get
        //the tracks, if the Info cache handling is changed
        private INumericTimeDataSeries CadencePerMinuteTrack
        {
            get
            {
                if (this.Info == null)
                {
                    return new NumericTimeDataSeries();
                }
                return Info.SmoothedCadenceTrack;
            }
        }

        private INumericTimeDataSeries ElevationMetersTrack
        {
            get
            {
                if (this.Info == null)
                {
                    return new NumericTimeDataSeries();
                }
                return Info.SmoothedElevationTrack;
            }
        }
        private INumericTimeDataSeries GradeTrack
        {
            get
            {
                if (this.Info == null)
                {
                    return new NumericTimeDataSeries();
                }
                return Info.SmoothedGradeTrack;
            }
        }

        private INumericTimeDataSeries HeartRatePerMinuteTrack
        {
            get
            {
                if (this.Info == null)
                {
                    return new NumericTimeDataSeries();
                }
                return Info.SmoothedHeartRateTrack;
            }
        }

        private INumericTimeDataSeries PowerWattsTrack
        {
            get
            {
                if (this.Info == null)
                {
                    return new NumericTimeDataSeries();
                }
                return Info.SmoothedPowerTrack;
            }
        }

        private INumericTimeDataSeries SpeedTrack
        {
            get
            {
                if (this.Info == null)
                {
                    return new NumericTimeDataSeries();
                }
                return Info.SmoothedSpeedTrack;
            }
        }

        #endregion

        //Create a copy of this result as an activity
        //Incomplete right now, used to create trails from results
        public IActivity CopyToActivity()
        {
            IActivity activity = Plugin.GetApplication().Logbook.Activities.Add(this.StartTime);
            activity.Category = this.Activity.Category;
            activity.GPSRoute = this.GPSRoute;
            if (Settings.RunningGradeAdjustMethod != RunningGradeAdjustMethodEnum.None ||
                TrailsPlugin.Data.Settings.AdjustDiffSplitTimes != null)
            {
                IGPSRoute gpsRoute = new GPSRoute();
                IDistanceDataTrack dt = this.GradeRunAdjustedTimeTrack(this.m_cacheTrackRef);
                foreach (ITimeValueEntry<IGPSPoint> g in this.GPSRoute)
                {
                    DateTime d1 = this.GPSRoute.EntryDateTime(g);
                    DateTime d2 = this.getDateTimeFromTimeResult(dt.GetInterpolatedValue(d1).Value);
                    gpsRoute.Add(d2, g.Value);
                }
                activity.GPSRoute = gpsRoute;
            }
            activity.TimeZoneUtcOffset = this.Activity.TimeZoneUtcOffset;
            activity.TimerPauses.Clear();
            foreach (IValueRange<DateTime> t in this.Pauses)
            {
                activity.TimerPauses.Add(t);
            }
            return activity;
        }

        /**********************************************************/
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

        public override string ToString()
        {
            return (this.m_startTime != null ? m_startTime.ToString() : "") + " " +
                ((this.TrailPointDateTime.Count == 0) ? "" : this.TrailPointDateTime[0].ToShortTimeString()) +
                " " + this.TrailPointDateTime.Count;
        }
    }
}
