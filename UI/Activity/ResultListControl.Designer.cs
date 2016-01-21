namespace TrailsPlugin.UI.Activity {
    partial class ResultListControl {
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
            this.chartTablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.ButtonPanel = new ZoneFiveSoftware.Common.Visuals.Panel();
            this.HelpTutorialBtn = new ZoneFiveSoftware.Common.Visuals.Button();
            this.HelpFeaturesBtn = new ZoneFiveSoftware.Common.Visuals.Button();
            this.listSettingsBtn = new ZoneFiveSoftware.Common.Visuals.Button();
            this.insertActivitiesBtn = new ZoneFiveSoftware.Common.Visuals.Button();
            this.SummaryPanel = new System.Windows.Forms.Panel();
            this.summaryList = new ZoneFiveSoftware.Common.Visuals.TreeList();
            this.listMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyTableMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listSettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.referenceResultMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertActivitiesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectWithURMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addCurrentCategoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addTopCategoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analyzeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.highScoreMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.performancePredictorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addInBoundActivitiesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excludeResultsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.limitActivityMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.limitURMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.markCommonStretchesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.chartTablePanel.SuspendLayout();
            this.ButtonPanel.SuspendLayout();
            this.SummaryPanel.SuspendLayout();
            this.listMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // chartTablePanel
            // 
            this.chartTablePanel.AutoSize = true;
            this.chartTablePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.chartTablePanel.ColumnCount = 1;
            this.chartTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.chartTablePanel.Controls.Add(this.ButtonPanel, 0, 0);
            this.chartTablePanel.Controls.Add(this.SummaryPanel, 0, 1);
            this.chartTablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartTablePanel.Location = new System.Drawing.Point(0, 0);
            this.chartTablePanel.Margin = new System.Windows.Forms.Padding(0);
            this.chartTablePanel.Name = "chartTablePanel";
            this.chartTablePanel.RowCount = 2;
            this.chartTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.chartTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.chartTablePanel.Size = new System.Drawing.Size(400, 60);
            this.chartTablePanel.TabIndex = 0;
            this.chartTablePanel.SizeChanged += ChartTablePanel_SizeChanged;
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.AutoSize = true;
            this.ButtonPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonPanel.BackColor = System.Drawing.Color.Transparent;
            this.ButtonPanel.Border = ZoneFiveSoftware.Common.Visuals.ControlBorder.Style.None;
            this.ButtonPanel.BorderColor = System.Drawing.Color.Gray;
            this.ButtonPanel.Controls.Add(this.HelpTutorialBtn);
            this.ButtonPanel.Controls.Add(this.HelpFeaturesBtn);
            this.ButtonPanel.Controls.Add(this.listSettingsBtn);
            this.ButtonPanel.Controls.Add(this.insertActivitiesBtn);
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
            // HelpTutorialBtn
            // 
            this.HelpTutorialBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.HelpTutorialBtn.BackColor = System.Drawing.Color.Transparent;
            this.HelpTutorialBtn.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.HelpTutorialBtn.CenterImage = null;
            this.HelpTutorialBtn.DialogResult = System.Windows.Forms.DialogResult.None;
            this.HelpTutorialBtn.HyperlinkStyle = false;
            this.HelpTutorialBtn.ImageMargin = 2;
            this.HelpTutorialBtn.LeftImage = null;
            this.HelpTutorialBtn.Location = new System.Drawing.Point(374, 0);
            this.HelpTutorialBtn.Margin = new System.Windows.Forms.Padding(0);
            this.HelpTutorialBtn.Name = "HelpTutorialBtn";
            this.HelpTutorialBtn.PushStyle = true;
            this.HelpTutorialBtn.RightImage = null;
            this.HelpTutorialBtn.Size = new System.Drawing.Size(22, 22);
            this.HelpTutorialBtn.TabIndex = 0;
            this.HelpTutorialBtn.TextAlign = System.Drawing.StringAlignment.Center;
            this.HelpTutorialBtn.TextLeftMargin = 2;
            this.HelpTutorialBtn.TextRightMargin = 2;
            this.HelpTutorialBtn.Click += new System.EventHandler(this.HelpTutorialBtn_Click);
            // 
            // HelpFeaturesBtn
            // 
            this.HelpFeaturesBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.HelpFeaturesBtn.BackColor = System.Drawing.Color.Transparent;
            this.HelpFeaturesBtn.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.HelpFeaturesBtn.CenterImage = null;
            this.HelpFeaturesBtn.DialogResult = System.Windows.Forms.DialogResult.None;
            this.HelpFeaturesBtn.HyperlinkStyle = false;
            this.HelpFeaturesBtn.ImageMargin = 2;
            this.HelpFeaturesBtn.LeftImage = null;
            this.HelpFeaturesBtn.Location = new System.Drawing.Point(349, 0);
            this.HelpFeaturesBtn.Margin = new System.Windows.Forms.Padding(0);
            this.HelpFeaturesBtn.Name = "HelpFeaturesBtn";
            this.HelpFeaturesBtn.PushStyle = true;
            this.HelpFeaturesBtn.RightImage = null;
            this.HelpFeaturesBtn.Size = new System.Drawing.Size(22, 22);
            this.HelpFeaturesBtn.TabIndex = 0;
            this.HelpFeaturesBtn.TextAlign = System.Drawing.StringAlignment.Center;
            this.HelpFeaturesBtn.TextLeftMargin = 2;
            this.HelpFeaturesBtn.TextRightMargin = 2;
            this.HelpFeaturesBtn.Click += new System.EventHandler(this.HelpFeaturesBtn_Click);
            // 
            // listSettingsBtn
            // 
            this.listSettingsBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.listSettingsBtn.BackColor = System.Drawing.Color.Transparent;
            this.listSettingsBtn.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.listSettingsBtn.CenterImage = null;
            this.listSettingsBtn.DialogResult = System.Windows.Forms.DialogResult.None;
            this.listSettingsBtn.HyperlinkStyle = false;
            this.listSettingsBtn.ImageMargin = 2;
            this.listSettingsBtn.LeftImage = null;
            this.listSettingsBtn.Location = new System.Drawing.Point(324, 0);
            this.listSettingsBtn.Margin = new System.Windows.Forms.Padding(0);
            this.listSettingsBtn.Name = "listSettingsBtn";
            this.listSettingsBtn.PushStyle = true;
            this.listSettingsBtn.RightImage = null;
            this.listSettingsBtn.Size = new System.Drawing.Size(22, 22);
            this.listSettingsBtn.TabIndex = 0;
            this.listSettingsBtn.TextAlign = System.Drawing.StringAlignment.Center;
            this.listSettingsBtn.TextLeftMargin = 2;
            this.listSettingsBtn.TextRightMargin = 2;
            this.listSettingsBtn.Visible = false;
            this.listSettingsBtn.Click += new System.EventHandler(this.listSettingsToolStripMenuItem_Click);
            // 
            // insertActivitiesBtn
            // 
            this.insertActivitiesBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.insertActivitiesBtn.BackColor = System.Drawing.Color.Transparent;
            this.insertActivitiesBtn.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.insertActivitiesBtn.CenterImage = null;
            this.insertActivitiesBtn.DialogResult = System.Windows.Forms.DialogResult.None;
            this.insertActivitiesBtn.HyperlinkStyle = false;
            this.insertActivitiesBtn.ImageMargin = 2;
            this.insertActivitiesBtn.LeftImage = null;
            this.insertActivitiesBtn.Location = new System.Drawing.Point(324, 0);
            this.insertActivitiesBtn.Margin = new System.Windows.Forms.Padding(0);
            this.insertActivitiesBtn.Name = "insertActivitiesBtn";
            this.insertActivitiesBtn.PushStyle = true;
            this.insertActivitiesBtn.RightImage = null;
            this.insertActivitiesBtn.Size = new System.Drawing.Size(22, 22);
            this.insertActivitiesBtn.TabIndex = 0;
            this.insertActivitiesBtn.TextAlign = System.Drawing.StringAlignment.Center;
            this.insertActivitiesBtn.TextLeftMargin = 2;
            this.insertActivitiesBtn.TextRightMargin = 2;
            this.insertActivitiesBtn.Click += new System.EventHandler(this.insertActivitiesBtn_Click);
            // 
            // SummaryPanel
            // 
            this.SummaryPanel.AutoSize = true;
            this.SummaryPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SummaryPanel.Controls.Add(this.summaryList);
            this.SummaryPanel.Controls.Add(this.progressBar);
            this.SummaryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SummaryPanel.Location = new System.Drawing.Point(0, 25);
            this.SummaryPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SummaryPanel.Name = "SummaryPanel";
            this.SummaryPanel.Size = new System.Drawing.Size(400, 35);
            this.SummaryPanel.TabIndex = 1;
            // 
            // summaryList
            // 
            this.summaryList.AutoScroll = true;
            this.summaryList.AutoSize = true;
            this.summaryList.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.summaryList.BackColor = System.Drawing.Color.Transparent;
            this.summaryList.Border = ZoneFiveSoftware.Common.Visuals.ControlBorder.Style.SmallRoundShadow;
            this.summaryList.CheckBoxes = false;
            this.summaryList.ContextMenuStrip = this.listMenu;
            this.summaryList.DefaultIndent = 15;
            this.summaryList.DefaultRowHeight = -1;
            this.summaryList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.summaryList.HeaderRowHeight = 21;
            this.summaryList.Location = new System.Drawing.Point(0, 0);
            this.summaryList.Margin = new System.Windows.Forms.Padding(0);
            this.summaryList.MultiSelect = true;
            this.summaryList.Name = "summaryList";
            this.summaryList.NumHeaderRows = ZoneFiveSoftware.Common.Visuals.TreeList.HeaderRows.One;
            this.summaryList.NumLockedColumns = 0;
            this.summaryList.RowAlternatingColors = true;
            this.summaryList.RowHotlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.summaryList.RowHotlightColorText = System.Drawing.SystemColors.HighlightText;
            this.summaryList.RowHotlightMouse = true;
            this.summaryList.RowSelectedColor = System.Drawing.SystemColors.Highlight;
            this.summaryList.RowSelectedColorText = System.Drawing.SystemColors.HighlightText;
            this.summaryList.RowSeparatorLines = true;
            this.summaryList.ShowLines = false;
            this.summaryList.ShowPlusMinus = true;
            this.summaryList.Size = new System.Drawing.Size(400, 35);
            this.summaryList.TabIndex = 11;
            this.summaryList.ColumnResized += new ZoneFiveSoftware.Common.Visuals.TreeList.ColumnEventHandler(this.SummaryList_ColumnResized);
            this.summaryList.Click += new System.EventHandler(this.summaryList_Click);
            this.summaryList.DoubleClick += new System.EventHandler(this.summaryList_DoubleClick);
            this.summaryList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.summaryList_KeyDown);
            this.summaryList.MouseLeave += new System.EventHandler(this.summaryList_MouseLeave);
            this.summaryList.MouseMove += new System.Windows.Forms.MouseEventHandler(this.summaryList_MouseMove);
            // 
            // listMenu
            // 
            this.listMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyTableMenuItem,
            this.listSettingsMenuItem,
            this.referenceResultMenuItem,
            this.insertActivitiesMenuItem,
            this.analyzeMenuItem,
            this.advancedMenuItem});
            this.listMenu.Name = "listContextMenuStrip";
            this.listMenu.Size = new System.Drawing.Size(207, 136);
            this.listMenu.Opening += new System.ComponentModel.CancelEventHandler(this.listMenu_Opening);
            // 
            // copyTableMenuItem
            // 
            this.copyTableMenuItem.Name = "copyTableMenuItem";
            this.copyTableMenuItem.Size = new System.Drawing.Size(206, 22);
            this.copyTableMenuItem.Text = "<Copy table to clipboard";
            this.copyTableMenuItem.Click += new System.EventHandler(this.copyTableMenu_Click);
            // 
            // listSettingsMenuItem
            // 
            this.listSettingsMenuItem.Name = "listSettingsMenuItem";
            this.listSettingsMenuItem.Size = new System.Drawing.Size(206, 22);
            this.listSettingsMenuItem.Text = "List Settings...";
            this.listSettingsMenuItem.Click += new System.EventHandler(this.listSettingsToolStripMenuItem_Click);
            // 
            // referenceResultMenuItem
            // 
            this.referenceResultMenuItem.Name = "referenceResultMenuItem";
            this.referenceResultMenuItem.Size = new System.Drawing.Size(206, 22);
            this.referenceResultMenuItem.Text = "<Set reference trail...";
            this.referenceResultMenuItem.Click += new System.EventHandler(this.referenceResultMenuItem_Click);
            // 
            // insertActivitiesMenuItem
            // 
            this.insertActivitiesMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectWithURMenuItem,
            this.addCurrentCategoryMenuItem,
            this.addTopCategoryMenuItem});
            this.insertActivitiesMenuItem.Name = "insertActivitiesMenuItem";
            this.insertActivitiesMenuItem.Size = new System.Drawing.Size(206, 22);
            this.insertActivitiesMenuItem.Text = "<Insert>";
            // 
            // selectWithURMenuItem
            // 
            this.selectWithURMenuItem.Name = "selectWithURMenuItem";
            this.selectWithURMenuItem.Size = new System.Drawing.Size(270, 22);
            this.selectWithURMenuItem.Text = "<Select with UR to current activities...";
            this.selectWithURMenuItem.Click += new System.EventHandler(this.selectWithURMenuItem_Click);
            // 
            // addCurrentCategoryMenuItem
            // 
            this.addCurrentCategoryMenuItem.Name = "addCurrentCategoryMenuItem";
            this.addCurrentCategoryMenuItem.Size = new System.Drawing.Size(270, 22);
            this.addCurrentCategoryMenuItem.Text = "<addCurrentCategoryMenuItem...";
            this.addCurrentCategoryMenuItem.Click += new System.EventHandler(this.addCurrentCategoryMenuItem_Click);
            // 
            // addTopCategoryMenuItem
            // 
            this.addTopCategoryMenuItem.Name = "addTopCategoryMenuItem";
            this.addTopCategoryMenuItem.Size = new System.Drawing.Size(270, 22);
            this.addTopCategoryMenuItem.Text = "<addTopCategoryMenuItem...";
            this.addTopCategoryMenuItem.Click += new System.EventHandler(this.addTopCategoryMenuItem_Click);
            // 
            // analyzeMenuItem
            // 
            this.analyzeMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.highScoreMenuItem,
            this.performancePredictorMenuItem});
            this.analyzeMenuItem.Name = "analyzeMenuItem";
            this.analyzeMenuItem.Size = new System.Drawing.Size(206, 22);
            this.analyzeMenuItem.Text = "<Analyze";
            this.analyzeMenuItem.DropDownOpened += new System.EventHandler(this.analyzeMenuItem_DropDownOpened);
            // 
            // highScoreMenuItem
            // 
            this.highScoreMenuItem.Name = "highScoreMenuItem";
            this.highScoreMenuItem.Size = new System.Drawing.Size(207, 22);
            this.highScoreMenuItem.Text = "<highScore...";
            this.highScoreMenuItem.Click += new System.EventHandler(this.highScoreMenuItem_Click);
            // 
            // performancePredictorMenuItem
            // 
            this.performancePredictorMenuItem.Name = "performancePredictorMenuItem";
            this.performancePredictorMenuItem.Size = new System.Drawing.Size(207, 22);
            this.performancePredictorMenuItem.Text = "<PerformancePredictor...";
            this.performancePredictorMenuItem.Click += new System.EventHandler(this.performancePredictorMenuItem_Click);
            // 
            // advancedMenuItem
            // 
            this.advancedMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addInBoundActivitiesMenuItem,
            this.excludeResultsMenuItem,
            this.limitActivityMenuItem,
            this.limitURMenuItem,
            this.markCommonStretchesMenuItem});
            this.advancedMenuItem.Name = "advancedMenuItem";
            this.advancedMenuItem.Size = new System.Drawing.Size(206, 22);
            this.advancedMenuItem.Text = "<Advanced>";
            // 
            // addInBoundActivitiesMenuItem
            // 
            this.addInBoundActivitiesMenuItem.Name = "addInBoundActivitiesMenuItem";
            this.addInBoundActivitiesMenuItem.Size = new System.Drawing.Size(316, 22);
            this.addInBoundActivitiesMenuItem.Text = "<addInBoundActivities...";
            this.addInBoundActivitiesMenuItem.Click += new System.EventHandler(this.addInBoundActivitiesMenuItem_Click);
            // 
            // excludeResultsMenuItem
            // 
            this.excludeResultsMenuItem.Name = "excludeResultsMenuItem";
            this.excludeResultsMenuItem.Size = new System.Drawing.Size(316, 22);
            this.excludeResultsMenuItem.Text = "<Exclude results from list...";
            this.excludeResultsMenuItem.Click += new System.EventHandler(this.excludeResultsMenuItem_Click);
            // 
            // limitActivityMenuItem
            // 
            this.limitActivityMenuItem.Name = "limitActivityMenuItem";
            this.limitActivityMenuItem.Size = new System.Drawing.Size(316, 22);
            this.limitActivityMenuItem.Text = "<Limit selection to current activities...";
            this.limitActivityMenuItem.Click += new System.EventHandler(this.limitActivityMenuItem_Click);
            // 
            // limitURMenuItem
            // 
            this.limitURMenuItem.Name = "limitURMenuItem";
            this.limitURMenuItem.Size = new System.Drawing.Size(316, 22);
            this.limitURMenuItem.Text = "<Limit selection with UR to current activities...";
            this.limitURMenuItem.Click += new System.EventHandler(this.limitURMenuItem_Click);
            // 
            // markCommonStretchesMenuItem
            // 
            this.markCommonStretchesMenuItem.Name = "markCommonStretchesMenuItem";
            this.markCommonStretchesMenuItem.Size = new System.Drawing.Size(316, 22);
            this.markCommonStretchesMenuItem.Text = "<Mark common stretches...";
            this.markCommonStretchesMenuItem.Click += new System.EventHandler(this.markCommonStretchesMenuItem_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(0, 0);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(391, 24);
            this.progressBar.TabIndex = 2;
            this.progressBar.Visible = false;
            // 
            // ResultListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.chartTablePanel);
            this.Name = "ResultListControl";
            this.Size = new System.Drawing.Size(400, 60);
            this.chartTablePanel.ResumeLayout(false);
            this.chartTablePanel.PerformLayout();
            this.ButtonPanel.ResumeLayout(false);
            this.SummaryPanel.ResumeLayout(false);
            this.SummaryPanel.PerformLayout();
            this.listMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel chartTablePanel;
        private ZoneFiveSoftware.Common.Visuals.Panel ButtonPanel;
        private ZoneFiveSoftware.Common.Visuals.Button HelpTutorialBtn;
        private ZoneFiveSoftware.Common.Visuals.Button HelpFeaturesBtn;
        private ZoneFiveSoftware.Common.Visuals.Button listSettingsBtn;
        private ZoneFiveSoftware.Common.Visuals.Button insertActivitiesBtn;
        private System.Windows.Forms.Panel SummaryPanel;
        private ZoneFiveSoftware.Common.Visuals.TreeList summaryList;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ContextMenuStrip listMenu;
        private System.Windows.Forms.ToolStripMenuItem copyTableMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listSettingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem referenceResultMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertActivitiesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem analyzeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem highScoreMenuItem;
        private System.Windows.Forms.ToolStripMenuItem performancePredictorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem advancedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem excludeResultsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem limitActivityMenuItem;
        private System.Windows.Forms.ToolStripMenuItem limitURMenuItem;
        private System.Windows.Forms.ToolStripMenuItem markCommonStretchesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectWithURMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addInBoundActivitiesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addCurrentCategoryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addTopCategoryMenuItem;
        private System.Windows.Forms.ToolTip summaryListToolTip = new System.Windows.Forms.ToolTip();
        private System.Windows.Forms.Timer summaryListToolTipTimer = new System.Windows.Forms.Timer();
    }
}
