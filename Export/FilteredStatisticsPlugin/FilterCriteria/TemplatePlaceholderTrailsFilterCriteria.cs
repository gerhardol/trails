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
using System.Diagnostics;
using System.IO;
using System.Text;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;

namespace TrailsPlugin.Export //FilteredStatisticsPlugin
{
    class TemplateTrailsPlaceholderFilterCriteria
    {
        //public TemplateTrailsPlaceholderFilterCriteria()
        //{
        //}

#region IFilterCriteria members

        public Guid ReferenceId
        {
            get { return GUIDs.FilteredStatisticsTemplateFilter; }
        }

        public IActivity Activity
        {
            set { }
        }

        public string DisplayName
        {
            get { return Properties.Resources.TrailName; }
        }

        public List<object> NamedZones
        {
            get { return null; }
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
            get { return null; }
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
            if (version >= 1)
            {
                byte[] intBuffer = new byte[sizeof(Int32)];
                byte[] stringBuffer;
                Int32 stringLength;

                stream.Read(intBuffer, 0, sizeof(Int32));
                stringLength = BitConverter.ToInt32(intBuffer, 0);
                stringBuffer = new byte[stringLength];
                stream.Read(stringBuffer, 0, stringLength);

                return new TemplateTrailsFilterCriteria(null, Encoding.UTF8.GetString(stringBuffer));
            }
            else
            {
                throw new Exception("Invalid version");
            }
        }

        public object OnSelectedInList(object previousSelection)
        {
            if (TrailsCriteriaSelected != null)
            {
                // Make sure we have maximum 1 registered object since we have a return value
                Debug.Assert(TrailsCriteriaSelected.GetInvocationList().GetLength(0) == 1);

                object result;
                TrailsCriteriaSelected(this, previousSelection, out result);

                return result;
            }

            return previousSelection;
        }

        public event PropertyChangedEventHandler NamedZonedListChanged;

#endregion

        protected void TriggerNamedZonesListChanged()
        {
            if (NamedZonedListChanged != null)
            {
                NamedZonedListChanged(this, new PropertyChangedEventArgs("NamedZones"));
            }
        }

        private const UInt16 m_DataVersion = 1;
        public delegate void TrailsCriteriaSelectedEventHandler(TemplateTrailsPlaceholderFilterCriteria criteria, object previousCriteria, out object resultCriteria);
        public event TrailsCriteriaSelectedEventHandler TrailsCriteriaSelected;
    }
}
