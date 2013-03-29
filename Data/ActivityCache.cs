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
using System.Text;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;

namespace TrailsPlugin.Data
{
    public static class ActivityCache
    {
        private static IDictionary<IActivity, IGPSBounds> activityGps = new Dictionary<IActivity, IGPSBounds>();

        public static IGPSBounds GpsBoundsCache(IActivity activity)
        {
            if (!activityGps.ContainsKey(activity))
            {
                //activityGps controls if the property listener is added
                activity.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Activity_PropertyChanged);
                activityGps.Add(activity, null);
            }
            if (activityGps[activity] == null)
            {
                activityGps[activity] = GPSBounds.FromGPSRoute(activity.GPSRoute);
            }
            return activityGps[activity];
        }

        public static void ClearGpsBoundsCache()
        {
            foreach (IActivity activity in activityGps.Keys)
            {
                activity.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(Activity_PropertyChanged);
            }
            activityGps.Clear();
        }

        static void Activity_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //e is null at reset. For other this is called multiple times - only run once
            if (sender is IActivity && (e == null || e.PropertyName == "GPSRoute"))
            {
                IActivity activity = sender as IActivity;
                if (activity != null && activityGps.ContainsKey(activity))
                {
                    activityGps[activity] = null;
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