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
        public NormalChildTrailResult(ParentTrailResult par, int order, TrailResultInfo indexes) :
            base(par, order, indexes, indexes.DistDiff)
        {
        }
    }

    public class PausedChildTrailResult : ChildTrailResult
    {
        public PausedChildTrailResult(ParentTrailResult par, int order, TrailResultInfo indexes) :
            base(par, order, indexes, 0)
        {
        }
    }

    public class SubChildTrailResult : ChildTrailResult
    {
        public SubChildTrailResult(ChildTrailResult par, int order, TrailResultInfo indexes) :
            base(par, order, indexes)
        {
            par.SubResults.Add(this);
        }
    }

    public class HighScoreChildTrailResult : ChildTrailResult
    {
        public HighScoreChildTrailResult(HighScoreParentTrailResult par, int order, TrailResultInfo indexes, string tt) :
            base(par, order, indexes, 0)
        {
            this.m_toolTip = tt;
            this.PartOfParent = false;
        }
    }

    public class ChildTrailResult : TrailResult
    {
        protected ChildTrailResult(ParentTrailResult par, int order, TrailResultInfo indexes, float distDiff) :
            base(par.ActivityTrail, order, indexes, distDiff)
        {
            this.m_parentResult = par;
            if (indexes.Count == 2)
            {
                //Always overwrite the (possibly) calculated data
                this.m_duration = indexes.Points[0].Duration;
            }
        }

        protected ChildTrailResult(ChildTrailResult cpar, int order, TrailResultInfo indexes) :
            base(cpar.ActivityTrail, order, indexes, 0)
        {
            //Let this point to main parent, no ref to (parent) child (can be derived too)
            this.m_parentResult = cpar.ParentResult;
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

        //Subresults, not recursively handled
        public IList<SubChildTrailResult> SubResults = new List<SubChildTrailResult>();
    }
}
