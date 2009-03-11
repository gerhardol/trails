using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ZoneFiveSoftware.Common.Visuals.Chart;

namespace TrailsPlugin.UI.Activity
{
    public partial class ExtendedLineChart : LineChart
    {
        public ExtendedLineChart()
        {
            //InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (PaintActive)
            {
                if (m_LastGraphicsRender == null ||
                    e.ClipRectangle.Width != m_LastGraphicsRender.Width ||
                    e.ClipRectangle.Height != m_LastGraphicsRender.Height)
                {
                    m_LastGraphicsRender = new Bitmap(e.ClipRectangle.Width, e.ClipRectangle.Height);
                }

                base.OnPaint(new PaintEventArgs(Graphics.FromImage(m_LastGraphicsRender), e.ClipRectangle));
            }

            ///Utils.RenderBitmapToGraphics(m_LastGraphicsRender, e.Graphics, e.ClipRectangle);
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (AllowSelection)
            {
                base.OnMouseMove(e);
            }
            else
            {
                // This override will prevent the mouse from changing cursor over
                //  the graph lines but, unfortunately, it disables dragging.  So we
                //  must put that functionnality back in
                Point clientMousePosition = new Point(e.X, e.Y);
                IList<IAxis> axesUnderMouse = GetAxesUnderMouse(clientMousePosition);

                if (!m_MouseDown)
                {
                    // Start by setting the right cursor
                    if (axesUnderMouse.Count > 1)
                    {
                        // We have multiple axes, we're in chart region
                        Cursor = m_OpenHand;
                    }
                    else if (axesUnderMouse.Count == 1)
                    {
                        if (axesUnderMouse[0] == base.XAxis)
                        {
                            Cursor = Cursors.SizeWE;
                        }
                        else
                        {
                            Cursor = Cursors.SizeNS;
                        }
                    }
                }
                else
                {
                    Point mouseMovement = new Point(m_LastMousePosition.X - clientMousePosition.X,
                                                    m_LastMousePosition.Y - clientMousePosition.Y);

                    // If we're dragging, pan the right axes.  It is possible that multiple axes are
                    //  being panned.  In that case, the display was slow.  In order to fix this, we
                    //  disable painting while panning.  Check out OnPaint for the trick.
                    PaintActive = false;
                    foreach (IAxis axis in m_AxesUnderMouseOnDragStart)
                    {
                        axis.ChangeOrigin(mouseMovement);
                    }
                    PaintActive = true;
                }

                m_LastMousePosition = clientMousePosition;
            }
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (AllowSelection)
            {
                base.OnMouseDown(e);
            }
            else
            {
                m_MouseDown = true;
                m_LastMousePosition = new Point(e.X, e.Y);

                // The fisrt thing that is managed is getting the focus and this enables
                //  zooming with the scroll button
                this.Focus();

                if (base.ChartDataRect.Contains(m_LastMousePosition))
                {
                    Cursor = m_ClosedHand;
                }

                m_AxesUnderMouseOnDragStart = GetAxesUnderMouse(m_LastMousePosition);
            }
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (AllowSelection)
            {
                base.OnMouseUp(e);
            }
            else
            {
                m_MouseDown = false;
            }
        }

        private IList<IAxis> GetAxesUnderMouse(Point clientMousePosition)
        {
            List<IAxis> axes = new List<IAxis>();

            if (base.ChartDataRect.Contains(clientMousePosition))
            {
                // Add all since we're in the chart rect
                axes.Add(base.XAxis);
                axes.Add(base.YAxis);
                foreach (IAxis axis in base.YAxisRight)
                {
                    axes.Add(axis);
                }
            }
            else if (base.XAxis.Bounds.Contains(clientMousePosition))
            {
                axes.Add(base.XAxis);
            }
            else if (base.YAxis.Bounds.Contains(clientMousePosition))
            {
                axes.Add(base.YAxis);
            }
            else
            {
                // Check YAxisRight list
                foreach (IAxis axis in base.YAxisRight)
                {
                    if (axis.Bounds.Contains(clientMousePosition))
                    {
                        axes.Add(axis);
                    }
                }
            }

            return axes;
        }

        [DisplayName("Allow Selection")]
        public bool AllowSelection
        {
            get { return m_AllowSelection; }
            set { m_AllowSelection = value; }
        }

        private bool PaintActive
        {
            get { return m_PaintActive; }
            set
            {
                if(PaintActive != value)
                {
                    m_PaintActive = value;

                    if (PaintActive)
                    {
                        Refresh();
                    }
                }
            }
        }

        private bool m_AllowSelection = true;
        private bool m_MouseDown = false;
        private bool m_PaintActive = true;
        private Point m_LastMousePosition;
        Bitmap m_LastGraphicsRender;
        private IList<IAxis> m_AxesUnderMouseOnDragStart;
        ///private static Cursor m_ClosedHand = new Cursor(new MemoryStream(Resources.Resources.ResourceManager.GetObject("ClosedHandCursor") as Byte[]));
        ///private static Cursor m_OpenHand = new Cursor(new MemoryStream(Resources.Resources.ResourceManager.GetObject("OpenHandCursor") as Byte[]));
    }
}

