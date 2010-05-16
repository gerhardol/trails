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
using System.Xml;
using System.Collections.Generic;
using TrailsPlugin.UI.Activity;

namespace TrailsPlugin.Data {
	class Settings {

        public Settings()
        {
            m_activityPageNumFixedColumns = 1;
            m_defaultRadius = 20;
            m_activityPageColumns = new List<string>();
            m_xAxisValue = TrailLineChart.XAxisValue.Distance;
            m_chartType = TrailLineChart.LineChartTypes.Speed;
        }
        private IList<string> m_activityPageColumns = new List<string>();
		private int m_activityPageNumFixedColumns;
		private float m_defaultRadius;
		private TrailLineChart.XAxisValue m_xAxisValue;
        private TrailLineChart.LineChartTypes m_chartType;

        //Note: The data structures need restructuring...
        //Temporary hack
		public TrailLineChart.LineChartTypes ChartType {
			get {
				return m_chartType;
			}
			set {
				m_chartType = value;
				PluginMain.WriteExtensionData();
			}
		}
        public string ChartTypeString(TrailLineChart.LineChartTypes x)
        {
            return TrailLineChart.LineChartTypesString((TrailLineChart.LineChartTypes)x);
        }

		public TrailLineChart.XAxisValue XAxisValue {
			get {
				return m_xAxisValue;
			}
			set {
				m_xAxisValue = value;
				PluginMain.WriteExtensionData();
			}
		}
        public string XAxisValueString(TrailLineChart.XAxisValue x)
        {
            return TrailLineChart.XAxisValueString((TrailLineChart.XAxisValue)x);
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

			XmlNode settingsNode = pluginNode.SelectSingleNode("Settings");
            if (settingsNode != null && settingsNode.SelectSingleNode("@defaultRadius") != null)
            {
				m_defaultRadius = float.Parse(settingsNode.SelectSingleNode("@defaultRadius").Value);
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
					m_xAxisValue = (TrailLineChart.XAxisValue)Enum.Parse(typeof(TrailLineChart.XAxisValue),activityPageNode.SelectSingleNode("@xAxis").Value);
				}
				if (activityPageNode.SelectSingleNode("@chartType") != null) {
					m_chartType = (TrailLineChart.LineChartTypes)Enum.Parse(typeof(TrailLineChart.LineChartTypes),activityPageNode.SelectSingleNode("@chartType").Value);
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
            if (activityPageNode.Attributes["numFixedColumns"] == null)
            {
                activityPageNode.Attributes.Append(doc.CreateAttribute("numFixedColumns"));
            }
            activityPageNode.Attributes["numFixedColumns"].Value = m_activityPageNumFixedColumns.ToString();
            if (activityPageNode.Attributes["xAxis"] == null)
            {
                activityPageNode.Attributes.Append(doc.CreateAttribute("xAxis"));
            }
            activityPageNode.Attributes["xAxis"].Value = m_xAxisValue.ToString();
            if (activityPageNode.Attributes["chartType"] == null)
            {
                activityPageNode.Attributes.Append(doc.CreateAttribute("chartType"));
            }
            activityPageNode.Attributes["chartType"].Value = m_chartType.ToString();
            foreach (string columnName in m_activityPageColumns)
            {
				XmlNode column = doc.CreateElement("Column");
				column.AppendChild(doc.CreateTextNode(columnName));
				activityPageNode.AppendChild(column);
			}
			return settingsNode;
		}
	}
}
