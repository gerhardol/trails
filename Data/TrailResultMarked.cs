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
            this.trailResult = tr;
            this.selInfo.MarkedTimes = tr.getSelInfo(true);
            this.selInfo.Activity = tr.Activity;
        }

        public TrailResultMarked(TrailResult tr, IValueRangeSeries<DateTime> t)
        {
            this.trailResult = tr;
            this.selInfo.MarkedTimes = TrailsItemTrackSelectionInfo.excludePauses(t, tr.Pauses);
            this.selInfo.Activity = tr.Activity;
        }

        public TrailResultMarked(TrailResult tr, IValueRange<DateTime> t)
        {
            this.trailResult = tr;
            this.selInfo.SelectedTime = t; //include pauses
            this.selInfo.Activity = tr.Activity;
        }

        //Note: IItemTrackSelectionInfo uses Activity distances, avoid...
        //public TrailResultMarked(TrailResult tr, IValueRangeSeries<double> t)
        //{
        //    trailResult = tr;
        //    selInfo.MarkedDistances = t;
        //}

        public TrailResultMarked(TrailResult tr, IItemTrackSelectionInfo t)
        {
            this.trailResult = tr;
            this.selInfo.SetFromSelection(t, tr.Activity);
            if (selInfo.MarkedTimes != null)
            {
                selInfo.MarkedTimes = TrailsItemTrackSelectionInfo.excludePauses(selInfo.MarkedTimes, tr.Pauses);
            }
            //Note that SelectedTime can still include paused time
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
                result.Activity = trm.trailResult.Activity; //TODO: verfify only one activity
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
