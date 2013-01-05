/*
Copyright (C) 2012 Gerhard Olsson

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

namespace TrailsPlugin.Utils
{
    public static class ColorUtil
    {
        public static readonly IDictionary<LineChartTypes, ChartColors> ChartColor = new Dictionary<LineChartTypes, ChartColors>()
            {
                //Aproximate same as ST colors
                {LineChartTypes.Speed, new ChartColors(Color.FromArgb(0x20, 0x4A, 0x87), Color.FromArgb(0x78, 0xC6, 0xCD, 0xD8), Color.FromArgb(0xc8, 0x94, 0xA7, 0xC2))},
                {LineChartTypes.Pace, new ChartColors(Color.FromArgb(0x20, 0x4A, 0x87), Color.FromArgb(0x78, 0xC6, 0xCD, 0xD8), Color.FromArgb(0xc8, 0x94, 0xA7, 0xC2))},
                {LineChartTypes.Elevation, new ChartColors(Color.FromArgb(0x8F, 0x59, 0x02), Color.FromArgb(0x78, 0xE3, 0xD4, 0xBB), Color.FromArgb(0xc8, 0xC8, 0xAE, 0x83))},
                {LineChartTypes.Grade, new ChartColors(Color.FromArgb(0xC1, 0x7D, 0x11), Color.FromArgb(0x78, 0xEE, 0xDC, 0xBF), Color.FromArgb(0xc8, 0xE0, 0xBF, 0x8A))},
                {LineChartTypes.HeartRateBPM, new ChartColors(/*Red*/ Color.FromArgb(0xCC, 0x00, 0x00), Color.FromArgb(0x78, 0xF1, 0xBF, 0xBB), Color.FromArgb(0xc8, 0xE5, 0x84, 0x82))},
                {LineChartTypes.Cadence, new ChartColors(Color.FromArgb(0x4E, 0x9A, 0x06), Color.FromArgb(0x78, 0xD3, 0xE3, 0xBC), Color.FromArgb(0xc8, 0xD3, 0xE3, 0xBC))},
                {LineChartTypes.Power, new ChartColors(Color.FromArgb(0x5C, 0x35, 0x66), Color.FromArgb(0x78, 0xD7, 0xCB, 0xD3), Color.FromArgb(0xc8, 0xB0, 0x9D, 0xB2))},
                //Private
                {LineChartTypes.DiffTime, new ChartColors(/*DarkCyan*/ Color.FromArgb(0x00, 0x8B, 0x8B), Color.FromArgb(0x78, 0x89, 0xE9, 0xFF), Color.FromArgb(0xc8, 0x4C, 0xDE, 0xFF))},
                {LineChartTypes.DiffDist, new ChartColors(/*Color.CornflowerBlue*/ Color.FromArgb(0x64, 0x95, 0xED), Color.FromArgb(0x78, 0x89, 0xE9, 0xFF), Color.FromArgb(0xc8, 0x4C, 0xDE, 0xFF))},
                //Device, slightly red
                {LineChartTypes.DeviceSpeed, new ChartColors(Color.FromArgb(0x3E, 0x41, 0x7A), Color.FromArgb(0x78, 0xC6, 0xCD, 0xD8), Color.FromArgb(0xc8, 0x94, 0xA7, 0xC2))},
                {LineChartTypes.DevicePace, new ChartColors(Color.FromArgb(0x3E, 0x41, 0x7A), Color.FromArgb(0x78, 0xC6, 0xCD, 0xD8), Color.FromArgb(0xc8, 0x94, 0xA7, 0xC2))}, 
                {LineChartTypes.DeviceElevation, new ChartColors(Color.FromArgb(0xB7, 0x46, 0x02), Color.FromArgb(0x78, 0xE3, 0xD4, 0xBB), Color.FromArgb(0xc8, 0xC8, 0xAE, 0x83))},
                {LineChartTypes.DeviceDiffDist, new ChartColors(Color.FromArgb(0x00, 0x8B, 0x8B), Color.FromArgb(0x78, 0x89, 0xE9, 0xFF), Color.FromArgb(0xc8, 0x4C, 0xDE, 0xFF))},
            };

        public static readonly IDictionary<int, ChartColors> ResultColor = new Dictionary<int, ChartColors>()
            {
                { 0, new ChartColors(Color.Blue) },
                { 1, new ChartColors(Color.Red) },
                { 2, new ChartColors(Color.Green) },
                { 3, new ChartColors(Color.Orange) },
                { 4, new ChartColors(Color.Plum) },
                { 5, new ChartColors(Color.HotPink) },
                { 6, new ChartColors(Color.Gold) },
                { 7, new ChartColors(Color.Silver) },
                { 8, new ChartColors(Color.YellowGreen) },
                { 9, new ChartColors(Color.Turquoise) }
            };
        public static readonly ChartColors SummaryColor = new ChartColors(Color.Black);

    }

    public class ChartColors
    {
        public ChartColors(Color LineNormal, Color FillNormal, Color FillSelected)
        {
            this.LineNormal = LineNormal;
            this.FillNormal = FillNormal;
            this.FillSelected = FillSelected;
        }
        public ChartColors(Color LineNormal)
        {
            this.LineNormal = LineNormal;
            this.FillNormal = Color.FromArgb(0x78, 200, 200, 205);
            this.FillSelected = Color.FromArgb(0xc8, 197, 197, 197);
        }
        public Color LineNormal;
        public Color FillNormal;
        public Color FillSelected;
    }
}
