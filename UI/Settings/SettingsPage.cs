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
			m_control.ThemeChanged(visualTheme);
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
