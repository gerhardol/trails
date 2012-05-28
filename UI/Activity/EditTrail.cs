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
            m_addMode = addMode;

            InitializeComponent();

            if (Controller.TrailController.Instance.CurrentActivityTrail == null)
            {
                m_addMode = true;
            }
            if (m_addMode)
            {
                m_TrailToEdit = new TrailsPlugin.Data.Trail(System.Guid.NewGuid());
                this.Name = Properties.Resources.UI_Activity_EditTrail_AddTrail;
            }
            else
            {
                m_TrailToEdit = Controller.TrailController.Instance.CurrentActivityTrail.Trail;
                this.Name = Properties.Resources.UI_Activity_EditTrail_EditTrail;
                if (m_TrailToEdit.Generated)
                {
                    IActivity activity = Controller.TrailController.Instance.ReferenceActivity;
                    //Create copy of the trail
                    m_TrailToEdit = m_TrailToEdit.Copy(false, activity);
                    m_addMode = true;
                    this.Name = Properties.Resources.UI_Activity_EditTrail_AddTrail;
                }
                else
                {
                    m_TrailToEdit = m_TrailToEdit.Copy(true);
                }
            }
            //Done after init code, as it depends on it
            InitControls();
        }
#if ST_2_1
        public EditTrail(ITheme visualTheme, System.Globalization.CultureInfo culture, Object view, bool addMode)
#else
        public EditTrail(ITheme visualTheme, System.Globalization.CultureInfo culture, IDailyActivityView view, 
            TrailPointsLayer layer, bool addMode, TrailResult tr)
#endif
            : this (addMode)
        {
#if !ST_2_1
            m_view = view;
            m_layer = layer;
            m_layer.TrailPoints = m_TrailToEdit.TrailLocations;
#endif
            ThemeChanged(visualTheme);
            UICultureChanged(culture);
            m_trailResult = tr;
        }

        void InitControls()
        {
            btnAdd.BackgroundImage = CommonIcons.Add;
            btnAdd.Text = "";
            btnEdit.BackgroundImage = CommonIcons.Edit;
            btnEdit.Text = "";
            btnDelete.BackgroundImage = CommonIcons.Delete;
            btnDelete.Text = "";
            btnDelete.Enabled = false;
            btnZUp.BackgroundImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.ZoomOut16;
            btnZUp.Text = "";
            //btnZUp.Enabled = false;
            btnZUp.Visible = false;
            btnZDown.BackgroundImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.ZoomIn16;
            btnZDown.Text = "";
            //btnZDown.Enabled = false;
            btnZDown.Visible = false;
            btnUp.BackgroundImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.MoveUp16;
            btnUp.Text = "";
            btnUp.Enabled = false;
            btnDown.BackgroundImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.MoveDown16;
            btnDown.Text = "";
            btnDown.Enabled = false;
            btnCopy.Text = "";
            btnCopy.BackgroundImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.DocumentCopy16;
            if (m_addMode)
            {
                this.btnCopy.Enabled = false;
            }
            btnExport.Text = "";
            btnExport.BackgroundImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Export16;
            btnRefresh.Text = "";
            btnRefresh.BackgroundImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Refresh16;
#if ST_2_1
            this.EList.SelectedChanged += new System.EventHandler(EList_SelectedItemsChanged);
#else
            this.EList.SelectedItemsChanged += new System.EventHandler(EList_SelectedItemsChanged);
#endif
            this.EList.LabelProvider = new EditTrailLabelProvider();
        }
        
        public virtual void ThemeChanged(ITheme visualTheme)
        {
			m_visualTheme = visualTheme;
			this.BackColor = visualTheme.Control;
			EList.ThemeChanged(visualTheme);
            TrailName.ThemeChanged(visualTheme);
            Radius.ThemeChanged(visualTheme);
            editBox.ThemeChanged(visualTheme);
        }

        public void UICultureChanged(System.Globalization.CultureInfo culture)
        {
            this.lblTrail.Text = Properties.Resources.TrailName;
            lblRadius.Text = Properties.Resources.UI_Activity_EditTrail_Radius + ":";
            this.btnOk.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionOk;
            this.btnCancel.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionCancel;
            presentRadius();
        }

        public void UpdatePointFromMap(TrailGPSLocation point)
        {
            IList<EditTrailRow> list = (IList<EditTrailRow>)EList.RowData;
            for (int i = 0; i < list.Count; i++)
            {
                if (point == list[i].TrailGPS)
                {
                    m_updatingFromMap = true;
                    this.EList.SelectedItems = new object[] { list[i] };
                    m_updatingFromMap = false;
                    EList.RefreshElements(this.EList.SelectedItems);
                    break;
                }
            }
        }

        public Data.Trail Trail
        {
            get
            {
                return m_TrailToEdit;
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
                if (recalculate || m_trailResult == null)
                {
                    ActivityTrail at = new ActivityTrail(Controller.TrailController.Instance, m_TrailToEdit);
                    at.CalcResults(new List<IActivity> { Controller.TrailController.Instance.ReferenceActivity }, m_TrailToEdit.MaxRequiredMisses, true, null);
                    if (at.ParentResults.Count > 0)
                    {
                        m_trailResult = at.ParentResults[0];
                    }
                    else
                    {
                        at.Reset();
                        at.CalcResults(new List<IActivity> { Controller.TrailController.Instance.ReferenceActivity }, 99, true, null);
                        if (at.ParentResults.Count > 0)
                        {
                            //The best result is the result with most matches
                            //forward may be better than reverse, but those should be sorted first anyway
                            int currMaxRes = -1;
                            foreach (TrailResult tr in at.ParentResults)
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
                                    m_trailResult = tr;
                                }
                            }
                        }
                        else
                        {
                            if (at.IncompleteResults.Count > 0)
                            {
                                //Result is already sorted after no of matches
                                m_trailResult = at.IncompleteResults[0];
                            }
                            else
                            {
                                m_trailResult = null;
                            }
                        }
                    }
                    at.Reset();
                }
            }
            EList.RowData = EditTrailRow.getEditTrailRows(m_TrailToEdit.TrailLocations, m_trailResult);
            foreach (EditTrailRow t in (IList<EditTrailRow>)EList.RowData)
            {
                //Note: For reverse results, this is incorrect (but reverse results are only for incomplete, so no impact)
                EList.SetChecked(t, t.TrailGPS.Required);
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
			if (TrailName.Text.Length == 0) {
				MessageBox.Show(Properties.Resources.UI_Activity_EditTrail_TrailNameReqiured);
				return;
			}
            //m_TrailToEdit contains the scratchpad of trail.
            //However TrailPoints uses the row (with meta data, could be a separate cache)
            //m_TrailToEdit.TrailLocations = EditTrailRow.getTrailGPSLocation((IList<EditTrailRow>)EList.RowData);

            Data.Trail trail = Data.TrailData.GetFromName(TrailName.Text);
            if (m_addMode && trail != null ||
                !m_addMode && trail.Id != m_TrailToEdit.Id)
            {
                MessageBox.Show(Properties.Resources.UI_Activity_EditTrail_UniqueTrailNameRequired);
                return;
            }
            if (m_addMode && !Controller.TrailController.Instance.AddTrail(m_TrailToEdit) ||
                !m_addMode && !Controller.TrailController.Instance.UpdateTrail(m_TrailToEdit))
            {
                MessageBox.Show(Properties.Resources.UI_Activity_EditTrail_UpdateFailed);
                return;
            }
            this.DialogResult = DialogResult.OK;
			Close();
		}

        void btnCopy_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show(string.Format(Properties.Resources.UI_Activity_EditTrail_Copy, CommonResources.Text.ActionOk, CommonResources.Text.ActionCancel),
                "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                m_addMode = true;
                this.btnCopy.Enabled = false;
            }
        }

        void btnExport_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show(string.Format(Properties.Resources.UI_Activity_EditTrail_Export, CommonResources.Text.ActionOk, CommonResources.Text.ActionCancel),
                "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                DateTime startTime = DateTime.Now;
                IActivity activity;
                bool hasGps = m_trailResult != null && m_trailResult.GPSRoute != null;
                if (hasGps)
                {
                    activity = m_trailResult.CopyToActivity();
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
                IList<EditTrailRow> list = (IList<EditTrailRow>)EList.RowData;
                for (int i = 0; i < list.Count - 1; i++)
                {
                    if (hasGps)
                    {
                        activity.Laps.Add(m_trailResult.TrailPointDateTime[i],
                           TimeSpan.FromSeconds(m_trailResult.TrailPointTime0(m_trailResult)[i + 1] -
                           m_trailResult.TrailPointTime0(m_trailResult)[i]));
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
			TrailName.Focus();
		}

        //protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
        //    base.OnPaint(e);
        //    Utils.Dialog.DrawButtonRowBackground(e.Graphics, ClientRectangle, m_visualTheme);
        //}

        private void EditTrail_Shown(object sender, System.EventArgs e)
        {
            TrailName.Text = m_TrailToEdit.Name;
            presentRadius();

            EList.Columns.Clear();
            EList.CheckBoxes = true;
            EList.Columns.Add(new TreeList.Column("Required", Properties.Resources.Required, 20, StringAlignment.Far));
            EList.Columns.Add(new TreeList.Column("LongitudeDegrees", Properties.Resources.UI_Activity_EditTrail_Longitude, 70, StringAlignment.Far));
            EList.Columns.Add(new TreeList.Column("LatitudeDegrees", Properties.Resources.UI_Activity_EditTrail_Latitude, 70, StringAlignment.Far));
            EList.Columns.Add(new TreeList.Column("Name", CommonResources.Text.LabelName, 80, StringAlignment.Near));
            cDistCol = EList.Columns.Count;
            EList.Columns.Add(new TreeList.Column("Distance", CommonResources.Text.LabelDistance, 60, StringAlignment.Far));
            cTimeCol = EList.Columns.Count;
            EList.Columns.Add(new TreeList.Column("Time", CommonResources.Text.LabelTime, 60, StringAlignment.Far));

            RefreshResult(false);

            EList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SMKMouseDown);
            EList.DoubleClick += new System.EventHandler(this.SMKDoubleClick);
            EList.KeyDown += new KeyEventHandler(EList_KeyDown);
            EList.CheckedChanged += new TreeList.ItemEventHandler(EList_CheckedChanged);
        }

        private int EList_SelectedRow()
        {
            int result = -1;
            if (EList.SelectedItems.Count > 0 && ((IList<EditTrailRow>)EList.RowData).Count>0)
            {
                IList selected = EList.SelectedItems;
                if (selected != null && selected.Count > 0)
                {
                    for (int j = selected.Count - 1; j >= 0; j--)
                    {
                        IList<EditTrailRow> list = ((IList<EditTrailRow>)EList.RowData);
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
            if (EList.SelectedItems.Count > 0 && ((IList<EditTrailRow>)EList.RowData).Count > 0)
            {
                IList selected = EList.SelectedItems;
                if (selected != null && selected.Count > 0)
                {
                    for (int j = selected.Count - 1; j >= 0; j--)
                    {
                        IList<EditTrailRow> list = ((IList<EditTrailRow>)EList.RowData);
                        for (int i = 0; i < list.Count; i++)
                        {
                            //Only first selected
                            if (selected[j].Equals(list[i]))
                            {
                                ((IList<EditTrailRow>)EList.RowData).RemoveAt(i);
                                //Required to make ST see the update, Refresh() is not enough?
                                EList.RowData = ((IList<EditTrailRow>)EList.RowData);
                                m_TrailToEdit.TrailLocations = EditTrailRow.getTrailGPSLocation((IList<EditTrailRow>)EList.RowData);
                                m_layer.TrailPoints = m_TrailToEdit.TrailLocations;
                                break;
                            }
                        }
                    }
                }
            }
            EList.Refresh();
        }

        private void EList_AddRow()
        {
            int i = EList_SelectedRow();
            if (i < 0)
            {
                if (((IList<EditTrailRow>)EList.RowData).Count > 0)
                {
                    i = ((IList<EditTrailRow>)EList.RowData).Count-1;
                }
                else
                {
                    i = 0;
                }
            }
            TrailGPSLocation sel;
            if (((IList<EditTrailRow>)EList.RowData).Count >0)
            {
                sel = ((IList<EditTrailRow>)EList.RowData)[i].TrailGPS;
            }
            else
            {
                sel = new TrailGPSLocation(0,0,"");
            }
            TrailGPSLocation add = sel.Copy();
                add.Name +=
                " " + ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionNew;
            add.GpsLocation = m_layer.GetCenterMap();
            IList<IActivity> activities = new List<IActivity> { Controller.TrailController.Instance.ReferenceActivity };
            IList<IItemTrackSelectionInfo> selectedGPS = (IList<IItemTrackSelectionInfo>)
                        TrailsItemTrackSelectionInfo.SetAndAdjustFromSelection(m_view.RouteSelectionProvider.SelectedItems, activities, true);
            if (TrailsItemTrackSelectionInfo.ContainsData(selectedGPS))
            {
                IList<TrailGPSLocation> loc = TrailSelectorControl.getGPS(this.Trail, activities, selectedGPS);
                if (loc.Count > 0)
                {
                    add.GpsLocation = loc[0].GpsLocation;
                }
            }
            if (((IList<EditTrailRow>)EList.RowData).Count > 0)
            {
                ((IList<EditTrailRow>)EList.RowData).Insert(i + 1, new EditTrailRow(add));
            }
            else
            {
                EList.RowData = new List<EditTrailRow> { new EditTrailRow(add) };
            }
            //Make ST see the update
            EList.RowData = ((IList<EditTrailRow>)EList.RowData);
            EList.Refresh();
            m_TrailToEdit.TrailLocations = EditTrailRow.getTrailGPSLocation((IList<EditTrailRow>)EList.RowData);
            m_layer.TrailPoints = m_TrailToEdit.TrailLocations;
            m_layer.SelectedTrailPoints = new List<TrailGPSLocation> { add };
            m_layer.Refresh();
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
            m_TrailToEdit.Name = TrailName.Text;
        }

        private void presentRadius()
        {
            Radius.Text = UnitUtil.Elevation.ToString(m_TrailToEdit.Radius, "u");
        }
        private void Radius_LostFocus(object sender, System.EventArgs e)
        {
            float result;
            result = (float)UnitUtil.Elevation.Parse(Radius.Text);
            if (!float.IsNaN(result) && result > 0)
            {
                m_TrailToEdit.Radius = result;
            }
            else
            {
                MessageBox.Show(Properties.Resources.UI_Activity_EditTrail_RadiusNumeric);
            }
            presentRadius();
            //Refresh on map
            m_layer.TrailPoints = m_TrailToEdit.TrailLocations;
        }

        private void ValidateEdit()
        {
            IList<EditTrailRow> t = (IList<EditTrailRow>)EList.RowData;
            if (m_subItemSelected == cDistCol)
            {
                try
                {
                    double dist = UnitUtil.Distance.Parse(editBox.Text);
                    DateTime d1 = m_trailResult.getDateTimeFromDistActivity((float)dist);
                    t[m_rowDoubleClickSelected].UpdateRow(m_trailResult, d1);
                }
                catch { }
            }
            else if (m_subItemSelected == cTimeCol)
            {
                try
                {
                    double time = UnitUtil.Time.Parse(editBox.Text);
                    DateTime d1 = m_trailResult.getDateTimeFromElapsedActivity((float)time);
                    t[m_rowDoubleClickSelected].UpdateRow(m_trailResult, d1);
                }
                catch { }
            }
            else
            {
                //Note: result need to be recalculated to be accurate. However, the recalc could find other results,
                //let the user do this manually
                t[m_rowDoubleClickSelected].UpdateRow(m_trailResult,
                    t[m_rowDoubleClickSelected].TrailGPS.setField(m_subItemSelected, editBox.Text));
            }
            EList.RowData = t;
            m_layer.TrailPoints = m_TrailToEdit.TrailLocations;
            m_layer.SelectedTrailPoints = new List<TrailGPSLocation>{t[m_rowDoubleClickSelected].TrailGPS};
            m_layer.Refresh();
        }

        private void editBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Return)
            {
                ValidateEdit();
                editBox.Visible = false;
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Escape)
            {
                editBox.Visible = false;
                e.Handled = true;
            }
        }

        private void editBox_LostFocus(object sender, System.EventArgs e)
        {
            ValidateEdit();
            editBox.Visible = false;
        }

        public void SMKDoubleClick(object sender, System.EventArgs e)
        {
            TreeList.RowHitState hitState = TreeList.RowHitState.Nothing;
            EditTrailRow t = (EditTrailRow)EList.RowHitTest(new Point(m_lastMouseArg.X, m_lastMouseArg.Y), out hitState);

            if (t != null && hitState != TreeList.RowHitState.Nothing)
            {
                for (int i = 0; i < ((IList<EditTrailRow>)EList.RowData).Count; i++)
                {
                    EditTrailRow r = (EditTrailRow)((IList<EditTrailRow>)EList.RowData)[i];
                    if (t.Equals(r))
                    {
                        m_rowDoubleClickSelected = i;
                        break;
                    }
                }
                // Check the subitem clicked
                //TODO: Not handling resize (scrolling) correctly
                int nStart = m_lastMouseArg.Location.X;// EList.PointToScreen(m_lastMouseArg.Location).X;
                int spos = EList.Location.X;
                int epos = spos;
                for (int i = 0; i < EList.Columns.Count; i++)
                {
                    epos += EList.Columns[i].Width;
                    if (nStart >= spos && nStart < epos)
                    {
                        m_subItemSelected = i;
                        break;
                    }

                    spos = epos;
                }
                //Only edit first rows
                if (m_subItemSelected <= cTimeCol)
                {
                    m_subItemText = (new EditTrailLabelProvider()).GetText(t, EList.Columns[m_subItemSelected]);
                    ///The positioning is incorrect, set at header
                    int rowHeight = EList.HeaderRowHeight;// (EList.Height - EList.HeaderRowHeight) / ((IList<TrailGPSLocation>)EList.RowData).Count;
                    int yTop = 0;// EList.HeaderRowHeight + rowSelected * rowHeight;
                    editBox.Size = new System.Drawing.Size(epos - spos, rowHeight);
                    editBox.Location = new System.Drawing.Point(spos - 1, yTop);
                    editBox.Text = m_subItemText;
                    editBox.Visible = true;
                    editBox.SelectAll();
                    editBox.Focus();
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
             m_lastMouseArg = e;
        }

        void EList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                EList_DeleteRow();
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

        private void btnZUp_Click(object sender, EventArgs e)
        {
            m_layer.ZoomOut();
        }

        private void btnZDown_Click(object sender, EventArgs e)
        {
            m_layer.ZoomIn();
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
            if (EList.SelectedItems.Count == 1)
            {
                IList selected = EList.SelectedItems;
                IList<EditTrailRow> result = (IList<EditTrailRow>)EList.RowData;
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
                    EList.RowData = result;
                    m_TrailToEdit.TrailLocations = EditTrailRow.getTrailGPSLocation((IList<EditTrailRow>)EList.RowData);
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
            if (EList.SelectedItems.Count == 1)
            {
                btnUp.Enabled = true;
                btnDown.Enabled = true;
            }
            else
            {
                btnUp.Enabled = false;
                btnDown.Enabled = false;
            }
            if (EList.SelectedItems.Count > 0)
            {
                IList selected = EList.SelectedItems;
                if (selected != null && selected.Count > 0)
                {
                    for (int j = 0; j < selected.Count; j++)
                    {
                        for (int i = ((IList<EditTrailRow>)EList.RowData).Count - 1; i >= 0; i--)
                        {
                            EditTrailRow r = (EditTrailRow)((IList<EditTrailRow>)EList.RowData)[i];
                            if (selected[j].Equals(r))
                            {
                                result.Add(r);
                            }
                        }
                    }
                }
                btnDelete.Enabled = true;
            }
            else
            {
                btnDelete.Enabled = false;
            }
            if (!this.m_updatingFromMap)
            {
                m_layer.SelectedTrailPoints = EditTrailRow.getTrailGPSLocation(result);
            }
        }
    }
}
