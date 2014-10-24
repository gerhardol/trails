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
            this.SummaryPanel = new System.Windows.Forms.Panel();
            this.summaryList = new ZoneFiveSoftware.Common.Visuals.TreeList();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.listMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyTableMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listSettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectSimilarSplitsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.referenceResultMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analyzeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.highScoreMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.performancePredictorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excludeResultsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.limitActivityMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.limitURMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectWithURMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.markCommonStretchesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addInBoundActivitiesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addCurrentCategoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runGradeAdjustMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SummaryPanel.SuspendLayout();
            this.listMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // SummaryPanel
            // 
            this.SummaryPanel.AutoSize = true;
            this.SummaryPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SummaryPanel.Controls.Add(this.summaryList);
            this.SummaryPanel.Controls.Add(this.progressBar);
            this.SummaryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SummaryPanel.Location = new System.Drawing.Point(0, 0);
            this.SummaryPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SummaryPanel.Name = "SummaryPanel";
            this.SummaryPanel.Size = new System.Drawing.Size(400, 80);
            this.SummaryPanel.TabIndex = 1;
            this.SummaryPanel.SizeChanged += new System.EventHandler(SummaryPanel_SizeChanged);
            //this.SummaryPanel.HandleCreated += new System.EventHandler(SummaryPanel_HandleCreated);
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
            this.summaryList.Size = new System.Drawing.Size(400, 60);
            this.summaryList.TabIndex = 11;
            this.summaryList.Click += new System.EventHandler(this.summaryList_Click);
            this.summaryList.DoubleClick += new System.EventHandler(this.summaryList_DoubleClick);
            this.summaryList.MouseLeave += new System.EventHandler(this.summaryList_MouseLeave);
            //this.summaryList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.summaryList_MouseDoubleClick);
            this.summaryList.MouseMove +=new System.Windows.Forms.MouseEventHandler(summaryList_MouseMove);
            this.summaryList.KeyDown +=new System.Windows.Forms.KeyEventHandler(summaryList_KeyDown);
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
            // listMenu
            // 
            this.listMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyTableMenuItem,
            this.listSettingsMenuItem,
            this.selectSimilarSplitsMenuItem,
            this.referenceResultMenuItem,
            this.analyzeMenuItem,
            this.advancedMenuItem});
            this.listMenu.Name = "listContextMenuStrip";
            this.listMenu.Size = new System.Drawing.Size(199, 48);
            this.listMenu.Opening += new System.ComponentModel.CancelEventHandler(listMenu_Opening);
            // 
            // copyTableMenuItem
            // 
            this.copyTableMenuItem.Name = "copyTableMenuItem";
            this.copyTableMenuItem.Size = new System.Drawing.Size(198, 22);
            this.copyTableMenuItem.Text = "<Copy table to clipboard";
            this.copyTableMenuItem.Click += new System.EventHandler(this.copyTableMenu_Click);
            // 
            // listSettingsMenuItem
            // 
            this.listSettingsMenuItem.Name = "listSettingsMenuItem";
            this.listSettingsMenuItem.Size = new System.Drawing.Size(198, 22);
            this.listSettingsMenuItem.Text = "List Settings...";
            this.listSettingsMenuItem.Click += new System.EventHandler(this.listSettingsToolStripMenuItem_Click);
            // 
            // selectSimilarSplitsMenuItem
            // 
            this.selectSimilarSplitsMenuItem.Name = "selectSimilarSplitsMenuItem";
            this.selectSimilarSplitsMenuItem.Size = new System.Drawing.Size(198, 22);
            this.selectSimilarSplitsMenuItem.Text = "<Select similar splits...";
            this.selectSimilarSplitsMenuItem.Click +=new System.EventHandler(selectSimilarSplitsMenuItem_Click);
            // 
            // referenceResultMenuItem
            // 
            this.referenceResultMenuItem.Name = "referenceResultMenuItem";
            this.referenceResultMenuItem.Size = new System.Drawing.Size(198, 22);
            this.referenceResultMenuItem.Text = "<Set reference trail...";
            this.referenceResultMenuItem.Click += new System.EventHandler(referenceResultMenuItem_Click);
            // 
            // analyzeMenuItem
            // 
            this.analyzeMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.highScoreMenuItem,
            this.performancePredictorMenuItem});
            this.analyzeMenuItem.Name = "sendToMenuItem";
            this.analyzeMenuItem.Size = new System.Drawing.Size(198, 22);
            this.analyzeMenuItem.Text = "<Analyze";
            this.analyzeMenuItem.DropDownOpened += new System.EventHandler(analyzeMenuItem_DropDownOpened);
            // 
            // highScoreMenuItem
            // 
            this.highScoreMenuItem.Name = "highScoreMenuItem";
            this.highScoreMenuItem.Size = new System.Drawing.Size(198, 22);
            this.highScoreMenuItem.Text = "<highScore...";
            this.highScoreMenuItem.Click += new System.EventHandler(this.highScoreMenuItem_Click);
            // 
            // performancePredictorMenuItem
            // 
            this.performancePredictorMenuItem.Name = "performancePredictorMenuItem";
            this.performancePredictorMenuItem.Size = new System.Drawing.Size(198, 22);
            this.performancePredictorMenuItem.Text = "<PerformancePredictor...";
            this.performancePredictorMenuItem.Click += new System.EventHandler(this.performancePredictorMenuItem_Click);
            // 
            // advancedMenuItem
            // 
            this.advancedMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.excludeResultsMenuItem,
            this.limitActivityMenuItem,
            this.limitURMenuItem,
            this.selectWithURMenuItem,
            this.markCommonStretchesMenuItem,
            this.addInBoundActivitiesMenuItem,
            this.addCurrentCategoryMenuItem,
            this.runGradeAdjustMenuItem});
            this.advancedMenuItem.Name = "advancedMenuItem";
            this.advancedMenuItem.Size = new System.Drawing.Size(199, 48);
            this.advancedMenuItem.Text = "<Advanced>";
            // 
            // excludeResultsMenuItem
            // 
            this.excludeResultsMenuItem.Name = "excludeResultsMenuItem";
            this.excludeResultsMenuItem.Size = new System.Drawing.Size(198, 22);
            this.excludeResultsMenuItem.Text = "<Exclude results from list...";
            this.excludeResultsMenuItem.Click += new System.EventHandler(this.excludeResultsMenuItem_Click);
            // 
            // limitActivityMenuItem
            // 
            this.limitActivityMenuItem.Name = "limitActivityMenuItem";
            this.limitActivityMenuItem.Size = new System.Drawing.Size(198, 22);
            this.limitActivityMenuItem.Text = "<Limit selection to current activities...";
            this.limitActivityMenuItem.Click += new System.EventHandler(this.limitActivityMenuItem_Click);
            // 
            // limitURMenuItem
            // 
            this.limitURMenuItem.Name = "limitURMenuItem";
            this.limitURMenuItem.Size = new System.Drawing.Size(198, 22);
            this.limitURMenuItem.Text = "<Limit selection with UR to current activities...";
            this.limitURMenuItem.Click += new System.EventHandler(this.limitURMenuItem_Click);
            // 
            // selectWithURMenuItem
            // 
            this.selectWithURMenuItem.Name = "selectWithURMenuItem";
            this.selectWithURMenuItem.Size = new System.Drawing.Size(198, 22);
            this.selectWithURMenuItem.Text = "<Select with UR to current activities...";
            this.selectWithURMenuItem.Click += new System.EventHandler(selectWithURMenuItem_Click);
            // 
            // markCommonStretchesMenuItem
            // 
            this.markCommonStretchesMenuItem.Name = "markCommonStretchesMenuItem";
            this.markCommonStretchesMenuItem.Size = new System.Drawing.Size(198, 22);
            this.markCommonStretchesMenuItem.Text = "<Mark common stretches...";
            this.markCommonStretchesMenuItem.Click += new System.EventHandler(this.markCommonStretchesMenuItem_Click);
            // 
            // addInBoundActivitiesMenuItem
            // 
            this.addInBoundActivitiesMenuItem.Name = "addInBoundActivitiesMenuItem";
            this.addInBoundActivitiesMenuItem.Size = new System.Drawing.Size(198, 22);
            this.addInBoundActivitiesMenuItem.Text = "<addInBoundActivities...";
            this.addInBoundActivitiesMenuItem.Click += new System.EventHandler(this.addInBoundActivitiesMenuItem_Click);
            // 
            // addActivityCategoryMenuItem
            // 
            this.addCurrentCategoryMenuItem.Name = "addCurrentCategoryMenuItem";
            this.addCurrentCategoryMenuItem.Size = new System.Drawing.Size(198, 22);
            this.addCurrentCategoryMenuItem.Text = "<addCurrentCategoryMenuItem...";
            this.addCurrentCategoryMenuItem.Click += new System.EventHandler(this.addCurrentCategoryMenuItem_Click);
            // 
            // runGradeAdjustMenuItem
            // 
            this.runGradeAdjustMenuItem.Name = "runGradeAdjustMenuItem";
            this.runGradeAdjustMenuItem.Size = new System.Drawing.Size(198, 22);
            this.runGradeAdjustMenuItem.Text = "<runGradeAdjustMenuItem";
            this.runGradeAdjustMenuItem.Click += new System.EventHandler(this.runGradeAdjustMenuItem_Click);
            // 
            // ResultListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.SummaryPanel);
            this.Name = "ResultListControl";
            this.Size = new System.Drawing.Size(400, 60);
            this.SummaryPanel.ResumeLayout(false);
            this.listMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel SummaryPanel;
        private ZoneFiveSoftware.Common.Visuals.TreeList summaryList;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ContextMenuStrip listMenu;
        private System.Windows.Forms.ToolStripMenuItem copyTableMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listSettingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectSimilarSplitsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem referenceResultMenuItem;
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
        private System.Windows.Forms.ToolStripMenuItem runGradeAdjustMenuItem;
        private System.Windows.Forms.ToolTip summaryListToolTip = new System.Windows.Forms.ToolTip();
        private System.Windows.Forms.Timer summaryListToolTipTimer = new System.Windows.Forms.Timer();
    }
}
