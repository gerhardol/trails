/*
Copyright (C) 2007, 2009 Gerhard Olsson 

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

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using TrailsPlugin.Data;

namespace TrailsPlugin.DataImport
{
    class ExtendDataImporters : IExtendDataImporters
    {
        #region IExtendDataImporters Members

        public IList<IFileImporter> FileImporters
        {
            get
            {
                return null;
            }
        }

        public void BeforeImport(IList items)
        {
            foreach (object item in items)
            {
                if (item is IActivity)
                {
                    IActivity activity = (IActivity)item;
                    bool setName = (TrailsPlugin.Data.Settings.SetNameAtImport &&
                        (string.IsNullOrEmpty(activity.Name) ||
                        //Obvious timestamps
                        Regex.IsMatch(activity.Name, "^\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{1,2}:\\d{1,2}Z$") ||
                        Regex.IsMatch(activity.Name, "^\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{1,2}$")
                        ));
                    if (setName)
                    {
                        //Do not keep selection, sort find best
                        Controller.TrailController.Instance.SetActivities(new List<IActivity> { activity }, false);

                        foreach (ActivityTrail at in Controller.TrailController.Instance.OrderedTrails())
                        {
                            if (at.Status <= TrailOrderStatus.MatchPartial &&
                                !at.Trail.Generated && (at.Trail.TrailLocations.Count > 0))
                            {
                                activity.Name = at.Trail.Name;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void AfterImport(IList added, IList updated)
        {
            //Only added, not updated
            foreach (object item in added)
            {
                if (item is IActivity)
                {
                    IActivity activity = (IActivity)item;
                    //Set for barometric devices only
                    bool setEle = false;
                    if (TrailsPlugin.Data.Settings.SetAdjustElevationAtImport)
                    {
                        foreach (string devName in Settings.BarometricDevices)
                        {
                            if (activity.Metadata.Source.Contains(devName))
                            {
                                setEle = true;
                                break;
                            }
                        }
                    }

                    if (setEle)
                    {
                        //Do not keep selection, sort find best
                        Controller.TrailController.Instance.SetActivities(new List<IActivity> { activity }, false);

                        foreach (ActivityTrail at in Controller.TrailController.Instance.OrderedTrails())
                        {
                            if (at.Trail.IsSplits)
                            {
                                at.CalcResults(null);
                            }
                            if (at.Status == TrailOrderStatus.Match)
                            {
                                TrailResult tr = null;
                                if (!at.Trail.IsCompleteActivity)
                                {
                                    //Recalculate, to get "full" first result
                                    IList<TrailResultWrapper> ats = at.CalcTrailCompleteResult(activity);
                                    if(ats.Count > 0)
                                    {
                                        tr = ats[0].Result;
                                    }
                                }
                                else if (at.Results.Count > 0)
                                {
                                    tr = at.Results[0].Result;
                                }
                                if (tr != null)
                                {
                                    tr.SetDeviceElevation(true);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
