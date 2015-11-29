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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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
                        string.IsNullOrEmpty(activity.Name));
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
                    //Set for non barometric devices only
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
                                    //Recalculate, to get "full" result
                                    tr = at.CalcTrailCompleteResult(activity);
                                }
                                else if (at.ResultTreeList.Count > 0)
                                {
                                    tr = at.ResultTreeList[0].Result;
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
