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
using ZoneFiveSoftware.Common.Data.Fitness;
using System.Collections.Generic;
using Microsoft.Win32;
using TrailsPlugin.Data;

namespace TrailsPlugin.UI.MapLayers
{
    public class TrailMapPolyline : MapPolyline
    {
        private TrailResult m_trailResult;
        private string m_key;
        private TrailMapPolyline(IList<IGPSPoint> g, int w, Color c, TrailResult tr)
            : base(g, w, c)
        {
            m_trailResult = tr;
            m_key = tr.Activity + ":" + tr.Order;
        }
        private TrailMapPolyline(IList<IGPSPoint> g, int w, Color c, TrailResult tr, string tkey)
            : this(g, w, c, tr)
        {
            m_key += tkey;
        }

        //Complete trail
        public TrailMapPolyline(TrailResult tr)
            : this(tr.GpsPoints(), GetApplication().SystemPreferences.RouteSettings.RouteWidth, tr.TrailColor, tr)
        { }

        //Marked part of a track
        public static IList<TrailMapPolyline> GetTrailMapPolyline(TrailResult tr, TrailsItemTrackSelectionInfo sel)
        {
            IList<TrailMapPolyline> results = new List<TrailMapPolyline>();
            foreach (IList<IGPSPoint> gp in tr.GpsPoints(sel))
            {
                results.Add(new TrailMapPolyline(gp, GetApplication().SystemPreferences.RouteSettings.RouteWidth * 2, MarkedColor(tr.TrailColor), tr, "m" + results.Count));
            }
            return results;
        }

        private static Color MarkedColor(Color tColor)
        {
            //The Marked color can be adjusted, but several overlays makes it hard to find which is which
            //return ControlPaint.Dark(tColor);
            return tColor;
        }
        public TrailResult TrailRes
        {
            get { return m_trailResult; }
        }
        public string key { get { return m_key; } }
        private static IApplication GetApplication()
        {
#if GPSRUNNING_UNIQUEROUTES||GPSRUNNING_OVERLAY
            return GpsRunningPlugin.Plugin.GetApplication();
#elif MATRIXPLUGIN
            return MatrixPlugin.MatrixPlugin.GetApplication();
#else // TRAILSPLUGIN
            return PluginMain.GetApplication();
#endif
        }
            
    }
}
