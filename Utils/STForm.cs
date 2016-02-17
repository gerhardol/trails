/*
Copyright (C) 2016 Gerhard Olsson

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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZoneFiveSoftware.Common.Visuals;

namespace TrailsPlugin.Utils
{
    //Windows Form with SportTracks Theming
    public class STForm : Form
    {
        ITheme m_visualTheme;

        public STForm(ITheme m_visualTheme, int width, int height)
            : base()
        {
            this.m_visualTheme = m_visualTheme;

            this.Icon = Icon.FromHandle(TrailsPlugin.Properties.Resources.trails.GetHicon());
            this.BackColor = this.m_visualTheme.Control;
            this.Size = new System.Drawing.Size(width, height);

            ZoneFiveSoftware.Common.Visuals.Button okButton = new ZoneFiveSoftware.Common.Visuals.Button();
            ZoneFiveSoftware.Common.Visuals.Button cancelButton = new ZoneFiveSoftware.Common.Visuals.Button();

            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);
            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;

            cancelButton.Location = new System.Drawing.Point(this.Size.Width - 27 - cancelButton.Size.Width, this.Height - 44 - cancelButton.Height);
            cancelButton.DialogResult = DialogResult.Cancel;
            okButton.Location = new System.Drawing.Point(cancelButton.Left - 10 - okButton.Size.Width, this.Height - 44 - okButton.Height);
            okButton.DialogResult = DialogResult.OK;

            okButton.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionOk;
            cancelButton.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionCancel;

            okButton.Click += new EventHandler(btn_Click);
            cancelButton.Click += new EventHandler(btn_Click);
        }

        void btn_Click(object sender, EventArgs e)
        {
            this.DialogResult = ((ZoneFiveSoftware.Common.Visuals.Button)sender).DialogResult;
            Close();
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            ZoneFiveSoftware.Common.Visuals.MessageDialog.DrawButtonRowBackground(e.Graphics, this, m_visualTheme);
        }
    }
}
