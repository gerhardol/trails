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

#if ST_2_1
//xxx

using System;
using System.Drawing;
using System.Windows.Forms;
using ZoneFiveSoftware.Common.Data.GPS;
#if ST_2_1
using ZoneFiveSoftware.Common.Visuals.Fitness.GPS;
#else
using ZoneFiveSoftware.Common.Visuals.Mapping;
#endif
using System.Collections.Generic;
using Microsoft.Win32;

namespace TrailsPlugin.UI.MapLayers {
	class MapControlLayer
#if ST_2_1
        : 
        IMapControlLayer
#else
 : RouteControlLayerBase, IRouteControlLayer
        //ST3fix This file is patched to compile in ST3 only, not working
#endif
    {
		public event System.EventHandler SelectedGPSLocationsChanged;

		private bool m_CaptureSelectedGPSLocations;
		private IList<IGPSLocation> m_SelectedGPSLocations = new List<IGPSLocation>();
		private IList<IGPSLocation> m_HighlightedGPSLocations = new List<IGPSLocation>();
		private bool m_ShowHighlight = false;
		private float m_highlightRadius;
        private static bool _showPage;

        public bool ShowPage
        {
            //get { return _showPage; }
            set
            {
                _showPage = value;
                if (!value)
                {
                    m_CaptureSelectedGPSLocations = false;
                }
            }
        }
        
        private static MapControlLayer m_instance = null;
		public static MapControlLayer Instance {
			get {
				if (MapControlLayer.m_instance == null) {
					MapControlLayer.m_instance = new MapControlLayer();
				}
				return MapControlLayer.m_instance;
            }
		}

		//private MapControlLayer() {
		//}

		private IMapControl m_mapControl;
		public IMapControl MapControl {
			set {
				m_mapControl = value;
			}
			get {
                return m_mapControl;
			}
        }

#if ST_2_1
		private IList<IGPSLocation> getSelectedGPSLocations(IMapDrawContext drawContext) {
			IList<IGPSLocation> list = new List<IGPSLocation>() { };
			IMapControl mapControl = UI.MapLayers.MapControlLayer.Instance.MapControl;
             
#if ST_2_1
			ICollection<IMapControlObject> selectedGPS = mapControl.Selected;
#else
            IList<IGPSLocation> selectedGPS = UI.MapLayers.MapControlLayer.Instance.SelectedGPSLocations;
#endif
			if (selectedGPS.Count > 0) {
#if ST_2_1
            IMapControlObject[] selectedMapControlObjects = new IMapControlObject[selectedGPS.Count];
				selectedGPS.CopyTo(selectedMapControlObjects, 0);
				for (int i = 0; i < selectedGPS.Count; i++) {
					Rectangle rec = selectedMapControlObjects[i].PixelBounds(drawContext);

					int X = rec.X + (rec.Width / 2) - (drawContext.DrawRectangle.Width / 2);
					int Y = rec.Y + (rec.Height / 2) - (drawContext.DrawRectangle.Height / 2);
					IGPSLocation loc = drawContext.Projection.PixelToGPS(drawContext.Center, drawContext.ZoomLevel, new Point(X, Y));
					list.Add(loc);
				}
#else
                //ST3fix
#endif
            }
			return list;
		}
#endif

		public void CaptureSelectedGPSLocations() {
			m_SelectedGPSLocations.Clear();
			m_CaptureSelectedGPSLocations = true;
#if ST_2_1
			if (null!=m_mapControl) m_mapControl.Refresh();
#else
            //ST3fix
#endif
		}

		public IList<IGPSLocation> SelectedGPSLocations {
			get {
				return m_SelectedGPSLocations;
			}
		}

		public IList<IGPSLocation> HighlightedGPSLocations {
			set {
				m_HighlightedGPSLocations = value;
			}
			get {
				return m_HighlightedGPSLocations;
			}
		}

		public float HighlightRadius {
			set {
				m_highlightRadius = value;
			}
		}

		public bool ShowHighlight {
			set {
				m_ShowHighlight = value;
#if ST_2_1
                if (null != m_mapControl) m_mapControl.Refresh();
#else
            //ST3fix
#endif
			}
		}


#if ST_2_1
#region IMapControlLayer Members

		public void Draw(IMapDrawContext drawContext) {
			if (m_CaptureSelectedGPSLocations) {
				m_CaptureSelectedGPSLocations = false;
#if ST_2_1
				if (m_SelectedGPSLocations.Count != MapControl.Selected.Count) {
					m_SelectedGPSLocations = getSelectedGPSLocations(drawContext);
					SelectedGPSLocationsChanged(this, new System.EventArgs());
				}
#else
                //ST3fix
#endif
            }

			if (m_ShowHighlight) {
				//drawContext.Center
	
				IGPSLocation loc1 = drawContext.Projection.PixelToGPS(drawContext.Center, drawContext.ZoomLevel, new Point(0,0));
				IGPSLocation loc2 = drawContext.Projection.PixelToGPS(drawContext.Center, drawContext.ZoomLevel, new Point(0,100));
				IGPSPoint point1 = Utils.GPS.LocationToPoint(loc1);
				IGPSPoint point2 = Utils.GPS.LocationToPoint(loc2);
				float meters = point1.DistanceMetersToPoint(point2) / 100;
				float radiusInPixels = m_highlightRadius / meters;
								
				foreach (IGPSLocation gpsLocation in m_HighlightedGPSLocations) {
					Point point = drawContext.Projection.GPSToPixel(drawContext.Center, drawContext.ZoomLevel, gpsLocation );
					Pen pen = new Pen(Color.Red, 5.0F);
					
					drawContext.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
					float X = point.X + (drawContext.DrawRectangle.Width / 2) - radiusInPixels;
					float Y = point.Y + (drawContext.DrawRectangle.Height / 2) - radiusInPixels;
					drawContext.Graphics.DrawEllipse(pen, X, Y, radiusInPixels * 2, radiusInPixels * 2);					
				}
			}
		}

        public ICollection<IMapControlObject> HitTest(Rectangle rectClient, IMapDrawContext drawContext) {
            if (_showPage)
            {
                //No select by rect in Trails page, just individual points
                throw new NotImplementedException();
            }
            else
            {
                return null;
            }
 		}

		public IMapControlObject HitTest(Point ptClient, bool bSelect, IMapDrawContext drawContext, out Cursor cursor) {
			cursor = Cursors.Default;
			return null;
		}

		public System.Guid Id {
			get {
				return GUIDs.MapControlLayer;
			}
		}

		public string Name {
			get {
				return "Click Map Layer";
			}
		}

		public string Path {
			get {
				return null;
			}
		}

#endregion

        private class MapControlObject : IMapControlObject
        {

#region IMapControlObject Members

            public void DrawHighlight(IMapDrawContext drawContext) { }

            public void DrawInfo(IMapDrawContext drawContext, System.Drawing.Rectangle infoRect, bool bSelected) { }

            public void DrawSelected(IMapDrawContext drawContext) { }

            public Size InfoSize(IMapDrawContext drawContext, bool bSelected)
            {
                return new Size();
            }

            public Rectangle PixelBounds(IMapDrawContext drawContext)
            {
                return new Rectangle();
            }

#endregion
        }
#endif
		
    }
}
#else
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals.Drawing;
using ZoneFiveSoftware.Common.Visuals.Mapping;

using TrailsPlugin.Data;

namespace TrailsPlugin.UI.MapLayers
{
    class RouteGPSTrackPointsLayer : RouteControlLayerBase, IRouteControlLayer
    {
        public RouteGPSTrackPointsLayer(IRouteControlLayerProvider provider, IRouteControl control)
            : base(provider, control, 2)
        {
            PluginMain.GetApplication().SystemPreferences.PropertyChanged += new PropertyChangedEventHandler(SystemPreferences_PropertyChanged);
           // listener = new RouteItemsDataChangeListener(control);
           // listener.PropertyChanged += new PropertyChangedEventHandler(OnRouteItemsPropertyChanged);
        }

        public IList<IGPSLocation> HighlightedGPSLocations
        {
            set
            {
                m_HighlightedGPSLocations = value;
            }
            get
            {
                return m_HighlightedGPSLocations;
            }
        }

        public float HighlightRadius
        {
            set
            {
                m_highlightRadius = value;
            }
        }

        public bool ShowHighlight
        {
            set
            {
                m_ShowHighlight = value;
                if (null != m_mapControl) m_mapControl.Refresh();
            }
        }
        protected override void OnMapControlZoomChanged(object sender, EventArgs e)
        {
            RefreshOverlays();
        }

        protected override void OnMapControlCenterMoveEnd(object sender, EventArgs e)
        {
            RefreshOverlays();
        }

        protected override void OnRouteControlResize(object sender, EventArgs e)
        {
            RefreshOverlays();
        }

        protected override void OnRouteControlVisibleChanged(object sender, EventArgs e)
        {
            if (RouteControl.Visible && routeSettingsChanged)
            {
                ClearOverlays();
                routeSettingsChanged = false;
                RefreshOverlays();
            }
        }

        protected override void OnRouteControlMapControlChanged(object sender, EventArgs e)
        {
            ClearCachedLocations();
            RefreshOverlays();
        }

        protected override void OnRouteControlItemsChanged(object sender, EventArgs e)
        {
            ClearCachedLocations();
            RefreshOverlays();
        }

        private void SystemPreferences_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null //||
/*xxx                e.PropertyName == PluginMain.GetApplication().SystemPreferences.RouteSettings_ShowGPSPoints ||
                e.PropertyName == PluginMain.GetApplication().SystemPreferences.RouteSettings_MarkerShape ||
                e.PropertyName == PluginMain.GetApplication().SystemPreferences.RouteSettings_MarkerSize ||
                e.PropertyName == PluginMain.GetApplication().SystemPreferences.RouteSettings_MarkerColor)
*/)            {
                if (RouteControl.Visible)
                {
                    RefreshOverlays();
                }
                else
                {
                    routeSettingsChanged = true;
                }
            }
        }

/*xxx        private void OnRouteItemsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (true/* xxx e.PropertyName == Activity.PropertyName.GPSRoute ||
                e.PropertyName == Route.PropertyName.GPSRoute ||
                e.PropertyName == Activity.PropertyName.Category )
            {
                ClearCachedLocations();
                RefreshOverlays();
            }
        }
        */
        private void RefreshOverlays()
        {
            if (MapControlChanged)
            {
                ClearOverlays();
                ResetMapControl();
            }

            if (!PluginMain.GetApplication().SystemPreferences.RouteSettings.ShowGPSPoints) return;

//            CalculatePointLocations();

            IGPSBounds windowBounds = MapControlBounds;

/*            IList<IGPSPoint> visibleLocations = new List<IGPSPoint>();
            foreach (IGPSPoint point in routePointsAtZoom)
            {
                if (windowBounds.Contains(point))
                {
                    visibleLocations.Add(point);
                }
            }
*/

            IDictionary<IGPSPoint, IMapOverlay> newPointOverlays = new Dictionary<IGPSPoint, IMapOverlay>();
            IList<IMapOverlay> addedOverlays = new List<IMapOverlay>();

/*xxx            foreach (IGPSPoint location in visibleLocations)
            {
                if (pointOverlays.ContainsKey(location))
                {
                    newPointOverlays.Add(location, pointOverlays[location]);
                    pointOverlays.Remove(location);
                }
                else
                {
                    MapIcon icon = MapIconCache.Instance.GetGPSPointIcon();
                    MapMarker pointOverlay = new MapMarker(location, icon, false);
                    newPointOverlays.Add(location, pointOverlay);
                    addedOverlays.Add(pointOverlay);
                }
            }
 * */
            if (m_ShowHighlight)
            {
                //drawContext.Center

                IGPSLocation loc1 = drawContext.Projection.PixelToGPS(drawContext.Center, drawContext.ZoomLevel, new Point(0, 0));
                IGPSLocation loc2 = drawContext.Projection.PixelToGPS(drawContext.Center, drawContext.ZoomLevel, new Point(0, 100));
                IGPSPoint point1 = Utils.GPS.LocationToPoint(loc1);
                IGPSPoint point2 = Utils.GPS.LocationToPoint(loc2);
                float meters = point1.DistanceMetersToPoint(point2) / 100;
                float radiusInPixels = m_highlightRadius / meters;

                foreach (IGPSLocation gpsLocation in m_HighlightedGPSLocations)
                {
                    Point point = drawContext.Projection.GPSToPixel(drawContext.Center, drawContext.ZoomLevel, gpsLocation);
                    Pen pen = new Pen(Color.Red, 5.0F);

                    drawContext.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    float X = point.X + (drawContext.DrawRectangle.Width / 2) - radiusInPixels;
                    float Y = point.Y + (drawContext.DrawRectangle.Height / 2) - radiusInPixels;
                    drawContext.Graphics.DrawEllipse(pen, X, Y, radiusInPixels * 2, radiusInPixels * 2);
                }
            }

            ClearOverlays();

            MapControl.AddOverlays(addedOverlays);
            pointOverlays = newPointOverlays;
        }

        private void ClearOverlays()
        {
            MapControl.RemoveOverlays(pointOverlays.Values);
            pointOverlays.Clear();
        }

/*        private void CalculatePointLocations()
        {
            if (routePointsZoom == MapControl.Zoom) return;

            ClearCachedLocations();
            routePointsZoom = MapControl.Zoom;

            IGPSLocation pointIntervalOffsetLocation = MapControl.MapProjection.PixelToGPS(MapControl.Center, MapControl.Zoom, new Point(20, 0));
            double pointIntervalDistanceMeters = new GPSPoint(MapControl.Center.LatitudeDegrees, MapControl.Center.LongitudeDegrees, float.NaN).DistanceMetersToPoint(
                new GPSPoint(pointIntervalOffsetLocation.LatitudeDegrees, pointIntervalOffsetLocation.LongitudeDegrees, float.NaN));

            foreach (IRouteControlItem item in RouteControl.Items)
            {
                if (item.DisplayRoute && item.Item.GPSRoute != null)
                {
                    IGPSRoute route = item.Item.GPSRoute;
                    double lastPtDistance = 0;
                    for (int pt = 0; pt < route.Count; pt++)
                    {
                        IGPSPoint point = route[pt].Value;
                        if (pt == 0 || pt == route.Count - 1 || routePointsZoom == MapControl.MaximumZoom)
                        {
                            routePointsAtZoom.Add(point);
                        }
                        else
                        {
                            double segmentDistance = point.DistanceMetersToPoint(route[pt - 1].Value);
                            if (lastPtDistance + segmentDistance >= pointIntervalDistanceMeters)
                            {
                                routePointsAtZoom.Add(point);
                                lastPtDistance = 0;
                            }
                            else
                            {
                                lastPtDistance += segmentDistance;
                            }
                        }
                    }
                }
            }
        }
*/
        private void ClearCachedLocations()
        {
            routePointsAtZoom.Clear();
            routePointsZoom = double.NaN;
        }

        private bool routeSettingsChanged = false;
        private IDictionary<IGPSPoint, IMapOverlay> pointOverlays = new Dictionary<IGPSPoint, IMapOverlay>();

        double routePointsZoom = double.NaN;
        private IList<IGPSPoint> routePointsAtZoom = new List<IGPSPoint>();
        //private RouteItemsDataChangeListener listener;

        private IList<IGPSLocation> m_SelectedGPSLocations = new List<IGPSLocation>();
        private IList<IGPSLocation> m_HighlightedGPSLocations = new List<IGPSLocation>();
        private bool m_ShowHighlight = false;
        private float m_highlightRadius;
    }
}

#endif
