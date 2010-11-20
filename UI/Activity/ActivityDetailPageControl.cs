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
using System.ComponentModel;
using System.Drawing;
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
using TrailsPlugin.UI.MapLayers;
#else
using ZoneFiveSoftware.Common.Visuals.Forms;
#endif
#if !ST_2_1
using TrailsPlugin.UI.MapLayers;
using ZoneFiveSoftware.Common.Visuals.Util;
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
        private UI.MapLayers.MapControlLayer m_layer { get { return UI.MapLayers.MapControlLayer.Instance; } }
#else
        private IDetailPage m_DetailPage = null;
        private IDailyActivityView m_view = null;
        private TrailPointsLayer m_layer = null;
#endif

#if ST_2_1
        public ActivityDetailPageControl()
        {
#else
        public ActivityDetailPageControl(IDetailPage detailPage, IDailyActivityView view)
        {
            m_DetailPage = detailPage;
            m_view = view;
            m_layer = TrailPointsLayer.Instance(m_view);
#endif
            m_controller = Controller.TrailController.Instance;

			this.InitializeComponent();
			InitControls();
#if !ST_2_1
            this.ExpandSplitContainer.Panel2Collapsed = true;
#endif
		}

		void InitControls()
        {
            TrailName.ButtonImage = CommonIcons.MenuCascadeArrowDown;
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

            this.ExpandSplitContainer.Panel2Collapsed = true;
            LineChart.ShowChartToolBar = m_showChartToolBar;
            LineChart.DetailPage = this;
            if (null != m_chartsControl) { m_chartsControl.ShowChartToolBar = m_showChartToolBar; }

#if ST_2_1
            summaryListControl.SetResultList(this, m_controller);
#else
            summaryListControl.SetResultList(m_view, this, m_controller);
#endif
            summaryListControl.RefreshColumns();
		}

        private bool _showPage = false;
        public bool ShowPage
        {
            get { return _showPage; }
            set
            {
                _showPage = value;
                m_layer.ShowPage = value;
                summaryListControl.ShowPage = value;
#if !ST_2_1
                if (value)
                {
                    //Not needed now
                    //RefreshData();
                    m_view.RouteSelectionProvider.SelectedItemsChanged += new EventHandler(RouteSelectionProvider_SelectedItemsChanged);
                }
                else
                {
                    m_view.RouteSelectionProvider.SelectedItemsChanged -= new EventHandler(RouteSelectionProvider_SelectedItemsChanged);
                }
#endif
            }
        }

		private void RefreshControlState() 
        {
            bool enabled = (m_controller.FirstActivity != null);
			btnAdd.Enabled = enabled;
			TrailName.Enabled = enabled;

			enabled = (m_controller.CurrentActivityTrail != null);
			btnEdit.Enabled = enabled;
			btnDelete.Enabled = enabled;

            summaryListControl.RefreshControlState();
            if (m_controller.CurrentActivityTrail != null)
            {
                TrailName.Text = m_controller.CurrentActivityTrail.Trail.Name;
            }
            else
            {
                TrailName.Text = "";
            }
        }

        private void RefreshData()
        {
            m_layer.ShowPage = false; //defer updates
            //Update list first, so not refresh changes selection
            summaryListControl.RefreshList();
            RefreshRoute(); 
            RefreshChart();
            m_layer.ShowPage = _showPage;
        }

        private void RefreshRoute()
        {
            if((! m_isExpanded || isReportView)
                && m_controller.CurrentActivityTrail != null)
            {
                m_layer.HighlightRadius = m_controller.CurrentActivityTrail.Trail.Radius;

                IList<TrailGPSLocation> points = new List<TrailGPSLocation>();
                //route
                foreach (TrailGPSLocation point in m_controller.CurrentActivityTrail.Trail.TrailLocations)
                {
                    points.Add(point);
                }
                if (!isSingleView)
                {
                    IList<TrailResult> results = m_controller.CurrentActivityTrail.Results;
                    IDictionary<string, MapPolyline> routes = new Dictionary<string, MapPolyline>();
                    foreach (TrailResult tr in results)
                    {
                        //Possibly limit no of Trails shown, it slows down (but show complete Activities?)
                        TrailMapPolyline m = new TrailMapPolyline(tr);
                        m.Click += new MouseEventHandler(mapPoly_Click);
                        routes.Add(m.key, m);
                    }
                    m_layer.TrailRoutes = routes;
                }
                else
                {
                    m_layer.TrailRoutes = new Dictionary<string, MapPolyline>();
                }
                m_layer.MarkedTrailRoutes = new Dictionary<string, MapPolyline>();
                m_layer.TrailPoints = points;
            }
        }

        public void UICultureChanged(CultureInfo culture)
        {
            m_culture = culture;
            toolTip.SetToolTip(btnAdd, Properties.Resources.UI_Activity_Page_AddTrail_TT);
            toolTip.SetToolTip(btnEdit, Properties.Resources.UI_Activity_Page_EditTrail_TT);
            toolTip.SetToolTip(btnDelete, Properties.Resources.UI_Activity_Page_DeleteTrail_TT);
            this.ChartBanner.Text = Properties.Resources.TrailChartsName;
            this.lblTrail.Text = Properties.Resources.TrailName+":";

            this.RefreshChartMenu();
            this.summaryListControl.RefreshColumns();

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
			summaryListControl.ThemeChanged(visualTheme);
			ChartBanner.ThemeChanged(visualTheme);

			LineChart.ThemeChanged(visualTheme);
			if (m_chartsControl != null) {
				m_chartsControl.ThemeChanged(visualTheme);
			}
		}

        public IList<IActivity> Activities
        {
            set
            {
                m_controller.Activities = value;
#if !ST_2_1
                m_layer.ClearOverlays();
#endif
                summaryListControl.RefreshColumns();
                RefreshData();
                RefreshControlState();
            }
        }

        //Some views like mapping is only working in single view - there are likely better tests
        public bool isSingleView
        {
            get
            {
#if !ST_2_1
                if (CollectionUtils.GetSingleItemOfType<IActivity>(m_view.SelectionProvider.SelectedItems) == null)
                {
                    return false;
                }
#endif
                return true;
            }
        }
        private bool isReportView
        {
            get
            {
            bool result = false;
#if !ST_2_1
            string viewType = m_view.GetType().FullName;

            //if (viewType.EndsWith(".DailyActivityView.MainView"))
            //{
            //    result = false; 
            //}
            //else 
            if (viewType.EndsWith(".ActivityReportDetailsPage"))
            { 
                result = true;
            }
#endif
            return result;
            }
        }

        private IList<TreeList.TreeListNode> getTreeListNodeSplits(IList<TrailResult> results)
        {
            ((List<TrailResult>)results).Sort();
            IList<TreeList.TreeListNode> res2 = new List<TreeList.TreeListNode>();
            foreach (TrailResult tr in results)
            {
                TreeList.TreeListNode tn = new TreeList.TreeListNode(null, tr);
                IList<TrailResult> splits = tr.getSplits();
                //Do not add single splits - nothing to expand
                if (splits.Count > 1)
                {
                    ((List<TrailResult>)splits).Sort();
                    foreach (TrailResult tr2 in splits)
                    {
                        TreeList.TreeListNode tn2 = new TreeList.TreeListNode(tn, tr2);
                        tn.Children.Add(tn2);
                    }
                }
                res2.Add(tn);
            }
            return res2;
        }
        /************************************************************/
		private void btnAdd_Click(object sender, EventArgs e) {

            int countGPS = 0;
#if ST_2_1
			IMapControl mapControl = m_layer.MapControl;
			ICollection<IMapControlObject> selectedGPS = null;
            if (null != mapControl) { selectedGPS = mapControl.Selected; }
#else
            IList<IItemTrackSelectionInfo> selectedGPS = m_view.RouteSelectionProvider.SelectedItems;
#endif
            countGPS = selectedGPS.Count;
            if (countGPS > 0)
            {
#if ST_2_1
                m_layer.SelectedGPSLocationsChanged += new System.EventHandler(layer_SelectedGPSLocationsChanged_AddTrail);
				m_layer.CaptureSelectedGPSLocations();
#else
                selectedGPSLocationsChanged_AddTrail(selectedGPS);
#endif
            } else {
#if ST_2_1
                string message = String.Format(Properties.Resources.UI_Activity_Page_SelectPointsError, countGPS);
                MessageBox.Show(message, "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
#else
                //It is currently not possible to select while in multimode
                //The button could be disabled, error ignored for now
                if (isSingleView && m_controller.CurrentActivity != null)
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
                else
                {
                    selectedGPSLocationsChanged_AddTrail(selectedGPS);
                }
#endif
            }
 		}
        private TrailsItemTrackSelectionInfo getSel(DateTime t)
        {
            IValueRange<DateTime> v = new ValueRange<DateTime>(t, t);
            TrailsItemTrackSelectionInfo s = new TrailsItemTrackSelectionInfo();
            s.SelectedTime = v;
            return s;
        }
        private void btnEdit_Click(object sender, EventArgs e) {
            int countGPS = 0;
#if ST_2_1
			IMapControl mapControl = m_layer.MapControl;
            ICollection<IMapControlObject> selectedGPS = null;
            if (null != mapControl) { selectedGPS = mapControl.Selected; }
#else
            IList<IItemTrackSelectionInfo> selectedGPS = m_view.RouteSelectionProvider.SelectedItems;
#endif
            countGPS = selectedGPS.Count;
            if (countGPS > 0)
            {
#if ST_2_1
				m_layer.SelectedGPSLocationsChanged += new System.EventHandler(layer_SelectedGPSLocationsChanged_EditTrail);
				m_layer.CaptureSelectedGPSLocations();
#else
                selectedGPSLocationsChanged_EditTrail(selectedGPS);
#endif
            } else {
#if ST_2_1
				EditTrail dialog = new EditTrail(m_visualTheme, m_culture, false);
#else
                EditTrail dialog = new EditTrail(m_visualTheme, m_culture, m_view, false);
#endif
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

        /*************************************************************************************************************/
//ST3
        //TODO: Rewrite, using IItemTrackSelectionInfo help functions
        IList<TrailGPSLocation> getGPS(IValueRange<DateTime> ts, IValueRange<double> di)
        {
            IList<TrailGPSLocation> result = new List<TrailGPSLocation>();
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
                result.Add(new TrailGPSLocation(p.Value.LatitudeDegrees, p.Value.LongitudeDegrees, ""));
            }
            return result;
        }
        IList<TrailGPSLocation> getGPS(IList<IItemTrackSelectionInfo> aSelectGPS)
        {
            IList<TrailGPSLocation> result = new List<TrailGPSLocation>();
            for (int i = 0; i < aSelectGPS.Count; i++)
            {
                IItemTrackSelectionInfo selectGPS = aSelectGPS[i];
                IList<TrailGPSLocation> result2 = new List<TrailGPSLocation>();

                //Marked
                IValueRangeSeries<DateTime> tm = selectGPS.MarkedTimes;
                if (null != tm)
                {
                    foreach (IValueRange<DateTime> ts in tm)
                    {
                        result2 = Trail.MergeTrailLocations(result2, getGPS(ts, null));
                    }
                }
                if (result2.Count == 0)
                {
                    IValueRangeSeries<double> td = selectGPS.MarkedDistances;
                    if (null != td)
                    {

                        foreach (IValueRange<double> td1 in td)
                        {
                            result2 = Trail.MergeTrailLocations(result2, getGPS(null, td1));
                        }
                    }
                    if (result2.Count == 0)
                    {
                        //Selected
                        result2 = getGPS(selectGPS.SelectedTime, selectGPS.SelectedDistance);
                    }
                }
                result = Trail.MergeTrailLocations(result, result2);
            }
            return result;
        }
#if ST_2_1
//ST_2_1
        IList<TrailGPSLocation> getGPS(IList<IGPSLocation> aSelectGPS)
        {
            IList<TrailGPSLocation> result = new List<TrailGPSLocation>();
            for (int i = 0; i < aSelectGPS.Count; i++)
            {
                IGPSLocation selectGPS = aSelectGPS[i];
            result.Add(new TrailGPSLocation(selectGPS.LatitudeDegrees, selectGPS.LongitudeDegrees, ""));
            }
            return result;
        }
#endif
#if !ST_2_1
        private void selectedGPSLocationsChanged_AddTrail(IList<IItemTrackSelectionInfo> selectedGPS)
        {
#else
		private void layer_SelectedGPSLocationsChanged_AddTrail(object sender, EventArgs e)
        {
			//UI.MapLayers.MapControlLayer layer = (UI.MapLayers.MapControlLayer)sender;
			m_layer.SelectedGPSLocationsChanged -= new System.EventHandler(layer_SelectedGPSLocationsChanged_AddTrail);
            IList<IGPSLocation> selectedGPS = m_layer.SelectedGPSLocations;
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
#if ST_2_1
            EditTrail dialog = new EditTrail(m_visualTheme, m_culture, !addCurrent);
#else
            EditTrail dialog = new EditTrail(m_visualTheme, m_culture, m_view, !addCurrent);
#endif
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
            dialog.Trail.TrailLocations = Trail.MergeTrailLocations(dialog.Trail.TrailLocations, getGPS(selectedGPS));

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
			m_layer.SelectedGPSLocationsChanged -= new System.EventHandler(layer_SelectedGPSLocationsChanged_EditTrail);
            IList<IGPSLocation> selectedGPS = m_layer.SelectedGPSLocations;
#endif
#if ST_2_1
            EditTrail dialog = new EditTrail(m_visualTheme, m_culture, false);
#else
            EditTrail dialog = new EditTrail(m_visualTheme, m_culture, m_view, false);
#endif
            bool selectionIsDifferent = selectedGPS.Count != dialog.Trail.TrailLocations.Count;
            if (!selectionIsDifferent)
            {
                IList<TrailGPSLocation> loc = getGPS(selectedGPS);
                if (loc.Count == selectedGPS.Count)
                {
                    for (int i = 0; i < loc.Count; i++)
                    {
                        TrailGPSLocation loc1 = loc[i];
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
            TreeListPopup treeListPopup = new TreeListPopup();
            treeListPopup.ThemeChanged(m_visualTheme);
            treeListPopup.Tree.Columns.Add(new TreeList.Column());

            treeListPopup.Tree.RowData = m_controller.OrderedTrails;
            treeListPopup.Tree.LabelProvider = new TrailDropdownLabelProvider();

            if (m_controller.CurrentActivityTrail != null)
            {
#if ST_2_1
                treeListPopup.Tree.Selected = new object[] { m_controller.CurrentActivityTrail };
#else
                treeListPopup.Tree.SelectedItems = new object[] { m_controller.CurrentActivityTrail };
#endif
            }
            treeListPopup.ItemSelected += new TreeListPopup.ItemSelectedEventHandler(TrailName_ItemSelected);
            treeListPopup.Popup(this.TrailName.Parent.RectangleToScreen(this.TrailName.Bounds));
        }

        /*******************************************************/

		class TrailDropdownLabelProvider : TreeList.ILabelProvider {

			public Image GetImage(object element, TreeList.Column column) {
				ActivityTrail t = (ActivityTrail)element;
				if (!t.IsInBounds) {
					return CommonIcons.BlueSquare;
				} else if (t.Results.Count > 0) {
					return CommonIcons.GreenSquare;
				} else {
					return CommonIcons.RedSquare;
				}
			}

			public string GetText(object element, TreeList.Column column) {
				ActivityTrail t = (ActivityTrail)element;
				return t.Trail.Name;
			}
		}

		private void TrailName_ItemSelected(object sender, EventArgs e) {
			ActivityTrail t = (ActivityTrail)((TreeListPopup.ItemSelectedEventArgs)e).Item;
			m_controller.CurrentActivityTrail = t;
			RefreshData();
			RefreshControlState();
		}

        public void MarkTrack(IList<TrailResultMarked> atr)
        {
#if !ST_2_1
            if (_showPage)
            {
                if (m_view != null &&
                    m_view.RouteSelectionProvider != null &&
                    isSingleView && m_controller.CurrentActivity != null)
                {
                    if (atr.Count > 0)
                    {
                        //Only one activity, OK to merge selections on one track
                        TrailsItemTrackSelectionInfo r = TrailResultMarked.SelInfoUnion(atr);
                        r.Activity = m_controller.CurrentActivity;
                        m_view.RouteSelectionProvider.SelectedItems = new IItemTrackSelectionInfo[] { r };
                        m_layer.ZoomRoute = atr[0].trailResult.GpsPoints(r);
                    }
                }
                else
                {
                    IDictionary<string, MapPolyline> result = new Dictionary<string, MapPolyline>();
                    foreach (TrailResultMarked trm in atr)
                    {
                        TrailMapPolyline m = new TrailMapPolyline(trm.trailResult, trm.selInfo);
                        m.Click += new MouseEventHandler(mapPoly_Click);
                        result.Add(m.key, m);
                    }
                    m_layer.MarkedTrailRoutes = result;
                }
            }
#endif
        }

        void mapPoly_Click(object sender, MouseEventArgs e)
        {
            if (sender is TrailMapPolyline)
            {
                TrailMapPolyline tm = sender as TrailMapPolyline;
                if (tm.key.Contains("m"))
                {
                    summaryListControl.SelectedItems = new List<TrailResult> { tm.TrailRes };
                }
                else
                {
                    summaryListControl.SelectedItems = TrailResult.TrailResultList(tm.TrailRes.Activity);
                }
            }
        }

		private void ChartBanner_MenuClicked(object sender, EventArgs e) {
			ChartBanner.ContextMenuStrip.Width = 100;
			ChartBanner.ContextMenuStrip.Show(ChartBanner.Parent.PointToScreen(new System.Drawing.Point(ChartBanner.Right - ChartBanner.ContextMenuStrip.Width - 2, 
                ChartBanner.Bottom + 1)));
		}

        public void RefreshChart() {
			if(m_isExpanded) {
                IList<TrailResult> list = this.summaryListControl.SelectedItems;
                if (list.Count > 0)
                {
                    m_chartsControl.RefreshCharts(list[0]);
                }
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
                    IList<TrailResult> list = this.summaryListControl.SelectedItems;
                    if (list.Count > 0)
                    {
                        this.LineChart.TrailResult = list[0];
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
                    this.LineChart.SetSelected(selected.SelectedItems);
                    if (null != m_chartsControl) { m_chartsControl.SetSelected(selected.SelectedItems); }
                }
            }
        }
#endif
    }
}
