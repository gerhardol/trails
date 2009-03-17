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
			this.btnSave = new ZoneFiveSoftware.Common.Visuals.Button();
			this.lblTrail = new System.Windows.Forms.Label();
			this.btnAdd = new ZoneFiveSoftware.Common.Visuals.Button();
			this.TrailName = new ZoneFiveSoftware.Common.Visuals.TextBox();
			this.btnDelete = new ZoneFiveSoftware.Common.Visuals.Button();
			this.btnEdit = new ZoneFiveSoftware.Common.Visuals.Button();
			this.List = new ZoneFiveSoftware.Common.Visuals.TreeList();
			this.label1 = new System.Windows.Forms.Label();
			this.Panel.SuspendLayout();
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
			this.Panel.Controls.Add(this.btnSave, 2, 0);
			this.Panel.Controls.Add(this.lblTrail, 0, 0);
			this.Panel.Controls.Add(this.btnAdd, 3, 0);
			this.Panel.Controls.Add(this.TrailName, 1, 0);
			this.Panel.Controls.Add(this.btnDelete, 5, 0);
			this.Panel.Controls.Add(this.btnEdit, 4, 0);
			this.Panel.Controls.Add(this.List, 0, 2);
			this.Panel.Controls.Add(this.label1, 0, 1);
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
			this.btnSave.Location = new System.Drawing.Point(243, 3);
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
			// lblTrail
			// 
			this.lblTrail.AutoSize = true;
			this.lblTrail.Location = new System.Drawing.Point(0, 0);
			this.lblTrail.Margin = new System.Windows.Forms.Padding(0);
			this.lblTrail.Name = "lblTrail";
			this.lblTrail.Size = new System.Drawing.Size(30, 13);
			this.lblTrail.TabIndex = 0;
			this.lblTrail.Text = "Trail:";
			this.lblTrail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
			this.btnAdd.Location = new System.Drawing.Point(268, 3);
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
			this.btnAdd.Visible = false;
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
			this.btnDelete.Visible = false;
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
			this.btnEdit.Visible = false;
			// 
			// List
			// 
			this.List.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.List.AutoScroll = true;
			this.List.BackColor = System.Drawing.Color.Transparent;
			this.List.Border = ZoneFiveSoftware.Common.Visuals.ControlBorder.Style.SmallRoundShadow;
			this.List.CheckBoxes = false;
			this.Panel.SetColumnSpan(this.List, 6);
			this.List.DefaultIndent = 15;
			this.List.DefaultRowHeight = -1;
			this.List.HeaderRowHeight = 21;
			this.List.Location = new System.Drawing.Point(3, 53);
			this.List.MultiSelect = false;
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
			this.List.Size = new System.Drawing.Size(344, 218);
			this.List.TabIndex = 11;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(0, 25);
			this.label1.Margin = new System.Windows.Forms.Padding(0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(43, 13);
			this.label1.TabIndex = 19;
			this.label1.Text = "Radius:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
			this.Shown += new System.EventHandler(this.EditTrail_Shown);
			this.Activated += new System.EventHandler(this.EditTrail_Activated);
			this.Panel.ResumeLayout(false);
			this.Panel.PerformLayout();
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
		private ZoneFiveSoftware.Common.Visuals.TreeList List;
		private ZoneFiveSoftware.Common.Visuals.TextBox Radius;
		private System.Windows.Forms.Label label1;

	}
}