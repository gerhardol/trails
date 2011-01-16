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

namespace TrailsPlugin.UI.Activity {
	public partial class TrailLineChart : UserControl {
        private Data.TrailResult m_refTrailResult = null;
        private IList<Data.TrailResult> m_trailResults = new List<Data.TrailResult>();
        private XAxisValue m_XAxisReferential = XAxisValue.Time;
        private LineChartTypes m_YAxisReferential = LineChartTypes.Speed;
        //private IList<LineChartTypes> m_YAxisReferential_right = null;
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
            DiffDist
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
                case LineChartTypes.DiffTime:
                    {
                        yAxisLabel = CommonResources.Text.LabelTime;
                        break;
                    }
                case LineChartTypes.DiffDist:
                    {
                        yAxisLabel = CommonResources.Text.LabelDistance;
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
            MainChart.AutozoomToData(true);
			MainChart.Refresh();
		}

        void copyChartMenuItem_Click(object sender, EventArgs e)
        {
            //Not visible menu item
            //MainChart.CopyTextToClipboard(true, System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator);
        }

        void MainChart_SelectData(object sender, ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataEventArgs e)
        {
            if (e != null && e.DataSeries != null && m_page != null)
            {
                //Get index for dataseries - same as for result
                int i = -1;
                if (MainChart.DataSeries.Count==2 &&
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
                if(i>=0)
                {
                    TrailResult tr = m_trailResults[i];
                    IList<float[]> regions;
                    e.DataSeries.GetSelectedRegions(out regions);

                    IList<Data.TrailResultMarked> results = new List<Data.TrailResultMarked>();
                    if (XAxisReferential == XAxisValue.Time)
                    {
                        IValueRangeSeries<DateTime> t = new ValueRangeSeries<DateTime>();
                        foreach (float[] at in regions)
                        {
                            t.Add(new ValueRange<DateTime>(
                                tr.getActivityTime(at[0]),
                                tr.getActivityTime(at[1])));
                        }
                        results.Add(new Data.TrailResultMarked(tr, t));
                    }
                    else
                    {
                        IValueRangeSeries<double> t = new ValueRangeSeries<double>();
                        foreach (float[] at in regions)
                        {
                            t.Add(new ValueRange<double>(
                                tr.getActivityDist(Utils.Units.SetDistance(at[0], m_refTrailResult.Activity)),
                                tr.getActivityDist(Utils.Units.SetDistance(at[1], m_refTrailResult.Activity))));
                        }
                        results.Add(new Data.TrailResultMarked(tr, t));
                    }
                    this.MainChart.SelectData -= new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataHandler(MainChart_SelectData);
                    const int MaxSelectedSeries = 5;
                    bool markAll=(MainChart.DataSeries.Count <= MaxSelectedSeries);
                    //Mark route track, but not chart
                    m_page.MarkTrack(results, false);
                    m_page.EnsureVisible(new List<Data.TrailResult>{tr}, false);

                    if (markAll)
                    {
                        m_multiple.SetSelectedRange(regions);
                    }
                    else
                    {
                        //Assumes that not single results are set
                        m_multiple.SetSelectedRange(i, regions);
                    }
                    this.MainChart.SelectData += new ZoneFiveSoftware.Common.Visuals.Chart.ChartBase.SelectDataHandler(MainChart_SelectData);
                }
            }
        }

        public void SetSelectedRange(IList<float[]> regions)
        {
            for (int i = 0; i < MainChart.DataSeries.Count; i++ )
            {
                MainChart.DataSeries[i].ClearSelectedRegions();
                //For "single result" only select first series
                if (m_trailResults.Count>1 || i==0)
                //if (MainChart.DataSeries[i].ChartType != ChartDataSeries.Type.Fill)
                {
                    SetSelectedRange(i, false, regions);
                }
            }
        }

        public void SetSelectedRange(int i, bool clear, IList<float[]> regions)
        {
            if (m_visible)
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
            if (MainChart != null && MainChart.DataSeries != null &&
                    MainChart.DataSeries.Count > 0 &&
                m_trailResults.Count > 0 && ShowPage)
            {
                //This is used in single activity mode, when selected on the route
                Data.TrailsItemTrackSelectionInfo sel = new Data.TrailsItemTrackSelectionInfo();
                foreach (IItemTrackSelectionInfo trm in asel)
                {
                    sel.Union(trm);
                }

                //Set the matching time distance for the activity
                for (int i = 0; i < m_trailResults.Count; i++)
                {
                        MainChart.DataSeries[i].ClearSelectedRegions();
                        //The "fill" chart is 0, line is 1
                        if (i == 0 && m_trailResults.Count == 1 &&
                                    MainChart.DataSeries.Count > 1)
                        {
                             MainChart.DataSeries[1].ClearSelectedRegions();
                        }
                    IList<float[]> l = GetSelection(i, sel);
                    if (l != null && l.Count > 0)
                    {
                        //Only one range can be selected
                        float x1 = l[0][0];
                        float x2 = l[l.Count - 1][1];
                        MainChart.DataSeries[i].ClearSelectedRegions();
                        //The "fill" chart is 0, line is 1
                        if (i == 0 && m_trailResults.Count == 1 &&
                            MainChart.DataSeries.Count > 1)
                        {
                            MainChart.DataSeries[1].ClearSelectedRegions();
                        }
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
            if (MainChart != null && MainChart.DataSeries != null &&
                    MainChart.DataSeries.Count > 0 &&
                m_trailResults.Count > 0 && ShowPage)
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
                            foreach (float[] ax in GetSelection(i, trm.selInfo))
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
            double t1 = 0, t2 = 0;
            DateTime d1 = DateTime.MinValue, d2 = DateTime.MinValue;
            d1 = tr.getTimeFromActivity(v.Lower);
            d2 = tr.getTimeFromActivity(v.Upper);
            if (XAxisReferential != XAxisValue.Time)
            {
                t1 = tr.getDistFromActivity(d1);
                t2 = tr.getDistFromActivity(d2);
            }
            return GetSingleSelection(tr, t1, t2, d1, d2);
        }
        float[] GetSingleSelection(TrailResult tr, IValueRange<double> v)
        {
            double t1 = 0, t2 = 0;
            DateTime d1 = DateTime.MinValue, d2 = DateTime.MinValue;
            t1 = tr.getDistFromActivity(v.Lower);
            t2 = tr.getDistFromActivity(v.Upper);
            if (XAxisReferential == XAxisValue.Time)
            {
                d1 = tr.getTimeFromActivity(t1);
                d2 = tr.getTimeFromActivity(t2);
            }
            return GetSingleSelection(tr, t1, t2, d1, d2);
        }
        float[] GetSingleSelection(TrailResult tr, double t1, double t2, DateTime d1, DateTime d2)
        {
            float x1 = float.MaxValue, x2 = float.MinValue;
            //Convert to distance display unit, Time is always in seconds
            if (XAxisReferential == XAxisValue.Time)
            {
                x1 = (float)(tr.getSeconds(d1));
                x2 = (float)(tr.getSeconds(d2));
            }
            else
            {
                x1 = Utils.Units.GetDistance(t1, m_refTrailResult.Activity);
                x2 = Utils.Units.GetDistance(t2, m_refTrailResult.Activity);
            }
            return new float[] { x1, x2 };
        }

        private IList<float[]> GetSelection(int i, IItemTrackSelectionInfo sel)
        {
            IList<float[]> result = new List<float[]>();

            TrailResult tr = m_trailResults[i]; 
            //Currently only one range but several regions can be selected
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
        private bool? hasValues=null;
        public bool HasValues()
        {
            if (hasValues == null)
            {
                foreach (ChartDataSeries t in MainChart.DataSeries)
                {
                    foreach (KeyValuePair<float, PointF> v in t.Points)
                    {
                        if (v.Value.Y != 0)
                        {
                            hasValues = true;
                            return true;
                        }
                    }
                }
                hasValues = false;
            }
            return (bool)hasValues;
        }

        private void SetupDataSeries()
        {
			MainChart.DataSeries.Clear();
            MainChart.XAxis.Markers.Clear();
            if (m_visible)
            {
                hasValues = null;

                // Add main data. We must use 2 separate data series to overcome the display
                //  bug in fill mode.  The main data series is normally rendered but the copy
                //  is set in Line mode to be displayed over the fill

                for (int i = 0; i < m_trailResults.Count; i++)
                {
                    TrailResult tr = m_trailResults[i];
                    INumericTimeDataSeries graphPoints = GetSmoothedActivityTrack(tr);

                    if (graphPoints.Count <= 1)
                    {
                        if (m_trailResults.Count > 1)
                        {
                            //Add empty, Dataseries index must match results 
                            MainChart.DataSeries.Add(new ChartDataSeries(MainChart, MainChart.YAxis));
                        }
                    }
                    else
                    {
                        Color chartFillColor = ChartFillColor;
                        Color chartLineColor = ChartLineColor;
                        Color chartSelectedColor = ChartSelectedColor;
                        if (m_trailResults.Count > 1)
                        {
                            chartFillColor = m_trailResults[i].TrailColor;
                            chartLineColor = chartFillColor;
                            chartSelectedColor = chartFillColor;
                        }

                        ChartDataSeries dataFill = null;
                        ChartDataSeries dataLine = new ChartDataSeries(MainChart, MainChart.YAxis);

                        if (m_trailResults.Count == 1)
                        {
                            dataFill = new ChartDataSeries(MainChart, MainChart.YAxis);
                            MainChart.DataSeries.Add(dataFill);

                            dataFill.ChartType = ChartDataSeries.Type.Fill;
                            dataFill.FillColor = chartFillColor;
                            dataFill.LineColor = chartLineColor;
                            dataFill.SelectedColor = chartSelectedColor;
                            dataFill.LineWidth = 2;

                            MainChart.XAxis.Markers.Clear();
                        }
                        MainChart.DataSeries.Add(dataLine);

                        dataLine.ChartType = ChartDataSeries.Type.Line;
                        dataLine.LineColor = chartLineColor;
                        dataLine.SelectedColor = chartSelectedColor;

                        if (XAxisReferential == XAxisValue.Time)
                        {
                            foreach (ITimeValueEntry<float> entry in graphPoints)
                            {
                                if (null != dataFill)
                                {
                                    dataFill.Points.Add(entry.ElapsedSeconds, new PointF(entry.ElapsedSeconds, entry.Value));
                                }
                                dataLine.Points.Add(entry.ElapsedSeconds, new PointF(entry.ElapsedSeconds, entry.Value));
                            }
                        }
                        else
                        {
                            IDistanceDataTrack distanceTrack = m_trailResults[i].DistanceMetersTrack;

                            //Debug.Assert(distanceTrack.Count == graphPoints.Count);
                            for (int j = 0; j < distanceTrack.Count; ++j)
                            {
                                float distanceValue = Utils.Units.GetDistance(distanceTrack[j].Value, m_refTrailResult.Activity);
                                if (j < graphPoints.Count)
                                {
                                    ITimeValueEntry<float> entry = graphPoints[j];

                                    ///Debug.Assert(distanceTrack[j].ElapsedSeconds == entry.ElapsedSeconds);
                                    if (null != dataFill)
                                    {
                                        dataFill.Points.Add(entry.ElapsedSeconds, new PointF(distanceValue, entry.Value));
                                    }
                                    dataLine.Points.Add(entry.ElapsedSeconds, new PointF(distanceValue, entry.Value));
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
                    if (XAxisReferential == XAxisValue.Time)
                    {
                        foreach (DateTime t in trailPointResult.TimeTrailPoints)
                        {
                            AxisMarker a = new AxisMarker(trailPointResult.getSeconds(t), icon);
                            a.Line1Style = System.Drawing.Drawing2D.DashStyle.Solid;
                            a.Line1Color = Color.Black;
                            MainChart.XAxis.Markers.Add(a);
                        }
                    }
                    else
                    {
                        foreach (double t in trailPointResult.DistanceTrailPoints)
                        {
                            AxisMarker a = new AxisMarker(Utils.Units.GetDistance(t, trailPointResult.Activity), icon);
                            a.Line1Style = System.Drawing.Drawing2D.DashStyle.Solid;
                            a.Line1Color = Color.Black;
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
                            MainChart.XAxis.Formatter = new Formatter.General();
                            MainChart.XAxis.Label = CommonResources.Text.LabelDistance + " (" +
                                                    Utils.Units.GetDistanceLabel(activity) + ")";
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
                MainChart.YAxis.Formatter = new Formatter.General();
                switch (YAxisReferential)
                {
                    case LineChartTypes.Cadence:
                        {
                            MainChart.YAxis.Label = CommonResources.Text.LabelCadence + " (" +
                                                    CommonResources.Text.LabelRPM + ")";
                            break;
                        }
                    case LineChartTypes.Grade:
                        {
                            MainChart.YAxis.Formatter = new Percent100();
                            MainChart.YAxis.Label = CommonResources.Text.LabelGrade + " (%)";
                            break;
                        }
                    case LineChartTypes.Elevation:
                        {
                            MainChart.YAxis.Label = CommonResources.Text.LabelElevation + " (" +
                                                       Utils.Units.GetElevationLabel(activity) + ")";
                            break;
                        }
                    case LineChartTypes.HeartRateBPM:
                        {
                            MainChart.YAxis.Label = CommonResources.Text.LabelHeartRate + " (" +
                                                    CommonResources.Text.LabelBPM + ")";
                            break;
                        }
                    case LineChartTypes.HeartRatePercentMax:
                        {
                            MainChart.YAxis.Label = CommonResources.Text.LabelHeartRate + " (" +
                                                    CommonResources.Text.LabelPercentOfMax + ")";
                            break;
                        }
                    case LineChartTypes.Power:
                        {
                            MainChart.YAxis.Label = CommonResources.Text.LabelPower + " (" +
                                                    CommonResources.Text.LabelWatts + ")";
                            break;
                        }
                    case LineChartTypes.Speed:
                        {
                            MainChart.YAxis.Label = CommonResources.Text.LabelSpeed + " (" +
                                                    Utils.Units.GetSpeedLabel(activity) + ")";
                            break;
                        }
                    case LineChartTypes.Pace:
                        {
                            MainChart.YAxis.Formatter = new Formatter.SecondsToTime();
                            MainChart.YAxis.Label = CommonResources.Text.LabelPace + " (" +
                                                    Utils.Units.GetPaceLabel(activity) + ")";
                            break;
                        }
                    case LineChartTypes.DiffTime:
                        {
                            MainChart.YAxis.Formatter = new Formatter.SecondsToTime();
                            MainChart.YAxis.Label = CommonResources.Text.LabelTime;
                            break;
                        }
                    case LineChartTypes.DiffDist:
                        {

                            MainChart.YAxis.Formatter = new Formatter.General();
                            MainChart.YAxis.Label = CommonResources.Text.LabelDistance + " (" +
                                                    Utils.Units.GetDistanceLabel(activity) + ")";
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

		private INumericTimeDataSeries GetSmoothedActivityTrack(Data.TrailResult result) {
			// Fail safe
			INumericTimeDataSeries track = new NumericTimeDataSeries();

			switch (YAxisReferential) {
				case LineChartTypes.Cadence: {
						track = result.CadencePerMinuteTrack;
						break;
					}
				case LineChartTypes.Elevation: {
						INumericTimeDataSeries tempResult = result.ElevationMetersTrack;

						// Value is in meters so convert to the right unit
						track = new NumericTimeDataSeries();
						foreach (ITimeValueEntry<float> entry in tempResult) {
                            float temp = Utils.Units.GetElevation(entry.Value, m_refTrailResult.Activity); 

							track.Add(tempResult.EntryDateTime(entry), (float)temp);
						}
						break;
					}
				case LineChartTypes.HeartRateBPM: {
						track = result.HeartRatePerMinuteTrack;
						break;
					}
				/*
								case LineChartTypes.HeartRatePercentMax: {
										track = new NumericTimeDataSeries();

										IAthleteInfoEntry lastAthleteEntry = PluginMain.GetApplication().Logbook.Athlete.InfoEntries.LastEntryAsOfDate(Activity.StartTime);

										// Value is in BPM so convert to the % max HR if we have the info
										if (!float.IsNaN(lastAthleteEntry.MaximumHeartRatePerMinute)) {
											INumericTimeDataSeries tempResult = activityInfo.SmoothedHeartRateTrack;

											foreach (ITimeValueEntry<float> entry in tempResult) {
												double temp = (entry.Value / lastAthleteEntry.MaximumHeartRatePerMinute) * 100;

												track.Add(tempResult.EntryDateTime(entry), (float)temp);
											}
										}
										break;
									}
				*/
				case LineChartTypes.Power: {
						track = result.PowerWattsTrack;
						break;
					}
				case LineChartTypes.Grade: {
						track = result.GradeTrack;
						break;
					}

				case LineChartTypes.Speed: {
						INumericTimeDataSeries tempResult = result.SpeedTrack;

						track = new NumericTimeDataSeries();
						foreach (ITimeValueEntry<float> entry in tempResult) {
							track.Add(tempResult.EntryDateTime(entry), entry.Value);
						}
						break;
					}

                case LineChartTypes.Pace:
                    {
                        INumericTimeDataSeries tempResult = result.PaceTrack;

                        track = new NumericTimeDataSeries();
                        foreach (ITimeValueEntry<float> entry in tempResult)
                        {
                            track.Add(tempResult.EntryDateTime(entry), entry.Value);
                        }
                        break;
                    }

                case LineChartTypes.DiffTime:
                    {
                        INumericTimeDataSeries tempResult = result.DiffTimeTrack(m_refTrailResult);

                        track = new NumericTimeDataSeries();
                        foreach (ITimeValueEntry<float> entry in tempResult)
                        {
                            track.Add(tempResult.EntryDateTime(entry), entry.Value);
                        }
                        break;
                    }
                case LineChartTypes.DiffDist:
                    {
                        INumericTimeDataSeries tempResult = result.DiffDistTrack(m_refTrailResult);

                        track = new NumericTimeDataSeries();
                        foreach (ITimeValueEntry<float> entry in tempResult)
                        {
                            track.Add(tempResult.EntryDateTime(entry), Utils.Units.GetDistance(entry.Value,m_refTrailResult.Activity));
                        }
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
            get { return m_YAxisReferential; }
            set
            {
                m_YAxisReferential = value;
            }
        }

        //[DisplayName("Y Axis value, right")]
        //public IList<LineChartTypes> YAxisReferential_right
        //{
        //    get { return m_YAxisReferential_right; }
        //    set
        //    {
        //        m_YAxisReferential_right = value;
        //    }
        //}

        public Color ChartFillColor
        {
			get { return m_ChartFillColor; }
			set {
				if (m_ChartFillColor != value) {
					m_ChartFillColor = value;

					foreach (ChartDataSeries dataSerie in MainChart.DataSeries) {
						dataSerie.FillColor = ChartFillColor;
					}
				}
			}
		}

		public Color ChartLineColor {
			get { return m_ChartLineColor; }
			set {
				if (ChartLineColor != value) {
					m_ChartLineColor = value;

					foreach (ChartDataSeries dataSerie in MainChart.DataSeries) {
						dataSerie.LineColor = ChartLineColor;
					}
				}
			}
		}

		public Color ChartSelectedColor {
			get { return m_ChartSelectedColor; }
			set {
				if (ChartSelectedColor != value) {
					m_ChartSelectedColor = value;

					foreach (ChartDataSeries dataSerie in MainChart.DataSeries) {
						dataSerie.SelectedColor = ChartSelectedColor;
					}
				}
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

		public bool BeginUpdate() {
			return MainChart.BeginUpdate();
		}

		public void EndUpdate() {
			MainChart.EndUpdate();
		}
	}
}
