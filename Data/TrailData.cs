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

using System;
using System.Collections.Generic;
using ZoneFiveSoftware.Common.Data.Fitness;
using System.Xml;
using System.Xml.Serialization;
using ZoneFiveSoftware.Common.Data.GPS;

namespace TrailsPlugin.Data {
	public class TrailData 
    {
        private static Data.Trail m_referenceTrail_Activity = null;
        private static SortedList<string, Data.Trail> m_AllTrails;
        
        public TrailData()
        {
            defaults();
        }
        private static void defaults()
        {
            m_AllTrails = new SortedList<string, Data.Trail>();

            //Splits Trail
            Data.Trail trail = new Data.Trail();
            trail.Id = System.Guid.NewGuid().ToString();
            trail.Name = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelSplits;
            trail.Generated = true;
            trail.MatchAll = true;
            m_AllTrails.Add(trail.Id, trail);

            //Reference Activity Trail
            trail = new Data.Trail();
            trail.Id = System.Guid.NewGuid().ToString();
            trail.Name = Properties.Resources.Trail_Reference_Name;
            trail.Generated = true;
            trail.IsReference = true;
            m_AllTrails.Add(trail.Id, trail);
            m_referenceTrail_Activity = trail;
        }

		public SortedList<string, Data.Trail> AllTrails {
			get {
				return m_AllTrails;
			}
		}

        public Data.Trail ReferenceTrail_Activity
        {
            get
            {
                return m_referenceTrail_Activity;
            }
        }

		public bool InsertTrail(Data.Trail trail) {
			foreach (Trail t in m_AllTrails.Values) {
				if (t.Name == trail.Name) {
					return false;
				}
			}
			trail.Id = System.Guid.NewGuid().ToString();
			m_AllTrails.Add(trail.Id, trail);
			PluginMain.WriteExtensionData();
			return true;
		}

		public bool UpdateTrail(Data.Trail trail) {
			foreach (Trail t in m_AllTrails.Values) {
				if (t.Name == trail.Name && t.Id != trail.Id) {
					return false;
				}
			}

			if (m_AllTrails.ContainsKey(trail.Id)) {
				m_AllTrails.Remove(trail.Id);
				m_AllTrails.Add(trail.Id, trail);
				PluginMain.WriteExtensionData();
				return true;
			} else {
				return false;
			}
		}
		public bool DeleteTrail(Data.Trail trail) {
			if (m_AllTrails.ContainsKey(trail.Id)) {
				m_AllTrails.Remove(trail.Id);
				PluginMain.WriteExtensionData();
				return true;
			} else {
				return false;
			}
		}
        public static void ReadOptions(XmlDocument xmlDoc, XmlNamespaceManager nsmgr, XmlElement pluginNode)
        {
            defaults();
            String attr;
            attr = pluginNode.GetAttribute(xmlTags.tTrails);
            XmlDocument doc = new XmlDocument();
            if (null == attr || 0 == attr.Length)
            {
                attr = "<Trails/>";
            }
            doc.LoadXml(attr);

            foreach (XmlNode node in doc.DocumentElement.SelectNodes("Trail"))
            {
                Data.Trail trail = Data.Trail.FromXml(node);
                m_AllTrails.Add(trail.Id, trail);
            }
        }

        public static void WriteOptions(XmlDocument doc, XmlElement pluginNode)
        {
            XmlNode trails = doc.CreateElement("Trails");
            foreach (Data.Trail trail in PluginMain.Data.AllTrails.Values)
            {
                if (!trail.Generated)
                {
                    trails.AppendChild(trail.ToXml(doc));
                }
            }
            pluginNode.SetAttribute(xmlTags.tTrails, trails.OuterXml.ToString());

        }
        private class xmlTags
        {
            public const string tTrails = "tTrails";
        }

        //Old version, read from logbook
        public void FromXml(XmlNode pluginNode)
        {
            defaults();
			foreach (XmlNode node in pluginNode.SelectNodes("Trails/Trail")) {
				Data.Trail trail = Data.Trail.FromXml(node);
				m_AllTrails.Add(trail.Id, trail);
			}

		}

        //This is not called by default
		public XmlNode ToXml(XmlDocument doc) {
			XmlNode trails = doc.CreateElement("Trails");
			foreach (Data.Trail trail in PluginMain.Data.AllTrails.Values) {
				trails.AppendChild(trail.ToXml(doc));
			}
			return trails;
		}

        //Matrix integration
        //Should use common library and data structures
        public static IList<IList<string[]>> ListTrails()
        {
            IList<IList<string[]>> result = new List<IList<string[]>>();
            foreach (Data.Trail trail in PluginMain.Data.AllTrails.Values)
            {
                IList<string[]> tl = new List<string[]>();
                foreach (Data.TrailGPSLocation trailpoint in trail.TrailLocations)
                {
                    string[] t = {trail.Name, 
                                   trail.Radius.ToString(),
                                   trailpoint.LatitudeDegrees.ToString(),
                                   trailpoint.LongitudeDegrees.ToString(),
                                   trailpoint.Name};
                    tl.Add(t);
                }
                result.Add(tl);
            }
            return result;
        }
	}
}
