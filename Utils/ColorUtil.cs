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
                //The colors are on the border of "color circle" to get most contrast
                //Some brighter values darkend to get better visibility without decreasing contrast too much
                //transparency lowered as many lines can be layered
                { 6,  new ChartColors(Color.FromArgb(0xff, 0x88, 0x88, 0x88), Color.FromArgb(0x48, 0xdd, 0xdd, 0xdd),Color.FromArgb(0xc8, 0xbb, 0xbb, 0xbb)) },
                { 1,  new ChartColors(Color.FromArgb(0xff, 0xff, 0x00, 0x00), Color.FromArgb(0x48, 0xf0, 0xc0, 0xc0),Color.FromArgb(0xc8, 0xf0, 0xa0, 0xa0)) },
                { 9,  new ChartColors(Color.FromArgb(0xff, 0xff, 0x6a, 0x00), Color.FromArgb(0x48, 0xf0, 0xd0, 0x90),Color.FromArgb(0xc8, 0xf0, 0xb0, 0x70)) },
                { 3,  new ChartColors(Color.FromArgb(0xff, 0xe8, 0xc9, 0x00), Color.FromArgb(0x48, 0xee, 0xdd, 0xcc),Color.FromArgb(0xc8, 0xff, 0xee, 0x77)) },
                { 12, new ChartColors(Color.FromArgb(0xff, 0xb2, 0xe1, 0x00), Color.FromArgb(0x48, 0xdd, 0xff, 0xcc),Color.FromArgb(0xc8, 0xcc, 0xff, 0x77)) },
                { 7,  new ChartColors(Color.FromArgb(0xff, 0x4c, 0xf0, 0x00), Color.FromArgb(0x48, 0xcc, 0xff, 0xcc),Color.FromArgb(0xc8, 0x99, 0xff, 0x77)) },
                { 0,  new ChartColors(Color.FromArgb(0xff, 0x00, 0xc2, 0x21), Color.FromArgb(0x48, 0xcc, 0xdd, 0xcc),Color.FromArgb(0xc8, 0x77, 0xee, 0x88)) },
                { 4,  new ChartColors(Color.FromArgb(0xff, 0x00, 0xf0, 0x8a), Color.FromArgb(0x48, 0xcc, 0xff, 0xdd),Color.FromArgb(0xc8, 0x77, 0xff, 0x99)) },
                { 13, new ChartColors(Color.FromArgb(0xff, 0x00, 0xf0, 0xf0), Color.FromArgb(0x48, 0xcc, 0xff, 0xff),Color.FromArgb(0xc8, 0x77, 0xff, 0xff)) },
                { 10, new ChartColors(Color.FromArgb(0xff, 0x00, 0x94, 0xff), Color.FromArgb(0x48, 0xcc, 0xdd, 0xff),Color.FromArgb(0xc8, 0x77, 0xcc, 0xff)) },
                { 2,  new ChartColors(Color.FromArgb(0xff, 0x00, 0x26, 0xff), Color.FromArgb(0x48, 0xcc, 0xdd, 0xff),Color.FromArgb(0xc8, 0x77, 0x88, 0xff)) },
                { 14, new ChartColors(Color.FromArgb(0xff, 0x48, 0x00, 0xff), Color.FromArgb(0x48, 0xcc, 0xcc, 0xff),Color.FromArgb(0xc8, 0x99, 0x77, 0xff)) },
                { 8,  new ChartColors(Color.FromArgb(0xff, 0xb2, 0x00, 0xff), Color.FromArgb(0x48, 0xdd, 0xcc, 0xff),Color.FromArgb(0xc8, 0xee, 0x77, 0xff)) },
                { 5,  new ChartColors(Color.FromArgb(0xff, 0xff, 0x00, 0xdc), Color.FromArgb(0x48, 0xff, 0xcc, 0xee),Color.FromArgb(0xc8, 0xff, 0x77, 0xee)) },
                { 11, new ChartColors(Color.FromArgb(0xff, 0xff, 0x00, 0x6e), Color.FromArgb(0x48, 0xff, 0xcc, 0xdd),Color.FromArgb(0xc8, 0xff, 0x77, 0xaa)) },
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
