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
