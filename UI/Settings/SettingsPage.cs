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

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ZoneFiveSoftware.Common.Visuals;

namespace TrailsPlugin.UI.Settings {
	internal class SettingsPage : ISettingsPage, INotifyPropertyChanged {

		#region ISettingsPage Members

		private SettingsPageControl m_control = null;

		public System.Guid Id {
            get { return GUIDs.Settings; }
		}

		public IList<ISettingsPage> SubPages {
			get {
				return new List<ISettingsPage>();
			}
		}

		#endregion

		#region IDialogPage Members

		public Control CreatePageControl() {
			if (m_control == null) {
				m_control = new SettingsPageControl();
			}
			return m_control;
		}

		public bool HidePage() {
			return true;
			//throw new System.NotImplementedException();
		}

		public string PageName {
			get {
				return Properties.Resources.TrailsPluginName;
			}
		}

		public void ShowPage(string bookmark) {
			//throw new System.NotImplementedException();
		}

		public IPageStatus Status {
			get {
				return null;
			}
		}

		public void ThemeChanged(ITheme visualTheme) {
            if (m_control != null)
            {
                m_control.ThemeChanged(visualTheme);
            }
		}

		public string Title {
			get {
				return Properties.Resources.TrailsName;
			}
		}

		public void UICultureChanged(CultureInfo culture) {
            if (m_control != null)
            {
                m_control.UICultureChanged(culture);
            }
            RefreshPage();
		}

		#endregion

		#region INotifyPropertyChanged Members

#pragma warning disable 67
        public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		public void RefreshPage() {
			if (m_control != null) {
				m_control.Refresh();
			}
		}
	}
}
