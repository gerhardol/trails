using ZoneFiveSoftware.Common.Data.GPS;
using System.Xml;

namespace TrailsPlugin.Data {
	public class TrailPoint {
		private IGPSLocation m_gpsLocation;
		public TrailPoint(IGPSLocation gpsLocation) {
			m_gpsLocation = gpsLocation;
		}
		public string Latitude {
			get {
				return m_gpsLocation.LatitudeDegrees.ToString();
			}
		}
		public string Longitude {
			get {
				return m_gpsLocation.LongitudeDegrees.ToString();
			}
		}

		public IGPSLocation GPSLocation {
			get {
				return m_gpsLocation;
			}
		}

		static public TrailPoint FromXml(XmlNode node) {

			IGPSLocation gpsLocation = new GPSLocation(
				float.Parse(node.Attributes["latitude"].Value),
				float.Parse(node.Attributes["longitude"].Value)
			);

			return new TrailPoint(gpsLocation);
		}

		public XmlNode ToXml(XmlDocument doc) {
			XmlNode trailPointNode = doc.CreateElement("TrailPoint");
			XmlAttribute a = doc.CreateAttribute("latitude");
			a.Value = this.Latitude;
			trailPointNode.Attributes.Append(a);
			a = doc.CreateAttribute("longitude");
			a.Value = this.Longitude;
			trailPointNode.Attributes.Append(a);
			return trailPointNode;
		}

	}
}
