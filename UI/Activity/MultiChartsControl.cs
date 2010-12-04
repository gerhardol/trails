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
using System.Globalization;
using System.Windows.Forms;

using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Visuals;

#if ST_2_1
using TrailsPlugin.Data;
#else
using ZoneFiveSoftware.Common.Visuals.Fitness;
#endif

namespace TrailsPlugin.UI.Activity {
	public partial class MultiChartsControl : UserControl {

        public event System.EventHandler Expand;
        public event System.EventHandler Collapse;

        private bool m_expanded = false;
        private bool m_multiple = false;
        private bool _showPage;
        private ActivityDetailPageControl m_page;
        private Controller.TrailController m_controller;
        private IList<TrailLineChart> m_lineCharts;

#if !ST_2_1
        private IDailyActivityView m_view = null;
#endif

		public MultiChartsControl() {
            InitializeComponent();
        }
#if ST_2_1
        public void SetControl(ActivityDetailPageControl page, Controller.TrailController controller)
        {
#else
        public void SetControl(ActivityDetailPageControl page, Controller.TrailController controller, IDailyActivityView view)
        {
            m_view = view;
#endif
            m_page = page;
            m_controller = controller;

            InitControls();
            foreach (TrailLineChart t in m_lineCharts)
            {
                t.SetControl(m_page, this);
            }
        }

        void InitControls()
        {
            //this.showToolBarMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Yeild16;
            this.speedPaceToolStripMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.TrackGPS16;
            this.speedToolStripMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.TrackGPS16;
            this.paceToolStripMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.TrackGPS16;
            this.heartRateToolStripMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.TrackHeartRate16;
            this.cadenceToolStripMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.TrackCadence16;
            this.elevationToolStripMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.TrackElevation16;
            this.gradeStripMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.TrackElevation16;
            this.powerToolStripMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.TrackPower16;
            this.distanceToolStripMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.TrackGPS16;
            this.timeToolStripMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Calendar16;

            timeDiffToolStripMenuItem.Visible = false;//TODO: temporary
            distDiffToolStripMenuItem.Visible = false;

            if (m_expanded)
            {
                btnCollapse.Visible = true;
            }
            else
            {
                btnExpand.Visible = true;
            }
            btnCollapse.CenterImage = CommonIcons.LowerLeft;
            btnCollapse.Text = "";
            btnCollapse.Left = this.Right - 46;
            btnCollapse.Top = 2;

            btnExpand.BackgroundImage = CommonIcons.LowerHalf;
            btnExpand.Text = "";
            //For some reason, the Designer moves this button out of the panel
            this.btnExpand.Location = new System.Drawing.Point(353, 1);


            this.Resize += new System.EventHandler(TrailLineChart_Resize);
            //this.showToolBarMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.

            //The panels and charts should probably be created manually instead
            m_lineCharts = new List<TrailLineChart>();
            foreach (Control t in this.ChartPanel.Controls)
            {
                if (t is TrailLineChart)
                {
                    m_lineCharts.Add((TrailLineChart)t);
                }
            }
            //speedChart.YAxisReferential = TrailLineChart.LineChartTypes.Speed;
            //paceChart.YAxisReferential = TrailLineChart.LineChartTypes.Pace;
            //heartrateChart.YAxisReferential = TrailLineChart.LineChartTypes.HeartRateBPM;
            //gradeChart.YAxisReferential = TrailLineChart.LineChartTypes.Grade;
            //elevationChart.YAxisReferential = TrailLineChart.LineChartTypes.Elevation;
            //cadenceChart.YAxisReferential = TrailLineChart.LineChartTypes.Cadence;
            //timeDiff.YAxisReferential = TrailLineChart.LineChartTypes.TimeDiff;
            //distDiff.YAxisReferential = TrailLineChart.LineChartTypes.DistDiff;
        }

        public void ThemeChanged(ITheme visualTheme)
        {
            foreach (Control t in this.ChartPanel.Controls)
            {
                if (t is ActionBanner)
                {
                    ((ActionBanner)t).ThemeChanged(visualTheme);
                }
            }
            foreach (TrailLineChart t in m_lineCharts)
            {
                t.ThemeChanged(visualTheme);
            }
        }
        public void UICultureChanged(CultureInfo culture)
        {
            this.ChartBanner.Text = Properties.Resources.TrailChartsName;
            foreach (TrailLineChart t in m_lineCharts)
            {
                t.UICultureChanged(culture);
            }
            this.speedToolStripMenuItem.Text = TrailLineChart.ChartTypeString(TrailLineChart.LineChartTypes.Speed);
            this.paceToolStripMenuItem.Text = TrailLineChart.ChartTypeString(TrailLineChart.LineChartTypes.Pace);
            this.speedPaceToolStripMenuItem.Text = TrailLineChart.ChartTypeString(TrailLineChart.LineChartTypes.SpeedPace);
            this.elevationToolStripMenuItem.Text = TrailLineChart.ChartTypeString(TrailLineChart.LineChartTypes.Elevation);
            this.cadenceToolStripMenuItem.Text = TrailLineChart.ChartTypeString(TrailLineChart.LineChartTypes.Cadence);
            this.heartRateToolStripMenuItem.Text = TrailLineChart.ChartTypeString(TrailLineChart.LineChartTypes.HeartRateBPM);
            this.gradeStripMenuItem.Text = TrailLineChart.ChartTypeString(TrailLineChart.LineChartTypes.Grade);
            this.powerToolStripMenuItem.Text = TrailLineChart.ChartTypeString(TrailLineChart.LineChartTypes.Power);

            this.timeDiffToolStripMenuItem.Text = TrailLineChart.ChartTypeString(TrailLineChart.LineChartTypes.TimeDiff);
            this.distDiffToolStripMenuItem.Text = TrailLineChart.ChartTypeString(TrailLineChart.LineChartTypes.DistDiff);

            this.timeToolStripMenuItem.Text = TrailLineChart.XAxisValueString(TrailLineChart.XAxisValue.Time);
            this.distanceToolStripMenuItem.Text = TrailLineChart.XAxisValueString(TrailLineChart.XAxisValue.Distance);
            this.showToolBarMenuItem.Text = Properties.Resources.UI_Activity_Menu_ShowToolBar;
        }

        public bool ShowPage
        {
            get { return _showPage; }
            set
            {
                _showPage = value;
                if (_showPage)
                {
                    RefreshChartMenu();
                    RefreshChart();
                }
                this.Visible = _showPage;
            }
        }

        public bool Expanded
        {
            set
            {
                m_expanded = value;
                m_multiple = value;
                if (m_expanded)
                {
                    this.ChartBanner.Style = ZoneFiveSoftware.Common.Visuals.ActionBanner.BannerStyle.Header2;
                    this.ChartBanner.Text = Properties.Resources.TrailChartsName;
                }
                else
                {
                    this.ChartBanner.Style = ZoneFiveSoftware.Common.Visuals.ActionBanner.BannerStyle.Header1;
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////
        private void RefreshRows()
        {
            int noOfGraphs = 0;
            for (int i = 0; i < m_lineCharts.Count; i++)
            {
                if (m_lineCharts[i].ShowPage)
                {
                    noOfGraphs++;
                }
            }
            int height = (ChartPanel.Height - (int)ChartPanel.RowStyles[0].Height);
            if (noOfGraphs > 0) { height = height / noOfGraphs; }
            for (int i = 0; i < m_lineCharts.Count; i++)
            {
                ChartPanel.RowStyles[i+1].SizeType = SizeType.Absolute;
                if (m_lineCharts[i].ShowPage)
                {
                    ChartPanel.RowStyles[i+1].Height = height;
                }
                else
                {
                    ChartPanel.RowStyles[i+1].Height = 0;
                }
            }
            ShowChartToolBar = PluginMain.Settings.ShowChartToolBar;
        }

        public void RefreshChart()
        {
            if (_showPage)
            {
                TrailLineChart.LineChartTypes speedPaceYaxis;
                if (m_controller.ReferenceActivity != null &&
                    m_controller.ReferenceActivity.Category.SpeedUnits.Equals(Speed.Units.Speed))
                {
                    speedPaceYaxis = TrailLineChart.LineChartTypes.Speed;
                }
                else
                {
                    speedPaceYaxis = TrailLineChart.LineChartTypes.Pace;
                }

                foreach (TrailLineChart chart in m_lineCharts)
                {
                    chart.BeginUpdate();
                    chart.XAxisReferential = PluginMain.Settings.XAxisValue;
                    bool visible = false;

                    if (m_multiple &&
                        (PluginMain.Settings.MultiChartType.Contains(TrailLineChart.LineChartTypes.SpeedPace) &&
                            chart.YAxisReferential == speedPaceYaxis ||
                            PluginMain.Settings.MultiChartType.Contains(chart.YAxisReferential)) ||
                       !m_multiple &&
                         (TrailLineChart.LineChartTypes.SpeedPace == PluginMain.Settings.ChartType &&
                            chart.YAxisReferential == speedPaceYaxis ||
                            chart.YAxisReferential == PluginMain.Settings.ChartType))
                    {
                        visible = true;
                    }

                    if (visible)
                    {
                        if (!m_multiple)
                        {
                            this.ChartBanner.Text = TrailLineChart.ChartTypeString(chart.YAxisReferential) + " / " +
                            TrailLineChart.XAxisValueString(chart.XAxisReferential);
                        }
                        IList<Data.TrailResult> list = this.m_page.SelectedItems;
                        chart.ReferenceTrailResult = m_controller.ReferenceTrailResult;
                        chart.TrailResults = list;
                    }
                    else
                    {
                        chart.ShowPage = visible;
                    }
                    chart.EndUpdate();
                }
                RefreshRows();
            }
		}

        private bool setLineChartChecked(TrailLineChart.LineChartTypes t)
        {
            if (m_multiple)
            {
                return PluginMain.Settings.MultiChartType.Contains(t);
            }
            else
            {
                return PluginMain.Settings.ChartType == t;
            }
        }
        void RefreshChartMenu()
        {
            //TODO: set enabled if track exists
            speedToolStripMenuItem.Checked = setLineChartChecked(TrailLineChart.LineChartTypes.Speed);
            paceToolStripMenuItem.Checked = setLineChartChecked(TrailLineChart.LineChartTypes.Pace);
            speedPaceToolStripMenuItem.Checked = setLineChartChecked(TrailLineChart.LineChartTypes.SpeedPace);
            elevationToolStripMenuItem.Checked = setLineChartChecked(TrailLineChart.LineChartTypes.Elevation);
            cadenceToolStripMenuItem.Checked = setLineChartChecked(TrailLineChart.LineChartTypes.Cadence);
            heartRateToolStripMenuItem.Checked = setLineChartChecked(TrailLineChart.LineChartTypes.HeartRateBPM);
            gradeStripMenuItem.Checked = setLineChartChecked(TrailLineChart.LineChartTypes.Grade);
            powerToolStripMenuItem.Checked = setLineChartChecked(TrailLineChart.LineChartTypes.Power);

            timeDiffToolStripMenuItem.Checked = setLineChartChecked(TrailLineChart.LineChartTypes.TimeDiff);
            distDiffToolStripMenuItem.Checked = setLineChartChecked(TrailLineChart.LineChartTypes.DistDiff);

            timeToolStripMenuItem.Checked = PluginMain.Settings.XAxisValue == TrailLineChart.XAxisValue.Time;
            distanceToolStripMenuItem.Checked = PluginMain.Settings.XAxisValue == TrailLineChart.XAxisValue.Distance;
            this.showToolBarMenuItem.Checked = PluginMain.Settings.ShowChartToolBar;
        }

        void RefreshChart(TrailLineChart.LineChartTypes t)
        {
            if (m_multiple)
            {
                PluginMain.Settings.ToggleMultiChartType = t;
            }
            else
            {
                PluginMain.Settings.ChartType = t;
            }
            RefreshChartMenu();
            RefreshChart();
        }

        void RefreshChart(TrailLineChart.XAxisValue t)
        {
            PluginMain.Settings.XAxisValue = t;
            RefreshChartMenu();
            RefreshChart();
        }

        public void SetSelected(IList<IItemTrackSelectionInfo> asel)
        {
            foreach (TrailLineChart chart in m_lineCharts)
            {
                chart.SetSelected(asel);
            }
        }
        public void SetSelectedRange(IList<float[]> regions)
        {
            foreach (TrailLineChart chart in m_lineCharts)
            {
                chart.SetSelectedRange(regions);
            }
        }

        public bool ShowChartToolBar
        {
            set
            {
                foreach (TrailLineChart chart in m_lineCharts)
                {
                    chart.ShowChartToolBar = value;
                }
                RefreshChartMenu();
            }
        }

        /***************************************/
        private void speedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(TrailLineChart.LineChartTypes.Speed);
        }
        private void paceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(TrailLineChart.LineChartTypes.Pace);
        }
        private void speedPaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(TrailLineChart.LineChartTypes.SpeedPace);
        }

        private void elevationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(TrailLineChart.LineChartTypes.Elevation);
        }

        private void heartRateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(TrailLineChart.LineChartTypes.HeartRateBPM);
        }

        private void cadenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(TrailLineChart.LineChartTypes.Cadence);
        }

        private void gradeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(TrailLineChart.LineChartTypes.Grade);
        }
        private void powerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(TrailLineChart.LineChartTypes.Power);
        }

        private void timeDiffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(TrailLineChart.LineChartTypes.TimeDiff);
        }
        private void distDiffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(TrailLineChart.LineChartTypes.DistDiff);
        }

        private void distanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(TrailLineChart.XAxisValue.Distance);
        }

        private void timeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(TrailLineChart.XAxisValue.Time);
        }

        private void showToolBarMenuItem_Click(object sender, EventArgs e)
        {
            PluginMain.Settings.ShowChartToolBar = !PluginMain.Settings.ShowChartToolBar;
            ShowChartToolBar = PluginMain.Settings.ShowChartToolBar;
        }

        /***************************************/
        private void btnCollapse_Click(object sender, EventArgs e)
        {
            Collapse(sender, e);
        }
        private void btnExpand_Click(object sender, EventArgs e)
        {
            Expand(sender, e);
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
    }
}
