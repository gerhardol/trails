/*
Copyright (C) 2009 Brendan Doherty
Copyright (C) 2011 Gerhard Olsson

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
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ITrailExport;
using TrailsPlugin.Data;
using TrailsPlugin.Utils;
using GpsRunningPlugin.Util;

namespace TrailsPlugin.Export
{
    public sealed class CFTrailResult : ITrailResult
    {
        public CFTrailResult(TrailResult tr)
        {
            this.m_tr = tr;
        }
        private TrailResult m_tr;

        /**********************************************************/
        #region Implementation of ITrailResult

        float ITrailResult.AvgCadence
        {
            get { return this.m_tr.AvgCadence; }
        }

        float ITrailResult.AvgGrade
        {
            get { return 100 * this.m_tr.AscAvgGrade; }
        }

        float ITrailResult.AvgHR
        {
            get { return this.m_tr.AvgHR; }
        }

        double ITrailResult.AvgPace
        {
            get { return (float)UnitUtil.Pace.ConvertFrom(this.m_tr.AvgSpeed); }
        }

        float ITrailResult.AvgPower
        {
            get { return this.m_tr.AvgPower; }
        }

        float ITrailResult.AvgSpeed
        {
            get { return (float)UnitUtil.Speed.ConvertFrom(this.m_tr.AvgSpeed); }
        }

        INumericTimeDataSeries ITrailResult.CadencePerMinuteTrack
        {
            get { return this.m_tr.CadencePerMinuteTrack0(); }
        }

        IActivityCategory ITrailResult.Category
        {
            get { return this.m_tr.Category; }
        }

        INumericTimeDataSeries ITrailResult.CopyTrailTrack(INumericTimeDataSeries source)
        {
            return this.m_tr.CopyTrailTrack(source);
        }

        string ITrailResult.Distance
        {
            get { return UnitUtil.Distance.ToString(this.m_tr.Distance, ""); }
        }

        IDistanceDataTrack ITrailResult.DistanceMetersTrack
        {
            get { return this.m_tr.DistanceMetersTrack; }
        }

        TimeSpan ITrailResult.Duration
        {
            get { return this.m_tr.Duration; }
        }

        string ITrailResult.ElevChg
        {
            get { return (this.m_tr.ElevChg > 0 ? "+" : "") + UnitUtil.Elevation.ToString(this.m_tr.ElevChg, ""); }
        }

        INumericTimeDataSeries ITrailResult.ElevationMetersTrack
        {
            get { return this.m_tr.ElevationMetersTrack0(); }
        }

        TimeSpan ITrailResult.EndTime
        {
            get { return this.m_tr.EndTimeOfDay; }
        }

        double ITrailResult.FastestPace
        {
            get
            {
                return UnitUtil.Pace.ConvertFrom(this.m_tr.FastestSpeed, this.m_tr.Activity);
            }
        }

        float ITrailResult.FastestSpeed
        {
            get
            {
                return (float)UnitUtil.Speed.ConvertFrom(this.m_tr.FastestSpeed, this.m_tr.Activity);
            }
        }

        INumericTimeDataSeries ITrailResult.GradeTrack
        {
            get
            {
                return this.m_tr.GradeTrack0();
            }
        }

        INumericTimeDataSeries ITrailResult.HeartRatePerMinuteTrack
        {
            get { return this.m_tr.HeartRatePerMinuteTrack0(); }
        }

        float ITrailResult.MaxHR
        {
            get { return this.m_tr.MaxHR; }
        }

        int ITrailResult.Order
        {
            get { return this.m_tr.Order; }
        }

        INumericTimeDataSeries ITrailResult.PaceTrack
        {
            get { return this.m_tr.PaceTrack0(); }
        }

        INumericTimeDataSeries ITrailResult.PowerWattsTrack
        {
            get { return this.m_tr.PowerWattsTrack0(); }
        }

        INumericTimeDataSeries ITrailResult.SpeedTrack
        {
            get { return this.m_tr.SpeedTrack0(); }
        }

        TimeSpan ITrailResult.StartTime
        {
            get { return this.m_tr.StartTimeOfDay; }
        }

        #endregion

    }
}
