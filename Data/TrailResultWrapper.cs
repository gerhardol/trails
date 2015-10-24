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

namespace TrailsPlugin.Data
{
    public class TrailResultWrapper : TreeList.TreeListNode, IComparable
    {
        //Normal result
        public TrailResultWrapper(ActivityTrail activityTrail, int order, TrailResultInfo indexes)
            : this(activityTrail, null, order, indexes, indexes.DistDiff, indexes.Reverse)
        { }

        private TrailResultWrapper(ActivityTrail activityTrail, TrailResultWrapper par, int order, TrailResultInfo indexes, float distDiff, bool reverse)
            : base(par, null)
        {
            base.Element = new NormalParentTrailResult(activityTrail, order, indexes, distDiff, reverse);
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
            TrailResultInfo indexes = Data.Trail.TrailResultInfoFromSplits(activity, false);
            base.Element = new SplitsParentTrailResult(activityTrail, order, indexes, 0);
        }

        public TrailResultWrapper(ActivityTrail activityTrail, TrailResultInfo indexes, int order)
            : base(null, null)
        {
            base.Element = new SwimSplitsParentTrailResult(activityTrail, order, indexes, 0);
        }

        //Create from HighScore, add the first and last time stamps in MarkedTimes
        public TrailResultWrapper(ActivityTrail activityTrail, TrailResultWrapper parent, IActivity activity, IItemTrackSelectionInfo selInfo, string tt, int order)
            : base(null, null)
        {
            TrailResultInfo indexes = new TrailResultInfo(activity, false);
            DateTime time = selInfo.MarkedTimes[0].Lower;
            IGPSPoint p = Utils.TrackUtil.getGpsLoc(activity, time);
            if (p != null)
            {
                indexes.Points.Add(new TrailResultPoint(new TrailGPSLocation(p), time));
            }
            time = selInfo.MarkedTimes[0].Upper;
            p = Utils.TrackUtil.getGpsLoc(activity, time);
            if (p != null)
            {
                indexes.Points.Add(new TrailResultPoint(new TrailGPSLocation(p), time));
            }
            if (indexes.Count >= 2)
            {
                if (order == 1 || parent == null)
                {
                    base.Element = new HighScoreParentTrailResult(activityTrail, order, indexes, 0, tt);
                }
                else if (parent.Element != null && parent.Element is ParentTrailResult)
                {
                    base.Parent = parent;
                    ParentTrailResult ptr = parent.Result as ParentTrailResult;
                    ChildTrailResult ctr = new HighScoreChildTrailResult(activityTrail, ptr, order, indexes, 0, tt);
                    base.Element = ctr;
                    parent.Children.Add(this);
                    parent.m_children.Add(this);
                    if (ptr.m_childrenResults == null)
                    {
                        ptr.m_childrenResults = new List<ChildTrailResult>();
                    }
                    ptr.m_childrenResults.Add(ctr);
                }
            }
        }

        //Summary line
        public TrailResultWrapper()
            : base(null, null)
        {
            m_isSummary = true;
            base.Element = new SummaryTrailResult();
        }

        public void SetSummary(IList<TrailResultWrapper> rows)
        {
            ((SummaryTrailResult)base.Element).SetSummary(GetTrailResults(rows, false));
        }

        //SubResults
        private TrailResultWrapper(TrailResultWrapper par, TrailResult ele)
            : base(par, ele) { }

        public TrailResult Result
        {
            get
            {
                return (TrailResult)this.Element;
            }
        }

        public bool IsSummary
        {
            get { return m_isSummary; }
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
                    if (!TrailsPlugin.Data.Settings.RestIsPause || tn.Result.Duration.TotalSeconds > 1)
                    {
                        this.Children.Add(tn);
                    }
                }
            }
        }

        //TODO: Calculate children when needed, by implementing Children
        //This is currently called after all parent results have been determined
        //A good enough reason is that this will give main activities separate colors, in the intended order
        public void getSplits()
        {
            if (this.Result != null && this.Result is ParentTrailResult)
            {
                ParentTrailResult ptr = this.Result as ParentTrailResult;
                IList<ChildTrailResult> children = ptr.getSplits();
                if (children != null && children.Count > 1)
                {
                    foreach (ChildTrailResult tr in children)
                    {
                        TrailResultWrapper tn = new TrailResultWrapper(this, tr);
                        //several separate substructues..
                        this.Children.Add(tn);
                        this.m_children.Add(tn);
                        if (ptr.m_childrenResults == null)
                        {
                            ptr.m_childrenResults = new List<ChildTrailResult>();
                        }
                        ptr.m_childrenResults.Add(tr);
                    }
                }
            }
        }

        public bool RemoveChildren(IList<TrailResultWrapper> tn, bool invertSelection)
        {
            bool result = false;
            foreach (TrailResultWrapper tr in tn)
            {
                if (this.m_children.Contains(tr))
                {
                    this.m_children.Remove(tr);
                    result = true;
                }
                //May not be needed as Children are added when sorting
                if (this.Children.Contains(tr))
                {
                    this.Children.Remove(tr);
                }
                if (this.Result != null && this.Result is ParentTrailResult && (this.Result as ParentTrailResult).m_childrenResults != null
                    && tr.Result is ChildTrailResult)
                {
                    ChildTrailResult ctr = tr.Result as ChildTrailResult;
                    if ((this.Result as ParentTrailResult).m_childrenResults.Contains(ctr))
                    {
                        (this.Result as ParentTrailResult).m_childrenResults.Remove(ctr);
                    }
                }
            }
            return result;
        }

        private static IList<TrailResult> GetTrailResults(IList<TrailResultWrapper> tn, bool includeChildren)
        {
            IList<TrailResult> result = new List<TrailResult>();
            if (tn != null)
            {
                foreach (TrailResultWrapper tnp in tn)
                {
                    result.Add(tnp.Result);
                    if (includeChildren)
                    {
                        foreach (TrailResultWrapper tnc in tnp.Children)
                        {
                            if (!result.Contains(tnc.Result))
                            {
                                result.Add(tnc.Result);
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Return only Parent results (not the Splits/SubResults)
        /// </summary>
        public static IList<TrailResult> Results(IList<TrailResultWrapper> tn)
        {
            return TrailResultWrapper.GetTrailResults(tn, false);
        }

        public static IList<TrailResult> IncludeSubResults(IList<TrailResultWrapper> tn)
        {
            return TrailResultWrapper.GetTrailResults(tn, true);
        }

        public static IList<IActivity> Activities(IList<TrailResultWrapper> tn)
        {
            IList<IActivity> result = new List<IActivity>();
            if (tn != null)
            {
                foreach (TrailResultWrapper tnp in tn)
                {
                    if(!result.Contains(tnp.Result.Activity))
                    {
                        result.Add(tnp.Result.Activity);
                    }
                }
            }
            return result;
        }

        //Get all TrailResultWrapper (including children) for the provided TrailResult in the list
        //The check uses CompareTo() instead of Contains() as the result list may be for previous calculations
        public static IList<TrailResultWrapper> SelectedItems(IList<TrailResultWrapper> trws, IList<TrailResult> tr)
        {
            IList<TrailResultWrapper> result = new List<TrailResultWrapper>();
            if (trws != null && tr != null)
            {
                foreach (TrailResult trr in tr)
                {
                    bool isMatch = false;
                    foreach (TrailResultWrapper tnp in trws)
                    {
                        if (isMatch)
                        {
                            break;
                        }
                        if (!(trr is ChildTrailResult))
                        {
                            if (tnp.Result.CompareTo(trr) == 0)
                            {
                                result.Add(tnp);
                                //Break the loop
                                isMatch = true;
                            }
                        }
                        else
                        {
                            foreach (TrailResultWrapper tnc in tnp.m_children)
                            {
                                if (tnc.Result.CompareTo(trr) == 0)
                                {
                                    result.Add(tnc);
                                    //break from two levels of foreach
                                    isMatch = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        private IList<TrailResultWrapper> m_children = new List<TrailResultWrapper>();
        private bool m_isSummary = false;

        #region IComparable<Product> Members
        public int CompareTo(object obj)
        {
            if (obj is TrailResultWrapper && this.Result != null && ((TrailResultWrapper)obj).Result != null)
            {
                return this.Result.CompareTo(((TrailResultWrapper)obj).Result);
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "Unexpectedly comparing TrailResultWrapper to object or no result");
                return this.ToString().CompareTo(obj.ToString());
            }
        }
        #endregion

        public override string ToString()
        {
            return (this.Result != null ? this.Result.ToString() : "");
        }
    }
}
