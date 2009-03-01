using System;
using System.Drawing;
using System.Windows.Forms;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Visuals.Fitness.GPS;
using System.Collections.Generic;
using Microsoft.Win32;

namespace TrailsPlugin.UI.MapLayers {
	class MapControlLayer : IMapControlLayer {

		public event System.EventHandler SelectedGPSLocationsChanged;

		private bool m_CaptureSelectedGPSLocations;
		private IList<IGPSLocation> m_SelectedGPSLocations = new List<IGPSLocation>();
		private IList<IGPSLocation> m_HighlightedGPSLocations = new List<IGPSLocation>();
		private bool m_ShowHighlight = false;

		private static MapControlLayer m_instance = null;
		public static MapControlLayer Instance {
			get {
				if (MapControlLayer.m_instance == null) {
					MapControlLayer.m_instance = new MapControlLayer();
				}
				return MapControlLayer.m_instance;
			}
		}

		private MapControlLayer() {
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
			if (mapControl.Selected.Count > 0) {
				IMapControlObject[] selectedMapControlObjects = new IMapControlObject[mapControl.Selected.Count];
				mapControl.Selected.CopyTo(selectedMapControlObjects, 0);

				for (int i = 0; i < mapControl.Selected.Count; i++) {
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
			m_mapControl.Refresh();
		}

		public IList<IGPSLocation> SelectedGPSLocations {
			get {
				return m_SelectedGPSLocations;
			}
		}

		public IList<IGPSLocation> HighlightedGPSLocations {
			set {
				m_HighlightedGPSLocations = value;
			}
			get {
				return m_HighlightedGPSLocations;
			}
		}

		public bool ShowHighlight {
			set {
				m_ShowHighlight = value;
				m_mapControl.Refresh();
			}
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

			if (m_ShowHighlight) {
				//drawContext.Center	
				
				foreach (IGPSLocation gpsLocation in m_HighlightedGPSLocations) {
					Point point = drawContext.Projection.GPSToPixel(drawContext.Center, drawContext.ZoomLevel, gpsLocation );
					Pen pen = new Pen(Color.Red, 5.0F);
					drawContext.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
					int X = point.X + (drawContext.DrawRectangle.Width / 2) - 1;
					int Y = point.Y + (drawContext.DrawRectangle.Height / 2) - 1;
					drawContext.Graphics.DrawEllipse(pen, X, Y, 1, 1);					
				}
			}
		}

		public ICollection<IMapControlObject> HitTest(Rectangle rectClient, IMapDrawContext drawContext) {
			throw new NotImplementedException();
		}

		public IMapControlObject HitTest(Point ptClient, bool bSelect, IMapDrawContext drawContext, out Cursor cursor) {
			cursor = Cursors.Default;
			return null;
		}

		public System.Guid Id {
			get {
				return GUIDs.MapControLayer;
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

		private class MapControlObject : IMapControlObject {

			#region IMapControlObject Members

			public void DrawHighlight(IMapDrawContext drawContext) { }

			public void DrawInfo(IMapDrawContext drawContext, System.Drawing.Rectangle infoRect, bool bSelected) { }

			public void DrawSelected(IMapDrawContext drawContext) { }

			public Size InfoSize(IMapDrawContext drawContext, bool bSelected) {
				return new Size();
			}

			public Rectangle PixelBounds(IMapDrawContext drawContext) {
				return new Rectangle();
			}

			#endregion
		}
	}
}
