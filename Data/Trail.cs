using System.Collections.Generic;
using ZoneFiveSoftware.Common.Data.GPS;
using System.Xml;

namespace TrailsPlugin.Data {
	public class Trail {
		public string name;
		public IList<TrailPoint> points = new List<TrailPoint>();
		static public Trail FromXml(XmlNode node) {
			Trail trail = new Trail();
			trail.name = node.Attributes["name"].Value;
			trail.points.Clear();
			foreach(XmlNode trailPointNode in node.ChildNodes) {
				trail.points.Add(TrailPoint.FromXml(trailPointNode));
			}
			return trail;
		}

		public XmlNode ToXml(XmlDocument doc) {
			XmlNode trailNode = doc.CreateElement("Trail");
			XmlAttribute a = doc.CreateAttribute("name");
			a.Value = this.name;			
			trailNode.Attributes.Append(a);
			foreach (TrailPoint point in this.points) {
				trailNode.AppendChild(point.ToXml(doc));
			}
			return trailNode;
		}
	}
}
