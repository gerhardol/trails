﻿/*
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
using System.Text.RegularExpressions;

namespace TrailsPlugin.Data
{
    public static class TrailData
    {
        private static IDictionary<Guid, Data.Trail> m_AllTrails = DefaultTrails();
        public static Data.Trail ElevationPointsTrail;

        private static IDictionary<Guid, Data.Trail> DefaultTrails()
        {
            IDictionary<Guid, Data.Trail> allTrails = new Dictionary<Guid, Data.Trail>();
            //GUIDs could be dynamic or constants too
            Data.Trail trail;

            //Splits Trail
            trail = new Data.Trail(GUIDs.SplitsTrail, true)
            {
                Name = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelSplits,
                IsSplits = true,
                TrailType = Trail.CalcType.Splits,
                TrailPriority = -11
            };
            allTrails.Add(trail.Id, trail);

            //Reference Activity Trail
            trail = new Data.Trail(GUIDs.ReferenceTrail, true)
            {
                Name = Properties.Resources.Trail_Reference_Name,
                IsReference = true,
                //RefTrail is using the trail points, the points are limiting, not extending
                //IsCompleteActivity = false;
                TrailPriority = -10
            };
            allTrails.Add(trail.Id, trail);

            //HighScore Trail
            trail = new Data.Trail(GUIDs.HighScoreTrail, true)
            {
                Name = Properties.Resources.HighScore_Trail,
                TrailType = Trail.CalcType.HighScore,
                TrailPriority = -100
            };
            allTrails.Add(trail.Id, trail);

            //UniqueRoutes Trail
            trail = new Data.Trail(GUIDs.UniqueRoutesTrail, true)
            {
                Name = Properties.Resources.UniqueRoutes_Trail,
                TrailType = Trail.CalcType.UniqueRoutes,
                TrailPriority = -101
            };
            allTrails.Add(trail.Id, trail);

            //ElevationPoints Trail
            trail = new Data.Trail(GUIDs.ElevationPointsTrail, true)
            {
                Name = ZoneFiveSoftware.Common.Visuals.CommonResources.Text.LabelElevation,
                TrailType = Trail.CalcType.ElevationPoints,
                TrailPriority = -102,
                TrailLocations = new List<TrailGPSLocation>()
            };
            allTrails.Add(trail.Id, trail);
            ElevationPointsTrail = trail;

            return allTrails;
        }

        public static IDictionary<Guid, Data.Trail> AllTrails
        {
            get
            {
                return m_AllTrails;
            }
        }

        private static void AddElevationPoints(Data.Trail trail)
        {
            if (!trail.Generated)
            {
                foreach (TrailGPSLocation t in trail.TrailLocations)
                {
                    if (!trail.Generated && !float.IsNaN(t.ElevationMeters))
                    {
                        TrailGPSLocation l = new TrailGPSLocation(t);
                        l.Name = trail.Name + " " + GpsRunningPlugin.Util.UnitUtil.Elevation.ToString(l.Radius, "u");
                        //Set the trail radius to one of the points
                        ElevationPointsTrail.Radius = l.Radius;
                        ElevationPointsTrail.TrailLocations.Add(l);
                    }
                }
            }
        }

        public static Trail GetFromName(string trailName)
        {
            foreach (Trail t in m_AllTrails.Values)
            {
                if (t.Name == trailName)
                {
                    return t;
                }
            }
            return null;
        }

        //"Normalize" the name, to check for duplicates
        private static string NormName(string s)
        {
            string pattern = @"\s*:+\s*";
            string a = (new Regex(pattern)).Replace(s, ": ");
            return a;
        }

        private static void RefreshChildren()
        {
            foreach (Trail t in m_AllTrails.Values)
            {
                t.Children.Clear();
                t.Parent = null;
            }

            //Traverse over originalTrails, it cannot be modified
            IDictionary<string, Trail> originalTrails = new Dictionary<string, Trail>();
            IDictionary<string, Trail> allTrails = new Dictionary<string, Trail>();
            foreach (Trail t in m_AllTrails.Values)
            {
                originalTrails[NormName(t.Name)] = t;
                allTrails[NormName(t.Name)] = t;
            }
            //Make sure all parents exist
            foreach (string child0 in originalTrails.Keys)
            {
                string child = child0;
                while (child.IndexOf(':') >= 0)
                {
                    string parent = child.Substring(0, child.LastIndexOf(':'));
                    if (!allTrails.ContainsKey(parent))
                    {
                        //Add empty placeholder, deleted when exiting
                        Trail trail = new Data.Trail()
                        {
                            Name = parent,
                            IsTemporary = true,
                            TrailPriority = -9999
                        };
                        m_AllTrails.Add(trail.Id, trail);
                        Controller.TrailController.Instance.NewTrail(trail, false, null);
                        allTrails[parent] = trail;
                    }
                    child = parent;
                }
            }
            //Add the child/parents links, from the children
            foreach (string child in allTrails.Keys)
            {
                if (child.IndexOf(':') >= 0)
                {
                    string parent = child.Substring(0, child.LastIndexOf(':'));
                    System.Diagnostics.Debug.Assert(allTrails.ContainsKey(parent), "Parent to " + child + " not added");
                    allTrails[parent].Children.Add(allTrails[child]);
                    allTrails[child].Parent = allTrails[parent];
                }
            }
        }

        public static bool InsertTrail(Data.Trail trail)
        {
            foreach (Trail t in m_AllTrails.Values)
            {
                if (t.Name == trail.Name || NormName(t.Name) == NormName(trail.Name))
                {
                    return false;
                }
            }
            trail.Id = System.Guid.NewGuid();
            m_AllTrails.Add(trail.Id, trail);
            RefreshChildren();
            AddElevationPoints(trail);
            Plugin.WriteExtensionData();
            return true;
        }

        public static bool UpdateTrail(Data.Trail trail)
        {
            foreach (Trail t in m_AllTrails.Values)
            {
                if (t.Name == trail.Name && t.Id != trail.Id)
                {
                    return false;
                }
            }

            if (m_AllTrails.ContainsKey(trail.Id))
            {
                m_AllTrails[trail.Id] = trail;
                RefreshChildren();
                ElevationPointsTrail.TrailLocations.Clear();
                foreach (Trail t in m_AllTrails.Values)
                {
                    AddElevationPoints(t);
                }
                Plugin.WriteExtensionData();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool DeleteTrail(Data.Trail trail)
        {
            if (m_AllTrails.ContainsKey(trail.Id))
            {
                m_AllTrails.Remove(trail.Id);
                RefreshChildren();
                ElevationPointsTrail.TrailLocations.Clear();
                foreach (Trail t in m_AllTrails.Values)
                {
                    AddElevationPoints(t);
                }
                Plugin.WriteExtensionData();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void ReadOptions(XmlDocument xmlDoc, XmlNamespaceManager nsmgr, XmlElement pluginNode)
        {
            m_AllTrails = DefaultTrails();
            foreach (XmlElement node in pluginNode.SelectNodes(XmlTags.sTrail))
            {
                Data.Trail trail = Data.Trail.ReadOptions(xmlDoc, nsmgr, node);
                string name = trail.Name;
                int extraId = 0;
                bool isUnique = false;
                while (!isUnique)
                {
                    isUnique = true;
                    foreach (Trail t in m_AllTrails.Values)
                    {
                        if (t.Name == name || NormName(t.Name) == NormName(name))
                        {
                            isUnique = false;
                            extraId += 1;
                            name = trail.Name + "-" + extraId;
                            break;
                        }
                    }
                    if (extraId > 99)
                    {
                        break;
                    }
                }
                trail.Name = name;
                if (m_AllTrails.ContainsKey(trail.Id) && m_AllTrails[trail.Id].Generated)
                {
                    //http://www.zonefivesoftware.com/sporttracks/forums/viewtopic.php?f=76&t=17866
                    trail.Id = new Guid();
                }
                if (!m_AllTrails.ContainsKey(trail.Id))
                {
                    m_AllTrails.Add(trail.Id, trail);
                }
                else
                {}
                AddElevationPoints(trail);
            }

            RefreshChildren();
        }

        public static void WriteOptions(XmlDocument doc, XmlElement pluginNode)
        {
            foreach (Data.Trail trail in Data.TrailData.AllTrails.Values)
            {
                if (!trail.Generated && !trail.IsTemporary)
                {
                    XmlElement trailNode = doc.CreateElement(XmlTags.sTrail);
                    trail.WriteOptions(doc, trailNode);
                    pluginNode.AppendChild(trailNode);
                }
            }
        }

        private static class XmlTags
        {
            public const string sTrail = "Trail";
        }

        //Matrix integration, old call path
        public static IList<IList<string[]>> ListTrails()
        {
            return Export.Integration.ListTrails();
        }
    }
}
