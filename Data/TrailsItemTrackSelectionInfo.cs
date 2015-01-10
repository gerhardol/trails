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
using ZoneFiveSoftware.Common.Data.Algorithm;
using ZoneFiveSoftware.Common.Visuals.Fitness;

using GpsRunningPlugin.Util;

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
                (selectedGPS[0].ItemReferenceId == "" || 
                ((selectedGPS[0].MarkedDistances == null || selectedGPS[0].MarkedDistances != null && selectedGPS[0].MarkedDistances.Count == 0)&& 
                 selectedGPS[0].SelectedTime == null && selectedGPS[0].SelectedDistance == null && 
                 (selectedGPS[0].MarkedTimes == null || selectedGPS[0].MarkedTimes != null && selectedGPS[0].MarkedTimes.Count == 0))));
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

        //Exclude the pauses from the selection
        public static IValueRangeSeries<DateTime> excludePauses(IValueRangeSeries<DateTime> sels, IValueRangeSeries<DateTime> pauses)
        {
            IValueRangeSeries<DateTime> t = new ValueRangeSeries<DateTime>();
            foreach (IValueRange<DateTime> tsel in sels)
            {
                foreach (IValueRange<DateTime> t1 in DateTimeRangeSeries.TimesNotPaused(
                    tsel.Lower, tsel.Upper, pauses))
                {
                    t.Add(t1);
                }
            }
            return t;
        }

        //SelectedDistances from ST core is in Activity without pauses distance
        //Add SelectedTime instead
        public static IList<IItemTrackSelectionInfo> SetAndAdjustFromSelectionToST(IList<TrailResultMarked> res)
        {
            IList<IItemTrackSelectionInfo> sels;
            if (res == null)
            {
                sels = new List<IItemTrackSelectionInfo>();
            }
            else
            {
                //ST internal marking, use common marking
                //Only one activity, OK to merge selections on one track
                TrailsItemTrackSelectionInfo sel = TrailResultMarked.SelInfoUnion(res);
                IList<IActivity> activities = new List<IActivity>();
                foreach (TrailResultMarked trm in res)
                {
                    if (!activities.Contains(trm.selInfo.Activity))
                    {
                        activities.Add(trm.selInfo.Activity);
                    }
                }
                sels = TrailsItemTrackSelectionInfo.SetAndAdjustFromSelection(new IItemTrackSelectionInfo[] { sel }, activities, false);
            }
            return sels;
        }

        public static IList<IItemTrackSelectionInfo> SetAndAdjustFromSelectionToST
            (TrailsItemTrackSelectionInfo selected)
        {
            return SetAndAdjustFromSelectionToST(new List<TrailsItemTrackSelectionInfo> { selected });
        }

        public static IList<IItemTrackSelectionInfo> SetAndAdjustFromSelectionToST
    (IList<TrailsItemTrackSelectionInfo> selected)
        {
            IList<IItemTrackSelectionInfo> sels = new List<IItemTrackSelectionInfo>();
            foreach (TrailsItemTrackSelectionInfo t in selected)
            {
                sels.Add(t);
            }
            return SetAndAdjustFromSelection(sels, null, false);
        }

        public static IList<IItemTrackSelectionInfo> SetAndAdjustFromSelectionFromST
            (IList<IItemTrackSelectionInfo> selected, IEnumerable<IActivity> activities)
        {
           return SetAndAdjustFromSelection(selected, activities, true);
        }

        private static IList<IItemTrackSelectionInfo> SetAndAdjustFromSelection
            (IList<IItemTrackSelectionInfo> selected, IEnumerable<IActivity> activities, bool fromST)
        {
            if (selected == null || selected.Count == 0 || activities == null)
            {
                //Do not adjust selection
                return selected;
            }
            bool singleSelection = selected.Count == 0 || selected.Count == 1 && ! ( 
                selected[0].MarkedDistances != null && selected[0].MarkedDistances.Count > 1 ||
                selected[0].MarkedTimes != null && selected[0].MarkedTimes.Count > 1);

            IList<IItemTrackSelectionInfo> results = new List<IItemTrackSelectionInfo>();
            foreach (IItemTrackSelectionInfo sel in selected)
            {
                IActivity activity = null;
                if (sel is TrailsItemTrackSelectionInfo)
                {
                    activity = ((TrailsItemTrackSelectionInfo)sel).Activity;
                }
                else
                {
                    if (activities == null)
                    {
                        activities = UnitUtil.GetApplication().Logbook.Activities;
                    }
                    foreach (IActivity a in activities)
                    {
                        //In ST3.0.4068 (at least) only one activity is selected
                        if (a != null && sel.ItemReferenceId == a.ReferenceId)
                        {
                            activity = a;
                            break;
                        }
                    }
                }

                if (activity != null)
                {
                        //The distance is in unstopped/unpaused format
                    IDistanceDataTrack activityUnpausedDistanceMetersTrack =
                        ActivityInfoCache.Instance.GetInfo(activity).ActualDistanceMetersTrack;
                    TrailsItemTrackSelectionInfo tmpSel = new TrailsItemTrackSelectionInfo();
                    tmpSel.SetFromSelection(sel, activity);

                    if (fromST)
                    {
                        //Set MarkedTimes (or SelectedTime), used internally. No need to clear "unused"
                        if (sel.MarkedDistances != null && sel.MarkedDistances.Count > 0 && 
                            (sel.MarkedTimes == null || sel.MarkedTimes.Count == 0))
                        {
                            try
                            {
                                foreach (ValueRange<double> t in sel.MarkedDistances)
                                {
                                    DateTime d1 = activityUnpausedDistanceMetersTrack.GetTimeAtDistanceMeters(t.Lower);
                                    DateTime d2 = activityUnpausedDistanceMetersTrack.GetTimeAtDistanceMeters(t.Upper);
                                    AddMarkedOrSelectedTime(tmpSel, singleSelection, d1, d2);
                                }
                                tmpSel.MarkedDistances = null;
                            }
                            catch { }
                        }
                        if (sel.SelectedTime != null && sel.MarkedTimes == null)
                        {
                            try
                            {
                                AddMarkedOrSelectedTime(tmpSel, singleSelection, sel.SelectedTime.Lower, sel.SelectedTime.Upper);
                                tmpSel.SelectedTime = null;
                            }
                            catch { }
                        }
                        if (sel.SelectedDistance != null && sel.MarkedTimes == null)
                        {
                            tmpSel.MarkedTimes = new ValueRangeSeries<DateTime>();
                            try
                            {
                                DateTime d1 = activityUnpausedDistanceMetersTrack.GetTimeAtDistanceMeters(sel.SelectedDistance.Lower);
                                DateTime d2 = activityUnpausedDistanceMetersTrack.GetTimeAtDistanceMeters(sel.SelectedDistance.Upper);
                                AddMarkedOrSelectedTime(tmpSel, singleSelection, d1, d2);
                                tmpSel.SelectedDistance = null;
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        //To ST
                        //The standard in the plugin(time) to standard in ST core and omb's Track Coloring (unpaused distance)
                        if (sel.MarkedDistances == null &&
                            sel.MarkedTimes != null && sel.MarkedTimes.Count > 0)
                        {
                            try
                            {
                                tmpSel.MarkedDistances = new ValueRangeSeries<double>();
                                foreach (ValueRange<DateTime> t in sel.MarkedTimes)
                                {
                                    double d1 = activityUnpausedDistanceMetersTrack.GetInterpolatedValue(t.Lower).Value;
                                    double d2 = activityUnpausedDistanceMetersTrack.GetInterpolatedValue(t.Upper).Value;
                                    AddMarkedOrSelectedDistance(tmpSel, singleSelection, d1, d2);
                                }
                                tmpSel.MarkedTimes = null;
                            }
                            catch { }
                        }
                        if (sel.SelectedDistance == null &&
                            sel.SelectedTime != null)
                        {
                            try
                            {
                                tmpSel.SelectedDistance = new ValueRange<double>(
                                            activityUnpausedDistanceMetersTrack.GetInterpolatedValue(sel.SelectedTime.Lower).Value,
                                            activityUnpausedDistanceMetersTrack.GetInterpolatedValue(sel.SelectedTime.Upper).Value);
                                tmpSel.SelectedTime = null;
                            }
                            catch { }
                        }
                    }
                    results.Add(tmpSel);
                }
            }
            return results;
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
            if (singleSelection && (tmpSel.MarkedDistances == null || tmpSel.MarkedDistances.Count == 0))
            {
                //Could add to selected, single point marked
                tmpSel.SelectedDistance = vd;
                tmpSel.MarkedDistances = null;
            }
            else
            {
                if (tmpSel.MarkedDistances == null)
                {
                    tmpSel.MarkedDistances = new ValueRangeSeries<double>();
                }
                if (vd.Lower == vd.Upper)
                {
                    //fake, add a meter
                    vd = new ValueRange<double>(vd.Lower, vd.Upper + 1);
                }
                tmpSel.MarkedDistances.Add(vd);
            }
        }

        //ST IsPaused reports no pause if time matches, this also checks borders
        private static bool IsPause(DateTime time, IValueRangeSeries<DateTime> pauses)
        {
            bool res = DateTimeRangeSeries.IsPaused(time, pauses);
            if (!res)
            {
                foreach (IValueRange<DateTime> pause in pauses)
                {
                    if (time.CompareTo(pause.Lower) >= 0 &&
                        time.CompareTo(pause.Upper) <= 0)
                    {
                        res = true;
                        break;
                    }
                }
            }
            return res;
        }

        public static IList<IList<IGPSPoint>> GpsPoints(IGPSRoute gpsRoute, IValueRangeSeries<DateTime> selections)
        {
            IList<IList<IGPSPoint>> result = new List<IList<IGPSPoint>>();

            if (selections != null && selections.Count > 0 && gpsRoute != null && gpsRoute.Count > 1)
            {
                //selection and gps points are sorted without overlap so traverse over gps points only once
                //(previous version had much more complicated version, that also accounted for pauses)
                int i = 0;
                foreach (IValueRange<DateTime> sel in selections)
                {
                    IList<IGPSPoint> track = new List<IGPSPoint>();
                    //Use start/end "with priority", even if extra points are added. Care needed if returning GPSRoute
                    DateTime t =  DateTimeRangeSeries.Latest(sel.Lower, gpsRoute.StartTime);
                    ITimeValueEntry<IGPSPoint> pt = gpsRoute.GetInterpolatedValue(t);
                    if (pt != null)
                    {
                        track.Add(pt.Value);
                    }
                    while (i < gpsRoute.Count)
                    {
                        ITimeValueEntry<IGPSPoint> entry = gpsRoute[i];
                        DateTime time = gpsRoute.EntryDateTime(entry);
                        i++;
                        if (sel.Lower > time)
                        {
                            continue;
                        }
                        if (sel.Upper < time)
                        {
                            //Do not increase counter here, it could be needed
                            i--;
                            break;
                        }
                        track.Add(entry.Value);
                    }
                    t = DateTimeRangeSeries.Earliest(sel.Upper, gpsRoute.StartTime.AddSeconds(gpsRoute.TotalElapsedSeconds));
                    pt = gpsRoute.GetInterpolatedValue(t);
                    if (pt != null)
                    {
                        track.Add(pt.Value);
                    }
                    result.Add(track);
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
