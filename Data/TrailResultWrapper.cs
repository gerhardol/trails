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

using System;
using System.Drawing;
using System.Collections.Generic;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Visuals;
using ITrailExport;

namespace TrailsPlugin.Data {
    public class TrailResultWrapper : TreeList.TreeListNode, IComparable
    {
        public TrailResultWrapper(Trail trail, IActivity activity, int order, IList<int> indexes, float distDiff)
            : this(trail.TrailLocations, activity, order, indexes, distDiff)
        { }
        public TrailResultWrapper(IList<Data.TrailGPSLocation> trailgps, IActivity activity, int order, IList<int> indexes, float distDiff)
            : this(null, trailgps, activity, order, indexes, distDiff)
        { }

        private TrailResultWrapper(TrailResultWrapper par, IList<Data.TrailGPSLocation> trailgps, IActivity activity, int order, IList<int> indexes, float distDiff)
            : base(par, null)
        {
            base.Element = new TrailResult(trailgps, activity, order, indexes, distDiff);
            if (par == null)
            {
                getSplits();
            }
        }

        //Create from splits
        public TrailResultWrapper(Trail trail, IActivity activity, int order)
            : base(null, null)
        {
            IList<int> indexes;
            IList<Data.TrailGPSLocation> m_trailgps = Data.Trail.TrailGpsPointsFromSplits(activity, out indexes);
            base.Element = new TrailResult(m_trailgps, activity, order, indexes, float.MaxValue);
            getSplits();
        }
        private TrailResultWrapper(TrailResultWrapper par, TrailResult ele)
            : base(par, ele) { }

        public TrailResult Result
        {
            get
            {
                return (TrailResult)this.Element;
            }
        }
        public void Sort()
        {
            if (m_children.Count > 0)
            {
                //Sorting children directly fails- save original items
                ((List<TrailResultWrapper>)m_children).Sort();
                this.Children.Clear(); 
                foreach (TrailResultWrapper tn in m_children)
                {
                    this.Children.Add(tn);
                }
            }
        }

        private IList<TrailResultWrapper> m_children = new List<TrailResultWrapper>();
        private void getSplits()
        {
            IList<TrailResult> children = Result.getSplits();
            if (children != null && children.Count > 1)
            {
                foreach (TrailResult tr in children)
                {
                    TrailResultWrapper tn = new TrailResultWrapper(this, tr);
                    this.Children.Add(tn);
                    m_children.Add(tn);
                }
            }
        }
        public static IList<TrailResultWrapper> SelectedItems(IList<TrailResultWrapper> tn, IList<TrailResult> tr)
        {
            IList<TrailResultWrapper> result = new List<TrailResultWrapper>();
            if (tn != null && tr != null)
            {
                foreach (TrailResult trr in tr)
                {
                    foreach (TrailResultWrapper tnp in tn)
                    {
                        if (tnp.Result.Equals(trr))
                        {
                            result.Add(tnp);
                        }
                        foreach (TrailResultWrapper tnc in tnp.m_children)
                        {
                            if (tnc.Result.Equals(trr))
                            {
                                result.Add(tnc);
                            }
                        }
                    }
                }
            }
            return result;
        }
        #region IComparable<Product> Members
        public int CompareTo(object obj)
        {
            if (obj is TrailResultWrapper)
            {
                return this.Result.CompareTo(((TrailResultWrapper)obj).Result);
            }
            else
            {
                return this.ToString().CompareTo(obj.ToString());
            }
        }
        #endregion
    }
}
