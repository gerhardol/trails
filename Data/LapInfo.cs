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
    public class LapInfo : ILapInfo
    {
        public LapInfo(ILapInfo l)
        {
            this.TotalTime = l.TotalTime;
            this.TotalDistanceMeters = l.TotalDistanceMeters;

            this.AverageCadencePerMinute = l.AverageCadencePerMinute;
            this.AverageHeartRatePerMinute = l.AverageHeartRatePerMinute;
            this.AveragePowerWatts = l.AveragePowerWatts;
            this.ElevationChangeMeters = l.ElevationChangeMeters;
            this.m_PoolLengths = l.PoolLengths;
            this.Rest = l.Rest;
            this.StartTime = l.StartTime;
            this.TotalCalories = l.TotalCalories;
        }

        public LapInfo(ILapInfo k, ILapInfo l)
        {
            this.TotalTime = k.TotalTime + l.TotalTime;
            this.TotalDistanceMeters = k.TotalDistanceMeters + l.TotalDistanceMeters;

            this.TotalCalories = k.TotalCalories + l.TotalCalories;

            this.AverageCadencePerMinute = (float)((k.AverageCadencePerMinute * k.TotalTime.TotalSeconds + l.AverageCadencePerMinute * l.TotalTime.TotalSeconds) /
                this.TotalTime.TotalSeconds);
            this.AverageHeartRatePerMinute = (float)((k.AverageHeartRatePerMinute * k.TotalTime.TotalSeconds + l.AverageHeartRatePerMinute * l.TotalTime.TotalSeconds) /
                this.TotalTime.TotalSeconds);
            this.AveragePowerWatts = (float)((k.AveragePowerWatts * k.TotalTime.TotalSeconds + l.AveragePowerWatts * l.TotalTime.TotalSeconds) /
                this.TotalTime.TotalSeconds);

            //The next fields are not really mergeable manually...
            this.Rest = k.Rest && l.Rest;
            this.ElevationChangeMeters = k.ElevationChangeMeters + l.ElevationChangeMeters;
            this.StartTime = k.StartTime < l.StartTime ? k.StartTime : l.StartTime;
            //TBD - ILapPoolLengths not implemented, not a limitation
            //this.m_PoolLengths = new PoolLengthInfo(k.PoolLengths, l.PoolLengths);
        }

        private float m_AverageCadencePerMinute;
        public float AverageCadencePerMinute
        {
            get
            {
                return m_AverageCadencePerMinute;
            }

            set
            {
                m_AverageCadencePerMinute = value;
            }
        }

        private float m_AverageHeartRatePerMinute;
        public float AverageHeartRatePerMinute
        {
            get
            {
                return m_AverageHeartRatePerMinute;
            }

            set
            {
                m_AverageHeartRatePerMinute = value;
            }
        }

        private float m_AveragePowerWatts;
        public float AveragePowerWatts
        {
            get
            {
                return m_AveragePowerWatts;
            }

            set
            {
                m_AveragePowerWatts = value;
            }
        }

        private float m_ElevationChangeMeters;
        public float ElevationChangeMeters
        {
            get
            {
                return m_ElevationChangeMeters;
            }

            set
            {
                m_ElevationChangeMeters = value;
            }
        }

        private string m_Notes;
        public string Notes
        {
            get
            {
                return m_Notes;
            }

            set
            {
                m_Notes = value;
            }
        }

        private ILapPoolLengths m_PoolLengths;
        public ILapPoolLengths PoolLengths
        {
            get
            {
                return m_PoolLengths;
            }
        }

        private bool m_Rest;
        public bool Rest
        {
            get
            {
                return m_Rest;
            }

            set
            {
                m_Rest = value;
            }
        }

        private DateTime m_StartTime;
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

        private float m_TotalCalories;
        public float TotalCalories
        {
            get
            {
                return m_TotalCalories;
            }

            set
            {
                m_TotalCalories = value;
            }
        }

        private float m_TotalDistanceMeters;
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

        private TimeSpan m_TotalTime;
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

#pragma warning disable 67
        public event PropertyChangedEventHandler PropertyChanged;
    }
}