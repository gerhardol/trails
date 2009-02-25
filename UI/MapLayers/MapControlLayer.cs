using System;
using System.Drawing;
using System.Windows.Forms;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Visuals.Fitness.GPS;
using System.Collections.Generic;
using Microsoft.Win32;

namespace TrailsPlugin.UI.MapLayers {
	class MapControlLayer : IMapControlLayer {

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

		#region IMapControlLayer Members

		public void Draw(IMapDrawContext drawContext) {
			IMapControlObject moo = drawContext.Highlight;
			if (moo != null) {
				int a = 5;
			}
		}

		public ICollection<IMapControlObject> HitTest(Rectangle rectClient, IMapDrawContext drawContext) {
			throw new NotImplementedException();
		}

		public IMapControlObject HitTest(Point ptClient, bool bSelect, IMapDrawContext drawContext, out Cursor cursor) {
			cursor = Cursors.Arrow;
			if (bSelect) {
				IMapControlObject x = drawContext.Highlight;
			}

			return new MapControlObject();
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

/*
		private bool m_HaveEnteredControl;
//		private MapControlLayer.HitEventHandler m_HitEventHandler;
		private static MapControlLayer m_instance;
		private Cursor m_cursor;
		private Point m_ButtonDownPosition;
//		private Microsoft.Win32.MouseHook m_MouseHook;

		public static MapControlLayer Instance {
			get {
				if (MapControlLayer.m_instance == null)
					MapControlLayer.m_instance = new MapControlLayer();
				return MapControlLayer.m_instance;
			}
		}

		private MapControlLayer() {			
            m_cursor = Cursors.Cross;
			/*
			 * m_ButtonDownPosition = Point.Empty;
            MyMouseHook = new MouseHook();
            MyMouseHook.MouseDown += new MouseHookEventHandler(MyMouseHook_MouseDown);
            MyMouseHook.MouseMove += new MouseHookEventHandler(MyMouseHook_MouseMove);
            MyMouseHook.MouseUp += new MouseHookEventHandler(MyMouseHook_MouseUp);
            MyMouseHook.Install();
			 
        }

//		public event MapControlLayer.DrawingEventHandler Drawing;
/*		public event MapControlLayer.HitEventHandler Hit {
			add {
				MyHitEventHandler += value;
				MyHaveEnteredControl = false;
			}
		}

		#region IMapControlLayer Members

		public void Draw(IMapDrawContext drawContext) {
			throw new System.NotImplementedException();
		}

		public System.Collections.Generic.ICollection<IMapControlObject> HitTest(System.Drawing.Rectangle rectClient, IMapDrawContext drawContext) {
			throw new System.NotImplementedException();
		}

		public IMapControlObject HitTest(System.Drawing.Point ptClient, bool bSelect, IMapDrawContext drawContext, out System.Windows.Forms.Cursor cursor) {
			throw new System.NotImplementedException();
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


//		public delegate void DrawingEventHandler(object sender, MapControlLayer.DrawingEventArgs e);
//		public delegate void HitEventHandler(object sender, MapControlLayer.HitEventArgs e);

		public class DrawEventArgs : EventArgs {

			private IMapDrawContext MyDrawContext;

			public IMapDrawContext DrawContext {
				get {
					return MyDrawContext;
				}
			}

			public DrawEventArgs(IMapDrawContext drawContext) {
				MyDrawContext = drawContext;
			}

		}

		public class DrawingEventArgs : EventArgs {

			private IMapDrawContext m_MapDrawContext;

			public IMapDrawContext MapDrawContext {
				get {
					return m_MapDrawContext;
				}
			}

			public DrawingEventArgs(IMapDrawContext mapDrawContext) {
				m_MapDrawContext = mapDrawContext;
			}
		}

		private class HitEventArgs : EventArgs {
			private IGPSLocation m_Location;
			private IMapControl m_MapControl;

			public IGPSLocation Location {
				get {
					return m_Location;
				}
			}

			public IMapControl MapControl {
				get {
					return m_MapControl;
				}
			}

			public HitEventArgs(IGPSLocation location, IMapControl mapControl) {
				m_Location = location;
				m_MapControl = mapControl;
			}
		}


*/