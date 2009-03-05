using System.Collections.Generic;
using ZoneFiveSoftware.Common.Data.Fitness;
using System.Xml;
using System.Xml.Serialization;

namespace TrailsPlugin.Data {
	public class TrailData {

		private SortedDictionary<string, Data.Trail> m_AllTrails = new SortedDictionary<string, Data.Trail>();

		public SortedDictionary<string, Data.Trail> AllTrails {
			get {
				return m_AllTrails;
			}
		}

		public bool InsertTrail(Data.Trail trail) {
			if (m_AllTrails.ContainsKey(trail.Name)) {
				return false;
			} else {
				m_AllTrails.Add(trail.Name, trail);
				return true;
			}
		}
		public bool UpdateTrail(string oldKey, Data.Trail trail) {
			if (m_AllTrails.ContainsKey(oldKey)) {
				m_AllTrails.Remove(oldKey);
				m_AllTrails.Add(trail.Name, trail);
				return true;
			} else {				
				return false;
			}
		}
		public bool DeleteTrail(Data.Trail trail) {
			if (m_AllTrails.ContainsKey(trail.Name)) {
				m_AllTrails.Remove(trail.Name);
				return true;
			} else {
				return false;
			}
		}

		public void FromXml(XmlNode pluginNode, XmlNamespaceManager nsmgr) {
			m_AllTrails.Clear();
			foreach (XmlNode node in pluginNode.SelectNodes("trail:Trails/trail:Trail", nsmgr)) {
				Data.Trail trail = Data.Trail.FromXml(node, nsmgr);
				m_AllTrails.Add(trail.Name, trail);
			}

		}

		public XmlNode ToXml(XmlDocument doc) {
			XmlNode trails = doc.CreateElement("Trails");			
			foreach (Data.Trail trail in PluginMain.Data.AllTrails.Values) {
				trails.AppendChild(trail.ToXml(doc));
			}
			return trails;
		}
	}
}
