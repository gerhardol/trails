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

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Data.Fitness;

namespace TrailsPlugin.UI.Activity {
	public partial class ChartsControl : UserControl {

		public event System.EventHandler Collapse;
        private bool m_showChartToolBar = true;

		public ChartsControl() {
            InitializeComponent();
            InitControls();
            RefreshPage();
        }
        void InitControls()
        {
            this.Resize += new System.EventHandler(TrailLineChart_Resize);
            //this.showToolBarMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.
        }
        public void RefreshPage()
        {
            btnCollapse.CenterImage = CommonIcons.LowerLeft;
			btnCollapse.Text = "";
			btnCollapse.Left = this.Right - 46;
			btnCollapse.Top = 2;

			speedChart.YAxisReferential = TrailLineChart.LineChartTypes.Speed;
			heartrateChart.YAxisReferential = TrailLineChart.LineChartTypes.HeartRateBPM;
			gradeChart.YAxisReferential = TrailLineChart.LineChartTypes.Grade;
			elevationChart.YAxisReferential = TrailLineChart.LineChartTypes.Elevation;
			cadenceChart.YAxisReferential = TrailLineChart.LineChartTypes.Cadence;
		}
        public void ThemeChanged(ITheme visualTheme)
        {
            foreach (Control t in this.tableLayoutPanel1.Controls)
            {
                if (t is ActionBanner)
                {
                    ((ActionBanner)t).ThemeChanged(visualTheme);
                }
                else if (t is TrailLineChart)
                {
                    ((TrailLineChart)t).ThemeChanged(visualTheme);
                }
            }
        }
        public void UICultureChanged(CultureInfo culture)
        {
            this.ChartBanner.Text = Properties.Resources.TrailChartsName;
            foreach (Control t in this.tableLayoutPanel1.Controls)
            {
                if (t is TrailLineChart)
                {
                    ((TrailLineChart)t).UICultureChanged(culture);
                }
            }
            RefreshChartMenu();
        }

        public void RefreshRows()
        {
            int noOfGraphs = 0;
            for (int i = 1; i < tableLayoutPanel1.RowCount; i++)
            {
                if (true == tableLayoutPanel1.Controls[i].Visible)
                {
                    noOfGraphs++;
                }
            }
            int height = (tableLayoutPanel1.Height-(int)tableLayoutPanel1.RowStyles[0].Height);
            if (noOfGraphs > 0) { height = height / noOfGraphs; }
            for (int i = 1; i < tableLayoutPanel1.RowCount; i++)
            {
                tableLayoutPanel1.RowStyles[i].SizeType = SizeType.Absolute;
                if (true == tableLayoutPanel1.Controls[i].Visible)
                {
                    tableLayoutPanel1.RowStyles[i].Height = height;
                }
                else
                {
                    tableLayoutPanel1.RowStyles[i].Height = 0;
                }
            }
        }

        public void RefreshCharts(IActivity activity, Data.TrailResult result)
        {
            foreach (Control t in this.tableLayoutPanel1.Controls)
            {
                if (t is TrailLineChart)
                {
                    TrailLineChart chart = ((TrailLineChart)t);
                    chart.BeginUpdate();
                    chart.Activity = activity;
                    chart.XAxisReferential = PluginMain.Settings.XAxisValue;
                    chart.TrailResult = result;
                    chart.EndUpdate();
                }
            }
		}

        void RefreshChartMenu()
        {
            this.showToolBarMenuItem.Text = Properties.Resources.UI_Activity_Menu_ShowToolBar;
            this.showToolBarMenuItem.Checked = m_showChartToolBar;
        }

        public bool ShowChartToolBar
        {
            set
            {
                m_showChartToolBar = value;
                foreach (Control t in this.tableLayoutPanel1.Controls)
                {
                    if (t is TrailLineChart)
                    {
                        ((TrailLineChart)t).ShowChartToolBar = value;
                    }
                }
                RefreshChartMenu();
            }
        }

        /***************************************/
        private void btnCollapse_Click(object sender, EventArgs e)
        {
            Collapse(sender, e);
        }
        void TrailLineChart_Resize(object sender, System.EventArgs e)
        {
            this.RefreshRows();
        }
        private void ChartBanner_MenuClicked(object sender, EventArgs e)
        {
            ChartBanner.ContextMenuStrip.Width = 100;
            ChartBanner.ContextMenuStrip.Show(ChartBanner.Parent.PointToScreen(new System.Drawing.Point(ChartBanner.Right - ChartBanner.ContextMenuStrip.Width - 2,
                ChartBanner.Bottom + 1)));
        }
        private void showToolBarMenuItem_Click(object sender, EventArgs e)
        {
            ((ActivityDetailPageControl)this.Parent.Parent.Parent).ShowChartToolBar = !m_showChartToolBar;
        }
    }
}
