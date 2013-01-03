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
using TrailsPlugin.Utils;
using GpsRunningPlugin.Util;

namespace TrailsPlugin.Data
{
    public class ChildTrailResult : TrailResult
    {
        internal bool PartOfParent = true;

        private TrailResult m_parentResult;
        public TrailResult ParentResult
        {
            get
            {
                return m_parentResult;
            }
        }

        public ChildTrailResult(ActivityTrail activityTrail, TrailResult par, int order, TrailResultInfo indexes, float distDiff) :
            base(activityTrail, order, indexes, distDiff)
        {
            this.m_parentResult = par;
            if (par != null)
            {
                if (par.m_childrenResults == null)
                {
                    par.m_childrenResults = new List<ChildTrailResult>();
                }
                par.m_childrenResults.Add(this);
            }
        }

        public ChildTrailResult(ActivityTrail activityTrail, TrailResult par, int order, TrailResultInfo indexes, float distDiff, string tt) :
            this(activityTrail, par, order, indexes, distDiff)
        {
            this.m_toolTip = tt;
            this.PartOfParent = false;
        }

    }

    public class TrailResult : ITrailResult, IComparable
    {
        #region private variables
        public ActivityTrail m_activityTrail;
        private IActivity m_activity;
        protected int m_order;
        private string m_name;
        private bool m_reverse;

        internal IList<ChildTrailResult> m_childrenResults;
        private TrailResultInfo m_subResultInfo;

        private bool? m_includeStopped;
        private DateTime? m_startTime;
        private DateTime? m_endTime;
        private float? m_predictDistance;
        private float m_startDistance = float.NaN;
        private float m_totalDistDiff; //to give quality of results
        private Color m_trailColor = getColor(nextTrailColor++);
        protected string m_toolTip;
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
        IList<double> m_trailPointTime0;
        IList<double> m_trailPointDist0;
        private double? m_ascent;
        private double? m_descent;

        private IGPSRoute m_gpsTrack;
        private IList<IList<IGPSPoint>> m_gpsPoints;

        //common for all - UR uses activities and this is a cache for resultList too
        //This could well be moved to another cache
        private static IActivity m_cacheTrackActivity;
        private static IDictionary<IActivity, IItemTrackSelectionInfo[]> m_commonStretches;
        private static ActivityInfoOptions m_TrailActivityInfoOptions;
        internal IList<IActivity> SameTimeActivities = null;
        #endregion

        /**********************************************************/
        #region contructors
        //Normal TrailResult
        public TrailResult(ActivityTrail activityTrail, int order, TrailResultInfo indexes, float distDiff, bool reverse)
            : this(activityTrail, order, indexes, distDiff)
        {
            this.m_reverse = reverse;
        }

        //Results from Splits
        public TrailResult(ActivityTrail activityTrail, int order, TrailResultInfo indexes, float distDiff)
        {
            createTrailResult(activityTrail, order, indexes, distDiff);
        }

        //HighScore result
        public TrailResult(ActivityTrail activityTrail, int order, TrailResultInfo indexes, float distDiff, string toolTip)
            : this(activityTrail, order, indexes, distDiff)
        {
            m_toolTip = toolTip;
        }

        //Create from splits
        //public TrailResult(ActivityTrail activityTrail, Trail trail, IActivity activity, int order)
        //{
        //    TrailResultInfo indexes;
        //    Data.Trail.TrailGpsPointsFromSplits(activity, out indexes);
        //    createTrailResult(activityTrail, order, indexes, float.MaxValue);
        //}

        //Summary result (avoid having createTrailResult more public)
        public TrailResult()
        {
            //a summary result is not related to an activity trail
            createTrailResult(null, 0, new TrailResultInfo(null, false), 0);
            m_toolTip = "";
            m_trailColor = Color.Black;
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

        public IList<ChildTrailResult> getSplits()
        {
            IList<ChildTrailResult> splits = new List<ChildTrailResult>();
            if (this.m_subResultInfo.Count > 1)
            {
                int i; //start time index
                for (i = 0; i < m_subResultInfo.Count - 1; i++)
                {
                    if (m_subResultInfo.Points[i].Time != DateTime.MinValue)
                    {
                        int j; //end time index
                        for (j = i + 1; j < m_subResultInfo.Points.Count; j++)
                        {
                            if (m_subResultInfo.Points[j].Time != DateTime.MinValue)
                            {
                                break;
                            }
                        }
                        if (this.m_subResultInfo.Count > i &&
                            this.m_subResultInfo.Count > j)
                        {
                            if (m_subResultInfo.Points[j].Time != DateTime.MinValue)
                            {
                                TrailResultInfo t = m_subResultInfo.CopySlice(i, j);
                                ChildTrailResult tr = new ChildTrailResult(m_activityTrail, this, i + 1, t, m_totalDistDiff);
                                //if (aActivities.Count > 1)
                                //{
                                //    nextTrailColor--;
                                //    tr.m_trailColor = this.m_trailColor;
                                //}
                                splits.Add(tr);
                            }
                        }
                        i = j - 1;//Next index to try
                    }
                }
            }
            return splits;
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
                    m_ActivityInfo = null;
                    m_pauses = null;
                }
            }
        }

        public static void Reset()
        {
            nextTrailColor = 1;
            //nextActivityColor = 1;
        }
        #endregion

        /**********************************************************/
        #region basic fields

        public virtual IActivity Activity
        {
            get { return m_activity; }
        }
        //Distance error used to sort results
        public float DistDiff
        {
            get { return (float)(m_totalDistDiff / Math.Pow(m_subResultInfo.Count, 1.5)); }
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
        public virtual TimeSpan Duration
        {
            get
            {
                if (!(this is ChildTrailResult) &&
                    TrailsPlugin.Data.Settings.ResultSummaryIsDevice &&
                    this.Activity != null)
                {
                    return this.Activity.TotalTimeEntered;
                }
                return this.getTimeSpanResult(EndTime);
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
                            m_startTime = Info.ActualTrackStart;
                            if (m_startTime == DateTime.MinValue)
                            {
                                m_startTime = this.Activity.StartTime;
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
                            m_endTime = Info.ActualTrackEnd;
                            if (m_endTime == DateTime.MinValue)
                            {
                                m_endTime = Info.EndTime;
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
                    if (i >= 0 && this.m_activityTrail != null && i < this.m_activityTrail.Trail.TrailLocations.Count)
                    {
                        ITimeValueEntry<IGPSPoint> entry = this.Activity.GPSRoute.GetInterpolatedValue(StartTime);
                        if (entry != null)
                        {
                            startDistance = this.m_activityTrail.Trail.TrailLocations[i].DistanceMetersToPoint(entry.Value);
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
            return TrackUtil.getDistFromDateTime(ActivityDistanceMetersTrack, t);
        }
        public float getDistResult(DateTime t)
        {
            return TrackUtil.getDistFromDateTime(DistanceMetersTrack, t);
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
        #endregion

        /**********************************************************/
        #region basic tracks
        bool isIncludeStoppedCategory(IActivityCategory category)
        {
            if (category == null || Settings.ExcludeStoppedCategory == null || Settings.ExcludeStoppedCategory == "")
            {
                return true;
            }
            else
            {
                String[] values = Settings.ExcludeStoppedCategory.Split(';');
                foreach (String name in values)
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
                pauses.Add(new ValueRange<DateTime>( DateTime.MinValue, this.StartTime.AddSeconds(-1) ));
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
                if (this is ChildTrailResult && (this as ChildTrailResult).PartOfParent)
                {
                    return (this as ChildTrailResult).ParentResult.Pauses;
                }
                if (m_pauses == null)
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
                        //Check for active laps first
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
                                    DateTime upper;
                                    if (i < Activity.Laps.Count - 1)
                                    {
                                        upper = Activity.Laps[i + 1].StartTime;
                                        if (!Activity.Laps[i + 1].Rest)
                                        {
                                            upper -= TimeSpan.FromSeconds(1);
                                        }
                                        //Fix: Lap start time is in seconds, precision could be lost
                                        DateTime upper2 = lap.StartTime.Add(lap.TotalTime);
                                        if (upper.Millisecond == 0 &&  Math.Abs((upper2 - upper).TotalSeconds) < 2)
                                        {
                                            upper = upper2;
                                        }
                                    }
                                    else
                                    {
                                        upper = Info.EndTime;
                                    }
                                    m_pauses.Add(new ValueRange<DateTime>(lap.StartTime, upper));
                                }
                            }
                        }
                    }
                    if (Settings.RestIsPause)
                    {
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
                    m_activityDistanceMetersTrack.AllowMultipleAtSameTime = false;
                    if (m_activityDistanceMetersTrack != null)
                    {
                        //insert points at borders in m_activityDistanceMetersTrack
                        //Less special handling when transversing the activity track
                        m_activityDistanceMetersTrack = (DistanceDataTrack)(new InsertValues<float>(this, m_activityDistanceMetersTrack, m_activityDistanceMetersTrack)).
                            insertValues();
                    }
                }
                if (m_activityDistanceMetersTrack != null)
                {
                    int i = 0;
                    float distance = 0;
                    uint oldElapsed = 0;

                    m_distanceMetersTrack.Add(StartTime, distance);
                    m_startDistance = TrackUtil.getDistFromDateTime(m_activityDistanceMetersTrack, StartTime);
                    //Prune search
                    //Note that the distance track may not start at result StartTime
                    while (i < m_activityDistanceMetersTrack.Count &&
                        this.StartTime > m_activityDistanceMetersTrack.EntryDateTime(m_activityDistanceMetersTrack[i]))
                    {
                        i++;
                    }

                    float prevDist = m_startDistance;
                    while (i < m_activityDistanceMetersTrack.Count &&
                        EndTime > m_activityDistanceMetersTrack.EntryDateTime(m_activityDistanceMetersTrack[i]))
                    {
                        ITimeValueEntry<float> timeValue = m_activityDistanceMetersTrack[i];
                        DateTime dateTime = m_activityDistanceMetersTrack.EntryDateTime(timeValue);
                        if (!ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(dateTime, Pauses))
                        {
                            uint elapsed = timeValue.ElapsedSeconds;
                            if (elapsed > oldElapsed || oldElapsed == 0)
                            {
                                float actDist = timeValue.Value;
                                if (!float.IsNaN(prevDist))
                                {
                                    //TODO: Get the offsets at boundaries, instead of inserting values
                                    distance += actDist - prevDist;
                                }
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

                    //Set real last distance, even if elapsedSec is not matching
                    if (!float.IsNaN(prevDist))
                    {
                        distance += TrackUtil.getDistFromDateTime(this.m_activityDistanceMetersTrack, EndTime) - prevDist;
                        m_distanceMetersTrack.Add(EndTime, distance);
                    }
                }
            }
        }

        //Distance track for trail, adjusted with pauses
        private IDistanceDataTrack DistanceMetersTrack
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
                    string s = string.Format("{0} {1} {2}", Activity.StartTime.ToLocalTime(), Activity.Name, Activity.Notes.Substring(0, Math.Min(Activity.Notes.Length, 40)));
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
                m_ascent = Info.TotalAscendingMeters(Plugin.GetApplication().DisplayOptions.SelectedClimbZone);
                m_descent = Info.TotalDescendingMeters(Plugin.GetApplication().DisplayOptions.SelectedClimbZone);
            }
            else
            {
                m_ascent = 0;
                m_descent = 0;
                //Note: Using Trails Info inflates the values slightly, also standard Info does a little (due to points added)
                //Attempt to adjust factor, may need tuning
                ActivityInfo info = this.Info;
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

        public virtual double ElevChg
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
                if(m_predictDistance==null)
                {
                           if (TrailsPlugin.Integration.PerformancePredictor.PerformancePredictorIntegrationEnabled)
            {
 IList<TrailsPlugin.Integration.PerformancePredictor.PerformancePredictorResult> t = 
     TrailsPlugin.Integration.PerformancePredictor.PerformancePredictorFields(
     new List<IActivity>{this.Activity},new List<double>{this.Duration.TotalSeconds},new List<double>{this.Distance},new List<double>{Settings.PredictDistance},null);
                               if(t!=null && t.Count>0&&t[0]!=null)
                               {
                                   m_predictDistance=(float)t[0].predicted[0].new_time;
                               }
                           }
                if(m_predictDistance==null)
                {
                    m_predictDistance=float.NaN;
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
            if (refRes == null || refRes != m_cacheTrackRef)
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

        //Insert points at start/end and pauses
        //This simplifies tranversing distancetrack
        //It was originally added as an attempt to handle selections and improve averages etc
        internal class InsertValues<T>
        {
            TrailResult result;
            ITimeDataSeries<T> track;
            ITimeDataSeries<T> source;

            public InsertValues(TrailResult t, ITimeDataSeries<T> track, ITimeDataSeries<T> source)
            {
                this.result = t;
                this.track = track;
                this.source = source;
            }
            public void insertValues(DateTime atime)
            {
                if (atime >= result.StartTime && atime <= result.EndTime &&
                    !ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(atime, result.Pauses))
                {
                    ITimeValueEntry<T> interpolatedP = this.source.GetInterpolatedValue(atime);
                    if (interpolatedP != null)
                    {
                        try
                        {
                            this.track.Add(atime, interpolatedP.Value);
                        }
                        catch { }
                    }
                }
            }
            public ITimeDataSeries<T> insertValues()
            {
                //Insert points around pauses and points
                //This is needed to get the track match the cut-up activity
                //(otherwise for instance start point need to be added)

                //Bug in ST 3.0.4205 not resorting
                int noPoints = track.Count;

                //start/end should be included from points, but prepare for changes...
                insertValues(result.StartTime);
                insertValues(result.EndTime);

                foreach (IValueRange<DateTime> p in result.Pauses)
                {
                    if (p.Lower > DateTime.MinValue)
                    {
                        insertValues(p.Lower.AddSeconds(-1));
                    }
                    insertValues(p.Upper.AddSeconds(1));
                }
                foreach (TrailResultPoint t in result.m_subResultInfo.Points)
                {
                    DateTime dateTime = t.Time;
                    if (dateTime > DateTime.MinValue)
                    {
                        insertValues(dateTime.AddSeconds(-1));
                        insertValues(dateTime);
                    }
                }

                if (noPoints > 0)
                {
                    //AllowMultipleAtSameTime=false does not work in 3.0.4205
                    bool reSort = false;
                    for (int i = noPoints; i < track.Count; i++)
                    {
                        if (track[noPoints - 1].ElapsedSeconds > track[i].ElapsedSeconds)
                        {
                            reSort = true;
                            break;
                        }
                    }
                    if (reSort)
                    {
                        SortedDictionary<uint, ITimeValueEntry<T>> dic = new SortedDictionary<uint, ITimeValueEntry<T>>();
                        foreach (ITimeValueEntry<T> g in track)
                        {
                            if (!dic.ContainsKey(g.ElapsedSeconds))
                            {
                                dic.Add(g.ElapsedSeconds, g);
                            }
                        }
                        DateTime startTime = track.StartTime;
                        track.Clear();
                        foreach (KeyValuePair<uint, ITimeValueEntry<T>> g in dic)
                        {
                            track.Add(startTime.AddSeconds(g.Value.ElapsedSeconds), g.Value.Value);
                        }
                    }
                }
                //Return the original track, the typecasting must work
                return this.track;
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
                                track.SetValueAt(i - addOffset - tTrack.Count + 1 + j, tTrack[j].Value);
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
                if (insert)
                {
                    //Insert values around borders, to limit effects when track is chopped
                    //Do this before other additions, so start is StartTime for track
                    //track = (INumericTimeDataSeries)(new InsertValues<float>(this, track, source)).insertValues();
                }
                int oldElapsed = int.MinValue;
                foreach (ITimeValueEntry<float> t in source)
                {
                    DateTime dateTime = source.EntryDateTime(t);
                    if (this.StartTime <= dateTime && dateTime <= this.EndTime &&
                        !ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(dateTime, this.Pauses))
                    {
                        uint elapsed = t.ElapsedSeconds;
                        if (elapsed > oldElapsed)
                        {
                            track.Add(dateTime, (float)convert(t.Value, refActivity));
                            oldElapsed = (int)elapsed;
                        }
                    }
                    if (dateTime > this.EndTime)
                    {
                        break;
                    }
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
                m_distanceMetersTrack0 = new DistanceDataTrack();
                foreach (ITimeValueEntry<float> entry in this.DistanceMetersTrack)
                {
                    float val = (float)UnitUtil.Distance.ConvertFrom(entry.Value, m_cacheTrackRef.Activity);
                    if (!float.IsNaN(val))
                    {
                        DateTime dateTime = DistanceMetersTrack.EntryDateTime(entry);
                        m_distanceMetersTrack0.Add(dateTime, val);
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
                    new Convert(UnitUtil.Elevation.ConvertFrom), this.m_cacheTrackRef);
            }
            return m_elevationMetersTrack0;
        }

        public INumericTimeDataSeries GradeTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_gradeTrack0 == null)
            {
                m_gradeTrack0 = copySmoothTrack(this.GradeTrack, true, TrailActivityInfoOptions.ElevationSmoothingSeconds,
                    new Convert(UnitUtil.Grade.ConvertFrom), this.m_cacheTrackRef);
            }
            return m_gradeTrack0;
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

        public INumericTimeDataSeries SpeedTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_speedTrack0 == null)
            {
                m_speedTrack0 = copySmoothTrack(this.SpeedTrack, false, TrailActivityInfoOptions.SpeedSmoothingSeconds,
                                 new Convert(UnitUtil.Speed.ConvertFrom), this.m_cacheTrackRef);
            }
            return m_speedTrack0;
        }

        public INumericTimeDataSeries PaceTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_paceTrack0 == null)
            {
                INumericTimeDataSeries paceTrack;
                if (Settings.RunningGradeAdjustMethod == RunningGradeAdjustMethodEnum.None)
                {
                    paceTrack = this.SpeedTrack;
                }
                else
                {
                    if (!PaceTrackIsGradeAdjustedPaceAvg)
                    {
                        //Show pace, as it was flat
                        this.calcGradeRunAdjustedTime(this.m_cacheTrackRef);
                        paceTrack = new NumericTimeDataSeries();
                        foreach (ginfo t in m_grades)
                        {
                            //Add first point too?
                            if (t.time > 0)
                            {
                                paceTrack.Add(t.dateTime, t.dist / t.time / t.q);
                            }
                        }
                    }
                    else
                    {
                        //Show pace as it should have been with "even" pace"
                        paceTrack = new NumericTimeDataSeries();
                        INumericTimeDataSeries timeTrack = this.GradeRunAdjustedTimeTrack(this.m_cacheTrackRef);
                        for (int i = 1; i < timeTrack.Count; i++)
                        {
                            ginfo t = this.m_grades[i];
                            paceTrack.Add(t.dateTime, t.dist / (timeTrack[i].Value - timeTrack[i - 1].Value));
                        }
                    }

                }
                //Smooth speed track, as smoothing pace gives incorrect data (when fast is close to slow)
                m_paceTrack0 = copySmoothTrack(paceTrack, false, TrailActivityInfoOptions.SpeedSmoothingSeconds,
                                new Convert(ConvertNone), this.m_cacheTrackRef);
                for (int i = 0; i < m_paceTrack0.Count; i++)
                {
                    m_paceTrack0.SetValueAt(i, (float)UnitUtil.Pace.ConvertFrom(m_paceTrack0[i].Value, this.m_cacheTrackRef.Activity));
                }
            }
            return m_paceTrack0;
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
                bool isPace = this.m_cacheTrackRef.Activity.Category.SpeedUnits.Equals(Speed.Units.Pace);
                m_deviceSpeedPaceTrack0 = new NumericTimeDataSeries();
                if (this.Activity != null && this.Activity.DistanceMetersTrack != null && this.Activity.DistanceMetersTrack.Count > 0)
                {
                    ITimeValueEntry<float> prev = null;
                    foreach (ITimeValueEntry<float> t in this.Activity.DistanceMetersTrack)
                    {
                        if (t != null)
                        {
                            if (prev == null)
                            {
                                prev = t;
                            }
                            DateTime dateTime = this.Activity.DistanceMetersTrack.EntryDateTime(t);
                            if (this.StartTime <= dateTime && dateTime <= this.EndTime &&
                                !ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(dateTime, this.Pauses))
                            {
                                //Ignore 0 time and infinity pace
                                if (t.ElapsedSeconds - prev.ElapsedSeconds > 0 && 
                                    (!isPace || (t.Value - prev.Value > 0)))
                                {
                                    float val = (float)Math.Abs( (t.Value - prev.Value) / (t.ElapsedSeconds - prev.ElapsedSeconds));
                                    m_deviceSpeedPaceTrack0.Add(dateTime, val);
                                }
                            }
                            if (t.ElapsedSeconds - prev.ElapsedSeconds > 0)
                            {
                                //Update previous only if more than one sec has passed
                                prev = t;
                            }
                            if (dateTime > this.EndTime)
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

        public INumericTimeDataSeries DeviceElevationTrack0(TrailResult refRes)
        {
            return DeviceElevationTrack0(refRes, TrailActivityInfoOptions.ElevationSmoothingSeconds);
        }

        public INumericTimeDataSeries DeviceElevationTrack0(TrailResult refRes, int eleSmooth)
        {
            checkCacheRef(refRes);
            if (m_deviceElevationTrack0 == null || eleSmooth != TrailActivityInfoOptions.ElevationSmoothingSeconds)
            {
                INumericTimeDataSeries deviceElevationTrack0 = new NumericTimeDataSeries();
                if (this.Activity != null && this.Activity.ElevationMetersTrack != null && this.Activity.ElevationMetersTrack.Count > 0)
                {
                    deviceElevationTrack0 = copySmoothTrack(this.Activity.ElevationMetersTrack, true, eleSmooth,
                       new Convert(UnitUtil.Elevation.ConvertFrom), this.m_cacheTrackRef);
                }
                else if (this.SameTimeActivities != null)
                {
                    foreach (IActivity activity in this.SameTimeActivities)
                    {
                        if (activity.ElevationMetersTrack != null && activity.ElevationMetersTrack.Count > 0)
                        {
                            deviceElevationTrack0 = copySmoothTrack(activity.ElevationMetersTrack, true, eleSmooth,
                               new Convert(UnitUtil.Elevation.ConvertFrom), this.m_cacheTrackRef);
                        }
                    }
                }

                if (TrailActivityInfoOptions.ElevationSmoothingSeconds == eleSmooth)
                {
                    m_deviceElevationTrack0 = deviceElevationTrack0;
                }
                else
                {
                    //"Temporary" track
                    return deviceElevationTrack0;
                }
            }
            return m_deviceElevationTrack0;
        }

        internal int SetDeviceElevation()
        {
            INumericTimeDataSeries eTrack = this.DeviceElevationTrack0(this.m_cacheTrackRef, 0);
            if (eTrack != null && eTrack.Count > 0 &&
                this.Activity != null &&
                this.GPSRoute != null && this.GPSRoute.Count > 0)
            {
                IGPSRoute gpsRoute = new GPSRoute();
                INumericTimeDataSeries eleTrack = new NumericTimeDataSeries();
                foreach (ITimeValueEntry<IGPSPoint> g in this.GPSRoute)
                {
                    DateTime d1 = this.GPSRoute.EntryDateTime(g);
                    ITimeValueEntry<float> val = eTrack.GetInterpolatedValue(d1);
                    float? ele = null;
                    if (val == null)
                    {
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
                            //Should not happen...
                            //ele = eTrack[0].Value;
                        }
                    }
                    else
                    {
                        ele = val.Value;
                    }
                    if (ele != null)
                    {
                        //ele += UI.Activity.TrailLineChart.FixedSyncGraphMode;
                        IGPSPoint g2 = new GPSPoint(g.Value.LatitudeDegrees, g.Value.LongitudeDegrees, (float)ele);
                        gpsRoute.Add(d1, g.Value);
                        eleTrack.Add(d1, (float)ele);
                    }
                }
                //TBD this.Activity.GPSRoute = gpsRoute;
                this.Activity.ElevationMetersTrack = eleTrack;
            }
            return 0;
        }

        internal int SetDeviceElevationOffset()
        {
            if (this.Activity != null && this.Activity.ElevationMetersTrack != null &&
                this.Activity.ElevationMetersTrack.Count > 0 && Settings.ChartType == LineChartTypes.Elevation &&
                UI.Activity.TrailLineChart.FixedSyncGraphMode != 0)
            {
                for (int i = 0; i < this.ElevationMetersTrack.Count; i++)
                {
                    this.ElevationMetersTrack.SetValueAt(i, this.ElevationMetersTrack[i].Value - UI.Activity.TrailLineChart.FixedSyncGraphMode);
                }
            }
            return 0;
        }

        public INumericTimeDataSeries DeviceDiffDistTrack0(TrailResult refRes)
        {
            checkCacheRef(refRes);
            if (m_deviceDiffDistTrack0 == null)
            {
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
                            float val = (float)UnitUtil.Elevation.ConvertFrom(-t.Value + t2.Value - (float)start2);
                            m_deviceDiffDistTrack0.Add(dateTime, val);
                        }
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
            foreach (TrailResult t in TrailResultWrapper.AllResults(TrailsPlugin.Controller.TrailController.Instance.CurrentResultTreeList))
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

            public ginfo(DateTime dateTime, float time, float dist, float totDist, float q)
            {
                this.dateTime = dateTime;
                this.time = time;
                this.dist = dist;
                this.totDist = totDist;
                this.q = q;
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

        public float GradeRunAdjustedSpeed
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
        //TODO timeofday
        public static bool UseNormalElevation = true; 
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
                float? prevElev = null;
                float prevDist = 0;
                INumericTimeDataSeries eleTrack;
                if (UseNormalElevation)
                {
                    eleTrack = this.ElevationMetersTrack0(this.m_cacheTrackRef);
                }
                else
                {
                    eleTrack = this.DeviceElevationTrack0(this.m_cacheTrackRef);
                }

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
                        float elev = (float)UnitUtil.Elevation.ConvertFrom(t2.Value);//convert back...
                        //This point is valid, at least for next
                        if (prevElev != null)
                        {
                            if (dist - prevDist > 0)
                            {
                                float g = (elev - (float)prevElev) / (dist - prevDist); //grade
                                q = RunningGradeAdjustMethodClass.getGradeFactor(g, time, prevTime, dist, prevDist);
                            }
                        }
                        else
                        {
                        }
                        prevElev = elev;
                    }
                    else
                    {
                    }
                    float val = prevVal + q * (time - prevTime);
                    m_gradeRunAdjustedTime.Add(dateTime, val);
                    prevVal = val;
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
                    if (i > 3)
                    { }
                    foreach (ginfo t in m_grades)
                    {
                        m_gradeRunAdjustedTimeAvg.Add(t.dateTime, t.adjTime);
                        if (Math.Abs(t.adjTime - this.getTimeResult(t.dateTime)) > 4)
                        {
                        }
                        if (Math.Abs(t.adjTime - this.getTimeResult(t.dateTime)) <2)
                        {
                        }
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
                parRes != null && parRes.m_childrenResults != null)
            {
                //This is a subsplit, get the subsplit related to the ref
                foreach (TrailResult tr in parRes.m_childrenResults)
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
                        lastValue = (float)(trRef.TrailPointTime0(trRef)[dateTrailPointIndex] - this.TrailPointTime0(trRef)[dateTrailPointIndex] + diffOffset);
                        m_DiffTimeTrack0.Add(EndTime, lastValue);
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
                                            double refDistP = TrackUtil.getDistFromDateTime(trRef.DistanceMetersTrack,
                                                trRef.TrailPointDateTime[dateTrailPointIndex], out status2);
                                            if (status2 == 0)
                                            {
                                                //getDistResult
                                                double distP = TrackUtil.getDistFromDateTime(this.DistanceMetersTrack,
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
                                            refDist = TrackUtil.getDistFromDateTime(trRef.DistanceMetersTrack, d2, out status);
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
                        lastValue = (float)UnitUtil.Elevation.ConvertFrom(trRef.TrailPointDist0(trRef)[dateTrailPointIndex] - 
                            this.TrailPointDist0(trRef)[dateTrailPointIndex] + diffOffset, trRef.Activity);
                        m_DiffDistTrack0.Add(EndTime, lastValue);
                    }
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
            foreach (float f in m_trailPointDistOffset)
            {
                trailPointDistOffset0.Add(UnitUtil.Distance.ConvertFrom(f, m_cacheTrackRef.Activity));
            }

            return trailPointDistOffset0;
        }

        public IList<double> TrailPointTime0(TrailResult refRes)
        {
            checkCacheRef(refRes);
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
                        DateTime t1 = TrackUtil.getFirstUnpausedTime(this.TrailPointDateTime[k], Pauses, true);
                        //Same as val = getTimeSpanResult(t1);
                        val = ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.TimeNotPaused(
                                StartTime, t1, Pauses).TotalSeconds;
                        //Offset time from detected to actual start
                        //NonMovingTimes here instead?
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
                    double val = 0, val1 = 0;
                    if (TrailPointDateTime[k] > DateTime.MinValue)
                    {
                        ITimeValueEntry<float> interpolatedP = DistanceMetersTrack0(m_cacheTrackRef).GetInterpolatedValue(TrackUtil.getFirstUnpausedTime(this.TrailPointDateTime[k], Pauses, true));
                        if (interpolatedP != null)
                        {
                            val = interpolatedP.Value;
                        }
                        else if (k > 0)
                        {
                            val = m_trailPointDist0[k - 1];
                        }

                        //Use "ST" distance track to get offset from detected start (that could be paused) to used start
                        interpolatedP = ActivityDistanceMetersTrack.GetInterpolatedValue(TrackUtil.getFirstUnpausedTime(this.TrailPointDateTime[k], Pauses, true));
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
                //Insert values at borders in m_gpsTrack
                gpsTrack = (GPSRoute)(new InsertValues<IGPSPoint>(this, gpsTrack, m_activity.GPSRoute)).insertValues();

                int i = 0;
                while (i < m_activity.GPSRoute.Count)
                {
                    DateTime dateTime = m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[i]);
                    if (this.StartTime.AddSeconds(1) <= dateTime)
                    {
                        break;
                    }
                    i++;
                }
                while (i < m_activity.GPSRoute.Count)
                {
                    DateTime dateTime = m_activity.GPSRoute.EntryDateTime(m_activity.GPSRoute[i]);
                    if (dateTime >= this.EndTime)
                    {
                        break;
                    }

                    if (!ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(dateTime, Pauses))
                    {
                        IGPSPoint point = m_activity.GPSRoute[i].Value;
                        gpsTrack.Add(dateTime, point);
                    }
                    i++;
                }
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
                if (!m_colorOverridden && Controller.TrailController.Instance.Activities.Count > 1 &&
                    (this is ChildTrailResult) && (this as ChildTrailResult).PartOfParent)
                {
                    return (this as ChildTrailResult).ParentResult.m_trailColor;
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
            switch (color % 10)
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

        ActivityInfo m_ActivityInfo = null;
        ActivityInfo Info
        {
            get
            {
                //Custom InfoCache, to control smoothing (otherwise ST could cache)
                if (this is ChildTrailResult && (this as ChildTrailResult).PartOfParent)
                {
                    return (this as ChildTrailResult).ParentResult.Info;
                }
                if (m_ActivityInfo == null)
                {
                    ActivityInfoCache c = new ActivityInfoCache();
                    ActivityInfoOptions t = new ActivityInfoOptions(false, this.IncludeStopped);
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
                    if (activity != null)
                    {
                        m_ActivityInfo = c.GetInfo(activity);
                    }
                    else
                    {
                        //TODO: This data should not be used, just return any activity to avoid exceptions
                        m_ActivityInfo = ActivityInfoCache.Instance.GetInfo(Plugin.GetApplication().Logbook.Activities[0]);
                    }
                }
                return m_ActivityInfo;
            }
        }

        //The Activity related tracks were previously cached. Kept as an uniform method to get
        //the tracks, if the Info cache handling is changed
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
        #region Implementation of ITrailResult

        float ITrailResult.AvgCadence
        {
            get { return AvgCadence; }
        }

        float ITrailResult.AvgGrade
        {
            get { return 100*AscAvgGrade; }
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
            get { return EndTimeOfDay; }
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
            get
            {
                return copySmoothTrack(this.GradeTrack, true, TrailActivityInfoOptions.ElevationSmoothingSeconds,
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
            get { return StartTimeOfDay; }
        }

        #endregion

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
