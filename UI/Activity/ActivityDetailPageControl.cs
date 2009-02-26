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

			InitControls();
		}

		void InitControls() {

			TrailName.ButtonImage = CommonIcons.MenuCascadeArrowDown;

			btnAdd.BackgroundImage = CommonIcons.Add;
			btnAdd.Text = "";
			btnEditSave.BackgroundImage = CommonIcons.Edit;
			btnEditSave.Text = "";
			btnDelete.BackgroundImage = CommonIcons.Delete;
			btnDelete.Text = "";

		}

		public IActivity[] Activities {
			set {
				m_activities = value;
			}
		}


		public void ThemeChanged(ITheme visualTheme) {
			TrailName.ThemeChanged(visualTheme);
			List.ThemeChanged(visualTheme);			
		}

		private void btnAdd_Click(object sender, EventArgs e) {

			UI.MapLayers.MapControlLayer layer = UI.MapLayers.MapControlLayer.Instance;
			IMapControl mapControl = layer.MapControl;
			m_trailData.Clear();
			if (mapControl.Selected.Count > 0) {
				layer.SelectedGPSPointsChanged += new System.EventHandler(layer_SelectedGPSPointsChanged);
				layer.CaptureSelectedGPSPoints = true;
			}
		}

		private void layer_SelectedGPSPointsChanged(object sender, EventArgs e) {

/*
			UI.MapLayers.MapControlLayer layer = UI.MapLayers.MapControlLayer.Instance;
			m_trailData.Clear();
			for (int i = 0; i < layer.SelectedGPSPoints.Count; i++) {
				m_trailData.Add(new TrailDetailsRow(layer.SelectedGPSPoints[i]));
			}
			layer.CaptureSelectedGPSPoints = false;
			this.listTrailPoint.RowData = this.m_trailData;
 */
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

			public IGPSLocation GPSLocation {
				get {
					return m_GPSLocation;
				}
			}
		}

		private void btnSave_Click(object sender, EventArgs e) {
		}

		private void listTrailPoint_SelectedChanged(object sender, EventArgs e) {
/*
			if (listTrailPoint.Selected.Count > 0) {
				UI.MapLayers.MapControlLayer layer = UI.MapLayers.MapControlLayer.Instance;
				layer.HighlightGPSLocation = ((TrailDetailsRow)listTrailPoint.Selected[0]).GPSLocation;
				layer.ShowHighlight = true;
				layer.MapControl.Refresh();
			}
*/
			this.btnDelete.Enabled = true;
		}

		private void txtTrail_ButtonClick(object sender, EventArgs e) {

		}

		private void ActivityDetailPageControl_Load(object sender, EventArgs e) {

		}

	}
}
