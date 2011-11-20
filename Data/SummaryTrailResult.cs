/*
Copyright (C) 2009 Brendan Doherty
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
using System.Drawing;
using System.Collections.Generic;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ITrailExport;
using GpsRunningPlugin.Util;

namespace TrailsPlugin.Data
{
    public class SummaryTrailResult : TrailResult
    {
        public SummaryTrailResult(ActivityTrail activityTrail, IList<TrailResult> list) :
            base(activityTrail, list.Count)
        {
            results = list;
        }

        public override double StartDist
        {
            get
            {
                double tot = 0;
                foreach (TrailResult t in results)
                {
                    tot += t.StartDist;
                }
                return tot / NoOfResults(0);
            }
        }

        public override TimeSpan Duration
        {
            get
            {
                TimeSpan tot = TimeSpan.Zero;
                foreach (TrailResult t in results)
                {
                    tot += t.Duration;
                }
                return TimeSpan.FromSeconds(tot.TotalSeconds / NoOfResults(0));
            }
        }

        public override double Distance
        {
            get
            {
                double tot = 0;
                foreach (TrailResult t in results)
                {
                    tot += t.Distance;
                }
                return tot / NoOfResults(0);
            }
        }

        public override float AvgCadence
        {
            get
            {
                float tot = 0;
                int skip = 0;
                foreach (TrailResult t in results)
                {
                    if (t.AvgCadence == 0)
                    {
                        skip++;
                    }
                    tot += t.AvgCadence;
                }
                return tot / NoOfResults(skip);
            }
        }

        public override float AvgHR
        {
            get
            {
                float tot = 0;
                int skip=0;
                foreach (TrailResult t in results)
                {
                    if (t.AvgHR == 0)
                    {
                        skip++;
                    }
                    tot += t.AvgHR;
                }
                return tot / NoOfResults(skip);
            }
        }

        public override float MaxHR
        {
            get
            {
                float tot = 0;
                int skip = 0;
                foreach (TrailResult t in results)
                {
                    if (t.MaxHR == 0)
                    {
                        skip++;
                    }
                    tot += t.MaxHR;
                }
                return tot / NoOfResults(skip);
            }
        }

        public override double ElevChg
        {
            get
            {
                double tot = 0;
                foreach (TrailResult t in results)
                {
                    tot += t.ElevChg;
                }
                return tot / NoOfResults(0);
            }
        }

        public override float AvgPower
        {
            get
            {
                float tot = 0;
                int skip = 0;
                foreach (TrailResult t in results)
                {
                    if (t.AvgPower == 0)
                    {
                        skip++;
                    }
                    tot += t.AvgPower;
                }
                return tot / NoOfResults(skip);
            }
        }

        public override float AvgGrade
        {
            get
            {
                float tot = 0;
                foreach (TrailResult t in results)
                {
                    tot += t.AvgGrade;
                }
                return tot / NoOfResults(0);
            }
        }

        IList<TrailResult> results;
        private int NoOfResults(int skip)
        {
            //convenience to avoid null checks...
            if (results == null || results.Count == 0 || results.Count - skip <= 0)
            {
                return 1;
            }
            return results.Count;
        }
    }
}
