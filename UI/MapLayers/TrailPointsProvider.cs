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

//ST_3_0: Extend Map layers

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
                return new IRouteControlLayerProvider[] { new TrailPointsProvider() };
            }
        }
    }
    class TrailPointsProvider : IRouteControlLayerProvider
    {
        private IRouteControlLayer m_layer = null;
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
            get
            {
#if GPSRUNNING_UNIQUEROUTES||GPSRUNNING_OVERLAY
                return GpsRunningPlugin.Properties.Resources.TrailPointsControlLayer; 
#elif MATRIXPLUGIN
                return MatrixPlugin.Properties.Resources.TrailPointsControlLayer;
#else // TRAILSPLUGIN
                return Properties.Resources.TrailPointsControlLayer;
#endif
            }
        }
    }
}
#endif