/*
Copyright (C) 2010 Sylvain Gravel

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
using System.IO;
using System.Text;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;

namespace TrailsPlugin.Export //FilteredStatisticsPlugin
{
    class TemplateTrailsFilterCriteria
    {
        public TemplateTrailsFilterCriteria()
        {
        }
        public TemplateTrailsFilterCriteria(IActivity activity)
        {
            Activity = activity;
            BuildNamedZones();
        }

#region IFilterCriteria members

        public Guid ReferenceId
        {
            get { return GUIDs.FilteredStatisticsTemplateFilter; }
        }

        public IActivity Activity
        {
            set
            {
                if (m_Activity != value)
                {
                    m_Activity = value;
                    if (m_Activity == null)
                    {
                        TrailsFilterCriteriasProvider.Controller.Activities = new List<IActivity>();
                    }
                    else
                    {
                        TrailsFilterCriteriasProvider.Controller.Activities = new List<IActivity> { m_Activity };
                    }

                    BuildNamedZones();
                }
            }
        }

        public string DisplayName
        {
            get { return Properties.Resources.TrailName; }
        }

        public List<object> NamedZones
        {
            get { return m_NamedZones; }
        }

        public bool IsTemplateOnly
        {
            get
            {
                return false;
            }
        }

        public bool IsMainViewOnly
        {
            get
            {
                return false;
            }
        }

        public object TemplateCompatibleCriteria
        {
            get
            {
                return new TemplateTrailsFilterCriteria(m_Activity);
            }
        }

        public bool IsSerializable
        {
            get { return true; }
        }

        public void SerializeCriteria(Stream stream)
        {
        }

        public UInt16 DataVersion
        {
            get { return m_DataVersion; }
        }

        public object DeserializeCriteria(Stream stream, UInt16 version)
        {
            return this;
        }

        //public bool ValidateConsistency()
        //{
        //    return true;
        //}

        public event PropertyChangedEventHandler NamedZonedListChanged;
#endregion

        private void BuildNamedZones()
        {
            m_NamedZones.Clear();

            IList<TrailsPlugin.Data.ActivityTrail> res = TrailsFilterCriteriasProvider.Controller.OrderedTrails;

            foreach (TrailsPlugin.Data.ActivityTrail trail in res)
            {
                if (!trail.Trail.Generated)
                {
                    m_NamedZones.Add(new TrailResultNamedZone(trail, m_Activity));
                }
            }
            TriggerNamedZonesListChanged();
        }

        protected void TriggerNamedZonesListChanged()
        {
            if (NamedZonedListChanged != null)
            {
                NamedZonedListChanged(this, new PropertyChangedEventArgs("NamedZones"));
            }
        }

        private const UInt16 m_DataVersion = 1;
        private IActivity m_Activity = null;
        private List<object> m_NamedZones = new List<object>();
    }
}
