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

using TrailsPlugin.Utils;
using ZoneFiveSoftware.Common.Data.Fitness;
using System.Drawing;

namespace TrailsPlugin.Data
{
    static class Settings
    {
        private static IList<string> m_activityPageColumns;
        private static IDictionary<string, int> m_activityPageColumnSizes;
        private static int m_activityPageNumFixedColumns;
        private static float m_defaultRadius;
        private static XAxisValue m_xAxisValue;
        private static LineChartTypes m_chartType;
        private static IList<LineChartTypes> m_MultiChartTypes;
        private static IList<LineChartTypes> m_MultiGraphTypes;
        private static bool m_ShowChartToolBar;
        private static bool m_SelectSimilarSplits;
        private static bool m_AddCurrentCategory;
        private static IList<string> m_SummaryViewSortColumns;
        private static ListSortDirection m_SummaryViewSortDirection;
        private static bool m_SetNameAtImport;
        private static bool m_SetAdjustElevationAtImport;
        private static int m_MaxAutoCalcActivitiesTrails;
        private static int m_MaxAutoCalcActivitiesSingleTrail;
        private static int m_MaxChartResults;
        private static bool m_RestLapIsPause;
        private static bool m_ShowPausesAsResults;
        private static bool m_NonReqIsPause;
        private static bool m_ResyncDiffAtTrailPoints;
        private static bool m_AdjustResyncDiffAtTrailPoints;
        private static bool m_SyncChartAtTrailPoints;
        private static bool m_OnlyReferenceRight;
        private static bool m_ZoomToSelection;
        private static bool m_ShowOnlyMarkedOnRoute;
        private static bool m_ShowActivityValuesForResults;
        private static bool m_ResultSummaryStdDev;
        private static bool m_ResultSummaryTotal;
        private static String[] m_ExcludeStoppedCategory;
        private static String[] m_BarometricDevices;
        private static SmoothOverTrailBorders m_SmoothOverTrailPoints;
        private static float m_PredictDistance;
        private static RunningGradeAdjustMethodEnum m_RunningGradeAdjustMethod;
        private static bool m_DeviceElevationFromOther;
        private static bool m_CadenceFromOther;
        private static bool m_UseDeviceElevationForCalc;
        private static bool m_UseTrailElevationAdjust;
        private static float[,] m_AdjustDiffSplitTimes;
        private static float[,] m_PandolfTerrainDist;
        private static float m_MervynDaviesUp;
        private static float m_MervynDaviesDown;
        private static float m_JackDanielsUp;
        private static float m_JackDanielsDown;
        private static string m_SaveChartImagePath;

        private static bool m_startDistOffsetFromStartPoint; //Not in xml
        private static bool m_diffUsingCommonStretches; //Not in xml
        private static bool m_UseDeviceDistance;
        private static bool m_ShowSummaryTotal;
        private static bool m_ShowSummaryAverage;
        private static bool m_ShowTrailPointsOnChart;
        private static bool m_ShowTrailPointsOnMap;
        private static bool m_ShowListToolBar;
        private static byte m_RouteLineAlpha;
        private static Size m_PopupSize = new Size(600, 450);
        private static int m_PopupDivider = 100;
        private static bool m_PopupInActionMenu = true;
        private static bool m_PopupUpdatedBySelection = false;

        //properties not in xml can be handled as variables, no write trigger needed
        public static bool OverlappingResultUseTimeOfDayDiff; //not in xml
        public static bool OverlappingResultUseReferencePauses; //not in xml
        public static bool OverlappingResultShareSplitTime; //not in xml
        public static bool ShowSummaryForChildren; //not in xml
        public static GpsFilterType UseGpsFilter; //not in xml
        public static int GpsFilterMaximumTime; //not in xml
        public static int GpsFilterMinimumTime; //not in xml
        public static float GpsFilterMinimumDistance; //not in xml
        public static int SelectSimilarModulu; //not in xml

        static Settings()
        {
            Defaults();
        }

        private static void Defaults()
        {
            m_activityPageColumns = TrailResultColumns.DefaultColumns();
            m_activityPageColumnSizes = new Dictionary<string, int>();
            m_activityPageNumFixedColumns = 0;
            m_defaultRadius = 30;
            m_xAxisValue = XAxisValue.Distance;
            m_chartType = LineChartTypes.SpeedPace;
            m_MultiChartTypes = LineChartUtil.DefaultLineChartTypes();
            m_MultiGraphTypes = LineChartUtil.DefaultLineChartTypes();
            m_ShowChartToolBar = true;
            m_SelectSimilarSplits = false;
            m_AddCurrentCategory = false;
            m_SummaryViewSortColumns = new List<string>(3) { TrailResultColumns.DefaultSortColumn() };
            m_SummaryViewSortDirection = ListSortDirection.Descending;
            m_SetNameAtImport = true;
            m_SetAdjustElevationAtImport = false;
            m_MaxAutoCalcActivitiesTrails = 10000;
            m_MaxAutoCalcActivitiesSingleTrail = 10000;
            m_MaxChartResults = 5;
            m_RestLapIsPause = false;
            m_ShowPausesAsResults = false;
            m_NonReqIsPause = false;
            m_ResyncDiffAtTrailPoints = true;
            m_AdjustResyncDiffAtTrailPoints = false;
            m_SyncChartAtTrailPoints = false;
            m_OnlyReferenceRight = false;
            m_ZoomToSelection = false;
            m_ShowOnlyMarkedOnRoute = false;
            m_ShowActivityValuesForResults = false;
            m_ResultSummaryStdDev = false;
            m_ResultSummaryTotal = false;
            m_ExcludeStoppedCategory = new String[0];
            m_BarometricDevices = new String[5] { "Edge", "920XT", "910XT", "fenix", "GB-580" };
            m_SmoothOverTrailPoints = SmoothOverTrailBorders.Unchanged;
            m_PredictDistance = 10000;
            m_RunningGradeAdjustMethod = RunningGradeAdjustMethodEnum.None;
            m_DeviceElevationFromOther = false;
            m_CadenceFromOther = false;
            m_UseDeviceElevationForCalc = false;
            m_UseTrailElevationAdjust = false;
            m_AdjustDiffSplitTimes = null;
            m_PandolfTerrainDist = null;
            m_MervynDaviesUp = 0.033f;
            m_MervynDaviesDown = 0.017f;
            m_JackDanielsUp = 15 / 1609f;
            m_JackDanielsDown = 8 / 1609f;
            m_SaveChartImagePath = null;

            m_startDistOffsetFromStartPoint = false;
            m_diffUsingCommonStretches = false;
            m_UseDeviceDistance = false;
            m_ShowSummaryTotal = true;
            m_ShowSummaryAverage = true;
            m_ShowTrailPointsOnChart = true;
            m_ShowTrailPointsOnMap = true;
            m_ShowListToolBar = true;
            m_RouteLineAlpha = 0xa0;

            OverlappingResultUseTimeOfDayDiff = false;
            OverlappingResultUseReferencePauses = false;
            OverlappingResultShareSplitTime = false;
            ShowSummaryForChildren = false;
            UseGpsFilter = GpsFilterType.None;
            GpsFilterMinimumTime = 2;
            GpsFilterMaximumTime = 5;
            GpsFilterMinimumDistance = 10f;
            SelectSimilarModulu = 0;

            m_PopupSize = new Size(600, 450);
            m_PopupDivider = 100;
            m_PopupInActionMenu = true;
            m_PopupUpdatedBySelection = false;
        }

        private static bool isHandlingXml = false;
        private static void WriteExtensionData()
        {
            //Intercept triggering of xml update while parsing/writing xml
            if (!isHandlingXml)
            {
                //The settings are stored in preferences, so this trigger is currently not used
                //Plugin.WriteExtensionData();
            }
        }

        //Temporary hack to translate to strings
        public static LineChartTypes ChartType
        {
            get
            {
                return m_chartType;
            }
            set
            {
                m_chartType = value; WriteExtensionData();
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
                m_MultiChartTypes = LineChartUtil.ParseLineChartType(value); WriteExtensionData();
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
                WriteExtensionData();
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
                m_MultiGraphTypes = LineChartUtil.ParseLineChartType(value); WriteExtensionData();
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
                    m_MultiGraphTypes.Add(value); WriteExtensionData();
                }
            }
        }

        public static XAxisValue XAxisValue
        {
            get {
                return m_xAxisValue;
            }
            set {
                m_xAxisValue = value; WriteExtensionData();
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
                m_activityPageColumns = value; WriteExtensionData();
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
            m_activityPageColumnSizes[id] = value; WriteExtensionData();
        }

        public static int ActivityPageNumFixedColumns
        {
            get
            {
                return m_activityPageNumFixedColumns;
            }
            set {
                m_activityPageNumFixedColumns = value; WriteExtensionData();
            }
        }

        public static float DefaultRadius
        {
            get
            {
                return m_defaultRadius;
            }
            set {
                m_defaultRadius = value; WriteExtensionData();
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
                m_ShowChartToolBar = value; WriteExtensionData();
            }
        }

        public static bool SelectSimilarSplits
        {
            get
            {
                return m_SelectSimilarSplits;
            }
            set
            {
                m_SelectSimilarSplits = value; WriteExtensionData();
            }
        }

        public static bool AddCurrentCategory
        {
            get
            {
                return m_AddCurrentCategory;
            }
            set
            {
                m_AddCurrentCategory = value; WriteExtensionData();
            }
        }

        public static IList<string> SummaryViewSortColumns
        {
            get { return m_SummaryViewSortColumns; }
        }

        public static string UpdateSummaryViewSortColumn
        {
            set
            {
                if (!string.IsNullOrEmpty(value)
                    && !TrailResultColumnIds.ObsoleteFields.Contains(value))
                {
                    m_SummaryViewSortColumns.Remove(value);
                    if (m_SummaryViewSortColumns.Count >= ((List < string > )m_SummaryViewSortColumns).Capacity)
                    {
                        m_SummaryViewSortColumns.RemoveAt(m_SummaryViewSortColumns.Count - 1);
                    }
                    m_SummaryViewSortColumns.Insert(0, value);
                }
                WriteExtensionData();
            }
        }

        public static string GetSummaryViewSortColumns
        {
            get
            {
                string s = "";
                foreach (string col in m_SummaryViewSortColumns)
                {
                    s += col + ",";
                }
                return s;
            }
        }

        public static ListSortDirection SummaryViewSortDirection
        {
            get { return m_SummaryViewSortDirection; }
            set { m_SummaryViewSortDirection = value; WriteExtensionData(); }
        }

        public static bool SetNameAtImport
        {
            get { return m_SetNameAtImport; }
            set { m_SetNameAtImport = value; WriteExtensionData(); }
        }

        public static bool SetAdjustElevationAtImport
        {
            get { return m_SetAdjustElevationAtImport; }
            set { m_SetAdjustElevationAtImport = value; WriteExtensionData(); }
        }

        public static int MaxAutoCalcActivitiesTrails
        {
            get { return m_MaxAutoCalcActivitiesTrails; }
            set { m_MaxAutoCalcActivitiesTrails = value; WriteExtensionData(); }
        }

        public static int MaxAutoCalcActitiesSingleTrail
        {
            get { return m_MaxAutoCalcActivitiesSingleTrail; }
            set { m_MaxAutoCalcActivitiesSingleTrail = value; WriteExtensionData(); }
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
            get { return m_RestLapIsPause; }
            set { m_RestLapIsPause = value; WriteExtensionData(); }
        }

        public static bool NonReqIsPause
        {
            get { return m_NonReqIsPause; }
            set { m_NonReqIsPause = value; WriteExtensionData(); }
        }

        public static bool ResyncDiffAtTrailPoints
        {
            get { return m_ResyncDiffAtTrailPoints; }
            set { m_ResyncDiffAtTrailPoints = value; WriteExtensionData(); }
        }

        public static bool AdjustResyncDiffAtTrailPoints
        {
            get { return m_AdjustResyncDiffAtTrailPoints; }
            set { m_AdjustResyncDiffAtTrailPoints = value; WriteExtensionData(); }
        }

        public static bool SyncChartAtTrailPoints
        {
            get { return m_SyncChartAtTrailPoints; }
            set { m_SyncChartAtTrailPoints = value; WriteExtensionData(); }
        }

        public static bool OnlyReferenceRight
        {
            get { return m_OnlyReferenceRight; }
            set { m_OnlyReferenceRight = value; WriteExtensionData(); }
        }

        public static String[] ExcludeStoppedCategory
        {
            get { return m_ExcludeStoppedCategory; }
        }

        public static String GetExcludeStoppedCategory
        {
            get
            {
                string s = "";
                foreach (string i in m_ExcludeStoppedCategory)
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

        public static void SetExcludeStoppedCategory(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                m_ExcludeStoppedCategory = new string[0];
            }
            else
            {
                m_ExcludeStoppedCategory = s.Split(';');
            }
            WriteExtensionData();
        }

        public static String[] BarometricDevices
        {
            get { return m_BarometricDevices; }
        }

        public static String GetBarometricDevices
        {
            get
            {
                string s = "";
                foreach (string i in m_BarometricDevices)
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
                m_BarometricDevices = new string[0];
            }
            else
            {
                m_BarometricDevices = s.Split(';');
            }
            WriteExtensionData();
        }

        public static bool StartDistOffsetFromStartPoint
        {
            get { return m_startDistOffsetFromStartPoint; }
            set { m_startDistOffsetFromStartPoint = value; WriteExtensionData(); }
        }

        public static bool DiffUsingCommonStretches
        {
            get { return m_diffUsingCommonStretches; }
            set { m_diffUsingCommonStretches = value; WriteExtensionData(); }
        }

        public static bool ZoomToSelection
        {
            get { return m_ZoomToSelection; }
            set { m_ZoomToSelection = value; WriteExtensionData(); }
        }

        public static bool ShowOnlyMarkedOnRoute
        {
            get { return m_ShowOnlyMarkedOnRoute; }
            set { m_ShowOnlyMarkedOnRoute = value; WriteExtensionData(); }
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
                m_SmoothOverTrailPoints++; WriteExtensionData();
            }
        }
        ///
        /// Predict distances in PerformancePredictor
        public static float PredictDistance
        {
            get
            {
                return m_PredictDistance;
            }
            set
            {
                m_PredictDistance = value; WriteExtensionData();
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
                m_RunningGradeAdjustMethod = value; WriteExtensionData();
            }
        }

        internal static void IncreaseRunningGradeCalcMethod(bool increase)
        {
            if (increase)
            {
                m_RunningGradeAdjustMethod++;
                if (m_RunningGradeAdjustMethod == RunningGradeAdjustMethodEnum.Last)
                {
                    m_RunningGradeAdjustMethod = RunningGradeAdjustMethodEnum.None;
                }
            }
            else
            {
                if (m_RunningGradeAdjustMethod == RunningGradeAdjustMethodEnum.None)
                {
                    m_RunningGradeAdjustMethod = RunningGradeAdjustMethodEnum.Last;
                }
                m_RunningGradeAdjustMethod--;
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
                m_MervynDaviesUp = value; WriteExtensionData();
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
                m_MervynDaviesDown = value; WriteExtensionData();
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
                m_JackDanielsUp = value; WriteExtensionData();
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
                m_JackDanielsDown = value; WriteExtensionData();
            }
        }

        public static bool CadenceFromOther
        {
            get
            {
                return m_CadenceFromOther;
            }
            set
            {
                m_CadenceFromOther = value; WriteExtensionData();
            }
        }

        public static bool DeviceElevationFromOther
        {
            get
            {
                return m_DeviceElevationFromOther;
            }
            set
            {
                m_DeviceElevationFromOther = value; WriteExtensionData();
            }
        }

        public static bool UseDeviceElevationForCalc
        {
            get
            {
                return m_UseDeviceElevationForCalc;
            }
            set
            {
                m_UseDeviceElevationForCalc = value; WriteExtensionData();
            }
        }

        public static bool UseTrailElevationAdjust
        {
            get
            {
                return m_UseTrailElevationAdjust;
            }
            set
            {
                m_UseTrailElevationAdjust = value; WriteExtensionData();
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
                m_AdjustDiffSplitTimes = value; WriteExtensionData();
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
                m_PandolfTerrainDist = value; WriteExtensionData();
            }
        }

        /// <summary>
        /// Show standard deviation for certain fields in the summary
        /// </summary>
        public static bool ResultSummaryStdDev
        {
            get { return m_ResultSummaryStdDev; }
            set { m_ResultSummaryStdDev = value; WriteExtensionData(); }
        }

        public static bool ResultSummaryTotal
        {
            get { return m_ResultSummaryTotal; }
            set { m_ResultSummaryTotal = value; WriteExtensionData(); }
        }

        /// <summary>
        /// Show the summary from the activity, instead of the track result summaries
        /// </summary>
        public static bool ShowActivityValuesForResults
        {
            get { return m_ShowActivityValuesForResults; }
            set { m_ShowActivityValuesForResults = value; WriteExtensionData(); }
        }
        public static string SaveChartImagePath
        {
            get
            {
                if (m_SaveChartImagePath == null) { m_SaveChartImagePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); }
                return m_SaveChartImagePath;
            }
            set { m_SaveChartImagePath = value; WriteExtensionData(); }
        }

        public static int MaxChartResults
        {
            get { return m_MaxChartResults; }
            set
            {
                m_MaxChartResults = value;
                WriteExtensionData();
            }
        }

        public static bool UseDeviceDistance
        {
            get { return m_UseDeviceDistance; }
            set
            {
                m_UseDeviceDistance = value;
                WriteExtensionData();
            }
        }

        public static bool ShowPausesAsResults
        {
            get { return m_ShowPausesAsResults; }
            set
            {
                m_ShowPausesAsResults = value;
                WriteExtensionData();
            }
        }

        public static bool ShowSummaryTotal
        {
            get { return m_ShowSummaryTotal; }
            set
            {
                m_ShowSummaryTotal = value;
                WriteExtensionData();
            }
        }

        public static bool ShowSummaryAverage
        {
            get { return m_ShowSummaryAverage; }
            set
            {
                m_ShowSummaryAverage = value;
                WriteExtensionData();
            }
        }

        public static bool ShowTrailPointsOnChart
        {
            get { return m_ShowTrailPointsOnChart; }
            set
            {
                m_ShowTrailPointsOnChart = value;
                WriteExtensionData();
            }
        }

        public static bool ShowTrailPointsOnMap
        {
            get { return m_ShowTrailPointsOnMap; }
            set
            {
                m_ShowTrailPointsOnMap = value;
                WriteExtensionData();
            }
        }

        public static bool ShowListToolBar
        {
            get { return m_ShowListToolBar; }
            set
            {
                m_ShowListToolBar = value;
                WriteExtensionData();
            }
        }

        public static byte RouteLineAlpha
        {
            get { return m_RouteLineAlpha; }
            set
            {
                m_RouteLineAlpha = value;
                WriteExtensionData();
            }
        }

        public static Size PopupSize
        {
            get { return m_PopupSize; }
            set
            {
                m_PopupSize = value;
                WriteExtensionData();
            }
        }

        public static int PopupDivider
        {
            get { return m_PopupDivider; }
            set
            {
                m_PopupDivider = value;
                WriteExtensionData();
            }
        }

        public static bool PopupInActionMenu
        {
            get { return m_PopupInActionMenu; }
            set
            {
                m_PopupInActionMenu = value;
                WriteExtensionData();
            }
        }

        public static bool PopupUpdatedBySelection
        {
            get { return m_PopupUpdatedBySelection; }
            set
            {
                m_PopupUpdatedBySelection = value;
                WriteExtensionData();
            }
        }

        /******************************************************/
        public static void ReadOptions(XmlDocument xmlDoc, XmlNamespaceManager nsmgr, XmlElement pluginNode)
        {
            Defaults();
            isHandlingXml = true;
            try
            {
                String attr;
                attr = pluginNode.GetAttribute(XmlTags.sDefaultRadius);
                if (attr.Length > 0) { m_defaultRadius = Settings.ParseFloat(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sNumFixedColumns);
                if (attr.Length > 0) { m_activityPageNumFixedColumns = (Int16)XmlConvert.ToInt16(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sXAxis);
                if (attr.Length > 0) { m_xAxisValue = (XAxisValue)Enum.Parse(typeof(XAxisValue), attr, true); }
                attr = pluginNode.GetAttribute(XmlTags.sChartType);
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
                attr = pluginNode.GetAttribute(XmlTags.SelectSimilarSplits);
                if (attr.Length > 0) { m_SelectSimilarSplits = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.AddCurrentActivity);
                if (attr.Length > 0) { m_AddCurrentCategory = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.ShowChartToolBar);
                if (attr.Length > 0) { m_ShowChartToolBar = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.ShowListToolBar);
                if (attr.Length > 0) { m_ShowListToolBar = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.SetNameAtImport);
                if (attr.Length > 0) { m_SetNameAtImport = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.SetAdjustElevationAtImport);
                if (attr.Length > 0) { m_SetAdjustElevationAtImport = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.RestLapIsPause);
                if (attr.Length > 0) { m_RestLapIsPause = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sShowPausesAsResults);
                if (attr.Length > 0) { m_ShowPausesAsResults = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.NonReqIsPause);
                if (attr.Length > 0) { m_NonReqIsPause = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.ResyncDiffAtTrailPoints);
                if (attr.Length > 0) { m_ResyncDiffAtTrailPoints = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.AdjustResyncDiffAtTrailPoints);
                if (attr.Length > 0) { m_AdjustResyncDiffAtTrailPoints = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.SyncChartAtTrailPoints);
                if (attr.Length > 0) { m_SyncChartAtTrailPoints = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.OnlyReferenceRight);
                if (attr.Length > 0) { m_OnlyReferenceRight = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.ZoomToSelection);
                if (attr.Length > 0) { m_ZoomToSelection = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.ShowOnlyMarkedOnRoute);
                if (attr.Length > 0) { m_ShowOnlyMarkedOnRoute = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.ExcludeStoppedCategory);
                if (attr.Length > 0) { SetExcludeStoppedCategory(attr); }
                attr = pluginNode.GetAttribute(XmlTags.BarometricDevices);
                if (attr.Length > 0) { SetBarometricDevices(attr); }
                attr = pluginNode.GetAttribute(XmlTags.SmoothOverTrailPoints);
                try
                {
                    if (attr.Length > 0) { m_SmoothOverTrailPoints = (SmoothOverTrailBorders)Enum.Parse(typeof(SmoothOverTrailBorders), attr, true); }
                }
                catch { }
                attr = pluginNode.GetAttribute(XmlTags.PredictDistance);
                if (attr.Length > 0) { m_PredictDistance = Settings.ParseFloat(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sCadenceFromOther);
                if (attr.Length > 0) { m_CadenceFromOther = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sDeviceElevationFromOther);
                if (attr.Length > 0) { m_DeviceElevationFromOther = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sUseDeviceElevationForCalc);
                if (attr.Length > 0) { m_UseDeviceElevationForCalc = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sUseDeviceDistance);
                if (attr.Length > 0) { m_UseDeviceDistance = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sShowSummaryTotal);
                if (attr.Length > 0) { m_ShowSummaryTotal = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sShowSummaryAverage);
                if (attr.Length > 0) { m_ShowSummaryAverage = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sShowTrailPointsOnChart);
                if (attr.Length > 0) { m_ShowTrailPointsOnChart = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sShowTrailPointsOnMap);
                if (attr.Length > 0) { m_ShowTrailPointsOnMap = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sUseTrailElevationAdjust);
                if (attr.Length > 0) { m_UseTrailElevationAdjust = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sRunningGradeAdjustMethod);
                try
                {
                    if (attr.Length > 0) { m_RunningGradeAdjustMethod = (RunningGradeAdjustMethodEnum)Enum.Parse(typeof(RunningGradeAdjustMethodEnum), attr, true); }
                }
                catch { }
                attr = pluginNode.GetAttribute(XmlTags.sMervynDaviesUp);
                if (attr.Length > 0) { m_MervynDaviesUp = Settings.ParseFloat(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sMervynDaviesDown);
                if (attr.Length > 0) { m_MervynDaviesDown = Settings.ParseFloat(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sJackDanielsUp);
                if (attr.Length > 0) { m_JackDanielsUp = Settings.ParseFloat(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sJackDanielsDown);
                if (attr.Length > 0) { m_JackDanielsDown = Settings.ParseFloat(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sRouteLineAlpha);
                if (attr.Length > 0) { m_RouteLineAlpha = XmlConvert.ToByte(attr); }
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
                attr = pluginNode.GetAttribute(XmlTags.sResultSummaryStdDev);
                if (attr.Length > 0) { m_ResultSummaryStdDev = XmlConvert.ToBoolean(attr); }

                attr = pluginNode.GetAttribute(XmlTags.MaxAutoCalcActivitiesTrails);
                if (attr.Length > 0)
                {
                    m_MaxAutoCalcActivitiesTrails = XmlConvert.ToInt32(attr);
                    //Change defaults without version number
                    if (m_MaxAutoCalcActivitiesTrails == 500) { m_MaxAutoCalcActivitiesTrails = 10000; }
                }
                attr = pluginNode.GetAttribute(XmlTags.MaxAutoCalcActivitiesSingleTrail);
                if (attr.Length > 0) {
                    m_MaxAutoCalcActivitiesSingleTrail = XmlConvert.ToInt32(attr);
                    if (m_MaxAutoCalcActivitiesSingleTrail == 200) { m_MaxAutoCalcActivitiesSingleTrail = 10000; }
                }
                attr = pluginNode.GetAttribute(XmlTags.sMaxChartResults);
                if (attr.Length > 0) { m_MaxChartResults = XmlConvert.ToInt32(attr); }

                int popupWidth = 0;
                int popupHeight = 0;
                attr = pluginNode.GetAttribute(XmlTags.sPopupSizeWidth);
                if (attr.Length > 0) { popupWidth = XmlConvert.ToInt16(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sPopupSizeHeight);
                if (attr.Length > 0) { popupHeight = XmlConvert.ToInt16(attr); }
                if (popupWidth > 0 && popupHeight > 0)
                {
                    m_PopupSize = new Size(popupWidth, popupHeight);
                }
                attr = pluginNode.GetAttribute(XmlTags.sPopupDivider);
                if (attr.Length > 0) { m_PopupDivider = XmlConvert.ToInt32(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sPopupInActionMenu);
                if (attr.Length > 0) { m_PopupInActionMenu = XmlConvert.ToBoolean(attr); }
                attr = pluginNode.GetAttribute(XmlTags.sPopupUpdatedBySelection);
                if (attr.Length > 0) { m_PopupUpdatedBySelection = XmlConvert.ToBoolean(attr); }

                 attr = pluginNode.GetAttribute(XmlTags.sColumns);
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
                            if (int.TryParse(idWidth[1], out int width))
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
                m_SummaryViewSortColumns.Clear();
                attr = pluginNode.GetAttribute(XmlTags.summaryViewSortColumn);
                if (attr.Length > 0) {
                    String[] values = attr.Split(',');
                    foreach (String column in values)
                    {
                        UpdateSummaryViewSortColumn = column;
                    }
                }
                if (m_SummaryViewSortColumns.Count == 0 && m_activityPageColumns != null && m_activityPageColumns.Count > 1)
                { UpdateSummaryViewSortColumn = m_activityPageColumns[1]; }

                attr = pluginNode.GetAttribute(XmlTags.summaryViewSortDirection);
                if (attr.Length > 0) { m_SummaryViewSortDirection = (ListSortDirection)Enum.Parse(typeof(ListSortDirection), attr); }

                attr = pluginNode.GetAttribute(XmlTags.sMultiChartType);
                if (attr.Length > 0)
                {
                    String[] values = attr.Split(';');
                    SetMultiChartType = values;
                }

                attr = pluginNode.GetAttribute(XmlTags.sMultiGraphType);
                if (attr.Length > 0)
                {
                    String[] values = attr.Split(';');
                    SetMultiGraphType = values;
                }
                attr = pluginNode.GetAttribute(XmlTags.sSaveChartImagePath);
                if (attr.Length > 0) { m_SaveChartImagePath = attr; }
            }
            catch { }
            isHandlingXml = false;
        }

        public static void WriteOptions(XmlDocument xmlDoc, XmlElement pluginNode)
        {
            isHandlingXml = true;
            pluginNode.SetAttribute(XmlTags.sDefaultRadius, XmlConvert.ToString(m_defaultRadius));
            pluginNode.SetAttribute(XmlTags.sNumFixedColumns, XmlConvert.ToString(m_activityPageNumFixedColumns));
            pluginNode.SetAttribute(XmlTags.sXAxis, m_xAxisValue.ToString());
            pluginNode.SetAttribute(XmlTags.sChartType, m_chartType.ToString());
            pluginNode.SetAttribute(XmlTags.summaryViewSortColumn, GetSummaryViewSortColumns);
            pluginNode.SetAttribute(XmlTags.summaryViewSortDirection, m_SummaryViewSortDirection.ToString());
            pluginNode.SetAttribute(XmlTags.ShowChartToolBar, XmlConvert.ToString(m_ShowChartToolBar));
            pluginNode.SetAttribute(XmlTags.ShowListToolBar, XmlConvert.ToString(m_ShowListToolBar));
            pluginNode.SetAttribute(XmlTags.SelectSimilarSplits, XmlConvert.ToString(m_SelectSimilarSplits));
            pluginNode.SetAttribute(XmlTags.AddCurrentActivity, XmlConvert.ToString(m_AddCurrentCategory));
            pluginNode.SetAttribute(XmlTags.SetNameAtImport, XmlConvert.ToString(m_SetNameAtImport));
            pluginNode.SetAttribute(XmlTags.SetAdjustElevationAtImport, XmlConvert.ToString(m_SetAdjustElevationAtImport));
            pluginNode.SetAttribute(XmlTags.RestLapIsPause, XmlConvert.ToString(m_RestLapIsPause));
            pluginNode.SetAttribute(XmlTags.sShowPausesAsResults, XmlConvert.ToString(m_ShowPausesAsResults));
            pluginNode.SetAttribute(XmlTags.NonReqIsPause, XmlConvert.ToString(m_NonReqIsPause));
            pluginNode.SetAttribute(XmlTags.ResyncDiffAtTrailPoints, XmlConvert.ToString(m_ResyncDiffAtTrailPoints));
            pluginNode.SetAttribute(XmlTags.AdjustResyncDiffAtTrailPoints, XmlConvert.ToString(m_AdjustResyncDiffAtTrailPoints));
            pluginNode.SetAttribute(XmlTags.SyncChartAtTrailPoints, XmlConvert.ToString(m_SyncChartAtTrailPoints));
            pluginNode.SetAttribute(XmlTags.OnlyReferenceRight, XmlConvert.ToString(m_OnlyReferenceRight));
            pluginNode.SetAttribute(XmlTags.ZoomToSelection, XmlConvert.ToString(m_ZoomToSelection));
            pluginNode.SetAttribute(XmlTags.ShowOnlyMarkedOnRoute, XmlConvert.ToString(m_ShowOnlyMarkedOnRoute));
            pluginNode.SetAttribute(XmlTags.ExcludeStoppedCategory, GetExcludeStoppedCategory);
            pluginNode.SetAttribute(XmlTags.BarometricDevices, GetBarometricDevices);
            pluginNode.SetAttribute(XmlTags.SmoothOverTrailPoints, m_SmoothOverTrailPoints.ToString());
            pluginNode.SetAttribute(XmlTags.PredictDistance, XmlConvert.ToString(m_PredictDistance));
            pluginNode.SetAttribute(XmlTags.sCadenceFromOther, XmlConvert.ToString(m_CadenceFromOther));
            pluginNode.SetAttribute(XmlTags.sDeviceElevationFromOther, XmlConvert.ToString(m_DeviceElevationFromOther));
            pluginNode.SetAttribute(XmlTags.sUseDeviceElevationForCalc, XmlConvert.ToString(m_UseDeviceElevationForCalc));
            pluginNode.SetAttribute(XmlTags.sUseDeviceDistance, XmlConvert.ToString(m_UseDeviceDistance));
            pluginNode.SetAttribute(XmlTags.sShowSummaryTotal, XmlConvert.ToString(m_ShowSummaryTotal));
            pluginNode.SetAttribute(XmlTags.sShowSummaryAverage, XmlConvert.ToString(m_ShowSummaryAverage));
            pluginNode.SetAttribute(XmlTags.sShowTrailPointsOnChart, XmlConvert.ToString(m_ShowTrailPointsOnChart));
            pluginNode.SetAttribute(XmlTags.sShowTrailPointsOnMap, XmlConvert.ToString(m_ShowTrailPointsOnMap));
            pluginNode.SetAttribute(XmlTags.sUseTrailElevationAdjust, XmlConvert.ToString(m_UseTrailElevationAdjust));
            pluginNode.SetAttribute(XmlTags.sRunningGradeAdjustMethod, m_RunningGradeAdjustMethod.ToString());
            pluginNode.SetAttribute(XmlTags.sMervynDaviesUp, XmlConvert.ToString(m_MervynDaviesUp));
            pluginNode.SetAttribute(XmlTags.sMervynDaviesDown, XmlConvert.ToString(m_MervynDaviesDown));
            pluginNode.SetAttribute(XmlTags.sJackDanielsUp, XmlConvert.ToString(m_JackDanielsUp));
            pluginNode.SetAttribute(XmlTags.sJackDanielsDown, XmlConvert.ToString(m_JackDanielsDown));
            pluginNode.SetAttribute(XmlTags.sRouteLineAlpha, XmlConvert.ToString(m_RouteLineAlpha));
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
            pluginNode.SetAttribute(XmlTags.sResultSummaryStdDev, XmlConvert.ToString(m_ResultSummaryStdDev));

            pluginNode.SetAttribute(XmlTags.MaxAutoCalcActivitiesTrails, XmlConvert.ToString(m_MaxAutoCalcActivitiesTrails));
            pluginNode.SetAttribute(XmlTags.MaxAutoCalcActivitiesSingleTrail, XmlConvert.ToString(m_MaxAutoCalcActivitiesSingleTrail));
            pluginNode.SetAttribute(XmlTags.sMaxChartResults, XmlConvert.ToString(m_MaxChartResults));

            pluginNode.SetAttribute(XmlTags.sPopupSizeWidth, XmlConvert.ToString(m_PopupSize.Width));
            pluginNode.SetAttribute(XmlTags.sPopupSizeHeight, XmlConvert.ToString(m_PopupSize.Height));
            pluginNode.SetAttribute(XmlTags.sPopupDivider, XmlConvert.ToString(m_PopupDivider));
            pluginNode.SetAttribute(XmlTags.sPopupInActionMenu, XmlConvert.ToString(m_PopupInActionMenu));
            pluginNode.SetAttribute(XmlTags.sPopupUpdatedBySelection, XmlConvert.ToString(m_PopupUpdatedBySelection));

            colText = null;
            foreach (String column in m_activityPageColumns)
            {
                if (TrailResultColumns.IsColumn(column))
                {
                    if (colText == null) { colText = column; }
                    else { colText += ";" + column; }
                    if (ActivityPageColumnsSizeGet(column) >= 0)
                    {
                        colText += ":" + ActivityPageColumnsSizeGet(column);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.Assert(false, string.Format("Ignoring unknown column {0}", column));
                }
            }
            pluginNode.SetAttribute(XmlTags.sColumns, colText);

            colText = null;
            foreach (LineChartTypes column in m_MultiChartTypes)
            {
                if (colText == null) { colText = column.ToString(); }
                else { colText += ";" + column.ToString(); }
            }
            pluginNode.SetAttribute(XmlTags.sMultiChartType, colText);

            colText = null;
            foreach (LineChartTypes column in m_MultiGraphTypes)
            {
                if (colText == null) { colText = column.ToString(); }
                else { colText += ";" + column.ToString(); }
            }
            pluginNode.SetAttribute(XmlTags.sMultiGraphType, colText);
            if (!String.IsNullOrEmpty(m_SaveChartImagePath))
            {
                pluginNode.SetAttribute(XmlTags.sSaveChartImagePath, m_SaveChartImagePath);
            }
            isHandlingXml = false;
        }

        private class XmlTags
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
            public const string ShowListToolBar = "ShowListToolBar";
            public const string SelectSimilarSplits = "SelectSimilarSplits";
            public const string AddCurrentActivity = "AddCurrentActivity";
            public const string MaxAutoCalcActivitiesTrails = "MaxAutoCalcActivitiesTrails";
            public const string MaxAutoCalcActivitiesSingleTrail = "MaxAutoCalcActivitiesSingleTrail";
            public const string sMaxChartResults = "sMaxChartResults";
            public const string SetNameAtImport = "SetNameAtImport";
            public const string SetAdjustElevationAtImport = "SetAdjustElevationAtImport";
            public const string RestLapIsPause = "RestIsPause";
            public const string sShowPausesAsResults = "sShowPausesAsResults";
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
            public const string sRouteLineAlpha = "sRouteLineAlpha";
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

            public const string sPopupSizeWidth = "sPopupSizeWidth";
            public const string sPopupSizeHeight = "sPopupSizeHeight";
            public const string sPopupDivider = "sPopupDivider";
            public const string sPopupInActionMenu = "sPopupInActionMenu";
            public const string sPopupUpdatedBySelection = "sPopupUpdatedBySelection";

            public const string sColumns = "sColumns";
        }

        //Parse dont care how the information was stored
        public static float ParseFloat(string val)
        {
            bool p = TryParseFloat(val, NumberStyles.Any, out float result);
            if (!p) { result = float.NaN; }
            return result;
        }

        public static bool TryParseFloat(string val, NumberStyles style, out float result)
        {
            if (System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator !=
                System.Globalization.CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator)
            {
                val = val.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator,
                    System.Globalization.CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator);
            }
            return float.TryParse(val, style, System.Globalization.NumberFormatInfo.InvariantInfo, out result);
        }

        public static string PrintFullCategoryPath(IActivityCategory iActivityCategory)
        {
            return PrintFullCategoryPath(iActivityCategory, null, ": ");
        }

        private static string PrintFullCategoryPath(IActivityCategory iActivityCategory, string p, string sep)
        {
            if (iActivityCategory == null) return p;
            if (p == null)
                return PrintFullCategoryPath(iActivityCategory.Parent,
                             iActivityCategory.Name, sep);
            return PrintFullCategoryPath(iActivityCategory.Parent,
                                iActivityCategory.Name + sep + p, sep);
        }
    }
}
