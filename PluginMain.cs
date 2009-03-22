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

using System.ComponentModel;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using System.Xml;

namespace TrailsPlugin {
	class PluginMain : IPlugin {
		#region IPlugin Members

		public IApplication Application {
			set {
				if (m_App != null) {
					m_App.PropertyChanged -= new PropertyChangedEventHandler(AppPropertyChanged);
				}

				m_App = value;

				if (m_App != null) {
					m_App.PropertyChanged += new PropertyChangedEventHandler(AppPropertyChanged);
				}
			}
		}

		public System.Guid Id {
			get {
				return GUIDs.PluginMain;
			}
		}

		public string Name {
			get {
				return PluginView.GetLocalizedString("TrailsPluginName");
			}
		}

		public string Version {
			get { return GetType().Assembly.GetName().Version.ToString(4); }
		}

		public void ReadOptions(XmlDocument xmlDoc, XmlNamespaceManager nsmgr, XmlElement pluginNode) {			
		}

		public void WriteOptions(XmlDocument xmlDoc, XmlElement pluginNode) {
			
		}

		#endregion

		public static void ReadExtensionData() {
			m_data = new TrailsPlugin.Data.TrailData();
			m_settings = new TrailsPlugin.Data.Settings();

			XmlDocument doc = new XmlDocument();
			string xml = PluginMain.GetApplication().Logbook.GetExtensionText(GUIDs.PluginMain);
			if (xml == "") {
				xml = "<TrailsPlugin/>";
			}
			doc.LoadXml(xml);
			m_data.FromXml(doc.DocumentElement);
			m_settings.FromXml(doc.DocumentElement);			
		}

		public static void WriteExtensionData() {
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<TrailsPlugin/>");
			doc.DocumentElement.AppendChild(m_data.ToXml(doc));
			doc.DocumentElement.AppendChild(m_settings.ToXml(doc));
			PluginMain.GetApplication().Logbook.SetExtensionText(GUIDs.PluginMain, doc.OuterXml);
			PluginMain.GetApplication().Logbook.Modified = true;
		}

		public static IApplication GetApplication() {
			return m_App;
		}

		void AppPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e != null && e.PropertyName == "Logbook") {
				m_settings = null;
				m_data = null;
			}

		}

		private static IApplication m_App = null;
		private static Data.TrailData m_data = null;
		private static Data.Settings m_settings = null;
		public static Data.TrailData Data {
			get {
				if (m_data == null) {
					PluginMain.ReadExtensionData();
				}
				return m_data;
			}
		}

		public static Data.Settings Settings {
			get {
				if (m_settings == null) {
					PluginMain.ReadExtensionData();
				}
				return m_settings;
			}
		}
	}
}
