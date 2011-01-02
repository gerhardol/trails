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

        public void SetFromSelection(IItemTrackSelectionInfo t)
        {
            // Set from activity: this.ItemReferenceId = t.ItemReferenceId;
            this.MarkedDistances = t.MarkedDistances;
            this.m_MarkedTimes = t.MarkedTimes;
            this.m_SelectedDistance = t.SelectedDistance;
            this.m_SelectedTime = t.SelectedTime;
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
        public TrailsItemTrackSelectionInfo FirstSelection()
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
