/*
Copyright (C) 2016 Gerhard Olsson

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
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ZoneFiveSoftware.Common.Data.Fitness;

namespace TrailsPlugin.UI.Edit
{
    class TrailsEdit :
#if ST_2_1
    IExtendActivityEditActions
#else
    IExtendDailyActivityViewActions, IExtendActivityReportsViewActions
#endif

    {

#if ST_2_1
        #region IExtendActivityEditActions Members

        public IList<IAction> GetActions(IList<IActivity> activities)
        {
            if (activities.Count > 0)
                return new IAction[] { new TrailsAction(activities) };
            return new IAction[] { };
        }

        public IList<IAction> GetActions(IActivity activity)
        {
            return new IAction[] { new TrailsAction(new IActivity[] { activity }) };
        }

        #endregion
#else
        #region IExtendDailyActivityViewActions Members
        public IList<IAction> GetActions(IDailyActivityView view,
                                         ExtendViewActions.Location location)
        {
            if (location == ExtendViewActions.Location.AnalyzeMenu)
            {
                return new IAction[] { new TrailsAction(view) };
            }
            else return new IAction[0];
        }

        public IList<IAction> GetActions(IActivityReportsView view,
                                         ExtendViewActions.Location location)
        {
            /* IActivityReportsView do not have Selection provider, cannot be used
            if (location == ExtendViewActions.Location.AnalyzeMenu)
            {
                return new IAction[] { new TrailsAction(view) };
            }
            else 
            */
            return new IAction[0];
        }
        #endregion
#endif
    }
}
