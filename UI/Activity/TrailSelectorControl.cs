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

            if (null != m_controller.CurrentActivityTrailDisplayed)
            {
                TrailName.Text = m_controller.CurrentActivityTrailDisplayed.Trail.Name;
                TrailName.Enabled = (m_editTrail == null);
            }
            else
            {
                TrailName.Text = Properties.Resources.Trail_NoTrailSelected;
            }
            enabled = enabled && ((m_controller.CurrentActivityTrailDisplayed == null) || !m_controller.CurrentActivityTrailDisplayed.Trail.Generated);
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
            else if ((m_editTrail == null) && this.m_controller.CurrentActivityTrailDisplayed != null &&
               this.m_controller.CurrentActivityTrailDisplayed.Trail.Generated)
            {
                //Just for conveience (the popup text next contradicts this currently)
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
                EditTrail dialog = new EditTrail(m_visualTheme, m_culture, m_view, m_layer, true, m_controller.ReferenceTrailResult);
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
            IList<IItemTrackSelectionInfo> selectedGPS = (IList<IItemTrackSelectionInfo>)
                        TrailsItemTrackSelectionInfo.SetAndAdjustFromSelection(m_view.RouteSelectionProvider.SelectedItems, m_page.ViewActivities, true);
#endif

            if (TrailsItemTrackSelectionInfo.ContainsData(selectedGPS) && 
                !m_controller.CurrentActivityTrail.Trail.Generated &&
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
                EditTrail dialog = new EditTrail(m_visualTheme, m_culture, m_view, m_layer, false, m_controller.ReferenceTrailResult);
                showEditDialog(dialog);
			}
		}
        private void showEditDialog(EditTrail dialog)
        {
            m_editTrail = dialog;
            m_layer.editTrail = dialog;
            m_page.RefreshControlState(); 
            
            dialog.TopMost = true;
            dialog.FormClosed += new FormClosedEventHandler(popupForm_FormClosed);
            dialog.Show();
        }

        void popupForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_editTrail = null;
            m_layer.editTrail = null;

            m_page.RefreshControlState();
            m_page.RefreshData();
        }

		private void btnDelete_Click(object sender, EventArgs e)
        {
			if (MessageBox.Show(Properties.Resources.UI_Activity_Page_DeleteTrailConfirm, m_controller.CurrentActivityTrail.Trail.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) 
                == DialogResult.Yes)
            {
				m_controller.DeleteCurrentTrail();
				m_page.RefreshControlState();
				m_page.RefreshData();
			}
		}

        /*************************************************************************************************************/
//ST3
        //TODO: Rewrite, using IItemTrackSelectionInfo help functions
        static IList<TrailGPSLocation> getGPS(Trail trail, IList<IActivity> activities, IValueRange<DateTime> ts, IValueRange<double> di, string id)
        {
            IList<TrailGPSLocation> result = new List<TrailGPSLocation>();
            DateTime d = DateTime.MaxValue;

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
                    d = ts.Lower;
                    IDistanceDataTrack dtrack = activity.GPSRoute.GetDistanceMetersTrack();
                    double s = dtrack.GetInterpolatedValue(ts.Upper).Value - dtrack.GetInterpolatedValue(ts.Lower).Value;
                    //TODO: pass radius for trail
                    if (s > 2 * TrailsPlugin.Data.Settings.DefaultRadius)
                    {
                        //TODO: Common handling, avoid duplication
                        if (d != DateTime.MaxValue)
                        {
                            ITimeValueEntry<IGPSPoint> p2 = activity.GPSRoute.GetInterpolatedValue(d);
                            if (null != p2)
                            {
                                result.Add(new TrailGPSLocation(d, p2, "", true));
                            }
                        }
                        d = ts.Upper;
                    }
                }
                //MarkedTimes set when importing
                //else
                //{
                //    //Normally, ST is selecting by distance, this is the common path
                //    if (null != di &&
                //        null != m_controller.ReferenceTrailResult &&
                //        m_controller.ReferenceTrailResult.Activity == activity)
                //    {
                //        d = m_controller.ReferenceTrailResult.getDateTimeFromDistActivity(di.Lower);
                //    }
                //}
                if (d != DateTime.MaxValue)
                {
                    ITimeValueEntry<IGPSPoint> p = activity.GPSRoute.GetInterpolatedValue(d);
                    if (null != p)
                    {
                        result.Add(new TrailGPSLocation(d, p, "", true));
                    }
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

                //Marked
                IValueRangeSeries<DateTime> tm = selectGPS.MarkedTimes;
                if (null != tm)
                {
                    foreach (IValueRange<DateTime> ts in tm)
                    {
                        result2 = Trail.MergeTrailLocations(result2, getGPS(trail, activities, ts, null, aSelectGPS[i].ItemReferenceId));
                    }
                }
                //Should not be needed, MarkedTimes are set at import
                //if (result2.Count == 0)
                //{
                //    IValueRangeSeries<double> td = selectGPS.MarkedDistances;
                //    if (null != td)
                //    {

                //        foreach (IValueRange<double> td1 in td)
                //        {
                //            result2 = Trail.MergeTrailLocations(result2, getGPS(null, td1, aSelectGPS[i].ItemReferenceId));
                //        }
                //    }
                //    if (result2.Count == 0)
                //    {
                //        //Selected
                //        result2 = getGPS(selectGPS.SelectedTime, selectGPS.SelectedDistance, aSelectGPS[i].ItemReferenceId);
                //    }
                //}
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
            if (m_controller.CurrentActivityTrailDisplayed != null && !m_controller.CurrentActivityTrailDisplayed.Trail.Generated)
            {
                if (MessageBox.Show(string.Format(Properties.Resources.UI_Activity_Page_AddTrail_Replace, CommonResources.Text.ActionYes,CommonResources.Text.ActionNo),
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    addCurrent = true;
                }
            }
            EditTrail dialog = new EditTrail(m_visualTheme, m_culture, m_view, m_layer, !addCurrent, m_controller.ReferenceTrailResult);
            if (m_controller.CurrentActivityTrailDisplayed != null)
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
            EditTrail dialog = new EditTrail(m_visualTheme, m_culture, m_view, m_layer, false, m_controller.ReferenceTrailResult);
            bool selectionIsDifferent = selectedGPS.Count != dialog.Trail.TrailLocations.Count;
            if (!selectionIsDifferent)
            {
                IList<TrailGPSLocation> loc = getGPS(dialog.Trail, m_page.ViewActivities, selectedGPS);
                if (loc.Count == selectedGPS.Count)
                {
                    for (int i = 0; i < loc.Count; i++)
                    {
                        TrailGPSLocation loc1 = loc[i];
                        IGPSLocation loc2 = dialog.Trail.TrailLocations[i].GpsLocation;
                        if (loc1.LatitudeDegrees != loc2.LatitudeDegrees
                                || loc1.LongitudeDegrees != loc2.LongitudeDegrees)
                        {
                            selectionIsDifferent = true;
                            break;
                        }
                    }
                }
            }
 
            if (selectionIsDifferent)
            {
                if (MessageBox.Show(Properties.Resources.UI_Activity_Page_UpdateTrail, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dialog.Trail.TrailLocations = getGPS(dialog.Trail, m_page.ViewActivities, selectedGPS);
                }
            }

            showEditDialog(dialog);
        }


        private void TrailName_ButtonClick(object sender, EventArgs e)
        {
            TreeListPopup treeListPopup = new TreeListPopup();
            treeListPopup.ThemeChanged(m_visualTheme);
            treeListPopup.Tree.Columns.Add(new TreeList.Column());

            System.Windows.Forms.ProgressBar progressBar = m_page.StartProgressBar(Data.TrailData.AllTrails.Values.Count * m_controller.Activities.Count);
            treeListPopup.Tree.RowData = m_controller.GetOrderedTrails(progressBar, false);
            m_page.StopProgressBar();
            //Note: Just checking for current trail could modify the ordered list, so do this first
            System.Collections.IList currSel = null;
            if (m_controller.CurrentActivityTrailDisplayed != null)
            {
                currSel = new object[] { m_controller.CurrentActivityTrailDisplayed };
            }
#if ST_2_1
            treeListPopup.Tree.Selected = currSel;
#else
            treeListPopup.Tree.SelectedItems = currSel;
#endif
            treeListPopup.Tree.LabelProvider = new TrailDropdownLabelProvider();

            treeListPopup.ItemSelected += new TreeListPopup.ItemSelectedEventHandler(TrailName_ItemSelected);
            treeListPopup.Popup(this.TrailName.Parent.RectangleToScreen(this.TrailName.Bounds));
        }

        /*******************************************************/

		private void TrailName_ItemSelected(object sender, EventArgs e)
        {
            ActivityTrail t = ((ActivityTrail)((TreeListPopup.ItemSelectedEventArgs)e).Item);
            if(sender is TreeListPopup)
            {
                ((TreeListPopup)sender).Hide();
            }
            System.Windows.Forms.ProgressBar progressBar = m_page.StartProgressBar(Data.TrailData.AllTrails.Values.Count * m_controller.Activities.Count);
            m_controller.SetCurrentActivityTrail(t, progressBar);
            m_page.StopProgressBar();
            m_page.RefreshData();
            m_page.RefreshControlState();

           GPSBounds area = TrailGPSLocation.getGPSBounds(t.Trail.TrailLocations, 3*t.Trail.Radius);
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
