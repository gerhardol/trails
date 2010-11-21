/*
Copyright (C) 2009 Brendan Doherty
Copyright (C) 2010 Gerhard Olsson

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
using System.Windows.Forms;
using System.Globalization;

using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Chart;

#if !ST_2_1
using ZoneFiveSoftware.Common.Visuals.Fitness;
#endif
using TrailsPlugin.Data;

namespace TrailsPlugin.UI.Activity {
	public partial class SingleChartsControl : UserControl {

        public event System.EventHandler Expand;
        ActivityDetailPageControl m_page;
        private Controller.TrailController m_controller;
        private bool m_showChartToolBar = true;

#if !ST_2_1
        private IDailyActivityView m_view = null;
#endif

        public SingleChartsControl()
        {
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
            LineChart.SetControl(m_page);
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

            btnExpand.BackgroundImage = CommonIcons.LowerHalf;
            btnExpand.Text = "";
            //For some reason, the Designer moves this button out of the panel
            this.btnExpand.Location = new System.Drawing.Point(353, 1);

            LineChart.ShowChartToolBar = m_showChartToolBar;
		}

        private bool _showPage = false;
        public bool ShowPage
        {
            get { return _showPage; }
            set
            {
                _showPage = value;
            }
        }

		private void RefreshControlState() 
        {
        }

        public void UICultureChanged(CultureInfo culture)
        {
            this.ChartBanner.Text = Properties.Resources.TrailChartsName;

            this.RefreshChartMenu();
            LineChart.UICultureChanged(culture);
        }
        public void ThemeChanged(ITheme visualTheme)
        {
			ChartBanner.ThemeChanged(visualTheme);

			LineChart.ThemeChanged(visualTheme);
		}

        public void SetSelected(IList<IItemTrackSelectionInfo> asel)
        {
            this.LineChart.SetSelected(asel);
        }
        /************************************************************/

		private void ChartBanner_MenuClicked(object sender, EventArgs e) {
			ChartBanner.ContextMenuStrip.Width = 100;
			ChartBanner.ContextMenuStrip.Show(ChartBanner.Parent.PointToScreen(new System.Drawing.Point(ChartBanner.Right - ChartBanner.ContextMenuStrip.Width - 2, 
                ChartBanner.Bottom + 1)));
		}

        public void RefreshChart() {
				this.LineChart.BeginUpdate();
				this.LineChart.ReferenceTrailResult = null;
				if (m_controller.CurrentActivityTrail != null) {
                    if (TrailLineChart.LineChartTypes.SpeedPace == PluginMain.Settings.ChartType)
                    {
                        if (m_controller.FirstActivity != null && 
                            m_controller.FirstActivity.Category.SpeedUnits.Equals(Speed.Units.Speed))
                        {
                            this.LineChart.YAxisReferential = TrailLineChart.LineChartTypes.Speed;
                        }
                        else
                        {
                            this.LineChart.YAxisReferential = TrailLineChart.LineChartTypes.Pace;
                        }
                    }
                    else
                    {
                        this.LineChart.YAxisReferential = PluginMain.Settings.ChartType;
                    }
					this.LineChart.XAxisReferential = PluginMain.Settings.XAxisValue;
                    this.ChartBanner.Text = PluginMain.Settings.ChartTypeString(this.LineChart.YAxisReferential) + " / " +
                        PluginMain.Settings.XAxisValueString(this.LineChart.XAxisReferential);
                    IList<TrailResult> list = this.m_page.SelectedItems;
                    if (list.Count > 0)
                    {
                        this.LineChart.ReferenceTrailResult = list[0];
                    }
                    this.LineChart.TrailResults = list;
                }
				this.LineChart.EndUpdate();
		}

		void RefreshChartMenu() {
			speedToolStripMenuItem.Checked = PluginMain.Settings.ChartType == TrailLineChart.LineChartTypes.Speed;
            this.speedToolStripMenuItem.Text = PluginMain.Settings.ChartTypeString(TrailLineChart.LineChartTypes.Speed);
			paceToolStripMenuItem.Checked = PluginMain.Settings.ChartType == TrailLineChart.LineChartTypes.Pace;
            this.paceToolStripMenuItem.Text = PluginMain.Settings.ChartTypeString(TrailLineChart.LineChartTypes.Pace);
            speedPaceToolStripMenuItem.Checked = PluginMain.Settings.ChartType == TrailLineChart.LineChartTypes.SpeedPace;
            this.speedPaceToolStripMenuItem.Text = PluginMain.Settings.ChartTypeString(TrailLineChart.LineChartTypes.SpeedPace);
            elevationToolStripMenuItem.Checked = PluginMain.Settings.ChartType == TrailLineChart.LineChartTypes.Elevation;
            this.elevationToolStripMenuItem.Text = PluginMain.Settings.ChartTypeString(TrailLineChart.LineChartTypes.Elevation);
            cadenceToolStripMenuItem.Checked = PluginMain.Settings.ChartType == TrailLineChart.LineChartTypes.Cadence;
            this.cadenceToolStripMenuItem.Text = PluginMain.Settings.ChartTypeString(TrailLineChart.LineChartTypes.Cadence);
            heartRateToolStripMenuItem.Checked = PluginMain.Settings.ChartType == TrailLineChart.LineChartTypes.HeartRateBPM;
            this.heartRateToolStripMenuItem.Text = PluginMain.Settings.ChartTypeString(TrailLineChart.LineChartTypes.HeartRateBPM);
            gradeStripMenuItem.Checked = PluginMain.Settings.ChartType == TrailLineChart.LineChartTypes.Grade;
            this.gradeStripMenuItem.Text = PluginMain.Settings.ChartTypeString(TrailLineChart.LineChartTypes.Grade);
            powerToolStripMenuItem.Checked = PluginMain.Settings.ChartType == TrailLineChart.LineChartTypes.Power;
            this.powerToolStripMenuItem.Text = PluginMain.Settings.ChartTypeString(TrailLineChart.LineChartTypes.Power);

            timeDiffToolStripMenuItem.Checked = PluginMain.Settings.ChartType == TrailLineChart.LineChartTypes.TimeDiff;
            this.timeDiffToolStripMenuItem.Text = PluginMain.Settings.ChartTypeString(TrailLineChart.LineChartTypes.TimeDiff);
            distDiffToolStripMenuItem.Checked = PluginMain.Settings.ChartType == TrailLineChart.LineChartTypes.DistDiff;
            this.distDiffToolStripMenuItem.Text = PluginMain.Settings.ChartTypeString(TrailLineChart.LineChartTypes.DistDiff);
            timeDiffToolStripMenuItem.Visible = false;//TODO: temporary
            distDiffToolStripMenuItem.Visible = false;

            timeToolStripMenuItem.Checked = PluginMain.Settings.XAxisValue == TrailLineChart.XAxisValue.Time;
            this.timeToolStripMenuItem.Text = PluginMain.Settings.XAxisValueString(TrailLineChart.XAxisValue.Time);
            distanceToolStripMenuItem.Checked = PluginMain.Settings.XAxisValue == TrailLineChart.XAxisValue.Distance;
            this.distanceToolStripMenuItem.Text = PluginMain.Settings.XAxisValueString(TrailLineChart.XAxisValue.Distance);
            this.showToolBarMenuItem.Text = Properties.Resources.UI_Activity_Menu_ShowToolBar;
            this.showToolBarMenuItem.Checked = m_showChartToolBar;
            //this.showToolBarMenuItem.Text = m_showChartToolBar ? Properties.Resources.UI_Activity_Menu_HideToolBar
            //   : Properties.Resources.UI_Activity_Menu_ShowToolBar;
        }

		private void speedToolStripMenuItem_Click(object sender, EventArgs e) {
			PluginMain.Settings.ChartType = TrailLineChart.LineChartTypes.Speed;
			RefreshChartMenu();
			RefreshChart();
		}

		private void paceToolStripMenuItem_Click(object sender, EventArgs e) {
			PluginMain.Settings.ChartType = TrailLineChart.LineChartTypes.Pace;
			RefreshChartMenu();
			RefreshChart();
		}
        private void speedPaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PluginMain.Settings.ChartType = TrailLineChart.LineChartTypes.SpeedPace;
            RefreshChartMenu();
            RefreshChart();
        }

		private void elevationToolStripMenuItem_Click(object sender, EventArgs e) {
			PluginMain.Settings.ChartType = TrailLineChart.LineChartTypes.Elevation;
			RefreshChartMenu();
			RefreshChart();
		}

		private void heartRateToolStripMenuItem_Click(object sender, EventArgs e) {
			PluginMain.Settings.ChartType = TrailLineChart.LineChartTypes.HeartRateBPM;
			RefreshChartMenu();
			RefreshChart();
		}

		private void cadenceToolStripMenuItem_Click(object sender, EventArgs e) {
			PluginMain.Settings.ChartType = TrailLineChart.LineChartTypes.Cadence;
			RefreshChartMenu();
			RefreshChart();
		}

		private void gradeToolStripMenuItem_Click(object sender, EventArgs e) {
			PluginMain.Settings.ChartType = TrailLineChart.LineChartTypes.Grade;
			RefreshChartMenu();
			RefreshChart();
		}
        private void powerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PluginMain.Settings.ChartType = TrailLineChart.LineChartTypes.Power;
            RefreshChartMenu();
            RefreshChart();
        }

        private void timeDiffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PluginMain.Settings.ChartType = TrailLineChart.LineChartTypes.TimeDiff;
            RefreshChartMenu();
            RefreshChart();
        }
        private void distDiffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PluginMain.Settings.ChartType = TrailLineChart.LineChartTypes.DistDiff;
            RefreshChartMenu();
            RefreshChart();
        }

		private void distanceToolStripMenuItem_Click(object sender, EventArgs e) {
			PluginMain.Settings.XAxisValue = TrailLineChart.XAxisValue.Distance;
			RefreshChartMenu();
			RefreshChart();
		}

		private void timeToolStripMenuItem_Click(object sender, EventArgs e) {
			PluginMain.Settings.XAxisValue = TrailLineChart.XAxisValue.Time;
			RefreshChartMenu();
			RefreshChart();
		}

        public bool ShowChartToolBar
        {
            get { return m_showChartToolBar; }
            set
            {
                m_showChartToolBar = value;
                RefreshChartMenu();
                LineChart.ShowChartToolBar = m_showChartToolBar;
            }
        }
        private void showToolBarMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowChartToolBar = !m_showChartToolBar;
        }

        private void btnExpand_Click(object sender, EventArgs e) {
            Expand(sender, e);
		}
    }
}
