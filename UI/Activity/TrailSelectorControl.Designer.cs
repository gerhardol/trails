﻿namespace TrailsPlugin.UI.Activity {
    partial class TrailSelectorControl {
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
            this.TrailSelectorPanel = new System.Windows.Forms.Panel();
            this.lblTrail = new System.Windows.Forms.Label();
            this.TrailName = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.btnAdd = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnEdit = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnDelete = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnMenu = new ZoneFiveSoftware.Common.Visuals.Button();
            this.chartPanelMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.runGradeAdjustMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectSimilarSplitsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useDeviceDistanceMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setRestLapsAsPausesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nonReqIsPauseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ShowPausesAsResultsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ResultSummaryStdDevMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSummaryTotalMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSummaryAverageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOnlyMarkedResultsOnMapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trailPointsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolBarMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolBarMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.TrailSelectorPanel.SuspendLayout();
            this.chartPanelMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // TrailSelectorPanel
            // 
            this.TrailSelectorPanel.AutoSize = true;
            this.TrailSelectorPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TrailSelectorPanel.Controls.Add(this.lblTrail);
            this.TrailSelectorPanel.Controls.Add(this.TrailName);
            this.TrailSelectorPanel.Controls.Add(this.btnAdd);
            this.TrailSelectorPanel.Controls.Add(this.btnEdit);
            this.TrailSelectorPanel.Controls.Add(this.btnDelete);
            this.TrailSelectorPanel.Controls.Add(this.btnMenu);
            this.TrailSelectorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TrailSelectorPanel.Location = new System.Drawing.Point(0, 0);
            this.TrailSelectorPanel.Margin = new System.Windows.Forms.Padding(0);
            this.TrailSelectorPanel.Name = "TrailSelectorPanel";
            this.TrailSelectorPanel.Size = new System.Drawing.Size(425, 22);
            this.TrailSelectorPanel.TabIndex = 8;
            // 
            // lblTrail
            // 
            this.lblTrail.AutoSize = true;
            this.lblTrail.Location = new System.Drawing.Point(0, 3);
            this.lblTrail.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lblTrail.Name = "lblTrail";
            this.lblTrail.Size = new System.Drawing.Size(30, 13);
            this.lblTrail.TabIndex = 0;
            this.lblTrail.Text = "Trail:";
            this.lblTrail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TrailName
            // 
            this.TrailName.AcceptsReturn = false;
            this.TrailName.AcceptsTab = false;
            this.TrailName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TrailName.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TrailName.BackColor = System.Drawing.Color.White;
            this.TrailName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.TrailName.ButtonImage = null;
            this.TrailName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrailName.Location = new System.Drawing.Point(92, 1);
            this.TrailName.Margin = new System.Windows.Forms.Padding(1);
            this.TrailName.MaxLength = 32767;
            this.TrailName.Multiline = false;
            this.TrailName.Name = "TrailName";
            this.TrailName.ReadOnly = true;
            this.TrailName.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.TrailName.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.TrailName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.TrailName.Size = new System.Drawing.Size(221, 19);
            this.TrailName.TabIndex = 15;
            this.TrailName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.TrailName.ButtonClick += new System.EventHandler(this.TrailName_ButtonClick);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.BackColor = System.Drawing.Color.Transparent;
            this.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnAdd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnAdd.CenterImage = null;
            this.btnAdd.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnAdd.HyperlinkStyle = false;
            //this.btnAdd.ImageMargin = 2;
            this.btnAdd.LeftImage = null;
            this.btnAdd.Location = new System.Drawing.Point(328, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Padding = new System.Windows.Forms.Padding(2);
            this.btnAdd.PushStyle = true;
            this.btnAdd.RightImage = null;
            this.btnAdd.Size = new System.Drawing.Size(22, 18);
            this.btnAdd.TabIndex = 10;
            this.btnAdd.Text = "A";
            this.btnAdd.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnAdd.TextLeftMargin = 2;
            this.btnAdd.TextRightMargin = 2;
            this.btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.BackColor = System.Drawing.Color.Transparent;
            this.btnEdit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnEdit.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnEdit.CenterImage = null;
            this.btnEdit.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnEdit.HyperlinkStyle = false;
            //this.btnEdit.ImageMargin = 2;
            this.btnEdit.LeftImage = null;
            this.btnEdit.Location = new System.Drawing.Point(353, 3);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Padding = new System.Windows.Forms.Padding(2);
            this.btnEdit.PushStyle = true;
            this.btnEdit.RightImage = null;
            this.btnEdit.Size = new System.Drawing.Size(22, 18);
            this.btnEdit.TabIndex = 17;
            this.btnEdit.Text = "E";
            this.btnEdit.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnEdit.TextLeftMargin = 2;
            this.btnEdit.TextRightMargin = 2;
            this.btnEdit.Click += new System.EventHandler(this.BtnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.BackColor = System.Drawing.Color.Transparent;
            this.btnDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnDelete.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnDelete.CenterImage = null;
            this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnDelete.HyperlinkStyle = false;
            //this.btnDelete.ImageMargin = 2;
            this.btnDelete.LeftImage = null;
            this.btnDelete.Location = new System.Drawing.Point(378, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Padding = new System.Windows.Forms.Padding(2);
            this.btnDelete.PushStyle = true;
            this.btnDelete.RightImage = null;
            this.btnDelete.Size = new System.Drawing.Size(22, 18);
            this.btnDelete.TabIndex = 13;
            this.btnDelete.Text = "D";
            this.btnDelete.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnDelete.TextLeftMargin = 2;
            this.btnDelete.TextRightMargin = 2;
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // btnMenu
            // 
            this.btnMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMenu.BackColor = System.Drawing.Color.Transparent;
            this.btnMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnMenu.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnMenu.CenterImage = null;
            this.btnMenu.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnMenu.HyperlinkStyle = false;
            this.btnDelete.LeftImage = null;
            this.btnMenu.Location = new System.Drawing.Point(403, 3);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Padding = new System.Windows.Forms.Padding(2);
            this.btnMenu.PushStyle = true;
            this.btnMenu.RightImage = null;
            this.btnMenu.Size = new System.Drawing.Size(22, 18);
            this.btnMenu.TabIndex = 13;
            this.btnMenu.Text = "M";
            this.btnMenu.Click += new System.EventHandler(this.BtnMenu_Click);
            // 
            // chartPanelMenu
            // 
            this.chartPanelMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showSummaryTotalMenuItem,
            this.showSummaryAverageMenuItem,
            this.ResultSummaryStdDevMenuItem,
            this.useDeviceDistanceMenuItem,
            this.setRestLapsAsPausesMenuItem,
            this.nonReqIsPauseMenuItem,
            this.ShowPausesAsResultsMenuItem,
            this.showOnlyMarkedResultsOnMapMenuItem,
            this.trailPointsMenuItem,
            this.selectSimilarSplitsMenuItem,
            this.runGradeAdjustMenuItem,
            this.showToolBarMenuItem,
            this.settingsToolBarMenuItem});
            this.chartPanelMenu.Name = "chartPanelMenu";
            this.chartPanelMenu.Size = new System.Drawing.Size(199, 48);
            this.chartPanelMenu.Opening += new System.ComponentModel.CancelEventHandler(ChartPanelMenu_Opening);
            // 
            // selectSimilarSplitsMenuItem
            // 
            this.selectSimilarSplitsMenuItem.Name = "selectSimilarSplitsMenuItem";
            this.selectSimilarSplitsMenuItem.Size = new System.Drawing.Size(206, 22);
            this.selectSimilarSplitsMenuItem.Text = "<Select similar splits...";
            this.selectSimilarSplitsMenuItem.Click += new System.EventHandler(this.SelectSimilarSplitsMenuItem_Click);
            // 
            // useDeviceDistanceMenuItem
            // 
            this.useDeviceDistanceMenuItem.Name = "useDeviceDistanceMenuItem";
            this.useDeviceDistanceMenuItem.Size = new System.Drawing.Size(198, 22);
            this.useDeviceDistanceMenuItem.Text = "<useDeviceDistanceMenuItem...";
            this.useDeviceDistanceMenuItem.Click += new System.EventHandler(this.UseDeviceDistanceMenuItem_Click);
            // 
            // setRestLapsAsPausesMenuItem
            // 
            this.setRestLapsAsPausesMenuItem.Name = "setRestLapsAsPausesMenuItem";
            this.setRestLapsAsPausesMenuItem.Size = new System.Drawing.Size(198, 22);
            this.setRestLapsAsPausesMenuItem.Text = "<setRestLapsAsPausesMenuItem...";
            this.setRestLapsAsPausesMenuItem.Click += new System.EventHandler(this.SetRestLapsAsPausesMenuItem_Click);
            // 
            // nonReqIsPauseMenuItem
            // 
            this.nonReqIsPauseMenuItem.Name = "nonReqIsPauseMenuItem";
            this.nonReqIsPauseMenuItem.Size = new System.Drawing.Size(198, 22);
            this.nonReqIsPauseMenuItem.Text = "<nonReqIsPauseMenuItem...";
            this.nonReqIsPauseMenuItem.Click += new System.EventHandler(this.NonReqIsPauseMenuItem_Click);
            // 
            // ShowPausesAsResultsMenuItem
            // 
            this.ShowPausesAsResultsMenuItem.Name = "ShowPausesAsResultsMenuItem";
            this.ShowPausesAsResultsMenuItem.Size = new System.Drawing.Size(198, 22);
            this.ShowPausesAsResultsMenuItem.Text = "<ShowPausesAsResultsMenuItem...";
            this.ShowPausesAsResultsMenuItem.Click += new System.EventHandler(this.ShowPausesAsResultsMenuItem_Click);
            // 
            // ResultSummaryStdDevMenuItem
            // 
            this.ResultSummaryStdDevMenuItem.Name = "ResultSummaryStdDevMenuItem";
            this.ResultSummaryStdDevMenuItem.Size = new System.Drawing.Size(198, 22);
            this.ResultSummaryStdDevMenuItem.Text = "<ResultSummaryStdDevMenuItem...";
            this.ResultSummaryStdDevMenuItem.Click += new System.EventHandler(this.ResultSummaryStdDevMenuItem_Click);
            // 
            // showSummaryTotalMenuItem
            // 
            this.showSummaryTotalMenuItem.Name = "showSummaryTotalMenuItem";
            this.showSummaryTotalMenuItem.Size = new System.Drawing.Size(198, 22);
            this.showSummaryTotalMenuItem.Text = "<showSummaryTotalMenuItem...";
            this.showSummaryTotalMenuItem.Click += new System.EventHandler(this.ShowSummaryTotalMenuItem_Click);
            // 
            // showSummaryAverageMenuItem
            // 
            this.showSummaryAverageMenuItem.Name = "showSummaryAverageMenuItem";
            this.showSummaryAverageMenuItem.Size = new System.Drawing.Size(198, 22);
            this.showSummaryAverageMenuItem.Text = "<showSummaryAverageMenuItem...";
            this.showSummaryAverageMenuItem.Click += new System.EventHandler(this.ShowSummaryAverageMenuItem_Click);
            // 
            // showOnlyMarkedResultsOnMapMenuItem
            // 
            this.showOnlyMarkedResultsOnMapMenuItem.Name = "showOnlyMarkedResultsOnMapMenuItem";
            this.showOnlyMarkedResultsOnMapMenuItem.Size = new System.Drawing.Size(198, 22);
            this.showOnlyMarkedResultsOnMapMenuItem.Text = "<showOnlyMarkedResultsOnMapMenuItem...";
            this.showOnlyMarkedResultsOnMapMenuItem.Click += new System.EventHandler(this.ShowOnlyMarkedResultsOnMapMenuItem_Click);
            // 
            // trailPointsMenuItem
            // 
            this.trailPointsMenuItem.Name = "trailPointsMenuItem";
            this.trailPointsMenuItem.Size = new System.Drawing.Size(198, 22);
            this.trailPointsMenuItem.Text = "<trailPointsMenuItem...";
            this.trailPointsMenuItem.Click += new System.EventHandler(this.TrailPointsMenuItem_Click);
            // 
            // runGradeAdjustMenuItem
            // 
            this.runGradeAdjustMenuItem.Name = "runGradeAdjustMenuItem";
            this.runGradeAdjustMenuItem.Size = new System.Drawing.Size(198, 22);
            this.runGradeAdjustMenuItem.Text = "<runGradeAdjustMenuItem";
            this.runGradeAdjustMenuItem.Click += new System.EventHandler(this.RunGradeAdjustMenuItem_Click);
            // 
            // showToolBarMenuItem
            // 
            this.showToolBarMenuItem.Name = "showToolBarMenuItem";
            this.showToolBarMenuItem.Size = new System.Drawing.Size(198, 22);
            this.showToolBarMenuItem.Text = "showToolBarMenuItem";
            this.showToolBarMenuItem.Click += new System.EventHandler(this.ShowToolBarMenuItem_Click);
            // 
            // settingsToolBarMenuItem
            // 
            this.settingsToolBarMenuItem.Name = "settingsToolBarMenuItem";
            this.settingsToolBarMenuItem.Size = new System.Drawing.Size(198, 22);
            this.settingsToolBarMenuItem.Text = "settingsToolBarMenuItem";
            this.settingsToolBarMenuItem.Click += new System.EventHandler(this.SettingsToolBarMenuItem_Click);
            // 
            // TrailSelectorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.TrailSelectorPanel);
            this.Name = "TrailSelectorControl";
            this.Size = new System.Drawing.Size(425, 22);
            this.TrailSelectorPanel.ResumeLayout(false);
            this.TrailSelectorPanel.PerformLayout();
            this.chartPanelMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTrail;
        private ZoneFiveSoftware.Common.Visuals.Button btnDelete;
        private ZoneFiveSoftware.Common.Visuals.Button btnAdd;
        private ZoneFiveSoftware.Common.Visuals.TextBox TrailName;
        private ZoneFiveSoftware.Common.Visuals.Button btnEdit;
        private ZoneFiveSoftware.Common.Visuals.Button btnMenu;
        private System.Windows.Forms.ContextMenuStrip chartPanelMenu;
        private System.Windows.Forms.ToolStripMenuItem runGradeAdjustMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setRestLapsAsPausesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nonReqIsPauseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ResultSummaryStdDevMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSummaryTotalMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSummaryAverageMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useDeviceDistanceMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectSimilarSplitsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ShowPausesAsResultsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOnlyMarkedResultsOnMapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trailPointsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showToolBarMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolBarMenuItem;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Panel TrailSelectorPanel;
    }
}
