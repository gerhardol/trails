namespace TrailsPlugin.UI.Activity
{
    partial class TrailLineChart
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrailLineChart));
            this.chartTablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.ButtonPanel = new ZoneFiveSoftware.Common.Visuals.Panel();
            this.ZoomInButton = new ZoneFiveSoftware.Common.Visuals.Button();
            this.ZoomToContentButton = new ZoneFiveSoftware.Common.Visuals.Button();
            this.ZoomOutButton = new ZoneFiveSoftware.Common.Visuals.Button();
            this.SaveImageButton = new ZoneFiveSoftware.Common.Visuals.Button();
            this.MoreChartsButton = new ZoneFiveSoftware.Common.Visuals.Button();
            this.TrailPointsButton = new ZoneFiveSoftware.Common.Visuals.Button();
            this.MainChart = new ZoneFiveSoftware.Common.Visuals.Chart.LineChart();
            this.chartContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyChartMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fitToWindowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moreChartsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smoothingPicker = new System.Windows.Forms.NumericUpDown();
            this.smoothingLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.smoothingPicker)).BeginInit();
            this.chartTablePanel.SuspendLayout();
            this.ButtonPanel.SuspendLayout();
            this.chartContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // chartTablePanel
            // 
            this.chartTablePanel.AutoSize = true;
            this.chartTablePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.chartTablePanel.ColumnCount = 1;
            this.chartTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.chartTablePanel.Controls.Add(this.ButtonPanel, 0, 0);
            this.chartTablePanel.Controls.Add(this.MainChart, 0, 1);
            this.chartTablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartTablePanel.Location = new System.Drawing.Point(0, 0);
            this.chartTablePanel.Margin = new System.Windows.Forms.Padding(0);
            this.chartTablePanel.Name = "chartTablePanel";
            this.chartTablePanel.RowCount = 2;
            this.chartTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.chartTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.chartTablePanel.Size = new System.Drawing.Size(400, 31);
            this.chartTablePanel.TabIndex = 0;
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.AutoSize = true;
            this.ButtonPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonPanel.BackColor = System.Drawing.Color.Transparent;
            this.ButtonPanel.Border = ZoneFiveSoftware.Common.Visuals.ControlBorder.Style.None;
            this.ButtonPanel.BorderColor = System.Drawing.Color.Gray;
            this.ButtonPanel.Controls.Add(this.ZoomInButton);
            this.ButtonPanel.Controls.Add(this.ZoomToContentButton);
            this.ButtonPanel.Controls.Add(this.ZoomOutButton);
            this.ButtonPanel.Controls.Add(this.SaveImageButton);
            this.ButtonPanel.Controls.Add(this.MoreChartsButton);
            this.ButtonPanel.Controls.Add(this.TrailPointsButton);
            this.ButtonPanel.Controls.Add(this.smoothingLabel);
            this.ButtonPanel.Controls.Add(this.smoothingPicker);
            this.ButtonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonPanel.HeadingBackColor = System.Drawing.Color.LightBlue;
            this.ButtonPanel.HeadingFont = null;
            this.ButtonPanel.HeadingLeftMargin = 0;
            this.ButtonPanel.HeadingText = null;
            this.ButtonPanel.HeadingTextColor = System.Drawing.Color.Black;
            this.ButtonPanel.HeadingTopMargin = 0;
            this.ButtonPanel.Location = new System.Drawing.Point(0, 0);
            this.ButtonPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(400, 24);
            this.ButtonPanel.TabIndex = 1;
            // 
            // ZoomInButton
            // 
            this.ZoomInButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ZoomInButton.BackColor = System.Drawing.Color.Transparent;
            this.ZoomInButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.ZoomInButton.CenterImage = ((System.Drawing.Image)(resources.GetObject("ZoomInButton.CenterImage")));
            this.ZoomInButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ZoomInButton.HyperlinkStyle = false;
            //this.ZoomInButton.ImageMargin = 1;
            this.ZoomInButton.LeftImage = null;
            this.ZoomInButton.Location = new System.Drawing.Point(374, 0);
            this.ZoomInButton.Margin = new System.Windows.Forms.Padding(0);
            this.ZoomInButton.Name = "ZoomInButton";
            this.ZoomInButton.PushStyle = true;
            this.ZoomInButton.RightImage = null;
            this.ZoomInButton.Size = new System.Drawing.Size(22, 22);
            this.ZoomInButton.TabIndex = 0;
            this.ZoomInButton.TextAlign = System.Drawing.StringAlignment.Center;
            this.ZoomInButton.TextLeftMargin = 2;
            this.ZoomInButton.TextRightMargin = 2;
            this.ZoomInButton.Click += new System.EventHandler(this.ZoomInButton_Click);
            // 
            // ZoomToContentButton
            // 
            this.ZoomToContentButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ZoomToContentButton.BackColor = System.Drawing.Color.Transparent;
            this.ZoomToContentButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.ZoomToContentButton.CenterImage = global::TrailsPlugin.Properties.Resources.ZoomToContent;
            this.ZoomToContentButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ZoomToContentButton.HyperlinkStyle = false;
            //this.ZoomToContentButton.ImageMargin = 1;
            this.ZoomToContentButton.LeftImage = null;
            this.ZoomToContentButton.Location = new System.Drawing.Point(322, 0);
            this.ZoomToContentButton.Name = "ZoomToContentButton";
            this.ZoomToContentButton.PushStyle = true;
            this.ZoomToContentButton.RightImage = null;
            this.ZoomToContentButton.Size = new System.Drawing.Size(22, 22);
            this.ZoomToContentButton.TabIndex = 1;
            this.ZoomToContentButton.TextAlign = System.Drawing.StringAlignment.Center;
            this.ZoomToContentButton.TextLeftMargin = 2;
            this.ZoomToContentButton.TextRightMargin = 2;
            this.ZoomToContentButton.Click += new System.EventHandler(this.ZoomToContentButton_Click);
            // 
            // ZoomOutButton
            // 
            this.ZoomOutButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ZoomOutButton.BackColor = System.Drawing.Color.Transparent;
            this.ZoomOutButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.ZoomOutButton.CenterImage = ((System.Drawing.Image)(resources.GetObject("ZoomOutButton.CenterImage")));
            this.ZoomOutButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ZoomOutButton.HyperlinkStyle = false;
            //this.ZoomOutButton.ImageMargin = 1;
            this.ZoomOutButton.LeftImage = null;
            this.ZoomOutButton.Location = new System.Drawing.Point(348, 0);
            this.ZoomOutButton.Margin = new System.Windows.Forms.Padding(0);
            this.ZoomOutButton.Name = "ZoomOutButton";
            this.ZoomOutButton.PushStyle = true;
            this.ZoomOutButton.RightImage = null;
            this.ZoomOutButton.Size = new System.Drawing.Size(22, 22);
            this.ZoomOutButton.TabIndex = 0;
            this.ZoomOutButton.TextAlign = System.Drawing.StringAlignment.Center;
            this.ZoomOutButton.TextLeftMargin = 2;
            this.ZoomOutButton.TextRightMargin = 2;
            this.ZoomOutButton.Click += new System.EventHandler(this.ZoomOutButton_Click);
            // 
            // SaveImageButton
            // 
            this.SaveImageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveImageButton.BackColor = System.Drawing.Color.Transparent;
            this.SaveImageButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.SaveImageButton.CenterImage = ((System.Drawing.Image)(resources.GetObject("SaveImageButton.CenterImage")));
            this.SaveImageButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.SaveImageButton.HyperlinkStyle = false;
            //this.SaveImageButton.ImageMargin = 0;
            this.SaveImageButton.LeftImage = null;
            this.SaveImageButton.Location = new System.Drawing.Point(296, 0);
            this.SaveImageButton.Margin = new System.Windows.Forms.Padding(0);
            this.SaveImageButton.Name = "SaveImageButton";
            this.SaveImageButton.PushStyle = true;
            this.SaveImageButton.RightImage = null;
            this.SaveImageButton.Size = new System.Drawing.Size(22, 22);
            this.SaveImageButton.TabIndex = 0;
            this.SaveImageButton.TextAlign = System.Drawing.StringAlignment.Center;
            this.SaveImageButton.TextLeftMargin = 2;
            this.SaveImageButton.TextRightMargin = 2;
            this.SaveImageButton.Click += new System.EventHandler(this.SaveImageButton_Click);
            // 
            // MoreChartsButton
            // 
            this.MoreChartsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MoreChartsButton.BackColor = System.Drawing.Color.Transparent;
            this.MoreChartsButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.MoreChartsButton.CenterImage = global::TrailsPlugin.Properties.Resources.MoreCharts;
            this.MoreChartsButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.MoreChartsButton.HyperlinkStyle = false;
            //this.MoreChartsButton.ImageMargin = 1;
            this.MoreChartsButton.LeftImage = null;
            this.MoreChartsButton.Location = new System.Drawing.Point(270, 0);
            this.MoreChartsButton.Margin = new System.Windows.Forms.Padding(0);
            this.MoreChartsButton.Name = "MoreChartsButton";
            this.MoreChartsButton.PushStyle = true;
            this.MoreChartsButton.RightImage = null;
            this.MoreChartsButton.Size = new System.Drawing.Size(24, 24);
            this.MoreChartsButton.TabIndex = 0;
            this.MoreChartsButton.TextAlign = System.Drawing.StringAlignment.Center;
            this.MoreChartsButton.TextLeftMargin = 2;
            this.MoreChartsButton.TextRightMargin = 2;
            this.MoreChartsButton.Click += new System.EventHandler(this.moreCharts_Click);
            // 
            // TrailPointsButton
            // 
            this.TrailPointsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TrailPointsButton.BackColor = System.Drawing.Color.Transparent;
            this.TrailPointsButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.TrailPointsButton.CenterImage = global::TrailsPlugin.Properties.Resources.SplitPoints;
            this.TrailPointsButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.TrailPointsButton.HyperlinkStyle = false;
            //this.TrailPointsButton.ImageMargin = 1;
            this.TrailPointsButton.LeftImage = null;
            this.TrailPointsButton.Location = new System.Drawing.Point(244, 0);
            this.TrailPointsButton.Margin = new System.Windows.Forms.Padding(0);
            this.TrailPointsButton.Name = "TrailPointsButton";
            this.TrailPointsButton.PushStyle = true;
            this.TrailPointsButton.RightImage = null;
            this.TrailPointsButton.Size = new System.Drawing.Size(24, 24);
            this.TrailPointsButton.TabIndex = 0;
            this.TrailPointsButton.TextAlign = System.Drawing.StringAlignment.Center;
            this.TrailPointsButton.TextLeftMargin = 2;
            this.TrailPointsButton.TextRightMargin = 2;
            this.TrailPointsButton.Click += new System.EventHandler(this.TrailPoints_Click);
            // 
            // smoothingLabel
            // 
            this.smoothingLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.smoothingLabel.AutoSize = true;
            this.smoothingLabel.Location = new System.Drawing.Point(150, 4);
            this.smoothingLabel.Name = "smoothingLabel";
            this.smoothingLabel.Size = new System.Drawing.Size(52, 13);
            this.smoothingLabel.TabIndex = 3;
            this.smoothingLabel.Text = "<smoothing>";
            // 
            // smoothingPicker
            // 
            this.smoothingPicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.smoothingPicker.Location = new System.Drawing.Point(200, 0);
            this.smoothingPicker.Name = "smoothingPicker";
            this.smoothingPicker.Size = new System.Drawing.Size(39, 20);
            this.smoothingPicker.TabIndex = 7;
            this.smoothingPicker.MouseUp += new System.Windows.Forms.MouseEventHandler(smoothingPicker_LostFocus);
            this.smoothingPicker.LostFocus += new System.EventHandler(smoothingPicker_LostFocus);
            // 
            // MainChart
            // 
            this.MainChart.AutoSize = true;
            this.MainChart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MainChart.BackColor = System.Drawing.Color.Transparent;
            this.MainChart.Border = ZoneFiveSoftware.Common.Visuals.ControlBorder.Style.None;
            this.MainChart.ContextMenuStrip = this.chartContextMenu;
            this.MainChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainChart.Location = new System.Drawing.Point(3, 28);
            this.MainChart.Name = "MainChart";
            this.MainChart.Size = new System.Drawing.Size(394, 1);
            this.MainChart.TabIndex = 0;
            this.MainChart.SelectData += new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataHandler(this.MainChart_SelectData);
            this.MainChart.SelectingData += new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataHandler(this.MainChart_SelectingData);
            this.MainChart.SelectAxisLabel += new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectAxisLabelHandler(this.MainChart_SelectAxisLabel);
            this.MainChart.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainChart_KeyDown);
            this.MainChart.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainChart_KeyUp);
            this.MainChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainChart_MouseDown);
            this.MainChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainChart_MouseMove);
            this.MainChart.MouseLeave += MainChart_MouseLeave;
            //this.MainChart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainChart_MouseUp);
            // 
            // chartContextMenu
            // 
            this.chartContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moreChartsMenuItem,
            this.fitToWindowMenuItem,
            this.saveImageMenuItem,
            this.copyChartMenuItem});
            this.chartContextMenu.Name = "chartContextMenu";
            this.chartContextMenu.Size = new System.Drawing.Size(163, 92);
            // 
            // copyChartMenuItem
            // 
            this.copyChartMenuItem.Name = "copyChartMenuItem";
            this.copyChartMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.DocumentCopy16;
            this.copyChartMenuItem.Size = new System.Drawing.Size(162, 22);
            this.copyChartMenuItem.Text = "<Copy>";
            this.copyChartMenuItem.Click += new System.EventHandler(this.copyCharts_Click);
            // 
            // moreChartsMenuItem
            // 
            this.moreChartsMenuItem.Name = "moreChartsMenuItem";
            this.moreChartsMenuItem.Image = Properties.Resources.MoreCharts;
            this.moreChartsMenuItem.Size = new System.Drawing.Size(162, 22);
            this.moreChartsMenuItem.Text = "<more charts>";
            this.moreChartsMenuItem.Click += new System.EventHandler(this.moreCharts_Click);
            // 
            // fitToWindowMenuItem
            // 
            this.fitToWindowMenuItem.Name = "fitToWindowMenuItem";
            this.fitToWindowMenuItem.Image = Properties.Resources.ZoomToContent;
            this.fitToWindowMenuItem.Size = new System.Drawing.Size(162, 22);
            this.fitToWindowMenuItem.Text = "<Fit to window>";
            this.fitToWindowMenuItem.Click += new System.EventHandler(this.ZoomToContentButton_Click);
            // 
            // saveImageMenuItem
            // 
            this.saveImageMenuItem.Name = "saveImageMenuItem";
            this.saveImageMenuItem.Size = new System.Drawing.Size(162, 22);
            this.saveImageMenuItem.Text = "<Save>";
            this.saveImageMenuItem.Click += new System.EventHandler(this.SaveImageButton_Click);
            // 
            // TrailLineChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.chartTablePanel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "TrailLineChart";
            this.Size = new System.Drawing.Size(400, 31);
            ((System.ComponentModel.ISupportInitialize)(this.smoothingPicker)).EndInit();
            this.chartTablePanel.ResumeLayout(false);
            this.chartTablePanel.PerformLayout();
            this.ButtonPanel.ResumeLayout(false);
            this.chartContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private System.Windows.Forms.TableLayoutPanel chartTablePanel;
        private ZoneFiveSoftware.Common.Visuals.Chart.LineChart MainChart;
        private ZoneFiveSoftware.Common.Visuals.Panel ButtonPanel;
        private System.Windows.Forms.ContextMenuStrip chartContextMenu;
        private System.Windows.Forms.ToolStripMenuItem moreChartsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyChartMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fitToWindowMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveImageMenuItem;
        private ZoneFiveSoftware.Common.Visuals.Button ZoomOutButton;
        private ZoneFiveSoftware.Common.Visuals.Button ZoomInButton;
        private ZoneFiveSoftware.Common.Visuals.Button ZoomToContentButton;
        private ZoneFiveSoftware.Common.Visuals.Button SaveImageButton;
        private ZoneFiveSoftware.Common.Visuals.Button MoreChartsButton;
        private ZoneFiveSoftware.Common.Visuals.Button TrailPointsButton;
        private System.Windows.Forms.Label smoothingLabel;
        private System.Windows.Forms.NumericUpDown smoothingPicker;
        private System.Windows.Forms.ToolTip summaryListToolTip = new System.Windows.Forms.ToolTip();
    }
}
