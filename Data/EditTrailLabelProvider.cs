﻿/*
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
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Visuals;
using System.Drawing;
using GpsRunningPlugin.Util;

namespace TrailsPlugin.Data
{
    class EditTrailRow
    {
        private TrailGPSLocation m_gpsLoc;
        private int m_resultPointIndex;
        internal double? m_distance;
        internal double? m_time;
        internal double m_diff = float.NaN;
        
        public EditTrailRow(TrailGPSLocation loc)
        {
            m_gpsLoc = loc;
        }

        private EditTrailRow(TrailGPSLocation loc, TrailResult tr, int i)
            : this(loc)
        {
            this.m_resultPointIndex = i;
            if (tr != null && tr.TrailPointDateTime.Count > 0)
            {
                DateTime d = DateTime.MinValue;
                if (tr.TrailPointDateTime != null &&
                tr.TrailPointDateTime.Count > m_resultPointIndex && m_resultPointIndex >= 0)
                {
                    d = tr.TrailPointDateTime[m_resultPointIndex];
                }
                SetDistance(tr, d);
                this.m_diff = tr.PointDiff(this.m_resultPointIndex);
            }
        }

        private void SetDistance(TrailResult tr, DateTime d1)
        {
            //if (DateTime.MinValue == d1)
            //{
            //    //Try get the value from the track
            //    if (null != this.TrailGPS.DateTime)
            //    {
            //        d1 = (DateTime)this.TrailGPS.DateTime;
            //    }
            //}
            if (DateTime.MinValue != d1)
            {
                try
                {
                    m_distance = UnitUtil.Distance.ConvertFrom(tr.getDistActivity(d1), tr.Activity);
                    //Elapsed time is for the activity
                    m_time = tr.getTimeActivity(d1);
                }
                catch 
                { }
            }
        }

        public void UpdateRow(TrailResult tr, DateTime d1)
        {
            //ITimeValueEntry<IGPSPoint> entry = tr.Activity.GPSRoute.GetInterpolatedValue(d1);
            //if (entry != null)
            //{
            //    this.TrailGPS.GpsLocation = new GPSLocation(
            //        entry.Value.LatitudeDegrees,
            //        entry.Value.LongitudeDegrees);
            //}
            IGPSPoint t = Utils.TrackUtil.getGpsLoc(tr.Activity, d1);
            this.TrailGPS.GpsPoint = t;
            SetDistance(tr, d1);
        }

        public void UpdateRow(TrailResult tr, TrailGPSLocation t)
        {
            this.TrailGPS = t;
            SetDistance(tr, tr.TrailPointDateTime[m_resultPointIndex]);
        }

        public static IList<EditTrailRow> getEditTrailRows(Trail tgps, TrailResult tr)
        {
            IList<EditTrailRow> result = new List<EditTrailRow>();
            int i = 0;
            int inc = 1;
            //The result index may not match the trailGPS index
            if (tgps.IsCompleteActivity)
            {
                //An extra point at start/end has been added compared to the trail points
                i = 1;
            }
            if (tr.Reverse)
            {
                i = tr.TrailPointDateTime.Count - 1 - i;
                inc = -1;
            }
            foreach (TrailGPSLocation t in tgps.TrailLocations)
            {
                i += inc;
                EditTrailRow row = new EditTrailRow(t, tr, i);
                result.Add(row);
            }
            return result;
        }

        public static IList<TrailGPSLocation> getTrailGPSLocation(IList<EditTrailRow> rowData)
        {
            IList<TrailGPSLocation> result = new List<TrailGPSLocation>();
            if (rowData != null)
            {
                foreach (EditTrailRow t in rowData)
                {
                    result.Add(t.m_gpsLoc);
                }
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
                case "Diff":
                    return UnitUtil.Elevation.ToString(row.m_diff, "u");

                default:
                    return base.GetText(row.TrailGPS, column);
            }
        }

        #endregion
    }
}
