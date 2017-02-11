/*
Copyright (C) 2009 Brendan Doherty
Copyright (C) 2010-2015 Gerhard Olsson

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
using TrailsPlugin.Utils;
using System.Diagnostics;

namespace TrailsPlugin.UI.Activity {
    public partial class ResultListControl : UserControl
    {
        ActivityDetailPageControl m_page;
        private ITheme m_visualTheme;
        private TrailResultWrapper m_summaryTotal;
        private TrailResultWrapper m_summaryAverage;
        private TrailResultWrapper m_lastClickedResult = null;
        private IList<TrailResultWrapper> m_PersistentSelectionResults = new List<TrailResultWrapper>();
        private IList<IItemTrackSelectionInfo> lastSTselectionWhenClick;

#if !ST_2_1
        private IDailyActivityView m_view = null;
#endif

        public ResultListControl()
        {
            InitializeComponent();
            this.m_summaryTotal = new TrailResultWrapper(new SummaryTrailResult(true));
            this.m_summaryAverage = new TrailResultWrapper(new SummaryTrailResult(false));
        }
#if ST_2_1
        public void SetControl(ActivityDetailPageControl page, Object view)
        {
#else
        public void SetControl(ActivityDetailPageControl page, IDailyActivityView view)
        {
            this.m_view = view;
#endif
            this.m_page = page;

            InitControls();
#if ST_2_1
            this.summaryList.SelectedChanged += new System.EventHandler(this.summaryList_SelectedItemsChanged);
#else
            this.summaryList.SelectedItemsChanged += new System.EventHandler(this.SummaryList_SelectedItemsChanged);
#endif
            Controller.TrailController.Instance.SelectedResults = null;
        }

        void InitControls()
        {
            copyTableMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.DocumentCopy16;
#if ST_2_1
            limitActivityMenuItem.Visible = false;
            limitURMenuItem.Visible = false;
#else
            this.listSettingsMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Table16;
            this.insertActivitiesMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.DocumentAdd16;
            this.analyzeMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Analyze16;
            this.advancedMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Analyze16;
            this.excludeResultsMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Delete16;
#endif
            this.highScoreMenuItem.Image = Properties.Resources.Image_16_HighScore;
            this.performancePredictorMenuItem.Image = Properties.Resources.Image_16_PerformancePredictor;

            this.HelpTutorialBtn.CenterImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Information16;
            this.HelpFeaturesBtn.CenterImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Information16;
            this.listSettingsBtn.CenterImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Table16;
            this.insertActivitiesBtn.CenterImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.DocumentAdd16;

            this.summaryList.NumHeaderRows = TreeList.HeaderRows.Two;
            this.summaryList.LabelProvider = new TrailResultLabelProvider();
            this.summaryList.RowDataRenderer = new SummaryRowDataRenderer(this.summaryList);

            //this.progressBar.Visible = false;

            this.selectWithURMenuItem.Enabled = Integration.UniqueRoutes.UniqueRouteIntegrationEnabled;
            this.limitURMenuItem.Enabled = Integration.UniqueRoutes.UniqueRouteIntegrationEnabled;
            this.markCommonStretchesMenuItem.Enabled = Integration.UniqueRoutes.UniqueRouteIntegrationEnabled;
            this.summaryListToolTipTimer.Tick += new System.EventHandler(ToolTipTimer_Tick);
            this.summaryListToolTipDisableTimer.Tick += new System.EventHandler(ToolTipDisableTimer_Tick);
            ShowListToolBar();
        }

        public void UICultureChanged(CultureInfo culture)
        {
            summaryListToolTip.SetToolTip(this.insertActivitiesBtn, Properties.Resources.UI_Activity_List_AddActivities);
            summaryListToolTip.SetToolTip(this.HelpFeaturesBtn, Properties.Resources.UI_Settings_PageControl_linkInformativeUrl_Text);
            summaryListToolTip.SetToolTip(this.HelpTutorialBtn, HelpTutorialBtn_url);

            this.copyTableMenuItem.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionCopy;
            this.listSettingsMenuItem.Text = Properties.Resources.UI_Activity_List_ListSettings;
            this.insertActivitiesMenuItem.Text = Properties.Resources.UI_Activity_List_AddActivities;
            //this.referenceTrailMenuItem.Text = Properties.Resources.UI_Activity_List_ReferenceResult;
            this.analyzeMenuItem.Text = CommonResources.Text.ActionAnalyze;
            this.highScoreMenuItem.Text = Properties.Resources.HighScorePluginName;
            this.performancePredictorMenuItem.Text = Properties.Resources.PerformancePredictorPluginName;
            this.advancedMenuItem.Text = Properties.Resources.UI_Activity_List_Advanced;
            this.excludeResultsMenuItem.Text = Properties.Resources.UI_Activity_List_ExcludeResult;
            this.limitActivityMenuItem.Text = Properties.Resources.UI_Activity_List_LimitSelection;
            //this.limitURMenuItem.Text = string.Format(Properties.Resources.UI_Activity_List_URLimit, "");
            //this.selectWithURMenuItem.Text = string.Format(Properties.Resources.UI_Activity_List_URSelect, "");
            this.markCommonStretchesMenuItem.Text = Properties.Resources.UI_Activity_List_URCommon;
            this.addInBoundActivitiesMenuItem.Text = Properties.Resources.UI_Activity_List_AddInBound;
            //this.addCurrentCategoryMenuItem.Text = Properties.Resources.UI_Activity_List_AddReferenceCategory;
            //this.addTopCategoryMenuItem.Text = Properties.Resources.UI_Activity_List_AddTopCategory;
            this.RefreshColumns();
        }

        public void ThemeChanged(ITheme visualTheme)
        {
            this.m_visualTheme = visualTheme;
            this.summaryList.ThemeChanged(visualTheme);
            this.ButtonPanel.ThemeChanged(visualTheme);
            //this.ButtonPanel.BackColor = visualTheme.Control;
            this.listMenu.Renderer = new ThemedContextMenuStripRenderer(visualTheme);
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

        private bool MultiActivity() {
            return Controller.TrailController.Instance.Activities.Count > 1 || Controller.TrailController.Instance.PrimaryCurrentActivityTrail != null && Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail.TrailType == Trail.CalcType.UniqueRoutes;
        }

        private const int cPlusMinusWidth = 15;
        private void RefreshColumns()
        {
            this.summaryList.Columns.Clear();
            //The first column has the optional '+' that must be added to the size
            int plusMinusSize = this.summaryList.ShowPlusMinus ? cPlusMinusWidth : 0;

            //Permanent fields
            foreach (IListColumnDefinition columnDef in TrailResultColumns.PermanentMultiColumnDefs())
            {
                int width = Data.Settings.ActivityPageColumnsSizeGet(columnDef.Id);
                if (width < 0) { width = columnDef.Width; }
                TreeList.Column column = new TreeList.Column(
                    columnDef.Id,
                    columnDef.Text(columnDef.Id),
                    width + plusMinusSize,
                    columnDef.Align
                );
                this.summaryList.Columns.Add(column);
                plusMinusSize = 0;
            }

            int noResults = 1;
            if (Controller.TrailController.Instance.CurrentActivityTrailIsSelected)
            {
                noResults = Controller.TrailController.Instance.Results.Count;
            }
            bool splits = false;
            if (Controller.TrailController.Instance.PrimaryCurrentActivityTrail != null)
            {
                splits = Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail.IsSplits;
            }
            TrailResultColumns cols = new TrailResultColumns(Controller.TrailController.Instance.ReferenceActivity, noResults, MultiActivity(), splits);

            foreach (string id in Data.Settings.ActivityPageColumns)
            {
                IListColumnDefinition columnDef = cols.ColumnDef(id);
                if (columnDef != null)
                {
                    int width = Data.Settings.ActivityPageColumnsSizeGet(columnDef.Id);
                    if (width < 0) { width = columnDef.Width; }
                    TreeList.Column column = new TreeList.Column(
                        columnDef.Id,
                        columnDef.Text(columnDef.Id),
                        width + plusMinusSize,
                        columnDef.Align
                    );
                    this.summaryList.Columns.Add(column);
                    plusMinusSize = 0;
                }
            }
            this.summaryList.NumLockedColumns = Data.Settings.ActivityPageNumFixedColumns;
        }

        public void RefreshControlState()
        {
            limitActivityMenuItem.Enabled = MultiActivity();
            addCurrentCategoryMenuItem.Checked = Data.Settings.AddCurrentCategory;
        }

        public void RefreshList()
        {
            if (Controller.TrailController.Instance.CurrentActivityTrailIsSelected)
            {
                RefreshColumns();

                if (Controller.TrailController.Instance.Results.Count > Data.Settings.MaxAutoCalcResults)
                {
                    //Avoid sort on some fields that are heavy to calculate at auto updates
                    Controller.TrailController.Instance.AutomaticUpdate = true;
                }
                SummaryList_Sort();
                Controller.TrailController.Instance.AutomaticUpdate = false;
                ((TrailResultLabelProvider)this.summaryList.LabelProvider).MultipleActivities = MultiActivity();

                //Set to previous selection (or default)
                this.SelectedResults = null;
            }
            else
            {
#if ST_2_1
                this.summaryList.SelectedChanged -= new System.EventHandler(summaryList_SelectedItemsChanged);
                summaryList.RowData = null;
                this.summaryList.SelectedChanged += new System.EventHandler(summaryList_SelectedItemsChanged);
#else
                this.summaryList.SelectedItemsChanged -= new System.EventHandler(SummaryList_SelectedItemsChanged);
                this.summaryList.RowData = null;
                this.summaryList.SelectedItemsChanged += new System.EventHandler(SummaryList_SelectedItemsChanged);
#endif
            }
            SummaryPanel_HandleCreated(this.summaryList, null);
        }

        public void ShowListToolBar()
        {
            this.chartTablePanel.RowStyles[0].Height = Data.Settings.ShowListToolBar ? 25 : 0;
        }

        /***********************************************/

        private const int cResultListHeight = 17;//Should be possible to read out from list...
        void SummaryPanel_HandleCreated(object sender, System.EventArgs e)
        {
            if (m_page != null)
            {
                if (m_page.IsPopup)
                {
                    m_page.PopupAdjustSize();
                }
                else
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
            ChartTablePanel_SizeChanged(null, null);
        }

        private IList<TrailResultWrapper> m_lastSelectedItems = null;
        //Wrap the table SelectedItems, from a generic type
        private IList<TrailResultWrapper> SelectedResults
        {
            set
            {
                IList<TrailResultWrapper> setValue = value;
                if (null != setValue)
                {
                    //Explicit selection of results (null is used for previous/defaults)
                    this.m_lastSelectedItems = setValue;
                }
                if (null == setValue || !setValue.Equals(m_lastSelectedItems))
                {
                    if ((setValue == null || setValue.Count == 0))
                    {
                        //Get all current values in prev selection
                        setValue = Controller.TrailController.Instance.UpdateResults(m_lastSelectedItems);
                        if (null == setValue || setValue.Count == 0)
                        {
                            //get a result for the reference activity
                            if (Controller.TrailController.Instance.ReferenceActivity != null &&
                                this.summaryList.RowData != null)
                            {
                                foreach (TrailResultWrapper tr in (IList<TrailResultWrapper>)this.summaryList.RowData)
                                {
                                    if (Controller.TrailController.Instance.ReferenceActivity.Equals(tr.Result.Activity))
                                    {
                                        setValue = new List<TrailResultWrapper> { tr };
                                        break;
                                    }
                                }
                            }
                            if (setValue == null && Controller.TrailController.Instance.Results.Count > 0)
                            {
                                //Still no match, could use first in list
                                //Commenting this out, using Summary
                                //setValue = new List<TrailResultWrapper> { Controller.TrailController.Instance.Results[0] };
                            }
                        }
                    }
                }
#if ST_2_1
                this.summaryList.SelectedChanged -= new System.EventHandler(summaryList_SelectedItemsChanged);
                this.summaryList.Selected  = (List<TrailResultWrapper>)setValue;
                this.summaryList.SelectedChanged += new System.EventHandler(summaryList_SelectedItemsChanged);
#else
                this.summaryList.SelectedItemsChanged -= new System.EventHandler(SummaryList_SelectedItemsChanged);
                this.summaryList.SelectedItems = (List<TrailResultWrapper>)setValue;
                this.summaryList.SelectedItemsChanged += new System.EventHandler(SummaryList_SelectedItemsChanged);
#endif
                this.UpdateSelectedItems(this.SelectedResults);
            }
            get
            {
#if ST_2_1
                System.Collections.IList selectedItemsRaw = this.summaryList.Selected;
#else
                System.Collections.IList selectedItemsRaw = this.summaryList.SelectedItems;
#endif
                return TrailResultWrapper.WrapperIList(selectedItemsRaw);
            }
        }

        public IList<TrailResultWrapper> SpecialSelectionResults
        {
            get
            {
                IList<TrailResultWrapper> srw = this.SelectedResults;
                if (SummaryTrailResult.IsSummarySelection(srw))
                {
                    if (Controller.TrailController.Instance.ReferenceResult != null)
                    {
                        srw = new List<TrailResultWrapper> { Controller.TrailController.Instance.ReferenceResult };
                    }
                    else
                    {
                        srw = new List<TrailResultWrapper>();
                    }
                }
                return srw;
            }
        }

        public void EnsureVisible(IList<TrailResult> atr, bool selectList)
        {
            EnsureVisible(TrailResultWrapper.Results((atr)), selectList);
        }

        public void EnsureVisible(IList<TrailResultWrapper> atr, bool selectList)
        {
            if (atr != null && atr.Count > 0)
            {
                if(selectList)
                {
                    IList<TrailResultWrapper> sels = new List<TrailResultWrapper>();
                    foreach (TrailResultWrapper trw in this.SelectedResults)
                    {
                        sels.Add(trw);
                    }
                    foreach (TrailResultWrapper trw in atr)
                    {
                        if (!sels.Contains(trw))
                        {
                            sels.Add(trw);
                        }
                    }
                    this.SelectedResults = sels;
                }
                this.summaryList.EnsureVisible(atr[0]);
            }
        }

        public System.Windows.Forms.ProgressBar StartProgressBar(int val)
        {
            if (val == 0)
            {
                val = Controller.TrailController.Instance.OrderedTrails().Count;
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
        private void SummaryList_Sort()
        {
            if (Controller.TrailController.Instance.CurrentActivityTrailIsSelected)
            {
                this.summaryList.SetSortIndicator(Data.Settings.SummaryViewSortColumns[0],
                    Data.Settings.SummaryViewSortDirection == ListSortDirection.Ascending);

                IList<TrailResultWrapper> atr = Controller.TrailController.Instance.Results;
                ((List<TrailResultWrapper>)(atr)).Sort();
                int i = 1;
                foreach (TrailResultWrapper tr in atr)
                {
                    tr.Result.Order = i;
                    i++;
                    Boolean expanded = Data.Settings.SelectSimilarSplits || this.summaryList.IsExpanded(tr)
                        //workaround for incorrect expand detection
                        || atr.Count < 9;
                    if (!expanded)
                    {
                        foreach (TrailResultWrapper etr in summaryList.Expanded)
                        {
                            //When inserting the current content of the list are other objects, so compare
                            if (etr.Result.CompareTo(tr.Result) == 0)
                            {
                                expanded = true;
                                break;
                            }
                        }
                    }
                    if (expanded)
                    {
                        //Only sort similar and visible
                        tr.Sort();
                    }
                }

                int resultsInList = atr.Count;
                if (atr.Count > 0)
                {
                    TrailResultWrapper t;
                    t = this.GetSummaryAverage();
                    if (t != null)
                    {
                        atr.Insert(0, t);
                    }
                    t = this.GetSummaryTotal();
                    if (t != null)
                    {
                        atr.Insert(0, t);
                    }
                }
#if ST_2_1
                this.summaryList.SelectedChanged -= new System.EventHandler(summaryList_SelectedItemsChanged);
                this.summaryList.RowData = atr;
                this.summaryList.SelectedChanged += new System.EventHandler(summaryList_SelectedItemsChanged);
#else
                this.summaryList.SelectedItemsChanged -= new System.EventHandler(SummaryList_SelectedItemsChanged);
                this.summaryList.RowData = atr;
                this.summaryList.SelectedItemsChanged += new System.EventHandler(SummaryList_SelectedItemsChanged);
#endif
                if ((this.summaryList.Expanded == null || this.summaryList.Expanded.Count == 0)
                    && resultsInList == 1)
                {
                    this.summaryList.Expanded = new object[1] { atr[atr.Count - 1] };
                }
            }
        }

        private void SetSummary(IList<TrailResultWrapper> selected)
        {
            IList<TrailResultWrapper> selected2 = new List<TrailResultWrapper>();
            if (selected != null)
            {
                foreach (TrailResultWrapper t in selected)
                {
                    if (!(t.Result is SummaryTrailResult) && !(t.Result is PausedChildTrailResult))
                    {
                        selected2.Add(t);
                    }
                }
            }
            if (selected2.Count <= 1)
            {
                //0 or 1 selected, show something "smarter" in summary instead
                if (selected2.Count == 1 &&
                    (selected2[0].Children.Count > 0 ||
                    Controller.TrailController.Instance.Results.Count == 1 &&
                    selected2[0].Result.Wrapper.Parent != null &&
                    ((TrailResultWrapper)selected2[0].Result.Wrapper.Parent).Children.Count > 0))
                {
                    //only one result, show summary for child results
                    IList<TreeList.TreeListNode> clist = selected2[0].Children;
                    if (clist.Count == 0)
                    {
                        clist = ((TrailResultWrapper)selected2[0].Result.Wrapper.Parent).Children;
                    }
                    IList<TrailResultWrapper> list2 = new List<TrailResultWrapper>();
                    foreach (TrailResultWrapper t in clist)
                    {
                        list2.Add(t);
                    }
                    selected2 = list2;
                }
                else if (selected2.Count == 0 || !(selected2[0].Result is ChildTrailResult))
                {
                    //Show summary for all current results
                    selected2 = Controller.TrailController.Instance.Results;
                }
                else
                {
                    //Child is selected, use all similar (numbered) children
                    TrailResultWrapper tr = selected2[0];
                    selected2 = new List<TrailResultWrapper>();
                    foreach (TrailResultWrapper rtn in Controller.TrailController.Instance.Results)
                    {
                        foreach (TrailResultWrapper ctn in rtn.Children)
                        {
                            if (tr.Result.Order == ctn.Result.Order)
                            {
                                selected2.Add(ctn);
                            }
                        }
                    }
                    if (selected2.Count == 1)
                    {
                        //Still 1, use summary
                        selected2 = Controller.TrailController.Instance.Results;
                    }
                }
            }

            if (Data.Settings.ShowSummaryForChildren)
            {
                IList<TrailResultWrapper> selected3 = new List<TrailResultWrapper>();
                foreach (TrailResultWrapper t in selected2)
                {
                    if (t.Result is ParentTrailResult)
                    {
                        foreach (TrailResultWrapper t2 in t.Children)
                        {
                            selected3.Add(t2);
                        }
                    }
                    else
                    {
                        selected3.Add(t);
                    }
                }
                selected2 = selected3;
            }

            if (Data.Settings.ShowSummaryTotal)
            {
                (m_summaryTotal.Result as SummaryTrailResult).SetSummary(selected2);
            }
            if (Data.Settings.ShowSummaryAverage)
            {
                (m_summaryAverage.Result as SummaryTrailResult).SetSummary(selected2);
            }
            RefreshSummary();
        }

        public void RefreshSummary()
        {
            TrailResultWrapper t;
            t = this.GetSummaryTotal();
            if (t != null)
            {
                this.summaryList.RefreshElements(new List<TrailResultWrapper> { t });
            }
            t = this.GetSummaryAverage();
            if (t != null)
            {
                this.summaryList.RefreshElements(new List<TrailResultWrapper> { t });
            }
        }

        private TrailResultWrapper GetSummaryTotal()
        {
            if (Data.Settings.ShowSummaryTotal)
            {
                return m_summaryTotal;
            }
            return null;
        }

        private TrailResultWrapper GetSummaryAverage()
        {
            if (Data.Settings.ShowSummaryAverage)
            {
                return m_summaryAverage;
            }
            return null;
        }

        internal bool SelectSimilarSplits()
        {
            bool isChange = false;
            IList<TrailResultWrapper> atr = this.SelectedResults;
            if (atr != null && atr.Count > 0)
            {
                //The implementation only supports adding new splits not deselecting
                IList<TrailResultWrapper> selectResults = new List<TrailResultWrapper>();
                IList<TrailResult> trailResults = new List<TrailResult>(); //To find what is added
                IList<TrailResultWrapper> searchResults = new List<TrailResultWrapper>();

                foreach (TrailResultWrapper t in atr)
                {
                    //These results are included 
                    selectResults.Add(t);
                    trailResults.Add(t.Result);
                    if (!(t.Result is ChildTrailResult))
                    {
                        searchResults.Add(t);
                    }
                }
                if(searchResults.Count == 0)
                {
                    //No parent selected, search subresults from all results
                    searchResults = Controller.TrailController.Instance.Results;
                }

                foreach (TrailResultWrapper t in atr)
                {
                    //Add matching children
                    if (t.Result is ChildTrailResult)
                    {
                        int splitIndex = t.Result.Order; //Only match on order, not position in list....
                        foreach (TrailResultWrapper rtn in searchResults)
                        {
                            int i = 0;
                            foreach (TrailResultWrapper ctn in TrailResultWrapper.ChildrenTimeSorted(rtn))
                            {
                                if (Data.Settings.SelectSimilarModulu == 0 && ctn.Result.Order == splitIndex ||
                                    Data.Settings.SelectSimilarModulu > 0 && i % Data.Settings.SelectSimilarModulu == (splitIndex - 1) % Data.Settings.SelectSimilarModulu)
                                {
                                    if (!trailResults.Contains(ctn.Result))
                                    {
                                        selectResults.Add(ctn);
                                        trailResults.Add(ctn.Result);
                                    }
                                }
                                i++;
                            }
                        }
                    }
                }

                if (selectResults != this.SelectedResults)
                {
                    this.SelectedResults = selectResults;
                    isChange = true;
                }

                ////If a single index is selected, let the reference follow the current result
                //if (atr.Count == 1)
                //{
                //    TrailResultWrapper refRes;
                //    if (!(atr[0].Result is ChildTrailResult))
                //    {
                //        refRes = atr[0];
                //    }
                //    else
                //    {
                //        refRes = (atr[0].Result as ChildTrailResult).ParentResult.Wrapper;
                //    }
                //    if (refRes != Controller.TrailController.Instance.ReferenceResult)
                //    {
                //        Controller.TrailController.Instance.ReferenceResult = refRes;
                //        isChange = true;
                //    }
                //}
            }
            return isChange;
        }

        private int ExcludeSelectedResults(bool invertSelection)
        {
            IList<TrailResultWrapper> atr = this.SelectedResults;
            if (atr != null && atr.Count > 0 &&
                Controller.TrailController.Instance.Results.Count > 0)
            {
                if (invertSelection)
                {
                    //Only invert if at least one parent
                    //exclude selected parants and the parents to selected children
                    IList<TrailResultWrapper> selected = Controller.TrailController.Instance.Results;
                    bool onlyChildren = true;
                    foreach (TrailResultWrapper tr in atr)
                    {
                        if (tr.Result is ChildTrailResult)
                        {
                            selected.Remove((tr.Result as ChildTrailResult).ParentResult.Wrapper);
                        }
                        else
                        {
                            onlyChildren = false;
                            selected.Remove(tr);
                        }
                    }
                    if (onlyChildren)
                    {
                        //unclear how to handle only children, just selected results?
                        return 0;
                    }
                    atr = selected;
                }
                foreach (TrailResultWrapper tr in atr)
                {
                    tr.Result.ActivityTrail.Remove(tr);
                }
                m_page.RefreshData(false);
                m_page.RefreshControlState();
            }
            return atr.Count;
        }

        void SelectAll()
        {
            IList<TrailResultWrapper> all = new List<TrailResultWrapper>();
            if (Controller.TrailController.Instance.CurrentActivityTrailIsSelected)
            {
                foreach (TrailResultWrapper t in Controller.TrailController.Instance.Results)
                {
                    if (!(t.Result is SummaryTrailResult))
                    {
                        all.Add(t);
                    }
                }
            }
            this.SelectedResults = all;
        }

        void CopyTable()
        {
            this.summaryList.CopyTextToClipboard(true, System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator);
        }

        private int SelectWithUR()
        {
            int res = 0;
            if (Integration.UniqueRoutes.UniqueRouteIntegrationEnabled && Controller.TrailController.Instance.ReferenceActivity != null)
            {
                try
                {
                    IList<IActivity> similarActivities = null;
                    System.Windows.Forms.ProgressBar progressBar = StartProgressBar(1);
                    if (Controller.TrailController.Instance.ReferenceResult != null &&
                        Controller.TrailController.Instance.ReferenceResult.Result.GPSRoute != null)
                    {
                        similarActivities = UniqueRoutes.GetUniqueRoutesForActivity(
                           Controller.TrailController.Instance.ReferenceResult.Result.GPSRoute, null, progressBar);
                    }
                    else if (Controller.TrailController.Instance.ReferenceActivity != null)
                    {
                        similarActivities = UniqueRoutes.GetUniqueRoutesForActivity(
                           Controller.TrailController.Instance.ReferenceActivity.GPSRoute, null, progressBar);
                    }
                    StopProgressBar();
                    if (similarActivities != null)
                    {
                        IList<IActivity> allActivities = new List<IActivity>();
                        foreach (IActivity activity in Controller.TrailController.Instance.Activities)
                        {
                            allActivities.Add(activity);
                        }
                        foreach (IActivity activity in similarActivities)
                        {
                            if (!Controller.TrailController.Instance.Activities.Contains(activity))
                            {
                                allActivities.Add(activity);
                                res++;
                            }
                        }
                        //Set activities, keep trail/selection
                        Controller.TrailController.Instance.Activities = allActivities;
                        m_page.RefreshData(false);
                        m_page.RefreshControlState();
                    }
                }
                catch (Exception ex)
                {
                    MessageDialog.Show(ex.Message, "Plugin error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return res;
        }

        void MarkCommonStretches()
        {
            if (Integration.UniqueRoutes.UniqueRouteIntegrationEnabled && Controller.TrailController.Instance.ReferenceActivity != null)
            {
                IList<IActivity> activities = new List<IActivity>();
                foreach (TrailResultWrapper t in this.SelectedResults)
                {
                    if (t.Result.Activity != null && !activities.Contains(t.Result.Activity))
                    {
                        activities.Add(t.Result.Activity);
                    }
                }
                System.Windows.Forms.ProgressBar progressBar = StartProgressBar(activities.Count);
                IList<TrailResultMarked> aTrm = new List<TrailResultMarked>();
                IDictionary<IActivity, IItemTrackSelectionInfo[]> commonStretches = TrailResult.CommonStretches(Controller.TrailController.Instance.ReferenceActivity, activities, progressBar);
                if (commonStretches != null && commonStretches.Count > 0)
                {
                    foreach (TrailResultWrapper tr in this.SelectedResults)
                    {
                        if (Controller.TrailController.Instance.ReferenceActivity != tr.Result.Activity &&
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

        private static TreeList.Column GetColumn(TreeList l, int eX)
        {
            int epos = 0;
            int colSelected = 0; //Select first by default
            for (int i = 0; i < l.Columns.Count; i++)
            {
                epos += l.Columns[i].Width;
                if (eX < epos)
                {
                    colSelected = i;
                    break;
                }
            }
            return l.Columns[colSelected];
        }

        /************************************************************/
        void SummaryList_Click(object sender, System.EventArgs e)
        {
            //SelectTrack, for ST3
            if (sender is TreeList)
            {
                TreeList l = sender as TreeList;
                MouseEventArgs e2 = (MouseEventArgs)e;
                int xScrolled = e2.X + l.HScrollBar.Value;
                TreeList.Column selectedColumn = GetColumn(l, xScrolled);
                //Check if header. ColumnHeaderClicked will not fire if Click enabled
                if (l.HeaderRowHeight >= e2.Y)
                {
                    SummaryList_ColumnHeaderMouseClick(sender, selectedColumn);
                }
                else
                {
                    object row = this.summaryList.RowHitTest(e2.Location, out TreeList.RowHitState hit);
                    if (row != null && row is TrailResultWrapper)
                    {
                        TrailResultWrapper tr = row as TrailResultWrapper;
                        if (summaryList.Columns[0] == selectedColumn)
                        {
                            //Workaround to sort first when expanding (there is no explicit event when expanding)
                            //For some reason, this do not fully work
                            if (summaryList.IsExpanded(row))
                            {
                                tr.Sort();
                            }
                            else
                            {
                                //If scrolling after expanding, it is likely that a child is selected
                                TrailResultWrapper par = (TrailResultWrapper)(tr).Parent;
                                if (par != null && summaryList.IsExpanded(par))
                                {
                                    par.Sort();
                                }
                            }
                        }

                        //RowHitState is always Row, use position to filter out likely "+" clicks
                        if (selectedColumn.Id == TrailResultColumnIds.ResultColor && hit == TreeList.RowHitState.Row &&
                           (xScrolled > 18 || !(tr.Result is ParentTrailResult)))
                        {
                            ColorSelectorPopup cs = new ColorSelectorPopup()
                            {
                                Width = 70,
                                DesktopLocation = ((Control)sender).PointToScreen(e2.Location),
                                Selected = tr.Result.ResultColor.LineNormal
                            };
                            cs.ThemeChanged(m_visualTheme);
                            cs.ItemSelected += new ColorSelectorPopup.ItemSelectedEventHandler(CS_ItemSelected);
                        m_ColorSelectorResult = tr.Result;
                        cs.Show();
                        }
                        else
                        {
                            bool clickSelected = false;
                            foreach (TrailResultWrapper t in this.SelectedResults)
                            {
                                if (t == tr)
                                {
                                    //Ignore clicking on summary, route is updated and no specific marking in chart
                                    if (!(tr.Result is SummaryTrailResult))
                                    {
                                        clickSelected = true;
                                    }
                                    break;
                                }
                            }
                            if (clickSelected)
                            {
                                lastSTselectionWhenClick = TrailsItemTrackSelectionInfo.SetAndAdjustFromSelectionFromST(
                                    m_view.RouteSelectionProvider.SelectedItems, new List<IActivity> { tr.Result.Activity });

                                IList<TrailResult> aTr = new List<TrailResult>();
                                //if (Data.Settings.SelectSimilarResults)
                                {
                                    //Select the single row only
                                    aTr.Add(tr.Result);
                                }
                                //else
                                //{
                                //    //The user can control what is selected - mark all
                                //    aTr = new List<TrailResult>{tr};
                                //}

                                bool markChart = false;
                                if (this.m_lastClickedResult != null &&
                                    tr.CompareTo(this.m_lastClickedResult) == 0 &&
                                    this.SelectedResults.Count > 1)
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
                                    this.m_lastClickedResult = tr;
                                }
                                m_page.MarkTrack(TrailResultMarked.TrailResultMarkAll(aTr), markChart, false);
                            }
                        }
                    }
                }
            }
        }

        void SummaryList_DoubleClick(object sender, System.EventArgs e)
        {
            if (sender is TreeList)
            {
                TreeList l = sender as TreeList;
                MouseEventArgs e2 = (MouseEventArgs)e;
                object row = this.summaryList.RowHitTest(e2.Location, out TreeList.RowHitState hit);
                if (row != null && hit == TreeList.RowHitState.Row && row is TrailResultWrapper)
                {
                    TrailResultWrapper trw = row as TrailResultWrapper;
                    if (trw != null)
                    {
                        TreeList.Column selectedColumn = GetColumn(l, e2.X + l.HScrollBar.Value);
                        if (selectedColumn.Id == TrailResultColumnIds.LapInfo_Rest && trw.Result.LapInfo != null)
                        {
                            //Unofficial, see also Order below
                            DialogResult popRes = MessageDialog.Show(string.Format("Set to {0}?", !trw.Result.LapInfo.Rest),
                                  "Toggle rest on lap", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (popRes == DialogResult.OK)
                            {
                                trw.Result.LapInfo.Rest = !trw.Result.LapInfo.Rest;
                            }

                            TrailResultWrapper parent = trw.GetParent();
                            if (parent != null)
                            {
                                parent.RefreshChildren();
                            }
                            m_page.RefreshData(true);
                        }
                        else if (selectedColumn.Id == TrailResultColumnIds.MetaData_Source && trw.Result.Activity != null)
                        {
                            if (DialogResult.OK == SetMetaImportSource(trw.Result.Activity))
                            {
                                this.summaryList.RefreshElements(new List<TrailResultWrapper> { trw });
                                m_page.RefreshData(true, true);
                            }
                        }
                        //else if (selectedColumn.Id == TrailResultColumnIds.StartTime)
                        //{
                        //    TBD
                        //}
                        else if (selectedColumn.Id == TrailResultColumnIds.Order)
                        {
                            //Unofficial(?)
                            if (trw.Result.Activity != null)
                            {
                                if (trw.Result is PausedChildTrailResult && (trw.Result as PausedChildTrailResult).pauseType == PauseType.Timer)
                                {
                                    DialogResult popRes = MessageDialog.Show(
                                        string.Format("To remove timer pause select {0}, to set as rest lap select {1}", CommonResources.Text.ActionYes, CommonResources.Text.ActionNo),
                                       "Remove Pause", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                                    if (popRes == DialogResult.Yes || popRes == DialogResult.No)
                                    {
                                        TrackUtil.removePause(trw.Result.Activity.TimerPauses, trw.Result.StartTime, trw.Result.EndTime);
                                        TrackUtil.removePause(trw.Result.Pauses, trw.Result.StartTime, trw.Result.EndTime);
                                        if (popRes == DialogResult.No)
                                        {
                                            InsertRestLap(trw.Result.Activity.Laps, trw.Result.StartTime, trw.Result.Duration);
                                        }
                                    }
                                }
                                else if (trw.Result.LapInfo != null)
                                {
                                    DialogResult popRes = MessageDialog.Show(
                                         string.Format("To set split as timer pause select {0}, to set rest to {2} select {1}", 
                                         CommonResources.Text.ActionYes, CommonResources.Text.ActionNo, !trw.Result.LapInfo.Rest),
                                          "Update Split", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                                    if (popRes == DialogResult.Yes)
                                    {
                                        trw.Result.Activity.TimerPauses.Add(new ValueRange<DateTime>(trw.Result.StartTime, trw.Result.EndTime));
                                    }
                                    else if (popRes == DialogResult.No)
                                    {
                                        trw.Result.LapInfo.Rest = !trw.Result.LapInfo.Rest;
                                    }
                                }
                                else if (trw.Result is ChildTrailResult)
                                {
                                    DialogResult popRes = MessageDialog.Show(
                                         string.Format("To set result as timer pause select {0}, to set as split select {1}",
                                         CommonResources.Text.ActionYes, CommonResources.Text.ActionNo),
                                          "Update Result", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                                    if (popRes == DialogResult.Yes)
                                    {
                                        trw.Result.Activity.TimerPauses.Add(new ValueRange<DateTime>(trw.Result.StartTime, trw.Result.EndTime));
                                    }
                                    else if (popRes == DialogResult.No)
                                    {
                                        InsertRestLap(trw.Result.Activity.Laps, trw.Result.StartTime, trw.Result.Duration);
                                    }
                                }
                                else if (trw.Result is ParentTrailResult)
                                {
                                    //TBD last selection?
                                    //IList<IItemTrackSelectionInfo> selectedGPS = TrailsItemTrackSelectionInfo.SetAndAdjustFromSelectionFromST(
                                    //    m_view.RouteSelectionProvider.SelectedItems, new List<IActivity> { trw.Result.Activity });
                                    if (TrailsItemTrackSelectionInfo.ContainsData(lastSTselectionWhenClick))
                                    {
                                        DialogResult popRes = MessageDialog.Show(
                                            string.Format("To set selection as timer pause select {0}, to set as split select {1}",
                                            CommonResources.Text.ActionYes, CommonResources.Text.ActionNo),
                                            "Update Result", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                                        foreach (IValueRange<DateTime> v in lastSTselectionWhenClick[0].MarkedTimes)
                                        {
                                            if (popRes == DialogResult.Yes)
                                            {
                                                trw.Result.Activity.TimerPauses.Add(v);
                                            }
                                            else if (popRes == DialogResult.No)
                                            {
                                                InsertRestLap(trw.Result.Activity.Laps, v.Lower, v.Upper - v.Lower);
                                            }
                                        }
                                    }
                                    else
                                    { }
                                }
                            }

                            TrailResultWrapper parent = trw.GetParent();
                            if (parent != null)
                            {
                                parent.RefreshChildren();
                            }
                            m_page.RefreshData(true);
                        }
                        else if (trw.Result.Activity != null && trw.Result is ParentTrailResult)
                        {
                            Guid view = GUIDs.DailyActivityView;
                            Controller.TrailController.Instance.ReferenceResult = trw;
                            string bookmark = "id=" + trw.Result.Activity;
                            Plugin.GetApplication().ShowView(view, bookmark);
                        }
                    }
                }
            }
        }

        private TrailResult m_ColorSelectorResult = null;
        void CS_ItemSelected(object sender, ColorSelectorPopup.ItemSelectedEventArgs e)
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

        private void SummaryList_ColumnHeaderMouseClick(object sender, TreeList.ColumnEventArgs e)
        {
            this.SummaryList_ColumnHeaderMouseClick(sender, e.Column);
        }

        private void SummaryList_ColumnHeaderMouseClick(object sender, TreeList.Column e)
        {
            if (Data.Settings.SummaryViewSortColumns[0] == e.Id)
            {
                Data.Settings.SummaryViewSortDirection = Data.Settings.SummaryViewSortDirection == ListSortDirection.Ascending ?
                       ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            Data.Settings.UpdateSummaryViewSortColumn = e.Id;
            this.SummaryList_Sort();
        }

        void SummaryList_SelectedItemsChanged(object sender, System.EventArgs e)
        {
            bool updated = false;
            if (Data.Settings.SelectSimilarSplits)
            {
                //At changes: Updates m_lastSelectedItems too, but clears ExplicitSelection
                updated = this.SelectSimilarSplits();
            }

            if (!updated)
            {
                //Explicit selection of results
                this.m_lastSelectedItems = this.SelectedResults;
                this.UpdateSelectedItems(this.SelectedResults);
                Controller.TrailController.Instance.ExplicitSelection = true;
            }
        }

        //Summary list updated (possibly by mouse selection), other related changes
        private void UpdateSelectedItems(IList<TrailResultWrapper> setValue)
        {
            Controller.TrailController.Instance.ExplicitSelection = false;
            Controller.TrailController.Instance.SelectedResults = setValue;
            this.SetSummary(setValue);
            //For instance colors change after selection changes
            this.summaryList.Refresh();
            this.m_page.RefreshRoute(false);
            this.m_page.RefreshChart();
        }
      
        /************************************************/

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

        public void AddCurrentCategoryCheck()
        {
            if (this.addCurrentCategoryMenuItem.Checked &&
                Controller.TrailController.Instance.ReferenceResult != null)
            {
                this.AddActivityFromCategory(this.GetCurrentCategory(InsertCategoryTypes.CurrentCategory));
            }
        }

        private void SelectResultsWithDeviceElevation()
        {
            IList<TrailResultWrapper> selected = new List<TrailResultWrapper>();
            foreach (TrailResultWrapper trw in Controller.TrailController.Instance.Results)
            {
                if (trw.Result.DeviceElevationTrack0() != null && trw.Result.DeviceElevationTrack0().Count > 0)
                {
                    selected.Add(trw);
                }
            }
            this.SelectedResults = selected;
        }

        private enum InsertCategoryTypes { CurrentCategory, SelectedTree, All };
        IActivityCategory GetCurrentCategory(InsertCategoryTypes addAll)
        {
            IActivityCategory cat = null;
            if (addAll == InsertCategoryTypes.SelectedTree && Controller.TrailController.Instance.ReferenceActivity != null)
            {
                cat = Controller.TrailController.Instance.ReferenceActivity.Category;
                IActivityCategory cat0 = cat.Parent;
                while (cat0.Parent != null)
                {
                    cat = cat0;
                    cat0 = cat0.Parent;
                }
            }
            else if (addAll == InsertCategoryTypes.CurrentCategory)
            {
                if (Controller.TrailController.Instance.ReferenceActivity == null ||
                    Plugin.GetApplication().DisplayOptions.SelectedCategoryFilter != Plugin.GetApplication().Logbook.ActivityCategories[0] &&
                    Plugin.GetApplication().DisplayOptions.SelectedCategoryFilter != Plugin.GetApplication().Logbook.ActivityCategories[1])
                {
                    //User has selected an activity filter - use it
                    cat = Plugin.GetApplication().DisplayOptions.SelectedCategoryFilter;
                }
                else if (Controller.TrailController.Instance.ReferenceActivity != null)
                {
                    //Use the category for the activity
                    cat = Controller.TrailController.Instance.ReferenceActivity.Category;
                }
            }
            return cat;
        }

        private int AddActivityFromCategory(IActivityCategory cat)
        {
            int res = 0;
            IList<IActivity> allActivities = new List<IActivity>();
            foreach (IActivity activity in Controller.TrailController.Instance.Activities)
            {
                allActivities.Add(activity);
            }
            foreach (IActivity activity in Plugin.GetApplication().Logbook.Activities)
            {
                if (!Controller.TrailController.Instance.Activities.Contains(activity) &&
                    (cat == null || IsCurrentCategory(activity.Category, cat)))
                {
                    //Insert after the current activities, then the order is normally OK
                    allActivities.Insert(Controller.TrailController.Instance.Activities.Count, activity);
                    res++;
                }
            }
            //Set activities, keep trail/selection
            Controller.TrailController.Instance.Activities = allActivities;
            m_page.RefreshData(false);
            m_page.RefreshControlState();
            return res;
        }

        private int AddCurrentTime()
        {
            int res = 0;
            IList<TrailResultWrapper> srw = this.SpecialSelectionResults;
            if (srw.Count > 0)
            {
                IList<IActivity> allActivities = new List<IActivity>();
                IList<IActivity> addActivities = new List<IActivity>();
                foreach (IActivity activity in Controller.TrailController.Instance.Activities)
                {
                    allActivities.Add(activity);
                }
                foreach (IActivity activity in Plugin.GetApplication().Logbook.Activities)
                {
                    foreach (TrailResultWrapper tr in srw)
                    {
                        if (!addActivities.Contains(activity) &&
                            !Controller.TrailController.Instance.Activities.Contains(activity) &&
                        tr.Result.AnyOverlap(activity))
                        {
                            //Insert after the current activities, then the order is normally OK
                            allActivities.Insert(Controller.TrailController.Instance.Activities.Count, activity);
                            addActivities.Add(activity);
                            res++;
                        }
                    }
                }

                //Set activities, keep trail/selection
                Controller.TrailController.Instance.Activities = allActivities;
                m_page.RefreshData(false);
                m_page.RefreshControlState();
            }
            return res;
        }

        private DialogResult SetMetaImportSource(IActivity activity)
        {
            STForm form = new STForm(m_visualTheme, 410, 105);
            ZoneFiveSoftware.Common.Visuals.TextBox textBox = new ZoneFiveSoftware.Common.Visuals.TextBox();
            form.Controls.Add(textBox);

            textBox.Width = form.Width - 37;
            textBox.Location = new System.Drawing.Point(10, 8);

            textBox.ThemeChanged(this.m_visualTheme);

            form.Text = "Set Import Source";
            textBox.Text = activity.Metadata.Source;

            DialogResult r = form.ShowDialog();
            if (r == DialogResult.OK)
            {
                activity.Metadata.Source = textBox.Text;
            }
            return r;
        }

        private enum SplitTimesPopup { AdjustDiff, PandolfTerrain }
        private void SetAdjustSplitTimesPopup(SplitTimesPopup splitTimesPopup)
        {
            STForm form = new STForm(m_visualTheme, 410, 105);

            ZoneFiveSoftware.Common.Visuals.TextBox SplitTimes_TextBox = new ZoneFiveSoftware.Common.Visuals.TextBox();
            form.Controls.Add(SplitTimes_TextBox);
            SplitTimes_TextBox.ThemeChanged(this.m_visualTheme);
            SplitTimes_TextBox.Width = form.Width - 37;
            SplitTimes_TextBox.Location = new System.Drawing.Point(10, 8);

            if (splitTimesPopup == SplitTimesPopup.AdjustDiff)
            {
                form.Text = string.Format("Diff Adjust: dist ({0}); timeOffset (s)", GpsRunningPlugin.Util.UnitUtil.Distance.LabelAbbrAct(Controller.TrailController.Instance.ReferenceActivity));
            }
            else
            {
                form.Text = string.Format("Pandolf Terrain: dist ({0}); factor", GpsRunningPlugin.Util.UnitUtil.Distance.LabelAbbrAct(Controller.TrailController.Instance.ReferenceActivity));
            }

            String colText = "";
            float[,] splitTimes;
            if (splitTimesPopup == SplitTimesPopup.AdjustDiff)
            {
                splitTimes = Data.Settings.AdjustDiffSplitTimes;
            }
            else
            {
                splitTimes = Data.Settings.PandolfTerrainDist;
            }
            if (splitTimes != null)
            {
                for (int i = 0; i < splitTimes.Length; i++)
                {
                    float f = splitTimes[i / 2, i % 2];
                    if (i % 2 == 0)
                    {
                        f = (float)GpsRunningPlugin.Util.UnitUtil.Distance.ConvertFrom(f);
                    }
                    if (colText == "") { colText = f.ToString(); }
                    else { colText += ";" + f; }
                }
            }
            SplitTimes_TextBox.Text = colText;

            DialogResult r = form.ShowDialog();
            if (r == DialogResult.OK)
            {
                try
                {
                    string[] values = SplitTimes_TextBox.Text.Split(';');
                    splitTimes = new float[(1 + values.Length) / 2, 2];
                    int i = 0;
                    foreach (string column in values)
                    {
                        float f = 0;
                        if (!string.IsNullOrEmpty(column))
                        {
                            f = Data.Settings.parseFloat(column);
                        }
                        if (i % 2 == 0)
                        {
                            f = (float)GpsRunningPlugin.Util.UnitUtil.Distance.ConvertTo(f, Controller.TrailController.Instance.ReferenceActivity);
                        }
                        splitTimes[i / 2, i % 2] = f;
                        i++;
                    }
                    if (splitTimes == null || splitTimes.Length == 0 ||
                        splitTimes.Length == 2 && splitTimes[0, 0] == 0 && splitTimes[0, 1] == 0)
                    {
                        //empty is null
                        splitTimes = null;
                    }
                    if (splitTimesPopup == SplitTimesPopup.AdjustDiff)
                    {
                        Data.Settings.AdjustDiffSplitTimes = splitTimes;
                    }
                    else
                    {
                        Data.Settings.PandolfTerrainDist = splitTimes;
                    }
                }
                catch { }
            }
        }

        private TrailResult GetSingleSelectedTrailResult()
        {
            TrailResult tr = null;
            IList<TrailResultWrapper> atr = this.SelectedResults;
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
            if (tr == null && Controller.TrailController.Instance.CurrentActivityTrailIsSelected)
            {
                TrailResultWrapper trw;
                trw = this.GetSummaryTotal();
                if (trw == null)
                {
                    trw = this.GetSummaryAverage();
                }
                if (trw != null)
                {
                    tr = trw.Result;
                }
            }

            return tr;
        }

        private IList<TrailsItemTrackSelectionInfo> GetSelections()
        {
            IList<TrailsItemTrackSelectionInfo> res = new List<TrailsItemTrackSelectionInfo>();
            IList<TrailResultWrapper> atr = this.SelectedResults;
            if (atr == null || atr.Count == 0)
            {
                TrailResultWrapper trw;
                trw = this.GetSummaryTotal();
                if (trw == null)
                {
                    trw = this.GetSummaryAverage();
                }
                if (trw == null)
                {
                    atr = new List<TrailResultWrapper>();
                }
                else
                {
                    atr = new List<TrailResultWrapper> { this.GetSummaryAverage() };
                }
            }

            foreach (TrailResultWrapper t in atr)
            {
                //Special handling for Summary, faking selection
                TrailResultMarked trm = new TrailResultMarked(t.Result);
                res.Add(TrailResultMarked.SelInfoUnion(new List<TrailResultMarked> { new TrailResultMarked(t.Result) }));
            }

            return res;
        }

        void PerformancePredictorPopup()
        {
            if (PerformancePredictor.PerformancePredictorIntegrationEnabled)
            {
                IList<TrailsItemTrackSelectionInfo> sels = GetSelections();
                PerformancePredictor.PerformancePredictorPopup(sels, m_view, null);
            }
        }

        void HighScorePopup()
        {
            if (HighScore.HighScoreIntegrationEnabled)
            {
                TrailResult tr = GetSingleSelectedTrailResult();
                if (tr != null)
                {
                    IList<IActivity> activities = new List<IActivity>();
                    IList<IValueRangeSeries<DateTime>> pauses = new List<IValueRangeSeries<DateTime>>();
                    if (tr is SummaryTrailResult)
                    {
                        foreach (TrailResult t in ((SummaryTrailResult)tr).Results)
                        {
                            //Only add one activity, HS only uses that
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
        
        private void MergeSubResults(IList<TrailResultWrapper> atr, bool all)
        {
            if (atr != null && atr.Count > 0)
            {
                //Get all relevant child results
                IDictionary<TrailResultWrapper, IList<TrailResultWrapper>> subRes = new Dictionary<TrailResultWrapper, IList<TrailResultWrapper>>();
                IDictionary<ActivityTrail, bool> multipleActivityTrails = new Dictionary<ActivityTrail, bool>();
                foreach (TrailResultWrapper tr in atr)
                {
                    if (tr.Result is ChildTrailResult &&
                        ((tr.Parent as TrailResultWrapper).Result is PositionParentTrailResult || (tr.Parent as TrailResultWrapper).Result is SplitsParentTrailResult))
                    {
                        TrailResultWrapper parent = (tr.Parent as TrailResultWrapper);
                        if (!subRes.ContainsKey(parent))
                        {
                            subRes[parent] = new List<TrailResultWrapper>();
                        }

                        subRes[parent].Add(tr);
                        if (multipleActivityTrails.ContainsKey(parent.Result.ActivityTrail))
                        {
                            multipleActivityTrails[parent.Result.ActivityTrail] = false;
                        }
                        else
                        {
                            multipleActivityTrails[parent.Result.ActivityTrail] = true;
                        }
                    }
                }

                foreach (TrailResultWrapper trw in subRes.Keys)
                {
                    trw.Result.ActivityTrail.MergeSubResults(trw, subRes[trw], all && multipleActivityTrails[trw.Result.ActivityTrail]);
                }
            }
        }

        void FixDistanceTrack()
        {
            //Temporary fix for changed distance tracks
            IList<TrailResultWrapper> atr = this.SelectedResults;
            if (atr != null && atr.Count > 0 &&
                Controller.TrailController.Instance.Results.Count > 0)
            {
                foreach (IActivity activity in Plugin.GetApplication().Logbook.Activities)
                //foreach (TrailResultWrapper trw in atr)
                {
                    //IActivity activity = trw.Result.Activity;
                    if (activity != null && activity.DistanceMetersTrack != null && activity.DistanceMetersTrack.Count > 0 &&
                        activity.GPSRoute != null && activity.GPSRoute.Count < activity.DistanceMetersTrack.Count)
                    {
                        IDistanceDataTrack track = new TrackUtil.DistanceDataTrack();
                        TrackUtil.setCapacity(track, activity.DistanceMetersTrack.Count);
                        int prev = int.MinValue;
                        foreach (ITimeValueEntry<float> t in activity.DistanceMetersTrack)
                        {
                            DateTime d = activity.DistanceMetersTrack.EntryDateTime(t);
                            if (t.ElapsedSeconds - prev != 1)
                            {
                                track.Add(d, t.Value);
                            }
                            prev = (int)t.ElapsedSeconds;
                        }
                        if (track.Count != activity.DistanceMetersTrack.Count)
                        {
                            STForm form = new STForm(m_visualTheme, 370, 105);
                            ZoneFiveSoftware.Common.Visuals.TextBox AdjustDiffSplitTimes_TextBox = new ZoneFiveSoftware.Common.Visuals.TextBox();
                            form.Text = string.Format("{0} {1}", activity.Name,  activity.StartTime);
                            form.Controls.Add(AdjustDiffSplitTimes_TextBox);
                            AdjustDiffSplitTimes_TextBox.ThemeChanged(this.m_visualTheme);
                            AdjustDiffSplitTimes_TextBox.Width = form.Width - 37;
                            AdjustDiffSplitTimes_TextBox.Location = new System.Drawing.Point(10, 8);

                            AdjustDiffSplitTimes_TextBox.Text = string.Format("{0}/{1}/ {2} {3} {4}", track.Count, activity.DistanceMetersTrack.Count, activity.GPSRoute.Count, activity.Name, activity.StartTime);

                            //update is done in clicking OK/Enter
                            if (form.ShowDialog() == DialogResult.OK)
                            {
                                activity.DistanceMetersTrack = track;
                            }
                        }
                    }
                }
            }
        }

        private void InsertRestLap(IActivityLaps laps, DateTime startTime, TimeSpan duration)
        {
            ILapInfo l = null;
            if (laps.Count > 0)
            {
                for (int i = 0; i < laps.Count; i++)
                {
                    if (laps[i].StartTime > startTime)
                    {
                        l = laps.Insert(i, startTime, duration);
                        break;
                    }
                }
            }
            if (l == null)
            {
                l = laps.Add(startTime, duration);
            }
            l.Rest = true;
        }

        /*************************************************************************/
        void SummaryList_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                int c = this.ExcludeSelectedResults(e.Modifiers == Keys.Shift);
                ShowToolTip(this.excludeResultsMenuItem + ": " + c);
            }

            else if (e.KeyCode == Keys.Space)
            {
                ShowToolTip(Properties.Resources.UI_Activity_List_Splits + ": " + Data.Settings.SelectSimilarSplits);
                this.SelectSimilarSplits();
            }

            else if (e.KeyCode == Keys.Escape)
            {
                this.m_page.ResultList_Collapse();
            }

            else if (e.KeyCode == Keys.F11)
            {
                this.m_page.ResultList_Expand((e.Modifiers & Keys.Shift) == 0 && !m_page.IsPopup);
            }

            else if (e.KeyCode == Keys.A)
            {
                if (e.Modifiers == (Keys.Control | Keys.Alt | Keys.Shift))
                {
                    //Unofficial
                    //Set the part marked (on chart on map) as a "Rest" lap
                    //A little inconveient to use. After marking right click in list to get focus, then activate
                    if (this.m_page.m_lastMarkedResult != null)
                    {
                        int res = 0;
                        foreach (TrailResultMarked mtr in this.m_page.m_lastMarkedResult)
                        {
                            foreach (IValueRange<DateTime> t in mtr.selInfo.MarkedTimes)
                            {
                                TimeSpan duration = ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.TimeNotPaused(
                                    mtr.trailResult.Activity.StartTime, t.Lower, mtr.trailResult.Activity.TimerPauses);
                                if (duration.TotalSeconds >= 1)
                                {
                                    InsertRestLap(mtr.trailResult.Activity.Laps, t.Lower, duration);
                                    res++;
                                }
                            }
                        }
                        ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelLap + ": " + res);
                    }
                    else
                    {
                        ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionSelectNone);
                    }
                    ////Fix to remove laps added last insert of inserted (code to be removed)
                    //IList<TrailResultWrapper> atr = this.SelectedResultWrapper;
                    //if (atr != null && atr.Count > 0)
                    //{
                    //    IActivity activity = atr[0].Result.Activity;
                    //    if (activity.Laps.Count > 1)
                    //    {
                    //        int j = -1;
                    //        for (int i = 1; i < activity.Laps.Count; i++)
                    //        {
                    //            if (j >= 0)
                    //            {
                    //                if (activity.Laps[i].StartTime < activity.Laps[j].StartTime)
                    //                {
                    //                    activity.Laps.Remove(activity.Laps[i]);
                    //                }
                    //            }
                    //            else if (activity.Laps[i].StartTime < activity.Laps[i - 1].StartTime)
                    //            {
                    //                j = i;
                    //            }
                    //        }
                    //    }
                    //}
                }
                else if (e.Modifiers == (Keys.Control | Keys.Shift))
                {
                    //Unofficial
                    DialogResult popRes = MessageDialog.Show("Set marked timer pauses as rest laps?",
                        "Remove Pauses", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (popRes == DialogResult.OK)
                    {
                        int res = 0;
                        IList<TrailResultWrapper> atr = this.SelectedResults;
                        if (atr != null && atr.Count > 0)
                        {
                            foreach (TrailResultWrapper tr in atr)
                            {
                                if (tr.Result is PausedChildTrailResult && (tr.Result as PausedChildTrailResult).pauseType == PauseType.Timer)
                                {
                                    TrackUtil.removePause(tr.Result.Activity.TimerPauses, tr.Result.StartTime, tr.Result.EndTime);
                                    TrackUtil.removePause(tr.Result.Pauses, tr.Result.StartTime, tr.Result.EndTime);
                                    InsertRestLap(tr.Result.Activity.Laps, tr.Result.StartTime, tr.Result.Duration);
                                    res++;
                                }
                            }
                        }
                        ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelLap + ": " + res);
                        this.m_page.RefreshData(true);
                    }
                }
                else if (e.Modifiers == (Keys.Alt | Keys.Shift))
                {
                    //Unofficial
                    DialogResult popRes = MessageDialog.Show("Set marked results as pauses?",
                        "Add Pause", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (popRes == DialogResult.OK)
                    {
                        int res = 0;
                        IList<TrailResultWrapper> atr = this.SelectedResults;
                        if (atr != null && atr.Count > 0)
                        {
                            foreach (TrailResultWrapper tr in atr)
                            {
                                if (tr.Result is ChildTrailResult)
                                {
                                    tr.Result.Activity.TimerPauses.Add(new ValueRange<DateTime>(tr.Result.StartTime, tr.Result.EndTime));
                                    res++;
                                }
                            }
                        }
                        ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelEndTime + ": " + res);
                        this.m_page.RefreshData(true);
                    }
                }
                else if (e.Modifiers == Keys.Control)
                {
                    this.SelectAll();
                }
                else
                {
                    Data.Settings.RestIsPause = !(e.Modifiers == Keys.Shift);
                    ShowToolTip(Properties.Resources.UI_Activity_List_SetRestLapsAsPauses + ": " + Data.Settings.RestIsPause);
                    Controller.TrailController.Instance.CurrentReset(false); //TBD
                    this.m_page.RefreshData(true);
                }
            }

            else if (e.KeyCode == Keys.B)
            {
                if (e.Modifiers == (Keys.Shift | Keys.Control))
                {
                    //Unofficial temp fix
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelDistance + " Fix");
                    FixDistanceTrack();
                }
            }

            else if (e.KeyCode == Keys.C)
            {
                if (e.Modifiers == Keys.Control)
                {
                    this.CopyTable();
                }
                else if (e.Modifiers == (Keys.Shift | Keys.Control))
                {
                    //Unofficial, undocumented, to simplify comparisions
                    try
                    {
                        SummaryTrailResult avg = this.m_summaryAverage.Result as SummaryTrailResult;
                        SummaryValue<double> a = avg.DistanceStdDev();
                        String s = Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail.Name + " " + Data.Settings.UseDeviceDistance + " " +
                            this.SelectedResults[0].Result.Activity.Metadata.Source + " " + avg.Order + " " +
                            GpsRunningPlugin.Util.UnitUtil.Distance.ToString(a.Value, Controller.TrailController.Instance.ReferenceActivity, "") +
                            " σ" + GpsRunningPlugin.Util.UnitUtil.Elevation.ToString(a.StdDev, Controller.TrailController.Instance.ReferenceActivity, "");
                        System.Windows.Forms.Clipboard.SetText(System.Windows.Forms.Clipboard.GetText() + "\n" + s);
                        ShowToolTip(s);
                    }
#pragma warning disable 0168
                    catch (Exception e1)
                    { }
                }
                else if (e.Modifiers == Keys.Shift)
                {
                    Data.Settings.DiffUsingCommonStretches = !Data.Settings.DiffUsingCommonStretches;
                    ShowToolTip(Properties.Resources.UI_Activity_List_URCommon + ": " + Data.Settings.DiffUsingCommonStretches);
                    this.m_page.RefreshData(true);
                }
                else
                {
                    ShowToolTip(Properties.Resources.UI_Activity_List_URCommon + "...");
                    this.MarkCommonStretches();
                }
            }

            else if (e.KeyCode == Keys.D)
            {
                if (e.Modifiers == Keys.Control)
                {
                    Data.Settings.UseDeviceDistance = !Data.Settings.UseDeviceDistance;
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelDevice + ": " + Data.Settings.UseDeviceDistance);
                    this.m_page.RefreshData(true);
                    this.SetSummary(this.SelectedResults);
                }
                else if (e.Modifiers == Keys.Alt)
                {
                    if (Data.Settings.UseGpsFilter == GpsFilterType.DistanceAndTime)
                    {
                        Data.Settings.UseGpsFilter = GpsFilterType.None;
                    }
                    else
                    {
                        Data.Settings.UseGpsFilter++;
                    }
                    ShowToolTip("GPS: " + Data.Settings.UseGpsFilter + " " +
                        ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelDistance + ": " + Data.Settings.GpsFilterMinimumDistance + " " +
                        ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelTime + ": " + Data.Settings.GpsFilterMinimumTime);
                    if (!Data.Settings.UseDeviceDistance)
                    {
                        this.m_page.RefreshData(true);
                        this.m_page.RefreshRoute(false);
                    }
                }
                else if (e.Modifiers == (Keys.Alt | Keys.Shift))
                {
                    //Undocumented, toggle through some presets
                    if (Data.Settings.UseGpsFilter == GpsFilterType.None)
                    {
                        Data.Settings.UseGpsFilter = GpsFilterType.DistanceOrTime;
                    }
                    if (Data.Settings.GpsFilterMinimumTime == 2)
                    {
                        Data.Settings.GpsFilterMinimumDistance = 15;
                        Data.Settings.GpsFilterMinimumTime = 3;
                    }
                    else if (Data.Settings.GpsFilterMinimumTime == 3)
                    {
                        Data.Settings.GpsFilterMinimumDistance = 25;
                        Data.Settings.GpsFilterMinimumTime = 5;
                    }
                    else
                    {
                        Data.Settings.GpsFilterMinimumDistance = 10;
                        Data.Settings.GpsFilterMinimumTime = 2;
                        Data.Settings.GpsFilterMaximumTime = 5;
                    }
                    ShowToolTip("GPS: " + Data.Settings.UseGpsFilter + " " +
                        ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelDistance + ": " + Data.Settings.GpsFilterMinimumDistance + " " +
                        ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelTime + ": " + Data.Settings.GpsFilterMinimumTime);
                    if (!Data.Settings.UseDeviceDistance)
                    {
                        this.m_page.RefreshData(true);
                        this.m_page.RefreshRoute(false);
                    }
                }
                else if (Controller.TrailController.Instance.PrimaryCurrentActivityTrail != null &&
                    !Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail.Generated)
                {
                    if (e.Modifiers == Keys.Shift)
                    {
                        Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail.DefaultRefActivity = null;
                        this.m_page.RefreshData(true);
                    }
                    else
                    {
                        Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail.DefaultRefActivity =
                            Controller.TrailController.Instance.ReferenceActivity;
                    }
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelActivity +
                        Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail.DefaultRefActivity);
                }
            }

            else if (e.KeyCode == Keys.E)
            {
                if (e.Modifiers == (Keys.Shift | Keys.Control))
                {
                    Data.Settings.UseTrailElevationAdjust = !Data.Settings.UseTrailElevationAdjust;
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelElevation + ": " +
                        Data.Settings.UseTrailElevationAdjust);
                    this.m_page.RefreshData(true);
                }
                else if (e.Modifiers == Keys.Control)
                {
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelDevice + " " +
                        ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelElevation);
                    this.SelectResultsWithDeviceElevation();
                }
                else if (e.Modifiers == Keys.Alt)
                {
                    Data.Settings.DeviceElevationFromOther = !Data.Settings.DeviceElevationFromOther;
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelElevation + " " +
                        ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelActivity + ": " +
                        Data.Settings.DeviceElevationFromOther);
                    this.m_page.RefreshData(true);
                }
                else if (e.Modifiers == Keys.Shift)
                {
                    Data.Settings.UseDeviceElevationForCalc = !Data.Settings.UseDeviceElevationForCalc;
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelElevation + " " +
                        ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelDevice + ": " +
                        Data.Settings.UseDeviceElevationForCalc);
                    this.m_page.RefreshData(true);
                }
            }

            else if (e.KeyCode == Keys.F)
            {
                //Unofficial shortcuts
                if (e.Modifiers == Keys.Control || e.Modifiers == (Keys.Shift | Keys.Control))
                {
                    //Set "Device elevation" (possibly in other activity) as ST normal (GPS) elevation
                    IList<TrailResultWrapper> atr = this.SelectedResults;
                    if (atr != null && atr.Count > 0 &&
                        Controller.TrailController.Instance.Results.Count > 0)
                    {
                        int c = 0;
                        foreach (TrailResultWrapper trw in atr)
                        {
                            //TBD if (trw.Result.SetDeviceElevation(Data.Settings.UseTrailElevationAdjust, (e.Modifiers & Keys.Shift) != 0))
                            if (trw.Result.SetDeviceElevation(Data.Settings.UseTrailElevationAdjust))
                            {
                                c++;
                            }
                        }
                        ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelElevation + " " +
                            ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelDevice + ": " +
                            Data.Settings.UseTrailElevationAdjust + " " + c + "/" + atr.Count);
                        m_page.RefreshData(true);
                    }
                }
                else if (e.Modifiers == Keys.Alt)
                {
                    //Put alternatively calculated grade in Cadence
                    TrailResult.CalculateGradeInCadence = !TrailResult.CalculateGradeInCadence;
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelGrade + " " +
                        ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelCadence + ": " +
                        TrailResult.CalculateGradeInCadence);
                    m_page.RefreshData(true);
                }
            }

            else if (e.KeyCode == Keys.G)
            {
                //Unofficial shortcuts
                if (e.Modifiers == Keys.Control)
                {
                    IList<TrailResultWrapper> atr = new List<TrailResultWrapper>();
                    IList<TrailResultWrapper> srw = this.SpecialSelectionResults;
                    foreach (TrailResultWrapper tr in srw)
                    {
                        if (tr.Result.Activity != null)
                        {
                            //Select the results with the same import metasource
                            string refSource = tr.Result.Activity.Metadata.Source;
                            foreach (TrailResultWrapper trw in Controller.TrailController.Instance.Results)
                            {
                                if (trw.Result.Activity != null &&
                                    (string.IsNullOrEmpty(trw.Result.Activity.Metadata.Source) && string.IsNullOrEmpty(refSource) ||
                                    !string.IsNullOrEmpty(trw.Result.Activity.Metadata.Source) && !string.IsNullOrEmpty(refSource) &&
                                      //ExportToFunbeat adds suffix to meta data...
                                      (refSource.StartsWith(trw.Result.Activity.Metadata.Source) ||
                                      trw.Result.Activity.Metadata.Source.StartsWith(refSource))) &&
                                      !atr.Contains(trw))
                                {
                                    atr.Add(trw);
                                }
                            }
                        }
                    }
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelDevice + ": " +
                        atr.Count);
                    this.SelectedResults = atr;
                }
                else if (e.Modifiers == (Keys.Control | Keys.Shift))
                {
                    if (Controller.TrailController.Instance.ReferenceActivity != null)
                    {
                        IList<IActivity> atr = TrailResultWrapper.Activities(this.SelectedResults);
                        string refSource = Controller.TrailController.Instance.ReferenceActivity.Metadata.Source;

                        DialogResult popRes = MessageDialog.Show(string.Format("Set import source to \"{0}\" for {1} activities?", refSource, atr.Count),
                         "Set Import Source", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (popRes == DialogResult.OK)
                        {
                            foreach (IActivity activity in atr)
                            {
                                activity.Metadata.Source = refSource;
                            }
                            this.summaryList.RefreshElements( this.summaryList.SelectedItems );
                        }
                    }
                }
                else if (e.Modifiers == Keys.Alt)
                {
                    this.SelectedResults = Controller.TrailController.Instance.UpdateResults(
                        m_PersistentSelectionResults);
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionReplace + ": " +
                        ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelActivity + ": " +
                        this.SelectedResults.Count);
                }
                else if (e.Modifiers == (Keys.Alt | Keys.Shift))
                {
                    //set special/reference results/activities
                    m_PersistentSelectionResults = this.SelectedResults;
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionSave + ": " +
                        ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelActivity + ": " +
                        this.SelectedResults.Count);
                }
                else if (e.Modifiers == (Keys.Alt | Keys.Shift | Keys.Control))
                {
                    int res = 0;
                    IList<IActivity> allActivities = Controller.TrailController.Instance.Activities;
                    foreach (TrailResultWrapper t in m_PersistentSelectionResults)
                    {
                        if (!Controller.TrailController.Instance.CurrentActivityTrails.Contains(t.Result.ActivityTrail))
                        {
                            Controller.TrailController.Instance.CurrentActivityTrails.Add(t.Result.ActivityTrail);
                            res++;
                        }
                        if (!allActivities.Contains(t.Result.Activity))
                        {
                            allActivities.Add(t.Result.Activity);
                            res++;
                        }
                        t.Result.ActivityTrail.ReAdd(t);
                    }
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionAdd + ": " +
                        ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelActivity + ": " +
                        m_PersistentSelectionResults.Count + " (" + res + ")");
                    Controller.TrailController.Instance.Activities = allActivities;
                    this.SelectedResults = m_PersistentSelectionResults;
                }
            }

            else if (e.KeyCode == Keys.H)
            {
                //Unofficial
                if (e.Modifiers == Keys.Alt)
                {
                    //Select Similar splits that matches a certain modulo
                    //Used similar to lap merge, when Splits have several laps for one "loop"
                    Data.Settings.SelectSimilarModulu++;
                }
                else if (e.Modifiers == (Keys.Alt | Keys.Shift))
                {
                    //Default, select similar from child index
                    Data.Settings.SelectSimilarModulu = 0;
                }
                ShowToolTip(Properties.Resources.UI_Activity_List_Splits + ": " + Data.Settings.SelectSimilarSplits + " (" +
                    Data.Settings.SelectSimilarModulu + ")");
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

                int res = this.AddActivityFromCategory(this.GetCurrentCategory(c));
                ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionAdd + " " +
                    ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelActivity + ": " + c + " " + res);
            }

            else if (e.KeyCode == Keys.K)
            {
                if (e.Modifiers == Keys.Alt)
                {
                    Data.Settings.CadenceFromOther = !Data.Settings.CadenceFromOther;
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelActivity + " " +
                        ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelCadence + ": " + Data.Settings.CadenceFromOther);
                    this.m_page.RefreshData(true);
                }
            }

            else if (e.KeyCode == Keys.L)
            {
                if ((e.Modifiers & Keys.Control) != 0)
                {
                    this.MergeSubResults(this.SelectedResults, (e.Modifiers & Keys.Shift) != 0);
                    this.m_page.RefreshData(false);
                    this.m_page.RefreshRoute(false);
                }
                else
                {
                    //Unofficial
                    Data.Settings.ShowTrailPointsOnMap = !Data.Settings.ShowTrailPointsOnMap;
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelRoute + ": " +
                        Data.Settings.ShowTrailPointsOnMap);
                    this.m_page.RefreshRoute(false);
                }
            }

            else if (e.KeyCode == Keys.N)
            {
                Data.Settings.NonReqIsPause = !(e.Modifiers == Keys.Shift);
                ShowToolTip(Properties.Resources.Required + ": " +
                    Data.Settings.NonReqIsPause);
                this.m_page.RefreshData(true);
            }

            else if (e.KeyCode == Keys.O)
            {
                if (e.Modifiers == Keys.Control)
                {
                    Data.Settings.ResultSummaryStdDev = !Data.Settings.ResultSummaryStdDev;
                    ShowToolTip(Properties.Resources.UI_Activity_List_ResultSummaryStdDev + ": " +
                        Data.Settings.ResultSummaryStdDev);
                }
                else
                {
                    Data.Settings.StartDistOffsetFromStartPoint = !Data.Settings.StartDistOffsetFromStartPoint;
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelStart + ": " +
                        Data.Settings.StartDistOffsetFromStartPoint);
                }
                this.summaryList.Refresh();
                //Only in table, no need to refresh
            }

            else if (e.KeyCode == Keys.P)
            {
                //Unofficial
                if (e.Modifiers == Keys.Control)
                {
                    Data.Settings.PopupUpdatedBySelection = !Data.Settings.PopupUpdatedBySelection;
                    if (this.m_page.IsPopup)
                    {
                        if (Data.Settings.PopupUpdatedBySelection)
                        {
                            m_view.SelectionProvider.SelectedItemsChanged += new EventHandler(m_page.PopupForm_OnViewSelectedItemsChanged);
                        }
                        else
                        {
                            m_view.SelectionProvider.SelectedItemsChanged -= new EventHandler(m_page.PopupForm_OnViewSelectedItemsChanged);
                        }
                    }
                }
                //In context menu, not documented, to be removed?
                else if (e.Modifiers == Keys.Shift)
                {
                    this.PerformancePredictorPopup();
                }
                else
                {
                    this.HighScorePopup();
                }
            }

            else if (e.KeyCode == Keys.Q)
            {
                if (e.Modifiers == (Keys.Shift | Keys.Control))
                {
                    TrailResult.PaceTrackIsGradeAdjustedPaceAvg = !TrailResult.PaceTrackIsGradeAdjustedPaceAvg;
                    ShowToolTip(Properties.Resources.UI_Settings_GradeAdjustedPace + ": " +
                        TrailResult.PaceTrackIsGradeAdjustedPaceAvg);
                }
                else if (e.Modifiers == Keys.Control)
                {
                    TrailResult.DiffToSelf = !TrailResult.DiffToSelf;
                    ShowToolTip(Properties.Resources.UI_Chart_Difference + ": " +
                        TrailResult.DiffToSelf);
                }
                else if (e.Modifiers == Keys.Alt)
                {
                    this.SetAdjustSplitTimesPopup(SplitTimesPopup.AdjustDiff);
                }
                else if (e.Modifiers == (Keys.Control | Keys.Alt)) //AltGr
                {
                    this.SetAdjustSplitTimesPopup(SplitTimesPopup.PandolfTerrain);
                }
                else
                {
                    if (e.Modifiers == (Keys.Shift | Keys.Alt))
                    {
                        Data.Settings.IncreaseRunningGradeCalcMethod(false);
                    }
                    else if (e.Modifiers == Keys.Shift)
                    {
                        Data.Settings.RunningGradeAdjustMethod = RunningGradeAdjustMethodEnum.None;
                    }
                    else
                    {
                        Data.Settings.IncreaseRunningGradeCalcMethod(false);
                    }
                    //foreach (TrailResultWrapper t in TrailsPlugin.Controller.TrailController.Instance.Results)
                    //{
                    //    t.Result.ResetRunningGradeCalc();
                    //    foreach (TrailResultWrapper t2 in t.AllChildren)
                    //    {
                    //        t.Result.ResetRunningGradeCalc();
                    //    }
                    //}
                    ShowToolTip(Properties.Resources.UI_Settings_GradeAdjustedPace + ": " + Data.Settings.RunningGradeAdjustMethod);
                }
                this.m_page.RefreshData(true);
            }

            else if (e.KeyCode == Keys.R || e.KeyCode == Keys.F5)
            {
                //'r' or f5 can beused to recalc when an activity is changed
                //Other combinations are for debugging, like test trail calculation time
                //Time for the different alternatives, with debugger on my PC, 3176 activitities, 94 trails
                if ((e.Modifiers & Keys.Alt) != 0)
                {
                    //Clear all calculated data for the results: 0,20s
                    Controller.TrailController.Instance.Clear(false);
                }
                if ((e.Modifiers & Keys.Control) != 0)
                {
                    //Clear activity cache: 0,30s
                    ActivityCache.ClearActivityCache();
                }

                bool allRefresh = ((e.Modifiers & Keys.Shift) != 0);
                DateTime startTime = DateTime.Now;
                int progressCount = 0;
                if (allRefresh)
                {
                    //All recalc: 7,79s
                    Controller.TrailController.Instance.Reset();
                }
                else
                {
                    progressCount = Controller.TrailController.Instance.Activities.Count;
                    //ReCalcAll will force recalc, so Alt have no effect
                    if ((e.Modifiers & Keys.Alt) != 0)
                    {
                        //Single: 0,05s
                        Controller.TrailController.Instance.CurrentReset(false);
                    }
                }

                System.Windows.Forms.ProgressBar progressBar = StartProgressBar(progressCount);
                Controller.TrailController.Instance.ReCalcTrails(allRefresh, progressBar);
                this.m_page.RefreshData(false);
                StopProgressBar();

                if (allRefresh)
                {
                    //For debugging, the tooltip can be overwritten by other info in list
                    ShowToolTip((DateTime.Now - startTime).ToString());
                }
            }

            else if (e.KeyCode == Keys.S)
            {
                if (e.Modifiers == Keys.Control)
                {
                    Data.Settings.ShowActivityValuesForResults = !Data.Settings.ShowActivityValuesForResults;
                    //This is all dynamic, but we want to retrigger sort
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelActivity + ": " +
                        Data.Settings.ShowActivityValuesForResults);
                    this.m_page.RefreshData(false);
                }
                else if (e.Modifiers == Keys.Alt)
                {
                    Data.Settings.ShowSummaryForChildren = !Data.Settings.ShowSummaryForChildren;
                    ShowToolTip(Properties.Resources.UI_Activity_List_ShowSummaryAverage + ": " +
                        Data.Settings.ShowSummaryForChildren);
                    this.SetSummary(this.SelectedResults);
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
                    Data.Settings.XAxisValue = Utils.XAxisValue.Time;
                    Data.Settings.OverlappingResultUseTimeOfDayDiff = true;
                    Data.Settings.OverlappingResultUseReferencePauses = true;
                    Data.Settings.OverlappingResultShareSplitTime =
                        (Controller.TrailController.Instance.PrimaryCurrentActivityTrail.Trail.TrailType == Trail.CalcType.Splits);
                    int res = this.AddCurrentTime();
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelNumActivities + ": " + res);
                }
                else if (e.Modifiers == (Keys.Alt | Keys.Shift))
                {
                    Data.Settings.OverlappingResultShareSplitTime = !Data.Settings.OverlappingResultShareSplitTime;
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelTime + ": " +
                        Data.Settings.OverlappingResultShareSplitTime);
                    //The overlapping is set when calculating the trail
                    this.m_page.RefreshData(true, true);
                }
                else if (e.Modifiers == (Keys.Control | Keys.Alt | Keys.Shift))
                {
                    Data.Settings.OverlappingResultUseTimeOfDayDiff = !Data.Settings.OverlappingResultUseTimeOfDayDiff;
                    ShowToolTip(Properties.Resources.UI_Activity_List_DiffPresent + ": " +
                        Data.Settings.OverlappingResultUseTimeOfDayDiff);
                    this.m_page.RefreshData(true);
                }
                else if (e.Modifiers == (Keys.Control | Keys.Alt))
                {
                    Data.Settings.OverlappingResultUseReferencePauses = !Data.Settings.OverlappingResultUseReferencePauses;
                    ShowToolTip("Pause" + ": " +
                        Data.Settings.OverlappingResultUseReferencePauses);
                    this.m_page.RefreshData(true);
                }
                //else if (e.Modifiers == Keys.Control)
                //{
                //    //TBD, currently unused
                //    this.m_page.RefreshData(true);
                //}
                else
                {
                    //Depreciated since adding both average/total
                    Data.Settings.ResultSummaryTotal = !Data.Settings.ResultSummaryTotal;
                    ShowToolTip(Properties.Resources.UI_Activity_List_ShowSummaryTotal + ": " +
                        Data.Settings.ResultSummaryTotal);
                    RefreshSummary();
                }
            }

            else if (e.KeyCode == Keys.U)
            {
                int res = this.SelectWithUR();
                ShowToolTip(string.Format(Properties.Resources.UI_Activity_List_URSelect, res));
            }

            else if (e.KeyCode == Keys.X)
            {
                if (e.Modifiers == Keys.Alt || e.Modifiers == (Keys.Alt | Keys.Shift))
                {
                    bool save = (e.Modifiers == Keys.Alt);
                    FileDialog fileDialog;
                    if (save)
                    {
                        fileDialog = new SaveFileDialog();
                    }
                    else
                    {
                        fileDialog = new OpenFileDialog();
                    }

                    fileDialog.InitialDirectory = System.IO.Path.Combine(
                        Plugin.GetApplication().Configuration.UserPluginsDataFolder, Properties.Resources.ApplicationName);
                    fileDialog.FileName = Properties.Resources.ApplicationName + ".backup.xml";
                    fileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                    fileDialog.FilterIndex = 1;

                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string xmlFile = fileDialog.FileName;
                        try
                        {
                            if (save)
                            {
                                Plugin.SettingsToFile(xmlFile);
                            }
                            else
                            {
                                Plugin.SettingsFromFile(xmlFile);
                                this.m_page.RefreshData(true);
                                this.m_page.RefreshControlState();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageDialog.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                        }
                    }
                }
            }

            else if (e.KeyCode == Keys.Z)
            {
                if (e.Modifiers == Keys.Control)
                {
                    Data.Settings.ZoomToSelection = !Data.Settings.ZoomToSelection;
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionZoomIn + ": " + Data.Settings.ZoomToSelection);
                }
                else if (e.Modifiers == Keys.Shift)
                {
                    Data.Settings.ShowOnlyMarkedOnRoute = !Data.Settings.ShowOnlyMarkedOnRoute;
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelRoute + ": " + Data.Settings.ShowOnlyMarkedOnRoute);
                    this.m_page.RefreshData(false);
                }
                else
                {
                    //Zoom to selected parts
                    ShowToolTip(ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionZoomIn);
                    this.m_page.ZoomMarked();
                }
            }
            m_page.RefreshControlState();
        }

        private void SummaryList_ColumnResized(object sender, ZoneFiveSoftware.Common.Visuals.TreeList.ColumnEventArgs e)
        {
            TreeList.Column col = (TreeList.Column)e.Column;
            TreeList l = (TreeList)sender;
            int width = col.Width;
            if (l.Columns[0].Id == col.Id) { width -= cPlusMinusWidth; }
            Data.Settings.ActivityPageColumnsSizeSet(col.Id, width);
        }

        private void ShowToolTip(string s)
        {
            this.summaryListToolTipTimer.Stop();
            this.summaryListToolTipDisableTimer.Interval = 2000;
            this.summaryListToolTipDisableTimer.Start();
            this.summaryListTooltipDisabled = true;

            this.summaryListToolTip.Show(s,
                this.summaryList,
                new System.Drawing.Point(this.summaryListCursorLocationAtMouseMove.X +
                  Cursor.Current.Size.Width / 2,
                  this.summaryListCursorLocationAtMouseMove.Y),
                this.summaryListToolTip.AutoPopDelay);
        }

        private System.Windows.Forms.MouseEventArgs m_mouseClickArgs = null;
        bool summaryListTooltipDisabled = false; // is set to true, whenever a tooltip would be annoying, e.g. while a context menu is shown
        System.Drawing.Point summaryListCursorLocationAtMouseMove;
        TrailResultWrapper summaryListLastEntryAtMouseMove = null;

        private void SummaryList_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            m_mouseClickArgs = e;
            TrailResultWrapper entry = (TrailResultWrapper)this.summaryList.RowHitTest(e.Location, out TreeList.RowHitState rowHitState);
            if (entry == this.summaryListLastEntryAtMouseMove)
            {
                return;
            }
            else if (!this.summaryListTooltipDisabled)
            {
                this.summaryListToolTip.Hide(this.summaryList);
            }
            this.summaryListLastEntryAtMouseMove = entry;
            this.summaryListCursorLocationAtMouseMove = e.Location;

            if (entry != null)
                this.summaryListToolTipTimer.Start();
            else
                this.summaryListToolTipTimer.Stop();
        }

        private void SummaryList_MouseLeave(object sender, EventArgs e)
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
                this.summaryListToolTip.ReshowDelay = 100; //Default
                string tt = this.summaryListLastEntryAtMouseMove.Result.ToolTip;
                this.summaryListToolTip.Show(tt,
                              this.summaryList,
                              new System.Drawing.Point(this.summaryListCursorLocationAtMouseMove.X +
                                  Cursor.Current.Size.Width / 2,
                                        this.summaryListCursorLocationAtMouseMove.Y),
                              this.summaryListToolTip.AutoPopDelay);
            }
        }

        private void ToolTipDisableTimer_Tick(object sender, EventArgs e)
        {
            this.summaryListToolTipDisableTimer.Stop();
            this.summaryListTooltipDisabled = false;
        }

        private TrailResultWrapper GetMouseResult()
        {
            TrailResultWrapper trw = null;
            if (m_mouseClickArgs != null)
            {
                object row = this.summaryList.RowHitTest(m_mouseClickArgs.Location, out TreeList.RowHitState hitState);
                if (row != null && row is TrailResultWrapper)
                {
                    trw = row as TrailResultWrapper;
                }
            }
            return trw;
        }

        /*************************************************************************************************************/
        void ListMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string currRes = "none"; //TBD
            TrailResultWrapper tr = GetMouseResult();
            if (tr != null && !(tr.Result is SummaryTrailResult))
            {
                currRes = tr.Result.StartTime.ToLocalTime().ToString();
            }

            string refRes = "";
            if (Controller.TrailController.Instance.ReferenceResult != null)
            {
                refRes = Controller.TrailController.Instance.ReferenceResult.Result.StartTime.ToLocalTime().ToString();
            }
            this.referenceResultMenuItem.Text = string.Format(
                Properties.Resources.UI_Activity_List_ReferenceResult, currRes, refRes);
            this.limitURMenuItem.Text = string.Format(Properties.Resources.UI_Activity_List_URLimit, refRes);
            this.selectWithURMenuItem.Text = string.Format(Properties.Resources.UI_Activity_List_URSelect, refRes);
            if (tr == null || tr == Controller.TrailController.Instance.ReferenceResult)
            {
                //Instead of a special text
                this.referenceResultMenuItem.Enabled = false;
            }
            else
            {
                this.referenceResultMenuItem.Enabled = true;
            }
            if (Controller.TrailController.Instance.CurrentActivityTrailIsSelected)
            {
                this.addInBoundActivitiesMenuItem.Enabled = false;
                foreach (ActivityTrail at in Controller.TrailController.Instance.CurrentActivityTrails)
                {
                    this.addInBoundActivitiesMenuItem.Enabled |= at.CanAddInbound;
                }
            }
            if (tr == null || tr.Result.Activity == null)
            {
                //Summary result
                this.referenceResultMenuItem.Enabled = false;
                this.excludeResultsMenuItem.Enabled = false;
                this.markCommonStretchesMenuItem.Enabled = false;
            }
            else
            {
                //Separate handled
                //this.referenceResultMenuItem.Enabled = true;
                this.excludeResultsMenuItem.Enabled = true;
                this.markCommonStretchesMenuItem.Enabled = true && Integration.UniqueRoutes.UniqueRouteIntegrationEnabled;
            }

            if (Controller.TrailController.Instance.ReferenceActivity != null)
            {
                InsertCategoryTypes c = InsertCategoryTypes.SelectedTree;
                IActivityCategory cat = this.GetCurrentCategory(c);

                this.addTopCategoryMenuItem.Enabled = true;
                this.addTopCategoryMenuItem.Text = string.Format(Properties.Resources.UI_Activity_List_AddTopCategory, 
                    Data.Settings.printFullCategoryPath(cat));
                this.addCurrentCategoryMenuItem.Enabled = true;
                this.addCurrentCategoryMenuItem.Text = string.Format(Properties.Resources.UI_Activity_List_AddReferenceCategory, 
                    Data.Settings.printFullCategoryPath(Controller.TrailController.Instance.ReferenceActivity.Category));
            }
            else
            {
                this.addTopCategoryMenuItem.Enabled = false;
                this.addCurrentCategoryMenuItem.Enabled = false;
            }

            e.Cancel = false;
        }

        static string HelpTutorialBtn_url = "https://github.com/gerhardol/trails/wiki/Tutorials";
        private void HelpTutorialBtn_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(HelpTutorialBtn_url);
        }

        private void HelpFeaturesBtn_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/gerhardol/trails/wiki/Features");
        }

        private void InsertActivitiesBtn_Click(object sender, EventArgs e)
        {
            //Should probably just be the insert....
            ZoneFiveSoftware.Common.Visuals.Button btnSender = (ZoneFiveSoftware.Common.Visuals.Button)sender;
            System.Drawing.Point ptLowerLeft = new System.Drawing.Point(btnSender.Width - listMenu.Width, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            listMenu.Show(ptLowerLeft);
        }

        void CopyTableMenu_Click(object sender, EventArgs e)
        {
            CopyTable();
        }

        private void ListSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if ST_2_1
            ListSettings dialog = new ListSettings();
            dialog.ColumnsAvailable = TrailResultColumnIds.ColumnDefs_ST2(Controller.TrailController.Instance.ReferenceActivity, false);
#else
            ListSettingsDialog dialog = new ListSettingsDialog();
            //always show 'Splits' columns, even if only visible for Splits trail 
            IList<IListColumnDefinition> cols = (new TrailResultColumns(Controller.TrailController.Instance.ReferenceActivity, Controller.TrailController.Instance.Activities.Count, true)).ColumnDefs();
            dialog.AvailableColumns = cols;
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

        void ReferenceResultMenuItem_Click(object sender, System.EventArgs e)
        {
            TrailResultWrapper tr = GetMouseResult();
            if (tr != Controller.TrailController.Instance.ReferenceResult)
            {
                TrailResultWrapper prev = Controller.TrailController.Instance.ReferenceResult;
                Controller.TrailController.Instance.ReferenceResult = tr;
                this.summaryList.RefreshElements(new List<TrailResultWrapper> { tr, prev });
                this.m_page.RefreshData(true);
            }
        }

        void AnalyzeMenuItem_DropDownOpened(object sender, System.EventArgs e)
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

        void HighScoreMenuItem_Click(object sender, System.EventArgs e)
        {
            HighScorePopup();
        }

        void PerformancePredictorMenuItem_Click(object sender, System.EventArgs e)
        {
            PerformancePredictorPopup();
        }

        void ExcludeResultsMenuItem_Click(object sender, System.EventArgs e)
        {
            ExcludeSelectedResults(false);
        }

        void LimitActivityMenuItem_Click(object sender, System.EventArgs e)
        {
#if !ST_2_1
            IList<TrailResultWrapper> atr = this.SelectedResults;
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

        void AddInBoundActivitiesMenuItem_Click(object sender, System.EventArgs e)
        {
            if (Controller.TrailController.Instance.CurrentActivityTrailIsSelected)
            {
                System.Windows.Forms.ProgressBar progressBar = this.StartProgressBar(0);
                Controller.TrailController.Instance.CurrentReset(false);
                foreach (ActivityTrail t in Controller.TrailController.Instance.CurrentActivityTrails)
                {
                    t.AddInBoundResult(progressBar);
                }
                this.StopProgressBar();
                m_page.RefreshData(false);
                m_page.RefreshControlState();
            }
        }
        
        void AddCurrentCategoryMenuItem_Click(object sender, System.EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem addCurrent = (ToolStripMenuItem)sender; //addCurrentCategoryMenuItem
                addCurrent.Checked = !addCurrent.Checked;
                Data.Settings.AddCurrentCategory = addCurrent.Checked;
                AddCurrentCategoryCheck();
            }
        }

        void AddTopCategoryMenuItem_Click(object sender, System.EventArgs e)
        {
            InsertCategoryTypes c = InsertCategoryTypes.SelectedTree;
            this.AddActivityFromCategory(this.GetCurrentCategory(c));
        }

        void LimitURMenuItem_Click(object sender, System.EventArgs e)
        {
#if !ST_2_1
            try
            {
                IList<IActivity> similarActivities = null;
                if (Controller.TrailController.Instance.ReferenceActivity != null)
                {
                    System.Windows.Forms.ProgressBar progressBar = StartProgressBar(1);
                    similarActivities = UniqueRoutes.GetUniqueRoutesForActivity(
                        Controller.TrailController.Instance.ReferenceActivity, Controller.TrailController.Instance.Activities, progressBar);
                    StopProgressBar();
                }
                if (similarActivities != null)
                {
                    IList<IActivity> allActivities = new List<IActivity> { Controller.TrailController.Instance.ReferenceActivity };
                    foreach (IActivity activity in Controller.TrailController.Instance.Activities)
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
                MessageDialog.Show(ex.Message, "Plugin error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
#endif
        }

        void MarkCommonStretchesMenuItem_Click(object sender, System.EventArgs e)
        {
            MarkCommonStretches();
        }

        void SelectWithURMenuItem_Click(object sender, System.EventArgs e)
        {
            SelectWithUR();
        }

        private void ChartTablePanel_SizeChanged(object sender, System.EventArgs e)
        {
            //Fix for Summary List not shrinking (related to the table panel)
            int h = this.chartTablePanel.Height - (int)this.chartTablePanel.RowStyles[0].Height;
            if (h < this.summaryList.Size.Height)
            {
                this.summaryList.Size = new System.Drawing.Size(this.summaryList.Size.Width, h);
            }
        }
    }
}
