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
		private string m_lastTrail;

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
			List.NumHeaderRows = TreeList.HeaderRows.Two;
			List.Columns.Add(new TreeList.Column(TrailResultColumnIds.Order, "#", 30, StringAlignment.Near));
			List.Columns.Add(new TreeList.Column(TrailResultColumnIds.StartTime, "Start", 70, StringAlignment.Near));
			List.Columns.Add(new TreeList.Column(TrailResultColumnIds.EndTime, "End", 70, StringAlignment.Near));
			List.Columns.Add(new TreeList.Column(TrailResultColumnIds.Duration, "Duration", 60, StringAlignment.Near));
			List.Columns.Add(new TreeList.Column(TrailResultColumnIds.Distance, "Distance", 60, StringAlignment.Near));
			List.Columns.Add(new TreeList.Column(TrailResultColumnIds.AvgCadence, "Avg\nCadence", 60, StringAlignment.Near));
			List.Columns.Add(new TreeList.Column(TrailResultColumnIds.AvgHR, "Avg\nHR", 50, StringAlignment.Near));
			List.Columns.Add(new TreeList.Column(TrailResultColumnIds.MaxHR, "Max\nHR", 50, StringAlignment.Near));
			List.Columns.Add(new TreeList.Column(TrailResultColumnIds.ElevChg, "Elev.\nChg", 50, StringAlignment.Near));
			List.LabelProvider = new TrailResultLabelProvider();
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
				} else if (this.m_lastTrail != "" && names.Contains(this.m_lastTrail)) {
					m_currentTrail = TrailSettings.Instance.AllTrails[this.m_lastTrail];
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
			ChartBanner.ThemeChanged(visualTheme);
			lineChart1.ThemeChanged(visualTheme);
		}

		public IList<string> TrailNames {
			get {
				SortedDictionary<long, string> sortedNames = new SortedDictionary<long, string>();
				IGPSBounds gpsBounds = GPSBounds.FromGPSRoute(m_activity.GPSRoute);
				foreach (Data.Trail trail in TrailSettings.Instance.AllTrails.Values) {
					if (trail.IsInBounds(gpsBounds)) {
						IList<Data.TrailResult> results = trail.Results(m_activity);
						if (results.Count > 0) {
							sortedNames.Add(results[0].StartTime.Ticks, trail.Name);
						}
					}
				}
				IList<string> names = new List<string>();
				foreach (string name in sortedNames.Values) {
					names.Add(name);
				}
				return names;
			}
		}

		public IActivity Activity {
			set {
				m_activity = value;
				if (this.TrailName.Text != "") {
					m_lastTrail = this.TrailName.Text;
				}
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
				if (dialog.ShowDialog() == DialogResult.OK) {
					this.TrailName.Text = dialog.Trail.Name;
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

		private void ListContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {

		}

		private void listSettingsToolStripMenuItem_Click(object sender, EventArgs e) {
			ListSettings dialog = new ListSettings();
			dialog.ThemeChanged(m_visualTheme);
			dialog.ColumnsAvailable = this.AllColumns().Values;
			dialog.AllowFixedColumnSelect = true;
			// dialog.SelectedColumns = ZoneFiveSoftware.SportTracks.Data.ListItemOptions.ItemIds(ZoneFiveSoftware.SportTracks.Plugin.Activities.ActivitiesPlugin.Instance.ElevationPageOptions.Columns);
			// dialog.NumFixedColumns = ZoneFiveSoftware.SportTracks.Plugin.Activities.ActivitiesPlugin.Instance.ElevationPageOptions.NumLockedColumns;
			
			if (dialog.ShowDialog() == DialogResult.OK) {
				//ZoneFiveSoftware.SportTracks.Plugin.Activities.ActivitiesPlugin.Instance.ElevationPageOptions.Columns = ZoneFiveSoftware.SportTracks.Data.ListItemOptions.NewItems(ZoneFiveSoftware.SportTracks.Plugin.Activities.ActivitiesPlugin.Instance.ElevationPageOptions.Columns, listSettings.SelectedColumns, idictionary.get_Values());
				//ZoneFiveSoftware.SportTracks.Plugin.Activities.ActivitiesPlugin.Instance.ElevationPageOptions.NumLockedColumns = listSettings.NumFixedColumns;
				//ConfigureTreeColumns(ZoneFiveSoftware.SportTracks.Plugin.Activities.ActivitiesPlugin.Instance.ElevationPageOptions.Columns, ZoneFiveSoftware.SportTracks.Plugin.Activities.ActivitiesPlugin.Instance.ElevationPageOptions.NumLockedColumns);
		}

		}

		public IDictionary<string, IListItem> AllColumns() {
			IDictionary<string, IListItem> dictionary = new Dictionary<string, IListItem>();
			dictionary.Add(TrailResultColumnIds.Order, new ListItemInfo(TrailResultColumnIds.Order, "#", "", 30, StringAlignment.Near));
			dictionary.Add(TrailResultColumnIds.StartTime, new ListItemInfo(TrailResultColumnIds.StartTime, "Start","", 70, StringAlignment.Near));
			dictionary.Add(TrailResultColumnIds.EndTime, new ListItemInfo(TrailResultColumnIds.EndTime, "End", "", 70, StringAlignment.Near));
			dictionary.Add(TrailResultColumnIds.Duration, new ListItemInfo(TrailResultColumnIds.Duration, "Duration", "", 60, StringAlignment.Near));
			dictionary.Add(TrailResultColumnIds.Distance, new ListItemInfo(TrailResultColumnIds.Distance, "Distance", "", 60, StringAlignment.Near));
			dictionary.Add(TrailResultColumnIds.AvgCadence, new ListItemInfo(TrailResultColumnIds.AvgCadence, "Avg\nCadence", "", 60, StringAlignment.Near));
			dictionary.Add(TrailResultColumnIds.AvgHR, new ListItemInfo(TrailResultColumnIds.AvgHR, "Avg\nHR", "", 50, StringAlignment.Near));
			dictionary.Add(TrailResultColumnIds.MaxHR, new ListItemInfo(TrailResultColumnIds.MaxHR, "Max\nHR", "", 50, StringAlignment.Near));
			dictionary.Add(TrailResultColumnIds.ElevChg, new ListItemInfo(TrailResultColumnIds.ElevChg, "Elev.\nChg", "", 50, StringAlignment.Near));

			return dictionary;
		}

		private void List_SelectedChanged(object sender, EventArgs e) {

		}

	}
}
