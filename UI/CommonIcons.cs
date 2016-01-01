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
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using System.IO;
using System.Collections.Generic;

//Used in both Trails and Matrix plugin, slightly reduced and modified in Matrix

namespace TrailsPlugin
{
    class CommonIcons {

        static public Image MenuCascadeArrowDown
        {
            get
            {
                return Properties.Resources.DropDown;
            }
        }

        static public Image Edit
        {
            get
            {
                return ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Edit16;
            }
        }

        static public Image RedSquare
        {
            get
            {
                return Properties.Resources.square_red;
            }
        }

        static public Image GreenSquare
        {
            get
            {
                return Properties.Resources.square_green;
            }
        }

        static public Image BlueSquare
        {
            get
            {
                return Properties.Resources.square_blue;
            }
        }

        static public Image LowerHalf
        {
            get
            {
#if ST_2_1
                ResourceManager ResourceManager = new ResourceManager("ZoneFiveSoftware.SportTracks.Properties.Resources", System.Reflection.Assembly.GetEntryAssembly());
                return (Bitmap)ResourceManager.GetObject("Image_16_View_2Pane_LowerHalf");
#else
                return ZoneFiveSoftware.Common.Visuals.CommonResources.Images.View2PaneLowerHalf16;
#endif
            }
        }

        static public Image LowerLeft
        {
            get
            {
#if ST_2_1
                ResourceManager ResourceManager = new ResourceManager("ZoneFiveSoftware.SportTracks.Properties.Resources", System.Reflection.Assembly.GetEntryAssembly());
                return (Bitmap)ResourceManager.GetObject("Image_16_View_3Pane_LowerLeft");
#else
                return ZoneFiveSoftware.Common.Visuals.CommonResources.Images.View3PaneLowerLeft16;
#endif
            }
        }

        static public Image Add
        {
            get
            {
                return ZoneFiveSoftware.Common.Visuals.CommonResources.Images.DocumentAdd16;
            }
        }

        static public Image Delete
        {
            get
            {
                return ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Delete16;
            }
        }

        static public Image Save
        {
            get
            {
                return ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Save16;
            }
        }

        static public Image ListSettings
        {
            get
            {
                return ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Table16;
            }
        }

        private enum IconType { Circle, Rhombus };

        static private string fileImage(int sizeX, int sizeY, int brushSize, Color color, bool centerPoint, IconType iconType, out Size iconSize)
        {
            string basePath = Plugin.GetApplication().
#if ST_2_1
                SystemPreferences.WebFilesFolder +
#else
                Configuration.CommonWebFilesFolder +
#endif
                                  System.IO.Path.DirectorySeparatorChar +
                                  TrailsPlugin.GUIDs.PluginMain.ToString() + System.IO.Path.DirectorySeparatorChar;
            if (!Directory.Exists(basePath))
            {

                DirectoryInfo di = Directory.CreateDirectory(basePath);
            }

            //TODO: get largest meaningful icon somehow, to avoid crashes. Use fixed size for now, using minimal marker
            const int maxCircle = 999;
            if (sizeX > maxCircle || sizeY > maxCircle)
            {
                sizeX = 1;
                sizeY = 1;
            }
            sizeX = Math.Max(sizeX, brushSize * 2 - 1);
            sizeY = Math.Max(sizeY, brushSize * 2 - 1);

            //As the image is anchored in the middle, make size odd so (size/2) give center point
            if (1 != sizeX % 2) { sizeX++; }
            if (1 != sizeY % 2) { sizeY++; }
            iconSize = new Size(sizeX, sizeY);
            string filePath = basePath + iconType + "-" + color + sizeX + "_" + sizeY;
            if (centerPoint)
            {
                filePath += "_center";
            }
            filePath += ".png";
            if (!File.Exists(filePath))
            {
                //No version handling other than filename
                Bitmap myBitmap = new Bitmap(sizeX, sizeY);
                Graphics myGraphics = Graphics.FromImage(myBitmap);
                myGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                if (iconType == IconType.Circle)
                {
                    myGraphics.DrawEllipse(new Pen(color, brushSize), new Rectangle(brushSize / 2, brushSize / 2,
                        myBitmap.Width - brushSize, myBitmap.Height - brushSize));
                    myGraphics.DrawEllipse(new Pen(Color.Black, 1), new Rectangle(1, 1,
                        myBitmap.Width - 2, myBitmap.Height - 2));
                    if (centerPoint)
                    {
                        const int midSize = 2;
                        myGraphics.DrawEllipse(new Pen(color, midSize), new Rectangle(sizeX / 2 - midSize, sizeY / 2 - midSize, 1 + midSize * 2, 1 + midSize * 2));
                    }
                }
                else
                {
                    Point[] points = new Point[]{ new Point(sizeX / 2, brushSize/2), new Point(sizeX-brushSize/2, sizeY / 2),
                    new Point(sizeX / 2, sizeY-brushSize/2) , new Point(brushSize/2, sizeY / 2), new Point(sizeX / 2, brushSize/2) };
                    myGraphics.DrawLines(new Pen(color, brushSize), points);
                    points = new Point[]{ new Point(sizeX / 2, 1), new Point(sizeX-1, sizeY / 2),
                    new Point(sizeX / 2, sizeY-1) , new Point(1, sizeY / 2), new Point(sizeX / 2, 1) };
                    myGraphics.DrawLines(new Pen(Color.Black, 1), points);
                }
                FileStream myFileOut = new FileStream(filePath, FileMode.Create);
                myBitmap.Save(myFileOut, ImageFormat.Png);
                myFileOut.Close();
            }
            return filePath;
        }

        //The outer radius defines the included area
        static public string fileCircle(int sizeX, int sizeY, Color color)
        {
            Size iconSize;
            const int brushSize = 6; //Even
            return fileImage(sizeX, sizeY, brushSize, color, false, IconType.Circle, out iconSize);
        }

        static public string Circle(int sizeX, int sizeY, bool centerPoint, out Size iconSize)
        {
            const int brushSize = 6; //Even
            return "file://" + fileImage(sizeX, sizeY, brushSize, Color.Red, centerPoint, IconType.Circle, out iconSize);
        }

        static public string Rhombus(int sizeX, int sizeY, Color c, out Size iconSize)
        {
            const int brushSize = 4;
            return "file://" + fileImage(sizeX, sizeY, brushSize, c, false, IconType.Rhombus, out iconSize);
        }

        private static IDictionary<string, Image> resultColorCache = new Dictionary<string, Image>();
        public static void ResultColorCacheClean()
        {
            resultColorCache.Clear();
        }
        public static Image ResultColorCache(int width, int height, Color col)
        {
            if (resultColorCache.Count > 16*3*2)
            {
                //Do not let cache grow infinetely. It should be sufficient with 16 standard colors for 3 levels
                //More can be added when changing col sizes or using custom colors
                ResultColorCacheClean();
            }

            string key = width + "_" + height + "_" + col;
            if (!resultColorCache.ContainsKey(key))
            {
                Bitmap image = new Bitmap(width, height);
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        //SetPixel is slow, but image is small and this is a cache
                        image.SetPixel(x, y, col);
                    }
                }
                resultColorCache[key] = image;
            }
            return resultColorCache[key];
        }
    }
}
