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

using System;
using System.Collections.Generic;
using System.Drawing;
using ZoneFiveSoftware.Common.Visuals;
using System.Drawing.Drawing2D;

namespace TrailsPlugin.Utils {
    class Dialog {

        public static void DrawButtonRowBackground(Graphics graphics, Rectangle clientRect, ITheme theme) {
            Color color1 = Color.FromKnownColor(KnownColor.Window);
            Color color2 = Color.FromKnownColor(KnownColor.ControlDark);
            if (theme != null) {
                color1 = theme.Window;
                color2 = theme.Border;
            }
            Rectangle rectangle1 = new Rectangle(0, clientRect.Height - 35, clientRect.Width, 35);
            using (Brush brush1 = new SolidBrush(color1)) {
                graphics.FillRectangle(brush1, rectangle1);
            }
            using (Brush brush2 = new SolidBrush(color2)) {
                graphics.FillRectangle(brush2, new Rectangle(rectangle1.Left, rectangle1.Top, rectangle1.Width, 2));
            }
            Rectangle rectangle2 = new Rectangle((int)((double)rectangle1.Left + ((double)rectangle1.Width * 0.1)) + 1, rectangle1.Top, (int)((double)rectangle1.Width * 0.9), rectangle1.Height);
            using (Brush brush3 = new LinearGradientBrush(rectangle2, Color.Transparent, Color.FromArgb(25, 10, 0, 20), LinearGradientMode.Horizontal)) {
                graphics.FillRectangle(brush3, new Rectangle(rectangle2.Left + 2, rectangle2.Top, rectangle2.Width - 2, rectangle2.Height));
            }
        }
    }
}
