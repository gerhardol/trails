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

//Used in both Trails and Matrix plugin

#if ST_2_1
using System.Collections.Generic;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ZoneFiveSoftware.Common.Visuals.Fitness.GPS;

namespace TrailsPlugin.UI.MapLayers 
{
	class ExtendMapControlLayers : IExtendMapControlLayers
    {
		#region IExtendMapControlLayers Members
		public IList<IMapControlLayer> MapLayers(IMapControl mapControl) {
			MapControlLayer layer = MapControlLayer.Instance;
			layer.MapControl = mapControl;
			return new IMapControlLayer[] { layer };
        }
		#endregion
	}
}
#endif