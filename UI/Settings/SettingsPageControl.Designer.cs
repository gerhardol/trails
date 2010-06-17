﻿namespace TrailsPlugin.UI.Settings {
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
            this.components = new System.ComponentModel.Container();
            this.PluginInfoPanel = new ZoneFiveSoftware.Common.Visuals.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtDefaultRadius = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.lblDefaultRadius = new System.Windows.Forms.Label();
            this.lblLicense = new System.Windows.Forms.Label();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.PluginInfoBanner = new ZoneFiveSoftware.Common.Visuals.ActionBanner();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.PluginInfoPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PluginInfoPanel
            // 
            this.PluginInfoPanel.AutoSize = true;
            this.PluginInfoPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PluginInfoPanel.BackColor = System.Drawing.Color.Transparent;
            this.PluginInfoPanel.BorderColor = System.Drawing.Color.Gray;
            this.PluginInfoPanel.Controls.Add(this.tableLayoutPanel1);
            this.PluginInfoPanel.Controls.Add(this.lblLicense);
            this.PluginInfoPanel.Controls.Add(this.lblCopyright);
            this.PluginInfoPanel.Controls.Add(this.PluginInfoBanner);
            this.PluginInfoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PluginInfoPanel.HeadingBackColor = System.Drawing.Color.LightBlue;
            this.PluginInfoPanel.HeadingFont = null;
            this.PluginInfoPanel.HeadingLeftMargin = 0;
            this.PluginInfoPanel.HeadingText = null;
            this.PluginInfoPanel.HeadingTextColor = System.Drawing.Color.Black;
            this.PluginInfoPanel.HeadingTopMargin = 3;
            this.PluginInfoPanel.Location = new System.Drawing.Point(0, 0);
            this.PluginInfoPanel.Name = "PluginInfoPanel";
            this.PluginInfoPanel.Size = new System.Drawing.Size(409, 129);
            this.PluginInfoPanel.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.txtDefaultRadius, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblDefaultRadius, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 26);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(406, 48);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // txtDefaultRadius
            // 
            this.txtDefaultRadius.AcceptsReturn = false;
            this.txtDefaultRadius.AcceptsTab = false;
            this.txtDefaultRadius.BackColor = System.Drawing.Color.White;
            this.txtDefaultRadius.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.txtDefaultRadius.ButtonImage = null;
            this.txtDefaultRadius.Location = new System.Drawing.Point(150, 10);
            this.txtDefaultRadius.Margin = new System.Windows.Forms.Padding(0);
            this.txtDefaultRadius.MaxLength = 32767;
            this.txtDefaultRadius.Multiline = false;
            this.txtDefaultRadius.Name = "txtDefaultRadius";
            this.txtDefaultRadius.ReadOnly = false;
            this.txtDefaultRadius.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.txtDefaultRadius.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.txtDefaultRadius.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtDefaultRadius.Size = new System.Drawing.Size(100, 19);
            this.txtDefaultRadius.TabIndex = 1;
            this.txtDefaultRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtDefaultRadius.LostFocus += new System.EventHandler(this.txtDefaultRadius_LostFocus);
            // 
            // lblDefaultRadius
            // 
            this.lblDefaultRadius.AutoSize = true;
            this.lblDefaultRadius.Location = new System.Drawing.Point(3, 13);
            this.lblDefaultRadius.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblDefaultRadius.Name = "lblDefaultRadius";
            this.lblDefaultRadius.Size = new System.Drawing.Size(80, 13);
            this.lblDefaultRadius.TabIndex = 0;
            this.lblDefaultRadius.Text = "Default Radius:";
            // 
            // lblLicense
            // 
            this.lblLicense.AutoSize = true;
            this.lblLicense.Location = new System.Drawing.Point(3, 90);
            this.lblLicense.Name = "lblLicense";
            this.lblLicense.Size = new System.Drawing.Size(350, 39);
            this.lblLicense.TabIndex = 3;
            this.lblLicense.Text = "Trails Plugin is distributed under the GNU Lesser General Public Licence.\r\nThe Li" +
                "cense is included in the plugin installation directory and at:\r\nhttp://www.gnu.o" +
                "rg/licenses/lgpl.html.";
            // 
            // lblCopyright
            // 
            this.lblCopyright.AutoSize = true;
            this.lblCopyright.Location = new System.Drawing.Point(3, 77);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(161, 13);
            this.lblCopyright.TabIndex = 1;
            this.lblCopyright.Text = "Copyright Brendan Doherty 2009";
            // 
            // PluginInfoBanner
            // 
            this.PluginInfoBanner.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PluginInfoBanner.BackColor = System.Drawing.Color.Transparent;
            this.PluginInfoBanner.HasMenuButton = false;
            this.PluginInfoBanner.Location = new System.Drawing.Point(0, 0);
            this.PluginInfoBanner.Margin = new System.Windows.Forms.Padding(0);
            this.PluginInfoBanner.Name = "PluginInfoBanner";
            this.PluginInfoBanner.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.PluginInfoBanner.Size = new System.Drawing.Size(409, 23);
            this.PluginInfoBanner.Style = ZoneFiveSoftware.Common.Visuals.ActionBanner.BannerStyle.Header2;
            this.PluginInfoBanner.TabIndex = 0;
            this.PluginInfoBanner.Text = "Plugin Information";
            this.PluginInfoBanner.UseStyleFont = true;
            // 
            // SettingsPageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.PluginInfoPanel);
            this.Name = "SettingsPageControl";
            this.Size = new System.Drawing.Size(409, 129);
            this.PluginInfoPanel.ResumeLayout(false);
            this.PluginInfoPanel.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private ZoneFiveSoftware.Common.Visuals.Panel PluginInfoPanel;
		private ZoneFiveSoftware.Common.Visuals.ActionBanner PluginInfoBanner;
        private System.Windows.Forms.Label lblCopyright;
		private System.Windows.Forms.Label lblLicense;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label lblDefaultRadius;
		private ZoneFiveSoftware.Common.Visuals.TextBox txtDefaultRadius;
		private System.Windows.Forms.ToolTip toolTip;



	}
}
