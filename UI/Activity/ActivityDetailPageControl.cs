using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ZoneFiveSoftware.Common.Visuals.Fitness.GPS;

namespace TrailsPlugin.UI.Activity {
	public partial class ActivityDetailPageControl : UserControl {
		
		private IList<IActivity> m_activities;
		private IList<TrailDetailsRow> m_trailData;


		public ActivityDetailPageControl(IList<IActivity> activities) {
			InitializeComponent();
			m_activities = activities;
			m_trailData = new List<TrailDetailsRow>();

			this.listTrailPoint.Columns.Add(new TreeList.Column("longitude", "Longitude", 150, StringAlignment.Near));
			this.listTrailPoint.Columns[0].CanSelect = false;
			this.listTrailPoint.Columns.Add(new TreeList.Column("latitude", "Latitude", 150, StringAlignment.Near));
			this.listTrailPoint.Columns[1].CanSelect = false;

			RefreshData();
		}

		public IActivity[] Activities {
			set {
				m_activities = value;
				RefreshData();
			}
		}

		public void RefreshData() {

			bool enabled = (m_activities.Count != 0);
						
			this.txtTrailName.Enabled = enabled;
			this.btnAdd.Enabled = enabled;
			this.btnDelete.Enabled = false;
			this.listTrailPoint.Enabled = enabled;

			this.listTrailPoint.RowData = this.m_trailData;


		}

		public void ThemeChanged(ITheme visualTheme) {
			listTrailPoint.ThemeChanged(visualTheme);
			List.ThemeChanged(visualTheme);
			txtTrailName.ThemeChanged(visualTheme);
		}

		private void ActivityDetailPageControl_Load(object sender, EventArgs e) {
			panelTrailDetails.Top = 0;
			panelTrailDetails.Height = this.Height;
			panelTrailDetails.Left = 0;
			panelTrailDetails.Width = this.Width;
			panelTrailDetails.Anchor = (AnchorStyles)((int)AnchorStyles.Top + (int)AnchorStyles.Bottom + (int)AnchorStyles.Left + (int)AnchorStyles.Right);
		}
		
		private void button1_Click(object sender, EventArgs e) {
			IMapControl mapControl = UI.MapLayers.MapControlLayer.Instance.MapControl;
			if (mapControl.Selected.Count > 0) {
				IMapControlObject[] x = new IMapControlObject[mapControl.Selected.Count];
				//((IList<IMapControlObject>)(mapControl.Selected))[0];
				button1.Text = mapControl.GPSLocation.ToString();
//				mapControl.Selected[0]
//				ZoneFiveSoftware.Common.Data.GPS.
//					.MapControlLayer.GPSRoutes.SelectedGPSRoutePoint a;
/*				IMapControlObject = mapControl.Selected
					(
						(ZoneFiveSoftware.Common.Visuals.GPS.MapControlLayer.GPSRoutes.SelectedGPSRoutePoint)
							(
								(new System.Collections.Generic.Mscorlib_CollectionDebugView
									<ZoneFiveSoftware.Common.Visuals.Fitness.GPS.IMapControlObject>
									(mapControl.Selected)
								).Items[0]
							)
						)
 */
			} else {
				button1.Text = "not selected";
			}
		}

		private void btnAdd_Click(object sender, EventArgs e) {
			IMapControl mapControl = UI.MapLayers.MapControlLayer.Instance.MapControl;
			switch (mapControl.Selected.Count) {
				case 0:
					break;
				case 1:
					m_trailData.Add(new TrailDetailsRow(mapControl.GPSLocation));
					break;
				default:
					break;
			}
			RefreshData();
		}

		class TrailDetailsRow {
			private IGPSLocation m_GPSLocation;
			public TrailDetailsRow(IGPSLocation gpsLocation) {
				m_GPSLocation = gpsLocation;
			}

			public string latitude {
				get {
					return m_GPSLocation.LatitudeDegrees.ToString();
				}
			}

			public string longitude {
				get {
					return m_GPSLocation.LongitudeDegrees.ToString();
				}
			}

		}

	}
}
