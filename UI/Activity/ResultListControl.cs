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
        public void SetControl(ActivityDetailPageControl page, Controller.TrailController controller, Object view)
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
            this.summaryList.NumHeaderRows = TreeList.HeaderRows.Two;
            this.summaryList.LabelProvider = new TrailResultLabelProvider();
            this.summaryList.RowDataRenderer = new SummaryRowDataRenderer(this.summaryList);

            this.progressBar.Visible = false;

            this.selectWithURMenuItem.Enabled = Integration.UniqueRoutes.UniqueRouteIntegrationEnabled;
            this.limitURMenuItem.Enabled = Integration.UniqueRoutes.UniqueRouteIntegrationEnabled;
            this.markCommonStretchesMenuItem.Enabled = Integration.UniqueRoutes.UniqueRouteIntegrationEnabled;
            this.summaryListToolTipTimer.Tick += new System.EventHandler(ToolTipTimer_Tick);

            ((TrailResultLabelProvider)summaryList.LabelProvider).Controller = m_controller;
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
            this.addInBoundActivitiesMenuItem.Text = Properties.Resources.UI_Activity_List_AddInBound;
            this.addCurrentCategoryMenuItem.Text = Properties.Resources.UI_Activity_List_AddCurrentCategory;
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
            summaryList.NumLockedColumns = Data.Settings.ActivityPageNumFixedColumns;
        }

        public void RefreshControlState()
        {
            limitActivityMenuItem.Enabled = m_controller.Activities.Count > 1;
            selectSimilarSplitsMenuItem.Checked = Data.Settings.SelectSimilarResults;
            addCurrentCategoryMenuItem.Checked = Data.Settings.AddCurrentCategory;
        }

        public void RefreshList()
        {
            System.Collections.IList currSelected = summaryList.SelectedItems;
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
            }

            //By setting to null, the last used is selected, or some defaults
            SelectedItemsWrapper = null;
            SummaryPanel_HandleCreated(this.SummaryPanel, null);
        }

        private const int cResultListHeight = 17;//Should be possible to read out from list...
        void SummaryPanel_HandleCreated(object sender, System.EventArgs e)
        {
            if (m_page != null)
            {
                //Set size, to not waste chart
                int minRows = 2;
                const int maxRows = 8;
                int listRows = minRows;
                if (summaryList.RowData != null)
                {
                    listRows = ((IList<TrailResultWrapper>)summaryList.RowData).Count;
                }
                int setRows = Math.Max(minRows, listRows);
                setRows = Math.Min(maxRows, setRows);
                int displayRows = (m_page.SetResultListHeight - 16 - this.summaryList.HeaderRowHeight) / cResultListHeight;
                if (summaryList.HorizontalScroll.Enabled)
                {
                    //About one row hidden
                    displayRows--;
                    setRows++;
                }

                //Change size if much too small/big only
                if (//listRows + 1 < displayRows && setRows < displayRows || //wasted space //disabled decreasing
                    listRows > displayRows && displayRows <= maxRows //Too small, increase
                    )
                {
                    m_page.SetResultListHeight = this.summaryList.HeaderRowHeight + 16 +
                        cResultListHeight * setRows;
                }
            }
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

        private System.Collections.IList m_prevSelectedItems = null;
        //Wrap the table SelectedItems, from a generic type
        private IList<TrailResultWrapper> SelectedItemsWrapper
        {
            set
            {
                IList<TrailResultWrapper> setValue = value;
                if (null == setValue || !setValue.Equals(m_prevSelectedItems))
                {
                    if (setValue == null || setValue.Count == 0)
                    {
                        //Get all current values in prev selection
                        setValue = TrailResultWrapper.SelectedItems(
                            m_controller.CurrentActivityTrailDisplayed.ResultTreeList,
                    //(IList<TrailResultWrapper>)summaryList.RowData,
                    TrailResultWrapper.GetTrailResults(getTrailResultWrapperSelection(m_prevSelectedItems, m_controller.CurrentActivityTrailDisplayed.ResultTreeList)));//(IList<TrailResultWrapper>)this.summaryList.RowData)));
                        if ((null == setValue || setValue.Count == 0) &&
                            null != m_controller.CurrentActivityTrailDisplayed.ResultTreeList && m_controller.CurrentActivityTrailDisplayed.ResultTreeList.Count > 0)
                        {
                            setValue = new List<TrailResultWrapper> {
                           m_controller.CurrentActivityTrailDisplayed.ResultTreeList[0] };
                        }
                    }
                }
                //Set value, let callback update m_prevSelectedItems and refresh chart
                SelectedItemsRaw = (List<TrailResultWrapper>)setValue;
            }
            get
            {
                return getTrailResultWrapperSelection(this.SelectedItemsRaw, m_controller.CurrentActivityTrailDisplayed.ResultTreeList);
            }
        }

        public IList<TrailResult> SelectedItems
        {
            get
            {
                return TrailResultWrapper.GetTrailResults(SelectedItemsWrapper);
            }
        }

        public void EnsureVisible(IList<TrailResult> atr)
        {
            EnsureVisible(TrailResultWrapper.SelectedItems
                    (m_controller.CurrentActivityTrailDisplayed.ResultTreeList, atr));
        }
        public void EnsureVisible(IList<TrailResultWrapper> atr)
        {
            if (atr != null && atr.Count > 0)
            {
                summaryList.EnsureVisible(atr[0]);
            }
        }

        public System.Windows.Forms.ProgressBar StartProgressBar(int val)
        {
            this.summaryList.Visible = false;
            this.progressBar.Value = 0;
            this.progressBar.Minimum = 0;
            this.progressBar.Maximum = val;
            this.progressBar.Visible = true;
            this.progressBar.BringToFront();
            return this.progressBar;
        }
        public void StopProgressBar()
        {
            this.summaryList.Visible = true;
            this.progressBar.Visible = false;
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

        public static IList<TrailResultWrapper> getTrailResultWrapperSelection(System.Collections.IList tlist, IList<TrailResultWrapper> rowData)
        {
            IList<TrailResultWrapper> aTr = new List<TrailResultWrapper>();
            if (tlist != null)
            {
                foreach (object t in tlist)
                {
                    if (t != null)
                    {
                        if (((TrailResultWrapper)t).IsSummary && rowData != null)
                        {
                            List<TrailResultWrapper> alist = new List<TrailResultWrapper>();
                            foreach (TrailResultWrapper t2 in rowData)
                            {
                                alist.Add(t2);
                            }
                            return alist;
                        }
                        aTr.Add(((TrailResultWrapper)t));
                    }
                }
            }
            return aTr;
        }

        private void summaryList_Sort()
        {
            if (m_controller.CurrentActivityTrailDisplayed != null)
            {
                m_controller.CurrentActivityTrailDisplayed.Sort();
                summaryList.RowData = m_controller.CurrentActivityTrailDisplayed.ResultTreeListRows(this.SelectedItemsWrapper);
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
                    if (t.Parent != null)
                    {
                        splitIndex = t.Result.Order;
                    }
                    isSingleIndex = (lastSplitIndex == null || lastSplitIndex == splitIndex) ? true : false;
                    lastSplitIndex = splitIndex;
                    foreach (TrailResultWrapper rtn in m_controller.CurrentActivityTrailDisplayed.ResultTreeList)
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
                //If a single index is selected, let the reference follow the current result
                if (isSingleIndex)
                {
                    if (this.m_controller.ReferenceTrailResult.Order != lastSplitIndex)
                    {
                        if (lastSplitIndex < 0)
                        {
                            this.m_controller.ReferenceTrailResult = this.m_controller.ReferenceTrailResult.ParentResult;
                            isChange = true;
                        }
                        else
                        {
                            IList<TrailResultWrapper> atrp;
                            if (this.m_controller.ReferenceTrailResult.ParentResult == null)
                            {
                                atrp = TrailResultWrapper.SelectedItems(m_controller.CurrentActivityTrail.ResultTreeList,
                                    new List<TrailResult> { this.m_controller.ReferenceTrailResult });
                            }
                            else
                            {
                                atrp = TrailResultWrapper.SelectedItems(m_controller.CurrentActivityTrail.ResultTreeList,
                                    new List<TrailResult> { this.m_controller.ReferenceTrailResult.ParentResult });
                            }
                            if (atrp != null && atrp.Count > 0)
                            {
                                foreach (TrailResultWrapper trc in atrp[0].Children)
                                {
                                    if (trc.Result.Order == lastSplitIndex)
                                    {
                                        this.m_controller.ReferenceTrailResult = trc.Result;
                                        isChange = true;
                                        break;
                                    }
                                }
                            }
                        }
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

        void excludeSelectedResults(bool invertSelection)
        {
            if (this.SelectedItemsRaw != null && this.SelectedItemsRaw.Count > 0 &&
                m_controller.CurrentActivityTrail.ResultTreeList != null)
            {
                IList<TrailResultWrapper> atr = this.SelectedItemsWrapper;
                m_controller.CurrentActivityTrail.Remove(atr, invertSelection);
                m_page.RefreshData();
                m_page.RefreshControlState();
            }
        }
        void selectAll()
        {
            System.Collections.IList all = new List<TrailResultWrapper>();
            foreach (TrailResultWrapper t in m_controller.CurrentActivityTrailDisplayed.ResultTreeList)
            {
                if (!t.IsSummary)
                {
                    all.Add(t);
                }
            }
            this.SelectedItemsRaw = all;
        }
        void copyTable()
        {
            summaryList.CopyTextToClipboard(true, System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator);
        }

        void selectSimilarSplitsChanged()
        {
            TrailsPlugin.Data.Settings.SelectSimilarResults = !Data.Settings.SelectSimilarResults;
            selectSimilarSplits();
            RefreshControlState();
        }

        void selectWithUR()
        {
            if (Integration.UniqueRoutes.UniqueRouteIntegrationEnabled)
            {
                try
                {
                    IList<IActivity> similarActivities = null;
                    System.Windows.Forms.ProgressBar progressBar = StartProgressBar(1);
                    if (m_controller.ReferenceTrailResult != null &&
                        m_controller.ReferenceTrailResult.GPSRoute != null)
                    {
                        similarActivities = UniqueRoutes.GetUniqueRoutesForActivity(
                           m_controller.ReferenceTrailResult.GPSRoute, null, progressBar);
                    }
                    else if (m_controller.ReferenceActivity != null)
                    {
                        similarActivities = UniqueRoutes.GetUniqueRoutesForActivity(
                           m_controller.ReferenceActivity.GPSRoute, null, progressBar);
                    }
                    StopProgressBar();
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
        }

        void markCommonStretches()
        {
            if (Integration.UniqueRoutes.UniqueRouteIntegrationEnabled)
            {
                IList<IActivity> activities = new List<IActivity>();
                foreach (TrailResultWrapper t in SelectedItemsWrapper)
                {
                    if (t.Result.Activity != null && !activities.Contains(t.Result.Activity))
                    {
                        activities.Add(t.Result.Activity);
                    }
                }
                System.Windows.Forms.ProgressBar progressBar = StartProgressBar(activities.Count);
                IList<TrailResultMarked> aTrm = new List<TrailResultMarked>();
                IDictionary<IActivity, IItemTrackSelectionInfo[]> commonStretches = TrailResult.CommonStretches(m_controller.ReferenceActivity, activities, progressBar);
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
                StopProgressBar();
                m_page.MarkTrack(aTrm);
                m_page.SetSelectedRegions(aTrm);
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

        void summaryList_DoubleClick(object sender, System.EventArgs e)
        {
            if (sender is TreeList)
            {
                object row;
                TreeList.RowHitState hit;
                //Note: As ST scrolls before Location is recorded, incorrect row may be selected...
                row = summaryList.RowHitTest(((MouseEventArgs)e).Location, out hit);
                if (row != null && hit == TreeList.RowHitState.Row)
                {
                    Guid view = GUIDs.DailyActivityView;
                    TrailResult tr = getTrailResultRow(row);
                    string bookmark = "id=" + tr.Activity;
                    Plugin.GetApplication().ShowView(view, bookmark);
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

        //Never active when plusminus (?) is added
        //private void summaryList_MouseDoubleClick(object sender, MouseEventArgs e)
        //{
        //    Guid view = GUIDs.DailyActivityView;

        //    object row;
        //    TreeList.RowHitState dummy;
        //    row = summaryList.RowHitTest(e.Location, out dummy);
        //    if (row != null)
        //    {
        //        TrailResult tr = getTrailResultRow(row);
        //        string bookmark = "id=" + tr.Activity;
        //        Plugin.GetApplication().ShowView(view, bookmark);
        //    }
        //}

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
            m_prevSelectedItems = summaryList.SelectedItems;
            summaryList.RowData = m_controller.CurrentActivityTrailDisplayed.ResultTreeListRows(this.SelectedItemsWrapper);
        }

        bool IsCurrentCategory(IActivityCategory activityCat, IActivityCategory filterCat)
        {
            if (activityCat == null)
            {
                return false;
            }
            else if (activityCat == filterCat)
            {
                return true;
            }
            return IsCurrentCategory(activityCat.Parent, filterCat);
        }

        public void addCurrentCategoryCheck()
        {
            if (this.addCurrentCategoryMenuItem.Checked &&
                m_controller.ReferenceTrailResult != null)
            {
                addCurrentCategory(false);
            }
        }

        void addCurrentCategory(bool addAll)
        {
            IList<IActivity> allActivities = new List<IActivity>();
            foreach (IActivity activity in m_controller.Activities)
            {
                allActivities.Add(activity);
            }
            IActivityCategory cat = null;
            if (!addAll)
            {
                if (m_controller.ReferenceActivity == null ||
                    Plugin.GetApplication().DisplayOptions.SelectedCategoryFilter != Plugin.GetApplication().Logbook.ActivityCategories[0] &&
                    Plugin.GetApplication().DisplayOptions.SelectedCategoryFilter != Plugin.GetApplication().Logbook.ActivityCategories[1])
                {
                    //User has selected an activity filter - use it
                    cat = Plugin.GetApplication().DisplayOptions.SelectedCategoryFilter;
                }
                else if (m_controller.ReferenceActivity != null)
                {
                    //Use the category for the activity
                    cat = m_controller.ReferenceActivity.Category;
                }
            }
            foreach (IActivity activity in Plugin.GetApplication().Logbook.Activities)
            {
                if (!m_controller.Activities.Contains(activity) &&
                    (cat == null || IsCurrentCategory(activity.Category, cat)))
                {
                    //Insert after the current activities, then the order is normally OK
                    allActivities.Insert(m_controller.Activities.Count, activity);
                }
            }
            ActivityTrail t = m_controller.CurrentActivityTrail;
            m_controller.Activities = allActivities;
            if (m_controller.CurrentActivityTrailDisplayed != t)
            {
                m_controller.CurrentActivityTrail = t;
            }
            m_page.RefreshData();
            m_page.RefreshControlState();
        }

        void addCurrentTime()
        {
            IList<IActivity> allActivities = new List<IActivity>();
            foreach (IActivity activity in m_controller.Activities)
            {
                allActivities.Add(activity);
            }
            if (m_controller.ReferenceActivity != null)
            {
                //Not always true or set...
                DateTime start = m_controller.ReferenceActivity.StartTime;
                DateTime end = ActivityInfoCache.Instance.GetInfo(m_controller.ReferenceActivity).EndTime;
                foreach (IActivity activity in Plugin.GetApplication().Logbook.Activities)
                {
                    //aprox end
                    DateTime end2 = activity.StartTime + activity.TotalTimeEntered;
                    if (!m_controller.Activities.Contains(activity) &&
                        (activity.StartTime >= start && activity.StartTime <= end ||
                        start >= activity.StartTime && start <= end2))
                    {
                        //Insert after the current activities, then the order is normally OK
                        allActivities.Insert(m_controller.Activities.Count, activity);
                    }
                }
            }
            ActivityTrail t = m_controller.CurrentActivityTrail;
            m_controller.Activities = allActivities;
            if (m_controller.CurrentActivityTrailDisplayed != t)
            {
                m_controller.CurrentActivityTrail = t;
            }
            m_page.RefreshData();
            m_page.RefreshControlState();

        }
        void summaryList_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                excludeSelectedResults(e.Modifiers == Keys.Shift);
            }
            else if (e.KeyCode == Keys.Space)
            {
                selectSimilarSplitsChanged();
            }
            else if (e.KeyCode == Keys.A)
            {
                if (e.Modifiers == Keys.Control)
                {
                    selectAll();
                }
                else
                {
                    if (e.Modifiers == Keys.Shift)
                    {
                        TrailsPlugin.Data.Settings.RestIsPause = false;
                    }
                    else
                    {
                        TrailsPlugin.Data.Settings.RestIsPause = true;
                    }
                    m_page.RefreshData();
                }
            }
            else if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                copyTable();
            }
            else if (e.KeyCode == Keys.C)
            {
                if (e.Modifiers == Keys.Shift)
                {
                    TrailsPlugin.Data.Settings.DiffUsingCommonStretches = !TrailsPlugin.Data.Settings.DiffUsingCommonStretches;
                    m_page.RefreshData();
                }
                else
                {
                    markCommonStretches();
                }
            }
            else if (e.KeyCode == Keys.I)
            {
                addCurrentCategory(e.Modifiers == Keys.Shift);
            }
            else if (e.KeyCode == Keys.N)
            {
                if (e.Modifiers == Keys.Shift)
                {
                    TrailsPlugin.Data.Settings.NonReqIsPause = false;
                }
                else
                {
                    TrailsPlugin.Data.Settings.NonReqIsPause = true;
                }
                m_page.RefreshData();
            }
            else if (e.KeyCode == Keys.O)
            {
                if (e.Modifiers == Keys.Shift)
                {
                    TrailsPlugin.Data.Settings.StartDistOffsetFromStartPoint = false;
                }
                else
                {
                    TrailsPlugin.Data.Settings.StartDistOffsetFromStartPoint = true;
                }
                //Only in table, no need to refresh
            }
            else if (e.KeyCode == Keys.R)
            {
                m_controller.Reset(false);

                if (e.Modifiers != Keys.Shift)
                {
                    m_page.RefreshData();
                }
                else
                {
                    System.Windows.Forms.ProgressBar progressBar = StartProgressBar(Data.TrailData.AllTrails.Values.Count * m_controller.Activities.Count);
                    //Test trail detection - not documented
                    IList<ActivityTrail> temp = m_controller.GetOrderedTrails(progressBar, true);
                    StopProgressBar();
                }
            }
            else if (e.KeyCode == Keys.S)
            {
                if (e.Modifiers == Keys.Shift)
                {
                    m_page.SetResultListHeight -= cResultListHeight;
                }
                else
                {
                    m_page.SetResultListHeight += cResultListHeight;
                }
            }
            else if (e.KeyCode == Keys.T)
            {
                if (e.Modifiers == Keys.Shift)
                {
                    addCurrentTime();
                }
                else if (e.Modifiers == Keys.Control)
                {
                    TrailResult.m_diffOnDateTime = !TrailResult.m_diffOnDateTime;
                }
            }
            else if (e.KeyCode == Keys.U)
            {
                selectWithUR();
            }
            else if (e.KeyCode == Keys.Z && e.Modifiers == Keys.Control)
            {
                Data.Settings.ZoomToSelection = !Data.Settings.ZoomToSelection;
            }
        }

        void SummaryPanel_SizeChanged(object sender, System.EventArgs e)
        {
            //Not working
            //if (!changedSizeAfterCreation && m_page != null)
            //{
            //    //By default, list is too big, even if set when activities are updated
            //    changedSizeAfterCreation = true;
            //    SummaryPanel_HandleCreated(sender, e);
            //}
        }

        private System.Windows.Forms.MouseEventArgs m_mouseClickArgs = null;
        bool summaryListTooltipDisabled = false; // is set to true, whenever a tooltip would be annoying, e.g. while a context menu is shown
        System.Drawing.Point summaryListCursorLocationAtMouseMove;
        TrailResultWrapper summaryListLastEntryAtMouseMove = null;

        void summaryList_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            m_mouseClickArgs = e;
            TreeList.RowHitState rowHitState;
            TrailResultWrapper entry = (TrailResultWrapper)summaryList.RowHitTest(e.Location, out rowHitState);
            if (entry == summaryListLastEntryAtMouseMove)
                return;
            else
                summaryListToolTip.Hide(summaryList);
            summaryListLastEntryAtMouseMove = entry;
            summaryListCursorLocationAtMouseMove = e.Location;

            if (entry != null)
                summaryListToolTipTimer.Start();
            else
                summaryListToolTipTimer.Stop();
        }
        private void summaryList_MouseLeave(object sender, EventArgs e)
        {
            summaryListToolTipTimer.Stop();
            summaryListToolTip.Hide(summaryList);
        }

        private void ToolTipTimer_Tick(object sender, EventArgs e)
        {
            summaryListToolTipTimer.Stop();

            if (summaryListLastEntryAtMouseMove != null &&
                summaryListCursorLocationAtMouseMove != null &&
                !summaryListTooltipDisabled)
            {
                string tt = summaryListLastEntryAtMouseMove.Result.ToolTip;
                summaryListToolTip.Show(tt,
                              summaryList,
                              new System.Drawing.Point(summaryListCursorLocationAtMouseMove.X +
                                  Cursor.Current.Size.Width / 2,
                                        summaryListCursorLocationAtMouseMove.Y),
                              summaryListToolTip.AutoPopDelay);
            }
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
                currRes = tr.StartDateTime.ToLocalTime().ToString();
            }
            string refRes = "";
            if (m_controller.ReferenceTrailResult != null)
            {
                refRes = m_controller.ReferenceTrailResult.StartDateTime.ToLocalTime().ToString();
            }
            this.referenceResultMenuItem.Text = string.Format(
                Properties.Resources.UI_Activity_List_ReferenceResult, currRes, refRes);
            if (tr == null || tr == m_controller.ReferenceTrailResult)
            {
                //Instead of a special text
                this.referenceResultMenuItem.Enabled = false;
            }
            else
            {
                this.referenceResultMenuItem.Enabled = true;
            }
            if (m_controller.CurrentActivityTrailDisplayed != null)
            {
                this.addInBoundActivitiesMenuItem.Enabled = m_controller.CurrentActivityTrailDisplayed.CanAddInbound;
            }
            if (tr.Activity == null)
            {
                //Summary result
                this.referenceResultMenuItem.Enabled = false;
                this.addCurrentCategoryMenuItem.Enabled = false;
                this.excludeResultsMenuItem.Enabled = false;
                this.markCommonStretchesMenuItem.Enabled = false;
            }
            else
            {
                //Separate handled
                //this.referenceResultMenuItem.Enabled = true;
                this.addCurrentCategoryMenuItem.Enabled = true;
                this.excludeResultsMenuItem.Enabled = true;
                this.markCommonStretchesMenuItem.Enabled = true;
            }
            e.Cancel = false;
        }

        void copyTableMenu_Click(object sender, EventArgs e)
        {
            copyTable();
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
            selectSimilarSplitsChanged();
        }

        void excludeResultsMenuItem_Click(object sender, System.EventArgs e)
        {
            excludeSelectedResults(false);
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
                    if (tr.Activity != null)
                    {
                        aAct.Add(tr.Activity);
                    }
                }
                m_view.SelectionProvider.SelectedItems = (List<IActivity>)aAct;
            }
#endif
        }

        void addInBoundActivitiesMenuItem_Click(object sender, System.EventArgs e)
        {
            if (m_controller.CurrentActivityTrail != null)
            {
                m_controller.CurrentActivityTrail.Clear();
                System.Windows.Forms.ProgressBar progressBar = this.StartProgressBar(1);
                m_controller.CurrentActivityTrail.AddInBoundResult(progressBar);
                m_page.RefreshData();
                m_page.RefreshControlState();
                this.StopProgressBar();
            }
        }

        void addCurrentCategoryMenuItem_Click(object sender, System.EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem addCurrent = (ToolStripMenuItem)sender; //addCurrentCategoryMenuItem
                addCurrent.Checked = !addCurrent.Checked;
                Data.Settings.AddCurrentCategory = addCurrent.Checked;
                addCurrentCategoryCheck();
            }
        }

        void limitURMenuItem_Click(object sender, System.EventArgs e)
        {
#if !ST_2_1
            try
            {
                System.Windows.Forms.ProgressBar progressBar = StartProgressBar(1);
                IList<IActivity> similarActivities = UniqueRoutes.GetUniqueRoutesForActivity(
                    m_controller.ReferenceActivity, m_controller.Activities, progressBar);
                StopProgressBar();
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

        void markCommonStretchesMenuItem_Click(object sender, System.EventArgs e)
        {
            markCommonStretches();
        }
        void selectWithURMenuItem_Click(object sender, System.EventArgs e)
        {
            selectWithUR();
        }
    }
}
