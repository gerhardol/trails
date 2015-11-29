/*
Copyright (C) 2013 Gerhard Olsson

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
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ITrailExport;
using TrailsPlugin.Utils;
using GpsRunningPlugin.Util;

namespace TrailsPlugin.Data
{
    //All but HighScore
    public class NormalChildTrailResult : ChildTrailResult
    {
        public NormalChildTrailResult(ActivityTrail activityTrail, ParentTrailResult par, int order, TrailResultInfo indexes, float distDiff) :
            base(activityTrail, par, order, indexes, distDiff)
        {
        }
    }

    public class PausedChildTrailResult : ChildTrailResult
    {
        public PausedChildTrailResult(ActivityTrail activityTrail, ParentTrailResult par, int order, TrailResultInfo indexes) :
            base(activityTrail, par, order, indexes, 0)
        {
        }
    }

    public class HighScoreChildTrailResult : ChildTrailResult
    {
        public HighScoreChildTrailResult(ActivityTrail activityTrail, HighScoreParentTrailResult par, int order, TrailResultInfo indexes, string tt) :
            base(activityTrail, par, order, indexes, 0)
        {
            this.m_toolTip = tt;
            this.PartOfParent = false;
        }
    }

    public class ChildTrailResult : TrailResult
    {
        protected ChildTrailResult(ActivityTrail activityTrail, ParentTrailResult par, int order, TrailResultInfo indexes, float distDiff) :
            base(activityTrail, order, indexes, distDiff)
        {
            this.m_parentResult = par;
            if (indexes.Count == 2)
            {
                //Always overwrite the (possibly) calculated data
                this.m_duration = indexes.Points[0].Duration;
            }
        }

        internal bool PartOfParent = true;

        private ParentTrailResult m_parentResult;
        public ParentTrailResult ParentResult
        {
            get
            {
                return m_parentResult;
            }
        }
    }
}
