using ZoneFiveSoftware.Common.Visuals.Chart;

namespace TrailsPlugin.UI.Activity {
	class Percent100 : IAxisFormatter {
		private Formatter.Percent m_percent;
		public Percent100() {
			m_percent = new Formatter.Percent();
		}

		#region IAxisFormatter Members

		public string DiffLabel(double value, System.Drawing.Graphics graphics) {
			return m_percent.DiffLabel(value*100, graphics);
		}

		public string TickLabel(double value, System.Drawing.Graphics graphics) {
			return Formatter.Default.TickLabel(value*100, 0);
		}

		public double TickValueSpacing(int minPixelSpacing, double pixelsPerValue) {
			return m_percent.TickValueSpacing(minPixelSpacing, pixelsPerValue);
		}

		public string ValueLabel(double value, System.Drawing.Graphics graphics) {
			return m_percent.ValueLabel(value*100, graphics);
		}

		#endregion
	}
}
