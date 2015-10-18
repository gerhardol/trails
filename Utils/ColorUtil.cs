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
            //Approximate same as ST colors, using Paint.NET to pick
            //For fill colors the alpha values are assumed, then Fill adjusted to the background (0xFCFAF5)
            //Calculate with alpha blending:
            //original R = (resulting R - ((1-opacity)*background R)) / opacity. (0xc6 - (255-0x78)*0xfc/255)*255/0x78
            //perl -e '@a=(0x78,0xc8);@b=(fc, fa, f5);$n=0;for $i (@ARGV){@q= $i=~/(\w\w)(\w\w)(\w\w)/;printf " Color.FromArgb(0x%x", $a[$n%2]; $m=0; for $j(@q){$h=(hex($j)-(255-$a[$n%2])*hex($b[$m%3])/255)*255/$a[$n%2]; printf ", 0x%x",$h;}$n++; print "),"; if ($n==0){print "\n";}}' F8C6D6  F389B2 CFCDC9 9B9A97 FAE8C1  F7D384 F8C6D6 F389B2 CBECDD  92DDC2 FDD3C9 FEA597

            //(SelectColors are slightly off, could be separate. Change alpha?)
            //Chart MeasuredFill MeasuredSelect, ActualFill(?) ActualSelect(?)
            //Speed  0xC6CDD8  0x94A7C2  C5CCD8 A1AFC5
            //Elevation 0xE3D4BB  0xC8AE83 E2D4BB D1BB94
            //Grade 0xEEDCBF  0xE0BF8A  EDDCBF E4C89B
            //HR 0xF1BFBB  0xE58482  F0BEBB E99794
            //Cadence 0xD3E3BC  0xD3E3BC D2E3BC B7D495
            //Power 0xD7CBD3 0xB09DB2  D6CBD3  BEACBC 

            // MaxPulse CC0000
            //Line, line chart, chart fill, chart select
        //LeftPower,  EA1C72 EA1D74  F8C6D6  F389B2
        //Temperature,  3C3C3C 3C3C3C CFCDC9 9B9A97
        //GroundContactTime, F2AD18 F4B42E  FAE8C1  F7D384
        //VerticalOscillation, EA1C72  EA1C72  F8C6D6 F389B2
        //SaturatedHemoglobin,  2CC192 2DC293 CBECDD  92DDC2
        //TotalHemoglobinConcentration,   FF533D FF533D FDD3C9 FEA597

                {LineChartTypes.Speed,                        new ChartColors(Color.FromArgb(0x20, 0x4A, 0x87), Color.FromArgb(0x78, 0x89, 0x9a, 0xb7), Color.FromArgb(0xc8, 0x89, 0x9a, 0xb7))},
                {LineChartTypes.Pace,                         new ChartColors(Color.FromArgb(0x20, 0x4A, 0x87), Color.FromArgb(0x78, 0x89, 0x9a, 0xb7), Color.FromArgb(0xc8, 0x89, 0x9a, 0xb7))},
                {LineChartTypes.Elevation,                    new ChartColors(Color.FromArgb(0x8F, 0x59, 0x02), Color.FromArgb(0x78, 0xc6, 0xa9, 0x79), Color.FromArgb(0xc8, 0xc6, 0xa9, 0x79))},
                {LineChartTypes.Grade,                        new ChartColors(Color.FromArgb(0xC1, 0x7D, 0x11), Color.FromArgb(0x78, 0xde, 0xba, 0x82), Color.FromArgb(0xc8, 0xde, 0xba, 0x82))},
                {LineChartTypes.HeartRateBPM,                 new ChartColors(Color.FromArgb(0xCC, 0x00, 0x00), Color.FromArgb(0x78, 0xe4, 0x7c, 0x79), Color.FromArgb(0xc8, 0xe4, 0x7c, 0x79))},
                {LineChartTypes.Cadence,                      new ChartColors(Color.FromArgb(0x4E, 0x9A, 0x06), Color.FromArgb(0x78, 0xa4, 0xc9, 0x7b), Color.FromArgb(0xc8, 0xa4, 0xc9, 0x7b))},
                {LineChartTypes.Power,                        new ChartColors(Color.FromArgb(0x5C, 0x35, 0x66), Color.FromArgb(0x78, 0xad, 0x96, 0xac), Color.FromArgb(0xc8, 0xad, 0x96, 0xac))},
                
                {LineChartTypes.PowerBalance,                    new ChartColors(Color.FromArgb(0xea, 0x1c, 0x72), Color.FromArgb(0x78, 0xf3, 0x89, 0xab), Color.FromArgb(0xc8, 0xf0, 0x69, 0x9d))},
                {LineChartTypes.Temperature,                  new ChartColors(Color.FromArgb(0x3c, 0x3c, 0x3c), Color.FromArgb(0x78, 0x9c, 0x98, 0x8f), Color.FromArgb(0xc8, 0x80, 0x7f, 0x7b))},
                {LineChartTypes.GroundContactTime,            new ChartColors(Color.FromArgb(0xf2, 0xad, 0x18), Color.FromArgb(0x78, 0xf7, 0xd1, 0x7e), Color.FromArgb(0xc8, 0xf5, 0xc7, 0x63))},
                {LineChartTypes.VerticalOscillation,          new ChartColors(Color.FromArgb(0xea, 0x1c, 0x72), Color.FromArgb(0x78, 0xf3, 0x89, 0xab), Color.FromArgb(0xc8, 0xf0, 0x69, 0x9d))},
                {LineChartTypes.SaturatedHemoglobin,          new ChartColors(Color.FromArgb(0x2c, 0xc1, 0x92), Color.FromArgb(0x78, 0x93, 0xda, 0xba), Color.FromArgb(0xc8, 0x74, 0xd4, 0xb2))},
                {LineChartTypes.TotalHemoglobinConcentration, new ChartColors(Color.FromArgb(0xff, 0x53, 0x3d), Color.FromArgb(0x78, 0xfe, 0xa4, 0x8f), Color.FromArgb(0xc8, 0xfe, 0x8d, 0x7b))},
                //Private
                {LineChartTypes.DiffTime,        new ChartColors(/*DarkCyan*/ Color.FromArgb(0x00, 0x8B, 0x8B), Color.FromArgb(0x78, 0x89, 0xE9, 0xFF), Color.FromArgb(0xc8, 0x4C, 0xDE, 0xFF))},
                {LineChartTypes.DiffDist,  new ChartColors(/*CornflowerBlue*/ Color.FromArgb(0x64, 0x95, 0xED), Color.FromArgb(0x78, 0x89, 0xE9, 0xFF), Color.FromArgb(0xc8, 0x4C, 0xDE, 0xFF))},
                //Device, slightly red
                {LineChartTypes.DeviceSpeed,                  new ChartColors(Color.FromArgb(0x3E, 0x41, 0x7A), Color.FromArgb(0x78, 0xC6, 0xCD, 0xD8), Color.FromArgb(0xc8, 0x94, 0xA7, 0xC2))},
                {LineChartTypes.DevicePace,                   new ChartColors(Color.FromArgb(0x3E, 0x41, 0x7A), Color.FromArgb(0x78, 0xC6, 0xCD, 0xD8), Color.FromArgb(0xc8, 0x94, 0xA7, 0xC2))},
                {LineChartTypes.DeviceElevation,              new ChartColors(Color.FromArgb(0xB7, 0x46, 0x02), Color.FromArgb(0x78, 0xE3, 0xD4, 0xBB), Color.FromArgb(0xc8, 0xC8, 0xAE, 0x83))},
                {LineChartTypes.DeviceDiffDist,               new ChartColors(Color.FromArgb(0x00, 0x8B, 0x8B), Color.FromArgb(0x78, 0x89, 0xE9, 0xFF), Color.FromArgb(0xc8, 0x4C, 0xDE, 0xFF))},
            };

        public static readonly IDictionary<int, ChartColors> ResultColor = new Dictionary<int, ChartColors>()
            {
                //The colors are on the border of "color circle" to get most contrast
                //Some brighter values darkend to get better visibility without decreasing contrast too much
                //transparency lowered as many lines can be layered
                { 6,  new ChartColors(Color.FromArgb(0xff, 0x88, 0x88, 0x88), Color.FromArgb(0x70, 0xdd, 0xdd, 0xdd),Color.FromArgb(0xc8, 0xbb, 0xbb, 0xbb)) },
                { 1,  new ChartColors(Color.FromArgb(0xff, 0xff, 0x00, 0x00), Color.FromArgb(0x70, 0xf0, 0xc0, 0xc0),Color.FromArgb(0xc8, 0xf0, 0xa0, 0xa0)) },
                { 9,  new ChartColors(Color.FromArgb(0xff, 0xff, 0x6a, 0x00), Color.FromArgb(0x70, 0xf0, 0xd0, 0x90),Color.FromArgb(0xc8, 0xf0, 0xb0, 0x70)) },
                { 3,  new ChartColors(Color.FromArgb(0xff, 0xe8, 0xc9, 0x00), Color.FromArgb(0x70, 0xee, 0xdd, 0xcc),Color.FromArgb(0xc8, 0xff, 0xee, 0x77)) },
                { 12, new ChartColors(Color.FromArgb(0xff, 0xb2, 0xe1, 0x00), Color.FromArgb(0x70, 0xdd, 0xff, 0xcc),Color.FromArgb(0xc8, 0xcc, 0xff, 0x77)) },
                { 7,  new ChartColors(Color.FromArgb(0xff, 0x4c, 0xf0, 0x00), Color.FromArgb(0x70, 0xcc, 0xff, 0xcc),Color.FromArgb(0xc8, 0x99, 0xff, 0x77)) },
                { 0,  new ChartColors(Color.FromArgb(0xff, 0x00, 0xc2, 0x21), Color.FromArgb(0x70, 0xcc, 0xdd, 0xcc),Color.FromArgb(0xc8, 0x77, 0xee, 0x88)) },
                { 4,  new ChartColors(Color.FromArgb(0xff, 0x00, 0xf0, 0x8a), Color.FromArgb(0x70, 0xcc, 0xff, 0xdd),Color.FromArgb(0xc8, 0x77, 0xff, 0x99)) },
                { 13, new ChartColors(Color.FromArgb(0xff, 0x00, 0xf0, 0xf0), Color.FromArgb(0x70, 0xcc, 0xff, 0xff),Color.FromArgb(0xc8, 0x77, 0xff, 0xff)) },
                { 10, new ChartColors(Color.FromArgb(0xff, 0x00, 0x94, 0xff), Color.FromArgb(0x70, 0xcc, 0xdd, 0xff),Color.FromArgb(0xc8, 0x77, 0xcc, 0xff)) },
                { 2,  new ChartColors(Color.FromArgb(0xff, 0x00, 0x26, 0xff), Color.FromArgb(0x70, 0xcc, 0xdd, 0xff),Color.FromArgb(0xc8, 0x77, 0x88, 0xff)) },
                { 14, new ChartColors(Color.FromArgb(0xff, 0x48, 0x00, 0xff), Color.FromArgb(0x70, 0xcc, 0xcc, 0xff),Color.FromArgb(0xc8, 0x99, 0x77, 0xff)) },
                { 8,  new ChartColors(Color.FromArgb(0xff, 0xb2, 0x00, 0xff), Color.FromArgb(0x70, 0xdd, 0xcc, 0xff),Color.FromArgb(0xc8, 0xee, 0x77, 0xff)) },
                { 5,  new ChartColors(Color.FromArgb(0xff, 0xff, 0x00, 0xdc), Color.FromArgb(0x70, 0xff, 0xcc, 0xee),Color.FromArgb(0xc8, 0xff, 0x77, 0xee)) },
                { 11, new ChartColors(Color.FromArgb(0xff, 0xff, 0x00, 0x6e), Color.FromArgb(0x70, 0xff, 0xcc, 0xdd),Color.FromArgb(0xc8, 0xff, 0x77, 0xaa)) },
            };
        public static readonly ChartColors SummaryColor = new ChartColors(Color.Black);

    }

    public class ChartColors : IComparable
    {
        public ChartColors(Color LineNormal, Color FillNormal, Color FillSelected)
        {
            this.LineNormal = LineNormal;
            this.FillNormal = FillNormal;
            this.FillSelected = FillSelected;
        }
        public ChartColors(Color LineNormal)
        {
            //Manually set color
            this.LineNormal = LineNormal;
            //Auto colors look strange, use gray instead
            this.FillNormal = Color.FromArgb(0x78, 200, 200, 205);
            this.FillSelected = Color.FromArgb(0xc8, 197, 197, 197);
        }
        public Color LineNormal;
        public Color FillNormal;
        public Color FillSelected;

        public int CompareTo(object obj)
        {
            if (obj is ChartColors)
            {
                return this.LineNormal.ToArgb().CompareTo((obj as ChartColors).LineNormal.ToArgb());
            }
            else
            {
                return -1;
            }
        }
    }
}
