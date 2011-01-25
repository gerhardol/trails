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
using System.Reflection;
using System.Windows.Forms;
using ZoneFiveSoftware.Common.Data.Fitness;
using TrailsPlugin.UI.Activity;

namespace TrailsPlugin.Export //FilteredStatisticsPlugin
{
    class TrailsFilterCriteriasProvider
    {
        public TrailsFilterCriteriasProvider()
        {
            RebuildFilterCriterias();
        }

#region IFilterCriteriaProvider Members

        public IList<object> GetFilterCriterias(ILogbook logbook)
        {
            return m_Criterias;
        }

        public event PropertyChangedEventHandler FilterCriteriasChanged;

#endregion

        private void RebuildFilterCriterias()
        {
            Int32 index = 0;

            //Disable for now - could be confusing
            index = AddGenericCriteria(index, typeof(TrailsFilterCriteria));
            index = AddGenericCriteria(index, typeof(TemplateTrailsFilterCriteria));

            if (m_Criterias.Count > index)
            {
                m_Criterias.RemoveRange(index, m_Criterias.Count - index);
            }

            // Register on the template UI callback
            if (!m_TemplateSelectionPlaceholderRegistered)
            {
                //(m_Criterias[1] as TemplateTrailsPlaceholderFilterCriteria).TrailsCriteriaSelected += new TemplateTrailsPlaceholderFilterCriteria.TrailsCriteriaSelectedEventHandler(OnTemplateTrailsCriteriaSelected);
                m_TemplateSelectionPlaceholderRegistered = true;
            }

            TriggerFilterCriteriasChanged();
        }

        protected void TriggerFilterCriteriasChanged()
        {
            if (FilterCriteriasChanged != null)
            {
                FilterCriteriasChanged(this, new PropertyChangedEventArgs("FilterCriterias"));
            }
        }

        private Int32 AddGenericCriteria(Int32 index, Type criteriaType)
        {
            ConstructorInfo constructor = criteriaType.GetConstructor(System.Type.EmptyTypes);

            Debug.Assert(constructor != null);

            if (m_Criterias.Count == index)
            {
                m_Criterias.Add(constructor.Invoke(null));
            }
            else if (!(m_Criterias[index].GetType() == criteriaType))
            {
                m_Criterias[index] = constructor.Invoke(null);
            }

            return ++index;
        }

        public static TrailsPlugin.Controller.TrailController Controller
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
        private static TrailsPlugin.Controller.TrailController m_controller;
        private List<object> m_Criterias = new List<object>();
        private bool m_TemplateSelectionPlaceholderRegistered = false;
    }
}
