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
#if !ST_2_1
using ZoneFiveSoftware.Common.Visuals.Fitness;
#endif
using TrailsPlugin.Data;

namespace TrailsPlugin.UI.Activity {
	public partial class MultiChartsControl : UserControl {

        public event System.EventHandler Expand;
        public event System.EventHandler Collapse;

        private bool m_expanded = false;
        private bool m_multipleGraphs = false;
        private bool m_multipleCharts = true;
        private bool m_showPage;
        private ActivityDetailPageControl m_page;
        private Controller.TrailController m_controller;
        private IList<TrailLineChart> m_lineCharts;
        private TrailLineChart m_multiChart;

#if !ST_2_1
        private IDailyActivityView m_view = null;
#endif

		public MultiChartsControl() {
            InitializeComponent();
            InitControls();
        }
#if ST_2_1
        public void SetControl(ActivityDetailPageControl page, Controller.TrailController controller, Object view)
        {
#else
        public void SetControl(ActivityDetailPageControl page, Controller.TrailController controller, IDailyActivityView view)
        {
            m_view = view;
#endif
            m_page = page;
            m_controller = controller;
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

            this.diffDistTimeToolStripMenuItem.Image = Properties.Resources.delta;
            //this.diffTimeToolStripMenuItem.Image = Properties.Resources.delta;
            //this.diffDistToolStripMenuItem.Image = Properties.Resources.delta;
            this.diffTimeToolStripMenuItem.Visible = false;
            this.diffDistToolStripMenuItem.Visible = false;

            this.distanceToolStripMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.TrackGPS16;
            this.timeToolStripMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Calendar16;
            //this.showToolBarMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.

            this.adjustResyncDiffAtTrailPointsToolStripMenuItem.Visible = false;

            //The panels and charts should probably be created manually instead
            m_lineCharts = new List<TrailLineChart>();
            foreach (Control t in this.ChartPanel.Controls)
            {
                if (t is TrailLineChart)
                {
                    TrailLineChart lChart = (TrailLineChart)t;
                    m_lineCharts.Add(lChart);
                    if(lChart.MultipleCharts)
                    {
                        m_multiChart=lChart;
                    }
                }
            }
            //speedChart.YAxisReferential = LineChartTypes.Speed;
            //paceChart.YAxisReferential = LineChartTypes.Pace;
            //heartrateChart.YAxisReferential = LineChartTypes.HeartRateBPM;
            //gradeChart.YAxisReferential = LineChartTypes.Grade;
            //elevationChart.YAxisReferential = LineChartTypes.Elevation;
            //cadenceChart.YAxisReferential = LineChartTypes.Cadence;
            //diffTime.YAxisReferential = LineChartTypes.diffTime;
            //diffDist.YAxisReferential = LineChartTypes.diffDist;
            this.Expanded = m_expanded;
            this.Resize += new System.EventHandler(TrailLineChart_Resize);
        }

        public void ThemeChanged(ITheme visualTheme)
        {
            this.ChartBanner.ThemeChanged(visualTheme);
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
            this.speedToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.Speed);
            this.paceToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.Pace);
            this.speedPaceToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.SpeedPace);
            this.elevationToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.Elevation);
            this.cadenceToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.Cadence);
            this.heartRateToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.HeartRateBPM);
            this.gradeStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.Grade);
            this.powerToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.Power);

            this.diffTimeToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.DiffTime);
            this.diffDistToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.DiffDist);
            //Set when updating chart
            this.diffDistTimeToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.DiffDistTime);
            this.resyncDiffAtTrailPointsToolStripMenuItem.Text = Properties.Resources.UI_Chart_resyncDiffAtTrailPoints;
            this.adjustResyncDiffAtTrailPointsToolStripMenuItem.Text = " "+Properties.Resources.UI_Chart_adjustResyncDiffAtTrailPoints;
            this.syncChartAtTrailPointsToolStripMenuItem.Text = Properties.Resources.UI_Chart_syncChartAtTrailPoints;

            this.timeToolStripMenuItem.Text = LineChartUtil.XAxisValueString(XAxisValue.Time);
            this.distanceToolStripMenuItem.Text = LineChartUtil.XAxisValueString(XAxisValue.Distance);
            this.showToolBarMenuItem.Text = Properties.Resources.UI_Activity_Menu_ShowToolBar;
        }

        public bool ShowPage
        {
            get { return m_showPage; }
            set
            {
                m_showPage = value;
                if (m_showPage)
                {
                    RefreshChart();
                }
            }
        }

        public bool Expanded
        {
            set
            {
                 btnExpand.Text = "";
                //For some reason, the Designer moves this button out of the panel
                btnExpand.Left = this.ChartBanner.Right - 46;

                m_expanded = value;
                m_multipleGraphs = value;
                m_multipleCharts = !value;
                if (m_expanded)
                {
                    this.ChartBanner.Style = ZoneFiveSoftware.Common.Visuals.ActionBanner.BannerStyle.Header1;
                    this.ChartBanner.Text = Properties.Resources.TrailChartsName;
                    btnExpand.BackgroundImage = CommonIcons.LowerLeft;
                }
                else
                {
                    this.ChartBanner.Style = ZoneFiveSoftware.Common.Visuals.ActionBanner.BannerStyle.Header2;
                    btnExpand.BackgroundImage = CommonIcons.LowerHalf;
                }
                ShowPage = m_showPage;
                //This could be changed to zoom to data only at changes
                foreach (TrailLineChart t in m_lineCharts)
                {
                    if (t.ShowPage)
                    {
                        t.ZoomToData();
                    }
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
            //The first row is the banner, the following is the charts
            for (int i = 0; i < m_lineCharts.Count; i++)
            {
                ChartPanel.RowStyles[i+1].SizeType = SizeType.Absolute;
                if (m_lineCharts[i].ShowPage && height > 0)
                {
                    ChartPanel.RowStyles[i+1].Height = height;
                }
                else
                {
                    ChartPanel.RowStyles[i+1].Height = 0;
                }
            }
            ShowChartToolBar = Data.Settings.ShowChartToolBar;
        }

        public void RefreshChart()
        {
            if (m_showPage)
            {
                LineChartTypes speedPaceYaxis;
                if (m_controller.ReferenceActivity != null &&
                    m_controller.ReferenceActivity.Category.SpeedUnits.Equals(Speed.Units.Speed))
                {
                    speedPaceYaxis = LineChartTypes.Speed;
                }
                else
                {
                    speedPaceYaxis = LineChartTypes.Pace;
                }
                LineChartTypes diffYaxis = LineChartTypes.DiffDist;
                if (Data.Settings.XAxisValue == XAxisValue.Distance)
                {
                    diffYaxis = LineChartTypes.DiffTime;
                }

                bool isData = m_controller.CurrentActivityTrailDisplayed != null;

                m_multiChart.YAxisReferentials=new List<LineChartTypes>();
                multiChart.ShowPage = false;
                //TODO: Temporary handling. Cleanup and decide multiple graphs and charts
                TrailLineChart updateChart = m_multiChart;
                if (m_multipleCharts)
                {
                    foreach (TrailLineChart chart in m_lineCharts)
                    {
                        chart.ShowPage = false;
                    }
                    m_multiChart.BeginUpdate();
                    //m_multiChart.ShowPage = false;
                    m_multiChart.XAxisReferential = Data.Settings.XAxisValue;
                    IList<Data.TrailResult> list = this.m_page.SelectedItems;
                    if (isData)
                    {
                        m_multiChart.ReferenceTrailResult = m_controller.ReferenceTrailResult;
                    }
                    m_multiChart.TrailResults = list;
                    foreach (LineChartTypes t in Data.Settings.MultiChartType)
                    {
                        LineChartTypes t2 = t;
                        if (t2 == LineChartTypes.SpeedPace)
                        {
                            t2 = speedPaceYaxis;
                        }
                        if (t2 == LineChartTypes.DiffDistTime)
                        {
                            t2 = diffYaxis;
                        }
                        if (!m_multiChart.YAxisReferentials.Contains(t2) &&
                            m_multiChart.HasValues(t2))
                        {
                            m_multiChart.YAxisReferentials.Add(t2);
                        }
                    }
                    m_multiChart.ShowPage = true;

                    if (!m_multiChart.AnyData())
                    {
                        m_multiChart.YAxisReferential = speedPaceYaxis;
                        m_multiChart.ShowPage = true;
                    }
                    this.ChartBanner.Text = LineChartUtil.ChartTypeString(m_multiChart.YAxisReferential) + " / " +
                    LineChartUtil.XAxisValueString(m_multiChart.XAxisReferential);
                    m_multiChart.EndUpdate();
                }
                else
                {
                    foreach (TrailLineChart chart in m_lineCharts)
                    {
                        bool visible = false;

                        if (m_multipleGraphs &&
                            (Data.Settings.MultiGraphType.Contains(chart.YAxisReferential) ||
                            chart.YAxisReferential == speedPaceYaxis &&
                            Data.Settings.MultiGraphType.Contains(LineChartTypes.SpeedPace) ||
                            chart.YAxisReferential == diffYaxis &&
                            Data.Settings.MultiGraphType.Contains(LineChartTypes.DiffDistTime)) ||

                            !m_multipleGraphs &&
                            (chart.YAxisReferential == Data.Settings.ChartType ||
                            chart.YAxisReferential == speedPaceYaxis &&
                                LineChartTypes.SpeedPace == Data.Settings.ChartType ||
                            chart.YAxisReferential == diffYaxis &&
                                LineChartTypes.DiffDistTime == Data.Settings.ChartType))
                        {
                            visible = true;
                        }

                        updateChart = chart;
                        updateChart.BeginUpdate();
                        chart.ShowPage = false;
                        if (visible && isData)
                        {
                            updateChart.XAxisReferential = Data.Settings.XAxisValue;
                            IList<Data.TrailResult> list = this.m_page.SelectedItems;
                            updateChart.ReferenceTrailResult = m_controller.ReferenceTrailResult;
                            updateChart.TrailResults = list;
                            if (!m_multipleGraphs && updateChart.YAxisReferentials.Count == 1)
                            {
                                this.ChartBanner.Text = LineChartUtil.ChartTypeString(chart.YAxisReferential) + " / " +
                                LineChartUtil.XAxisValueString(chart.XAxisReferential);
                            }
                            if (updateChart.HasValues(chart.YAxisReferential))
                            {
                                updateChart.ShowPage = visible;
                            }
                            else
                            {
                                if (visible && !updateChart.HasValues(chart.YAxisReferential))
                                {
                                    chart.ShowPage = false;
                                    //Replace empty chart
                                    if (!m_multipleGraphs && chart.YAxisReferential != speedPaceYaxis)
                                    {
                                        foreach (TrailLineChart replaceChart in m_lineCharts)
                                        {
                                            if (replaceChart.YAxisReferential == speedPaceYaxis)
                                            {
                                                replaceChart.BeginUpdate();
                                                replaceChart.ShowPage = false;
                                                replaceChart.XAxisReferential = Data.Settings.XAxisValue;
                                                IList<Data.TrailResult> list2 = this.m_page.SelectedItems;
                                                replaceChart.ReferenceTrailResult = m_controller.ReferenceTrailResult;
                                                replaceChart.TrailResults = list2;
                                                if (!m_multipleGraphs)
                                                {
                                                    this.ChartBanner.Text = LineChartUtil.ChartTypeString(replaceChart.YAxisReferential) + " / " +
                                                    LineChartUtil.XAxisValueString(replaceChart.XAxisReferential);
                                                }
                                                replaceChart.ShowPage = visible;
                                                replaceChart.EndUpdate();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        updateChart.EndUpdate();
                    }
                }
                RefreshRows();
                RefreshChartMenu();
            }
        }

        private bool setLineChartChecked(LineChartTypes t)
        {
            if (m_multipleCharts)
            {
                return Data.Settings.MultiChartType.Contains(t);
            }
            else if (m_multipleGraphs)
            {
                return Data.Settings.MultiGraphType.Contains(t);
            }
            else
            {
                return Data.Settings.ChartType == t;
            }
        }
        void RefreshChartMenu()
        {
            //TODO: disable if track exists (or ref for diff). 
            speedToolStripMenuItem.Checked = setLineChartChecked(LineChartTypes.Speed);
            paceToolStripMenuItem.Checked = setLineChartChecked(LineChartTypes.Pace);
            speedPaceToolStripMenuItem.Checked = setLineChartChecked(LineChartTypes.SpeedPace);
            elevationToolStripMenuItem.Checked = setLineChartChecked(LineChartTypes.Elevation);
            cadenceToolStripMenuItem.Checked = setLineChartChecked(LineChartTypes.Cadence);
            heartRateToolStripMenuItem.Checked = setLineChartChecked(LineChartTypes.HeartRateBPM);
            gradeStripMenuItem.Checked = setLineChartChecked(LineChartTypes.Grade);
            powerToolStripMenuItem.Checked = setLineChartChecked(LineChartTypes.Power);

            diffTimeToolStripMenuItem.Checked = setLineChartChecked(LineChartTypes.DiffTime);
            diffDistToolStripMenuItem.Checked = setLineChartChecked(LineChartTypes.DiffDist);
            diffDistTimeToolStripMenuItem.Checked = setLineChartChecked(LineChartTypes.DiffDistTime);
            if (Data.Settings.XAxisValue == XAxisValue.Distance)
            {
                this.diffDistTimeToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.DiffTime);
            }
            else
            {
                this.diffDistTimeToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.DiffDist);
            }
            resyncDiffAtTrailPointsToolStripMenuItem.Checked = Data.Settings.ResyncDiffAtTrailPoints;
            adjustResyncDiffAtTrailPointsToolStripMenuItem.Enabled = resyncDiffAtTrailPointsToolStripMenuItem.Checked;
            adjustResyncDiffAtTrailPointsToolStripMenuItem.Checked = Data.Settings.AdjustResyncDiffAtTrailPoints;
            syncChartAtTrailPointsToolStripMenuItem.Checked = Data.Settings.SyncChartAtTrailPoints;

            timeToolStripMenuItem.Checked = Data.Settings.XAxisValue == XAxisValue.Time;
            distanceToolStripMenuItem.Checked = Data.Settings.XAxisValue == XAxisValue.Distance;
            this.showToolBarMenuItem.Checked = Data.Settings.ShowChartToolBar;
        }

        void RefreshChart(LineChartTypes t)
        {
            if (m_multipleCharts)
            {
                Data.Settings.ToggleMultiChartType = t;
            }
            if (m_multipleGraphs)
            {
                Data.Settings.ToggleMultiGraphType = t;
            }
            else
            {
                Data.Settings.ChartType = t;
            }
            RefreshChart();
        }

        void RefreshChart(XAxisValue t)
        {
            Data.Settings.XAxisValue = t;
            RefreshChart();
        }

        public void SetSelectedRegions(IList<TrailResultMarked> atr)
        {
            foreach (TrailLineChart chart in m_lineCharts)
            {
                chart.SetSelectedRegions(atr);
            }
        }
        //Used as callback when selected from chart - should be only for single activity
        public void SetSelectedRange(IList<IItemTrackSelectionInfo> asel)
        {
            foreach (TrailLineChart chart in m_lineCharts)
            {
                chart.SetSelectedRange(asel);
            }
        }
        public void SetSelectedResultRange(IList<float[]> regions)
        {
            foreach (TrailLineChart chart in m_lineCharts)
            {
                chart.SetSelectedResultRange(regions);
            }
        }
        //Update a specific dataseries, called from one TrailLineChart to all other
        public void SetSeriesSelectedResultRange(int i, IList<float[]> regions)
        {
            foreach (TrailLineChart chart in m_lineCharts)
            {
                chart.SetSeriesSelectedResultRange(i, true, regions);
            }
        }
        public void EnsureVisible(IList<TrailResult> atr)
        {
            foreach (TrailLineChart chart in m_lineCharts)
            {
                chart.EnsureVisible(atr);
            }
        }

        public bool ShowChartToolBar
        {
            set
            {
                if (ShowPage)
                {
                    foreach (TrailLineChart chart in m_lineCharts)
                    {
                        chart.ShowChartToolBar = value;
                    }
                    RefreshChartMenu();
                }
            }
        }

        /***************************************/
        private void speedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.Speed);
        }
        private void paceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.Pace);
        }
        private void speedPaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.SpeedPace);
        }

        private void elevationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.Elevation);
        }

        private void heartRateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.HeartRateBPM);
        }

        private void cadenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.Cadence);
        }

        private void gradeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.Grade);
        }
        private void powerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.Power);
        }

        private void diffTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.DiffTime);
        }
        private void diffDistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.DiffDist);
        }
        private void diffDistTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.DiffDistTime);
        }

        private void distanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(XAxisValue.Distance);
        }

        private void timeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(XAxisValue.Time);
        }

        private void resyncDiffAtTrailPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Data.Settings.ResyncDiffAtTrailPoints = !Data.Settings.ResyncDiffAtTrailPoints;
            RefreshChartMenu();
            m_page.RefreshData();
        }
        private void adjustResyncDiffAtTrailPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Data.Settings.AdjustResyncDiffAtTrailPoints = !Data.Settings.AdjustResyncDiffAtTrailPoints;
            RefreshChartMenu();
            m_page.RefreshData();
        }
        private void syncChartAtTrailPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Data.Settings.SyncChartAtTrailPoints = !Data.Settings.SyncChartAtTrailPoints;
            RefreshChartMenu();
            m_page.RefreshChart();
        }
        private void showToolBarMenuItem_Click(object sender, EventArgs e)
        {
            Data.Settings.ShowChartToolBar = !Data.Settings.ShowChartToolBar;
            ShowChartToolBar = Data.Settings.ShowChartToolBar;
        }

        /***************************************/
        private void btnExpand_Click(object sender, EventArgs e)
        {
            if (m_expanded)
            {
                Collapse(sender, e);
            }
            else
            {
                Expand(sender, e);
            }
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
