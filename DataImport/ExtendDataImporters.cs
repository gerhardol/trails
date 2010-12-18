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
                    Controller.TrailController m_controller = Controller.TrailController.Instance;
                    if (TrailsPlugin.Data.Settings.SetNameAtImport &&
                        string.IsNullOrEmpty(activity.Name))
                    {
                        m_controller.Activities = new List<IActivity> { activity };
                        foreach (ActivityTrail at in m_controller.OrderedTrails)
                        {
                            if (at.status == TrailOrderStatus.Match &&
                                !at.Trail.Generated)
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
        }

        #endregion
    }
}
