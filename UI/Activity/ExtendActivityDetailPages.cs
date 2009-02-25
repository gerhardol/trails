using System.Collections.Generic;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace TrailsPlugin.UI.Activity {
	internal class ExtendActivityDetailPages : IExtendActivityDetailPages {
		#region IExtendActivityDetailPages Members

		public IList<IActivityDetailPage> ActivityDetailPages {
			get { return new IActivityDetailPage[] { new ActivityDetailPage() }; }
		}

		#endregion
	}
}
