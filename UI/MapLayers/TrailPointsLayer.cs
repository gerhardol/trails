/*
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

//Temporary empty implementation
//ST_3_0: Both display/select, not included for ST_2_1 

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Visuals.Mapping;
using System.Collections.Generic;
using Microsoft.Win32;

/* Display locations in ST3 */
namespace TrailsPlugin.UI.MapLayers
{
    class TrailPointsProvider : IRouteControlLayerProvider
    {
        public IRouteControlLayer CreateControlLayer(IRouteControl control)
        {
            throw new NotImplementedException();
        }

        public Guid Id
        {
            get { return GUIDs.TrailPointsControlLayerProvider; }
        }

        public string Name
        {
            get { return Properties.Resources.TrailPointsControlLayer; }
        }
    }
    class TrailPointsLayer : RouteControlLayerBase, IRouteControlLayer
    {
        public TrailPointsLayer() { } //xxx - temporary
        public TrailPointsLayer(IRouteControlLayerProvider provider, IRouteControl control)
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
               // if (null != m_mapControl) m_mapControl.Refresh();
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
            //if (RouteControl.Visible && routeSettingsChanged)
            //{
            //    ClearOverlays();
            //    routeSettingsChanged = false;
            //    RefreshOverlays();
            //}
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
            if (e.PropertyName == null/* ||
               e.PropertyName == PluginMain.GetApplication().SystemPreferences.RouteSettings_ShowGPSPoints ||
                e.PropertyName == PluginMain.GetApplication().SystemPreferences.RouteSettings.MarkerShape ||
                e.PropertyName == PluginMain.GetApplication().SystemPreferences.RouteSettings.MarkerSize ||
                e.PropertyName == PluginMain.GetApplication().SystemPreferences.RouteSettings.MarkerColor)*/)
            {
                //xxx if (RouteControl.Visible)
                //{
                //    RefreshOverlays();
                //}
                //else
                //{
                //    routeSettingsChanged = true;
                //}
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

                //IGPSLocation loc1 = drawContext.Projection.PixelToGPS(drawContext.Center, drawContext.ZoomLevel, new Point(0, 0));
                //IGPSLocation loc2 = drawContext.Projection.PixelToGPS(drawContext.Center, drawContext.ZoomLevel, new Point(0, 100));
                //IGPSPoint point1 = Utils.GPS.LocationToPoint(loc1);
                //IGPSPoint point2 = Utils.GPS.LocationToPoint(loc2);
                //float meters = point1.DistanceMetersToPoint(point2) / 100;
                //float radiusInPixels = m_highlightRadius / meters;

                //foreach (IGPSLocation gpsLocation in m_HighlightedGPSLocations)
                //{
                //    Point point = drawContext.Projection.GPSToPixel(drawContext.Center, drawContext.ZoomLevel, gpsLocation);
                //    Pen pen = new Pen(Color.Red, 5.0F);

                //    drawContext.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //    float X = point.X + (drawContext.DrawRectangle.Width / 2) - radiusInPixels;
                //    float Y = point.Y + (drawContext.DrawRectangle.Height / 2) - radiusInPixels;
                //    drawContext.Graphics.DrawEllipse(pen, X, Y, radiusInPixels * 2, radiusInPixels * 2);
                //}
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
