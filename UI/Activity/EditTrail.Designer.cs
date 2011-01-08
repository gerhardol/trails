namespace TrailsPlugin.UI.Activity {
	partial class EditTrail {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnOk = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnCancel = new ZoneFiveSoftware.Common.Visuals.Button();
            this.Panel = new System.Windows.Forms.TableLayoutPanel();
            this.Radius = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.lblTrail = new System.Windows.Forms.Label();
            this.TrailName = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.EList = new ZoneFiveSoftware.Common.Visuals.TreeList();
            this.editBox = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.lblRadius = new System.Windows.Forms.Label();
            this.btnDelete = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnEdit = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnAdd = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnSave = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnUp = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnDown = new ZoneFiveSoftware.Common.Visuals.Button();
            this.Panel.SuspendLayout();
            this.EList.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.Color.Transparent;
            this.btnOk.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnOk.CenterImage = null;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnOk.HyperlinkStyle = false;
            this.btnOk.ImageMargin = 2;
            this.btnOk.LeftImage = null;
            this.btnOk.Location = new System.Drawing.Point(215, 303);
            this.btnOk.Name = "btnOk";
            this.btnOk.PushStyle = true;
            this.btnOk.RightImage = null;
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnOk.TextLeftMargin = 2;
            this.btnOk.TextRightMargin = 2;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnCancel.CenterImage = null;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.HyperlinkStyle = false;
            this.btnCancel.ImageMargin = 2;
            this.btnCancel.LeftImage = null;
            this.btnCancel.Location = new System.Drawing.Point(296, 303);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.PushStyle = true;
            this.btnCancel.RightImage = null;
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnCancel.TextLeftMargin = 2;
            this.btnCancel.TextRightMargin = 2;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Panel
            // 
            this.Panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel.ColumnCount = 6;
            this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.Panel.Controls.Add(this.Radius, 1, 1);
            this.Panel.Controls.Add(this.lblTrail, 0, 0);
            this.Panel.Controls.Add(this.TrailName, 1, 0);
            this.Panel.Controls.Add(this.EList, 0, 2);
            this.Panel.Controls.Add(this.lblRadius, 0, 1);
            this.Panel.Controls.Add(this.btnAdd, 3, 1);
            this.Panel.Controls.Add(this.btnSave, 2, 1);
            this.Panel.Controls.Add(this.btnEdit, 4, 0);
            this.Panel.Controls.Add(this.btnDelete, 5, 0);
            this.Panel.Controls.Add(this.btnUp, 4, 1);
            this.Panel.Controls.Add(this.btnDown, 5, 1);
            this.Panel.Location = new System.Drawing.Point(15, 13);
            this.Panel.Margin = new System.Windows.Forms.Padding(0);
            this.Panel.Name = "Panel";
            this.Panel.RowCount = 3;
            this.Panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.Panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.Panel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Panel.Size = new System.Drawing.Size(350, 271);
            this.Panel.TabIndex = 9;
            // 
            // Radius
            // 
            this.Radius.AcceptsReturn = false;
            this.Radius.AcceptsTab = false;
            this.Radius.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Radius.BackColor = System.Drawing.Color.White;
            this.Radius.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.Radius.ButtonImage = null;
            this.Radius.Location = new System.Drawing.Point(91, 25);
            this.Radius.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.Radius.MaxLength = 32767;
            this.Radius.Multiline = false;
            this.Radius.Name = "Radius";
            this.Radius.ReadOnly = false;
            this.Radius.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.Radius.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.Radius.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Radius.Size = new System.Drawing.Size(149, 19);
            this.Radius.TabIndex = 20;
            this.Radius.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.Radius.LostFocus += new System.EventHandler(this.Radius_LostFocus);
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
            this.TrailName.BackColor = System.Drawing.Color.White;
            this.TrailName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.TrailName.ButtonImage = null;
            this.TrailName.Location = new System.Drawing.Point(91, 0);
            this.TrailName.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.TrailName.MaxLength = 32767;
            this.TrailName.Multiline = false;
            this.TrailName.Name = "TrailName";
            this.TrailName.ReadOnly = false;
            this.TrailName.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.TrailName.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.TrailName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.TrailName.Size = new System.Drawing.Size(149, 19);
            this.TrailName.TabIndex = 1;
            this.TrailName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // EList
            // 
            this.EList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.EList.AutoScroll = true;
            this.EList.BackColor = System.Drawing.Color.Transparent;
            this.EList.Border = ZoneFiveSoftware.Common.Visuals.ControlBorder.Style.SmallRoundShadow;
            this.EList.CheckBoxes = false;
            this.Panel.SetColumnSpan(this.EList, 6);
            this.EList.Controls.Add(this.editBox);
            this.EList.DefaultIndent = 15;
            this.EList.DefaultRowHeight = -1;
            this.EList.HeaderRowHeight = 21;
            this.EList.Location = new System.Drawing.Point(3, 53);
            this.EList.MultiSelect = true;
            this.EList.Name = "EList";
            this.EList.NumHeaderRows = ZoneFiveSoftware.Common.Visuals.TreeList.HeaderRows.One;
            this.EList.NumLockedColumns = 0;
            this.EList.RowAlternatingColors = true;
            this.EList.RowHotlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.EList.RowHotlightColorText = System.Drawing.SystemColors.HighlightText;
            this.EList.RowHotlightMouse = true;
            this.EList.RowSelectedColor = System.Drawing.SystemColors.Highlight;
            this.EList.RowSelectedColorText = System.Drawing.SystemColors.HighlightText;
            this.EList.RowSeparatorLines = true;
            this.EList.ShowLines = false;
            this.EList.ShowPlusMinus = false;
            this.EList.Size = new System.Drawing.Size(344, 218);
            this.EList.TabIndex = 11;
            this.EList.DoubleClick += new System.EventHandler(this.SMKDoubleClick);
            this.EList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EList_KeyDown);
            this.EList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SMKMouseDown);
            // 
            // editBox
            // 
            this.editBox.AcceptsReturn = false;
            this.editBox.AcceptsTab = false;
            this.editBox.BackColor = System.Drawing.Color.White;
            this.editBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.editBox.ButtonImage = null;
            this.editBox.Location = new System.Drawing.Point(0, 0);
            this.editBox.MaxLength = 32767;
            this.editBox.Multiline = false;
            this.editBox.Name = "editBox";
            this.editBox.ReadOnly = false;
            this.editBox.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.editBox.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.editBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.editBox.Size = new System.Drawing.Size(0, 0);
            this.editBox.TabIndex = 2;
            this.editBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.editBox.Visible = false;
            this.editBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EditOver);
            this.editBox.LostFocus += new System.EventHandler(this.FocusOver);
            // 
            // lblRadius
            // 
            this.lblRadius.AutoSize = true;
            this.lblRadius.Location = new System.Drawing.Point(0, 28);
            this.lblRadius.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lblRadius.Name = "lblRadius";
            this.lblRadius.Size = new System.Drawing.Size(43, 13);
            this.lblRadius.TabIndex = 19;
            this.lblRadius.Text = "Radius:";
            this.lblRadius.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.Transparent;
            this.btnDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnDelete.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnDelete.CenterImage = null;
            this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnDelete.HyperlinkStyle = false;
            this.btnDelete.ImageMargin = 2;
            this.btnDelete.LeftImage = null;
            this.btnDelete.Location = new System.Drawing.Point(318, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Padding = new System.Windows.Forms.Padding(2);
            this.btnDelete.PushStyle = true;
            this.btnDelete.RightImage = null;
            this.btnDelete.Size = new System.Drawing.Size(20, 19);
            this.btnDelete.TabIndex = 13;
            this.btnDelete.Text = "D";
            this.btnDelete.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnDelete.TextLeftMargin = 2;
            this.btnDelete.TextRightMargin = 2;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.Transparent;
            this.btnEdit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnEdit.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnEdit.CenterImage = null;
            this.btnEdit.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnEdit.HyperlinkStyle = false;
            this.btnEdit.ImageMargin = 2;
            this.btnEdit.LeftImage = null;
            this.btnEdit.Location = new System.Drawing.Point(293, 3);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Padding = new System.Windows.Forms.Padding(2);
            this.btnEdit.PushStyle = true;
            this.btnEdit.RightImage = null;
            this.btnEdit.Size = new System.Drawing.Size(19, 19);
            this.btnEdit.TabIndex = 17;
            this.btnEdit.Text = "E";
            this.btnEdit.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnEdit.TextLeftMargin = 2;
            this.btnEdit.TextRightMargin = 2;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.Transparent;
            this.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnAdd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnAdd.CenterImage = null;
            this.btnAdd.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnAdd.HyperlinkStyle = false;
            this.btnAdd.ImageMargin = 2;
            this.btnAdd.LeftImage = null;
            this.btnAdd.Location = new System.Drawing.Point(268, 28);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Padding = new System.Windows.Forms.Padding(2);
            this.btnAdd.PushStyle = true;
            this.btnAdd.RightImage = null;
            this.btnAdd.Size = new System.Drawing.Size(19, 19);
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
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSave.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnSave.CenterImage = null;
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnSave.HyperlinkStyle = false;
            this.btnSave.ImageMargin = 2;
            this.btnSave.LeftImage = null;
            this.btnSave.Location = new System.Drawing.Point(243, 28);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(2);
            this.btnSave.PushStyle = true;
            this.btnSave.RightImage = null;
            this.btnSave.Size = new System.Drawing.Size(19, 19);
            this.btnSave.TabIndex = 18;
            this.btnSave.Text = "S";
            this.btnSave.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnSave.TextLeftMargin = 2;
            this.btnSave.TextRightMargin = 2;
            this.btnSave.Visible = false;
            // 
            // btnUp
            // 
            this.btnUp.BackColor = System.Drawing.Color.Transparent;
            this.btnUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnUp.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnUp.CenterImage = null;
            this.btnUp.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnUp.HyperlinkStyle = false;
            this.btnUp.ImageMargin = 2;
            this.btnUp.LeftImage = null;
            this.btnUp.Location = new System.Drawing.Point(293, 28);
            this.btnUp.Name = "btnUp";
            this.btnUp.Padding = new System.Windows.Forms.Padding(2);
            this.btnUp.PushStyle = true;
            this.btnUp.RightImage = null;
            this.btnUp.Size = new System.Drawing.Size(19, 19);
            this.btnUp.TabIndex = 21;
            this.btnUp.Text = "A";
            this.btnUp.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnUp.TextLeftMargin = 2;
            this.btnUp.TextRightMargin = 2;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.BackColor = System.Drawing.Color.Transparent;
            this.btnDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnDown.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnDown.CenterImage = null;
            this.btnDown.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnDown.HyperlinkStyle = false;
            this.btnDown.ImageMargin = 2;
            this.btnDown.LeftImage = null;
            this.btnDown.Location = new System.Drawing.Point(318, 28);
            this.btnDown.Name = "btnDown";
            this.btnDown.Padding = new System.Windows.Forms.Padding(2);
            this.btnDown.PushStyle = true;
            this.btnDown.RightImage = null;
            this.btnDown.Size = new System.Drawing.Size(19, 19);
            this.btnDown.TabIndex = 22;
            this.btnDown.Text = "V";
            this.btnDown.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnDown.TextLeftMargin = 2;
            this.btnDown.TextRightMargin = 2;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // EditTrail
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(381, 328);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.Panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditTrail";
            this.Text = "EditTrail";
            this.Activated += new System.EventHandler(this.EditTrail_Activated);
            this.Shown += new System.EventHandler(this.EditTrail_Shown);
            this.Panel.ResumeLayout(false);
            this.Panel.PerformLayout();
            this.EList.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolTip toolTip;
		private ZoneFiveSoftware.Common.Visuals.Button btnOk;
		private ZoneFiveSoftware.Common.Visuals.Button btnCancel;
		private System.Windows.Forms.TableLayoutPanel Panel;
		private ZoneFiveSoftware.Common.Visuals.Button btnSave;
		private System.Windows.Forms.Label lblTrail;
		private ZoneFiveSoftware.Common.Visuals.Button btnAdd;
		private ZoneFiveSoftware.Common.Visuals.TextBox TrailName;
		private ZoneFiveSoftware.Common.Visuals.Button btnDelete;
		private ZoneFiveSoftware.Common.Visuals.Button btnEdit;
        private ZoneFiveSoftware.Common.Visuals.TreeList EList;
        private ZoneFiveSoftware.Common.Visuals.TextBox Radius;
		private System.Windows.Forms.Label lblRadius;
        private ZoneFiveSoftware.Common.Visuals.TextBox editBox;
        private ZoneFiveSoftware.Common.Visuals.Button btnUp;
        private ZoneFiveSoftware.Common.Visuals.Button btnDown;

	}
}