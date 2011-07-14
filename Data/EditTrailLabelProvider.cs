/*
Copyright (C) 2010 Gerhard Olsson

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
using ZoneFiveSoftware.Common.Visuals;
using System.Drawing;
using GpsRunningPlugin.Util;

namespace TrailsPlugin.Data
{
    class EditTrailRow
    {
        private TrailGPSLocation m_gpsLoc;
        private int m_trailPointIndex;
        public DateTime? m_date;
        public double? m_distance;
        public double? m_time;
        //public bool m_firstRow = false;
        
        public EditTrailRow(TrailGPSLocation loc)
        {
            m_gpsLoc = loc;
        }
        private EditTrailRow(TrailGPSLocation loc, TrailResult tr, int i)
            : this(loc)
        {
            m_trailPointIndex = i;
            if (tr != null && tr.TrailPointDateTime != null && 
                tr.TrailPointDateTime.Count > i && tr.TrailPointDateTime[i] > DateTime.MinValue)
            {
                m_date = tr.TrailPointDateTime[i];
                //m_distance = tr.TrailPointDist0(tr)[i];
                //m_time = tr.TrailPointTime0(tr)[i];
                try
                {
                    ITimeValueEntry<float> entry = tr.ActivityDistanceMetersTrack.GetInterpolatedValue(tr.TrailPointDateTime[i]);
                    m_distance = UnitUtil.Distance.ConvertFrom(entry.Value, tr.Activity);
                    m_time = entry.ElapsedSeconds;
                }
                catch { }
            }
            //m_firstRow = false;
        }
        public static IList<EditTrailRow> getEditTrailRows(IList<TrailGPSLocation> tgps, TrailResult tr)
        {
            IList<EditTrailRow> result = new List<EditTrailRow>();
            //bool firstValid = false;
            foreach (TrailGPSLocation t in tgps)
            {
                EditTrailRow row = new EditTrailRow(t, tr, result.Count);
                result.Add(row);
                //if (!firstValid && tr != null &&
                //    tr.TrailPointDateTime[result.Count - 1] > DateTime.MinValue)
                //{
                //    firstValid = true;
                //    row.m_firstRow = true;
                //    try
                //    {
                //        ITimeValueEntry<float> entry = tr.ActivityDistanceMetersTrack.GetInterpolatedValue(tr.TrailPointDateTime[result.Count - 1]);
                //        row.m_distance = UnitUtil.Distance.ConvertFrom(entry.Value, tr.Activity);
                //        row.m_time = entry.ElapsedSeconds;
                //    }
                //    catch { }
                //}
            }
            return result;
        }
        public static IList<TrailGPSLocation> getTrailGPSLocation(IList<EditTrailRow> rowData)
        {
            IList<TrailGPSLocation> result = new List<TrailGPSLocation>();
            foreach (EditTrailRow t in rowData)
            {
                result.Add(t.m_gpsLoc);
            }
            return result;
        }
        public TrailGPSLocation TrailGPS
        {
            get
            {
                return m_gpsLoc;
            }
            set
            {
                m_gpsLoc = value;
            }
        }
    }

    class EditTrailLabelProvider : TreeList.DefaultLabelProvider
    {

        #region ILabelProvider Members

        //public override Image GetImage(object element, TreeList.Column column)
        //{
        //    return base.GetImage(row.Activity, column);
        //}

        public override string GetText(object element, TreeList.Column column)
        {
            Data.EditTrailRow row = (EditTrailRow)(element);
            switch (column.Id)
            {
                case "Distance":
                    if (row.m_distance != null)
                    {
                        return UnitUtil.Distance.ToString((double)row.m_distance, "u", false);
                    }
                    else
                    {
                        return "";
                    }
                case "Time":
                    if (row.m_time != null)
                    {
                        return UnitUtil.Time.ToString((double)row.m_time);
                    }
                    else
                    {
                        return "";
                    }
                default:
                    return base.GetText(row.TrailGPS, column);
            }
        }

        #endregion
    }
}
