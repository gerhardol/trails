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
using TrailsPlugin.UI.Activity;
using GpsRunningPlugin.Util;

namespace TrailsPlugin.UI.MapLayers
{
    public class TrailPointsLayer : RouteControlLayerBase, IRouteControlLayer
    {
        class PointMapMarker : MapMarker
        {
            public PointMapMarker(TrailGPSLocation location, MapIcon icon, bool clickable)
                : base(location, icon, clickable)
            {
            }

            public TrailGPSLocation TrailPoint
            {
                get
                {
                    return this.Location as TrailGPSLocation;
                }
                set
                {
                    this.Location = value;
                }
            }

            public override string ToString()
            {
                return this.TrailPoint.ToString();
            }
        }

        public TrailPointsLayer(IRouteControlLayerProvider provider, IRouteControl control, int zorder, bool mouseEvents)
            : base(provider, control, zorder, mouseEvents)
        {
            if (UnitUtil.GetApplication() != null && UnitUtil.GetApplication().ActiveView != null)
            {
                Guid currentView = UnitUtil.GetApplication().ActiveView.Id;
                string key = currentView.ToString() + zorder;
                if (m_layers.ContainsKey(key))
                {
                    m_layers[key].m_extraMapLayer = this;
                }
                else
                {
                    m_layers[key] = this;
                }
            }
        }

        //Note: There is an assumption of the relation between view and route control/layer
        //See the following: http://www.zonefivesoftware.com/sporttracks/forums/viewtopic.php?t=9465
        public static TrailPointsLayer InstanceRoutes(ActivityDetailPageControl page, IView view)
        {
            return TrailPointsLayer.InstanceBase(page, view, TrailPointsProvider.TrailsLayerZOrderRoutes);
        }
        public static TrailPointsLayer InstancePoints(ActivityDetailPageControl page, IView view)
        {
            return TrailPointsLayer.InstanceBase(page, view, TrailPointsProvider.TrailsLayerZOrderPoints);
        }
        public static TrailPointsLayer InstanceMarked(ActivityDetailPageControl page, IView view)
        {
            return TrailPointsLayer.InstanceBase(page, view, TrailPointsProvider.TrailsLayerZOrderMarked);
        }
        private static TrailPointsLayer InstanceBase(ActivityDetailPageControl page, IView view, int zorder)
        {
            TrailPointsLayer result = null;
            if (view != null && m_layers != null)
            {
                string key = view.Id.ToString() + zorder;
                if (m_layers.ContainsKey(key))
                {
                    result = m_layers[key];
                }
            }
            if (result == null && m_layers.Count > 0)
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
            set
            {
                m_TrailPoints = value;
                RefreshOverlays(true);
            }
        }

        public IList<SplitGPSLocation> SplitPoints
        {
            set
            {
                m_SplitPoints = value;
                RefreshOverlays(true);
            }
        }

        public IList<TrailGPSLocation> SelectedTrailPoints
        {
            set
            {
                //Set selected area to include selected points, including radius and some more
                if (value.Count > 0)
                {
                    //Normal trails all have the same radius, should not be much difference anyway
                    float highlightRadius = value[0].Radius;
                    GPSBounds area = TrailGPSLocation.getGPSBounds(value, 4 * highlightRadius);
                    this.DoZoom(area);
                }
            }
        }

        public IDictionary<string, MapPolyline> TrailRoutes
        {
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
            set
            {
                m_MarkedTrailRoutes = value;
                //Zooming done within ST
                RefreshOverlays(true);
            }
        }

        public IDictionary<string, MapPolyline> MarkedTrailRoutesNoShow
        {
            set
            {
                m_MarkedTrailRoutesNoShow = value;
                //Zooming done within ST
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

        public IGPSBounds RelevantArea()
        {
            IGPSBounds area = null;
            if (m_TrailRoutes.Count > 0)
            {
                area = TrailMapPolyline.getGPSBounds(m_TrailRoutes);
            }
            if (m_TrailPoints.Count > 0)
            {
                //Normal trails all have the same radius, should not be much difference anyway
                float highlightRadius = m_TrailPoints[0].Radius;
                IGPSBounds area2 = TrailGPSLocation.getGPSBounds(m_TrailPoints, 2 * highlightRadius);
                area = this.Union(area, area2);
            }
            if (m_SplitPoints.Count > 0)
            {
                IGPSBounds area2 = SplitGPSLocation.getGPSBounds(m_SplitPoints);
                area = this.Union(area, area2);
            }
            if (m_MarkedTrailRoutes.Count > 0 || m_MarkedTrailRoutesNoShow.Count > 0)
            {
                IGPSBounds area2 = Union(TrailMapPolyline.getGPSBounds(m_MarkedTrailRoutes),
                    TrailMapPolyline.getGPSBounds(m_MarkedTrailRoutesNoShow));
                area = this.Union(area, area2);
            }
            return area;
        }

        public void EnsureVisible()
        {
            this.EnsureVisible(this.RelevantArea());
        }

        public new void EnsureVisible(IGPSBounds area)
        {
            if (m_showPage)
            {
                base.EnsureVisible(area);
                if (m_extraMapLayer != null)
                {
                    m_extraMapLayer.EnsureVisible(area);
                }
            }
        }

        public new void SetLocation(IGPSBounds area)
        {
            if (m_showPage)
            {
                base.SetLocation(area);
                if (m_extraMapLayer != null)
                {
                    m_extraMapLayer.SetLocation(area);
                }
            }
        }

        //Zoom to "relevant" contents (normally done when activities are updated)
        public void DoZoom()
        {
            this.DoZoom(this.RelevantArea());
        }

        public new void DoZoom(IGPSBounds area)
        {
            //Note no m_showPage here, can be done when creating tracks
            base.DoZoom(area);
            if (m_extraMapLayer != null)
            {
                m_extraMapLayer.DoZoom(area);
            }
        }

        public void ZoomIn()
        {
            this.MapControl.ZoomIn();
        }

        public void ZoomOut()
        {
            this.MapControl.ZoomOut();
        }

        public IGPSLocation GetGps(Point p)
        {
            return this.MapControl.MapProjection.PixelToGPS(this.MapControl.MapBounds.NorthWest, this.MapControl.Zoom, p);
        }

        public void Refresh()
        {
            //Should not be necessary in ST3, updated when needed
            RefreshOverlays();
        }
        public bool HidePage()
        {
            m_showPage = false;
            RemoveMapControlEventHandlers();
            RefreshOverlays(true);
            return true;
        }
        public void ShowPage(string bookmark)
        {
            m_showPage = true;
            RefreshOverlays(true);
            AddMapControlEventHandlers();
        }

        public EditTrail editTrail
        {
            set
            {
                this.m_editTrail = value;
            }
        }
        private void SetMovingWaypoint(PointMapMarker waypoint)
        {
            if (this.m_editTrail != null)
            {
                if (this._selectedPointMoving != null)
                {
                    if (this._selectedPointMoving != null &&
                        this._selectedPointOriginal != null)
                    {
                        //New selection before old finished - set old location back
                        this._selectedPointMoving.TrailPoint = this._selectedPointOriginal;
                        this.m_editTrail.UpdatePointFromMap(this._selectedPointMoving.TrailPoint);
                    }
                }

                if (waypoint != null)
                {
                    //Start - offset saved separately
                    this._selectedPointOriginal = waypoint.TrailPoint;

                    this._selectedPointMoving = waypoint;
                    this.m_editTrail.UpdatePointFromMap(this._selectedPointMoving.TrailPoint);
                    this.MapControl.CanMouseDrag = false;
                }
                else
                {
                    //End or cancel
                    this._selectedPointMoving = null;
                    this._selectedPointOriginal = null;
                    this.MapControl.CanMouseDrag = true;
                    this.RefreshOverlays(true);
                }
            }
        }

        /*************************************************************/
        private void pointOverlay_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is PointMapMarker)
            {
                PointMapMarker selectedPoint = sender as PointMapMarker;
                if (this.m_editTrail != null)
                {
                    if (e.Button == MouseButtons.Left ||
                        //Google Maps always reports button None
                        e.Button == MouseButtons.None)
                    {
                        SetMovingWaypoint(selectedPoint);
                        //Save offset from click to marker reference
                        Point midPoint = MapControl.MapProjection.GPSToPixel(MapControl.MapBounds.NorthWest,
                            MapControl.Zoom, selectedPoint.TrailPoint);
                        this.m_clickToCenterOffset = new Point(midPoint.X - e.Location.X, midPoint.Y - e.Location.Y);
                    }
                }
                else
                {
                    //TODO: Not working
                    //toolTip.ShowAlways = true;
                    //this.toolTip.Show(selectedPoint.TrailPoint.Name,MapControl.Control.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent, 2000);
                }
            }
        }

        private void pointOverlay_MouseUp(object sender, MouseEventArgs e)
        {
            if (this._selectedPointMoving != null)
            {
                //Clear original location - no cancel
                this._selectedPointOriginal = null;
            }
            SetMovingWaypoint(null);
        }

        private void CancelSelection()
        {
            SetMovingWaypoint(null);
        }

        protected override void OnMapControlMouseMove(object sender, MouseEventArgs e)
        {
            if (this._selectedPointMoving != null)
            {
                if (_selectedPointMoving != null)
                {
                    //MapControl.RemoveOverlay(_selectedWaypointMoving);
                    Point p = new Point(m_clickToCenterOffset.X + e.Location.X, m_clickToCenterOffset.Y + e.Location.Y);
                    IGPSLocation l = MapControl.MapProjection.PixelToGPS(MapControl.MapBounds.NorthWest, MapControl.Zoom, p);
                    _selectedPointMoving.TrailPoint.GpsLoc = l;
                    this.m_editTrail.UpdatePointFromMap(this._selectedPointMoving.TrailPoint);

                    //Refresh this point. Only seem to be possible with refreshing all
                    RefreshOverlays(true);
                    //MapControl.AddOverlay(_selectedWaypointMoving);
                    //MapControl.RefreshMap();
                }
            }
        }

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
            if (RouteControl.Visible && m_routeSettingsChanged)
            {
                ClearOverlays();
                m_routeSettingsChanged = false;
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

        protected override void OnMapControlMouseLeave(object sender, EventArgs e)
        {
            CancelSelection();
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

        private static void pixelSize(IMapControl mapControl, out float x, out float y)
        {
            //Use an "arbitrary" circlePixelSize to find the distance/size for a pixel - can differ X and Y
            //TBD: Cache per zoom level
            const int circlePixelSize = 1000;
            IGPSPoint point0 = Utils.GPS.LocationToPoint(mapControl.MapProjection.PixelToGPS(mapControl.Center, mapControl.Zoom,
                new Point(0, 0)));
            IGPSPoint pointX = Utils.GPS.LocationToPoint(mapControl.MapProjection.PixelToGPS(mapControl.Center, mapControl.Zoom,
                new Point(circlePixelSize / 2, 0)));
            IGPSPoint pointY = Utils.GPS.LocationToPoint(mapControl.MapProjection.PixelToGPS(mapControl.Center, mapControl.Zoom,
                new Point(0, circlePixelSize / 2)));
            x = (float)circlePixelSize / point0.DistanceMetersToPoint(pointX);
            y = (float)circlePixelSize / point0.DistanceMetersToPoint(pointY);
        }

        /// <summary>
        /// Get a radius for a trail matching a minimum number of pixels
        /// </summary>
        /// <param name="minPixels"></param>
        /// <returns></returns>
        public float getRadius(int pixels)
        {
            float x, y;
            pixelSize(this.MapControl, out x, out y);
            float radius = pixels * Math.Max(1/x, 1/y);
            return radius;
        }


        private MapIcon getCircleIcon(IMapControl mapControl, float radius)
        {
            if (this.m_icon.icon == null || this.m_icon.radius != radius || this.m_icon.scaling != this.MapControl.Zoom)
            {
                this.m_icon.radius = radius;
                this.m_icon.scaling = this.MapControl.Zoom;
                this.m_icon.icon = getCircle(this.MapControl, this.m_icon.radius, true);
            }
            return this.m_icon.icon;
        }

        private static MapIcon getCircle(IMapControl mapControl, float radius, bool centerPoint)
        {
            float x, y;
            pixelSize(mapControl, out x, out y);
            int sizeInPixelsX = (int)(x * radius);
            int sizeInPixelsY = (int)(y * radius);
            if (sizeInPixelsX < 15 || sizeInPixelsY < 15)
            {
                centerPoint = false;
            }

            Size iconSize;
            string fileURL = TrailsPlugin.CommonIcons.Circle(sizeInPixelsX, sizeInPixelsY, centerPoint, out iconSize);
            return new MapIcon(fileURL, iconSize, new Point(iconSize.Width / 2, iconSize.Height / 2));
        }

        private IDictionary<Color, MapIcon> m_SplitPointIconCache = new Dictionary<Color, MapIcon>();
        private MapIcon getRhombusIcon(Color c)
        {
            if (!m_SplitPointIconCache.ContainsKey(c))
            {
                Size iconSize;
                string fileURL = TrailsPlugin.CommonIcons.Rhombus(11, 11, c, out iconSize);
                m_SplitPointIconCache[c] = new MapIcon(fileURL, iconSize, new Point(iconSize.Width / 2, iconSize.Height / 2));
            }
            return m_SplitPointIconCache[c];
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

            if (!m_showPage) return;

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
                    m_routeOverlays.ContainsKey(m.Locations) && 
                    !dupRoutes.ContainsKey(m.Locations))
                {
                    //No need to refresh this point
                    m_routeOverlays.Remove(m.Locations);
                }
                else
                {
                    addedOverlays.Add(m);
                }
            }

            //TrailPoints
            IDictionary<IGPSPoint, IMapOverlay> newPointOverlays = new Dictionary<IGPSPoint, IMapOverlay>();
            foreach (TrailGPSLocation location in m_TrailPoints)
            {
                PointMapMarker pointOverlay = new PointMapMarker(location, getCircleIcon(this.MapControl, location.Radius), MouseEvents && (this.m_editTrail != null));
                if (this.MouseEvents && location.Name != "DebugNotClickableDebug")
                {
                    pointOverlay.MouseDown += new MouseEventHandler(pointOverlay_MouseDown);
                    pointOverlay.MouseUp += new MouseEventHandler(pointOverlay_MouseUp);
                }
                if (!newPointOverlays.ContainsKey(location))
                {
                    newPointOverlays.Add(location, pointOverlay);
                }
                addedOverlays.Add(pointOverlay);
            }

            //SplitPoints
            foreach (SplitGPSLocation location in m_SplitPoints)
            {
                PointMapMarker pointOverlay = new PointMapMarker(location, getRhombusIcon(location.PointColor), false);
                if (!newPointOverlays.ContainsKey(location))
                {
                    newPointOverlays.Add(location, pointOverlay);
                }
                addedOverlays.Add(pointOverlay);
            }

            // Draw overlay
            if (0 == newPointOverlays.Count && 0 == visibleRoutes.Count) return;

            m_scalingChanged = false;
            if (!clear && !MapControlChanged)
            {
                ClearOverlays();
            }
            MapControl.AddOverlays(addedOverlays);
            m_pointOverlays = newPointOverlays;
            m_routeOverlays = newRouteOverlays;
            if (m_extraMapLayer != null)
            {
                try
                {
                    //Remove overlays are not working properly, the Map is not very usable
                    m_extraMapLayer.MapControl.AddOverlays(addedOverlays);
                }
                catch(Exception){}
                m_extraMapLayer.m_pointOverlays = newPointOverlays;
                m_extraMapLayer.m_routeOverlays = newRouteOverlays;
            }
        }

        public void ClearOverlays()
        {
            foreach (PointMapMarker w in this.m_pointOverlays.Values)
            {
                w.MouseDown -= new MouseEventHandler(pointOverlay_MouseDown);
                w.MouseUp -= new MouseEventHandler(pointOverlay_MouseUp);
            }
            MapControl.RemoveOverlays(m_pointOverlays.Values);
            m_pointOverlays.Clear();
            MapControl.RemoveOverlays(m_routeOverlays.Values);
            m_routeOverlays.Clear();
            if (m_extraMapLayer != null)
            {
                m_extraMapLayer.ClearOverlays();
            }
        }

        private class MapIconCache
        {
            public MapIcon icon = null;
            public double scaling = 0;
            public float radius = 0;
        }

        private bool m_scalingChanged = false;
        MapIconCache m_icon = new MapIconCache(); //Cache one icon, normally the same for all in a trail
        private bool m_routeSettingsChanged = false;
        private IDictionary<IGPSPoint, IMapOverlay> m_pointOverlays = new Dictionary<IGPSPoint, IMapOverlay>();
        private IDictionary<IList<IGPSPoint>, IMapOverlay> m_routeOverlays = new Dictionary<IList<IGPSPoint>, IMapOverlay>();

        private IList<TrailGPSLocation> m_TrailPoints = new List<TrailGPSLocation>();
        private IList<SplitGPSLocation> m_SplitPoints = new List<SplitGPSLocation>();
        private IDictionary<string, MapPolyline> m_TrailRoutes = new Dictionary<string, MapPolyline>();
        private IDictionary<string, MapPolyline> m_MarkedTrailRoutes = new Dictionary<string, MapPolyline>();
        //Rendered w standard ST ovelay, needed here when zooming
        private IDictionary<string, MapPolyline> m_MarkedTrailRoutesNoShow = new Dictionary<string, MapPolyline>();
        private bool m_showPage;
        private static IDictionary<string, TrailPointsLayer> m_layers = new Dictionary<string, TrailPointsLayer>();
        private TrailPointsLayer m_extraMapLayer = null;
        private PointMapMarker _selectedPointMoving = null;
        private TrailGPSLocation _selectedPointOriginal = null;
        private Point m_clickToCenterOffset = Point.Empty;
        private EditTrail m_editTrail = null;
    }
}
#endif