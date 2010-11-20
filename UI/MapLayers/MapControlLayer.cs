/*
Copyright (C) 2009 Brendan Doherty

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

//ST_2_1: Both display/select

#if ST_2_1
using System;
using System.Drawing;
using System.Windows.Forms;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Visuals.Fitness.GPS;
using System.Collections.Generic;
using Microsoft.Win32;
using TrailsPlugin.Data;

namespace TrailsPlugin.UI.MapLayers {
    public class MapPolyline {
        //Dummy for ST3 compile
        public MapPolyline(IList<IGPSPoint> g, int w, Color c) { }
        public event MouseEventHandler Click;
    };
	public class MapControlLayer : IMapControlLayer
    {
		public event System.EventHandler SelectedGPSLocationsChanged;

		private bool m_CaptureSelectedGPSLocations;
		private IList<IGPSLocation> m_SelectedGPSLocations = new List<IGPSLocation>();
        private IList<TrailGPSLocation> m_TrailPoints = new List<TrailGPSLocation>();
		private float m_highlightRadius;
        private static bool _showPage;

        public bool ShowPage
        {
            //get { return _showPage; }
            set
            {
                bool changed = (value != _showPage);
                _showPage = value;
                if (!value)
                {
                    m_CaptureSelectedGPSLocations = false;
                }
                if (changed)
                {
                    Refresh();
                }
            }
        }
        
        private static MapControlLayer m_instance = null;
		public static MapControlLayer Instance {
			get {
				if (MapControlLayer.m_instance == null) {
					MapControlLayer.m_instance = new MapControlLayer();
				}
				return MapControlLayer.m_instance;
            }
		}

		private IMapControl m_mapControl;
		public IMapControl MapControl {
			set {
				m_mapControl = value;
			}
			get {
                return m_mapControl;
			}
        }

		private IList<IGPSLocation> getSelectedGPSLocations(IMapDrawContext drawContext) {
			IList<IGPSLocation> list = new List<IGPSLocation>() { };
			IMapControl mapControl = UI.MapLayers.MapControlLayer.Instance.MapControl;
             
			ICollection<IMapControlObject> selectedGPS = mapControl.Selected;
			if (selectedGPS.Count > 0) {
            IMapControlObject[] selectedMapControlObjects = new IMapControlObject[selectedGPS.Count];
				selectedGPS.CopyTo(selectedMapControlObjects, 0);
				for (int i = 0; i < selectedGPS.Count; i++) {
					Rectangle rec = selectedMapControlObjects[i].PixelBounds(drawContext);

					int X = rec.X + (rec.Width / 2) - (drawContext.DrawRectangle.Width / 2);
					int Y = rec.Y + (rec.Height / 2) - (drawContext.DrawRectangle.Height / 2);
					IGPSLocation loc = drawContext.Projection.PixelToGPS(drawContext.Center, drawContext.ZoomLevel, new Point(X, Y));
					list.Add(loc);
				}
            }
			return list;
		}

		public void CaptureSelectedGPSLocations() {
			m_SelectedGPSLocations.Clear();
			m_CaptureSelectedGPSLocations = true;
			if (null!=m_mapControl) m_mapControl.Refresh();
		}

		public IList<IGPSLocation> SelectedGPSLocations {
			get {
				return m_SelectedGPSLocations;
			}
		}

        public IList<TrailGPSLocation> TrailPoints
        {
            set
            {
                m_TrailPoints = value;
            }
            get
            {
                return m_TrailPoints;
            }
        }
        //Dummy for ST3 compile
        public IDictionary<string, MapPolyline> TrailRoutes
        {
            set
            {
            }
            get
            {
                return new Dictionary<string, MapPolyline>();
            }
        }
        public IDictionary<string, MapPolyline> MarkedTrailRoutes
        {
            set
            {
            }
            get
            {
                return new Dictionary<string, MapPolyline>();
            }
        }

        //No effect in ST2 (zoom in ST3 only)
        public IList<TrailGPSLocation> SelectedTrailPoints
        {
            get
            {
                return new List<TrailGPSLocation>();
            }
            set
            {
            }
        }

        public float HighlightRadius
        {
			set {
				m_highlightRadius = value;
			}
		}

        public void Refresh()
        {
            if (null != m_mapControl) m_mapControl.Refresh();
        }

#region IMapControlLayer Members

		public void Draw(IMapDrawContext drawContext) {
			if (m_CaptureSelectedGPSLocations) {
				m_CaptureSelectedGPSLocations = false;
				if (m_SelectedGPSLocations.Count != MapControl.Selected.Count) {
					m_SelectedGPSLocations = getSelectedGPSLocations(drawContext);
					SelectedGPSLocationsChanged(this, new System.EventArgs());
				}
            }

			if (_showPage) {
				//drawContext.Center
	
				IGPSLocation loc1 = drawContext.Projection.PixelToGPS(drawContext.Center, drawContext.ZoomLevel, new Point(0,0));
				IGPSLocation loc2 = drawContext.Projection.PixelToGPS(drawContext.Center, drawContext.ZoomLevel, new Point(0,100));
				IGPSPoint point1 = Utils.GPS.LocationToPoint(loc1);
				IGPSPoint point2 = Utils.GPS.LocationToPoint(loc2);
				float meters = point1.DistanceMetersToPoint(point2) / 100;
				float radiusInPixels = m_highlightRadius / meters;

                foreach (TrailGPSLocation gpsLocation in m_TrailPoints)
                {
					Point point = drawContext.Projection.GPSToPixel(drawContext.Center, drawContext.ZoomLevel, gpsLocation.GpsLocation);
					Pen pen = new Pen(Color.Red, 5.0F);
					
					drawContext.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
					float X = point.X + (drawContext.DrawRectangle.Width / 2) - radiusInPixels;
					float Y = point.Y + (drawContext.DrawRectangle.Height / 2) - radiusInPixels;
                    if (X > 0 && X < 1000 && Y > 0 && Y < 1000)
                    {
                        //Prevent crashes at large elipses
                        drawContext.Graphics.DrawEllipse(pen, X, Y, radiusInPixels * 2, radiusInPixels * 2);
                    }
				}
			}
		}

        public ICollection<IMapControlObject> HitTest(Rectangle rectClient, IMapDrawContext drawContext) {
            if (_showPage)
            {
                //No select by rect in Trails page, just individual points
                throw new NotImplementedException();
            }
            else
            {
                return null;
            }
 		}

		public IMapControlObject HitTest(Point ptClient, bool bSelect, IMapDrawContext drawContext, out Cursor cursor) {
			cursor = Cursors.Default;
			return null;
		}

		public System.Guid Id {
			get {
				return GUIDs.MapControlLayer;
			}
		}

		public string Name {
			get {
				return "Click Map Layer";
			}
		}

		public string Path {
			get {
				return null;
			}
		}

#endregion

        private class MapControlObject : IMapControlObject
        {

#region IMapControlObject Members

            public void DrawHighlight(IMapDrawContext drawContext) { }

            public void DrawInfo(IMapDrawContext drawContext, System.Drawing.Rectangle infoRect, bool bSelected) { }

            public void DrawSelected(IMapDrawContext drawContext) { }

            public Size InfoSize(IMapDrawContext drawContext, bool bSelected)
            {
                return new Size();
            }

            public Rectangle PixelBounds(IMapDrawContext drawContext)
            {
                return new Rectangle();
            }

#endregion
        }
		
    }
}
#endif
