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
using GpsRunningPlugin.Util;

namespace TrailsPlugin.UI.Settings
{
    public partial class SettingsPageControl : UserControl
    {
        public SettingsPageControl()
        {
            this.InitializeComponent();
            this.presentSettings();
            if (!TrailsPlugin.Integration.PerformancePredictor.PerformancePredictorIntegrationEnabled)
            {
                this.lblPredictDistance.Enabled = false;
                this.boxPredictDistance.Enabled = false;
            }
        }

        private void presentSettings()
        {
            this.txtDefaultRadius.Text = UnitUtil.Elevation.ToString(Data.Settings.DefaultRadius, "u");
            this.txtSetNameAtImport.Checked = Data.Settings.SetNameAtImport;
            this.boxStoppedCategory.Text = Data.Settings.GetExcludeStoppedCategory;
            this.boxBarometricDevices.Text = Data.Settings.GetBarometricDevices;
            this.txtAdjustElevationAtImport.Checked = Data.Settings.SetAdjustElevationAtImport;
            this.boxPredictDistance.Text = UnitUtil.Distance.ToString(Data.Settings.PredictDistance, "u");
        }
        public void ThemeChanged(ITheme visualTheme)
        {
            this.PluginInfoBanner.ThemeChanged(visualTheme);
            this.PluginInfoPanel.ThemeChanged(visualTheme);
            this.txtDefaultRadius.ThemeChanged(visualTheme);
            this.boxStoppedCategory.ThemeChanged(visualTheme);
            this.boxBarometricDevices.ThemeChanged(visualTheme);
            this.boxPredictDistance.ThemeChanged(visualTheme);
        }
        public void UICultureChanged(System.Globalization.CultureInfo culture)
        {
            this.lblDefaultRadius.Text = Properties.Resources.UI_Settings_DefaultRadius + ":";
            //Not working
            //toolTip.SetToolTip(txtDefaultRadius, Properties.Resources.UI_Settings_DefaultRadius_ToolTip);

            this.lblSetNameAtImport.Text = Properties.Resources.SetNameAtImport;
            this.lblStoppedCategory.Text = "Stopped Category Override" + ":"; //TODO: Translate 
            this.lblBarometricDevices.Text = "Barometric Devices" + ":"; //TODO: Translate 
            this.lblAdjustElevationAtImport.Text = "Adjust Elevation at Import"; //TODO: Translate
            this.presentSettings();

            this.lblUniqueRoutes.Text = Integration.UniqueRoutes.CompabilityText;
            this.lblHighScore.Text = Integration.HighScore.CompabilityText;
            this.lblPerformancePredictor.Text = Integration.PerformancePredictor.CompabilityText;
            this.lblPredictDistance.Text = "Predict Time for Distance:";  //TODO: Translate

            //Some untranslated strings....
            this.lblLicense.Text = "\r\nTrails Plugin is distributed under the GNU Lesser General Public Licence.\r\nThe Li" +
                "cense is included in the plugin installation directory and at:\r\nhttp://www.gnu.o" +
                "rg/licenses/lgpl.html.";
            this.lblCopyright.Text = "Copyright Brendan Doherty 2009, Gerhard Olsson 2010-2013";
            this.PluginInfoBanner.Text = "Plugin Information";
        }

        private void txtDefaultRadius_LostFocus(object sender, EventArgs e) {
            float result;
            result = (float)UnitUtil.Elevation.Parse(txtDefaultRadius.Text);
            if (!float.IsNaN(result) && result > 0)
            {
                Data.Settings.DefaultRadius = result;
            }
            else
            {
                MessageBox.Show(Properties.Resources.UI_Activity_EditTrail_RadiusNumeric);
            }
            this.presentSettings();
        }

        void txtSetNameAtImport_CheckedChanged(object sender, System.EventArgs e)
        {
            Data.Settings.SetNameAtImport = txtSetNameAtImport.Checked;
            presentSettings();
        }

        void boxStoppedCategory_LostFocus(object sender, System.EventArgs e)
        {
            Data.Settings.SetExcludeStoppedCategory(this.boxStoppedCategory.Text);
        }

        void boxBarometricDevices_LostFocus(object sender, System.EventArgs e)
        {
            Data.Settings.SetBarometricDevices(this.boxBarometricDevices.Text);
        }

        void txtAdjustElevationAtImport_CheckedChanged(object sender, System.EventArgs e)
        {
            Data.Settings.SetAdjustElevationAtImport = txtAdjustElevationAtImport.Checked;
            presentSettings();
        }
        
        private void boxPredictDistance_LostFocus(object sender, EventArgs e)
        {
            float result;
            result = (float)UnitUtil.Distance.Parse(this.boxPredictDistance.Text);
            if (!float.IsNaN(result) && result > 0)
            {
                Data.Settings.PredictDistance = result;
            }
            else
            {
                MessageBox.Show("Incorrect distance format"); //TODO: Translate
            }
            presentSettings();
        }
    }
}
