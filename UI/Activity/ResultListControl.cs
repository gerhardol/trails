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
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;

using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

#if ST_2_1
//IListItem, ListSettings
using ZoneFiveSoftware.SportTracks.Util;
using ZoneFiveSoftware.SportTracks.UI;
using ZoneFiveSoftware.SportTracks.UI.Forms;
using ZoneFiveSoftware.SportTracks.Data;
using TrailsPlugin.UI.MapLayers;
#else
using ZoneFiveSoftware.Common.Visuals.Forms;
#endif
using TrailsPlugin.Data;
using TrailsPlugin.Integration;

namespace TrailsPlugin.UI.Activity {
    public partial class ResultListControl : UserControl
    {
        ActivityDetailPageControl m_page;
        private ITheme m_visualTheme;
        private Controller.TrailController m_controller;

#if !ST_2_1
        private IDailyActivityView m_view = null;
#endif

        public ResultListControl()
        {
            InitializeComponent();
        }
#if ST_2_1
        public void SetControl(ActivityDetailPageControl page, Controller.TrailController controller)
        {
#else
        public void SetControl(ActivityDetailPageControl page, Controller.TrailController controller, IDailyActivityView view)
        {
            m_view = view;
#endif
            m_page = page;
            m_controller = controller;

            InitControls();
#if ST_2_1
            this.summaryList.SelectedChanged += new System.EventHandler(this.summaryList_SelectedItemsChanged);
#else
            this.summaryList.SelectedItemsChanged += new System.EventHandler(this.summaryList_SelectedItemsChanged);
#endif
        }

        void InitControls()
        {
            copyTableMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.DocumentCopy16;
#if ST_2_1
            limitActivityMenuItem.Visible = false;
            limitURMenuItem.Visible = false;
#else
            this.advancedMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Analyze16;
#endif
            summaryList.NumHeaderRows = TreeList.HeaderRows.Two;
            summaryList.LabelProvider = new TrailResultLabelProvider();

            this.selectWithURMenuItem.Enabled = Integration.UniqueRoutes.UniqueRouteIntegrationEnabled;
            this.limitURMenuItem.Enabled = Integration.UniqueRoutes.UniqueRouteIntegrationEnabled;
            this.markCommonStretchesMenuItem.Enabled = Integration.UniqueRoutes.UniqueRouteIntegrationEnabled;
        }

        public void UICultureChanged(CultureInfo culture)
        {
            copyTableMenuItem.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionCopy;
            this.listSettingsMenuItem.Text = Properties.Resources.UI_Activity_List_ListSettings;
            this.selectSimilarSplitsMenuItem.Text = Properties.Resources.UI_Activity_List_Splits;
            //this.referenceTrailMenuItem.Text = Properties.Resources.UI_Activity_List_ReferenceResult;
            this.advancedMenuItem.Text = Properties.Resources.UI_Activity_List_Advanced;
            this.excludeResultsMenuItem.Text = Properties.Resources.UI_Activity_List_ExcludeResult;
            this.limitActivityMenuItem.Text = Properties.Resources.UI_Activity_List_LimitSelection;
            this.limitURMenuItem.Text = string.Format(Properties.Resources.UI_Activity_List_URLimit, "");
            this.selectWithURMenuItem.Text = string.Format(Properties.Resources.UI_Activity_List_URSelect, "");
            this.markCommonStretchesMenuItem.Text = Properties.Resources.UI_Activity_List_URCommon;
            this.RefreshColumns();
        }
        public void ThemeChanged(ITheme visualTheme)
        {
            m_visualTheme = visualTheme;
            summaryList.ThemeChanged(visualTheme);
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

        private void RefreshColumns()
        {
            summaryList.Columns.Clear();
            int plusMinusSize = summaryList.ShowPlusMinus ? 15 : 0;

            //Permanent fields
            foreach (IListColumnDefinition columnDef in TrailResultColumnIds.PermanentMultiColumnDefs())
            {
                TreeList.Column column = new TreeList.Column(
                    columnDef.Id,
                    columnDef.Text(columnDef.Id),
                    columnDef.Width + plusMinusSize,
                    columnDef.Align
                );
                summaryList.Columns.Add(column);
                plusMinusSize = 0;
            }
            foreach (string id in Data.Settings.ActivityPageColumns)
            {
                foreach (IListColumnDefinition columnDef in TrailResultColumnIds.ColumnDefs(m_controller.ReferenceActivity, m_controller.Activities.Count > 1))
                {
                    if (columnDef.Id == id)
                    {
                        TreeList.Column column = new TreeList.Column(
                            columnDef.Id,
                            columnDef.Text(columnDef.Id),
                            columnDef.Width + plusMinusSize,
                            columnDef.Align
                        );
                        summaryList.Columns.Add(column);
                        plusMinusSize = 0;
                        break;
                    }
                }
            }
        }

        public void RefreshControlState()
        {
            limitActivityMenuItem.Enabled = m_controller.Activities.Count > 1;
            selectSimilarSplitsMenuItem.Checked = Data.Settings.SelectSimilarResults;
        }
        
        public void RefreshList()
        {
            summaryList.RowData = null;

            if (m_controller.CurrentActivityTrailDisplayed != null)
            {
                RefreshColumns();

#if ST_2_1
                this.summaryList.SelectedChanged -= new System.EventHandler(this.summaryList_SelectedItemsChanged);
#else
                this.summaryList.SelectedItemsChanged -= new System.EventHandler(this.summaryList_SelectedItemsChanged);
#endif
                summaryList_Sort();
#if ST_2_1
                this.summaryList.SelectedChanged += new System.EventHandler(this.summaryList_SelectedItemsChanged);
#else
                this.summaryList.SelectedItemsChanged += new System.EventHandler(this.summaryList_SelectedItemsChanged);
#endif
                ((TrailResultLabelProvider)summaryList.LabelProvider).MultipleActivities = (m_controller.Activities.Count > 1);

                //Set size, to not waste chart
                int resRows = Math.Min(5, ((IList<TrailResultWrapper>)summaryList.RowData).Count);
                this.summaryList.Height = this.summaryList.HeaderRowHeight +
                    this.summaryList.DefaultRowHeight * resRows;
            }
            //By setting to null, the last used is selected, or some defaults
            SelectedItemsWrapper = null;
        }

        //Wrapper for ST2
        private System.Collections.IList SelectedItemsRaw
        {
            set
            {
#if ST_2_1
                    summaryList.Selected
#else
                    summaryList.SelectedItems
#endif
                        = value;
                }
            get
            {
                return 
#if ST_2_1
                  summaryList.Selected;
#else
                  summaryList.SelectedItems;
#endif
            }
        }
        private IList<TrailResultWrapper> m_SelectedItemsWrapper=null;
        //Wrap the table SelectedItems, from a generic type
        private IList<TrailResultWrapper> SelectedItemsWrapper
        {
            set
            {
                IList<TrailResultWrapper> setValue = (List<TrailResultWrapper>)value;
                if (null == setValue || !setValue.Equals(m_SelectedItemsWrapper))
                {
                    if (setValue == null || setValue.Count == 0)
                    {
                        //Select a value
                        setValue = TrailResultWrapper.SelectedItems
                    ((IList<TrailResultWrapper>)summaryList.RowData, TrailResultWrapper.GetTrailResults(m_SelectedItemsWrapper));
                        if (((IList<TrailResultWrapper>)summaryList.RowData).Count > 0)
                        {
                            setValue = new List<TrailResultWrapper> {
                           ((IList<TrailResultWrapper>)summaryList.RowData)[0] };
                        }
                    }
                    m_SelectedItemsWrapper = setValue;
                    SelectedItemsRaw = (List<TrailResultWrapper>)setValue;
                }
            }
            get
            {
                return (IList<TrailResultWrapper>)getTrailResultWrapperSelection(this.SelectedItemsRaw);
            }
        }

        public IList<TrailResult> SelectedItems
        {
//            set
//            {
//                IList<TrailResultWrapper> results = TrailResultWrapper.SelectedItems
//                    ((IList<TrailResultWrapper>)summaryList.RowData, value);
//                SelectedItemsWrapper = (List<TrailResultWrapper>)results;
//                summaryList.EnsureVisible(results);
//            }
            get {
                return getTrailResultSelection(SelectedItemsWrapper);
            }
        }
        public void EnsureVisible(IList<TrailResult> atr)
        {
            EnsureVisible(TrailResultWrapper.SelectedItems
                    ((IList<TrailResultWrapper>)summaryList.RowData, atr));
        }
        public void EnsureVisible(IList<TrailResultWrapper> atr)
        {
            if (atr != null && atr.Count > 0)
            {
                summaryList.EnsureVisible(atr[0]);
            }
        }
        /*********************************************************/
        public static TrailResult getTrailResultRow(object element)
        {
            return getTrailResultRow(element, false);
        }
        public static TrailResult getTrailResultRow(object element, bool ensureParent)
        {
            TrailResultWrapper result = (TrailResultWrapper)element;
            if (ensureParent)
            {
                if (result.Parent != null)
                {
                    result = (TrailResultWrapper)result.Parent;
                }
            }
            return result.Result;
        }

        public static IList<TrailResultWrapper> getTrailResultWrapperSelection(System.Collections.IList tlist)
        {
            IList<TrailResultWrapper> aTr = new List<TrailResultWrapper>();
            if (tlist != null)
            {
                foreach (object t in tlist)
                {
                    if (t != null)
                    {
                        aTr.Add(((TrailResultWrapper)t));
                    }
                }
            }
            return aTr;
        }

        public static IList<TrailResult> getTrailResultSelection(IList<TrailResultWrapper> tlist)
        {
            IList<TrailResult> aTr = new List<TrailResult>();
            foreach (TrailResultWrapper t in tlist)
            {
                if (t != null)
                {
                    aTr.Add(((TrailResultWrapper)t).Result);
                }
            }
            return aTr;
        }

        private void summaryList_Sort()
        {
            if (m_controller.CurrentActivityTrailDisplayed != null)
            {
                m_controller.CurrentActivityTrailDisplayed.Sort();
                summaryList.RowData = m_controller.CurrentActivityTrailDisplayed.ResultTreeList;
                summaryList.SetSortIndicator(TrailsPlugin.Data.Settings.SummaryViewSortColumn,
                    TrailsPlugin.Data.Settings.SummaryViewSortDirection == ListSortDirection.Ascending);
            }
        }

        void selectSimilarSplits()
        {
            bool isChange = false;
#if ST_2_1
            this.summaryList.SelectedChanged -= new System.EventHandler(summaryList_SelectedItemsChanged);
#else
            this.summaryList.SelectedItemsChanged -= new System.EventHandler(summaryList_SelectedItemsChanged);
#endif
            if (Data.Settings.SelectSimilarResults && this.SelectedItemsRaw != null)
            {
                //Note that selecting will scroll
                int? lastSplitIndex = null;
                bool isSingleIndex = false;
                IList<TrailResultWrapper> results = new List<TrailResultWrapper>();
                foreach (TrailResultWrapper t in this.SelectedItemsWrapper)
                {
                    int splitIndex = -1;
                    if (((TrailResultWrapper)t).Parent != null)
                    {
                        splitIndex = ((TrailResultWrapper)t).Result.Order;
                    }
                    isSingleIndex = (lastSplitIndex == null || lastSplitIndex == splitIndex) ? true : false;
                    lastSplitIndex=splitIndex;
                    foreach (TrailResultWrapper rtn in (IList<TrailResultWrapper>)summaryList.RowData)
                    {
                        if (splitIndex < 0)
                        {
                            results.Add(rtn);
                        }
                        else
                        {
                            foreach (TrailResultWrapper ctn in rtn.Children)
                            {
                                if (ctn.Result.Order == splitIndex)
                                {
                                    results.Add(ctn);
                                }
                            }
                        }
                    }
                }
                if (results != SelectedItemsWrapper)
                {
                    this.SelectedItemsWrapper = results;
                    isChange = true;
                }
                if (isSingleIndex)
                {
                    //ST scrolls before the mouse is recorded. Therefore the selected item could be one step off
                    TrailResult currTr = getMouseResult(false);
                    if (currTr != null && lastSplitIndex >= 0 && currTr.Order != lastSplitIndex)
                    {
                        IList<TrailResultWrapper> atrp = TrailResultWrapper.SelectedItems(m_controller.CurrentActivityTrail.ResultTreeList, new List<TrailResult> { currTr.ParentResult });
                        if (atrp != null && atrp.Count > 0)
                        {
                            foreach (TrailResultWrapper trc in atrp[0].Children)
                            {
                                if (trc.Result.Order == lastSplitIndex)
                                {
                                    currTr = trc.Result;
                                    break;
                                }
                            }
                        }
                    }
                    if (currTr != this.m_controller.ReferenceTrailResult)
                    {
                        this.m_controller.ReferenceTrailResult = currTr;
                        isChange = true;
                    }
                }
            }
#if ST_2_1
            this.summaryList.SelectedChanged += new System.EventHandler(summaryList_SelectedItemsChanged);
#else
            this.summaryList.SelectedItemsChanged += new System.EventHandler(summaryList_SelectedItemsChanged);
#endif
            if (isChange)
            {
                this.m_page.RefreshChart();
            }
        }

        /************************************************************/
        void summaryList_Click(object sender, System.EventArgs e)
        {
            //SelectTrack, for ST3
            if (sender is TreeList)
            {
                TreeList l = sender as TreeList;
                //Check if header. ColumnHeaderClicked will not fire due to this
                if (l.HeaderRowHeight >= ((MouseEventArgs)e).Y)
                {
                    int nStart = ((MouseEventArgs)e).X;
                    int spos = l.Location.X;// +l.Parent.Location.X;
                    int subItemSelected = 0;
                    for (int i = 0; i < l.Columns.Count; i++)
                    {
                        int epos = spos + l.Columns[i].Width;
                        if (nStart > spos && nStart < epos)
                        {
                            subItemSelected = i;
                            break;
                        }

                        spos = epos;
                    }
                    summaryList_ColumnHeaderMouseClick(sender, l.Columns[subItemSelected]);
                }
                else
                {
                    object row;
                    TreeList.RowHitState hit;
                    //Note: As ST scrolls before Location is recorded, incorrect row may be selected...
                    row = summaryList.RowHitTest(((MouseEventArgs)e).Location, out hit);
                    if (row != null && hit == TreeList.RowHitState.Row)
                    {
                        TrailResult tr = getTrailResultRow(row);
                        bool colorSelected = false;
                        if (hit != TreeList.RowHitState.PlusMinus)
                        {
                            int nStart = ((MouseEventArgs)e).X;
                            int spos = l.Location.X;// +l.Parent.Location.X;
                            for (int i = 0; i < l.Columns.Count; i++)
                            {
                                int epos = spos + l.Columns[i].Width;
                                if (nStart > spos && nStart < epos)
                                {
                                    if (l.Columns[i].Id == TrailResultColumnIds.Color)
                                    {
                                        colorSelected = true;
                                        break;
                                    }
                                }

                                spos = epos;
                            }
                        }
                        if (colorSelected)
                        {
                            ColorSelectorPopup cs = new ColorSelectorPopup();
                            cs.Width = 70;
                            cs.ThemeChanged(m_visualTheme);
                            cs.DesktopLocation = ((Control)sender).PointToScreen(((MouseEventArgs)e).Location);
                            cs.Selected = tr.TrailColor;
                            m_ColorSelectorResult = tr;
                            cs.ItemSelected += new ColorSelectorPopup.ItemSelectedEventHandler(cs_ItemSelected);
                            cs.Show();
                        }
                        else
                        {
                            bool isMatch = false;
                            foreach (TrailResultWrapper t in SelectedItemsWrapper)
                            {
                                if (t.Result == tr)
                                {
                                    isMatch = true;
                                    break;
                                }
                            }
                            if (isMatch)
                            {
                                IList<TrailResult> aTr = new List<TrailResult>();
                                if (TrailsPlugin.Data.Settings.SelectSimilarResults)
                                {
                                    //Select the single row only
                                    aTr.Add(tr);
                                }
                                else
                                {
                                    //The user can control what is selected - mark all
                                    aTr = this.SelectedItems;
                                }
                                m_page.MarkTrack(TrailResultMarked.TrailResultMarkAll(aTr));
                            }
                        }
                    }
                }
            }
        }

        private TrailResult m_ColorSelectorResult = null;
        void cs_ItemSelected(object sender, ColorSelectorPopup.ItemSelectedEventArgs e)
        {
            if (sender is ColorSelectorPopup && m_ColorSelectorResult != null)
            {
                ColorSelectorPopup cs = sender as ColorSelectorPopup;
                if (cs.Selected != m_ColorSelectorResult.TrailColor)
                {
                    m_ColorSelectorResult.TrailColor = cs.Selected;
                    m_page.RefreshData();
                }
            }
            m_ColorSelectorResult = null;
        }

        private void selectedRow_DoubleClick(object sender, MouseEventArgs e)
        {
            Guid view = GUIDs.DailyActivityView;

            object row;
            TreeList.RowHitState dummy;
            row = summaryList.RowHitTest(e.Location, out dummy);
            if (row != null)
            {
                TrailResult tr = getTrailResultRow(row);
                string bookmark = "id=" + tr.Activity;
                PluginMain.GetApplication().ShowView(view, bookmark);
            }
        }

        private void summaryList_ColumnHeaderMouseClick(object sender, TreeList.ColumnEventArgs e)
        {
            summaryList_ColumnHeaderMouseClick(sender, e.Column);
        }

        private void summaryList_ColumnHeaderMouseClick(object sender, TreeList.Column e)
        {
            if (TrailsPlugin.Data.Settings.SummaryViewSortColumn == e.Id)
            {
                TrailsPlugin.Data.Settings.SummaryViewSortDirection = TrailsPlugin.Data.Settings.SummaryViewSortDirection == ListSortDirection.Ascending ?
                       ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            TrailsPlugin.Data.Settings.SummaryViewSortColumn = e.Id;
            summaryList_Sort();
        }

        void summaryList_SelectedItemsChanged(object sender, System.EventArgs e)
        {
            if (Data.Settings.SelectSimilarResults)
            {
                selectSimilarSplits();
            }
            else
            {
                m_page.RefreshChart();
            }
        }
        private System.Windows.Forms.MouseEventArgs m_mouseClickArgs = null;
        void summaryList_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            m_mouseClickArgs = e;
        }
        private TrailResult getMouseResult(bool ensureParent)
        {
            TrailResult tr = null;
            if (m_mouseClickArgs != null)
            {
                object row = null;
                TreeList.RowHitState dummy;
                row = summaryList.RowHitTest(m_mouseClickArgs.Location, out dummy);
                if (row != null)
                {
                    tr = getTrailResultRow(row, ensureParent);
                }
            }
            return tr;
        }

        /*************************************************************************************************************/
        void listMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string currRes = "";
            TrailResult tr = getMouseResult(false);
            if (tr != null)
            {
                currRes = tr.FirstTime.ToLocalTime().ToShortDateString() + " " + 
                    tr.FirstTime.ToLocalTime().ToShortTimeString();
            }
            string refRes = "";
            if (m_controller.ReferenceTrailResult != null)
            {
                refRes = m_controller.ReferenceTrailResult.FirstTime.ToLocalTime().ToShortDateString() + " " + 
                    m_controller.ReferenceTrailResult.FirstTime.ToLocalTime().ToShortTimeString();
            }
            this.referenceResultMenuItem.Text = string.Format(
                Properties.Resources.UI_Activity_List_ReferenceResult, currRes, refRes);
            e.Cancel = false;
        }

        void copyTableMenu_Click(object sender, EventArgs e)
        {
            summaryList.CopyTextToClipboard(true, System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator);
        }

        private void listSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if ST_2_1
            ListSettings dialog = new ListSettings();
            dialog.ColumnsAvailable = TrailResultColumnIds.ColumnDefs_ST2(m_controller.ReferenceActivity, false);
#else
            ListSettingsDialog dialog = new ListSettingsDialog();
            dialog.AvailableColumns = TrailResultColumnIds.ColumnDefs(m_controller.ReferenceActivity, m_controller.Activities.Count > 1);
#endif
            dialog.ThemeChanged(m_visualTheme);
            dialog.AllowFixedColumnSelect = true;
            dialog.SelectedColumns = Data.Settings.ActivityPageColumns;
            dialog.NumFixedColumns = Data.Settings.ActivityPageNumFixedColumns;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Data.Settings.ActivityPageNumFixedColumns = dialog.NumFixedColumns;
                Data.Settings.ActivityPageColumns = dialog.SelectedColumns;
                RefreshColumns();
            }
        }

        void referenceResultMenuItem_Click(object sender, System.EventArgs e)
        { 
            TrailResult tr = getMouseResult(false);
            if (tr != m_controller.ReferenceTrailResult)
            {
                m_controller.ReferenceTrailResult = tr;
                m_page.RefreshChart();
            }
        }

        void selectSimilarSplitsMenuItem_Click(object sender, System.EventArgs e)
        {
            TrailsPlugin.Data.Settings.SelectSimilarResults = !Data.Settings.SelectSimilarResults;
            selectSimilarSplits();
            RefreshControlState();
        }

        void excludeResultsMenuItem_Click(object sender, System.EventArgs e)
        {
            if (this.SelectedItemsRaw != null && this.SelectedItemsRaw.Count > 0 &&
                m_controller.CurrentActivityTrail.ResultTreeList!= null)
            {
                IList<TrailResultWrapper> atr = this.SelectedItemsWrapper;
                m_controller.CurrentActivityTrail.Remove(atr);
                m_page.RefreshData();
                m_page.RefreshControlState();
            }
        }

        void limitActivityMenuItem_Click(object sender, System.EventArgs e)
        {
#if !ST_2_1
            if (this.SelectedItemsRaw != null && this.SelectedItemsRaw.Count > 0)
            {
                IList<TrailResult> atr = this.SelectedItems;
                IList<IActivity> aAct = new List<IActivity>();
                foreach (TrailResult tr in atr)
                {
                    aAct.Add(tr.Activity);
                }
                m_view.SelectionProvider.SelectedItems = (List<IActivity>)aAct;
            }
#endif
        }

        void limitURMenuItem_Click(object sender, System.EventArgs e)
        {
#if !ST_2_1
            try
            {
                IList<IActivity> similarActivities = UniqueRoutes.GetUniqueRoutesForActivity(
                    m_controller.ReferenceActivity, m_controller.Activities, null);

                if (similarActivities != null)
                {
                    IList<IActivity> allActivities = new List<IActivity> { m_controller.ReferenceActivity };
                    foreach (IActivity activity in m_controller.Activities)
                    {
                        if (similarActivities.Contains(activity) && !allActivities.Contains(activity))
                        {
                            allActivities.Add(activity);
                        }
                    }
                    m_page.Activities = allActivities;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plugin error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
#endif
        }

        void selectWithURMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                IList<IActivity> similarActivities = UniqueRoutes.GetUniqueRoutesForActivity(
                    m_controller.ReferenceTrailResult.GpsTrack, null, null);
                if (similarActivities != null)
                {
                    IList<IActivity> allActivities = new List<IActivity> { m_controller.ReferenceActivity };
                    foreach (IActivity activity in similarActivities)
                    {
                        if (!m_controller.Activities.Contains(activity))
                        {
                            allActivities.Add(activity);
                        }
                    }
                    ActivityTrail t = m_controller.CurrentActivityTrail;
                    m_controller.Activities = allActivities;
                    if (m_controller.CurrentActivityTrailDisplayed == null)
                    {
                        m_controller.CurrentActivityTrail = t;
                    }
                    m_page.RefreshData();
                    m_page.RefreshControlState();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plugin error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void markCommonStretchesMenuItem_Click(object sender, System.EventArgs e)
        {
            IList<IActivity> activities = new List<IActivity>();
            foreach (TrailResultWrapper t in SelectedItemsWrapper)
            {
                if (!activities.Contains(t.Result.Activity))
                {
                    activities.Add(t.Result.Activity);
                }
            }
            IList<TrailResultMarked> aTrm = new List<TrailResultMarked>();
            IDictionary<IActivity, IItemTrackSelectionInfo[]> commonStretches = UniqueRoutes.GetCommonStretchesForActivity(m_controller.ReferenceActivity, activities, null);
            if (commonStretches != null && commonStretches.Count > 0)
            {
                foreach (TrailResult tr in this.SelectedItems)
                {
                    if (m_controller.ReferenceActivity != tr.Activity &&
                        commonStretches.ContainsKey(tr.Activity) &&
                        commonStretches[tr.Activity] != null)
                    {
                        aTrm.Add(new TrailResultMarked(tr, commonStretches[tr.Activity][0].MarkedTimes));
                    }
                }
            }
            m_page.MarkTrack(aTrm);
            m_page.SetSelectedRegions(aTrm);
        }
    }
}
