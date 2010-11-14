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
            this.ActPagePanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblTrail = new System.Windows.Forms.Label();
            this.TrailName = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.btnAdd = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnEdit = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnDelete = new ZoneFiveSoftware.Common.Visuals.Button();
            this.ActPageSplitContainer = new System.Windows.Forms.SplitContainer();
            this.summaryList = new ZoneFiveSoftware.Common.Visuals.TreeList();
            this.listMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyTableMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listSettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ChartPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ChartBanner = new ZoneFiveSoftware.Common.Visuals.ActionBanner();
            this.detailMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.speedPaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.speedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.heartRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cadenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elevationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gradeStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.powerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.distanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.showToolBarMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnExpand = new ZoneFiveSoftware.Common.Visuals.Button();
            this.LineChart = new TrailsPlugin.UI.Activity.TrailLineChart();
            this.ExpandSplitContainer = new System.Windows.Forms.SplitContainer();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ActPagePanel.SuspendLayout();
            this.ActPageSplitContainer.Panel1.SuspendLayout();
            this.ActPageSplitContainer.Panel2.SuspendLayout();
            this.ActPageSplitContainer.SuspendLayout();
            this.listMenu.SuspendLayout();
            this.ChartPanel.SuspendLayout();
            this.ChartBanner.SuspendLayout();
            this.detailMenu.SuspendLayout();
            this.ExpandSplitContainer.Panel1.SuspendLayout();
            this.ExpandSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // ActPagePanel
            // 
            this.ActPagePanel.AutoSize = true;
            this.ActPagePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ActPagePanel.ColumnCount = 6;
            this.ActPagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.ActPagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.ActPagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.ActPagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.ActPagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.ActPagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.ActPagePanel.Controls.Add(this.lblTrail, 0, 0);
            this.ActPagePanel.Controls.Add(this.TrailName, 1, 0);
            this.ActPagePanel.Controls.Add(this.btnAdd, 3, 0);
            this.ActPagePanel.Controls.Add(this.btnEdit, 4, 0);
            this.ActPagePanel.Controls.Add(this.btnDelete, 5, 0);
            this.ActPagePanel.Controls.Add(this.ActPageSplitContainer, 0, 2);
            this.ActPagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ActPagePanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.ActPagePanel.Location = new System.Drawing.Point(0, 0);
            this.ActPagePanel.Margin = new System.Windows.Forms.Padding(0);
            this.ActPagePanel.Name = "ActPagePanel";
            this.ActPagePanel.RowCount = 3;
            this.ActPagePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.ActPagePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.ActPagePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ActPagePanel.Size = new System.Drawing.Size(400, 300);
            this.ActPagePanel.TabIndex = 8;
            this.ActPagePanel.SizeChanged += new System.EventHandler(this.ActPagePanel_SizeChanged);
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
            // TrailName
            // 
            this.TrailName.AcceptsReturn = false;
            this.TrailName.AcceptsTab = false;
            this.TrailName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TrailName.AutoSize = true;
            this.TrailName.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TrailName.BackColor = System.Drawing.Color.White;
            this.TrailName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.TrailName.ButtonImage = null;
            this.TrailName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrailName.Location = new System.Drawing.Point(91, 1);
            this.TrailName.Margin = new System.Windows.Forms.Padding(1);
            this.TrailName.MaxLength = 32767;
            this.TrailName.Multiline = false;
            this.TrailName.Name = "TrailName";
            this.TrailName.ReadOnly = true;
            this.TrailName.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.TrailName.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.TrailName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.TrailName.Size = new System.Drawing.Size(208, 18);
            this.TrailName.TabIndex = 15;
            this.TrailName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.TrailName.ButtonClick += new System.EventHandler(this.TrailName_ButtonClick);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
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
            this.btnAdd.PushStyle = true;
            this.btnAdd.RightImage = null;
            this.btnAdd.Size = new System.Drawing.Size(19, 16);
            this.btnAdd.TabIndex = 10;
            this.btnAdd.Text = "A";
            this.btnAdd.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnAdd.TextLeftMargin = 2;
            this.btnAdd.TextRightMargin = 2;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
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
            this.btnEdit.Size = new System.Drawing.Size(19, 16);
            this.btnEdit.TabIndex = 17;
            this.btnEdit.Text = "E";
            this.btnEdit.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnEdit.TextLeftMargin = 2;
            this.btnEdit.TextRightMargin = 2;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
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
            this.btnDelete.Size = new System.Drawing.Size(19, 16);
            this.btnDelete.TabIndex = 13;
            this.btnDelete.Text = "D";
            this.btnDelete.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnDelete.TextLeftMargin = 2;
            this.btnDelete.TextRightMargin = 2;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // ActPageSplitContainer
            // 
            this.ActPagePanel.SetColumnSpan(this.ActPageSplitContainer, 6);
            this.ActPageSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ActPageSplitContainer.Location = new System.Drawing.Point(0, 27);
            this.ActPageSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.ActPageSplitContainer.Name = "ActPageSplitContainer";
            this.ActPageSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // ActPageSplitContainer.Panel1
            // 
            this.ActPageSplitContainer.Panel1.Controls.Add(this.summaryList);
            this.ActPageSplitContainer.Panel1.Cursor = System.Windows.Forms.Cursors.Default;
            this.ActPageSplitContainer.Panel1MinSize = 50;
            // 
            // ActPageSplitContainer.Panel2
            // 
            this.ActPageSplitContainer.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.ActPageSplitContainer.Panel2.Controls.Add(this.ChartPanel);
            this.ActPageSplitContainer.Panel2MinSize = 100;
            this.ActPageSplitContainer.Size = new System.Drawing.Size(400, 273);
            this.ActPageSplitContainer.SplitterDistance = 60;
            this.ActPageSplitContainer.SplitterWidth = 1;
            this.ActPageSplitContainer.TabIndex = 18;
            // 
            // summaryList
            // 
            this.summaryList.AutoScroll = true;
            this.summaryList.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.summaryList.BackColor = System.Drawing.Color.Transparent;
            this.summaryList.Border = ZoneFiveSoftware.Common.Visuals.ControlBorder.Style.SmallRoundShadow;
            this.summaryList.CheckBoxes = false;
            this.summaryList.ContextMenuStrip = this.listMenu;
            this.summaryList.DefaultIndent = 15;
            this.summaryList.DefaultRowHeight = -1;
            this.summaryList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.summaryList.HeaderRowHeight = 21;
            this.summaryList.Location = new System.Drawing.Point(0, 0);
            this.summaryList.Margin = new System.Windows.Forms.Padding(0);
            this.summaryList.MultiSelect = true;
            this.summaryList.Name = "summaryList";
            this.summaryList.NumHeaderRows = ZoneFiveSoftware.Common.Visuals.TreeList.HeaderRows.One;
            this.summaryList.NumLockedColumns = 0;
            this.summaryList.RowAlternatingColors = true;
            this.summaryList.RowHotlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.summaryList.RowHotlightColorText = System.Drawing.SystemColors.HighlightText;
            this.summaryList.RowHotlightMouse = true;
            this.summaryList.RowSelectedColor = System.Drawing.SystemColors.Highlight;
            this.summaryList.RowSelectedColorText = System.Drawing.SystemColors.HighlightText;
            this.summaryList.RowSeparatorLines = true;
            this.summaryList.ShowLines = false;
            this.summaryList.ShowPlusMinus = false;
            this.summaryList.Size = new System.Drawing.Size(400, 60);
            this.summaryList.TabIndex = 11;
            // 
            // listMenu
            // 
            this.listMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyTableMenuItem,
            this.listSettingsMenuItem});
            this.listMenu.Name = "listContextMenuStrip";
            this.listMenu.Size = new System.Drawing.Size(199, 48);
            // 
            // copyTableMenuItem
            // 
            this.copyTableMenuItem.Name = "copyTableMenuItem";
            this.copyTableMenuItem.Size = new System.Drawing.Size(198, 22);
            this.copyTableMenuItem.Text = "Copy table to clipboard";
            this.copyTableMenuItem.Click += new System.EventHandler(this.copyTableMenu_Click);
            // 
            // listSettingsMenuItem
            // 
            this.listSettingsMenuItem.Name = "listSettingsMenuItem";
            this.listSettingsMenuItem.Size = new System.Drawing.Size(198, 22);
            this.listSettingsMenuItem.Text = "List Settings...";
            this.listSettingsMenuItem.Click += new System.EventHandler(this.listSettingsToolStripMenuItem_Click);
            // 
            // ChartPanel
            // 
            this.ChartPanel.AutoSize = true;
            this.ChartPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ChartPanel.ColumnCount = 1;
            this.ChartPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400F));
            this.ChartPanel.Controls.Add(this.ChartBanner, 0, 0);
            this.ChartPanel.Controls.Add(this.LineChart, 0, 1);
            this.ChartPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChartPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.ChartPanel.Location = new System.Drawing.Point(0, 0);
            this.ChartPanel.Margin = new System.Windows.Forms.Padding(0);
            this.ChartPanel.Name = "ChartPanel";
            this.ChartPanel.RowCount = 2;
            this.ChartPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ChartPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ChartPanel.Size = new System.Drawing.Size(400, 212);
            this.ChartPanel.TabIndex = 0;
            // 
            // ChartBanner
            // 
            this.ChartBanner.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ChartBanner.BackColor = System.Drawing.SystemColors.Control;
            this.ChartBanner.ContextMenuStrip = this.detailMenu;
            this.ChartBanner.Controls.Add(this.btnExpand);
            this.ChartBanner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChartBanner.HasMenuButton = true;
            this.ChartBanner.Location = new System.Drawing.Point(0, 0);
            this.ChartBanner.Margin = new System.Windows.Forms.Padding(0);
            this.ChartBanner.Name = "ChartBanner";
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
            this.speedPaceToolStripMenuItem,
            this.speedToolStripMenuItem,
            this.paceToolStripMenuItem,
            this.heartRateToolStripMenuItem,
            this.cadenceToolStripMenuItem,
            this.elevationToolStripMenuItem,
            this.gradeStripMenuItem,
            this.powerToolStripMenuItem,
            this.toolStripSeparator1,
            this.distanceToolStripMenuItem,
            this.timeToolStripMenuItem,
            this.toolStripSeparator2,
            this.showToolBarMenuItem});
            this.detailMenu.Name = "detailMenu";
            this.detailMenu.Size = new System.Drawing.Size(199, 258);
            // 
            // speedPaceToolStripMenuItem
            // 
            this.speedPaceToolStripMenuItem.Name = "speedPaceToolStripMenuItem";
            this.speedPaceToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.speedPaceToolStripMenuItem.Text = "Speed";
            this.speedPaceToolStripMenuItem.Click += new System.EventHandler(this.speedPaceToolStripMenuItem_Click);
            // 
            // speedToolStripMenuItem
            // 
            this.speedToolStripMenuItem.Name = "speedToolStripMenuItem";
            this.speedToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.speedToolStripMenuItem.Text = "Speed";
            this.speedToolStripMenuItem.Click += new System.EventHandler(this.speedToolStripMenuItem_Click);
            // 
            // paceToolStripMenuItem
            // 
            this.paceToolStripMenuItem.Name = "paceToolStripMenuItem";
            this.paceToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.paceToolStripMenuItem.Text = "Pace";
            this.paceToolStripMenuItem.Click += new System.EventHandler(this.paceToolStripMenuItem_Click);
            // 
            // heartRateToolStripMenuItem
            // 
            this.heartRateToolStripMenuItem.Name = "heartRateToolStripMenuItem";
            this.heartRateToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.heartRateToolStripMenuItem.Text = "Heart Rate";
            this.heartRateToolStripMenuItem.Click += new System.EventHandler(this.heartRateToolStripMenuItem_Click);
            // 
            // cadenceToolStripMenuItem
            // 
            this.cadenceToolStripMenuItem.Name = "cadenceToolStripMenuItem";
            this.cadenceToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.cadenceToolStripMenuItem.Text = "Cadence";
            this.cadenceToolStripMenuItem.Click += new System.EventHandler(this.cadenceToolStripMenuItem_Click);
            // 
            // elevationToolStripMenuItem
            // 
            this.elevationToolStripMenuItem.Name = "elevationToolStripMenuItem";
            this.elevationToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.elevationToolStripMenuItem.Text = "Elevation";
            this.elevationToolStripMenuItem.Click += new System.EventHandler(this.elevationToolStripMenuItem_Click);
            // 
            // gradeStripMenuItem
            // 
            this.gradeStripMenuItem.Name = "gradeStripMenuItem";
            this.gradeStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.gradeStripMenuItem.Text = "Grade";
            this.gradeStripMenuItem.Click += new System.EventHandler(this.gradeToolStripMenuItem_Click);
            // 
            // powerToolStripMenuItem
            // 
            this.powerToolStripMenuItem.Name = "powerToolStripMenuItem";
            this.powerToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.powerToolStripMenuItem.Text = "Power";
            this.powerToolStripMenuItem.Click += new System.EventHandler(this.powerToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(195, 6);
            // 
            // distanceToolStripMenuItem
            // 
            this.distanceToolStripMenuItem.Name = "distanceToolStripMenuItem";
            this.distanceToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.distanceToolStripMenuItem.Text = "Distance";
            this.distanceToolStripMenuItem.Click += new System.EventHandler(this.distanceToolStripMenuItem_Click);
            // 
            // timeToolStripMenuItem
            // 
            this.timeToolStripMenuItem.Name = "timeToolStripMenuItem";
            this.timeToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.timeToolStripMenuItem.Text = "Time";
            this.timeToolStripMenuItem.Click += new System.EventHandler(this.timeToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(195, 6);
            // 
            // showToolBarMenuItem
            // 
            this.showToolBarMenuItem.Name = "showToolBarMenuItem";
            this.showToolBarMenuItem.Size = new System.Drawing.Size(198, 22);
            this.showToolBarMenuItem.Text = "showToolBarMenuItem";
            this.showToolBarMenuItem.Click += new System.EventHandler(this.showToolBarMenuItem_Click);
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
            this.LineChart.AutoScroll = true;
            this.LineChart.AutoSize = true;
            this.LineChart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.LineChart.BackColor = System.Drawing.SystemColors.Control;
            this.LineChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
            this.LineChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
            this.LineChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
            this.LineChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LineChart.Location = new System.Drawing.Point(0, 20);
            this.LineChart.Margin = new System.Windows.Forms.Padding(0);
            this.LineChart.MinimumSize = new System.Drawing.Size(250, 100);
            this.LineChart.Name = "LineChart";
            this.LineChart.Size = new System.Drawing.Size(400, 192);
            this.LineChart.TabIndex = 1;
            this.LineChart.TrailResult = null;
            this.LineChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
            this.LineChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Speed;
            this.LineChart.YAxisReferential_right = null;
            // 
            // ExpandSplitContainer
            // 
            this.ExpandSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExpandSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.ExpandSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.ExpandSplitContainer.Name = "ExpandSplitContainer";
            // 
            // ExpandSplitContainer.Panel1
            // 
            this.ExpandSplitContainer.Panel1.Controls.Add(this.ActPagePanel);
            this.ExpandSplitContainer.Panel1.Cursor = System.Windows.Forms.Cursors.Default;
            this.ExpandSplitContainer.Panel1MinSize = 225;
            // 
            // ExpandSplitContainer.Panel2
            // 
            this.ExpandSplitContainer.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.ExpandSplitContainer.Panel2Collapsed = true;
            this.ExpandSplitContainer.Panel2MinSize = 0;
            this.ExpandSplitContainer.Size = new System.Drawing.Size(400, 300);
            this.ExpandSplitContainer.SplitterDistance = 400;
            this.ExpandSplitContainer.SplitterWidth = 1;
            this.ExpandSplitContainer.TabIndex = 18;
            // 
            // ActivityDetailPageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.ExpandSplitContainer);
            this.Name = "ActivityDetailPageControl";
            this.Size = new System.Drawing.Size(400, 300);
            this.ActPagePanel.ResumeLayout(false);
            this.ActPagePanel.PerformLayout();
            this.ActPageSplitContainer.Panel1.ResumeLayout(false);
            this.ActPageSplitContainer.Panel2.ResumeLayout(false);
            this.ActPageSplitContainer.Panel2.PerformLayout();
            this.ActPageSplitContainer.ResumeLayout(false);
            this.listMenu.ResumeLayout(false);
            this.ChartPanel.ResumeLayout(false);
            this.ChartPanel.PerformLayout();
            this.ChartBanner.ResumeLayout(false);
            this.detailMenu.ResumeLayout(false);
            this.ExpandSplitContainer.Panel1.ResumeLayout(false);
            this.ExpandSplitContainer.Panel1.PerformLayout();
            this.ExpandSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel ActPagePanel;
		private System.Windows.Forms.Label lblTrail;
		private ZoneFiveSoftware.Common.Visuals.TreeList summaryList;
		private ZoneFiveSoftware.Common.Visuals.Button btnDelete;
		private ZoneFiveSoftware.Common.Visuals.Button btnAdd;
		private ZoneFiveSoftware.Common.Visuals.TextBox TrailName;
		private ZoneFiveSoftware.Common.Visuals.Button btnEdit;
		private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.SplitContainer ExpandSplitContainer;
        private System.Windows.Forms.SplitContainer ActPageSplitContainer;
        private System.Windows.Forms.TableLayoutPanel ChartPanel;
		private ZoneFiveSoftware.Common.Visuals.ActionBanner ChartBanner;
		private System.Windows.Forms.ContextMenuStrip listMenu;
        private System.Windows.Forms.ToolStripMenuItem copyTableMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listSettingsMenuItem;
		private System.Windows.Forms.ContextMenuStrip detailMenu;
        private TrailsPlugin.UI.Activity.TrailLineChart LineChart;
		private System.Windows.Forms.ToolStripMenuItem speedToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem elevationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem heartRateToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cadenceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem powerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem distanceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem timeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem gradeStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem speedPaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showToolBarMenuItem;
        private ZoneFiveSoftware.Common.Visuals.Button btnExpand;
	}
}
