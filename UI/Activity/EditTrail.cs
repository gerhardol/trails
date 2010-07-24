﻿/*
Copyright (C) 2009 Brendan Doherty

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library. If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Data.GPS;
using TrailsPlugin.Data;

namespace TrailsPlugin.UI.Activity {
	public partial class EditTrail : Form {

		protected ITheme m_visualTheme;
		protected bool m_addMode;
		protected Data.Trail m_TrailToEdit;

		public EditTrail(ITheme visualTheme, System.Globalization.CultureInfo culture, bool addMode) {
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
            UICultureChanged(culture);
		}

		public virtual void ThemeChanged(ITheme visualTheme) {
			m_visualTheme = visualTheme;
			this.BackColor = visualTheme.Control;
			EList.ThemeChanged(visualTheme);
            TrailName.ThemeChanged(visualTheme);
            Radius.ThemeChanged(visualTheme);
        }

        public void UICultureChanged(System.Globalization.CultureInfo culture)
        {
            this.lblTrail.Text = Properties.Resources.TrailName;
            lblRadius.Text = Properties.Resources.UI_Activity_EditTrail_Radius + " :";
            this.btnOk.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionOk;
            this.btnCancel.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionCancel;
            presentRadius();
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

        private System.Windows.Forms.TextBox editBox = new System.Windows.Forms.TextBox();
        private MouseEventArgs lastMouseArg = null;
        private string subItemText;
        private int rowSelected = 0;
        private int subItemSelected = 0;
        private void EditTrail_Shown(object sender, System.EventArgs e)
        {
            TrailName.Text = m_TrailToEdit.Name;
            presentRadius();

            EList.Columns.Clear();
            EList.Columns.Add(new TreeList.Column("LongitudeDegrees", Properties.Resources.UI_Activity_EditTrail_Longitude, 100, StringAlignment.Near));
            EList.Columns.Add(new TreeList.Column("LatitudeDegrees", Properties.Resources.UI_Activity_EditTrail_Latitude, 100, StringAlignment.Near));
            EList.Columns.Add(new TreeList.Column("Name", CommonResources.Text.LabelName, 100, StringAlignment.Near));
            EList.RowData = m_TrailToEdit.TrailLocations;

            EList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SMKMouseDown);
            EList.DoubleClick += new System.EventHandler(this.SMKDoubleClick);
            EList.KeyDown += new KeyEventHandler(EList_KeyDown);
            //Overlay editable box
            editBox.Size = new System.Drawing.Size(0, 0);
            editBox.Location = new System.Drawing.Point(0, 0);
            EList.Controls.AddRange(new System.Windows.Forms.Control[] { this.editBox });
            editBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EditOver);
            editBox.LostFocus += new System.EventHandler(this.FocusOver);
            editBox.BackColor = Color.LightYellow;
            editBox.BorderStyle = BorderStyle.Fixed3D;
            editBox.Hide();
            editBox.Text = "";
        }

        void EList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46 && EList.Selected.Count > 0)
            {
                IList t = EList.Selected;
                IList<TrailGPSLocation> result = (IList<TrailGPSLocation>)EList.RowData;
                if (t != null && t.Count > 0)
                {
                    for (int j = 0; j < t.Count; j++)
                    {
                        for (int i = ((IList<TrailGPSLocation>)EList.RowData).Count - 1; i >= 0; i--)
                        {
                            //Only first selected removed now
                            TrailGPSLocation r = (TrailGPSLocation)((IList<TrailGPSLocation>)EList.RowData)[i];
                            if (t[j].Equals(r))
                            {
                                result.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    EList.RowData = result;
                }
            }
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

        private void ValidateEdit()
        {
            IList<TrailGPSLocation> t = (IList<TrailGPSLocation>)EList.RowData;
            t[rowSelected]=
                ((IList<TrailGPSLocation>)EList.RowData)[rowSelected].setField(subItemSelected, editBox.Text);
            EList.RowData = t;
        }
        private void EditOver(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                ValidateEdit();
                editBox.Hide();
            }
            if (e.KeyChar == 27) //Escape
            {
                editBox.Hide();
            }
        }

        private void FocusOver(object sender, System.EventArgs e)
        {
            ValidateEdit();
            editBox.Hide();
        }
        public void SMKDoubleClick(object sender, System.EventArgs e)
        {
            TreeList.RowHitState hitState = TreeList.RowHitState.Nothing;
            TrailGPSLocation t = (TrailGPSLocation)EList.RowHitTest(new Point(lastMouseArg.X, lastMouseArg.Y), out hitState);

            if (t != null && hitState != TreeList.RowHitState.Nothing)
            {
                for (int i = 0; i < ((IList<TrailGPSLocation>)EList.RowData).Count; i++)
                {
                    TrailGPSLocation r = (TrailGPSLocation)((IList<TrailGPSLocation>)EList.RowData)[i];
                    if (t.Equals(r))
                    {
                        rowSelected = i;
                        break;
                    }
                }
                // Check the subitem clicked .
                int nStart = lastMouseArg.X;
                int spos = EList.Location.X + EList.Parent.Location.X;
                int epos = EList.Columns[0].Width;
                for (int i = 0; i < EList.Columns.Count; i++)
                {
                    if (nStart > spos && nStart < epos)
                    {
                        subItemSelected = i;
                        break;
                    }

                    spos = epos;
                    epos += EList.Columns[i].Width;
                }
                subItemText = ((IList<TrailGPSLocation>)EList.RowData)[rowSelected].getField(subItemSelected);
                ///The positioning is incorrect, set at header
                int rowHeight = EList.HeaderRowHeight;// (EList.Height - EList.HeaderRowHeight) / ((IList<TrailGPSLocation>)EList.RowData).Count;
                int yTop = 0;// EList.HeaderRowHeight + rowSelected * rowHeight;
                editBox.Size = new System.Drawing.Size(epos - spos, rowHeight);
                editBox.Location = new System.Drawing.Point(spos, yTop);
                editBox.Show();
                editBox.Text = subItemText;
                editBox.SelectAll();
                editBox.Focus();
            }

        }
        
        public void SMKMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
             lastMouseArg = e;
        }
   	}
}
