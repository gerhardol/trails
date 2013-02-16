/*
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

namespace TrailsPlugin.UI.Activity {
	public partial class EditTrail : Form {

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
        private /*readonly*/ int cDistCol;
        private /*readonly*/ int cTimeCol;

        private MouseEventArgs m_lastMouseArg = null;
        private string m_subItemText;
        private int m_rowDoubleClickSelected = 0;
        private int m_subItemSelected = 0;
        private bool m_updatingFromMap = false;

        private EditTrail(bool addMode)
        {
            this.m_addMode = addMode;

            InitializeComponent();

            if (!Controller.TrailController.Instance.CurrentActivityTrailIsSelected)
            {
                this.m_addMode = true;
            }

            if (this.m_addMode)
            {
                this.m_TrailToEdit = new TrailsPlugin.Data.Trail(System.Guid.NewGuid());
                this.Name = Properties.Resources.UI_Activity_EditTrail_AddTrail;
            }
            else
            {
                this.m_TrailToEdit = Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail;
                this.Name = Properties.Resources.UI_Activity_EditTrail_EditTrail;
                if (this.m_TrailToEdit.Generated)
                {
                    IActivity activity = Controller.TrailController.Instance.ReferenceActivity;
                    //Create copy of the trail
                    this.m_TrailToEdit = m_TrailToEdit.Copy(false, activity);
                    this.m_addMode = true;
                    this.Name = Properties.Resources.UI_Activity_EditTrail_AddTrail;
                }
                else
                {
                    this.m_TrailToEdit = m_TrailToEdit.Copy(true);
                }
            }

            //Done after init code, as it depends on it
            this.InitControls();
        }

#if ST_2_1
        public EditTrail(ITheme visualTheme, System.Globalization.CultureInfo culture, Object view, bool addMode)
#else
        public EditTrail(ITheme visualTheme, System.Globalization.CultureInfo culture, ActivityDetailPageControl page, 
            IDailyActivityView view, TrailPointsLayer layer, bool addMode, TrailResult tr)
#endif
            : this (addMode)
        {
#if !ST_2_1
            this.m_page = page;
            this.m_view = view;
            this.m_layer = layer;
            this.m_layer.TrailPoints = m_TrailToEdit.TrailLocations;
#endif
            this.m_trailResult = tr;
            //It is possible that the trail result is not for the trail to edit (if more than one is selected)
            if (tr != null && tr.m_activityTrail.Trail != Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail)
            {
                this.m_trailResult = null;
            }

            ThemeChanged(visualTheme);
            UICultureChanged(culture);
        }

        void InitControls()
        {
            this.btnAdd.BackgroundImage = CommonIcons.Add;
            this.btnAdd.Text = "";
            this.btnEdit.BackgroundImage = CommonIcons.Edit;
            this.btnEdit.Text = "";
            this.btnDelete.BackgroundImage = CommonIcons.Delete;
            this.btnDelete.Text = "";
            this.btnDelete.Enabled = false;
            this.btnUp.BackgroundImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.MoveUp16;
            this.btnUp.Text = "";
            this.btnUp.Enabled = false;
            this.btnDown.BackgroundImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.MoveDown16;
            this.btnDown.Text = "";
            this.btnDown.Enabled = false;
            this.btnCopy.Text = "";
            this.btnCopy.BackgroundImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.DocumentCopy16;
            if (this.m_addMode)
            {
                this.btnCopy.Enabled = false;
            }
            this.btnExport.Text = "";
            this.btnExport.BackgroundImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Export16;
            this.btnRefresh.Text = "";
            this.btnRefresh.BackgroundImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Refresh16;
            this.btnReverse.Text = "";
            this.btnReverse.BackgroundImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.MoveUp16;

            this.chkTwoWay.Checked = this.m_TrailToEdit.BiDirectional;
            this.chkTemporaryTrail.Checked = this.m_TrailToEdit.IsTemporary;
            this.chkName.Checked = this.m_TrailToEdit.IsNameMatch;
            this.chkCompleteActivity.Checked = this.m_TrailToEdit.IsCompleteActivity;
            this.chkAutoTryAll.Checked = this.m_TrailToEdit.IsAutoTryAll;

            this.chkTwoWay.CheckedChanged += new System.EventHandler(this.chkTwoWay_CheckedChanged);
            this.chkTemporaryTrail.CheckedChanged += new System.EventHandler(this.chkTemporaryTrail_CheckedChanged);
            this.chkName.CheckedChanged += new System.EventHandler(this.chkName_CheckedChanged);
            this.chkCompleteActivity.CheckedChanged += new System.EventHandler(this.chkCompleteActivity_CheckedChanged);
            this.chkAutoTryAll.CheckedChanged += new System.EventHandler(this.chkAutoTryAll_CheckedChanged);
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
            this.Radius.ThemeChanged(visualTheme);
            this.editBox.ThemeChanged(visualTheme);
        }

        public void UICultureChanged(System.Globalization.CultureInfo culture)
        {
            this.lblTrail.Text = Properties.Resources.TrailName;
            this.lblRadius.Text = Properties.Resources.UI_Activity_EditTrail_Radius + ":";
            this.btnOk.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionOk;
            this.btnCancel.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionCancel;
            this.presentRadius();
            this.toolTip.SetToolTip(this.btnCopy, "Create a Copy of the Trail");
            this.toolTip.SetToolTip(this.btnExport, "Export Trail to Activity");
            this.toolTip.SetToolTip(this.btnRefresh, "Refresh Calculation");
            this.toolTip.SetToolTip(this.btnReverse, "Reverse TrailPoints");
            this.toolTip.SetToolTip(this.chkTemporaryTrail, "Temporary Trail (deleted when exiting SportTracks)");
            this.toolTip.SetToolTip(this.chkTwoWay, "Two-way Match");
            this.toolTip.SetToolTip(this.chkName, "Match by Name");
            this.toolTip.SetToolTip(this.chkCompleteActivity, "Match Complete Activity");
            this.toolTip.SetToolTip(this.chkAutoTryAll, "Calculate Trail in automatic updates");
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
                //if (recalculate)
                //{
                //    m_TrailToEdit.TrailLocations = EditTrailRow.getTrailGPSLocation((IList<EditTrailRow>)EList.RowData);
                //}
                if (recalculate || this.m_trailResult == null)
                {
                    ActivityTrail at = new ActivityTrail(Controller.TrailController.Instance, m_TrailToEdit);
                    at.CalcResults(new List<IActivity> { Controller.TrailController.Instance.ReferenceActivity }, m_TrailToEdit.MaxRequiredMisses, true, null);
                    if (TrailResultWrapper.Results(at.ResultTreeList).Count > 0)
                    {
                        this.m_trailResult = TrailResultWrapper.Results(at.ResultTreeList)[0];
                    }
                    else
                    {
                        at.Init();
                        at.CalcResults(new List<IActivity> { Controller.TrailController.Instance.ReferenceActivity }, 99, true, null);
                        if (TrailResultWrapper.Results(at.ResultTreeList).Count > 0)
                        {
                            //The best result is the result with most matches
                            //forward may be better than reverse, but those should be sorted first anyway
                            int currMaxRes = -1;
                            foreach (TrailResult tr in TrailResultWrapper.Results(at.ResultTreeList))
                            {
                                int res = 0;
                                foreach (DateTime d in tr.TrailPointDateTime)
                                {
                                    if (d > DateTime.MinValue)
                                    {
                                        res++;
                                    }
                                }
                                if (res > currMaxRes)
                                {
                                    currMaxRes = res;
                                    this.m_trailResult = tr;
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
            this.EList.RowData = l;

            foreach (EditTrailRow t in (IList<EditTrailRow>)this.EList.RowData)
            {
                //Note: For reverse results, this is incorrect (but reverse results are only for incomplete, so no impact)
                this.EList.SetChecked(t, t.TrailGPS.Required);
            }
        }
        
        ///////////////////////////
        private void btnCancel_Click(object sender, System.EventArgs e)
        {
			this.DialogResult = DialogResult.Cancel;
			Close();
		}

		private void btnOk_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(this.TrailName.Text))
            {
				MessageBox.Show(Properties.Resources.UI_Activity_EditTrail_TrailNameReqiured);
				return;
			}
            //m_TrailToEdit contains the scratchpad of trail.
            //However TrailPoints uses the row (with meta data, could be a separate cache)
            //m_TrailToEdit.TrailLocations = EditTrailRow.getTrailGPSLocation((IList<EditTrailRow>)EList.RowData);

            Data.Trail trail = Data.TrailData.GetFromName(this.TrailName.Text);
            if (trail != null && (this.m_addMode ||
                !this.m_addMode && trail.Id != this.m_TrailToEdit.Id))
            {
                MessageBox.Show(Properties.Resources.UI_Activity_EditTrail_UniqueTrailNameRequired);
                return;
            }
            if (this.m_addMode && !Controller.TrailController.Instance.AddTrail(this.m_TrailToEdit, null) ||
                !this.m_addMode && !Controller.TrailController.Instance.UpdateTrail(this.m_TrailToEdit, null))
            {
                MessageBox.Show(Properties.Resources.UI_Activity_EditTrail_UpdateFailed);
                return;
            }
            this.DialogResult = DialogResult.OK;
            //Successful: Clear marking on route (otherwise same point could be added again)
            this.m_page.ClearCurrentSelectedOnRoute();

            Close();
		}

        void btnCopy_Click(object sender, System.EventArgs e)
        {
            this.m_TrailToEdit.Name += " " + CommonResources.Text.ActionCopy;
            this.TrailName.Text = this.m_TrailToEdit.Name;
            this.m_addMode = true;
            //Disable copy, as the previous copy is not handled otherwise
            this.btnCopy.Enabled = false;
        }

        void btnExport_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show(string.Format(Properties.Resources.UI_Activity_EditTrail_Export, CommonResources.Text.ActionOk, CommonResources.Text.ActionCancel),
                "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                DateTime startTime = DateTime.Now;
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
                    activity.GPSRoute = new GPSRoute();
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

        void btnRefresh_Click(object sender, System.EventArgs e)
        {
            RefreshResult(true);
        }

        private void EditTrail_Activated(object sender, System.EventArgs e)
        {
            this.TrailName.Focus();
		}

        //protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
        //    base.OnPaint(e);
        //    Utils.Dialog.DrawButtonRowBackground(e.Graphics, ClientRectangle, m_visualTheme);
        //}

        private void EditTrail_Shown(object sender, System.EventArgs e)
        {
            this.TrailName.Text = m_TrailToEdit.Name;
            presentRadius();

            this.EList.Columns.Clear();
            this.EList.CheckBoxes = true;
            this.EList.Columns.Add(new TreeList.Column("Required", Properties.Resources.Required, 20, StringAlignment.Far));
            this.EList.Columns.Add(new TreeList.Column("LongitudeDegrees", Properties.Resources.UI_Activity_EditTrail_Longitude, 70, StringAlignment.Far));
            this.EList.Columns.Add(new TreeList.Column("LatitudeDegrees", Properties.Resources.UI_Activity_EditTrail_Latitude, 70, StringAlignment.Far));
            this.EList.Columns.Add(new TreeList.Column("Name", CommonResources.Text.LabelName, 80, StringAlignment.Near));
            this.cDistCol = EList.Columns.Count;
            this.EList.Columns.Add(new TreeList.Column("Distance", CommonResources.Text.LabelDistance, 60, StringAlignment.Far));
            this.cTimeCol = EList.Columns.Count;
            this.EList.Columns.Add(new TreeList.Column("Time", CommonResources.Text.LabelTime, 60, StringAlignment.Far));
            this.EList.Columns.Add(new TreeList.Column("Diff", "Diff", 60, StringAlignment.Far)); //TBD Translate

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
                                break;
                            }
                        }
                    }
                }
            }
            this.EList.Refresh();
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
                sel = new TrailGPSLocation(l);
            }

            sel.Name += " " + ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionNew;

            //If a point is selected on the track, use it instead
            IList<IActivity> activities = new List<IActivity> { Controller.TrailController.Instance.ReferenceActivity };
            IList<IItemTrackSelectionInfo> selectedGPS =
                        TrailsItemTrackSelectionInfo.SetAndAdjustFromSelection(m_view.RouteSelectionProvider.SelectedItems, activities, true);
            if (TrailsItemTrackSelectionInfo.ContainsData(selectedGPS))
            {
                IList<TrailGPSLocation> loc = TrailSelectorControl.getGPS(this.Trail, activities, selectedGPS);
                if (loc.Count > 0 && loc[0] != null)
                {
                    sel.GpsLoc = loc[0];
                }
            }

            if (((IList<EditTrailRow>)this.EList.RowData).Count > 0)
            {
                ((IList<EditTrailRow>)this.EList.RowData).Insert(i + 1, new EditTrailRow(sel));
            }
            else
            {
                this.EList.RowData = new List<EditTrailRow> { new EditTrailRow(sel) };
            }

            //Make ST see the update
            this.EList.RowData = ((IList<EditTrailRow>)this.EList.RowData);
            foreach (EditTrailRow t in (IList<EditTrailRow>)this.EList.RowData)
            {
                //Note: For reverse results, this is incorrect (but reverse results are only for incomplete, so no impact)
                this.EList.SetChecked(t, t.TrailGPS.Required);
            }
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

        private void presentRadius()
        {
            this.Radius.Text = UnitUtil.Elevation.ToString(this.m_TrailToEdit.Radius, "u");
        }

        private void Radius_LostFocus(object sender, System.EventArgs e)
        {
            float result;
            result = (float)UnitUtil.Elevation.Parse(this.Radius.Text);
            if (!float.IsNaN(result) && result > 0)
            {
                this.m_TrailToEdit.Radius = result;
            }
            else
            {
                MessageBox.Show(Properties.Resources.UI_Activity_EditTrail_RadiusNumeric);
            }
            this.presentRadius();
            //Refresh on map
            this.m_layer.TrailPoints = m_TrailToEdit.TrailLocations;
        }

        private void ValidateEdit()
        {
            IList<EditTrailRow> t = (IList<EditTrailRow>)this.EList.RowData;
            if (this.m_subItemSelected == cDistCol)
            {
                try
                {
                    double dist = UnitUtil.Distance.Parse(editBox.Text);
                    DateTime d1 = m_trailResult.getDateTimeFromDistActivity((float)dist);
                    t[m_rowDoubleClickSelected].UpdateRow(m_trailResult, d1);
                }
                catch { }
            }
            else if (this.m_subItemSelected == cTimeCol)
            {
                try
                {
                    double time = UnitUtil.Time.Parse(editBox.Text);
                    DateTime d1 = this.m_trailResult.getDateTimeFromTimeActivity((float)time);
                    t[this.m_rowDoubleClickSelected].UpdateRow(m_trailResult, d1);
                }
                catch { }
            }
            else
            {
                //Note: result need to be recalculated to be accurate. However, the recalc could find other results,
                //let the user do this manually
                t[this.m_rowDoubleClickSelected].UpdateRow(this.m_trailResult,
                    t[this.m_rowDoubleClickSelected].TrailGPS.setField(this.m_subItemSelected, editBox.Text));
            }
            this.EList.RowData = t;
            this.m_layer.TrailPoints = this.m_TrailToEdit.TrailLocations;
            this.m_layer.SelectedTrailPoints = new List<TrailGPSLocation> { t[this.m_rowDoubleClickSelected].TrailGPS };
            this.m_layer.Refresh();
        }

        private void editBox_KeyDown(object sender, KeyEventArgs e)
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

        private void editBox_LostFocus(object sender, System.EventArgs e)
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
                int spos = EList.Location.X;
                int epos = spos;
                for (int i = 0; i < EList.Columns.Count; i++)
                {
                    epos += EList.Columns[i].Width;
                    if (nStart >= spos && nStart < epos)
                    {
                        this.m_subItemSelected = i;
                        break;
                    }

                    spos = epos;
                }
                //Only edit first rows
                if (this.m_subItemSelected <= cTimeCol)
                {
                    this.m_subItemText = (new EditTrailLabelProvider()).GetText(t, EList.Columns[this.m_subItemSelected]);
                    ///The positioning is incorrect, set at header
                    int rowHeight = EList.HeaderRowHeight;// (EList.Height - EList.HeaderRowHeight) / ((IList<TrailGPSLocation>)EList.RowData).Count;
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            EList_DeleteRow();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Properties.Resources.UI_Activity_EditTrail_EditRow);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            moveRow(1);
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            moveRow(-1);
        }

        private void moveRow(int isUp)
        {
            if (this.EList.SelectedItems.Count == 1)
            {
                IList selected = EList.SelectedItems;
                IList<EditTrailRow> result = (IList<EditTrailRow>)this.EList.RowData;
                if (selected != null && selected.Count > 0)
                {
                    for (int j = selected.Count - 1; j >= 0; j--)
                    {
                        for (int i = result.Count - 1; i >= 0; i--)
                        {
                            EditTrailRow r = (EditTrailRow)((IList<EditTrailRow>)EList.RowData)[i];
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

        private void btnAdd_Click(object sender, EventArgs e)
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

        private void btnReverse_Click(object sender, EventArgs e)
        {
            IList<TrailGPSLocation> trailLocations = new List<TrailGPSLocation>();
            foreach (TrailGPSLocation t in this.m_TrailToEdit.TrailLocations)
            {
                trailLocations.Insert(0, t);
            }
            this.m_TrailToEdit.TrailLocations = trailLocations;
            RefreshResult(true);
        }

        private void chkTwoWay_CheckedChanged(object sender, EventArgs e)
        {
            this.m_TrailToEdit.BiDirectional = !this.m_TrailToEdit.BiDirectional;
            RefreshResult(true);
        }

        private void chkTemporaryTrail_CheckedChanged(object sender, EventArgs e)
        {
            this.m_TrailToEdit.IsTemporary = !this.m_TrailToEdit.IsTemporary;
        }

        private void chkName_CheckedChanged(object sender, EventArgs e)
        {
            this.m_TrailToEdit.IsNameMatch = !this.m_TrailToEdit.IsNameMatch;
        }

        private void chkCompleteActivity_CheckedChanged(object sender, EventArgs e)
        {
            this.m_TrailToEdit.IsCompleteActivity = !this.m_TrailToEdit.IsCompleteActivity;
        }

        private void chkAutoTryAll_CheckedChanged(object sender, EventArgs e)
        {
            this.m_TrailToEdit.IsAutoTryAll = !this.m_TrailToEdit.IsAutoTryAll;
        }
    }
}
