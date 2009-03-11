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

using ZoneFiveSoftware.Common.Data.GPS;
using System.Xml;

namespace TrailsPlugin.Data {
	public class TrailGPSLocation : GPSLocation {
		public TrailGPSLocation(float latitudeDegrees, float longitudeDegrees) : base(latitudeDegrees, longitudeDegrees) { }
	
		static public TrailGPSLocation FromXml(XmlNode node, XmlNamespaceManager nsmgr) {

			return new TrailGPSLocation(
				float.Parse(node.Attributes["latitude"].Value), 
				float.Parse(node.Attributes["longitude"].Value)
			);
		}

		public XmlNode ToXml(XmlDocument doc) {
			XmlNode TrailGPSLocationNode = doc.CreateElement("TrailGPSLocation");
			XmlAttribute a = doc.CreateAttribute("latitude");
			a.Value = this.LatitudeDegrees.ToString();
			TrailGPSLocationNode.Attributes.Append(a);
			a = doc.CreateAttribute("longitude");
			a.Value = this.LongitudeDegrees.ToString();
			TrailGPSLocationNode.Attributes.Append(a);
			return TrailGPSLocationNode;
		}

		public float DistanceMetersToPoint(IGPSPoint point) {
			GPSPoint thisPoint = new GPSPoint(
				this.LatitudeDegrees,
				this.LongitudeDegrees,
				0);
			return point.DistanceMetersToPoint(thisPoint);
		}

	}
}
