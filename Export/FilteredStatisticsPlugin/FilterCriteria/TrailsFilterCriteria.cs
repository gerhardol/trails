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

        //void OnActivityDataChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    BuildNamedZones();
        //}

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
                    //If activity is null, controler will handle the list as empty
                    //Do not keep selection, sort find best
                    TrailsPlugin.Controller.TrailController.Instance.SetActivities(new List<IActivity> { m_Activity }, false);

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

        public object TemplateCompatibleCriteria
        {
            get
            {
                return this;// new TemplateTrailsFilterCriteria(m_Activity);
            }
        }

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
            List<object> namedZones = new List<object>();

            if (m_Activity != null)
            {
                foreach (TrailsPlugin.Data.ActivityTrail trail in TrailsPlugin.Controller.TrailController.Instance.OrderedTrails())
                {
                    //TrailResults
                    if (!trail.Trail.Generated &&
                        (m_Activity == null || trail.Status < TrailsPlugin.Data.TrailOrderStatus.MatchNoCalc))
                    {
                        bool added = false;
                        foreach (object o in m_NamedZones)
                        {
                            TrailsPlugin.Data.ActivityTrail t2 = o as TrailsPlugin.Data.ActivityTrail;
                            if (t2 == trail)
                            {
                                namedZones.Add(o);
                                added = true;
                                break;
                            }
                        }
                        if (!added)
                        {
                            namedZones.Add(new TrailResultNamedZone(trail, m_Activity));
                        }
                    }

                    //Generated results. Only HighScore is interesting
                    if (trail.Trail.TrailType  == TrailsPlugin.Data.Trail.CalcType.HighScore && trail.Status <= TrailsPlugin.Data.TrailOrderStatus.MatchNoCalc)
                    {
                        foreach (TrailsPlugin.Data.TrailResult tr in TrailsPlugin.Data.TrailResultWrapper.Results(trail.ResultTreeList))
                        {
                            namedZones.Add(new TrailResultNamedZone(trail, tr));
                        }
                    }
                }
            }

            m_NamedZones = namedZones;
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
        //public delegate void TrailsCriteriaSelectedEventHandler(TrailsFilterCriteria criteria, object previousCriteria, out object resultCriteria);
        //public event TrailsCriteriaSelectedEventHandler TrailsCriteriaSelected;
    }
}
