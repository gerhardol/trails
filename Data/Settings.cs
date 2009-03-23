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

using System.Xml;
using System.Collections.Generic;

namespace TrailsPlugin.Data {
	class Settings {

		private IList<string> m_activityPageColumns = new List<string>();
		private int m_activityPageNumFixedColumns;
		private float m_defaultRadius;
		private UI.Activity.TrailLineChart.XAxisValue m_xAxisValue = TrailsPlugin.UI.Activity.TrailLineChart.XAxisValue.Distance;
		private UI.Activity.TrailLineChart.LineChartTypes m_chartType = TrailsPlugin.UI.Activity.TrailLineChart.LineChartTypes.Cadence;

		public UI.Activity.TrailLineChart.LineChartTypes ChartType {
			get {
				return m_chartType;
			}
			set {
				m_chartType = value;
				PluginMain.WriteExtensionData();
			}
		}

		public UI.Activity.TrailLineChart.XAxisValue XAxisValue {
			get {
				return m_xAxisValue;
			}
			set {
				m_xAxisValue = value;
				PluginMain.WriteExtensionData();
			}
		}

		public IList<string> ActivityPageColumns {
			get {
				return m_activityPageColumns;
			}
			set {
				m_activityPageColumns = value;
				PluginMain.WriteExtensionData();
			}
		}

		public int ActivityPageNumFixedColumns {
			get {
				return m_activityPageNumFixedColumns;
			}
			set {
				m_activityPageNumFixedColumns = value;
				PluginMain.WriteExtensionData();
			}
		}

		public float DefaultRadius {
			get {
				return m_defaultRadius;
			}
			set {
				m_defaultRadius = value;
				PluginMain.WriteExtensionData();
			}
		}

		public void FromXml(XmlNode pluginNode) {
			m_activityPageColumns.Clear();
			m_activityPageNumFixedColumns = 1;
			m_defaultRadius = 20;

			XmlNode settingsNode = pluginNode.SelectSingleNode("Settings");
			if (settingsNode.SelectSingleNode("@defaultRadius") != null) {
				m_defaultRadius = int.Parse(settingsNode.SelectSingleNode("@defaultRadius").Value);
			}

			XmlNode activityPageNode = null;
			if (settingsNode != null) {
				activityPageNode = settingsNode.SelectSingleNode("ActivityPage");
			}

			if (activityPageNode != null) {
				if (activityPageNode.SelectSingleNode("@numFixedColumns") != null) {
					m_activityPageNumFixedColumns = int.Parse(activityPageNode.SelectSingleNode("@numFixedColumns").Value);
				}
				if (activityPageNode.SelectSingleNode("@xAxis") != null) {
					m_xAxisValue = (UI.Activity.TrailLineChart.XAxisValue)int.Parse(activityPageNode.SelectSingleNode("@xAxis").Value);
				}
				if (activityPageNode.SelectSingleNode("@chartType") != null) {
					m_chartType = (UI.Activity.TrailLineChart.LineChartTypes)int.Parse(activityPageNode.SelectSingleNode("@chartType").Value);
				}
				foreach (XmlNode node in activityPageNode.SelectNodes("Column")) {
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
			if (settingsNode.Attributes["defaultRadius"] == null) {
				settingsNode.Attributes.Append(doc.CreateAttribute("defaultRadius"));
			}
			settingsNode.Attributes["defaultRadius"].Value = m_defaultRadius.ToString();			

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
