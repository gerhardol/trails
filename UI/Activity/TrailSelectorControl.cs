/*
Copyright (C) 2009 Brendan Doherty
Copyright (C) 2010 Gerhard Olsson

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

#if ST_2_1
using ZoneFiveSoftware.Common.Visuals.Fitness.GPS;
//IListItem, ListSettings
using ZoneFiveSoftware.SportTracks.Util;
using ZoneFiveSoftware.SportTracks.UI;
using ZoneFiveSoftware.SportTracks.UI.Forms;
using ZoneFiveSoftware.SportTracks.Data;
using TrailsPlugin.UI.MapLayers;
#else
using TrailsPlugin.UI.MapLayers;
#endif
using TrailsPlugin.Data;
using TrailsPlugin.Controller;

namespace TrailsPlugin.UI.Activity
{
	public partial class TrailSelectorControl : UserControl
    {

        private ITheme m_visualTheme;
        private CultureInfo m_culture;
        private TrailController m_controller;
#if ST_2_1
        private Object m_view = null; //dummy
        private UI.MapLayers.MapControlLayer m_layer;
#else
        private IDailyActivityView m_view = null;
        private TrailPointsLayer m_layer = null;
#endif

        ActivityDetailPageControl m_page;
        private EditTrail m_editTrail = null;
        private bool m_selectTrailAddMode;
        //private bool m_CtrlPressed = false;

        /*********************************************/

        public TrailSelectorControl()
        {
            InitializeComponent();
        }
        public void SetControl(ActivityDetailPageControl page, Controller.TrailController controller,
#if ST_2_1
          Object view, UI.MapLayers.MapControlLayer layer)
#else
          IDailyActivityView view, TrailPointsLayer layer)
#endif
        {
            m_view = view;
            m_layer = layer;
            m_page = page;
            m_controller = controller;

            InitControls();
		}

		void InitControls()
        {
            TrailName.ButtonImage = CommonIcons.MenuCascadeArrowDown;

			btnAdd.BackgroundImage = CommonIcons.Add;
			btnAdd.Text = "";
			btnEdit.BackgroundImage = CommonIcons.Edit;
			btnEdit.Text = "";
			btnDelete.BackgroundImage = CommonIcons.Delete;
			btnDelete.Text = "";
		}

        public void UICultureChanged(CultureInfo culture)
        {
            m_culture = culture;
            toolTip.SetToolTip(btnAdd, Properties.Resources.UI_Activity_Page_AddTrail_TT);
            toolTip.SetToolTip(btnEdit, Properties.Resources.UI_Activity_Page_EditTrail_TT);
            toolTip.SetToolTip(btnDelete, Properties.Resources.UI_Activity_Page_DeleteTrail_TT);
            this.lblTrail.Text = Properties.Resources.TrailName + ":";
        }
        public void ThemeChanged(ITheme visualTheme)
        {
            m_visualTheme = visualTheme;
            TrailName.ThemeChanged(visualTheme);
        }
        private bool m_showPage = false;
        public bool ShowPage
        {
            get { return m_showPage; }
            set
            {
                m_showPage = value;
                if (!this.m_showPage && this.m_editTrail != null)
                {
                    this.m_editTrail.Close();
                }
                //if (_showPage)
                //{
                //    RefreshControlState();
                //}
            }
        }

        public void UpdatePointFromMap(TrailGPSLocation point)
        {
            if (this.m_editTrail != null)
            {
                this.m_editTrail.UpdatePointFromMap(point);
            }
        }

        public void RefreshControlState() 
        {
            bool enabled = true;//Enabled also when no trails/activities (m_controller.ReferenceActivity != null);
            enabled = (m_editTrail == null);

            btnAdd.Enabled = enabled;
            TrailName.Enabled = enabled;

            enabled = (m_editTrail == null);//Enabled also when no trails/activities (m_controller.CurrentActivityTrailDisplayed != null);
            btnEdit.Enabled = enabled;

            if (m_controller.CurrentActivityTrailIsSelected)
            {
                TrailName.Text = m_controller.PrimaryCurrentActivityTrail.Trail.Name;
                if (m_controller.CurrentActivityTrails.Count > 1)
                {
                    TrailName.Text += " (*)";
                }
                TrailName.Enabled = (m_editTrail == null);
            }
            else
            {
                TrailName.Text = Properties.Resources.Trail_NoTrailSelected;
            }
            enabled = enabled && ((!m_controller.CurrentActivityTrailIsSelected) || !m_controller.PrimaryCurrentActivityTrail.Trail.Generated);
            btnDelete.Enabled = enabled;
        }


        /************************************************************/
		private void btnAdd_Click(object sender, EventArgs e)
        {
            //single activity use
#if ST_2_1
			IMapControl mapControl = m_layer.MapControl;
			ICollection<IMapControlObject> selectedGPS = null;
            if (null != mapControl) { selectedGPS = mapControl.Selected; }
#else
            IList<IItemTrackSelectionInfo> selectedGPS = TrailsItemTrackSelectionInfo.SetAndAdjustFromSelection(m_view.RouteSelectionProvider.SelectedItems, m_page.ViewActivities, true);
#endif

            if (TrailsItemTrackSelectionInfo.ContainsData(selectedGPS))
            {
#if ST_2_1
                m_layer.SelectedGPSLocationsChanged += new System.EventHandler(layer_SelectedGPSLocationsChanged_AddTrail);
				m_layer.CaptureSelectedGPSLocations();
#else
                selectedGPSLocationsChanged_AddTrail(selectedGPS);
#endif
            }
            else if ((m_editTrail == null) && this.m_controller.CurrentActivityTrailIsSelected &&
               this.m_controller.PrimaryCurrentActivityTrail.Trail.Generated)
            {
                //Just for convenience (the popup text next contradicts this currently)
                btnEdit_Click(sender, e);
            }
            else
            {
#if ST_2_1
                string message = String.Format(Properties.Resources.UI_Activity_Page_SelectPointsError_ST2, 
                   Properties.Resources.Trail_Reference_Name);
                MessageBox.Show(message, "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
#else
                //Change: Just popup new trail
                //string message = String.Format(Properties.Resources.UI_Activity_Page_SelectPointsError,
                EditTrail dialog = new EditTrail(m_visualTheme, m_culture, m_page, m_view, m_layer, true, m_controller.ReferenceTrailResult);
                showEditDialog(dialog);
#endif
            }
 		}
        //private TrailsItemTrackSelectionInfo getSel(DateTime t)
        //{
        //    IValueRange<DateTime> v = new ValueRange<DateTime>(t, t);
        //    TrailsItemTrackSelectionInfo s = new TrailsItemTrackSelectionInfo();
        //    s.SelectedTime = v;
        //    return s;
        //}

        private void btnEdit_Click(object sender, EventArgs e)
        {
#if ST_2_1
			IMapControl mapControl = m_layer.MapControl;
            ICollection<IMapControlObject> selectedGPS = null;
            if (null != mapControl) { selectedGPS = mapControl.Selected; }
#else
            IList<IItemTrackSelectionInfo> selectedGPS =
                        TrailsItemTrackSelectionInfo.SetAndAdjustFromSelection(m_view.RouteSelectionProvider.SelectedItems, m_page.ViewActivities, true);
#endif
            if (m_controller.PrimaryCurrentActivityTrail != null &&
                m_controller.PrimaryCurrentActivityTrail.Trail.TrailType == Trail.CalcType.HighScore)
            {
                Guid view = GUIDs.SettingsView;
                String bookmark = "PageId=" + GUIDs.HighScorePluginMain.ToString();
                Plugin.GetApplication().ShowView(view, bookmark);
                return;
            }
            else if (m_controller.PrimaryCurrentActivityTrail != null &&
                m_controller.PrimaryCurrentActivityTrail.Trail.TrailType == Trail.CalcType.UniqueRoutes)
            {
                Guid view = GUIDs.SettingsView;
                String bookmark = "PageId=" + GUIDs.UniqueRoutesPluginMain.ToString();
                Plugin.GetApplication().ShowView(view, bookmark);
                return;
            }
            else if (m_controller.PrimaryCurrentActivityTrail != null &&
                m_controller.PrimaryCurrentActivityTrail.Trail.Generated &&
                m_controller.PrimaryCurrentActivityTrail.Trail.TrailType != Trail.CalcType.ElevationPoints)
            {
                Guid view = GUIDs.SettingsView;
                String bookmark = "PageId=" + GUIDs.Settings.ToString();
                Plugin.GetApplication().ShowView(view, bookmark);
                return;
            }
            else if (TrailsItemTrackSelectionInfo.ContainsData(selectedGPS) &&
                m_controller.CurrentActivityTrailIsSelected &&
                !m_controller.PrimaryCurrentActivityTrail.Trail.Generated &&
                //Change: never replace points when editing trails
                false)
            {
#if ST_2_1
				m_layer.SelectedGPSLocationsChanged += new System.EventHandler(layer_SelectedGPSLocationsChanged_EditTrail);
				m_layer.CaptureSelectedGPSLocations();
#else
                selectedGPSLocationsChanged_EditTrail(selectedGPS);
#endif
            }
            else
            {
                EditTrail dialog = new EditTrail(m_visualTheme, m_culture, m_page, m_view, m_layer, false, m_controller.ReferenceTrailResult);
                showEditDialog(dialog);
            }
        }
        private void showEditDialog(EditTrail dialog)
        {
            m_editTrail = dialog;
            m_layer.editTrail = dialog;
            m_page.RefreshControlState(); 
            
            dialog.TopMost = true;
            dialog.FormClosed += new FormClosedEventHandler(editTrail_FormClosed);
            dialog.Show();
        }

        void editTrail_FormClosed(object sender, FormClosedEventArgs e)
        {
            //There is no need to check the sender or the result, edit form handles calculation and revert
            m_editTrail = null;
            m_layer.editTrail = null;

            m_page.RefreshControlState();
            m_page.RefreshData(false);
        }

		private void btnDelete_Click(object sender, EventArgs e)
        {
            if (m_controller.CurrentActivityTrailIsSelected &&
                MessageDialog.Show(Properties.Resources.UI_Activity_Page_DeleteTrailConfirm, m_controller.PrimaryCurrentActivityTrail.Trail.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) 
                == DialogResult.Yes)
            {
				m_controller.DeleteCurrentTrail();
				m_page.RefreshControlState();
				m_page.RefreshData(false);
			}
		}

        /*************************************************************************************************************/
//ST3
        //TODO: Rewrite, using IItemTrackSelectionInfo help functions?
        private static IList<TrailGPSLocation> getGPS(Trail trail, IList<IActivity> activities, IValueRange<DateTime> ts, IValueRange<double> di, string id)
        {
            IList<TrailGPSLocation> result = new List<TrailGPSLocation>();
            IList<DateTime> dates = new List<DateTime>();

            IActivity activity = null;
            foreach (IActivity a in activities)
            {
                //In ST3.0.4068 only one activity here 
                if (id == a.ReferenceId)
                {
                    activity = a;
                    break;
                }
            }

            if (null != activity && null != activity.GPSRoute)
            {
                if (null != ts)
                {
                    dates.Add(ts.Lower);
                    if (ts.Upper > ts.Lower)
                    {
                        IDistanceDataTrack dtrack = activity.GPSRoute.GetDistanceMetersTrack();
                        double s = dtrack.GetInterpolatedValue(ts.Upper).Value - dtrack.GetInterpolatedValue(ts.Lower).Value;
                        if (s > 2 * trail.Radius)
                        {
                            dates.Add(ts.Upper);
                        }
                    }
                }
                //Ignore di (distances)
            }
            foreach (DateTime d in dates)
            {
                IGPSPoint t = Utils.TrackUtil.getGpsLoc(activity, d);
                if (t != null)
                {
                    result.Add(new TrailGPSLocation(t, "", true));
                }
            }

            return result;
        }

        public static IList<TrailGPSLocation> getGPS(Trail trail, IList<IActivity> activities,  IList<IItemTrackSelectionInfo> aSelectGPS)
        {
            IList<TrailGPSLocation> result = new List<TrailGPSLocation>();
            for (int i = 0; i < aSelectGPS.Count; i++)
            {
                IItemTrackSelectionInfo selectGPS = aSelectGPS[i];
                IList<TrailGPSLocation> result2 = new List<TrailGPSLocation>();

                //Marked and Selected Times are set at import (ST uses Distances)
                IValueRangeSeries<DateTime> tm = selectGPS.MarkedTimes;
                if (null != tm)
                {
                    foreach (IValueRange<DateTime> ts in tm)
                    {
                        result2 = Trail.MergeTrailLocations(result2, getGPS(trail, activities, ts, null, aSelectGPS[i].ItemReferenceId));
                    }
                }
                if (result2.Count == 0)
                {
                    if (result2.Count == 0)
                    {
                        //Selected
                        result2 = getGPS(trail, activities, selectGPS.SelectedTime, null, aSelectGPS[i].ItemReferenceId);
                    }
                }
                result = Trail.MergeTrailLocations(result, result2);
            }
            return result;
        }

#if ST_2_1
        IList<TrailGPSLocation> getGPS(IList<IGPSLocation> aSelectGPS)
        {
            IList<TrailGPSLocation> result = new List<TrailGPSLocation>();
            for (int i = 0; i < aSelectGPS.Count; i++)
            {
                IGPSLocation selectGPS = aSelectGPS[i];
            result.Add(new TrailGPSLocation(selectGPS.LatitudeDegrees, selectGPS.LongitudeDegrees, ""));
            }
            return result;
        }
#endif
#if !ST_2_1
        private void selectedGPSLocationsChanged_AddTrail(IList<IItemTrackSelectionInfo> selectedGPS)
        {
#else
		private void layer_SelectedGPSLocationsChanged_AddTrail(object sender, EventArgs e)
        {
			//UI.MapLayers.MapControlLayer layer = (UI.MapLayers.MapControlLayer)sender;
			m_layer.SelectedGPSLocationsChanged -= new System.EventHandler(layer_SelectedGPSLocationsChanged_AddTrail);
            IList<IGPSLocation> selectedGPS = m_layer.SelectedGPSLocations;
#endif
            bool addCurrent = false;
            if (m_controller.CurrentActivityTrailIsSelected && !m_controller.PrimaryCurrentActivityTrail.Trail.Generated)
            {
                if (MessageDialog.Show(string.Format(Properties.Resources.UI_Activity_Page_AddTrail_Replace, CommonResources.Text.ActionYes,CommonResources.Text.ActionNo),
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    addCurrent = true;
                }
            }
            EditTrail dialog = new EditTrail(m_visualTheme, m_culture, m_page, m_view, m_layer, !addCurrent, m_controller.ReferenceTrailResult);
            if (m_controller.CurrentActivityTrailIsSelected)
            {
                if (addCurrent)
                {
                }
                else
                {
                    dialog.Trail.TrailLocations.Clear();
                }
            }
            dialog.Trail.TrailLocations = Trail.MergeTrailLocations(dialog.Trail.TrailLocations, getGPS(dialog.Trail, m_page.ViewActivities, selectedGPS));

            showEditDialog(dialog);
		}


        //Unused
#if !ST_2_1
        private void selectedGPSLocationsChanged_EditTrail(IList<IItemTrackSelectionInfo> selectedGPS)
        {
#else
 		private void layer_SelectedGPSLocationsChanged_EditTrail(object sender, EventArgs e)
        {
			//UI.MapLayers.MapControlLayer layer = (UI.MapLayers.MapControlLayer)sender;
			m_layer.SelectedGPSLocationsChanged -= new System.EventHandler(layer_SelectedGPSLocationsChanged_EditTrail);
            IList<IGPSLocation> selectedGPS = m_layer.SelectedGPSLocations;
#endif
            EditTrail dialog = new EditTrail(m_visualTheme, m_culture, m_page, m_view, m_layer, false, m_controller.ReferenceTrailResult);
            bool selectionIsDifferent = selectedGPS.Count != dialog.Trail.TrailLocations.Count;
            if (!selectionIsDifferent)
            {
                IList<TrailGPSLocation> loc = getGPS(dialog.Trail, m_page.ViewActivities, selectedGPS);
                if (loc.Count == selectedGPS.Count)
                {
                    for (int i = 0; i < loc.Count; i++)
                    {
                        TrailGPSLocation loc1 = loc[i];
                        TrailGPSLocation loc2 = dialog.Trail.TrailLocations[i];
                        if (loc1.LatitudeDegrees  != loc2.LatitudeDegrees ||
                            loc1.LongitudeDegrees != loc2.LongitudeDegrees)
                        {
                            selectionIsDifferent = true;
                            break;
                        }
                    }
                }
            }
 
            if (selectionIsDifferent)
            {
                if (MessageDialog.Show(Properties.Resources.UI_Activity_Page_UpdateTrail, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dialog.Trail.TrailLocations = getGPS(dialog.Trail, m_page.ViewActivities, selectedGPS);
                }
            }

            showEditDialog(dialog);
        }

        /***************************************************************************/

        //The following is not working, KeyUp is never seen
        //Key down could be used with any other key, which is not intuitive...
        //void TrailName_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        //{
        //    this.m_CtrlPressed = false;
        //}

        //void TrailName_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        //{
        //    this.m_CtrlPressed = e.Modifiers == Keys.Control;
        //}

        private void TrailName_ButtonClick(object sender, EventArgs e)
        {
            TreeListPopup treeListPopup = new TreeListPopup();
            treeListPopup.ThemeChanged(m_visualTheme);
            treeListPopup.Tree.Columns.Add(new TreeList.Column());

            //Get the list of ordered activity trails, without forcing calculation
            treeListPopup.Tree.RowData = m_controller.OrderedTrails();
            //Note: Just checking for current trail could modify the ordered list, so do this first
            System.Collections.IList currSel = null;
            if (m_controller.CurrentActivityTrailIsSelected)
            {
                currSel = new object[m_controller.CurrentActivityTrails.Count];
                for (int i = 0; i < m_controller.CurrentActivityTrails.Count; i++)
                {
                    currSel[i] = m_controller.CurrentActivityTrails[i];
                }
            }
#if ST_2_1
            treeListPopup.Tree.Selected = currSel;
#else
            treeListPopup.Tree.SelectedItems = currSel;
#endif
            treeListPopup.Tree.LabelProvider = new TrailDropdownLabelProvider();
            m_selectTrailAddMode = false;
            if (e is MouseEventArgs && (e as MouseEventArgs).Button == System.Windows.Forms.MouseButtons.Right)
            {
                m_selectTrailAddMode = true;
            }
            treeListPopup.ItemSelected += new TreeListPopup.ItemSelectedEventHandler(TrailName_ItemSelected);
            treeListPopup.Popup(this.TrailName.Parent.RectangleToScreen(this.TrailName.Bounds));
        }

        /*******************************************************/
        private void TrailName_ItemSelected(object sender, EventArgs e)
        {
            if (sender is TreeListPopup)
            {
                ((TreeListPopup)sender).Hide();
            }

            IList <ActivityTrail> ats = new List<ActivityTrail>();
            if (m_selectTrailAddMode /*|| this.m_CtrlPressed*/)
            {
                //Handle all the existing as selected too
                foreach (ActivityTrail at in this.m_controller.CurrentActivityTrails)
                {
                    ats.Add(at);
                }
            }
            ActivityTrail t = ((ActivityTrail)((TreeListPopup.ItemSelectedEventArgs)e).Item);
            if (ats.Contains(t))
            {
                ats.Remove(t);
            }
            else
            {
                ats.Add(t);
            }

            System.Windows.Forms.ProgressBar progressBar = m_page.StartProgressBar(0);
            m_controller.SetCurrentActivityTrail(ats, true, progressBar);
            m_page.StopProgressBar();
            m_page.RefreshData(false);
            m_page.RefreshControlState();

            GPSBounds area = TrailGPSLocation.getGPSBounds(t.Trail.TrailLocations, 3 * t.Trail.Radius);
            m_layer.SetLocation(area);
        }

        private void TrailSelectorPanel_SizeChanged(object sender, EventArgs e) {
			// autosize column doesn't seem to be working.
            //Sizing is flaky in general
			float width = 0;
			for (int i = 0; i < TrailSelectorPanel.ColumnStyles.Count; i++) {
				if (i != 1) {
					width += this.TrailSelectorPanel.ColumnStyles[i].Width;
				}
			}
			this.TrailSelectorPanel.ColumnStyles[1].SizeType = SizeType.Absolute;
            this.TrailSelectorPanel.ColumnStyles[1].Width = this.TrailSelectorPanel.Width - width;
		}
    }
}
