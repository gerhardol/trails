using System.Collections.Generic;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using System.Xml;

namespace TrailsPlugin.Data {
	public class Trail {
		public string Name;
		private IList<TrailGPSLocation> m_trailLocations = new List<TrailGPSLocation>();

		public IList<TrailGPSLocation> TrailLocations {
			get {
				return m_trailLocations;
			}
		}

		static public Trail FromXml(XmlNode node, XmlNamespaceManager nsmgr) {
			Trail trail = new Trail();
			trail.Name = node.Attributes["name"].Value;
			trail.TrailLocations.Clear();
			foreach (XmlNode TrailGPSLocationNode in node.SelectNodes("ns:TrailGPSLocation", nsmgr)) {
				trail.TrailLocations.Add(TrailGPSLocation.FromXml(TrailGPSLocationNode, nsmgr));
			}
			return trail;
		}

		public XmlNode ToXml(XmlDocument doc) {
			XmlNode trailNode = doc.CreateElement("Trail");
			XmlAttribute a = doc.CreateAttribute("name");
			a.Value = this.Name;
			trailNode.Attributes.Append(a);
			foreach (TrailGPSLocation point in this.TrailLocations) {
				trailNode.AppendChild(point.ToXml(doc));
			}
			return trailNode;
		}

		public bool IsInBounds(IGPSBounds gpsBounds) {
			foreach (TrailGPSLocation trailGPSLocation in this.TrailLocations) {
				if (!gpsBounds.Contains(trailGPSLocation)) {
					return false;
				}
			}
			return true;
		}

		public IList<TrailResult> Results(IActivity activity) {
			IList<TrailResult> resultsList = new List<TrailResult>();
			if (activity.GPSRoute == null || activity.GPSRoute.Count == 0) {
				return resultsList ;
			}			

			float radius = 45;
			int trailIndex = 0;
			int startIndex = -1, endIndex = -1;
			
			for (int routeIndex = 0; routeIndex < activity.GPSRoute.Count; routeIndex++) {
				IGPSPoint routePoint = activity.GPSRoute[routeIndex].Value;
				if (trailIndex != 0) {
					float distFromStartToPoint = this.TrailLocations[0].DistanceMetersToPoint(routePoint);
					if (distFromStartToPoint < radius) {
						trailIndex = 0;
					}
				}
				float distToPoint = this.TrailLocations[trailIndex].DistanceMetersToPoint(routePoint);
				if (distToPoint < radius) {
					for (int routeIndex2 = routeIndex+1; routeIndex2 < activity.GPSRoute.Count; routeIndex2++) {
						IGPSPoint routePoint2 = activity.GPSRoute[routeIndex2].Value;
						float distToPoint2 = this.TrailLocations[0].DistanceMetersToPoint(routePoint2);
						if (distToPoint2 > distToPoint) {
							break;
						} else {
							distToPoint = distToPoint2;
							routeIndex = routeIndex2;
						}
					}
					if (trailIndex == 0) {
						// found the start						
						startIndex = routeIndex;
						trailIndex++;

					} else if (trailIndex == this.TrailLocations.Count - 1) {
						// found the end
						endIndex = routeIndex;
						TrailResult result = new TrailResult(activity, resultsList.Count + 1, startIndex, endIndex);
						resultsList.Add(result);
						result = null;
						trailIndex = 0;
					} else {
						// found a mid point
						trailIndex++;
					}
				}
			}
			return resultsList;
		}
	}
}

