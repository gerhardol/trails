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
    internal class ActivityDetailPage : IDisposable,
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
            m_prevViewId = view.Id;
            view.SelectionProvider.SelectedItemsChanged += new EventHandler(OnViewSelectedItemsChanged);
            Plugin.GetApplication().PropertyChanged += new PropertyChangedEventHandler(Application_PropertyChanged);
        }

        private void OnViewSelectedItemsChanged(object sender, EventArgs e)
        {
            m_activities = CollectionUtils.GetAllContainedItemsOfType<IActivity>(m_view.SelectionProvider.SelectedItems);
            if ((m_control != null))
            {
                m_control.Activities = m_activities;
            }
        }

        void Application_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Hide/Show the page if view is changing, to handle listeners and refresh
            if (m_showPage && e.PropertyName == "ActiveView")
            {
                Guid viewId = Plugin.GetApplication().ActiveView.Id;
                if (viewId == m_view.Id)
                {
                    if (m_control != null) { m_control.ShowPage(m_bookmark); }
                }
                else if (m_prevViewId == m_view.Id)
                {
                    if (m_control != null) { m_control.HidePage(); }
                }
                m_prevViewId = viewId;
            }
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
        public Control CreatePageControl()
        {
            if (m_control == null) {                
#if ST_2_1
                m_control = new ActivityDetailPageControl();
                m_control.Activities = m_activities;
#else
                m_control = new ActivityDetailPageControl(this, m_view);
#endif
            }
            return m_control;
        }

        public bool HidePage()
        {
            m_showPage = false;
#if !ST_2_1
            this.m_view.SelectionProvider.SelectedItemsChanged -= new EventHandler(OnViewSelectedItemsChanged);
#endif
            if (null != m_control)
            {
                m_control.HidePage();
            }
            return true;
        }

        public IList<string> MenuPath
        {
            get { return m_menuPath; }
            set { m_menuPath = value; OnPropertyChanged("MenuPath"); }
        }

        public bool MenuEnabled
        {
            get { return m_menuEnabled; }
            set { m_menuEnabled = value; OnPropertyChanged("MenuEnabled"); }
        }

        public bool MenuVisible
        {
            get { return m_menuVisible; }
            set { m_menuVisible = value; OnPropertyChanged("MenuVisible"); }
        }

        public bool PageMaximized
        {
            get { return m_pageMaximized; }
            set { m_pageMaximized = value; OnPropertyChanged("PageMaximized"); }
        }
        public void RefreshPage()
        {
            if (m_control != null) {
                m_control.Refresh();
            }
        }

        public void ShowPage(string bookmark)
        {
#if ST_2_1
            m_control.Activities = m_activities;
#else
            this.m_view.SelectionProvider.SelectedItemsChanged += new EventHandler(OnViewSelectedItemsChanged);
            //Set activities
            OnViewSelectedItemsChanged(this, null);
#endif
            m_showPage = true;
            m_bookmark = bookmark;
            if (m_control != null)
            {
                m_control.ShowPage(bookmark);
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
                return Properties.Resources.PluginTitle;
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
#if !ST_2_1
        private IDailyActivityView m_view = null;
        private Guid m_prevViewId;
#endif
        private bool m_showPage = false;
        private string m_bookmark = null;
        private IList<IActivity> m_activities = new List<IActivity>();
        private ActivityDetailPageControl m_control = null;
        private IList<string> m_menuPath = null;
        private bool m_menuEnabled = true;
        private bool m_menuVisible = true;
        private bool m_pageMaximized = false;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.m_control.Dispose();
            }
        }
    }
}
