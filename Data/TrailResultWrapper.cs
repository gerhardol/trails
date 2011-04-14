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
using System.Drawing;
using System.Collections.Generic;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ITrailExport;

namespace TrailsPlugin.Data {
    public class TrailResultWrapper : TreeList.TreeListNode, IComparable
    {
        public TrailResultWrapper(ActivityTrail activityTrail, int order, TrailResultInfo indexes, float distDiff)
            : this(activityTrail, null, order, indexes, distDiff)
        { }

        private TrailResultWrapper(ActivityTrail activityTrail, TrailResultWrapper par, int order, TrailResultInfo indexes, float distDiff)
            : base(par, null)
        {
            base.Element = new TrailResult(activityTrail, order, indexes, distDiff);
            if (par == null)
            {
                //Children are not created by default
                //getSplits();
            }
        }

        //Create results from splits
        public TrailResultWrapper(ActivityTrail activityTrail, IActivity activity, int order)
            : base(null, null)
        {
            TrailResultInfo indexes;
            IList<Data.TrailGPSLocation> m_trailgps = Data.Trail.TrailGpsPointsFromSplits(activity, out indexes, false);
            base.Element = new TrailResult(activityTrail, order, indexes, float.MaxValue);
            //Children are not created by default
            //getSplits();
        }

        //Create from HighScore, add the first and last time stamps
        public TrailResultWrapper(ActivityTrail activityTrail, IActivity activity, IItemTrackSelectionInfo selInfo, string tt, int order)
            : base(null, null)
        {
            TrailResultInfo indexes = new TrailResultInfo(activity);
            indexes.Points.Add(new TrailResultPoint(selInfo.MarkedTimes[0].Lower, ""));
            indexes.Points.Add(new TrailResultPoint(selInfo.MarkedTimes[0].Upper, ""));
            if (indexes.Count >= 2)
            {
                base.Element = new TrailResult(activityTrail, order, indexes, float.MaxValue, tt);
            }
            //No Children
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

        //TODO: Calculate children when needed, by implementing Children
        //This is currently called after all parent results have been determined
        //A good enough reason is that this willgive main activities separate colors, in the intended order
        private IList<TrailResultWrapper> m_children = new List<TrailResultWrapper>();
        public void getSplits()
        {
            if (this.Result != null)
            {
                IList<TrailResult> children = this.Result.getSplits();
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
        }
        public bool RemoveChildren(IList<TrailResultWrapper> tn, bool invertSelection)
        {
            bool result = true;
            foreach (TrailResultWrapper tr in tn)
            {
                if (m_children.Contains(tr))
                {
                    m_children.Remove(tr);
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }
        public static IList<TrailResult> GetTrailResults(IList<TrailResultWrapper> tn)
        {
            IList<TrailResult> result = new List<TrailResult>();
            if (tn != null)
            {
                foreach (TrailResultWrapper tnp in tn)
                {
                    result.Add(tnp.Result);
                    foreach (TrailResultWrapper tnc in tnp.Children)
                    {
                        if (!result.Contains(tnc.Result))
                        {
                            result.Add(tnc.Result);
                        }
                    }
                }
            }
            return result;
        }

        //Get all TrailResultWrapper (including children) for the provided TrailResult in the list
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
            if (obj is TrailResultWrapper && this.Result != null && ((TrailResultWrapper)obj).Result != null)
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
