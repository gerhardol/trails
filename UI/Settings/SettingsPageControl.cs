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
			txtDefaultRadius.Text = PluginMain.Settings.DefaultRadius.ToString();
			toolTip.SetToolTip(txtDefaultRadius, "Default radius for trail points when adding new trails.");
		}

		public void ThemeChanged(ITheme visualTheme) {
			PluginInfoBanner.ThemeChanged(visualTheme);
			PluginInfoPanel.ThemeChanged(visualTheme);
		}

		private void txtDefaultRadius_Validating(object sender, CancelEventArgs e) {
			int result;
			if (int.TryParse(txtDefaultRadius.Text, out result)) {
				PluginMain.Settings.DefaultRadius = result;
			} else {
				e.Cancel = true;
			}
		}

	}
}
