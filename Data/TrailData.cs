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
using ZoneFiveSoftware.Common.Data.Fitness;
using System.Xml;
using System.Xml.Serialization;
using ZoneFiveSoftware.Common.Data.GPS;

namespace TrailsPlugin.Data {
	public class TrailData {

		private SortedList<string, Data.Trail> m_AllTrails = new SortedList<string, Data.Trail>();

		public SortedList<string, Data.Trail> AllTrails {
			get {
				return m_AllTrails;
			}
		}

		public bool InsertTrail(Data.Trail trail) {
			if (m_AllTrails.ContainsKey(trail.Name)) {
				return false;
			} else {
				m_AllTrails.Add(trail.Name, trail);
				return true;
			}
		}
		public bool UpdateTrail(string oldKey, Data.Trail trail) {
			if (m_AllTrails.ContainsKey(oldKey)) {
				m_AllTrails.Remove(oldKey);
				m_AllTrails.Add(trail.Name, trail);
				return true;
			} else {
				return false;
			}
		}
		public bool DeleteTrail(Data.Trail trail) {
			if (m_AllTrails.ContainsKey(trail.Name)) {
				m_AllTrails.Remove(trail.Name);
				return true;
			} else {
				return false;
			}
		}

		public void FromXml(XmlNode pluginNode, XmlNamespaceManager nsmgr) {
			m_AllTrails.Clear();
			foreach (XmlNode node in pluginNode.SelectNodes("trail:Trails/trail:Trail", nsmgr)) {
				Data.Trail trail = Data.Trail.FromXml(node, nsmgr);
				m_AllTrails.Add(trail.Name, trail);
			}

		}

		public XmlNode ToXml(XmlDocument doc) {
			XmlNode trails = doc.CreateElement("Trails");
			foreach (Data.Trail trail in PluginMain.Data.AllTrails.Values) {
				trails.AppendChild(trail.ToXml(doc));
			}
			return trails;
		}
	}
}
