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
            txtDefaultRadius.Text = Utils.Units.ElevationToString(Data.Settings.DefaultRadius, "u");
            txtSetNameAtImport.Checked = Data.Settings.SetNameAtImport;
        }
        public void ThemeChanged(ITheme visualTheme)
        {
			PluginInfoBanner.ThemeChanged(visualTheme);
			PluginInfoPanel.ThemeChanged(visualTheme);
			txtDefaultRadius.ThemeChanged(visualTheme);
		}
        public void UICultureChanged(System.Globalization.CultureInfo culture)
        {
            lblDefaultRadius.Text = Properties.Resources.UI_Settings_DefaultRadius + ":";
            toolTip.SetToolTip(txtDefaultRadius, Properties.Resources.UI_Settings_DefaultRadius_ToolTip);
            lblSetNameAtImport.Text = Properties.Resources.SetNameAtImport;
            presentSettings();

            this.lblUniqueRoutes.Text = Integration.UniqueRoutes.CompabilityText;
            this.lblHighScore.Text = Integration.HighScore.CompabilityText;
            //Some untranslated strings....
            this.lblLicense.Text = "Trails Plugin is distributed under the GNU Lesser General Public Licence.\r\nThe Li" +
                "cense is included in the plugin installation directory and at:\r\nhttp://www.gnu.o" +
                "rg/licenses/lgpl.html.";
            this.lblCopyright.Text = "Copyright Brendan Doherty 2009, Gerhard Olsson 2010";
            this.PluginInfoBanner.Text = "Plugin Information";
        }

		private void txtDefaultRadius_LostFocus(object sender, EventArgs e) {
			float result;
            result = Utils.Units.ParseElevation(txtDefaultRadius.Text);
            if (result > 0)
            {
                Data.Settings.DefaultRadius = result;
            }
            else
            {
                MessageBox.Show(Properties.Resources.UI_Activity_EditTrail_RadiusNumeric);
            }
            presentSettings();
        }
        void txtSetNameAtImport_CheckedChanged(object sender, System.EventArgs e)
        {
            Data.Settings.SetNameAtImport = txtSetNameAtImport.Checked;
            presentSettings();
        }
	}
}
