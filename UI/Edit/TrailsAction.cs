/*
Copyright (C) 2016 Gerhard Olsson

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
using System.Collections.Generic;
using System.Text;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Data.Fitness;
using TrailsPlugin.Properties;
using TrailsPlugin.UI.Activity;
#if !ST_2_1
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ZoneFiveSoftware.Common.Visuals.Util;
#endif

namespace TrailsPlugin.UI.Edit
{
    class TrailsAction: IAction
    {
#if !ST_2_1
        public TrailsAction(IDailyActivityView view)
        {
            this.m_dailyView = view;
        }
        public TrailsAction(IActivityReportsView view)
        {
            this.m_reportView = view;
        }
#else
        public TrailsAction(IList<IActivity> activities)
        {
            this.activities = activities;
        }
#endif

        #region IAction Members

        public bool Enabled
        {
            get { return true; }
        }

        public bool HasMenuArrow
        {
            get { return false; }
        }

        public System.Drawing.Image Image
        {
            get { return TrailsPlugin.Properties.Resources.trails; }
        }

        public IList<string> MenuPath
        {
            get
            {
                return new List<string>();
            }
        }
        public void Refresh()
        {
        }

        public void Run(System.Drawing.Rectangle rectButton)
        {
            ActivityDetailPageControl t;
#if ST_2_1
                t = new ActivityDetailPageControl(this);
#else
            if (m_reportView != null)
            {
                //t = new ActivityDetailPageControl(m_reportView);
            }
            else
            {
                t = new ActivityDetailPageControl(m_dailyView);
                t.Activities = activities;
            }
#endif
        }

        public string Title
        {
            get 
            {
                return Resources.PluginTitle; 
            }
        }

        public bool Visible
        {
            get
            {
                return Data.Settings.PopupInActionMenu;
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

#pragma warning disable 67
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion
#if !ST_2_1
        private IDailyActivityView m_dailyView = null;
        private IActivityReportsView m_reportView = null;
#endif
        private IList<IActivity> m_activities = null;
        private IList<IActivity> activities
        {
            get
            {
#if !ST_2_1
                //activities are set either directly or by selection,
                //not by more than one view
                if (m_activities == null)
                {
                    if (m_dailyView != null)
                    {
                        return CollectionUtils.GetAllContainedItemsOfType<IActivity>(m_dailyView.SelectionProvider.SelectedItems);
                    }
                    else if (m_reportView != null)
                    {
                        return CollectionUtils.GetAllContainedItemsOfType<IActivity>(m_reportView.SelectionProvider.SelectedItems);
                    }
                    else
                    {
                        return new List<IActivity>();
                    }
                }
#endif
                return m_activities;
            }
            set
            {
                m_activities = value;
            }
        }
    }
}
