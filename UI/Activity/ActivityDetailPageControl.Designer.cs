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
            this.ActPagePanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblTrail = new System.Windows.Forms.Label();
            this.TrailName = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.btnAdd = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnEdit = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnDelete = new ZoneFiveSoftware.Common.Visuals.Button();
            this.ActPageSplitContainer = new System.Windows.Forms.SplitContainer();
            this.summaryList = new ResultListControl();
            this.SingleChart = new SingleChartsControl();
            this.ExpandSplitContainer = new System.Windows.Forms.SplitContainer();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ActPagePanel.SuspendLayout();
            this.ActPageSplitContainer.Panel1.SuspendLayout();
            this.ActPageSplitContainer.Panel2.SuspendLayout();
            this.ActPageSplitContainer.SuspendLayout();
            this.ExpandSplitContainer.Panel1.SuspendLayout();
            this.ExpandSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // ActPagePanel
            // 
            this.ActPagePanel.AutoSize = true;
            this.ActPagePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ActPagePanel.ColumnCount = 6;
            this.ActPagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.ActPagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.ActPagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.ActPagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.ActPagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.ActPagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.ActPagePanel.Controls.Add(this.lblTrail, 0, 0);
            this.ActPagePanel.Controls.Add(this.TrailName, 1, 0);
            this.ActPagePanel.Controls.Add(this.btnAdd, 3, 0);
            this.ActPagePanel.Controls.Add(this.btnEdit, 4, 0);
            this.ActPagePanel.Controls.Add(this.btnDelete, 5, 0);
            this.ActPagePanel.Controls.Add(this.ActPageSplitContainer, 0, 2);
            this.ActPagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ActPagePanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.ActPagePanel.Location = new System.Drawing.Point(0, 0);
            this.ActPagePanel.Margin = new System.Windows.Forms.Padding(0);
            this.ActPagePanel.Name = "ActPagePanel";
            this.ActPagePanel.RowCount = 3;
            this.ActPagePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.ActPagePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.ActPagePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ActPagePanel.Size = new System.Drawing.Size(400, 300);
            this.ActPagePanel.TabIndex = 8;
            this.ActPagePanel.SizeChanged += new System.EventHandler(this.ActPagePanel_SizeChanged);
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
            this.TrailName.AutoSize = true;
            this.TrailName.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TrailName.BackColor = System.Drawing.Color.White;
            this.TrailName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.TrailName.ButtonImage = null;
            this.TrailName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrailName.Location = new System.Drawing.Point(91, 1);
            this.TrailName.Margin = new System.Windows.Forms.Padding(1);
            this.TrailName.MaxLength = 32767;
            this.TrailName.Multiline = false;
            this.TrailName.Name = "TrailName";
            this.TrailName.ReadOnly = true;
            this.TrailName.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.TrailName.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.TrailName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.TrailName.Size = new System.Drawing.Size(208, 18);
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
            this.btnAdd.ImageMargin = 2;
            this.btnAdd.LeftImage = null;
            this.btnAdd.Location = new System.Drawing.Point(328, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Padding = new System.Windows.Forms.Padding(2);
            this.btnAdd.PushStyle = true;
            this.btnAdd.RightImage = null;
            this.btnAdd.Size = new System.Drawing.Size(19, 16);
            this.btnAdd.TabIndex = 10;
            this.btnAdd.Text = "A";
            this.btnAdd.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnAdd.TextLeftMargin = 2;
            this.btnAdd.TextRightMargin = 2;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
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
            this.btnEdit.ImageMargin = 2;
            this.btnEdit.LeftImage = null;
            this.btnEdit.Location = new System.Drawing.Point(353, 3);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Padding = new System.Windows.Forms.Padding(2);
            this.btnEdit.PushStyle = true;
            this.btnEdit.RightImage = null;
            this.btnEdit.Size = new System.Drawing.Size(19, 16);
            this.btnEdit.TabIndex = 17;
            this.btnEdit.Text = "E";
            this.btnEdit.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnEdit.TextLeftMargin = 2;
            this.btnEdit.TextRightMargin = 2;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
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
            this.btnDelete.ImageMargin = 2;
            this.btnDelete.LeftImage = null;
            this.btnDelete.Location = new System.Drawing.Point(378, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Padding = new System.Windows.Forms.Padding(2);
            this.btnDelete.PushStyle = true;
            this.btnDelete.RightImage = null;
            this.btnDelete.Size = new System.Drawing.Size(19, 16);
            this.btnDelete.TabIndex = 13;
            this.btnDelete.Text = "D";
            this.btnDelete.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnDelete.TextLeftMargin = 2;
            this.btnDelete.TextRightMargin = 2;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // summaryListControl
            // 
            this.summaryList.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // SingleChart
            // 
            this.SingleChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SingleChart.Expand += new System.EventHandler(btnExpand_Click);
            // 
            // ActPageSplitContainer
            // 
            this.ActPagePanel.SetColumnSpan(this.ActPageSplitContainer, 6);
            this.ActPageSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ActPageSplitContainer.Location = new System.Drawing.Point(0, 27);
            this.ActPageSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.ActPageSplitContainer.Name = "ActPageSplitContainer";
            this.ActPageSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // ActPageSplitContainer.Panel1
            // 
            this.ActPageSplitContainer.Panel1.Controls.Add(this.summaryList);
            this.ActPageSplitContainer.Panel1.Cursor = System.Windows.Forms.Cursors.Default;
            this.ActPageSplitContainer.Panel1MinSize = 50;
            // 
            // ActPageSplitContainer.Panel2
            // 
            this.ActPageSplitContainer.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.ActPageSplitContainer.Panel2.Controls.Add(this.SingleChart);
            this.ActPageSplitContainer.Panel2MinSize = 100;
            this.ActPageSplitContainer.Size = new System.Drawing.Size(400, 273);
            this.ActPageSplitContainer.SplitterDistance = 60;
            this.ActPageSplitContainer.SplitterWidth = 1;
            this.ActPageSplitContainer.TabIndex = 18;
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
            this.ExpandSplitContainer.Panel1.Controls.Add(this.ActPagePanel);
            this.ExpandSplitContainer.Panel1.Cursor = System.Windows.Forms.Cursors.Default;
            this.ExpandSplitContainer.Panel1MinSize = 225;
            // 
            // ExpandSplitContainer.Panel2
            // 
            this.ExpandSplitContainer.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.ExpandSplitContainer.Panel2Collapsed = true;
            this.ExpandSplitContainer.Panel2MinSize = 0;
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
            this.ActPagePanel.ResumeLayout(false);
            this.ActPagePanel.PerformLayout();
            this.ActPageSplitContainer.Panel1.ResumeLayout(false);
            this.ActPageSplitContainer.Panel2.ResumeLayout(false);
            this.ActPageSplitContainer.Panel2.PerformLayout();
            this.ActPageSplitContainer.ResumeLayout(false);
            this.ExpandSplitContainer.Panel1.ResumeLayout(false);
            this.ExpandSplitContainer.Panel1.PerformLayout();
            this.ExpandSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel ActPagePanel;
		private System.Windows.Forms.Label lblTrail;
        private ResultListControl summaryList;
        private SingleChartsControl SingleChart;
		private ZoneFiveSoftware.Common.Visuals.Button btnDelete;
		private ZoneFiveSoftware.Common.Visuals.Button btnAdd;
		private ZoneFiveSoftware.Common.Visuals.TextBox TrailName;
		private ZoneFiveSoftware.Common.Visuals.Button btnEdit;
		private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.SplitContainer ExpandSplitContainer;
        private System.Windows.Forms.SplitContainer ActPageSplitContainer;
	}
}
