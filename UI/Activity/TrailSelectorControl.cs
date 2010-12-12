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
using TrailsPlugin.Controller;

namespace TrailsPlugin.UI.Activity {
	public partial class TrailSelectorControl : UserControl {

        private ITheme m_visualTheme;
        private CultureInfo m_culture;
        private TrailController m_controller;
#if ST_2_1
        private UI.MapLayers.MapControlLayer m_layer;
#else
        private IDailyActivityView m_view = null;
        private TrailPointsLayer m_layer = null;
#endif

        ActivityDetailPageControl m_page;

        public TrailSelectorControl()
        {
            InitializeComponent();
        }
        public void SetControl(ActivityDetailPageControl page, Controller.TrailController controller,
#if ST_2_1
          UI.MapLayers.MapControlLayer layer)
        {
#else
          IDailyActivityView view, TrailPointsLayer layer)
        {
            m_view = view;
#endif
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
        private bool _showPage = false;
        public bool ShowPage
        {
            get { return _showPage; }
            set
            {
                _showPage = value;
            }
        }

		public void RefreshControlState() 
        {
            bool enabled = (m_controller.ReferenceActivity != null);
			btnAdd.Enabled = enabled;
			TrailName.Enabled = enabled;

			enabled = (m_controller.CurrentActivityTrailDisplayed != null);
            btnEdit.Enabled = enabled;
			btnDelete.Enabled = enabled;

            if (enabled)
            {
                TrailName.Text = m_controller.CurrentActivityTrailDisplayed.Trail.Name;
            }
            else
            {
                TrailName.Text = Properties.Resources.Trail_NoTrailSelected;
            }
        }


        /************************************************************/
		private void btnAdd_Click(object sender, EventArgs e) {

            int countGPS = 0;
#if ST_2_1
			IMapControl mapControl = m_layer.MapControl;
			ICollection<IMapControlObject> selectedGPS = null;
            if (null != mapControl) { selectedGPS = mapControl.Selected; }
#else
            IList<IItemTrackSelectionInfo> selectedGPS = m_view.RouteSelectionProvider.SelectedItems;
#endif
            countGPS = selectedGPS.Count;
            if (countGPS > 0)
            {
#if ST_2_1
                m_layer.SelectedGPSLocationsChanged += new System.EventHandler(layer_SelectedGPSLocationsChanged_AddTrail);
				m_layer.CaptureSelectedGPSLocations();
#else
                selectedGPSLocationsChanged_AddTrail(selectedGPS);
#endif
            } else {
#if ST_2_1
                string message = String.Format(Properties.Resources.UI_Activity_Page_SelectPointsError_ST2, 
#else
                string message = String.Format(Properties.Resources.UI_Activity_Page_SelectPointsError,
#endif
                  Properties.Resources.Trail_Reference_Name);
                MessageBox.Show(message, "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
 		}
        private TrailsItemTrackSelectionInfo getSel(DateTime t)
        {
            IValueRange<DateTime> v = new ValueRange<DateTime>(t, t);
            TrailsItemTrackSelectionInfo s = new TrailsItemTrackSelectionInfo();
            s.SelectedTime = v;
            return s;
        }

        private void btnEdit_Click(object sender, EventArgs e) {
            int countGPS = 0;
#if ST_2_1
			IMapControl mapControl = m_layer.MapControl;
            ICollection<IMapControlObject> selectedGPS = null;
            if (null != mapControl) { selectedGPS = mapControl.Selected; }
#else
            IList<IItemTrackSelectionInfo> selectedGPS = m_view.RouteSelectionProvider.SelectedItems;
#endif
            countGPS = selectedGPS.Count;
            if (countGPS > 0)
            {
#if ST_2_1
				m_layer.SelectedGPSLocationsChanged += new System.EventHandler(layer_SelectedGPSLocationsChanged_EditTrail);
				m_layer.CaptureSelectedGPSLocations();
#else
                selectedGPSLocationsChanged_EditTrail(selectedGPS);
#endif
            } else {
#if ST_2_1
				EditTrail dialog = new EditTrail(m_visualTheme, m_culture, false);
#else
                EditTrail dialog = new EditTrail(m_visualTheme, m_culture, m_view, false);
#endif
                if (dialog.ShowDialog() == DialogResult.OK) {
					m_page.RefreshControlState();
					m_page.RefreshData();
				}
			}
		}

		private void btnDelete_Click(object sender, EventArgs e) {
			if (MessageBox.Show(Properties.Resources.UI_Activity_Page_DeleteTrailConfirm, m_controller.CurrentActivityTrail.Trail.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) 
                == DialogResult.Yes) {
				m_controller.DeleteCurrentTrail();
				m_page.RefreshControlState();
				m_page.RefreshData();
			}
		}

        /*************************************************************************************************************/
//ST3
        //TODO: Rewrite, using IItemTrackSelectionInfo help functions
        IList<TrailGPSLocation> getGPS(IValueRange<DateTime> ts, IValueRange<double> di)
        {
            IList<TrailGPSLocation> result = new List<TrailGPSLocation>();
            ITimeValueEntry<IGPSPoint> p = null;
            if (null != ts)
            {
                p = m_controller.SingleActivity.GPSRoute.GetInterpolatedValue(ts.Lower);
            }
            else
            {
                //Normally, selecting by time is null, fall back to select by distance
                if (null != di && null != m_controller.SingleActivity && null != m_controller.SingleActivity.GPSRoute)
                {
                    IDistanceDataTrack dt = m_controller.SingleActivity.GPSRoute.GetDistanceMetersTrack();
                    p = m_controller.SingleActivity.GPSRoute.GetInterpolatedValue(dt.GetTimeAtDistanceMeters(di.Lower));
                }
            }
            if (null != p)
            {
                result.Add(new TrailGPSLocation(p.Value.LatitudeDegrees, p.Value.LongitudeDegrees, ""));
            }
            return result;
        }
        IList<TrailGPSLocation> getGPS(IList<IItemTrackSelectionInfo> aSelectGPS)
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
                        result2 = Trail.MergeTrailLocations(result2, getGPS(ts, null));
                    }
                }
                if (result2.Count == 0)
                {
                    IValueRangeSeries<double> td = selectGPS.MarkedDistances;
                    if (null != td)
                    {

                        foreach (IValueRange<double> td1 in td)
                        {
                            result2 = Trail.MergeTrailLocations(result2, getGPS(null, td1));
                        }
                    }
                    if (result2.Count == 0)
                    {
                        //Selected
                        result2 = getGPS(selectGPS.SelectedTime, selectGPS.SelectedDistance);
                    }
                }
                result = Trail.MergeTrailLocations(result, result2);
            }
            return result;
        }
#if ST_2_1
//ST_2_1
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
            if (m_controller.CurrentActivityTrailDisplayed != null)
            {
                if (MessageBox.Show(string.Format(Properties.Resources.UI_Activity_Page_AddTrail_Replace, CommonResources.Text.ActionYes,CommonResources.Text.ActionNo),
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    addCurrent = true;
                }
            }
#if ST_2_1
            EditTrail dialog = new EditTrail(m_visualTheme, m_culture, !addCurrent);
#else
            EditTrail dialog = new EditTrail(m_visualTheme, m_culture, m_view, !addCurrent);
#endif
            if (m_controller.CurrentActivityTrailDisplayed != null)
            {
                if (addCurrent)
                {
                    //TODO: sort old/new points, so it is possible to add in middle?
                }
                else
                {
                    dialog.Trail.TrailLocations.Clear();
                }
            }
            dialog.Trail.TrailLocations = Trail.MergeTrailLocations(dialog.Trail.TrailLocations, getGPS(selectedGPS));

			if (dialog.ShowDialog() == DialogResult.OK) {
				m_page.RefreshControlState();
				m_page.RefreshData();
			}
		}


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
#if ST_2_1
            EditTrail dialog = new EditTrail(m_visualTheme, m_culture, false);
#else
            EditTrail dialog = new EditTrail(m_visualTheme, m_culture, m_view, false);
#endif
            bool selectionIsDifferent = selectedGPS.Count != dialog.Trail.TrailLocations.Count;
            if (!selectionIsDifferent)
            {
                IList<TrailGPSLocation> loc = getGPS(selectedGPS);
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
                    dialog.Trail.TrailLocations = getGPS(selectedGPS);
                }
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                m_page.RefreshControlState();
                m_page.RefreshData();
            }
        }


        private void TrailName_ButtonClick(object sender, EventArgs e)
        {
            TreeListPopup treeListPopup = new TreeListPopup();
            treeListPopup.ThemeChanged(m_visualTheme);
            treeListPopup.Tree.Columns.Add(new TreeList.Column());

            treeListPopup.Tree.RowData = m_controller.OrderedTrails;
            //Note: Just checking for current trail could modify the ordered list, so do this first
            System.Collections.IList currSel = null;
            if (m_controller.CurrentActivityTrailDisplayed != null)
            {
                currSel = new object[] { m_controller.CurrentActivityTrailDisplayed };
            //    foreach (TrailOrdered to in m_controller.OrderedTrails)
            //    {
            //        if (m_controller.CurrentActivityTrail.Equals(to.activityTrail))
            //        {
            //            currSel = new object[] { to };
            //            break;
            //        }
            //    }
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

		class TrailDropdownLabelProvider : TreeList.ILabelProvider {

			public Image GetImage(object element, TreeList.Column column) {
                ActivityTrail t = (ActivityTrail)element;
                if (t.status == TrailOrderStatus.Match)
                {
                    return Properties.Resources.square_green;
                }
                else if (t.status == TrailOrderStatus.MatchNoCalc)
                {
                    return Properties.Resources.square_green_check;
                }
                else if (t.status == TrailOrderStatus.InBoundNoCalc)
                {
                    return Properties.Resources.square_green_add;
                }
                else if (t.status == TrailOrderStatus.InBound)
                {
                    return Properties.Resources.square_red;
                }
                else if (t.status == TrailOrderStatus.NotInBound)
                {
                    return Properties.Resources.square_blue;
                }
                else
                {
                    return null;
                }
			}

			public string GetText(object element, TreeList.Column column) {
                ActivityTrail t = (ActivityTrail)element;
                string name = t.Trail.Name;
                if (t.status == TrailOrderStatus.Match)
                {
                    name += " (" + t.Results.Count + ")";
                }
                else if (t.status == TrailOrderStatus.MatchNoCalc)
                {
                    if (t.Trail.MatchAll)
                    {
                        name += " (" + t.ActivityCount + ")";
                    }
                }
                return name;
            }
		}

		private void TrailName_ItemSelected(object sender, EventArgs e) {
            ActivityTrail t = ((ActivityTrail)((TreeListPopup.ItemSelectedEventArgs)e).Item);
            if(sender is TreeListPopup)
            {
                ((TreeListPopup)sender).Hide();
            }
			m_controller.CurrentActivityTrail = t;
            m_page.RefreshData();
            m_page.RefreshControlState();
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
