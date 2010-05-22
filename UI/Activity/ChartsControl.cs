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
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Data.Fitness;

namespace TrailsPlugin.UI.Activity {
	public partial class ChartsControl : UserControl {

		public event System.EventHandler Collapse;

		public ChartsControl() {
			InitializeComponent();

			btnCollapse.CenterImage = CommonIcons.LowerLeft;
			btnCollapse.Text = "";
			btnCollapse.Left = this.Right - 46;
			btnCollapse.Top = 2;

			speedChart.YAxisReferential = TrailLineChart.LineChartTypes.Speed;
			heartrateChart.YAxisReferential = TrailLineChart.LineChartTypes.HeartRateBPM;
			gradeChart.YAxisReferential = TrailLineChart.LineChartTypes.Grade;
			elevationChart.YAxisReferential = TrailLineChart.LineChartTypes.Elevation;
			cadenceChart.YAxisReferential = TrailLineChart.LineChartTypes.Cadence;
			RefreshRows();
		}

		public void ThemeChanged(ITheme visualTheme) {
			this.ChartBanner.ThemeChanged(visualTheme);
		}

		private void btnCollapse_Click(object sender, EventArgs e) {
			Collapse(sender, e);
		}

		public void RefreshRows() {
			tableLayoutPanel1.RowStyles[0].SizeType = SizeType.AutoSize;
			tableLayoutPanel1.RowStyles[1].SizeType = SizeType.AutoSize;
			tableLayoutPanel1.RowStyles[2].SizeType = SizeType.AutoSize;
			tableLayoutPanel1.RowStyles[3].SizeType = SizeType.AutoSize;
			tableLayoutPanel1.RowStyles[4].SizeType = SizeType.AutoSize;

		}

		public void RefreshCharts(IActivity activity, Data.TrailResult result) {

			speedChart.BeginUpdate();
			speedChart.Activity = activity;
			speedChart.XAxisReferential = PluginMain.Settings.XAxisValue;
			speedChart.TrailResult = result;
			speedChart.EndUpdate();

			heartrateChart.BeginUpdate();
			heartrateChart.Activity = activity;
			heartrateChart.XAxisReferential = PluginMain.Settings.XAxisValue;
			heartrateChart.TrailResult = result;
			heartrateChart.EndUpdate();

			elevationChart.BeginUpdate();
			elevationChart.Activity = activity;
			elevationChart.XAxisReferential = PluginMain.Settings.XAxisValue;
			elevationChart.TrailResult = result;
			elevationChart.EndUpdate();

			gradeChart.BeginUpdate();
			gradeChart.Activity = activity;
			gradeChart.XAxisReferential = PluginMain.Settings.XAxisValue;
			gradeChart.TrailResult = result;
			gradeChart.EndUpdate();

			cadenceChart.BeginUpdate();
			cadenceChart.Activity = activity;
			cadenceChart.XAxisReferential = PluginMain.Settings.XAxisValue;
			cadenceChart.TrailResult = result;
			cadenceChart.EndUpdate();			
			
		}


	}
}
