using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using System.Collections.Generic;


namespace TrailsPlugin {
	internal class ActivityEdit : IExtendActivityEditActions {
		public IList<IAction> GetActions(IList<IActivity> activities) {
			return new IAction[] { new Action(activities) };
		}

		public IList<IAction> GetActions(IActivity activity) {
			return new IAction[] { new Action(new IActivity[] { activity }) };
		}
	}
}
