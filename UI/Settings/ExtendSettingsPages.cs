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
