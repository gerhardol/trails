/*
Copyright (C) 2010 Sylvain Gravel
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

//From STGearChart plugin

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data;
using GpsRunningPlugin.Util;

namespace TrailsPlugin.Export //FilteredStatisticsPlugin
{
    class TrailResultNamedZone
    {
        public TrailResultNamedZone(TrailsPlugin.Data.ActivityTrail trail, IActivity activity)
        {
            m_Trail = trail;
            m_Activity = activity;

            m_ValidTimesDirty = true;
            TriggerValidTimesChanged();
        }
        public TrailResultNamedZone(TrailsPlugin.Data.ActivityTrail trail, TrailsPlugin.Data.TrailResult tr)
        {
            m_Trail = trail;
            m_TrailResult = tr;

            m_ValidTimesDirty = true;
            TriggerValidTimesChanged();
        }

#region INamedZone members

        public String Name
        {
            get
            {
                string result = m_Trail.Trail.Name;
                if (m_TrailResult != null)
                {
                    if (m_Trail.Trail.HighScore > 0)
                    {
                        result += " (" +
                            UnitUtil.Distance.ToString(m_TrailResult.Distance, "u") + ")";
                    }
                    else
                    {
                        result += " " + m_TrailResult.StartDateTime.ToLocalTime().ToString();
                    }
                }
                return result;
            }
        }
        public IValueRangeSeries<DateTime> ValidTimes
        {
            get
            {
                RefreshValidTimes();

                return m_ValidTimes;
            }
        }

        public event PropertyChangedEventHandler ValidTimesChanged;

#endregion

        protected void TriggerValidTimesChanged()
        {
            if (ValidTimesChanged != null)
            {
                ValidTimesChanged(this, new PropertyChangedEventArgs("ValidTimes"));
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is TrailResultNamedZone)
            {
                TrailResultNamedZone namedZone = obj as TrailResultNamedZone;

                return namedZone.m_Trail.Equals(m_Trail);
            }

            return base.Equals(obj);
        }

        private void RefreshValidTimes()
        {
            if (m_ValidTimesDirty)
            {
                m_ValidTimesDirty = false;
                m_ValidTimes.Clear();

                if (m_TrailResult != null)
                {
                    m_ValidTimes = m_TrailResult.getSelInfo();
                }
                else if (m_Activity != null)
                {
                    TrailsFilterCriteriasProvider.Controller.Activities = new List<IActivity> { m_Activity };
                    foreach (TrailsPlugin.Data.TrailResult tr in m_Trail.Results)
                    {
                        foreach(IValueRange<DateTime> t in tr.getSelInfo())
                        {
                            m_ValidTimes.Add(t);
                        }
                    }
                }
            }
        }

        private IActivity m_Activity = null;
        private TrailsPlugin.Data.ActivityTrail m_Trail;
        private TrailsPlugin.Data.TrailResult m_TrailResult = null;
        private IValueRangeSeries<DateTime> m_ValidTimes = new ValueRangeSeries<DateTime>();
        private bool m_ValidTimesDirty = false;
    }
}
