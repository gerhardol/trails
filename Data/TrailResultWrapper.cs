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
    public class PositionTrailResultWrapper : TrailResultWrapper
    {
        //Normal result
        public PositionTrailResultWrapper(ActivityTrail activityTrail, int order, TrailResultInfo indexes)
               : base()
        {
            base.Element = new PositionParentTrailResult(activityTrail, order, indexes, indexes.DistDiff, indexes.Reverse);
            this.getChildren();
        }
    }

    public class SummaryTrailResultWrapper : TrailResultWrapper
    {
        //Summary line
        public SummaryTrailResultWrapper(bool isTotal)
        : base()
        {
            base.Element = new SummaryTrailResult(isTotal);
        }

        public void SetSummary(IList<TrailResultWrapper> rows)
        {
            ((SummaryTrailResult)base.Element).SetSummary(GetTrailResults(rows, false));
        }
    }

    public class SplitsTrailResultWrapper : TrailResultWrapper
    {
        public SplitsTrailResultWrapper(ActivityTrail activityTrail, IActivity activity, int order)
            : this(activityTrail, Data.Trail.TrailResultInfoFromSplits(activity, false), order)
        {
        }
        public SplitsTrailResultWrapper(ActivityTrail activityTrail, TrailResultInfo indexes, int order)
            : base()
        {
            base.Element = new SplitsParentTrailResult(activityTrail, order, indexes, 0);
            this.getChildren();
        }

        protected SplitsTrailResultWrapper()
             : base()
        {
        }
   }

    public class TimeSplitsTrailResultWrapper : SplitsTrailResultWrapper
    {
        //create results on datetime info
        public TimeSplitsTrailResultWrapper(ActivityTrail activityTrail, IActivity activity, TrailResultInfo tri, int order)
            : base()
        {
            TrailResultInfo indexes = tri.CopyFromReference(activity);
            base.Element = new SplitsParentTrailResult(activityTrail, order, indexes, 0);
            this.getChildren();
        }
    }

    public class SwimSplitsTrailResultWrapper : SplitsTrailResultWrapper
    {
        public SwimSplitsTrailResultWrapper(ActivityTrail activityTrail, TrailResultInfo indexes, int order)
            : base()
        {
            base.Element = new SwimSplitsParentTrailResult(activityTrail, order, indexes, 0);
            this.getChildren();
        }
    }

    public class HighScoreTrailResultWrapper : TrailResultWrapper
    {
        //Create from HighScore, add the first and last time stamps in MarkedTimes
        public HighScoreTrailResultWrapper(ActivityTrail activityTrail, TrailResultInfo indexes, string tt, int order)
            : base()
        {
            base.Element = new HighScoreParentTrailResult(activityTrail, order, indexes, 0, tt);
        }
        //Special children, not part of the activity
        public void addChild(TrailResultInfo indexes, string tt, int order)
        {
            HighScoreParentTrailResult ptr = this.Result as HighScoreParentTrailResult;
            ChildTrailResult ctr = ptr.getChild(order, indexes, 0, tt);
            TrailResultWrapper child = new TrailResultWrapper(this, ctr);
        }
    }

    public class TrailResultWrapper : TreeList.TreeListNode, IComparable
    {
        //Parent
        protected TrailResultWrapper()
            : base(null, null)
        { }

        //Child
        internal TrailResultWrapper(TrailResultWrapper parent, ChildTrailResult element)
            : base(parent, element)
        {
            //several separate substructues..
            parent.Children.Add(this);
            parent.m_allChildren.Add(this);
        }

        public TrailResult Result
        {
            get
            {
                return (TrailResult)this.Element;
            }
        }

        //TODO: Calculate children when needed, by implementing Children
        //This is currently called after all parent results have been determined
        //A good enough reason is that this will give main activities separate colors, in the intended order
        protected void getChildren()
        {
            if (this.Result != null && this.Result is ParentTrailResult)
            {
                ParentTrailResult ptr = this.Result as ParentTrailResult;
                IList<ChildTrailResult> children = ptr.getChildren();
                if (children != null && children.Count > 1)
                {
                    foreach (ChildTrailResult tr in children)
                    {
                        TrailResultWrapper tn = new TrailResultWrapper(this, tr);
                    }
                }
            }
        }

        public void Sort()
        {
            if (m_allChildren.Count > 0)
            {
                //Sorting children directly fails- save original items
                ((List<TrailResultWrapper>)m_allChildren).Sort();
                this.Children.Clear();
                foreach (TrailResultWrapper tn in m_allChildren)
                {
                    if (!TrailsPlugin.Data.Settings.RestIsPause || tn.Result.Duration.TotalSeconds > 1)
                    {
                        this.Children.Add(tn);
                    }
                }
            }
        }

        public bool RemoveChildren(IList<TrailResultWrapper> tn)
        {
            bool result = false;
            foreach (TrailResultWrapper tr in tn)
            {
                if (this.m_allChildren.Contains(tr))
                {
                    this.m_allChildren.Remove(tr);
                    result = true;
                }
                //May not be needed as Children are added when sorting
                if (this.Children.Contains(tr))
                {
                    this.Children.Remove(tr);
                }
                if (this.Result != null && this.Result is ParentTrailResult)
                {
                    (this.Result as ParentTrailResult).RemoveChildren(tr);
                }
            }
            return result;
        }

        protected static IList<TrailResult> GetTrailResults(IList<TrailResultWrapper> tn, bool includeChildren)
        {
            IList<TrailResult> result = new List<TrailResult>();
            if (tn != null)
            {
                foreach (TrailResultWrapper tnp in tn)
                {
                    if (!(tnp.Result is PausedChildTrailResult))
                    {
                        result.Add(tnp.Result);
                    }
                    if (includeChildren)
                    {
                        foreach (TrailResultWrapper tnc in tnp.Children)
                        {
                            if (!result.Contains(tnc.Result) && !(tnc.Result is PausedChildTrailResult))
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
                    if (!result.Contains(tnp.Result.Activity))
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
                //This should not be needed, but a crash when developing occurred here set breakpoint
                try
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
                                foreach (TrailResultWrapper tnc in tnp.m_allChildren)
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
                catch { }
            }
            return result;
        }

        //The Children may not include all (hide paused laps, deleted etc). These are all results
        internal IList<TrailResultWrapper> m_allChildren = new List<TrailResultWrapper>();

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
