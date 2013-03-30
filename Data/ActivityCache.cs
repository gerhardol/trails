/*
Copyright (C) 2010-2013 Gerhard Olsson

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
using System.Diagnostics;
using System.Text;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;

namespace TrailsPlugin.Data
{
    public static class ActivityCache
    {
        private static IList<IActivity> activityPropLists = new List<IActivity>();
        //If an activity exists in the specific caches, it must exist in the propList too
        private static IDictionary<IActivity, IGPSBounds> activityGpss = new Dictionary<IActivity, IGPSBounds>();
        private static IDictionary<IActivity, IDictionary<bool, ActivityInfo>> activityInfos = new Dictionary<IActivity, IDictionary<bool, ActivityInfo>>();

        public static IGPSBounds GpsBoundsCache(IActivity activity)
        {
            if (!activityGpss.ContainsKey(activity))
            {
                if (!activityPropLists.Contains(activity))
                {
                    //activityGps controls if the property listener is added
                    activity.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Activity_PropertyChanged);
                    activityPropLists.Add(activity);
                }
                activityGpss.Add(activity, null);
            }
            if (activityGpss[activity] == null)
            {
                activityGpss[activity] = GPSBounds.FromGPSRoute(activity.GPSRoute);
            }
            return activityGpss[activity];
        }

        public static ActivityInfo GetActivityInfo(IActivity activity, bool includeStopped)
        {
            if (activity == null)
            {
                Debug.Assert(false);
                return null;
            }

            if (!activityInfos.ContainsKey(activity))
            {
                if (!activityPropLists.Contains(activity))
                {
                    //activityGps controls if the property listener is added
                    activity.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Activity_PropertyChanged);
                    activityPropLists.Add(activity);
                }
                activityInfos.Add(activity, null);
            }
            //includeStopped changes could be catched from changing system preferences and the activities too
            //xxx check include includeSopped as key
            if (activityInfos[activity] == null)
            {
                activityInfos[activity] = new Dictionary<bool, ActivityInfo>();
            }
            if (!activityInfos[activity].ContainsKey(includeStopped))
            {
                activityInfos[activity].Add(includeStopped, null);
            }
            if (activityInfos[activity][includeStopped] == null)
            {
                ActivityInfoCache c = new ActivityInfoCache();
                ActivityInfoOptions t = new ActivityInfoOptions(false, includeStopped);
                c.Options = t;
                //if (this.Pauses != activity.TimerPauses)
                //{
                //    IActivity activity2 = Plugin.GetApplication().Logbook.Activities.AddCopy(activity);
                //    activity = activity2;
                //    activity.TimerPauses.Clear();
                //    foreach (IValueRange<DateTime> p in this.Pauses)
                //    {
                //        activity.TimerPauses.Add(p);
                //    }
                //    activity.Category = Plugin.GetApplication().Logbook.ActivityCategories[1];
                //    if (activity.Category.SubCategories.Count > 0)
                //    {
                //        activity.Category = activity.Category.SubCategories[0];
                //    }
                //}
                if (activity != null)
                {
                    activityInfos[activity][includeStopped] = c.GetInfo(activity);
                }
                else
                {
                    //TODO: This data should not be used, just return any activity to avoid exceptions
                    Debug.Assert(false);
                    activityInfos[activity][includeStopped] = ActivityInfoCache.Instance.GetInfo(Plugin.GetApplication().Logbook.Activities[0]);
                }
            }
            return activityInfos[activity][includeStopped];
        }

        public static void ClearActivityCache()
        {
            foreach (IActivity activity in activityPropLists)
            {
                activity.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(Activity_PropertyChanged);
            }
            activityPropLists.Clear();
            activityGpss.Clear();
            activityInfos.Clear();
        }

        static void Activity_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //e is null at reset. For other this is called multiple times - only run once
            if (sender is IActivity /*&& (e == null || e.PropertyName == "GPSRoute")*/)
            {
                IActivity activity = sender as IActivity;
                if (activity != null)
                {
                    if (activityGpss.ContainsKey(activity))
                    {
                        activityGpss[activity] = null;
                    }
                    if (activityInfos.ContainsKey(activity))
                    {
                        activityInfos[activity] = null;
                    }
                }
                //all trails must be recalculated, there is no possibility to recalculate for one trail only
                //(almost: Clear results for a certain activity followed by CalcTrail then sets status)
                //As activities are edit in single view normally, recalc time is not an issue
                //(except if results have been 
                //(if a user auto edits, there could be seconds of slowdown).
                Controller.TrailController.Instance.Reset();
                //Make sure reference activity is 'reasonable' in case the reference trail is selected
                Controller.TrailController.Instance.checkReferenceActivity(null);

                //Calculate trails - at least InBounds, set apropriate ActivityTrail
                Controller.TrailController.Instance.ReCalcTrails(false, null);
            }
        }

    }
}