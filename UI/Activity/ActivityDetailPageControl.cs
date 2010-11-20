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
		private bool m_isExpanded = false;

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

            this.ExpandSplitContainer.Panel2Collapsed = true;

#if ST_2_1
            TrailSelector.SetTrailSelectorControl(this, m_controller, m_layer);
            ResultList.SetResultListControl(this, m_controller);
            SingleChart.SetSingleChartsControl(this, m_controller);
#else
            TrailSelector.SetTrailSelectorControl(this, m_controller, m_view, m_layer);
            ResultList.SetResultListControl(this, m_controller, m_view);
            SingleChart.SetSingleChartsControl(this, m_controller, m_view);
#endif
            this.MultiCharts.DetailPage = this;
#if ST_2_1
			SplitContainer sc = DailyActivitySplitter;
            if (sc != null)
            {
               sc.Panel2.Controls.Add(MultiCharts);
            }
#endif
            MultiCharts.ShowChartToolBar = ShowChartToolBar;

            //summaryList.RefreshColumns();
		}

        public void UICultureChanged(CultureInfo culture)
        {
            m_culture = culture;

            this.TrailSelector.UICultureChanged(culture);
            this.ResultList.UICultureChanged(culture);
            this.SingleChart.UICultureChanged(culture);
            this.MultiCharts.UICultureChanged(culture);
        }
        public void ThemeChanged(ITheme visualTheme)
        {
            m_visualTheme = visualTheme;
            TrailSelector.ThemeChanged(visualTheme);
            ResultList.ThemeChanged(visualTheme);
            SingleChart.ThemeChanged(visualTheme);
            MultiCharts.ThemeChanged(visualTheme);
        }

        public IList<IActivity> Activities
        {
            set
            {
                m_controller.Activities = value;
#if !ST_2_1
                m_layer.ClearOverlays();
#endif
                ResultList.RefreshColumns();
                RefreshData();
                RefreshControlState();
            }
        }

        private bool _showPage = false;
        public bool ShowPage
        {
            get { return _showPage; }
            set
            {
                _showPage = value;
                m_layer.ShowPage = value;
                TrailSelector.ShowPage = value;
                ResultList.ShowPage = value;
                SingleChart.ShowPage = value;
                MultiCharts.ShowPage = value;
#if !ST_2_1
                if (value)
                {
                    m_view.RouteSelectionProvider.SelectedItemsChanged += new EventHandler(RouteSelectionProvider_SelectedItemsChanged);
                }
                else
                {
                    m_view.RouteSelectionProvider.SelectedItemsChanged -= new EventHandler(RouteSelectionProvider_SelectedItemsChanged);
                }
#endif
            }
        }

		public void RefreshControlState() 
        {
            ResultList.RefreshControlState();
            TrailSelector.RefreshControlState();
        }

        public void RefreshData()
        {
            bool showPage = _showPage;
            ShowPage = false; //defer updates
            //Update list first, so not refresh changes selection
            ResultList.RefreshList();
            RefreshRoute(); 
            RefreshChart();
            ShowPage = showPage;
        }
        public void RefreshChart()
        {
            if(m_isExpanded) {
                IList<TrailResult> list = this.SelectedItems;
                if (list.Count > 0)
                {
                    MultiCharts.RefreshCharts(list[0]);
                }
            }
            else
            {
                SingleChart.RefreshChart();
            }
        }
        public IList<TrailResult> SelectedItems
        {
            get
            {
                return this.ResultList.SelectedItems;
            }
            set { this.ResultList.SelectedItems = value; }
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

        public bool ShowChartToolBar
        {
            get { return SingleChart.ShowChartToolBar; }
            set
            {
                SingleChart.ShowChartToolBar = value;
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

        /*************************************************************************************************************/
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
                    ResultList.SelectedItems = new List<TrailResult> { tm.TrailRes };
                }
                else
                {
                    ResultList.SelectedItems = TrailResult.TrailResultList(tm.TrailRes.Activity);
                }
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
#if ST_2_1
            SplitContainer sc = DailyActivitySplitter;
            if (sc != null)
            {
#endif
            MultiCharts.Visible = true;
            MultiCharts.ShowPage = _showPage;
            SingleChart.Visible = false;
            SingleChart.ShowPage = false;

            LowerSplitContainer.Panel2Collapsed = true;
#if ST_2_1
                if (sc.Panel2.Controls != null && sc.Panel2.Controls.Count==0)
                {
                    sc.Panel2.Controls.Add(MultiCharts);
                }
                SplitterPanel p2 = DailyActivitySplitter.Panel2;
                sc.Panel2.Controls[0].Visible = false;
                MultiCharts.Width = p2.Width;
                MultiCharts.Height = p2.Height;
#else
            m_DetailPage.PageMaximized = true;
            this.ExpandSplitContainer.Panel2Collapsed = false;
            this.ExpandSplitContainer.SplitterDistance = this.Width;
#endif
            m_isExpanded = true;
            RefreshChart();
#if ST_2_1
 		}
#endif
        }
		private void MultiCharts_Collapse(object sender, EventArgs e)
        {
            MultiCharts.Visible = false;
            MultiCharts.ShowPage = false;
            SingleChart.Visible = true;
            SingleChart.ShowPage = _showPage;
            
            LowerSplitContainer.Panel2Collapsed = false;
#if ST_2_1
            SplitContainer sc = DailyActivitySplitter;
            if (sc != null)
            {
                sc.Panel2.Controls[0].Visible = true;
            }
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
                    this.SingleChart.SetSelected(selected.SelectedItems);
                    MultiCharts.SetSelected(selected.SelectedItems);
                }
            }
        }
#endif
    }
}
