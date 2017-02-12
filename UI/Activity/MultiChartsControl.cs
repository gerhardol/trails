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
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Visuals;
#if !ST_2_1
using ZoneFiveSoftware.Common.Visuals.Fitness;
using TrailsPlugin.UI.MapLayers;
#endif
using TrailsPlugin.Data;
using TrailsPlugin.Utils;

namespace TrailsPlugin.UI.Activity {
    public partial class MultiChartsControl : UserControl {

        public event System.EventHandler Expand;
        public event System.EventHandler Collapse;

        //Expanded state, controls also if multiple charts/graphs
        private bool m_expanded = false;
        private bool m_showPage;
        private ActivityDetailPageControl m_page;
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
        public void SetControl(ActivityDetailPageControl page, Object view)
        {
#else
        public void SetControl(ActivityDetailPageControl page, IDailyActivityView view)
        {
            m_view = view;
#endif
            m_page = page;
            foreach (TrailLineChart t in m_lineCharts)
            {
                t.SetControl(m_page, this);
            }
            if (this.m_page.IsPopup)
            {
                this.btnExpand.Visible = false;
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
            this.deviceDiffToolStripMenuItem.Image = Properties.Resources.delta;
            //this.diffTimeToolStripMenuItem.Image = Properties.Resources.delta;
            //this.diffDistToolStripMenuItem.Image = Properties.Resources.delta;
            //disable, only show diffDistTime
            this.diffTimeToolStripMenuItem.Visible = false;
            this.diffDistToolStripMenuItem.Visible = false;
            //this.deviceToolStripMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.
            this.deviceSpeedPaceToolStripMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.TrackGPS16;
            this.deviceElevationToolStripMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.TrackElevation16;

            this.PowerBalanceToolStripMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.TrackPower16;

            //this.distanceToolStripMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.TrackGPS16;
            //this.timeToolStripMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Calendar16;
            this.showToolBarMenuItem.Image = Properties.Resources.ChartTools;

            this.adjustResyncDiffAtTrailPointsToolStripMenuItem.Visible = false;

            //The panels and charts should probably be created manually instead
            m_lineCharts = new List<TrailLineChart>();
            foreach (Control t in this.ChartPanel.Controls)
            {
                if (t is TrailLineChart)
                {
                    TrailLineChart lChart = (TrailLineChart)t;
                    m_lineCharts.Add(lChart);
                    if (lChart.MultipleCharts)
                    {
                        m_multiChart = lChart;
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
#if !chartsInDropDownMenu
            //old style select in drop down menu
            paceToolStripMenuItem.Visible = false;
            speedPaceToolStripMenuItem.Visible = false;
            speedToolStripMenuItem.Visible = false;
            gradeStripMenuItem.Visible = false;
            elevationToolStripMenuItem.Visible = false;
            heartRateToolStripMenuItem.Visible = false;
            cadenceToolStripMenuItem.Visible = false;
            powerToolStripMenuItem.Visible = false;
            this.diffDistTimeToolStripMenuItem.Visible = false;

            this.deviceToolStripMenuItem.Visible = false;
            PowerBalanceToolStripMenuItem.Visible = false;
            TemperatureToolStripMenuItem.Visible = false;
            GroundContactTimeToolStripMenuItem.Visible = false;
            VerticalOscillationToolStripMenuItem.Visible = false;
            SaturatedHemoglobinToolStripMenuItem.Visible = false;
            TotalHemoglobinConcentrationToolStripMenuItem.Visible = false;
            deviceDiffToolStripMenuItem.Visible = false;
            deviceSpeedPaceToolStripMenuItem.Visible = false;
            deviceElevationToolStripMenuItem.Visible = false;
#endif
        }

        public void ThemeChanged(ITheme visualTheme)
        {
            this.ChartBanner.ThemeChanged(visualTheme);
            foreach (TrailLineChart t in m_lineCharts)
            {
                t.ThemeChanged(visualTheme);
            }
            this.detailMenu.Renderer = new ThemedContextMenuStripRenderer(visualTheme);
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

            this.PowerBalanceToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.PowerBalance);
            this.TemperatureToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.Temperature);
            this.GroundContactTimeToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.GroundContactTime);
            this.VerticalOscillationToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.VerticalOscillation);
            this.SaturatedHemoglobinToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.SaturatedHemoglobin);
            this.TotalHemoglobinConcentrationToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.TotalHemoglobinConcentration);

            this.diffTimeToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.DiffTime);
            this.diffDistToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.DiffDist);
            this.deviceToolStripMenuItem.Text = CommonResources.Text.LabelDevice;
            this.deviceSpeedPaceToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.DeviceSpeedPace);
            this.deviceElevationToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.DeviceElevation);
            this.deviceDiffToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.DeviceDiffDist);
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
                if (!m_showPage)
                {
                    foreach (TrailLineChart t in m_lineCharts)
                    {
                        t.ShowPage = m_showPage;
                    }
                }
                //refresh (if visible)
                RefreshChart();
            }
        }

        public bool ChartVisible = true;

        public bool Expanded
        {
            set
            {
                 btnExpand.Text = "";
                //For some reason, the Designer moves this button out of the panel
                btnExpand.Left = this.ChartBanner.Right - 46;

                m_expanded = value;
                if (m_expanded)
                {
                    this.ChartBanner.Style = ZoneFiveSoftware.Common.Visuals.ActionBanner.BannerStyle.Header1;
                    this.ChartBanner.Text = Properties.Resources.TrailChartsName;
                    btnExpand.CenterImage = CommonIcons.LowerLeft;

#if chartsInDropDownMenu
                    this.deviceToolStripMenuItem.Visible = false;
                    PowerBalanceToolStripMenuItem.Visible = false;
                    TemperatureToolStripMenuItem.Visible = false;
                    GroundContactTimeToolStripMenuItem.Visible = false;
                    VerticalOscillationToolStripMenuItem.Visible = false;
                    SaturatedHemoglobinToolStripMenuItem.Visible = false;
                    TotalHemoglobinConcentrationToolStripMenuItem.Visible = false;
                    deviceDiffToolStripMenuItem.Visible = false;
                    deviceSpeedPaceToolStripMenuItem.Visible = false;
                    deviceElevationToolStripMenuItem.Visible = false;
#endif
                }
                else
                {
                    this.ChartBanner.Style = ZoneFiveSoftware.Common.Visuals.ActionBanner.BannerStyle.Header2;
                    btnExpand.CenterImage = CommonIcons.LowerHalf;

#if chartsInDropDownMenu
                    this.deviceToolStripMenuItem.Visible = true;
                    PowerBalanceToolStripMenuItem.Visible = true;
                    TemperatureToolStripMenuItem.Visible = true;
                    GroundContactTimeToolStripMenuItem.Visible = true;
                    VerticalOscillationToolStripMenuItem.Visible = true;
                    SaturatedHemoglobinToolStripMenuItem.Visible = true;
                    TotalHemoglobinConcentrationToolStripMenuItem.Visible = true;
                    deviceDiffToolStripMenuItem.Visible = true;
                    deviceSpeedPaceToolStripMenuItem.Visible = true;
                    deviceElevationToolStripMenuItem.Visible = true;
#endif
                }
                this.RefreshChart();
            }
        }

        public TrailResult ReferenceTrailResult
        {
            set
            {
                foreach (TrailLineChart t in m_lineCharts)
                {
                    t.ReferenceTrailResult = value;
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
            if (m_showPage && this.ChartVisible)
            {
                LineChartTypes speedPaceChart = LineChartTypes.Speed;
                LineChartTypes deviceSpeedPaceChart = LineChartTypes.DeviceSpeed;
                if (Controller.TrailController.Instance.ReferenceActivity != null &&
                    Controller.TrailController.Instance.ReferenceActivity.Category.SpeedUnits.Equals(Speed.Units.Pace))
                {
                    speedPaceChart = LineChartTypes.Pace;
                    deviceSpeedPaceChart = LineChartTypes.DevicePace;
                }
                LineChartTypes diffChart = LineChartTypes.DiffDist;
                if (Data.Settings.XAxisValue == XAxisValue.Distance)
                {
                    diffChart = LineChartTypes.DiffTime;
                }

                bool isData = Controller.TrailController.Instance.CurrentActivityTrailIsSelected;

                IList<Data.TrailResult> selectedPresentableResults =
                    TrailResultWrapper.TrailResults(Controller.TrailController.Instance.SelectedResults);
                if (selectedPresentableResults.Count > Data.Settings.MaxChartResults)
                {
                    //Move summary first, so it is not truncated
                    for(int i= 0; i < selectedPresentableResults.Count; i++)
                    {
                        TrailResult t = selectedPresentableResults[i];
                        if (t is SummaryTrailResult)
                        {
                            selectedPresentableResults.RemoveAt(i);
                            selectedPresentableResults.Insert(0, t);
                            break;
                        }
                    }

                    ((List<Data.TrailResult>)selectedPresentableResults).RemoveRange(Data.Settings.MaxChartResults,
                        selectedPresentableResults.Count- Data.Settings.MaxChartResults);
                }

                m_multiChart.ChartTypes=new List<LineChartTypes>();
                multiChart.ShowPage = false;
                //TODO: Temporary handling. Cleanup and decide multiple graphs and charts
                TrailLineChart updateChart = m_multiChart;
                if (!m_expanded)
                {
                    foreach (TrailLineChart chart in m_lineCharts)
                    {
                        chart.ShowPage = false;
                    }
                    m_multiChart.BeginUpdate();
                    m_multiChart.XAxisReferential = Data.Settings.XAxisValue;
                    m_multiChart.TrailResults = selectedPresentableResults;
                    foreach (LineChartTypes t in Data.Settings.MultiChartType)
                    {
                        LineChartTypes t2 = t;
                        if (t2 == LineChartTypes.SpeedPace)
                        {
                            t2 = speedPaceChart;
                        }
                        else if (t2 == LineChartTypes.DeviceSpeedPace)
                        {
                            t2 = deviceSpeedPaceChart;
                        }
                        else if (t2 == LineChartTypes.DiffDistTime)
                        {
                            t2 = diffChart;
                        }
                        if (!m_multiChart.ChartTypes.Contains(t2) &&
                            m_multiChart.HasValues(t2))
                        {
                            m_multiChart.ChartTypes.Add(t2);
                        }
                    }
                    m_multiChart.ShowPage = true;

                    if (!m_multiChart.AnyData())
                    {
                        m_multiChart.ShowPage = false;
                        m_multiChart.LeftChartType = speedPaceChart;
                        m_multiChart.ShowPage = true;
                    }
                    this.ChartBanner.Text = LineChartUtil.ChartTypeString(m_multiChart.LeftChartType) + " / " +
                    LineChartUtil.XAxisValueString(m_multiChart.XAxisReferential);
                    m_multiChart.EndUpdate(true);
                }
                else
                {
                    foreach (TrailLineChart chart in m_lineCharts)
                    {
                        bool visible = false;

                        if (m_expanded &&
                            (Data.Settings.MultiGraphType.Contains(chart.LeftChartType) ||
                            chart.LeftChartType == speedPaceChart &&
                            (Data.Settings.MultiGraphType.Contains(LineChartTypes.SpeedPace) || Data.Settings.MultiGraphType.Contains(LineChartTypes.DeviceSpeedPace))||
                            chart.LeftChartType == diffChart &&
                            (Data.Settings.MultiGraphType.Contains(LineChartTypes.DiffDistTime) || Data.Settings.MultiGraphType.Contains(LineChartTypes.DeviceDiffDist))) ||

                            !m_expanded &&
                            (chart.LeftChartType == Data.Settings.ChartType ||
                            chart.LeftChartType == speedPaceChart &&
                                (LineChartTypes.SpeedPace == Data.Settings.ChartType || LineChartTypes.DeviceSpeedPace == Data.Settings.ChartType) ||
                            chart.LeftChartType == diffChart &&
                                (LineChartTypes.DiffDistTime == Data.Settings.ChartType || LineChartTypes.DiffDistTime == Data.Settings.ChartType)))
                        {
                            visible = true;
                        }

                        updateChart = chart;
                        updateChart.BeginUpdate();
                        chart.ShowPage = false;
                        if (visible && isData)
                        {
                            updateChart.XAxisReferential = Data.Settings.XAxisValue;
                            updateChart.TrailResults = selectedPresentableResults;
                            if (!m_expanded && updateChart.ChartTypes.Count == 1)
                            {
                                this.ChartBanner.Text = LineChartUtil.ChartTypeString(chart.LeftChartType) + " / " +
                                LineChartUtil.XAxisValueString(chart.XAxisReferential);
                            }
                            if (updateChart.HasValues(chart.LeftChartType))
                            {
                                updateChart.ShowPage = visible;
                            }
                            else
                            {
                                if (visible && !updateChart.HasValues(chart.LeftChartType))
                                {
                                    chart.ShowPage = false;
                                    //Replace empty chart
                                    if (!m_expanded && chart.LeftChartType != speedPaceChart)
                                    {
                                        foreach (TrailLineChart replaceChart in m_lineCharts)
                                        {
                                            if (replaceChart.LeftChartType == speedPaceChart)
                                            {
                                                replaceChart.BeginUpdate();
                                                replaceChart.ShowPage = false;
                                                replaceChart.XAxisReferential = Data.Settings.XAxisValue;
                                                replaceChart.TrailResults = selectedPresentableResults;
                                                if (!m_expanded)
                                                {
                                                    this.ChartBanner.Text = LineChartUtil.ChartTypeString(replaceChart.LeftChartType) + " / " +
                                                    LineChartUtil.XAxisValueString(replaceChart.XAxisReferential);
                                                }
                                                replaceChart.ShowPage = visible;
                                                replaceChart.EndUpdate(true);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        updateChart.EndUpdate(true);
                    }
                }
                RefreshRows();
                RefreshChartMenu();
            }
        }

        private bool SetLineChartChecked(LineChartTypes t)
        {
            if (!m_expanded)
            {
                return Data.Settings.MultiChartType.Contains(t);
            }
            else
            {
                return Data.Settings.MultiGraphType.Contains(t);
            }
        }

        public void RefreshChartMenu()
        {
            //TODO: disable if track exists (or ref for diff). 
            speedToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.Speed);
            paceToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.Pace);
            speedPaceToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.SpeedPace);
            elevationToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.Elevation);
            cadenceToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.Cadence);
            heartRateToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.HeartRateBPM);
            gradeStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.Grade);
            powerToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.Power);

            PowerBalanceToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.PowerBalance);
            TemperatureToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.Temperature);
            GroundContactTimeToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.GroundContactTime);
            VerticalOscillationToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.VerticalOscillation);
            SaturatedHemoglobinToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.SaturatedHemoglobin);
            TotalHemoglobinConcentrationToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.TotalHemoglobinConcentration);

            diffTimeToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.DiffTime);
            diffDistToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.DiffDist);
            diffDistTimeToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.DiffDistTime);

            deviceSpeedPaceToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.DeviceSpeedPace);
            deviceElevationToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.DeviceElevation);
            deviceDiffToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.DeviceDiffDist);
            if (Data.Settings.XAxisValue == XAxisValue.Distance)
            {
                this.diffDistTimeToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.DiffTime);
            }
            else
            {
                this.diffDistTimeToolStripMenuItem.Text = LineChartUtil.ChartTypeString(LineChartTypes.DiffDist);
            }
            deviceSpeedPaceToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.DeviceSpeedPace);
            deviceElevationToolStripMenuItem.Checked = SetLineChartChecked(LineChartTypes.DeviceElevation);
            //set as marker, for all subitems -not changable directly
            deviceToolStripMenuItem.Checked = 
                SetLineChartChecked(LineChartTypes.DeviceSpeedPace) ||
                SetLineChartChecked(LineChartTypes.DeviceElevation) || 
                SetLineChartChecked(LineChartTypes.DeviceDiffDist);
            deviceToolStripMenuItem.Enabled = true;

            resyncDiffAtTrailPointsToolStripMenuItem.Checked = Data.Settings.ResyncDiffAtTrailPoints;
            adjustResyncDiffAtTrailPointsToolStripMenuItem.Enabled = resyncDiffAtTrailPointsToolStripMenuItem.Checked;
            adjustResyncDiffAtTrailPointsToolStripMenuItem.Checked = Data.Settings.AdjustResyncDiffAtTrailPoints;
            syncChartAtTrailPointsToolStripMenuItem.Checked = Data.Settings.SyncChartAtTrailPoints;

            timeToolStripMenuItem.Checked = Data.Settings.XAxisValue == XAxisValue.Time;
            distanceToolStripMenuItem.Checked = Data.Settings.XAxisValue == XAxisValue.Distance;
            this.chartSmoothMenuItem.Text = LineChartUtil.SmoothOverTrailBordersString(Data.Settings.SmoothOverTrailPoints);
            //this.showToolBarMenuItem.Checked = Data.Settings.ShowChartToolBar;
            this.showToolBarMenuItem.Text = Data.Settings.ShowChartToolBar ?
                Properties.Resources.UI_Activity_Menu_HideToolBar :
                Properties.Resources.UI_Activity_Menu_ShowToolBar;
        }

        private void RefreshChart(LineChartTypes t)
        {
            if (!m_expanded)
            {
                Data.Settings.ToggleMultiChartType = t;
            }
            else
            {
                Data.Settings.ToggleMultiGraphType = t;
            }
            this.RefreshChart();
        }

        private void RefreshChart(XAxisValue t)
        {
            Data.Settings.XAxisValue = t;
            this.RefreshChart();
        }

        public void ClearSelectedRegions(bool clearRange)
        {
            foreach (TrailLineChart chart in m_lineCharts)
            {
                chart.ClearSelectedRegions(clearRange);
            }
        }

        //Update all (-1) or a specific result (with dataseries), called from one TrailLineChart to all other
        public void SetSelectedResultRange(int resultIndex, IList<float[]> regions, float[] range)
        {
            foreach (TrailLineChart chart in m_lineCharts)
            {
                chart.SetSelectedResultRegions(resultIndex, regions, range);
            }
        }

        public void SetSelectedResultRegions(IList<TrailResultMarked> atr, TrailResultMarked markedRange)
        {
            foreach (TrailLineChart chart in m_lineCharts)
            {
                chart.SetSelectedResultRegions(atr, markedRange);
            }
        }

        public void EnsureVisible(IList<TrailResult> atr)
        {
            foreach (TrailLineChart chart in m_lineCharts)
            {
                chart.EnsureVisible(atr);
            }
        }

        public void ShowGeneralToolTip(string s)
        {
            if (this.m_multiChart != null && this.m_multiChart.Visible)
            {
                this.m_multiChart.ShowGeneralToolTip(s);
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
        private void SpeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.Speed);
        }

        private void PaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.Pace);
        }

        private void SpeedPaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.SpeedPace);
        }

        private void ElevationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.Elevation);
        }

        private void HeartRateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.HeartRateBPM);
        }

        private void CadenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.Cadence);
        }

        private void GradeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.Grade);
        }

        private void PowerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LineChartTypes t = LineChartTypes.Unknown;
            if (sender == powerToolStripMenuItem)
            {
                t = LineChartTypes.Power;
            }
            else if (sender == PowerBalanceToolStripMenuItem)
            {
                t = LineChartTypes.PowerBalance;
            }
            else if (sender == TemperatureToolStripMenuItem)
            {
                t = LineChartTypes.Temperature;
            }
            else if (sender == GroundContactTimeToolStripMenuItem)
            {
                t = LineChartTypes.GroundContactTime;
            }
            else if (sender == VerticalOscillationToolStripMenuItem)
            {
                t = LineChartTypes.VerticalOscillation;
            }
            else if (sender == SaturatedHemoglobinToolStripMenuItem)
            {
                t = LineChartTypes.SaturatedHemoglobin;
            }
            else if (sender == TotalHemoglobinConcentrationToolStripMenuItem)
            {
                t = LineChartTypes.TotalHemoglobinConcentration;
            }

            if (t != LineChartTypes.Unknown)
            {
                RefreshChart(t);
            }
            else
            {
                Debug.Assert(false, "Unknown object" + sender.ToString());
            }
        }

        private void DiffTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.DiffTime);
        }

        private void DiffDistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.DiffDist);
        }

        private void DiffDistTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.DiffDistTime);
        }

        private void DeviceSpeedPaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.DeviceSpeedPace);
        }

        private void DeviceElevationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.DeviceElevation);
        }

        private void DeviceDiffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(LineChartTypes.DeviceDiffDist);
        }

        private void DistanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(XAxisValue.Distance);
        }

        private void TimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshChart(XAxisValue.Time);
        }

        private void ResyncDiffAtTrailPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Data.Settings.ResyncDiffAtTrailPoints = !Data.Settings.ResyncDiffAtTrailPoints;
            RefreshChartMenu();
            m_page.RefreshData(true);
        }

        private void AdjustResyncDiffAtTrailPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Data.Settings.AdjustResyncDiffAtTrailPoints = !Data.Settings.AdjustResyncDiffAtTrailPoints;
            RefreshChartMenu();
            m_page.RefreshData(true);
        }

        private void SyncChartAtTrailPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Data.Settings.SyncChartAtTrailPoints = !Data.Settings.SyncChartAtTrailPoints;
            RefreshChartMenu();
            m_page.RefreshChart();
        }

        private void ChartSmoothMenuItem_Click(object sender, EventArgs e)
        {
            Data.Settings.SmoothOverTrailPointsToggle();
            RefreshChartMenu();
            Controller.TrailController.Instance.CurrentReset(true);
            m_page.RefreshChart();
        }

        private void ShowToolBarMenuItem_Click(object sender, EventArgs e)
        {
            Data.Settings.ShowChartToolBar = !Data.Settings.ShowChartToolBar;
            ShowChartToolBar = Data.Settings.ShowChartToolBar;
        }

        /***************************************/
        private void BtnExpand_Click(object sender, EventArgs e)
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
