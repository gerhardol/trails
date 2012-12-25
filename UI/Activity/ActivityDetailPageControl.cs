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

//Note: This module has been split in several classes, but there is still some interwining...

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

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
using TrailsPlugin.UI.MapLayers;
using ZoneFiveSoftware.Common.Visuals.Util;
#endif
using TrailsPlugin.Data;
using TrailsPlugin.Utils;

namespace TrailsPlugin.UI.Activity {
	public partial class ActivityDetailPageControl : UserControl {

        private ITheme m_visualTheme =
#if ST_2_1
                PluginMain.GetApplication().VisualTheme;
#else
                Plugin.GetApplication().SystemPreferences.VisualTheme;
#endif
        private CultureInfo m_culture =
#if ST_2_1
                new System.Globalization.CultureInfo("en");
#else
                Plugin.GetApplication().SystemPreferences.UICulture;
#endif

		private Controller.TrailController m_controller;
		private bool m_isExpanded = false;

#if ST_2_1
        private Object m_view = null;
        private UI.MapLayers.MapControlLayer m_layer { get { return UI.MapLayers.MapControlLayer.Instance; } }
#else
        private IDetailPage m_DetailPage = null;
        private IDailyActivityView m_view = null;
        private TrailPointsLayer m_layerPoints = null;
        private TrailPointsLayer m_layerRoutes = null;
        private TrailPointsLayer m_layerMarked = null;
#endif

#if ST_2_1
        public ActivityDetailPageControl()
        {
#else
        public ActivityDetailPageControl(IDetailPage detailPage, IDailyActivityView view)
        {
            m_DetailPage = detailPage;
            m_view = view;
            m_layerPoints = TrailPointsLayer.InstancePoints(this, m_view);
            m_layerRoutes = TrailPointsLayer.InstanceRoutes(this, m_view);
            m_layerMarked = TrailPointsLayer.InstanceMarked(this, m_view);
#endif
            m_controller = Controller.TrailController.Instance;

			this.InitializeComponent();
			InitControls();
		}

		void InitControls()
        {
#if !ST_2_1
            this.ExpandSplitContainer.Panel2Collapsed = true;
#endif

            TrailSelector.SetControl(this, m_controller, m_view, m_layerPoints);
            ResultList.SetControl(this, m_controller, m_view);
            MultiCharts.SetControl(this, m_controller, m_view, m_layerMarked);
#if ST_2_1
			SplitContainer sc = DailyActivitySplitter;
            if (sc != null)
            {
               sc.Panel2.Controls.Add(MultiCharts);
            }
#endif
		}

        public void UICultureChanged(CultureInfo culture)
        {
            m_culture = culture;

            this.TrailSelector.UICultureChanged(culture);
            this.ResultList.UICultureChanged(culture);
            this.MultiCharts.UICultureChanged(culture);
        }

        public void ThemeChanged(ITheme visualTheme)
        {
            m_visualTheme = visualTheme;
            TrailSelector.ThemeChanged(visualTheme);
            ResultList.ThemeChanged(visualTheme);
            MultiCharts.ThemeChanged(visualTheme);
        }

        public IList<IActivity> Activities
        {
            set
            {
                m_controller.Activities = value;
#if !ST_2_1
                m_layerPoints.ClearOverlays();
                m_layerRoutes.ClearOverlays();
                m_layerMarked.ClearOverlays();
#endif
                RefreshData();
                RefreshControlState();
                //RefreshChart();
                if (value != null && value.Count == 1)
                {
                    this.ResultList.addCurrentCategoryCheck();
                }
            }
        }

        private bool m_showPage = false;
        public bool HidePage()
        {
            m_showPage = false;
#if !ST_2_1
            m_view.RouteSelectionProvider.SelectedItemsChanged -= new EventHandler(RouteSelectionProvider_SelectedItemsChanged);
#endif
            m_layerPoints.HidePage();
            m_layerRoutes.HidePage();
            m_layerMarked.HidePage();
            TrailSelector.ShowPage = false;
            ResultList.ShowPage = false;
            MultiCharts.ShowPage = false;
            return true;
        }

        public void ShowPage(string bookmark)
        {
            bool showPage = m_showPage;
            m_showPage = true;
            m_layerPoints.ShowPage(bookmark);
            m_layerRoutes.ShowPage(bookmark);
            m_layerMarked.ShowPage(bookmark);
            TrailSelector.ShowPage = true;
            ResultList.ShowPage = true;
            MultiCharts.ShowPage = true;
#if !ST_2_1
            //Avoid reregistering
            if (!showPage)
            {
                 m_view.RouteSelectionProvider.SelectedItemsChanged += new EventHandler(RouteSelectionProvider_SelectedItemsChanged);
            }
#endif
        }

        //Point selected/updated on map, Notify those interested
        //Could use listeners...
        public void UpdatePointFromMap(TrailGPSLocation point)
        {
            TrailSelector.UpdatePointFromMap(point);
        }

        public void RefreshControlState() 
        {
            ResultList.RefreshControlState();
            TrailSelector.RefreshControlState();
        }

        public void RefreshData()
        {
            bool showPage = m_showPage;
            HidePage(); //defer updates
            m_controller.Clear();

            //Initial calculation, to get progressbar
            System.Windows.Forms.ProgressBar progressBar = this.StartProgressBar(Data.TrailData.AllTrails.Values.Count * m_controller.Activities.Count);
            IList<ActivityTrail> temp = m_controller.GetOrderedTrails(progressBar, false);
            this.StopProgressBar();

            //Update list first, so not refresh changes selection
            ResultList.RefreshList();
            RefreshRoute();
            //Charts are refreshed when list is changed, no need for RefreshChart();
            if (showPage)
            {
                ShowPage("");
            }
        }

        public void RefreshChart()
        {
            MultiCharts.RefreshChart();
        }
        public IList<TrailResult> SelectedItems
        {
            get
            {
                return this.ResultList.SelectedItems;
            }
            //set { this.ResultList.SelectedItems = value; }
        }

        public System.Windows.Forms.ProgressBar StartProgressBar(int val)
        {
            return ResultList.StartProgressBar(val);
        }
        public void StopProgressBar()
        {
            ResultList.StopProgressBar();
        }

        private void RefreshRoute()
        {
            //Refreh map when visibe
            //for reports view, also the separate Map could be updated
            if ((!m_isExpanded || isReportView)
                && m_controller.CurrentActivityTrail != null)
            {
                //m_layerPoints.HighlightRadius = m_controller.CurrentActivityTrail.Trail.Radius;

                IList<TrailGPSLocation> points = new List<TrailGPSLocation>();
                //route
                foreach (TrailGPSLocation point in m_controller.CurrentActivityTrail.Trail.TrailLocations)
                {
                    points.Add(point);
                }
                m_layerPoints.TrailPoints = points;

                IDictionary<string, MapPolyline> routes = new Dictionary<string, MapPolyline>();
                //check for TrailOrdered - displayed status
                if (m_controller.CurrentActivityTrailIsDisplayed)
                {
                    IList<TrailResult> results = m_controller.CurrentActivityTrail.ParentResults;
                    bool showAll = !Data.Settings.ShowOnlyMarkedOnRoute;
                    if (!showAll)
                    {
                        if (this.ResultList.SelectedItems == null)
                        {
                            showAll = true;
                        }
                        else
                        {
                            foreach (TrailResult tr in this.ResultList.SelectedItems)
                            {
                                if (tr is SummaryTrailResult)
                                {
                                    showAll = true;
                                    break;
                                }
                            }
                        }
                    }
                    foreach (TrailResult tr in results)
                    {
                        if (showAll || this.ResultList.SelectedItems.Contains(tr))
                        {
                            //Do not map activities displayed already by ST
                            if (!ViewSingleActivity(tr.Activity))
                            {
                                //Note: Possibly limit no of Trails shown, it slows down Gmaps some
                                foreach (TrailMapPolyline m in TrailMapPolyline.GetTrailMapPolyline(tr))
                                {
                                    m.Click += new MouseEventHandler(mapPoly_Click);
                                    routes.Add(m.key, m);
                                }
                            }
                        }
                    }
                }
                m_layerMarked.MarkedTrailRoutesNoShow = new Dictionary<string, MapPolyline>();
                m_layerMarked.MarkedTrailRoutes = new Dictionary<string, MapPolyline>();
                m_layerMarked.ClearOverlays();
                m_layerRoutes.TrailRoutes = routes;
            }
        }

        public IList<IActivity> ViewActivities
        {
            get
            {
                return CollectionUtils.GetAllContainedItemsOfType<IActivity>(m_view.SelectionProvider.SelectedItems);
            }
        }
        //Try to find if ST is mapping a certain activity
        public bool ViewSingleActivity(IActivity activity)
        {
            return activity == CollectionUtils.GetSingleItemOfType<IActivity>(m_view.SelectionProvider.SelectedItems);
        }

        //Some views like mapping is only working in single view - there are likely better tests
//        public bool isSingleView
//        {
//            get
//            {
//#if !ST_2_1
//                if (CollectionUtils.GetSingleItemOfType<IActivity>(m_view.SelectionProvider.SelectedItems) == null)
//                {
//                    return false;
//                }
//#endif
//                return true;
//            }
//        }
        private bool isReportView
        {
            get
            {
            bool result = false;
#if !ST_2_1
            if (m_view.GetType().Name == "ActivityReportDetailsPage")
            //if (m_view.Id == GUIDs.ReportView)
            { 
                result = true;
            }
#endif
            return result;
            }
        }

        /*************************************************************************************************************/
        public int SetResultListHeight
        {
            get
            {
                return this.LowerSplitContainer.SplitterDistance;
            }
            set
            {
                this.LowerSplitContainer.SplitterDistance = value;
            }
        }
        
        public void MarkTrack(IList<TrailResultMarked> atr)
        {
            MarkTrack(atr, true);
        }

        public void MarkTrack(IList<TrailResultMarked> atr, bool markChart)
        {
#if !ST_2_1
            if (m_showPage)
            {
                IList<TrailResultMarked> atrST = new List<TrailResultMarked>();
                IDictionary<string, MapPolyline> mresult = new Dictionary<string, MapPolyline>();
                IDictionary<string, MapPolyline> marked = new Dictionary<string, MapPolyline>();
                foreach (TrailResultMarked trm in atr)
                {
                    if (m_view != null &&
                        //m_view.RouteSelectionProvider != null &&
                      ViewSingleActivity(trm.trailResult.Activity))
                    {
                        //One ST activity is drawn with standard methods
                        //Use ST standard display of track where possible
                        atrST.Add(trm);
                        foreach (TrailMapPolyline m in TrailMapPolyline.GetTrailMapPolyline(trm.trailResult, trm.selInfo))
                        {
                            if (!marked.ContainsKey(m.key))
                            {
                                marked.Add(m.key, m);
                            }
                        }
                    }
                    else
                    {
                        //Trails internal display of tracks
                        foreach (TrailMapPolyline m in TrailMapPolyline.GetTrailMapPolyline(trm.trailResult, trm.selInfo))
                        {
                            if (!mresult.ContainsKey(m.key))
                            {
                                m.Click += new MouseEventHandler(mapPoly_Click);
                                mresult.Add(m.key, m);
                            }
                        }
                    }
                }
                //Trails track display update
                if (Data.Settings.ShowOnlyMarkedOnRoute)
                {
                    this.RefreshRoute();
                }
                m_layerMarked.MarkedTrailRoutesNoShow = marked;
                m_layerMarked.MarkedTrailRoutes = mresult;

                //ST internal marking, use common marking
                //Only one activity, OK to merge selections on one track
                TrailsItemTrackSelectionInfo result = TrailResultMarked.SelInfoUnion(atrST);
                //Deactivate ST callback
                m_view.RouteSelectionProvider.SelectedItemsChanged -= new EventHandler(RouteSelectionProvider_SelectedItemsChanged);
                m_view.RouteSelectionProvider.SelectedItems = TrailsItemTrackSelectionInfo.SetAndAdjustFromSelection(new IItemTrackSelectionInfo[] { result }, null, false);
                m_view.RouteSelectionProvider.SelectedItemsChanged += new EventHandler(RouteSelectionProvider_SelectedItemsChanged);

                if (marked != null && marked.Count > 0 || 
                    mresult != null && mresult.Count > 0)
                //if (atr != null && atr.Count > 0 || atrST.Count > 0)
                {
                    if (Data.Settings.ZoomToSelection)
                    {
                        this.m_layerMarked.DoZoomMarkedTracks();
                    }
                    else
                    {
                        this.m_layerMarked.SetLocationMarkedTracks();
                    }
                }
                else if (Data.Settings.ShowOnlyMarkedOnRoute)
                {
                    this.m_layerRoutes.DoZoom();
                }
                else if (!markChart)
                {
                    this.m_layerPoints.DoZoom();
                }

                //Mark chart
                if (markChart)
                {
                    MultiCharts.SetSelectedRegions(atr);
                }
            }
#endif
        }

        private TrailResult m_currentSelectedMapResult = null;
        private IGPSLocation m_currentSelectedMapLocation = null;
        private IList<TrailResultMarked> m_currentSelectedMapRanges = new List<TrailResultMarked>();
        void mapPoly_Click(object sender, MouseEventArgs e)
        {
            if (sender is TrailMapPolyline)
            {
                TrailMapPolyline m = sender as TrailMapPolyline;
                if (m_currentSelectedMapResult != m.TrailRes)
                {
                    IList<TrailResult> result = new List<TrailResult> { m.TrailRes };
                    this.EnsureVisible(result, false);
                    //Could be new selection start
                    m_currentSelectedMapLocation = null;
                    m_currentSelectedMapRanges.Clear();
                    m_layerMarked.MarkedTrailRoutes = new Dictionary<string, MapPolyline>();
                }
                m_currentSelectedMapResult = m.TrailRes;

                bool sectionFound = false;
                //Use pixels to get radius from zoom level, to get click limit. (Tests indicate the limit is about 7 pixels.) Affects pass-by trail detection too
                float radius = Math.Max(1, m_layerMarked.getRadius(10));
                IGPSLocation egps = m_layerRoutes.GetGps(e.Location);

                if (m_currentSelectedMapLocation != null)
                {
                    IList<TrailResultInfo> trailResults = new List<TrailResultInfo>();
                    IList<IGPSLocation> trailgps = new List<IGPSLocation>{ m_currentSelectedMapLocation, egps };
                    TrailOrderStatus status;
                    status = ActivityTrail.GetTrailResultInfo(m.TrailRes.Activity, trailgps, radius, true, trailResults);

                    if (status == TrailOrderStatus.Match)
                    {
                        DateTime t1 = trailResults[0].Points[0].Time;
                        DateTime t2 = trailResults[0].Points[1].Time;
                        if (t1 > t2)
                        {
                            DateTime t3 = t1;
                            t1 = t2;
                            t2 = t3;
                        }
                        ValueRange<DateTime> time = new ValueRange<DateTime>(t1, t2);
                        if (m_currentSelectedMapRanges.Count == 0)
                        {
                            IValueRangeSeries<DateTime> t = new ValueRangeSeries<DateTime>();
                            t.Add(time);
                            TrailResultMarked trm = new TrailResultMarked(m.TrailRes, t);
                            m_currentSelectedMapRanges.Add(trm);
                        }
                        else
                        {
                            m_currentSelectedMapRanges[0].selInfo.MarkedTimes.Add(time);
                        }
                        MultiCharts.SetSelectedRange(new List<IItemTrackSelectionInfo> { m_currentSelectedMapRanges[0].selInfo });
                        MarkTrack(m_currentSelectedMapRanges, true);
                        sectionFound = true;
                        m_currentSelectedMapLocation = null;
                    }
                    else
                    {
                        //Should be status message somehow here
                        //Debug where points are clicked
                        m_layerPoints.TrailPoints = Trail.TrailGpsPointsFromGps(trailgps);
                    }
                }

                //Show position on chart at "first click" or section not found
                if (!sectionFound)
                {
                    TrailResultPoint t = ActivityTrail.GetClosestMatch(m.TrailRes.Activity, egps, radius);
                    if (t != null)
                    {
                        //Use new section only when match found
                        //if point was found first time but no "trail" with two points, still replace to not "get lost in bad point"
                        m_currentSelectedMapLocation = egps;

                        //get position, set a 1s range, mark on chart
                        TrailsItemTrackSelectionInfo sel = new TrailsItemTrackSelectionInfo();
                        sel.SelectedTime = new ValueRange<DateTime>(t.Time, t.Time);
                        sel.Activity = m.TrailRes.Activity;
                        MultiCharts.SetSelectedRange(new List<IItemTrackSelectionInfo> { sel });
                    }
                    else
                    {
                        //Should be status message somehow here
                        //Debug where points are clicked
                        m_layerPoints.TrailPoints = Trail.TrailGpsPointsFromGps(new List<IGPSLocation> { egps });
                    }
                }
            }
        }

        public void EnsureVisible(IList<TrailResult> atr, bool chart)
        {
            ResultList.EnsureVisible(atr);
            if (chart)
            {
                MultiCharts.EnsureVisible(atr);
            }
        }
        public void SetSelectedRegions(IList<TrailResultMarked> atr)
        {
            MultiCharts.SetSelectedRegions(atr);
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
                return null;
                //throw new Exception("Daily Activity Splitter not found");
			}
		}
#endif

        private void btnExpand_Click(object sender, EventArgs e)
        {
            this.LowerSplitContainer.Panel2.Controls.Remove(this.MultiCharts);
#if !ST_2_1
            this.ExpandSplitContainer.Panel2.Controls.Add(this.MultiCharts);
#else
            SplitContainer sc = DailyActivitySplitter;
            if (sc != null)
            {
#endif
            int width = this.UpperSplitContainer.Width;

            LowerSplitContainer.Panel2Collapsed = true;
#if ST_2_1
                if (sc.Panel2.Controls != null && sc.Panel2.Controls.Count<=1)
                {
                    sc.Panel2.Controls.Add(this.MultiCharts);
                }
                SplitterPanel p2 = DailyActivitySplitter.Panel2;
                sc.Panel2.Controls[0].Visible = false;
                MultiCharts.Width = p2.Width;
                MultiCharts.Height = p2.Height;
#else
            m_DetailPage.PageMaximized = true;
            this.ExpandSplitContainer.Panel2Collapsed = false;
            this.ExpandSplitContainer.SplitterDistance = width;
#endif
            m_isExpanded = true;
            MultiCharts.Expanded = m_isExpanded;
#if ST_2_1
 		    }
#endif
        }
		private void MultiCharts_Collapse(object sender, EventArgs e)
        {
#if !ST_2_1
            this.ExpandSplitContainer.Panel2.Controls.Remove(this.MultiCharts);
#endif
            this.LowerSplitContainer.Panel2.Controls.Add(this.MultiCharts);
            
            LowerSplitContainer.Panel2Collapsed = false;
#if ST_2_1
            SplitContainer sc = DailyActivitySplitter;
            if (sc != null)
            {
                sc.Panel2.Controls[0].Visible = true;
                if (sc.Panel2.Controls != null && sc.Panel2.Controls.Count > 0)
                {
                    sc.Panel2.Controls.Remove(this.MultiCharts);
                }
            }
#else
            this.ExpandSplitContainer.Panel2Collapsed = true;
            m_DetailPage.PageMaximized = false;
#endif
            m_isExpanded = false;
            MultiCharts.Expanded = m_isExpanded;
        }
        
#if !ST_2_1
        void RouteSelectionProvider_SelectedItemsChanged(object sender, EventArgs e)
        {
            if (sender is ISelectionProvider<IItemTrackSelectionInfo>)
            {
                m_currentSelectedMapResult = null; //new result set
                m_layerMarked.MarkedTrailRoutes = new Dictionary<string, MapPolyline>();
                //m_view.RouteSelectionProvider.SelectedItems
                ISelectionProvider<IItemTrackSelectionInfo> selected = sender as ISelectionProvider<IItemTrackSelectionInfo>;
                if (selected != null)
                {
                    IList<IItemTrackSelectionInfo> selectedGPS = TrailsItemTrackSelectionInfo.SetAndAdjustFromSelection(selected.SelectedItems, this.ViewActivities, true);
                    MultiCharts.SetSelectedRange(selectedGPS);
                }
            }
        }
#endif
    }
}
