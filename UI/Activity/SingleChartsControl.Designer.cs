namespace TrailsPlugin.UI.Activity {
	partial class SingleChartsControl {
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
            this.timeDiffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.distDiffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.distanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.showToolBarMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnExpand = new ZoneFiveSoftware.Common.Visuals.Button();
            this.LineChart = new TrailsPlugin.UI.Activity.TrailLineChart();
            this.ChartPanel.SuspendLayout();
            this.ChartBanner.SuspendLayout();
            this.detailMenu.SuspendLayout();
            this.SuspendLayout();
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
            this.timeDiffToolStripMenuItem,
            this.distDiffToolStripMenuItem,
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
            // timeDiffToolStripMenuItem
            // 
            this.timeDiffToolStripMenuItem.Name = "timeDiffToolStripMenuItem";
            this.timeDiffToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.timeDiffToolStripMenuItem.Text = "TimeDiff";
            this.timeDiffToolStripMenuItem.Click += new System.EventHandler(this.timeDiffToolStripMenuItem_Click);
            // 
            // distDiffToolStripMenuItem
            // 
            this.distDiffToolStripMenuItem.Name = "distDiffToolStripMenuItem";
            this.distDiffToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.distDiffToolStripMenuItem.Text = "DistDiff";
            this.distDiffToolStripMenuItem.Click += new System.EventHandler(this.distDiffToolStripMenuItem_Click);
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
            this.LineChart.ReferenceTrailResult = null;
            this.LineChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
            this.LineChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Speed;
            this.LineChart.YAxisReferential_right = null;
            // 
            // SingleChartsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.ChartPanel);
            this.Name = "SingleChartsControl";
            this.Size = new System.Drawing.Size(400, 300);
            this.ChartPanel.ResumeLayout(false);
            this.ChartPanel.PerformLayout();
            this.ChartBanner.ResumeLayout(false);
            this.detailMenu.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.TableLayoutPanel ChartPanel;
		private ZoneFiveSoftware.Common.Visuals.ActionBanner ChartBanner;
        private System.Windows.Forms.ContextMenuStrip detailMenu;
        private TrailsPlugin.UI.Activity.TrailLineChart LineChart;
		private System.Windows.Forms.ToolStripMenuItem speedToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem elevationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem heartRateToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cadenceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem powerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem distanceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timeDiffToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem distDiffToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gradeStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem speedPaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showToolBarMenuItem;
        private ZoneFiveSoftware.Common.Visuals.Button btnExpand;
	}
}
