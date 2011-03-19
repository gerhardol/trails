/*
Copyright (C) 2010 Aaron Averill

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

//ST_3_0: Display trail points

#if !ST_2_1
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Visuals.Mapping;

namespace TrailsPlugin.UI.MapLayers
{
    public class RouteControlLayerBase
    {
        public RouteControlLayerBase(IRouteControlLayerProvider provider, IRouteControl control, int zOrder)
        {
            this.provider = provider;
            this.control = control;
            this.zOrder = zOrder;
            this.mapControl = control.MapControl;
            //AddMapControlEventHandlers();
            control.Resize += new EventHandler(OnRouteControlResize);
            control.VisibleChanged += new EventHandler(OnRouteControlVisibleChanged);
            control.MapControlChanged += new EventHandler(OnRouteControlMapControlChanged);
            control.ItemsChanged += new EventHandler(OnRouteControlItemsChanged);
            control.SelectedItemsChanged += new EventHandler(OnRouteControlSelectedItemsChanged);
            control.Disposed += delegate(object sender, EventArgs e)
            {
                //RemoveMapControlEventHandlers();
                control.Resize -= new EventHandler(OnRouteControlResize);
                control.VisibleChanged -= new EventHandler(OnRouteControlVisibleChanged);
                control.MapControlChanged -= new EventHandler(OnRouteControlMapControlChanged);
                control.ItemsChanged -= new EventHandler(OnRouteControlItemsChanged);
                control.SelectedItemsChanged -= new EventHandler(OnRouteControlSelectedItemsChanged);
            };
        }

        public IRouteControlLayerProvider Provider
        {
            get { return provider; }
        }

        public int ZOrder
        {
            get { return zOrder; }
        }

        protected IRouteControl RouteControl
        {
            get { return control; }
        }

        protected IMapControl MapControl
        {
            get { return mapControl; }
        }

        protected bool MapControlChanged
        {
            get { return mapControl != control.MapControl; }
        }

        protected void ResetMapControl()
        {
            RemoveMapControlEventHandlers();
            this.mapControl = control.MapControl;
            AddMapControlEventHandlers();
        }

        protected IGPSBounds MapControlBounds
        {
            get
            {
                int mapControlWidth = mapControl.Control.ClientRectangle.Width;
                int mapControlHeight = mapControl.Control.ClientRectangle.Height;

                IGPSLocation topLeft = mapControl.MapProjection.PixelToGPS(mapControl.Center, mapControl.Zoom, new Point(-mapControlWidth / 2 - 10, -mapControlHeight / 2 - 10));
                IGPSLocation bottomRight = mapControl.MapProjection.PixelToGPS(mapControl.Center, mapControl.Zoom, new Point(mapControlWidth / 2 + 10, mapControlHeight / 2 + 10));
                double maxWidth = mapControl.MapProjection.GPSToPixel(new GPSPoint(0, 0, float.NaN), mapControl.Zoom, new GPSPoint(0, 179.9999F, float.NaN)).X * 2;
                if (maxWidth > 10 && mapControlWidth >= (maxWidth - 20))
                {
                    topLeft = new GPSLocation(topLeft.LatitudeDegrees, -180);
                    bottomRight = new GPSLocation(bottomRight.LatitudeDegrees, 179.999F);
                }
                return new GPSBounds(topLeft, bottomRight);
            }
        }

        protected virtual void OnMapControlZoomChanged(object sender, EventArgs e)
        {
        }

        protected virtual void OnMapControlCenterMoveEnd(object sender, EventArgs e)
        {
        }

        protected virtual void OnRouteControlResize(object sender, EventArgs e)
        {
        }

        protected virtual void OnRouteControlVisibleChanged(object sender, EventArgs e)
        {
        }

        protected virtual void OnRouteControlMapControlChanged(object sender, EventArgs e)
        {
        }

        protected virtual void OnRouteControlItemsChanged(object sender, EventArgs e)
        {
        }

        protected virtual void OnRouteControlSelectedItemsChanged(object sender, EventArgs e)
        {
        }

        //protected void UpdateMapInfoBox(ref MapInfoBox infoBox, IGPSPoint location, string caption, string[] infoLines, string imageUrl)
        //{
        //    if (infoBox == null)
        //    {
        //        infoBox = new MapInfoBox(location, caption, infoLines, imageUrl);
        //        mapControl.AddOverlay(infoBox);
        //    }
        //    else
        //    {
        //        infoBox.Location = location;
        //        infoBox.SetContent(caption, infoLines, imageUrl);
        //        infoBox.Visible = true;
        //    }
        //}

        protected virtual void AddMapControlEventHandlers()
        {
            mapControl.ZoomChanged += new EventHandler(OnMapControlZoomChanged);
            mapControl.CenterMoveEnd += new EventHandler(OnMapControlCenterMoveEnd);
        }

        protected virtual void RemoveMapControlEventHandlers()
        {
            mapControl.ZoomChanged -= new EventHandler(OnMapControlZoomChanged);
            mapControl.CenterMoveEnd -= new EventHandler(OnMapControlCenterMoveEnd);
        }

        private IRouteControlLayerProvider provider;
        private IRouteControl control;
        private int zOrder = 0;
        private IMapControl mapControl;
    }
}
#endif