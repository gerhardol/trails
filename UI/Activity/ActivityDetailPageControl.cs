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
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;
#if ST_2_1
using ZoneFiveSoftware.Common.Visuals.Fitness.GPS;
#else
using ZoneFiveSoftware.Common.Visuals.Mapping;
#endif
//IListItem, ListSettings
using ZoneFiveSoftware.SportTracks.Util;
using ZoneFiveSoftware.SportTracks.UI;
using ZoneFiveSoftware.SportTracks.UI.Forms;
using ZoneFiveSoftware.SportTracks.Data;
using ZoneFiveSoftware.Common.Visuals.Chart;

namespace TrailsPlugin.UI.Activity {
	public partial class ActivityDetailPageControl : UserControl {

		private ITheme m_visualTheme;
		private Controller.TrailController m_controller;
		private ChartsControl m_chartsControl = null;
		private bool m_isExpanded = false;

		public ActivityDetailPageControl(IActivity activity) {

			m_controller = Controller.TrailController.Instance;

			InitializeComponent();
			InitControls();

			m_controller.CurrentActivity = activity;

			RefreshControlState();
			RefreshData();
		}

		void InitControls() {

			TrailName.ButtonImage = CommonIcons.MenuCascadeArrowDown;

			btnAdd.BackgroundImage = CommonIcons.Add;
			btnAdd.Text = "";
			btnEdit.BackgroundImage = CommonIcons.Edit;
			btnEdit.Text = "";
			btnDelete.BackgroundImage = CommonIcons.Delete;
			btnDelete.Text = "";
			btnExpand.BackgroundImage = CommonIcons.LowerHalf;
			btnExpand.Text = "";
			toolTip.SetToolTip(btnAdd, Properties.Resources.UI_Activity_Page_AddTrail_TT);
			toolTip.SetToolTip(btnEdit, Properties.Resources.UI_Activity_Page_EditTrail_TT);
            toolTip.SetToolTip(btnDelete, Properties.Resources.UI_Activity_Page_DeleteTrail_TT);

            this.listSettingsMenuItem.Text = Properties.Resources.UI_Activity_Page_ListSettings;
			listSettingsMenuItem.Image = CommonIcons.ListSettings;

			List.NumHeaderRows = TreeList.HeaderRows.Two;
			List.LabelProvider = new TrailResultLabelProvider();
            btnExpand.Left = this.Right - 46;

			this.RefreshColumns();
			this.RefreshChartMenu();
		}

		private void RefreshColumns() {

			List.Columns.Clear();
			foreach (string id in PluginMain.Settings.ActivityPageColumns) {
				foreach (ListItemInfo columnDef in TrailResultColumnIds.ColumnDefs(m_controller.CurrentActivity)) {
					if (columnDef.Id == id) {
						TreeList.Column column = new TreeList.Column(
							columnDef.Id,
							columnDef.ToString(),
							columnDef.Width,
							columnDef.Align
						);
						List.Columns.Add(column);
						break;
					}
				}
			}
		}

		private void RefreshControlState() {

			bool enabled = (m_controller.CurrentActivity != null);
			btnAdd.Enabled = enabled;
			TrailName.Enabled = enabled;

			enabled = (m_controller.CurrentActivityTrail != null);
			btnEdit.Enabled = enabled;
			btnDelete.Enabled = enabled;

		}

		private void RefreshData() {
			UI.MapLayers.MapControlLayer layer = UI.MapLayers.MapControlLayer.Instance;
			layer.HighlightedGPSLocations.Clear();
			layer.ShowHighlight = false;
			List.RowData = null;

			if (m_controller.CurrentActivityTrail != null) {
				TrailName.Text = m_controller.CurrentActivityTrail.Trail.Name;
				IList<Data.TrailResult> results = m_controller.CurrentActivityTrail.Results;
				List.RowData = results;
				if (results.Count > 0) {
					List.Selected = new object[] { results[0] };
				}

				foreach (Data.TrailGPSLocation point in m_controller.CurrentActivityTrail.Trail.TrailLocations) {
					layer.HighlightedGPSLocations.Add(point);
				}
				layer.HighlightRadius = m_controller.CurrentActivityTrail.Trail.Radius;
				layer.ShowHighlight = true;

			} else {
				TrailName.Text = "";
			}
			RefreshChart();
		}


		public void ThemeChanged(ITheme visualTheme) {
			m_visualTheme = visualTheme;
			TrailName.ThemeChanged(visualTheme);
			List.ThemeChanged(visualTheme);
			ChartBanner.ThemeChanged(visualTheme);
			LineChart.ThemeChanged(visualTheme);
			if (m_chartsControl != null) {
				m_chartsControl.ThemeChanged(visualTheme);
			}
		}

		public IActivity Activity {
			set {
				m_controller.CurrentActivity = value;
				RefreshColumns();
				RefreshData();
				RefreshControlState();
			}
		}

		private void btnAdd_Click(object sender, EventArgs e) {

			UI.MapLayers.MapControlLayer layer = UI.MapLayers.MapControlLayer.Instance;
			IMapControl mapControl = layer.MapControl;
            if (
#if ST_2_1
			mapControl.Selected.Count
#else
            layer.SelectedGPSLocations.Count
#endif
               > 1)
            {
                layer.SelectedGPSLocationsChanged += new System.EventHandler(layer_SelectedGPSLocationsChanged_AddTrail);
				layer.CaptureSelectedGPSLocations();
			} else {
                string message = String.Format(Properties.Resources.UI_Activity_Page_SelectPointsError,
#if ST_2_1
			mapControl.Selected.Count
#else
            layer.SelectedGPSLocations.Count
#endif
                    );
				MessageBox.Show(message, "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		private void btnEdit_Click(object sender, EventArgs e) {
			UI.MapLayers.MapControlLayer layer = UI.MapLayers.MapControlLayer.Instance;
			IMapControl mapControl = layer.MapControl;
            if (
#if ST_2_1
			   mapControl.Selected.Count
#else
               layer.SelectedGPSLocations.Count
#endif
                > 1)
            {
				layer.SelectedGPSLocationsChanged += new System.EventHandler(layer_SelectedGPSLocationsChanged_EditTrail);
				layer.CaptureSelectedGPSLocations();
			} else {
				EditTrail dialog = new EditTrail(m_visualTheme, false);
				if (dialog.ShowDialog() == DialogResult.OK) {
					RefreshControlState();
					RefreshData();
				}
			}
		}

		private void btnDelete_Click(object sender, EventArgs e) {
			if (MessageBox.Show(Properties.Resources.UI_Activity_Page_DeleteTrailConfirm, m_controller.CurrentActivityTrail.Trail.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
				m_controller.DeleteCurrentTrail();
				RefreshControlState();
				RefreshData();
			}
		}

		private void layer_SelectedGPSLocationsChanged_AddTrail(object sender, EventArgs e) {

			UI.MapLayers.MapControlLayer layer = (UI.MapLayers.MapControlLayer)sender;
			layer.SelectedGPSLocationsChanged -= new System.EventHandler(layer_SelectedGPSLocationsChanged_AddTrail);

			EditTrail dialog = new EditTrail(m_visualTheme, true);
			for (int i = 0; i < layer.SelectedGPSLocations.Count; i++) {
				dialog.Trail.TrailLocations.Add(
					new Data.TrailGPSLocation(
						layer.SelectedGPSLocations[i].LatitudeDegrees,
						layer.SelectedGPSLocations[i].LongitudeDegrees,
                        ""
					)
				);
			}

			if (dialog.ShowDialog() == DialogResult.OK) {
				RefreshControlState();
				RefreshData();
			}

		}

		private void layer_SelectedGPSLocationsChanged_EditTrail(object sender, EventArgs e) {
			UI.MapLayers.MapControlLayer layer = (UI.MapLayers.MapControlLayer)sender;
			layer.SelectedGPSLocationsChanged -= new System.EventHandler(layer_SelectedGPSLocationsChanged_EditTrail);

			EditTrail dialog = new EditTrail(m_visualTheme, false);
			bool selectionIsDifferent = layer.SelectedGPSLocations.Count != dialog.Trail.TrailLocations.Count;
			if (!selectionIsDifferent) {
				for (int i = 0; i < layer.SelectedGPSLocations.Count; i++) {
					IGPSLocation loc1 = layer.SelectedGPSLocations[i];
					IGPSLocation loc2 = dialog.Trail.TrailLocations[i];
					if (loc1.LatitudeDegrees != loc2.LatitudeDegrees) {
						selectionIsDifferent = true;
						break;
					}
					if (loc1.LongitudeDegrees != loc2.LongitudeDegrees) {
						selectionIsDifferent = true;
						break;
					}
				}
			}
			if (selectionIsDifferent) {
				if (MessageBox.Show(Properties.Resources.UI_Activity_Page_UpdateTrail, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
					dialog.Trail.TrailLocations.Clear();
					for (int i = 0; i < layer.SelectedGPSLocations.Count; i++) {
						dialog.Trail.TrailLocations.Add(
							new Data.TrailGPSLocation(
								layer.SelectedGPSLocations[i].LatitudeDegrees,
								layer.SelectedGPSLocations[i].LongitudeDegrees,
                                ""
							)
						);
					}
				}
			}

			if (dialog.ShowDialog() == DialogResult.OK) {
				RefreshControlState();
				RefreshData();
			}
		}

		private void TrailName_ButtonClick(object sender, EventArgs e) {
			if (m_controller.CurrentActivity == null) {
				return;
			}

			TreeListPopup treeListPopup = new TreeListPopup();
			treeListPopup.ThemeChanged(m_visualTheme);
			treeListPopup.Tree.Columns.Add(new TreeList.Column());

			treeListPopup.Tree.RowData = this.OrderedTrails;
			treeListPopup.Tree.LabelProvider = new TrailDropdownLabelProvider();

			if (m_controller.CurrentActivityTrail != null) {
				treeListPopup.Tree.Selected = new object[] { m_controller.CurrentActivityTrail };
			}
			treeListPopup.ItemSelected += new TreeListPopup.ItemSelectedEventHandler(TrailName_ItemSelected);
			treeListPopup.Popup(this.TrailName.Parent.RectangleToScreen(this.TrailName.Bounds));
		}

		class TrailDropdownLabelProvider : TreeList.ILabelProvider {

			public Image GetImage(object element, TreeList.Column column) {
				Data.ActivityTrail t = (Data.ActivityTrail)element;
				if (!t.IsInBounds) {
					return CommonIcons.BlueSquare;
				} else if (t.Results.Count > 0) {
					return CommonIcons.GreenSquare;
				} else {
					return CommonIcons.RedSquare;
				}
			}

			public string GetText(object element, TreeList.Column column) {
				Data.ActivityTrail t = (Data.ActivityTrail)element;
				return t.Trail.Name;
			}
		}

		private void TrailName_ItemSelected(object sender, EventArgs e) {
			Data.ActivityTrail t = (Data.ActivityTrail)((TreeListPopup.ItemSelectedEventArgs)e).Item;
			m_controller.CurrentActivityTrail = t;
			RefreshData();
			RefreshControlState();
		}

		private void listSettingsToolStripMenuItem_Click(object sender, EventArgs e) {
			ListSettings dialog = new ListSettings();
			dialog.ThemeChanged(m_visualTheme);
			dialog.ColumnsAvailable = TrailResultColumnIds.ColumnDefs(m_controller.CurrentActivity);
			dialog.AllowFixedColumnSelect = true;
			dialog.SelectedColumns = PluginMain.Settings.ActivityPageColumns;
			dialog.NumFixedColumns = PluginMain.Settings.ActivityPageNumFixedColumns;

			if (dialog.ShowDialog() == DialogResult.OK) {
				PluginMain.Settings.ActivityPageNumFixedColumns = dialog.NumFixedColumns;
				PluginMain.Settings.ActivityPageColumns = dialog.SelectedColumns;
				RefreshColumns();
			}
		}

		private void List_SelectedChanged(object sender, EventArgs e) {
			RefreshChart();
		}

		private void ChartBanner_MenuClicked(object sender, EventArgs e) {
			ChartBanner.ContextMenuStrip.Width = 100;
			ChartBanner.ContextMenuStrip.Show(ChartBanner.Parent.PointToScreen(new System.Drawing.Point(ChartBanner.Right - ChartBanner.ContextMenuStrip.Width - 2, ChartBanner.Bottom + 1)));

		}

		void RefreshChart() {
			if(m_isExpanded) {				
				IActivity activity = null;
				Data.TrailResult result = null;
				if (m_controller.CurrentActivityTrail != null) {								
					activity = m_controller.CurrentActivityTrail.Activity;
					IList<Data.TrailResult> results = m_controller.CurrentActivityTrail.Results;
					if (((IList<Data.TrailResult>)this.List.RowData).Count > 0 && this.List.Selected.Count > 0) {
						result = (Data.TrailResult)this.List.SelectedItems[0];
					}
				}
                m_chartsControl.RefreshCharts(activity, result);
                m_chartsControl.RefreshRows();
            }
            else
            {
				this.LineChart.BeginUpdate();
				this.LineChart.Activity = null;
				this.LineChart.TrailResult = null;
				if (m_controller.CurrentActivityTrail != null) {
					this.LineChart.Activity = m_controller.CurrentActivityTrail.Activity;
                    this.ChartBanner.Text = PluginMain.Settings.ChartTypeString(PluginMain.Settings.ChartType) + " / " +
                        PluginMain.Settings.XAxisValueString(PluginMain.Settings.XAxisValue);
                    this.LineChart.YAxisReferential = PluginMain.Settings.ChartType;
					this.LineChart.XAxisReferential = PluginMain.Settings.XAxisValue;
					IList<Data.TrailResult> results = m_controller.CurrentActivityTrail.Results;
					if (((IList<Data.TrailResult>)this.List.RowData).Count > 0 && this.List.Selected.Count > 0) {
						this.LineChart.TrailResult = (Data.TrailResult)this.List.SelectedItems[0];
					}
				}
				this.LineChart.EndUpdate();
			}
		}

		void RefreshChartMenu() {
			speedToolStripMenuItem.Checked = PluginMain.Settings.ChartType == TrailLineChart.LineChartTypes.Speed;
            this.speedToolStripMenuItem.Text = PluginMain.Settings.ChartTypeString(TrailLineChart.LineChartTypes.Speed);
			paceToolStripMenuItem.Checked = PluginMain.Settings.ChartType == TrailLineChart.LineChartTypes.Pace;
            this.paceToolStripMenuItem.Text = PluginMain.Settings.ChartTypeString(TrailLineChart.LineChartTypes.Pace);
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

			timeToolStripMenuItem.Checked = PluginMain.Settings.XAxisValue == TrailLineChart.XAxisValue.Time;
            this.timeToolStripMenuItem.Text = PluginMain.Settings.XAxisValueString(TrailLineChart.XAxisValue.Time);
            distanceToolStripMenuItem.Checked = PluginMain.Settings.XAxisValue == TrailLineChart.XAxisValue.Distance;
            this.distanceToolStripMenuItem.Text = PluginMain.Settings.XAxisValueString(TrailLineChart.XAxisValue.Distance);
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
		private void powerToolStripMenuItem_Click(object sender, EventArgs e) {
			PluginMain.Settings.ChartType = TrailLineChart.LineChartTypes.Power;
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

		private IList<Data.ActivityTrail> OrderedTrails {
			get {
				SortedList<double, Data.ActivityTrail> trailsUsed = new SortedList<double, Data.ActivityTrail>();
				SortedList<string, Data.ActivityTrail> trailsInBounds = new SortedList<string, Data.ActivityTrail>();
				SortedList<string, Data.ActivityTrail> trailsNotInBound = new SortedList<string, Data.ActivityTrail>();

				IGPSBounds gpsBounds = GPSBounds.FromGPSRoute(m_controller.CurrentActivity.GPSRoute);
				foreach (Data.Trail trail in PluginMain.Data.AllTrails.Values) {
					Data.ActivityTrail at = new TrailsPlugin.Data.ActivityTrail(m_controller.CurrentActivity, trail);
					if (trail.IsInBounds(gpsBounds)) {
						if (at.Results.Count > 0) {
							double key = at.Results[0].StartTime.TotalSeconds;
							while (trailsUsed.ContainsKey(key)) {
								key++;
							}
							trailsUsed.Add(key, at);
						} else {
							trailsInBounds.Add(at.Trail.Name, at);
						}
					} else {
						trailsNotInBound.Add(at.Trail.Name, at);
					}
				}

				IList<Data.ActivityTrail> trails = new List<Data.ActivityTrail>();
				foreach (Data.ActivityTrail t in trailsUsed.Values) {
					trails.Add(t);
				}
				foreach (Data.ActivityTrail t in trailsInBounds.Values) {
					trails.Add(t);
				}
				foreach (Data.ActivityTrail t in trailsNotInBound.Values) {
					trails.Add(t);
				}
				return trails;
			}
		}

		private void ActivityDetailPageControl_SizeChanged(object sender, EventArgs e) {
			// autosize column doesn't seem to be working. 
			float width = 0;
			for (int i = 0; i < Panel.ColumnStyles.Count; i++) {
				if (i != 1) {
					width += this.Panel.ColumnStyles[i].Width;
				}
			}
			this.Panel.ColumnStyles[1].SizeType = SizeType.Absolute;
			this.Panel.ColumnStyles[1].Width = this.Width - width;
		}

		private System.Windows.Forms.SplitContainer DailyActivitySplitter {
			get
            {
#if ST_2_1
//ST3fix
				Control c = this.Parent;
				while (c != null) {
                    if (c is ZoneFiveSoftware.SportTracks.UI.Views.Activities.ActivityDetailPanel) {
						return (System.Windows.Forms.SplitContainer)((ZoneFiveSoftware.SportTracks.UI.Views.Activities.ActivityDetailPanel)c).Controls[0];
                }
					c = c.Parent;
				}
#endif
                throw new Exception("Daily Activity Splitter not found");
			}
		}

		private void btnExpand_Click(object sender, EventArgs e) {
			SplitterPanel p2 = DailyActivitySplitter.Panel2;
			p2.Controls[0].Visible = false;
			if (m_chartsControl == null) {
				m_chartsControl = new ChartsControl();
                m_chartsControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                m_chartsControl.Dock = DockStyle.Fill;
                m_chartsControl.Top = 0;
                m_chartsControl.Left = 0;
                m_chartsControl.Width = p2.Width;
                m_chartsControl.Height = p2.Height;
                DailyActivitySplitter.Panel2.Controls.Add(m_chartsControl);
				m_chartsControl.ThemeChanged(m_visualTheme);
				m_chartsControl.Collapse += new EventHandler(m_chartsControl_Collapse);
			}
			m_chartsControl.Visible = true;
			SplitContainer.Panel2Collapsed = true;
			m_isExpanded = true;
			RefreshChart();
		}

		private void m_chartsControl_Collapse(object sender, EventArgs e) {
			SplitterPanel p2 = DailyActivitySplitter.Panel2;
			p2.Controls[0].Visible = true;
			m_chartsControl.Visible = false;
			SplitContainer.Panel2Collapsed = false;
			m_isExpanded = false;
			RefreshChart();
		}
	}
}
