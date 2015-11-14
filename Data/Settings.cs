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

using TrailsPlugin.Utils;
using ZoneFiveSoftware.Common.Data.Fitness;

namespace TrailsPlugin.Data
{
    static class Settings
    {
        private static IList<string> m_activityPageColumns = TrailResultColumns.DefaultColumns();
        private static IDictionary<string,int> m_activityPageColumnSizes = new Dictionary<string,int>();
        private static int m_activityPageNumFixedColumns = 0;
        private static float m_defaultRadius = 30;
        private static XAxisValue m_xAxisValue = XAxisValue.Distance;
        private static LineChartTypes m_chartType = LineChartTypes.SpeedPace;
        private static IList<LineChartTypes> m_MultiChartTypes = LineChartUtil.DefaultLineChartTypes();
        private static IList<LineChartTypes> m_MultiGraphTypes = LineChartUtil.DefaultLineChartTypes();
        private static bool m_ShowChartToolBar = true;
        private static bool m_SelectSimilarResults = false;
        private static bool m_addCurrentCategory = false;
        private static IList<string> m_summaryViewSortColumns = new List<string>(3) { TrailResultColumns.DefaultSortColumn() };
        private static ListSortDirection m_summaryViewSortDirection = ListSortDirection.Descending;
        private static bool m_SetNameAtImport = true;
        private static bool m_SetAdjustElevationAtImport = false;
        private static int m_MaxAutoCalcActivitiesTrails = 10000;
        private static int m_MaxAutoCalcActivitiesSingleTrail = 10000;
        private static bool m_restLapIsPause = false;
        private static bool m_nonReqIsPause = false;
        private static bool m_resyncDiffAtTrailPoints = true;
        private static bool m_adjustResyncDiffAtTrailPoints = false;
        private static bool m_syncChartAtTrailPoints = false;
        private static bool m_onlyReferenceRight = false;
        private static bool m_zoomToSelection = false;
        private static bool m_showOnlyMarkedOnRoute = false;
        private static bool m_resultSummaryIsDevice = false;
        private static bool m_resultSummaryStdDev = false;
        private static bool m_resultSummaryTotal = false;
        private static String[] m_excludeStoppedCategory = new String[0];
        private static String[] m_barometricDevices = new String[5] {"Edge", "920XT", "910XT", "fenix", "GB-580"};
        private static SmoothOverTrailBorders m_SmoothOverTrailPoints = SmoothOverTrailBorders.Unchanged;
        private static float m_predictDistance = 10000;
        private static RunningGradeAdjustMethodEnum m_RunningGradeAdjustMethod = RunningGradeAdjustMethodEnum.None;
        private static bool m_deviceElevationFromOther = false;
        private static bool m_cadenceFromOther = false;
        private static bool m_useDeviceElevationForCalc = false;
        private static bool m_useTrailElevationAdjust = false;
        private static float[,] m_AdjustDiffSplitTimes = null;
        private static float[,] m_PandolfTerrainDist = null;
        private static float m_MervynDaviesUp = 0.033f;
        private static float m_MervynDaviesDown = 0.017f;
        private static float m_JackDanielsUp = 15/1609f;
        private static float m_JackDanielsDown = 8/1609f;
        private static string m_saveChartImagePath = null;

        private static bool m_startDistOffsetFromStartPoint = false; //Not in xml
        private static bool m_diffUsingCommonStretches = false; //Not in xml
        public static bool UseDeviceDistance = false;
        public static bool ShowSummaryTotal = true;
        public static bool ShowSummaryAverage = true;
        public static bool ShowTrailPointsOnChart = true;
        public static bool ShowTrailPointsOnMap = true;

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

        public static string[] SetMultiChartType
        {
            set
            {
                m_MultiChartTypes = LineChartUtil.ParseLineChartType(value);
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

        public static string[] SetMultiGraphType
        {
            set
            {
                m_MultiGraphTypes = LineChartUtil.ParseLineChartType(value);
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
            get
            {
                return m_activityPageColumns;
            }
            set
            {
                m_activityPageColumns = value;
                Plugin.WriteExtensionData();
            }
        }
        public static int ActivityPageColumnsSizeGet(string id)
        {
            if (m_activityPageColumnSizes.ContainsKey(id))
            {
                return m_activityPageColumnSizes[id];
            }
            else
            {
                return -1; //Unknown
            }
        }

        public static void ActivityPageColumnsSizeSet(string id, int value)
        {
            m_activityPageColumnSizes[id] = value;
        }

        public static int ActivityPageNumFixedColumns
        {
            get
            {
                return m_activityPageNumFixedColumns;
            }
            set {
                m_activityPageNumFixedColumns = value;
                Plugin.WriteExtensionData();
            }
        }

        public static float DefaultRadius
        {
            get
            {
                return m_defaultRadius;
            }
            set {
                m_defaultRadius = value;
                Plugin.WriteExtensionData();
            }
        }

        public static bool ShowChartToolBar
        {
            get
            {
                return m_ShowChartToolBar;
            }
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

        public static IList<string> SummaryViewSortColumns
        {
            get { return m_summaryViewSortColumns; }
        }

        public static string UpdateSummaryViewSortColumn
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (m_summaryViewSortColumns.Contains(value))
                    {
                        m_summaryViewSortColumns.Remove(value);
                    }
                    if (m_summaryViewSortColumns.Count >= 2)
                    {
                        m_summaryViewSortColumns.RemoveAt(m_summaryViewSortColumns.Count - 1);
                    }
                    m_summaryViewSortColumns.Insert(0, value);
                }
            }
        }

        public static string GetSummaryViewSortColumns
        {
            get
            {
                string s = "";
                foreach (string col in m_summaryViewSortColumns)
                {
                    s += col + ",";
                }
                return s;
            }
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

        public static bool SetAdjustElevationAtImport
        {
            get { return m_SetAdjustElevationAtImport; }
            set { m_SetAdjustElevationAtImport = value; }
        }

        public static int MaxAutoCalcActivitiesTrails
        {
            get { return m_MaxAutoCalcActivitiesTrails; }
            set { m_MaxAutoCalcActivitiesTrails = value; }
        }

        public static int MaxAutoCalcActitiesSingleTrail
        {
            get { return m_MaxAutoCalcActivitiesSingleTrail; }
            set { m_MaxAutoCalcActivitiesSingleTrail = value; }
        }

        public static int MaxAutoCalcResults
        {
            get { return 250; }
        }

        public static int MaxAutoSelectSplits
        {
            get { return 25; }
        }

        public static bool RestIsPause
        {
            get { return m_restLapIsPause; }
            set { m_restLapIsPause = value; }
        }

        public static bool NonReqIsPause
        {
            get { return m_nonReqIsPause; }
            set { m_nonReqIsPause = value; }
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

        public static String[] ExcludeStoppedCategory
        {
            get { return m_excludeStoppedCategory; }
        }

        public static String GetExcludeStoppedCategory
        {
            get
            {
                string s = "";
                foreach (string i in m_excludeStoppedCategory)
                {
                    if(!string.IsNullOrEmpty(i))
                    {
                        s += i + ';';
                    }
                }
                if (!string.IsNullOrEmpty(s))
                {
                    s = s.Remove(s.Length - 1);
                }
                return s;
            }
        }

        public static void SetExcludeStoppedCategory(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                m_excludeStoppedCategory = new string[0];
            }
            else
            {
                m_excludeStoppedCategory = s.Split(';');
            }
        }

        public static String[] BarometricDevices
        {
            get { return m_barometricDevices; }
        }

        public static String GetBarometricDevices
        {
            get
            {
                string s = "";
                foreach (string i in m_barometricDevices)
                {
                    if (!string.IsNullOrEmpty(i))
                    {
                        s += i + ';';
                    }
                }
                if (!string.IsNullOrEmpty(s))
                {
                    s = s.Remove(s.Length - 1);
                }
                return s;
            }
        }

        public static void SetBarometricDevices(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                m_barometricDevices = new string[0];
            }
            else
            {
                m_barometricDevices = s.Split(';');
            }
        }

        public static bool StartDistOffsetFromStartPoint
        {
            get { return m_startDistOffsetFromStartPoint; }
            set { m_startDistOffsetFromStartPoint = value; }
        }

        public static bool DiffUsingCommonStretches
        {
            get { return m_diffUsingCommonStretches; }
            set { m_diffUsingCommonStretches = value; }
        }

        public static bool ZoomToSelection
        {
            get { return m_zoomToSelection; }
            set { m_zoomToSelection = value; }
        }

        public static bool ShowOnlyMarkedOnRoute
        {
            get { return m_showOnlyMarkedOnRoute; }
            set { m_showOnlyMarkedOnRoute = value; }
        }

        public static SmoothOverTrailBorders SmoothOverTrailPoints
        {
            get { return m_SmoothOverTrailPoints; }
        }

        public static void SmoothOverTrailPointsToggle()
        {
            if (m_SmoothOverTrailPoints >= SmoothOverTrailBorders.None)
            {
                m_SmoothOverTrailPoints = SmoothOverTrailBorders.All;
            }
            else
            {
                m_SmoothOverTrailPoints++;
            }
        }
        ///
        /// Predict distances in PerformancePredictor
        public static float PredictDistance
        {
            get
            {
                return m_predictDistance;
            }
            set
            {
                m_predictDistance = value;
            }
        }

        ///
        /// Adjust pace according to grade
        public static RunningGradeAdjustMethodEnum RunningGradeAdjustMethod
        {
            get
            {
                return m_RunningGradeAdjustMethod;
            }
            set
            {
                m_RunningGradeAdjustMethod = value;
            }
        }

        public static float MervynDaviesUp
        {
            get
            {
                return m_MervynDaviesUp;
            }
            set
            {
                m_MervynDaviesUp = value;
            }
        }

        public static float MervynDaviesDown
        {
            get
            {
                return m_MervynDaviesDown;
            }
            set
            {
                m_MervynDaviesDown = value;
            }
        }

        public static float JackDanielsUp
        {
            get
            {
                return m_JackDanielsUp;
            }
            set
            {
                m_JackDanielsUp = value;
            }
        }

        public static float JackDanielsDown
        {
            get
            {
                return m_JackDanielsDown;
            }
            set
            {
                m_JackDanielsDown = value;
            }
        }

        public static bool CadenceFromOther
        {
            get
            {
                return m_cadenceFromOther;
            }
            set
            {
                m_cadenceFromOther = value;
            }
        }

        public static bool DeviceElevationFromOther
        {
            get
            {
                return m_deviceElevationFromOther;
            }
            set
            {
                m_deviceElevationFromOther = value;
            }
        }

        public static bool UseDeviceElevationForCalc
        {
            get
            {
                return m_useDeviceElevationForCalc;
            }
            set
            {
                m_useDeviceElevationForCalc = value;
            }
        }

        public static bool UseTrailElevationAdjust
        {
            get
            {
                return m_useTrailElevationAdjust;
            }
            set
            {
                m_useTrailElevationAdjust = value;
            }
        }

        ///
        ///Make diff to ideal time adjusted to grade
        ///Not stored in preferences
        public static float[,] AdjustDiffSplitTimes
        {
            get
            {
                return m_AdjustDiffSplitTimes;
            }
            set
            {
                m_AdjustDiffSplitTimes = value;
            }
        }

        ///
        ///Make diff to ideal time adjusted to grade
        ///Not stored in preferences
        public static float[,] PandolfTerrainDist
        {
            get
            {
                return m_PandolfTerrainDist;
            }
            set
            {
                m_PandolfTerrainDist = value;
            }
        }

        /// <summary>
        /// Show standard deviation for certain fields in the summary
        /// </summary>
        public static bool ResultSummaryStdDev
        {
            get { return m_resultSummaryStdDev; }
            set { m_resultSummaryStdDev = value; }
        }

        public static bool ResultSummaryTotal
        {
            get { return m_resultSummaryTotal; }
            set { m_resultSummaryTotal = value; }
        }

        /// <summary>
        /// Show the summary from the device, instead of the track summaries
        /// </summary>
        public static bool ResultSummaryIsDevice
        {
            get { return m_resultSummaryIsDevice; }
            set { m_resultSummaryIsDevice = value; }
        }
        public static string SaveChartImagePath
        {
            get
            {
                if (m_saveChartImagePath == null) { m_saveChartImagePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); }
                return m_saveChartImagePath; 
            }
            set { m_saveChartImagePath = value; }
        }
        /******************************************************/
        public static void ReadOptions(XmlDocument xmlDoc, XmlNamespaceManager nsmgr, XmlElement pluginNode)
        {
            try
            {
                String attr;
                attr = pluginNode.GetAttribute(xmlTags.sDefaultRadius);
                if (attr.Length > 0) { m_defaultRadius = Settings.parseFloat(attr); }
                attr = pluginNode.GetAttribute(xmlTags.sNumFixedColumns);
                if (attr.Length > 0) { m_activityPageNumFixedColumns = (Int16)XmlConvert.ToInt16(attr); }
                attr = pluginNode.GetAttribute(xmlTags.sXAxis);
                if (attr.Length > 0) { m_xAxisValue = (XAxisValue)Enum.Parse(typeof(XAxisValue), attr, true); }
                attr = pluginNode.GetAttribute(xmlTags.sChartType);
                try
                {
                    if (attr.Length > 0) { m_chartType = (LineChartTypes)Enum.Parse(typeof(LineChartTypes), attr, true); }
                    if (m_chartType == LineChartTypes.DiffDist ||
                        m_chartType == LineChartTypes.DiffTime)
                    {
                        m_chartType = LineChartTypes.DiffDistTime;
                    }
                }
                catch { }
                attr = pluginNode.GetAttribute(xmlTags.SelectSimilarResults);
                if (attr.Length > 0) { m_SelectSimilarResults = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.AddCurrentActivity);
                if (attr.Length > 0) { m_addCurrentCategory = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.ShowChartToolBar);
                if (attr.Length > 0) { m_ShowChartToolBar = XmlConvert.ToBoolean(attr); } 
                attr = pluginNode.GetAttribute(xmlTags.SetNameAtImport);
                if (attr.Length > 0) { SetNameAtImport = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.SetAdjustElevationAtImport);
                if (attr.Length > 0) { SetAdjustElevationAtImport = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.RestLapIsPause);
                if (attr.Length > 0) { RestIsPause = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.NonReqIsPause);
                if (attr.Length > 0) { NonReqIsPause = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.ResyncDiffAtTrailPoints);
                if (attr.Length > 0) { ResyncDiffAtTrailPoints = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.AdjustResyncDiffAtTrailPoints);
                if (attr.Length > 0) { AdjustResyncDiffAtTrailPoints = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.SyncChartAtTrailPoints);
                if (attr.Length > 0) { SyncChartAtTrailPoints = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.OnlyReferenceRight);
                if (attr.Length > 0) { OnlyReferenceRight = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.ZoomToSelection);
                if (attr.Length > 0) { ZoomToSelection = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.ShowOnlyMarkedOnRoute);
                if (attr.Length > 0) { ShowOnlyMarkedOnRoute = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.ExcludeStoppedCategory);
                if (attr.Length > 0) { SetExcludeStoppedCategory(attr); }
                attr = pluginNode.GetAttribute(xmlTags.BarometricDevices);
                if (attr.Length > 0) { SetBarometricDevices(attr); }
                attr = pluginNode.GetAttribute(xmlTags.SmoothOverTrailPoints);
                try
                {
                    if (attr.Length > 0) { m_SmoothOverTrailPoints = (SmoothOverTrailBorders)Enum.Parse(typeof(SmoothOverTrailBorders), attr, true); }
                }
                catch { }
                attr = pluginNode.GetAttribute(xmlTags.PredictDistance);
                if (attr.Length > 0) { m_predictDistance = Settings.parseFloat(attr); }
                attr = pluginNode.GetAttribute(xmlTags.sCadenceFromOther);
                if (attr.Length > 0) { CadenceFromOther = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.sDeviceElevationFromOther);
                if (attr.Length > 0) { DeviceElevationFromOther = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.sUseDeviceElevationForCalc);
                if (attr.Length > 0) { UseDeviceElevationForCalc = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.sUseDeviceDistance);
                if (attr.Length > 0) { UseDeviceDistance = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.sShowSummaryTotal);
                if (attr.Length > 0) { ShowSummaryTotal = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.sShowSummaryAverage);
                if (attr.Length > 0) { ShowSummaryAverage = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.sShowTrailPointsOnChart);
                if (attr.Length > 0) { ShowTrailPointsOnChart = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.sShowTrailPointsOnMap);
                if (attr.Length > 0) { ShowTrailPointsOnMap = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.sUseTrailElevationAdjust);
                if (attr.Length > 0) { UseTrailElevationAdjust = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(xmlTags.sRunningGradeAdjustMethod);
                try
                {
                    if (attr.Length > 0) { m_RunningGradeAdjustMethod = (RunningGradeAdjustMethodEnum)Enum.Parse(typeof(RunningGradeAdjustMethodEnum), attr, true); }
                }
                catch { }
                attr = pluginNode.GetAttribute(xmlTags.sMervynDaviesUp);
                if (attr.Length > 0) { m_MervynDaviesUp = Settings.parseFloat(attr); }
                attr = pluginNode.GetAttribute(xmlTags.sMervynDaviesDown);
                if (attr.Length > 0) { m_MervynDaviesDown = Settings.parseFloat(attr); }
                attr = pluginNode.GetAttribute(xmlTags.sJackDanielsUp);
                if (attr.Length > 0) { m_JackDanielsUp = Settings.parseFloat(attr); }
                attr = pluginNode.GetAttribute(xmlTags.sJackDanielsDown);
                if (attr.Length > 0) { m_JackDanielsDown = Settings.parseFloat(attr); }
                //Not read or saved in preferences right now
                //attr = pluginNode.GetAttribute(xmlTags.sAdjustDiffSplitTimes);
                //try
                //{
                //    if (attr.Length > 0)
                //    {
                //        string[] values = attr.Split(';');
                //        m_AdjustDiffSplitTimes = new float[values.Length/2, 2];
                //        int i = 0;
                //        foreach (string column in values)
                //        {
                //            float f = Settings.parseFloat(column);
                //            //if (i % 2 == 0)
                //            //{
                //            //    f = (float)GpsRunningPlugin.Util.UnitUtil.Distance.ConvertTo(f,null);
                //            //}
                //            m_AdjustDiffSplitTimes[i / 2, i % 2] = f;
                //            i++;
                //        }
                //    }
                //}
                //catch { }
                attr = pluginNode.GetAttribute(xmlTags.sResultSummaryStdDev);
                if (attr.Length > 0) { m_resultSummaryStdDev = XmlConvert.ToBoolean(attr); }
            
                attr = pluginNode.GetAttribute(xmlTags.MaxAutoCalcActivitiesTrails);
                if (attr.Length > 0)
                {
                    m_MaxAutoCalcActivitiesTrails = (Int16)XmlConvert.ToInt16(attr);
                    //Change defaults without version number
                    if (m_MaxAutoCalcActivitiesTrails == 500) { m_MaxAutoCalcActivitiesTrails = 10000; }
                }
                attr = pluginNode.GetAttribute(xmlTags.MaxAutoCalcActivitiesSingleTrail);
                if (attr.Length > 0) {
                    m_MaxAutoCalcActivitiesSingleTrail = (Int16)XmlConvert.ToInt16(attr);
                    if (m_MaxAutoCalcActivitiesSingleTrail == 200) { m_MaxAutoCalcActivitiesSingleTrail = 10000; }
                }

                attr = pluginNode.GetAttribute(xmlTags.sColumns);
                if (attr.Length > 0)
                {
                    m_activityPageColumns.Clear();
                    String[] values = attr.Split(';');
                    foreach (String column in values)
                    {
                        String id = column;
                        String[] idWidth = id.Split(':');
                        if (idWidth.Length > 1)
                        {
                            //The column has width appended
                            id = idWidth[0];
                            int width;
                            if (int.TryParse(idWidth[1], out width))
                            {
                                ActivityPageColumnsSizeSet(id, width);
                            }
                        }
                        if (id == "VAM")
                        {
                            //Renamed
                            id = TrailResultColumnIds.AscendingSpeed_VAM;
                        }
                        if (!TrailResultColumnIds.ObsoleteFields.Contains(id))
                        {
                            m_activityPageColumns.Add(id);
                        }
                    }
                }
                m_summaryViewSortColumns.Clear();
                attr = pluginNode.GetAttribute(xmlTags.summaryViewSortColumn);
                if (attr.Length > 0) {
                    String[] values = attr.Split(',');
                    foreach (String column in values)
                    {
                        UpdateSummaryViewSortColumn = column;
                    }
                }
                if (m_summaryViewSortColumns.Count == 0 && m_activityPageColumns != null && m_activityPageColumns.Count > 1)
                { UpdateSummaryViewSortColumn = m_activityPageColumns[1]; }
                
                attr = pluginNode.GetAttribute(xmlTags.summaryViewSortDirection);
                if (attr.Length > 0) { m_summaryViewSortDirection = (ListSortDirection)Enum.Parse(typeof(ListSortDirection), attr); }

                attr = pluginNode.GetAttribute(xmlTags.sMultiChartType);
                if (attr.Length > 0)
                {
                    String[] values = attr.Split(';');
                    SetMultiChartType = values;
                }

                attr = pluginNode.GetAttribute(xmlTags.sMultiGraphType);
                if (attr.Length > 0)
                {
                    String[] values = attr.Split(';');
                    SetMultiGraphType = values;
                }
                attr = pluginNode.GetAttribute(xmlTags.sSaveChartImagePath);
                if (attr.Length > 0) { SaveChartImagePath = attr; }
            }
            catch { }
        }

        public static void WriteOptions(XmlDocument xmlDoc, XmlElement pluginNode)
        {
            pluginNode.SetAttribute(xmlTags.sDefaultRadius, XmlConvert.ToString(m_defaultRadius));
            pluginNode.SetAttribute(xmlTags.sNumFixedColumns, XmlConvert.ToString(m_activityPageNumFixedColumns));
            pluginNode.SetAttribute(xmlTags.sXAxis, m_xAxisValue.ToString());
            pluginNode.SetAttribute(xmlTags.sChartType, m_chartType.ToString());
            pluginNode.SetAttribute(xmlTags.summaryViewSortColumn, GetSummaryViewSortColumns);
            pluginNode.SetAttribute(xmlTags.summaryViewSortDirection, m_summaryViewSortDirection.ToString());
            pluginNode.SetAttribute(xmlTags.ShowChartToolBar, XmlConvert.ToString(m_ShowChartToolBar));
            pluginNode.SetAttribute(xmlTags.SelectSimilarResults, XmlConvert.ToString(m_SelectSimilarResults));
            pluginNode.SetAttribute(xmlTags.AddCurrentActivity, XmlConvert.ToString(m_addCurrentCategory));
            pluginNode.SetAttribute(xmlTags.SetNameAtImport, XmlConvert.ToString(m_SetNameAtImport));
            pluginNode.SetAttribute(xmlTags.SetAdjustElevationAtImport, XmlConvert.ToString(m_SetAdjustElevationAtImport));
            pluginNode.SetAttribute(xmlTags.RestLapIsPause, XmlConvert.ToString(m_restLapIsPause));
            pluginNode.SetAttribute(xmlTags.NonReqIsPause, XmlConvert.ToString(m_nonReqIsPause));
            pluginNode.SetAttribute(xmlTags.ResyncDiffAtTrailPoints, XmlConvert.ToString(m_resyncDiffAtTrailPoints));
            pluginNode.SetAttribute(xmlTags.AdjustResyncDiffAtTrailPoints, XmlConvert.ToString(m_adjustResyncDiffAtTrailPoints));
            pluginNode.SetAttribute(xmlTags.SyncChartAtTrailPoints, XmlConvert.ToString(m_syncChartAtTrailPoints));
            pluginNode.SetAttribute(xmlTags.OnlyReferenceRight, XmlConvert.ToString(m_onlyReferenceRight));
            pluginNode.SetAttribute(xmlTags.ZoomToSelection, XmlConvert.ToString(m_zoomToSelection));
            pluginNode.SetAttribute(xmlTags.ShowOnlyMarkedOnRoute, XmlConvert.ToString(m_showOnlyMarkedOnRoute));
            pluginNode.SetAttribute(xmlTags.ExcludeStoppedCategory, GetExcludeStoppedCategory);
            pluginNode.SetAttribute(xmlTags.BarometricDevices, GetBarometricDevices);
            pluginNode.SetAttribute(xmlTags.SmoothOverTrailPoints, m_SmoothOverTrailPoints.ToString());
            pluginNode.SetAttribute(xmlTags.PredictDistance, XmlConvert.ToString(m_predictDistance));
            pluginNode.SetAttribute(xmlTags.sCadenceFromOther, XmlConvert.ToString(m_cadenceFromOther));
            pluginNode.SetAttribute(xmlTags.sDeviceElevationFromOther, XmlConvert.ToString(m_deviceElevationFromOther));
            pluginNode.SetAttribute(xmlTags.sUseDeviceElevationForCalc, XmlConvert.ToString(m_useDeviceElevationForCalc));
            pluginNode.SetAttribute(xmlTags.sUseDeviceDistance, XmlConvert.ToString(UseDeviceDistance));
            pluginNode.SetAttribute(xmlTags.sShowSummaryTotal, XmlConvert.ToString(ShowSummaryTotal));
            pluginNode.SetAttribute(xmlTags.sShowSummaryAverage, XmlConvert.ToString(ShowSummaryAverage));
            pluginNode.SetAttribute(xmlTags.sShowTrailPointsOnChart, XmlConvert.ToString(ShowTrailPointsOnChart));
            pluginNode.SetAttribute(xmlTags.sShowTrailPointsOnMap, XmlConvert.ToString(ShowTrailPointsOnMap));
            pluginNode.SetAttribute(xmlTags.sUseTrailElevationAdjust, XmlConvert.ToString(m_useTrailElevationAdjust));
            pluginNode.SetAttribute(xmlTags.sRunningGradeAdjustMethod, m_RunningGradeAdjustMethod.ToString());
            pluginNode.SetAttribute(xmlTags.sMervynDaviesUp, XmlConvert.ToString(m_MervynDaviesUp));
            pluginNode.SetAttribute(xmlTags.sMervynDaviesDown, XmlConvert.ToString(m_MervynDaviesDown));
            pluginNode.SetAttribute(xmlTags.sJackDanielsUp, XmlConvert.ToString(m_JackDanielsUp));
            pluginNode.SetAttribute(xmlTags.sJackDanielsDown, XmlConvert.ToString(m_JackDanielsDown));
            String colText = null;
            //if (m_AdjustDiffSplitTimes != null)
            //{
            //    for (int i = 0; i < m_AdjustDiffSplitTimes.Length; i++)
            //    {
            //        float f = m_AdjustDiffSplitTimes[i / 2, i % 2];
            //        //if (i % 2 == 0)
            //        //{
            //        //    f = (float)GpsRunningPlugin.Util.UnitUtil.Distance.ConvertFrom(f);
            //        //}
            //        if (colText == null) { colText = f.ToString(); }
            //        else { colText += ";" + f; }
            //    }
            //}
            //pluginNode.SetAttribute(xmlTags.sAdjustDiffSplitTimes, colText);
            pluginNode.SetAttribute(xmlTags.sResultSummaryStdDev, XmlConvert.ToString(m_resultSummaryStdDev));

            pluginNode.SetAttribute(xmlTags.MaxAutoCalcActivitiesTrails, XmlConvert.ToString(m_MaxAutoCalcActivitiesTrails));
            pluginNode.SetAttribute(xmlTags.MaxAutoCalcActivitiesSingleTrail, XmlConvert.ToString(m_MaxAutoCalcActivitiesSingleTrail));

            colText = null;
            foreach (String column in m_activityPageColumns)
            {
                if (colText == null) { colText = column; }
                else { colText += ";" + column; }
                if (ActivityPageColumnsSizeGet(column) >= 0)
                {
                    colText += ":" + ActivityPageColumnsSizeGet(column);
                }
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
            if (!String.IsNullOrEmpty(m_saveChartImagePath))
            {
                pluginNode.SetAttribute(xmlTags.sSaveChartImagePath, m_saveChartImagePath);
            }
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
            public const string MaxAutoCalcActivitiesSingleTrail = "MaxAutoCalcActivitiesSingleTrail";
            public const string SetNameAtImport = "SetNameAtImport";
            public const string SetAdjustElevationAtImport = "SetAdjustElevationAtImport";
            public const string RestLapIsPause = "RestIsPause";
            public const string NonReqIsPause = "NonReqIsPause";
            public const string ResyncDiffAtTrailPoints = "ResyncDiffAtTrailPoints";
            public const string AdjustResyncDiffAtTrailPoints = "AdjustResyncDiffAtTrailPoints";
            public const string SyncChartAtTrailPoints = "SyncChartAtTrailPoints";
            public const string OnlyReferenceRight = "OnlyReferenceRight";
            public const string ZoomToSelection = "ZoomToSelection";
            public const string ShowOnlyMarkedOnRoute = "ShowOnlyMarkedOnRoute";
            public const string ExcludeStoppedCategory = "ExcludeStoppedCategory";
            public const string BarometricDevices = "BarometricDevices";
            public const string SmoothOverTrailPoints = "SmoothOverTrailPoints";
            public const string PredictDistance = "PredictDistance1";
            public const string sRunningGradeAdjustMethod = "sRunningGradeAdjustMethod";
            public const string sMervynDaviesUp = "sMervynDaviesUp";
            public const string sMervynDaviesDown = "sMervynDaviesDown";
            public const string sJackDanielsUp = "sJackDanielsUp";
            public const string sJackDanielsDown = "sJackDanielsDown";
            public const string sAdjustDiffSplitTimes = "sAdjustDiffSplitTimes";
            public const string sResultSummaryStdDev = "sResultSummaryStdDev";
            public const string sCadenceFromOther = "sCadenceFromOther";
            public const string sDeviceElevationFromOther = "sDeviceElevationFromOther";
            public const string sUseDeviceElevationForCalc = "sUseDeviceElevationForCalc";
            public const string sUseDeviceDistance = "sUseDeviceDistance";
            public const string sShowSummaryTotal = "sShowSummaryTotal";
            public const string sShowSummaryAverage = "sShowSummaryAverage";
            public const string sUseTrailElevationAdjust = "sUseTrailElevationAdjust";
            public const string sSaveChartImagePath = "sSaveChartImagePath";
            public const string sShowTrailPointsOnChart = "sShowTrailPointsOnChart";
            public const string sShowTrailPointsOnMap = "sShowTrailPointsOnMap";

            public const string sColumns = "sColumns";
        }

        //Parse dont care how the information was stored
        public static float parseFloat(string val)
        {
            float result;
            bool p = tryParseFloat(val, NumberStyles.Any, out result);
            if (!p) { result = float.NaN; }
            return result;
        }

        public static bool tryParseFloat(string val, NumberStyles style, out float result)
        {
            if (System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator !=
                System.Globalization.CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator)
            {
                val = val.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator,
                    System.Globalization.CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator);
            }
            return float.TryParse(val, style, System.Globalization.NumberFormatInfo.InvariantInfo, out result);
        }

        public static string printFullCategoryPath(IActivityCategory iActivityCategory)
        {
            return printFullCategoryPath(iActivityCategory, null, ": ");
        }
        private static string printFullCategoryPath(IActivityCategory iActivityCategory, string p, string sep)
        {
            if (iActivityCategory == null) return p;
            if (p == null)
                return printFullCategoryPath(iActivityCategory.Parent,
                             iActivityCategory.Name, sep);
            return printFullCategoryPath(iActivityCategory.Parent,
                                iActivityCategory.Name + sep + p, sep);
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
