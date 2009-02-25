using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;


namespace TrailsPlugin.UI.Settings {
	public partial class SettingsPageControl : UserControl {
		public SettingsPageControl() {
			InitializeComponent();
		}

		public void ThemeChanged(ITheme visualTheme) {
		}
	}
}
