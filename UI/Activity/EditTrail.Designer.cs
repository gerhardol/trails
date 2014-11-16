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
            this.chkTemporaryTrail = new System.Windows.Forms.CheckBox();
            this.chkTwoWay = new System.Windows.Forms.CheckBox();
            this.chkName = new System.Windows.Forms.CheckBox();
            this.chkCompleteActivity = new System.Windows.Forms.CheckBox();
            this.chkURFilter = new System.Windows.Forms.CheckBox();
            this.btnCopy = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnExport = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnRefresh = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnReverse = new ZoneFiveSoftware.Common.Visuals.Button();
            this.numericSortPrio = new System.Windows.Forms.NumericUpDown();
            this.btnOk = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnCancel = new ZoneFiveSoftware.Common.Visuals.Button();
            this.Panel = new System.Windows.Forms.TableLayoutPanel();
            this.lblTrail = new System.Windows.Forms.Label();
            this.TrailName = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.btnAdd = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnEdit = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnDelete = new ZoneFiveSoftware.Common.Visuals.Button();
            this.lblRadius = new System.Windows.Forms.Label();
            this.radiusBox = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.btnUp = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnDown = new ZoneFiveSoftware.Common.Visuals.Button();
            this.EList = new ZoneFiveSoftware.Common.Visuals.TreeList();
            this.editBox = new ZoneFiveSoftware.Common.Visuals.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericSortPrio)).BeginInit();
            this.Panel.SuspendLayout();
            this.EList.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkTemporaryTrail
            // 
            this.chkTemporaryTrail.AutoSize = true;
            this.chkTemporaryTrail.Checked = true;
            this.chkTemporaryTrail.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTemporaryTrail.Location = new System.Drawing.Point(189, 302);
            this.chkTemporaryTrail.Name = "chkTemporaryTrail";
            this.chkTemporaryTrail.Size = new System.Drawing.Size(15, 14);
            this.chkTemporaryTrail.TabIndex = 25;
            this.toolTip.SetToolTip(this.chkTemporaryTrail, ">Temporary trail (deleted when exiting SportTracks)");
            this.chkTemporaryTrail.UseVisualStyleBackColor = true;
            // 
            // chkTwoWay
            // 
            this.chkTwoWay.AutoSize = true;
            this.chkTwoWay.Checked = true;
            this.chkTwoWay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTwoWay.Location = new System.Drawing.Point(168, 302);
            this.chkTwoWay.Name = "chkTwoWay";
            this.chkTwoWay.Size = new System.Drawing.Size(15, 14);
            this.chkTwoWay.TabIndex = 26;
            this.toolTip.SetToolTip(this.chkTwoWay, ">Two-way match");
            this.chkTwoWay.UseVisualStyleBackColor = true;
            // 
            // chkName
            // 
            this.chkName.AutoSize = true;
            this.chkName.Checked = true;
            this.chkName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkName.Location = new System.Drawing.Point(210, 302);
            this.chkName.Name = "chkName";
            this.chkName.Size = new System.Drawing.Size(15, 14);
            this.chkName.TabIndex = 27;
            this.toolTip.SetToolTip(this.chkName, ">Name match");
            this.chkName.UseVisualStyleBackColor = true;
            // 
            // chkCompleteActivity
            // 
            this.chkCompleteActivity.AutoSize = true;
            this.chkCompleteActivity.Checked = true;
            this.chkCompleteActivity.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCompleteActivity.Location = new System.Drawing.Point(231, 302);
            this.chkCompleteActivity.Name = "chkCompleteActivity";
            this.chkCompleteActivity.Size = new System.Drawing.Size(15, 14);
            this.chkCompleteActivity.TabIndex = 28;
            this.toolTip.SetToolTip(this.chkCompleteActivity, ">Complete Activity");
            this.chkCompleteActivity.UseVisualStyleBackColor = true;
            // 
            // chkURFilter
            // 
            this.chkURFilter.AutoSize = true;
            this.chkURFilter.Checked = true;
            this.chkURFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkURFilter.Location = new System.Drawing.Point(252, 302);
            this.chkURFilter.Name = "chkURFilter";
            this.chkURFilter.Size = new System.Drawing.Size(15, 14);
            this.chkURFilter.TabIndex = 28;
            this.toolTip.SetToolTip(this.chkURFilter, ">UR Filter");
            this.chkURFilter.UseVisualStyleBackColor = true;
            // 
            // btnCopy
            // 
            this.btnCopy.BackColor = System.Drawing.Color.Transparent;
            this.btnCopy.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnCopy.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnCopy.CenterImage = null;
            this.btnCopy.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnCopy.HyperlinkStyle = false;
            this.btnCopy.ImageMargin = 2;
            this.btnCopy.LeftImage = null;
            this.btnCopy.Location = new System.Drawing.Point(18, 297);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Padding = new System.Windows.Forms.Padding(2);
            this.btnCopy.PushStyle = true;
            this.btnCopy.RightImage = null;
            this.btnCopy.Size = new System.Drawing.Size(19, 19);
            this.btnCopy.TabIndex = 23;
            this.btnCopy.Text = "C";
            this.btnCopy.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnCopy.TextLeftMargin = 2;
            this.btnCopy.TextRightMargin = 2;
            this.toolTip.SetToolTip(this.btnCopy, "<Copy Trail");
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.Transparent;
            this.btnExport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnExport.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnExport.CenterImage = null;
            this.btnExport.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnExport.HyperlinkStyle = false;
            this.btnExport.ImageMargin = 2;
            this.btnExport.LeftImage = null;
            this.btnExport.Location = new System.Drawing.Point(52, 297);
            this.btnExport.Name = "btnExport";
            this.btnExport.Padding = new System.Windows.Forms.Padding(2);
            this.btnExport.PushStyle = true;
            this.btnExport.RightImage = null;
            this.btnExport.Size = new System.Drawing.Size(19, 19);
            this.btnExport.TabIndex = 24;
            this.btnExport.Text = "E";
            this.btnExport.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnExport.TextLeftMargin = 2;
            this.btnExport.TextRightMargin = 2;
            this.toolTip.SetToolTip(this.btnExport, "<Export to activity");
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.Transparent;
            this.btnRefresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnRefresh.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnRefresh.CenterImage = null;
            this.btnRefresh.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnRefresh.HyperlinkStyle = false;
            this.btnRefresh.ImageMargin = 2;
            this.btnRefresh.LeftImage = null;
            this.btnRefresh.Location = new System.Drawing.Point(86, 297);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Padding = new System.Windows.Forms.Padding(2);
            this.btnRefresh.PushStyle = true;
            this.btnRefresh.RightImage = null;
            this.btnRefresh.Size = new System.Drawing.Size(19, 19);
            this.btnRefresh.TabIndex = 24;
            this.btnRefresh.Text = "R";
            this.btnRefresh.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnRefresh.TextLeftMargin = 2;
            this.btnRefresh.TextRightMargin = 2;
            this.toolTip.SetToolTip(this.btnRefresh, "<Refresh");
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnReverse
            // 
            this.btnReverse.BackColor = System.Drawing.Color.Transparent;
            this.btnReverse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnReverse.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnReverse.CenterImage = null;
            this.btnReverse.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnReverse.HyperlinkStyle = false;
            this.btnReverse.ImageMargin = 2;
            this.btnReverse.LeftImage = null;
            this.btnReverse.Location = new System.Drawing.Point(120, 297);
            this.btnReverse.Name = "btnReverse";
            this.btnReverse.Padding = new System.Windows.Forms.Padding(2);
            this.btnReverse.PushStyle = true;
            this.btnReverse.RightImage = null;
            this.btnReverse.Size = new System.Drawing.Size(19, 19);
            this.btnReverse.TabIndex = 29;
            this.btnReverse.Text = "R";
            this.btnReverse.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnReverse.TextLeftMargin = 2;
            this.btnReverse.TextRightMargin = 2;
            this.toolTip.SetToolTip(this.btnReverse, "<Reverse Trail");
            this.btnReverse.Click += new System.EventHandler(this.btnReverse_Click);
            // 
            // numericUpDown1
            // 
            this.numericSortPrio.Location = new System.Drawing.Point(272, 298);
            this.numericSortPrio.Name = "numericUpDown1";
            this.numericSortPrio.Size = new System.Drawing.Size(40, 20);
            this.numericSortPrio.TabIndex = 23;
            this.toolTip.SetToolTip(this.numericSortPrio, ">SortPriority");
            this.numericSortPrio.Minimum = int.MinValue;
            //this.numericSortPrio.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
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
            this.btnOk.Location = new System.Drawing.Point(394, 297);
            this.btnOk.Name = "btnOk";
            this.btnOk.PushStyle = true;
            this.btnOk.RightImage = null;
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "<OK";
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
            this.btnCancel.Location = new System.Drawing.Point(476, 297);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.PushStyle = true;
            this.btnCancel.RightImage = null;
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "<Cancel";
            this.btnCancel.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnCancel.TextLeftMargin = 2;
            this.btnCancel.TextRightMargin = 2;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Panel
            // 
            this.Panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Panel.ColumnCount = 9;
            this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.Panel.Controls.Add(this.lblTrail, 1, 1);
            this.Panel.Controls.Add(this.TrailName, 2, 1);
            this.Panel.Controls.Add(this.btnAdd, 4, 1);
            this.Panel.Controls.Add(this.btnEdit, 5, 1);
            this.Panel.Controls.Add(this.btnDelete, 6, 1);
            this.Panel.Controls.Add(this.lblRadius, 1, 2);
            this.Panel.Controls.Add(this.radiusBox, 2, 2);
            this.Panel.Controls.Add(this.btnUp, 5, 2);
            this.Panel.Controls.Add(this.btnDown, 6, 2);
            this.Panel.Controls.Add(this.EList, 1, 3);
            this.Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel.Location = new System.Drawing.Point(0, 0);
            this.Panel.Name = "Panel";
            this.Panel.RowCount = 5;
            this.Panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.Panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.Panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.Panel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.Panel.Size = new System.Drawing.Size(555, 325);
            this.Panel.TabIndex = 9;
            // 
            // lblTrail
            // 
            this.lblTrail.AutoSize = true;
            this.lblTrail.Location = new System.Drawing.Point(1, 5);
            this.lblTrail.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lblTrail.Name = "lblTrail";
            this.lblTrail.Size = new System.Drawing.Size(36, 13);
            this.lblTrail.TabIndex = 0;
            this.lblTrail.Text = "<Trail:";
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
            this.TrailName.Location = new System.Drawing.Point(92, 2);
            this.TrailName.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.TrailName.MaxLength = 32767;
            this.TrailName.Multiline = false;
            this.TrailName.Name = "TrailName";
            this.TrailName.ReadOnly = false;
            this.TrailName.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.TrailName.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.TrailName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.TrailName.Size = new System.Drawing.Size(359, 19);
            this.TrailName.TabIndex = 1;
            this.TrailName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.TrailName.LostFocus += new System.EventHandler(this.TrailName_LostFocus);
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
            this.btnAdd.Location = new System.Drawing.Point(479, 5);
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
            this.btnEdit.Location = new System.Drawing.Point(504, 5);
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
            this.btnDelete.Location = new System.Drawing.Point(529, 5);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Padding = new System.Windows.Forms.Padding(2);
            this.btnDelete.PushStyle = true;
            this.btnDelete.RightImage = null;
            this.btnDelete.Size = new System.Drawing.Size(19, 19);
            this.btnDelete.TabIndex = 13;
            this.btnDelete.Text = "D";
            this.btnDelete.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnDelete.TextLeftMargin = 2;
            this.btnDelete.TextRightMargin = 2;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblRadius
            // 
            this.lblRadius.AutoSize = true;
            this.lblRadius.Location = new System.Drawing.Point(1, 30);
            this.lblRadius.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lblRadius.Name = "lblRadius";
            this.lblRadius.Size = new System.Drawing.Size(43, 13);
            this.lblRadius.TabIndex = 19;
            this.lblRadius.Text = "Radius:";
            this.lblRadius.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Radius
            // 
            this.radiusBox.AcceptsReturn = false;
            this.radiusBox.AcceptsTab = false;
            this.radiusBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radiusBox.BackColor = System.Drawing.Color.White;
            this.radiusBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.radiusBox.ButtonImage = null;
            this.radiusBox.Location = new System.Drawing.Point(92, 27);
            this.radiusBox.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.radiusBox.MaxLength = 32767;
            this.radiusBox.Multiline = false;
            this.radiusBox.Name = "Radius";
            this.radiusBox.ReadOnly = false;
            this.radiusBox.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.radiusBox.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.radiusBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.radiusBox.Size = new System.Drawing.Size(359, 19);
            this.radiusBox.TabIndex = 20;
            this.radiusBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.radiusBox.LostFocus += new System.EventHandler(this.Radius_LostFocus);
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
            this.btnUp.Location = new System.Drawing.Point(504, 30);
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
            this.btnDown.Location = new System.Drawing.Point(529, 30);
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
            // EList
            // 
            this.EList.AutoScroll = true;
            this.EList.BackColor = System.Drawing.Color.Transparent;
            this.EList.Border = ZoneFiveSoftware.Common.Visuals.ControlBorder.Style.SmallRoundShadow;
            this.EList.CheckBoxes = false;
            this.Panel.SetColumnSpan(this.EList, 7);
            this.EList.Controls.Add(this.editBox);
            this.EList.DefaultIndent = 15;
            this.EList.DefaultRowHeight = -1;
            this.EList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EList.HeaderRowHeight = 21;
            this.EList.Location = new System.Drawing.Point(4, 55);
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
            this.EList.Size = new System.Drawing.Size(569, 233);
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
            this.editBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.editBox_KeyDown);
            this.editBox.LostFocus += new System.EventHandler(this.editBox_LostFocus);
            // 
            // EditTrail
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(555, 325);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.numericSortPrio);
            this.Controls.Add(this.chkCompleteActivity);
            this.Controls.Add(this.btnReverse);
            this.Controls.Add(this.chkName);
            this.Controls.Add(this.chkTwoWay);
            this.Controls.Add(this.chkTemporaryTrail);
            this.Controls.Add(this.chkURFilter);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.Panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditTrail";
            this.Text = "EditTrail";
            this.Activated += new System.EventHandler(this.EditTrail_Activated);
            this.Shown += new System.EventHandler(this.EditTrail_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.numericSortPrio)).EndInit();
            this.Panel.ResumeLayout(false);
            this.Panel.PerformLayout();
            this.EList.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private ZoneFiveSoftware.Common.Visuals.Button btnOk;
        private ZoneFiveSoftware.Common.Visuals.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel Panel;
        private System.Windows.Forms.Label lblTrail;
        private ZoneFiveSoftware.Common.Visuals.Button btnAdd;
        private ZoneFiveSoftware.Common.Visuals.TextBox TrailName;
        private ZoneFiveSoftware.Common.Visuals.Button btnDelete;
        private ZoneFiveSoftware.Common.Visuals.Button btnEdit;
        private ZoneFiveSoftware.Common.Visuals.TreeList EList;
        private ZoneFiveSoftware.Common.Visuals.TextBox radiusBox;
        private System.Windows.Forms.Label lblRadius;
        private ZoneFiveSoftware.Common.Visuals.TextBox editBox;
        private ZoneFiveSoftware.Common.Visuals.Button btnUp;
        private ZoneFiveSoftware.Common.Visuals.Button btnDown;
        private ZoneFiveSoftware.Common.Visuals.Button btnCopy;
        private ZoneFiveSoftware.Common.Visuals.Button btnExport;
        private ZoneFiveSoftware.Common.Visuals.Button btnRefresh;
        private System.Windows.Forms.CheckBox chkTemporaryTrail;
        private System.Windows.Forms.CheckBox chkTwoWay;
        //private System.Windows.Forms.CheckBox chkAutoTryAll;
        private System.Windows.Forms.CheckBox chkName;
        private ZoneFiveSoftware.Common.Visuals.Button btnReverse;
        private System.Windows.Forms.CheckBox chkCompleteActivity;
        private System.Windows.Forms.CheckBox chkURFilter;
        private System.Windows.Forms.NumericUpDown numericSortPrio;

    }
}