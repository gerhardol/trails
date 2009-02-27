using System.Collections.Generic;
using ZoneFiveSoftware.Common.Data.GPS;

namespace TrailsPlugin.Data {
	public class Trail {
		public string name;
		public IList<TrailPoint> points = new List<TrailPoint>();
	}
}
