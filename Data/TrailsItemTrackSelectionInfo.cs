/*
Copyright (C) 2010 Gerhard Olsson
Copyright (C) 2010 PissedOffCil

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
using System.Text;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace TrailsPlugin.Data
{
    //Class to have the same interface as when selecting items on the track
    public class TrailsItemTrackSelectionInfo : IItemTrackSelectionInfo
    {
        #region IItemTrackSelectionInfo Members
        public string ItemReferenceId
        {
            get
            {
                if (m_Activity != null)
                {
                    return m_Activity.ReferenceId;
                }

                return string.Empty;
            }
        }

        public IValueRangeSeries<double> MarkedDistances
        {
            get { return m_MarkedDistances; }
            set { m_MarkedDistances = value; }
        }

        public IValueRangeSeries<DateTime> MarkedTimes
        {
            get { return m_MarkedTimes; }
            set { m_MarkedTimes = value; }
        }

        public IValueRange<double> SelectedDistance
        {
            get { return m_SelectedDistance; }
            set { m_SelectedDistance = value; }
        }

        public IValueRange<DateTime> SelectedTime
        {
            get { return m_SelectedTime; }
            set { m_SelectedTime = value; }
        }
        #endregion

        public IActivity Activity
        {
            get { return m_Activity; }
            set { m_Activity = value; }
        }

        //Note: ST sets null selections occasionally
        //This checks common situations
        public static bool ContainsData(IList<IItemTrackSelectionInfo> selectedGPS)
        {
            int countGPS = selectedGPS.Count;
            return countGPS > 0 && 
                !(countGPS == 1 &&
                selectedGPS[0].ItemReferenceId == "" || selectedGPS[0].MarkedDistances == null && selectedGPS[0].SelectedTime == null && selectedGPS[0].SelectedDistance == null && 
                selectedGPS[0].MarkedTimes != null && selectedGPS[0].MarkedTimes.Count == 0);
        }

        public void SetFromSelection(IItemTrackSelectionInfo t, IActivity activity)
        {
            // Set from activity: this.ItemReferenceId = t.ItemReferenceId;
            this.MarkedDistances = t.MarkedDistances;
            this.m_MarkedTimes = t.MarkedTimes;
            this.m_SelectedDistance = t.SelectedDistance;
            this.m_SelectedTime = t.SelectedTime;
            this.Activity = activity;
        }

        //SelectedDistances from ST core is in Activity without pauses distance
        //Add SelectedTime instead
        public static IList<IItemTrackSelectionInfo> SetAndAdjustFromSelection
            (IList<IItemTrackSelectionInfo> selected, IList<IActivity> activities, bool fromST)
        {
            if (selected.Count == 0 || activities == null)
            {
                //Do not adjust selection
                return selected;
            }
            bool singleSelection = selected.Count == 0 || selected.Count == 1 && ! ( 
                selected[0].MarkedDistances != null && selected[0].MarkedDistances.Count > 1 ||
                selected[0].MarkedTimes != null && selected[0].MarkedTimes.Count > 1);
            for(int i = 0; i < selected.Count; i++)
            {
                IActivity activity = null;
                if (fromST)
                {
                    foreach (IActivity a in activities)
                    {
                        //In ST3.0.4068 (at least) only one activity is selected
                        if (a != null && selected[i].ItemReferenceId == a.ReferenceId)
                        {
                            activity = a;
                            break;
                        }
                    }
                }
                else
                {
                    if (selected[i] is TrailsItemTrackSelectionInfo)
                    {
                        activity = ((TrailsItemTrackSelectionInfo)selected[i]).Activity;
                    }
                }
                if (activity != null)
                {
                        //The distance is in unstopped/unpaused format
                    IDistanceDataTrack activityUnpausedDistanceMetersTrack =
                        ActivityInfoCache.Instance.GetInfo(activity).ActualDistanceMetersTrack;
                    TrailsItemTrackSelectionInfo tmpSel = new TrailsItemTrackSelectionInfo();
                    tmpSel.SetFromSelection(selected[i], activity);

                    if (fromST)
                    {
                        //Set MarkedTimes (or SelectedTime), used internally
                        if (selected[i].MarkedDistances != null && selected[i].MarkedDistances.Count > 0 && 
                            (selected[i].MarkedTimes == null || selected[i].MarkedTimes.Count == 0))
                        {
                            try
                            {
                                foreach (ValueRange<double> t in selected[i].MarkedDistances)
                                {
                                    DateTime d1 = activityUnpausedDistanceMetersTrack.GetTimeAtDistanceMeters(t.Lower);
                                    DateTime d2 = activityUnpausedDistanceMetersTrack.GetTimeAtDistanceMeters(t.Upper);
                                    AddMarkedOrSelectedTime(tmpSel, singleSelection, d1, d2);
                                }
                                tmpSel.MarkedDistances = null;
                            }
                            catch { }
                        }
                        if (selected[i].SelectedTime != null && selected[i].MarkedTimes == null)
                        {
                            try
                            {
                                AddMarkedOrSelectedTime(tmpSel, singleSelection, selected[i].SelectedTime.Lower, selected[i].SelectedTime.Upper);

                                tmpSel.SelectedTime = null;
                            }
                            catch { }
                        }
                        if (selected[i].SelectedDistance != null && selected[i].MarkedTimes == null)
                        {
                            tmpSel.MarkedTimes = new ValueRangeSeries<DateTime>();
                            try
                            {
                                DateTime d1 = activityUnpausedDistanceMetersTrack.GetTimeAtDistanceMeters(selected[i].SelectedDistance.Lower);
                                DateTime d2 = activityUnpausedDistanceMetersTrack.GetTimeAtDistanceMeters(selected[i].SelectedDistance.Upper);
                                AddMarkedOrSelectedTime(tmpSel, singleSelection, d1, d2);
                                tmpSel.SelectedDistance = null;
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        //The standard in the plugin(time) to standard in ST core and omb's Track Coloring (unpaused distance)
                        if (selected[i].MarkedDistances == null &&
                            selected[i].MarkedTimes != null && selected[i].MarkedTimes.Count > 0)
                        {
                            try
                            {
                                tmpSel.MarkedDistances = new ValueRangeSeries<double>();
                                foreach (ValueRange<DateTime> t in selected[i].MarkedTimes)
                                {
                                    double d1 = activityUnpausedDistanceMetersTrack.GetInterpolatedValue(t.Lower).Value;
                                    double d2 = activityUnpausedDistanceMetersTrack.GetInterpolatedValue(t.Upper).Value;
                                    AddMarkedOrSelectedDistance(tmpSel, singleSelection, d1, d2);
                                }
                                tmpSel.MarkedTimes = null;
                            }
                            catch { }
                        }
                        if (selected[i].SelectedDistance == null &&
                            selected[i].SelectedTime != null)
                        {
                            try
                            {
                                tmpSel.SelectedDistance = new ValueRange<double>(
                                            activityUnpausedDistanceMetersTrack.GetInterpolatedValue(selected[i].SelectedTime.Lower).Value,
                                            activityUnpausedDistanceMetersTrack.GetInterpolatedValue(selected[i].SelectedTime.Upper).Value);
                                tmpSel.SelectedTime = null;
                            }
                            catch { }
                        }
                    }
                    selected[i] = tmpSel;
                }
            }
            return selected;
        }

        private static void AddMarkedOrSelectedTime(TrailsItemTrackSelectionInfo tmpSel, bool singleSelection, DateTime d1, DateTime d2)
        {
            //Use SelectedTime with only one selection and 0 span - valueRanges will not handle it
            IValueRange<DateTime> vd = new ValueRange<DateTime>(d1, d2);
            if (tmpSel.MarkedTimes == null)
            {
                tmpSel.MarkedTimes = new ValueRangeSeries<DateTime>();
            }
            int currAdd = tmpSel.MarkedTimes.Count;
            tmpSel.MarkedTimes.Add(vd);
            if(tmpSel.MarkedTimes.Count == currAdd)
            {
                //Could not add to ranges, add Selected
                if (singleSelection && tmpSel.MarkedTimes.Count == 0)
                {
                    //Could add to selected
                    tmpSel.SelectedTime = vd;
                    tmpSel.MarkedTimes = null;
                }
                else
                {
                    //fake, add second
                    vd = new ValueRange<DateTime>(vd.Lower, vd.Upper.AddSeconds(1));
                    tmpSel.MarkedTimes.Add(vd);
                }
            }
        }

        private static void AddMarkedOrSelectedDistance(TrailsItemTrackSelectionInfo tmpSel, bool singleSelection, double d1, double d2)
        {
            //Use SelectedTime with only one selection and 0 span - valueRanges will not handle it
            //Add 1s if not possible
            IValueRange<double> vd = new ValueRange<double>(d1, d2);
            if (tmpSel.MarkedDistances == null)
            {
                tmpSel.MarkedDistances = new ValueRangeSeries<double>();
            }
            int currAdd = tmpSel.MarkedDistances.Count;
            tmpSel.MarkedDistances.Add(vd);
            if (tmpSel.MarkedDistances.Count == currAdd)
            {
                //Could not add to ranges, add Selected
                if (singleSelection && tmpSel.MarkedDistances.Count == 0)
                {
                    //Could add to selected
                    tmpSel.SelectedDistance = vd;
                    tmpSel.MarkedDistances = null;
                }
                else
                {
                    //fake, add second
                    vd = new ValueRange<double>(vd.Lower, vd.Upper + 1);
                    tmpSel.MarkedDistances.Add(vd);
                }
            }
        }

        public static IList<IList<IGPSPoint>> GpsPoints(IGPSRoute gpsRoute, IValueRangeSeries<DateTime> pauses, IValueRangeSeries<DateTime> t)
        {
            IList<IList<IGPSPoint>> result = new List<IList<IGPSPoint>>();

            if (t != null)
            {
                int pauseIndex = 0;
                int prevPauseIndex = -1;
                DateTime firstGpsTime = gpsRoute.EntryDateTime(gpsRoute[0]);
                DateTime lastGpsTime = gpsRoute.EntryDateTime(gpsRoute[gpsRoute.Count - 1]);

                foreach (IValueRange<DateTime> r in t)
                {
                    IList<IGPSPoint> track = null;
                    DateTime prevMatchTime = DateTime.MinValue;

                    foreach (ITimeValueEntry<IGPSPoint> entry in gpsRoute)
                    {
                        DateTime time = gpsRoute.EntryDateTime(entry);
                        bool isPause = ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.IsPaused(time, pauses);

                        //Add new track around pauses
                        if (!isPause)
                        {
                            while (pauseIndex < pauses.Count && pauses[pauseIndex].Lower < time)
                            {
                                pauseIndex++;
                            }
                        }

                        if (track != null && (isPause || prevPauseIndex != pauseIndex))
                        {
                            //previous point was the end of a track
                            //Add endpoint if not on the same second
                            if (prevMatchTime != DateTime.MinValue
                                && prevMatchTime.AddMilliseconds(-prevMatchTime.Millisecond) < r.Upper.AddMilliseconds(-r.Upper.Millisecond)
                                && r.Upper >= firstGpsTime && r.Upper <= lastGpsTime)
                            {
                                track.Add(gpsRoute.GetInterpolatedValue(r.Upper).Value);
                                prevMatchTime = r.Upper;
                            }
                            result.Add(track);
                            prevMatchTime = DateTime.MinValue;
                            track = null;
                        }

                        if (!isPause && r.Lower <= time && time <= r.Upper)
                        {
                            //Point is in the range add it
                            if (track == null)
                            {
                                //First point ot previous was a pause, add new track
                                track = new List<IGPSPoint>();
                                //Add startpoint if not on the same second as current point
                                if (time.AddMilliseconds(-time.Millisecond) > r.Lower.AddMilliseconds(-r.Lower.Millisecond)
                                    && r.Lower >= firstGpsTime && r.Lower <= lastGpsTime)
                                {
                                    track.Add(gpsRoute.GetInterpolatedValue(r.Lower).Value);
                                    prevMatchTime = r.Lower;
                                }
                            }
                            track.Add(entry.Value);
                            prevMatchTime = time;
                        }
                        if (time > r.Upper)
                        {
                            break;
                        }
                        prevPauseIndex = pauseIndex;
                    }

                    //Add point after last track point?
                    if (track!=null)
                    {
                        if (prevMatchTime != DateTime.MinValue
                            && prevMatchTime.AddMilliseconds(-prevMatchTime.Millisecond) < r.Upper.AddMilliseconds(-r.Upper.Millisecond)
                            && r.Upper >= firstGpsTime && r.Upper <= lastGpsTime)
                        {
                            track.Add(gpsRoute.GetInterpolatedValue(r.Upper).Value);
                            prevMatchTime = r.Upper;
                        }
                        result.Add(track);
                    }
                }
            }
            return result;
        }

        public override string ToString()
        {
            TrailsItemTrackSelectionInfo sel = this.FirstSelection();
            if (sel.MarkedTimes != null && sel.MarkedTimes.Count > 0)
            {
                return sel.MarkedTimes[0].Lower + "_" + sel.MarkedTimes[sel.MarkedTimes.Count - 1].Upper;
            }
            else if (sel.MarkedDistances != null && sel.MarkedDistances.Count > 0)
            {
                return sel.MarkedDistances[0].Lower + "_" + sel.MarkedDistances[sel.MarkedDistances.Count - 1].Upper;
            }
            return string.Empty;
        }
        private TrailsItemTrackSelectionInfo FirstSelection()
        {
            //Many commands can only handle one selection - this will set only one of them
            TrailsItemTrackSelectionInfo res = new TrailsItemTrackSelectionInfo();
            res.Activity = this.Activity;

            if (this.MarkedTimes != null && this.MarkedTimes.Count > 0)
            {
                res.MarkedTimes = this.MarkedTimes;
            }
            else if (this.MarkedDistances != null && this.m_MarkedDistances.Count > 0)
            {
                res.MarkedDistances = this.MarkedDistances;
            }
            else if (this.SelectedTime != null)
            {
                if (res.MarkedTimes == null) { res.m_MarkedTimes = new ValueRangeSeries<DateTime>(); }
                res.MarkedTimes.Add(this.SelectedTime);
            }
            else if (this.SelectedDistance != null)
            {
                if (res.MarkedDistances == null) { res.MarkedDistances = new ValueRangeSeries<double>(); }
                res.MarkedDistances.Add(this.SelectedDistance);
            }

            if (res.MarkedTimes != null)
            {
                res.MarkedDistances = null;
            }
            return res;
        }

        public void Union(IItemTrackSelectionInfo t)
        {
            if (m_MarkedTimes == null)
            {
                m_MarkedTimes = t.MarkedTimes;
            }
            else if (t.MarkedTimes != null)
            {
                foreach (IValueRange<DateTime> i in t.MarkedTimes)
                {
                    m_MarkedTimes.Add(i);
                }
            }
            if (m_MarkedDistances == null)
            {
                m_MarkedDistances = t.MarkedDistances;
            }
            else if (t.MarkedDistances != null)
            {
                foreach (IValueRange<double> i in t.MarkedDistances)
                {
                    m_MarkedDistances.Add(i);
                }
            }
            if (m_SelectedTime == null)
            {
                m_SelectedTime = t.SelectedTime;
            }
            else if (t.SelectedTime != null)
            {
                DateTime Lower = m_SelectedTime.Lower.CompareTo(t.SelectedTime.Lower) < 0 ?
                   m_SelectedTime.Lower : t.SelectedTime.Lower;
                DateTime Upper = m_SelectedTime.Upper.CompareTo(t.SelectedTime.Upper) > 0 ?
                   m_SelectedTime.Upper : t.SelectedTime.Upper;
                m_SelectedTime = new ValueRange<DateTime>(Lower, Upper);
            }
            if (m_SelectedDistance == null)
            {
                m_SelectedDistance = t.SelectedDistance;
            }
            else if (t.SelectedDistance != null)
            {
                double Lower = Math.Min(m_SelectedDistance.Lower, t.SelectedDistance.Lower);
                double Upper = Math.Max(m_SelectedDistance.Upper, t.SelectedDistance.Upper);
                m_SelectedDistance = new ValueRange<double>(Lower, Upper);
            }
        }
        private IValueRangeSeries<double> m_MarkedDistances = null;
        private IValueRangeSeries<DateTime> m_MarkedTimes = null;
        private IValueRange<double> m_SelectedDistance = null;
        private IValueRange<DateTime> m_SelectedTime = null;
        private IActivity m_Activity = null;
    }
#if ST_2_1
//Dummy definition for ST2, to minimize ifdef
    public interface IItemTrackSelectionInfo
    {
        string ItemReferenceId { get; }
        IValueRangeSeries<double> MarkedDistances { get; }
        IValueRangeSeries<DateTime> MarkedTimes { get; }
        IValueRange<double> SelectedDistance { get; }
        IValueRange<DateTime> SelectedTime { get; }
    }
#endif
}
