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
using System.Globalization;
using TrailsPlugin.UI.Activity;

namespace TrailsPlugin.Data {
	class Settings {

        public Settings()
        {
            defaults();
        }
        private static void defaults()
        {
            m_activityPageNumFixedColumns = 1;
            m_defaultRadius = 20;
            m_activityPageColumns = new List<string>();
            m_xAxisValue = TrailLineChart.XAxisValue.Distance;
            m_chartType = TrailLineChart.LineChartTypes.Speed;

            m_activityPageColumns = new List<string>();
            m_activityPageColumns.Add(UI.TrailResultColumnIds.Order);
            m_activityPageColumns.Add(UI.TrailResultColumnIds.StartTime);
            m_activityPageColumns.Add(UI.TrailResultColumnIds.Duration);
            m_activityPageColumns.Add(UI.TrailResultColumnIds.AvgHR);
            m_activityPageColumns.Add(UI.TrailResultColumnIds.AvgCadence);
        }
        private static IList<string> m_activityPageColumns;
        private static int m_activityPageNumFixedColumns;
        private static float m_defaultRadius;
        private static TrailLineChart.XAxisValue m_xAxisValue;
        private static TrailLineChart.LineChartTypes m_chartType;

        //Note: The data structures need restructuring...
        //Temporary hack to translate to strings
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

        private class xmlTags
        {
            public const string sDefaultRadius = "sDefaultRadius";
            public const string sNumFixedColumns = "sNumFixedColumns";
            public const string sXAxis = "sXAxis";
            public const string sChartType = "sChartType";
            public const string sColumns = "sColumns";
        }

        public static void ReadOptions(XmlDocument xmlDoc, XmlNamespaceManager nsmgr, XmlElement pluginNode)
        {
            String attr;
            defaults();

            attr = pluginNode.GetAttribute(xmlTags.sDefaultRadius);
            if (attr.Length > 0) { m_defaultRadius = float.Parse(attr, NumberFormatInfo.InvariantInfo); }
            attr = pluginNode.GetAttribute(xmlTags.sNumFixedColumns);
            if (attr.Length > 0) { m_activityPageNumFixedColumns = (Int16)XmlConvert.ToInt16(attr); }
            attr = pluginNode.GetAttribute(xmlTags.sXAxis);
            if (attr.Length > 0) { m_xAxisValue = (TrailLineChart.XAxisValue)Enum.Parse(typeof(TrailLineChart.XAxisValue), attr, true); }
            attr = pluginNode.GetAttribute(xmlTags.sChartType);
            if (attr.Length > 0) { m_chartType = (TrailLineChart.LineChartTypes)Enum.Parse(typeof(TrailLineChart.LineChartTypes), attr, true); }
            attr = pluginNode.GetAttribute(xmlTags.sColumns);
            if (attr.Length > 0)
            {
                m_activityPageColumns.Clear();
                String[] values = attr.Split(';');
                foreach (String column in values)
                {
                    m_activityPageColumns.Add(column);
                }
            }
        }

        public static void WriteOptions(XmlDocument xmlDoc, XmlElement pluginNode)
        {
            pluginNode.SetAttribute(xmlTags.sDefaultRadius, XmlConvert.ToString(m_defaultRadius));
            pluginNode.SetAttribute(xmlTags.sNumFixedColumns, XmlConvert.ToString(m_activityPageNumFixedColumns));
            pluginNode.SetAttribute(xmlTags.sXAxis, m_xAxisValue.ToString());
            pluginNode.SetAttribute(xmlTags.sChartType, m_chartType.ToString());

            String colText = null;
            foreach (String column in m_activityPageColumns)
            {
                if (colText == null) { colText = column; }
                else { colText += ";" + column; }
            }
            pluginNode.SetAttribute(xmlTags.sColumns, colText);
        }

        //Old version, read from logbook
        public void FromXml(XmlNode pluginNode)
        {
			defaults();

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
                m_activityPageColumns.Clear();
                foreach (XmlNode node in activityPageNode.SelectNodes("Column"))
                {
					m_activityPageColumns.Add(node.InnerText);
				}
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
