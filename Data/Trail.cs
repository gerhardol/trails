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
		private float m_radius;

		public Trail() {
			m_radius = PluginMain.Settings.DefaultRadius;
		}

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

		static public Trail FromXml(XmlNode node) {
			Trail trail = new Trail();
			if (node.Attributes["id"] == null) {
				trail.Id = System.Guid.NewGuid().ToString();
			} else {
				trail.Id = node.Attributes["id"].Value;
			}
			trail.Name = node.Attributes["name"].Value;
			trail.Radius = float.Parse(node.Attributes["radius"].Value);
			trail.TrailLocations.Clear();
			foreach (XmlNode TrailGPSLocationNode in node.SelectNodes("TrailGPSLocation")) {
				trail.TrailLocations.Add(TrailGPSLocation.FromXml(TrailGPSLocationNode));
                if (null == trail.TrailLocations[trail.TrailLocations.Count-1].Name
                    || trail.TrailLocations[trail.TrailLocations.Count-1].Name.Equals(""))
                {
                    //Name the trail points
                    trail.TrailLocations[trail.TrailLocations.Count-1].Name =
                        "#" + trail.TrailLocations.Count;
                }
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

		public bool IsInBounds(IGPSBounds activityBounds) {
            //increase bounds to include radius in the bounds checking
            //Use a magic aproximate formula, about twice the radius
            float latOffset = m_radius * 2 * 18 / 195/10000;
            float longOffset = latOffset * (1 - Math.Abs(activityBounds.NorthLatitudeDegrees) / 90);
            IGPSBounds gpsBounds = new GPSBounds(
                new GPSLocation(activityBounds.NorthLatitudeDegrees + latOffset, activityBounds.WestLongitudeDegrees - longOffset),
                new GPSLocation(activityBounds.SouthLatitudeDegrees - latOffset, activityBounds.EastLongitudeDegrees + longOffset));
            foreach (TrailGPSLocation trailGPSLocation in this.TrailLocations)
            {
                if (!gpsBounds.Contains(trailGPSLocation.GpsLocation)
                    ) {
					return false;
				}
			}
			return true;
		}
	}
}

