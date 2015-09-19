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
    public class SummaryValue<T>
    {
        public T Value;
        public T StdDev;
        public SummaryValue(T Value, T StdDev)
        {
            this.Value = Value;
            this.StdDev = StdDev;
        }
    }

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
            this.Clear(false);
        }

        private delegate double FieldGetter(TrailResult tr);

        private double GetSummaryValue(FieldGetter field, bool skipZero, bool total)
        {
            return GetSummary(field, skipZero, total, false).Value;
        }

        private SummaryValue<double> GetSummary(FieldGetter field, bool skipZero, bool total)
        {
            return GetSummary(field, skipZero, total, true);
        }

        private SummaryValue<double> GetSummary(FieldGetter field, bool skipZero, bool total, bool getStdDev)
        {
            int skip = 0;
            double a = 0;
            double tot = 0;
            int i = 0;
            double stdDev = 0;

            for (i = 0; i < this.results.Count; i++)
            {
                double x = field(this.results[i]);
                if (skipZero && x == 0 || double.IsNaN(x) || double.IsInfinity(x))
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

            if (!total || !Settings.ResultSummaryTotal)
            {
                tot = tot / NoOfResults(skip);
            }
            if (getStdDev)
            {
                stdDev = stdDev / NoOfResults(skip);
                stdDev = Math.Sqrt(stdDev);
            }

            return new SummaryValue<double>(tot, stdDev);
        }

        /*******************************************************/

        //TBD: Cache summary results. Profiling indicates minor difference
        public override DateTime StartTime
        {
            get
            {
                //Get average time of day
                double totSec = this.GetSummaryValue(delegate(TrailResult tr) { return tr.StartTimeOfDay.TotalSeconds; }, true, false);
                return DateTime.Today + TimeSpan.FromSeconds(totSec);
            }
        }

        public override double StartDist
        {
            get
            {
                return this.GetSummaryValue(delegate(TrailResult tr) { return tr.StartDist; }, false, false);
            }
        }

        public override TimeSpan Duration
        {
            get
            {
                return DurationStdDev(false).Value;
            }
        }

        public SummaryValue<TimeSpan> DurationStdDev()
        {
            return DurationStdDev(true);
        }

        private SummaryValue<TimeSpan> DurationStdDev(bool getStdDev)
        {
            SummaryValue<double> a = this.GetSummary(delegate(TrailResult tr) { return tr.Duration.TotalSeconds; }, false, true, getStdDev);
            TimeSpan stdDev = TimeSpan.Zero;
            if (getStdDev)
            {
                stdDev = TimeSpan.FromSeconds(a.StdDev);
            }
            return new SummaryValue<TimeSpan>(TimeSpan.FromSeconds(a.Value), stdDev);
        }

        public override TimeSpan GradeRunAdjustedTime
        {
            get
            {
                return TimeSpan.FromSeconds(this.GetSummaryValue(delegate(TrailResult tr) { return tr.GradeRunAdjustedTime.TotalSeconds; }, false, true));
            }
        }

        public override float GradeRunAdjustedSpeed
        {
            get
            {
                return (float)(Distance/GradeRunAdjustedTime.TotalSeconds);
            }
        }

        public override double Distance
        {
            get
            {
                return DistanceStdDev(false).Value;
            }
        }

        public SummaryValue<double> DistanceStdDev()
        {
            return DistanceStdDev(true);
        }

        private SummaryValue<double> DistanceStdDev(bool getStdDev)
        {
            return this.GetSummary(delegate(TrailResult tr) { return tr.Distance; }, false, true, getStdDev);
        }

        //No override for some methods
        public SummaryValue<double> AvgPaceStdDev()
        {
            SummaryValue<double> a = this.GetSummary(delegate(TrailResult tr) { return 1 / tr.AvgSpeed; }, false, false);
            a.StdDev = 1 / a.StdDev; //Convert back
            a.Value = this.AvgSpeed; //Return average, not "median"
            return a;
        }

        public SummaryValue<double> AvgSpeedStdDev()
        {
            SummaryValue<double> a = this.GetSummary(delegate(TrailResult tr) { return tr.AvgSpeed; }, false, false);
            a.Value = this.AvgSpeed; //Return average, not "median"
            return a;
        }

        //public SummaryValue<double> GradeRunAdjustedPaceStdDev()
        //{
        //    SummaryValue<double> a = this.GetSummary(delegate(TrailResult tr) { return 1 / tr.GradeRunAdjustedSpeed; }, false, false);
        //    a.StdDev = 1 / a.StdDev; //Convert back
        //    a.Value = this.AvgSpeed; //Return average, not "median"
        //    return a; 
        //}

        //public SummaryValue<TimeSpan> GradeRunAdjustedTimeStdDev()
        //{
        //    SummaryValue<double> a = this.GetSummary(delegate(TrailResult tr) { return tr.GradeRunAdjustedTime.TotalSeconds; }, false, true);
        //    return new SummaryValue<TimeSpan>(TimeSpan.FromSeconds(a.Value), TimeSpan.FromSeconds(a.StdDev));
        //}

        //public SummaryValue<double> DiffStdDev()
        //{
        //    return this.GetSummary(delegate(TrailResult tr) { return tr.Diff; }, false, false);
        //}

        public override float AvgCadence
        {
            get
            {
                return (float)this.GetSummaryValue(delegate(TrailResult tr) { return tr.AvgCadence; }, true, false);
            }
        }

        public override float AvgHR
        {
            get
            {
                return (float)this.GetSummaryValue(delegate(TrailResult tr) { return tr.AvgHR; }, true, false);
            }
        }

        public override float MaxHR
        {
            get
            {
                return (float)this.GetSummaryValue(delegate(TrailResult tr) { return tr.MaxHR; }, true, false);
            }
        }

        public override double Ascent
        {
            get
            {
                return this.GetSummaryValue(delegate(TrailResult tr) { return tr.Ascent; }, false, true);
            }
        }

        public override double Descent
        {
            get
            {
                return this.GetSummaryValue(delegate(TrailResult tr) { return tr.Descent; }, false, true);
            }
        }

        public override double ElevChg
        {
            get
            {
                return (float)this.GetSummaryValue(delegate(TrailResult tr) { return tr.ElevChg; }, false, true);
            }
        }

        public override float AvgPower
        {
            get
            {
                return (float)this.GetSummaryValue(delegate(TrailResult tr) { return tr.AvgPower; }, true, false);
            }
        }

        public override float AscAvgGrade
        {
            get
            {
                return (float)this.GetSummaryValue(delegate(TrailResult tr) { return tr.AscAvgGrade; }, false, false);
            }
        }

        public override float AscMaxAvgGrade
        {
            get
            {
                return (float)this.GetSummaryValue(delegate (TrailResult tr) { return tr.AscMaxAvgGrade; }, true, false);
            }
        }

        public override float DescAvgGrade
        {
            get
            {
                return (float)this.GetSummaryValue(delegate(TrailResult tr) { return tr.DescAvgGrade; }, false, false);
            }
        }

        public override double FastestPace
        {
            get
            {
                return this.GetSummaryValue(delegate(TrailResult tr) { return tr.FastestPace; }, false, false);
            }
        }

        public override float FastestSpeed
        {
            get
            {
                return (float)this.GetSummaryValue(delegate(TrailResult tr) { return tr.FastestSpeed; }, false, false);
            }
        }

        public override float Diff
        {
            get
            {
                return (float)this.GetSummaryValue(delegate(TrailResult tr) { return tr.Diff; }, false, true);
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
