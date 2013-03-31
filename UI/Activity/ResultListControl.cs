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

using ZoneFiveSoftware.Common.Data;
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
        private TrailResultWrapper m_summary;
        private TrailResult m_lastSelectedTrailResult = null;

#if !ST_2_1
        private IDailyActivityView m_view = null;
#endif

        public ResultListControl()
        {
            InitializeComponent();
            this.m_summary = new TrailResultWrapper();
        }
#if ST_2_1
        public void SetControl(ActivityDetailPageControl page, Controller.TrailController controller, Object view)
        {
#else
        public void SetControl(ActivityDetailPageControl page, Controller.TrailController controller, IDailyActivityView view)
        {
            this.m_view = view;
#endif
            this.m_page = page;
            this.m_controller = controller;

            InitControls();
#if ST_2_1
            this.summaryList.SelectedChanged += new System.EventHandler(this.summaryList_SelectedItemsChanged);
#else
            this.summaryList.SelectedItemsChanged += new System.EventHandler(this.summaryList_SelectedItemsChanged);
#endif
            this.m_controller.SelectedResults = null;
        }

        void InitControls()
        {
            copyTableMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.DocumentCopy16;
#if ST_2_1
            limitActivityMenuItem.Visible = false;
            limitURMenuItem.Visible = false;
#else
            this.analyzeMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Analyze16;
            this.advancedMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Analyze16;
#endif
            this.summaryList.NumHeaderRows = TreeList.HeaderRows.Two;
            this.summaryList.LabelProvider = new TrailResultLabelProvider();
            this.summaryList.RowDataRenderer = new SummaryRowDataRenderer(this.summaryList);

            //this.progressBar.Visible = false;

            this.selectWithURMenuItem.Enabled = Integration.UniqueRoutes.UniqueRouteIntegrationEnabled;
            this.limitURMenuItem.Enabled = Integration.UniqueRoutes.UniqueRouteIntegrationEnabled;
            this.markCommonStretchesMenuItem.Enabled = Integration.UniqueRoutes.UniqueRouteIntegrationEnabled;
            this.summaryListToolTipTimer.Tick += new System.EventHandler(ToolTipTimer_Tick);

            ((TrailResultLabelProvider)this.summaryList.LabelProvider).Controller = m_controller;
        }

        public void UICultureChanged(CultureInfo culture)
        {
            this.copyTableMenuItem.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionCopy;
            this.listSettingsMenuItem.Text = Properties.Resources.UI_Activity_List_ListSettings;
            this.selectSimilarSplitsMenuItem.Text = Properties.Resources.UI_Activity_List_Splits;
            //this.referenceTrailMenuItem.Text = Properties.Resources.UI_Activity_List_ReferenceResult;
            this.analyzeMenuItem.Text = CommonResources.Text.ActionAnalyze;
            this.highScoreMenuItem.Text = Properties.Resources.HighScorePluginName;
            this.performancePredictorMenuItem.Text = Properties.Resources.PerformancePredictorPluginName;
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
            this.m_visualTheme = visualTheme;
            this.summaryList.ThemeChanged(visualTheme);
        }

        private bool _showPage = false;
        public bool ShowPage
        {
            get { return this._showPage; }
            set
            {
                this._showPage = value;
                if (!value)
                {
                    //TBD: Hide HS/PP popups if shown
                }
            }
        }

        private void RefreshColumns()
        {
            this.summaryList.Columns.Clear();
            int plusMinusSize = this.summaryList.ShowPlusMinus ? 15 : 0;

            //Permanent fields
            foreach (IListColumnDefinition columnDef in TrailResultColumnIds.PermanentMultiColumnDefs())
            {
                TreeList.Column column = new TreeList.Column(
                    columnDef.Id,
                    columnDef.Text(columnDef.Id),
                    columnDef.Width + plusMinusSize,
                    columnDef.Align
                );
                this.summaryList.Columns.Add(column);
                plusMinusSize = 0;
            }
            foreach (string id in Data.Settings.ActivityPageColumns)
            {
                int noResults = 1;
                if (m_controller.CurrentActivityTrailIsSelected)
                {
                    noResults = m_controller.CurrentResultTreeList.Count;
                }
                foreach (IListColumnDefinition columnDef in TrailResultColumnIds.ColumnDefs(m_controller.ReferenceActivity, noResults, m_controller.Activities.Count>1))
                {
                    if (columnDef.Id == id)
                    {
                        TreeList.Column column = new TreeList.Column(
                            columnDef.Id,
                            columnDef.Text(columnDef.Id),
                            columnDef.Width + plusMinusSize,
                            columnDef.Align
                        );
                        this.summaryList.Columns.Add(column);
                        plusMinusSize = 0;
                        break;
                    }
                }
            }
            this.summaryList.NumLockedColumns = Data.Settings.ActivityPageNumFixedColumns;
        }

        public void RefreshControlState()
        {
            limitActivityMenuItem.Enabled = m_controller.Activities.Count > 1;
            selectSimilarSplitsMenuItem.Checked = Data.Settings.SelectSimilarResults;
            addCurrentCategoryMenuItem.Checked = Data.Settings.AddCurrentCategory;
        }

        public void RefreshList()
        {
            if (m_controller.CurrentActivityTrailIsSelected)
            {
                RefreshColumns();

                summaryList_Sort();
                ((TrailResultLabelProvider)this.summaryList.LabelProvider).MultipleActivities = (m_controller.Activities.Count > 1);
            }
            else
            {
#if ST_2_1
                this.summaryList.SelectedChanged -= new System.EventHandler(summaryList_SelectedItemsChanged);
                summaryList.RowData = null;
                this.summaryList.SelectedChanged += new System.EventHandler(summaryList_SelectedItemsChanged);
#else
                this.summaryList.SelectedItemsChanged -= new System.EventHandler(summaryList_SelectedItemsChanged);
                this.summaryList.RowData = null;
                this.summaryList.SelectedItemsChanged += new System.EventHandler(summaryList_SelectedItemsChanged);
#endif
                // controller is set below: this.m_controller.SelectedResults = null;
            }

            //By setting to null, the last used is selected, or some defaults
            this.SelectedResultWrapper = null;
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
                if (this.summaryList.RowData != null)
                {
                    listRows = ((IList<TrailResultWrapper>)this.summaryList.RowData).Count;
                }
                int setRows = Math.Max(minRows, listRows);
                setRows = Math.Min(maxRows, setRows);
                int displayRows = (m_page.SetResultListHeight - 16 - this.summaryList.HeaderRowHeight) / cResultListHeight;
                if (this.summaryList.HorizontalScroll.Enabled)
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

        private System.Collections.IList m_lastSelectedItems = null;
        //private System.Collections.IList m_beforeChangeSelectedItems = null;
        //Wrap the table SelectedItems, from a generic type
        private IList<TrailResultWrapper> SelectedResultWrapper
        {
            set
            {
                IList<TrailResultWrapper> setValue = value;
                if (null == setValue || !setValue.Equals(m_lastSelectedItems))
                {
                    if ((setValue == null || setValue.Count == 0))
                    {
                        //Get all current values in prev selection
                        setValue = TrailResultWrapper.SelectedItems(m_controller.CurrentResultTreeList,
                            TrailResultWrapper.Results(getTrailResultWrapperSelection(m_lastSelectedItems)));
                        if (null == setValue || setValue.Count == 0)
                        {
                            //get a result for the reference activity
                            if (m_controller.ReferenceActivity != null &&
                                this.summaryList.RowData != null)
                            {
                                foreach (TrailResultWrapper tr in (IList<TrailResultWrapper>)this.summaryList.RowData)
                                {
                                    if (m_controller.ReferenceActivity.Equals(tr.Result.Activity))
                                    {
                                        setValue = new List<TrailResultWrapper> { tr };
                                        break;
                                    }
                                }
                            }
                            if (setValue == null && m_controller.CurrentResultTreeList.Count > 0)
                            {
                                //Still no match, use first in list
                                setValue = new List<TrailResultWrapper> { m_controller.CurrentResultTreeList[0] };
                            }
                        }
                    }
                }
                //Set value, let callback update m_prevSelectedItems and refresh chart
#if ST_2_1
                this.summaryList.SelectedChanged -= new System.EventHandler(summaryList_SelectedItemsChanged);
                this.summaryList.Selected  = (List<TrailResultWrapper>)setValue;
                this.summaryList.SelectedChanged += new System.EventHandler(summaryList_SelectedItemsChanged);
#else
                this.summaryList.SelectedItemsChanged -= new System.EventHandler(summaryList_SelectedItemsChanged);
                this.summaryList.SelectedItems = (List<TrailResultWrapper>)setValue;
                this.summaryList.SelectedItemsChanged += new System.EventHandler(summaryList_SelectedItemsChanged);
#endif
                this.m_controller.SelectedResults = TrailResultWrapper.Results(setValue);
                this.SetSummary(setValue);
                //foreach (TrailResultWrapper t in setValue)
                //{
                //    if (t.Result is ChildTrailResult)
                //    {
                //        foreach (TrailResultWrapper tr in (IList<TrailResultWrapper>)this.summaryList.RowData)
                //        {
                //            if (t.Parent.Equals(tr))
                //            {
                //                //TBD expand selections
                //            }
                //        }
                //    }
                //}
            }
            get
            {
#if ST_2_1
                System.Collections.IList SelectedItemsRaw = this.summaryList.Selected;
#else
                System.Collections.IList SelectedItemsRaw = this.summaryList.SelectedItems;
#endif
                return getTrailResultWrapperSelection(SelectedItemsRaw);
            }
        }

        //public IList<TrailResult> SelectedResults
        //{
        //    get
        //    {
        //        return TrailResultWrapper.ParentResults(SelectedResultsWrapper);
        //    }
        //}

        public void EnsureVisible(IList<TrailResult> atr)
        {
            EnsureVisible(TrailResultWrapper.SelectedItems(m_controller.CurrentResultTreeList, atr));
        }
        public void EnsureVisible(IList<TrailResultWrapper> atr)
        {
            if (atr != null && atr.Count > 0)
            {
                this.summaryList.EnsureVisible(atr[0]);
            }
        }

        public System.Windows.Forms.ProgressBar StartProgressBar(int val)
        {
            if (val == 0)
            {
                val = m_controller.OrderedTrails().Count;
            }
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

        private void summaryList_Sort()
        {
            if (m_controller.CurrentActivityTrailIsSelected)
            {
                this.summaryList.SetSortIndicator(TrailsPlugin.Data.Settings.SummaryViewSortColumn,
                    TrailsPlugin.Data.Settings.SummaryViewSortDirection == ListSortDirection.Ascending);

                IList<TrailResultWrapper> atr = m_controller.CurrentResultTreeList;
                ((List<TrailResultWrapper>)(atr)).Sort();
                int i = 1;
                foreach (TrailResultWrapper tr in atr)
                {
                    tr.Result.Order = i;
                    i++;
                    tr.Sort();
                }

                if (atr.Count > 0)
                {
                    atr.Insert(0, this.GetSummary());
                }
#if ST_2_1
                this.summaryList.SelectedChanged -= new System.EventHandler(summaryList_SelectedItemsChanged);
                this.summaryList.RowData = atr;
                this.summaryList.SelectedChanged += new System.EventHandler(summaryList_SelectedItemsChanged);
#else
                this.summaryList.SelectedItemsChanged -= new System.EventHandler(summaryList_SelectedItemsChanged);
                this.summaryList.RowData = atr;
                this.summaryList.SelectedItemsChanged += new System.EventHandler(summaryList_SelectedItemsChanged);
#endif
                this.m_controller.SelectedResults = null;
            }
        }

        public void SetSummary(IList<TrailResultWrapper> selected)
        {
            IList<TrailResultWrapper> selected2 = new List<TrailResultWrapper>();
            if (selected != null)
            {
                foreach (TrailResultWrapper t in selected)
                {
                    if (!t.IsSummary)
                    {
                        selected2.Add(t);
                    }
                }
            }
            if (selected2.Count <= 1)
            {
                //0 or 1 selected, use summary instead
                if (selected2.Count == 0 || !(selected2[0].Result is ChildTrailResult))
                {
                    selected2 = m_controller.CurrentResultTreeList;
                }
                else
                {
                    TrailResultWrapper tr = selected2[0];
                    selected2 = new List<TrailResultWrapper>();
                    foreach (TrailResultWrapper rtn in m_controller.CurrentResultTreeList)
                    {
                        foreach (TrailResultWrapper ctn in rtn.Children)
                        {
                            if (tr.Result.Order == ctn.Result.Order)
                            {
                                selected2.Add(ctn);
                            }
                        }
                    }
                }
            }
            m_summary.SetSummary(selected2);
            //TODO: Splits
        }

        public TrailResultWrapper GetSummary()
        {
            return m_summary;
        }

        bool selectSimilarSplits()
        {
            bool isChange = false;
            IList<TrailResultWrapper> atr = this.SelectedResultWrapper;
            if (Data.Settings.SelectSimilarResults && atr != null)
            {
                //The implementation only supports adding new splits not deselecting
                //Note that selecting will scroll, changing offsets why check what is clicked does not work well
                int? lastSplitIndex = null;
                bool isSingleIndex = false;
                IList<TrailResultWrapper> results = new List<TrailResultWrapper>();
                foreach (TrailResultWrapper t in atr)
                {
                    int splitIndex = -1; //Index for parent, not for child(subsplit)
                    if (t.Result is SummaryTrailResult)
                    {
                        //Summary, always parent
                        results.Add(t);
                    }
                    else if (t.Parent != null)
                    {
                        splitIndex = t.Result.Order;
                    }
                    isSingleIndex = (lastSplitIndex == null || lastSplitIndex == splitIndex) ? true : false;
                    lastSplitIndex = splitIndex;
                    foreach (TrailResultWrapper rtn in m_controller.CurrentResultTreeList)
                    {
                        if (splitIndex < 0)
                        {
                            if (!results.Contains(rtn))
                            {
                                results.Add(rtn);
                            }
                        }
                        else
                        {
                            foreach (TrailResultWrapper ctn in rtn.Children)
                            {
                                if (ctn.Result.Order == splitIndex)
                                {
                                    if (!results.Contains(ctn))
                                    {
                                        results.Add(ctn);
                                    }
                                }
                            }
                        }
                    }
                }

                if (results != this.SelectedResultWrapper)
                {
                    this.SelectedResultWrapper = results;
                    isChange = true;
                }

                //If a single index is selected, let the reference follow the current result
                if (isSingleIndex && this.m_controller.ReferenceTrailResult != null)
                {
                    if (this.m_controller.ReferenceTrailResult.Order != lastSplitIndex)
                    {
                        if (lastSplitIndex < 0)
                        {
                            //This should be a child(subsplit)
                            if (this.m_controller.ReferenceTrailResult is ChildTrailResult)
                            {
                                this.m_controller.ReferenceTrailResult = (this.m_controller.ReferenceTrailResult as ChildTrailResult).ParentResult;
                            }
                            isChange = true;
                        }
                        else
                        {
                            TrailResult rtr;
                            if (!(this.m_controller.ReferenceTrailResult is ChildTrailResult))
                            {
                                rtr = this.m_controller.ReferenceTrailResult;
                            }
                            else
                            {
                                rtr = (this.m_controller.ReferenceTrailResult as ChildTrailResult).ParentResult;
                            }
                            IList<TrailResult> trs = new List<TrailResult> { rtr };

                            IList<TrailResultWrapper> atrp = new List<TrailResultWrapper>();
                            foreach (TrailResultWrapper t2 in TrailResultWrapper.SelectedItems(m_controller.CurrentResultTreeList, trs))
                            {
                                atrp.Add(t2);
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
            return isChange;
        }

        void excludeSelectedResults(bool invertSelection)
        {
            IList<TrailResultWrapper> atr = this.SelectedResultWrapper;
            if (atr != null && atr.Count > 0 &&
                m_controller.CurrentResultTreeList.Count > 0)
            {
                foreach (ActivityTrail at in m_controller.CurrentActivityTrails)
                {
                    //Note: If more than one selected, will try to match all
                    at.Remove(atr, invertSelection);
                }
                m_page.RefreshData(false);
                m_page.RefreshControlState();
            }
        }

        void selectAll()
        {
            IList<TrailResultWrapper> all = new List<TrailResultWrapper>();
            if (m_controller.CurrentActivityTrailIsSelected)
            {
                foreach (TrailResultWrapper t in m_controller.CurrentResultTreeList)
                {
                    if (!t.IsSummary)
                    {
                        all.Add(t);
                    }
                }
            }
            this.SelectedResultWrapper = all;
            if (Data.Settings.ShowOnlyMarkedOnRoute)
            {
                this.m_page.RefreshRoute(false);
            }
            this.m_page.RefreshChart();
        }

        void copyTable()
        {
            this.summaryList.CopyTextToClipboard(true, System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator);
        }

        void selectSimilarSplitsChanged()
        {
            TrailsPlugin.Data.Settings.SelectSimilarResults = !Data.Settings.SelectSimilarResults;
            if (selectSimilarSplits())
            {
                this.m_page.RefreshChart();
            }
            if (Data.Settings.ShowOnlyMarkedOnRoute)
            {
                this.m_page.RefreshRoute(false);
            }
            RefreshControlState();
        }

        void selectWithUR()
        {
            if (Integration.UniqueRoutes.UniqueRouteIntegrationEnabled && m_controller.ReferenceActivity != null)
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
                        IList<IActivity> allActivities = new List<IActivity>();
                        foreach (IActivity activity in m_controller.Activities)
                        {
                            allActivities.Add(activity);
                        }
                        foreach (IActivity activity in similarActivities)
                        {
                            if (!m_controller.Activities.Contains(activity))
                            {
                                allActivities.Add(activity);
                            }
                        }
                        //Set activities, keep trail/selection
                        m_controller.Activities = allActivities;
                        m_page.RefreshData(false);
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
            if (Integration.UniqueRoutes.UniqueRouteIntegrationEnabled && m_controller.ReferenceActivity != null)
            {
                IList<IActivity> activities = new List<IActivity>();
                foreach (TrailResultWrapper t in this.SelectedResultWrapper)
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
                    foreach (TrailResultWrapper tr in this.SelectedResultWrapper)
                    {
                        if (m_controller.ReferenceActivity != tr.Result.Activity &&
                            commonStretches.ContainsKey(tr.Result.Activity) &&
                            commonStretches[tr.Result.Activity] != null)
                        {
                            aTrm.Add(new TrailResultMarked(tr.Result, commonStretches[tr.Result.Activity][0].MarkedTimes));
                        }
                    }
                }
                StopProgressBar();

                //Mark route and chart
                m_page.MarkTrack(aTrm, true, true);
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
                    row = this.summaryList.RowHitTest(((MouseEventArgs)e).Location, out hit);
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
                            cs.Selected = tr.ResultColor.LineNormal;
                            m_ColorSelectorResult = tr;
                            cs.ItemSelected += new ColorSelectorPopup.ItemSelectedEventHandler(cs_ItemSelected);
                            cs.Show();
                        }
                        else if (tr != null)
                        {
                            bool isMatch = false;
                            foreach (TrailResultWrapper t in this.SelectedResultWrapper)
                            {
                                if (t.Result == tr)
                                {
                                    //Ignore clicking on summary, route is updated and no specific marking in chart
                                    if (!(tr is SummaryTrailResult))
                                    {
                                        //if (!TrailResultWrapper.ParentResults(getTrailResultWrapperSelection(m_beforeChangeSelectedItems)).Contains(tr))
                                        {
                                             isMatch = true;
                                        }
                                    }
                                    break;
                                }
                            }
                            if (isMatch)
                            {
                                IList<TrailResult> aTr = new List<TrailResult>();
                                //if (TrailsPlugin.Data.Settings.SelectSimilarResults)
                                {
                                    //Select the single row only
                                    aTr.Add(tr);
                                }
                                //else
                                //{
                                //    //The user can control what is selected - mark all
                                //    aTr = new List<TrailResult>{tr};
                                //}
                                bool markChart = false;
                                if (tr.CompareTo(this.m_lastSelectedTrailResult) == 0 &&
                                    this.SelectedResultWrapper.Count > 1)
                                {
                                    markChart = true;
                                }
                                else
                                {
                                    //foreach (TrailResultWrapper t in SelectedItemsWrapper)
                                    //{
                                    //    foreach (TrailResultWrapper t2 in SelectedItemsWrapper)
                                    //    {
                                    //        if (t != t2 && t.Result.ResultColor == t2.Result.ResultColor)
                                    //        {
                                    //            markChart = true;
                                    //            break;
                                    //        }
                                    //    }
                                    //}
                                    this.m_lastSelectedTrailResult = tr;
                                }
                                m_page.MarkTrack(TrailResultMarked.TrailResultMarkAll(aTr), markChart, false);
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
                row = this.summaryList.RowHitTest(((MouseEventArgs)e).Location, out hit);
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
                if (cs.Selected != m_ColorSelectorResult.ResultColor.LineNormal)
                {
                    m_ColorSelectorResult.ResultColor = new Utils.ChartColors(cs.Selected);
                    //Refresh in the case columns are sorted on the color...
                    m_page.RefreshData(false);
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
            this.summaryList_ColumnHeaderMouseClick(sender, e.Column);
        }

        private void summaryList_ColumnHeaderMouseClick(object sender, TreeList.Column e)
        {
            if (TrailsPlugin.Data.Settings.SummaryViewSortColumn == e.Id)
            {
                TrailsPlugin.Data.Settings.SummaryViewSortDirection = TrailsPlugin.Data.Settings.SummaryViewSortDirection == ListSortDirection.Ascending ?
                       ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            TrailsPlugin.Data.Settings.SummaryViewSortColumn = e.Id;
            this.summaryList_Sort();
        }

        void summaryList_SelectedItemsChanged(object sender, System.EventArgs e)
        {
            this.m_controller.SelectedResults = TrailResultWrapper.Results(this.SelectedResultWrapper);

            bool isChange;
            if (Data.Settings.SelectSimilarResults)
            {
                isChange = selectSimilarSplits();
            }
            else
            {
                //Always assume change
                isChange = true;
            }
            if (this.m_controller.CurrentActivityTrailIsSelected)
            {
                this.SetSummary(this.SelectedResultWrapper);
                TrailResultWrapper t = this.GetSummary();
                if (t != null)
                {
                    this.summaryList.RefreshElements(new List<TrailResultWrapper> { t });
                }
            }
            if (isChange)
            {
                this.m_page.RefreshChart();
            }
            //Trails track display update
            if (Data.Settings.ShowOnlyMarkedOnRoute)
            {
                this.m_page.RefreshRoute(false);
            }

            //Save items selected before the change (needed by ClickRow if several are marked)
            //m_beforeChangeSelectedItems = m_lastSelectedItems;
            //Save previous items for selecting at updates of results
            this.m_lastSelectedItems = this.summaryList.SelectedItems;
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
                this.addActivityFromCategory(this.getCurrentCategory(InsertCategoryTypes.CurrentCategory));
            }
        }

        private void selectResultsWithDeviceElevation()
        {
            IList<TrailResultWrapper> selected = new List<TrailResultWrapper>();
            foreach (TrailResultWrapper trw in m_controller.CurrentResultTreeList)
            {
                if (trw.Result.DeviceElevationTrack0() != null && trw.Result.DeviceElevationTrack0().Count > 0)
                {
                    selected.Add(trw);
                }
            }
            this.SelectedResultWrapper = selected;
            if (Data.Settings.ShowOnlyMarkedOnRoute)
            {
                this.m_page.RefreshRoute(false);
            }
            this.m_page.RefreshChart();
        }

        private enum InsertCategoryTypes { CurrentCategory, SelectedTree, All };
        IActivityCategory getCurrentCategory(InsertCategoryTypes addAll)
        {
            IActivityCategory cat = null;
            if (addAll == InsertCategoryTypes.SelectedTree && m_controller.ReferenceActivity != null)
            {
                cat = m_controller.ReferenceActivity.Category;
                IActivityCategory cat0 = cat.Parent;
                while (cat0.Parent != null)
                {
                    cat = cat0;
                    cat0 = cat0.Parent;
                }
            }
            else if (addAll == InsertCategoryTypes.CurrentCategory)
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
            return cat;
        }

        void addActivityFromCategory(IActivityCategory cat)
        {
            IList<IActivity> allActivities = new List<IActivity>();
            foreach (IActivity activity in m_controller.Activities)
            {
                allActivities.Add(activity);
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
            //Set activities, keep trail/selection
            m_controller.Activities = allActivities;
            m_page.RefreshData(false);
            m_page.RefreshControlState();
        }

        void addCurrentTime()
        {
            IList<IActivity> allActivities = new List<IActivity>();
            IList<IActivity> addActivities = new List<IActivity>();
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
                        addActivities.Add(activity);
                    }
                }
            }
            //Set activities, keep trail/selection
            m_controller.Activities = allActivities;
            m_page.RefreshData(false);
            m_page.RefreshControlState();
            //if (m_controller.ReferenceTrailResult != null && addActivities.Count>0)
            //{
            //    m_controller.ReferenceTrailResult.SameTimeActivities = new List<IActivity>();
            //    foreach (IActivity activity in addActivities)
            //    {
            //        m_controller.ReferenceTrailResult.SameTimeActivities.Add(activity);
            //    }
            //    m_controller.ReferenceTrailResult.Clear(true);
            //}
        }

        private void setAdjustDiffSplitTimesPopup()
        {
            //Cannot use ST controls for most part here
            System.Windows.Forms.Form p = new System.Windows.Forms.Form();
            p.Size = new System.Drawing.Size(370, 105);
            ZoneFiveSoftware.Common.Visuals.Panel pa = new ZoneFiveSoftware.Common.Visuals.Panel();
            ZoneFiveSoftware.Common.Visuals.TextBox AdjustDiffSplitTimes_TextBox = new ZoneFiveSoftware.Common.Visuals.TextBox();
            System.Windows.Forms.Button b = new System.Windows.Forms.Button();
            System.Windows.Forms.Button c = new System.Windows.Forms.Button();
            p.Text = string.Format("Diff Adjust: dist ({0}); timeOffset (s)", GpsRunningPlugin.Util.UnitUtil.Distance.LabelAbbrAct(m_controller.ReferenceActivity));
            p.Controls.Add(pa);
            pa.Dock = DockStyle.Fill;
            pa.Controls.Add(AdjustDiffSplitTimes_TextBox);
            pa.Controls.Add(b);
            pa.Controls.Add(c);
            p.AcceptButton = b;
            p.CancelButton = c;
            b.Location = new System.Drawing.Point(p.Size.Width - 25 - b.Size.Width, p.Height - 40 - b.Height);
            b.DialogResult = DialogResult.OK;
            c.Location = new System.Drawing.Point(p.Size.Width - 25 - b.Size.Width - 15 - c.Size.Width, p.Height - 40 - c.Height);
            c.DialogResult = DialogResult.Cancel;
            b.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionOk;
            c.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionCancel;
            pa.ThemeChanged(this.m_visualTheme);
            //p.ThemeChanged(this.m_visualTheme);
            AdjustDiffSplitTimes_TextBox.ThemeChanged(this.m_visualTheme);
            AdjustDiffSplitTimes_TextBox.Width = p.Width - 37;
            AdjustDiffSplitTimes_TextBox.Location = new System.Drawing.Point(10, 10);

            b.Click +=
                delegate(object sender2, EventArgs args)
                {
                    try
                    {
                        string[] values = AdjustDiffSplitTimes_TextBox.Text.Split(';');
                        float[,] splitTimes = new float[(1 + values.Length) / 2, 2];
                        int i = 0;
                        foreach (string column in values)
                        {
                            float f = 0;
                            if (!string.IsNullOrEmpty(column))
                            {
                                f = TrailsPlugin.Data.Settings.parseFloat(column);
                            }
                            if (i % 2 == 0)
                            {
                                f = (float)GpsRunningPlugin.Util.UnitUtil.Distance.ConvertTo(f, m_controller.ReferenceActivity);
                            }
                            splitTimes[i / 2, i % 2] = f;
                            i++;
                        }
                        if (splitTimes == null || splitTimes.Length == 0 ||
                            splitTimes.Length == 2 && splitTimes[0, 0] == 0 && splitTimes[0, 1] == 0)
                        {
                            //empty is null
                            TrailsPlugin.Data.Settings.AdjustDiffSplitTimes = null;
                        }
                        else
                        {
                            TrailsPlugin.Data.Settings.AdjustDiffSplitTimes = splitTimes;
                        }
                    }
                    catch { }
                };

            String colText = "";
            if (TrailsPlugin.Data.Settings.AdjustDiffSplitTimes != null)
            {
                for (int i = 0; i < TrailsPlugin.Data.Settings.AdjustDiffSplitTimes.Length; i++)
                {
                    float f = TrailsPlugin.Data.Settings.AdjustDiffSplitTimes[i / 2, i % 2];
                    if (i % 2 == 0)
                    {
                        f = (float)GpsRunningPlugin.Util.UnitUtil.Distance.ConvertFrom(f);
                    }
                    if (colText == "") { colText = f.ToString(); }
                    else { colText += ";" + f; }
                }
            }
            AdjustDiffSplitTimes_TextBox.Text = colText;

            //update is done in clicking OK/Enter
            p.ShowDialog();
        }

        private TrailResult GetSelectedTrailResults()
        {
            TrailResult tr = null;
            IList<TrailResultWrapper> atr = this.SelectedResultWrapper;
            if (atr != null)
            {
                if (atr.Count == 1)
                {
                    //One selected use (regardless if summary/regular)
                    tr = atr[0].Result;
                }
                else
                {
                    //More than one or 0: use summary
                    foreach (TrailResultWrapper t in atr)
                    {
                        if (t.Result is SummaryTrailResult)
                        {
                            tr = t.Result;
                            break;
                        }
                    }
                }
            }
            if (tr == null && m_controller.CurrentActivityTrailIsSelected)
            {
                tr = this.GetSummary().Result;
            }

            return tr;
        }

        void PerformancePredictorPopup()
        {
            if (PerformancePredictor.PerformancePredictorIntegrationEnabled)
            {
                TrailResult tr = GetSelectedTrailResults();
                IList<IActivity> activities;
                if (tr != null)
                {
                    if (tr is SummaryTrailResult)
                    {
                        activities = ((SummaryTrailResult)tr).Activities;
                    }
                    else
                    {
                        activities = new List<IActivity> { tr.Activity };
                    }
                    PerformancePredictor.PerformancePredictorPopup(activities, m_view, tr.Duration, tr.Distance, null);
                }
            }
        }

        void HighScorePopup()
        {
            if (HighScore.HighScoreIntegrationEnabled)
            {
                TrailResult tr = GetSelectedTrailResults();
                IList<IActivity> activities = new List<IActivity>();
                IList<IValueRangeSeries<DateTime>> pauses = new List<IValueRangeSeries<DateTime>>();
                if (tr != null)
                {
                    if (tr is SummaryTrailResult)
                    {
                        foreach (TrailResult t in ((SummaryTrailResult)tr).Results)
                        {
                            //Only add one activity, PP only uses that
                            if (activities.Count == 0)
                            {
                                activities.Add(t.Activity);
                            }
                            pauses.Add(t.ExternalPauses);
                        }
                    }
                    else
                    {
                        activities.Add(tr.Activity);
                        pauses.Add(tr.ExternalPauses);
                    }
                    HighScore.HighScorePopup(activities, pauses, m_view, null);
                }
            }
        }

        /*************************************************************************/
        void summaryList_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                this.excludeSelectedResults(e.Modifiers == Keys.Shift);
            }
            else if (e.KeyCode == Keys.Space)
            {
                this.selectSimilarSplitsChanged();
            }
            else if (e.KeyCode == Keys.A)
            {
                if (e.Modifiers == Keys.Control)
                {
                    this.selectAll();
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
                    this.m_page.RefreshData(true);
                }
            }
            else if (e.KeyCode == Keys.C)
            {
                if (e.Modifiers == Keys.Control)
                {
                    this.copyTable();
                }
                else if (e.Modifiers == Keys.Shift)
                {
                    TrailsPlugin.Data.Settings.DiffUsingCommonStretches = !TrailsPlugin.Data.Settings.DiffUsingCommonStretches;
                    this.m_page.RefreshData(true);
                }
                else
                {
                    this.markCommonStretches();
                }
            }
            else if (e.KeyCode == Keys.E)
            {
                if (e.Modifiers == (Keys.Shift | Keys.Control))
                {
                    TrailsPlugin.Data.Settings.UseTrailElevationAdjust = !TrailsPlugin.Data.Settings.UseTrailElevationAdjust;
                    this.m_page.RefreshData(true);
                    this.m_page.RefreshChart();
                }
                else if (e.Modifiers == Keys.Control)
                {
                    this.selectResultsWithDeviceElevation();
                }
                else if (e.Modifiers == Keys.Alt)
                {
                    TrailsPlugin.Data.Settings.DeviceElevationFromOther = !TrailsPlugin.Data.Settings.DeviceElevationFromOther;
                    this.m_page.RefreshData(true);
                    this.m_page.RefreshChart();
                }
                else if (e.Modifiers == Keys.Shift)
                {
                    TrailsPlugin.Data.Settings.UseDeviceElevationForCalc = !TrailsPlugin.Data.Settings.UseDeviceElevationForCalc;
                    this.m_page.RefreshData(true);
                    this.m_page.RefreshChart();
                }
            }
            else if (e.KeyCode == Keys.F)
            {
                //Unofficial shortcuts
                if (e.Modifiers == Keys.Control)
                {
                    if (this.m_controller.ReferenceTrailResult != null)
                    {
                        IList<TrailResultWrapper> atr = this.SelectedResultWrapper;
                        if (atr != null && atr.Count > 0 &&
                            m_controller.CurrentResultTreeList.Count > 0)
                        {
                            foreach (TrailResultWrapper trw in atr)
                            {
                                trw.Result.SetDeviceElevation(TrailsPlugin.Data.Settings.UseTrailElevationAdjust);
                            }
                            m_page.RefreshData(true);
                        }
                    }
                }
                else if (e.Modifiers == Keys.Alt)
                {
                    //Put alternatively calculated grade in Cadence
                    TrailResult.CalculateGrade = !TrailResult.CalculateGrade;
                    m_page.RefreshData(true);
                    this.m_page.RefreshChart();
                }
            }
            else if (e.KeyCode == Keys.I)
            {
                InsertCategoryTypes c = InsertCategoryTypes.CurrentCategory;
                if (e.Modifiers == Keys.Shift)
                {
                    c = InsertCategoryTypes.All;
                }
                else if (e.Modifiers == Keys.Control)
                {
                    c = InsertCategoryTypes.SelectedTree;
                }

                this.addActivityFromCategory(this.getCurrentCategory(c));
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
                this.m_page.RefreshData(true);
            }
            else if (e.KeyCode == Keys.O)
            {
                if (e.Modifiers == Keys.Control)
                {
                    TrailsPlugin.Data.Settings.ResultSummaryStdDev = !TrailsPlugin.Data.Settings.ResultSummaryStdDev;
                }
                else
                {
                    TrailsPlugin.Data.Settings.StartDistOffsetFromStartPoint = !TrailsPlugin.Data.Settings.StartDistOffsetFromStartPoint;
                }
                this.summaryList.Refresh();
                //Only in table, no need to refresh
            }
            else if (e.KeyCode == Keys.P)
            {
                //In context menu, not documented, to be removed?
                //PerformancePredictorPopup();
                this.HighScorePopup();
            }
            else if (e.KeyCode == Keys.Q)
            {
                if (e.Modifiers == (Keys.Shift | Keys.Control))
                {
                    TrailResult.PaceTrackIsGradeAdjustedPaceAvg = !TrailResult.PaceTrackIsGradeAdjustedPaceAvg;
                }
                else if (e.Modifiers == Keys.Control)
                {
                    TrailResult.diffToSelf = !TrailResult.diffToSelf;
                }
                else if (e.Modifiers == Keys.Alt)
                {
                    this.setAdjustDiffSplitTimesPopup();
                }
                else if (e.Modifiers == Keys.Shift)
                {
                    TrailResult.ResetRunningGradeCalcMethod();
                }
                else
                {
                    TrailResult.IncreaseRunningGradeCalcMethod();
                }
                this.summaryListToolTip.Show(Data.Settings.RunningGradeAdjustMethod.ToString(),
                              this.summaryList,
                              new System.Drawing.Point(this.summaryListCursorLocationAtMouseMove.X +
                                  Cursor.Current.Size.Width / 2,
                                        this.summaryListCursorLocationAtMouseMove.Y),
                              this.summaryListToolTip.AutoPopDelay);
                this.m_page.RefreshData(true);
            }
            else if (e.KeyCode == Keys.R)
            {
                //Debugging, like test trail calculation time
                if (e.Modifiers == Keys.Alt)
                {
                    this.m_controller.Clear(false);
                    this.m_page.RefreshData(false);
                }
                else
                {
                    if (e.Modifiers == Keys.Control)
                    {
                        ActivityCache.ClearActivityCache();
                    }

                    bool allRefresh = e.Modifiers == Keys.Shift;
                    int progressCount = 0;
                    if (!allRefresh)
                    {
                        progressCount = m_controller.Activities.Count;
                        this.m_controller.CurrentReset(false);
                    }
                    else
                    {
                        this.m_controller.Reset();
                    }

                    System.Windows.Forms.ProgressBar progressBar = StartProgressBar(progressCount);
                    this.m_controller.ReCalcTrails(allRefresh, progressBar);
                    this.m_page.RefreshData(false);
                    StopProgressBar();
                }
            }
            else if (e.KeyCode == Keys.S)
            {
                if (e.Modifiers == Keys.Control)
                {
                    TrailsPlugin.Data.Settings.ResultSummaryIsDevice = !TrailsPlugin.Data.Settings.ResultSummaryIsDevice;
                    //This is all dynamic, but we want to retrigger sort
                    this.m_page.RefreshData(false);
                }
                else
                {
                    if (e.Modifiers == Keys.Shift)
                    {
                        this.m_page.SetResultListHeight -= cResultListHeight;
                    }
                    else
                    {
                        this.m_page.SetResultListHeight += cResultListHeight;
                    }
                }
            }
            else if (e.KeyCode == Keys.T)
            {
                if (e.Modifiers == Keys.Shift)
                {
                    this.addCurrentTime();
                }
                else if (e.Modifiers == Keys.Control)
                {
                    TrailResult.m_diffOnDateTime = !TrailResult.m_diffOnDateTime;
                    this.m_page.RefreshData(true);
                }
            }
            else if (e.KeyCode == Keys.U)
            {
                this.selectWithUR();
            }
            else if (e.KeyCode == Keys.Z)
            {
                if (e.Modifiers == Keys.Control)
                {
                    Data.Settings.ZoomToSelection = !Data.Settings.ZoomToSelection;
                }
                else if (e.Modifiers == Keys.Shift)
                {
                    Data.Settings.ShowOnlyMarkedOnRoute = !Data.Settings.ShowOnlyMarkedOnRoute;
                    this.m_page.RefreshData(false);
                }
                else
                {
                    //Zoom to selected parts
                    this.m_page.ZoomMarked();
                }
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

        private void summaryList_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            m_mouseClickArgs = e;
            TreeList.RowHitState rowHitState;
            TrailResultWrapper entry = (TrailResultWrapper)this.summaryList.RowHitTest(e.Location, out rowHitState);
            if (entry == this.summaryListLastEntryAtMouseMove)
                return;
            else
                this.summaryListToolTip.Hide(this.summaryList);
            this.summaryListLastEntryAtMouseMove = entry;
            this.summaryListCursorLocationAtMouseMove = e.Location;

            if (entry != null)
                this.summaryListToolTipTimer.Start();
            else
                this.summaryListToolTipTimer.Stop();
        }
        private void summaryList_MouseLeave(object sender, EventArgs e)
        {
            this.summaryListToolTipTimer.Stop();
            this.summaryListToolTip.Hide(this.summaryList);
        }

        private void ToolTipTimer_Tick(object sender, EventArgs e)
        {
            this.summaryListToolTipTimer.Stop();

            if (this.summaryListLastEntryAtMouseMove != null &&
                this.summaryListCursorLocationAtMouseMove != null &&
                !this.summaryListTooltipDisabled)
            {
                string tt = this.summaryListLastEntryAtMouseMove.Result.ToolTip;
                this.summaryListToolTip.Show(tt,
                              this.summaryList,
                              new System.Drawing.Point(this.summaryListCursorLocationAtMouseMove.X +
                                  Cursor.Current.Size.Width / 2,
                                        this.summaryListCursorLocationAtMouseMove.Y),
                              this.summaryListToolTip.AutoPopDelay);
            }
        }

        private TrailResult getMouseResult(bool ensureParent)
        {
            TrailResult tr = null;
            if (m_mouseClickArgs != null)
            {
                object row = null;
                TreeList.RowHitState dummy;
                row = this.summaryList.RowHitTest(m_mouseClickArgs.Location, out dummy);
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
            string currRes = "none"; //TBD
            TrailResult tr = getMouseResult(false);
            if (tr != null && !(tr is SummaryTrailResult))
            {
                currRes = tr.StartTime.ToLocalTime().ToString();
            }

            string refRes = "";
            if (m_controller.ReferenceTrailResult != null)
            {
                refRes = m_controller.ReferenceTrailResult.StartTime.ToLocalTime().ToString();
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
            if (m_controller.CurrentActivityTrailIsSelected)
            {
                this.addInBoundActivitiesMenuItem.Enabled = false;
                foreach (ActivityTrail at in m_controller.CurrentActivityTrails)
                {
                    this.addInBoundActivitiesMenuItem.Enabled |= at.CanAddInbound;
                }
            }
            if (tr == null || tr.Activity == null)
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
                this.markCommonStretchesMenuItem.Enabled = true && Integration.UniqueRoutes.UniqueRouteIntegrationEnabled;
            }
            this.runGradeAdjustMenuItem.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelGrade + ": " + Data.Settings.RunningGradeAdjustMethod.ToString();
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
            dialog.AvailableColumns = TrailResultColumnIds.ColumnDefs(m_controller.ReferenceActivity, m_controller.Activities.Count, true);
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
                this.m_page.RefreshChart();
            }
        }

        void analyzeMenuItem_DropDownOpened(object sender, System.EventArgs e)
        {
            if (HighScore.HighScoreIntegrationEnabled)
            {
                this.highScoreMenuItem.Enabled = true;
            }
            else
            {
                this.highScoreMenuItem.Enabled = false;
            }

            if (PerformancePredictor.PerformancePredictorIntegrationEnabled)
            {
                this.performancePredictorMenuItem.Enabled = true;
            }
            else
            {
                this.performancePredictorMenuItem.Enabled = false;
            }
        }

        void highScoreMenuItem_Click(object sender, System.EventArgs e)
        {
            HighScorePopup();
        }

        void performancePredictorMenuItem_Click(object sender, System.EventArgs e)
        {
            PerformancePredictorPopup();
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
            IList<TrailResultWrapper> atr = this.SelectedResultWrapper;
            if (atr != null && atr.Count > 0)
            {
                IList<IActivity> aAct = new List<IActivity>();
                foreach (TrailResultWrapper tr in atr)
                {
                    if (tr.Result != null && tr.Result.Activity != null)
                    {
                        aAct.Add(tr.Result.Activity);
                    }
                }
                m_view.SelectionProvider.SelectedItems = (List<IActivity>)aAct;
            }
#endif
        }

        void addInBoundActivitiesMenuItem_Click(object sender, System.EventArgs e)
        {
            if (m_controller.CurrentActivityTrailIsSelected)
            {
                System.Windows.Forms.ProgressBar progressBar = this.StartProgressBar(0);
                m_controller.CurrentReset(false);
                foreach (ActivityTrail t in m_controller.CurrentActivityTrails)
                {
                    t.AddInBoundResult(progressBar);
                }
                this.StopProgressBar();
                m_page.RefreshData(false);
                m_page.RefreshControlState();
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
                IList<IActivity> similarActivities = null;
                if (m_controller.ReferenceActivity != null)
                {
                    System.Windows.Forms.ProgressBar progressBar = StartProgressBar(1);
                    similarActivities = UniqueRoutes.GetUniqueRoutesForActivity(
                        m_controller.ReferenceActivity, m_controller.Activities, progressBar);
                    StopProgressBar();
                }
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

        private void runGradeAdjustMenuItem_Click(object sender, EventArgs e)
        {
            TrailResult.IncreaseRunningGradeCalcMethod();
            m_page.RefreshData(true);
        }
    }
}
