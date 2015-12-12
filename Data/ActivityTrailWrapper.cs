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

        public static IList<ActivityTrail> toATList(IList<ActivityTrailWrapper> atws)
        {
            IList<ActivityTrail> ats = new List<ActivityTrail>();
            foreach (ActivityTrailWrapper atw in atws)
            {
                ats.Add(atw.ActivityTrail);
            }
            return ats;
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
            if(selectedActivityTrails != null)
            {
                this.SelectedItems = new object[selectedActivityTrails.Count];
            }

            IDictionary<Trail, ActivityTrailWrapper> ats = new Dictionary<Trail, ActivityTrailWrapper>();
            IList<ActivityTrailWrapper> children = new List<ActivityTrailWrapper>();
            IList<ActivityTrailWrapper> exps = new List<ActivityTrailWrapper>();
            IList<ActivityTrailWrapper> rows = new List<ActivityTrailWrapper>();
            int selIndex = 0;

            foreach (ActivityTrail at in allActivityTrails)
            {
                ActivityTrailWrapper atw = new ActivityTrailWrapper(at);
                ats[at.Trail] = atw;
                if (at.Trail.Parent == null)
                {
                    rows.Add(atw);
                }
                else
                {
                    //added as links to the parent rows below
                    children.Add(atw);
                }
                if (at.Trail.Children.Count > 0 && at.Status < TrailOrderStatus.NotInBound)
                {
                    //expand only for (partial) matches
                    exps.Add(atw);
                }
                if (selectedActivityTrails != null && selectedActivityTrails.Contains(at))
                {
                    this.SelectedItems[selIndex++] = atw;
                }
            }

            foreach (ActivityTrailWrapper atw in children)
            {
                //Update link to and from parent
                Trail parentResult = atw.ActivityTrail.Trail.Parent;
                if (parentResult != null)
                {
                    ActivityTrailWrapper parentWrapper = ats[parentResult];
                    atw.Parent = parentWrapper;
                    parentWrapper.Children.Add(atw);
                }
            }
            RowData = rows;

            this.Expanded = new object[exps.Count];
            int allIndex = 0;
            foreach (ActivityTrailWrapper atw in exps)
            {
                this.Expanded[allIndex++] = atw;
            }
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
            ActivityTrail at = ((ActivityTrailWrapper)element).ActivityTrail;
            string name = at.Trail.Name;
            if (at.Trail.Parent != null)
            {
                //Remove parent part, colon/space is flexible
                name = name.Substring(at.Trail.Parent.Name.Length+1).TrimStart(' ',':');
            }
            if (at.Trail.IsReference && null != at.Trail.ReferenceActivity)
            {
                DateTime time = ActivityInfoCache.Instance.GetInfo(at.Trail.ReferenceActivity).ActualTrackStart;
                if (DateTime.MinValue == time)
                {
                    time = at.Trail.ReferenceActivity.StartTime;
                }
                name += " " + time.ToLocalTime().ToString();
            }

            if (at.Status == TrailOrderStatus.Match ||
                at.Status == TrailOrderStatus.MatchPartial)
            {
                int n = at.Results.Count;
                foreach (Trail t2 in at.Trail.AllChildren)
                {
                    n += Controller.TrailController.Instance.GetActivityTrail(t2).Results.Count;
                }
                name += " (" + n;
                if (at.Trail.IsURFilter && at.FilteredResults.Count > 0)
                {
                    name += " ," + at.FilteredResults.Count;
                }
                name += ")";
            }
            else if (at.Status == TrailOrderStatus.MatchNoCalc)
            {
                if (at.Trail.TrailType == Trail.CalcType.Splits ||
                    at.Trail.TrailType == Trail.CalcType.UniqueRoutes)
                {
                    name += " (" + at.ActivityCount + ")";
                }
            }
            else if ((at.Status == TrailOrderStatus.InBoundMatchPartial) &&
                 at.m_noResCount.ContainsKey(at.Status))
            {
                name += " (" + at.m_noResCount[at.Status];
                if (at.m_noResCount.ContainsKey(TrailOrderStatus.InBound))
                {
                    name += ", " + at.m_noResCount[TrailOrderStatus.InBound];
                }
                name += ")";
            }
            //Other results
            else if (at.m_noResCount.ContainsKey(at.Status))
            {
                name += " (" + at.m_noResCount[at.Status] + ")";
            }
            return name;
        }
    }
}
