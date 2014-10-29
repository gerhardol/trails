/*
Copyright (C) 2012 Gerhard Olsson

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

#if ST_3_0
//ST 3.1.5314 contains memory optimisations
//We assume that the plugin is only forward compatible, only change this file, not the .csproj file
#define ST_3_1_5314
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Visuals.Fitness;

using TrailsPlugin.Data;
using GpsRunningPlugin.Util;

namespace TrailsPlugin.Utils
{
    public static class TrackUtil
    {
        //Changed interfaces in ST, memory performance improvements
#if ST_3_1_5314
        internal class NumericTimeDataSeries : ZoneFiveSoftware.Common.Data.NumericTimeDataSeries2 {}
        internal class DistanceDataTrack : ZoneFiveSoftware.Common.Data.DistanceDataTrack2 { }
        internal class GPSRoute : ZoneFiveSoftware.Common.Data.GPS.GPSRoute2 { }
#else
        internal class NumericTimeDataSeries : ZoneFiveSoftware.Common.Data.NumericTimeDataSeries { }
        internal class DistanceDataTrack : ZoneFiveSoftware.Common.Data.DistanceDataTrack { }
        internal class GPSRoute : ZoneFiveSoftware.Common.Data.GPS.GPSRoute { }
#endif

        internal static void setCapacity(INumericTimeDataSeries t, int v)
        {
#if ST_3_1_5314
            if (t is ZoneFiveSoftware.Common.Data.NumericTimeDataSeries2)
            {
                ((ZoneFiveSoftware.Common.Data.NumericTimeDataSeries2)t).Capacity = v;
            }
#endif
        }

        internal static void setCapacity(IDistanceDataTrack t, int v)
        {
#if ST_3_1_5314
            if (t is ZoneFiveSoftware.Common.Data.DistanceDataTrack2)
            {
                ((ZoneFiveSoftware.Common.Data.DistanceDataTrack2)t).Capacity = v;
            }
#endif
        }

        internal static void setCapacity(IGPSRoute t, int v)
        {
#if ST_3_1_5314
            if (t is ZoneFiveSoftware.Common.Data.GPS.GPSRoute2)
            {
                ((ZoneFiveSoftware.Common.Data.GPS.GPSRoute2)t).Capacity = v;
            }
#endif
        }

        /**********************************/

        internal static float getValFromDateTime(INumericTimeDataSeries track, DateTime t)
        {
            int status;
            return getValFromDateTime(track, t, out status);
        }

        internal static float getValFromDateTime(INumericTimeDataSeries track, DateTime t, out int status)
        {
            //Ignore malformed activities and selection outside the result
            float res = 0;
            status = 1;
            ITimeValueEntry<float> entry = track.GetInterpolatedValue(t);
            if (entry != null)
            {
                res = entry.Value;
                status = 0;
            }
            else if (track.Count > 0 && t >= track.StartTime.AddSeconds(2))
            {
                //Seem to be out of bounds
                //Any time after start is handled as end, as pauses may complicate calcs (default is 0, i.e. before)
                res = track[track.Count - 1].Value;
            }
            return res;
        }

        //ST interpolate is not handling subseconds, interpolate better if possible
        internal static T getValFromDateTimeIndex<T>(ITimeDataSeries<T> track, DateTime time, int i)
        {
            T dist;

            if (i < 0 || i >= track.Count || track.Count <= 1)
            {
                //index out of bound
                //Note that the distance track may not start at result StartTime, then this will report result at 0 sec
                Debug.Assert(true, "index out of bounds");
                i = Math.Max(i, 0);
                i = Math.Min(i, track.Count-1);
                dist = track[i].Value;
            }
            else if (time == track.EntryDateTime(track[i]))
            {
                //Exact time, no interpolation
                dist = track[i].Value;
            }
            else
            {
                if (i == 0)
                {
                    i = 1;
                }
                float elapsed = (float)((time - track.StartTime).TotalSeconds);
                float f = (elapsed - track[i - 1].ElapsedSeconds) / (float)(track[i].ElapsedSeconds - track[i - 1].ElapsedSeconds);
                dist = track[i - 1].Value; //assignment for compiler
                if (dist is float)
                {
                    float v0 = (float)(object)(track[i - 1].Value);
                    float v1 = (float)(object)(track[i].Value);
                    dist = (T)(object)(v0 + (v1 - v0) * f);
                }
                else if (dist is IGPSPoint)
                {
                    IGPSPoint v0 = (IGPSPoint)(object)(track[i - 1].Value);
                    IGPSPoint v1 = (IGPSPoint)(object)(track[i].Value);
                    float lat = (v0.LatitudeDegrees + (v1.LatitudeDegrees - v0.LatitudeDegrees) * f);
                    float lon = (v0.LongitudeDegrees + (v1.LongitudeDegrees - v0.LongitudeDegrees) * f);
                    float ele = (v0.ElevationMeters + (v1.ElevationMeters - v0.ElevationMeters) * f);
                    dist = (T)(object)(new GPSPoint(lat, lon, ele));
                }
                else
                {
                    //Added to satisfy compiler - not possible to configure
                    Debug.Assert(true, "Unexpected ITimeDataSeries");
                    dist = default(T);
                }
            }
            return dist;
        }

        //Add a point last in the track (no check), override if point already exists
        internal static void trackAdd(INumericTimeDataSeries track, DateTime time, float val)
        {
            track.Add(time, val);
            //Check if the entry already existed, overwrite if necessary
            if (track[track.Count - 1].Value != val)
            {
                //(uint)(this.EndTime - this.StartTime.AddSeconds(-this.StartTime.Second)).TotalSeconds == m_activityDistanceMetersTrack[i - 1].ElapsedSeconds)
                track.SetValueAt(track.Count - 1, val);
            }
        }

        /******************************************************/
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
            IActivity activity = refRes == null ? null : refRes.Activity;
            return UnitUtil.Distance.ConvertTo(t, activity);
        }

        public static double DistanceConvertFrom(double t, TrailResult refRes)
        {
            IActivity activity = refRes == null ? null : refRes.Activity;
            return UnitUtil.Distance.ConvertFrom(t, activity);
        }

        /***************************************************/

        internal static DateTime getFirstUnpausedTime(DateTime dateTime, IValueRangeSeries<DateTime> pauses, bool next)
        {
            return getFirstUnpausedTime(dateTime, ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(dateTime, pauses), pauses, next);
        }

        internal static DateTime getFirstUnpausedTime(DateTime dateTime, bool isPause, IValueRangeSeries<DateTime> pauses, bool next)
        {
            if (isPause)
            {
                foreach (IValueRange<DateTime> pause in pauses)
                {
                    if (dateTime.CompareTo(pause.Lower) >= 0 &&
                        dateTime.CompareTo(pause.Upper) <= 0)
                    {
                        if (next)
                        {
                            dateTime = (pause.Upper);//.Add(TimeSpan.FromSeconds(1));
                        }
                        else if (pause.Lower > DateTime.MinValue)
                        {
                            dateTime = (pause.Lower);//.Add(TimeSpan.FromSeconds(-1));
                        }
                        break;
                    }
                }
            }
            return dateTime;
        }

        public static IGPSPoint getGpsLoc(ZoneFiveSoftware.Common.Data.Fitness.IActivity activity, DateTime time)
        {
            IGPSPoint result = null;
            if (activity != null && activity.GPSRoute != null && activity.GPSRoute.Count > 0)
            {
                ITimeValueEntry<IGPSPoint> p = activity.GPSRoute.GetInterpolatedValue(time);
                if (null != p)
                {
                    result = p.Value;
                }
                else
                {
                    //This can happen if the activity starts before the GPS track
                    if (time <= activity.GPSRoute.StartTime)
                    {
                        result = activity.GPSRoute[0].Value;
                    }
                    else if (time > activity.GPSRoute.EntryDateTime(activity.GPSRoute[activity.GPSRoute.Count - 1]))
                    {
                        result = activity.GPSRoute[activity.GPSRoute.Count - 1].Value;
                    }
                }
            }
            return result;
        }

        /*******************************************************/

        private static float GetTimeChartResultFromDateTime(bool isOffset, TrailResult tr, TrailResult ReferenceTrailResult, DateTime d1)
        {
            float x1 = (float)(tr.getTimeResult(d1));
            if (isOffset)
            {
                float nextElapsed;
                x1 += GetChartResultsResyncOffset(true, tr, ReferenceTrailResult, x1, out nextElapsed);
            }
            x1 += tr.GetXOffset(true, ReferenceTrailResult);
            return x1;
        }

        private static float GetDistChartResultFromDateTime(bool isOffset, TrailResult tr, TrailResult ReferenceTrailResult, DateTime d1)
        {
            float x1 = (float)(tr.getDistResult(d1));
            return GetDistChartResultFromDistResult(isOffset, tr, ReferenceTrailResult, x1);
        }

        private static float GetDistChartResultFromDistResult(bool isOffset, TrailResult tr, TrailResult ReferenceTrailResult, double t1)
        {
            //distance is for result, then to display units
            float x1 = (float)t1;
            if (isOffset)
            { 
                float nextElapsed;
                x1 += GetChartResultsResyncOffset(false, tr, ReferenceTrailResult, x1, out nextElapsed);
            }
            x1 += tr.GetXOffset(false, ReferenceTrailResult);
            x1 = (float)TrackUtil.DistanceConvertFrom(x1, ReferenceTrailResult);
            return x1;
        }

        private static float[] GetTimeChartResultFromDateTime(bool isOffset, TrailResult tr, TrailResult ReferenceTrailResult, DateTime d1, DateTime d2)
        {
            //Convert to distance display unit, Time is always in seconds
            float x1 = GetTimeChartResultFromDateTime(isOffset, tr, ReferenceTrailResult, d1);
            float x2 = GetTimeChartResultFromDateTime(isOffset, tr, ReferenceTrailResult, d2);
            return new float[] { x1, x2 };
        }

        private static float[] GetDistChartResultFromDistResult(bool isOffset, TrailResult tr, TrailResult ReferenceTrailResult, double t1, double t2)
        {
            //distance is for result, then to display units
            float x1 = GetDistChartResultFromDistResult(isOffset, tr, ReferenceTrailResult, t1);
            float x2 = GetDistChartResultFromDistResult(isOffset, tr, ReferenceTrailResult, t2);
            return new float[] { x1, x2 };
        }

        private static float[] GetChartResultFromDateTime(bool xIsTime, bool isOffset, TrailResult tr, TrailResult ReferenceTrailResult, IValueRange<DateTime> v)
        {
            DateTime d1 = v.Lower;
            DateTime d2 = v.Upper;
            if (xIsTime)
            {
                return GetTimeChartResultFromDateTime(isOffset, tr, ReferenceTrailResult, d1, d2);
            }
            else
            {
                double t1 = tr.getDistResult(d1);
                double t2 = tr.getDistResult(d2);
                return GetDistChartResultFromDistResult(isOffset, tr, ReferenceTrailResult, t1, t2);
            }
        }

        private static float[] GetChartResultFromDistResult(bool xIsTime, bool isOffset, TrailResult tr, TrailResult ReferenceTrailResult, IValueRange<double> v)
        {
            //Note: Selecting in Route gives unpaused distance, but this should be handled in the selection
            if (xIsTime)
            {
                DateTime d1 = tr.getDateTimeFromDistActivity(v.Lower);
                DateTime d2 = tr.getDateTimeFromDistActivity(v.Upper);
                return GetTimeChartResultFromDateTime(isOffset, tr, ReferenceTrailResult, d1, d2);
            }
            else
            {
                double t1 = tr.getDistResultFromDistActivity(v.Lower);
                double t2 = tr.getDistResultFromDistActivity(v.Upper);
                return GetDistChartResultFromDistResult(isOffset, tr, ReferenceTrailResult, t1, t2);
            }
        }

        internal static IList<float[]> GetChartResultFromActivity(bool xIsTime, bool isOffset, TrailResult tr, TrailResult ReferenceTrailResult, IItemTrackSelectionInfo sel)
        {
            IList<float[]> result = new List<float[]>();

            //Currently only one range but several regions in the chart can be selected
            //Only use one of the selections
            if (sel.MarkedTimes != null)
            {
                foreach (IValueRange<DateTime> v in sel.MarkedTimes)
                {
                    result.Add(GetChartResultFromDateTime(xIsTime, isOffset, tr, ReferenceTrailResult, v));
                }
            }
            else if (sel.MarkedDistances != null)
            {
                foreach (IValueRange<double> v in sel.MarkedDistances)
                {
                    result.Add(GetChartResultFromDistResult(xIsTime, isOffset, tr, ReferenceTrailResult, v));
                }
            }
            else if (sel.SelectedTime != null)
            {
                result.Add(GetChartResultFromDateTime(xIsTime, isOffset, tr, ReferenceTrailResult, sel.SelectedTime));
            }
            else if (sel.SelectedDistance != null)
            {
                result.Add(GetChartResultFromDistResult(xIsTime, isOffset, tr, ReferenceTrailResult, sel.SelectedDistance));
            }
            return result;
        }

        /*************************************************/
        //From result elapsed, get chart result
        //TBD: incorrect to result

        public static float GetChartResultsResyncOffset(bool xIsTime, TrailResult tr, TrailResult ReferenceTrailResult, float elapsed, out float nextElapsed)
        {
            float offset = 0;
            nextElapsed = float.MaxValue;
            if (Data.Settings.SyncChartAtTrailPoints)
            {
                IList<float> trElapsed;
                //IList<float> trOffset;
                IList<float> refElapsed;
                if (xIsTime)
                {
                    trElapsed = tr.TrailPointTime0(ReferenceTrailResult);
                    //trOffset = tr.TrailPointTimeOffset0(ReferenceTrailResult);
                    refElapsed = ReferenceTrailResult.TrailPointTime0(ReferenceTrailResult);
                }
                else
                {
                    trElapsed = tr.TrailPointDist0(ReferenceTrailResult);
                    //trOffset = tr.TrailPointDistOffset0(ReferenceTrailResult);
                    refElapsed = ReferenceTrailResult.TrailPointDist0(ReferenceTrailResult);
                }

                //If the result/reference has no laps, no offset
                if (trElapsed.Count <= 2 || refElapsed.Count <= 2)
                {
                    return offset;
                }
                int currOffsetIndex = 0;
                while (currOffsetIndex < trElapsed.Count && currOffsetIndex < refElapsed.Count &&
                    (float.IsNaN(trElapsed[currOffsetIndex]) || elapsed > trElapsed[currOffsetIndex]))
                {
                    if (!float.IsNaN(trElapsed[currOffsetIndex]) && !float.IsNaN(refElapsed[currOffsetIndex]))
                    {
                        offset = refElapsed[currOffsetIndex] - trElapsed[currOffsetIndex];
                        //if (currOffsetIndex < trOffset.Count && !float.IsNaN(trOffset[currOffsetIndex]))
                        //{
                        //    offset += trOffset[currOffsetIndex];
                        //}
                    }
                    currOffsetIndex++;
                }
                //currOffsetIndex is at least one step over already
                while (currOffsetIndex < trElapsed.Count && currOffsetIndex < refElapsed.Count &&
                    (float.IsNaN(trElapsed[currOffsetIndex]) || float.IsNaN(refElapsed[currOffsetIndex]) ||
                    elapsed < trElapsed[currOffsetIndex]))
                {
                    if (currOffsetIndex >= trElapsed.Count - 1 || currOffsetIndex >= refElapsed.Count - 1)
                    {
                        //Last point - no limitation
                        nextElapsed = float.MaxValue;
                        break;
                    }
                    else if (!float.IsNaN(trElapsed[currOffsetIndex]) && !float.IsNaN(refElapsed[currOffsetIndex]))
                    {
                        nextElapsed = refElapsed[currOffsetIndex];
                    }
                    currOffsetIndex++;
                }
            }
            return offset;
        }

        /****************************************************/

        private static float GetChartResultFromDateTime(bool xIsTime, bool isOffset, TrailResult tr, TrailResult ReferenceTrailResult, DateTime t)
        {
            //Note: Selecting in Route gives unpaused distance, but this should be handled in the selection
            if (xIsTime)
            {
                return GetTimeChartResultFromDateTime(isOffset, tr, ReferenceTrailResult, t);
            }
            else
            {
                return GetDistChartResultFromDateTime(isOffset, tr, ReferenceTrailResult, t);
            }
        }

        public static DateTime GetDateTimeFromChartResult(bool xIsTime, bool isOffset, TrailResult tr, TrailResult ReferenceTrailResult, float t)
        {
            DateTime dateTime;
            float xOffset = tr.GetXOffset(xIsTime, ReferenceTrailResult);
            if (!xIsTime)
            {
                xOffset = (float)TrackUtil.DistanceConvertFrom(xOffset, ReferenceTrailResult);
            }
            t -= xOffset;
            if (isOffset)
            {
                float nextElapsed;
                t -= GetChartResultsResyncOffset(xIsTime, tr, ReferenceTrailResult, t, out nextElapsed);
            }
            if (xIsTime)
            {
                dateTime = tr.getDateTimeFromTimeResult(t);
            }
            else
            {
                dateTime = tr.getDateTimeFromDistResult(TrackUtil.DistanceConvertTo(t, ReferenceTrailResult));
            }
            return dateTime;
        }

        public static IValueRangeSeries<DateTime> GetDateTimeFromChartResult(bool xIsTime, bool isOffset, TrailResult tr, TrailResult ReferenceTrailResult, IList<float[]> regions)
        {
            IValueRangeSeries<DateTime> t = new ValueRangeSeries<DateTime>();
            foreach (float[] at in regions)
            {
                DateTime d1 = GetDateTimeFromChartResult(xIsTime, isOffset, tr, ReferenceTrailResult, at[0]);
                DateTime d2 = GetDateTimeFromChartResult(xIsTime, isOffset, tr, ReferenceTrailResult, at[1]);
                t.Add(new ValueRange<DateTime>(d1, d2));
            }
            return t;
        }

        public static float ChartResultConvert(bool xOldIsTime, bool xNewIsTime, bool isOffset, TrailResult tr, TrailResult ReferenceTrailResult, float t)
        {
            DateTime time = TrackUtil.GetDateTimeFromChartResult(xOldIsTime, isOffset, tr, ReferenceTrailResult, t);
            float res = TrackUtil.GetChartResultFromDateTime(xNewIsTime, isOffset, tr, ReferenceTrailResult, time);
            return res;
        }

        public static void ChartResultConvert(bool xOldIsTime, bool xNewIsTime, bool isOffset, TrailResult tr, TrailResult ReferenceTrailResult, float[] t)
        {
            t[0] = ChartResultConvert(xOldIsTime, xNewIsTime, isOffset, tr, ReferenceTrailResult, t[0]);
            t[1] = ChartResultConvert(xOldIsTime, xNewIsTime, isOffset, tr, ReferenceTrailResult, t[1]);
        }

        /****************************************************/
        public static bool AnyOverlap(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            bool res = false;
            if (start1 >= start2 && start1 <= end2 ||
                start2 >= start1 && start2 <= end1)
            {
                res = true;
            }
            return res;
        }

        /****************************************************/
        //Before ST 3_0_4205 points was not always inserted in order
        //For some reason this occasionally occurs in Trails 1.2.821 too
        public static bool ResortTrack<T>(ITimeDataSeries<T> track)
        {
            bool reSort = false;
#if !NO_ST_RESORT_TRACKS
            if (track.Count > 0)
            {
                for (int i = 1; i < track.Count; i++)
                {
                    if (track[i - 1].ElapsedSeconds > track[i].ElapsedSeconds)
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
                    DateTime startTimeTrack = track.StartTime;
                    track.Clear();
                    foreach (KeyValuePair<uint, ITimeValueEntry<T>> g in dic)
                    {
                        track.Add(startTimeTrack.AddSeconds(g.Value.ElapsedSeconds), g.Value.Value);
                    }
                }
            }
#endif
            return reSort;
        }

        public static void InsertValue<T>(DateTime atime, ITimeDataSeries<T> track, ITimeDataSeries<T> source)
        {
            //Interpolation is down to seconds
            //TBD: Inefficient, source.IndexOf often fails
#if ST_3_1_5314_BUG
            //http://www.zonefivesoftware.com/sporttracks/forums/viewtopic.php?p=84638#p84638
            //System.Exception: FindPosOnOrBefore: Didn't find element properly.
            try
            {
#endif
                ITimeValueEntry<T> interpolatedP = source.GetInterpolatedValue(atime);
                if (interpolatedP != null)
                {
                    int index = source.IndexOf(interpolatedP);
                    T val = interpolatedP.Value;
                    if (index >= 0)
                    {
                        val = TrackUtil.getValFromDateTimeIndex(source, atime, index);
                    }
                    else
                    {
                    }
                    try
                    {
#if !NO_ST_INSERT_START_TIME
                        //ST bug: not inserted in order if ms differs for start
                        if (Math.Abs((atime - track.StartTime).TotalSeconds) < 1)
                        {
                            track.RemoveAt(0);
                        }
#endif
                        track.Add(atime, val);
                        //T val2 = track.GetInterpolatedValue(atime).Value;
                    }
                    catch { }
                }
#if ST_3_1_5314_BUG
            }
            catch (Exception e)
            {
            }
#endif
        }
    }

    //Insert points at start/end and pauses
    //This simplifies tranversing distancetrack
    //It was originally added as an attempt to handle selections and improve averages etc
    internal class InsertValues<T>
    {
        DateTime startTime;
        DateTime endTime;
        IValueRangeSeries<DateTime> pauses;
        IList<TrailResultPoint> points;

        public InsertValues(DateTime startTime, DateTime endTime, IValueRangeSeries<DateTime> pauses)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.pauses = pauses;
            this.points = null;
        }
        public InsertValues(TrailResult result) :
            this(result.StartTime, result.EndTime, result.Pauses)
        {
            this.points = result.SubResultInfo.Points;
        }

        private void InsertValue(DateTime atime, ITimeDataSeries<T> track, ITimeDataSeries<T> source)
        {
            if (atime >= this.startTime && atime <= this.endTime &&
                !ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(atime, this.pauses))
            {
                TrackUtil.InsertValue<T>(atime, track, source);
            }
        }

        //static inserted values (pauses and points to be handled separetly)
        public const int NoOfStaticInsertValues = 2;

        /// <summary>
        /// Insert values at borders: Start, stop, pauses, trailpoints
        /// </summary>
        /// <param name="track"></param>
        /// <param name="source"></param>
        public void insertValues(ITimeDataSeries<T> track, ITimeDataSeries<T> source)
        {
            //Insert points around pauses and points
            //This is needed to get the track match the cut-up activity
            //(otherwise for instance start point need to be added)

            IList<DateTime> dates = new List<DateTime>();

            //start/end should be included from points, but prepare for changes...
            InsertValue(startTime, track, source);
            InsertValue(endTime, track, source);
            dates.Add(startTime);
            dates.Add(endTime);

            foreach (IValueRange<DateTime> p in pauses)
            {
                if (p.Lower > startTime)
                {
                    InsertValue(p.Lower.AddSeconds(-1), track, source);
                    dates.Add(p.Lower.AddSeconds(-1));
                }
                if (p.Upper < endTime)
                {
                    InsertValue(p.Upper.AddSeconds(1), track, source);
                    dates.Add(p.Upper.AddSeconds(1));
                }
            }

            if (this.points != null)
            {
                foreach (TrailResultPoint t in points)
                {
                    DateTime dateTime = t.Time;
                    if (dateTime > startTime && dateTime < endTime)
                    {
                        //Adding an extra point will change averages etc
                        //InsertValue(dateTime.AddSeconds(-1), track, source);
                        InsertValue(dateTime, track, source);
                        dates.Add(dateTime);
                    }
                }
            }
            //((List<DateTime>)dates).Sort();
            //kolla avrundning av elapsed både då starttime.ms och entry.ms
            //int j = 0;
            //double elapsedDate = (dates[j] - source.StartTime).TotalSeconds;
            //for (int i = 0; i < source.Count - 1; i++)
            //{
            //    uint elapsedSrc = source[i].ElapsedSeconds;
            //    while (j < dates.Count && (dates[j] < source.StartTime ||
            //        elapsedDate < elapsedSrc))
            //    {
            //        j++;
            //        if (j < dates.Count)
            //        {
            //            elapsedDate = (dates[j] - source.StartTime).TotalSeconds;
            //        }
            //    }
            //    if (j >= dates.Count || elapsedDate > source.TotalElapsedSeconds)
            //    {
            //        //No more dates
            //        break;
            //    }
            //    if (elapsedDate >= elapsedSrc)
            //    {
            //        //xxx
            //    }
            //}
            //ST bug: track could be out of order (due to insert at least)
            TrackUtil.ResortTrack(track);
        }
    }
}
