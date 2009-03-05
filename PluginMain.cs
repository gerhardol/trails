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

		public void ReadOptions(System.Xml.XmlDocument xmlDoc, XmlNamespaceManager nsmgr, System.Xml.XmlElement pluginNode) {

			nsmgr.AddNamespace("trail", "urn:uuid:D0EB2ED5-49B6-44e3-B13C-CF15BE7DD7DD");
			m_data.FromXml(pluginNode, nsmgr);
			m_settings.FromXml(pluginNode, nsmgr);
		}

		public string Version {
			get { return GetType().Assembly.GetName().Version.ToString(4); }
		}

		public void WriteOptions(System.Xml.XmlDocument xmlDoc, System.Xml.XmlElement pluginNode) {	
			pluginNode.AppendChild(m_data.ToXml(xmlDoc));
			pluginNode.AppendChild(m_settings.ToXml(xmlDoc));
		}

		#endregion

		public static IApplication GetApplication() {
			return m_App;
		}

		void AppPropertyChanged(object sender, PropertyChangedEventArgs e) {
		}

		private static IApplication m_App = null;
		private static Data.TrailData m_data = new Data.TrailData();
		private static Data.Settings m_settings = new Data.Settings();
		public static Data.TrailData Data {
			get {
				return m_data;
			}
		}

		public static Data.Settings Settings {
			get {
				return m_settings;
			}
		}
	}
}
