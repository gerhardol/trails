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
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace TrailsPlugin {
	class CommonIcons {

		static public Image MenuCascadeArrowDown {
			get
            {
#if ST_2_1
				ResourceManager ResourceManager = new ResourceManager("ZoneFiveSoftware.SportTracks.Properties.Resources", System.Reflection.Assembly.GetEntryAssembly());
				return (Bitmap)ResourceManager.GetObject("Image_12_MenuCascadeArrowDown");
#else
                //Not ideal icon....
                return ZoneFiveSoftware.Common.Visuals.CommonResources.Images.MoveDown16;
#endif
            }
		}

		static public Image Edit {
			get {
				return ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Edit16;
			}
		}

		static public Image RedSquare {
			get {
//#if ST_2_1
//                ResourceManager ResourceManager = new ResourceManager("ZoneFiveSoftware.SportTracks.Properties.Resources", System.Reflection.Assembly.GetEntryAssembly());
//                return (Bitmap)ResourceManager.GetObject("Image_16_Marker_RedSquare");
//#else
                return Properties.Resources.square_red;
//#endif
            }
		}

		static public Image GreenSquare {
			get {
//#if ST_2_1
//                ResourceManager ResourceManager = new ResourceManager("ZoneFiveSoftware.SportTracks.Properties.Resources", System.Reflection.Assembly.GetEntryAssembly());
//                return (Bitmap)ResourceManager.GetObject("Image_16_Marker_GreenSquare");
//#else
                return Properties.Resources.square_green;
//#endif
			}
		}

		static public Image BlueSquare {
			get {
//#if ST_2_1
//                ResourceManager ResourceManager = new ResourceManager("ZoneFiveSoftware.SportTracks.Properties.Resources", System.Reflection.Assembly.GetEntryAssembly());
//                return (Bitmap)ResourceManager.GetObject("Image_16_Marker_BlueSquare");
//#else
                return Properties.Resources.square_blue;
//#endif
            }
		}

		static public Image LowerHalf {
			get {
#if ST_2_1
				ResourceManager ResourceManager = new ResourceManager("ZoneFiveSoftware.SportTracks.Properties.Resources", System.Reflection.Assembly.GetEntryAssembly());
				return (Bitmap)ResourceManager.GetObject("Image_16_View_2Pane_LowerHalf");
#else
                return ZoneFiveSoftware.Common.Visuals.CommonResources.Images.View2PaneLowerHalf16;
#endif
			}
		}

		static public Image LowerLeft {
			get {
#if ST_2_1
				ResourceManager ResourceManager = new ResourceManager("ZoneFiveSoftware.SportTracks.Properties.Resources", System.Reflection.Assembly.GetEntryAssembly());
				return (Bitmap)ResourceManager.GetObject("Image_16_View_3Pane_LowerLeft");
#else
                return ZoneFiveSoftware.Common.Visuals.CommonResources.Images.View3PaneLowerLeft16;
#endif
            }
		}

		static public Image Add {
			get {
				return ZoneFiveSoftware.Common.Visuals.CommonResources.Images.DocumentAdd16;
			}
		}

		static public Image Delete {
			get {
				return ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Delete16;				
			}
		}

		static public Image Save {
			get {
				return ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Save16;
			}
		}

		static public Image ListSettings {
			get {
				return ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Table16;
			}
		}
	}
}
