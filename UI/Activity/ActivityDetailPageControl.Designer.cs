namespace TrailsPlugin.UI.Activity {
	partial class ActivityDetailPageControl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.Panel = new System.Windows.Forms.TableLayoutPanel();
			this.lblTrail = new System.Windows.Forms.Label();
			this.btnAdd = new ZoneFiveSoftware.Common.Visuals.Button();
			this.TrailName = new ZoneFiveSoftware.Common.Visuals.TextBox();
			this.btnDelete = new ZoneFiveSoftware.Common.Visuals.Button();
			this.btnEdit = new ZoneFiveSoftware.Common.Visuals.Button();
			this.SplitContainer = new System.Windows.Forms.SplitContainer();
			this.List = new ZoneFiveSoftware.Common.Visuals.TreeList();
			this.ChartPanel = new System.Windows.Forms.TableLayoutPanel();
			this.ChartBanner = new ZoneFiveSoftware.Common.Visuals.ActionBanner();
			this.lineChart1 = new ZoneFiveSoftware.Common.Visuals.Chart.LineChart();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.listContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.listSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.Panel.SuspendLayout();
			this.SplitContainer.Panel1.SuspendLayout();
			this.SplitContainer.Panel2.SuspendLayout();
			this.SplitContainer.SuspendLayout();
			this.ChartPanel.SuspendLayout();
			this.listContextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// Panel
			// 
			this.Panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.Panel.ColumnCount = 6;
			this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
			this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
			this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.Panel.Controls.Add(this.lblTrail, 0, 0);
			this.Panel.Controls.Add(this.btnAdd, 3, 0);
			this.Panel.Controls.Add(this.TrailName, 1, 0);
			this.Panel.Controls.Add(this.btnDelete, 5, 0);
			this.Panel.Controls.Add(this.btnEdit, 4, 0);
			this.Panel.Controls.Add(this.SplitContainer, 0, 2);
			this.Panel.Location = new System.Drawing.Point(0, 0);
			this.Panel.Margin = new System.Windows.Forms.Padding(0);
			this.Panel.Name = "Panel";
			this.Panel.RowCount = 3;
			this.Panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.Panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
			this.Panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.Panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.Panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.Panel.Size = new System.Drawing.Size(350, 302);
			this.Panel.TabIndex = 8;
			// 
			// lblTrail
			// 
			this.lblTrail.AutoSize = true;
			this.lblTrail.Location = new System.Drawing.Point(0, 0);
			this.lblTrail.Margin = new System.Windows.Forms.Padding(0);
			this.lblTrail.Name = "lblTrail";
			this.lblTrail.Size = new System.Drawing.Size(30, 13);
			this.lblTrail.TabIndex = 0;
			this.lblTrail.Text = "Trail:";
			this.lblTrail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnAdd
			// 
			this.btnAdd.BackColor = System.Drawing.Color.Transparent;
			this.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnAdd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
			this.btnAdd.CenterImage = null;
			this.btnAdd.DialogResult = System.Windows.Forms.DialogResult.None;
			this.btnAdd.HyperlinkStyle = false;
			this.btnAdd.ImageMargin = 2;
			this.btnAdd.LeftImage = null;
			this.btnAdd.Location = new System.Drawing.Point(268, 3);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Padding = new System.Windows.Forms.Padding(2);
			this.btnAdd.PushStyle = true;
			this.btnAdd.RightImage = null;
			this.btnAdd.Size = new System.Drawing.Size(19, 19);
			this.btnAdd.TabIndex = 10;
			this.btnAdd.Text = "A";
			this.btnAdd.TextAlign = System.Drawing.StringAlignment.Center;
			this.btnAdd.TextLeftMargin = 2;
			this.btnAdd.TextRightMargin = 2;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// TrailName
			// 
			this.TrailName.AcceptsReturn = false;
			this.TrailName.AcceptsTab = false;
			this.TrailName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.TrailName.BackColor = System.Drawing.Color.White;
			this.TrailName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
			this.TrailName.ButtonImage = null;
			this.TrailName.Location = new System.Drawing.Point(91, 0);
			this.TrailName.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.TrailName.MaxLength = 32767;
			this.TrailName.Multiline = false;
			this.TrailName.Name = "TrailName";
			this.TrailName.ReadOnly = true;
			this.TrailName.ReadOnlyColor = System.Drawing.SystemColors.Control;
			this.TrailName.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
			this.TrailName.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.TrailName.Size = new System.Drawing.Size(149, 19);
			this.TrailName.TabIndex = 15;
			this.TrailName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			this.TrailName.ButtonClick += new System.EventHandler(this.TrailName_ButtonClick);
			// 
			// btnDelete
			// 
			this.btnDelete.BackColor = System.Drawing.Color.Transparent;
			this.btnDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnDelete.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
			this.btnDelete.CenterImage = null;
			this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.None;
			this.btnDelete.HyperlinkStyle = false;
			this.btnDelete.ImageMargin = 2;
			this.btnDelete.LeftImage = null;
			this.btnDelete.Location = new System.Drawing.Point(318, 3);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Padding = new System.Windows.Forms.Padding(2);
			this.btnDelete.PushStyle = true;
			this.btnDelete.RightImage = null;
			this.btnDelete.Size = new System.Drawing.Size(20, 19);
			this.btnDelete.TabIndex = 13;
			this.btnDelete.Text = "D";
			this.btnDelete.TextAlign = System.Drawing.StringAlignment.Center;
			this.btnDelete.TextLeftMargin = 2;
			this.btnDelete.TextRightMargin = 2;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnEdit
			// 
			this.btnEdit.BackColor = System.Drawing.Color.Transparent;
			this.btnEdit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnEdit.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
			this.btnEdit.CenterImage = null;
			this.btnEdit.DialogResult = System.Windows.Forms.DialogResult.None;
			this.btnEdit.HyperlinkStyle = false;
			this.btnEdit.ImageMargin = 2;
			this.btnEdit.LeftImage = null;
			this.btnEdit.Location = new System.Drawing.Point(293, 3);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Padding = new System.Windows.Forms.Padding(2);
			this.btnEdit.PushStyle = true;
			this.btnEdit.RightImage = null;
			this.btnEdit.Size = new System.Drawing.Size(19, 19);
			this.btnEdit.TabIndex = 17;
			this.btnEdit.Text = "E";
			this.btnEdit.TextAlign = System.Drawing.StringAlignment.Center;
			this.btnEdit.TextLeftMargin = 2;
			this.btnEdit.TextRightMargin = 2;
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// SplitContainer
			// 
			this.Panel.SetColumnSpan(this.SplitContainer, 6);
			this.SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SplitContainer.Location = new System.Drawing.Point(0, 35);
			this.SplitContainer.Margin = new System.Windows.Forms.Padding(0);
			this.SplitContainer.Name = "SplitContainer";
			this.SplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// SplitContainer.Panel1
			// 
			this.SplitContainer.Panel1.Controls.Add(this.List);
			this.SplitContainer.Panel1.Cursor = System.Windows.Forms.Cursors.Default;
			this.SplitContainer.Panel1MinSize = 100;
			// 
			// SplitContainer.Panel2
			// 
			this.SplitContainer.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.SplitContainer.Panel2.Controls.Add(this.ChartPanel);
			this.SplitContainer.Panel2MinSize = 100;
			this.SplitContainer.Size = new System.Drawing.Size(350, 267);
			this.SplitContainer.SplitterDistance = 150;
			this.SplitContainer.TabIndex = 18;
			// 
			// List
			// 
			this.List.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.List.AutoScroll = true;
			this.List.BackColor = System.Drawing.Color.Transparent;
			this.List.Border = ZoneFiveSoftware.Common.Visuals.ControlBorder.Style.SmallRoundShadow;
			this.List.CheckBoxes = false;
			this.List.ContextMenuStrip = this.listContextMenuStrip;
			this.List.DefaultIndent = 15;
			this.List.DefaultRowHeight = -1;
			this.List.HeaderRowHeight = 21;
			this.List.Location = new System.Drawing.Point(0, 0);
			this.List.Margin = new System.Windows.Forms.Padding(0);
			this.List.MultiSelect = false;
			this.List.Name = "List";
			this.List.NumHeaderRows = ZoneFiveSoftware.Common.Visuals.TreeList.HeaderRows.One;
			this.List.NumLockedColumns = 0;
			this.List.RowAlternatingColors = true;
			this.List.RowHotlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this.List.RowHotlightColorText = System.Drawing.SystemColors.HighlightText;
			this.List.RowHotlightMouse = true;
			this.List.RowSelectedColor = System.Drawing.SystemColors.Highlight;
			this.List.RowSelectedColorText = System.Drawing.SystemColors.HighlightText;
			this.List.RowSeparatorLines = true;
			this.List.ShowLines = false;
			this.List.ShowPlusMinus = false;
			this.List.Size = new System.Drawing.Size(350, 150);
			this.List.TabIndex = 11;
			this.List.SelectedChanged += new System.EventHandler(this.List_SelectedChanged);
			// 
			// ChartPanel
			// 
			this.ChartPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ChartPanel.ColumnCount = 1;
			this.ChartPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.ChartPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.ChartPanel.Controls.Add(this.ChartBanner, 0, 0);
			this.ChartPanel.Controls.Add(this.lineChart1, 0, 1);
			this.ChartPanel.Location = new System.Drawing.Point(0, 0);
			this.ChartPanel.Margin = new System.Windows.Forms.Padding(0);
			this.ChartPanel.Name = "ChartPanel";
			this.ChartPanel.RowCount = 2;
			this.ChartPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.ChartPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.ChartPanel.Size = new System.Drawing.Size(347, 113);
			this.ChartPanel.TabIndex = 0;
			// 
			// ChartBanner
			// 
			this.ChartBanner.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ChartBanner.BackColor = System.Drawing.SystemColors.Control;
			this.ChartBanner.ContextMenuStrip = this.listContextMenuStrip;
			this.ChartBanner.HasMenuButton = true;
			this.ChartBanner.Location = new System.Drawing.Point(0, 0);
			this.ChartBanner.Margin = new System.Windows.Forms.Padding(0);
			this.ChartBanner.Name = "ChartBanner";
			this.ChartBanner.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.ChartBanner.Size = new System.Drawing.Size(347, 20);
			this.ChartBanner.Style = ZoneFiveSoftware.Common.Visuals.ActionBanner.BannerStyle.Header2;
			this.ChartBanner.TabIndex = 0;
			this.ChartBanner.Text = "Chart Name";
			this.ChartBanner.UseStyleFont = true;
			// 
			// lineChart1
			// 
			this.lineChart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lineChart1.BackColor = System.Drawing.Color.White;
			this.lineChart1.Border = ZoneFiveSoftware.Common.Visuals.ControlBorder.Style.None;
			this.lineChart1.Location = new System.Drawing.Point(3, 23);
			this.lineChart1.Name = "lineChart1";
			this.lineChart1.Size = new System.Drawing.Size(341, 117);
			this.lineChart1.TabIndex = 1;
			// 
			// listContextMenuStrip
			// 
			this.listContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.listSettingsToolStripMenuItem});
			this.listContextMenuStrip.Name = "listContextMenuStrip";
			this.listContextMenuStrip.Size = new System.Drawing.Size(147, 26);
			// 
			// listSettingsToolStripMenuItem
			// 
			this.listSettingsToolStripMenuItem.Name = "listSettingsToolStripMenuItem";
			this.listSettingsToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
			this.listSettingsToolStripMenuItem.Text = "List Settings...";
			this.listSettingsToolStripMenuItem.Click += new System.EventHandler(this.listSettingsToolStripMenuItem_Click);
			// 
			// ActivityDetailPageControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.Panel);
			this.Name = "ActivityDetailPageControl";
			this.Size = new System.Drawing.Size(350, 302);
			this.Panel.ResumeLayout(false);
			this.Panel.PerformLayout();
			this.SplitContainer.Panel1.ResumeLayout(false);
			this.SplitContainer.Panel2.ResumeLayout(false);
			this.SplitContainer.ResumeLayout(false);
			this.ChartPanel.ResumeLayout(false);
			this.listContextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel Panel;
		private System.Windows.Forms.Label lblTrail;
		private ZoneFiveSoftware.Common.Visuals.TreeList List;
		private ZoneFiveSoftware.Common.Visuals.Button btnDelete;
		private ZoneFiveSoftware.Common.Visuals.Button btnAdd;
		private ZoneFiveSoftware.Common.Visuals.TextBox TrailName;
		private ZoneFiveSoftware.Common.Visuals.Button btnEdit;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.SplitContainer SplitContainer;
		private System.Windows.Forms.TableLayoutPanel ChartPanel;
		private ZoneFiveSoftware.Common.Visuals.ActionBanner ChartBanner;
		private ZoneFiveSoftware.Common.Visuals.Chart.LineChart lineChart1;
		private System.Windows.Forms.ContextMenuStrip listContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem listSettingsToolStripMenuItem;

	}
}
