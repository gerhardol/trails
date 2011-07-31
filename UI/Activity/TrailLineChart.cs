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
#endif
using TrailsPlugin.Data;
using GpsRunningPlugin.Util;

namespace TrailsPlugin.UI.Activity {
	public partial class TrailLineChart : UserControl {
        private Data.TrailResult m_refTrailResult = null;
        private IList<Data.TrailResult> m_trailResults = new List<Data.TrailResult>();

        private XAxisValue m_XAxisReferential = XAxisValue.Time;
        private IList<LineChartTypes> m_YAxisReferentials = new List<LineChartTypes>();
        private IDictionary<LineChartTypes, IAxis> m_axis = new Dictionary<LineChartTypes, IAxis>();
        private LineChartTypes m_lastSelectedType = LineChartTypes.Unknown;
        private IDictionary<LineChartTypes, bool> m_hasValues = null;

        private bool m_multipleCharts = false;
        private bool m_visible = false;
        private ITheme m_visualTheme;
        private ActivityDetailPageControl m_page;
        private MultiChartsControl m_multiple;
        private bool m_selectDataHandler = true; //Event handler is enabled by default

        const int MaxSelectedSeries = 5;

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
            m_page = page;
            m_multiple = multiple;
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
                foreach (KeyValuePair<LineChartTypes, IAxis> kp in m_axis)
                {
                    kp.Value.LabelColor = LineChartUtil.ChartColor[kp.Key];
                }
                e.DataSeries.ValueAxis.LabelColor = Color.Black;// e.DataSeries.SelectedColor;
                //Get index for dataseries - same as for result
                int i = -1;

                for (int j = 0; j < MainChart.DataSeries.Count; j++)
                {
                    if (e.DataSeries.Equals(MainChart.DataSeries[j]))
                    {
                        i = j;
                        break;
                    }
                }
                if (i >= 0)
                {
                    if (i==1 && MainChart.DataSeries[0].ChartType == ChartDataSeries.Type.Fill)
                    {
                        //Use the fill chart rather than the line chart (this normally not occurs)
                        i = 0;
                    }

                    //Results must be added in order, so they can be resolved to result here
                    TrailResult tr = m_trailResults[i % this.TrailResults.Count];
                    IList<float[]> regions;
                    e.DataSeries.GetSelectedRegions(out regions);

                    IList<Data.TrailResultMarked> results = new List<Data.TrailResultMarked>();
                    IValueRangeSeries<DateTime> t = new ValueRangeSeries<DateTime>();
                    foreach (float[] at in regions)
                    {
                        DateTime d1;
                        DateTime d2;
                        if (XAxisReferential == XAxisValue.Time)
                        {
                            d1 = tr.getDateTimeFromElapsedResult(GetResyncOffsetTime(tr, at[0]));
                            d2 = tr.getDateTimeFromElapsedResult(GetResyncOffsetTime(tr, at[1]));
                        }
                        else
                        {
                            
                            d1 = tr.getDateTimeFromDistResult(TrailResult.DistanceConvertTo(GetResyncOffsetDist(tr, at[0]), ReferenceTrailResult));
                            d2 = tr.getDateTimeFromDistResult(TrailResult.DistanceConvertTo(GetResyncOffsetDist(tr, at[1]), ReferenceTrailResult));
                        }
                        t.Add(new ValueRange<DateTime>(d1, d2));
                    }
                    results.Add(new Data.TrailResultMarked(tr, t));
                    this.MainChart.SelectData -= new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataHandler(MainChart_SelectData);
                    m_selectDataHandler = false;

                    bool markAll = (MainChart.DataSeries.Count <= MaxSelectedSeries);
                    //Mark route track, but not chart
                    m_page.MarkTrack(results, false);
                    m_page.EnsureVisible(new List<Data.TrailResult> { tr }, false);

                    if (markAll)
                    {
                        m_multiple.SetSelectedResultRange(regions);
                    }
                    else
                    {
                        //Assumes that not single results are set
                        m_multiple.SetSeriesSelectedResultRange(i, regions);
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
                //Do not mark the line chart related to fill chart
                if (!(MainChart.DataSeries[i].ChartType == ChartDataSeries.Type.Fill))
                {
                    SetSeriesSelectedResultRange(i, false, regions);
                }
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
                    }
                }
                if (m_selectDataHandler)
                {
                    this.MainChart.SelectData += new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataHandler(MainChart_SelectData);
                }
            }
        }

        //No TrailResult - use all possible matches
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
                //Note: Time/dist need the Trailresult related to the current results...
                //With single results, this can be done, but for mult results per activity this can be incorrect
                IList<float[]> l = null;
                IList<TrailResult> t = TrailResultWrapper.GetTrailResults(Controller.TrailController.Instance.CurrentActivityTrail.ResultTreeList);
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
                            //The "fill" chart is 0, line is 1
                            if (i == 0 && MainChart.DataSeries.Count > 1 &&
                                MainChart.DataSeries[i].ChartType == ChartDataSeries.Type.Fill)
                            {
                                MainChart.DataSeries[1].ClearSelectedRegions();
                            }

                            //The result is for the main result. Instead of calculating GetResultSelectionFromActivity() for each subsplit, find the offset
                            float offset = 0;
                            if (m_trailResults[i].ParentResult != null)
                            {
                                if (XAxisReferential == XAxisValue.Time)
                                {
                                    offset = (float)(m_trailResults[i].StartDateTime - m_trailResults[i].ParentResult.StartDateTime).TotalSeconds;
                                }
                                else
                                {
                                    offset = (float)TrailResult.DistanceConvertFrom(m_trailResults[i].StartDist - m_trailResults[i].ParentResult.StartDist, m_refTrailResult);
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

                                int index = i;
                                if (i == 0 && MainChart.DataSeries.Count > 1 &&
                                    MainChart.DataSeries[i].ChartType == ChartDataSeries.Type.Fill)
                                {
                                    //For line/fill graph
                                    index = 1;
                                }

                                MainChart.DataSeries[index].SetSelectedRange(x1, x2);
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
                            IList<float[]> t = GetResultSelectionFromActivity(m_trailResults[i], trm.selInfo);
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

                                    int index = i;
                                    if (i == 0 && MainChart.DataSeries.Count > 1 &&
                                        MainChart.DataSeries[i].ChartType == ChartDataSeries.Type.Fill)
                                    {
                                        //For line/fill graph
                                        index = 1;
                                    }
                                    MainChart.DataSeries[index].AddSelecedRegion(ax[0], ax[1]);
                                }
                            }
                        }
                    }
                }
            }
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
            x1 = (float)(tr.getElapsedResult(d1));
            x2 = (float)(tr.getElapsedResult(d2));
            return new float[] { x1, x2 };
        }
        float[] GetSingleSelectionFromResult(TrailResult tr, double t1, double t2)
        {
            float x1 = float.MaxValue, x2 = float.MinValue;
            //distance is for result, then to display units
            x1 = (float)TrailResult.DistanceConvertFrom(t1, ReferenceTrailResult);
            x2 = (float)TrailResult.DistanceConvertFrom(t2, ReferenceTrailResult);
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
                        //Clear all series, no line/fill check
                        MainChart.DataSeries[i].ClearSelectedRegions();
                        //For "single result" only select first series
                        if (i < m_trailResults.Count &&
                            m_trailResults[i].Equals(tr) &&
                            MainChart.DataSeries[i].ChartType != ChartDataSeries.Type.Fill)
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
            return m_axis != null && m_axis.Count>0;
        }

        public bool HasValues(LineChartTypes yaxis)
        {
            if (m_hasValues == null)
            {
                m_hasValues = new Dictionary<LineChartTypes, bool>();
            }
            if(!m_hasValues.ContainsKey(yaxis))
            {
                m_hasValues.Add(yaxis, false);
                //A diff to itself is not a value - enable replacing
                //TODO: Change diff to self to show diff to average
                //if (!(m_trailResults == null || m_refTrailResult == null ||
                //    (yaxis == LineChartTypes.DiffTime || yaxis == LineChartTypes.DiffDist) &&
                //    m_trailResults.Count == 1 && m_trailResults[0] == m_refTrailResult))
                {
                    for (int i = 0; i < m_trailResults.Count; i++)
                    {
                        TrailResult tr = m_trailResults[i];
                        //As this value is cached, it is no extra to request and drop it
                        INumericTimeDataSeries graphPoints = GetSmoothedActivityTrack(tr, yaxis, ReferenceTrailResult);

                        if (graphPoints != null && graphPoints.Count > 1)
                        {
                            m_hasValues[yaxis] = true;
                            break;
                        }
                    }
                }
            }
            return m_hasValues[yaxis];
        }

        virtual protected void SetupDataSeries()
        {
			MainChart.DataSeries.Clear();
            MainChart.XAxis.Markers.Clear();
            if (m_visible)
            {
                m_hasValues = null;

                // Add main data. We must use 2 separate data series to overcome the display
                //  bug in fill mode.  The main data series is normally rendered but the copy
                //  is set in Line mode to be displayed over the fill
                //Note: If the add order changes, the dataseries to result lookup in MainChart_SelectData is affected too
                foreach (LineChartTypes yaxis in m_axis.Keys)
                {

                    for (int i = 0; i < m_trailResults.Count; i++)
                    {
                        TrailResult tr = m_trailResults[i];
                        INumericTimeDataSeries graphPoints;

                        //Hide right column graph in some situations
                        if (1 >= m_trailResults.Count || !Data.Settings.OnlyReferenceRight ||
                           m_axis.ContainsKey(yaxis) && !(m_axis[yaxis] is RightVerticalAxis) || 
                           tr == m_refTrailResult || null == m_refTrailResult)
                        {
                            graphPoints = GetSmoothedActivityTrack(tr, yaxis, ReferenceTrailResult);
                        }
                        else
                        {
                            graphPoints = new NumericTimeDataSeries();
                        }

                        if (graphPoints.Count <= 1)
                        {
                            if (m_trailResults.Count > 1)
                            {
                                //Add empty, Dataseries index must match results 
                                MainChart.DataSeries.Add(new ChartDataSeries(MainChart, m_axis[yaxis]));
                            }
                        }
                        else
                        {
                            Color chartLineColor;
                            Color chartSelectedColor;
                            if (m_trailResults.Count <= 1 ||
                                Data.Settings.OnlyReferenceRight && tr == m_refTrailResult && (m_axis[yaxis] is RightVerticalAxis))
                            {
                                chartLineColor = LineChartUtil.ChartColor[yaxis];
                                chartSelectedColor = ControlPaint.Dark(LineChartUtil.ChartColor[yaxis], 0.01F);
                            }
                            else
                            {
                                chartLineColor = m_trailResults[i].TrailColor;
                                chartSelectedColor = chartLineColor;
                            }
                            Color chartFillColor = chartLineColor;

                            ChartDataSeries dataFill = null;

                            if (m_trailResults.Count == 1 && m_axis[yaxis] is LeftVerticalAxis)
                            {
                                dataFill = new ChartDataSeries(MainChart, MainChart.YAxis);
                                MainChart.DataSeries.Add(dataFill);

                                dataFill.ChartType = ChartDataSeries.Type.Fill;
                                dataFill.LineColor = chartLineColor;
                                dataFill.SelectedColor = chartSelectedColor;
                                dataFill.FillColor = Color.WhiteSmoke;
                            }
                            ChartDataSeries dataLine = new ChartDataSeries(MainChart, m_axis[yaxis]);
                            MainChart.DataSeries.Add(dataLine);

                            dataLine.ChartType = ChartDataSeries.Type.Line;
                            dataLine.LineColor = chartLineColor;
                            dataLine.SelectedColor = chartSelectedColor;

                            INumericTimeDataSeries dataPoints;
                            if (XAxisReferential == XAxisValue.Time)
                            {
                                dataPoints = graphPoints;
                            }
                            else
                            {
                                dataPoints = m_trailResults[i].DistanceMetersTrack0(ReferenceTrailResult);
                            }
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
                                        xValue = (float)m_trailResults[i].getElapsedResult(time);
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
                                        if (yValueEntry != null)
                                        {
                                            PointF point = new PointF(xValue, yValueEntry.Value);
                                            if (null != dataFill)
                                            {
                                                dataFill.Points.Add(elapsedEntry, point);
                                            }
                                            dataLine.Points.Add(elapsedEntry, point);
                                        }
                                        oldElapsedEntry = (int)elapsedEntry;
                                        oldXvalue = xValue;
                                    }
                                }
                            }
                        }
                    }
                }

                ///////TrailPoints
                Data.TrailResult trailPointResult = ReferenceTrailResult;
                //If only one result is used, it can be confusing if the trail points are set for ref
                if (!Data.Settings.SyncChartAtTrailPoints && m_trailResults.Count == 1 ||
                    m_trailResults.Count > 0 && trailPointResult == null)
                {
                    trailPointResult = m_trailResults[0];
                }

                if (trailPointResult != null)
                {
                    Image icon =
#if ST_2_1
                        CommonResources.Images.Information16;
#else
                        new Bitmap(TrailsPlugin.CommonIcons.fileCircle(11, 11, trailPointResult.TrailColor));
#endif
                    double oldElapsed = double.MinValue;
                    foreach (DateTime t in trailPointResult.TrailPointDateTime)
                    {
                        double elapsed;
                        if (XAxisReferential == XAxisValue.Time)
                        {
                            elapsed = trailPointResult.getElapsedResult(t);
                        }
                        else
                        {
                            elapsed = TrailResult.DistanceConvertFrom(
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
                foreach (LineChartTypes yaxis in m_YAxisReferentials)
                {
                    CreateAxis(yaxis, m_axis.Count==0);
                }
            }
        }
        
        private void CreateAxis(LineChartTypes axisType, bool left)
        {
            if ((m_trailResults == null || ReferenceTrailResult == null ||
                (axisType == LineChartTypes.DiffTime || axisType == LineChartTypes.DiffDist) &&
                m_trailResults.Count == 1 && m_trailResults == ReferenceTrailResult))
            {
                return;
            }
            if (!m_axis.ContainsKey(axisType))
            {
                IAxis axis;
                if (left)
                {
                    axis = MainChart.YAxis;
                }
                else
                {
                    axis = new RightVerticalAxis(MainChart);
                    axis.SmartZoom = true;
                    MainChart.YAxisRight.Add(axis);
                }
                m_axis.Add(axisType, axis);
                axis.LabelColor = LineChartUtil.ChartColor[axisType];
                LineChartUtil.SetupYAxisFormatter(axisType, axis, ReferenceTrailResult.Activity);
            }
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
        public LineChartTypes YAxisReferential
        {
            get
            {
                if (m_YAxisReferentials == null || m_YAxisReferentials.Count == 0)
                {
                    return LineChartTypes.Unknown;
                }
                return m_YAxisReferentials[0];
            }
            set
            {
                YAxisReferentials = new List<LineChartTypes> { value };
            }
        }
        public IList<LineChartTypes> YAxisReferentials
        {
            get
            {
                return m_YAxisReferentials;
            }
            set
            {
                m_YAxisReferentials = value;
                m_axis = new Dictionary<LineChartTypes, IAxis>();

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
                        m_trailResults = new List<Data.TrailResult>();
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

        System.Drawing.Point summaryListCursorLocationAtMouseMove;
        void MainChart_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            summaryListCursorLocationAtMouseMove = e.Location;
        }

        int MainChart_KeyDown_Smooth(int val, IList<LineChartTypes> chartTypes, bool increase, bool reset, bool zero)
        {
            //No action for reset, value alreade set
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
            if (summaryListCursorLocationAtMouseMove != null)
            {
                summaryListToolTip.Show(val.ToString(),
                              this,
                              new System.Drawing.Point(summaryListCursorLocationAtMouseMove.X +
                                  Cursor.Current.Size.Width / 2,
                                        summaryListCursorLocationAtMouseMove.Y),
                              summaryListToolTip.AutoPopDelay);
            }
            return val;
        }

        void MainChart_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            LineChartTypes selectedTypes = LineChartTypes.Unknown;

            bool increase = true;
            bool reset = false;
            bool zero = false;
            bool refreshData = true;

            if (e.KeyCode == Keys.Home)
            {
                reset = true;
            }
            else if (e.KeyCode == Keys.End)
            {
                zero = true;
            }
            else if (e.Modifiers == Keys.Shift)
            {
                increase = false;
            }

            if (e.KeyCode == Keys.PageDown || e.KeyCode == Keys.PageUp)
            {
                selectedTypes = m_lastSelectedType;
                if (e.KeyCode == Keys.PageDown)
                {
                    increase = false;
                }
                if (!m_axis.ContainsKey(selectedTypes))
                {
                    foreach(LineChartTypes l in m_axis.Keys)
                    {
                        if (m_axis[l] is LeftVerticalAxis)
                        {
                            selectedTypes=l;
                            break;
                        }
                    }
                }
            }
            else if (e.KeyCode == Keys.C)
            {
                selectedTypes = LineChartTypes.Cadence;
            }
            else if (e.KeyCode == Keys.E)
            {
                selectedTypes = LineChartTypes.Elevation;
            }
            else if (e.KeyCode == Keys.H)
            {
                selectedTypes = LineChartTypes.HeartRateBPM;
            }
            else if (e.KeyCode == Keys.P)
            {
                selectedTypes = LineChartTypes.Power;
            }
            else if (e.KeyCode == Keys.R)
            {
                Data.Settings.OnlyReferenceRight = !(e.Modifiers == Keys.Shift);
            }
            else if (e.KeyCode == Keys.S)
            {
                selectedTypes = LineChartTypes.Speed;
            }
            else if (e.KeyCode == Keys.T)
            {
                Data.Settings.SyncChartAtTrailPoints = (e.Modifiers != Keys.Shift);
                refreshData = false;
            }

            IList<LineChartTypes> chartTypes = new List<LineChartTypes> { selectedTypes };
            switch (selectedTypes)
            {
                case LineChartTypes.Cadence:
                {
                    int val = TrailResult.TrailActivityInfoOptions.CadenceSmoothingSeconds;
                    if (reset)
                    {
                        val = TrailResult.TrailActivityInfoOptions.CadenceSmoothingSeconds;
                    }
                    TrailResult.TrailActivityInfoOptions.CadenceSmoothingSeconds =
                        MainChart_KeyDown_Smooth(val, chartTypes, increase, reset, zero);
                    break;
                }
                case LineChartTypes.Elevation:
                case LineChartTypes.Grade:
                {
                    int val = TrailResult.TrailActivityInfoOptions.ElevationSmoothingSeconds;
                    if (reset)
                    {
                        val = TrailResult.TrailActivityInfoOptions.ElevationSmoothingSeconds;
                    }
                    chartTypes.Add(LineChartTypes.Grade);
                    TrailResult.TrailActivityInfoOptions.ElevationSmoothingSeconds =
                        MainChart_KeyDown_Smooth(val, chartTypes, increase, reset, zero);
                    break;
                }
                case LineChartTypes.HeartRateBPM:
                {
                    int val = TrailResult.TrailActivityInfoOptions.HeartRateSmoothingSeconds;
                    if (reset)
                    {
                        val = TrailResult.TrailActivityInfoOptions.HeartRateSmoothingSeconds;
                    }
                    TrailResult.TrailActivityInfoOptions.HeartRateSmoothingSeconds =
                        MainChart_KeyDown_Smooth(val, chartTypes, increase, reset, zero);
                    break;
                }
                case LineChartTypes.Power:
                {
                    int val = TrailResult.TrailActivityInfoOptions.PowerSmoothingSeconds;
                    if (reset)
                    {
                        val = TrailResult.TrailActivityInfoOptions.PowerSmoothingSeconds;
                    }
                    TrailResult.TrailActivityInfoOptions.PowerSmoothingSeconds =
                        MainChart_KeyDown_Smooth(val, chartTypes, increase, reset, zero);
                    break;
                }
                case LineChartTypes.Speed:
                case LineChartTypes.Pace:
                {
                    int val = TrailResult.TrailActivityInfoOptions.SpeedSmoothingSeconds;
                    if (reset)
                    {
                        val = TrailResult.TrailActivityInfoOptions.SpeedSmoothingSeconds;
                    }
                    chartTypes.Add(LineChartTypes.Pace);
                    //chartTypes.Add(LineChartTypes.SpeedPace);
                    TrailResult.TrailActivityInfoOptions.SpeedSmoothingSeconds =
                        MainChart_KeyDown_Smooth(val, chartTypes, increase, reset, zero);
                    break;
                }
            }

            if (refreshData)
            {
                foreach (TrailResult t in TrailResults)
                {
                    t.Clear(true);
                }
            }
            m_page.RefreshControlState(); 
            m_page.RefreshChart();
            foreach (KeyValuePair<LineChartTypes, IAxis> kp in m_axis)
            {
                if (chartTypes.Contains(kp.Key))
                {
                    kp.Value.LabelColor = Color.Black;
                }
                else
                {
                    kp.Value.LabelColor = LineChartUtil.ChartColor[kp.Key];
                }
            }
        }

        void MainChart_SelectAxisLabel(object sender, ChartBase.AxisEventArgs e)
        {
            if (e.Axis is RightVerticalAxis || e.Axis is LeftVerticalAxis)
            {
                foreach (LineChartTypes l in m_axis.Keys)
                {
                    if (m_axis[l] == e.Axis)
                    {
                        m_lastSelectedType = l;
                        break;
                    }
                }
            }
        }

        public bool BeginUpdate()
        {
			return MainChart.BeginUpdate();
		}

		public void EndUpdate() {
			MainChart.EndUpdate();
		}
	}
}
