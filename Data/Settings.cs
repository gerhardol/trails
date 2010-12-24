/*
Copyright (C) 2009 Brendan Doherty

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Xml;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
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
            m_xAxisValue = TrailLineChart.XAxisValue.Distance;
            m_chartType = TrailLineChart.LineChartTypes.SpeedPace;
            m_MultiChartTypes = TrailsPlugin.UI.Activity.TrailLineChart.DefaultLineChartTypes();

            m_activityPageColumns = TrailResultColumnIds.DefaultColumns();
            m_summaryViewSortColumn = TrailResultColumnIds.Order;
            m_summaryViewSortDirection = ListSortDirection.Ascending;
            m_ShowChartToolBar = true;
            m_SetNameAtImport = true;
            m_SelectSimilarResults = false;
            m_MaxAutoCalcActivitiesTrails = 150;
            m_MaxAutoCalcResults = 200;
        }
        private static IList<string> m_activityPageColumns;
        private static int m_activityPageNumFixedColumns;
        private static float m_defaultRadius;
        private static TrailLineChart.XAxisValue m_xAxisValue;
        private static TrailLineChart.LineChartTypes m_chartType;
        private static IList<TrailLineChart.LineChartTypes> m_MultiChartTypes;
        private static bool m_ShowChartToolBar;
        private static bool m_SelectSimilarResults;
        private static string m_summaryViewSortColumn;
        private static ListSortDirection m_summaryViewSortDirection;
        private static bool m_SetNameAtImport;
        private static int m_MaxAutoCalcActivitiesTrails;
        private static int m_MaxAutoCalcResults;

        //Note: The data structures need restructuring...
        //Temporary hack to translate to strings
        public static TrailLineChart.LineChartTypes ChartType
        {
            get
            {
                return m_chartType;
            }
            set
            {
                m_chartType = value;
                PluginMain.WriteExtensionData();
            }
        }
        public static IList<TrailLineChart.LineChartTypes> MultiChartType
        {
            get
            {
                return m_MultiChartTypes;
            }
        }
        public static TrailLineChart.LineChartTypes ToggleMultiChartType
        {
            set
            {
                if (m_MultiChartTypes.Contains(value))
                {
                    m_MultiChartTypes.Remove(value);
                }
                else
                {
                    m_MultiChartTypes.Add(value);
                }
            }
        }

        public static TrailLineChart.XAxisValue XAxisValue
        {
			get {
				return m_xAxisValue;
			}
			set {
				m_xAxisValue = value;
				PluginMain.WriteExtensionData();
			}
		}
        public static IList<string> ActivityPageColumns
        {
			get {
				return m_activityPageColumns;
			}
			set {
				m_activityPageColumns = value;
				PluginMain.WriteExtensionData();
			}
		}

        public static int ActivityPageNumFixedColumns
        {
			get {
				return m_activityPageNumFixedColumns;
			}
			set {
				m_activityPageNumFixedColumns = value;
				PluginMain.WriteExtensionData();
			}
		}

        public static float DefaultRadius
        {
			get {
				return m_defaultRadius;
			}
			set {
				m_defaultRadius = value;
				PluginMain.WriteExtensionData();
			}
		}
        public static bool ShowChartToolBar
        {
            get { return m_ShowChartToolBar; }
            set
            {
                m_ShowChartToolBar = value;
            }
        }
        public static bool SelectSimilarResults
        {
            get
            {
                return m_SelectSimilarResults;
            }
            set
            {
                m_SelectSimilarResults = value;
            }
        }

        public static string SummaryViewSortColumn
        {
            get { return m_summaryViewSortColumn; }
            set { m_summaryViewSortColumn = value; }
        }

        public static ListSortDirection SummaryViewSortDirection
        {
            get { return m_summaryViewSortDirection; }
            set { m_summaryViewSortDirection = value; }
        }
        public static bool SetNameAtImport
        {
            get { return m_SetNameAtImport; }
            set { m_SetNameAtImport = value; }
        }
        public static int MaxAutoCalcActivitiesTrails
        {
            get { return m_MaxAutoCalcActivitiesTrails; }
            set { m_MaxAutoCalcActivitiesTrails = value; }
        }
        public static int MaxAutoCalcResults
        {
            get { return m_MaxAutoCalcResults; }
            set { m_MaxAutoCalcResults = value; }
        }

        public static void ReadOptions(XmlDocument xmlDoc, XmlNamespaceManager nsmgr, XmlElement pluginNode)
        {
            defaults();

            String attr;
            attr = pluginNode.GetAttribute(xmlTags.sDefaultRadius);
            if (attr.Length > 0) { m_defaultRadius = Settings.parseFloat(attr); }
            attr = pluginNode.GetAttribute(xmlTags.sNumFixedColumns);
            if (attr.Length > 0) { m_activityPageNumFixedColumns = (Int16)XmlConvert.ToInt16(attr); }
            attr = pluginNode.GetAttribute(xmlTags.sXAxis);
            if (attr.Length > 0) { m_xAxisValue = (TrailLineChart.XAxisValue)Enum.Parse(typeof(TrailLineChart.XAxisValue), attr, true); }
            attr = pluginNode.GetAttribute(xmlTags.sChartType);
            if (attr.Length > 0) { m_chartType = (TrailLineChart.LineChartTypes)Enum.Parse(typeof(TrailLineChart.LineChartTypes), attr, true); }
            attr = pluginNode.GetAttribute(xmlTags.summaryViewSortColumn);
            if (attr.Length > 0) { m_summaryViewSortColumn = attr; }
            attr = pluginNode.GetAttribute(xmlTags.summaryViewSortDirection);
            if (attr.Length > 0) { m_summaryViewSortDirection = (ListSortDirection)Enum.Parse(typeof(ListSortDirection), attr); }
            attr = pluginNode.GetAttribute(xmlTags.SelectSimilarResults);
            if (attr.Length > 0) { m_SelectSimilarResults = XmlConvert.ToBoolean(attr); }
            attr = pluginNode.GetAttribute(xmlTags.ShowChartToolBar);
            if (attr.Length > 0) { m_ShowChartToolBar = XmlConvert.ToBoolean(attr); }
            attr = pluginNode.GetAttribute(xmlTags.MaxAutoCalcActivitiesTrails);
            if (attr.Length > 0) { m_MaxAutoCalcActivitiesTrails = (Int16)XmlConvert.ToInt16(attr); }
            attr = pluginNode.GetAttribute(xmlTags.MaxAutoCalcResults);
            if (attr.Length > 0) { m_MaxAutoCalcResults = (Int16)XmlConvert.ToInt16(attr); }
            attr = pluginNode.GetAttribute(xmlTags.SetNameAtImport);
            if (attr.Length > 0) { SetNameAtImport = XmlConvert.ToBoolean(attr); }
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
            attr = pluginNode.GetAttribute(xmlTags.sMultiChartType);
            if (attr.Length > 0)
            {
                m_MultiChartTypes.Clear();
                String[] values = attr.Split(';');
                foreach (String column in values)
                {
                    m_MultiChartTypes.Add((TrailLineChart.LineChartTypes)Enum.Parse(typeof(TrailLineChart.LineChartTypes), column, true));
                }
            }
        }

        public static void WriteOptions(XmlDocument xmlDoc, XmlElement pluginNode)
        {
            if (null == m_activityPageColumns)
            {
                //This can occur if the logbook was not loaded when exiting
                defaults();
            }
            pluginNode.SetAttribute(xmlTags.sDefaultRadius, XmlConvert.ToString(m_defaultRadius));
            pluginNode.SetAttribute(xmlTags.sNumFixedColumns, XmlConvert.ToString(m_activityPageNumFixedColumns));
            pluginNode.SetAttribute(xmlTags.sXAxis, m_xAxisValue.ToString());
            pluginNode.SetAttribute(xmlTags.sChartType, m_chartType.ToString());
            pluginNode.SetAttribute(xmlTags.summaryViewSortColumn, m_summaryViewSortColumn);
            pluginNode.SetAttribute(xmlTags.summaryViewSortDirection, m_summaryViewSortDirection.ToString());
            pluginNode.SetAttribute(xmlTags.ShowChartToolBar, XmlConvert.ToString(m_ShowChartToolBar));
            pluginNode.SetAttribute(xmlTags.SelectSimilarResults, XmlConvert.ToString(m_SelectSimilarResults));
            pluginNode.SetAttribute(xmlTags.SetNameAtImport, XmlConvert.ToString(m_SetNameAtImport));
            pluginNode.SetAttribute(xmlTags.MaxAutoCalcActivitiesTrails, XmlConvert.ToString(m_MaxAutoCalcActivitiesTrails));
            pluginNode.SetAttribute(xmlTags.MaxAutoCalcResults, XmlConvert.ToString(m_MaxAutoCalcResults));

            String colText = null;
            foreach (String column in m_activityPageColumns)
            {
                if (colText == null) { colText = column; }
                else { colText += ";" + column; }
            }
            pluginNode.SetAttribute(xmlTags.sColumns, colText);
            colText = null;
            foreach (TrailLineChart.LineChartTypes column in m_MultiChartTypes)
            {
                if (colText == null) { colText = column.ToString(); }
                else { colText += ";" + column.ToString(); }
            }
            pluginNode.SetAttribute(xmlTags.sMultiChartType, colText);
        }

        private class xmlTags
        {
            public const string sDefaultRadius = "sDefaultRadius";
            public const string sNumFixedColumns = "sNumFixedColumns";
            public const string sXAxis = "sXAxis";
            public const string sChartType = "sChartType";
            public const string sMultiChartType = "sMultiChartType";
            public const string summaryViewSortColumn = "summaryViewSortColumn";
            public const string summaryViewSortDirection = "summaryViewSortDirection";
            public const string ShowChartToolBar = "ShowChartToolBar";
            public const string SelectSimilarResults = "SelectSimilarResults";
            public const string MaxAutoCalcActivitiesTrails = "MaxAutoCalcActivitiesTrails";
            public const string MaxAutoCalcResults = "MaxAutoCalcResults";
            public const string SetNameAtImport = "SetNameAtImport";
            public const string sColumns = "sColumns";
        }

        //Parse dont care how the information was stored
        public static float parseFloat(string val)
        {
            if (System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator !=
                System.Globalization.CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator)
            {
                val=val.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator,
                    System.Globalization.CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator);
            }
            return float.Parse(val, System.Globalization.NumberFormatInfo.InvariantInfo);
        }


        ////Old version, read from logbook ("new" settings not implemented)
        //public void FromXml(XmlNode pluginNode)
        //{
        //    defaults();

        //    XmlNode settingsNode = pluginNode.SelectSingleNode("Settings");
        //    if (settingsNode != null && settingsNode.SelectSingleNode("@defaultRadius") != null)
        //    {
        //        m_defaultRadius = float.Parse(settingsNode.SelectSingleNode("@defaultRadius").Value);
        //    }

        //    XmlNode activityPageNode = null;
        //    if (settingsNode != null) {
        //        activityPageNode = settingsNode.SelectSingleNode("ActivityPage");
        //    }

        //    if (activityPageNode != null) {
        //        if (activityPageNode.SelectSingleNode("@numFixedColumns") != null) {
        //            m_activityPageNumFixedColumns = int.Parse(activityPageNode.SelectSingleNode("@numFixedColumns").Value);
        //        }
        //        if (activityPageNode.SelectSingleNode("@xAxis") != null) {
        //            m_xAxisValue = (TrailLineChart.XAxisValue)Enum.Parse(typeof(TrailLineChart.XAxisValue),activityPageNode.SelectSingleNode("@xAxis").Value);
        //        }
        //        if (activityPageNode.SelectSingleNode("@chartType") != null) {
        //            m_chartType = (TrailLineChart.LineChartTypes)Enum.Parse(typeof(TrailLineChart.LineChartTypes),activityPageNode.SelectSingleNode("@chartType").Value);
        //        }
        //        m_activityPageColumns.Clear();
        //        foreach (XmlNode node in activityPageNode.SelectNodes("Column"))
        //        {
        //            m_activityPageColumns.Add(node.InnerText);
        //        }
        //    }
        //}

        ////This is not called by default
        //public XmlNode ToXml(XmlDocument doc) {
        //    XmlNode settingsNode = doc.CreateElement("Settings");
        //    if (settingsNode.Attributes["defaultRadius"] == null) {
        //        settingsNode.Attributes.Append(doc.CreateAttribute("defaultRadius"));
        //    }
        //    settingsNode.Attributes["defaultRadius"].Value = m_defaultRadius.ToString();			

        //    XmlNode activityPageNode = doc.CreateElement("ActivityPage");
        //    settingsNode.AppendChild(activityPageNode);
        //    if (activityPageNode.Attributes["numFixedColumns"] == null)
        //    {
        //        activityPageNode.Attributes.Append(doc.CreateAttribute("numFixedColumns"));
        //    }
        //    activityPageNode.Attributes["numFixedColumns"].Value = m_activityPageNumFixedColumns.ToString();
        //    if (activityPageNode.Attributes["xAxis"] == null)
        //    {
        //        activityPageNode.Attributes.Append(doc.CreateAttribute("xAxis"));
        //    }
        //    activityPageNode.Attributes["xAxis"].Value = m_xAxisValue.ToString();
        //    if (activityPageNode.Attributes["chartType"] == null)
        //    {
        //        activityPageNode.Attributes.Append(doc.CreateAttribute("chartType"));
        //    }
        //    activityPageNode.Attributes["chartType"].Value = m_chartType.ToString();
        //    foreach (string columnName in m_activityPageColumns)
        //    {
        //        XmlNode column = doc.CreateElement("Column");
        //        column.AppendChild(doc.CreateTextNode(columnName));
        //        activityPageNode.AppendChild(column);
        //    }
        //    return settingsNode;
        //}
	}
}
