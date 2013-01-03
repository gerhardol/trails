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
            private TrailGPSLocation trailPoint;
            public PointMapMarker(IGPSPoint location, MapIcon icon, bool clickable, TrailGPSLocation Wp)
                : base(location, icon, clickable)
            {
                trailPoint = Wp;
            }

            public TrailGPSLocation TrailPoint
            {
                get
                {
                    return trailPoint;
                }
                set
                {
                    this.trailPoint = value;
                }
            }
            public override string ToString()
            {
                return trailPoint.ToString();
            }
        }

        public TrailPointsLayer(IRouteControlLayerProvider provider, IRouteControl control, int zorder, bool mouseEvents)
            : base(provider, control, zorder, mouseEvents)
        {
            Guid currentView = UnitUtil.GetApplication().ActiveView.Id;
            string key = currentView.ToString()+zorder;
            if (m_layers.ContainsKey(key))
            {
                m_layers[key].m_extraMapLayer = this;
            }
            else
            {
                m_layers[key] = this;
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
            //if (!pages.ContainsKey(result))
            //{
            //    pages[result] = page;
            //}
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
                m_TrailPoints = value;
                //Change scaling, if radius changes
                m_scalingChanged = true;
                RefreshOverlays(true);
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
                    //Assume all points have the same radius
                    float highlightRadius = value[0].Radius;
                    GPSBounds area = TrailGPSLocation.getGPSBounds(value, 4 * highlightRadius);
                    this.SetLocation(area);
                    //m_SelectedTrailPoints = value;
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
            //    IDictionary<string, MapPolyline> result = new Dictionary<string, MapPolyline>();
            //    foreach (KeyValuePair<string, MapPolyline> t in m_MarkedTrailRoutes)
            //    {
            //        result.Add(t);
            //    }
            //    foreach (KeyValuePair<string, MapPolyline> t in m_MarkedTrailRoutesNoShow)
            //    {
            //        result.Add(t);
            //    }
            //    return result;
            //}
            set
            {
                m_MarkedTrailRoutes = value;
                //if (value.Count > 0)
                //{
                //    IGPSBounds area = TrailMapPolyline.getGPSBounds(value);
                //    SetLocation(area);
                //}
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
            if (m_TrailRoutes.Count > 0)
            {
                IGPSBounds area1 = TrailMapPolyline.getGPSBounds(m_TrailRoutes);
                if (m_TrailPoints.Count > 0)
                {
                    //All points currently have the same radius
                    float highlightRadius = m_TrailPoints[0].Radius;
                    IGPSBounds area2 = TrailGPSLocation.getGPSBounds(m_TrailPoints, 2 * highlightRadius);
                    area1 = Union(area1, area2);
                }
                DoZoom(area1);
            }
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

        public void DoZoomMarkedTracks()
        {
            IGPSBounds area = Union(TrailMapPolyline.getGPSBounds(m_MarkedTrailRoutes),
                TrailMapPolyline.getGPSBounds(m_MarkedTrailRoutesNoShow));
            DoZoom(area);
        }

        public void SetLocationMarkedTracks()
        {
            IGPSBounds area = Union(TrailMapPolyline.getGPSBounds(m_MarkedTrailRoutes),
                TrailMapPolyline.getGPSBounds(m_MarkedTrailRoutesNoShow));
            SetLocation(area);
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
        //public float HighlightRadius
        //{
        //    set
        //    {
        //        if (m_highlightRadius != value)
        //        {
        //            m_scalingChanged = true;
        //        }
        //        m_highlightRadius = value;
        //    }
        //}

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
                if (_selectedPointMoving != null)
                {
                    if (this._selectedPointMoving != null &&
                        this._selectedWaypointOriginalLocation != null)
                    {
                        //New selection before old finished - set old location back
                        IGPSPoint p = this._selectedWaypointOriginalLocation;
                        if (p != null)
                        {
                            this._selectedPointMoving.TrailPoint = new TrailGPSLocation(this._selectedPointMoving.TrailPoint, p);
                            this.m_editTrail.UpdatePointFromMap(_selectedPointMoving.TrailPoint);
                        }
                        //pages[this].UpdatePointFromMap(_selectedPointMoving.TrailPoint);
                    }
                }

                if (waypoint != null)
                {
                    //Start - offset saved separately
                    this._selectedWaypointOriginalLocation = waypoint.Location;

                    _selectedPointMoving = waypoint;
                    this.m_editTrail.UpdatePointFromMap(_selectedPointMoving.TrailPoint);
                    //pages[this].UpdatePointFromMap(_selectedPointMoving.TrailPoint);
                    MapControl.CanMouseDrag = false;
                }
                else
                {
                    //End or cancel
                    _selectedPointMoving = null;
                    this._selectedWaypointOriginalLocation = null;
                    MapControl.CanMouseDrag = true;
                    RefreshOverlays(true);
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
                        Point midPoint = MapControl.MapProjection.GPSToPixel(MapControl.MapBounds.NorthWest,
                            MapControl.Zoom, selectedPoint.Location);
                        m_clickToCenterOffset = new Point(midPoint.X - e.Location.X, midPoint.Y - e.Location.Y);
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
                this._selectedWaypointOriginalLocation = null;
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
                    if (l != null)
                    {
                        _selectedPointMoving.TrailPoint = new TrailGPSLocation(_selectedPointMoving.TrailPoint, l);
                        this.m_editTrail.UpdatePointFromMap(_selectedPointMoving.TrailPoint);
                        //pages[this].UpdatePointFromMap(_selectedPointMoving.TrailPoint);

                        //Refresh this point. Only seem to be possible with refreshing all
                        RefreshOverlays(true);
                        //MapControl.AddOverlay(_selectedWaypointMoving);
                        //MapControl.RefreshMap();
                    }
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

        /// <summary>
        /// Get a radius for a trail matching a minimum number of pixels
        /// </summary>
        /// <param name="minPixels"></param>
        /// <returns></returns>
        public float getRadius(int minPixels)
        {
            const int circlePixelSize = 1000;
            IGPSPoint point0 = Utils.GPS.LocationToPoint(this.MapControl.MapProjection.PixelToGPS(this.MapControl.Center, this.MapControl.Zoom,
                new Point(0, 0)));
            IGPSPoint pointX = Utils.GPS.LocationToPoint(this.MapControl.MapProjection.PixelToGPS(this.MapControl.Center, this.MapControl.Zoom,
                new Point(circlePixelSize / 2, 0)));
            IGPSPoint pointY = Utils.GPS.LocationToPoint(this.MapControl.MapProjection.PixelToGPS(this.MapControl.Center, this.MapControl.Zoom,
                new Point(0, circlePixelSize / 2)));
            float radius = minPixels * Math.Max(point0.DistanceMetersToPoint(pointX), point0.DistanceMetersToPoint(pointY)) / (float)circlePixelSize;
            return radius;
        }

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
            if ((m_scalingChanged || null == m_icon) && m_TrailPoints.Count > 0)
            {
                //All points have the same radius, at least now
                float highlightRadius = m_TrailPoints[0].Radius;
                m_icon = getCircle(this.MapControl, highlightRadius);
            }
            IDictionary<IGPSPoint, IMapOverlay> newPointOverlays = new Dictionary<IGPSPoint, IMapOverlay>();
            foreach (TrailGPSLocation location in m_TrailPoints)
            {
                IGPSPoint point = location;
                PointMapMarker pointOverlay = new PointMapMarker(point, m_icon, MouseEvents, location);
                if (MouseEvents)
                {
                    pointOverlay.MouseDown += new MouseEventHandler(pointOverlay_MouseDown);
                    pointOverlay.MouseUp += new MouseEventHandler(pointOverlay_MouseUp);
                }
                newPointOverlays.Add(point, pointOverlay);
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
                }catch(Exception){}
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

        private bool m_scalingChanged = false;
        MapIcon m_icon = null;
        private bool m_routeSettingsChanged = false;
        private IDictionary<IGPSPoint, IMapOverlay> m_pointOverlays = new Dictionary<IGPSPoint, IMapOverlay>();
        private IDictionary<IList<IGPSPoint>, IMapOverlay> m_routeOverlays = new Dictionary<IList<IGPSPoint>, IMapOverlay>();
        //private RouteItemsDataChangeListener listener;

        private IList<TrailGPSLocation> m_TrailPoints = new List<TrailGPSLocation>();
        //private IList<TrailGPSLocation> m_SelectedTrailPoints = new List<TrailGPSLocation>();
        private IDictionary<string, MapPolyline> m_TrailRoutes = new Dictionary<string, MapPolyline>();
        private IDictionary<string, MapPolyline> m_MarkedTrailRoutes = new Dictionary<string, MapPolyline>();
        //Rendered w standard ST ovelay, needed here when zooming
        private IDictionary<string, MapPolyline> m_MarkedTrailRoutesNoShow = new Dictionary<string, MapPolyline>();
        //private float m_highlightRadius;
        private bool m_showPage;
        private static IDictionary<string, TrailPointsLayer> m_layers = new Dictionary<string, TrailPointsLayer>();
        private TrailPointsLayer m_extraMapLayer = null;
        //private static IDictionary<TrailPointsLayer, ActivityDetailPageControl> pages = new Dictionary<TrailPointsLayer, ActivityDetailPageControl>();
        private PointMapMarker _selectedPointMoving = null;
        private IGPSPoint _selectedWaypointOriginalLocation = null;
        private Point m_clickToCenterOffset = Point.Empty;
        private EditTrail m_editTrail = null;
    }
}
#endif