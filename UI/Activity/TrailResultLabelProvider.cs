﻿/******************************************************************************

    This file is part of TrailsPlugin.

    TrailsPlugin is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TrailsPlugin is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TrailsPlugin.  If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using ZoneFiveSoftware.Common.Visuals;
using System.Drawing;

namespace TrailsPlugin.UI.Activity {
	class TrailResultLabelProvider : TreeList.ILabelProvider {

		#region ILabelProvider Members

		public Image GetImage(object element, TreeList.Column column) {
			return null;
		}
		
		public string GetText(object element, TreeList.Column column) {

			Data.TrailResult row = (Data.TrailResult)element;
			switch (column.Id) {
				case TrailResultColumnIds.Order:
					return row.Order.ToString();
				case TrailResultColumnIds.StartTime:
					return row.StartTime.ToString();
				case TrailResultColumnIds.EndTime:
					return row.EndTime.ToString();
				case TrailResultColumnIds.Duration:
					return Utils.Text.ToString(row.Duration);
				case TrailResultColumnIds.Distance:
					return row.Distance.ToString("0.00");
				case TrailResultColumnIds.AvgCadence:
					return row.AvgCadence.ToString("0.0");
				case TrailResultColumnIds.AvgHR:
					return row.AvgHR.ToString("0");
				case TrailResultColumnIds.MaxHR:
					return row.MaxHR.ToString("0");
				case TrailResultColumnIds.ElevChg:
					int change = (int)row.ElevChg;
					return (change > 0 ? "+" : "") + change.ToString();
				default:
					return "error";
			}			
		}

		#endregion
	}
}
