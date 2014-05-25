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

        public static void GetDistanceTimeSelection(bool xIsTime, TrailResult tr, TrailResult ReferenceTrailResult, float[] x, float xmin, float xmax, ref float dist, ref float time)
        {
            //Ignore sections outside what is displayed
            x[0] = Math.Max(x[0], xmin);
            x[1] = Math.Min(x[1], xmax);

            if (x[1] > x[0])
            {
                if (xIsTime)
                {
                    time += x[1] - x[0];
                    DateTime d1 = DateTime.MinValue, d2 = DateTime.MinValue;
                    d1 = tr.getDateTimeFromTimeResult(x[0]);
                    d2 = tr.getDateTimeFromTimeResult(x[1]);
                    double t1 = tr.getDistResult(d1);
                    double t2 = tr.getDistResult(d2);
                    dist += (float)(t2 - t1);
                }
                else
                {
                    float x1 = float.MaxValue, x2 = float.MinValue;
                    //distance is for result, then to display units
                    x1 = (float)TrackUtil.DistanceConvertTo(x[0], ReferenceTrailResult);
                    x2 = (float)TrackUtil.DistanceConvertTo(x[1], ReferenceTrailResult);
                    dist += (float)(x2 - x1);

                    DateTime d1 = DateTime.MinValue, d2 = DateTime.MinValue;
                    d1 = tr.getDateTimeFromDistResult(x1);
                    d2 = tr.getDateTimeFromDistResult(x2);

                    double t1 = tr.getTimeResult(d1);
                    double t2 = tr.getTimeResult(d2);
                    time += (float)(t2 - t1);
                }
            }
        }

        /*******************************************************/

        public static float[] GetSingleSelection(bool xIsTime, TrailResult tr, TrailResult ReferenceTrailResult, IValueRange<DateTime> v)
        {
            DateTime d1 = v.Lower;
            DateTime d2 = v.Upper;
            if (xIsTime)
            {
                return GetSingleSelectionFromResult(tr, ReferenceTrailResult, d1, d2);
            }
            else
            {
                double t1 = tr.getDistResult(d1);
                double t2 = tr.getDistResult(d2);
                return GetSingleSelectionFromResult(tr, ReferenceTrailResult, t1, t2);
            }
        }

        private static float[] GetSingleSelection(bool xIsTime, TrailResult tr, TrailResult ReferenceTrailResult, IValueRange<double> v)
        {
            //Note: Selecting in Route gives unpaused distance, but this should be handled in the selection
            if (xIsTime)
            {
                DateTime d1 = DateTime.MinValue, d2 = DateTime.MinValue;
                d1 = tr.getDateTimeFromDistActivity(v.Lower);
                d2 = tr.getDateTimeFromDistActivity(v.Upper);
                return GetSingleSelectionFromResult(tr, ReferenceTrailResult, d1, d2);
            }
            else
            {
                double t1 = tr.getDistResultFromDistActivity(v.Lower);
                double t2 = tr.getDistResultFromDistActivity(v.Upper);
                return GetSingleSelectionFromResult(tr, ReferenceTrailResult, t1, t2);
            }
        }

        private static float[] GetSingleSelectionFromResult(TrailResult tr, TrailResult ReferenceTrailResult, DateTime d1, DateTime d2)
        {
            float x1 = float.MaxValue, x2 = float.MinValue;
            //Convert to distance display unit, Time is always in seconds
            x1 = (float)(tr.getTimeResult(d1));
            x2 = (float)(tr.getTimeResult(d2));
            return new float[] { x1, x2 };
        }

        private static float[] GetSingleSelectionFromResult(TrailResult tr, TrailResult ReferenceTrailResult, double t1, double t2)
        {
            float x1 = float.MaxValue, x2 = float.MinValue;
            //distance is for result, then to display units
            x1 = (float)TrackUtil.DistanceConvertFrom(t1, ReferenceTrailResult);
            x2 = (float)TrackUtil.DistanceConvertFrom(t2, ReferenceTrailResult);
            return new float[] { x1, x2 };
        }

        internal static IList<float[]> GetResultSelectionFromActivity(bool xIsTime, TrailResult tr, TrailResult ReferenceTrailResult, IItemTrackSelectionInfo sel)
        {
            IList<float[]> result = new List<float[]>();

            //Currently only one range but several regions in the chart can be selected
            //Only use one of the selections
            if (sel.MarkedTimes != null)
            {
                foreach (IValueRange<DateTime> v in sel.MarkedTimes)
                {
                    result.Add(GetSingleSelection(xIsTime, tr, ReferenceTrailResult, v));
                }
            }
            else if (sel.MarkedDistances != null)
            {
                foreach (IValueRange<double> v in sel.MarkedDistances)
                {
                    result.Add(GetSingleSelection(xIsTime, tr, ReferenceTrailResult, v));
                }
            }
            else if (sel.SelectedTime != null)
            {
                result.Add(GetSingleSelection(xIsTime, tr, ReferenceTrailResult, sel.SelectedTime));
            }
            else if (sel.SelectedDistance != null)
            {
                result.Add(GetSingleSelection(xIsTime, tr, ReferenceTrailResult, sel.SelectedDistance));
            }
            return result;
        }

        /*************************************************/
        //From a value in the chart, get "real" elapsed
        //TBD: incorrect around trail points
        private static float GetResyncOffsetTime(TrailResult tr, TrailResult ReferenceTrailResult, float elapsed)
        {
            float nextElapsed;
            return elapsed - GetResyncOffset(true, tr, ReferenceTrailResult, elapsed, out nextElapsed);
        }

        private static float GetResyncOffsetDist(TrailResult tr, TrailResult ReferenceTrailResult, float elapsed)
        {
            float nextElapsed;
            return elapsed - GetResyncOffset(false, tr, ReferenceTrailResult, elapsed, out nextElapsed);
        }

        public static float GetResyncOffset(bool xIsTime, TrailResult tr, TrailResult ReferenceTrailResult, float elapsed, out float nextElapsed)
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
        public static IValueRangeSeries<DateTime> GetResultRegions(bool xIsTime, TrailResult tr, TrailResult ReferenceTrailResult, IList<float[]> regions)
        {
            IValueRangeSeries<DateTime> t = new ValueRangeSeries<DateTime>();
            foreach (float[] at in regions)
            {
                DateTime d1;
                DateTime d2;
                if (xIsTime)
                {
                    d1 = tr.getDateTimeFromTimeResult(GetResyncOffsetTime(tr, ReferenceTrailResult, at[0]));
                    d2 = tr.getDateTimeFromTimeResult(GetResyncOffsetTime(tr, ReferenceTrailResult, at[1]));
                }
                else
                {

                    d1 = tr.getDateTimeFromDistResult(TrackUtil.DistanceConvertTo(GetResyncOffsetDist(tr, ReferenceTrailResult, at[0]), ReferenceTrailResult));
                    d2 = tr.getDateTimeFromDistResult(TrackUtil.DistanceConvertTo(GetResyncOffsetDist(tr, ReferenceTrailResult, at[1]), ReferenceTrailResult));
                }
                t.Add(new ValueRange<DateTime>(d1, d2));
            }
            return t;
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
