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
			this.ChartBanner.SuspendLayout();
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
			this.btnCollapse.Location = new System.Drawing.Point(350, 0);
			this.btnCollapse.Name = "btnCollapse";
			this.btnCollapse.Padding = new System.Windows.Forms.Padding(2);
			this.btnCollapse.PushStyle = false;
			this.btnCollapse.RightImage = null;
			this.btnCollapse.Size = new System.Drawing.Size(19, 19);
			this.btnCollapse.TabIndex = 12;
			this.btnCollapse.Text = "X";
			this.btnCollapse.TextAlign = System.Drawing.StringAlignment.Center;
			this.btnCollapse.TextLeftMargin = 2;
			this.btnCollapse.TextRightMargin = 2;
			this.btnCollapse.Click += new System.EventHandler(this.btnCollapse_Click);
			// 
			// ChartsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.ChartBanner);
			this.Name = "ChartsControl";
			this.Size = new System.Drawing.Size(400, 300);
			this.ChartBanner.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ZoneFiveSoftware.Common.Visuals.ActionBanner ChartBanner;
		private ZoneFiveSoftware.Common.Visuals.Button btnCollapse;
	}
}
