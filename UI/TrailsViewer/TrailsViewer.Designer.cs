namespace TrailsPlugin {
	partial class TrailsViewer {
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
			this.List.Location = new System.Drawing.Point(14, 16);
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
			this.List.Load += new System.EventHandler(this.List_Load);
			// 
			// TrailsViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.List);
			this.Name = "TrailsViewer";
			this.Size = new System.Drawing.Size(454, 150);
			this.Load += new System.EventHandler(this.TrailsViewer_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ZoneFiveSoftware.Common.Visuals.TreeList List;

	}
}
