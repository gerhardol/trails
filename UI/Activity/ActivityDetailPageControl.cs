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
using System.Drawing;
using System.ComponentModel;

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

        //If chart is expanded, route is hidden too
        private bool m_isChartExpanded = false;

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

        public bool IsPopup;
        private System.Windows.Forms.Form popupForm;
        private TrailResult m_currentSelectedMapResult = null;
        private IGPSLocation m_currentSelectedMapLocation = null;
        //Marked on Trails marking
        private IList<TrailResultMarked> m_currentSelectedMapRanges = new List<TrailResultMarked>(); //Note that Count is 0 or 1 now
        //Selected on ST drawn activities
        private IList<IItemTrackSelectionInfo> m_currentItemTrackSelected = new List<IItemTrackSelectionInfo>();
        internal IList<TrailResultMarked> m_lastMarkedResult = null; //Fix: save last part of a result marked

#if ST_2_1
        public ActivityDetailPageControl()
        {
#else
#endif
        public ActivityDetailPageControl(IDetailPage detailPage, IDailyActivityView view)
            : this(detailPage, view, false)
        { }

        private ActivityDetailPageControl(IDetailPage detailPage, IDailyActivityView view, bool isPopUp)
        {
            m_DetailPage = detailPage;
            m_view = view;
            IsPopup = isPopUp;

            m_layerPoints = TrailPointsLayer.InstancePoints(this, view);
            m_layerRoutes = TrailPointsLayer.InstanceRoutes(this, view);
            m_layerMarked = TrailPointsLayer.InstanceMarked(this, view);

            this.InitializeComponent();
            InitControls();
        }

        public ActivityDetailPageControl(IDailyActivityView view)
            : this(null, view, true)
        {
            if (Data.Settings.PopupUpdatedBySelection)
            {
                view.SelectionProvider.SelectedItemsChanged += new EventHandler(popupForm_OnViewSelectedItemsChanged);
            }

            //Theme and Culture must be set manually
            this.ThemeChanged(m_visualTheme);
            this.UICultureChanged(m_culture);
            this.ShowDialog();
            ShowPage("");
        }

        public void ShowDialog()
        {
            popupForm = new Form();
            popupForm.Size = Data.Settings.PopupSize;
            popupForm.Controls.Add(this);

            popupForm.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(Parent.Size.Width - 17, Parent.Size.Height - 38);
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom)));
            popupForm.Icon = Icon.FromHandle(Properties.Resources.trails.GetHicon());
            popupForm.Text = Properties.Resources.PluginTitle;

            popupForm.SizeChanged += new EventHandler(popupForm_SizeChanged);
            popupForm.FormClosed += new FormClosedEventHandler(popupForm_FormClosed);
            this.LowerSplitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(LowerSplitContainer_SplitterMoved);

            popupForm.Show();
        }

        void InitControls()
        {
#if !ST_2_1
            this.ExpandSplitContainer.Panel2Collapsed = true;
#endif

            TrailSelector.SetControl(this, m_view, m_layerPoints);
            ResultList.SetControl(this, m_view);
            MultiCharts.SetControl(this, m_view);
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
                //Reselecting activities updates calculations
                System.Windows.Forms.ProgressBar progressBar = this.StartProgressBar(0);
                Controller.TrailController.Instance.SetActivities(value, false, progressBar);

                this.StopProgressBar();
#if !ST_2_1
                m_layerPoints.ClearOverlays();
                m_layerRoutes.ClearOverlays();
                m_layerMarked.ClearOverlays();
#endif
                RefreshData(false);
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
            return HidePage(true);
        }

        public bool HidePage(bool resetData)
        {
            m_showPage = false;
#if !ST_2_1
            m_view.RouteSelectionProvider.SelectedItemsChanged -= new EventHandler(RouteSelectionProvider_SelectedItemsChanged);
#endif
            m_layerPoints.HidePage();
            m_layerRoutes.HidePage();
            m_layerMarked.HidePage();
            ResultList.ShowPage = false; //No action
            MultiCharts.ShowPage = false; //No action
            if (resetData)
            {
                TrailSelector.ShowPage = false; //hide edit dialog if shown
                //Free memory by destroying all results
                Controller.TrailController.Instance.Reset();
                ActivityCache.ClearActivityCache();
            }
            return true;
        }

        public void ShowPage(string bookmark)
        {
            bool showPage = m_showPage;
            m_showPage = true;
            TrailSelector.ShowPage = true; //No action
            ResultList.ShowPage = true; //No action
            MultiCharts.ShowPage = true; //Refresh chart
            //Refresh the map overlays already set
            m_layerPoints.ShowPage(bookmark);
            m_layerRoutes.ShowPage(bookmark);
            m_layerMarked.ShowPage(bookmark);
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
            MultiCharts.RefreshChartMenu();
        }

        //Refresh the data displayed, recalculate if needed
        public void RefreshData(bool clearResults)
        {
            RefreshData(clearResults, false);
        }

        public void RefreshData(bool clearResults, bool reCalcTrails)
        {
            bool showPage = m_showPage;
            m_lastMarkedResult = null;
            HidePage(false); //defer updates
            if (clearResults)
            {
                //The trail results will be cleared
                Controller.TrailController.Instance.Clear(false);
            }
            if (reCalcTrails)
            {
                Controller.TrailController.Instance.Reset();
            }

            //Calculate results (not needed explicitly)
            Controller.TrailController.Instance.ReCalcTrails(reCalcTrails, null);
            ResultList.RefreshList();
            //Data could be updated and reference changed
            MultiCharts.ReferenceTrailResult = Controller.TrailController.Instance.ReferenceTrailResult;
            RefreshRoute(true);
            //Charts are refreshed in ShowPage, no need for RefreshChart();
            if (showPage)
            {
                ShowPage("");
            }
        }

        public void RefreshChart()
        {
            MultiCharts.ReferenceTrailResult = Controller.TrailController.Instance.ReferenceTrailResult;
            MultiCharts.RefreshChart();
        }

        public void RefreshSummary()
        {
            ResultList.RefreshSummary();
        }

        public void ShowListToolBar()
        {
            ResultList.ShowListToolBar();
        }

        public void selectSimilarSplitsChanged()
        {
            ResultList.selectSimilarSplits();
        }
        //public IList<TrailResult> SelectedItems
        //{
        //    get
        //    {
        //        return this.ResultList.SelectedItems;
        //    }
        //}

        public System.Windows.Forms.ProgressBar StartProgressBar(int val)
        {
            return ResultList.StartProgressBar(val);
        }

        public void StopProgressBar()
        {
            ResultList.StopProgressBar();
        }

        public void RefreshRouteCheck()
        {
            if (Data.Settings.ShowOnlyMarkedOnRoute)
            {
                RefreshRoute(false);
            }
        }

        public void RefreshRoute(bool zoomOrVisible)
        {
            //Refreh map when visible
            //for reports view, also the separate Map could be updated
            if ((!m_isChartExpanded || isReportView)
                && Controller.TrailController.Instance.CurrentActivityTrailIsSelected)
            {
                //m_layerPoints.HighlightRadius = Controller.TrailController.Instance.CurrentActivityTrail.Trail.Radius;

                //Clear marked routes layer (set separately, runs after) 
                m_layerMarked.MarkedTrailRoutesNoShow = new Dictionary<string, MapPolyline>();
                m_layerMarked.MarkedTrailRoutes = new Dictionary<string, MapPolyline>();
                m_layerMarked.ClearOverlays();

                //TrailPoints (SplitPoints separetly)
                IList<TrailGPSLocation> trailPoints = new List<TrailGPSLocation>();
                if (Data.Settings.ShowTrailPointsOnMap)
                {
                    foreach (ActivityTrail at in Controller.TrailController.Instance.CurrentActivityTrails)
                    {
                        foreach (TrailGPSLocation point in at.Trail.TrailLocations)
                        {
                            trailPoints.Add(point);
                        }
                    }
                }
                m_layerPoints.TrailPoints = trailPoints;

                IDictionary<string, MapPolyline> routes = new Dictionary<string, MapPolyline>();
                IList<SplitGPSLocation> splitPoints = new List<SplitGPSLocation>();
                IList<TrailResultWrapper> results;
                IList<TrailResultWrapper> selected;
                if (Data.Settings.ShowOnlyMarkedOnRoute)
                {
                    selected = Controller.TrailController.Instance.SelectedResults;
                }
                else
                {
                    selected = Controller.TrailController.Instance.Results;
                }
                if (SummaryTrailResult.IsSummarySelection(selected))
                {
                    //Show all results also with ShowOnlyMarkedOnRoute
                    results = TrailResultWrapper.UnpausedResults(Controller.TrailController.Instance.Results);
                }
                else
                {
                    //There are results other than Summary, show them
                    results = new List<TrailResultWrapper>();
                    foreach (TrailResultWrapper tr in selected)
                    {
                        if (!(tr.Result is SummaryTrailResult))
                        {
                            results.Add(tr);
                        }
                    }
                }

                //Result related - routes and splitpoints
                foreach (TrailResultWrapper tr in results)
                {
                    //Do not map routes displayed already by ST (unless GPS filter)
                    if (ViewSingleActivity() != tr.Result.Activity || Data.Settings.UseGpsFilter)
                    {
                        //Note: Possibly limit no of Trails shown, it slows down Gmaps some
                        foreach (TrailMapPolyline m in TrailMapPolyline.GetTrailMapPolyline(tr.Result))
                        {
                            m.Click += new MouseEventHandler(mapPoly_Click);
                            if (!routes.ContainsKey(m.key))
                            {
                                routes.Add(m.key, m);
                            }
                        }
                    }
                    if (Data.Settings.ShowTrailPointsOnMap)
                    {
                        if (tr.Result.Trail.TrailType != Trail.CalcType.TrailPoints)
                        {
                            foreach (TrailResultPoint tp in tr.Result.SubResultInfo.Points)
                            {
                                SplitGPSLocation tl = new SplitGPSLocation(tp, tr.Result.ResultColor.FillSelected);
                                splitPoints.Add(tl);
                            }
                        }
                    }
                }

                m_layerPoints.SplitPoints = splitPoints;
                m_layerRoutes.TrailRoutes = routes;

                if (zoomOrVisible)
                {
                    if (Data.Settings.ZoomToSelection)
                    {
                        m_layerRoutes.DoZoom();
                    }
                    else
                    {
                        //Make sure routes are visible
                        this.m_layerRoutes.EnsureVisible();
                    }
                }
            }
        }

        public IList<IActivity> ViewActivities
        {
            get
            {
                return CollectionUtils.GetAllContainedItemsOfType<IActivity>(m_view.SelectionProvider.SelectedItems);
            }
        }

        //Find the activity ST maps
        public IActivity ViewSingleActivity()
        {
            return (m_view == null) ? null : CollectionUtils.GetSingleItemOfType<IActivity>(m_view.SelectionProvider.SelectedItems);
        }

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

        public void MarkTrack(IList<TrailResultMarked> atr, bool markChart, bool zoom)
        {
#if !ST_2_1
            if (m_showPage)
            {
                IList<TrailResultMarked> atrST = new List<TrailResultMarked>();
                IDictionary<string, MapPolyline> mresult = new Dictionary<string, MapPolyline>();
                IDictionary<string, MapPolyline> marked = new Dictionary<string, MapPolyline>();

                foreach (TrailResultMarked trm in atr)
                {
                    m_lastMarkedResult = atr;
                    if (ViewSingleActivity() == trm.trailResult.Activity)
                    {
                        //One ST activity is drawn with standard methods
                        //Use ST standard display of track where possible
                        atrST.Add(trm);
                        foreach (TrailMapPolyline m in TrailMapPolyline.GetTrailMapMarkedPolyline(trm.trailResult, trm.selInfo))
                        {
                            if (!marked.ContainsKey(m.key))
                            {
                                marked.Add(m.key, m);
                            }
                        }
                    }
                    else
                    {
                        //Trails display of tracks
                        foreach (TrailMapPolyline m in TrailMapPolyline.GetTrailMapMarkedPolyline(trm.trailResult, trm.selInfo))
                        {
                            if (!mresult.ContainsKey(m.key))
                            {
                                m.Click += new MouseEventHandler(mapPoly_Click);
                                mresult.Add(m.key, m);
                            }
                        }
                    }
                }
                m_layerMarked.MarkedTrailRoutesNoShow = marked;
                m_layerMarked.MarkedTrailRoutes = mresult;

                RouteSelectionProvider_SelectedItemsUpdate(atrST);

                if (zoom && Data.Settings.ZoomToSelection)
                {
                    this.m_layerMarked.DoZoom();
                }
                else
                {
                    //Make sure it is visible
                    this.m_layerMarked.EnsureVisible();
                }

                //Mark chart, normally done from ResultList
                if (markChart)
                {
                    TrailResultMarked range = null;
                    if (atr.Count > 0)
                    {
                        range = atr[atr.Count - 1];
                    }
                    MultiCharts.SetSelectedResultRegions(atr, range);
                }
            }
#endif
        }

        public void ZoomMarked()
        {
            this.m_layerMarked.DoZoom();
        }

        public void ClearCurrentSelectedOnRoute()
        {
            m_currentSelectedMapLocation = null;
            m_currentSelectedMapRanges.Clear();
            this.m_currentItemTrackSelected.Clear();
            m_layerMarked.MarkedTrailRoutes = new Dictionary<string, MapPolyline>();
#if DEBUG_CLICKED_ROUTES
            this.m_layerMarked.TrailPoints = new List<TrailGPSLocation>(); ; //Debug finding clicks
#endif
            RouteSelectionProvider_SelectedItemsUpdate(this.m_currentItemTrackSelected);
        }

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
                    this.ClearCurrentSelectedOnRoute();
                }
                m_currentSelectedMapResult = m.TrailRes;

                //bool sectionFound = false;
                //Use pixels to get radius from zoom level, to get click limit. (Tests indicate the limit is about 7 pixels.) Affects pass-by trail detection too
                float radius = Math.Max(1, m_layerMarked.getRadius(10));
                IGPSLocation egps = m_layerRoutes.GetGps(e.Location);

                if (m_currentSelectedMapLocation != null)
                {
                    //A previous match existed, try to find a section
                    IList<TrailResultInfo> trailResults = new List<TrailResultInfo>();
                    IList<IGPSLocation> trailgps = new List<IGPSLocation> { m_currentSelectedMapLocation, egps };
                    TrailOrderStatus status = TrailOrderStatus.NoInfo;
                    int i = 5;
                    while (status != TrailOrderStatus.Match && i-- > 0)
                    {
                        status = ActivityTrail.GetTrailResultInfo(m.TrailRes.Activity, m.TrailRes.ExternalPauses, trailgps, radius, true, trailResults);
                        radius *= 2;
                    }
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
                        TrailResultMarked markedRange = new TrailResultMarked(m.TrailRes, time);
                        MultiCharts.SetSelectedResultRegions(m_currentSelectedMapRanges, markedRange);
                        MarkTrack(m_currentSelectedMapRanges, false, false);
                        //sectionFound = true;
                        m_currentSelectedMapLocation = null;
                    }
                    else
                    {
#if DEBUG_CLICKED_ROUTES
                        //Debug where points are clicked
                        IList<TrailGPSLocation> points = Trail.TrailGpsPointsFromGps(trailgps);
                        points[0].Radius = radius/2;
                        points[0].Name = "DebugNotClickableDebug"; //Hack to avoid clicks
                        points[1].Name = "DebugNotClickableDebug";
                        this.m_layerMarked.TrailPoints = points;
#endif
                        this.MultiCharts.ShowGeneralToolTip("Failed to find section match");
                    }
                }

                //Show position on chart at "first click" or section not found
                else
                    //Disable "show on chart if not found" as this hides the tooltip too
                    //if (!sectionFound)
                {
                    TrailResultPoint t = null;
                    int i = 4; //Smaller max then for "trail" detection, max around 8m
                    while (t == null && i-- > 0)
                    {
                        t = ActivityTrail.GetClosestMatch(m.TrailRes.Activity, m.TrailRes.ExternalPauses, egps, radius);
                        radius *= 2;
                    }
                    if (t != null)
                    {
                        //Use new section only when match found
                        //if point was found first time but no "trail" with two points, still replace to not "get lost in bad point"
                        m_currentSelectedMapLocation = egps;

                        ValueRange<DateTime> time = new ValueRange<DateTime>(t.Time, t.Time);
                        TrailResultMarked markedRange = new TrailResultMarked(m.TrailRes, time);
                        MultiCharts.SetSelectedResultRegions(m_currentSelectedMapRanges, markedRange);
                    }
                    else
                    {
#if DEBUG_CLICKED_ROUTES
                        //Debug where points are clicked
                        IList<TrailGPSLocation> points = Trail.TrailGpsPointsFromGps(new List<IGPSLocation> { egps });
                        points[0].Radius = radius/2;
                        points[0].Name = "DebugNotClickableDebug"; //Hack to avoid clicks
                        this.m_layerMarked.TrailPoints = points;
#endif
                        this.MultiCharts.ShowGeneralToolTip("Failed to find match on route");
                    }
                }
            }
        }

        //Ensure that activities are visible in list and chart (but not on route, handled when marking)
        public void EnsureVisible(IList<TrailResult> atr, bool chart)
        {
            ResultList.EnsureVisible(atr);
            if (chart)
            {
                MultiCharts.EnsureVisible(atr);
            }
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
            if (!this.IsPopup)
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
                m_isChartExpanded = true;
                MultiCharts.Expanded = m_isChartExpanded;
#if ST_2_1
             }
#endif
            }
        }

        private void MultiCharts_Collapse(object sender, EventArgs e)
        {
            if (!this.IsPopup)
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
                m_isChartExpanded = false;
                MultiCharts.Expanded = m_isChartExpanded;
            }
        }

        public void ResultList_Expand(bool actPage)
        {
            this.LowerSplitContainer.Panel2.Controls.Remove(this.MultiCharts);
            this.ExpandSplitContainer.Panel2.Controls.Remove(this.MultiCharts);

            LowerSplitContainer.Panel2Collapsed = true;
            this.ExpandSplitContainer.Panel2Collapsed = true;
            if (actPage && m_DetailPage !=null)
            {
                m_DetailPage.PageMaximized = true;
            }
            MultiCharts.ChartVisible = false;
            MultiCharts.ShowPage = false;
            if (actPage)
            {
                m_isChartExpanded = true;
                MultiCharts.Expanded = m_isChartExpanded;
            }
        }

        public void ResultList_Collapse()
        {
            this.LowerSplitContainer.Panel2.Controls.Add(this.MultiCharts);
            this.ExpandSplitContainer.Panel2.Controls.Remove(this.MultiCharts);

            LowerSplitContainer.Panel2Collapsed = false;
            this.ExpandSplitContainer.Panel2Collapsed = true;
            if (m_DetailPage != null)
            {
                m_DetailPage.PageMaximized = false;
            }
            m_isChartExpanded = false;
            MultiCharts.ChartVisible = true;
            MultiCharts.ShowPage = true;
            MultiCharts.Expanded = m_isChartExpanded;
        }

#if !ST_2_1
        void RouteSelectionProvider_SelectedItemsUpdate(IList<IItemTrackSelectionInfo> sels)
        {
            //Deactivate ST callback
            m_view.RouteSelectionProvider.SelectedItemsChanged -= new EventHandler(RouteSelectionProvider_SelectedItemsChanged);
            m_view.RouteSelectionProvider.SelectedItems = sels;
            m_view.RouteSelectionProvider.SelectedItemsChanged += new EventHandler(RouteSelectionProvider_SelectedItemsChanged);
        }

        void RouteSelectionProvider_SelectedItemsUpdate(IList<TrailResultMarked> res)
        {
            IList<IItemTrackSelectionInfo> sels = TrailsItemTrackSelectionInfo.SetAndAdjustFromSelectionToST(res);
            RouteSelectionProvider_SelectedItemsUpdate(sels);
        }

        void RouteSelectionProvider_SelectedItemsChanged(object sender, EventArgs e)
        {
            if (sender is ISelectionProvider<IItemTrackSelectionInfo>)
            {
                ISelectionProvider<IItemTrackSelectionInfo> selected = sender as ISelectionProvider<IItemTrackSelectionInfo>;

                TrailResult trSel = null; //(one of) the selected results, used to check changes
                if (selected != null)
                {
                    //Note: All results, not just the displayed
                    IList<TrailResultWrapper> t = Controller.TrailController.Instance.Results;
                    foreach (IItemTrackSelectionInfo sel in selected.SelectedItems)
                    {
                        foreach (TrailResultWrapper tr in t)
                        {
                            if (tr.Result.Activity.ToString() == sel.ItemReferenceId)
                            {
                                trSel = tr.Result;
                                break;
                            }
                        }
                    }
                }
                //Note: the selection is occasionally zero when starting selecting
                if (m_currentSelectedMapResult != null && trSel != null && m_currentSelectedMapResult != trSel)
                {
                    IList<TrailResult> result = new List<TrailResult> { trSel };
                    this.EnsureVisible(result, false);
                    //Could be new selection start
                    this.ClearCurrentSelectedOnRoute();
                }
                m_currentSelectedMapResult = trSel;
                //Forget last clicked "multi route"
                m_currentSelectedMapLocation = null;

                if (selected != null && selected.SelectedItems.Count > 0 && trSel != null)
                {
                    IList<IActivity> activities = this.ViewActivities; //Should be ViewSingleActivity

                    IList<IItemTrackSelectionInfo> selectedGPS = TrailsItemTrackSelectionInfo.SetAndAdjustFromSelectionFromST(selected.SelectedItems, activities);
                    foreach(IItemTrackSelectionInfo t in selectedGPS)
                    {
                        //Only one item in the list
                        if(this.m_currentSelectedMapRanges.Count == 0)
                        {
                            this.m_currentSelectedMapRanges.Add(new TrailResultMarked(trSel,t));
                        }
                        else
                        {
                            this.m_currentSelectedMapRanges[0].selInfo.Union(t);
                        }
                    }
                    TrailResultMarked markedRange = null;
                    if (selectedGPS.Count > 0)
                    {
                        markedRange = new TrailResultMarked(trSel, selectedGPS[selectedGPS.Count - 1]);
                    }
                    MultiCharts.SetSelectedResultRegions(this.m_currentSelectedMapRanges, markedRange);
                    foreach (IItemTrackSelectionInfo sel in selected.SelectedItems)
                    {
                        //As the selection can be made outside the trail result, the complete activity is saved (no conversion error
                        this.m_currentItemTrackSelected.Add(sel);
                    }
                    RouteSelectionProvider_SelectedItemsUpdate(this.m_currentItemTrackSelected);
                }
            }
        }
#endif

        public void PopupAdjustSize()
        {
            this.LowerSplitContainer.SplitterDistance = Data.Settings.PopupDivider;
        }

        private void LowerSplitContainer_SplitterMoved(object sender, System.Windows.Forms.SplitterEventArgs e)
        {
            if (this.IsPopup)
            {
                Data.Settings.PopupDivider = this.LowerSplitContainer.SplitterDistance;
            }
        }

        private void popupForm_OnViewSelectedItemsChanged(object sender, EventArgs e)
        {
            this.Activities = CollectionUtils.GetAllContainedItemsOfType<IActivity>(m_view.SelectionProvider.SelectedItems);
        }

        private void popupForm_SizeChanged(object sender, EventArgs e)
        {
            if (m_showPage)
            {
                if (popupForm != null)
                {
                    Data.Settings.PopupSize = popupForm.Size;
                }
            }
        }

        void popupForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.HidePage();
        }
    }
}
