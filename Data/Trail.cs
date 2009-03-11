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

using System.Collections.Generic;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using System.Xml;
using System;

namespace TrailsPlugin.Data {
	public class Trail {
		public string Id = Guid.NewGuid().ToString();
		public string Name;
		private IList<TrailGPSLocation> m_trailLocations = new List<TrailGPSLocation>();
		private float m_radius = 5;

		public IList<TrailGPSLocation> TrailLocations {
			get {
				return m_trailLocations;
			}
		}

		public float Radius {
			get {
				return m_radius;
			}
			set {
				m_radius = value;
			}
		}

		static public Trail FromXml(XmlNode node, XmlNamespaceManager nsmgr) {
			Trail trail = new Trail();
			trail.Id = node.Attributes["id"].Value;
			trail.Name = node.Attributes["name"].Value;
			trail.Radius = float.Parse(node.Attributes["radius"].Value);
			trail.TrailLocations.Clear();
			foreach (XmlNode TrailGPSLocationNode in node.SelectNodes("ns:TrailGPSLocation", nsmgr)) {
				trail.TrailLocations.Add(TrailGPSLocation.FromXml(TrailGPSLocationNode, nsmgr));
			}
			return trail;
		}

		public XmlNode ToXml(XmlDocument doc) {
			XmlNode trailNode = doc.CreateElement("Trail");
			XmlAttribute a = doc.CreateAttribute("id");
			a.Value = this.Id;
			trailNode.Attributes.Append(a);
			a = doc.CreateAttribute("name");
			a.Value = this.Name;
			trailNode.Attributes.Append(a);
			a = doc.CreateAttribute("radius");
			a.Value = this.Radius.ToString();
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
	}
}

