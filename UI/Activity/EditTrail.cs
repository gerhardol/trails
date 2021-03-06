﻿/*
Copyright (C) 2009 Brendan Doherty

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

using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Data.GPS;
using TrailsPlugin.Data;
using TrailsPlugin.UI.MapLayers;
using GpsRunningPlugin.Util;
using TrailsPlugin.Utils;

namespace TrailsPlugin.UI.Activity
{
    public partial class EditTrail : Form
    {

        protected ITheme m_visualTheme;
        protected bool m_addMode;
        protected Data.Trail m_TrailToEdit; //Scratch, the copy of the trail add or newly created
        private ActivityDetailPageControl m_page = null;
#if ST_2_1
        private UI.MapLayers.MapControlLayer m_layer { get { return UI.MapLayers.MapControlLayer.Instance; } }
#else
        private TrailPointsLayer m_layer;
        private IDailyActivityView m_view;
#endif
        private TrailResult m_trailResult;
        private /*readonly*/ int cMaxEditColumn;

        private MouseEventArgs m_lastMouseArg = null;
        private string m_subItemText;
        private int m_rowDoubleClickSelected = 0;
        private int m_subItemSelected = 0;
        private bool m_updatingFromMap = false;

        private EditTrail(bool addMode, bool copyMode)
        {
            this.m_addMode = addMode;

            InitializeComponent();

            if (Controller.TrailController.Instance.PrimaryCurrentActivityTrail == null)
            {
                this.m_addMode = true;
                copyMode = false;
            }

            if (this.m_addMode && !copyMode)
            {
                //new empty trail
                this.m_TrailToEdit = new TrailsPlugin.Data.Trail();
            }
            else
            {
                Trail trail = Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail;
                if (copyMode || trail.Generated)
                {
                    //Add a copy of the trail from the (reference) activity
                    IActivity activity = Controller.TrailController.Instance.ReferenceActivity;
                    this.m_TrailToEdit = trail.Copy(activity, this.m_addMode);
                }
                else
                {
                    //Make a copy (with same Guid) of current, so changes can be undone
                    this.m_TrailToEdit = trail.Duplicate();
                }
            }

            if (this.m_addMode)
            {
                this.Name = Properties.Resources.UI_Activity_EditTrail_AddTrail;
            }
            else
            {
                this.Name = Properties.Resources.UI_Activity_EditTrail_EditTrail;
            }
            this.Text = this.Name;
            //Done after init code, as it depends on it
            this.InitControls();
        }

#if ST_2_1
        public EditTrail(ITheme visualTheme, System.Globalization.CultureInfo culture, Object view, bool addMode)
#else
        public EditTrail(ITheme visualTheme, System.Globalization.CultureInfo culture, ActivityDetailPageControl page, 
            IDailyActivityView view, TrailPointsLayer layer, bool addMode, bool copy, TrailResult tr)
#endif
            : this (addMode, copy)
        {
#if !ST_2_1
            this.m_page = page;
            this.m_view = view;
            this.m_layer = layer;
            this.m_layer.TrailPoints = m_TrailToEdit.TrailLocations;
#endif
            this.m_trailResult = tr;
            //It is possible that the trail result is not for the trail to edit (if more than one is selected)
            if (tr != null && tr.Trail != Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail)
            {
                this.m_trailResult = null;
            }

            ThemeChanged(visualTheme);
            UICultureChanged(culture);
        }

        void InitControls()
        {
            this.boxDefActivity.ButtonImage = CommonIcons.MenuCascadeArrowDown;
            this.btnAdd.CenterImage = CommonIcons.Add;
            this.btnAdd.Text = "";
            this.btnEdit.CenterImage = CommonIcons.Edit;
            this.btnEdit.Text = "";
            this.btnDelete.CenterImage = CommonIcons.Delete;
            this.btnDelete.Text = "";
            this.btnDelete.Enabled = false;
            this.btnUp.CenterImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.MoveUp16;
            this.btnUp.Text = "";
            this.btnUp.Enabled = false;
            this.btnDown.CenterImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.MoveDown16;
            this.btnDown.Text = "";
            this.btnDown.Enabled = false;
            this.btnCopy.Text = "";
            this.btnCopy.CenterImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.DocumentCopy16;
            if (this.m_addMode)
            {
                this.btnCopy.Enabled = false;
            }
            this.btnExport.Text = "";
            this.btnExport.CenterImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Export16;
            this.btnRefresh.Text = "";
            this.btnRefresh.CenterImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Refresh16;
            this.btnReverse.Text = "";
            this.btnReverse.CenterImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.MoveUp16;

            this.chkTwoWay.Checked = this.m_TrailToEdit.BiDirectional;
            this.chkTemporaryTrail.Checked = this.m_TrailToEdit.IsTemporary;
            this.chkName.Checked = this.m_TrailToEdit.IsNameMatch;
            this.chkCompleteActivity.Checked = this.m_TrailToEdit.IsCompleteActivity;
            this.chkURFilter.Checked = this.m_TrailToEdit.IsURFilter;
            this.numericSortPrio.Value = this.m_TrailToEdit.TrailPriority;
            //this.chkAutoTryAll.Checked = this.m_TrailToEdit.IsAutoTryAll;

            this.chkTwoWay.CheckedChanged += new System.EventHandler(this.ChkTwoWay_CheckedChanged);
            this.chkTemporaryTrail.CheckedChanged += new System.EventHandler(this.ChkTemporaryTrail_CheckedChanged);
            this.chkName.CheckedChanged += new System.EventHandler(this.ChkName_CheckedChanged);
            this.chkCompleteActivity.CheckedChanged += new System.EventHandler(this.ChkCompleteActivity_CheckedChanged);
            this.chkURFilter.CheckedChanged += new System.EventHandler(this.ChkURFilter_CheckedChanged);
            //this.chkAutoTryAll.CheckedChanged += new System.EventHandler(this.chkAutoTryAll_CheckedChanged);
            this.numericSortPrio.ValueChanged += new System.EventHandler(this.NumericUpDown1_ValueChanged);
#if ST_2_1
            this.EList.SelectedChanged += new System.EventHandler(EList_SelectedItemsChanged);
#else
            this.EList.SelectedItemsChanged += new System.EventHandler(EList_SelectedItemsChanged);
#endif
            this.EList.LabelProvider = new EditTrailLabelProvider();
        }
        
        public virtual void ThemeChanged(ITheme visualTheme)
        {
            this.m_visualTheme = visualTheme;
            this.BackColor = visualTheme.Control;
            this.EList.ThemeChanged(visualTheme);
            this.TrailName.ThemeChanged(visualTheme);
            this.radiusBox.ThemeChanged(visualTheme);
            this.boxDefActivity.ThemeChanged(visualTheme);
            this.editBox.ThemeChanged(visualTheme);
        }

        public void UICultureChanged(System.Globalization.CultureInfo culture)
        {
            this.lblTrail.Text = Properties.Resources.TrailName + ":";
            this.lblRadius.Text = Properties.Resources.UI_Activity_EditTrail_Radius + ":";
            this.lblDefActivity.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelActivity + ":";
            this.toolTip.SetToolTip(this.lblDefActivity, "The default reference activity for the trail.");
            this.btnOk.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionOk;
            this.btnCancel.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionCancel;
            this.PresentRadius();
            this.PresentDefAct();
            this.toolTip.SetToolTip(this.btnCopy, "Create a Copy of the Trail");
            this.toolTip.SetToolTip(this.btnExport, "Export Trail to Activity");
            this.toolTip.SetToolTip(this.btnRefresh, "Refresh Calculation");
            this.toolTip.SetToolTip(this.btnReverse, "Reverse TrailPoints");
            this.toolTip.SetToolTip(this.chkTemporaryTrail, "Temporary Trail (deleted when exiting SportTracks)");
            this.toolTip.SetToolTip(this.chkTwoWay, "Two-way Match");
            this.toolTip.SetToolTip(this.chkName, "Match by Name");
            this.toolTip.SetToolTip(this.chkCompleteActivity, "Match Complete Activity");
            this.toolTip.SetToolTip(this.chkURFilter, "Filter with UniqueRoutes");
            this.toolTip.SetToolTip(this.numericSortPrio, "Sort Priority");
            //this.toolTip.SetToolTip(this.chkAutoTryAll, "Calculate Trail in automatic updates");
        }

        public void UpdatePointFromMap(TrailGPSLocation point)
        {
            IList<EditTrailRow> list = (IList<EditTrailRow>)this.EList.RowData;
            for (int i = 0; i < list.Count; i++)
            {
                if (point == list[i].TrailGPS)
                {
                    this.m_updatingFromMap = true;
                    this.EList.SelectedItems = new object[] { list[i] };
                    this.m_updatingFromMap = false;
                    this.EList.RefreshElements(this.EList.SelectedItems);
                    break;
                }
            }
        }

        public Data.Trail Trail
        {
            get
            {
                return this.m_TrailToEdit;
            }
        }

        private void RefreshResult(bool recalculate)
        {
            if (Controller.TrailController.Instance.ReferenceActivity != null)
            {
                if (recalculate || this.m_trailResult == null)
                {
                    ActivityTrail at = new ActivityTrail(this.m_TrailToEdit);
                    at.CalcResults(new List<IActivity> { Controller.TrailController.Instance.ReferenceActivity }, this.m_TrailToEdit.MaxRequiredMisses, true, null);
                    if (at.Results.Count > 0)
                    {
                        this.m_trailResult = at.Results[0].Result;
                    }
                    else
                    {
                        at.Init();
                        at.CalcResults(new List<IActivity> { Controller.TrailController.Instance.ReferenceActivity }, 99, true, null);
                        if (at.Results.Count > 0)
                        {
                            //The best result is the result with most matches
                            //forward may be better than reverse, but those should be sorted first anyway
                            int currMaxRes = -1;
                            foreach (TrailResultWrapper tr in at.Results)
                            {
                                int res = 0;
                                foreach (DateTime d in tr.Result.TrailPointDateTime)
                                {
                                    if (d > DateTime.MinValue)
                                    {
                                        res++;
                                    }
                                }
                                if (res > currMaxRes)
                                {
                                    currMaxRes = res;
                                    this.m_trailResult = tr.Result;
                                }
                            }
                        }
                        else
                        {
                            if (at.IncompleteResults.Count > 0)
                            {
                                //Result is already sorted after no of matches
                                this.m_trailResult = at.IncompleteResults[0];
                            }
                            else
                            {
                                this.m_trailResult = null;
                            }
                        }
                    }
                    at.Init();
                }
            }
            IList<EditTrailRow> l = EditTrailRow.getEditTrailRows(this.m_TrailToEdit, this.m_trailResult);
            int sel = EList_SelectedRow(); //Get one of the selected, if any
            this.EList.RowData = l;
            if (sel >= 0)
            {
                //This is incorrect if the trail was reversed
                this.EList.SelectedItems = new object[] { l[sel] };
            }

            foreach (EditTrailRow t in (IList<EditTrailRow>)this.EList.RowData)
            {
                //Note: For reverse results, this is incorrect (but reverse results are only for incomplete, so no impact)
                this.EList.SetChecked(t, t.TrailGPS.Required);
            }
        }
        
        ///////////////////////////
        private void BtnCancel_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void BtnOk_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(this.TrailName.Text))
            {
                MessageDialog.Show(Properties.Resources.UI_Activity_EditTrail_TrailNameReqiured);
                return;
            }

            Data.Trail trail = Data.TrailData.GetFromName(this.TrailName.Text);
            if (trail != null && (this.m_addMode ||
                !this.m_addMode && trail.Id != this.m_TrailToEdit.Id))
            {
                MessageDialog.Show(Properties.Resources.UI_Activity_EditTrail_UniqueTrailNameRequired);
                return;
            }
            if (this.m_addMode && !Controller.TrailController.Instance.AddTrail(this.m_TrailToEdit, null) ||
                !this.m_addMode && !Controller.TrailController.Instance.UpdateTrail(this.m_TrailToEdit, null))
            {
                MessageDialog.Show(Properties.Resources.UI_Activity_EditTrail_UpdateFailed);
                return;
            }
            this.DialogResult = DialogResult.OK;
            //Successful: Clear marking on route (otherwise same point could be added again)
            this.m_page.ClearCurrentSelectedOnRoute();

            Close();
        }

        void BtnCopy_Click(object sender, System.EventArgs e)
        {
            this.m_TrailToEdit.Name += " " + CommonResources.Text.ActionCopy;
            this.TrailName.Text = this.m_TrailToEdit.Name;
            this.m_addMode = true;
            //Disable copy, as the previous copy is not handled otherwise
            this.btnCopy.Enabled = false;
        }

        void BtnExport_Click(object sender, System.EventArgs e)
        {
            if (MessageDialog.Show(string.Format(Properties.Resources.UI_Activity_EditTrail_Export, CommonResources.Text.ActionOk, CommonResources.Text.ActionCancel),
                "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                DateTime startTime = DateTime.UtcNow;
                IActivity activity;
                bool hasGps = this.m_trailResult != null && this.m_trailResult.GPSRoute != null;
                if (hasGps)
                {
                    activity = this.m_trailResult.CopyToActivity();
                    activity.Laps.Clear();
                }
                else
                {
                    activity = Plugin.GetApplication().Logbook.Activities.Add(startTime);
                    activity.GPSRoute = new TrackUtil.GPSRoute();
                    TrackUtil.setCapacity(activity.GPSRoute, ((IList<EditTrailRow>)this.EList.RowData).Count);
                }
                activity.Name = TrailName.Text;
                activity.Notes += "Radius: " + UnitUtil.Elevation.ToString(m_TrailToEdit.Radius, "u");
                const int lapLength = 60; //A constant time between points
                IList<EditTrailRow> list = (IList<EditTrailRow>)this.EList.RowData;
                for (int i = 0; i < list.Count - 1; i++)
                {
                    if (hasGps)
                    {
                        activity.Laps.Add(m_trailResult.TrailPointDateTime[i],
                           TimeSpan.FromSeconds(m_trailResult.TrailPointTime0(m_trailResult)[i + 1] -
                           this.m_trailResult.TrailPointTime0(m_trailResult)[i]));
                    }
                    else
                    {
                        activity.GPSRoute.Add(startTime.AddSeconds(i * lapLength), 
                            new GPSPoint(list[i].TrailGPS.LatitudeDegrees, list[i].TrailGPS.LongitudeDegrees, 0));
                        activity.Laps.Add(startTime.AddSeconds(i * lapLength), TimeSpan.FromSeconds(lapLength));
                    }
                    activity.Laps[i].Rest = !list[i].TrailGPS.Required;
                    activity.Laps[i].Notes = list[i].TrailGPS.Name;
                }
                if (!hasGps)
                {
                    int i = list.Count - 1;
                    activity.GPSRoute.Add(startTime.AddSeconds(i * lapLength), new GPSPoint(list[i].TrailGPS.LatitudeDegrees, list[i].TrailGPS.LongitudeDegrees, 0));
                }
            }
        }

        void BtnRefresh_Click(object sender, System.EventArgs e)
        {
            RefreshResult(true);
        }

        private void EditTrail_Activated(object sender, System.EventArgs e)
        {
            this.TrailName.Focus();
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
            base.OnPaint(e);
            ZoneFiveSoftware.Common.Visuals.MessageDialog.DrawButtonRowBackground(e.Graphics, this, m_visualTheme);
        }

        private void EditTrail_Shown(object sender, System.EventArgs e)
        {
            this.TrailName.Text = m_TrailToEdit.Name;
            PresentRadius();

            this.EList.Columns.Clear();
            this.EList.CheckBoxes = true;
            this.EList.Columns.Add(new TreeList.Column(EditTrailColumnIds.Required, Properties.Resources.Required, 20, StringAlignment.Far));
            this.EList.Columns.Add(new TreeList.Column(EditTrailColumnIds.LongitudeDegrees, Properties.Resources.UI_Activity_EditTrail_Longitude, 70, StringAlignment.Far));
            this.EList.Columns.Add(new TreeList.Column(EditTrailColumnIds.LatitudeDegrees, Properties.Resources.UI_Activity_EditTrail_Latitude, 70, StringAlignment.Far));
            this.EList.Columns.Add(new TreeList.Column(EditTrailColumnIds.Name, CommonResources.Text.LabelName, 80, StringAlignment.Near));
            this.EList.Columns.Add(new TreeList.Column(EditTrailColumnIds.ElevationMeters, CommonResources.Text.LabelElevation, 60, StringAlignment.Far));
            this.EList.Columns.Add(new TreeList.Column(EditTrailColumnIds.Distance, CommonResources.Text.LabelDistance, 60, StringAlignment.Far));
            this.cMaxEditColumn = EList.Columns.Count;
            this.EList.Columns.Add(new TreeList.Column(EditTrailColumnIds.Time, CommonResources.Text.LabelTime, 60, StringAlignment.Far));
            this.EList.Columns.Add(new TreeList.Column(EditTrailColumnIds.Diff, TrailResultColumnIds.Diff, 60, StringAlignment.Far));
            this.EList.Columns.Add(new TreeList.Column(EditTrailColumnIds.ResultElevation, CommonResources.Text.LabelElevation, 60, StringAlignment.Far));

            RefreshResult(false);

            this.EList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SMKMouseDown);
            this.EList.DoubleClick += new System.EventHandler(this.SMKDoubleClick);
            this.EList.KeyDown += new KeyEventHandler(EList_KeyDown);
            this.EList.CheckedChanged += new TreeList.ItemEventHandler(EList_CheckedChanged);
        }

        private int EList_SelectedRow()
        {
            int result = -1;
            if (this.EList.SelectedItems.Count > 0 && ((IList<EditTrailRow>)this.EList.RowData).Count > 0)
            {
                IList selected = EList.SelectedItems;
                if (selected != null && selected.Count > 0)
                {
                    for (int j = selected.Count - 1; j >= 0; j--)
                    {
                        IList<EditTrailRow> list = ((IList<EditTrailRow>)this.EList.RowData);
                        for (int i = 0; i < list.Count; i++)
                        {
                            //Only first selected
                            if (selected[j].Equals(list[i]))
                            {
                                result = i;
                                break;
                            }
                        }
                    }
                }
            }
            return result;
        }

        private void EList_DeleteRow()
        {
            int lastDeleted = -1;
            if (this.EList.SelectedItems.Count > 0 && ((IList<EditTrailRow>)this.EList.RowData).Count > 0)
            {
                IList selected = EList.SelectedItems;
                if (selected != null && selected.Count > 0)
                {
                    for (int j = selected.Count - 1; j >= 0; j--)
                    {
                        IList<EditTrailRow> list = ((IList<EditTrailRow>)this.EList.RowData);
                        for (int i = 0; i < list.Count; i++)
                        {
                            //Only first selected
                            if (selected[j].Equals(list[i]))
                            {
                                ((IList<EditTrailRow>)EList.RowData).RemoveAt(i);
                                //Required to make ST see the update, Refresh() is not enough?
                                this.EList.RowData = ((IList<EditTrailRow>)this.EList.RowData);
                                m_TrailToEdit.TrailLocations = EditTrailRow.getTrailGPSLocation((IList<EditTrailRow>)EList.RowData);
                                m_layer.TrailPoints = m_TrailToEdit.TrailLocations;
                                lastDeleted = i;
                                break;
                            }
                        }
                    }
                }
            }
            this.EList.Refresh();
            if (EList.SelectedItems.Count == 1 && lastDeleted >= 0)
            {
                if (lastDeleted > 0) { lastDeleted--; }
                if (lastDeleted < ((IList<EditTrailRow>)this.EList.RowData).Count)
                {
                    EList.SelectedItems = new object[] { ((IList<EditTrailRow>)this.EList.RowData)[lastDeleted] };
                }
            }
        }

        private void EList_AddRow()
        {
            int i = EList_SelectedRow();
            if (i < 0)
            {
                if (((IList<EditTrailRow>)this.EList.RowData).Count > 0)
                {
                    i = ((IList<EditTrailRow>)this.EList.RowData).Count - 1;
                }
                else
                {
                    i = 0;
                }
            }
            TrailGPSLocation sel;
            if (((IList<EditTrailRow>)this.EList.RowData).Count > 0)
            {
                sel = new TrailGPSLocation(((IList<EditTrailRow>)this.EList.RowData)[i].TrailGPS);
                sel.GpsLoc = new GPSLocation(sel.LatitudeDegrees+0.0001F, sel.LongitudeDegrees+0.0001F);
            }
            else
            {
                IGPSLocation l = m_layer.GetCenterMap();
                sel = new TrailGPSLocation(l, this.m_TrailToEdit.Radius);
            }

            sel.Name += " " + ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionNew;

            //If a point is selected on the track, use it instead
            IList<IActivity> activities = new List<IActivity> { Controller.TrailController.Instance.ReferenceActivity };
            IList<IItemTrackSelectionInfo> selectedGPS =
                        TrailsItemTrackSelectionInfo.SetAndAdjustFromSelectionFromST(m_view.RouteSelectionProvider.SelectedItems, activities);
            if (TrailsItemTrackSelectionInfo.ContainsData(selectedGPS))
            {
                IList<TrailGPSLocation> loc = TrailSelectorControl.GetGPS(this.Trail, activities, selectedGPS);
                if (loc.Count > 0 && loc[0] != null)
                {
                    sel.GpsLoc = loc[0];
                }
            }

            EditTrailRow newRow = new EditTrailRow(sel);
            if (((IList<EditTrailRow>)this.EList.RowData).Count > 0)
            {
                ((IList<EditTrailRow>)this.EList.RowData).Insert(i + 1, newRow);
            }
            else
            {
                this.EList.RowData = new List<EditTrailRow> { newRow };
            }

            //Make ST see the update
            this.EList.RowData = ((IList<EditTrailRow>)this.EList.RowData);
            foreach (EditTrailRow t in (IList<EditTrailRow>)this.EList.RowData)
            {
                //Note: For reverse results, this is incorrect (but reverse results are only for incomplete, so no impact)
                this.EList.SetChecked(t, t.TrailGPS.Required);
            }
            this.EList.SelectedItems = new object[] { newRow };
            this.EList.Refresh();

            this.m_TrailToEdit.TrailLocations = EditTrailRow.getTrailGPSLocation((IList<EditTrailRow>)this.EList.RowData);
            this.m_layer.TrailPoints = m_TrailToEdit.TrailLocations;
            this.m_layer.SelectedTrailPoints = new List<TrailGPSLocation> { sel };
            this.m_layer.Refresh();
        }

        void EList_CheckedChanged(object sender, TreeList.ItemEventArgs e)
        {
            if (e.Item is EditTrailRow)
            {
                EditTrailRow t = e.Item as EditTrailRow;
                t.TrailGPS.Required = !t.TrailGPS.Required;
            }
        }

        void TrailName_LostFocus(object sender, System.EventArgs e)
        {
            this.m_TrailToEdit.Name = this.TrailName.Text;
        }

        private void PresentRadius()
        {
            this.radiusBox.Text = UnitUtil.Elevation.ToString(this.m_TrailToEdit.Radius, "u");
        }

        private void Radius_LostFocus(object sender, System.EventArgs e)
        {
            float result;
            result = (float)UnitUtil.Elevation.Parse(this.radiusBox.Text);
            if (!float.IsNaN(result) && result >= 0)
            {
                this.m_TrailToEdit.Radius = result;
            }
            else
            {
                MessageDialog.Show(Properties.Resources.UI_Activity_EditTrail_RadiusNumeric);
            }
            this.PresentRadius();
            //Refresh on map
            this.m_layer.TrailPoints = this.m_TrailToEdit.TrailLocations;
        }

        private void ValidateEdit()
        {
            IList<EditTrailRow> t = (IList<EditTrailRow>)this.EList.RowData;
            if (this.EList.Columns[this.m_subItemSelected].Id == EditTrailColumnIds.Distance)
            {
                try
                {
                    double dist = UnitUtil.Distance.Parse(editBox.Text);
                    DateTime d1 = this.m_trailResult.GetDateTimeFromDistActivity((float)dist);
                    t[this.m_rowDoubleClickSelected].UpdateRow(this.m_trailResult, d1);
                }
                catch { }
            }
            else if (this.EList.Columns[this.m_subItemSelected].Id == EditTrailColumnIds.Time)
            {
                try
                {
                    double time = UnitUtil.Time.Parse(editBox.Text);
                    DateTime d1 = this.m_trailResult.GetDateTimeFromTimeActivity((float)time);
                    t[this.m_rowDoubleClickSelected].UpdateRow(m_trailResult, d1);
                }
                catch { }
            }
            else
            {
                //TrailGPS fields
                if (this.EList.Columns[this.m_subItemSelected].Id == EditTrailColumnIds.LatitudeDegrees)
                {
                    float pos = TrailGPSLocation.ParseLatLon(editBox.Text);
                    t[this.m_rowDoubleClickSelected].TrailGPS.SetLatitude(pos);
                }
                else if (this.EList.Columns[this.m_subItemSelected].Id == EditTrailColumnIds.LongitudeDegrees)
                {
                    float pos = TrailGPSLocation.ParseLatLon(editBox.Text);
                    t[this.m_rowDoubleClickSelected].TrailGPS.SetLongitude(pos);
                }
                else if (this.EList.Columns[this.m_subItemSelected].Id == EditTrailColumnIds.Name)
                {
                    t[this.m_rowDoubleClickSelected].TrailGPS.Name = editBox.Text;
                }
                else if (this.EList.Columns[this.m_subItemSelected].Id == EditTrailColumnIds.ElevationMeters)
                {
                    //Allow NaN
                    float pos = (float)GpsRunningPlugin.Util.UnitUtil.Elevation.Parse(editBox.Text);
                    t[this.m_rowDoubleClickSelected].TrailGPS.SetElevation(pos);
                }
                //Note: result need to be recalculated to be accurate. However, the recalc could find other results,
                //let the user request recalc manually
                t[this.m_rowDoubleClickSelected].UpdateRow(this.m_trailResult,
                    t[this.m_rowDoubleClickSelected].TrailGPS);
            }
            this.EList.RowData = t;
            this.m_layer.TrailPoints = this.m_TrailToEdit.TrailLocations;
            this.m_layer.SelectedTrailPoints = new List<TrailGPSLocation> { t[this.m_rowDoubleClickSelected].TrailGPS };
            this.m_layer.Refresh();
        }

        private void PresentDefAct()
        {
            if (this.m_TrailToEdit.DefaultRefActivity != null)
            {
                this.boxDefActivity.Text = this.m_TrailToEdit.DefaultRefActivity.StartTime.ToLongDateString() +
                    " " + this.m_TrailToEdit.DefaultRefActivity.Name;
            }
            else
            {
                this.boxDefActivity.Text = TrailsPlugin.Properties.Resources.UI_EditList_AutomaticRefActivity;
            }
        }

        private void BoxDefActivity_ButtonClick(object sender, EventArgs e)
        {
            TreeListPopup treeListPopup = new TreeListPopup();
            treeListPopup.ThemeChanged(m_visualTheme);
            treeListPopup.Tree.Columns.Add(new TreeList.Column());

            IList<object> acts = new List<object>{ TrailsPlugin.Properties.Resources.UI_EditList_AutomaticRefActivity };//TBD: How to handle null activity?
            System.Collections.IList currSel = new object[1] { acts[0] };
            foreach (IActivity act in TrailResultWrapper.Activities(TrailsPlugin.Controller.TrailController.Instance.Results))
            {
                acts.Add(act);
                if (act != null && act == this.Trail.DefaultRefActivity)
                {
                    currSel[0] = (IActivity)act;
                }
            }

            treeListPopup.Tree.RowData = acts;
#if ST_2_1
            treeListPopup.Tree.Selected = currSel;
#else
            treeListPopup.Tree.SelectedItems = currSel;
#endif
            treeListPopup.Tree.LabelProvider = new ActivityDropdownLabelProvider();
            treeListPopup.ItemSelected += new TreeListPopup.ItemSelectedEventHandler(BoxDefActivity_ItemSelected);
            treeListPopup.Popup(this.boxDefActivity.Parent.RectangleToScreen(this.boxDefActivity.Bounds));
        }

        private void BoxDefActivity_ItemSelected(object sender, EventArgs e)
        {
            if (sender is TreeListPopup)
            {
                ((TreeListPopup)sender).Hide();
            }

            if ((((TreeListPopup.ItemSelectedEventArgs)e).Item) is string)
            {
                this.Trail.DefaultRefActivity = null;
            }
            else
            {
                IActivity t = ((IActivity)((TreeListPopup.ItemSelectedEventArgs)e).Item);
                this.Trail.DefaultRefActivity = t;
            }
            PresentDefAct();
        }

        private void EditBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Return)
            {
                ValidateEdit();
                this.editBox.Visible = false;
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Escape)
            {
                this.editBox.Visible = false;
                e.Handled = true;
            }
        }

        private void EditBox_LostFocus(object sender, System.EventArgs e)
        {
            ValidateEdit();
            this.editBox.Visible = false;
        }

        public void SMKDoubleClick(object sender, System.EventArgs e)
        {
            TreeList.RowHitState hitState = TreeList.RowHitState.Nothing;
            EditTrailRow t = (EditTrailRow)EList.RowHitTest(new Point(this.m_lastMouseArg.X, this.m_lastMouseArg.Y), out hitState);

            if (t != null && hitState != TreeList.RowHitState.Nothing)
            {
                for (int i = 0; i < ((IList<EditTrailRow>)this.EList.RowData).Count; i++)
                {
                    EditTrailRow r = (EditTrailRow)((IList<EditTrailRow>)this.EList.RowData)[i];
                    if (t.Equals(r))
                    {
                        this.m_rowDoubleClickSelected = i;
                        break;
                    }
                }
                // Check the subitem clicked
                //TODO: Not handling resize (scrolling) correctly
                int nStart = this.m_lastMouseArg.Location.X;// EList.PointToScreen(m_lastMouseArg.Location).X;
                int spos = this.EList.Location.X;
                int epos = spos;
                for (int i = 0; i < EList.Columns.Count; i++)
                {
                    epos += this.EList.Columns[i].Width;
                    if (nStart >= spos && nStart < epos)
                    {
                        this.m_subItemSelected = i;
                        break;
                    }

                    spos = epos;
                }
                //Only edit first rows
                if (this.m_subItemSelected <= cMaxEditColumn)
                {
                    this.m_subItemText = (new EditTrailLabelProvider()).GetText(t, this.EList.Columns[this.m_subItemSelected]);
                    ///The positioning is incorrect, set at header
                    int rowHeight = this.EList.HeaderRowHeight;// (EList.Height - EList.HeaderRowHeight) / ((IList<TrailGPSLocation>)EList.RowData).Count;
                    int yTop = 0;// EList.HeaderRowHeight + rowSelected * rowHeight;
                    this.editBox.Size = new System.Drawing.Size(epos - spos, rowHeight);
                    this.editBox.Location = new System.Drawing.Point(spos - 1, yTop);
                    this.editBox.Text = m_subItemText;
                    this.editBox.Visible = true;
                    this.editBox.SelectAll();
                    this.editBox.Focus();
                }
                //else if (m_subItemSelected > 99)
                //{
                //    //TODO: disabled, not working yet
                //    if (t.m_time != null && m_trailResult.Activity != null && //!t.m_firstRow && 
                //        m_trailResult.Activity.GPSRoute != null)
                //    {
                //        for (int i = m_rowDoubleClickSelected - 1; i >= 0; i--)
                //        {
                //            if (((IList<EditTrailRow>)EList.RowData)[i].m_time != null)
                //            {
                //                IGPSRoute route = new GPSRoute();
                //                DateTime startTime = (DateTime)((IList<EditTrailRow>)EList.RowData)[i].m_date;
                //                DateTime endTime = (DateTime)t.m_date;
                //                ITimeValueEntry<float> startDist = m_trailResult.ActivityDistanceMetersTrack.GetInterpolatedValue(startTime);
                //                ITimeValueEntry<float> endDist = m_trailResult.ActivityDistanceMetersTrack.GetInterpolatedValue(endTime);
                //                double speed = (endDist.Value - startDist.Value) /
                //                    (m_trailResult.getElapsedResult(endTime) - m_trailResult.getElapsedResult(startTime));
                //                for (int j = 0; j < m_trailResult.Activity.GPSRoute.Count; j++)
                //                {
                //                    ITimeValueEntry<IGPSPoint> g = m_trailResult.Activity.GPSRoute[j];
                //                    DateTime date = m_trailResult.Activity.GPSRoute.EntryDateTime(g);
                //                    if (date < endTime && date > startTime)
                //                    {
                //                        ITimeValueEntry<float> dist = m_trailResult.ActivityDistanceMetersTrack.GetInterpolatedValue(date);
                //                        uint s = (uint)((dist.Value-startDist.Value)/speed)+startDist.ElapsedSeconds;
                //                        g = new TimeValueEntry<IGPSPoint>(s, g.Value);
                //                        date = m_trailResult.Activity.GPSRoute.EntryDateTime(g);
                //                        //date = startTime+TimeSpan.FromSeconds(s);
                //                    }
                //                    route.Add(date, g.Value);
                //                }
                //                m_trailResult.Activity.GPSRoute = route;
                //                break;
                //            }
                //        }
                //    }
                //}

            }
        }

        public void SMKMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.m_lastMouseArg = e;
        }

        void EList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                this.EList_DeleteRow();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.R)
            {
                RefreshResult(true);
                e.Handled = true;
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            EList_DeleteRow();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            MessageDialog.Show(Properties.Resources.UI_Activity_EditTrail_EditRow);
        }

        private void BtnUp_Click(object sender, EventArgs e)
        {
            MoveRow(1);
        }

        private void BtnDown_Click(object sender, EventArgs e)
        {
            MoveRow(-1);
        }

        private void MoveRow(int isUp)
        {
            if (this.EList.SelectedItems.Count == 1)
            {
                IList selected = this.EList.SelectedItems;
                IList<EditTrailRow> result = (IList<EditTrailRow>)this.EList.RowData;
                if (selected != null && selected.Count > 0)
                {
                    for (int j = selected.Count - 1; j >= 0; j--)
                    {
                        for (int i = result.Count - 1; i >= 0; i--)
                        {
                            EditTrailRow r = (EditTrailRow)((IList<EditTrailRow>)this.EList.RowData)[i];
                            if ((isUp < 0 && i-isUp<result.Count ||
                                isUp > 0 && i-isUp>=0) &&
                                selected[j].Equals(r))
                            {
                                result[i] = result[i-isUp];
                                result[i-isUp] = r;
                                break;
                            }
                        }
                    }
                    this.EList.RowData = result;
                    this.m_TrailToEdit.TrailLocations = EditTrailRow.getTrailGPSLocation((IList<EditTrailRow>)this.EList.RowData);
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            EList_AddRow();
        }

        void EList_SelectedItemsChanged(object sender, System.EventArgs e)
        {
            IList<EditTrailRow> result = new List<EditTrailRow>();
            if (this.EList.SelectedItems.Count == 1)
            {
                this.btnUp.Enabled = true;
                this.btnDown.Enabled = true;
            }
            else
            {
                this.btnUp.Enabled = false;
                this.btnDown.Enabled = false;
            }

            if (this.EList.SelectedItems.Count > 0)
            {
                IList selected = EList.SelectedItems;
                if (selected != null && selected.Count > 0)
                {
                    for (int j = 0; j < selected.Count; j++)
                    {
                        for (int i = ((IList<EditTrailRow>)this.EList.RowData).Count - 1; i >= 0; i--)
                        {
                            EditTrailRow r = (EditTrailRow)((IList<EditTrailRow>)this.EList.RowData)[i];
                            if (selected[j].Equals(r))
                            {
                                result.Add(r);
                            }
                        }
                    }
                }
                this.btnDelete.Enabled = true;
            }
            else
            {
                this.btnDelete.Enabled = false;
            }

            if (!this.m_updatingFromMap)
            {
                this.m_layer.SelectedTrailPoints = EditTrailRow.getTrailGPSLocation(result);
            }
        }

        private void BtnReverse_Click(object sender, EventArgs e)
        {
            IList<TrailGPSLocation> trailLocations = new List<TrailGPSLocation>();
            foreach (TrailGPSLocation t in this.m_TrailToEdit.TrailLocations)
            {
                trailLocations.Insert(0, t);
            }
            this.m_TrailToEdit.TrailLocations = trailLocations;
            RefreshResult(true);
        }

        private void ChkTwoWay_CheckedChanged(object sender, EventArgs e)
        {
            this.m_TrailToEdit.BiDirectional = !this.m_TrailToEdit.BiDirectional;
            RefreshResult(true);
        }

        private void ChkTemporaryTrail_CheckedChanged(object sender, EventArgs e)
        {
            this.m_TrailToEdit.IsTemporary = !this.m_TrailToEdit.IsTemporary;
        }

        private void ChkName_CheckedChanged(object sender, EventArgs e)
        {
            this.m_TrailToEdit.IsNameMatch = !this.m_TrailToEdit.IsNameMatch;
        }

        private void ChkCompleteActivity_CheckedChanged(object sender, EventArgs e)
        {
            this.m_TrailToEdit.IsCompleteActivity = !this.m_TrailToEdit.IsCompleteActivity;
        }

        private void ChkURFilter_CheckedChanged(object sender, EventArgs e)
        {
            this.m_TrailToEdit.IsURFilter = !this.m_TrailToEdit.IsURFilter;
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.m_TrailToEdit.TrailPriority = (int)this.numericSortPrio.Value;
        }

        //private void ChkAutoTryAll_CheckedChanged(object sender, EventArgs e)
        //{
        //    this.m_TrailToEdit.IsAutoTryAll = !this.m_TrailToEdit.IsAutoTryAll;
        //}
    }

    public class ActivityDropdownLabelProvider : TreeList.ILabelProvider
    {
        public System.Drawing.Image GetImage(object element, TreeList.Column column)
        {
            return null;
        }

        public string GetText(object element, TreeList.Column column)
        {
            if (element != null && element is string)
            {
                return element as string; //TBD
            }
            if (!(element is IActivity))
            {
                return "";
            }

            IActivity act = element as IActivity;
            string name = act.StartTime.ToLongDateString() + " " + act.Name;
            return name;
        }
    }
}
