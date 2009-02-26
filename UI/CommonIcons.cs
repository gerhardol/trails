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

	}
}
