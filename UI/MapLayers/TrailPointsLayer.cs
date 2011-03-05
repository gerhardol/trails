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
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ZoneFiveSoftware.Common.Visuals.Mapping;
using System.Collections.Generic;
using Microsoft.Win32;
using TrailsPlugin.Data;
using GpsRunningPlugin.Util;

namespace TrailsPlugin.UI.MapLayers
{
    public class TrailPointsLayer : RouteControlLayerBase, IRouteControlLayer
    {
        public TrailPointsLayer(IRouteControlLayerProvider provider, IRouteControl control)
            : base(provider, control, 1)
        {
            Guid currentView = UnitUtil.GetApplication().ActiveView.Id;
            if (m_layers.ContainsKey(currentView))
            {
                m_layers[currentView].m_extraMapLayer = this;
            }
            else
            {
                m_layers[currentView] = this;
            }
            //UnitUtil.GetApplication().SystemPreferences.PropertyChanged += new PropertyChangedEventHandler(SystemPreferences_PropertyChanged);
            //listener = new RouteItemsDataChangeListener(control);
            //listener.PropertyChanged += new PropertyChangedEventHandler(OnRouteItemsPropertyChanged);
        }

        //Note: There is an assumption of the relation between view and route control/layer
        //See the following: http://www.zonefivesoftware.com/sporttracks/forums/viewtopic.php?t=9465
        public static TrailPointsLayer Instance(IView view)
        {
            TrailPointsLayer result = null;
            if (view != null && m_layers != null && m_layers.ContainsKey(view.Id))
            {
                result = m_layers[view.Id];
            }
            else if (m_layers.Count > 0)
            {
                foreach (TrailPointsLayer l in m_layers.Values)
                {
                    //Just any layer - the first should be the best
                    result = l;
                    break;
                }
            }
            return result;
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
                    GPSBounds area = TrailGPSLocation.getGPSBounds(value, 2*this.m_highlightRadius);
                    this.DoZoom(area);
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
                        IGPSBounds area = TrailMapPolyline.getGPSBounds(value);
                        DoZoom(area);
                    }
                    RefreshOverlays(true);
                }
            }
        }

        public IGPSBounds Union(IGPSBounds area1, IGPSBounds area2)
        {
            if (area1 != null && area2 != null)
            {
                area1 = area1.Union(area2);
            }
            else
            {
                //At least one of the areas is null
                if (area2 != null)
                {
                    area1 = area2;
                }
            }
            return area1;
        }

        //Zoom to "relevant" contents (normally done when activities are updated)
        public void DoZoom()
        {
            IGPSBounds area1 = TrailMapPolyline.getGPSBounds(m_TrailRoutes);
            IGPSBounds area2 = TrailGPSLocation.getGPSBounds(m_TrailPoints, 2*this.m_highlightRadius);
            area1 = Union(area1, area2);
            DoZoom(area1);
        }

        public void DoZoom(IGPSBounds area)
        {
            if (_showPage)
            {
                if (area != null)
                {
                    this.MapControl.SetLocation(area.Center,
                    this.MapControl.ComputeZoomToFit(area));
                    if (m_extraMapLayer != null)
                    {
                        m_extraMapLayer.MapControl.SetLocation(area.Center,
                        m_extraMapLayer.MapControl.ComputeZoomToFit(area));
                    }
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
        public bool HidePage()
        {
            _showPage = false;
            RefreshOverlays(true);
            return true;
        }
        public void ShowPage(string bookmark)
        {
            _showPage = true;
            RefreshOverlays(true);
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
            const int circlePixelSize = 1000;
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
            //Only add a route exactly once, prefer marked routes
            IDictionary<IList<IGPSPoint>, MapPolyline> allRoutes = new Dictionary<IList<IGPSPoint>, MapPolyline>();
            IDictionary<IList<IGPSPoint>, MapPolyline> dupRoutes = new Dictionary<IList<IGPSPoint>, MapPolyline>();
            foreach (KeyValuePair<string, MapPolyline> pair in m_MarkedTrailRoutes)
            {
                if (!allRoutes.ContainsKey(pair.Value.Locations))
                {
                    allRoutes.Add(new KeyValuePair<IList<IGPSPoint>, MapPolyline>(pair.Value.Locations,pair.Value));
                }
            }
            foreach (KeyValuePair<string, MapPolyline> pair in m_TrailRoutes)
            {
                if (!allRoutes.ContainsKey(pair.Value.Locations))
                {
                    allRoutes.Add(new KeyValuePair<IList<IGPSPoint>, MapPolyline>(pair.Value.Locations,pair.Value));
                }
                else if (!dupRoutes.ContainsKey(pair.Value.Locations))
                {
                    dupRoutes.Add(new KeyValuePair<IList<IGPSPoint>, MapPolyline>(pair.Value.Locations, pair.Value));
                }
            }
            IDictionary<IList<IGPSPoint>, MapPolyline> visibleRoutes = new Dictionary<IList<IGPSPoint>, MapPolyline>();
            foreach (KeyValuePair<IList<IGPSPoint>, MapPolyline> pair in allRoutes)
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
            IDictionary<IList<IGPSPoint>, IMapOverlay> newRouteOverlays = new Dictionary<IList<IGPSPoint>, IMapOverlay>();

            foreach (KeyValuePair<IList<IGPSPoint>, MapPolyline> pair in visibleRoutes)
            {
                MapPolyline m = pair.Value;
                newRouteOverlays.Add(m.Locations, m);
                if ((!m_scalingChanged) && 
                    routeOverlays.ContainsKey(m.Locations) && 
                    !dupRoutes.ContainsKey(m.Locations))
                {
                    //No need to refresh this point
                    routeOverlays.Remove(m.Locations);
                }
                else
                {
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
            if (m_extraMapLayer != null)
            {
                try
                {
                    //Remove overlays are not working properly, the Map is not very usable
                    m_extraMapLayer.MapControl.AddOverlays(addedOverlays);
                }catch(Exception){}
                m_extraMapLayer.pointOverlays = newPointOverlays;
                m_extraMapLayer.routeOverlays = newRouteOverlays;
            }
        }

        public void ClearOverlays()
        {
            MapControl.RemoveOverlays(pointOverlays.Values);
            pointOverlays.Clear();
            MapControl.RemoveOverlays(routeOverlays.Values);
            routeOverlays.Clear();
            if (m_extraMapLayer != null)
            {
                m_extraMapLayer.ClearOverlays();
            }
        }

        private bool m_scalingChanged = false;
        MapIcon m_icon = null;
        private bool routeSettingsChanged = false;
        private IDictionary<IGPSPoint, IMapOverlay> pointOverlays = new Dictionary<IGPSPoint, IMapOverlay>();
        private IDictionary<IList<IGPSPoint>, IMapOverlay> routeOverlays = new Dictionary<IList<IGPSPoint>, IMapOverlay>();
        //private RouteItemsDataChangeListener listener;

        private IList<TrailGPSLocation> m_TrailPoints = new List<TrailGPSLocation>();
        private IList<TrailGPSLocation> m_SelectedTrailPoints = new List<TrailGPSLocation>();
        private IDictionary<string, MapPolyline> m_TrailRoutes = new Dictionary<string, MapPolyline>();
        private IDictionary<string, MapPolyline> m_MarkedTrailRoutes = new Dictionary<string, MapPolyline>();
        private float m_highlightRadius;
        private bool _showPage;
        private static IDictionary<Guid, TrailPointsLayer> m_layers = new Dictionary<Guid, TrailPointsLayer>();
        private TrailPointsLayer m_extraMapLayer = null;
    }
}
#endif