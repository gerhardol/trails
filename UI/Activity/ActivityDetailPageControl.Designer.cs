namespace TrailsPlugin.UI.Activity {
	partial class ActivityDetailPageControl {
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
            this.TrailSelector = new TrailSelectorControl();
            this.UpperSplitContainer = new System.Windows.Forms.SplitContainer();
            this.LowerSplitContainer = new System.Windows.Forms.SplitContainer();
            this.ResultList = new TrailsPlugin.UI.Activity.ResultListControl();
            this.MultiCharts = new TrailsPlugin.UI.Activity.MultiChartsControl();
            this.ExpandSplitContainer = new System.Windows.Forms.SplitContainer();
            this.UpperSplitContainer.Panel1.SuspendLayout();
            this.UpperSplitContainer.Panel2.SuspendLayout();
            this.UpperSplitContainer.SuspendLayout();
            this.LowerSplitContainer.Panel1.SuspendLayout();
            this.LowerSplitContainer.Panel2.SuspendLayout();
            this.LowerSplitContainer.SuspendLayout();
            this.ExpandSplitContainer.Panel1.SuspendLayout();
            this.ExpandSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // TrailSelector
            // 
            this.TrailSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TrailSelector.Name = "TrailSelector";
            // 
            // ResultList
            // 
            this.ResultList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultList.Name = "ResultList";
            this.ResultList.Location = new System.Drawing.Point(0, 0);
            this.ResultList.Margin = new System.Windows.Forms.Padding(0);
            // 
            // MultiCharts
            // 
            this.MultiCharts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MultiCharts.Collapse += new System.EventHandler(MultiCharts_Collapse);
            this.MultiCharts.Expand += new System.EventHandler(btnExpand_Click);
            // 
            // UpperSplitContainer
            // 
            this.UpperSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UpperSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.UpperSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.UpperSplitContainer.Name = "UpperSplitContainer";
            this.UpperSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.UpperSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            // 
            // UpperSplitContainer.Panel1
            // 
            this.UpperSplitContainer.Panel1.Controls.Add(this.TrailSelector);
            this.UpperSplitContainer.Panel1.Cursor = System.Windows.Forms.Cursors.Default;
            this.UpperSplitContainer.Panel1MinSize = 20;
            // 
            // UpperSplitContainer.Panel2
            // 
            this.UpperSplitContainer.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.UpperSplitContainer.Panel2.Controls.Add(this.LowerSplitContainer);
            this.UpperSplitContainer.Panel2MinSize = 100;
            this.UpperSplitContainer.Size = new System.Drawing.Size(400, 273);
            this.UpperSplitContainer.SplitterDistance = 22;
            this.UpperSplitContainer.SplitterWidth = 1;
            this.UpperSplitContainer.TabIndex = 17;
            // 
            // LowerSplitContainer
            // 
            this.LowerSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LowerSplitContainer.Location = new System.Drawing.Point(0, 27);
            this.LowerSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.LowerSplitContainer.Name = "LowerSplitContainer";
            this.LowerSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // LowerSplitContainer.Panel1
            // 
            this.LowerSplitContainer.Panel1.Controls.Add(this.ResultList);
            this.LowerSplitContainer.Panel1.Cursor = System.Windows.Forms.Cursors.Default;
            this.LowerSplitContainer.Panel1MinSize = 50;
            this.LowerSplitContainer.Panel1.AutoScroll = false;
            // 
            // LowerSplitContainer.Panel2
            // 
            this.LowerSplitContainer.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.LowerSplitContainer.Panel2.Controls.Add(this.MultiCharts);
            this.LowerSplitContainer.Panel2MinSize = 100;
            this.LowerSplitContainer.Size = new System.Drawing.Size(400, 273);
            this.LowerSplitContainer.SplitterDistance = 60;
            this.LowerSplitContainer.SplitterWidth = 1;
            this.LowerSplitContainer.TabIndex = 18;
            // 
            // ExpandSplitContainer
            // 
            this.ExpandSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExpandSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.ExpandSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.ExpandSplitContainer.Name = "ExpandSplitContainer";
            // 
            // ExpandSplitContainer.Panel1
            // 
            this.ExpandSplitContainer.Panel1.Controls.Add(this.UpperSplitContainer);
            this.ExpandSplitContainer.Panel1.Cursor = System.Windows.Forms.Cursors.Default;
            this.ExpandSplitContainer.Panel1MinSize = 225;
            // 
            // ExpandSplitContainer.Panel2
            // 
            this.ExpandSplitContainer.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.ExpandSplitContainer.Panel2Collapsed = true;
            this.ExpandSplitContainer.Panel2MinSize = 0;
            //this.ExpandSplitContainer.Panel2.Controls.Add(MultiCharts);
            this.ExpandSplitContainer.Size = new System.Drawing.Size(400, 300);
            this.ExpandSplitContainer.SplitterDistance = 400;
            this.ExpandSplitContainer.SplitterWidth = 1;
            this.ExpandSplitContainer.TabIndex = 18;
            // 
            // ActivityDetailPageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.ExpandSplitContainer);
            this.Name = "ActivityDetailPageControl";
            this.Size = new System.Drawing.Size(400, 300);
            this.UpperSplitContainer.Panel1.ResumeLayout(false);
            this.UpperSplitContainer.Panel2.ResumeLayout(false);
            this.UpperSplitContainer.Panel2.PerformLayout();
            this.UpperSplitContainer.ResumeLayout(false);
            this.LowerSplitContainer.Panel1.ResumeLayout(false);
            this.LowerSplitContainer.Panel2.ResumeLayout(false);
            this.LowerSplitContainer.Panel2.PerformLayout();
            this.LowerSplitContainer.ResumeLayout(false);
            this.ExpandSplitContainer.Panel1.ResumeLayout(false);
            this.ExpandSplitContainer.Panel1.PerformLayout();
            this.ExpandSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
		}

		#endregion

		private TrailSelectorControl TrailSelector;
        private ResultListControl ResultList;
        private MultiChartsControl MultiCharts;
        private System.Windows.Forms.SplitContainer ExpandSplitContainer;
        private System.Windows.Forms.SplitContainer LowerSplitContainer;
        private System.Windows.Forms.SplitContainer UpperSplitContainer;
    }
}
