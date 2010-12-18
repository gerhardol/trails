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
        private bool m_matchAll = false;
        private bool m_generated = false;
        private bool m_isReference = false;
        private IActivity m_referenceActivity = null;
        private static Controller.TrailController m_controller = Controller.TrailController.Instance;

		public Trail()
        {
            m_radius = Data.Settings.DefaultRadius;
		}

        public Trail Copy(bool isEdit)
        {
            Trail result = new Trail();
            if (isEdit)
            {
                result.Id = this.Id;
                result.Name = this.Name;
            }
            else
            {
                if (this.m_isReference && m_referenceActivity != null && m_referenceActivity.Name != "")
                {
                    result.Name = this.m_referenceActivity.Name;
                }
                else
                {
                    result.Name = this.Name + ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionCopy;
                }
            }
            //Do not copy "auto" attributes
            result.m_radius = this.m_radius;
            foreach (TrailGPSLocation t in this.TrailLocations)
            {
                result.m_trailLocations.Add(new TrailGPSLocation(t.LatitudeDegrees, t.LongitudeDegrees, t.Name));
            }
            return result;
        }

        public IList<TrailGPSLocation> TrailLocations
        {
            get
            {
                //Refresh TrailPoints
                if (m_isReference)
                {
                    checkReferenceChanged();
                    if (m_referenceActivity != null &&
                        m_trailLocations.Count == 0)
                    {
                        m_trailLocations = TrailGpsPointsFromSplits(m_referenceActivity);
                    }
                }
                return m_trailLocations;
            }
            set
            {
                m_trailLocations = value;
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

        public bool MatchAll
        {
            get
            {
                return m_matchAll;
            }
            set
            {
                m_matchAll = value;
            }
        }
        public bool Generated
        {
            get
            {
                return m_generated;
            }
            set
            {
                m_generated = value;
            }
        }
        public bool IsReference
        {
            get
            {
                return m_isReference;
            }
            set
            {
                m_isReference = value;
            }
        }
        public IActivity ReferenceActivity
        {
            get
            {
                checkReferenceChanged();
                return m_referenceActivity;
            }
        }
        public IActivity ReferenceActivityNoCalc
        {
            get
            {
                return m_referenceActivity;
            }
            set
            {
                if (m_referenceActivity != value)
                {
                    //Just reset, value is fetched when needed
                    m_referenceActivity = null;
                }
            }
        }
        private bool checkReferenceChanged()
        {
            bool result = false;
            if (m_isReference)
            {
                IActivity refAct = m_controller.checkReferenceActivity(false);
                if ((m_referenceActivity == null || refAct != m_referenceActivity && refAct != null) && 
                    refAct.GPSRoute != null && refAct.GPSRoute.Count > 0)
                {
                    m_referenceActivity = refAct;
                    m_trailLocations = new List<TrailGPSLocation>(); ;
                    result = true;
                }
            }
            return result;
        }

        public bool TrailChanged(IActivity activity)
        {
            return m_isReference && activity != m_referenceActivity && checkReferenceChanged();
        }

        public static IList<Data.TrailGPSLocation> TrailGpsPointsFromSplits(IActivity activity)
        {
            IList<int> indexes;

            return TrailGpsPointsFromSplits(activity, out indexes);
        }
        public static IList<Data.TrailGPSLocation> TrailGpsPointsFromSplits(IActivity activity,
            out IList<int> indexes)
        {
            IList<Data.TrailGPSLocation> results = new List<Data.TrailGPSLocation>();

            //Add start indexes for active laps and for first point for rest following active
            //A pause at the end of the lap is not considered
            const bool onlyActiveLaps = true;
            int lastIndex = 0;
            indexes = new List<int>();
            IList<string> names = new List<string>();
            if (null == activity.Laps || 0 == activity.Laps.Count)
            {
                indexes.Add(0);
                names.Add(activity.Name);
            }
            else
            {
                lastIndex = activity.GPSRoute.Count - 1;
                for (int j = 0; j < activity.Laps.Count; j++)
                {
                    ILapInfo l = activity.Laps[j];
                    for (int i = 0; i < activity.GPSRoute.Count; i++)
                    {
                        if (0 > l.StartTime.CompareTo(activity.GPSRoute.EntryDateTime(activity.GPSRoute[i]).AddSeconds(0.5)) &&
                            (indexes.Count == 0 || i > indexes[indexes.Count - 1]) &&
                            (!onlyActiveLaps || !l.Rest || j > 0 && !activity.Laps[j - 1].Rest))
                        {
                            indexes.Add(i);
                            names.Add(l.Notes);
                            i++;
                            break;
                        }
                    }
                }
            }
            if (indexes.Count == 0 || 
                activity.GPSRoute.Count - 1 > indexes[indexes.Count - 1] &&
                (!onlyActiveLaps || null == activity.Laps || 0 == activity.Laps.Count || !activity.Laps[activity.Laps.Count-1].Rest))
            {
                indexes.Add(activity.GPSRoute.Count - 1);
                names.Add(activity.Name);
            }

            for (int i = 0; i < indexes.Count; i++)
            {
                string name = "";
                if (i < names.Count)
                {
                    name = names[i];
                }
                results.Add(new Data.TrailGPSLocation(
                activity.GPSRoute[indexes[i]].Value.LatitudeDegrees,
                activity.GPSRoute[indexes[i]].Value.LongitudeDegrees,
                name));
            }
            return results;
        }

        static public IList<Data.TrailGPSLocation> MergeTrailLocations(IList<Data.TrailGPSLocation> t1, IList<Data.TrailGPSLocation> t2)
        {
            foreach (Data.TrailGPSLocation t in t2)
            {
                t1.Add(t);
            }
            return t1;
        }
		static public Trail FromXml(XmlNode node) {
			Trail trail = new Trail();
			if (node.Attributes["id"] == null) {
				trail.Id = System.Guid.NewGuid().ToString();
			} else {
				trail.Id = node.Attributes["id"].Value;
			}
			trail.Name = node.Attributes["name"].Value;
            if (trail.Name.EndsWith("MatchAll"))
            {
                trail.MatchAll = true;
            }
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

        public XmlNode ToXml(XmlDocument doc)
        {
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
            foreach (TrailGPSLocation point in this.TrailLocations)
            {
                trailNode.AppendChild(point.ToXml(doc));
            }
            return trailNode;
        }

        public bool IsInBounds(IList<IActivity> acts)
        {
            bool result = false;
            foreach (IActivity activity in acts)
            {
                IGPSBounds gpsBounds = GPSBounds.FromGPSRoute(activity.GPSRoute);
                result = this.IsInBounds(gpsBounds);
                if (result)
                {
                    break;
                }
            }
            return result;
        }

		private bool IsInBounds(IGPSBounds activityBounds) {
            if (null == activityBounds || this.TrailLocations.Count == 0)
            {
                return false;
            }
            //increase bounds to include radius in the bounds checking
            //Use a magic aproximate formula, about twice the radius
            float latOffset = m_radius * 2 * 18 / 195/10000;
            float longOffset = latOffset * (1 - Math.Abs(activityBounds.NorthLatitudeDegrees) / 90);
            IGPSBounds gpsBounds = new GPSBounds(
                new GPSLocation(activityBounds.NorthLatitudeDegrees + latOffset, activityBounds.WestLongitudeDegrees - longOffset),
                new GPSLocation(activityBounds.SouthLatitudeDegrees - latOffset, activityBounds.EastLongitudeDegrees + longOffset));
            foreach (TrailGPSLocation trailGPSLocation in this.TrailLocations)
            {
                if (!gpsBounds.Contains(trailGPSLocation.GpsLocation)) 
                {
					return false;
				}
			}
			return true;
		}
	}
}

