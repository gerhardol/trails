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

using System.Collections.Generic;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ZoneFiveSoftware.Common.Visuals;

namespace TrailsPlugin.UI.Settings {
	class ExtendSettingsPages : IExtendSettingsPages {
		#region IExtendSettingsPages Members

		System.Collections.Generic.IList<ISettingsPage> IExtendSettingsPages.SettingsPages {
			get {
				return new ISettingsPage[] { new SettingsPage() };
			}
		}

		#endregion
	}
}
