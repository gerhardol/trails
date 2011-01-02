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
using System.Collections.Generic;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace TrailsPlugin.Data {
    public class TrailResultMarked
    {
        //Mark all
        public TrailResultMarked(TrailResult tr)
        {
            trailResult = tr;
            IValueRangeSeries<DateTime> t = new ValueRangeSeries<DateTime>();
            t.Add(new ValueRange<DateTime>(tr.FirstTime, tr.LastTime));
            selInfo.MarkedTimes = t;
        }
        public TrailResultMarked(TrailResult tr, IValueRangeSeries<DateTime> t)
        {
            trailResult = tr;
            selInfo.MarkedTimes = t;
        }
        public TrailResultMarked(TrailResult tr, IValueRangeSeries<double> t)
        {
            trailResult = tr;
            selInfo.MarkedDistances = t;
        }
        public TrailResultMarked(TrailResult tr, IItemTrackSelectionInfo t)
        {
            trailResult = tr;
            selInfo.SetFromSelection(t);
        }
        public TrailResult trailResult;
        public Data.TrailsItemTrackSelectionInfo selInfo = new Data.TrailsItemTrackSelectionInfo();

        public static IList<TrailResultMarked> TrailResultMarkAll(IList<TrailResult> atr)
        {
            IList<TrailResultMarked> result = new List<TrailResultMarked>();
            foreach (TrailResult tr in atr)
            {
                result.Add(new TrailResultMarked(tr));
            }
            return result;
        }
        public static Data.TrailsItemTrackSelectionInfo SelInfoUnion(IList<TrailResultMarked> atrm)
        {
            Data.TrailsItemTrackSelectionInfo result = new Data.TrailsItemTrackSelectionInfo();
            foreach (TrailResultMarked trm in atrm)
            {
                result.Union(trm.selInfo);
            }
            return result;
        }
        public static IList<TrailResult> getTrailResult(IList<TrailResultMarked> atr)
        {
            IList<TrailResult> trr = new List<TrailResult>();
            foreach (TrailResultMarked trm in atr)
            {
                trr.Add(trm.trailResult);
            }
            return trr;
        }
    }
}
