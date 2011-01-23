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
using System.IO;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;

namespace TrailsPlugin.Export //FilteredStatisticsPlugin
{
	class TrailsFilterCriteria
    {
        public TrailsFilterCriteria() :
            this(null)
        {
        }

        public TrailsFilterCriteria(IActivity activity)
        {
            Activity = activity;

            BuildNamedZones();
        }

        void OnActivityDataChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Activity.EquipmentUsed")
            {
                BuildNamedZones();
            }
        }

#region IFilterCriteria members

        public Guid ReferenceId
        {
            get { return GUIDs.FilteredStatisticsFilter; }
        }

        public IActivity Activity
        {
            set
            {
                if (m_Activity != value)
                {
                    m_Activity = value;
                    Controller.Activities = new List<IActivity> { m_Activity };

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
                return true;
            }
        }
        
        //public object TemplateCompatibleCriteria
        //{
        //    get
        //    {
        //        //TODO
        //        return new TemplateTrailsFilterCriteria(m_Activity, "");
        //    }
        //}

        public bool IsSerializable
        {
            get { return false; }
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

        public event PropertyChangedEventHandler NamedZonedListChanged;

#endregion

        private void BuildNamedZones()
        {
            m_NamedZones.Clear();

            if (m_Activity != null)
            {
                IList<TrailsPlugin.Data.ActivityTrail> res = Controller.OrderedTrails;

                foreach(TrailsPlugin.Data.ActivityTrail trail in res)
                {
                    if (trail.status <= TrailsPlugin.Data.TrailOrderStatus.MatchNoCalc)
                    {
                        foreach (TrailsPlugin.Data.TrailResult tr in trail.Results)
                        {
                            m_NamedZones.Add(new TrailResultNamedZone(trail, tr));
                        }
                    }
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

        private TrailsPlugin.Controller.TrailController Controller
        {
            get
            {
                if (m_controller == null)
                {
                    m_controller = TrailsPlugin.Controller.TrailController.Instance;
                }
                return m_controller;
            }
        }
        private TrailsPlugin.Controller.TrailController m_controller;
        private const UInt16 m_DataVersion = 1; 
        private IActivity m_Activity = null;
        private List<object> m_NamedZones = new List<object>();
        public delegate void TrailsCriteriaSelectedEventHandler(TrailsFilterCriteria criteria, object previousCriteria, out object resultCriteria);
        public event TrailsCriteriaSelectedEventHandler TrailsCriteriaSelected;
    }
}
