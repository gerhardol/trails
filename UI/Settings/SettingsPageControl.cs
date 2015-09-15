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
using System.Globalization;
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

            //Present up as negative
            this.boxMervynDaviesUp.Text = (-Data.Settings.MervynDaviesUp).ToString("P1");
            this.boxMervynDaviesDown.Text = Data.Settings.MervynDaviesDown.ToString("P1");
            //Present per distance unit, saved s/m
            this.boxJackDanielsUp.Text = UnitUtil.Time.ToString((-Data.Settings.JackDanielsUp) * UnitUtil.Distance.ConvertTo(1, UnitUtil.Distance.Unit),"ss") + 
                " s/" + UnitUtil.Distance.LabelAbbr;
            this.boxJackDanielsDown.Text = UnitUtil.Time.ToString(Data.Settings.JackDanielsDown * UnitUtil.Distance.ConvertTo(1, UnitUtil.Distance.Unit),"ss") + 
                " s/" + UnitUtil.Distance.LabelAbbr;
        }

        public void ThemeChanged(ITheme visualTheme)
        {
            this.PluginInfoBanner.ThemeChanged(visualTheme);
            this.PluginInfoPanel.ThemeChanged(visualTheme);
            this.txtDefaultRadius.ThemeChanged(visualTheme);
            this.boxStoppedCategory.ThemeChanged(visualTheme);
            this.boxBarometricDevices.ThemeChanged(visualTheme);
            this.boxPredictDistance.ThemeChanged(visualTheme);

            this.boxMervynDaviesUp.ThemeChanged(visualTheme);
            this.boxMervynDaviesDown.ThemeChanged(visualTheme);
            this.boxJackDanielsUp.ThemeChanged(visualTheme);
            this.boxJackDanielsDown.ThemeChanged(visualTheme);
        }

        public void UICultureChanged(System.Globalization.CultureInfo culture)
        {
            this.lblDefaultRadius.Text = Properties.Resources.UI_Settings_DefaultRadius + ":";
            //Not working
            //toolTip.SetToolTip(txtDefaultRadius, Properties.Resources.UI_Settings_DefaultRadius_ToolTip);

            this.lblSetNameAtImport.Text = Properties.Resources.SetNameAtImport;
            this.lblStoppedCategory.Text = Properties.Resources.UI_Settings_StoppedCategoryOverride + ":";
            this.lblBarometricDevices.Text = Properties.Resources.UI_Settings_BarometricDevices + ":";
            this.lblAdjustElevationAtImport.Text = Properties.Resources.UI_Settings_AdjustElevationAtImport + ":";

            this.gradeAdjustedPaceGroup.Text = Properties.Resources.UI_Settings_GradeAdjustedPace;
            this.lblMervynDaviesUp.Text = "Mervyn Davies " + CommonResources.Text.LabelAscending;
            this.lblMervynDaviesDown.Text = "Mervyn Davies " + CommonResources.Text.LabelDescending;
            this.lblJackDanielsUp.Text = "Jack Daniels " + CommonResources.Text.LabelAscending;
            this.lblJackDanielsDown.Text = "Jack Daniels " + CommonResources.Text.LabelDescending;

            this.presentSettings();

            this.lblUniqueRoutes.Text = Integration.UniqueRoutes.CompabilityText;
            this.lblHighScore.Text = Integration.HighScore.CompabilityText;
            this.lblPerformancePredictor.Text = Integration.PerformancePredictor.CompabilityText;
            this.lblPredictDistance.Text = Properties.Resources.UI_Settings_PredictTimeForDistance + ":";

            //Some untranslated strings....
            this.lblLicense.Text = "\r\nTrails Plugin is distributed under the GNU Lesser General Public Licence.\r\nThe Li" +
                "cense is included in the plugin installation directory and at:\r\nhttp://www.gnu.o" +
                "rg/licenses/lgpl.html.";
            this.lblCopyright.Text = "Copyright Brendan Doherty 2009, Gerhard Olsson 2010-2015";
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

        private void boxMervynDavies_LostFocus(object sender, EventArgs e)
        {
            float result;
            string val = ((ZoneFiveSoftware.Common.Visuals.TextBox)sender).Text;
            
            //Simple parsing, no check misaligned %
            val = val.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
            bool p = Data.Settings.tryParseFloat(val, NumberStyles.Any, out result);
            if (p && !float.IsNaN(result) /*&& Math.Abs(result) < 100*/)
            {
                if (sender == this.boxMervynDaviesUp)
                {
                    //Present as - only
                    Data.Settings.MervynDaviesUp = -result/100;
                }
                else
                {
                    Data.Settings.MervynDaviesDown = result/100;
                }
            }
            else
            {
                MessageBox.Show("Incorrect percent format"); //TODO: Translate
            }
            presentSettings();
        }

        private void boxJackDaniels_LostFocus(object sender, EventArgs e)
        {
            string val = ((ZoneFiveSoftware.Common.Visuals.TextBox)sender).Text;
            //Simple parsing to skip unit etc
            int i = 1 + val.LastIndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' });
            if (i < val.Length)
            {
                val = val.Remove(i);
            }
            float result = float.NaN;
            try
            {
                result = (float)UnitUtil.Time.Parse(val);
            }
            catch { }
            if (!float.IsNaN(result))
            {
                if (sender == this.boxJackDanielsUp)
                {
                    Data.Settings.JackDanielsUp = (float)(-result / UnitUtil.Distance.ConvertTo(1, UnitUtil.Distance.Unit));
                }
                else
                {
                    Data.Settings.JackDanielsDown = (float)(result / UnitUtil.Distance.ConvertTo(1, UnitUtil.Distance.Unit));
                }
            }
            else
            {
                MessageBox.Show("Incorrect time format"); //TODO: Translate
            }
            presentSettings();
        }
    }
}
