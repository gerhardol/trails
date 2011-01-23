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

        //void OnBikeSetupChanged(object sender, string setupId)
        //{
        //    TriggerFilterCriteriasChanged();
        //}

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

            index = AddGenericCriteria(index, typeof(TrailsFilterCriteria));
            index = AddGenericCriteria(index, typeof(TemplateTrailsPlaceholderFilterCriteria));

            if (m_Criterias.Count > index)
            {
                m_Criterias.RemoveRange(index, m_Criterias.Count - index);
            }

            // Register on the template UI callback
            if (!m_TemplateSelectionPlaceholderRegistered)
            {
                (m_Criterias[1] as TemplateTrailsPlaceholderFilterCriteria).TrailsCriteriaSelected += new TemplateTrailsPlaceholderFilterCriteria.TrailsCriteriaSelectedEventHandler(OnTemplateTrailsCriteriaSelected);

                m_TemplateSelectionPlaceholderRegistered = true;
            }

            TriggerFilterCriteriasChanged();
        }

        void OnTemplateTrailsCriteriaSelected(TemplateTrailsPlaceholderFilterCriteria criteria, object previousCriteria, out object resultCriteria)
        {
            //TODO 
            //TrailSelectorBox dlg = new TrailSelectorBox(previousCriteria);

            //dlg.ShowDialog();
            //if (dlg.DialogResult == DialogResult.OK)
            //{
            //    resultCriteria = new TemplateTrailsFilterCriteria(null, dlg.SelectedEquipmentId);
            //}
            //else
            //{
            //    resultCriteria = previousCriteria;
            //}
            resultCriteria = null;
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

        private List<object> m_Criterias = new List<object>();
        private bool m_TemplateSelectionPlaceholderRegistered = false;
    }
}
