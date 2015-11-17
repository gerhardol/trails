/*
Copyright (C) 2009 Brendan Doherty
Copyright (C) 2011-2013 Gerhard Olsson

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
    public class NormalParentTrailResult : ParentTrailResult
    {
        //Normal TrailResult
        public NormalParentTrailResult(ActivityTrail activityTrail, int order, TrailResultInfo indexes, float distDiff, bool reverse)
            : base(activityTrail, order, indexes, distDiff)
        {
            this.m_reverse = reverse;
        }
    }
    public class SplitsParentTrailResult : ParentTrailResult
    {
        public SplitsParentTrailResult(ActivityTrail activityTrail, int order, TrailResultInfo indexes, float distDiff) :
            base(activityTrail, order, indexes, distDiff)
        {
        }
    }
    public class SwimSplitsParentTrailResult : ParentTrailResult
    {
        public SwimSplitsParentTrailResult(ActivityTrail activityTrail, int order, TrailResultInfo indexes, float distDiff) :
            base(activityTrail, order, indexes, distDiff)
        {
        }
    }
    public class HighScoreParentTrailResult : ParentTrailResult
    {
        public HighScoreParentTrailResult(ActivityTrail activityTrail, int order, TrailResultInfo indexes, float distDiff, string toolTip)
            : base(activityTrail, order, indexes, distDiff)
        {
            this.m_toolTip = toolTip;
        }
    }

    public class ParentTrailResult : TrailResult
    {
        protected ParentTrailResult(ActivityTrail activityTrail, int order, TrailResultInfo indexes, float distDiff) :
            base(activityTrail, order, indexes, distDiff)
        {
        }

        public IList<ChildTrailResult> getSplits()
        {
            IList<ChildTrailResult> splits = new List<ChildTrailResult>();
            if (this.m_subResultInfo.Count > 1)
            {
                int i; //start time index
                for (i = 0; i < m_subResultInfo.Count - 1; i++)
                {
                    if (m_subResultInfo.Points[i].Time != DateTime.MinValue &&
                                (!this.m_activityTrail.Trail.IsSplits || !Settings.RestIsPause || m_subResultInfo.Points[i].Required))
                    {
                        int j; //end time index
                        for (j = i + 1; j < m_subResultInfo.Points.Count; j++)
                        {
                            if (m_subResultInfo.Points[j].Time != DateTime.MinValue)
                            {
                                break;
                            }
                        }
                        if (this.m_subResultInfo.Count > i &&
                            this.m_subResultInfo.Count > j)
                        {
                            if (m_subResultInfo.Points[j].Time != DateTime.MinValue)
                            {
                                TrailResultInfo t = m_subResultInfo.CopyToChild(i, j);
                                int subresultIndex = i + 1;
                                if (m_subResultInfo.Points[i].Order >= 0)
                                {
                                    subresultIndex = m_subResultInfo.Points[i].Order;
                                }
                                ChildTrailResult tr = new NormalChildTrailResult(m_activityTrail, this, subresultIndex, t, t.DistDiff);
                                //Note: paused results may be added, no limit for childresults
                                splits.Add(tr);
                            }
                        }
                        i = j - 1;//Next index to try
                    }
                }

                TimeSpan sp = TimeSpan.Zero;
                bool ok = true;
                foreach (ChildTrailResult ctr in splits)
                {
                    if (ctr.DurationIsNull())
                    {
                        ok = false;
                        break;
                    }
                    sp = sp.Add((TimeSpan)ctr.Duration);
                }
                if (ok)
                {
                    this.m_duration = sp;
                }
            }
            //Only one subresult is not shown, add pool length to main result
            if (splits.Count <= 1 && this.SubResultInfo.Points.Count > 0)
            {
                this.m_PoolLengthInfo = this.SubResultInfo.Points[0].PoolLengthInfo;
            }
            return splits;
        }

        internal IList<ChildTrailResult> m_childrenResults;
    }
}
