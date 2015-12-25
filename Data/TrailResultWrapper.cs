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

using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;

namespace TrailsPlugin.Data
{
    public class TrailResultWrapper : TreeList.TreeListNode, IComparable
    {
        private TrailResultWrapper(TrailResultWrapper parent, TrailResult element)
            : base(parent, element)
        {
            element.Wrapper = this;
        }

        public TrailResultWrapper(SummaryTrailResult element)
            : this(null, element)
        {
        }

        public TrailResultWrapper(ParentTrailResult element)
            : this(null, element)
        {
            getChildren();
        }

        private void getChildren()
        {
            //TODO This should be lazy (calculated if children visible), 
            //requires overiding Children
            if (this.Element != null && this.Element is ParentTrailResult)
            {
                IList<ChildTrailResult> children = (this.Element as ParentTrailResult).getChildren();
                if (children != null && (children.Count > 1 ||
                    children.Count == 1 && children[0].SubResults.Count > 0))
                {
                    foreach (ChildTrailResult tr in children)
                    {
                        TrailResultWrapper tn = new TrailResultWrapper(this, tr);
                    }
                }
            }
        }

        public void updateIndexes(TrailResultInfo indexes)
        {
            this.Children.Clear();
            this.m_allChildren.Clear();
            this.Result.updateIndexes(indexes);
            this.getChildren();
        }

        public TrailResultWrapper(HighScoreParentTrailResult element)
            : this(null, element)
        {
        }

        public TrailResultWrapper(TrailResultWrapper parent, ChildTrailResult element)
            : base(parent, element)
        {
            element.Wrapper = this;
            //several separate substructues..
            parent.Children.Add(this);
            parent.m_allChildren.Add(this);

            if (element.SubResults.Count > 1)
            {
                foreach (ChildTrailResult sctr in element.SubResults)
                {
                    TrailResultWrapper strw = new TrailResultWrapper(this, sctr);
                }
            }
        }

        public TrailResult Result
        {
            get
            {
                return (TrailResult)this.Element;
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

        private static IList<TrailResultWrapper> GetUnpausedResults(IList<TrailResultWrapper> tn, bool includeChildren)
        {
            IList<TrailResultWrapper> result = new List<TrailResultWrapper>();
            if (tn != null)
            {
                foreach (TrailResultWrapper tnp in tn)
                {
                    if (!(tnp.Result is PausedChildTrailResult))
                    {
                        result.Add(tnp);
                    }
                    if (includeChildren)
                    {
                        foreach (TrailResultWrapper tnc in tnp.Children)
                        {
                            if (!result.Contains(tnc) && !(tnc.Result is PausedChildTrailResult))
                            {
                                result.Add(tnc);
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static IList<TrailResultWrapper> UnpausedResults(IList<TrailResultWrapper> tn)
        {
            return TrailResultWrapper.GetUnpausedResults(tn, false);
        }

        //Convert from TrailResult to TrailResultWrapper
        public static IList<TrailResultWrapper> Results(IList<TrailResult> tn)
        {
            IList<TrailResultWrapper> results = new List<TrailResultWrapper>();
            foreach(TrailResult t in tn)
            {
                if (t.Wrapper != null)
                {
                    results.Add(t.Wrapper);
                }
            }
            return results;
        }

        /// <summary>
        /// Return only Parent results (not the Splits/SubResults)
        /// </summary>
        public static IList<TrailResult> TrailResults(IList<TrailResultWrapper> tn)
        {
            IList<TrailResult> result = new List<TrailResult>();
            foreach (TrailResultWrapper tnp in tn)
            {
                result.Add(tnp.Result);
            }
            return result;
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
        
        public static IList<TrailResultWrapper> WrapperIList(System.Collections.IList tlist)
        {
            IList<TrailResultWrapper> aTr = new List<TrailResultWrapper>();
            if (tlist != null)
            {
                foreach (object t in tlist)
                {
                    if (t != null && t is TrailResultWrapper)
                    {
                        aTr.Add(((TrailResultWrapper)t));
                    }
                }
            }
            return aTr;
        }

        //The Children may not include all (hide paused laps, deleted etc). These are all results
        private IList<TrailResultWrapper> m_allChildren = new List<TrailResultWrapper>();
        public IList<TrailResultWrapper> AllChildren {  get { return m_allChildren; } }

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
