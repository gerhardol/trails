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

        internal static float getDistFromDateTime(IDistanceDataTrack distTrack, DateTime t)
        {
            int status;
            return getDistFromDateTime(distTrack, t, out status);
        }

        internal static float getDistFromDateTime(IDistanceDataTrack distTrack, DateTime t, out int status)
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
                            dateTime = (pause.Upper).Add(TimeSpan.FromSeconds(1));
                        }
                        else if (pause.Lower > DateTime.MinValue)
                        {
                            dateTime = (pause.Lower).Add(TimeSpan.FromSeconds(-1));
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
                    else if(time > activity.GPSRoute.EntryDateTime(activity.GPSRoute[activity.GPSRoute.Count-1]))
                    {
                        result = activity.GPSRoute[activity.GPSRoute.Count - 1].Value;
                    }
                }
            }
            return result;
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
        //TODO: incorrect around trail points
        private static float GetResyncOffsetTime(TrailResult tr, TrailResult ReferenceTrailResult, float elapsed)
        {
            float nextElapsed;
            int currOffsetIndex = 0;
            return elapsed - GetResyncOffset(true, true, tr, ReferenceTrailResult, elapsed, out nextElapsed, ref currOffsetIndex);
        }

        private static float GetResyncOffsetDist(TrailResult tr, TrailResult ReferenceTrailResult, float elapsed)
        {
            float nextElapsed;
            int currOffsetIndex = 0;
            return elapsed - GetResyncOffset(false, true, tr, ReferenceTrailResult, elapsed, out nextElapsed, ref currOffsetIndex);
        }

        //private static float GetResyncOffset(XAxisValue XAxisReferential, bool elapsedIsRef, TrailResult tr, float elapsed)
        //{
        //    float nextElapsed;
        //    float startOffset;
        //    int currOffsetIndex = 0;
        //    return GetResyncOffset(XAxisReferential, false, tr, elapsed, out nextElapsed, out startOffset, ref currOffsetIndex);
        //}

        public static float GetResyncOffset(bool xIsTime, bool elapsedIsRef, TrailResult tr, TrailResult ReferenceTrailResult, float elapsed, out float nextElapsed)
        {
            //Possibility to cache the index
            int currOffsetIndex = 0;
            return GetResyncOffset(xIsTime, elapsedIsRef, tr, ReferenceTrailResult, elapsed, out nextElapsed, ref currOffsetIndex);
        }

        public static float GetResyncOffset(bool xIsTime, bool elapsedIsRef, TrailResult tr, TrailResult ReferenceTrailResult, float elapsed, out float nextElapsed, ref int currOffsetIndex)
        {
            IList<double> trElapsed;
            IList<double> trOffset;
            IList<double> refElapsed;
            float offset = 0;
            nextElapsed = float.MaxValue;
            if (Data.Settings.SyncChartAtTrailPoints)
            {
                if (xIsTime)
                {
                    trElapsed = tr.TrailPointTime0(ReferenceTrailResult);
                    trOffset = tr.TrailPointTimeOffset0(ReferenceTrailResult);
                    refElapsed = ReferenceTrailResult.TrailPointTime0(ReferenceTrailResult);
                }
                else
                {
                    trElapsed = tr.TrailPointDist0(ReferenceTrailResult);
                    trOffset = tr.TrailPointDistOffset01(ReferenceTrailResult);
                    refElapsed = ReferenceTrailResult.TrailPointDist0(ReferenceTrailResult);
                }

                if (trElapsed.Count == refElapsed.Count)
                {
                    if (elapsedIsRef)
                    {
                        while (currOffsetIndex < refElapsed.Count - 1 && elapsed >= refElapsed[currOffsetIndex + 1])
                        {
                            currOffsetIndex++;
                        }
                        if (currOffsetIndex < trElapsed.Count - 1)
                        {
                            nextElapsed = (float)trElapsed[currOffsetIndex + 1];
                        }
                    }
                    else
                    {
                        while (currOffsetIndex < trElapsed.Count - 1 &&
                            //compare must be using same type here to avoid end effects
                            elapsed > (float)trElapsed[currOffsetIndex + 1])
                        {
                            currOffsetIndex++;
                        }
                        if (currOffsetIndex < refElapsed.Count - 1)
                        {
                            nextElapsed = (float)refElapsed[currOffsetIndex + 1];
                        }
                    }
                    if (currOffsetIndex > 1)
                    {
                    }
                    float startOffset;
                    if (currOffsetIndex < trOffset.Count)
                    {
                        startOffset = (float)trOffset[currOffsetIndex];
                    }
                    else
                    {
                        startOffset = 0;
                    }
                    if (currOffsetIndex < refElapsed.Count)
                    {
                        offset = (float)((refElapsed[currOffsetIndex] - trElapsed[currOffsetIndex]) + startOffset);
                    }
                }
            }
            return offset;
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


    }
}
