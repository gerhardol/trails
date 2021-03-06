﻿/*
Copyright (C) 2009 Brendan Doherty

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

//Used in both Trails and Matrix plugin

using System.Xml;
using System;
using System.Collections.Generic;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;

namespace TrailsPlugin.Data
{
    public class TrailResultInfo : IComparable
    {
        public IList<TrailResultPoint> Points;
        public IActivity Activity;
        public float? m_DistDiff;
        public bool Reverse;
        public ILapInfo LapInfo;

        public IPoolLengthInfo PoolLengthInfo
        {
            get
            {
                IList<IPoolLengthInfo> res = new List<IPoolLengthInfo>();
                for(int i = 0; i < this.Points.Count-1; i++)
                {
                    //a summary of all but the end point
                    TrailResultPoint trp = this.Points[i];
                    if (trp.SubPoints.Count == 0)
                    {
                        if (trp.PoolLengthInfo != null)
                        {
                            res.Add(trp.PoolLengthInfo);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < trp.SubPoints.Count - 1; j++)
                        {
                            //a summary of all but the end point
                            TrailResultPoint strp = trp.SubPoints[j];
                            if (strp.PoolLengthInfo != null)
                            {
                                res.Add(strp.PoolLengthInfo);
                            }
                        }

                    }
                }
                if (res.Count>0)
                {
                    return new PoolLengthInfo(res);
                }
                return null;
            }
        }

        public TrailResultInfo(IActivity activity, bool reverse)
        {
            this.Activity = activity;
            this.Points = new List<TrailResultPoint>();
            this.m_DistDiff = null;
            this.Reverse = reverse;
        }

        public float DistDiff
        {
            get
            {
                if (m_DistDiff == null)
                {
                    m_DistDiff = 0;
                    foreach (TrailResultPoint t in this.Points)
                    {
                        m_DistDiff = t.DistDiff + (float)m_DistDiff;
                    }
                }
                return (float)m_DistDiff;
            }
        }

        //Hide the handling slightly
        public TrailResultInfo Copy()
        {
            TrailResultInfo result = new TrailResultInfo(this.Activity, this.Reverse);
            result.m_DistDiff = this.m_DistDiff;
            result.LapInfo = this.LapInfo;
            foreach (TrailResultPoint p in Points)
            {
                result.Points.Add(p);
            }
            return result;
        }

        //Handle the points as a child result - promote info
        public TrailResultInfo CopyToChild(int i, int j)
        {
            TrailResultInfo result = new TrailResultInfo(this.Activity, this.Reverse);
            result.Points.Add(new TrailResultPoint(this.Points[i]));
            result.Points.Add(new TrailResultPoint(this.Points[j]));
            //Point 0 lap data applies to this result (PoolLengthInfo is extracted from point 0)
            result.LapInfo = this.Points[i].LapInfo;
            return result;
        }

        public IList<DateTime> CopyTime()
        {
            IList<DateTime> result = new List<DateTime>();
            foreach (TrailResultPoint p in Points)
            {
                result.Add(p.Time);
            }
            return result;
        }

        public TrailResultInfo CopyFromReference(IActivity activity)
        {
            TrailResultInfo result = new TrailResultInfo(activity, this.Reverse);
            foreach (TrailResultPoint p in Points)
            {
                TrailGPSLocation loc;
                ZoneFiveSoftware.Common.Data.GPS.IGPSPoint t = Utils.TrackUtil.getGpsLoc(activity, p.Time);
                if (t != null)
                {
                    loc = new TrailGPSLocation(t, p.Name, p.Required);
                }
                else
                {
                    loc = new TrailGPSLocation(p.Name, p.Required);
                }

                TrailResultPoint p2 = new TrailResultPoint(p, loc);
                p2.LapInfo = null;
                p2.PoolLengthInfo = null;
                result.Points.Add(p2);
            }
            return result;
        }

        public string Name
        {
            get
            {
                if (Points.Count > 0)
                {
                    return Points[0].Name;
                }
                else if (LapInfo != null)
                {
                    string name = LapInfo.Notes;
                    if (string.IsNullOrEmpty(name))
                    {
                        //TBD - order when adding
                        name = "#" + (this.Points.Count + 1);
                    }

                    return LapInfo.Notes;
                }
                return "";
            }
        }

        public int Count
        {
            get
            {
                return Points.Count;
            }
        }

        public int CompareTo(object obj)
        {
            if (obj is TrailResultInfo)
            {
                TrailResultInfo t = obj as TrailResultInfo;
                if (this.DistDiff == t.DistDiff)
                {
                    return 0;
                }
                return this.DistDiff < t.DistDiff ? -1 : 1;
            }
            return -1;
        }
    }

    public class SortResultPointsByTime : IComparer<TrailResultPoint>
    {
        public int Compare(TrailResultPoint x, TrailResultPoint y)
        {
            return x.Time < y.Time ? -1 : 1;
        }
    }

    public class TrailResultPoint : TrailGPSLocation, IComparable
    {
        //public TrailResultPoint(DateTime time, string name, bool active, float distDiff)
        //{
        //    this.m_time = time;
        //    this.m_name = name;
        //    this.Active = active;
        //    this.DistDiff = distDiff;
        //}
        //public TrailResultPoint(DateTime time, string name, bool active)
        //    : this(time, name, active, 0)
        //{
        //}
        //public TrailResultPoint(DateTime time, string name)
        //    : this(time, name, true, 0)
        //{
        //}
        public TrailResultPoint(TrailGPSLocation trailLocation, DateTime time, float distDiff)
            : base(trailLocation)
        {
            this.m_time = time;
            this.DistDiff = distDiff;
        }
        public TrailResultPoint(TrailGPSLocation trailLocation, DateTime time)
            : this(trailLocation, time, 0)
        {
        }
        public TrailResultPoint(TrailGPSLocation trailLocation, DateTime time, TimeSpan duration)
            : this(trailLocation, time)
        {
            this.Duration = duration;
        }
        public TrailResultPoint(TrailGPSLocation trailLocation, DateTime time, TimeSpan duration, ILapInfo lapInfo)
            : this(trailLocation, time, duration)
        {
            this.LapInfo = lapInfo;
        }
        public TrailResultPoint(TrailGPSLocation trailLocation, DateTime time, TimeSpan duration, IPoolLengthInfo lapInfo, int subresultIndex)
            : this(trailLocation, time, duration)
        {
            this.PoolLengthInfo = lapInfo;
            this.Order = subresultIndex;
        }
        public TrailResultPoint(TrailResultPoint t)
            : this(t,t)
        {
        }
        public TrailResultPoint(TrailResultPoint t, TrailGPSLocation loc)
            : base(loc)
        {
            this.m_time = t.Time;
            //this.m_name = t.Name;
            this.DistDiff = t.DistDiff;
            this.Duration = t.Duration;
            this.LapInfo = t.LapInfo;
            this.PoolLengthInfo = t.PoolLengthInfo;
            this.Order = t.Order;
            foreach (TrailResultPoint t2 in t.SubPoints)
            {
                this.SubPoints.Add(t2);
            }
        }

        public void Merge(TrailResultPoint t)
        {
            //Merge with later point, some fields kept
            this.Duration += t.Duration;
            if (this.LapInfo != null && t.LapInfo != null)
            {
                this.LapInfo = new LapInfo(this.LapInfo, t.LapInfo);
            }
            else if (t.LapInfo != null)
            {
                this.LapInfo = t.LapInfo;
            }

            if (this.PoolLengthInfo != null && t.PoolLengthInfo != null)
            {
                this.PoolLengthInfo = new PoolLengthInfo(this.PoolLengthInfo, t.PoolLengthInfo);
            }
            else if (t.PoolLengthInfo != null)
            {
                this.PoolLengthInfo = t.PoolLengthInfo;
            }

            foreach(TrailResultPoint t2 in t.SubPoints)
            {
                this.SubPoints.Add(t2);
            }
        }

        public override string ToString()
        {
            return this.Name + " " + m_time;
        }

        private DateTime m_time;
        /// <summary>
        /// The time the point was hit, DateTime.MinValue for "invalid" points
        /// </summary>
        // TBD: Change "invalid" handling to something else than MinValue, to handle routes without time somehow
        public DateTime Time
        {
            get
            {
                return m_time;
            }
            set
            {
                this.m_time = value;
            }
        }
        //private string m_name;
        //public string Name
        //{
        //    get
        //    {
        //        return m_name;
        //    }
        //    set
        //    {
        //        this.m_name = value;
        //    }
        //}
        //TrailGPSLocation TrailLocation = null;
        //public bool Active = true;
        public float DistDiff = 0;
        //Just a high number, affects sorting
        public const float DiffDistMax = 0xffff;
        internal TimeSpan? Duration = null;
        public ILapInfo LapInfo = null;
        public IPoolLengthInfo PoolLengthInfo = null;
        public int Order = -1;
        //SubPoints - only swimming now
        public IList<TrailResultPoint> SubPoints = new List<TrailResultPoint>();

        public int CompareTo(object obj)
        {
            if (obj is TrailResultPoint)
            {
                TrailResultPoint t = obj as TrailResultPoint;
                if (this.DistDiff == t.DistDiff)
                {
                    return this.Time < t.Time ? -1 : 1;
                }
                return this.DistDiff < t.DistDiff ? -1 : 1;
            }
            return -1;
        }
    }
}
