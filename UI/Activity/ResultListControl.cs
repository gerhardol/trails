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
            this.summaryList.SelectedChanged += new System.EventHandler(this.List_SelectedChanged);
#else
            this.summaryList.SelectedItemsChanged += new System.EventHandler(this.List_SelectedChanged);
#endif
        }

        void InitControls()
        {
            copyTableMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.DocumentCopy16;
#if !ST_2_1
            selectActivityMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Analyze16;
#endif
            //TODO: 
            selectWithURMenuItem.Visible = false;

            summaryList.NumHeaderRows = TreeList.HeaderRows.Two;
            summaryList.LabelProvider = new TrailResultLabelProvider();
        }

        public void UICultureChanged(CultureInfo culture)
        {
            copyTableMenuItem.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionCopy;
            this.listSettingsMenuItem.Text = Properties.Resources.UI_Activity_List_ListSettings;
            this.selectActivityMenuItem.Text = Properties.Resources.UI_Activity_List_LimitSelection;
            this.selectSimilarSplitsMenuItem.Text = Properties.Resources.UI_Activity_List_Splits;
            //this.referenceTrailMenuItem.Text = Properties.Resources.UI_Activity_List_ReferenceResult;
            this.selectWithURMenuItem.Text = string.Format(Properties.Resources.UI_Activity_List_URLimit, "");

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
            foreach (string id in PluginMain.Settings.ActivityPageColumns)
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
            selectActivityMenuItem.Enabled = m_controller.Activities.Count > 1;
        }


        public void RefreshList()
        {
            summaryList.RowData = null;

            if (m_controller.CurrentTrailOrdered != null)
            {
                IList<TrailResult> results = m_controller.CurrentTrailOrdered.activityTrail.Results;
                RefreshColumns();

#if ST_2_1
                this.summaryList.SelectedChanged -= new System.EventHandler(this.List_SelectedChanged);
#else
                this.summaryList.SelectedItemsChanged -= new System.EventHandler(this.List_SelectedChanged);
#endif
                summaryList_Sort();
#if ST_2_1
                this.summaryList.SelectedChanged += new System.EventHandler(this.List_SelectedChanged);
#else
                this.summaryList.SelectedItemsChanged += new System.EventHandler(this.List_SelectedChanged);
#endif
                ((TrailResultLabelProvider)summaryList.LabelProvider).MultipleActivities = (m_controller.Activities.Count > 1);
                //TODO: Keep selection in list?
                if (results.Count > 0)
                {
                    SelectedItems = new List<TrailResult> { results[0] };
                }
                else
                {
                    SelectedItems = null;
                }
                //Set size, to not waste chart
                int resRows = Math.Min(5, results.Count);
                this.summaryList.Height = this.summaryList.HeaderRowHeight +
                    this.summaryList.DefaultRowHeight * resRows;
            }
            else
            {
                SelectedItems = null;
            }
        }

        public IList<TrailResult> SelectedItems
        {
            set
            {
                IList<TreeList.TreeListNode> results = new List<TreeList.TreeListNode>();
                if (summaryList.RowData != null && value != null)
                {
                    foreach (TreeList.TreeListNode tn in (IList<TreeList.TreeListNode>)summaryList.RowData)
                    {
                        foreach (TrailResult tr in value)
                        {
                            if (tn.Element is TrailResult && tr.Equals((TrailResult)(tn.Element)))
                            {
                                results.Add(tn);
                            }
                        }
                    }
                }
#if ST_2_1
                summaryList.Selected = (List<TreeList.TreeListNode>)results;
#else
                summaryList.SelectedItems = (List<TreeList.TreeListNode>)results;
#endif
            }
            get {
                IList<TrailResult> results;
#if ST_2_1
                results = getTrailResultSelection(summaryList.Selected);
#else
                results = getTrailResultSelection(summaryList.SelectedItems);
#endif
                return results;
            }
        }

        /*********************************************************/
        private IList<TreeList.TreeListNode> getTreeListNodeSplits()
        {
            summaryList.SetSortIndicator(TrailsPlugin.Data.Settings.SummaryViewSortColumn,
                TrailsPlugin.Data.Settings.SummaryViewSortDirection == ListSortDirection.Ascending);

            IList<TreeList.TreeListNode> results = 
                (IList<TreeList.TreeListNode>)m_controller.CurrentTrailOrdered.activityTrail.ResultListCache;
            if (results == null)
            {
                results = new List<TreeList.TreeListNode>();

                IList<TrailResult> res1 = m_controller.CurrentTrailOrdered.activityTrail.Results;
                ((List<TrailResult>)res1).Sort();
            foreach (TrailResult tr in res1)
            {
                TreeList.TreeListNode tn = new TreeList.TreeListNode(null, tr);
                IList<TrailResult> splits = tr.getSplits();
                //Do not add single splits - nothing to expand
                if (splits.Count > 1)
                {
                    ((List<TrailResult>)splits).Sort();
                    foreach (TrailResult tr2 in splits)
                    {
                        TreeList.TreeListNode tn2 = new TreeList.TreeListNode(tn, tr2);
                        tn.Children.Add(tn2);
                    }
                }
                results.Add(tn);
            }
            //xxx Will not work when sorting - the creation of treelist is slow 
                //m_controller.CurrentTrailOrdered.activityTrail.ResultListCache = results;
            }
            else
            {
                ((List<TreeList.TreeListNode>)results).Sort();
            }
            return results;
        }

        public static TrailResult getTrailResultRow(object element)
        {
            return (TrailResult)((TreeList.TreeListNode)element).Element;
        }

        public static IList<TrailResult> getTrailResultSelection(System.Collections.IList tlist)
        {
            IList<TrailResult> aTr = new List<TrailResult>();
            if (tlist != null)
            {
                foreach (object t in tlist)
                {
                    object t2 = t;
                    if (t != null && t is TreeList.TreeListNode)
                    {
                        t2 = (object)(t as TreeList.TreeListNode).Element;
                    }
                    if (t2 != null && t2 is TrailResult)
                    {
                        TrailResult tr = t2 as TrailResult;
                        aTr.Add(tr);
                    }
                }
            }
            return aTr;
        }

        private void summaryList_Sort()
        {
            if (m_controller.CurrentTrailOrdered != null)
            {
                summaryList.RowData = getTreeListNodeSplits();
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
                    IList<TrailResult> aTr = getTrailResultSelection(l.SelectedItems);
                    m_page.MarkTrack(TrailResultMarked.TrailResultMarkAll(aTr));
                }
            }
        }

        private void List_SelectedChanged(object sender, EventArgs e)
        {
            m_page.RefreshChart();
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

        /*************************************************************************************************************/
        void listMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //TODO: set selected result when opening
            string refRes="";
            if (m_controller.ReferenceTrailResult != null)
            {
                refRes = m_controller.ReferenceTrailResult.FirstTime.ToLocalTime().ToShortDateString() +
                   " "+ m_controller.ReferenceTrailResult.FirstTime.ToLocalTime().ToShortTimeString();
            }
            this.referenceTrailMenuItem.Text = string.Format(
                Properties.Resources.UI_Activity_List_ReferenceResult, refRes);
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
			dialog.ColumnsAvailable = TrailResultColumnIds.ColumnDefs_ST2(m_controller.FirstActivity, false);
#else
            ListSettingsDialog dialog = new ListSettingsDialog();
            dialog.AvailableColumns = TrailResultColumnIds.ColumnDefs(m_controller.ReferenceActivity, m_controller.Activities.Count > 1);
#endif
            dialog.ThemeChanged(m_visualTheme);
            dialog.AllowFixedColumnSelect = true;
            dialog.SelectedColumns = PluginMain.Settings.ActivityPageColumns;
            dialog.NumFixedColumns = PluginMain.Settings.ActivityPageNumFixedColumns;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                PluginMain.Settings.ActivityPageNumFixedColumns = dialog.NumFixedColumns;
                PluginMain.Settings.ActivityPageColumns = dialog.SelectedColumns;
                RefreshColumns();
            }
        }

        void selectSimilarSplitsMenuItem_Click(object sender, System.EventArgs e)
        {
            if (summaryList.SelectedItems != null)
            {
                System.Collections.IList results = new List<TreeList.TreeListNode>();
                foreach (object t in summaryList.SelectedItems)
                {
                    object t2 = t;
                    TreeList.TreeListNode tn = t as TreeList.TreeListNode;
                    int splitIndex = -1;
                    if (tn.Parent != null)
                    {
                        if (t != null && t is TreeList.TreeListNode)
                        {
                            t2 = (object)(t as TreeList.TreeListNode).Element;
                        }
                        if (t2 != null && t2 is TrailResult)
                        {
                            //Default select all "main" trails, here select from number
                            splitIndex = ((TrailResult)t2).Order;
                        }
                    }
                    foreach (TreeList.TreeListNode rtn in (IList<TreeList.TreeListNode>)summaryList.RowData)
                    {
                        if (splitIndex < 0)
                        {
                            results.Add(rtn);
                        }
                        else
                        {
                            foreach (TreeList.TreeListNode ctn in rtn.Children)
                            {
                                if (ctn.Element is TrailResult)
                                {
                                    if (((TrailResult)ctn.Element).Order == splitIndex)
                                    {
                                        results.Add(ctn);
                                    }
                                }
                            }
                        }
                    }
                }
#if ST_2_1
                summaryList.Selected = results;
#else
                summaryList.SelectedItems = results;
#endif
            }
        }

        void selectActivityMenuItem_Click(object sender, System.EventArgs e)
        {
#if !ST_2_1
            if (summaryList.SelectedItems != null && summaryList.SelectedItems.Count > 0)
            {
                IList<TrailResult> atr = getTrailResultSelection(summaryList.SelectedItems);
                IList<IActivity> aAct = new List<IActivity>();
                foreach (TrailResult tr in atr)
                {
                    aAct.Add(tr.Activity);
                }
                m_view.SelectionProvider.SelectedItems = (List<IActivity>)aAct;
            }
#endif
        }

        void referenceTrailMenuItem_Click(object sender, System.EventArgs e)
        {
            IList<TrailResult> atr = getTrailResultSelection(summaryList.SelectedItems);
            if (atr != null && atr.Count > 0)
            {
                m_controller.ReferenceTrailResult = atr[0];
            }
        }

        void selectWithURMenuItem_Click(object sender, System.EventArgs e)
        {
            //TODO: Also set name/time?
            throw new System.NotImplementedException();
        }

    }
}
