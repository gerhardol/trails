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
