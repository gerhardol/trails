/******************************************************************************

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
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace TrailsPlugin.UI.Activity {
	internal class ActivityDetailPage : IActivityDetailPage, IDialogPage, INotifyPropertyChanged {
		private IActivity m_activity = null;
		private ActivityDetailPageControl m_control = null;

		public Control CreatePageControl() {
			if (m_control == null) {				
				m_control = new ActivityDetailPageControl(m_activity);
			}
			return m_control;
		}

		public bool HidePage() {
			//control.ShowPage = false;
			return true;
		}

		public void RefreshPage() {
			if (m_control != null) {
				m_control.Refresh();
			}
		}

		public void ShowPage(string bookmark) {
			//control.ShowPage = true;
		}

		public void ThemeChanged(ITheme visualTheme) {
			if (m_control != null) {
				m_control.ThemeChanged(visualTheme);
				RefreshPage();
			}
		}

		public void UICultureChanged(CultureInfo culture) {
			RefreshPage();
		}

		public IActivity Activity {
			set {
				m_activity = value;

				if (m_control != null) {
					m_control.Activity = value;
				}
			}
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
				return PluginView.GetLocalizedString("TrailsName");
			}
		}


		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}
