/*
Copyright (C) 2009 Brendan Doherty

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

using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using System.Collections.Generic;


namespace TrailsPlugin.UI.Activity {
	internal class ExtendActivityEditActions :
#if ST_2_1
    IExtendActivityEditActions
#else
     IExtendDailyActivityViewActions, IExtendActivityReportsViewActions
#endif
    {

        //Note: Unused right now

#if ST_2_1
        #region IExtendActivityEditActions Members
		public IList<IAction> GetActions(IList<IActivity> activities) {
			return new IAction[] { /*new UI.Actions.Action(activities)*/ };
		}

		public IList<IAction> GetActions(IActivity activity) {
			return new IAction[] { /*new UI.Actions.Action(new IActivity[] { activity })*/ };
		}
        #endregion
#else
        #region IExtendDailyActivityViewActions Members
        public IList<IAction> GetActions(IDailyActivityView view,
                                                 ExtendViewActions.Location location)
        {
            //if (location == ExtendViewActions.Location.AnalyzeMenu)
            //{
            //    return new IAction[] { new UI.Actions.Action(view) };
            //}
            //else
            return new IAction[0];
        }
        public IList<IAction> GetActions(IActivityReportsView view,
                                         ExtendViewActions.Location location)
        {
            //if (location == ExtendViewActions.Location.AnalyzeMenu)
            //{
            //    return new IAction[] { new UI.Actions.Action(view) };
            //}
            //else 
            return new IAction[0];
        }
        #endregion
#endif
    }
}
