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
        public SummaryTrailResult() :
            base()
        {
            this.results = new List<TrailResult>();
        }

        public void SetSummary(IList<TrailResult> list) 
        {
            this.results = list; //must not be summary...
            this.m_order = list.Count;
        }

        private delegate double FieldGetter(TrailResult tr);

        private double GetSummary(FieldGetter field, bool skipZero)
        {
            double stdDev;
            return GetSummary(field, skipZero, false, out  stdDev);
        }

        private double GetSummary(FieldGetter field, bool skipZero, out double stdDev)
        {
            return GetSummary(field, skipZero, true, out stdDev);
        }

        private double GetSummary(FieldGetter field, bool skipZero, bool getStdDev, out double stdDev)
        {
            int skip = 0;
            double a = 0;
            double tot = 0;
            int i = 0;
            stdDev = 0;

            for (i = 0; i < this.results.Count; i++)
            {
                double x = field(this.results[i]);
                if (skipZero && x == 0)
                {
                    skip++;
                }
                else
                {
                    tot += x;
                    if (getStdDev)
                    {
                        //standard deviation
                        double ap = a;
                        a += (x - ap) / (i + 1);
                        stdDev += (x - a) * (x - ap);
                    }
                }
            }

            tot = tot / NoOfResults(skip);
            if (getStdDev)
            {
                stdDev = stdDev / NoOfResults(skip);
                stdDev = Math.Sqrt(stdDev);
            }

            return tot;
        }

        //TBD: Cache summary results. Profiling indicates minor difference
        public override double StartDist
        {
            get
            {
                return this.GetSummary(delegate(TrailResult tr) { return tr.StartDist; }, false);
            }
        }

        public override TimeSpan Duration
        {
            get
            {
                return TimeSpan.FromSeconds(this.GetSummary(delegate(TrailResult tr) { return tr.Duration.TotalSeconds; }, false));
            }
        }

        public TimeSpan DurationStdDev(out TimeSpan stdDev)
        {
            double s;
            TimeSpan a = TimeSpan.FromSeconds(this.GetSummary(delegate(TrailResult tr) { return tr.Duration.TotalSeconds; }, false, out s));
            stdDev = TimeSpan.FromSeconds(s);
            return a;
        }

        public override TimeSpan GradeRunAdjustedTime
        {
            get
            {
                return TimeSpan.FromSeconds(this.GetSummary(delegate(TrailResult tr) { return tr.GradeRunAdjustedTime.TotalSeconds; }, false));
            }
        }

        public override double Distance
        {
            get
            {
                return this.GetSummary(delegate(TrailResult tr) { return tr.Distance; }, false);
            }
        }

        public double DistanceStdDev(out double stdDev)
        {
            return this.GetSummary(delegate(TrailResult tr) { return tr.Distance; }, false, true, out stdDev);
        }

        public override float AvgCadence
        {
            get
            {
                return (float)this.GetSummary(delegate(TrailResult tr) { return tr.AvgCadence; }, true);
            }
        }

        public override float AvgHR
        {
            get
            {
                return (float)this.GetSummary(delegate(TrailResult tr) { return tr.AvgHR; }, true);
            }
        }

        public override float MaxHR
        {
            get
            {
                return (float)this.GetSummary(delegate(TrailResult tr) { return tr.MaxHR; }, true);
            }
        }

        public override double Ascent
        {
            get
            {
                return this.GetSummary(delegate(TrailResult tr) { return tr.Ascent; }, false);
            }
        }

        public override double Descent
        {
            get
            {
                return this.GetSummary(delegate(TrailResult tr) { return tr.Descent; }, false);
            }
        }

        public override double ElevChg
        {
            get
            {
                return (float)this.GetSummary(delegate(TrailResult tr) { return tr.ElevChg; }, false);
            }
        }

        public override float AvgPower
        {
            get
            {
                return (float)this.GetSummary(delegate(TrailResult tr) { return tr.AvgPower; }, true);
            }
        }

        public override float AscAvgGrade
        {
            get
            {
                return (float)this.GetSummary(delegate(TrailResult tr) { return tr.AscAvgGrade; }, false);
            }
        }

        public override float DescAvgGrade
        {
            get
            {
                return (float)this.GetSummary(delegate(TrailResult tr) { return tr.DescAvgGrade; }, false);
            }
        }

        public override double FastestPace
        {
            get
            {
                return this.GetSummary(delegate(TrailResult tr) { return tr.FastestPace; }, false);
            }
        }

        public override float FastestSpeed
        {
            get
            {
                return (float)this.GetSummary(delegate(TrailResult tr) { return tr.FastestSpeed; }, false);
            }
        }

        public override float DistDiff
        {
            get
            {
                return (float)this.GetSummary(delegate(TrailResult tr) { return tr.DistDiff; }, false);
            }
        }

        /************************************/

        private int NoOfResults(int skip)
        {
            //convenience to avoid null checks...
            if (this.results == null || this.results.Count == 0 || this.results.Count - skip <= 0)
            {
                return 1;
            }
            return this.results.Count - skip;
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
            return "Summary:" + this.results.Count;
        }
    }
}
