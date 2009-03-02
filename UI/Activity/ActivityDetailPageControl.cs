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
using ZoneFiveSoftware.SportTracks.Util;

namespace TrailsPlugin.UI.Activity {
	public partial class ActivityDetailPageControl : UserControl {

		private ITheme m_visualTheme;
		private IActivity m_activity;
		private Data.Trail m_currentTrail;

		public ActivityDetailPageControl(IActivity activity) {
			InitializeComponent();
			InitControls();

			Activity = activity;
			RefreshControlState();
			RefreshData();			
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

			List.Columns.Clear();
			List.Columns.Add(new TreeList.Column("Order", "#", 20, StringAlignment.Near));			
			List.Columns.Add(new TreeList.Column("StartTime", "Start", 75, StringAlignment.Near));
			List.Columns.Add(new TreeList.Column("EndTime", "Start", 75, StringAlignment.Near));			
			List.Columns.Add(new TreeList.Column("Duration", "Duration", 100, StringAlignment.Near));
			List.Columns.Add(new TreeList.Column("Distance", "Distance", 100, StringAlignment.Near));
			List.Columns.Add(new TreeList.Column("AvgCadence", "AvgCadence", 100, StringAlignment.Near));
			
		}

		private void RefreshControlState() {

			bool enabled = (m_activity != null);
			btnAdd.Enabled = enabled;
			TrailName.Enabled = enabled;

			enabled = (TrailName.Text.Length != 0);
			TrailName.Enabled = enabled;
			btnEdit.Enabled = enabled;
			btnDelete.Enabled = enabled;
			
		}

		private void RefreshData() {
			UI.MapLayers.MapControlLayer layer = UI.MapLayers.MapControlLayer.Instance;
			layer.HighlightedGPSLocations.Clear();
			layer.ShowHighlight = false;
			List.RowData = null;

			if (m_activity != null) {
				IList<string> names = this.TrailNames;
				if (this.TrailName.Text != "" && names.Contains(this.TrailName.Text)) {
					m_currentTrail = TrailSettings.Instance.AllTrails[this.TrailName.Text];
				} else if (names.Count > 0) {
					m_currentTrail = TrailSettings.Instance.AllTrails[names[0]];
				}
			}

			if (m_currentTrail != null) {
				TrailName.Text = m_currentTrail.Name;
				foreach (Data.TrailGPSLocation point in m_currentTrail.TrailLocations) {
					layer.HighlightedGPSLocations.Add(point);
				}
				layer.ShowHighlight = true;
				List.RowData = m_currentTrail.Results(m_activity);
			} else {
				TrailName.Text = "";
			}
		}


		public void ThemeChanged(ITheme visualTheme) {
			m_visualTheme = visualTheme;
			TrailName.ThemeChanged(visualTheme);
			List.ThemeChanged(visualTheme);
		}

		public IList<string> TrailNames {
			get {
				List<string> names = new List<string>();
				IGPSBounds gpsBounds = GPSBounds.FromGPSRoute(m_activity.GPSRoute);
				foreach (Data.Trail trail in TrailSettings.Instance.AllTrails.Values) {
					if (trail.IsInBounds(gpsBounds)) {
						names.Add(trail.Name);
					}
				}
				return names;
			}
		}

		public IActivity Activity {
			set {
				m_activity = value;
				m_currentTrail = null;				
				RefreshData();
				RefreshControlState();
			}
		}

		private void btnAdd_Click(object sender, EventArgs e) {

			UI.MapLayers.MapControlLayer layer = UI.MapLayers.MapControlLayer.Instance;
			IMapControl mapControl = layer.MapControl;
			if (mapControl.Selected.Count > 1) {

				layer.SelectedGPSLocationsChanged += new System.EventHandler(layer_SelectedGPSLocationsChanged_AddTrail);
				layer.CaptureSelectedGPSLocations();
				EditTrail dialog = new EditTrail(m_currentTrail, m_visualTheme, true);
				dialog.ShowDialog();
				RefreshData();

			} else {
				MessageBox.Show("You must select at least two activities on the map", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		private void btnEdit_Click(object sender, EventArgs e) {
			EditTrail dialog = new EditTrail(m_currentTrail, m_visualTheme, false);
			dialog.ShowDialog();
			RefreshData();
		}

		private void btnDelete_Click(object sender, EventArgs e) {
			if (MessageBox.Show("Are you sure you want to delete this trail?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
				TrailSettings.Instance.DeleteTrail(m_currentTrail);
				RefreshData();
				RefreshControlState();
			}
		}

		private void layer_SelectedGPSLocationsChanged_AddTrail(object sender, EventArgs e) {

			UI.MapLayers.MapControlLayer layer = (UI.MapLayers.MapControlLayer)sender;
			layer.SelectedGPSLocationsChanged -= new System.EventHandler(layer_SelectedGPSLocationsChanged_AddTrail);

			m_currentTrail = new Data.Trail();
			for (int i = 0; i < layer.SelectedGPSLocations.Count; i++) {
				m_currentTrail.TrailLocations.Add(
					new Data.TrailGPSLocation(
						layer.SelectedGPSLocations[i].LatitudeDegrees,
						layer.SelectedGPSLocations[i].LongitudeDegrees
					)
				);
			}
		}

		private void TrailName_ButtonClick(object sender, EventArgs e) {
			if (m_activity == null) {
				return;
			}

			TreeListPopup treeListPopup = new TreeListPopup();
			treeListPopup.ThemeChanged(m_visualTheme);
			treeListPopup.Tree.Columns.Add(new TreeList.Column());

			treeListPopup.Tree.RowData = this.TrailNames;

			if (m_currentTrail != null) {
				treeListPopup.Tree.Selected = new object[] { m_currentTrail.Name };
			}
			treeListPopup.ItemSelected += new TreeListPopup.ItemSelectedEventHandler(TrailName_ItemSelected);
			treeListPopup.Popup(this.TrailName.Parent.RectangleToScreen(this.TrailName.Bounds));
		}

		private void TrailName_ItemSelected(object sender, EventArgs e) {
			this.TrailName.Text = ((TreeListPopup.ItemSelectedEventArgs)e).Item.ToString();
			RefreshData();
		}
	}

}
