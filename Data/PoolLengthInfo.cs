/*
Copyright (C) 2010-2015 Gerhard Olsson

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

using System.Collections.Generic;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using System.Xml;
using System;
using System.ComponentModel;
using ZoneFiveSoftware.Common.Data.Measurement;

namespace TrailsPlugin.Data
{
    public class PoolLengthInfo : IPoolLengthInfo
    {
        public static IPoolLengthInfo GetPoolLength(IPoolLengthInfo p)
        {
            IPoolLengthInfo res = p;
            if (p.TotalDistanceMeters > 600)
            {
                //At least 920XT w fw 5.20 is imported incorrectly in ST
                res = new PoolLengthInfo(p);
                uint iByte = BitConverter.ToUInt32(BitConverter.GetBytes(p.TotalDistanceMeters), 0);
                if (iByte == 0x442338f6)
                {
                    res.TotalDistanceMeters = 25;
                }
                else if (iByte == 0x4423345c)
                {
                    res.TotalDistanceMeters = 50;
                }
                else if (iByte == 0x44233b85)
                {
                    res.TotalDistanceMeters = 33.333f;
                }
                else if (iByte == 0x44235852)
                {
                    res.TotalDistanceMeters = 150;
                }
                else if (iByte == 0x4423370a)
                {
                    res.TotalDistanceMeters = 17;
                }
                else if (iByte == 0x44233a3d)
                {
                    res.TotalDistanceMeters = (float)Length.Convert(33.333, Length.Units.Yard, Length.Units.Meter);
                }
                else if (iByte == 0x44233852)
                {
                    res.TotalDistanceMeters = (float)Length.Convert(25, Length.Units.Yard, Length.Units.Meter);
                }
            }
            return res;
        }

        private PoolLengthInfo(IPoolLengthInfo p)
        {
            this.m_StartTime = p.StartTime;
            this.m_StrokeCount = p.StrokeCount;
            this.m_StrokeType = p.StrokeType;
            this.m_TotalDistanceMeters = p.TotalDistanceMeters;
            this.m_TotalTime = p.TotalTime;
            this.m_DistanceUnits = p.DistanceUnits;
        }

        public static IPoolLengthInfo GetPoolLength(IList<TrailResult> list)
        {
            IList<IPoolLengthInfo> r = new List<IPoolLengthInfo>();
            foreach (TrailResult l in list)
            {
                if (l.PoolLengthInfo != null)
                {
                    r.Add(l.PoolLengthInfo);
                }
            }
            if (r.Count > 0)
            {
                return new PoolLengthInfo(r);
            }
            return null;
        }

        public PoolLengthInfo(IList<IPoolLengthInfo> laps)
        {
            this.m_StartTime = DateTime.MaxValue;
            this.m_StrokeCount = 0;
            this.m_StrokeType = SwimStroke.Type.Unspecified;
            this.m_TotalDistanceMeters = 0;
            this.m_TotalTime = TimeSpan.Zero;
            if (laps.Count > 0)
            {
                this.m_DistanceUnits = laps[0].DistanceUnits;
            }
            else
            {
                this.m_DistanceUnits = Length.Units.Meter;
            }
            foreach (IPoolLengthInfo p in laps)
            {
                this.m_StartTime = this.m_StartTime < p.StartTime ? this.m_StartTime : p.StartTime;
                this.m_StrokeCount += p.StrokeCount;
                if (p.StrokeType != SwimStroke.Type.Unspecified)
                {
                    if (this.m_StrokeType == SwimStroke.Type.Unspecified)
                    {
                        this.m_StrokeType = p.StrokeType;
                    }
                    else if (this.m_StrokeType != p.StrokeType)
                    {
                        this.m_StrokeType = SwimStroke.Type.Mixed;
                    }
                }
                if (!float.IsNaN(p.TotalDistanceMeters))
                {
                    this.m_TotalDistanceMeters += p.TotalDistanceMeters;
                }
                this.m_TotalTime += p.TotalTime;
            }
        }

        public float AverageStrokeDistance
        {
            get
            {
                return TotalDistanceMeters / StrokeCount;
            }
        }

        public float AverageStrokeRate
        {
            get
            {
                return (float)(StrokeCount / TotalTime.TotalSeconds);
            }
        }

        public Length.Units DistanceUnits
        {
            get
            {
                return m_DistanceUnits;
            }

            set
            {
                m_DistanceUnits = value;
            }
        }

        public float Efficiency
        {
            get
            {
                if (TotalDistanceMeters == 0) { return float.NaN; }
                return SWOLF * 25 / TotalDistanceMeters;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return m_StartTime;
            }

            set
            {
                m_StartTime = value;
            }
        }

        public int StrokeCount
        {
            get
            {
                return m_StrokeCount;
            }

            set
            {
                m_StrokeCount = value;
            }
        }

        public SwimStroke.Type StrokeType
        {
            get
            {
                return m_StrokeType;
            }

            set
            {
                m_StrokeType = value;
            }
        }

        public float SWOLF
        {
            get
            {
                return (float)(StrokeCount + TotalTime.TotalSeconds);
            }
        }

        public float TotalDistanceMeters
        {
            get
            {
                return m_TotalDistanceMeters;
            }

            set
            {
                m_TotalDistanceMeters = value;
            }
        }

        public TimeSpan TotalTime
        {
            get
            {
                return m_TotalTime;
            }

            set
            {
                m_TotalTime = value;
            }
        }

        private Length.Units m_DistanceUnits;
        private DateTime m_StartTime;
        private int m_StrokeCount;
        private SwimStroke.Type m_StrokeType;
        private float m_TotalDistanceMeters;
        private TimeSpan m_TotalTime;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}