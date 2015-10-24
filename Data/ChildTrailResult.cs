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
    public class NormalChildTrailResult : ChildTrailResult
    {
        public NormalChildTrailResult(ActivityTrail activityTrail, ParentTrailResult par, int order, TrailResultInfo indexes, float distDiff) :
            base(activityTrail, par, order, indexes, distDiff)
        {
            createResult(par, indexes, true);
        }
    }
    public class HighScoreChildTrailResult : ChildTrailResult
    {
        public HighScoreChildTrailResult(ActivityTrail activityTrail, ParentTrailResult par, int order, TrailResultInfo indexes, float distDiff, string tt) :
            base(activityTrail, par, order, indexes, distDiff, tt)
        {
            createResult(par, indexes, false);
        }
    }

    public class ChildTrailResult : TrailResult
    {
        internal bool PartOfParent = true;

        private ParentTrailResult m_parentResult;
        public ParentTrailResult ParentResult
        {
            get
            {
                return m_parentResult;
            }
        }

        protected ChildTrailResult(ActivityTrail activityTrail, ParentTrailResult par, int order, TrailResultInfo indexes, float distDiff) :
            base(activityTrail, order, indexes, distDiff)
        {
            createResult(par, indexes, true);
        }

        //HighScore
        protected ChildTrailResult(ActivityTrail activityTrail, ParentTrailResult par, int order, TrailResultInfo indexes, float distDiff, string tt) :
            base(activityTrail, order, indexes, distDiff, tt)
        {
            createResult(par, indexes, false);
        }

        protected void createResult(ParentTrailResult par, TrailResultInfo indexes, bool part)
        {
            this.m_parentResult = par;
            if (indexes.Count == 2)
            {
                this.m_duration = indexes.Points[0].Duration;
            }
            this.PartOfParent = part;
            this.m_LapInfo = indexes.LapInfo;
            if(indexes.Points != null && indexes.Points.Count>0)
            {
                this.m_PoolLengthInfo = indexes.Points[0].PoolLengthInfo;
            }
        }
    }
}
