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
			this.ButtonPanel = new ZoneFiveSoftware.Common.Visuals.Panel();
			this.ZoomInButton = new ZoneFiveSoftware.Common.Visuals.Button();
			this.ZoomOutButton = new ZoneFiveSoftware.Common.Visuals.Button();
			this.ZoomToContentButton = new ZoneFiveSoftware.Common.Visuals.Button();
			this.SaveImageButton = new ZoneFiveSoftware.Common.Visuals.Button();
			this.MainChart = new ZoneFiveSoftware.Common.Visuals.Chart.LineChart();
			this.ButtonPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// ButtonPanel
			// 
			this.ButtonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ButtonPanel.BackColor = System.Drawing.Color.Transparent;
			this.ButtonPanel.Border = ZoneFiveSoftware.Common.Visuals.ControlBorder.Style.Square;
			this.ButtonPanel.BorderColor = System.Drawing.Color.Gray;
			this.ButtonPanel.Controls.Add(this.ZoomInButton);
			this.ButtonPanel.Controls.Add(this.ZoomOutButton);
			this.ButtonPanel.Controls.Add(this.ZoomToContentButton);
			this.ButtonPanel.Controls.Add(this.SaveImageButton);
			this.ButtonPanel.HeadingBackColor = System.Drawing.Color.LightBlue;
			this.ButtonPanel.HeadingFont = null;
			this.ButtonPanel.HeadingLeftMargin = 0;
			this.ButtonPanel.HeadingText = null;
			this.ButtonPanel.HeadingTextColor = System.Drawing.Color.Black;
			this.ButtonPanel.HeadingTopMargin = 0;
			this.ButtonPanel.Location = new System.Drawing.Point(0, 0);
			this.ButtonPanel.Margin = new System.Windows.Forms.Padding(0);
			this.ButtonPanel.Name = "ButtonPanel";
			this.ButtonPanel.Size = new System.Drawing.Size(398, 24);
			this.ButtonPanel.TabIndex = 1;
			// 
			// ZoomInButton
			// 
			this.ZoomInButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ZoomInButton.BackColor = System.Drawing.Color.Transparent;
			this.ZoomInButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
			this.ZoomInButton.CenterImage = null;
			this.ZoomInButton.DialogResult = System.Windows.Forms.DialogResult.None;
			this.ZoomInButton.HyperlinkStyle = false;
			this.ZoomInButton.ImageMargin = 2;
			this.ZoomInButton.LeftImage = null;
			this.ZoomInButton.Location = new System.Drawing.Point(372, 0);
			this.ZoomInButton.Margin = new System.Windows.Forms.Padding(0);
			this.ZoomInButton.Name = "ZoomInButton";
			this.ZoomInButton.PushStyle = true;
			this.ZoomInButton.RightImage = null;
			this.ZoomInButton.Size = new System.Drawing.Size(24, 24);
			this.ZoomInButton.TabIndex = 0;
			this.ZoomInButton.TextAlign = System.Drawing.StringAlignment.Center;
			this.ZoomInButton.TextLeftMargin = 2;
			this.ZoomInButton.TextRightMargin = 2;
			this.ZoomInButton.Click += new System.EventHandler(this.ZoomInButton_Click);
			// 
			// ZoomOutButton
			// 
			this.ZoomOutButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ZoomOutButton.BackColor = System.Drawing.Color.Transparent;
			this.ZoomOutButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
			this.ZoomOutButton.CenterImage = null;
			this.ZoomOutButton.DialogResult = System.Windows.Forms.DialogResult.None;
			this.ZoomOutButton.HyperlinkStyle = false;
			this.ZoomOutButton.ImageMargin = 2;
			this.ZoomOutButton.LeftImage = null;
			this.ZoomOutButton.Location = new System.Drawing.Point(348, 0);
			this.ZoomOutButton.Margin = new System.Windows.Forms.Padding(0);
			this.ZoomOutButton.Name = "ZoomOutButton";
			this.ZoomOutButton.PushStyle = true;
			this.ZoomOutButton.RightImage = null;
			this.ZoomOutButton.Size = new System.Drawing.Size(24, 24);
			this.ZoomOutButton.TabIndex = 0;
			this.ZoomOutButton.TextAlign = System.Drawing.StringAlignment.Center;
			this.ZoomOutButton.TextLeftMargin = 2;
			this.ZoomOutButton.TextRightMargin = 2;
			this.ZoomOutButton.Click += new System.EventHandler(this.ZoomOutButton_Click);
			// 
			// ZoomToContentButton
			// 
			this.ZoomToContentButton.BackColor = System.Drawing.Color.Transparent;
			this.ZoomToContentButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
			this.ZoomToContentButton.CenterImage = null;
			this.ZoomToContentButton.DialogResult = System.Windows.Forms.DialogResult.None;
			this.ZoomToContentButton.HyperlinkStyle = false;
			this.ZoomToContentButton.ImageMargin = 2;
			this.ZoomToContentButton.LeftImage = null;
			this.ZoomToContentButton.Location = new System.Drawing.Point(0, 0);
			this.ZoomToContentButton.Name = "ZoomToContentButton";
			this.ZoomToContentButton.PushStyle = true;
			this.ZoomToContentButton.RightImage = null;
			this.ZoomToContentButton.Size = new System.Drawing.Size(75, 23);
			this.ZoomToContentButton.TabIndex = 1;
			this.ZoomToContentButton.TextAlign = System.Drawing.StringAlignment.Center;
			this.ZoomToContentButton.TextLeftMargin = 2;
			this.ZoomToContentButton.TextRightMargin = 2;
			// 
			// SaveImageButton
			// 
			this.SaveImageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.SaveImageButton.BackColor = System.Drawing.Color.Transparent;
			this.SaveImageButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
			this.SaveImageButton.CenterImage = null;
			this.SaveImageButton.DialogResult = System.Windows.Forms.DialogResult.None;
			this.SaveImageButton.HyperlinkStyle = false;
			this.SaveImageButton.ImageMargin = 2;
			this.SaveImageButton.LeftImage = null;
			this.SaveImageButton.Location = new System.Drawing.Point(300, 0);
			this.SaveImageButton.Margin = new System.Windows.Forms.Padding(0);
			this.SaveImageButton.Name = "SaveImageButton";
			this.SaveImageButton.PushStyle = true;
			this.SaveImageButton.RightImage = null;
			this.SaveImageButton.Size = new System.Drawing.Size(24, 24);
			this.SaveImageButton.TabIndex = 0;
			this.SaveImageButton.TextAlign = System.Drawing.StringAlignment.Center;
			this.SaveImageButton.TextLeftMargin = 2;
			this.SaveImageButton.TextRightMargin = 2;
			this.SaveImageButton.Click += new System.EventHandler(this.SaveImageButton_Click);
			// 
			// MainChart
			// 
			this.MainChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.MainChart.BackColor = System.Drawing.Color.Transparent;
			this.MainChart.Border = ZoneFiveSoftware.Common.Visuals.ControlBorder.Style.Square;
			this.MainChart.Location = new System.Drawing.Point(0, 23);
			this.MainChart.Name = "MainChart";
			this.MainChart.Size = new System.Drawing.Size(398, 300);
			this.MainChart.TabIndex = 0;
			// 
			// TrailLineChart
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.MainChart);
			this.Controls.Add(this.ButtonPanel);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "TrailLineChart";
			this.Size = new System.Drawing.Size(400, 325);
			this.ButtonPanel.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion


		private ZoneFiveSoftware.Common.Visuals.Chart.LineChart MainChart;
        private ZoneFiveSoftware.Common.Visuals.Panel ButtonPanel;
        private ZoneFiveSoftware.Common.Visuals.Button ZoomOutButton;
        private ZoneFiveSoftware.Common.Visuals.Button ZoomInButton;
        private ZoneFiveSoftware.Common.Visuals.Button ZoomToContentButton;
        private ZoneFiveSoftware.Common.Visuals.Button SaveImageButton;
    }
}
