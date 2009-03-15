namespace TrailsPlugin.UI.Settings {
	partial class SettingsPageControl {
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
			this.PluginInfoPanel = new ZoneFiveSoftware.Common.Visuals.Panel();
			this.PluginInfoBanner = new ZoneFiveSoftware.Common.Visuals.ActionBanner();
			this.label1 = new System.Windows.Forms.Label();
			this.PluginInfoPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// PluginInfoPanel
			// 
			this.PluginInfoPanel.BackColor = System.Drawing.Color.Transparent;
			this.PluginInfoPanel.BorderColor = System.Drawing.Color.Gray;
			this.PluginInfoPanel.Controls.Add(this.label1);
			this.PluginInfoPanel.Controls.Add(this.PluginInfoBanner);
			this.PluginInfoPanel.HeadingBackColor = System.Drawing.Color.LightBlue;
			this.PluginInfoPanel.HeadingFont = null;
			this.PluginInfoPanel.HeadingLeftMargin = 0;
			this.PluginInfoPanel.HeadingText = null;
			this.PluginInfoPanel.HeadingTextColor = System.Drawing.Color.Black;
			this.PluginInfoPanel.HeadingTopMargin = 3;
			this.PluginInfoPanel.Location = new System.Drawing.Point(0, 0);
			this.PluginInfoPanel.Name = "PluginInfoPanel";
			this.PluginInfoPanel.Size = new System.Drawing.Size(461, 147);
			this.PluginInfoPanel.TabIndex = 0;
			// 
			// PluginInfoBanner
			// 
			this.PluginInfoBanner.BackColor = System.Drawing.Color.Transparent;
			this.PluginInfoBanner.HasMenuButton = false;
			this.PluginInfoBanner.Location = new System.Drawing.Point(0, 0);
			this.PluginInfoBanner.Margin = new System.Windows.Forms.Padding(0);
			this.PluginInfoBanner.Name = "PluginInfoBanner";
			this.PluginInfoBanner.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.PluginInfoBanner.Size = new System.Drawing.Size(461, 23);
			this.PluginInfoBanner.Style = ZoneFiveSoftware.Common.Visuals.ActionBanner.BannerStyle.Header2;
			this.PluginInfoBanner.TabIndex = 0;
			this.PluginInfoBanner.Text = "Plugin Information";
			this.PluginInfoBanner.UseStyleFont = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(161, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Copyright Brendan Doherty 2009";
			// 
			// SettingsPageControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.PluginInfoPanel);
			this.Name = "SettingsPageControl";
			this.Size = new System.Drawing.Size(461, 150);
			this.PluginInfoPanel.ResumeLayout(false);
			this.PluginInfoPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ZoneFiveSoftware.Common.Visuals.Panel PluginInfoPanel;
		private ZoneFiveSoftware.Common.Visuals.ActionBanner PluginInfoBanner;
		private System.Windows.Forms.Label label1;



	}
}
