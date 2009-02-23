using System.Collections.Generic;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace TrailsPlugin {
	internal class ActivityDetailsPages : IExtendActivityDetailPages {
		public IList<IActivityDetailPage> ActivityDetailPages {
			get { return new IActivityDetailPage[] { new ActivityDetailPage() }; }
		}
	}
}
