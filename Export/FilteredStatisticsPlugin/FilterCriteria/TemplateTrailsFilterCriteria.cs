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
        public TemplateTrailsFilterCriteria(IActivity activity, string equipmentId)
        {
            Activity = activity;
            m_EquipmentId = equipmentId;

            //m_ActivityDataChangedHelper.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OnActivityDataChanged);
            //Common.Data.BikeSetupChanged += new Common.Data.BikeSetupChangedEventHandler(OnGearChartBikeSetupChanged);

            BuildNamedZones();
        }

        void OnActivityDataChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Activity.EquipmentUsed")
            {
                BuildNamedZones();
            }
        }

        //void OnGearChartBikeSetupChanged(object sender, string setupId)
        //{
        //    // Check if the modified setup is used by our current activity
        //    if (m_EquipmentId.Equals(setupId))
        //    {
        //        BuildNamedZones();
        //    }
        //}

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
                    //m_ActivityDataChangedHelper.Activity = m_Activity;

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
                return true;
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
                //TODO  Common.Data.GetActivityEquipmentId(m_Activity)
                return new TemplateTrailsFilterCriteria(m_Activity, "");
            }
        }

        public bool IsSerializable
        {
            get { return true; }
        }

        public void SerializeCriteria(Stream stream)
        {
            stream.Write(BitConverter.GetBytes(Encoding.UTF8.GetByteCount(m_EquipmentId)), 0, sizeof(Int32));
            stream.Write(Encoding.UTF8.GetBytes(m_EquipmentId), 0, Encoding.UTF8.GetByteCount(m_EquipmentId));
        }

        public UInt16 DataVersion
        {
            get { return m_DataVersion; }
        }

        public object DeserializeCriteria(Stream stream, UInt16 version)
        {
            return null;
        }

        public bool ValidateConsistency()
        {
            //TODO FS integration
            //IList<string> equipmentIds = new List<string>{""};// Common.Data.GetEquipmentIds();

            //return equipmentIds.IndexOf(EquipmentId) != -1;
            return true;
        }

        public event PropertyChangedEventHandler NamedZonedListChanged;

#endregion

        private void BuildNamedZones()
        {
            m_NamedZones.Clear();

            //List<SprocketCombo> sprockets = Common.Data.GetSprocketCombos(m_EquipmentId);

            //foreach(SprocketCombo gear in sprockets)
            //{
            //    m_NamedZones.Add(new GearNamedZone(m_Activity, gear));
            //}

            TriggerNamedZonesListChanged();
        }

        protected void TriggerNamedZonesListChanged()
        {
            if (NamedZonedListChanged != null)
            {
                NamedZonedListChanged(this, new PropertyChangedEventArgs("NamedZones"));
            }
        }

        public string EquipmentId
        {
            get { return m_EquipmentId; }
        }

        private const UInt16 m_DataVersion = 1;
        private IActivity m_Activity = null;
        //private ActivityDataChangedHelper m_ActivityDataChangedHelper = new ActivityDataChangedHelper(null);
        private string m_EquipmentId = null;
        private List<object> m_NamedZones = new List<object>();
    }
}
