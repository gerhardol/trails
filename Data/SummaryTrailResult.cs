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
        public SummaryTrailResult(ActivityTrail activityTrail) :
            base(activityTrail)
        {
            results = new List<TrailResult>();
        }

        public void SetSummary(IList<TrailResult> list) 
        {
            results = list; //must not be summary...
            this.m_order = list.Count;
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

        public override TimeSpan GradeRunAdjustedTime
        {
            get
            {
                TimeSpan tot = TimeSpan.Zero;
                foreach (TrailResult t in results)
                {
                    tot += t.GradeRunAdjustedTime;
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

        public override double Ascent
        {
            get
            {
                double tot = 0;
                foreach (TrailResult t in results)
                {
                    tot += t.Ascent;
                }
                return tot / NoOfResults(0);
            }
        }

        public override double Descent
        {
            get
            {
                double tot = 0;
                foreach (TrailResult t in results)
                {
                    tot += t.Descent;
                }
                return tot / NoOfResults(0);
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

        public override float AscAvgGrade
        {
            get
            {
                float tot = 0;
                foreach (TrailResult t in results)
                {
                    tot += t.AscAvgGrade;
                }
                return tot / NoOfResults(0);
            }
        }

        public override float DescAvgGrade
        {
            get
            {
                float tot = 0;
                foreach (TrailResult t in results)
                {
                    tot += t.DescAvgGrade;
                }
                return tot / NoOfResults(0);
            }
        }

        public override double FastestPace
        {
            get
            {
                double tot = 0;
                foreach (TrailResult t in results)
                {
                    tot += t.FastestPace;
                }
                return tot / NoOfResults(0);
            }
        }

        public override float FastestSpeed
        {
            get
            {
                float tot = 0;
                foreach (TrailResult t in results)
                {
                    tot += t.FastestSpeed;
                }
                return tot / NoOfResults(0);
            }
        }

        private int NoOfResults(int skip)
        {
            //convenience to avoid null checks...
            if (results == null || results.Count == 0 || results.Count - skip <= 0)
            {
                return 1;
            }
            return results.Count - skip;
        }

        public override IActivity Activity { get { return null; } }
        public IList<IActivity> Activities
        {
            get
            {
                IList<IActivity> activities = new List<IActivity>();
                foreach (TrailResult t in this.Results)
                {
                    if (!activities.Contains(t.Activity))
                    {
                        activities.Add(t.Activity);
                    }
                }
                return activities;
            }
        }
        public IList<TrailResult> Results
        {
            get
            {
                IList<TrailResult> res = new List<TrailResult>();
                foreach (TrailResult t in this.results)
                {
                    res.Add(t);
                }
                return res;
            }
        }

        private IList<TrailResult> results;

        public override string ToString()
        {
            return "Summary:" + results.Count;
        }
    }
}
