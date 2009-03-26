/******************************************************************************

    This file is part of TrailsPlugin.

    TrailsPlugin is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TrailsPlugin is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TrailsPlugin.  If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

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

		static public Bitmap MenuCascadeArrowDown {
			get {
				ResourceManager ResourceManager = new ResourceManager("ZoneFiveSoftware.SportTracks.Properties.Resources", System.Reflection.Assembly.GetEntryAssembly());
				return (Bitmap)ResourceManager.GetObject("Image_12_MenuCascadeArrowDown");
			}
		}

		static public Image Edit {
			get {
				return ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Edit16;
			}
		}

		static public Image RedSquare {
			get {
				ResourceManager ResourceManager = new ResourceManager("ZoneFiveSoftware.SportTracks.Properties.Resources", System.Reflection.Assembly.GetEntryAssembly());
				return (Bitmap)ResourceManager.GetObject("Image_16_Marker_RedSquare");
			}
		}

		static public Image GreenSquare {
			get {
				ResourceManager ResourceManager = new ResourceManager("ZoneFiveSoftware.SportTracks.Properties.Resources", System.Reflection.Assembly.GetEntryAssembly());
				return (Bitmap)ResourceManager.GetObject("Image_16_Marker_GreenSquare");
			}
		}

		static public Image BlueSquare {
			get {
				ResourceManager ResourceManager = new ResourceManager("ZoneFiveSoftware.SportTracks.Properties.Resources", System.Reflection.Assembly.GetEntryAssembly());
				return (Bitmap)ResourceManager.GetObject("Image_16_Marker_BlueSquare");
			}
		}

		static public Image LowerHalf {
			get {
				ResourceManager ResourceManager = new ResourceManager("ZoneFiveSoftware.SportTracks.Properties.Resources", System.Reflection.Assembly.GetEntryAssembly());
				return (Bitmap)ResourceManager.GetObject("Image_16_View_2Pane_LowerHalf");
			}
		}

		static public Image LowerLeft {
			get {
				ResourceManager ResourceManager = new ResourceManager("ZoneFiveSoftware.SportTracks.Properties.Resources", System.Reflection.Assembly.GetEntryAssembly());
				return (Bitmap)ResourceManager.GetObject("Image_16_View_3Pane_LowerLeft");
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
