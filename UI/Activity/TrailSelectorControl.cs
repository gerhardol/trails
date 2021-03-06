﻿/*
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

namespace TrailsPlugin.UI.Activity
{
    public partial class TrailSelectorControl : UserControl
    {

        private ITheme m_visualTheme;
        private CultureInfo m_culture;
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

        public void SetControl(ActivityDetailPageControl page,
#if ST_2_1
          Object view, UI.MapLayers.MapControlLayer layer)
#else
          IDailyActivityView view, TrailPointsLayer layer)
#endif
        {
            m_view = view;
            m_layer = layer;
            m_page = page;

            InitControls();
        }

        void InitControls()
        {
            TrailName.ButtonImage = CommonIcons.MenuCascadeArrowDown;

            btnAdd.CenterImage = CommonIcons.Add;
            btnAdd.Text = "";
            btnEdit.CenterImage = CommonIcons.Edit;
            btnEdit.Text = "";
            btnDelete.CenterImage = CommonIcons.Delete;
            btnDelete.Text = "";
            btnMenu.CenterImage = Properties.Resources.ChartMenuButton;
            btnMenu.Text = "";
            //this.trailPointsMenuItem.Image = Properties.Resources.SplitPoints;
            this.showToolBarMenuItem.Image = Properties.Resources.ChartTools;
            this.settingsToolBarMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Settings16;
        }

        public void UICultureChanged(CultureInfo culture)
        {
            m_culture = culture;
            toolTip.SetToolTip(btnAdd, Properties.Resources.UI_Activity_Page_AddTrail_TT);
            toolTip.SetToolTip(btnEdit, Properties.Resources.UI_Activity_Page_EditTrail_TT);
            toolTip.SetToolTip(btnDelete, Properties.Resources.UI_Activity_Page_DeleteTrail_TT);
            this.lblTrail.Text = Properties.Resources.TrailName + ":";
            this.ResultSummaryStdDevMenuItem.Text = Properties.Resources.UI_Activity_List_ResultSummaryStdDev;
            this.showSummaryTotalMenuItem.Text = Properties.Resources.UI_Activity_List_ShowSummaryTotal;
            this.showSummaryAverageMenuItem.Text = Properties.Resources.UI_Activity_List_ShowSummaryAverage;
            this.selectSimilarSplitsMenuItem.Text = Properties.Resources.UI_Activity_List_Splits;
            this.useDeviceDistanceMenuItem.Text = Properties.Resources.UI_Activity_List_UseDeviceDistance;
            this.setRestLapsAsPausesMenuItem.Text = Properties.Resources.UI_Activity_List_SetRestLapsAsPauses;
            this.nonReqIsPauseMenuItem.Text = Properties.Resources.UI_Activity_List_SetNonRequiredAsPauses;
            this.ShowPausesAsResultsMenuItem.Text = Properties.Resources.UI_Activity_List_ShowPausesAsResults;
            this.showOnlyMarkedResultsOnMapMenuItem.Text = Properties.Resources.UI_Activity_List_ShowOnlyMarkedResultsOnMap;
            this.trailPointsMenuItem.Text = Properties.Resources.UI_Activity_List_ShowSplitPointsOnMap;
            this.showToolBarMenuItem.Text = Properties.Resources.UI_Activity_Menu_ShowToolBar;
            this.settingsToolBarMenuItem.Text = Properties.Resources.Settings;
        }

        public void ThemeChanged(ITheme visualTheme)
        {
            m_visualTheme = visualTheme;
            TrailName.ThemeChanged(visualTheme);
            this.chartPanelMenu.Renderer = new ThemedContextMenuStripRenderer(visualTheme);
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
            bool enabled = true;//Enabled also when no trails/activities (Controller.TrailController.Instance.ReferenceActivity != null);
            enabled = (m_editTrail == null);

            btnAdd.Enabled = enabled;
            TrailName.Enabled = enabled;

            enabled = (m_editTrail == null);//Enabled also when no trails/activities (Controller.TrailController.Instance.CurrentActivityTrailDisplayed != null);
            btnEdit.Enabled = enabled;
            btnEdit.CenterImage = CommonIcons.Edit;

            if (Controller.TrailController.Instance.CurrentActivityTrailIsSelected)
            {
                TrailName.Text = Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail.Name;
                if (Controller.TrailController.Instance.CurrentActivityTrails.Count > 1)
                {
                    string s = "";
                    for (int i = 1; i < Controller.TrailController.Instance.CurrentActivityTrails.Count; i++)
                    {
                        ActivityTrail at = Controller.TrailController.Instance.CurrentActivityTrails[i];
                        s += at.Trail.Name;
                        if(i < Controller.TrailController.Instance.CurrentActivityTrails.Count-1)
                        {
                            s += ", ";
                        }
                    }
                    TrailName.Text += " (" + Controller.TrailController.Instance.CurrentActivityTrails.Count + ": "+s+")";
                }
                TrailName.Enabled = (m_editTrail == null);
                if (IsSettingsIcon())
                {
                    btnEdit.CenterImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Settings16;
                }
            }
            else
            {
                TrailName.Text = Properties.Resources.Trail_NoTrailSelected;
            }
            enabled = enabled && ((!Controller.TrailController.Instance.CurrentActivityTrailIsSelected) || !Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail.Generated);
            btnDelete.Enabled = enabled;
            this.useDeviceDistanceMenuItem.Checked = Data.Settings.UseDeviceDistance;
            this.setRestLapsAsPausesMenuItem.Checked = Data.Settings.RestIsPause;
            this.nonReqIsPauseMenuItem.Checked = Data.Settings.NonReqIsPause;
            this.ShowPausesAsResultsMenuItem.Checked = Data.Settings.ShowPausesAsResults;
            this.ResultSummaryStdDevMenuItem.Checked = !Data.Settings.ResultSummaryTotal;
            this.showSummaryTotalMenuItem.Checked = Data.Settings.ShowSummaryTotal;
            this.showSummaryAverageMenuItem.Checked = Data.Settings.ShowSummaryAverage;
            this.ResultSummaryStdDevMenuItem.Checked = Data.Settings.ResultSummaryStdDev;
            this.showOnlyMarkedResultsOnMapMenuItem.Checked = Data.Settings.ShowOnlyMarkedOnRoute;
            this.selectSimilarSplitsMenuItem.Checked = Data.Settings.SelectSimilarSplits;
            this.trailPointsMenuItem.Checked = Data.Settings.ShowTrailPointsOnMap;
            //this.showToolBarMenuItem.Checked = Data.Settings.ShowListToolBar;
            this.showToolBarMenuItem.Text = Data.Settings.ShowListToolBar ?
                Properties.Resources.UI_Activity_Menu_HideToolBar :
                Properties.Resources.UI_Activity_Menu_ShowToolBar;
        }


        /************************************************************/
        private bool IsSettingsIcon()
        {
            return Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail.Generated &&
                    Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail.TrailType != Trail.CalcType.ElevationPoints;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
#if ST_2_1
            IMapControl mapControl = m_layer.MapControl;
            ICollection<IMapControlObject> selectedGPS = null;
            if (null != mapControl) { selectedGPS = mapControl.Selected; }
#else
            IList<IItemTrackSelectionInfo> selectedGPS = TrailsItemTrackSelectionInfo.SetAndAdjustFromSelectionFromST(m_view.RouteSelectionProvider.SelectedItems, m_page.ViewActivities);
#endif

            if (TrailsItemTrackSelectionInfo.ContainsData(selectedGPS))
            {
#if ST_2_1
                m_layer.SelectedGPSLocationsChanged += new System.EventHandler(layer_SelectedGPSLocationsChanged_AddTrail);
                m_layer.CaptureSelectedGPSLocations();
#else
                //copying the existing result if generated, popup new empty trail otherwise
                bool copyActivity = Controller.TrailController.Instance.PrimaryCurrentActivityTrail != null &&
                    Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail.Generated;

                bool newTrail = true;
                if (Controller.TrailController.Instance.CurrentActivityTrailIsSelected)
                {
                    string s;
                    if (copyActivity)
                    {
                        s = Properties.Resources.UI_Activity_Page_AddTrail_Generated;
                    }
                    else
                    {
                        s = Properties.Resources.UI_Activity_Page_AddTrail_Replace;
                    }
                    //popup to add to current or add a new trail
                    DialogResult popRes = MessageDialog.Show(string.Format(s,
                            CommonResources.Text.ActionYes, CommonResources.Text.ActionNo),
                          "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (popRes == DialogResult.Cancel)
                    {
                        return;
                    }
                    if (popRes == DialogResult.Yes)
                    {
                        newTrail = false;
                    }
                }

                EditTrail dialog = new EditTrail(m_visualTheme, m_culture, m_page, m_view, m_layer, newTrail, copyActivity, Controller.TrailController.Instance.ReferenceTrailResult);
                if (newTrail)
                {
                    dialog.Trail.TrailLocations.Clear();
                }
                if (newTrail || !copyActivity)
                {
                    dialog.Trail.TrailLocations = Trail.MergeTrailLocations(dialog.Trail.TrailLocations, GetGPS(dialog.Trail, m_page.ViewActivities, selectedGPS));
                }

                ShowEditDialog(dialog);
#endif
            }
            else
            {
#if ST_2_1
                string message = String.Format(Properties.Resources.UI_Activity_Page_SelectPointsError_ST2, 
                   Properties.Resources.Trail_Reference_Name);
                MessageDialog.Show(message, "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
#else
                //Add a copy of the current activity
                EditTrail dialog = new EditTrail(m_visualTheme, m_culture, m_page, m_view, m_layer, true, true, Controller.TrailController.Instance.ReferenceTrailResult);
                ShowEditDialog(dialog);
#endif
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (Controller.TrailController.Instance.PrimaryCurrentActivityTrail == null)
            {
                //nothing selected, new trail
                EditTrail dialog = new EditTrail(m_visualTheme, m_culture, m_page, m_view, m_layer, true, false, Controller.TrailController.Instance.ReferenceTrailResult);
                ShowEditDialog(dialog);
            }
            else if (IsSettingsIcon())
            {
                Guid pageId;

                if (Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail.TrailType == Trail.CalcType.HighScore)
                {
                    pageId=GUIDs.HighScorePluginMain;
                }
                else if (Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail.TrailType == Trail.CalcType.UniqueRoutes)
                {
                    pageId=GUIDs.UniqueRoutesPluginMain;
                }
                else
                {
                    //All other generated trails, go to Trails settings
                    pageId=GUIDs.Settings;
                }
                Guid view = GUIDs.SettingsView;
                String bookmark = "PageId=" + pageId.ToString();
                Plugin.GetApplication().ShowView(view, bookmark);
                return;
            }
            else
            {
                //Do not care about marked points when editing, the user must select add then
                EditTrail dialog = new EditTrail(m_visualTheme, m_culture, m_page, m_view, m_layer, false, false, Controller.TrailController.Instance.ReferenceTrailResult);
                ShowEditDialog(dialog);
            }
        }

        private void ShowEditDialog(EditTrail dialog)
        {
            m_editTrail = dialog;
            m_layer.editTrail = dialog;
            m_page.RefreshControlState(); 
            
            dialog.TopMost = true;
            dialog.FormClosed += new FormClosedEventHandler(EditTrail_FormClosed);
            dialog.Show();
        }

        void EditTrail_FormClosed(object sender, FormClosedEventArgs e)
        {
            //There is no need to check the sender or the result, edit form handles calculation and revert
            m_editTrail = null;
            m_layer.editTrail = null;

            m_page.RefreshControlState();
            m_page.RefreshData(false);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (Controller.TrailController.Instance.CurrentActivityTrailIsSelected &&
                MessageDialog.Show(Properties.Resources.UI_Activity_Page_DeleteTrailConfirm, Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                Controller.TrailController.Instance.DeleteCurrentTrail();
                m_page.RefreshControlState();
                m_page.RefreshData(false);
            }
        }

        private void BtnMenu_Click(object sender, EventArgs e)
        {
            ZoneFiveSoftware.Common.Visuals.Button btnSender = (ZoneFiveSoftware.Common.Visuals.Button)sender;
            Point ptLowerLeft = new Point(btnSender.Width-chartPanelMenu.Width, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            chartPanelMenu.Show(ptLowerLeft);
        }

        void SelectSimilarSplitsMenuItem_Click(object sender, System.EventArgs e)
        {
            Data.Settings.SelectSimilarSplits = !Data.Settings.SelectSimilarSplits;
            this.RefreshControlState();
            if (Data.Settings.SelectSimilarSplits)
            {
                m_page.SelectSimilarSplitsChanged();
            }
        }

        void UseDeviceDistanceMenuItem_Click(object sender, System.EventArgs e)
        {
            Data.Settings.UseDeviceDistance = !Data.Settings.UseDeviceDistance;
            this.RefreshControlState();
            m_page.RefreshData(true);
        }

        void SetRestLapsAsPausesMenuItem_Click(object sender, System.EventArgs e)
        {
            Data.Settings.RestIsPause = !Data.Settings.RestIsPause;
            this.RefreshControlState();
            Controller.TrailController.Instance.CurrentReset(false); //TBD
            m_page.RefreshData(true);
        }

        void NonReqIsPauseMenuItem_Click(object sender, System.EventArgs e)
        {
            Data.Settings.NonReqIsPause = !Data.Settings.NonReqIsPause;
            this.RefreshControlState();
            Controller.TrailController.Instance.CurrentReset(false); //TBD
            m_page.RefreshData(true);
        }
        
        void ShowPausesAsResultsMenuItem_Click(object sender, System.EventArgs e)
        {
            Data.Settings.ShowPausesAsResults = !Data.Settings.ShowPausesAsResults;
            this.RefreshControlState();
            Controller.TrailController.Instance.CurrentReset(false);
            m_page.RefreshData(true);
        }

        void ResultSummaryStdDevMenuItem_Click(object sender, System.EventArgs e)
        {
            Data.Settings.ResultSummaryStdDev = !Data.Settings.ResultSummaryStdDev;
            this.RefreshControlState();
            m_page.RefreshSummary();
        }

        void ShowSummaryTotalMenuItem_Click(object sender, System.EventArgs e)
        {
            Data.Settings.ShowSummaryTotal = !Data.Settings.ShowSummaryTotal;
            this.RefreshControlState();
            m_page.RefreshSummary();
            m_page.RefreshData(false);
        }

        void ShowSummaryAverageMenuItem_Click(object sender, System.EventArgs e)
        {
            Data.Settings.ShowSummaryAverage = !Data.Settings.ShowSummaryAverage;
            this.RefreshControlState();
            m_page.RefreshSummary();
            m_page.RefreshData(false);
        }

        void ShowOnlyMarkedResultsOnMapMenuItem_Click(object sender, System.EventArgs e)
        {
            Data.Settings.ShowOnlyMarkedOnRoute = !Data.Settings.ShowOnlyMarkedOnRoute;
            this.RefreshControlState();
            m_page.RefreshData(false);
        }

        void TrailPointsMenuItem_Click(object sender, System.EventArgs e)
        {
            Data.Settings.ShowTrailPointsOnMap = !Data.Settings.ShowTrailPointsOnMap;
            this.RefreshControlState();
            m_page.RefreshData(false);
        }

        private void RunGradeAdjustMenuItem_Click(object sender, EventArgs e)
        {
            Data.Settings.IncreaseRunningGradeCalcMethod(true);
            //Must reset calculations (if not refreshing)
            m_page.RefreshData(true);
        }

        void ChartPanelMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.runGradeAdjustMenuItem.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelGrade + ": " + Data.Settings.RunningGradeAdjustMethod.ToString();
            e.Cancel = false;
        }

        private void ShowToolBarMenuItem_Click(object sender, EventArgs e)
        {
            Data.Settings.ShowListToolBar = !Data.Settings.ShowListToolBar;
            m_page.ShowListToolBar();
            RefreshControlState();
        }

        private void SettingsToolBarMenuItem_Click(object sender, EventArgs e)
        {
            // go to Trails settings
            Guid view = GUIDs.SettingsView;
            String bookmark = "PageId=" + GUIDs.Settings.ToString();
            Plugin.GetApplication().ShowView(view, bookmark);
        }

        /*************************************************************************************************************/
        //ST3
        //TODO: Rewrite, using IItemTrackSelectionInfo help functions?
        private static IList<TrailGPSLocation> GetGPS(Trail trail, IList<IActivity> activities, IValueRange<DateTime> ts, IValueRange<double> di, string id)
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

        public static IList<TrailGPSLocation> GetGPS(Trail trail, IList<IActivity> activities,  IList<IItemTrackSelectionInfo> aSelectGPS)
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
                        result2 = Trail.MergeTrailLocations(result2, GetGPS(trail, activities, ts, null, aSelectGPS[i].ItemReferenceId));
                    }
                }
                if (result2.Count == 0)
                {
                    if (result2.Count == 0)
                    {
                        //Selected
                        result2 = GetGPS(trail, activities, selectGPS.SelectedTime, null, aSelectGPS[i].ItemReferenceId);
                    }
                }
                result = Trail.MergeTrailLocations(result, result2);
            }
            return result;
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
            treeListPopup.Tree.ShowPlusMinus = true;
            treeListPopup.Tree.ShowLines = true;

            //Note: Just checking for current trail could modify the ordered list, so do this first
            IList<ActivityTrail> selectedActivityTrails = new List<ActivityTrail>();
            if (Controller.TrailController.Instance.CurrentActivityTrailIsSelected)
            {
                selectedActivityTrails = Controller.TrailController.Instance.CurrentActivityTrails;
            }
            //Get the list of ordered activity trails, without forcing calculation
            IList<ActivityTrail> allActivityTrails = Controller.TrailController.Instance.OrderedTrails();

            TrailNameWrapper tnw = new TrailNameWrapper(allActivityTrails, selectedActivityTrails);
            treeListPopup.Tree.RowData = tnw.RowData;
            treeListPopup.Tree.Expanded = tnw.Expanded;
#if ST_2_1
            treeListPopup.Tree.Selected = tnw.SelectedItems;
#else
            treeListPopup.Tree.SelectedItems = tnw.SelectedItems;
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
                TreeListPopup popup = sender as TreeListPopup;
                popup.Hide();

                IList<ActivityTrail> ats = new List<ActivityTrail>();
                if (m_selectTrailAddMode)//|| this.m_CtrlPressed)
                {
                    foreach (ActivityTrail at in Controller.TrailController.Instance.CurrentActivityTrails)
                    {
                        ats.Add(at);
                    }
                }
                ActivityTrail selAt = ((ActivityTrailWrapper)((TreeListPopup.ItemSelectedEventArgs)e).Item).ActivityTrail;
                if (ats.Contains(selAt))
                {
                    ats.Remove(selAt);
                }
                else
                {
                    ats.Add(selAt);
                }

                System.Windows.Forms.ProgressBar progressBar = m_page.StartProgressBar(0);
                Controller.TrailController.Instance.SetCurrentActivityTrail(ats, true, progressBar);
                m_page.StopProgressBar();
                m_page.RefreshData(false);
                m_page.RefreshControlState();

                //Set current viewed area to the selected trail
                if (ats.Contains(selAt))
                {
                    GPSBounds area = TrailGPSLocation.getGPSBounds(selAt.Trail.TrailLocations, 3 * selAt.Trail.Radius);
                    m_layer.SetLocation(area);
                }
            }
        }
    }
}
