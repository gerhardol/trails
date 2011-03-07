namespace TrailsPlugin.UI.Activity {
	partial class MultiChartsControl {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiChartsControl));
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
            this.diffTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.diffDistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.diffDistTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.distanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.showToolBarMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnExpand = new ZoneFiveSoftware.Common.Visuals.Button();
            this.ChartPanel = new System.Windows.Forms.TableLayoutPanel();
            this.speedChart = new TrailsPlugin.UI.Activity.TrailLineChart();
            this.paceChart = new TrailsPlugin.UI.Activity.TrailLineChart();
            this.elevationChart = new TrailsPlugin.UI.Activity.TrailLineChart();
            this.heartrateChart = new TrailsPlugin.UI.Activity.TrailLineChart();
            this.cadenceChart = new TrailsPlugin.UI.Activity.TrailLineChart();
            this.gradeChart = new TrailsPlugin.UI.Activity.TrailLineChart();
            this.diffTimeChart = new TrailsPlugin.UI.Activity.TrailLineChart();
            this.diffDistChart = new TrailsPlugin.UI.Activity.TrailLineChart();
            this.ChartBanner.SuspendLayout();
            this.detailMenu.SuspendLayout();
            this.ChartPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ChartBanner
            // 
            this.ChartBanner.BackColor = System.Drawing.SystemColors.Control;
            this.ChartBanner.ContextMenuStrip = this.detailMenu;
            this.ChartBanner.Controls.Add(this.btnExpand);
            this.ChartBanner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChartBanner.HasMenuButton = true;
            this.ChartBanner.Location = new System.Drawing.Point(0, 0);
            this.ChartBanner.Margin = new System.Windows.Forms.Padding(0);
            this.ChartBanner.Name = "ChartBanner";
            this.ChartBanner.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ChartBanner.Size = new System.Drawing.Size(500, 20);
            this.ChartBanner.Style = ZoneFiveSoftware.Common.Visuals.ActionBanner.BannerStyle.Header2;
            this.ChartBanner.TabIndex = 1;
            this.ChartBanner.Text = "Trail Charts";
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
            this.diffTimeToolStripMenuItem,
            this.diffDistToolStripMenuItem,
            this.diffDistTimeToolStripMenuItem,
            this.toolStripSeparator2,
            this.distanceToolStripMenuItem,
            this.timeToolStripMenuItem,
            this.toolStripSeparator3,
            this.showToolBarMenuItem});
            this.detailMenu.Name = "detailMenu";
            this.detailMenu.Size = new System.Drawing.Size(199, 258);
            // 
            // speedPaceToolStripMenuItem
            // 
            this.speedPaceToolStripMenuItem.Name = "speedPaceToolStripMenuItem";
            this.speedPaceToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.speedPaceToolStripMenuItem.Text = "SpeedPace";
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
            // diffTimeToolStripMenuItem
            // 
            this.diffTimeToolStripMenuItem.Name = "diffTimeToolStripMenuItem";
            this.diffTimeToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.diffTimeToolStripMenuItem.Text = "diffTime";
            this.diffTimeToolStripMenuItem.Click += new System.EventHandler(this.diffTimeToolStripMenuItem_Click);
            // 
            // diffDistToolStripMenuItem
            // 
            this.diffDistToolStripMenuItem.Name = "diffDistToolStripMenuItem";
            this.diffDistToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.diffDistToolStripMenuItem.Text = "diffDist";
            this.diffDistToolStripMenuItem.Click += new System.EventHandler(this.diffDistToolStripMenuItem_Click);
            // 
            // diffDistTimeToolStripMenuItem
            // 
            this.diffDistTimeToolStripMenuItem.Name = "diffDistTimeToolStripMenuItem";
            this.diffDistTimeToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.diffDistTimeToolStripMenuItem.Text = "diffDistTime";
            this.diffDistTimeToolStripMenuItem.Click += new System.EventHandler(this.diffDistTimeToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(195, 6);
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
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(195, 6);
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
            this.btnExpand.Location = new System.Drawing.Point(353, 0);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.PushStyle = false;
            this.btnExpand.RightImage = null;
            this.btnExpand.Size = new System.Drawing.Size(20, 20);
            this.btnExpand.TabIndex = 11;
            this.btnExpand.Text = "X";
            this.btnExpand.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnExpand.TextLeftMargin = 2;
            this.btnExpand.TextRightMargin = 2;
            this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
            // 
            // ChartPanel
            // 
            this.ChartPanel.AutoSize = true;
            this.ChartPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ChartPanel.ColumnCount = 1;
            this.ChartPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ChartPanel.Controls.Add(this.ChartBanner, 0, 0);
            this.ChartPanel.Controls.Add(this.speedChart, 0, 1);
            this.ChartPanel.Controls.Add(this.paceChart, 0, 2);
            this.ChartPanel.Controls.Add(this.elevationChart, 0, 3);
            this.ChartPanel.Controls.Add(this.heartrateChart, 0, 4);
            this.ChartPanel.Controls.Add(this.cadenceChart, 0, 5);
            this.ChartPanel.Controls.Add(this.gradeChart, 0, 6);
            this.ChartPanel.Controls.Add(this.diffTimeChart, 0, 7);
            this.ChartPanel.Controls.Add(this.diffDistChart, 0, 8);
            this.ChartPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChartPanel.Location = new System.Drawing.Point(0, 0);
            this.ChartPanel.Name = "ChartPanel";
            this.ChartPanel.RowCount = 9;
            this.ChartPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ChartPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.ChartPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.ChartPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.ChartPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.ChartPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.ChartPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.ChartPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.ChartPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.ChartPanel.Size = new System.Drawing.Size(260, 520);
            this.ChartPanel.TabIndex = 2;
            // 
            // speedChart
            // 
            this.speedChart.AutoSize = true;
            this.speedChart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.speedChart.BackColor = System.Drawing.SystemColors.Control;
            this.speedChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
            this.speedChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
            this.speedChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
            this.speedChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.speedChart.Location = new System.Drawing.Point(0, 0);
            this.speedChart.Margin = new System.Windows.Forms.Padding(0);
            this.speedChart.MinimumSize = new System.Drawing.Size(100, 0);
            this.speedChart.Name = "speedChart";
            this.speedChart.ReferenceTrailResult = null;
            this.speedChart.ShowPage = false;
            this.speedChart.Size = new System.Drawing.Size(500, 100);
            this.speedChart.TabIndex = 2;
            this.speedChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
            this.speedChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Speed;
            // 
            // paceChart
            // 
            this.paceChart.AutoSize = true;
            this.paceChart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.paceChart.BackColor = System.Drawing.SystemColors.Control;
            this.paceChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
            this.paceChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
            this.paceChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
            this.paceChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paceChart.Location = new System.Drawing.Point(0, 120);
            this.paceChart.Margin = new System.Windows.Forms.Padding(0);
            this.paceChart.MinimumSize = new System.Drawing.Size(100, 0);
            this.paceChart.Name = "paceChart";
            this.paceChart.ReferenceTrailResult = null;
            this.paceChart.ShowPage = false;
            this.paceChart.Size = new System.Drawing.Size(500, 100);
            this.paceChart.TabIndex = 2;
            this.paceChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
            this.paceChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Pace;
            // 
            // elevationChart
            // 
            this.elevationChart.AutoSize = true;
            this.elevationChart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.elevationChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
            this.elevationChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
            this.elevationChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
            this.elevationChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elevationChart.Location = new System.Drawing.Point(0, 220);
            this.elevationChart.Margin = new System.Windows.Forms.Padding(0);
            this.elevationChart.MinimumSize = new System.Drawing.Size(100, 0);
            this.elevationChart.Name = "elevationChart";
            this.elevationChart.ReferenceTrailResult = null;
            this.elevationChart.ShowPage = false;
            this.elevationChart.Size = new System.Drawing.Size(500, 100);
            this.elevationChart.TabIndex = 3;
            this.elevationChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
            this.elevationChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Elevation;
            // 
            // heartrateChart
            // 
            this.heartrateChart.AutoSize = true;
            this.heartrateChart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.heartrateChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
            this.heartrateChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
            this.heartrateChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
            this.heartrateChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.heartrateChart.Location = new System.Drawing.Point(0, 320);
            this.heartrateChart.Margin = new System.Windows.Forms.Padding(0);
            this.heartrateChart.MinimumSize = new System.Drawing.Size(100, 0);
            this.heartrateChart.Name = "heartrateChart";
            this.heartrateChart.ReferenceTrailResult = null;
            this.heartrateChart.ShowPage = false;
            this.heartrateChart.Size = new System.Drawing.Size(500, 100);
            this.heartrateChart.TabIndex = 3;
            this.heartrateChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
            this.heartrateChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.HeartRateBPM;
            // 
            // cadenceChart
            // 
            this.cadenceChart.AutoSize = true;
            this.cadenceChart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.cadenceChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
            this.cadenceChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
            this.cadenceChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
            this.cadenceChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cadenceChart.Location = new System.Drawing.Point(0, 420);
            this.cadenceChart.Margin = new System.Windows.Forms.Padding(0);
            this.cadenceChart.MinimumSize = new System.Drawing.Size(100, 0);
            this.cadenceChart.Name = "cadenceChart";
            this.cadenceChart.ReferenceTrailResult = null;
            this.cadenceChart.ShowPage = false;
            this.cadenceChart.Size = new System.Drawing.Size(500, 100);
            this.cadenceChart.TabIndex = 6;
            this.cadenceChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
            this.cadenceChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Cadence;
            // 
            // gradeChart
            // 
            this.gradeChart.AutoSize = true;
            this.gradeChart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gradeChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
            this.gradeChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
            this.gradeChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
            this.gradeChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gradeChart.Location = new System.Drawing.Point(0, 520);
            this.gradeChart.Margin = new System.Windows.Forms.Padding(0);
            this.gradeChart.MinimumSize = new System.Drawing.Size(100, 0);
            this.gradeChart.Name = "gradeChart";
            this.gradeChart.ReferenceTrailResult = null;
            this.gradeChart.ShowPage = false;
            this.gradeChart.Size = new System.Drawing.Size(500, 100);
            this.gradeChart.TabIndex = 7;
            this.gradeChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
            this.gradeChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Grade;
            // 
            // diffTime
            // 
            this.diffTimeChart.AutoSize = true;
            this.diffTimeChart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.diffTimeChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
            this.diffTimeChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
            this.diffTimeChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
            this.diffTimeChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diffTimeChart.Location = new System.Drawing.Point(0, 620);
            this.diffTimeChart.Margin = new System.Windows.Forms.Padding(0);
            this.diffTimeChart.MinimumSize = new System.Drawing.Size(100, 0);
            this.diffTimeChart.Name = "diffTime";
            this.diffTimeChart.ReferenceTrailResult = null;
            this.diffTimeChart.ShowPage = false;
            this.diffTimeChart.Size = new System.Drawing.Size(500, 100);
            this.diffTimeChart.TabIndex = 7;
            this.diffTimeChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
            this.diffTimeChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.DiffTime;
            // 
            // diffDist
            // 
            this.diffDistChart.AutoSize = true;
            this.diffDistChart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.diffDistChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
            this.diffDistChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
            this.diffDistChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
            this.diffDistChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diffDistChart.Location = new System.Drawing.Point(0, 720);
            this.diffDistChart.Margin = new System.Windows.Forms.Padding(0);
            this.diffDistChart.MinimumSize = new System.Drawing.Size(100, 0);
            this.diffDistChart.Name = "diffDist";
            this.diffDistChart.ReferenceTrailResult = null;
            this.diffDistChart.ShowPage = false;
            this.diffDistChart.Size = new System.Drawing.Size(500, 100);
            this.diffDistChart.TabIndex = 7;
            this.diffDistChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
            this.diffDistChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.DiffDist;
            // 
            // MultiChartsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.ChartPanel);
            this.Name = "MultiChartsControl";
            this.Size = new System.Drawing.Size(500, 520);
            this.ChartBanner.ResumeLayout(false);
            this.detailMenu.ResumeLayout(false);
            this.ChartPanel.ResumeLayout(false);
            this.ChartPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private ZoneFiveSoftware.Common.Visuals.ActionBanner ChartBanner;
        private System.Windows.Forms.ContextMenuStrip detailMenu;
        private ZoneFiveSoftware.Common.Visuals.Button btnExpand;
        private System.Windows.Forms.TableLayoutPanel ChartPanel;
		private TrailLineChart cadenceChart;
		private TrailLineChart heartrateChart;
        private TrailLineChart speedChart;
        private TrailLineChart paceChart;
        private TrailLineChart elevationChart;
        private TrailLineChart gradeChart;
        private TrailLineChart diffTimeChart;
        private TrailLineChart diffDistChart;
        private System.Windows.Forms.ToolStripMenuItem speedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem elevationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem heartRateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cadenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem powerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem diffTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem diffDistToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem diffDistTimeToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem distanceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gradeStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem speedPaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showToolBarMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}
