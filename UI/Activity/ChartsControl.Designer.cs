namespace TrailsPlugin.UI.Activity {
	partial class ChartsControl {
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
            this.ChartBanner = new ZoneFiveSoftware.Common.Visuals.ActionBanner();
            this.btnCollapse = new ZoneFiveSoftware.Common.Visuals.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.speedChart = new TrailsPlugin.UI.Activity.TrailLineChart();
            this.heartrateChart = new TrailsPlugin.UI.Activity.TrailLineChart();
            this.cadenceChart = new TrailsPlugin.UI.Activity.TrailLineChart();
            this.elevationChart = new TrailsPlugin.UI.Activity.TrailLineChart();
            this.gradeChart = new TrailsPlugin.UI.Activity.TrailLineChart();
            this.ChartBanner.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ChartBanner
            // 
            this.ChartBanner.BackColor = System.Drawing.SystemColors.Control;
            this.ChartBanner.Controls.Add(this.btnCollapse);
            this.ChartBanner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChartBanner.HasMenuButton = true;
            this.ChartBanner.Location = new System.Drawing.Point(0, 0);
            this.ChartBanner.Margin = new System.Windows.Forms.Padding(0);
            this.ChartBanner.Name = "ChartBanner";
            this.ChartBanner.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.ChartBanner.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ChartBanner.Size = new System.Drawing.Size(500, 20);
            this.ChartBanner.Style = ZoneFiveSoftware.Common.Visuals.ActionBanner.BannerStyle.Header1;
            this.ChartBanner.TabIndex = 1;
            this.ChartBanner.Text = "Trail Charts";
            this.ChartBanner.UseStyleFont = true;
            // 
            // btnCollapse
            // 
            this.btnCollapse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollapse.BackColor = System.Drawing.Color.Transparent;
            this.btnCollapse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnCollapse.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnCollapse.CenterImage = null;
            this.btnCollapse.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnCollapse.HyperlinkStyle = false;
            this.btnCollapse.ImageMargin = 2;
            this.btnCollapse.LeftImage = null;
            this.btnCollapse.Location = new System.Drawing.Point(471, 3);
            this.btnCollapse.Name = "btnCollapse";
            this.btnCollapse.PushStyle = false;
            this.btnCollapse.RightImage = null;
            this.btnCollapse.Size = new System.Drawing.Size(20, 20);
            this.btnCollapse.TabIndex = 12;
            this.btnCollapse.Text = "X";
            this.btnCollapse.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnCollapse.TextLeftMargin = 2;
            this.btnCollapse.TextRightMargin = 2;
            this.btnCollapse.Click += new System.EventHandler(this.btnCollapse_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.ChartBanner, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.speedChart, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.heartrateChart, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.cadenceChart, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.elevationChart, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.gradeChart, 0, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(500, 520);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // speedChart
            // 
            this.speedChart.AutoSize = true;
            this.speedChart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.speedChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
            this.speedChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
            this.speedChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
            this.speedChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.speedChart.Location = new System.Drawing.Point(0, 20);
            this.speedChart.Margin = new System.Windows.Forms.Padding(0);
            this.speedChart.Name = "speedChart";
            this.speedChart.Size = new System.Drawing.Size(500, 100);
            this.speedChart.TabIndex = 2;
            this.speedChart.TrailResult = null;
            this.speedChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
            this.speedChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Speed;
            // 
            // heartrateChart
            // 
            this.heartrateChart.AutoSize = true;
            this.heartrateChart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.heartrateChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
            this.heartrateChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
            this.heartrateChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
            this.heartrateChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.heartrateChart.Location = new System.Drawing.Point(0, 120);
            this.heartrateChart.Margin = new System.Windows.Forms.Padding(0);
            this.heartrateChart.Name = "heartrateChart";
            this.heartrateChart.Size = new System.Drawing.Size(500, 100);
            this.heartrateChart.TabIndex = 3;
            this.heartrateChart.TrailResult = null;
            this.heartrateChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
            this.heartrateChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Speed;
            // 
            // cadenceChart
            // 
            this.cadenceChart.AutoSize = true;
            this.cadenceChart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.cadenceChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
            this.cadenceChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
            this.cadenceChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
            this.cadenceChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cadenceChart.Location = new System.Drawing.Point(0, 220);
            this.cadenceChart.Margin = new System.Windows.Forms.Padding(0);
            this.cadenceChart.Name = "cadenceChart";
            this.cadenceChart.Size = new System.Drawing.Size(500, 100);
            this.cadenceChart.TabIndex = 6;
            this.cadenceChart.TrailResult = null;
            this.cadenceChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
            this.cadenceChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Speed;
            // 
            // elevationChart
            // 
            this.elevationChart.AutoSize = true;
            this.elevationChart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.elevationChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
            this.elevationChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
            this.elevationChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
            this.elevationChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elevationChart.Location = new System.Drawing.Point(0, 320);
            this.elevationChart.Margin = new System.Windows.Forms.Padding(0);
            this.elevationChart.Name = "elevationChart";
            this.elevationChart.Size = new System.Drawing.Size(500, 100);
            this.elevationChart.TabIndex = 3;
            this.elevationChart.TrailResult = null;
            this.elevationChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
            this.elevationChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Speed;
            // 
            // gradeChart
            // 
            this.gradeChart.AutoSize = true;
            this.gradeChart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gradeChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
            this.gradeChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
            this.gradeChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
            this.gradeChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gradeChart.Location = new System.Drawing.Point(0, 420);
            this.gradeChart.Margin = new System.Windows.Forms.Padding(0);
            this.gradeChart.Name = "gradeChart";
            this.gradeChart.Size = new System.Drawing.Size(500, 100);
            this.gradeChart.TabIndex = 7;
            this.gradeChart.TrailResult = null;
            this.gradeChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
            this.gradeChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Grade;
            // 
            // ChartsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ChartsControl";
            this.Size = new System.Drawing.Size(500, 520);
            this.ChartBanner.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private ZoneFiveSoftware.Common.Visuals.ActionBanner ChartBanner;
		private ZoneFiveSoftware.Common.Visuals.Button btnCollapse;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private TrailLineChart cadenceChart;
		private TrailLineChart heartrateChart;
		private TrailLineChart speedChart;
        private TrailLineChart elevationChart;
        private TrailLineChart gradeChart;
	}
}
