/*
Copyright (C) 2011 Gerhard Olsson

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
using ZoneFiveSoftware.Common.Visuals;
using System.Drawing;
using GpsRunningPlugin.Util;

namespace TrailsPlugin.Data
{
    public class SummaryRowDataRenderer : TreeList.DefaultRowDataRenderer
    {
        public SummaryRowDataRenderer(TreeList list)
            : base(list)
        {
        }

        protected override TreeList.DefaultRowDataRenderer.RowDecoration GetRowDecoration(object element)
        {
            if (element is TrailResultWrapper && (element as TrailResultWrapper).IsSummary)
            {
                return RowDecoration.BottomLineSingle;
            }
            return base.GetRowDecoration(element);
        }

        protected override System.Drawing.FontStyle GetCellFontStyle(object element, TreeList.Column column)
        {
            if (element is TrailResultWrapper)
            {
                TrailResultWrapper wrapper = (element as TrailResultWrapper);
                if (wrapper.IsSummary)
                {
                    return System.Drawing.FontStyle.Bold;
                }
                else
                {
                    /*Controller.TrailController controller = Controller.TrailController.Instance;
                    if (controller != null && controller.CurrentActivityTrailNoChecks != null && 
                        controller.CurrentActivityTrailNoChecks.ResultTreeList.Count > 1 && 
                        wrapper.Result.Equals(controller.ReferenceTrailResultNoChecks))
                    {
                        return System.Drawing.FontStyle.Italic;
                    }
                     * */
                }
            }
            return base.GetCellFontStyle(element, column);
        }
    }
}
