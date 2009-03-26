using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ZoneFiveSoftware.Common.Visuals;

namespace TrailsPlugin.UI.Activity {
	public partial class ChartsControl : UserControl {

		public event System.EventHandler Collapse;

		public ChartsControl() {
			InitializeComponent();

			btnCollapse.CenterImage = CommonIcons.LowerLeft;
			btnCollapse.Text = "";
			btnCollapse.Left = this.Right - 46;
			btnCollapse.Top = 2;

		}

		public void ThemeChanged(ITheme visualTheme) {
			this.ChartBanner.ThemeChanged(visualTheme);
		}

		private void btnCollapse_Click(object sender, EventArgs e) {
			Collapse(sender, e);
		}
	}
}
