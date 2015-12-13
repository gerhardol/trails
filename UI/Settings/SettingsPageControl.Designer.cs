using System;

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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.PluginInfoBanner = new ZoneFiveSoftware.Common.Visuals.ActionBanner();
            this.lblInfo = new System.Windows.Forms.LinkLabel();
            this.lblDefaultRadius = new System.Windows.Forms.Label();
            this.txtDefaultRadius = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.lblSetNameAtImport = new System.Windows.Forms.Label();
            this.txtSetNameAtImport = new System.Windows.Forms.CheckBox();
            this.lblStoppedCategory = new System.Windows.Forms.Label();
            this.boxStoppedCategory = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.lblBarometricDevices = new System.Windows.Forms.Label();
            this.boxBarometricDevices = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.lblAdjustElevationAtImport = new System.Windows.Forms.Label();
            this.txtAdjustElevationAtImport = new System.Windows.Forms.CheckBox();
            this.lblUniqueRoutes = new System.Windows.Forms.Label();
            this.lblHighScore = new System.Windows.Forms.Label();
            this.lblPerformancePredictor = new System.Windows.Forms.Label();
            this.lblPredictDistance = new System.Windows.Forms.Label();
            this.boxPredictDistance = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.lblLicense = new System.Windows.Forms.Label();
            this.labelRouteTransparency = new System.Windows.Forms.Label();
            this.upDownRouteTransparency = new System.Windows.Forms.NumericUpDown();
            this.labelRouteTransparencyPercent = new System.Windows.Forms.Label();
            this.labelMaxChartResults = new System.Windows.Forms.Label();
            this.upDownMaxChartResults = new System.Windows.Forms.NumericUpDown();
            this.gradeAdjustedPaceGroup = new System.Windows.Forms.GroupBox();
            this.tablePanelGradeAdjustedPace = new System.Windows.Forms.TableLayoutPanel();
            this.lblMervynDaviesName = new System.Windows.Forms.Label();
            this.lblMervynDaviesUp = new System.Windows.Forms.Label();
            this.boxMervynDaviesUp = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.lblMervynDaviesDown = new System.Windows.Forms.Label();
            this.boxMervynDaviesDown = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.lblJackDanielsName = new System.Windows.Forms.Label();
            this.lblJackDanielsUp = new System.Windows.Forms.Label();
            this.boxJackDanielsUp = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.lblJackDanielsDown = new System.Windows.Forms.Label();
            this.boxJackDanielsDown = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.PluginInfoPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.gradeAdjustedPaceGroup.SuspendLayout();
            this.tablePanelGradeAdjustedPace.SuspendLayout();
            this.SuspendLayout();
            // 
            // PluginInfoPanel
            // 
            this.PluginInfoPanel.AutoSize = true;
            this.PluginInfoPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PluginInfoPanel.BackColor = System.Drawing.Color.Transparent;
            this.PluginInfoPanel.BorderColor = System.Drawing.Color.Gray;
            this.PluginInfoPanel.Controls.Add(this.tableLayoutPanel1);
            this.PluginInfoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PluginInfoPanel.HeadingBackColor = System.Drawing.Color.LightBlue;
            this.PluginInfoPanel.HeadingFont = null;
            this.PluginInfoPanel.HeadingLeftMargin = 0;
            this.PluginInfoPanel.HeadingText = null;
            this.PluginInfoPanel.HeadingTextColor = System.Drawing.Color.Black;
            this.PluginInfoPanel.HeadingTopMargin = 3;
            this.PluginInfoPanel.Location = new System.Drawing.Point(0, 0);
            this.PluginInfoPanel.Name = "PluginInfoPanel";
            this.PluginInfoPanel.Size = new System.Drawing.Size(362, 466);
            this.PluginInfoPanel.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 155F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.PluginInfoBanner, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblInfo, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblDefaultRadius, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtDefaultRadius, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblSetNameAtImport, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.txtSetNameAtImport, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblStoppedCategory, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.boxStoppedCategory, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.lblBarometricDevices, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.boxBarometricDevices, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.lblAdjustElevationAtImport, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.txtAdjustElevationAtImport, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.labelRouteTransparency, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.upDownRouteTransparency, 1, 8);
            //this.tableLayoutPanel1.Controls.Add(this.labelRouteTransparencyPercent, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.labelMaxChartResults, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.upDownMaxChartResults, 1, 9);
            this.tableLayoutPanel1.Controls.Add(this.gradeAdjustedPaceGroup, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.lblUniqueRoutes, 0, 12);
            this.tableLayoutPanel1.Controls.Add(this.lblHighScore, 0, 13);
            this.tableLayoutPanel1.Controls.Add(this.lblPerformancePredictor, 0, 14);
            this.tableLayoutPanel1.Controls.Add(this.lblPredictDistance, 0, 15);
            this.tableLayoutPanel1.Controls.Add(this.boxPredictDistance, 1, 15);
            this.tableLayoutPanel1.Controls.Add(this.lblCopyright, 0, 17);
            this.tableLayoutPanel1.Controls.Add(this.lblLicense, 0, 18);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 19;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 137F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(362, 466);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // PluginInfoBanner
            // 
            this.PluginInfoBanner.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PluginInfoBanner.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.SetColumnSpan(this.PluginInfoBanner, 2);
            this.PluginInfoBanner.HasMenuButton = false;
            this.PluginInfoBanner.Location = new System.Drawing.Point(0, 0);
            this.PluginInfoBanner.Margin = new System.Windows.Forms.Padding(0);
            this.PluginInfoBanner.Name = "PluginInfoBanner";
            this.PluginInfoBanner.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.PluginInfoBanner.Size = new System.Drawing.Size(362, 20);
            this.PluginInfoBanner.Style = ZoneFiveSoftware.Common.Visuals.ActionBanner.BannerStyle.Header2;
            this.PluginInfoBanner.TabIndex = 0;
            this.PluginInfoBanner.Text = "Plugin Information";
            this.PluginInfoBanner.UseStyleFont = true;
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblInfo, 2);
            this.lblInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInfo.Location = new System.Drawing.Point(3, 407);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(356, 20);
            this.lblInfo.TabIndex = 1;
            this.lblInfo.Text = "<info";
            this.lblInfo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblInfo_LinkClicked);
            // 
            // lblDefaultRadius
            // 
            this.lblDefaultRadius.AutoSize = true;
            this.lblDefaultRadius.Location = new System.Drawing.Point(3, 33);
            this.lblDefaultRadius.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblDefaultRadius.Name = "lblDefaultRadius";
            this.lblDefaultRadius.Size = new System.Drawing.Size(86, 13);
            this.lblDefaultRadius.TabIndex = 0;
            this.lblDefaultRadius.Text = "<Default Radius:";
            // 
            // txtDefaultRadius
            // 
            this.txtDefaultRadius.AcceptsReturn = false;
            this.txtDefaultRadius.AcceptsTab = false;
            this.txtDefaultRadius.BackColor = System.Drawing.Color.White;
            this.txtDefaultRadius.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.txtDefaultRadius.ButtonImage = null;
            this.txtDefaultRadius.Location = new System.Drawing.Point(155, 30);
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
            // lblSetNameAtImport
            // 
            this.lblSetNameAtImport.AutoSize = true;
            this.lblSetNameAtImport.Location = new System.Drawing.Point(3, 53);
            this.lblSetNameAtImport.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblSetNameAtImport.Name = "lblSetNameAtImport";
            this.lblSetNameAtImport.Size = new System.Drawing.Size(96, 13);
            this.lblSetNameAtImport.TabIndex = 0;
            this.lblSetNameAtImport.Text = "<SetNameAtImport";
            // 
            // txtSetNameAtImport
            // 
            this.txtSetNameAtImport.AutoSize = true;
            this.txtSetNameAtImport.Location = new System.Drawing.Point(158, 53);
            this.txtSetNameAtImport.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.txtSetNameAtImport.Name = "txtSetNameAtImport";
            this.txtSetNameAtImport.Size = new System.Drawing.Size(15, 14);
            this.txtSetNameAtImport.TabIndex = 0;
            this.txtSetNameAtImport.CheckedChanged += new System.EventHandler(this.txtSetNameAtImport_CheckedChanged);
            // 
            // lblStoppedCategory
            // 
            this.lblStoppedCategory.AutoSize = true;
            this.lblStoppedCategory.Location = new System.Drawing.Point(3, 73);
            this.lblStoppedCategory.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblStoppedCategory.Name = "lblStoppedCategory";
            this.lblStoppedCategory.Size = new System.Drawing.Size(95, 13);
            this.lblStoppedCategory.TabIndex = 4;
            this.lblStoppedCategory.Text = "<StoppedCategory";
            // 
            // boxStoppedCategory
            // 
            this.boxStoppedCategory.AcceptsReturn = false;
            this.boxStoppedCategory.AcceptsTab = false;
            this.boxStoppedCategory.BackColor = System.Drawing.Color.White;
            this.boxStoppedCategory.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.boxStoppedCategory.ButtonImage = null;
            this.boxStoppedCategory.Location = new System.Drawing.Point(155, 70);
            this.boxStoppedCategory.Margin = new System.Windows.Forms.Padding(0);
            this.boxStoppedCategory.MaxLength = 32767;
            this.boxStoppedCategory.Multiline = false;
            this.boxStoppedCategory.Name = "boxStoppedCategory";
            this.boxStoppedCategory.ReadOnly = false;
            this.boxStoppedCategory.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.boxStoppedCategory.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.boxStoppedCategory.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.boxStoppedCategory.Size = new System.Drawing.Size(100, 19);
            this.boxStoppedCategory.TabIndex = 5;
            this.boxStoppedCategory.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.boxStoppedCategory.LostFocus += new System.EventHandler(this.boxStoppedCategory_LostFocus);
            // 
            // lblBarometricDevices
            // 
            this.lblBarometricDevices.AutoSize = true;
            this.lblBarometricDevices.Location = new System.Drawing.Point(3, 93);
            this.lblBarometricDevices.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblBarometricDevices.Name = "lblBarometricDevices";
            this.lblBarometricDevices.Size = new System.Drawing.Size(102, 13);
            this.lblBarometricDevices.TabIndex = 4;
            this.lblBarometricDevices.Text = "<BarometricDevices";
            // 
            // boxBarometricDevices
            // 
            this.boxBarometricDevices.AcceptsReturn = false;
            this.boxBarometricDevices.AcceptsTab = false;
            this.boxBarometricDevices.BackColor = System.Drawing.Color.White;
            this.boxBarometricDevices.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.boxBarometricDevices.ButtonImage = null;
            this.boxBarometricDevices.Location = new System.Drawing.Point(155, 90);
            this.boxBarometricDevices.Margin = new System.Windows.Forms.Padding(0);
            this.boxBarometricDevices.MaxLength = 32767;
            this.boxBarometricDevices.Multiline = false;
            this.boxBarometricDevices.Name = "boxBarometricDevices";
            this.boxBarometricDevices.ReadOnly = false;
            this.boxBarometricDevices.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.boxBarometricDevices.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.boxBarometricDevices.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.boxBarometricDevices.Size = new System.Drawing.Size(100, 19);
            this.boxBarometricDevices.TabIndex = 5;
            this.boxBarometricDevices.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.boxBarometricDevices.LostFocus += new System.EventHandler(this.boxBarometricDevices_LostFocus);
            // 
            // lblAdjustElevationAtImport
            // 
            this.lblAdjustElevationAtImport.AutoSize = true;
            this.lblAdjustElevationAtImport.Location = new System.Drawing.Point(3, 113);
            this.lblAdjustElevationAtImport.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblAdjustElevationAtImport.Name = "lblAdjustElevationAtImport";
            this.lblAdjustElevationAtImport.Size = new System.Drawing.Size(125, 13);
            this.lblAdjustElevationAtImport.TabIndex = 0;
            this.lblAdjustElevationAtImport.Text = "<AdjustElevationAtImport";
            // 
            // txtAdjustElevationAtImport
            // 
            this.txtAdjustElevationAtImport.AutoSize = true;
            this.txtAdjustElevationAtImport.Location = new System.Drawing.Point(158, 113);
            this.txtAdjustElevationAtImport.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.txtAdjustElevationAtImport.Name = "txtAdjustElevationAtImport";
            this.txtAdjustElevationAtImport.Size = new System.Drawing.Size(15, 14);
            this.txtAdjustElevationAtImport.TabIndex = 0;
            this.txtAdjustElevationAtImport.CheckedChanged += new System.EventHandler(this.txtAdjustElevationAtImport_CheckedChanged);
            // 
            // labelRouteTransparency
            // 
            this.labelRouteTransparency.AutoSize = true;
            this.labelRouteTransparency.Location = new System.Drawing.Point(3, 133);
            this.labelRouteTransparency.Name = "labelRouteTransparency";
            this.labelRouteTransparency.Size = new System.Drawing.Size(142, 13);
            this.labelRouteTransparency.TabIndex = 3;
            this.labelRouteTransparency.Text = "<routeTransparency:";
            // 
            // upDownRouteTransparency
            // 
            this.upDownRouteTransparency.Location = new System.Drawing.Point(155, 133);
            this.upDownRouteTransparency.Name = "upDownRouteTransparency";
            this.upDownRouteTransparency.Size = new System.Drawing.Size(39, 20);
            this.upDownRouteTransparency.TabIndex = 7;
            this.upDownRouteTransparency.Minimum = 0;
            this.upDownRouteTransparency.Maximum = 100;
            this.upDownRouteTransparency.LostFocus += new EventHandler(upDownRouteTransparency_LostFocus);
            // 
            // labelRouteTransparencyPercent
            // (not visible)
            this.labelRouteTransparencyPercent.AutoSize = true;
            this.labelRouteTransparencyPercent.Location = new System.Drawing.Point(280, 42);
            this.labelRouteTransparencyPercent.Name = "labelRouteTransparencyPercent";
            this.labelRouteTransparencyPercent.Size = new System.Drawing.Size(15, 13);
            this.labelRouteTransparencyPercent.TabIndex = 10;
            this.labelRouteTransparencyPercent.Text = "%";
            this.labelRouteTransparencyPercent.Visible = false;
            // 
            // labelMaxChartResults
            // 
            this.labelMaxChartResults.AutoSize = true;
            this.labelMaxChartResults.Location = new System.Drawing.Point(3, 133);
            this.labelMaxChartResults.Name = "labelMaxChartResults";
            this.labelMaxChartResults.Size = new System.Drawing.Size(142, 13);
            this.labelMaxChartResults.TabIndex = 3;
            this.labelMaxChartResults.Text = "<MaxChartResults:";
            // 
            // upDownMaxChartResults
            // 
            this.upDownMaxChartResults.Location = new System.Drawing.Point(155, 133);
            this.upDownMaxChartResults.Name = "upDownMaxChartResults";
            this.upDownMaxChartResults.Size = new System.Drawing.Size(39, 20);
            this.upDownMaxChartResults.TabIndex = 7;
            this.upDownMaxChartResults.Minimum = 0;
            this.upDownMaxChartResults.Maximum = 10000;
            this.upDownMaxChartResults.LostFocus += new EventHandler(upDownMaxChartResults_LostFocus);
            // 
            // lblUniqueRoutes
            // 
            this.lblUniqueRoutes.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblUniqueRoutes, 2);
            this.lblUniqueRoutes.Location = new System.Drawing.Point(3, 282);
            this.lblUniqueRoutes.Name = "lblUniqueRoutes";
            this.lblUniqueRoutes.Size = new System.Drawing.Size(93, 26);
            this.lblUniqueRoutes.TabIndex = 1;
            this.lblUniqueRoutes.Text = "<UR placeholder>\r\nline2";
            // 
            // lblHighScore
            // 
            this.lblHighScore.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblHighScore, 2);
            this.lblHighScore.Location = new System.Drawing.Point(3, 312);
            this.lblHighScore.Name = "lblHighScore";
            this.lblHighScore.Size = new System.Drawing.Size(92, 26);
            this.lblHighScore.TabIndex = 1;
            this.lblHighScore.Text = "<HS placeholder>\r\nline2";
            // 
            // lblPerformancePredictor
            // 
            this.lblPerformancePredictor.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblPerformancePredictor, 2);
            this.lblPerformancePredictor.Location = new System.Drawing.Point(3, 342);
            this.lblPerformancePredictor.Name = "lblPerformancePredictor";
            this.lblPerformancePredictor.Size = new System.Drawing.Size(91, 26);
            this.lblPerformancePredictor.TabIndex = 1;
            this.lblPerformancePredictor.Text = "<PP placeholder>\r\nline2";
            // 
            // lblPredictDistance
            // 
            this.lblPredictDistance.AutoSize = true;
            this.lblPredictDistance.Location = new System.Drawing.Point(3, 375);
            this.lblPredictDistance.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblPredictDistance.Name = "lblPredictDistance";
            this.lblPredictDistance.Size = new System.Drawing.Size(91, 13);
            this.lblPredictDistance.TabIndex = 0;
            this.lblPredictDistance.Text = "<PredictDistance:";
            // 
            // boxPredictDistance
            // 
            this.boxPredictDistance.AcceptsReturn = false;
            this.boxPredictDistance.AcceptsTab = false;
            this.boxPredictDistance.BackColor = System.Drawing.Color.White;
            this.boxPredictDistance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.boxPredictDistance.ButtonImage = null;
            this.boxPredictDistance.Location = new System.Drawing.Point(155, 372);
            this.boxPredictDistance.Margin = new System.Windows.Forms.Padding(0);
            this.boxPredictDistance.MaxLength = 32767;
            this.boxPredictDistance.Multiline = false;
            this.boxPredictDistance.Name = "boxPredictDistance";
            this.boxPredictDistance.ReadOnly = false;
            this.boxPredictDistance.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.boxPredictDistance.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.boxPredictDistance.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.boxPredictDistance.Size = new System.Drawing.Size(100, 19);
            this.boxPredictDistance.TabIndex = 1;
            this.boxPredictDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.boxPredictDistance.LostFocus += new System.EventHandler(this.boxPredictDistance_LostFocus);
            // 
            // lblCopyright
            // 
            this.lblCopyright.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblCopyright, 2);
            this.lblCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCopyright.Location = new System.Drawing.Point(3, 407);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(356, 20);
            this.lblCopyright.TabIndex = 1;
            this.lblCopyright.Text = "<Copyright Brendan Doherty 2009";
            // 
            // lblLicense
            // 
            this.lblLicense.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblLicense, 2);
            this.lblLicense.Location = new System.Drawing.Point(3, 427);
            this.lblLicense.Name = "lblLicense";
            this.lblLicense.Size = new System.Drawing.Size(356, 39);
            this.lblLicense.TabIndex = 3;
            this.lblLicense.Text = "<Trails Plugin is distributed under the GNU Lesser General Public Licence.\r\nThe L" +
    "icense is included in the plugin installation directory and at:\r\nhttp://www.gnu." +
    "org/licenses/lgpl.html.";
            // 
            // gradeAdjustedPaceGroup
            // 
            this.gradeAdjustedPaceGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gradeAdjustedPaceGroup.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.gradeAdjustedPaceGroup, 2);
            this.gradeAdjustedPaceGroup.Controls.Add(this.tablePanelGradeAdjustedPace);
            this.gradeAdjustedPaceGroup.Location = new System.Drawing.Point(3, 133);
            this.gradeAdjustedPaceGroup.Name = "gradeAdjustedPaceGroup";
            this.gradeAdjustedPaceGroup.Size = new System.Drawing.Size(356, 131);
            this.gradeAdjustedPaceGroup.TabIndex = 6;
            this.gradeAdjustedPaceGroup.TabStop = false;
            this.gradeAdjustedPaceGroup.Text = "<Grade Adjusted Pace";
            // 
            // tablePanelGradeAdjustedPace
            // 
            this.tablePanelGradeAdjustedPace.AutoSize = true;
            this.tablePanelGradeAdjustedPace.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tablePanelGradeAdjustedPace.ColumnCount = 2;
            this.tablePanelGradeAdjustedPace.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 149F));
            this.tablePanelGradeAdjustedPace.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tablePanelGradeAdjustedPace.Controls.Add(this.lblMervynDaviesName, 0, 0);
            this.tablePanelGradeAdjustedPace.Controls.Add(this.lblMervynDaviesUp, 0, 1);
            this.tablePanelGradeAdjustedPace.Controls.Add(this.boxMervynDaviesUp, 1, 1);
            this.tablePanelGradeAdjustedPace.Controls.Add(this.lblMervynDaviesDown, 0, 2);
            this.tablePanelGradeAdjustedPace.Controls.Add(this.boxMervynDaviesDown, 1, 2);
            this.tablePanelGradeAdjustedPace.Controls.Add(this.lblJackDanielsName, 0, 4);
            this.tablePanelGradeAdjustedPace.Controls.Add(this.lblJackDanielsUp, 0, 5);
            this.tablePanelGradeAdjustedPace.Controls.Add(this.boxJackDanielsUp, 1, 5);
            this.tablePanelGradeAdjustedPace.Controls.Add(this.lblJackDanielsDown, 0, 6);
            this.tablePanelGradeAdjustedPace.Controls.Add(this.boxJackDanielsDown, 1, 6);
            this.tablePanelGradeAdjustedPace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablePanelGradeAdjustedPace.Location = new System.Drawing.Point(3, 16);
            this.tablePanelGradeAdjustedPace.Name = "tablePanelGradeAdjustedPace";
            this.tablePanelGradeAdjustedPace.RowCount = 7;
            this.tablePanelGradeAdjustedPace.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tablePanelGradeAdjustedPace.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tablePanelGradeAdjustedPace.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tablePanelGradeAdjustedPace.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 3F));
            this.tablePanelGradeAdjustedPace.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tablePanelGradeAdjustedPace.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tablePanelGradeAdjustedPace.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tablePanelGradeAdjustedPace.Size = new System.Drawing.Size(350, 112);
            this.tablePanelGradeAdjustedPace.TabIndex = 0;
            // 
            // lblMervynDaviesName
            // 
            this.lblMervynDaviesName.AutoSize = true;
            this.tablePanelGradeAdjustedPace.SetColumnSpan(this.lblMervynDaviesName, 2);
            this.lblMervynDaviesName.Location = new System.Drawing.Point(3, 3);
            this.lblMervynDaviesName.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblMervynDaviesName.Name = "lblMervynDaviesName";
            this.lblMervynDaviesName.Size = new System.Drawing.Size(84, 13);
            this.lblMervynDaviesName.TabIndex = 1;
            this.lblMervynDaviesName.Text = "<MervynDavies:";
            // 
            // lblMervynDaviesUp
            // 
            this.lblMervynDaviesUp.AutoSize = true;
            this.lblMervynDaviesUp.Location = new System.Drawing.Point(3, 19);
            this.lblMervynDaviesUp.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblMervynDaviesUp.Name = "lblMervynDaviesUp";
            this.lblMervynDaviesUp.Size = new System.Drawing.Size(30, 13);
            this.lblMervynDaviesUp.TabIndex = 1;
            this.lblMervynDaviesUp.Text = "<Up:";
            // 
            // boxMervynDaviesUp
            // 
            this.boxMervynDaviesUp.AcceptsReturn = false;
            this.boxMervynDaviesUp.AcceptsTab = false;
            this.boxMervynDaviesUp.BackColor = System.Drawing.Color.White;
            this.boxMervynDaviesUp.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.boxMervynDaviesUp.ButtonImage = null;
            this.boxMervynDaviesUp.Location = new System.Drawing.Point(149, 16);
            this.boxMervynDaviesUp.Margin = new System.Windows.Forms.Padding(0);
            this.boxMervynDaviesUp.MaxLength = 32767;
            this.boxMervynDaviesUp.Multiline = false;
            this.boxMervynDaviesUp.Name = "boxMervynDaviesUp";
            this.boxMervynDaviesUp.ReadOnly = false;
            this.boxMervynDaviesUp.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.boxMervynDaviesUp.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.boxMervynDaviesUp.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.boxMervynDaviesUp.Size = new System.Drawing.Size(100, 19);
            this.boxMervynDaviesUp.TabIndex = 2;
            this.boxMervynDaviesUp.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.boxMervynDaviesUp.LostFocus += new System.EventHandler(this.boxMervynDavies_LostFocus);
            // 
            // lblMervynDaviesDown
            // 
            this.lblMervynDaviesDown.AutoSize = true;
            this.lblMervynDaviesDown.Location = new System.Drawing.Point(3, 38);
            this.lblMervynDaviesDown.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblMervynDaviesDown.Name = "lblMervynDaviesDown";
            this.lblMervynDaviesDown.Size = new System.Drawing.Size(44, 13);
            this.lblMervynDaviesDown.TabIndex = 3;
            this.lblMervynDaviesDown.Text = "<Down:";
            // 
            // boxMervynDaviesDown
            // 
            this.boxMervynDaviesDown.AcceptsReturn = false;
            this.boxMervynDaviesDown.AcceptsTab = false;
            this.boxMervynDaviesDown.BackColor = System.Drawing.Color.White;
            this.boxMervynDaviesDown.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.boxMervynDaviesDown.ButtonImage = null;
            this.boxMervynDaviesDown.Location = new System.Drawing.Point(149, 35);
            this.boxMervynDaviesDown.Margin = new System.Windows.Forms.Padding(0);
            this.boxMervynDaviesDown.MaxLength = 32767;
            this.boxMervynDaviesDown.Multiline = false;
            this.boxMervynDaviesDown.Name = "boxMervynDaviesDown";
            this.boxMervynDaviesDown.ReadOnly = false;
            this.boxMervynDaviesDown.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.boxMervynDaviesDown.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.boxMervynDaviesDown.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.boxMervynDaviesDown.Size = new System.Drawing.Size(100, 19);
            this.boxMervynDaviesDown.TabIndex = 2;
            this.boxMervynDaviesDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.boxMervynDaviesDown.LostFocus += new System.EventHandler(this.boxMervynDavies_LostFocus);
            // 
            // lblJackDanielsName
            // 
            this.lblJackDanielsName.AutoSize = true;
            this.tablePanelGradeAdjustedPace.SetColumnSpan(this.lblJackDanielsName, 2);
            this.lblJackDanielsName.Location = new System.Drawing.Point(3, 60);
            this.lblJackDanielsName.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblJackDanielsName.Name = "lblJackDanielsName";
            this.lblJackDanielsName.Size = new System.Drawing.Size(74, 13);
            this.lblJackDanielsName.TabIndex = 1;
            this.lblJackDanielsName.Text = "<JackDaniels:";
            // 
            // lblJackDanielsUp
            // 
            this.lblJackDanielsUp.AutoSize = true;
            this.lblJackDanielsUp.Location = new System.Drawing.Point(3, 76);
            this.lblJackDanielsUp.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblJackDanielsUp.Name = "lblJackDanielsUp";
            this.lblJackDanielsUp.Size = new System.Drawing.Size(30, 13);
            this.lblJackDanielsUp.TabIndex = 1;
            this.lblJackDanielsUp.Text = "<Up:";
            // 
            // boxJackDanielsUp
            // 
            this.boxJackDanielsUp.AcceptsReturn = false;
            this.boxJackDanielsUp.AcceptsTab = false;
            this.boxJackDanielsUp.BackColor = System.Drawing.Color.White;
            this.boxJackDanielsUp.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.boxJackDanielsUp.ButtonImage = null;
            this.boxJackDanielsUp.Location = new System.Drawing.Point(149, 73);
            this.boxJackDanielsUp.Margin = new System.Windows.Forms.Padding(0);
            this.boxJackDanielsUp.MaxLength = 32767;
            this.boxJackDanielsUp.Multiline = false;
            this.boxJackDanielsUp.Name = "boxJackDanielsUp";
            this.boxJackDanielsUp.ReadOnly = false;
            this.boxJackDanielsUp.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.boxJackDanielsUp.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.boxJackDanielsUp.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.boxJackDanielsUp.Size = new System.Drawing.Size(100, 19);
            this.boxJackDanielsUp.TabIndex = 2;
            this.boxJackDanielsUp.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.boxJackDanielsUp.LostFocus += new System.EventHandler(this.boxJackDaniels_LostFocus);
            // 
            // lblJackDanielsDown
            // 
            this.lblJackDanielsDown.AutoSize = true;
            this.lblJackDanielsDown.Location = new System.Drawing.Point(3, 95);
            this.lblJackDanielsDown.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblJackDanielsDown.Name = "lblJackDanielsDown";
            this.lblJackDanielsDown.Size = new System.Drawing.Size(44, 13);
            this.lblJackDanielsDown.TabIndex = 1;
            this.lblJackDanielsDown.Text = "<Down:";
            // 
            // boxJackDanielsDown
            // 
            this.boxJackDanielsDown.AcceptsReturn = false;
            this.boxJackDanielsDown.AcceptsTab = false;
            this.boxJackDanielsDown.BackColor = System.Drawing.Color.White;
            this.boxJackDanielsDown.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.boxJackDanielsDown.ButtonImage = null;
            this.boxJackDanielsDown.Location = new System.Drawing.Point(149, 92);
            this.boxJackDanielsDown.Margin = new System.Windows.Forms.Padding(0);
            this.boxJackDanielsDown.MaxLength = 32767;
            this.boxJackDanielsDown.Multiline = false;
            this.boxJackDanielsDown.Name = "boxJackDanielsDown";
            this.boxJackDanielsDown.ReadOnly = false;
            this.boxJackDanielsDown.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.boxJackDanielsDown.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.boxJackDanielsDown.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.boxJackDanielsDown.Size = new System.Drawing.Size(100, 19);
            this.boxJackDanielsDown.TabIndex = 2;
            this.boxJackDanielsDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.boxJackDanielsDown.LostFocus += new System.EventHandler(this.boxJackDaniels_LostFocus);
            // 
            // SettingsPageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.PluginInfoPanel);
            this.MinimumSize = new System.Drawing.Size(350, 450);
            this.Name = "SettingsPageControl";
            this.Size = new System.Drawing.Size(362, 466);
            this.PluginInfoPanel.ResumeLayout(false);
            this.PluginInfoPanel.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.gradeAdjustedPaceGroup.ResumeLayout(false);
            this.gradeAdjustedPaceGroup.PerformLayout();
            this.tablePanelGradeAdjustedPace.ResumeLayout(false);
            this.tablePanelGradeAdjustedPace.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZoneFiveSoftware.Common.Visuals.Panel PluginInfoPanel;
        private ZoneFiveSoftware.Common.Visuals.ActionBanner PluginInfoBanner;
        private System.Windows.Forms.LinkLabel lblInfo;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblDefaultRadius;
        private ZoneFiveSoftware.Common.Visuals.TextBox txtDefaultRadius;
        private System.Windows.Forms.Label lblSetNameAtImport;
        private System.Windows.Forms.CheckBox txtSetNameAtImport;
        //private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label lblStoppedCategory;
        private ZoneFiveSoftware.Common.Visuals.TextBox boxStoppedCategory;
        private System.Windows.Forms.Label lblBarometricDevices;
        private ZoneFiveSoftware.Common.Visuals.TextBox boxBarometricDevices;
        private System.Windows.Forms.Label lblAdjustElevationAtImport;
        private System.Windows.Forms.CheckBox txtAdjustElevationAtImport;
        private System.Windows.Forms.Label lblUniqueRoutes;
        private System.Windows.Forms.Label lblHighScore;
        private System.Windows.Forms.Label lblPerformancePredictor;
        private System.Windows.Forms.Label lblPredictDistance;
        private ZoneFiveSoftware.Common.Visuals.TextBox boxPredictDistance;
        private System.Windows.Forms.Label lblLicense;
        private System.Windows.Forms.Label labelRouteTransparency;
        private System.Windows.Forms.NumericUpDown upDownRouteTransparency;
        private System.Windows.Forms.Label labelRouteTransparencyPercent;
        private System.Windows.Forms.Label labelMaxChartResults;
        private System.Windows.Forms.NumericUpDown upDownMaxChartResults;
        private System.Windows.Forms.GroupBox gradeAdjustedPaceGroup;
        private System.Windows.Forms.Label lblMervynDaviesName;
        private System.Windows.Forms.Label lblMervynDaviesUp;
        private ZoneFiveSoftware.Common.Visuals.TextBox boxMervynDaviesUp;
        private System.Windows.Forms.Label lblMervynDaviesDown;
        private ZoneFiveSoftware.Common.Visuals.TextBox boxMervynDaviesDown;
        private System.Windows.Forms.Label lblJackDanielsName;
        private System.Windows.Forms.Label lblJackDanielsUp;
        private ZoneFiveSoftware.Common.Visuals.TextBox boxJackDanielsUp;
        private System.Windows.Forms.Label lblJackDanielsDown;
        private ZoneFiveSoftware.Common.Visuals.TextBox boxJackDanielsDown;
        public System.Windows.Forms.TableLayoutPanel tablePanelGradeAdjustedPace;
    }
}
