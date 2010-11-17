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

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Visuals.Fitness;
#if !ST_2_1
using ZoneFiveSoftware.Common.Visuals.Mapping;
#endif
using System.Collections.Generic;
using Microsoft.Win32;
using TrailsPlugin.Data;

namespace TrailsPlugin.UI.MapLayers
{
    public class TrailMapPolyline : MapPolyline
    {
        private TrailResult m_trailResult;
        private string m_key;
        public TrailMapPolyline(IList<IGPSPoint> g, int w, Color c, TrailResult tr)
            : base(g, w, c)
        {
            m_trailResult = tr;
            m_key = tr.Activity + ":" + tr.Order;
        }
        //Marked part of a track
        public TrailMapPolyline(TrailResult tr, TrailsItemTrackSelectionInfo sel)
            : this(tr.GpsPoints(sel), PluginMain.GetApplication().SystemPreferences.RouteSettings.RouteWidth * 2, tr.TrailColor, tr)
        { m_key += "m" + sel.ToString(); }
        //Complete trail
        public TrailMapPolyline(TrailResult tr)
            : this(tr.GpsPoints(), PluginMain.GetApplication().SystemPreferences.RouteSettings.RouteWidth, tr.TrailColor, tr)
        { }

        public TrailResult TrailRes
        {
            get { return m_trailResult; }
        }
        public string key { get { return m_key; } }
    }
}
