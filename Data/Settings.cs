using System.Xml;
using System.Collections.Generic;

namespace TrailsPlugin.Data {
	class Settings {

		private IList<string> m_activityPageColumns = new List<string>();
		private int m_activityPageNumFixedColumns;

		public IList<string> ActivityPageColumns {
			get {
				return m_activityPageColumns;
			}
			set {
				m_activityPageColumns = value;
			}
		}

		public int ActivityPageNumFixedColumns {
			get {
				return m_activityPageNumFixedColumns;
			}
			set {
				m_activityPageNumFixedColumns = value;
			}
		}

		public void FromXml(XmlNode pluginNode, XmlNamespaceManager nsmgr) {
			m_activityPageColumns.Clear();
			m_activityPageNumFixedColumns = 1;

			XmlNode settingsNode = pluginNode.SelectSingleNode("trail:Settings", nsmgr);
			XmlNode activityPageNode = null;
			if (settingsNode != null) {
				settingsNode.SelectSingleNode("trail:ActivityPage", nsmgr);
			}

			if (activityPageNode != null) {
				if (activityPageNode.SelectSingleNode("@numFixedColumns") != null) {
					m_activityPageNumFixedColumns = int.Parse(activityPageNode.SelectSingleNode("@numFixedColumns").Value);
				}
				foreach (XmlNode node in activityPageNode.SelectNodes("trail:Column", nsmgr)) {
					m_activityPageColumns.Add(node.InnerText);
				}
			} else {
				m_activityPageColumns.Add(UI.TrailResultColumnIds.Order);
				m_activityPageColumns.Add(UI.TrailResultColumnIds.StartTime);
				m_activityPageColumns.Add(UI.TrailResultColumnIds.Duration);
				m_activityPageColumns.Add(UI.TrailResultColumnIds.AvgHR);
				m_activityPageColumns.Add(UI.TrailResultColumnIds.AvgCadence);
			}
		}

		public XmlNode ToXml(XmlDocument doc) {
			XmlNode settingsNode = doc.CreateElement("Settings");
			XmlNode activityPageNode = doc.CreateElement("ActivityPage");
			settingsNode.AppendChild(activityPageNode);
			if(activityPageNode.Attributes["numFixedColumns"] == null) {
				activityPageNode.Attributes.Append(doc.CreateAttribute("numFixedColumns"));
			}
			activityPageNode.Attributes["numFixedColumns"].Value = m_activityPageNumFixedColumns.ToString();
			foreach (string columnName in m_activityPageColumns) {
				XmlNode column = doc.CreateElement("Column");
				column.AppendChild(doc.CreateTextNode(columnName));
				activityPageNode.AppendChild(column);
			}
			return settingsNode;
		}
	}
}
