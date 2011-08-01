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
                //One layer for normal tracks, another for marked tracks (should be above tracks)
                return new IRouteControlLayerProvider[]
                { 
                    new TrailPointsProvider(TrailPointsProvider.TrailsLayerZOrderBase)
#if !(GPSRUNNING_UNIQUEROUTES||GPSRUNNING_OVERLAY||GPSRUNNING_HIGHSCORE||GPSRUNNING_PERFORMANCEPREDICTOR||MATRIXPLUGIN)
                    , new TrailPointsProvider(TrailPointsProvider.TrailsLayerZOrderMarked)
#endif
                };
            }
        }
    }
    class TrailPointsProvider : IRouteControlLayerProvider
    {
        public const int TrailsLayerZOrderBase = 1;
        public const int TrailsLayerZOrderMarked = 5;
        private int m_zorder = TrailsLayerZOrderBase;

        public TrailPointsProvider(int zorder)
        {
            m_zorder = zorder;
        }
        private IRouteControlLayer m_layer = null;
        public IRouteControlLayer CreateControlLayer(IRouteControl control)
        {
            if (m_layer == null)
            {
                m_layer = new TrailPointsLayer(this, control, m_zorder);
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
#if GPSRUNNING_UNIQUEROUTES||GPSRUNNING_OVERLAY||GPSRUNNING_HIGHSCORE||GPSRUNNING_PERFORMANCEPREDICTOR
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