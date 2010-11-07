/*
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
        public static Dictionary<string, List<ITrailResult>> GetTrailsResultsForActivity(IActivity activity)
        {
            var results = new Dictionary<string, List<ITrailResult>>();

            foreach (Trail trail in PluginMain.Data.AllTrails.Values)
            {
                var activityTrail = new ActivityTrail(new List<IActivity>{activity}, trail);

                List<ITrailResult> trailResults = new List<ITrailResult>();
                foreach (var result in activityTrail.Results)
                {
                    trailResults.Add(result);
                }

                results.Add(trail.Name, trailResults);
            }

            return results;
        }
    }
}
