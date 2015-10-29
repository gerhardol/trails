/*
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
                    if (this.Points[i].PoolLengthInfo != null)
                    {
                        res.Add(this.Points[i].PoolLengthInfo);
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
            foreach (TrailResultPoint p in Points)
            {
                result.Points.Add(p);
            }
            return result;
        }
        public TrailResultInfo CopySlice(int i, int j)
        {
            TrailResultInfo result = new TrailResultInfo(this.Activity, this.Reverse);
            result.Points.Add(new TrailResultPoint(this.Points[i]));
            result.Points.Add(new TrailResultPoint(this.Points[j]));
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
            : base(t)
        {
            this.m_time = t.Time;
            //this.m_name = t.Name;
            this.DistDiff = t.DistDiff;
            this.Duration = t.Duration;
            this.LapInfo = t.LapInfo;
            this.PoolLengthInfo = t.PoolLengthInfo;
            this.Order = t.Order;
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
