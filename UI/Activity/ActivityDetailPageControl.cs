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
using ZoneFiveSoftware.SportTracks.UI;
using ZoneFiveSoftware.SportTracks.UI.Forms;
using ZoneFiveSoftware.SportTracks.Data;

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


			List.NumHeaderRows = TreeList.HeaderRows.Two;
			List.LabelProvider = new TrailResultLabelProvider();

			this.RefreshColumns();
		}

		private void RefreshColumns() {

			List.Columns.Clear();
			foreach (string id in PluginMain.Settings.ActivityPageColumns) {
				foreach (ListItemInfo columnDef in TrailResultColumnIds.ColumnDefs()) {
					if (columnDef.Id == id) {
						TreeList.Column column = new TreeList.Column(
							columnDef.Id,
							columnDef.ToString(),
							columnDef.Width,
							columnDef.Align
						);
						List.Columns.Add(column);
						break;
					}
				}
			}
		}

		private void RefreshControlState() {

			bool enabled = (m_activity != null);
			btnAdd.Enabled = enabled;
			TrailName.Enabled = enabled;

			enabled = (m_currentTrail  != null);
			TrailName.Enabled = enabled;
			btnEdit.Enabled = enabled;
			btnDelete.Enabled = enabled;

		}

		private void RefreshData() {
			UI.MapLayers.MapControlLayer layer = UI.MapLayers.MapControlLayer.Instance;
			layer.HighlightedGPSLocations.Clear();
			layer.ShowHighlight = false;
			List.RowData = null;

			if (m_currentTrail != null) {
				TrailName.Text = m_currentTrail.Name;
				foreach (Data.TrailGPSLocation point in m_currentTrail.TrailLocations) {
					layer.HighlightedGPSLocations.Add(point);
				}
				layer.HighlightRadius = m_currentTrail.Radius;
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
			ChartBanner.ThemeChanged(visualTheme);
			lineChart1.ThemeChanged(visualTheme);
		}

		public IActivity Activity {
			set {
				m_activity = value;
				Data.Trail prevTrail = m_currentTrail;
				m_currentTrail = null;
				if (m_activity != null) {
					IList<Data.Trail> trails = PluginMain.Data.TrailsInBounds(m_activity);
					if (!trails.Contains(prevTrail)) {
						if (trails.Count > 0) {
							m_currentTrail = trails[0];
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
				if (dialog.ShowDialog() == DialogResult.OK) {
					m_currentTrail = dialog.Trail;
					RefreshControlState();
					RefreshData();
				}
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
			if (MessageBox.Show("Are you sure you want to delete this trail?", m_currentTrail.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
				PluginMain.Data.DeleteTrail(m_currentTrail);
				RefreshControlState();
				RefreshData();
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

			IList<string> trailNames = new List<string>();
			foreach(Data.Trail trail in PluginMain.Data.TrailsInBounds(m_activity)) {
				trailNames.Add(trail.Name);
			}
			treeListPopup.Tree.RowData = trailNames;

			if (m_currentTrail != null) {
				treeListPopup.Tree.Selected = new object[] { m_currentTrail.Name };
			}
			treeListPopup.ItemSelected += new TreeListPopup.ItemSelectedEventHandler(TrailName_ItemSelected);
			treeListPopup.Popup(this.TrailName.Parent.RectangleToScreen(this.TrailName.Bounds));
		}

		private void TrailName_ItemSelected(object sender, EventArgs e) {
			string trailName = (((TreeListPopup.ItemSelectedEventArgs)e).Item).ToString();
			foreach (Data.Trail trail in PluginMain.Data.TrailsInBounds(m_activity)) {
				if (trail.Name == trailName) {
					m_currentTrail = trail;
					break;
				}
			}
			RefreshData();
		}

		private void listSettingsToolStripMenuItem_Click(object sender, EventArgs e) {
			ListSettings dialog = new ListSettings();
			dialog.ThemeChanged(m_visualTheme);
			dialog.ColumnsAvailable = TrailResultColumnIds.ColumnDefs();
			dialog.AllowFixedColumnSelect = true;
			dialog.SelectedColumns = PluginMain.Settings.ActivityPageColumns;
			dialog.NumFixedColumns = PluginMain.Settings.ActivityPageNumFixedColumns;

			if (dialog.ShowDialog() == DialogResult.OK) {
				PluginMain.Settings.ActivityPageNumFixedColumns = dialog.NumFixedColumns;
				PluginMain.Settings.ActivityPageColumns = dialog.SelectedColumns;
				RefreshColumns();
			}
		}

		private void List_SelectedChanged(object sender, EventArgs e) {

		}

	}
}
