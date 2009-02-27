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

		private ITheme m_visualTheme;
		private IActivity m_activity;
		private Data.Trail m_trailToEdit;

		public ActivityDetailPageControl(IActivity activity) {
			InitializeComponent();
			m_activity = activity;

			InitControls();
			RefreshControlState();

		}

		void InitControls() {

			TrailName.ButtonImage = CommonIcons.MenuCascadeArrowDown;

			btnAdd.BackgroundImage = CommonIcons.Add;
			btnAdd.Text = "";
			btnEdit.BackgroundImage = CommonIcons.Edit;
			btnEdit.Text = "";
			btnDelete.BackgroundImage = CommonIcons.Delete;
			btnDelete.Text = "";
			toolTip.SetToolTip(btnAdd, "Add new trail. (Select the trail points on the map before pushing this button)");
			toolTip.SetToolTip(btnEdit, "Edit this trail. (Select the trail points on the map before pushing this button)");
			toolTip.SetToolTip(btnDelete, "Delete this trail.");

		}

		public IActivity Activity {
			set {
				m_activity = value;
				RefreshControlState();
			}
		}

		private void RefreshControlState() {

			btnAdd.Enabled = (m_activity != null);
			bool enabled = (TrailName.Text.Length != 0);
			btnEdit.Enabled = enabled;
			btnDelete.Enabled = enabled;
		}


		public void ThemeChanged(ITheme visualTheme) {
			m_visualTheme = visualTheme;
			TrailName.ThemeChanged(visualTheme);
			List.ThemeChanged(visualTheme);
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

		private void txtTrail_ButtonClick(object sender, EventArgs e) {

		}

		private void ActivityDetailPageControl_Load(object sender, EventArgs e) {

		}

		private void btnDelete_Click(object sender, EventArgs e) {

		}

		private void btnEdit_Click(object sender, EventArgs e) {

		}

		private void btnAdd_Click(object sender, EventArgs e) {

			UI.MapLayers.MapControlLayer layer = UI.MapLayers.MapControlLayer.Instance;
			IMapControl mapControl = layer.MapControl;
			if (mapControl.Selected.Count > 1) {

				layer.SelectedGPSPointsChanged += new System.EventHandler(layer_SelectedGPSPointsChanged_AddTrail);
				layer.CaptureSelectedGPSPoints();
				EditTrail dialog = new EditTrail(this.m_trailToEdit, m_visualTheme);
				dialog.ShowDialog();
			} else {
				MessageBox.Show("You must select at least two activities on the map", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		private void layer_SelectedGPSPointsChanged_AddTrail(object sender, EventArgs e) {

			UI.MapLayers.MapControlLayer layer = (UI.MapLayers.MapControlLayer)sender;
			layer.SelectedGPSPointsChanged -= new System.EventHandler(layer_SelectedGPSPointsChanged_AddTrail);

			this.m_trailToEdit = new Data.Trail();
			for (int i = 0; i < layer.SelectedGPSPoints.Count; i++) {
				this.m_trailToEdit.points.Add(new Data.TrailPoint(layer.SelectedGPSPoints[i]));
			}
		}

		private void btnSave_Click(object sender, EventArgs e) {

		}

	}
}

/*		private void listTrailPoint_SelectedChanged(object sender, EventArgs e) {
			if (listTrailPoint.Selected.Count > 0) {
				UI.MapLayers.MapControlLayer layer = UI.MapLayers.MapControlLayer.Instance;
				layer.HighlightGPSLocation = ((TrailDetailsRow)listTrailPoint.Selected[0]).GPSLocation;
				layer.ShowHighlight = true;
				layer.MapControl.Refresh();
			}			
			this.btnDelete.Enabled = true;
		}
*/
