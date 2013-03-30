/*
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
using System.Drawing;
using System.Collections.Generic;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ITrailExport;

namespace TrailsPlugin.Data
{
    public class ActivityInfoOptions : IActivityInfoOptions
    {
        public ActivityInfoOptions(bool defaultSmoothing, bool includeStopped)
            : this(defaultSmoothing)
        {
            m_IncludeStopped = includeStopped;
        }
        public ActivityInfoOptions(bool defaultSmoothing)
            : base()
        {
            IAnalysisSettings a = TrailsPlugin.Plugin.GetApplication().SystemPreferences.AnalysisSettings;
            m_CadenceCutoff = a.CadenceCutoff;
            m_IncludePaused = a.IncludePaused;
            m_IncludeStopped = a.IncludeStopped;
            m_PowerCutoff = a.PowerCutoff;

            if (defaultSmoothing)
            {
                m_CadenceSmoothingSeconds = a.CadenceSmoothingSeconds;
                m_ElevationSmoothingSeconds = a.ElevationSmoothingSeconds;
                m_HeartRateSmoothingSeconds = a.HeartRateSmoothingSeconds;
                m_PowerSmoothingSeconds = a.PowerSmoothingSeconds;
                m_SpeedSmoothingSeconds = a.SpeedSmoothingSeconds;
            }
            else
            {
                m_CadenceSmoothingSeconds = 0;
                m_ElevationSmoothingSeconds = 0;
                m_HeartRateSmoothingSeconds = 0;
                m_PowerSmoothingSeconds = 0;
                m_SpeedSmoothingSeconds = 0;
            }
        }

        private int m_CadenceCutoff;
        private int m_CadenceSmoothingSeconds;
        private int m_ElevationSmoothingSeconds;
        public int m_HeartRateSmoothingSeconds;
        private bool m_IncludePaused;
        private bool m_IncludeStopped;
        private int m_PowerCutoff;
        private int m_PowerSmoothingSeconds;
        private int m_SpeedSmoothingSeconds;

        public int CadenceCutoff
        {
            get { return m_CadenceCutoff; }
            set { m_CadenceCutoff = value; }
        }

        public int CadenceSmoothingSeconds
        {
            get { return m_CadenceSmoothingSeconds; }
            set { m_CadenceSmoothingSeconds = value; }
        }

        public int ElevationSmoothingSeconds
        {
            get { return m_ElevationSmoothingSeconds; }
            set { m_ElevationSmoothingSeconds = value; }
        }

        public int HeartRateSmoothingSeconds
        {
            get { return m_HeartRateSmoothingSeconds; }
            set { m_HeartRateSmoothingSeconds = value; }
        }

        public bool IncludePaused
        {
            get { return m_IncludePaused; }
            set { m_IncludePaused = value; }
        }

        public bool IncludeStopped
        {
            get { return m_IncludeStopped; }
            set { m_IncludeStopped = value; }
        }

        public int PowerCutoff
        {
            get { return m_PowerCutoff; }
            set { m_PowerCutoff = value; }
        }

        public int PowerSmoothingSeconds
        {
            get { return m_PowerSmoothingSeconds; }
            set { m_PowerSmoothingSeconds = value; }
        }

        public int SpeedSmoothingSeconds
        {
            get { return m_SpeedSmoothingSeconds; }
            set { m_SpeedSmoothingSeconds = value; }
        }
    }
}
