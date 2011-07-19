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

namespace TrailsPlugin.Data
{
	static class Settings
    {
        private static IList<string> m_activityPageColumns = TrailResultColumnIds.DefaultColumns();
        private static int m_activityPageNumFixedColumns = 0;
        private static float m_defaultRadius = 30;
        private static XAxisValue m_xAxisValue = XAxisValue.Distance;
        private static LineChartTypes m_chartType = LineChartTypes.SpeedPace;
        private static IList<LineChartTypes> m_MultiChartTypes = LineChartUtil.DefaultLineChartTypes();
        private static IList<LineChartTypes> m_MultiGraphTypes = LineChartUtil.DefaultLineChartTypes();
        private static bool m_ShowChartToolBar = true;
        private static bool m_SelectSimilarResults = false;
        private static bool m_addCurrentCategory = false;
        private static string m_summaryViewSortColumn = TrailResultColumnIds.Order;
        private static ListSortDirection m_summaryViewSortDirection = ListSortDirection.Ascending;
        private static bool m_SetNameAtImport = true;
        private static int m_MaxAutoCalcActivitiesTrails = 500;
        private static int m_MaxAutoCalcResults = 800;
        private static bool m_restIsPause = false;
        private static bool m_resyncDiffAtTrailPoints = true;
        private static bool m_adjustResyncDiffAtTrailPoints = false;
        private static bool m_syncChartAtTrailPoints = false;
        private static bool m_onlyReferenceRight = false;

        //Note: The data structures need restructuring...
        //Temporary hack to translate to strings
        public static LineChartTypes ChartType
        {
            get
            {
                return m_chartType;
            }
            set
            {
                m_chartType = value;
                Plugin.WriteExtensionData();
            }
        }
        public static IList<LineChartTypes> MultiChartType
        {
            get
            {
                return m_MultiChartTypes;
            }
        }
        public static LineChartTypes ToggleMultiChartType
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

        public static IList<LineChartTypes> MultiGraphType
        {
            get
            {
                return m_MultiGraphTypes;
            }
        }
        public static LineChartTypes ToggleMultiGraphType
        {
            set
            {
                if (m_MultiGraphTypes.Contains(value))
                {
                    m_MultiGraphTypes.Remove(value);
                }
                else
                {
                    m_MultiGraphTypes.Add(value);
                }
            }
        }

        public static XAxisValue XAxisValue
        {
			get {
				return m_xAxisValue;
			}
			set {
				m_xAxisValue = value;
				Plugin.WriteExtensionData();
			}
		}
        public static IList<string> ActivityPageColumns
        {
			get {
				return m_activityPageColumns;
			}
			set {
				m_activityPageColumns = value;
				Plugin.WriteExtensionData();
			}
		}

        public static int ActivityPageNumFixedColumns
        {
			get {
				return m_activityPageNumFixedColumns;
			}
			set {
				m_activityPageNumFixedColumns = value;
				Plugin.WriteExtensionData();
			}
		}

        public static float DefaultRadius
        {
			get {
				return m_defaultRadius;
			}
			set {
				m_defaultRadius = value;
				Plugin.WriteExtensionData();
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
        public static bool AddCurrentCategory
        {
            get
            {
                return m_addCurrentCategory;
            }
            set
            {
                m_addCurrentCategory = value;
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
        public static bool RestIsPause
        {
            get { return m_restIsPause; }
            set { m_restIsPause = value; }
        }
        public static bool ResyncDiffAtTrailPoints
        {
            get { return m_resyncDiffAtTrailPoints; }
            set { m_resyncDiffAtTrailPoints = value; }
        }
        public static bool AdjustResyncDiffAtTrailPoints
        {
            get { return m_adjustResyncDiffAtTrailPoints; }
            set { m_adjustResyncDiffAtTrailPoints = value; }
        }
        public static bool SyncChartAtTrailPoints
        {
            get { return m_syncChartAtTrailPoints; }
            set { m_syncChartAtTrailPoints = value; }
        }
        public static bool OnlyReferenceRight
        {
            get { return m_onlyReferenceRight; }
            set { m_onlyReferenceRight = value; }
        }

        public static void ReadOptions(XmlDocument xmlDoc, XmlNamespaceManager nsmgr, XmlElement pluginNode)
        {
            String attr;
            attr = pluginNode.GetAttribute(xmlTags.sDefaultRadius);
            if (attr.Length > 0) { m_defaultRadius = Settings.parseFloat(attr); }
            attr = pluginNode.GetAttribute(xmlTags.sNumFixedColumns);
            if (attr.Length > 0) { m_activityPageNumFixedColumns = (Int16)XmlConvert.ToInt16(attr); }
            attr = pluginNode.GetAttribute(xmlTags.sXAxis);
            if (attr.Length > 0) { m_xAxisValue = (XAxisValue)Enum.Parse(typeof(XAxisValue), attr, true); }
            attr = pluginNode.GetAttribute(xmlTags.sChartType);
            if (attr.Length > 0) { m_chartType = (LineChartTypes)Enum.Parse(typeof(LineChartTypes), attr, true); }
            if (m_chartType == LineChartTypes.DiffDist ||
                m_chartType == LineChartTypes.DiffTime)
            {
                m_chartType = LineChartTypes.DiffDistTime;
            }
            attr = pluginNode.GetAttribute(xmlTags.summaryViewSortColumn);
            if (attr.Length > 0) { m_summaryViewSortColumn = attr; }
            attr = pluginNode.GetAttribute(xmlTags.summaryViewSortDirection);
            if (attr.Length > 0) { m_summaryViewSortDirection = (ListSortDirection)Enum.Parse(typeof(ListSortDirection), attr); }
            attr = pluginNode.GetAttribute(xmlTags.SelectSimilarResults);
            if (attr.Length > 0) { m_SelectSimilarResults = XmlConvert.ToBoolean(attr); }
            attr = pluginNode.GetAttribute(xmlTags.AddCurrentActivity);
            if (attr.Length > 0) { m_addCurrentCategory = XmlConvert.ToBoolean(attr); }
            attr = pluginNode.GetAttribute(xmlTags.ShowChartToolBar);
            if (attr.Length > 0) { m_ShowChartToolBar = XmlConvert.ToBoolean(attr); }
            attr = pluginNode.GetAttribute(xmlTags.MaxAutoCalcActivitiesTrails);
            if (attr.Length > 0) { 
                //TODO: Changed secret setting, too low default
                if((Int16)XmlConvert.ToInt16(attr) > m_MaxAutoCalcActivitiesTrails)
                    m_MaxAutoCalcActivitiesTrails = (Int16)XmlConvert.ToInt16(attr); }
            attr = pluginNode.GetAttribute(xmlTags.MaxAutoCalcResults);
            if (attr.Length > 0) {
                //TODO: Changed secret setting, too low default
                if((Int16)XmlConvert.ToInt16(attr) > m_MaxAutoCalcResults)
                    m_MaxAutoCalcResults = (Int16)XmlConvert.ToInt16(attr); }
            attr = pluginNode.GetAttribute(xmlTags.SetNameAtImport);
            if (attr.Length > 0) { SetNameAtImport = XmlConvert.ToBoolean(attr); }
            attr = pluginNode.GetAttribute(xmlTags.RestIsPause);
            if (attr.Length > 0) { RestIsPause = XmlConvert.ToBoolean(attr); }
            attr = pluginNode.GetAttribute(xmlTags.ResyncDiffAtTrailPoints);
            if (attr.Length > 0) { ResyncDiffAtTrailPoints = XmlConvert.ToBoolean(attr); }
            attr = pluginNode.GetAttribute(xmlTags.AdjustResyncDiffAtTrailPoints);
            if (attr.Length > 0) { AdjustResyncDiffAtTrailPoints = XmlConvert.ToBoolean(attr); }
            attr = pluginNode.GetAttribute(xmlTags.SyncChartAtTrailPoints);
            if (attr.Length > 0) { SyncChartAtTrailPoints = XmlConvert.ToBoolean(attr); }
            attr = pluginNode.GetAttribute(xmlTags.OnlyReferenceRight);
            if (attr.Length > 0) { OnlyReferenceRight = XmlConvert.ToBoolean(attr); }

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
                    LineChartTypes t = (LineChartTypes)Enum.Parse(typeof(LineChartTypes), column, true);
                    //Compatibility w previous, where DifTime/DiffDist could be speced directly
                    if (t == LineChartTypes.DiffDist || t == LineChartTypes.DiffTime)
                    {
                        if (!m_MultiChartTypes.Contains(LineChartTypes.DiffDistTime))
                        {
                            m_MultiChartTypes.Add(LineChartTypes.DiffDistTime);
                        }
                    }
                    else
                    {
                        m_MultiChartTypes.Add(t);
                    }
                }
            }

            attr = pluginNode.GetAttribute(xmlTags.sMultiGraphType);
            if (attr.Length > 0)
            {
                m_MultiGraphTypes.Clear();
                String[] values = attr.Split(';');
                foreach (String column in values)
                {
                    LineChartTypes t = (LineChartTypes)Enum.Parse(typeof(LineChartTypes), column, true);
                    //Compatibility w previous, where DifTime/DiffDist could be speced directly
                    if (t == LineChartTypes.DiffDist || t == LineChartTypes.DiffTime)
                    {
                        if (!m_MultiGraphTypes.Contains(LineChartTypes.DiffDistTime))
                        {
                            m_MultiGraphTypes.Add(LineChartTypes.DiffDistTime);
                        }
                    }
                    else
                    {
                        m_MultiGraphTypes.Add(t);
                    }
                }
            }
        }

        public static void WriteOptions(XmlDocument xmlDoc, XmlElement pluginNode)
        {
            pluginNode.SetAttribute(xmlTags.sDefaultRadius, XmlConvert.ToString(m_defaultRadius));
            pluginNode.SetAttribute(xmlTags.sNumFixedColumns, XmlConvert.ToString(m_activityPageNumFixedColumns));
            pluginNode.SetAttribute(xmlTags.sXAxis, m_xAxisValue.ToString());
            pluginNode.SetAttribute(xmlTags.sChartType, m_chartType.ToString());
            pluginNode.SetAttribute(xmlTags.summaryViewSortColumn, m_summaryViewSortColumn);
            pluginNode.SetAttribute(xmlTags.summaryViewSortDirection, m_summaryViewSortDirection.ToString());
            pluginNode.SetAttribute(xmlTags.ShowChartToolBar, XmlConvert.ToString(m_ShowChartToolBar));
            pluginNode.SetAttribute(xmlTags.SelectSimilarResults, XmlConvert.ToString(m_SelectSimilarResults));
            pluginNode.SetAttribute(xmlTags.AddCurrentActivity, XmlConvert.ToString(m_addCurrentCategory));
            pluginNode.SetAttribute(xmlTags.SetNameAtImport, XmlConvert.ToString(m_SetNameAtImport));
            pluginNode.SetAttribute(xmlTags.RestIsPause, XmlConvert.ToString(m_restIsPause));
            pluginNode.SetAttribute(xmlTags.ResyncDiffAtTrailPoints, XmlConvert.ToString(m_resyncDiffAtTrailPoints));
            pluginNode.SetAttribute(xmlTags.AdjustResyncDiffAtTrailPoints, XmlConvert.ToString(m_adjustResyncDiffAtTrailPoints));
            pluginNode.SetAttribute(xmlTags.SyncChartAtTrailPoints, XmlConvert.ToString(m_syncChartAtTrailPoints));
            pluginNode.SetAttribute(xmlTags.OnlyReferenceRight, XmlConvert.ToString(m_onlyReferenceRight));

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
            foreach (LineChartTypes column in m_MultiChartTypes)
            {
                if (colText == null) { colText = column.ToString(); }
                else { colText += ";" + column.ToString(); }
            }
            pluginNode.SetAttribute(xmlTags.sMultiChartType, colText);

            colText = null;
            foreach (LineChartTypes column in m_MultiGraphTypes)
            {
                if (colText == null) { colText = column.ToString(); }
                else { colText += ";" + column.ToString(); }
            }
            pluginNode.SetAttribute(xmlTags.sMultiGraphType, colText);
        }

        private class xmlTags
        {
            public const string sDefaultRadius = "sDefaultRadius";
            public const string sNumFixedColumns = "sNumFixedColumns";
            public const string sXAxis = "sXAxis";
            public const string sChartType = "sChartType";
            public const string sMultiChartType = "sMultiChartType";
            public const string sMultiGraphType = "sMultiGraphType";
            public const string summaryViewSortColumn = "summaryViewSortColumn";
            public const string summaryViewSortDirection = "summaryViewSortDirection";
            public const string ShowChartToolBar = "ShowChartToolBar";
            public const string SelectSimilarResults = "SelectSimilarResults";
            public const string AddCurrentActivity = "AddCurrentActivity";
            public const string MaxAutoCalcActivitiesTrails = "MaxAutoCalcActivitiesTrails";
            public const string MaxAutoCalcResults = "MaxAutoCalcResults";
            public const string SetNameAtImport = "SetNameAtImport";
            public const string RestIsPause = "RestIsPause";
            public const string ResyncDiffAtTrailPoints = "ResyncDiffAtTrailPoints";
            public const string AdjustResyncDiffAtTrailPoints = "AdjustResyncDiffAtTrailPoints";
            public const string SyncChartAtTrailPoints = "SyncChartAtTrailPoints";
            public const string OnlyReferenceRight = "OnlyReferenceRight";

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
        //            m_xAxisValue = (XAxisValue)Enum.Parse(typeof(XAxisValue),activityPageNode.SelectSingleNode("@xAxis").Value);
        //        }
        //        if (activityPageNode.SelectSingleNode("@chartType") != null) {
        //            m_chartType = (LineChartTypes)Enum.Parse(typeof(LineChartTypes),activityPageNode.SelectSingleNode("@chartType").Value);
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
