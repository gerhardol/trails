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
        private IDictionary<LineChartTypes, Color> m_color = new Dictionary<LineChartTypes, Color>();
        private bool m_multipleCharts = false;
        private Color m_ChartFillColor = Color.WhiteSmoke;
        private Color m_ChartLineColor = Color.LightSkyBlue;
        private Color m_ChartSelectedColor = Color.AliceBlue;
        private ITheme m_visualTheme;
        private ActivityDetailPageControl m_page = null;
        private MultiChartsControl m_multiple = null;
        private bool m_visible = false;

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
            m_color.Add(LineChartTypes.Speed, Color.Navy);
            m_color.Add(LineChartTypes.Pace, Color.Navy);
            m_color.Add(LineChartTypes.Elevation, Color.Sienna);
            m_color.Add(LineChartTypes.Grade, Color.Sienna);
            m_color.Add(LineChartTypes.HeartRateBPM, Color.Red);
            m_color.Add(LineChartTypes.Cadence, Color.Green);
            m_color.Add(LineChartTypes.Power, Color.Purple);
            m_color.Add(LineChartTypes.DiffTime, Color.Chocolate);
            m_color.Add(LineChartTypes.DiffDist, Color.CornflowerBlue);
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
        public enum XAxisValue
        {
			Time,
			Distance
		}
        //No simple way to dynamically translate enum
        //The integer (raw) value is stored as defaults too
        public static string XAxisValueString(XAxisValue XAxisReferential)
        {
            string xAxisLabel="";
            switch (XAxisReferential)
            {
                case XAxisValue.Distance:
                    {
                        xAxisLabel = CommonResources.Text.LabelDistance;
                        break;
                    }
                case XAxisValue.Time:
                    {
                        xAxisLabel = CommonResources.Text.LabelTime;
                        break;
                    }
                default:
                    {
                        Debug.Assert(false);
                        break;
                    }
            }
            return xAxisLabel;
        }

        public enum LineChartTypes
        {
            Unknown,
            Cadence,
            Elevation,
            HeartRateBPM,
            HeartRatePercentMax,
            Power,
            Grade,
            Speed,
            Pace,
            SpeedPace,
            DiffTime,
            DiffDist,
            DiffDistTime,
            DiffHeartRateBPM, //NotUsedInTrails
            Time, //NotUsedInTrails
            Distance //NotUsedInTrails
        }
        public static IList<LineChartTypes> DefaultLineChartTypes()
        {
            return new List<LineChartTypes>{
                LineChartTypes.SpeedPace, LineChartTypes.Elevation,
                LineChartTypes.HeartRateBPM, LineChartTypes.Cadence};
        }
        public static string ChartTypeString(TrailLineChart.LineChartTypes x)
        {
            return TrailLineChart.LineChartTypesString((TrailLineChart.LineChartTypes)x);
        }
        public static string LineChartTypesString(LineChartTypes YAxisReferential)
        {
            string yAxisLabel="";
			switch (YAxisReferential) {
				case LineChartTypes.Cadence: {
						yAxisLabel = CommonResources.Text.LabelCadence;
						break;
					}
				case LineChartTypes.Elevation: {
						yAxisLabel = CommonResources.Text.LabelElevation;
						break;
					}
				case LineChartTypes.HeartRateBPM: {
						yAxisLabel = CommonResources.Text.LabelHeartRate;
						break;
					}
				case LineChartTypes.HeartRatePercentMax: {
						yAxisLabel = CommonResources.Text.LabelHeartRate;
						break;
					}
				case LineChartTypes.Power: {
						yAxisLabel = CommonResources.Text.LabelPower;
						break;
					}
				case LineChartTypes.Speed: {
						yAxisLabel = CommonResources.Text.LabelSpeed;
						break;
					}
				case LineChartTypes.Pace: {
						yAxisLabel = CommonResources.Text.LabelPace;
						break;
					}
                case LineChartTypes.SpeedPace:
                    {
                        yAxisLabel = CommonResources.Text.LabelSpeed + CommonResources.Text.LabelPace;
                        break;
                    }
                case LineChartTypes.Grade:
                    {
                        yAxisLabel = CommonResources.Text.LabelGrade;
                        break;
                    }
                case LineChartTypes.DiffDist:
                    {
                        yAxisLabel = CommonResources.Text.LabelDistance;
                        break;
                    }
                case LineChartTypes.DiffTime:
                    {
                        yAxisLabel = CommonResources.Text.LabelTime;
                        break;
                    }
                case LineChartTypes.DiffDistTime:
                    {
                        yAxisLabel = CommonResources.Text.LabelDistance + CommonResources.Text.LabelTime;
                        break;
                    }
                default:
                    {
						Debug.Assert(false);
						break;
					}
            }
            return yAxisLabel;
        }

        /********************************************/ 
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
            foreach (KeyValuePair<LineChartTypes, IAxis> kp in m_axis)
            {
                kp.Value.LabelColor = m_color[kp.Key];
            }
            e.DataSeries.ValueAxis.LabelColor = Color.Black;// e.DataSeries.SelectedColor;
        }

        void MainChart_SelectData(object sender, ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataEventArgs e)
        {
            if (e != null && e.DataSeries != null && m_page != null)
            {
                //Get index for dataseries - same as for result
                int i = -1;
                if (MainChart.DataSeries.Count==2 && m_YAxisReferentials.Count==1 &&
                    m_trailResults.Count==1)
                {
                    //Match the result, the first is the fill chart
                    i = 0;
                }
                else
                {
                    for (int j = 0; j < MainChart.DataSeries.Count; j++)
                    {
                        if (e.DataSeries.Equals(MainChart.DataSeries[j]))
                        {
                            i = j;
                            break;
                        }
                    }
                }
                if (i >= 0)
                {
                    //Results must be added in order
                    TrailResult tr = m_trailResults[i/m_YAxisReferentials.Count];
                    IList<float[]> regions;
                    e.DataSeries.GetSelectedRegions(out regions);

                    IList<Data.TrailResultMarked> results = new List<Data.TrailResultMarked>();
                    IValueRangeSeries<DateTime> t = new ValueRangeSeries<DateTime>();
                    if (XAxisReferential == XAxisValue.Time)
                    {
                        foreach (float[] at in regions)
                        {
                            t.Add(new ValueRange<DateTime>(
                                tr.getDateTimeFromElapsedResult(at[0]),
                                tr.getDateTimeFromElapsedResult(at[1])));
                        }
                    }
                    else
                    {
                        foreach (float[] at in regions)
                        {
                            t.Add(new ValueRange<DateTime>(
                                tr.getDateTimeFromDistResult(UnitUtil.Distance.ConvertTo(at[0], m_refTrailResult.Activity)),
                                tr.getDateTimeFromDistResult(UnitUtil.Distance.ConvertTo(at[1], m_refTrailResult.Activity))));
                        }
                    }
                    results.Add(new Data.TrailResultMarked(tr, t));
                    this.MainChart.SelectData -= new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataHandler(MainChart_SelectData);
                    const int MaxSelectedSeries = 5;
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
                        m_multiple.SetSelectedResultRange(i, regions);
                    }
                    this.MainChart.SelectData += new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataHandler(MainChart_SelectData);
                }
            }
        }

        public void SetSelectedResultRange(IList<float[]> regions)
        {
            for (int i = 0; i < MainChart.DataSeries.Count; i++ )
            {
                MainChart.DataSeries[i].ClearSelectedRegions();
                //For "single result" only select first series
                if (m_trailResults.Count>1 || i==0)
                //if (MainChart.DataSeries[i].ChartType != ChartDataSeries.Type.Fill)
                {
                    SetSelectedResultRange(i, false, regions);
                }
            }
        }

        public void SetSelectedResultRange(int i, bool clear, IList<float[]> regions)
        {
            if (ShowPage)
            {
                this.MainChart.SelectData -= new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataHandler(MainChart_SelectData);
                if (clear)
                {
                    foreach (ChartDataSeries t in MainChart.DataSeries)
                    {
                        //Note: This is not clearing ranges
                        t.ClearSelectedRegions();
                    }
                }
                if (MainChart.DataSeries != null && MainChart.DataSeries.Count > i)
                {
                    if (!clear)
                    {
                        MainChart.DataSeries[i].ClearSelectedRegions();
                    }
                    if (regions != null && regions.Count > 0)
                    {
                        //foreach (float[] at in regions)
                        //{
                        //    //s.AddSelecedRegion(at[0], at[1]);
                        //}
                        MainChart.DataSeries[i].SetSelectedRange(regions[0][0], regions[regions.Count - 1][1]);
                    }
                }
                this.MainChart.SelectData += new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataHandler(MainChart_SelectData);
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
                for (int i = 0; i < m_trailResults.Count; i++)
                {
                        MainChart.DataSeries[i].ClearSelectedRegions();
                        //MainChart.DataSeries[i].SetSelectedRange(0, 0);
                        //The "fill" chart is 0, line is 1
                        if (i == 0 && m_trailResults.Count == 1 &&
                                    MainChart.DataSeries.Count > 1)
                        {
                             MainChart.DataSeries[1].ClearSelectedRegions();
                        }
                    IList<float[]> l = GetResultSelectionFromActivity(i, sel);
                    if (l != null && l.Count > 0)
                    {
                        //Only one range can be selected
                        float x1 = l[0][0];
                        float x2 = l[l.Count - 1][1];
                        //MainChart.DataSeries[i].ClearSelectedRegions();
                        ////The "fill" chart is 0, line is 1
                        //if (i == 0 && m_trailResults.Count == 1 &&
                        //    MainChart.DataSeries.Count > 1)
                        //{
                        //    MainChart.DataSeries[1].ClearSelectedRegions();
                        //}
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
                            foreach (float[] ax in GetResultSelectionFromActivity(i, trm.selInfo))
                            {
                                //Ignore ranges outside current range and malformed scales
                                if (ax[0] < MainChart.XAxis.MaxOriginFarValue &&
                                    MainChart.XAxis.MinOriginValue > float.MinValue &&
                                    ax[1] > MainChart.XAxis.MinOriginValue &&
                                    MainChart.XAxis.MaxOriginFarValue < float.MaxValue)
                                {
                                    ax[0] = Math.Max(ax[0], (float)MainChart.XAxis.MinOriginValue);
                                    ax[1] = Math.Min(ax[1], (float)MainChart.XAxis.MaxOriginFarValue);
                                    MainChart.DataSeries[i].AddSelecedRegion(ax[0], ax[1]);
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
            x1 = (float)UnitUtil.Distance.ConvertFrom(t1, m_refTrailResult.Activity);
            x2 = (float)UnitUtil.Distance.ConvertFrom(t2, m_refTrailResult.Activity);
            return new float[] { x1, x2 };
        }

        private IList<float[]> GetResultSelectionFromActivity(int i, IItemTrackSelectionInfo sel)
        {
            IList<float[]> result = new List<float[]>();

            TrailResult tr = m_trailResults[i]; 
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
                        //For "single result" only select first series
                        if (i < m_trailResults.Count &&
                            m_trailResults[i].Equals(tr) &&
                            (m_trailResults.Count > 1 || i == 0))
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
            return YAxisReferentials != null && YAxisReferentials.Count>0;
        }
        private IDictionary<LineChartTypes,bool> m_hasValues=null;
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
                if (!(m_trailResults == null || m_refTrailResult == null ||
                    (yaxis == LineChartTypes.DiffTime || yaxis == LineChartTypes.DiffDist) &&
                    m_trailResults.Count == 1 && m_trailResults[0] == m_refTrailResult))
                {
                    for (int i = 0; i < m_trailResults.Count; i++)
                    {
                        TrailResult tr = m_trailResults[i];
                        //As this value is cached, it is no extra to request and drop it
                        INumericTimeDataSeries graphPoints = GetSmoothedActivityTrack(tr, yaxis);

                        if (graphPoints.Count > 1)
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
                foreach (LineChartTypes yaxis in m_YAxisReferentials)
                {

                    for (int i = 0; i < m_trailResults.Count; i++)
                    {
                        TrailResult tr = m_trailResults[i];
                        INumericTimeDataSeries graphPoints = GetSmoothedActivityTrack(tr, yaxis);

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
                            Color chartFillColor = System.Drawing.Color.WhiteSmoke;
                            Color chartLineColor = m_color[yaxis];
                            Color chartSelectedColor = ControlPaint.Dark(m_color[yaxis], 0.01F); ;
                            if (m_trailResults.Count > 1)
                            {
                                chartFillColor = m_trailResults[i].TrailColor;
                                chartLineColor = chartFillColor;
                                chartSelectedColor = chartFillColor;
                            }

                            ChartDataSeries dataFill = null;
                            ChartDataSeries dataLine = new ChartDataSeries(MainChart, m_axis[yaxis]);

                            if (m_trailResults.Count == 1 && m_YAxisReferentials.Count == 1)
                            {
                                dataFill = new ChartDataSeries(MainChart, MainChart.YAxis);
                                MainChart.DataSeries.Add(dataFill);

                                dataFill.ChartType = ChartDataSeries.Type.Fill;
                                dataFill.FillColor = chartFillColor;
                                dataFill.LineColor = chartLineColor;
                                dataFill.SelectedColor = chartSelectedColor;
                                dataFill.LineWidth = 2;
                            }
                            MainChart.DataSeries.Add(dataLine);

                            dataLine.ChartType = ChartDataSeries.Type.Line;
                            dataLine.LineColor = chartLineColor;
                            dataLine.SelectedColor = chartSelectedColor;

                            if (XAxisReferential == XAxisValue.Time)
                            {
                                float oldElapsedSeconds = -1;
                                foreach (ITimeValueEntry<float> entry in graphPoints)
                                {
                                    float value = entry.Value;//ConvertUnit(entry.Value, GenSeriesEntry.LineChartType);
                                    if (oldElapsedSeconds != entry.ElapsedSeconds)
                                    {
                                        if (null != dataFill)
                                        {
                                            dataFill.Points.Add(entry.ElapsedSeconds, new PointF(entry.ElapsedSeconds, value));
                                        }
                                        dataLine.Points.Add(entry.ElapsedSeconds, new PointF(entry.ElapsedSeconds, value));
                                    }
                                    oldElapsedSeconds = entry.ElapsedSeconds;
                                }
                            }
                            else
                            {
                                if (null != m_refTrailResult)
                                {
                                    IDistanceDataTrack distanceTrack = m_trailResults[i].DistanceMetersTrack0(m_refTrailResult);

                                    float oldElapsedSeconds = -1;
                                    foreach (ITimeValueEntry<float> dtEntry in distanceTrack)
                                    {
                                        float elapsedSeconds = dtEntry.ElapsedSeconds;
                                        if (elapsedSeconds <= graphPoints.TotalElapsedSeconds)
                                        {
                                            ITimeValueEntry<float> valueEntry = graphPoints.GetInterpolatedValue(graphPoints.StartTime.AddSeconds(elapsedSeconds));
                                            float value = valueEntry.Value;//ConvertUnit(valueEntry.Value, GenSeriesEntry.LineChartType);
                                            //float distanceValue = ConvertUnit(dtEntry.Value, LineChartTypes.Distance);
                                            float distanceValue = dtEntry.Value;// (float)UnitUtil.Distance.ConvertFrom(dtEntry.Value, m_refTrailResult.Activity);

                                            if (oldElapsedSeconds != elapsedSeconds)
                                            {
                                                if (null != dataFill)
                                                {
                                                    dataFill.Points.Add(elapsedSeconds, new PointF(distanceValue, value));
                                                }
                                                dataLine.Points.Add(elapsedSeconds, new PointF(distanceValue, value));
                                            }
                                            oldElapsedSeconds = elapsedSeconds;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Data.TrailResult trailPointResult = m_refTrailResult;
                //If only one result is used, it can be confusing if the trail points are set for ref
                if (m_trailResults.Count == 1 ||
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
                        new Bitmap(TrailsPlugin.CommonIcons.fileCircle(11, 11));
#endif
                    foreach (DateTime t in trailPointResult.TimeTrailPoints)
                    {
                        AxisMarker a = null;
                        if (XAxisReferential == XAxisValue.Time)
                        {
                            a = new AxisMarker(trailPointResult.getElapsedResult(t), icon);
                        }
                        else
                        {
                            float time = (float)UnitUtil.Distance.ConvertFrom(
                                trailPointResult.getDistResult(t), trailPointResult.Activity);
                            if (!float.IsNaN(time))
                            {
                                a = new AxisMarker(time, icon);
                            }
                        }
                        if (a != null)
                        {
                            a.Line1Style = System.Drawing.Drawing2D.DashStyle.Solid;
                            a.Line1Color = Color.Goldenrod;
                            MainChart.XAxis.Markers.Add(a);
                        }
                    }
                }
                ZoomToData();
            }
		}

        private void SetupAxes()
        {
            if (m_visible)
            {
                IActivity activity = null;
                if (m_refTrailResult != null)
                {
                    activity = m_refTrailResult.Activity;
                }
                // X axis
                switch (XAxisReferential)
                {
                    case XAxisValue.Distance:
                        {
                            MainChart.XAxis.Formatter = new Formatter.General(UnitUtil.Distance.DefaultDecimalPrecision);
                            MainChart.XAxis.Label = CommonResources.Text.LabelDistance +
                                                    UnitUtil.Distance.LabelAbbrAct2(activity);
                            break;
                        }
                    case XAxisValue.Time:
                        {

                            MainChart.XAxis.Formatter = new Formatter.SecondsToTime();
                            MainChart.XAxis.Label = CommonResources.Text.LabelTime;
                            break;
                        }
                    default:
                        {
                            Debug.Assert(false);
                            break;
                        }
                }
                // Y axis
                MainChart.YAxisRight.Clear();
                bool boFirst = true;
                foreach (LineChartTypes yaxis in m_YAxisReferentials)
                {
                    CreateAxis(yaxis, boFirst);
                    boFirst = false;

                }
            }
        }
        
        private void CreateAxis(LineChartTypes axisType, bool left)
        {
            if ((m_trailResults == null || m_refTrailResult == null ||
                (axisType == LineChartTypes.DiffTime || axisType == LineChartTypes.DiffDist) &&
                m_trailResults.Count == 1 && m_trailResults == m_refTrailResult))
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
                axis.LabelColor = m_color[axisType];

                switch (axisType)
                {
                    case LineChartTypes.Cadence:
                        {
                            axis.Formatter = new Formatter.General(UnitUtil.Cadence.DefaultDecimalPrecision);
                            axis.Label = CommonResources.Text.LabelCadence + UnitUtil.Cadence.LabelAbbr2;
                            break;
                        }
                    case LineChartTypes.Grade:
                        {
                            axis.Formatter = new Formatter.Percent();
                            axis.Label = CommonResources.Text.LabelGrade + " (%)";
                            break;
                        }
                    case LineChartTypes.Elevation:
                        {
                            axis.Formatter = new Formatter.General(UnitUtil.Elevation.DefaultDecimalPrecision);
                            axis.Label = CommonResources.Text.LabelElevation + UnitUtil.Elevation.LabelAbbrAct2(m_refTrailResult.Activity);
                            break;
                        }
                    case LineChartTypes.HeartRateBPM:
                    case LineChartTypes.DiffHeartRateBPM:
                        {
                            axis.Formatter = new Formatter.General(UnitUtil.HeartRate.DefaultDecimalPrecision);
                            axis.Label = CommonResources.Text.LabelHeartRate + UnitUtil.HeartRate.LabelAbbr2;
                            break;
                        }
                    case LineChartTypes.HeartRatePercentMax:
                        {
                            axis.Label = CommonResources.Text.LabelHeartRate + " (" +
                                                    CommonResources.Text.LabelPercentOfMax + ")";
                            break;
                        }
                    case LineChartTypes.Power:
                        {
                            axis.Formatter = new Formatter.General(UnitUtil.Power.DefaultDecimalPrecision);
                            axis.Label = CommonResources.Text.LabelPower + UnitUtil.Power.LabelAbbr2;
                            break;
                        }
                    case LineChartTypes.Speed:
                        {
                            axis.Formatter = new Formatter.General(UnitUtil.Speed.DefaultDecimalPrecision);
                            axis.Label = CommonResources.Text.LabelSpeed + UnitUtil.Pace.LabelAbbrAct2(m_refTrailResult.Activity);
                            break;
                        }
                    case LineChartTypes.Pace:
                        {
                            axis.Formatter = new Formatter.SecondsToTime();
                            axis.Label = CommonResources.Text.LabelPace + UnitUtil.Pace.LabelAbbrAct2(m_refTrailResult.Activity);
                            break;
                        }
                    case LineChartTypes.DiffTime:
                    case LineChartTypes.Time:
                        {
                            axis.Formatter = new Formatter.SecondsToTime();
                            axis.Label = CommonResources.Text.LabelTime;
                            break;
                        }
                    case LineChartTypes.DiffDist:
                    case LineChartTypes.Distance:
                        {
                            axis.Formatter = new Formatter.General(UnitUtil.Distance.DefaultDecimalPrecision);
                            axis.Label = CommonResources.Text.LabelDistance + UnitUtil.Distance.LabelAbbrAct2(m_refTrailResult.Activity);
                            break;
                        }
                    default:
                        {
                            Debug.Assert(false);
                            break;
                        }
                }
            }
        }

        private INumericTimeDataSeries GetSmoothedActivityTrack(Data.TrailResult result, LineChartTypes lineChart)
        {
			// Fail safe
			INumericTimeDataSeries track = new NumericTimeDataSeries();

            switch (lineChart)
            {
                case LineChartTypes.Cadence:
                    {
                        track = result.CadencePerMinuteTrack0(m_refTrailResult);
                        break;
                    }
                case LineChartTypes.Elevation:
                    {
                        track = result.ElevationMetersTrack0(m_refTrailResult);
                        break;
                    }
                case LineChartTypes.HeartRateBPM:
                    {
                        track = result.HeartRatePerMinuteTrack0(m_refTrailResult);
                        break;
                    }
                //case LineChartTypes.HeartRatePercentMax:
                //    {
                //        track = result.HeartRatePerMinutePercentMaxTrack;
                //        break;
                //    }
                case LineChartTypes.Power:
                    {
                        track = result.PowerWattsTrack0(m_refTrailResult);
                        break;
                    }
                case LineChartTypes.Grade:
                    {
                        track = result.GradeTrack0(m_refTrailResult);
                        break;
                    }

                case LineChartTypes.Speed:
                    {
                        track = result.SpeedTrack0(m_refTrailResult);
                        break;
                    }

                case LineChartTypes.Pace:
                    {
                        track = result.PaceTrack0(m_refTrailResult);
                        break;
                    }

                case LineChartTypes.DiffTime:
                    {
                        track = result.DiffTimeTrack0(m_refTrailResult);
                        break;
                    }
                case LineChartTypes.DiffDist:
                    {
                        track = result.DiffDistTrack0(m_refTrailResult);
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
        //public Color ChartFillColor
        //{
        //    get { return m_ChartFillColor; }
        //    set {
        //        if (m_ChartFillColor != value) {
        //            m_ChartFillColor = value;

        //            foreach (ChartDataSeries dataSerie in MainChart.DataSeries) {
        //                dataSerie.FillColor = ChartFillColor;
        //            }
        //        }
        //    }
        //}

        //public Color ChartLineColor {
        //    get { return m_ChartLineColor; }
        //    set {
        //        if (ChartLineColor != value) {
        //            m_ChartLineColor = value;

        //            foreach (ChartDataSeries dataSerie in MainChart.DataSeries) {
        //                dataSerie.LineColor = ChartLineColor;
        //            }
        //        }
        //    }
        //}

        //public Color ChartSelectedColor {
        //    get { return m_ChartSelectedColor; }
        //    set {
        //        if (ChartSelectedColor != value) {
        //            m_ChartSelectedColor = value;

        //            foreach (ChartDataSeries dataSerie in MainChart.DataSeries) {
        //                dataSerie.SelectedColor = ChartSelectedColor;
        //            }
        //        }
        //    }
        //}

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
            else if (e.KeyCode == Keys.S)
            {
                selectedTypes = LineChartTypes.Speed;
            }

            IList<LineChartTypes> chartTypes = new List<LineChartTypes> { selectedTypes };
            if (selectedTypes == LineChartTypes.Cadence)
            {
                int val = TrailResult.TrailActivityInfoOptions.CadenceSmoothingSeconds;
                if (reset)
                {
                    val = (new ActivityInfoOptions(true)).CadenceSmoothingSeconds;
                }
                TrailResult.TrailActivityInfoOptions.CadenceSmoothingSeconds =
                    MainChart_KeyDown_Smooth(val, chartTypes, increase, reset, zero);
            }
            else if (selectedTypes == LineChartTypes.Elevation || selectedTypes == LineChartTypes.Grade)
            {
                int val = TrailResult.TrailActivityInfoOptions.ElevationSmoothingSeconds;
                if (reset)
                {
                    val = (new ActivityInfoOptions(true)).ElevationSmoothingSeconds;
                }
                chartTypes.Add(LineChartTypes.Grade);
                TrailResult.TrailActivityInfoOptions.ElevationSmoothingSeconds =
                    MainChart_KeyDown_Smooth(val, chartTypes, increase, reset, zero);
            }
            else if (selectedTypes == LineChartTypes.HeartRateBPM)
            {
                int val = TrailResult.TrailActivityInfoOptions.HeartRateSmoothingSeconds;
                if (reset)
                {
                    val = (new ActivityInfoOptions(true)).HeartRateSmoothingSeconds;
                }
                TrailResult.TrailActivityInfoOptions.HeartRateSmoothingSeconds =
                    MainChart_KeyDown_Smooth(val, chartTypes, increase, reset, zero);
            }
            else if (selectedTypes == LineChartTypes.Power)
            {
                int val = TrailResult.TrailActivityInfoOptions.PowerSmoothingSeconds;
                if (reset)
                {
                    val = (new ActivityInfoOptions(true)).PowerSmoothingSeconds;
                }
                TrailResult.TrailActivityInfoOptions.PowerSmoothingSeconds =
                    MainChart_KeyDown_Smooth(val, chartTypes, increase, reset, zero);
            }
            else if (selectedTypes == LineChartTypes.Speed || selectedTypes == LineChartTypes.Pace)
            {
                int val = TrailResult.TrailActivityInfoOptions.SpeedSmoothingSeconds;
                if (reset)
                {
                    val = (new ActivityInfoOptions(true)).SpeedSmoothingSeconds;
                }
                chartTypes.Add(LineChartTypes.Pace);
                TrailResult.TrailActivityInfoOptions.SpeedSmoothingSeconds =
                    MainChart_KeyDown_Smooth(val, chartTypes, increase, reset, zero);
            }


            foreach (TrailResult t in TrailResults)
            {
                t.Clear(true);
            }
            m_page.RefreshChart();
            foreach (KeyValuePair<LineChartTypes, IAxis> kp in m_axis)
            {
                if (chartTypes.Contains(kp.Key))
                {
                    kp.Value.LabelColor = Color.Black;
                }
                else
                {
                    kp.Value.LabelColor = m_color[kp.Key];
                }
            }
        }

        private LineChartTypes m_lastSelectedType = LineChartTypes.Unknown;
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
