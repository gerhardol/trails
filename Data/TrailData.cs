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

namespace TrailsPlugin.Data
{
	public static class TrailData 
    {
        private static SortedList<string, Data.Trail> m_AllTrails = defaultTrails();
        
        private static SortedList<string, Data.Trail> defaultTrails()
        {
            SortedList<string, Data.Trail> allTrails = new SortedList<string, Data.Trail>();
            //GUIDs could be dynamic or constants too

            //Splits Trail
            Data.Trail trail = new Data.Trail();
            trail.Id = "30681520-b220-11e0-a00b-0800200c9a66";
            trail.Name = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelSplits;
            trail.Generated = true;
            trail.IsSplits = true;
            allTrails.Add(trail.Id, trail);

            //Reference Activity Trail
            trail = new Data.Trail();
            trail.Id = "30681521-b220-11e0-a00b-0800200c9a66";
            trail.Name = Properties.Resources.Trail_Reference_Name;
            trail.Generated = true;
            trail.IsReference = true;
            allTrails.Add(trail.Id, trail);

            //HighScore Trail
            trail = new Data.Trail();
            trail.Id = "30681522-b220-11e0-a00b-0800200c9a66";
            trail.Name = Properties.Resources.HighScore_Trail;
            trail.Generated = true;
            trail.HighScore = 1;
            allTrails.Add(trail.Id, trail);

            return allTrails;
        }

		public static SortedList<string, Data.Trail> AllTrails
        {
			get
            {
				return m_AllTrails;
			}
		}

        //A separate cache will require that m_AllTrails/m_referenceTrail_Activity is init in the same structure
        //public static Data.Trail ReferenceTrail_Activity
        //{
        //    get
        //    {
        //        return m_referenceTrail_Activity;
        //    }
        //}

        public static bool NameExists(string trailName)
        {
            foreach (Trail t in m_AllTrails.Values)
            {
                if (t.Name == trailName)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool InsertTrail(Data.Trail trail)
        {
            foreach (Trail t in m_AllTrails.Values)
            {
                if (t.Name == trail.Name)
                {
                    return false;
                }
            }
            trail.Id = System.Guid.NewGuid().ToString();
            m_AllTrails.Add(trail.Id, trail);
            Plugin.WriteExtensionData();
            return true;
        }

        public static bool UpdateTrail(Data.Trail trail)
        {
			foreach (Trail t in m_AllTrails.Values) {
				if (t.Name == trail.Name && t.Id != trail.Id) {
					return false;
				}
			}

			if (m_AllTrails.ContainsKey(trail.Id)) {
				m_AllTrails.Remove(trail.Id);
				m_AllTrails.Add(trail.Id, trail);
				Plugin.WriteExtensionData();
				return true;
			} else {
				return false;
			}
		}
        public static bool DeleteTrail(Data.Trail trail)
        {
			if (m_AllTrails.ContainsKey(trail.Id)) {
				m_AllTrails.Remove(trail.Id);
				Plugin.WriteExtensionData();
				return true;
			} else {
				return false;
			}
		}
        public static void ReadOptions(XmlDocument xmlDoc, XmlNamespaceManager nsmgr, XmlElement pluginNode)
        {
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
            foreach (Data.Trail trail in Data.TrailData.AllTrails.Values)
            {
                if (!trail.Generated)
                {
                    trails.AppendChild(trail.ToXml(doc));
                }
            }
            pluginNode.SetAttribute(xmlTags.tTrails, trails.OuterXml.ToString());

        }
        private static class xmlTags
        {
            public const string tTrails = "tTrails";
        }

        //Old version, read from logbook
        public static void FromXml(XmlNode pluginNode)
        {
            //foreach (XmlNode node in pluginNode.SelectNodes("Trails/Trail")) {
            //    Data.Trail trail = Data.Trail.FromXml(node);
            //    m_AllTrails.Add(trail.Id, trail);
            //}

		}

        //This is not called by default
        public static XmlNode ToXml(XmlDocument doc)
        {
            XmlNode trails = doc.CreateElement("Trails");
            //foreach (Data.Trail trail in Data.TrailData.AllTrails.Values)
            //{
            //    trails.AppendChild(trail.ToXml(doc));
            //}
            return trails;
		}

        //Matrix integration, old call path
        public static IList<IList<string[]>> ListTrails()
        {
            return Export.Integration.ListTrails();
        }
	}
}
