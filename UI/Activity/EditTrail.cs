/******************************************************************************

    This file is part of TrailsPlugin.

    TrailsPlugin is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TrailsPlugin is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TrailsPlugin.  If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Data.Measurement;

namespace TrailsPlugin.UI.Activity {
	public partial class EditTrail : Form {

		protected ITheme m_visualTheme;
		protected bool m_addMode;
		protected Data.Trail m_TrailToEdit;

		public EditTrail(ITheme visualTheme, bool addMode) {
			if (addMode) {
				m_TrailToEdit = new TrailsPlugin.Data.Trail();
				this.Name = Properties.Resources.UI_Activity_EditTrail_AddTrail;
			} else {
				m_TrailToEdit = Controller.TrailController.Instance.CurrentActivityTrail.Trail;
				this.Name = Properties.Resources.UI_Activity_EditTrail_EditTrail;
			}
			m_addMode = addMode;
			InitializeComponent();
			ThemeChanged(visualTheme);
		}

		public virtual void ThemeChanged(ITheme visualTheme) {
            presentRadius();
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
				MessageBox.Show(Properties.Resources.UI_Activity_EditTrail_TrailNameReqiured);
				return;
			}
			Data.Trail trail = null;
			if (m_addMode) {
				if (PluginMain.Data.AllTrails.ContainsKey(TrailName.Text)) {
					MessageBox.Show(Properties.Resources.UI_Activity_EditTrail_UniqueTrailNameRequired);
					return;
				}
			} else {
				if (PluginMain.Data.AllTrails.TryGetValue(TrailName.Text, out trail)) {
					if (trail != m_TrailToEdit) {
						MessageBox.Show(Properties.Resources.UI_Activity_EditTrail_UniqueTrailNameRequired);
						return;
					}
				}
			}
			string oldTrailName = m_TrailToEdit.Name;
			m_TrailToEdit.Name = TrailName.Text;
            //Radius handled on LostFocus
            if (this.m_addMode)
            {
				if (!Controller.TrailController.Instance.AddTrail(m_TrailToEdit)) {
					MessageBox.Show(Properties.Resources.UI_Activity_EditTrail_InsertFailed);
					return;
				}
			} else {
				if (!Controller.TrailController.Instance.UpdateTrail(m_TrailToEdit)) {
					MessageBox.Show(Properties.Resources.UI_Activity_EditTrail_UpdateFailed);
					return;
				}
			}
			this.DialogResult = DialogResult.OK;
			Close();
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

		private void EditTrail_Shown(object sender, System.EventArgs e) {
			List.Columns.Clear();
            List.Columns.Add(new TreeList.Column("LongitudeDegrees", Properties.Resources.UI_Activity_EditTrail_Longitude, 100, StringAlignment.Near));
            List.Columns.Add(new TreeList.Column("LatitudeDegrees", Properties.Resources.UI_Activity_EditTrail_Latitude, 100, StringAlignment.Near));
            //Add when list is editable
            //List.Columns.Add(new TreeList.Column("Name", CommonResources.Text.LabelName, 100, StringAlignment.Near));
            List.RowData = m_TrailToEdit.TrailLocations;
            
			TrailName.Text = m_TrailToEdit.Name;
			lblRadius.Text = Properties.Resources.UI_Activity_EditTrail_Radius+" :";
            presentRadius();
		}
        private void presentRadius()
        {
            Radius.Text = Utils.Units.ElevationToString(m_TrailToEdit.Radius, "u");
        }
        private void Radius_LostFocus(object sender, System.EventArgs e)
        {
            float result;
            result = Utils.Units.ParseElevation(Radius.Text);
            if (result > 0)
            {
                m_TrailToEdit.Radius = result;
            }
            else
            {
                MessageBox.Show(Properties.Resources.UI_Activity_EditTrail_RadiusNumeric);
            }
            presentRadius();
        }
	}
}
