using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using ZoneFiveSoftware.Common.Visuals;

namespace TrailsPlugin.UI.Activity {
	public partial class EditTrail : Form {

		protected ITheme m_visualTheme;
		protected bool m_addMode;
		protected Data.Trail m_TrailToEdit;

		public EditTrail(Data.Trail trailToEdit, ITheme visualTheme, bool addMode) {
			m_TrailToEdit = trailToEdit;
			m_addMode = addMode;
			InitializeComponent();
			ThemeChanged(visualTheme);

			if (m_addMode) {
				this.Name = "Add Trail";
			} else {
				this.Name = "Edit Trail";
			}

			List.Columns.Clear();
			List.Columns.Add(new TreeList.Column("LongitudeDegrees", "Longitude", 100, StringAlignment.Near));
			List.Columns.Add(new TreeList.Column("LatitudeDegrees", "Latitude", 100, StringAlignment.Near));
			List.RowData = m_TrailToEdit.TrailLocations;

			TrailName.Text = m_TrailToEdit.Name;
		}

		public virtual void ThemeChanged(ITheme visualTheme) {
			m_visualTheme = visualTheme;
			BackColor = visualTheme.Control;
			List.ThemeChanged(visualTheme);
			TrailName.ThemeChanged(visualTheme);
		}

		private void btnCancel_Click(object sender, System.EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
			Close();
		}

		private void btnOk_Click(object sender, System.EventArgs e) {
			if (TrailName.Text.Length == 0) {
				MessageBox.Show("Trail name is required.");
				return;
			}
			Data.Trail trail = null;
			if (m_addMode) {
				if (TrailSettings.Instance.AllTrails.ContainsKey(TrailName.Text)) {
					MessageBox.Show("Unique trail name is required.");
					return;
				}
			} else {
				if (TrailSettings.Instance.AllTrails.TryGetValue(TrailName.Text, out trail)) {
					if (trail == m_TrailToEdit) {
						MessageBox.Show("Unique trail name is required.");
						return;
					}
				}
			}

			m_TrailToEdit.Name = TrailName.Text;
			if (this.m_addMode) {
				TrailSettings.Instance.InsertTrail(m_TrailToEdit);
			} else {
				TrailSettings.Instance.UpdateTrail(m_TrailToEdit);
			}
			this.DialogResult = DialogResult.OK;
			Close();
		}

		private void TrailName_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			m_TrailToEdit.Name = TrailName.Text;
		}

		private void EditTrail_Activated(object sender, System.EventArgs e) {
			TrailName.Focus();

		}

		public Data.Trail Trail {
			get {
				return m_TrailToEdit;
			}
		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
			base.OnPaint(e);
			Utils.Dialog.DrawButtonRowBackground(e.Graphics, ClientRectangle, m_visualTheme);
		}
	}
}
