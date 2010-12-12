/*
Copyright (C) 2010 Gerhard Olsson

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
using ZoneFiveSoftware.Common.Visuals;
using System.Drawing;

namespace TrailsPlugin.Data {
    class TrailResultLabelProvider : TreeList.DefaultLabelProvider
    {

        private bool m_multiple = false;
        public bool MultipleActivities
        {
            set { m_multiple = value; }
        }
		#region ILabelProvider Members

        public override Image GetImage(object element, TreeList.Column column)
        {
            Data.TrailResult row = TrailsPlugin.UI.Activity.ResultListControl.getTrailResultRow(element);

            if (column.Id == "Color")
            {
                Bitmap image = new Bitmap(column.Width, 15);
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        image.SetPixel(x, y, row.TrailColor);
                    }
                }
                return image;
            }
            else
            {
                return base.GetImage(row.Activity, column);
            }
        }

        public override string GetText(object element, TreeList.Column column)
        {
            Data.TrailResult row = TrailsPlugin.UI.Activity.ResultListControl.getTrailResultRow(element);
            switch (column.Id)
            {
				case TrailResultColumnIds.Order:
					return row.Order.ToString();
                case TrailResultColumnIds.Color:
                    return null;
				case TrailResultColumnIds.StartTime:
                    string date = "";
                    if (m_multiple)
                    {
                        date = row.FirstTime.ToLocalTime().ToShortDateString()+" ";
                    }
                    return date + row.StartTime.ToString();
				case TrailResultColumnIds.EndTime:
					return row.EndTime.ToString();
				case TrailResultColumnIds.Duration:
					return Utils.Text.ToString(row.Duration);
				case TrailResultColumnIds.Distance:
					return row.Distance;
				case TrailResultColumnIds.AvgCadence:
					return row.AvgCadence.ToString("0.0");
				case TrailResultColumnIds.AvgHR:
					return row.AvgHR.ToString("0");
				case TrailResultColumnIds.MaxHR:
					return row.MaxHR.ToString("0");
				case TrailResultColumnIds.ElevChg:
					return row.ElevChg;
				case TrailResultColumnIds.AvgPower:
					return row.AvgPower.ToString("0.0");
				case TrailResultColumnIds.AvgGrade:
					return (row.AvgGrade).ToString("0.0%");
				case TrailResultColumnIds.AvgSpeed:
					return row.AvgSpeed.ToString("0.0");
				case TrailResultColumnIds.FastestSpeed:
					return row.FastestSpeed.ToString("0.0");
				case TrailResultColumnIds.AvgPace:
                    return Utils.Text.ToString(row.AvgPace);					
				case TrailResultColumnIds.FastestPace:
					return Utils.Text.ToString(row.FastestPace);
                case TrailResultColumnIds.Name:
                    return row.Name;
                default:
                    return base.GetText(row.Activity, column);
            }			
		}

		#endregion
	}
}
