/*
Copyright (C) 2009 Brendan Doherty
Copyright (C) 2010-2014 Gerhard Olsson

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

    /*******************************************************/

    internal class ActivityTrailWrapper : TreeList.TreeListNode
    {
        public ActivityTrailWrapper(ActivityTrail trail)
            : base(null, trail)
        {
        }

        public ActivityTrail ActivityTrail
        {
            get
            {
                return (ActivityTrail)this.Element;
            }
        }

        public override string ToString()
        {
            return this.ActivityTrail.ToString() + ": " + this.Children.Count;
        }
    }

    internal class TrailNameWrapper
    {
        public IList<ActivityTrailWrapper> RowData = new List<ActivityTrailWrapper>();
        public System.Collections.IList Expanded = null;
        public System.Collections.IList SelectedItems = null;

        public TrailNameWrapper(IList<ActivityTrail> allActivityTrails, IList<ActivityTrail> selectedActivityTrails)
        {
            int expRows = 0;
            foreach (ActivityTrail at in allActivityTrails)
            {
                if (at.Trail.Children.Count > 0)
                {
                    expRows++;
                }
            }
            this.Expanded = new object[expRows];

            if(selectedActivityTrails != null)
            {
                this.SelectedItems = new object[selectedActivityTrails.Count];
            }

            IDictionary<Trail, ActivityTrailWrapper> ats = new Dictionary<Trail, ActivityTrailWrapper>();
            IList<ActivityTrailWrapper> all = new List<ActivityTrailWrapper>();
            int allIndex = 0;
            int selIndex = 0;

            foreach (ActivityTrail at in allActivityTrails)
            {
                ActivityTrailWrapper atw = new ActivityTrailWrapper(at);
                ats[at.Trail] = atw;
                all.Add(atw);
                if (at.Trail.Children.Count > 0)
                {
                    this.Expanded[allIndex++] = atw;
                }
                if (selectedActivityTrails != null && selectedActivityTrails.Contains(at))
                {
                    this.SelectedItems[selIndex++] = atw;
                }
            }

            IList<ActivityTrailWrapper> rowdata = new List<ActivityTrailWrapper>();
            foreach (ActivityTrailWrapper atw in all)
            {
                foreach (Trail t in atw.ActivityTrail.Trail.Children)
                {
                    if (ats.ContainsKey(t))
                    {
                        if (rowdata.Contains(ats[t]))
                        {
                            //Remove, included as subresult
                            rowdata.Remove(ats[t]);
                        }

                        atw.Children.Add(ats[t]);
                        ats[t].Parent = atw;
                    }
                }
                if (atw.Parent != null)
                {
                    //Not found as child (yet?)
                    rowdata.Add(atw);
                }
            }
/*            for (int i = 0; i < trees.Count; i++)
            {
                ActivityTrailWrapper atw = trees[i];
                ActivityTrail at = (ActivityTrail)atw.Element;
                foreach (Trail t in at.Trail.Children)
                {
                    if (ats.ContainsKey(t) && trees.Contains(ats[t]))
                    {
                        //Remove, included as subresult
                        trees.Remove(ats[t]);
                    }
                    atw.Children.Add(ats[t]);
                    ats[t].Parent = atw;
                }
            }
            */
            RowData = rowdata;
        }
    }

    public class TrailDropdownLabelProvider : TreeList.ILabelProvider
    {

        public System.Drawing.Image GetImage(object element, TreeList.Column column)
        {
            ActivityTrail t = ((ActivityTrailWrapper)element).ActivityTrail;
            //if (t.ActivityCount == 0)
            //{
            //    return Properties.Resources.square_blue;
            //}
            switch (t.Status)
            {
                case TrailOrderStatus.Match:
                    return Properties.Resources.square_green;
                case TrailOrderStatus.MatchNoCalc:
                    return Properties.Resources.square_green_check;
                case TrailOrderStatus.MatchPartial:
                    return Properties.Resources.square_green_minus;
                case TrailOrderStatus.InBoundNoCalc:
                    return Properties.Resources.square_green_plus;
                case TrailOrderStatus.InBoundMatchPartial:
                    return Properties.Resources.square_red_plus;
                case TrailOrderStatus.InBound:
                    return Properties.Resources.square_red;
                case TrailOrderStatus.NotInBound:
                    return Properties.Resources.square_blue;
                default: //NoConfiguration, NoInfo, NotInstalled
                    return null;
            }
        }

        public string GetText(object element, TreeList.Column column)
        {
            ActivityTrail t = ((ActivityTrailWrapper)element).ActivityTrail;
            string name = t.Trail.Name;
            if (t.Trail.IsReference && null != t.Trail.ReferenceActivity)
            {
                DateTime time = ActivityInfoCache.Instance.GetInfo(t.Trail.ReferenceActivity).ActualTrackStart;
                if (DateTime.MinValue == time)
                {
                    time = t.Trail.ReferenceActivity.StartTime;
                }
                name += " " + time.ToLocalTime().ToString();
            }

            if (t.Status == TrailOrderStatus.Match ||
                t.Status == TrailOrderStatus.MatchPartial)
            {
                int n = TrailResultWrapper.Results(t.ResultTreeList).Count;
                foreach (Trail tr in t.Trail.Children)
                {
                    n += TrailResultWrapper.Results(Controller.TrailController.Instance.GetActivityTrail(tr).ResultTreeList).Count;
                }
                name += " (" + n;
                if (t.Trail.IsURFilter && t.FilteredResults.Count > 0)
                {
                    name += " ," + t.FilteredResults.Count;
                }
                name += ")";
            }
            else if (t.Status == TrailOrderStatus.MatchNoCalc)
            {
                if (t.Trail.TrailType == Trail.CalcType.Splits ||
                    t.Trail.TrailType == Trail.CalcType.SplitsTime ||
                    t.Trail.TrailType == Trail.CalcType.UniqueRoutes)
                {
                    name += " (" + t.ActivityCount + ")";
                }
            }
            else if ((t.Status == TrailOrderStatus.InBoundMatchPartial) &&
                 t.m_noResCount.ContainsKey(t.Status))
            {
                name += " (" + t.m_noResCount[t.Status];
                if (t.m_noResCount.ContainsKey(TrailOrderStatus.InBound))
                {
                    name += ", " + t.m_noResCount[TrailOrderStatus.InBound];
                }
                name += ")";
            }
            //Other results
            else if (t.m_noResCount.ContainsKey(t.Status))
            {
                name += " (" + t.m_noResCount[t.Status] + ")";
            }
            return name;
        }
    }
}
