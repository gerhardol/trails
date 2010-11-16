/*
Copyright (C) 2009 Brendan Doherty

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
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;
#if !ST_2_1
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Visuals.Util;
#endif

namespace TrailsPlugin.UI.Activity {
	internal class ActivityDetailPage :
#if ST_2_1
     IActivityDetailPage
#else
     IDetailPage
#endif
    {
#if !ST_2_1
        public ActivityDetailPage(IDailyActivityView view)
        {
            this.m_view = view;
            this.m_view.SelectionProvider.SelectedItemsChanged += new EventHandler(OnViewSelectedItemsChanged);
        }

        private void OnViewSelectedItemsChanged(object sender, EventArgs e)
        {
            m_activities = CollectionUtils.GetAllContainedItemsOfType<IActivity>(m_view.SelectionProvider.SelectedItems);
            if ((m_control != null))
            {
                m_control.Activities = m_activities;
            }
            RefreshPage();
        }
        public System.Guid Id { get { return GUIDs.Activity; } }
#else
		public IActivity Activity {
            set
            {
                if (null == value) { m_activities = new List<IActivity>(); }
                else { m_activities = new List<IActivity> { value }; }
                if ((m_control != null))
                {
                    m_control.Activities = m_activities;
                }
            }
		}
#endif
        private IList<IActivity> m_activities = new List<IActivity>();
		private ActivityDetailPageControl m_control = null;

		public Control CreatePageControl() {
			if (m_control == null) {				
#if ST_2_1
				m_control = new ActivityDetailPageControl();
#else
                m_control = new ActivityDetailPageControl(this, m_view);
#endif
                m_control.Activities = m_activities;
            }
			return m_control;
		}

		public bool HidePage() {
            if (null != m_control)
            {
                m_control.ShowPage = false;
            }
			return true;
		}

        public IList<string> MenuPath
        {
            get { return menuPath; }
            set { menuPath = value; OnPropertyChanged("MenuPath"); }
        }

        public bool MenuEnabled
        {
            get { return menuEnabled; }
            set { menuEnabled = value; OnPropertyChanged("MenuEnabled"); }
        }

        public bool MenuVisible
        {
            get { return menuVisible; }
            set { menuVisible = value; OnPropertyChanged("MenuVisible"); }
        }

        public bool PageMaximized
        {
            get { return pageMaximized; }
            set { pageMaximized = value; OnPropertyChanged("PageMaximized"); }
        }
        public void RefreshPage()
        {
			if (m_control != null) {
				m_control.Refresh();
			}
		}

		public void ShowPage(string bookmark) {
            if (m_control != null)
            {
                m_control.ShowPage = true;
            }
		}

		public void ThemeChanged(ITheme visualTheme) {
			if (m_control != null) {
				m_control.ThemeChanged(visualTheme);
				RefreshPage();
			}
		}

		public void UICultureChanged(CultureInfo culture) {
            if (m_control != null)
            {
                m_control.UICultureChanged(culture);
            }
            RefreshPage();
		}

		public string PageName {
			get {
				return Title;
			}
		}

		public IPageStatus Status {
			get { return null; }
		}

		public string Title {
			get {
				return Properties.Resources.TrailsName;
			}
		}


		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
#if !ST_2_1
        private IDailyActivityView m_view = null;
#endif
        private IList<string> menuPath = null;
        private bool menuEnabled = true;
        private bool menuVisible = true;
        private bool pageMaximized = false;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
