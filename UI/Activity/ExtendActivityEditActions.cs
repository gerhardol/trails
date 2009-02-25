using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using System.Collections.Generic;


namespace TrailsPlugin.UI.Activity {
	internal class ExtendActivityEditActions : IExtendActivityEditActions {
		public IList<IAction> GetActions(IList<IActivity> activities) {
			return new IAction[] { /*new UI.Actions.Action(activities)*/ };
		}

		public IList<IAction> GetActions(IActivity activity) {
			return new IAction[] { /*new UI.Actions.Action(new IActivity[] { activity })*/ };
		}
	}
}
