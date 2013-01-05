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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Chart;

#if ST_2_1
//SaveImage
using ZoneFiveSoftware.SportTracks.UI.Forms;
using TrailsPlugin.Data;
#else
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ZoneFiveSoftware.Common.Visuals.Forms;
using TrailsPlugin.UI.MapLayers;
#endif
using TrailsPlugin.Data;
using TrailsPlugin.Utils;
using GpsRunningPlugin.Util;

namespace TrailsPlugin.UI.Activity {
	public partial class TrailLineChart : UserControl {
        private Data.TrailResult m_refTrailResult = null;
        private IList<Data.TrailResult> m_trailResults = new List<Data.TrailResult>();

        private XAxisValue m_XAxisReferential = XAxisValue.Time;
        private IList<LineChartTypes> m_ChartTypes = new List<LineChartTypes>();
        private IDictionary<LineChartTypes, IAxis> m_axisCharts = new Dictionary<LineChartTypes, IAxis>();
        private LineChartTypes m_lastSelectedType = LineChartTypes.Unknown;
        private IDictionary<LineChartTypes, bool> m_hasValues = null;

        private bool m_multipleCharts = false;
        private bool m_visible = false;
        private ITheme m_visualTheme;
        private ActivityDetailPageControl m_page;
        private MultiChartsControl m_multiple;
        private bool m_selectDataHandler = true; //Event handler is enabled by default
        private bool m_showTrailPoints = true;
        private bool refIsSelf = false;
        private TrailPointsLayer m_layer;

        const int MaxSelectedSeries = 5;
        private static SyncGraphMode syncGraph = SyncGraphMode.None;
        public TrailLineChart()
        {
            InitializeComponent();
            InitControls();
        }

        void InitControls()
        {
#if ST_2_1
            this.MainChart.Margin = 0;
#else
            this.MainChart.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
#endif
            MainChart.YAxis.SmartZoom = true;
            copyChartMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.DocumentCopy16;
            copyChartMenuItem.Visible = false;
#if !ST_2_1
            saveImageMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Save16;
#endif
            //selectChartsMenuItem.Image = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Table16;
            selectChartsMenuItem.Visible = false;
#if !ST_2_1
//            this.listSettingsMenuItem.Click += new System.EventHandler(this.listSettingsToolStripMenuItem_Click);
#else
//            //No listSetting dialog in ST2
//            if (this.contextMenu.Items.Contains(this.listSettingsMenuItem))
//            {
//                this.contextMenu.Items.Remove(this.listSettingsMenuItem);
//            }
#endif
            fitToWindowMenuItem.Image = Properties.Resources.ZoomToContent;
        }

        public void SetControl(ActivityDetailPageControl page, MultiChartsControl multiple, TrailPointsLayer layer)
        {
            m_page = page;
            m_multiple = multiple;
            m_layer = layer;
        }

        public void ThemeChanged(ITheme visualTheme)
        {
            m_visualTheme = visualTheme;
            MainChart.ThemeChanged(visualTheme);
            ButtonPanel.ThemeChanged(visualTheme);
            ButtonPanel.BackColor = visualTheme.Window;
        }

        public void UICultureChanged(CultureInfo culture)
        {
            copyChartMenuItem.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionCopy;
#if ST_2_1
            saveImageMenuItem.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionSave;
#else
            saveImageMenuItem.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionSaveImage;
#endif
            fitToWindowMenuItem.Text = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionRefresh;
            SetupAxes();
        }

        public bool ShowPage
        {
            get
            {
                return m_visible;
            }
            set
            {
                m_visible = value;
                if (value)
                {
                    SetupAxes();
                    SetupDataSeries();
                }
            }
        }

        /********************************************************************************/

        private void SaveImageButton_Click(object sender, EventArgs e) {
#if ST_2_1
            SaveImage dlg = new SaveImage();
#else
            SaveImageDialog dlg = new SaveImageDialog();
#endif
            dlg.ThemeChanged(m_visualTheme);
            dlg.FileName = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + Path.DirectorySeparatorChar + "Trails";
            dlg.ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
			if (dlg.ShowDialog() == DialogResult.OK) {
				Size imgSize = dlg.CustomImageSize;

#if ST_2_1
                if (dlg.ImageSize != SaveImage.ImageSizeType.Custom)
#else
                if (dlg.ImageSize != SaveImageDialog.ImageSizeType.Custom)
#endif
                {
					imgSize = dlg.ImageSizes[dlg.ImageSize];
				}
				MainChart.SaveImage(imgSize, dlg.FileName, dlg.ImageFormat);
			}

			MainChart.Focus();
		}

        private void ZoomOutButton_Click(object sender, EventArgs e)
        {
            MainChart.ZoomOut();
            MainChart.Focus();
        }
        private void ZoomInButton_Click(object sender, EventArgs e)
        {
            MainChart.ZoomIn();
            MainChart.Focus();
        }

        private void ZoomToContentButton_Click(object sender, EventArgs e)
        {
			this.ZoomToData();
		}

 		public void ZoomToData() {
            //        IList<float[]> regions;
            //MainChart.DataSeries[1].GetSelectedRegions(out regions);
            //        if(regions.Count>0))
            MainChart.Refresh();
            MainChart.AutozoomToData(true);
		}

        void copyChartMenuItem_Click(object sender, EventArgs e)
        {
            //Not visible menu item
            //MainChart.CopyTextToClipboard(true, System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator);
        }

        void MainChart_SelectingData(object sender, ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataEventArgs e)
        {
            //Note: MainChart_SelectData fires before and after this event
        }

        void MainChart_SelectData(object sender, ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataEventArgs e)
        {
            if (e != null && e.DataSeries != null && m_page != null)
            {
                //Reset color on axis
                foreach (LineChartTypes chartType in m_ChartTypes)
                {
                    LineChartTypes axisType = ChartToAxis(chartType);
                    if (axisType == chartType || !m_ChartTypes.Contains(axisType))
                    {
                        m_axisCharts[axisType].LabelColor = ColorUtil.ChartColor[axisType].LineNormal;
                    }
                }
                e.DataSeries.ValueAxis.LabelColor = Color.Black;// e.DataSeries.SelectedColor;
                //Get index for dataseries - same as for result
                int seriesIndex = -1;
                for (int j = 0; j < MainChart.DataSeries.Count; j++)
                {
                    if (e.DataSeries.Equals(MainChart.DataSeries[j]))
                    {
                        seriesIndex = j;
                        break;
                    }
                }
                if (seriesIndex >= 0)
                {
                    //Results must be added in order, so they can be resolved to result here
                    TrailResult tr = m_trailResults[seriesIndex % this.m_trailResults.Count];

                    IList<TrailResult> markResults = new List<TrailResult>();
                    //Reuse ZoomToSelection setting, to select all results
                    if (Data.Settings.ZoomToSelection || tr is SummaryTrailResult)
                    {
                        markResults = new List<TrailResult>();
                        foreach (TrailResult tr2 in this.TrailResults)
                        {
                            markResults.Add(tr2);
                        }
                    }
                    else
                    {
                        markResults = new List<TrailResult> { tr };
                    }
                    IList<float[]> regions;
                    e.DataSeries.GetSelectedRegions(out regions);

                    IList<Data.TrailResultMarked> results = new List<Data.TrailResultMarked>();
                    foreach (TrailResult tr2 in markResults)
                    {
                        IValueRangeSeries<DateTime> t2 = GetResultRegions(tr2, regions);
                        results.Add(new Data.TrailResultMarked(tr2, t2));
                    }
                    this.MainChart.SelectData -= new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataHandler(MainChart_SelectData);
                    m_selectDataHandler = false;

                    bool markAll = (MainChart.DataSeries.Count <= MaxSelectedSeries);
                    //Mark route track, but not chart
                    m_page.MarkTrack(results, false);
                    m_page.EnsureVisible(new List<Data.TrailResult> { tr }, false);

                    //TODO: Should also zoom chart

                    if (markAll)
                    {
                        m_multiple.SetSelectedResultRange(regions);
                    }
                    else
                    {
                        //Assumes that not single results are set
                        m_multiple.SetSeriesSelectedResultRange(seriesIndex, regions);
                    }
                    this.MainChart.SelectData += new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataHandler(MainChart_SelectData);
                    m_selectDataHandler = true;
                }
            }
        }

        public void SetSelectedResultRange(IList<float[]> regions)
        {
            for (int i = 0; i < MainChart.DataSeries.Count; i++ )
            {
                MainChart.DataSeries[i].ClearSelectedRegions();
            }
        }

        //Mark a specific series
        public void SetSeriesSelectedResultRange(int i, bool clearAll, IList<float[]> regions)
        {
            if (ShowPage)
            {
                if (m_selectDataHandler)
                {
                    this.MainChart.SelectData -= new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataHandler(MainChart_SelectData);
                }
                if (clearAll)
                {
                    foreach (ChartDataSeries t in MainChart.DataSeries)
                    {
                        //Note: This is not clearing ranges
                        t.ClearSelectedRegions();
                    }
                }
                if (MainChart.DataSeries != null && MainChart.DataSeries.Count > i)
                {
                    if (!clearAll)
                    {
                        MainChart.DataSeries[i].ClearSelectedRegions();
                    }
                    if (regions != null && regions.Count > 0)
                    {
                        MainChart.DataSeries[i].SetSelectedRange(regions[0][0], regions[regions.Count - 1][1]);
                        MainChart.DataSeries[i].EnsureSelectedRangeVisible(); //Not working?
                    }
                }
                if (m_selectDataHandler)
                {
                    this.MainChart.SelectData += new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataHandler(MainChart_SelectData);
                }
            }
        }

        //No TrailResult - use all possible matches (if less than MaxSelectedSeries)
        //Set for primary chart only
        public void SetSelectedRange(IList<IItemTrackSelectionInfo> asel)
        {
            if (ShowPage && MainChart != null && MainChart.DataSeries != null &&
                    MainChart.DataSeries.Count > 0 &&
                m_trailResults.Count > 0)
            {
                //This is used in single activity mode, when selected on the route - all should be for the same activity
                Data.TrailsItemTrackSelectionInfo sel = new Data.TrailsItemTrackSelectionInfo();
                foreach (IItemTrackSelectionInfo trm in asel)
                {
                    sel.Union(trm);
                    if (trm is TrailsItemTrackSelectionInfo)
                    {
                        sel.Activity = (trm as TrailsItemTrackSelectionInfo).Activity;
                    }
                }

                //Set the matching time distance for the activity
                //TBD: Time/dist need the TrailResult related to the current results...
                //With single results, this can be done, but for multi results per activity this can be incorrect
                IList<float[]> l = null;
                IList<TrailResult> t = TrailResultWrapper.ParentResults(Controller.TrailController.Instance.CurrentResultTreeList);
                foreach(TrailResult tr in t)
                {
                    if (tr.Activity == sel.Activity)
                    {
                        l = GetResultSelectionFromActivity(tr, sel);
                        if (l != null && l.Count > 0)
                        {
                            break;
                        }
                    }
                }
                //update the result
                if (l != null && l.Count > 0)
                {
                    for (int i = 0; i < m_trailResults.Count; i++)
                    {
                        if (m_trailResults[i].Activity == sel.Activity || 
                            m_trailResults.Count < MaxSelectedSeries)
                        {
                            MainChart.DataSeries[i].ClearSelectedRegions();
                            //The result is for the main result. Instead of calculating GetResultSelectionFromActivity() for each subsplit, find the offset
                            float offset = 0;
                            if (m_trailResults[i] is ChildTrailResult)
                            {
                                TrailResult tr = (m_trailResults[i] as ChildTrailResult).ParentResult;
                                if (XAxisReferential == XAxisValue.Time)
                                {
                                    offset = (float)(m_trailResults[i].StartTime - tr.StartTime).TotalSeconds;
                                }
                                else
                                {
                                    offset = (float)TrackUtil.DistanceConvertFrom(m_trailResults[i].StartDist - tr.StartDist, ReferenceTrailResult);
                                }
                            }

                            //Only one range can be selected - select all
                            float x1 = l[0][0] - offset;
                            float x2 = l[l.Count - 1][1] - offset;

                            //Ignore ranges outside current range and malformed scales
                            if (x1 < MainChart.XAxis.MaxOriginFarValue &&
                                MainChart.XAxis.MinOriginValue > float.MinValue &&
                                x2 > MainChart.XAxis.MinOriginValue &&
                                MainChart.XAxis.MaxOriginFarValue < float.MaxValue)
                            {
                                x1 = Math.Max(x1, (float)MainChart.XAxis.MinOriginValue);
                                x2 = Math.Min(x2, (float)MainChart.XAxis.MaxOriginFarValue);

                                MainChart.DataSeries[i].SetSelectedRange(x1, x2);
                            }
                        }
                    }
                }
            }
        }

        //only mark in chart, no range/summary
        public void SetSelectedRegions(IList<TrailResultMarked> atr)
        {
            if (ShowPage && MainChart != null && MainChart.DataSeries != null &&
                    MainChart.DataSeries.Count > 0 &&
                m_trailResults.Count > 0)
            {
                foreach(ChartDataSeries c in MainChart.DataSeries)
                {
                    c.ClearSelectedRegions();
                }
                foreach (TrailResultMarked trm in atr)
                {
                    //Set the matching time distance for the activity
                    for (int i = 0; i < m_trailResults.Count; i++)
                    {
                        TrailResult tr = m_trailResults[i];
                        if (trm.trailResult == tr)
                        {
                            IList<float[]> t = GetResultSelectionFromActivity(tr, trm.selInfo);
                            foreach (float[] ax in t)
                            {
                                //Ignore ranges outside current range and malformed scales
                                if (ax[0] < MainChart.XAxis.MaxOriginFarValue &&
                                    MainChart.XAxis.MinOriginValue > float.MinValue &&
                                    ax[1] > MainChart.XAxis.MinOriginValue &&
                                    MainChart.XAxis.MaxOriginFarValue < float.MaxValue)
                                {
                                    ax[0] = Math.Max(ax[0], (float)MainChart.XAxis.MinOriginValue);
                                    ax[1] = Math.Min(ax[1], (float)MainChart.XAxis.MaxOriginFarValue);

                                    for (int j = i; j < MainChart.DataSeries.Count; j += m_trailResults.Count)
                                    {
                                        MainChart.DataSeries[j].AddSelecedRegion(ax[0], ax[1]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        IValueRangeSeries<DateTime> GetResultRegions(TrailResult tr, IList<float[]> regions)
        {
            IValueRangeSeries<DateTime> t = new ValueRangeSeries<DateTime>();
            foreach (float[] at in regions)
            {
                DateTime d1;
                DateTime d2;
                if (XAxisReferential == XAxisValue.Time)
                {
                    d1 = tr.getDateTimeFromTimeResult(GetResyncOffsetTime(tr, at[0]));
                    d2 = tr.getDateTimeFromTimeResult(GetResyncOffsetTime(tr, at[1]));
                }
                else
                {

                    d1 = tr.getDateTimeFromDistResult(TrackUtil.DistanceConvertTo(GetResyncOffsetDist(tr, at[0]), ReferenceTrailResult));
                    d2 = tr.getDateTimeFromDistResult(TrackUtil.DistanceConvertTo(GetResyncOffsetDist(tr, at[1]), ReferenceTrailResult));
                }
                t.Add(new ValueRange<DateTime>(d1, d2));
            }
            return t;
        }

        float[] GetSingleSelection(TrailResult tr, IValueRange<DateTime> v)
        {
            DateTime d1 = v.Lower;
            DateTime d2 = v.Upper;
            if (XAxisReferential == XAxisValue.Time)
            {
                return GetSingleSelectionFromResult(tr, d1, d2);
            }
            else
            {
                double t1 = tr.getDistResult(d1);
                double t2 = tr.getDistResult(d2);
                return GetSingleSelectionFromResult(tr, t1, t2);
            }
        }
        float[] GetSingleSelection(TrailResult tr, IValueRange<double> v)
        {
            //Note: Selecting in Route gives unpaused distance, but this should be handled in the selection
            if (XAxisReferential == XAxisValue.Time)
            {
                DateTime d1 = DateTime.MinValue, d2 = DateTime.MinValue;
                d1 = tr.getDateTimeFromDistActivity(v.Lower);
                d2 = tr.getDateTimeFromDistActivity(v.Upper);
                return GetSingleSelectionFromResult(tr, d1, d2);
            }
            else
            {
                double t1 = tr.getDistResultFromDistActivity(v.Lower);
                double t2 = tr.getDistResultFromDistActivity(v.Upper);
                return GetSingleSelectionFromResult(tr, t1, t2);
            }
        }

        float[] GetSingleSelectionFromResult(TrailResult tr, DateTime d1, DateTime d2)
        {
            float x1 = float.MaxValue, x2 = float.MinValue;
            //Convert to distance display unit, Time is always in seconds
            x1 = (float)(tr.getTimeResult(d1));
            x2 = (float)(tr.getTimeResult(d2));
            return new float[] { x1, x2 };
        }

        float[] GetSingleSelectionFromResult(TrailResult tr, double t1, double t2)
        {
            float x1 = float.MaxValue, x2 = float.MinValue;
            //distance is for result, then to display units
            x1 = (float)TrackUtil.DistanceConvertFrom(t1, ReferenceTrailResult);
            x2 = (float)TrackUtil.DistanceConvertFrom(t2, ReferenceTrailResult);
            return new float[] { x1, x2 };
        }

        private IList<float[]> GetResultSelectionFromActivity(TrailResult tr, IItemTrackSelectionInfo sel)
        {
            IList<float[]> result = new List<float[]>();

            //Currently only one range but several regions in the chart can be selected
            //Only use one of the selections
            if (sel.MarkedTimes != null)
            {
                foreach (IValueRange<DateTime> v in sel.MarkedTimes)
                {
                    result.Add(GetSingleSelection(tr, v));
                }
            }
            else if (sel.MarkedDistances != null)
            {
                foreach (IValueRange<double> v in sel.MarkedDistances)
                {
                    result.Add(GetSingleSelection(tr, v));
                }
            }
            else if (sel.SelectedTime != null)
            {
                result.Add(GetSingleSelection(tr, sel.SelectedTime));
            }
            else if (sel.SelectedDistance != null)
            {
                result.Add(GetSingleSelection(tr, sel.SelectedDistance));
            }
            return result;
        }

        //Could use TrailResultMarked, but a selection of the track cannot be marked in multi mode
        public void EnsureVisible(IList<TrailResult> atr)
        {
            if (ShowPage)
            {
                foreach (TrailResult tr in atr)
                {
                    for (int i = 0; i < MainChart.DataSeries.Count; i++)
                    {
                        MainChart.DataSeries[i].ClearSelectedRegions();
                        int resIndex = i % m_trailResults.Count;
                        if (m_trailResults[resIndex].Equals(tr))
                        {
                            MainChart.DataSeries[i].AddSelecedRegion(
                                MainChart.DataSeries[i].XMin, MainChart.DataSeries[i].XMax);
                        }
                    }
                }
            }
        }

        //Find if the chart has any data
        public bool AnyData()
        {
            return m_axisCharts != null && m_axisCharts.Count>0;
        }

        public bool HasValues(LineChartTypes chartType)
        {
            if (m_hasValues == null)
            {
                m_hasValues = new Dictionary<LineChartTypes, bool>();
            }
            if(!m_hasValues.ContainsKey(chartType))
            {
                m_hasValues.Add(chartType, false);
                //Previous check when a diff to itself is not a value - enable replacing
                //if (!(m_trailResults == null || m_refTrailResult == null ||
                //    (yaxis == LineChartTypes.DiffTime || yaxis == LineChartTypes.DiffDist) &&
                //    m_trailResults.Count == 1 && m_trailResults[0] == m_refTrailResult))
                {
                    for (int i = 0; i < this.TrailResults.Count; i++)
                    {
                        TrailResult tr = this.TrailResults[i];
                        //The track is mostly cached in result, it is not much extra to request and drop it
                        INumericTimeDataSeries graphPoints = GetSmoothedActivityTrack(tr, chartType, ReferenceTrailResult);

                        if (graphPoints != null && graphPoints.Count > 1 || 
                            //TODO: check data for summary too
                            tr is SummaryTrailResult)
                        {
                            m_hasValues[chartType] = true;
                            break;
                        }
                    }
                }
            }
            return m_hasValues[chartType];
        }

        virtual protected void SetupDataSeries()
        {
			MainChart.DataSeries.Clear();
            MainChart.XAxis.Markers.Clear();
            if (m_visible)
            {
                IList<TrailResult> chartResults = new List<TrailResult>();
                foreach (TrailResult tr in m_trailResults)
                {
                    chartResults.Add(tr);
                }

                //Special handling for summary, needs graphs for all results
                bool summarySpecialColor = false;
                SummaryTrailResult summaryResult = null;
                foreach (TrailResult tr in m_trailResults)
                {
                    if (tr is SummaryTrailResult)
                    {
                        summaryResult = (tr as SummaryTrailResult);
                        if (m_trailResults.Count == 2)
                        {
                            summarySpecialColor = true;
                        }
                        foreach (TrailResult tr2 in summaryResult.Results)
                        {
                            if (!m_trailResults.Contains(tr2))
                            {
                                chartResults.Add(tr2);
                            }
                        }
                        break;
                    }
                }

                //Find if ReferenceTrailResult is in the results - needed when displaying data
                TrailResult leftRefTr = null;
                if (m_trailResults.Count > 0)
                {
                    leftRefTr = m_trailResults[0];
                    for (int i = 0; i < m_trailResults.Count; i++)
                    {
                        if (m_trailResults[i] == ReferenceTrailResult)
                        {
                            leftRefTr = ReferenceTrailResult;
                            break;
                        }
                    }
                }

                float syncGraphOffsetSum = 0;
                int syncGraphOffsetCount = 0;
                LineChartTypes syncGraphOffsetChartType = LineChartTypes.Speed;
                if (m_ChartTypes.Count > 0)
                {
                    syncGraphOffsetChartType = m_ChartTypes[0];
                    if (syncGraphOffsetChartType != ChartToAxis(syncGraphOffsetChartType) && m_ChartTypes.Contains(ChartToAxis(syncGraphOffsetChartType)))
                    {
                        syncGraphOffsetChartType = ChartToAxis(syncGraphOffsetChartType);
                    }
                }
                //Note: If the add order changes, the dataseries to result lookup in MainChart_SelectData is affected too
                foreach (LineChartTypes chartType in m_ChartTypes)
                {
                    ChartDataSeries summaryDataLine = null;
                    IList<ChartDataSeries> summarySeries = new List<ChartDataSeries>();
                    INumericTimeDataSeries refGraphPoints = null;
                    LineChartTypes refChartType = chartType;

                    if(syncGraph != SyncGraphMode.None )
                    {
                        if (chartType != ChartToAxis(chartType) && m_ChartTypes.Contains(ChartToAxis(chartType)))
                        {
                            refChartType = ChartToAxis(chartType);
                        }
                        if (refChartType == syncGraphOffsetChartType)
                        {
                            refGraphPoints = GetSmoothedActivityTrack(ReferenceTrailResult, refChartType, ReferenceTrailResult);
                        }
                    }

                    for (int i = 0; i < chartResults.Count; i++)
                    {
                        TrailResult tr = chartResults[i];

                        ChartColors chartColor;
                        //Color for the graph - keep standard color if only one result displayed
                        if (m_trailResults.Count <= 1 || summarySpecialColor ||
                            Data.Settings.OnlyReferenceRight && (m_axisCharts[chartType] is RightVerticalAxis))
                        {
                            chartColor = ColorUtil.ChartColor[chartType];
                        }
                        else
                        {
                            chartColor = tr.ResultColor;
                        }

                        //Add empty Dataseries even if no graphpoints. index must match results
                        ChartDataSeries dataLine = new ChartDataSeries(MainChart, m_axisCharts[chartType]);
                        dataLine.ChartType = ChartDataSeries.Type.Line;
                        dataLine.ValueAxisLabel = ChartDataSeries.ValueAxisLabelType.Average;
                        dataLine.LineColor = chartColor.LineNormal;
                        dataLine.FillColor = chartColor.FillNormal;
                        dataLine.SelectedColor = chartColor.FillSelected;
                        //Decrease visibility for "secondary" results
                        if (i != 0)
                        {
                            dataLine.FillColor = Color.FromArgb(dataLine.FillColor.ToArgb() - 20 * 0x1000000);
                            dataLine.SelectedColor = Color.FromArgb(dataLine.SelectedColor.ToArgb() - 20 * 0x1000000);
                        }

                        //Set chart type to Fill similar to ST for first result
                        if (m_ChartTypes[0] == chartType)
                        {
                            dataLine.ChartType = ChartDataSeries.Type.Fill;
                        }

                        //Add to the chart only if result is visible (no "summary" results)
                        if (m_trailResults.Contains(tr))
                        {
                            MainChart.DataSeries.Add(dataLine);
                        }

                        if (tr is SummaryTrailResult)
                        {
                            summaryDataLine = dataLine;
                            if (m_trailResults.Count > 1)
                            {
                                dataLine.LineWidth *= 2;
                            }
                        }
                        else
                        {
                            INumericTimeDataSeries graphPoints;

                            //Hide right column graph in some situations
                            //Note that the results may be needed if only ref right also should show average...
                            if ((1 >= m_trailResults.Count ||
                                !Data.Settings.OnlyReferenceRight ||
                                !(m_axisCharts[chartType] is RightVerticalAxis) ||
                                tr == leftRefTr))
                            {
                                TrailResult refTr = ReferenceTrailResult;
                                if (refIsSelf || null == ReferenceTrailResult)
                                {
                                    refTr = tr;
                                }
                                graphPoints = GetSmoothedActivityTrack(tr, chartType, refTr);
                            }
                            else
                            {
                                //No data
                                graphPoints = new NumericTimeDataSeries();
                            }

                            if (graphPoints.Count > 1)
                            {
                                //Get the actual graph for all displayed
                                float syncGraphOffset = GetDataLine(tr, graphPoints, dataLine, refGraphPoints);
                                if (refChartType == ChartToAxis(chartType) && (refChartType != chartType || ReferenceTrailResult != tr))
                                {
                                    syncGraphOffsetSum += syncGraphOffset;
                                    syncGraphOffsetCount++;
                                }

                                //Add as graph for summary
                                if (dataLine.Points.Count > 1 && summaryResult != null &&
                                    summaryResult.Results.Contains(tr)//&&
                                    //Ignore ref for diff time/dist graphs
                                    //(pair.Key != LineChartTypes.DiffDist || pair.Key != LineChartTypes.DiffTime ||
                                        )
                                {
                                    summarySeries.Add(dataLine);
                                }
                            }
                        }
                    }
                    ////All results for this axis
                    //Create list summary from resulting datalines
                    if (summaryDataLine != null)
                    {
                        if (summarySeries.Count > 1)
                        {
                            //Only add if more than one one result
                            this.getCategoryAverage(summaryDataLine, summarySeries);
                        }
                    }
                }  //for all axis

                if (syncGraph != SyncGraphMode.None && syncGraphOffsetCount > 0)
                {
                    summaryListToolTip.Show(syncGraph.ToString() + ": " + syncGraphOffsetSum / syncGraphOffsetCount, this, //TODO: Translate
                      new System.Drawing.Point(10 + Cursor.Current.Size.Width / 2, 10),
                      summaryListToolTip.AutoPopDelay);
                }

                ///////TrailPoints
                Data.TrailResult trailPointResult = ReferenceTrailResult;
                //If only one result is used, it can be confusing if the trail points are set for ref
                if ((!Data.Settings.SyncChartAtTrailPoints && m_trailResults.Count == 1 ||
                    m_trailResults.Count > 0 && trailPointResult == null) &&
                    !(m_trailResults[0] is SummaryTrailResult))
                {
                    trailPointResult = m_trailResults[0];
                }

                if (m_showTrailPoints && trailPointResult != null)
                {
                    Image icon =
#if ST_2_1
                        CommonResources.Images.Information16;
#else
                        new Bitmap(TrailsPlugin.CommonIcons.fileCircle(11, 11, trailPointResult.ResultColor.LineNormal));
#endif
                    double oldElapsed = double.MinValue;
                    foreach (DateTime t in trailPointResult.TrailPointDateTime)
                    {
                        double elapsed;
                        if (XAxisReferential == XAxisValue.Time)
                        {
                            elapsed = trailPointResult.getTimeResult(t);
                        }
                        else
                        {
                            elapsed = TrackUtil.DistanceConvertFrom(
                                trailPointResult.getDistResult(t), trailPointResult);
                        }
                        if (!double.IsNaN(elapsed) && elapsed > oldElapsed)
                        {
                            AxisMarker a = new AxisMarker(elapsed, icon);
                                                    a.Line1Style = System.Drawing.Drawing2D.DashStyle.Solid;
                            a.Line1Color = Color.Goldenrod;
                            MainChart.XAxis.Markers.Add(a);
                        }
                    }
                }
                ZoomToData();
            }
		}

        internal static float FixedSyncGraphMode = 0;//Fix....
        private float GetDataLine(TrailResult tr, INumericTimeDataSeries graphPoints, 
            ChartDataSeries dataLine, INumericTimeDataSeries refGraphPoints)
        {
            INumericTimeDataSeries dataPoints;
            if (XAxisReferential == XAxisValue.Time)
            {
                dataPoints = graphPoints;
            }
            else
            {
                dataPoints = tr.DistanceMetersTrack0(ReferenceTrailResult);
            }
            float syncGraphOffset = 0;
            if (graphPoints != refGraphPoints &&
                refGraphPoints != null && refGraphPoints.Count > 1)
            {
                switch (syncGraph)
                {
                    case SyncGraphMode.None:
                        break;
                    case SyncGraphMode.Start:
                        syncGraphOffset = refGraphPoints[0].Value - graphPoints[0].Value;
                        break;
                    case SyncGraphMode.End:
                        syncGraphOffset = refGraphPoints[refGraphPoints.Count - 1].Value - graphPoints[graphPoints.Count-1].Value;
                        break;
                    case SyncGraphMode.Average:
                        syncGraphOffset = refGraphPoints.Avg - graphPoints.Avg;
                        break;
                    case SyncGraphMode.Min:
                        syncGraphOffset = refGraphPoints.Min - graphPoints.Min;
                        break;
                    case SyncGraphMode.Max:
                        syncGraphOffset = refGraphPoints.Max - graphPoints.Max;
                        break;
                    default:
                        {
                            Debug.Assert(false);
                            break;
                        }
                }
                if(float.IsNaN(syncGraphOffset) || float.IsInfinity(syncGraphOffset))
                {
                    syncGraphOffset = 0;
                }
            }
            TrailLineChart.FixedSyncGraphMode = syncGraphOffset;

            int oldElapsedEntry = int.MinValue;
            float oldXvalue = float.MinValue;
            foreach (ITimeValueEntry<float> entry in dataPoints)
            {
                uint elapsedEntry = entry.ElapsedSeconds;
                if (XAxisReferential == XAxisValue.Time || elapsedEntry <= graphPoints.TotalElapsedSeconds)
                {
                    //The time is required to get the xvalue(time) or yvalue(dist)
                    DateTime time = dataPoints.EntryDateTime(entry);
                    //The x value in the graph, the actual time or distance
                    float xValue;
                    if (XAxisReferential == XAxisValue.Time)
                    {
                        xValue = (float)tr.getTimeResult(time);
                    }
                    else
                    {
                        xValue = entry.Value;
                    }
                    //With "resync at Trail Points", the elapsed is adjusted to the reference at trail points
                    //So at the end of each "subtrail", the track can be extended (elapsed jumps) 
                    //or cut (elapsed is higher than next limit, then decreases at trail point)  
                    float nextXvalue = float.MaxValue;
                    if (Data.Settings.SyncChartAtTrailPoints)
                    {
                        xValue += GetResyncOffset(XAxisReferential, false, tr, xValue, out nextXvalue);
                    }
                    if (oldElapsedEntry < elapsedEntry &&
                        (!Data.Settings.SyncChartAtTrailPoints ||
                        oldXvalue < xValue && xValue < nextXvalue))
                    {
                        ITimeValueEntry<float> yValueEntry;
                        if (XAxisReferential == XAxisValue.Time)
                        {
                            yValueEntry = entry;
                        }
                        else
                        {
                            yValueEntry = graphPoints.GetInterpolatedValue(time);
                        }
                        //Infinity values gives garbled graphs
                        if (yValueEntry != null && !float.IsInfinity(yValueEntry.Value))
                        {
                            PointF point = new PointF(xValue, yValueEntry.Value + syncGraphOffset);
                            dataLine.Points.Add(elapsedEntry, point);
                        }
                        oldElapsedEntry = (int)elapsedEntry;
                        oldXvalue = xValue;
                    }
                }
            }
            return syncGraphOffset;
        }

        //From Overlay plugin
        private ChartDataSeries getCategoryAverage(ChartDataSeries average,
                  IList<ChartDataSeries> list)
        {
            SortedList<float, bool> xs = new SortedList<float, bool>();
            foreach (ChartDataSeries series in list)
            {
                //Average graph is very slow with many points, limit them somehow
                //A reasonable value is close to the averaging time
                float xref = 15;
                if (XAxisReferential != XAxisValue.Time)
                {
                    //In distance mode, use points corresponding to time intervall at 5min/km
                    xref = (float)UnitUtil.Distance.ConvertFrom(xref*1000.0/300.0);
                }
                foreach (PointF point in series.Points.Values)
                {
                    float x = (float)(Math.Round(point.X / xref) * xref);
                    if (!xs.ContainsKey(x))
                    {
                        xs.Add(x, true);
                    }
                }
            }
            foreach (float x in xs.Keys)
            {
                int seen = 0;
                float y = 0;
                foreach (ChartDataSeries series in list)
                {
                    float theX = x;
                    float theY = series.GetYValueAtX(ref theX);
                    if (!theY.Equals(float.NaN))
                    {
                        y += theY;
                        seen++;
                    }
                }
                if (seen > 1 &&
                    average.Points.IndexOfKey(x) == -1)
                {
                    average.Points.Add(x, new PointF(x, y / seen));
                }
            }
            return average;
        }

        //From a value in the chart, get "real" elapsed
        //TODO: incorrect around trail points
        private float GetResyncOffsetTime(TrailResult tr, float elapsed)
        {
            float nextElapsed;
            int currOffsetIndex = 0;
            return elapsed - GetResyncOffset(XAxisValue.Time, true, tr, elapsed, out nextElapsed, ref currOffsetIndex);
        }
        private float GetResyncOffsetDist(TrailResult tr, float elapsed)
        {
            float nextElapsed;
            int currOffsetIndex = 0;
            return elapsed - GetResyncOffset(XAxisValue.Distance, true, tr, elapsed, out nextElapsed, ref currOffsetIndex);
        }

        //private float GetResyncOffset(XAxisValue XAxisReferential, bool elapsedIsRef, TrailResult tr, float elapsed)
        //{
        //    float nextElapsed;
        //    float startOffset;
        //    int currOffsetIndex = 0;
        //    return GetResyncOffset(XAxisReferential, false, tr, elapsed, out nextElapsed, out startOffset, ref currOffsetIndex);
        //}
        private float GetResyncOffset(XAxisValue XAxisReferential, bool elapsedIsRef, TrailResult tr, float elapsed, out float nextElapsed)
        {
            //Possibility to cache the index
            int currOffsetIndex = 0;
            return GetResyncOffset(XAxisReferential, elapsedIsRef, tr, elapsed, out nextElapsed, ref currOffsetIndex);
        }
        private float GetResyncOffset(XAxisValue XAxisReferential, bool elapsedIsRef, TrailResult tr, float elapsed, out float nextElapsed, ref int currOffsetIndex)
        {
            IList<double> trElapsed;
            IList<double> trOffset;
            IList<double> refElapsed;
            float offset = 0;
            nextElapsed = float.MaxValue;
            if (Data.Settings.SyncChartAtTrailPoints)
            {
                if (XAxisReferential == XAxisValue.Time)
                {
                    trElapsed = tr.TrailPointTime0(ReferenceTrailResult);
                    trOffset = tr.TrailPointTimeOffset0(ReferenceTrailResult);
                    refElapsed = ReferenceTrailResult.TrailPointTime0(ReferenceTrailResult);
                }
                else
                {
                    trElapsed = tr.TrailPointDist0(ReferenceTrailResult);
                    trOffset = tr.TrailPointDistOffset01(ReferenceTrailResult);
                    refElapsed = ReferenceTrailResult.TrailPointDist0(ReferenceTrailResult);
                }

                if (trElapsed.Count == refElapsed.Count)
                {
                    if (elapsedIsRef)
                    {
                        while (currOffsetIndex < refElapsed.Count - 1 && elapsed >= refElapsed[currOffsetIndex + 1])
                        {
                            currOffsetIndex++;
                        }
                        if (currOffsetIndex < trElapsed.Count - 1)
                        {
                            nextElapsed = (float)trElapsed[currOffsetIndex + 1];
                        }
                    }
                    else
                    {
                        while (currOffsetIndex < trElapsed.Count - 1 &&
                            //compare must be using same type here to avoid end effects
                            elapsed > (float)trElapsed[currOffsetIndex + 1])
                        {
                            currOffsetIndex++;
                        }
                        if (currOffsetIndex < refElapsed.Count - 1)
                        {
                            nextElapsed = (float)refElapsed[currOffsetIndex + 1];
                        }
                    }
                    if (currOffsetIndex > 1)
                    {
                    }
                    float startOffset;
                    if (currOffsetIndex < trOffset.Count)
                    {
                        startOffset = (float)trOffset[currOffsetIndex];
                    }
                    else
                    {
                        startOffset = 0;
                    }
                    if (currOffsetIndex < refElapsed.Count)
                    {
                        offset = (float)((refElapsed[currOffsetIndex] - trElapsed[currOffsetIndex]) + startOffset);
                    }
                }
            }
            return offset;
        }

        /*********************************************/
        private void SetupAxes()
        {
            if (m_visible && ReferenceTrailResult != null)
            {
                // X axis
                LineChartUtil.SetupXAxisFormatter(XAxisReferential, MainChart.XAxis, ReferenceTrailResult.Activity);

                // Y axis
                MainChart.YAxisRight.Clear();
                foreach (LineChartTypes chartType in m_ChartTypes)
                {
                    CreateAxis(chartType, m_axisCharts.Count == 0);
                }
            }
        }

        private void CreateAxis(LineChartTypes chartType, bool left)
        {
            if ((m_trailResults == null || ReferenceTrailResult == null ||
                (chartType == LineChartTypes.DiffTime || chartType == LineChartTypes.DiffDist) &&
                m_trailResults.Count == 1 && m_trailResults == ReferenceTrailResult))
            {
                return;
            }
            LineChartTypes axisType = ChartToAxis(chartType);
            if (!m_axisCharts.ContainsKey(axisType))
            {
                IAxis axis;
                if (m_axisCharts.Count == 0)
                {
                    axis = MainChart.YAxis;
                }
                else
                {
                    axis = new RightVerticalAxis(MainChart);
                    MainChart.YAxisRight.Add(axis);
                }
                LineChartUtil.SetupYAxisFormatter(axisType, axis, ReferenceTrailResult.Activity);
                m_axisCharts.Add(axisType, axis);
            }
            if (!m_axisCharts.ContainsKey(chartType))
            {
                m_axisCharts.Add(chartType, m_axisCharts[axisType]);
            }
        }

        /// <summary>
        /// Some chartTypes share axis. Could be changeable in the chart
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static LineChartTypes ChartToAxis(LineChartTypes chart)
        {
            LineChartTypes axis = chart;

            if (chart == LineChartTypes.DeviceSpeed)
            {
                axis = LineChartTypes.Speed;
            }
            else if (chart == LineChartTypes.DevicePace)
            {
                axis = LineChartTypes.Pace;
            }
            else if (chart == LineChartTypes.DeviceElevation)
            {
                axis = LineChartTypes.Elevation;
            }
            else if (chart == LineChartTypes.DeviceDiffDist)
            {
                axis = LineChartTypes.DiffDist;
            }

            return axis;
        }

        private static INumericTimeDataSeries GetSmoothedActivityTrack(Data.TrailResult result, LineChartTypes lineChart, TrailResult refRes)
        {
			// Fail safe
			INumericTimeDataSeries track = new NumericTimeDataSeries();

            switch (lineChart)
            {
                case LineChartTypes.Cadence:
                    {
                        track = result.CadencePerMinuteTrack0(refRes);
                        break;
                    }
                case LineChartTypes.Elevation:
                    {
                        track = result.ElevationMetersTrack0(refRes);
                        break;
                    }
                case LineChartTypes.HeartRateBPM:
                    {
                        track = result.HeartRatePerMinuteTrack0(refRes);
                        break;
                    }
                //case LineChartTypes.HeartRatePercentMax:
                //    {
                //        track = result.HeartRatePerMinutePercentMaxTrack;
                //        break;
                //    }
                case LineChartTypes.Power:
                    {
                        track = result.PowerWattsTrack0(refRes);
                        break;
                    }
                case LineChartTypes.Grade:
                    {
                        track = result.GradeTrack0(refRes);
                        break;
                    }

                case LineChartTypes.Speed:
                    {
                        track = result.SpeedTrack0(refRes);
                        break;
                    }

                case LineChartTypes.Pace:
                    {
                        track = result.PaceTrack0(refRes);
                        break;
                    }

                case LineChartTypes.DiffTime:
                    {
                        track = result.DiffTimeTrack0(refRes);
                        break;
                    }
                case LineChartTypes.DiffDist:
                    {
                        track = result.DiffDistTrack0(refRes);
                        break;
                    }
                case LineChartTypes.DeviceSpeed:
                case LineChartTypes.DevicePace:
                    {
                        track = result.DeviceSpeedPaceTrack0(refRes);
                        break;
                    }
                case LineChartTypes.DeviceElevation:
                    {
                        track = result.DeviceElevationTrack0(refRes);
                        break;
                    }
                case LineChartTypes.DeviceDiffDist:
                    {
                        track = result.DeviceDiffDistTrack0(refRes);
                        break;
                    }

                default:
                    {
                        Debug.Assert(false);
                        break;
                    }
            }
			return track;
		}

		[DisplayName("X Axis value")]
		public XAxisValue XAxisReferential {
			get { return m_XAxisReferential; }
			set {
				m_XAxisReferential = value;
			}
		}

        [DisplayName("Y Axis value")]
        public LineChartTypes LeftChartType
        {
            get
            {
                if (m_ChartTypes == null || m_ChartTypes.Count == 0)
                {
                    return LineChartTypes.Unknown;
                }
                return m_ChartTypes[0];
            }
            set
            {
                ChartTypes = new List<LineChartTypes> { value };
            }
        }
        public IList<LineChartTypes> ChartTypes
        {
            get
            {
                return m_ChartTypes;
            }
            set
            {
                m_ChartTypes = value;
                //Clear list of axis
                m_axisCharts = new Dictionary<LineChartTypes, IAxis>();
            }
        }

        public bool MultipleCharts
        {
            get
            {
                return m_multipleCharts;
            }
            set
            {
                m_multipleCharts = value;
            }
        }

		[Browsable(false)]
        public Data.TrailResult ReferenceTrailResult
        {
            get
            {
                return m_refTrailResult;
            }
            set
            {
                if (m_refTrailResult != value)
                {
                    m_refTrailResult = value;
                    SetupAxes();
                    SetupDataSeries();
                }
            }
        }

        public IList<Data.TrailResult> TrailResults
        {
            get
            {
                return m_trailResults;
            }
            set
            {
                if (m_trailResults != value)
                {
                    m_hasValues = null;
                    if (value == null)
                    {
                        m_trailResults.Clear();
                    }
                    else
                    {
                        m_trailResults = value;
                    }
                    SetupAxes();
                    SetupDataSeries();
                }
            }
        }

        public bool ShowChartToolBar
        {
            set
            {
                   this.chartTablePanel.RowStyles[0].Height = value ? 25 : 0;
            }
        }

        void MainChart_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            bool smoothChanged = false;
            bool increase = true;
            bool reset = false;
            bool zero = false;
            bool refreshData = false;
            bool clearRefreshData = true;

            if (e.KeyCode == Keys.Home)
            {
                smoothChanged = true;
                reset = true;
            }
            else if (e.KeyCode == Keys.End)
            {
                smoothChanged = true;
                zero = true;
            }
            else if (e.Modifiers == Keys.Shift)
            {
                increase = false;
            }

            if (e.KeyCode == Keys.PageDown || e.KeyCode == Keys.PageUp)
            {
                smoothChanged = true;
                if (e.KeyCode == Keys.PageDown)
                {
                    increase = false;
                }
                if (!m_ChartTypes.Contains(m_lastSelectedType))
                {
                    foreach (LineChartTypes chartType in m_ChartTypes)
                    {
                        if (m_axisCharts[chartType] is LeftVerticalAxis)
                        {
                            m_lastSelectedType = chartType;
                            break;
                        }
                    }
                }
            }
            else if (e.KeyCode == Keys.A)
            {
                refreshData = true;
                if (e.Modifiers == Keys.Control)
                {
                    Data.Settings.SmoothOverTrailPointsToggle();
                }
                else
                {
                    if (e.Modifiers == Keys.Shift)
                    {
                        if (syncGraph <= SyncGraphMode.None)
                        {
                            syncGraph = SyncGraphMode.Max;
                        }
                        else
                        {
                            syncGraph--;
                        }
                    }
                    else
                    {
                        if (syncGraph >= SyncGraphMode.Max)
                        {
                            syncGraph = SyncGraphMode.None;
                        }
                        else
                        {
                            syncGraph++;
                        }
                    }
                }
            }
            else if (e.KeyCode == Keys.C)
            {
                smoothChanged = true;
                m_lastSelectedType = LineChartTypes.Cadence;
            }
            else if (e.KeyCode == Keys.E)
            {
                smoothChanged = true;
                m_lastSelectedType = LineChartTypes.Elevation;
            }
            else if (e.KeyCode == Keys.H)
            {
                smoothChanged = true;
                m_lastSelectedType = LineChartTypes.HeartRateBPM;
            }
            else if (e.KeyCode == Keys.L)
            {
                refreshData = true;
                m_showTrailPoints = (e.Modifiers == Keys.Shift);
            }
            else if (e.KeyCode == Keys.P)
            {
                smoothChanged = true;
                m_lastSelectedType = LineChartTypes.Power;
            }
            else if (e.KeyCode == Keys.R)
            {
                refreshData = true;
                clearRefreshData = false;
                if (e.Modifiers == Keys.Shift)
                {
                    refIsSelf = !refIsSelf;
                }
                else if (e.Modifiers == Keys.Control)
                {
                    Data.Settings.OnlyReferenceRight = !Data.Settings.OnlyReferenceRight;
                }
            }
            else if (e.KeyCode == Keys.S)
            {
                smoothChanged = true;
                m_lastSelectedType = LineChartTypes.Speed;
            }
            else if (e.KeyCode == Keys.T)
            {
                refreshData = true;
                Data.Settings.SyncChartAtTrailPoints = (e.Modifiers != Keys.Shift);
            }
            IList<LineChartTypes> charts = new List<LineChartTypes>();
            if (smoothChanged)
            {
                refreshData = true; 
                charts = MainChart_KeyDown_Smooth(m_lastSelectedType, increase, reset, zero);
            }

            if (refreshData)
            {
                if (clearRefreshData)
                {
                    foreach (TrailResult t in TrailResults)
                    {
                        t.Clear(true);
                    }
                }
                m_page.RefreshControlState();
                m_page.RefreshChart();
            }

            if (smoothChanged)
            {
                //Show smooth value once, change more than one axis if needed
                ShowSmoothToolTip(m_lastSelectedType);

                foreach (KeyValuePair<LineChartTypes, IAxis> kp in m_axisCharts)
                {
                    if (charts.Contains(kp.Key))
                    {
                        kp.Value.LabelColor = Color.Black;
                    }
                    else
                    {
                        kp.Value.LabelColor = ColorUtil.ChartColor[kp.Key].LineNormal;
                    }
                }
            }
        }

        IList<LineChartTypes> MainChart_KeyDown_Smooth(LineChartTypes chartType, bool increase, bool reset, bool zero)
        {
            int val = GetDefaultSmooth(chartType);
            //No action for reset, value already set
            if (zero)
            {
                val = 0;
            }
            else if (!reset)
            {
                int add = 1;
                if (!increase)
                {
                    add = -add;
                }
                val += add;
                if (val < 0)
                {
                    val = 0;
                }
            }
            return SetDefaultSmooth(chartType, val);
        }

        //Combine Set&Get to minimize the case usage
        IList<LineChartTypes> SetDefaultSmooth(LineChartTypes selectedType, int val)
        {
            int tmp;
            return SetGetDefaultSmooth(selectedType, val, out tmp);
        }

        int GetDefaultSmooth(LineChartTypes selectedType)
        {
            int res;
            SetGetDefaultSmooth(selectedType, null, out res);
            return res;
        }

        private IList<LineChartTypes> SetGetDefaultSmooth(LineChartTypes selectedType, int? val, out int res)
        {
            IList<LineChartTypes> charts = new List<LineChartTypes> { selectedType };
            switch (selectedType)
            {
                case LineChartTypes.Cadence:
                    {
                        if (val != null)
                        {
                            TrailResult.TrailActivityInfoOptions.CadenceSmoothingSeconds = (int)val;
                        }
                        val = TrailResult.TrailActivityInfoOptions.CadenceSmoothingSeconds;
                        break;
                    }
                case LineChartTypes.Elevation:
                case LineChartTypes.DeviceElevation:
                case LineChartTypes.Grade:
                    {
                        foreach (LineChartTypes l in new List<LineChartTypes> { LineChartTypes.Elevation, LineChartTypes.DeviceElevation, LineChartTypes.Grade })
                        {
                            if (!charts.Contains(l))
                            {
                                charts.Add(l);
                            }
                        }
                        if (val != null)
                        {
                            TrailResult.TrailActivityInfoOptions.ElevationSmoothingSeconds = (int)val;
                        }
                        val = TrailResult.TrailActivityInfoOptions.ElevationSmoothingSeconds;
                        break;
                    }
                case LineChartTypes.HeartRateBPM:
                case LineChartTypes.DiffHeartRateBPM:
                    {
                        if (val != null)
                        {
                            TrailResult.TrailActivityInfoOptions.HeartRateSmoothingSeconds = (int)val;
                        }
                        val = TrailResult.TrailActivityInfoOptions.HeartRateSmoothingSeconds;
                        break;
                    }
                case LineChartTypes.Power:
                    {
                        if (val != null)
                        {
                            TrailResult.TrailActivityInfoOptions.PowerSmoothingSeconds = (int)val;
                        }
                        val = TrailResult.TrailActivityInfoOptions.PowerSmoothingSeconds;
                        break;
                    }
                case LineChartTypes.Speed:
                case LineChartTypes.Pace:
                case LineChartTypes.DeviceSpeed:
                case LineChartTypes.DevicePace:
                    {
                        foreach (LineChartTypes l in new List<LineChartTypes> { LineChartTypes.Speed, LineChartTypes.Pace, LineChartTypes.DeviceSpeed, LineChartTypes.DevicePace })
                        {
                            if (!charts.Contains(l))
                            {
                                charts.Add(l);
                            }
                        }
                        if (val != null)
                        {
                            TrailResult.TrailActivityInfoOptions.SpeedSmoothingSeconds = (int)val;
                        }
                        val = TrailResult.TrailActivityInfoOptions.SpeedSmoothingSeconds;
                        break;
                    }
                default:
                    {
                        val = 0;
                        break;
                    }
            }
            res = (int)val;
            return charts;
        }

        void ShowSmoothToolTip(LineChartTypes chartType)
        {
            if (summaryListCursorLocationAtMouseMove != null)
            {
                summaryListToolTip.Show(
                    GetDefaultSmooth(chartType).ToString(),
                    this,
                    new System.Drawing.Point(summaryListCursorLocationAtMouseMove.X +
                                  Cursor.Current.Size.Width / 2,
                                        summaryListCursorLocationAtMouseMove.Y),
                   summaryListToolTip.AutoPopDelay);
            }
        }

        private System.Drawing.Point summaryListCursorLocationAtMouseMove;
        void MainChart_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            summaryListCursorLocationAtMouseMove = e.Location;
        }

        void MainChart_SelectAxisLabel(object sender, ChartBase.AxisEventArgs e)
        {
            if (e.Axis is RightVerticalAxis || e.Axis is LeftVerticalAxis)
            {
                //Select all charts for this axix
                for (int i = 0; i < MainChart.DataSeries.Count; i++)
                {
                    //Clear all series, no line/fill check
                    MainChart.DataSeries[i].ClearSelectedRegions();
                    //For "single result" only select first series
                    if (MainChart.DataSeries[i].ValueAxis == e.Axis)
                    {
                        MainChart.DataSeries[i].AddSelecedRegion(
                            MainChart.DataSeries[i].XMin, MainChart.DataSeries[i].XMax);
                    }
                }

                foreach (LineChartTypes chartType in m_ChartTypes)
                {
                    if (m_axisCharts[chartType] == e.Axis)
                    {
                        //More than one chart could exist for the axis, only select the first
                        m_lastSelectedType = chartType;
                        ShowSmoothToolTip(chartType);
                        break;
                    }
                }
            }
        }

        public bool BeginUpdate()
        {
			return MainChart.BeginUpdate();
		}

		public void EndUpdate()
        {
			MainChart.EndUpdate();
		}
	}
}
