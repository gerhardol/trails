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

//ST_3_0: Display TrailPoints, not included for ST_2_1 

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
            get
            {
                return m_TrailPoints;
            }
            set
            {
                bool changed = false;
                if (!value.Equals(m_TrailPoints)) { changed = true; }
                m_TrailPoints = value;
                if (changed) { RefreshOverlays(); }
            }
        }
        public IList<TrailGPSLocation> SelectedTrailPoints
        {
            get
            {
                return m_SelectedTrailPoints;
            }
            set
            {
                //Currently just zoom to selection
                this.MapControl.SetLocation(value[0].GpsLocation,
                this.MapControl.ComputeZoomToFit(new GPSBounds(value[0].GpsLocation, value[0].GpsLocation)));
                m_SelectedTrailPoints = value;
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

        public void Refresh()
        {
            //Should not be necessary in ST3, updated when needed
            RefreshOverlays(); 
        }
        public bool ShowPage
        {
            get { return _showPage; }
            set
            {
                bool changed = (value != _showPage);
                _showPage = value;
                if (changed)
                {
                    RefreshOverlays();
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
            if (MapControlChanged)
            {
                ClearOverlays();
                ResetMapControl();
            }

            if (!_showPage) return;

            IGPSBounds windowBounds = MapControlBounds;

            IList<IGPSPoint> visibleLocations = new List<IGPSPoint>();
            foreach (TrailGPSLocation point in m_TrailPoints)
            {
                if (windowBounds.Contains(point.GpsLocation))
                {
                    visibleLocations.Add(Utils.GPS.LocationToPoint(point.GpsLocation));
                }
            }
            if (0 == visibleLocations.Count) return;

            IDictionary<IGPSPoint, IMapOverlay> newPointOverlays = new Dictionary<IGPSPoint, IMapOverlay>();
            IList<IMapOverlay> addedOverlays = new List<IMapOverlay>();

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
                    if (m_scalingChanged || null == m_icon)
                    {
                        m_icon = getCircle(this.MapControl, m_highlightRadius);
                    }
                    MapMarker pointOverlay = new MapMarker(location, m_icon, false);
                    newPointOverlays.Add(location, pointOverlay);
                    addedOverlays.Add(pointOverlay);
                    m_scalingChanged = false;
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

        private bool m_scalingChanged = false;
        MapIcon m_icon = null;
        private bool routeSettingsChanged = false;
        private IDictionary<IGPSPoint, IMapOverlay> pointOverlays = new Dictionary<IGPSPoint, IMapOverlay>();

        //private RouteItemsDataChangeListener listener;

        private IList<TrailGPSLocation> m_TrailPoints = new List<TrailGPSLocation>();
        private IList<TrailGPSLocation> m_SelectedTrailPoints = new List<TrailGPSLocation>();
        private float m_highlightRadius;
        private static bool _showPage;
    }
}
