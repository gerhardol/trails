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
using ZoneFiveSoftware.Common.Data.Measurement;

namespace TrailsPlugin.UI.Settings {
	public partial class SettingsPageControl : UserControl {
		public SettingsPageControl() {
			InitializeComponent();
            presentSettings();
		}

        private void presentSettings()
        {
            lblDefaultRadius.Text = Properties.Resources.UI_Settings_DefaultRadius + " :";
            txtDefaultRadius.Text = Utils.Units.ElevationToString(PluginMain.Settings.DefaultRadius, "u");

            toolTip.SetToolTip(txtDefaultRadius, Properties.Resources.UI_Settings_DefaultRadius_ToolTip);
        }
        public void ThemeChanged(ITheme visualTheme)
        {
			PluginInfoBanner.ThemeChanged(visualTheme);
			PluginInfoPanel.ThemeChanged(visualTheme);
			txtDefaultRadius.ThemeChanged(visualTheme);
		}

		private void txtDefaultRadius_LostFocus(object sender, EventArgs e) {
			float result;
            result = Utils.Units.ParseElevation(txtDefaultRadius.Text);
            if (result > 0)
            {
				PluginMain.Settings.DefaultRadius = result;
            }
            else
            {
                MessageBox.Show(Properties.Resources.UI_Activity_EditTrail_RadiusNumeric);
            }
            presentSettings();
        }

	}
}
