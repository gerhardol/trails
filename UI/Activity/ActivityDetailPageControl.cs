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

		private void RefreshControlState() {

			btnAdd.Enabled = (m_activity != null);
			bool enabled = (TrailName.Text.Length != 0);
			btnEdit.Enabled = enabled;
			btnDelete.Enabled = enabled;
		}

		private void RefreshData() {
			TrailName.Text = m_currentTrail.name;
		}


		public void ThemeChanged(ITheme visualTheme) {
			m_visualTheme = visualTheme;
			TrailName.ThemeChanged(visualTheme);
			List.ThemeChanged(visualTheme);
		}

		public IActivity Activity {
			set {
				m_activity = value;
				RefreshControlState();
			}
		}

		private void btnAdd_Click(object sender, EventArgs e) {

			UI.MapLayers.MapControlLayer layer = UI.MapLayers.MapControlLayer.Instance;
			IMapControl mapControl = layer.MapControl;
			if (mapControl.Selected.Count > 1) {

				layer.SelectedGPSPointsChanged += new System.EventHandler(layer_SelectedGPSPointsChanged_AddTrail);
				layer.CaptureSelectedGPSPoints();
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

		private void layer_SelectedGPSPointsChanged_AddTrail(object sender, EventArgs e) {

			UI.MapLayers.MapControlLayer layer = (UI.MapLayers.MapControlLayer)sender;
			layer.SelectedGPSPointsChanged -= new System.EventHandler(layer_SelectedGPSPointsChanged_AddTrail);

			m_currentTrail = new Data.Trail();
			for (int i = 0; i < layer.SelectedGPSPoints.Count; i++) {
				m_currentTrail.points.Add(new Data.TrailPoint(layer.SelectedGPSPoints[i]));
			}
		}

		private void TrailName_ButtonClick(object sender, EventArgs e) {						
            /*
			if ((activity == null) || (Logbook == null))
                return;
            SortedList<string,string> sortedList = new SortedList<string,string>();
            IList<IActivity> ilist = ActivityCategory.ActivitiesForCategory(Logbook.Activities, ActivitiesPlugin.Instance.Application.DisplayOptions.SelectedCategoryFilter, true);
            string s1 = null;
            using (IEnumerator<IActivity> ienumerator = ilist.GetEnumerator())
            {
                while (ienumerator.MoveNext())
                {
                    IActivity iactivity = ienumerator.get_Current();
                    string s2 = iactivity.Name.Trim();
                    if (s2.Length > 0)
                    {
                        string s3 = s2.ToLower();
                        if (!sortedList.ContainsKey(s3))
                            sortedList.Add(s3, s2);
                        else
                            s2 = sortedList.get_Item(s3);
                        if (activity == iactivity)
                            s1 = s2;
                    }
                }
            }			
            //ControlUtils.OpenListPopup<string>(theme, sortedList.get_Values(), txtName, s1, new ControlUtils.ItemSelectHandler<string>(<txtName_ButtonClick>b__0));
*/
//        public static void OpenListPopup<T>(ITheme theme, IList<T> items, Control control, T selected, ControlUtils.ItemSelectHandler<T> selectHandler)
//        {
//            ControlUtils.<>c__DisplayClass4<T> <>c__DisplayClass4 = new ControlUtils.<>c__DisplayClass4<T>();
//            <>c__DisplayClass4.selectHandler = selectHandler;
			/*            if (selected != null)
						{
							object[] objArr = new object[] { selected };
							treeListPopup.Tree.Selected = objArr;
						}
			//            treeListPopup.ItemSelected += new TreeListPopup.ItemSelectedEventHandler(<>c__DisplayClass4.<OpenListPopup>b__3);
			*/
			TreeListPopup treeListPopup = new TreeListPopup();
            treeListPopup.ThemeChanged(m_visualTheme);
            treeListPopup.Tree.Columns.Add(new TreeList.Column());
			treeListPopup.Tree.RowData = TrailSettings.Instance.AllTrails.Keys;
			if (m_currentTrail != null) {
				treeListPopup.Tree.Selected = new object[] { m_currentTrail.name };
			}
			treeListPopup.Popup(this.TrailName.Parent.RectangleToScreen(this.TrailName.Bounds));
        }
	}
}
