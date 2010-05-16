﻿/******************************************************************************

    This file is part of TrailsPlugin.

    TrailsPlugin is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TrailsPlugin is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TrailsPlugin.  If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

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
			get { throw new System.NotImplementedException(); }
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
				return PluginView.GetLocalizedString("TrailsPluginName");
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
				return "My Title";
			}
		}

		public void UICultureChanged(CultureInfo culture) {
			RefreshPage();
		}

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		public void RefreshPage() {
			if (m_control != null) {
				m_control.Refresh();
			}
		}
	}
}
