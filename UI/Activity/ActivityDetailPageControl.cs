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
		private IList<string> m_trailNames;

		public ActivityDetailPageControl(IActivity activity) {
			InitializeComponent();
			m_activity = activity;

			InitControls();
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
		}

		private void RefreshControlState() {

			bool enabled = (m_activity != null);
			btnAdd.Enabled = enabled;
			TrailName.Enabled = enabled;

			enabled = (TrailName.Text.Length != 0);
			btnEdit.Enabled = enabled;
			btnDelete.Enabled = enabled;
			
		}

		private void RefreshData() {
			UI.MapLayers.MapControlLayer layer = UI.MapLayers.MapControlLayer.Instance;
			layer.HighlightedGPSLocations.Clear();
			TrailName.Text = "";
			layer.ShowHighlight = false;

			if (m_currentTrail != null) {
				TrailName.Text = m_currentTrail.name;
				foreach (Data.TrailPoint point in m_currentTrail.points) {
					layer.HighlightedGPSLocations.Add(point.GPSLocation);
				}
				layer.ShowHighlight = true;
				m_currentTrail.Results(m_activity);
			}
		}


		public void ThemeChanged(ITheme visualTheme) {
			m_visualTheme = visualTheme;
			TrailName.ThemeChanged(visualTheme);
			List.ThemeChanged(visualTheme);
		}

		public IActivity Activity {
			set {
				m_activity = value;
				m_currentTrail = null;				
				if (m_activity != null) {
					List<string> m_trailNames = new List<string>();
					IGPSBounds gpsBounds = GPSBounds.FromGPSRoute(m_activity.GPSRoute);
					foreach (Data.Trail trail in TrailSettings.Instance.AllTrails.Values) {
						if (trail.IsInBounds(gpsBounds)) {
							m_trailNames.Add(trail.name);
							if (m_currentTrail == null) {
								m_currentTrail = trail;
							}
						}
					}
			
				}
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

		}

		private void layer_SelectedGPSLocationsChanged_AddTrail(object sender, EventArgs e) {

			UI.MapLayers.MapControlLayer layer = (UI.MapLayers.MapControlLayer)sender;
			layer.SelectedGPSLocationsChanged -= new System.EventHandler(layer_SelectedGPSLocationsChanged_AddTrail);

			m_currentTrail = new Data.Trail();
			for (int i = 0; i < layer.SelectedGPSLocations.Count; i++) {
				m_currentTrail.points.Add(new Data.TrailPoint(layer.SelectedGPSLocations[i]));
			}
		}

		private void TrailName_ButtonClick(object sender, EventArgs e) {
			if (m_activity == null) {
				return;
			}

			TreeListPopup treeListPopup = new TreeListPopup();
			treeListPopup.ThemeChanged(m_visualTheme);
			treeListPopup.Tree.Columns.Add(new TreeList.Column());

			treeListPopup.Tree.RowData = m_trailNames;

			if (m_currentTrail != null) {
				treeListPopup.Tree.Selected = new object[] { m_currentTrail.name };
			}
			treeListPopup.ItemSelected += new TreeListPopup.ItemSelectedEventHandler(TrailName_ItemSelected);
			treeListPopup.Popup(this.TrailName.Parent.RectangleToScreen(this.TrailName.Bounds));
		}
		private void TrailName_ItemSelected(object sender, EventArgs e) {
			this.m_currentTrail = TrailSettings.Instance.AllTrails[((TreeListPopup.ItemSelectedEventArgs)e).Item.ToString()];
			RefreshData();
		}
	}

}
