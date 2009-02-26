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

		public event PropertyChangedEventHandler PropertyChanged;

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
			m_control.ThemeChanged(visualTheme);
			RefreshPage();
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

	}
}
