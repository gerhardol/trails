using System;
using System.Collections.Generic;
using System.Text;

namespace TrailsPlugin.Utils {
	class Constants {
		public static readonly UInt16 SecondsPerMinute = 60;
		public static readonly UInt16 MinutesPerHour = 60;
		public static readonly UInt16 SecondsPerHour = (UInt16)(MinutesPerHour * SecondsPerMinute);
	}
}
