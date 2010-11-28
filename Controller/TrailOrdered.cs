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
using System.Text;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;

namespace TrailsPlugin.Controller 
{
    public enum TrailOrderStatus
    {
        Match, MatchNoCalc, InBoundNoCalc, InBound, NotInBound, NoInfo
    }
    public class TrailOrdered : IComparable
    {
        public TrailOrdered(Data.ActivityTrail activityTrail, TrailOrderStatus status)
        { this.activityTrail = activityTrail; this.status = status; }
        public Data.ActivityTrail activityTrail;
        public TrailOrderStatus status;

        public int CompareTo(object obj)
        {
            if (!(obj is TrailOrdered))
            {
                return 1;
            }
            TrailOrdered to2 = obj as TrailOrdered;
            if(status != to2.status){
                return status > to2.status? 1: -1;
            }
            else if (status == TrailOrderStatus.Match)
            {
                if (activityTrail.Trail.MatchAll != to2.activityTrail.Trail.MatchAll)
                {
                    return (activityTrail.Trail.MatchAll) ? 1 : -1;
                }
                else if (activityTrail.Results.Count != to2.activityTrail.Results.Count)
                {
                    return (activityTrail.Results.Count < to2.activityTrail.Results.Count) ? 1 : -1;
                }
                else
                {
                    float e1 = 0;
                    foreach (Data.TrailResult tr in activityTrail.Results)
                    {
                        e1 += tr.DistDiff;
                    }
                    e1 = e1 / activityTrail.Results.Count;
                    float e2 = 0;
                    foreach (Data.TrailResult tr in activityTrail.Results)
                    {
                        e2 += tr.DistDiff;
                    }
                    e2 = e2 / activityTrail.Results.Count;
                    //No check if equal here
                    return e1 < e2 ? 1 : -1;
                }
            }
            //Sort remaining by name
            return activityTrail.Trail.Name.CompareTo(to2.activityTrail.Trail.Name);
        }
    }
}
