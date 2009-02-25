using System.Collections.Generic;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ZoneFiveSoftware.Common.Visuals.Fitness.GPS;

namespace TrailsPlugin.UI.MapLayers {
	class ExtendMapControlLayers : IExtendMapControlLayers {

		#region IExtendMapControlLayers Members

		public IList<IMapControlLayer> MapLayers(IMapControl mapControl) {
			MapControlLayer layer = MapControlLayer.Instance;
			layer.MapControl = mapControl;
			return new IMapControlLayer[] { layer };
		}

		#endregion
	}
}

