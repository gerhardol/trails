﻿/*
Copyright (C) 2010 Gerhard Olsson
Copyright (C) 2010 Peter Furucz

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

using System.Collections.Generic;
using ZoneFiveSoftware.Common.Data.Fitness;
using ITrailExport;
using TrailsPlugin.Data;

namespace TrailsPlugin.Export
{
    public static class Integration
    {
        //CalculatedFields integration
        public static Dictionary<string, List<ITrailResult>> GetTrailsResultsForActivities(IList<IActivity> activities)
        {
            var results = new Dictionary<string, List<ITrailResult>>();

            Controller.TrailController.Instance.Activities = activities;
            foreach (Trail trail in Data.TrailData.AllTrails.Values)
            {
                var activityTrail = new ActivityTrail(trail);

                List<ITrailResult> trailResults = new List<ITrailResult>();
                foreach (TrailResult tr in TrailResultWrapper.TrailResults(activityTrail.Results))
                {
                    if (!(tr is Data.PausedChildTrailResult))
                    {
                        trailResults.Add(new CFTrailResult(tr));
                    }
                }

                results.Add(trail.Name, trailResults);
            }

            return results;
        }

        public static Dictionary<string, List<ITrailResult>> GetTrailsResultsForActivity(IActivity activity)
        {
            return GetTrailsResultsForActivities(new List<IActivity> { activity });
        }


        //Matrix integration
        //Should use common library and data structures
        public static IList<IList<string[]>> ListTrails()
        {
            IList<IList<string[]>> result = new List<IList<string[]>>();
            foreach (Data.Trail trail in Data.TrailData.AllTrails.Values)
            {
                if (!trail.Generated)
                {
                    IList<string[]> tl = new List<string[]>();
                    foreach (Data.TrailGPSLocation trailpoint in trail.TrailLocations)
                    {
                        string[] t = {trail.Name, 
                                   trail.Radius.ToString(),
                                   trailpoint.LatitudeDegrees.ToString(),
                                   trailpoint.LongitudeDegrees.ToString(),
                                   trailpoint.Name};
                        tl.Add(t);
                    }
                    result.Add(tl);
                }
            }
            return result;
        }
    }
}
