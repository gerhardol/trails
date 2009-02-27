using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using ZoneFiveSoftware.Common.Visuals;

namespace TrailsPlugin.UI.Activity {
	public partial class EditTrail : Form {

		protected ITheme m_visualTheme;
		protected bool m_addMode;
		protected Data.Trail m_TrailToEdit;

		public EditTrail(Data.Trail trailToEdit, ITheme visualTheme) {
			m_TrailToEdit = trailToEdit;
			m_addMode = (trailToEdit == null);
			InitializeComponent();
			ThemeChanged(visualTheme);

			List.Columns.Clear();
			List.Columns.Add(new TreeList.Column("Longitude", "Longitude", 100, StringAlignment.Near));
			List.Columns.Add(new TreeList.Column("Latitude", "Latitude", 100, StringAlignment.Near));
			List.RowData = m_TrailToEdit.points;

			TrailName.Focus();
		}

		public virtual void ThemeChanged(ITheme visualTheme) {
			m_visualTheme = visualTheme;
			BackColor = visualTheme.Control;
			List.ThemeChanged(visualTheme);
			TrailName.ThemeChanged(visualTheme);			
			actionBanner1.ThemeChanged(visualTheme);
		}

		private void btnCancel_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void btnOk_Click(object sender, System.EventArgs e) {
			if (this.m_addMode) {
				Controller.TrailsController.Instance.InsertTrail(m_TrailToEdit);
			} else {
				Controller.TrailsController.Instance.UpdateTrail(m_TrailToEdit);
			}
			Close();
		}


	}
}
