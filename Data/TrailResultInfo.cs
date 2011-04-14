/*
Copyright (C) 2009 Brendan Doherty

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

//Used in both Trails and Matrix plugin

using System.Xml;
using System;
using System.Collections.Generic;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;

namespace TrailsPlugin.Data {
	public class TrailResultInfo {
        public IList<TrailResultPoint> Points;
        public IActivity Activity;

        public TrailResultInfo(IActivity activity)
        {
            Activity = activity;
            Points = new List<TrailResultPoint>();
        }
        //Hide the handling slightly
        public TrailResultInfo Copy()
        {
            TrailResultInfo result = new TrailResultInfo(this.Activity);
            foreach (TrailResultPoint p in Points)
            {
                result.Points.Add(p);
            }
            return result;
        }
        public TrailResultInfo CopySlice(int i, int j)
        {
            TrailResultInfo result = new TrailResultInfo(this.Activity);
            result.Points.Add(new TrailResultPoint(Points[i].Time, Points[i].Name));
            result.Points.Add(new TrailResultPoint(Points[j].Time, Points[j].Name));
            return result;
        }
        public IList<DateTime> CopyTime()
        {
            IList<DateTime> result = new List<DateTime>();
            foreach (TrailResultPoint p in Points)
            {
                result.Add(p.Time);
            }
            return result;
        }
        public string Name
        {
            get
            {
                if (Points.Count > 0)
                {
                    return Points[0].Name;
                }
                return "";
            }
        }
        public int Count
        {
            get
            {
                return Points.Count;
            }
        }
    }

    public class TrailResultPoint
    {
        public TrailResultPoint(DateTime time, string name)
        {
            this.m_name = name;
            this.m_time = time;
        }

        public override string ToString()
        {
            return m_name + " " + m_time;
        }

        private string m_name;
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                this.m_name = value;
            }
        }
        private DateTime m_time;
        public DateTime Time
        {
            get
            {
                return m_time;
            }
            set
            {
                this.m_time = value;
            }
        }

	}
}
