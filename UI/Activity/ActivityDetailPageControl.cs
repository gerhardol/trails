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

		enum ControlMode {
			View, 
			Edit
		}

		private Data.Trail m_Trail;
		private IActivity m_activity;
		private ControlMode m_controlMode = ControlMode.View;

		public ActivityDetailPageControl(IActivity activity) {
			InitializeComponent();
			m_activity = activity;

			InitControls();
			RefreshControls();
		}

		void InitControls() {

			TrailName.ButtonImage = CommonIcons.MenuCascadeArrowDown;

			btnAdd.BackgroundImage = CommonIcons.Add;
			btnAdd.Text = "";			
			btnEdit.BackgroundImage = CommonIcons.Edit;
			btnEdit.Text = "";
			btnDelete.BackgroundImage = CommonIcons.Delete;
			btnDelete.Text = "";

		}

		void RefreshControls() {
			switch (m_controlMode) {
				case ControlMode.View:
					toolTip.SetToolTip(btnAdd, "Add new trail");
					toolTip.SetToolTip(btnEdit, "Edit this trail");
					toolTip.SetToolTip(btnDelete, "Delete this trail");
					btnAdd.Enabled = (m_activity != null);
					bool enabled = (TrailName.Text.Length != 0);
					btnEdit.Enabled = enabled;					
					btnDelete.Enabled = enabled;


					break;
				case ControlMode.Edit:
					toolTip.SetToolTip(btnAdd, "Add new trail");
					toolTip.SetToolTip(btnEdit, "Edit this trail");
					toolTip.SetToolTip(btnDelete, "Delete this trail");


					break;
			}
		}

		public IActivity Activity {
			set {
				m_activity = value;
			}
		}


		public void ThemeChanged(ITheme visualTheme) {
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
			switch (m_controlMode) {
				case ControlMode.View:
					m_Trail = new Data.Trail();
					UI.MapLayers.MapControlLayer layer = UI.MapLayers.MapControlLayer.Instance;
					IMapControl mapControl = layer.MapControl;					
					if (mapControl.Selected.Count > 0) {
						layer.SelectedGPSPointsChanged += new System.EventHandler(layer_SelectedGPSPointsChanged);
						layer.CaptureSelectedGPSPoints = true;
					}
					m_controlMode = ControlMode.Edit;
					break;
				case ControlMode.Edit:
					break;
			}
		}

		private void layer_SelectedGPSPointsChanged(object sender, EventArgs e) {

			UI.MapLayers.MapControlLayer layer = UI.MapLayers.MapControlLayer.Instance;
			m_Trail.points = layer.SelectedGPSPoints;
			layer.CaptureSelectedGPSPoints = false;

			RefreshControls();
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
