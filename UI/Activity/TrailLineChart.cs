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
        //private bool m_selectDataHandler = true; //Event handler is enabled by default
        private bool m_showTrailPoints = true;
        private bool refIsSelf = false;

        private bool m_CtrlPressed = false;
        private Point m_MouseDownLocation;
        private System.Drawing.Point m_cursorLocationAtMouseMove;
        internal static float FixedSyncGraphMode = 0;//Fix....

        //selecting in the chart
        private DateTime m_lastSelectingTime = DateTime.MinValue;
        private bool m_endSelect = false;
        private float[] m_selectedStartRange = null;
        private int m_selectedDataSetries = -1;
 
        const int MaxSelectedSeries = 6;
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

        public void SetControl(ActivityDetailPageControl page, MultiChartsControl multiple)
        {
            this.m_page = page;
            this.m_multiple = multiple;
        }

        public void ThemeChanged(ITheme visualTheme)
        {
            this.m_visualTheme = visualTheme;
            this.MainChart.ThemeChanged(visualTheme);
            this.ButtonPanel.ThemeChanged(visualTheme);
            this.ButtonPanel.BackColor = visualTheme.Window;
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
			//this.ZoomToData();
            MainChart.AutozoomToData(true);
            MainChart.Focus();
        }

 		public void ZoomToData()
        {
            MainChart.AutozoomToData(true);
            MainChart.Refresh();
            MainChart.Focus();
        }

        void copyChartMenuItem_Click(object sender, EventArgs e)
        {
            //Not visible menu item
            //MainChart.CopyTextToClipboard(true, System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator);
        }

        //Fires about every 33ms when selecting
        void MainChart_SelectingData(object sender, ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataEventArgs e)
        {
            //Let update rate depend on number of chart activities, less choppy update
            if (DateTime.Now.Subtract(this.m_lastSelectingTime).TotalMilliseconds >= 33*this.m_trailResults.Count)
            {
                MainChart_SelectingData(this.m_selectedDataSetries, null, true, false);
            }
        }

        //Fires before and after selection, also when just clicking
        void MainChart_SelectData(object sender, ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataEventArgs e)
        {
            float[] range = null;

            this.m_selectedDataSetries = -1;

            if (e != null && e.DataSeries != null)
            {
                range = new float[2];
                e.DataSeries.GetSelectedRange(out range[0], out range[1]);
                if (float.IsNaN(range[1]))
                {
                    //Sync, should not be needed
                    this.m_endSelect = false;
                }

                if (!this.m_endSelect)
                {
                    //Save start range, to shrink if changed
                    this.m_selectedStartRange = range;
                }
                else
                {
                    //Clear at end (should not be required)
                    this.m_selectedStartRange = null;
                }

                if (!this.m_endSelect)
                {
                    //Reset color on axis when start to select
                    foreach (LineChartTypes chartType in m_ChartTypes)
                    {
                        if (this.m_multipleCharts && e.DataSeries.ValueAxis == m_axisCharts[chartType])
                        {
                            e.DataSeries.ValueAxis.LabelColor = Color.Black;
                            this.m_lastSelectedType = chartType;
                        }
                        else
                        {
                            LineChartTypes axisType =  LineChartUtil.ChartToAxis(chartType);
                            if (axisType == chartType || !m_ChartTypes.Contains(axisType))
                            {
                                m_axisCharts[axisType].LabelColor = ColorUtil.ChartColor[axisType].LineNormal;
                            }
                        }
                    }
                }
                //Get index for dataseries - relates to result
                for (int j = 0; j < MainChart.DataSeries.Count; j++)
                {
                    if (e.DataSeries.Equals(MainChart.DataSeries[j]))
                    {
                        this.m_selectedDataSetries = j;
                        break;
                    }
                }
            }
            //Clear if starting a new selection and ctrl is not pressed
            if (!this.m_endSelect && this.m_MouseDownLocation != Point.Empty)
            {
                this.m_multiple.ClearSelectedRegions();
                this.m_page.ClearCurrentSelectedOnRoute();
            }
            this.m_MouseDownLocation = Point.Empty;

            //Select in charts etc only with current series. Use range instead of e 
            if (range != null && !float.IsNaN(range[0]))
            {
                MainChart_SelectingData(this.m_selectedDataSetries, range, false, this.m_endSelect);
            }
            this.m_endSelect = !this.m_endSelect;
        }

        void MainChart_SelectingData(int seriesIndex, float[] range, bool selecting, bool endSelect)
        {
            if (seriesIndex >= 0)
            {
                //Series must be added in order, so they can be resolved to result here
                TrailResult tr = m_trailResults[this.SeriesIndexToResult(seriesIndex)];

                bool markAll = (MainChart.DataSeries.Count <= MaxSelectedSeries);
                IList<TrailResult> markResults = new List<TrailResult>();
                //Reuse ZoomToSelection setting, to select all results
                if (/*Data.Settings.ZoomToSelection ||*/ tr is SummaryTrailResult)
                {
                    foreach (TrailResult tr2 in this.TrailResults)
                    {
                        markResults.Add(tr2);
                    }
                }
                else
                {
                    //If not summary set only mark selected
                    markAll = false;
                    markResults.Add(tr);
                }
                IList<float[]> regions;
                this.MainChart.DataSeries[seriesIndex].GetSelectedRegions(out regions);

                if (selecting && range == null)
                {
                    range = new float[2];
                    this.MainChart.DataSeries[seriesIndex].GetSelectedRange(out range[0], out range[1]);
                }

                //Find if a selection has decreased
                bool clearDecreased = false;
                if (range != null && regions != null && this.m_selectedStartRange != null &&
                    !float.IsNaN(range[0]) && !float.IsNaN(this.m_selectedStartRange[0]))
                {
                    if (float.IsNaN(m_selectedStartRange[1]) || float.IsNaN(range[1]))
                    {
                        //If first was "new", check if regions not expanding at both sides
                        foreach (float[] r in regions)
                        {
                            if (r[0] <= range[0] && range[0] < r[1] ||
                                r[0] < range[0] && range[0] <= r[1])
                            {
                                //First selection, second not yet set, clicking in selected region
                                r[0] = range[0];
                                if (!float.IsNaN(range[1]))
                                {
                                    r[1] = range[1];
                                }
                                else
                                {
                                    r[1] = range[0];
                                }
                                clearDecreased = true;
                            }
                        }
                    }
                    else if (this.m_selectedStartRange[0] < range[0] || range[1] < this.m_selectedStartRange[1])
                    //Selection decreasing from first selection
                    {
                        foreach (float[] r in regions)
                        {
                            if (r[0] <= range[0] && range[1] < r[1] ||
                                     r[0] < range[0] && range[1] <= r[1])
                            {
                                //Selection decreasing
                                r[0] = range[0];
                                r[1] = range[1];
                                clearDecreased = true;
                            }
                        }
                    }
                }
                if (clearDecreased)
                {
                    this.m_multiple.ClearSelectedRegions();
                }

                IList<Data.TrailResultMarked> results = new List<Data.TrailResultMarked>();
                foreach (TrailResult tr2 in markResults)
                {
                    IValueRangeSeries<DateTime> t2 = TrackUtil.GetResultRegions(XAxisReferential == XAxisValue.Time, tr2, this.ReferenceTrailResult, regions);
                    //Add ranges if single set, then it is a part of a new selection
                    if (!selecting && range != null)
                    {
                        if (float.IsNaN(range[1]) && !float.IsNaN(range[0]))
                        {
                            DateTime time;
                            if (XAxisReferential == XAxisValue.Time)
                            {
                                time = tr2.getDateTimeFromTimeResult(range[0]);
                            }
                            else
                            {
                                time = tr2.getDateTimeFromDistResult(TrackUtil.DistanceConvertTo(range[0], this.ReferenceTrailResult));
                            }
                            //Add a one second duration, otherwise rhere will be a complicated shared/Marked times combination
                            t2.Add(new ValueRange<DateTime> ( time, time.AddSeconds(1) ));
                        }
                    }
                    results.Add(new Data.TrailResultMarked(tr2, t2));
                }
                //this.MainChart.SelectData -= new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataHandler(MainChart_SelectData);
                //m_selectDataHandler = false;

                //Mark route track, but not chart
                m_page.MarkTrack(results, false, true);
                m_page.EnsureVisible(new List<Data.TrailResult> { tr }, false);

                int resultIndex;
                if (markAll)
                {
                    resultIndex = -1;
                }
                else
                {
                    resultIndex = SeriesIndexToResult(seriesIndex);
                }
                m_multiple.SetSelectedResultRange(resultIndex, regions, range);
                //this.MainChart.SelectData += new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataHandler(MainChart_SelectData);
                //m_selectDataHandler = true;

                if (!selecting && endSelect && !(tr is SummaryTrailResult))
                {
                    //TBD summary result, multiple result?
                    this.ShowSpeedToolTip(tr, regions);
                }
            }
        }

        public void ClearSelectedRegions()
        {
            if (this.MainChart != null && this.MainChart.DataSeries != null)
            {
                foreach (ChartDataSeries c in this.MainChart.DataSeries)
                {
                    c.ClearSelectedRegions();
                    c.SetSelectedRange(float.NaN, float.NaN);
               }
            }
        }

        //Mark the series for all or a specific result
        //Note: Clear should be done prior to the call, regions are added only
        public void SetSelectedResultRegions(int resultIndex, IList<float[]> regions, float[] range)
        {
            if (ShowPage)
            {
                //if (m_selectDataHandler)
                //{
                //    this.MainChart.SelectData -= new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataHandler(MainChart_SelectData);
                //}
                if (resultIndex < 0)
                {
                    //Use recursion to set all series
                    for (int j = 0; j < MainChart.DataSeries.Count; j++)
                    {
                        this.SetSelectedResultRegions(j, regions, range);
                    }
                    return;
                }

                if (regions != null && regions.Count > 0)
                {
                    foreach (float[] ax in regions)
                    {
                        //Ignore ranges outside current range and malformed scales
                        if (ax[0] < MainChart.XAxis.MaxOriginFarValue &&
                            MainChart.XAxis.MinOriginValue > float.MinValue &&
                            ax[1] > MainChart.XAxis.MinOriginValue &&
                            MainChart.XAxis.MaxOriginFarValue < float.MaxValue)
                        {
                            ax[0] = Math.Max(ax[0], (float)MainChart.XAxis.MinOriginValue);
                            ax[1] = Math.Min(ax[1], (float)MainChart.XAxis.MaxOriginFarValue);

                            foreach (int j in ResultIndexToSeries(resultIndex))
                            {
                                MainChart.DataSeries[j].AddSelecedRegion(ax[0], ax[1]);
                            }
                        }
                    }
                }

                if (range != null)
                {
                    //Ignore ranges outside current range and malformed scales
                    if (range[0] < MainChart.XAxis.MaxOriginFarValue &&
                        MainChart.XAxis.MinOriginValue > float.MinValue &&
                        (float.IsNaN(range[1]) ||
                        range[1] > MainChart.XAxis.MinOriginValue &&
                        MainChart.XAxis.MaxOriginFarValue < float.MaxValue))
                    {
                        range[0] = Math.Max(range[0], (float)MainChart.XAxis.MinOriginValue);
                        if (!float.IsNaN(range[1]))
                        {
                            range[1] = Math.Min(range[1], (float)MainChart.XAxis.MaxOriginFarValue);
                        }
                        if (range[1] == range[0])
                        {
                            //"Single" selection on chart
                            range[1] = float.NaN;
                        }
                        foreach (int j in ResultIndexToSeries(resultIndex))
                        {
                            MainChart.DataSeries[j].SetSelectedRange(range[0], range[1]);
                            MainChart.DataSeries[j].EnsureSelectedRangeVisible(); //Not working?
                        }
                    }
                }
                //if (m_selectDataHandler)
                //{
                //    this.MainChart.SelectData += new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataHandler(MainChart_SelectData);
                //}
            }
        }

        private void SetSelectedResultRegions(TrailResultMarked trm, bool isRegion, ref bool toolTipShown)
        {
            //Set the matching time distance for the activity
            for (int resultIndex = 0; resultIndex < m_trailResults.Count; resultIndex++)
            {
                TrailResult tr = m_trailResults[resultIndex];
                if (trm.trailResult.Activity == tr.Activity || this.m_trailResults.Count == 1)
                {
                    if (tr is SummaryTrailResult)
                    {
                        tr = this.ReferenceTrailResult;
                    }
                    IList<float[]> regions = TrackUtil.GetResultSelectionFromActivity(XAxisReferential == XAxisValue.Time, tr, ReferenceTrailResult, trm.selInfo);
                    if (isRegion)
                    {
                        this.SetSelectedResultRegions(resultIndex, regions, null);
                        if (!toolTipShown && trm.trailResult == tr)
                        {
                            //While more than one result may be shown, only one tooltip
                            this.ShowSpeedToolTip(tr, regions);
                            toolTipShown = true;
                        }
                    }
                    else
                    {
                        if (regions != null && regions.Count > 0)
                        {
                            this.SetSelectedResultRegions(resultIndex, null, regions[regions.Count - 1]);
                        }
                    }
                }
            }
        }

        public void SetSelectedResultRegions(IList<TrailResultMarked> atr, TrailResultMarked markedRange)
        {
            if (ShowPage && MainChart != null && MainChart.DataSeries != null &&
                    MainChart.DataSeries.Count > 0 &&
                m_trailResults.Count > 0)
            {
                this.ClearSelectedRegions();
                bool toolTipShown = false;
                foreach (TrailResultMarked trm in atr)
                {
                    SetSelectedResultRegions(trm, true, ref toolTipShown);
                }

                if (markedRange != null)
                {
                    SetSelectedResultRegions(markedRange, false, ref toolTipShown);
                }
            }
        }

        //Could use TrailResultMarked, but a selection of the track cannot be marked in multi mode
        public void EnsureVisible(IList<TrailResult> atr)
        {
            if (ShowPage)
            {
                foreach (TrailResult tr in atr)
                {
                    int resultIndex = -1;
                    for (int i = 0; i < MainChart.DataSeries.Count; i++)
                    {
                        MainChart.DataSeries[i].ClearSelectedRegions();
                        if (m_trailResults[SeriesIndexToResult(i)].Equals(tr))
                        {
                            resultIndex = SeriesIndexToResult(i);
                            break;
                        }
                    }
                    foreach (int j in ResultIndexToSeries(resultIndex))
                    {
                        MainChart.DataSeries[j].AddSelecedRegion(
                                                MainChart.DataSeries[j].XMin, MainChart.DataSeries[j].XMax);
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
                        INumericTimeDataSeries graphPoints = LineChartUtil.GetSmoothedActivityTrack(tr, chartType, ReferenceTrailResult);

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

        private int SeriesIndexToResult(int seriesIndex)
        {
            return seriesIndex % this.m_trailResults.Count;
        }

        private int[] ResultIndexToSeries(int resultIndex)
        {
            if (resultIndex < 0) { return new int[0]; }
            int[] indexes = new int[MainChart.DataSeries.Count / this.m_trailResults.Count];
            for (int i = 0; i < indexes.Length; i++)
            {
                indexes[i] = this.SeriesIndexToResult(resultIndex) + i * this.m_trailResults.Count;
            }
            return indexes;
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
                    if (syncGraphOffsetChartType != LineChartUtil.ChartToAxis(syncGraphOffsetChartType) && m_ChartTypes.Contains(LineChartUtil.ChartToAxis(syncGraphOffsetChartType)))
                    {
                        syncGraphOffsetChartType = LineChartUtil.ChartToAxis(syncGraphOffsetChartType);
                    }
                }
                //The ST standard order is to draw the Fill chart latest, so it covers others
                //This hides charts when there are several
                IList<LineChartTypes> chartTypes = m_ChartTypes;
                if (m_trailResults.Count == 1)
                {
                    chartTypes = new List<LineChartTypes>();
                    foreach (LineChartTypes chartType in m_ChartTypes)
                    {
                        chartTypes.Insert(0, chartType);
                    }
                }
                foreach (LineChartTypes chartType in chartTypes)
                {
                    
                    //LineChartTypes chartType = m_ChartTypes[k];
                    ChartDataSeries summaryDataLine = null;
                    IList<ChartDataSeries> summarySeries = new List<ChartDataSeries>();
                    INumericTimeDataSeries refGraphPoints = null;
                    LineChartTypes refChartType = chartType;

                    if (syncGraph != SyncGraphMode.None)
                    {
                        if (chartType != LineChartUtil.ChartToAxis(chartType) && m_ChartTypes.Contains(LineChartUtil.ChartToAxis(chartType)))
                        {
                            refChartType = LineChartUtil.ChartToAxis(chartType);
                        }
                        if (refChartType == syncGraphOffsetChartType && ReferenceTrailResult != null)
                        {
                            refGraphPoints = LineChartUtil.GetSmoothedActivityTrack(ReferenceTrailResult, refChartType, ReferenceTrailResult);
                        }
                    }

                    //Note: If the add order changes, the dataseries to result lookup in MainChart_SelectData is affected too
                    for (int i = 0; i < chartResults.Count; i++)
                    {
                        TrailResult tr = chartResults[i];

                        ChartDataSeries dataLine = new ChartDataSeries(MainChart, m_axisCharts[chartType]);

                        //Add to the chart only if result is visible (no "summary" results)
                        if (m_trailResults.Contains(tr))
                        {
                            //Note: Add empty Dataseries even if no graphpoints. index must match results
                            MainChart.DataSeries.Add(dataLine);

                            //Update display only data
                            //It could be possible to add basis for dataseries in .Data, to only recalc the points. Not so much gain
                            dataLine.ValueAxisLabel = ChartDataSeries.ValueAxisLabelType.Average;
                            {
                                ChartColors chartColor;
                                //Color for the graph - keep standard color if only one result displayed
                                if (m_trailResults.Count <= 1 || summarySpecialColor ||
                                    Data.Settings.OnlyReferenceRight && (m_axisCharts[chartType] is RightVerticalAxis))
                                {
                                    chartColor = ColorUtil.ChartColor[chartType];
                                }
                                else
                                {
                                    //TBD? other color for children (at least if only one selected)
                                    chartColor = tr.ResultColor;
                                }

                                dataLine.LineColor = chartColor.LineNormal;
                                dataLine.FillColor = chartColor.FillNormal;
                                dataLine.SelectedColor = chartColor.FillSelected; //The selected fill color only
                            }

                            //Decrease alpha for many activities
                            if (m_trailResults.Count > 1)
                            {
                                int alpha = dataLine.FillColor.A - m_trailResults.Count * 3;
                                alpha = Math.Min(alpha, 0x77); //Color.FromArgb should take uint...
                                alpha = Math.Max(alpha, 0x10);
                                dataLine.FillColor = Color.FromArgb(dataLine.FillColor.ToArgb() - (dataLine.FillColor.A - alpha) * 0x1000000);
                            }

                            //Set chart type to Fill similar to ST for first result
                            if (m_ChartTypes[0] == chartType)
                            {
                                dataLine.ChartType = ChartDataSeries.Type.Fill;
                            }
                            else
                            {
                                dataLine.ChartType = ChartDataSeries.Type.Line;
                            }
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
                                graphPoints = LineChartUtil.GetSmoothedActivityTrack(tr, chartType, refTr);
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
                                if (refChartType == LineChartUtil.ChartToAxis(chartType) && (refChartType != chartType || ReferenceTrailResult != tr))
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
                        if (summarySeries.Count == 1)
                        {
                            //Cannot create a summary from one line, just copy the original
                            foreach (KeyValuePair<float, PointF> kv in summarySeries[0].Points)
                            {
                                summaryDataLine.Points.Add(kv.Key, kv.Value);
                            }
                        }
                        else
                        {
                            //Only add if more than one one result
                            this.getCategoryAverage(summaryDataLine, summarySeries);
                        }

                    }
                }  //for all axis

                if (syncGraph != SyncGraphMode.None && syncGraphOffsetCount > 0)
                {
                    ShowGeneralToolTip(syncGraph.ToString() + ": " + syncGraphOffsetSum / syncGraphOffsetCount); //TODO: Translate
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
                        xValue += TrackUtil.GetResyncOffset(XAxisReferential == XAxisValue.Time, false, tr, this.ReferenceTrailResult, xValue, out nextXvalue);
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
            LineChartTypes axisType = LineChartUtil.ChartToAxis(chartType);
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
                //Select the first axis by default
                if (this.m_ChartTypes != null && this.m_ChartTypes.Count > 0)
                {
                    this.m_lastSelectedType = this.m_ChartTypes[0];
                }
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

        void MainChart_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            this.m_CtrlPressed = false;
        }

        void MainChart_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            bool smoothChanged = false;
            bool increase = true;
            bool reset = false;
            bool zero = false;
            bool refreshData = false;
            bool clearRefreshData = true;

            this.m_CtrlPressed = e.Modifiers == Keys.Control;
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
                    if (this.m_multipleCharts && charts.Contains(kp.Key))
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
            if (m_cursorLocationAtMouseMove != null)
            {
                summaryListToolTip.Show(
                    GetDefaultSmooth(chartType).ToString(),
                    this,
                    new System.Drawing.Point(m_cursorLocationAtMouseMove.X +
                                  Cursor.Current.Size.Width / 2,
                                        m_cursorLocationAtMouseMove.Y),
                   summaryListToolTip.AutoPopDelay);
            }
        }

        private void ShowSpeedToolTip(TrailResult tr, IList<float[]> regions)
        {
            if (!(tr is SummaryTrailResult))
            {
                //TBD summary result, multiple result?
                float dist = 0;
                float time = 0;
                foreach (float[] r in regions)
                {
                    TrackUtil.GetDistanceTimeSelection(XAxisReferential == XAxisValue.Time, tr, this.ReferenceTrailResult, r, ref dist, ref time);
                }
                if (time > 0)
                {
                    float speed = dist / time;
                    string s = UnitUtil.PaceOrSpeed.ToString(speed, tr.Activity, "mm:ssU");
                    this.ShowGeneralToolTip(s);
                }
            }
        }

        public void ShowGeneralToolTip(string s)
        {
            summaryListToolTip.Show(s, this, //TODO: Relate to axis?
                new System.Drawing.Point(10 + Cursor.Current.Size.Width / 2, 10),
                summaryListToolTip.AutoPopDelay);
        }

        void MainChart_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.m_endSelect = false;
            if (!this.m_CtrlPressed)
            {
                this.m_MouseDownLocation = e.Location;
            }
            else
            {
                this.m_MouseDownLocation = Point.Empty;
            }
        }

        void MainChart_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            m_cursorLocationAtMouseMove = e.Location;
        }

        void MainChart_SelectAxisLabel(object sender, ChartBase.AxisEventArgs e)
        {
            if (this.m_multipleCharts && (e.Axis is RightVerticalAxis || e.Axis is LeftVerticalAxis))
            {
                //Select all charts for this axis
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

                foreach (LineChartTypes chartType in this.m_ChartTypes)
                {
                    if (m_axisCharts[chartType] == e.Axis)
                    {
                        //More than one chart could exist for the axis, only select the first
                        m_lastSelectedType = chartType;
                        m_axisCharts[m_lastSelectedType].LabelColor = Color.Black;
                        ShowSmoothToolTip(chartType);
                    }
                    else
                    {
                        m_axisCharts[chartType].LabelColor = ColorUtil.ChartColor[chartType].LineNormal;
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
