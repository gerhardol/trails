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
			this.gradeChart = new TrailsPlugin.UI.Activity.TrailLineChart();
			this.speedChart = new TrailsPlugin.UI.Activity.TrailLineChart();
			this.heartrateChart = new TrailsPlugin.UI.Activity.TrailLineChart();
			this.cadenceChart = new TrailsPlugin.UI.Activity.TrailLineChart();
			this.elevationChart = new TrailsPlugin.UI.Activity.TrailLineChart();
			this.ChartBanner.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// ChartBanner
			// 
			this.ChartBanner.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ChartBanner.BackColor = System.Drawing.SystemColors.Control;
			this.ChartBanner.Controls.Add(this.btnCollapse);
			this.ChartBanner.HasMenuButton = true;
			this.ChartBanner.Location = new System.Drawing.Point(0, 0);
			this.ChartBanner.Margin = new System.Windows.Forms.Padding(0);
			this.ChartBanner.Name = "ChartBanner";
			this.ChartBanner.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.ChartBanner.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.ChartBanner.Size = new System.Drawing.Size(400, 25);
			this.ChartBanner.Style = ZoneFiveSoftware.Common.Visuals.ActionBanner.BannerStyle.Header1;
			this.ChartBanner.TabIndex = 1;
			this.ChartBanner.Text = "Trail Charts";
			this.ChartBanner.UseStyleFont = true;
			// 
			// btnCollapse
			// 
			this.btnCollapse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnCollapse.BackColor = System.Drawing.Color.Transparent;
			this.btnCollapse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnCollapse.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
			this.btnCollapse.CenterImage = null;
			this.btnCollapse.DialogResult = System.Windows.Forms.DialogResult.None;
			this.btnCollapse.HyperlinkStyle = false;
			this.btnCollapse.ImageMargin = 2;
			this.btnCollapse.LeftImage = null;
			this.btnCollapse.Location = new System.Drawing.Point(1239, 0);
			this.btnCollapse.Name = "btnCollapse";
			this.btnCollapse.Padding = new System.Windows.Forms.Padding(2);
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
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.gradeChart, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.speedChart, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.heartrateChart, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.cadenceChart, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.elevationChart, 0, 3);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 28);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 5;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(397, 569);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// gradeChart
			// 
			this.gradeChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gradeChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
			this.gradeChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
			this.gradeChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
			this.gradeChart.Location = new System.Drawing.Point(0, 452);
			this.gradeChart.Margin = new System.Windows.Forms.Padding(0);
			this.gradeChart.Name = "gradeChart";
			this.gradeChart.Size = new System.Drawing.Size(397, 117);
			this.gradeChart.TabIndex = 7;
			this.gradeChart.TrailResult = null;
			this.gradeChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
			this.gradeChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Speed;
			// 
			// speedChart
			// 
			this.speedChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.speedChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
			this.speedChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
			this.speedChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
			this.speedChart.Location = new System.Drawing.Point(0, 0);
			this.speedChart.Margin = new System.Windows.Forms.Padding(0);
			this.speedChart.Name = "speedChart";
			this.speedChart.Size = new System.Drawing.Size(397, 113);
			this.speedChart.TabIndex = 2;
			this.speedChart.TrailResult = null;
			this.speedChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
			this.speedChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Speed;
			// 
			// heartrateChart
			// 
			this.heartrateChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.heartrateChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
			this.heartrateChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
			this.heartrateChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
			this.heartrateChart.Location = new System.Drawing.Point(0, 113);
			this.heartrateChart.Margin = new System.Windows.Forms.Padding(0);
			this.heartrateChart.Name = "heartrateChart";
			this.heartrateChart.Size = new System.Drawing.Size(397, 113);
			this.heartrateChart.TabIndex = 3;
			this.heartrateChart.TrailResult = null;
			this.heartrateChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
			this.heartrateChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Speed;
			// 
			// cadenceChart
			// 
			this.cadenceChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cadenceChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
			this.cadenceChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
			this.cadenceChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
			this.cadenceChart.Location = new System.Drawing.Point(0, 226);
			this.cadenceChart.Margin = new System.Windows.Forms.Padding(0);
			this.cadenceChart.Name = "cadenceChart";
			this.cadenceChart.Size = new System.Drawing.Size(397, 113);
			this.cadenceChart.TabIndex = 6;
			this.cadenceChart.TrailResult = null;
			this.cadenceChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
			this.cadenceChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Speed;
			// 
			// elevationChart
			// 
			this.elevationChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.elevationChart.ChartFillColor = System.Drawing.Color.WhiteSmoke;
			this.elevationChart.ChartLineColor = System.Drawing.Color.LightSkyBlue;
			this.elevationChart.ChartSelectedColor = System.Drawing.Color.AliceBlue;
			this.elevationChart.Location = new System.Drawing.Point(0, 339);
			this.elevationChart.Margin = new System.Windows.Forms.Padding(0);
			this.elevationChart.Name = "elevationChart";
			this.elevationChart.Size = new System.Drawing.Size(397, 113);
			this.elevationChart.TabIndex = 4;
			this.elevationChart.TrailResult = null;
			this.elevationChart.XAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Time;
			this.elevationChart.YAxisReferential = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Speed;
			// 
			// ChartsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.ChartBanner);
			this.Name = "ChartsControl";
			this.Size = new System.Drawing.Size(400, 600);
			this.ChartBanner.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

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
