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

//Used in both Trails and Matrix plugin

//ST_3_0: Display TrailPoints

#if !ST_2_1
using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ZoneFiveSoftware.Common.Visuals.Mapping;
using System.Collections.Generic;
using Microsoft.Win32;
using TrailsPlugin.Data;

namespace TrailsPlugin.UI.MapLayers
{
    class ExtendRouteControlLayerProviders : IExtendRouteControlLayerProviders
    {
        public IList<IRouteControlLayerProvider> RouteControlLayerProviders
        {
            get
            {
                return new IRouteControlLayerProvider[] { TrailPointsProvider.Instance };
            }
        }
    }
    class TrailPointsProvider : IRouteControlLayerProvider
    {
        private static TrailPointsProvider m_instance = null;
        public static TrailPointsProvider Instance
        {
            get
            {
                if (TrailPointsProvider.m_instance == null)
                {
                    TrailPointsProvider.m_instance = new TrailPointsProvider();
                }
                return TrailPointsProvider.m_instance;
            }
        }
        private IRouteControlLayer m_layer = null;
        public IRouteControlLayer RouteControlLayer
        {
            get { return m_layer; }
        }
        public IRouteControlLayer CreateControlLayer(IRouteControl control)
        {
            if (m_layer == null)
            {
                m_layer = new TrailPointsLayer(this,control);
            }
                return m_layer;
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
        public TrailPointsLayer(IRouteControlLayerProvider provider, IRouteControl control)
            : base(provider, control, 1)
        {
            //PluginMain.GetApplication().SystemPreferences.PropertyChanged += new PropertyChangedEventHandler(SystemPreferences_PropertyChanged);
            //listener = new RouteItemsDataChangeListener(control);
            //listener.PropertyChanged += new PropertyChangedEventHandler(OnRouteItemsPropertyChanged);
        }

        public IList<TrailGPSLocation> TrailPoints
        {
            //get
            //{
            //    return m_TrailPoints;
            //}
            set
            {
                bool changed = false;
                if (!value.Equals(m_TrailPoints)) { changed = true; }
                m_TrailPoints = value;
                if (changed) { RefreshOverlays(true); }
            }
        }
        public IList<TrailGPSLocation> SelectedTrailPoints
        {
            //get
            //{
            //    return m_SelectedTrailPoints;
            //}
            set
            {
                //Set selected area to include selected points, including radius and some more
                if (value.Count > 0)
                {
                    float north = -180;
                    float south = +180;
                    float east = -90;
                    float west = 90;
                    foreach (TrailGPSLocation g in value)
                    {
                        north = Math.Max(north, g.GpsLocation.LatitudeDegrees);
                        south = Math.Min(south, g.GpsLocation.LatitudeDegrees);
                        east = Math.Max(east, g.GpsLocation.LongitudeDegrees);
                        west = Math.Min(west, g.GpsLocation.LongitudeDegrees);
                    }
                    //Get approx degrees for the radius offset
                    //The magic numbers are size of a degree at the equator
                    //latitude increases about 1% at the poles
                    //longitude is up to 40% longer than linear extension - compensate 20%
                    float lat = 2 * this.m_highlightRadius / 110574 * 1.005F;
                    float lng = 2 * this.m_highlightRadius / 111320 * Math.Abs(south) / 90 * 1.2F;
                    north += lat;
                    south -= lat;
                    east += lng;
                    west -= lng;
                    GPSBounds area = new GPSBounds(new GPSLocation(north, west), new GPSLocation(south, east));
                    this.MapControl.SetLocation(area.Center,
                      this.MapControl.ComputeZoomToFit(area));
                    m_SelectedTrailPoints = value;
                }
            }
        }
        public IDictionary<string, MapPolyline> TrailRoutes
        {
            //get
            //{
            //    return m_TrailRoutes;
            //}
            set
            {
                bool changed = false;
                if (!value.Equals(m_TrailRoutes)) { changed = true; }
                m_TrailRoutes = value;
                if (changed) { RefreshOverlays(true); }
            }
        }

        public IDictionary<string, MapPolyline> MarkedTrailRoutes
        {
            //get
            //{
            //    return m_MarkedTrailRoutes;
            //}
            set
            {
                bool changed = false;
                if (!value.Equals(m_MarkedTrailRoutes)) { changed = true; }
                m_MarkedTrailRoutes = value;
                if (changed)
                {
                    if (value.Count > 0)
                    {
                        GPSBounds area = null;
                        foreach (MapPolyline m in value.Values)
                        {
                            GPSBounds area2 = GPSBounds.FromGPSPoints(m.Locations);
                            if (area2 != null)
                            {
                                if (area == null)
                                {
                                    area = area2;
                                }
                                else
                                {
                                    area = (GPSBounds)area.Union(area2);
                                }
                            }
                        }
                        if (area != null)
                        {
                            this.MapControl.SetLocation(area.Center,
                             this.MapControl.ComputeZoomToFit(area));
                        }
                    }
                    RefreshOverlays(true);
                }
            }
        }

        public float HighlightRadius
        {
            set
            {
                if (m_highlightRadius != value)
                {
                    m_scalingChanged = true;
                }
                m_highlightRadius = value;
            }
        }

        //public void Refresh()
        //{
        //    //Should not be necessary in ST3, updated when needed
        //    RefreshOverlays(); 
        //}
        public bool ShowPage
        {
            get { return _showPage; }
            set
            {
                bool changed = (value != _showPage);
                _showPage = value;
                if (changed)
                {
                    RefreshOverlays(true);
                }
            }
        }

        /*************************************************************/
        protected override void OnMapControlZoomChanged(object sender, EventArgs e)
        {
            m_scalingChanged = true;
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
            RefreshOverlays();
        }

        protected override void OnRouteControlItemsChanged(object sender, EventArgs e)
        {
            RefreshOverlays();
        }

        //private void SystemPreferences_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "RouteSettings.ShowGPSPoints" ||
        //        e.PropertyName == "RouteSettings.MarkerShape" ||
        //        e.PropertyName == "RouteSettings.MarkerSize" ||
        //        e.PropertyName == "RouteSettings.MarkerColor")
        //    {
        //        if (RouteControl.Visible)
        //        {
        //            RefreshOverlays();
        //        }
        //        else
        //        {
        //            routeSettingsChanged = true;
        //        }
        //    }
        //}

        //private void OnRouteItemsPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == Activity.PropertyName.GPSRoute ||
        //        e.PropertyName == Route.PropertyName.GPSRoute ||
        //        e.PropertyName == Activity.PropertyName.Category )
        //    {
        //        ClearCachedLocations();
        //        RefreshOverlays();
        //    }
        //}

        private static MapIcon getCircle(IMapControl mapControl, float radius)
        {
            //Get pixel Size for icon - can differ X and Y
            //Calculate to radius, use point at apropriate distance to get meters->pixels
            const int circlePixelSize = 100;
            IGPSPoint point0 = Utils.GPS.LocationToPoint(mapControl.MapProjection.PixelToGPS(mapControl.Center, mapControl.Zoom,
                new Point(0, 0)));
            IGPSPoint pointX = Utils.GPS.LocationToPoint(mapControl.MapProjection.PixelToGPS(mapControl.Center, mapControl.Zoom,
                new Point(circlePixelSize / 2, 0)));
            IGPSPoint pointY = Utils.GPS.LocationToPoint(mapControl.MapProjection.PixelToGPS(mapControl.Center, mapControl.Zoom,
                new Point(0, circlePixelSize / 2)));
            int sizeInPixelsX = (int)(circlePixelSize * radius / point0.DistanceMetersToPoint(pointX));
            int sizeInPixelsY = (int)(circlePixelSize * radius / point0.DistanceMetersToPoint(pointY));

            Size iconSize;
            string fileURL = TrailsPlugin.CommonIcons.Circle(sizeInPixelsX, sizeInPixelsY, out iconSize);
            return new MapIcon(fileURL, iconSize, new Point(iconSize.Width / 2, iconSize.Height / 2));
        }

        private void RefreshOverlays()
        {
            RefreshOverlays(false);
        }
        private void RefreshOverlays(bool clear)
        {
            if (clear || MapControlChanged)
            {
                ClearOverlays();
                ResetMapControl();
            }

            if (!_showPage) return;

            IGPSBounds windowBounds = MapControlBounds;
            IList<IMapOverlay> addedOverlays = new List<IMapOverlay>();

            //RouteOverlay
            IDictionary<string, MapPolyline> allRoutes = new Dictionary<string, MapPolyline>();
            foreach (KeyValuePair<string, MapPolyline> pair in m_TrailRoutes)
            {
                allRoutes.Add(pair);
            }
            foreach (KeyValuePair<string, MapPolyline> pair in m_MarkedTrailRoutes)
            {
                allRoutes.Add(pair);
            }
            IDictionary<string, MapPolyline> visibleRoutes = new Dictionary<string, MapPolyline>();
            foreach (KeyValuePair<string, MapPolyline> pair in allRoutes)
            {
                IList<IGPSPoint> r = new List<IGPSPoint>();
                foreach (IGPSPoint point in pair.Value.Locations)
                {
                    if (windowBounds.Contains(point))
                    {
                        visibleRoutes.Add(pair);
                        break;
                    }
                }
                //check for route in bounds only - the following does not seem to speed up
                //foreach (IGPSPoint point in pair.Value.Locations)
                //{
                //    if (windowBounds.Contains(point))
                //    {
                //        r.Add(point);
                //    }
                //}
                //if (r.Count > 0)
                //{
                //    MapPolyline m = new MapPolyline(r, pair.Value.LineWidth, pair.Value.LineColor);
                //    visibleRoutes.Add(pair.Key, m);
                //}
            }
            IDictionary<MapPolyline, IMapOverlay> newRouteOverlays = new Dictionary<MapPolyline, IMapOverlay>();

            foreach (KeyValuePair<string, MapPolyline> pair in visibleRoutes)
            {
                MapPolyline m = pair.Value;
                if ((!m_scalingChanged) && routeOverlays.ContainsKey(m))
                {
                    //No need to refresh this point
                    newRouteOverlays.Add(m, routeOverlays[m]);
                    routeOverlays.Remove(m);
                }
                else
                {
                    newRouteOverlays.Add(m, m);
                    addedOverlays.Add(m);
                }
            }

            //TrailPoints
            IList<IGPSPoint> visibleLocations = new List<IGPSPoint>();
            foreach (TrailGPSLocation point in m_TrailPoints)
            {
                if (windowBounds.Contains(point.GpsLocation))
                {
                    visibleLocations.Add(Utils.GPS.LocationToPoint(point.GpsLocation));
                }
            }
            IDictionary<IGPSPoint, IMapOverlay> newPointOverlays = new Dictionary<IGPSPoint, IMapOverlay>();

            if (m_scalingChanged || null == m_icon)
            {
                m_icon = getCircle(this.MapControl, m_highlightRadius);
            }
            foreach (IGPSPoint location in visibleLocations)
            {
                if ((!m_scalingChanged) && pointOverlays.ContainsKey(location))
                {
                    //No need to refresh this point
                    newPointOverlays.Add(location, pointOverlays[location]);
                    pointOverlays.Remove(location);
                }
                else
                {
                    MapMarker pointOverlay = new MapMarker(location, m_icon, false);
                    newPointOverlays.Add(location, pointOverlay);
                    addedOverlays.Add(pointOverlay);
                }
            }

            // Draw overlay
            if (0 == visibleLocations.Count && 0 == visibleRoutes.Count) return;

            m_scalingChanged = false;
            ClearOverlays();
            MapControl.AddOverlays(addedOverlays);
            pointOverlays = newPointOverlays;
            routeOverlays = newRouteOverlays;
        }

        private void ClearOverlays()
        {
            MapControl.RemoveOverlays(pointOverlays.Values);
            pointOverlays.Clear();
            MapControl.RemoveOverlays(routeOverlays.Values);
            routeOverlays.Clear();
        }

        private bool m_scalingChanged = false;
        MapIcon m_icon = null;
        private bool routeSettingsChanged = false;
        private IDictionary<IGPSPoint, IMapOverlay> pointOverlays = new Dictionary<IGPSPoint, IMapOverlay>();
        private IDictionary<MapPolyline, IMapOverlay> routeOverlays = new Dictionary<MapPolyline, IMapOverlay>();
        //private RouteItemsDataChangeListener listener;

        private IList<TrailGPSLocation> m_TrailPoints = new List<TrailGPSLocation>();
        private IList<TrailGPSLocation> m_SelectedTrailPoints = new List<TrailGPSLocation>();
        private IDictionary<string, MapPolyline> m_TrailRoutes = new Dictionary<string, MapPolyline>();
        private IDictionary<string, MapPolyline> m_MarkedTrailRoutes = new Dictionary<string, MapPolyline>();
        private float m_highlightRadius;
        private static bool _showPage;
    }
}
#endif