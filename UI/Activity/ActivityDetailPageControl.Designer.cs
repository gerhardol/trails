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
			this.List = new ZoneFiveSoftware.Common.Visuals.TreeList();
			this.button1 = new System.Windows.Forms.Button();
			this.panelTrailDetails = new System.Windows.Forms.TableLayoutPanel();
			this.lblTrailName = new System.Windows.Forms.Label();
			this.txtTrailName = new ZoneFiveSoftware.Common.Visuals.TextBox();
			this.listTrailPoint = new ZoneFiveSoftware.Common.Visuals.TreeList();
			this.btnDelete = new ZoneFiveSoftware.Common.Visuals.Button();
			this.btnAdd = new ZoneFiveSoftware.Common.Visuals.Button();
			this.btnSave = new ZoneFiveSoftware.Common.Visuals.Button();
			this.panelTrailDetails.SuspendLayout();
			this.SuspendLayout();
			// 
			// List
			// 
			this.List.AutoScroll = true;
			this.List.AutoSize = true;
			this.List.BackColor = System.Drawing.Color.Transparent;
			this.List.Border = ZoneFiveSoftware.Common.Visuals.ControlBorder.Style.SmallRoundShadow;
			this.List.CheckBoxes = false;
			this.List.DefaultIndent = 15;
			this.List.DefaultRowHeight = -1;
			this.List.HeaderRowHeight = 21;
			this.List.Location = new System.Drawing.Point(437, 109);
			this.List.MultiSelect = true;
			this.List.Name = "List";
			this.List.NumHeaderRows = ZoneFiveSoftware.Common.Visuals.TreeList.HeaderRows.One;
			this.List.NumLockedColumns = 0;
			this.List.RowAlternatingColors = true;
			this.List.RowHotlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this.List.RowHotlightColorText = System.Drawing.SystemColors.HighlightText;
			this.List.RowHotlightMouse = true;
			this.List.RowSelectedColor = System.Drawing.SystemColors.Highlight;
			this.List.RowSelectedColorText = System.Drawing.SystemColors.HighlightText;
			this.List.RowSeparatorLines = true;
			this.List.ShowLines = false;
			this.List.ShowPlusMinus = false;
			this.List.Size = new System.Drawing.Size(160, 82);
			this.List.TabIndex = 1;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(0, 0);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 5;
			// 
			// panelTrailDetails
			// 
			this.panelTrailDetails.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.panelTrailDetails.ColumnCount = 5;
			this.panelTrailDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
			this.panelTrailDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
			this.panelTrailDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.panelTrailDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.panelTrailDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.panelTrailDetails.Controls.Add(this.lblTrailName, 0, 0);
			this.panelTrailDetails.Controls.Add(this.txtTrailName, 1, 0);
			this.panelTrailDetails.Controls.Add(this.listTrailPoint, 0, 2);
			this.panelTrailDetails.Controls.Add(this.btnDelete, 4, 0);
			this.panelTrailDetails.Controls.Add(this.btnAdd, 3, 0);
			this.panelTrailDetails.Controls.Add(this.btnSave, 2, 0);
			this.panelTrailDetails.Location = new System.Drawing.Point(0, 0);
			this.panelTrailDetails.Margin = new System.Windows.Forms.Padding(0);
			this.panelTrailDetails.Name = "panelTrailDetails";
			this.panelTrailDetails.RowCount = 3;
			this.panelTrailDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.panelTrailDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
			this.panelTrailDetails.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.panelTrailDetails.Size = new System.Drawing.Size(361, 277);
			this.panelTrailDetails.TabIndex = 4;
			// 
			// lblTrailName
			// 
			this.lblTrailName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.lblTrailName.AutoSize = true;
			this.lblTrailName.Location = new System.Drawing.Point(0, 0);
			this.lblTrailName.Margin = new System.Windows.Forms.Padding(0);
			this.lblTrailName.Name = "lblTrailName";
			this.lblTrailName.Size = new System.Drawing.Size(38, 20);
			this.lblTrailName.TabIndex = 0;
			this.lblTrailName.Text = "Name:";
			this.lblTrailName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtTrailName
			// 
			this.txtTrailName.AcceptsReturn = false;
			this.txtTrailName.AcceptsTab = false;
			this.txtTrailName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtTrailName.BackColor = System.Drawing.Color.White;
			this.txtTrailName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
			this.txtTrailName.ButtonImage = null;
			this.txtTrailName.Location = new System.Drawing.Point(90, 0);
			this.txtTrailName.Margin = new System.Windows.Forms.Padding(0);
			this.txtTrailName.MaxLength = 32767;
			this.txtTrailName.Multiline = false;
			this.txtTrailName.Name = "txtTrailName";
			this.txtTrailName.ReadOnly = false;
			this.txtTrailName.ReadOnlyColor = System.Drawing.SystemColors.Control;
			this.txtTrailName.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
			this.txtTrailName.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtTrailName.Size = new System.Drawing.Size(150, 19);
			this.txtTrailName.TabIndex = 9;
			this.txtTrailName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			// 
			// listTrailPoint
			// 
			this.listTrailPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listTrailPoint.AutoScroll = true;
			this.listTrailPoint.BackColor = System.Drawing.Color.Transparent;
			this.listTrailPoint.Border = ZoneFiveSoftware.Common.Visuals.ControlBorder.Style.SmallRoundShadow;
			this.listTrailPoint.CheckBoxes = false;
			this.panelTrailDetails.SetColumnSpan(this.listTrailPoint, 5);
			this.listTrailPoint.DefaultIndent = 15;
			this.listTrailPoint.DefaultRowHeight = -1;
			this.listTrailPoint.HeaderRowHeight = 21;
			this.listTrailPoint.Location = new System.Drawing.Point(3, 33);
			this.listTrailPoint.MultiSelect = false;
			this.listTrailPoint.Name = "listTrailPoint";
			this.listTrailPoint.NumHeaderRows = ZoneFiveSoftware.Common.Visuals.TreeList.HeaderRows.One;
			this.listTrailPoint.NumLockedColumns = 0;
			this.listTrailPoint.RowAlternatingColors = true;
			this.listTrailPoint.RowHotlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this.listTrailPoint.RowHotlightColorText = System.Drawing.SystemColors.HighlightText;
			this.listTrailPoint.RowHotlightMouse = true;
			this.listTrailPoint.RowSelectedColor = System.Drawing.SystemColors.Highlight;
			this.listTrailPoint.RowSelectedColorText = System.Drawing.SystemColors.HighlightText;
			this.listTrailPoint.RowSeparatorLines = true;
			this.listTrailPoint.ShowLines = false;
			this.listTrailPoint.ShowPlusMinus = false;
			this.listTrailPoint.Size = new System.Drawing.Size(355, 241);
			this.listTrailPoint.TabIndex = 11;
			this.listTrailPoint.SelectedChanged += new System.EventHandler(this.listTrailPoint_SelectedChanged);
			// 
			// btnDelete
			// 
			this.btnDelete.BackColor = System.Drawing.Color.Transparent;
			this.btnDelete.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
			this.btnDelete.CenterImage = null;
			this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.None;
			this.btnDelete.HyperlinkStyle = false;
			this.btnDelete.ImageMargin = 2;
			this.btnDelete.LeftImage = null;
			this.btnDelete.Location = new System.Drawing.Point(344, 3);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.PushStyle = true;
			this.btnDelete.RightImage = null;
			this.btnDelete.Size = new System.Drawing.Size(14, 14);
			this.btnDelete.TabIndex = 13;
			this.btnDelete.Text = "D";
			this.btnDelete.TextAlign = System.Drawing.StringAlignment.Center;
			this.btnDelete.TextLeftMargin = 2;
			this.btnDelete.TextRightMargin = 2;
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnAdd.BackColor = System.Drawing.Color.Transparent;
			this.btnAdd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
			this.btnAdd.CenterImage = null;
			this.btnAdd.DialogResult = System.Windows.Forms.DialogResult.None;
			this.btnAdd.HyperlinkStyle = false;
			this.btnAdd.ImageMargin = 2;
			this.btnAdd.LeftImage = null;
			this.btnAdd.Location = new System.Drawing.Point(324, 3);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.PushStyle = true;
			this.btnAdd.RightImage = null;
			this.btnAdd.Size = new System.Drawing.Size(14, 14);
			this.btnAdd.TabIndex = 10;
			this.btnAdd.Text = "A";
			this.btnAdd.TextAlign = System.Drawing.StringAlignment.Center;
			this.btnAdd.TextLeftMargin = 2;
			this.btnAdd.TextRightMargin = 2;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnSave
			// 
			this.btnSave.BackColor = System.Drawing.Color.Transparent;
			this.btnSave.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
			this.btnSave.CenterImage = null;
			this.btnSave.DialogResult = System.Windows.Forms.DialogResult.None;
			this.btnSave.HyperlinkStyle = false;
			this.btnSave.ImageMargin = 2;
			this.btnSave.LeftImage = null;
			this.btnSave.Location = new System.Drawing.Point(243, 3);
			this.btnSave.Name = "btnSave";
			this.btnSave.PushStyle = true;
			this.btnSave.RightImage = null;
			this.btnSave.Size = new System.Drawing.Size(14, 14);
			this.btnSave.TabIndex = 14;
			this.btnSave.Text = "S";
			this.btnSave.TextAlign = System.Drawing.StringAlignment.Center;
			this.btnSave.TextLeftMargin = 2;
			this.btnSave.TextRightMargin = 2;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// ActivityDetailPageControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelTrailDetails);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.List);
			this.Name = "ActivityDetailPageControl";
			this.Size = new System.Drawing.Size(585, 426);
			this.Load += new System.EventHandler(this.ActivityDetailPageControl_Load);
			this.panelTrailDetails.ResumeLayout(false);
			this.panelTrailDetails.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ZoneFiveSoftware.Common.Visuals.TreeList List;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TableLayoutPanel panelTrailDetails;
		private System.Windows.Forms.Label lblTrailName;
		private ZoneFiveSoftware.Common.Visuals.TextBox txtTrailName;
		private ZoneFiveSoftware.Common.Visuals.Button btnAdd;
		private ZoneFiveSoftware.Common.Visuals.Button btnDelete;
		private ZoneFiveSoftware.Common.Visuals.TreeList listTrailPoint;
		private ZoneFiveSoftware.Common.Visuals.Button btnSave;

	}
}
