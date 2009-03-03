using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.SportTracks.Data;
using ZoneFiveSoftware.SportTracks.Properties;
using ZoneFiveSoftware.SportTracks.UI;
using ZoneFiveSoftware.SportTracks.UI.Controls;

namespace TrailsPlugin.UI.Activity {
	public partial class ListSettings : Form {

/*    
        private bool allowFixedColumnSelect;
        private ZoneFiveSoftware.Common.Visuals.Button btnCancel;
        private ZoneFiveSoftware.Common.Visuals.Button btnOk;
        private ZoneFiveSoftware.SportTracks.Data.IListItem fixedNone;
        private System.Collections.IList fixedPopupColumns;
        private System.Collections.Generic.IList<ZoneFiveSoftware.SportTracks.UI.GroupByInfo> groupByList;
        private ZoneFiveSoftware.SportTracks.UI.Controls.ItemListArranger m_itemListArranger;
        private System.Windows.Forms.Label labelFixedColumn;
        private System.Windows.Forms.Label labelGroupBy;
        private int numFixedColumns;
        private string selectedGroupBy;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private ZoneFiveSoftware.Common.Visuals.ITheme theme;
        private ZoneFiveSoftware.Common.Visuals.TextBox txtFixedColumn;
        private ZoneFiveSoftware.Common.Visuals.TextBox txtGroupBy;

        private static System.Drawing.Size windowSize;

        public string AddButtonLabel
        {
            set
            {
                m_itemListArranger.AddButtonLabel = value;
            }
        }

        public bool AllowFixedColumnSelect
        {
            get
            {
                return allowFixedColumnSelect;
            }
            set
            {
                if (allowFixedColumnSelect != value)
                {
                    allowFixedColumnSelect = value;
                    tableLayoutPanelMain.RowStyles[1].Height = (float)(allowFixedColumnSelect ? 27 : 0);
                }
            }
        }

        public bool AllowZeroSelected
        {
            get
            {
                return m_itemListArranger.AllowZeroSelected;
            }
            set
            {
                m_itemListArranger.AllowZeroSelected = value;
            }
        }

        public System.Collections.Generic.ICollection<ZoneFiveSoftware.SportTracks.Data.IListItem> ColumnsAvailable
        {
            set
            {
                m_itemListArranger.ItemsAvailable = value;
            }
        }

        public System.Collections.Generic.IList<ZoneFiveSoftware.SportTracks.UI.GroupByInfo> GroupByList
        {
            set
            {
                groupByList = value;
                tableLayoutPanelMain.RowStyles[2].Height = (float)((groupByList != null) && (groupByList.Count > 0) ? 24 : 0);
                RefreshGroupByList();
            }
        }

        public int NumFixedColumns
        {
            get
            {
                return numFixedColumns;
            }
            set
            {
                if (numFixedColumns != value)
                {
                    numFixedColumns = value;
                    RefreshFixedCombo();
                }
            }
        }

        public System.Collections.Generic.IList<string> SelectedColumns
        {
            get
            {
                return m_itemListArranger.SelectedItems;
            }
            set
            {
                m_itemListArranger.SelectedItems = value;
                RefreshFixedCombo();
            }
        }

        public string SelectedGroupBy
        {
            get
            {
                return selectedGroupBy;
            }
            set
            {
                selectedGroupBy = value;
                RefreshGroupByList();
            }
        }

        public string SelectedItemListLabel
        {
            set
            {
                m_itemListArranger.SelectedItemListLabel = value;
            }
        }

        public ListSettings()
        {
            allowFixedColumnSelect = true;
            fixedPopupColumns = new System.Collections.ArrayList();
            InitializeComponent();
            fixedNone = new ZoneFiveSoftware.SportTracks.UI.ListItemInfo("", ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelNone, null, 0, System.Drawing.StringAlignment.Near);
            labelFixedColumn.Text = ZoneFiveSoftware.SportTracks.Properties.xxx.UI_Forms_ListSettings_labelFixedColumn_Text;
            labelGroupBy.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelGroupBy + ":";
            btnOk.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionOk;
            btnCancel.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionCancel;
            Text = ZoneFiveSoftware.SportTracks.Properties.xxx.UI_Froms_ListSettings_Title;
            m_itemListArranger.ListChanged += new System.EventHandler(itemListArranger_ListChanged);
            txtFixedColumn.ButtonClick += new System.EventHandler(txtFixedColumn_ButtonClick);
            txtGroupBy.ButtonClick += new System.EventHandler(txtGroupBy_ButtonClick);
            btnOk.Click += new System.EventHandler(btnOk_Click);
            btnCancel.Click += new System.EventHandler(btnCancel_Click);
            tableLayoutPanelMain.RowStyles[2].Height = 0.0F;
            Size = ZoneFiveSoftware.SportTracks.UI.Forms.ListSettings.windowSize;
            RightToLeft = System.Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;
        }

        static ListSettings()
        {
            ZoneFiveSoftware.SportTracks.UI.Forms.ListSettings.windowSize = new System.Drawing.Size(500, 400);
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            // trial
        }

        private void btnOk_Click(object sender, System.EventArgs e)
        {
            ZoneFiveSoftware.SportTracks.UI.Forms.ListSettings.windowSize = Size;
            DialogResult = btnOk.DialogResult;
            Close();
        }


        private void itemListArranger_ListChanged(object sender, System.EventArgs e)
        {
            // trial
        }

        private void popupFixed_ItemSelected(object sender, ZoneFiveSoftware.Common.Visuals.TreeListPopup.ItemSelectedEventArgs e)
        {
            // trial
        }

        private void popupGroupBy_ItemSelected(object sender, ZoneFiveSoftware.Common.Visuals.TreeListPopup.ItemSelectedEventArgs e)
        {
            // trial
        }

        private void RefreshFixedCombo()
        {
            numFixedColumns = System.Math.Min(numFixedColumns, m_itemListArranger.SelectedItems.Count);
            if (numFixedColumns == 0)
            {
                txtFixedColumn.Text = fixedNone.Text(fixedNone.Id);
                return;
            }
            string s = m_itemListArranger.SelectedItems[numFixedColumns - 1];
            using (System.Collections.Generic.IEnumerator<ZoneFiveSoftware.SportTracks.Data.IListItem> ienumerator = m_itemListArranger.ItemsAvailable.GetEnumerator())
            {
                while (ienumerator.MoveNext())
                {
                    ZoneFiveSoftware.SportTracks.Data.IListItem ilistItem = ienumerator.Current;
                    if (ilistItem.Id.Equals(s))
                    {
                        txtFixedColumn.Text = ilistItem.Text(ilistItem.Id);
                        break;
                    }
                }
            }
        }

        private void RefreshGroupByList()
        {
            // trial
        }

        public void ThemeChanged(ZoneFiveSoftware.Common.Visuals.ITheme theme)
        {
            // trial
        }

        private void txtFixedColumn_ButtonClick(object sender, System.EventArgs e)
        {
            fixedPopupColumns.Clear();
            fixedPopupColumns.Add(fixedNone);
            System.Collections.Generic.IList<string> ilist = m_itemListArranger.SelectedItems;
            System.Collections.Generic.ICollection<ZoneFiveSoftware.SportTracks.Data.IListItem> icollection = m_itemListArranger.ItemsAvailable;
            using (System.Collections.Generic.IEnumerator<string> ienumerator = ilist.GetEnumerator())
            {
                while (ienumerator.MoveNext())
                {
                    string s = ienumerator.Current;
                    using (System.Collections.Generic.IEnumerator<ZoneFiveSoftware.SportTracks.Data.IListItem> ienumerator1 = icollection.GetEnumerator())
                    {
                        while (ienumerator1.MoveNext())
                        {
                            ZoneFiveSoftware.SportTracks.Data.IListItem ilistItem = ienumerator1.Current;
                            if (ilistItem.Id.Equals(s))
                            {
                                fixedPopupColumns.Add(ilistItem);
                                break;
                            }
                        }
                    }
                }
            }
            ZoneFiveSoftware.Common.Visuals.TreeListPopup treeListPopup = new ZoneFiveSoftware.Common.Visuals.TreeListPopup();
            treeListPopup.ThemeChanged(theme);
            treeListPopup.Tree.Columns.Add(new ZoneFiveSoftware.Common.Visuals.TreeList.Column());
            treeListPopup.Tree.RowData = fixedPopupColumns;
            object[] objArr = new object[] { fixedPopupColumns[numFixedColumns] };
            treeListPopup.Tree.Selected = objArr;
            treeListPopup.ItemSelected += new ZoneFiveSoftware.Common.Visuals.TreeListPopup.ItemSelectedEventHandler(popupFixed_ItemSelected);
            treeListPopup.Popup(txtFixedColumn.Parent.RectangleToScreen(txtFixedColumn.Bounds));
        }

        private void txtGroupBy_ButtonClick(object sender, System.EventArgs e)
        {
            ZoneFiveSoftware.Common.Visuals.TreeListPopup treeListPopup = new ZoneFiveSoftware.Common.Visuals.TreeListPopup();
            treeListPopup.ThemeChanged(theme);
            treeListPopup.Tree.Columns.Add(new ZoneFiveSoftware.Common.Visuals.TreeList.Column());
            treeListPopup.Tree.RowData = groupByList;
            using (System.Collections.Generic.IEnumerator<ZoneFiveSoftware.SportTracks.UI.GroupByInfo> ienumerator = groupByList.GetEnumerator())
            {
                while (ienumerator.MoveNext())
                {
                    ZoneFiveSoftware.SportTracks.UI.GroupByInfo groupByInfo = ienumerator.Current;
                    if (groupByInfo.Id == selectedGroupBy)
                    {
                        object[] objArr = new object[] { groupByInfo };
                        treeListPopup.Tree.Selected = objArr;
                        break;
                    }
                }
            }
            treeListPopup.ItemSelected += new ZoneFiveSoftware.Common.Visuals.TreeListPopup.ItemSelectedEventHandler(popupGroupBy_ItemSelected);
            treeListPopup.Popup(txtGroupBy.Parent.RectangleToScreen(txtGroupBy.Bounds));
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            ZoneFiveSoftware.Common.Visuals.MessageDialog.DrawButtonRowBackground(e.Graphics, ClientRectangle, theme);
        }
*/
    } // class ListSettings

}


