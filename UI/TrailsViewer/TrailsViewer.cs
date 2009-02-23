using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace TrailsPlugin {
	public partial class TrailsViewer : UserControl {
		IList<IActivity> m_activities;
		public TrailsViewer(IList<IActivity> activities) {
			InitializeComponent();
			m_activities = activities;
			this.List.Columns.Add(new TreeList.Column("aaa", "a", 100, StringAlignment.Near));
			this.List.Columns[0].CanSelect = true;
			this.List.Columns.Add(new TreeList.Column("bbb", "b", 50, StringAlignment.Far));
			this.List.Columns[1].CanSelect = false;
			RefreshData();
		}

		public IActivity[] Activities {
			set {
				m_activities = value;
				RefreshData();
			}
		}

		public void RefreshData() {

			if (m_activities.Count > 0) {
				List<TrailsViewerRow> rows = new List<TrailsViewerRow>();
				TrailsViewerRow row = new TrailsViewerRow(); ;
				row.aaa = "123";
				rows.Add(row);
				row = new TrailsViewerRow(); ;
				row.aaa = "456";
				rows.Add(row);
				this.List.RowDataRenderer = new TreeList.DefaultRowDataRenderer(this.List);
				this.List.RowData = rows;
			} else {

			}

		}

		public void ThemeChanged(ITheme visualTheme) {
			List.ThemeChanged(visualTheme);
		}

		private void TrailsViewer_Load(object sender, EventArgs e) {

		}

		private void List_Load(object sender, EventArgs e) {

		}
	}
}
