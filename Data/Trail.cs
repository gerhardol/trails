using System.Collections.Generic;
using ZoneFiveSoftware.Common.Data.GPS;

namespace TrailsPlugin.Data {
	class Trail {
		public string name;
		public IList<IGPSLocation> points;
	}
}
