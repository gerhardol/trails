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
			this.listMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.listSettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ChartPanel = new System.Windows.Forms.TableLayoutPanel();
			this.ChartBanner = new ZoneFiveSoftware.Common.Visuals.ActionBanner();
			this.detailMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.speedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.paceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.heartRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cadenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.elevationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.gradeStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.powerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.distanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.timeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.btnExpand = new ZoneFiveSoftware.Common.Visuals.Button();
			this.LineChart = new TrailsPlugin.UI.Activity.TrailLineChart();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.Panel.SuspendLayout();
			this.SplitContainer.Panel1.SuspendLayout();
			this.SplitContainer.Panel2.SuspendLayout();
			this.SplitContainer.SuspendLayout();
			this.listMenu.SuspendLayout();
			this.ChartPanel.SuspendLayout();
			this.ChartBanner.SuspendLayout();
			this.detailMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// Panel
			// 
			this.Panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.Panel.ColumnCount = 6;
			this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
			this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
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
			this.Panel.Size = new System.Drawing.Size(400, 300);
			this.Panel.TabIndex = 8;
			// 
			// lblTrail
			// 
			this.lblTrail.AutoSize = true;
			this.lblTrail.Location = new System.Drawing.Point(0, 3);
			this.lblTrail.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
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
			this.btnAdd.Location = new System.Drawing.Point(328, 3);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Padding = new System.Windows.Forms.Padding(2);
			this.btnAdd.PushStyle = false;
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
			this.TrailName.Size = new System.Drawing.Size(209, 19);
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
			this.btnDelete.Location = new System.Drawing.Point(378, 3);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Padding = new System.Windows.Forms.Padding(2);
			this.btnDelete.PushStyle = true;
			this.btnDelete.RightImage = null;
			this.btnDelete.Size = new System.Drawing.Size(19, 19);
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
			this.btnEdit.Location = new System.Drawing.Point(353, 3);
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
			this.SplitContainer.Size = new System.Drawing.Size(400, 265);
			this.SplitContainer.SplitterDistance = 100;
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
			this.List.ContextMenuStrip = this.listMenu;
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
			this.List.Size = new System.Drawing.Size(400, 100);
			this.List.TabIndex = 11;
			this.List.SelectedChanged += new System.EventHandler(this.List_SelectedChanged);
			// 
			// listMenu
			// 
			this.listMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.listSettingsMenuItem});
			this.listMenu.Name = "listContextMenuStrip";
			this.listMenu.Size = new System.Drawing.Size(147, 26);
			// 
			// listSettingsMenuItem
			// 
			this.listSettingsMenuItem.Name = "listSettingsMenuItem";
			this.listSettingsMenuItem.Size = new System.Drawing.Size(146, 22);
			this.listSettingsMenuItem.Text = "List Settings...";
			this.listSettingsMenuItem.Click += new System.EventHandler(this.listSettingsToolStripMenuItem_Click);
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
			this.ChartPanel.Controls.Add(this.LineChart, 0, 1);
			this.ChartPanel.Location = new System.Drawing.Point(0, 0);
			this.ChartPanel.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
			this.ChartPanel.Name = "ChartPanel";
			this.ChartPanel.RowCount = 2;
			this.ChartPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.ChartPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.ChartPanel.Size = new System.Drawing.Size(400, 161);
			this.ChartPanel.TabIndex = 0;
			// 
			// ChartBanner
			// 
			this.ChartBanner.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ChartBanner.BackColor = System.Drawing.SystemColors.Control;
			this.ChartBanner.ContextMenuStrip = this.detailMenu;
			this.ChartBanner.Controls.Add(this.btnExpand);
			this.ChartBanner.HasMenuButton = true;
			this.ChartBanner.Location = new System.Drawing.Point(0, 0);
			this.ChartBanner.Margin = new System.Windows.Forms.Padding(0);
			this.ChartBanner.Name = "ChartBanner";
			this.ChartBanner.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.ChartBanner.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.ChartBanner.Size = new System.Drawing.Size(400, 20);
			this.ChartBanner.Style = ZoneFiveSoftware.Common.Visuals.ActionBanner.BannerStyle.Header2;
			this.ChartBanner.TabIndex = 0;
			this.ChartBanner.UseStyleFont = true;
			this.ChartBanner.MenuClicked += new System.EventHandler(this.ChartBanner_MenuClicked);
			// 
			// detailMenu
			// 
			this.detailMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.speedToolStripMenuItem,
            this.paceToolStripMenuItem,
            this.heartRateToolStripMenuItem,
            this.cadenceToolStripMenuItem,
            this.elevationToolStripMenuItem,
            this.gradeStripMenuItem1,
            this.powerToolStripMenuItem,
            this.toolStripSeparator1,
            this.distanceToolStripMenuItem,
            this.timeToolStripMenuItem});
			this.detailMenu.Name = "detailMenu";
			this.detailMenu.Size = new System.Drawing.Size(130, 208);
			// 
			// speedToolStripMenuItem
			// 
			this.speedToolStripMenuItem.Name = "speedToolStripMenuItem";
			this.speedToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.speedToolStripMenuItem.Text = "Speed";
			this.speedToolStripMenuItem.Click += new System.EventHandler(this.speedToolStripMenuItem_Click);
			// 
			// paceToolStripMenuItem
			// 
			this.paceToolStripMenuItem.Name = "paceToolStripMenuItem";
			this.paceToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.paceToolStripMenuItem.Text = "Pace";
			this.paceToolStripMenuItem.Click += new System.EventHandler(this.paceToolStripMenuItem_Click);
			// 
			// heartRateToolStripMenuItem
			// 
			this.heartRateToolStripMenuItem.Name = "heartRateToolStripMenuItem";
			this.heartRateToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.heartRateToolStripMenuItem.Text = "Heart Rate";
			this.heartRateToolStripMenuItem.Click += new System.EventHandler(this.heartRateToolStripMenuItem_Click);
			// 
			// cadenceToolStripMenuItem
			// 
			this.cadenceToolStripMenuItem.Name = "cadenceToolStripMenuItem";
			this.cadenceToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.cadenceToolStripMenuItem.Text = "Cadence";
			this.cadenceToolStripMenuItem.Click += new System.EventHandler(this.cadenceToolStripMenuItem_Click);
			// 
			// elevationToolStripMenuItem
			// 
			this.elevationToolStripMenuItem.Name = "elevationToolStripMenuItem";
			this.elevationToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.elevationToolStripMenuItem.Text = "Elevation";
			this.elevationToolStripMenuItem.Click += new System.EventHandler(this.elevationToolStripMenuItem_Click);
			// 
			// gradeStripMenuItem1
			// 
			this.gradeStripMenuItem1.Name = "gradeStripMenuItem1";
			this.gradeStripMenuItem1.Size = new System.Drawing.Size(129, 22);
			this.gradeStripMenuItem1.Text = "Grade";
			this.gradeStripMenuItem1.Click += new System.EventHandler(this.gradeToolStripMenuItem_Click);
			// 
			// powerToolStripMenuItem
			// 
			this.powerToolStripMenuItem.Name = "powerToolStripMenuItem";
			this.powerToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.powerToolStripMenuItem.Text = "Power";
			this.powerToolStripMenuItem.Click += new System.EventHandler(this.powerToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(126, 6);
			// 
			// distanceToolStripMenuItem
			// 
			this.distanceToolStripMenuItem.Name = "distanceToolStripMenuItem";
			this.distanceToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.distanceToolStripMenuItem.Text = "Distance";
			this.distanceToolStripMenuItem.Click += new System.EventHandler(this.distanceToolStripMenuItem_Click);
			// 
			// timeToolStripMenuItem
			// 
			this.timeToolStripMenuItem.Name = "timeToolStripMenuItem";
			this.timeToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.timeToolStripMenuItem.Text = "Time";
			this.timeToolStripMenuItem.Click += new System.EventHandler(this.timeToolStripMenuItem_Click);
			// 
			// btnExpand
			// 
			this.btnExpand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnExpand.BackColor = System.Drawing.Color.Transparent;
			this.btnExpand.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnExpand.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
			this.btnExpand.CenterImage = null;
			this.btnExpand.DialogResult = System.Windows.Forms.DialogResult.None;
			this.btnExpand.HyperlinkStyle = false;
			this.btnExpand.ImageMargin = 2;
			this.btnExpand.LeftImage = null;
			this.btnExpand.Location = new System.Drawing.Point(353, 1);
			this.btnExpand.Name = "btnExpand";
			this.btnExpand.Padding = new System.Windows.Forms.Padding(2);
			this.btnExpand.PushStyle = false;
			this.btnExpand.RightImage = null;
			this.btnExpand.Size = new System.Drawing.Size(19, 19);
			this.btnExpand.TabIndex = 11;
			this.btnExpand.Text = "X";
			this.btnExpand.TextAlign = System.Drawing.StringAlignment.Center;
			this.btnExpand.TextLeftMargin = 2;
			this.btnExpand.TextRightMargin = 2;
			this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
			// 
			// LineChart
			// 
			this.LineChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.LineChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
			this.LineChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
			this.LineChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
			this.LineChart.Location = new System.Drawing.Point(0, 20);
			this.LineChart.Margin = new System.Windows.Forms.Padding(0);
			this.LineChart.Name = "LineChart";
			this.LineChart.Size = new System.Drawing.Size(400, 141);
			this.LineChart.TabIndex = 1;
			this.LineChart.TrailResult = null;
			this.LineChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
			this.LineChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Speed;
			// 
			// ActivityDetailPageControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.Panel);
			this.Name = "ActivityDetailPageControl";
			this.Size = new System.Drawing.Size(400, 300);
			this.SizeChanged += new System.EventHandler(this.ActivityDetailPageControl_SizeChanged);
			this.Panel.ResumeLayout(false);
			this.Panel.PerformLayout();
			this.SplitContainer.Panel1.ResumeLayout(false);
			this.SplitContainer.Panel2.ResumeLayout(false);
			this.SplitContainer.ResumeLayout(false);
			this.listMenu.ResumeLayout(false);
			this.ChartPanel.ResumeLayout(false);
			this.ChartBanner.ResumeLayout(false);
			this.detailMenu.ResumeLayout(false);
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
		private System.Windows.Forms.ContextMenuStrip listMenu;
		private System.Windows.Forms.ToolStripMenuItem listSettingsMenuItem;
		private System.Windows.Forms.ContextMenuStrip detailMenu;
		private TrailLineChart LineChart;
		private System.Windows.Forms.ToolStripMenuItem speedToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem elevationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem heartRateToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cadenceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem powerToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem distanceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem timeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem gradeStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem paceToolStripMenuItem;
		private ZoneFiveSoftware.Common.Visuals.Button btnExpand;

	}
}
