/*
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
