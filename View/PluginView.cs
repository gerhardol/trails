/******************************************************************************

    This file is part of TrailsPlugin.

    TrailsPlugin is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TrailsPlugin is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TrailsPlugin.  If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace TrailsPlugin {
	class PluginView {
		public static string GetLocalizedString(string name) {
			try {
				return ResourceManager.GetString(name);
			} catch {
				Debug.Assert(false, "Unable to find string resource named " + name);

				return String.Empty;
			}
		}

		public static ResourceManager ResourceManager {
			get { return m_ResourceManager; }
		}

		private static ResourceManager m_ResourceManager = new ResourceManager("TrailsPlugin.Resources.StringResources",
																			   Assembly.GetExecutingAssembly());
	}
}
