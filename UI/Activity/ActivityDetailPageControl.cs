﻿/*
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
using System.Globalization;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ZoneFiveSoftware.Common.Visuals.Chart;
#if ST_2_1
using ZoneFiveSoftware.Common.Visuals.Fitness.GPS;
#else
using ZoneFiveSoftware.Common.Visuals.Mapping;
#endif
#if ST_2_1
//IListItem, ListSettings
using ZoneFiveSoftware.SportTracks.Util;
using ZoneFiveSoftware.SportTracks.UI;
using ZoneFiveSoftware.SportTracks.UI.Forms;
using ZoneFiveSoftware.SportTracks.Data;
#else
using ZoneFiveSoftware.Common.Visuals.Forms;
#endif
#if !ST_2_1
using TrailsPlugin.UI.MapLayers;
#endif
using TrailsPlugin.Data;

namespace TrailsPlugin.UI.Activity {
	public partial class ActivityDetailPageControl : UserControl {

        private ITheme m_visualTheme =
#if ST_2_1
                PluginMain.GetApplication().VisualTheme;
#else
                PluginMain.GetApplication().SystemPreferences.VisualTheme;
#endif
        private CultureInfo m_culture =
#if ST_2_1
                new System.Globalization.CultureInfo("en");
#else
                PluginMain.GetApplication().SystemPreferences.UICulture;
#endif

		private Controller.TrailController m_controller;
		private ChartsControl m_chartsControl = null;
		private bool m_isExpanded = false;
        private bool m_showChartToolBar = true;

#if ST_2_1
        private UI.MapLayers.MapControlLayer layer { get { return UI.MapLayers.MapControlLayer.Instance; } }
#else
        private IDetailPage m_DetailPage = null;
        private IDailyActivityView m_view = null;
        private TrailPointsProvider m_TrailPointsProvider = TrailPointsProvider.Instance;
        private TrailPointsLayer layer { get { return (TrailPointsLayer)m_TrailPointsProvider.RouteControlLayer; } }
#endif

#if !ST_2_1
        public ActivityDetailPageControl(IDetailPage detailPage, IDailyActivityView view) : this()
        {
            m_DetailPage = detailPage;
            m_view = view;
            m_view.RouteSelectionProvider.SelectedItemsChanged += new EventHandler(RouteSelectionProvider_SelectedItemsChanged);
        }
#endif
        public ActivityDetailPageControl()
        {

			m_controller = Controller.TrailController.Instance;

			InitializeComponent();
			InitControls();
#if ST_2_1
            this.summaryList.SelectedChanged += new System.EventHandler(this.List_SelectedChanged);
#else
            this.summaryList.SelectedItemsChanged += new System.EventHandler(this.List_SelectedChanged);
            this.ExpandSplitContainer.Panel2Collapsed = true;
#endif
		}

		void InitControls()
        {
            TrailName.ButtonImage = CommonIcons.MenuCascadeArrowDown;
            copyTableMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.DocumentCopy16;
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

			btnAdd.BackgroundImage = CommonIcons.Add;
			btnAdd.Text = "";
			btnEdit.BackgroundImage = CommonIcons.Edit;
			btnEdit.Text = "";
			btnDelete.BackgroundImage = CommonIcons.Delete;
			btnDelete.Text = "";
            btnExpand.BackgroundImage = CommonIcons.LowerHalf;
            btnExpand.Text = "";
            //For some reason, the Designer moves this button out of the panel
            this.btnExpand.Location = new System.Drawing.Point(353, 1);
			listSettingsMenuItem.Image = CommonIcons.ListSettings;

			summaryList.NumHeaderRows = TreeList.HeaderRows.Two;
			summaryList.LabelProvider = new TrailResultLabelProvider();
            this.ExpandSplitContainer.Panel2Collapsed = true;
            LineChart.ShowChartToolBar = m_showChartToolBar;
            LineChart.DetailPage = this;
            if (null != m_chartsControl) { m_chartsControl.ShowChartToolBar = m_showChartToolBar; }

			this.RefreshColumns();
		}

        private bool _showPage = false;
        public bool ShowPage
        {
            get { return _showPage; }
            set
            {
                _showPage = value;
                layer.ShowPage = value;

                if (value)
                {
                    //Not needed now
                    //RefreshData();
                }
            }
        }

        private void RefreshColumns()
        {
			summaryList.Columns.Clear();
			foreach (string id in PluginMain.Settings.ActivityPageColumns) {
				foreach (
#if ST_2_1
                    ListItemInfo
#else
                    IListColumnDefinition
#endif
                columnDef in TrailResultColumnIds.ColumnDefs(m_controller.FirstActivity, m_controller.Activities.Count>1))
                {
					if (columnDef.Id == id) {
						TreeList.Column column = new TreeList.Column(
							columnDef.Id,
                            columnDef.Text(columnDef.Id),
							columnDef.Width,
							columnDef.Align
						);
						summaryList.Columns.Add(column);
						break;
					}
				}
			}
		}

		private void RefreshControlState() 
        {
            bool enabled = (m_controller.CurrentActivity != null);
			btnAdd.Enabled = enabled;
            enabled = (m_controller.FirstActivity != null);
			TrailName.Enabled = enabled;

			enabled = (m_controller.CurrentActivityTrail != null);
			btnEdit.Enabled = enabled;
			btnDelete.Enabled = enabled;

		}

        private void RefreshData()
        {
            layer.ShowPage = false; //defer updates
            layer.TrailPoints.Clear();
            summaryList.RowData = null;

            if (m_controller.CurrentActivityTrail != null)
            {
                TrailName.Text = m_controller.CurrentActivityTrail.Trail.Name;
                IList<Data.TrailResult> results = m_controller.CurrentActivityTrail.Results;
                summaryList.RowData = results;
                ((TrailResultLabelProvider)summaryList.LabelProvider).MultipleActivities = (m_controller.Activities.Count > 1);
                if (results.Count > 0)
                {
                    summaryList.Selected = new object[] { results[0] };
                }

                //Set size, to not waste chart
                int resRows = Math.Min(5, ((IList<Data.TrailResult>)(this.summaryList.RowData)).Count);
                this.summaryList.Height = this.summaryList.HeaderRowHeight +
                    this.summaryList.DefaultRowHeight * resRows;

                foreach (Data.TrailGPSLocation point in m_controller.CurrentActivityTrail.Trail.TrailLocations)
                {
                    layer.TrailPoints.Add(point);
                }
                layer.HighlightRadius = m_controller.CurrentActivityTrail.Trail.Radius;
                layer.ShowPage = _showPage;//Refresh
            }
            else
            {
                TrailName.Text = "";
            }
            RefreshChart();
        }
        public void UICultureChanged(CultureInfo culture)
        {
            m_culture = culture;
            toolTip.SetToolTip(btnAdd, Properties.Resources.UI_Activity_Page_AddTrail_TT);
            toolTip.SetToolTip(btnEdit, Properties.Resources.UI_Activity_Page_EditTrail_TT);
            toolTip.SetToolTip(btnDelete, Properties.Resources.UI_Activity_Page_DeleteTrail_TT);
            this.ChartBanner.Text = Properties.Resources.TrailChartsName;
            this.lblTrail.Text = Properties.Resources.TrailName+":";

            copyTableMenuItem.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionCopy;
            this.listSettingsMenuItem.Text = Properties.Resources.UI_Activity_Page_ListSettings;

            this.RefreshChartMenu();
            this.RefreshColumns();

            LineChart.UICultureChanged(culture);
            if (m_chartsControl != null)
            {
                m_chartsControl.UICultureChanged(culture);
            }
        }
        public void ThemeChanged(ITheme visualTheme)
        {
			m_visualTheme = visualTheme;
			TrailName.ThemeChanged(visualTheme);
			summaryList.ThemeChanged(visualTheme);
			ChartBanner.ThemeChanged(visualTheme);

			LineChart.ThemeChanged(visualTheme);
			if (m_chartsControl != null) {
				m_chartsControl.ThemeChanged(visualTheme);
			}
		}

		public IList<IActivity> Activities {
			set {
                    m_controller.Activities = value;
                    RefreshColumns();
                    RefreshData();
                    RefreshControlState();
			}
		}

        /************************************************************/
		private void btnAdd_Click(object sender, EventArgs e) {

            int countGPS = 0;
#if ST_2_1
			IMapControl mapControl = layer.MapControl;
			ICollection<IMapControlObject> selectedGPS = null;
            if (null != mapControl) { selectedGPS = mapControl.Selected; }
#else
            IList<IItemTrackSelectionInfo> selectedGPS = m_view.RouteSelectionProvider.SelectedItems;
#endif
            countGPS = selectedGPS.Count;
            if (countGPS > 0)
            {
#if ST_2_1
                layer.SelectedGPSLocationsChanged += new System.EventHandler(layer_SelectedGPSLocationsChanged_AddTrail);
				layer.CaptureSelectedGPSLocations();
#else
                selectedGPSLocationsChanged_AddTrail(selectedGPS);
#endif
            } else {
#if ST_2_1
                string message = String.Format(Properties.Resources.UI_Activity_Page_SelectPointsError, countGPS);
                MessageBox.Show(message, "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
#else
                //It is currently not possible to select while in multimode
                //The button should be disabled, error ignored for now
                if (null != m_controller.CurrentActivity)
                {
                    if (MessageBox.Show(string.Format(Properties.Resources.UI_Activity_Page_AddTrail_NoSelected, CommonResources.Text.ActionYes, CommonResources.Text.ActionNo)
                        , "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        //Using IItemTrackSelectionInfo to avoid duplicating code
                        if (null == m_controller.CurrentActivity.Laps || 0 == m_controller.CurrentActivity.Laps.Count)
                        {
                            selectedGPS.Add(getSel(m_controller.CurrentActivity.StartTime));
                        }
                        else
                        {
                            foreach (ILapInfo l in m_controller.CurrentActivity.Laps)
                            {
                                selectedGPS.Add(getSel(l.StartTime));
                            }
                        }
                        ActivityInfo activityInfo = ActivityInfoCache.Instance.GetInfo(m_controller.CurrentActivity);
                        selectedGPS.Add(getSel(activityInfo.EndTime));
                        selectedGPSLocationsChanged_AddTrail(selectedGPS);
                        selectedGPS.Clear();
                    }
                }
#endif
            }
 		}
        private Data.TrailsItemTrackSelectionInfo getSel(DateTime t)
        {
            IValueRange<DateTime> v = new ValueRange<DateTime>(t, t);
            Data.TrailsItemTrackSelectionInfo s = new Data.TrailsItemTrackSelectionInfo();
            s.SelectedTime = v;
            return s;
        }
        private void btnEdit_Click(object sender, EventArgs e) {
            int countGPS = 0;
#if ST_2_1
			IMapControl mapControl = layer.MapControl;
            ICollection<IMapControlObject> selectedGPS = null;
            if (null != mapControl) { selectedGPS = mapControl.Selected; }
#else
            IList<IItemTrackSelectionInfo> selectedGPS = m_view.RouteSelectionProvider.SelectedItems;
#endif
            countGPS = selectedGPS.Count;
            if (countGPS > 0)
            {
#if ST_2_1
				layer.SelectedGPSLocationsChanged += new System.EventHandler(layer_SelectedGPSLocationsChanged_EditTrail);
				layer.CaptureSelectedGPSLocations();
#else
                selectedGPSLocationsChanged_EditTrail(selectedGPS);
#endif
            } else {
				EditTrail dialog = new EditTrail(m_visualTheme, m_culture, false);
				if (dialog.ShowDialog() == DialogResult.OK) {
					RefreshControlState();
					RefreshData();
				}
			}
		}

		private void btnDelete_Click(object sender, EventArgs e) {
			if (MessageBox.Show(Properties.Resources.UI_Activity_Page_DeleteTrailConfirm, m_controller.CurrentActivityTrail.Trail.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) 
                == DialogResult.Yes) {
				m_controller.DeleteCurrentTrail();
				RefreshControlState();
				RefreshData();
			}
		}

        void copyTableMenu_Click(object sender, EventArgs e)
        {
            summaryList.CopyTextToClipboard(true, System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator);
        }

        /*************************************************************************************************************/
//ST3
        IList<Data.TrailGPSLocation> getGPS(IValueRange<DateTime> ts, IValueRange<double> di)
        {
            IList<Data.TrailGPSLocation> result = new List<Data.TrailGPSLocation>();
            ITimeValueEntry<IGPSPoint> p = null;
            if (null != ts)
            {
                p = m_controller.CurrentActivity.GPSRoute.GetInterpolatedValue(ts.Lower);
            }
            else
            {
                //Normally, selecting by time is null, fall back to select by distance
                if (null != di && null != m_controller.CurrentActivity && null != m_controller.CurrentActivity.GPSRoute)
                {
                    IDistanceDataTrack dt = m_controller.CurrentActivity.GPSRoute.GetDistanceMetersTrack();
                    p = m_controller.CurrentActivity.GPSRoute.GetInterpolatedValue(dt.GetTimeAtDistanceMeters(di.Lower));
                }
            }
            if (null != p)
            {
                result.Add(new Data.TrailGPSLocation(p.Value.LatitudeDegrees, p.Value.LongitudeDegrees, ""));
            }
            return result;
        }
        IList<Data.TrailGPSLocation> getGPS(IList<IItemTrackSelectionInfo> aSelectGPS)
        {
            IList<Data.TrailGPSLocation> result = new List<Data.TrailGPSLocation>();
            for (int i = 0; i < aSelectGPS.Count; i++)
            {
                IItemTrackSelectionInfo selectGPS = aSelectGPS[i];
                IList<Data.TrailGPSLocation> result2 = new List<Data.TrailGPSLocation>();

                //Marked
                IValueRangeSeries<DateTime> tm = selectGPS.MarkedTimes;
                if (null != tm)
                {
                    foreach (IValueRange<DateTime> ts in tm)
                    {
                        result2 = Data.Trail.MergeTrailLocations(result2, getGPS(ts, null));
                    }
                }
                if (result2.Count == 0)
                {
                    IValueRangeSeries<double> td = selectGPS.MarkedDistances;
                    if (null != td)
                    {

                        foreach (IValueRange<double> td1 in td)
                        {
                            result2 = Data.Trail.MergeTrailLocations(result2, getGPS(null, td1));
                        }
                    }
                    if (result2.Count == 0)
                    {
                        //Selected
                        result2 = getGPS(selectGPS.SelectedTime, selectGPS.SelectedDistance);
                    }
                }
                result = Data.Trail.MergeTrailLocations(result, result2);
            }
            return result;
        }
//ST_2_1
        IList<Data.TrailGPSLocation> getGPS(IList<IGPSLocation> aSelectGPS)
        {
            IList<Data.TrailGPSLocation> result = new List<Data.TrailGPSLocation>();
            for (int i = 0; i < aSelectGPS.Count; i++)
            {
                IGPSLocation selectGPS = aSelectGPS[i];
            result.Add(new Data.TrailGPSLocation(selectGPS.LatitudeDegrees, selectGPS.LongitudeDegrees, ""));
            }
            return result;
        }

#if !ST_2_1
        private void selectedGPSLocationsChanged_AddTrail(IList<IItemTrackSelectionInfo> selectedGPS)
        {
#else
		private void layer_SelectedGPSLocationsChanged_AddTrail(object sender, EventArgs e)
        {
			//UI.MapLayers.MapControlLayer layer = (UI.MapLayers.MapControlLayer)sender;
			layer.SelectedGPSLocationsChanged -= new System.EventHandler(layer_SelectedGPSLocationsChanged_AddTrail);
            IList<IGPSLocation> selectedGPS = layer.SelectedGPSLocations;
#endif
            bool addCurrent = false;
            if (m_controller.CurrentActivityTrail != null)
            {
                if (MessageBox.Show(string.Format(Properties.Resources.UI_Activity_Page_AddTrail_Replace, CommonResources.Text.ActionYes,CommonResources.Text.ActionNo),
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    addCurrent = true;
                }
            }
            EditTrail dialog = new EditTrail(m_visualTheme, m_culture, !addCurrent);
            if (m_controller.CurrentActivityTrail != null)
            {
                if (addCurrent)
                {
                    //TODO: sort old/new points, so it is possible to add in middle?
                }
                else
                {
                    dialog.Trail.TrailLocations.Clear();
                }
            }
            dialog.Trail.TrailLocations = Data.Trail.MergeTrailLocations(dialog.Trail.TrailLocations, getGPS(selectedGPS));

			if (dialog.ShowDialog() == DialogResult.OK) {
				RefreshControlState();
				RefreshData();
			}
		}


#if !ST_2_1
        private void selectedGPSLocationsChanged_EditTrail(IList<IItemTrackSelectionInfo> selectedGPS)
        {
#else
 		private void layer_SelectedGPSLocationsChanged_EditTrail(object sender, EventArgs e)
        {
			//UI.MapLayers.MapControlLayer layer = (UI.MapLayers.MapControlLayer)sender;
			layer.SelectedGPSLocationsChanged -= new System.EventHandler(layer_SelectedGPSLocationsChanged_EditTrail);
            IList<IGPSLocation> selectedGPS = layer.SelectedGPSLocations;
#endif
            EditTrail dialog = new EditTrail(m_visualTheme, m_culture, false);
            bool selectionIsDifferent = selectedGPS.Count != dialog.Trail.TrailLocations.Count;
            if (!selectionIsDifferent)
            {
                IList<Data.TrailGPSLocation> loc = getGPS(selectedGPS);
                if (loc.Count == selectedGPS.Count)
                {
                    for (int i = 0; i < loc.Count; i++)
                    {
                        Data.TrailGPSLocation loc1 = loc[i];
                        IGPSLocation loc2 = dialog.Trail.TrailLocations[i].GpsLocation;
                        if (loc1.LatitudeDegrees != loc2.LatitudeDegrees
                                || loc1.LongitudeDegrees != loc2.LongitudeDegrees)
                        {
                            selectionIsDifferent = true;
                            break;
                        }
                    }
                }
            }
 
            if (selectionIsDifferent)
            {
                if (MessageBox.Show(Properties.Resources.UI_Activity_Page_UpdateTrail, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dialog.Trail.TrailLocations = getGPS(selectedGPS);
                }
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                RefreshControlState();
                RefreshData();
            }
        }


        private void TrailName_ButtonClick(object sender, EventArgs e)
        {
            //xxx if (m_controller.Activities.Count > 0)
            {
                TreeListPopup treeListPopup = new TreeListPopup();
                treeListPopup.ThemeChanged(m_visualTheme);
                treeListPopup.Tree.Columns.Add(new TreeList.Column());

                treeListPopup.Tree.RowData = this.OrderedTrails;
                treeListPopup.Tree.LabelProvider = new TrailDropdownLabelProvider();

                if (m_controller.CurrentActivityTrail != null)
                {
                    treeListPopup.Tree.Selected = new object[] { m_controller.CurrentActivityTrail };
                }
                treeListPopup.ItemSelected += new TreeListPopup.ItemSelectedEventHandler(TrailName_ItemSelected);
                treeListPopup.Popup(this.TrailName.Parent.RectangleToScreen(this.TrailName.Bounds));
            }
        }


        /*******************************************************/

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
#if ST_2_1
            ListSettings dialog = new ListSettings();
			dialog.ColumnsAvailable = TrailResultColumnIds.ColumnDefs(m_controller.FirstActivity);
#else
            ListSettingsDialog dialog = new ListSettingsDialog();
            dialog.AvailableColumns = TrailResultColumnIds.ColumnDefs(m_controller.FirstActivity, m_controller.Activities.Count > 1);
#endif
            dialog.ThemeChanged(m_visualTheme);
			dialog.AllowFixedColumnSelect = true;
			dialog.SelectedColumns = PluginMain.Settings.ActivityPageColumns;
			dialog.NumFixedColumns = PluginMain.Settings.ActivityPageNumFixedColumns;

            if (dialog.ShowDialog() == DialogResult.OK) {
				PluginMain.Settings.ActivityPageNumFixedColumns = dialog.NumFixedColumns;
				PluginMain.Settings.ActivityPageColumns = dialog.SelectedColumns;
				RefreshColumns();
			}
        }

        public void SelectTrack(DateTime firstTime, DateTime lastTime)
        {
                IValueRangeSeries<DateTime> t = new ValueRangeSeries<DateTime>();
                t.Add(new ValueRange<DateTime>(firstTime, lastTime));
                SelectTrack(t);
        }
        public void SelectTrack(IValueRangeSeries<DateTime> t)
        {
#if !ST_2_1
            if (m_view != null &&
                m_view.RouteSelectionProvider != null &&
                m_controller.CurrentActivity != null)
            {
                Data.TrailsItemTrackSelectionInfo r = new Data.TrailsItemTrackSelectionInfo();
                r.MarkedTimes = t;
                r.Activity = m_controller.CurrentActivity;
                m_view.RouteSelectionProvider.SelectedItems = new IItemTrackSelectionInfo[] { r };
            }
#endif
        }
        public void SelectTrack(double firstDist, double lastDist)
        {
                IValueRangeSeries<double> t = new ValueRangeSeries<double>();
                t.Add(new ValueRange<double>(firstDist, lastDist));
                SelectTrack(t);
        }
        public void SelectTrack(IValueRangeSeries<double> t)
        {
#if !ST_2_1
            if (m_view != null &&
                m_view.RouteSelectionProvider != null &&
                m_controller.CurrentActivity != null)
            {
                Data.TrailsItemTrackSelectionInfo r = new Data.TrailsItemTrackSelectionInfo();
                r.MarkedDistances = t;
                r.Activity = m_controller.CurrentActivity;
                m_view.RouteSelectionProvider.SelectedItems = new IItemTrackSelectionInfo[] { r };
            }
#endif
        }
        private void List_SelectedChanged(object sender, EventArgs e)
        {
			RefreshChart();
            //SelectTrack, for ST3
            if (sender is TreeList)
            {
                TreeList l = sender as TreeList;
                if (l.SelectedItems != null && l.SelectedItems.Count > 0 &&
                    l.SelectedItems[0] is Data.TrailResult &&
                    l.SelectedItems[0] != null)
                {
                    Data.TrailResult tr = l.SelectedItems[0] as Data.TrailResult;
                    SelectTrack(tr.FirstTime, tr.LastTime);
                }
            }
        }

		private void ChartBanner_MenuClicked(object sender, EventArgs e) {
			ChartBanner.ContextMenuStrip.Width = 100;
			ChartBanner.ContextMenuStrip.Show(ChartBanner.Parent.PointToScreen(new System.Drawing.Point(ChartBanner.Right - ChartBanner.ContextMenuStrip.Width - 2, 
                ChartBanner.Bottom + 1)));
		}

		void RefreshChart() {
			if(m_isExpanded) {				
				Data.TrailResult result = null;
				if (m_controller.CurrentActivityTrail != null) {								
					IList<Data.TrailResult> results = m_controller.CurrentActivityTrail.Results;
					if (((IList<Data.TrailResult>)this.summaryList.RowData).Count > 0 && this.summaryList.Selected.Count > 0) {
						result = (Data.TrailResult)this.summaryList.SelectedItems[0];
                    }
				}
                m_chartsControl.RefreshCharts(result);
                m_chartsControl.RefreshRows();
            }
            else
            {
				this.LineChart.BeginUpdate();
				this.LineChart.TrailResult = null;
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
                    IList<Data.TrailResult> results = m_controller.CurrentActivityTrail.Results;
					if (((IList<Data.TrailResult>)this.summaryList.RowData).Count > 0 && this.summaryList.Selected.Count > 0) {
						this.LineChart.TrailResult = (Data.TrailResult)this.summaryList.SelectedItems[0];
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

        public bool ShowChartToolBar
        {
            set
            {
                m_showChartToolBar = value;
                RefreshChartMenu();
                LineChart.ShowChartToolBar = m_showChartToolBar;
                if (null != m_chartsControl) { m_chartsControl.ShowChartToolBar = m_showChartToolBar; }
            }
        }
        private void showToolBarMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowChartToolBar = !m_showChartToolBar;
        }

		private IList<Data.ActivityTrail> OrderedTrails {
			get {
				SortedList<double, Data.ActivityTrail> trailsUsed = new SortedList<double, Data.ActivityTrail>();
				SortedList<string, Data.ActivityTrail> trailsInBounds = new SortedList<string, Data.ActivityTrail>();
				SortedList<string, Data.ActivityTrail> trailsNotInBound = new SortedList<string, Data.ActivityTrail>();

				foreach (Data.Trail trail in PluginMain.Data.AllTrails.Values) {
					Data.ActivityTrail at = new TrailsPlugin.Data.ActivityTrail(m_controller.Activities, trail);
					if (trail.IsInBounds(m_controller.Activities)) {
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

		private void ActPagePanel_SizeChanged(object sender, EventArgs e) {
			// autosize column doesn't seem to be working.
            //Sizing is flaky in general
			float width = 0;
			for (int i = 0; i < ActPagePanel.ColumnStyles.Count; i++) {
				if (i != 1) {
					width += this.ActPagePanel.ColumnStyles[i].Width;
				}
			}
			this.ActPagePanel.ColumnStyles[1].SizeType = SizeType.Absolute;
            this.ActPagePanel.ColumnStyles[1].Width = this.ActPagePanel.Width - width;
		}

#if ST_2_1
		private System.Windows.Forms.SplitContainer DailyActivitySplitter {
			get
            {
				Control c = this.Parent;
				while (c != null) {
                    if (c is ZoneFiveSoftware.SportTracks.UI.Views.Activities.ActivityDetailPanel) {
						return (System.Windows.Forms.SplitContainer)((ZoneFiveSoftware.SportTracks.UI.Views.Activities.ActivityDetailPanel)c).Controls[0];
                }
					c = c.Parent;
				}
                throw new Exception("Daily Activity Splitter not found");
			}
		}
#endif

        private void btnExpand_Click(object sender, EventArgs e) {
#if ST_2_1
			SplitterPanel p2 = DailyActivitySplitter.Panel2;
#else
            int width = this.ActPagePanel.Width;
#endif
            if (m_chartsControl == null) {
				m_chartsControl = new ChartsControl();
                m_chartsControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                m_chartsControl.Dock = DockStyle.Fill;
                m_chartsControl.Top = 0;
                m_chartsControl.Left = 0;
#if ST_2_1
                p2.Controls.Add(m_chartsControl);
#else
                this.ExpandSplitContainer.Panel2.Controls.Add(m_chartsControl);
#endif
                m_chartsControl.ThemeChanged(m_visualTheme);
                m_chartsControl.UICultureChanged(m_culture);
                m_chartsControl.DetailPage = this;
                m_chartsControl.Collapse += new EventHandler(m_chartsControl_Collapse);
			}
			m_chartsControl.Visible = true;
            m_chartsControl.ShowChartToolBar = m_showChartToolBar; 
            ActPageSplitContainer.Panel2Collapsed = true;
#if ST_2_1
			p2.Controls[0].Visible = false;
            m_chartsControl.Width = p2.Width;
            m_chartsControl.Height = p2.Height;
#else
            m_DetailPage.PageMaximized = true;
            this.ExpandSplitContainer.Panel2Collapsed = false;
            this.ExpandSplitContainer.SplitterDistance = width;
#endif
            m_isExpanded = true;
            RefreshChart();
		}

		private void m_chartsControl_Collapse(object sender, EventArgs e) {
            m_chartsControl.Visible = false;
			ActPageSplitContainer.Panel2Collapsed = false;
#if ST_2_1
            SplitterPanel p2 = DailyActivitySplitter.Panel2;
            p2.Controls[0].Visible = true;
#else
            this.ExpandSplitContainer.Panel2Collapsed = true;
            m_DetailPage.PageMaximized = false;
#endif
            m_isExpanded = false;
            RefreshChart();
		}

#if !ST_2_1
        void RouteSelectionProvider_SelectedItemsChanged(object sender, EventArgs e)
        {
            if (sender is ISelectionProvider<IItemTrackSelectionInfo>)
            {
                //m_view.RouteSelectionProvider.SelectedItems
                ISelectionProvider<IItemTrackSelectionInfo> selected = sender as ISelectionProvider<IItemTrackSelectionInfo>;
                if (selected != null && selected.SelectedItems != null && selected.SelectedItems.Count > 0)
                {
                    //Only first set used (chart only selects one of the ranges anyway...)
                    IItemTrackSelectionInfo selectGPS = selected.SelectedItems[0];
                    IValueRangeSeries<DateTime> timeSeries = new ValueRangeSeries<DateTime>();
                    IValueRangeSeries<double> distSeries = new ValueRangeSeries<double>();

                    if (selectGPS.MarkedTimes != null)
                    {
                        timeSeries = selectGPS.MarkedTimes;
                    }
                    else if (selectGPS.MarkedDistances != null)
                    {
                        distSeries = selectGPS.MarkedDistances;
                    }
                    else if (selectGPS.SelectedTime != null)
                    {
                        timeSeries.Add(selectGPS.SelectedTime);
                    }
                    else if (selectGPS.SelectedDistance != null)
                    {
                        distSeries.Add(selectGPS.SelectedDistance);
                    }

                    if (timeSeries.Count > 0)
                    {
                        this.LineChart.SetSelected(timeSeries);
                        if (null != m_chartsControl) { m_chartsControl.SetSelected(timeSeries); }
                    }
                    else if (distSeries.Count > 0)
                    {
                        this.LineChart.SetSelected(distSeries);
                        if (null != m_chartsControl) { m_chartsControl.SetSelected(distSeries); }
                    }
                }
            }
        }
#endif
    }
}
