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

		public void ReadOptions(System.Xml.XmlDocument xmlDoc, System.Xml.XmlNamespaceManager nsmgr, System.Xml.XmlElement pluginNode) {

			TrailSettings.Instance.AllTrails.Clear();
			if (pluginNode.ChildNodes.Count > 0) {
				foreach (XmlNode node in pluginNode.FirstChild.ChildNodes) {
					Data.Trail trail = Data.Trail.FromXml(node);
					TrailSettings.Instance.AllTrails.Add(trail.Name, trail);
				}
			}
		}

		public string Version {
			get { return GetType().Assembly.GetName().Version.ToString(4); }
		}

		public void WriteOptions(System.Xml.XmlDocument xmlDoc, System.Xml.XmlElement pluginNode) {

			XmlNode trails = xmlDoc.CreateElement("Trails");
			pluginNode.AppendChild(trails);
			foreach (Data.Trail trail in TrailSettings.Instance.AllTrails.Values) {
				trails.AppendChild(trail.ToXml(xmlDoc));
			}
			pluginNode.AppendChild(trails);
		}

		#endregion

		public static IApplication GetApplication() {
			return m_App;
		}

		void AppPropertyChanged(object sender, PropertyChangedEventArgs e) {
		}

		private static IApplication m_App = null;
	}
}
