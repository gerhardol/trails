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
        private float m_minDistance = 0;
        private int m_maxRequiredMisses = 0;

        private bool m_matchAll = false;
        private bool m_generated = false;
        private bool m_isReference = false;
        private int m_HighScore = 0; //0 not used, 1 standard HighScore (could be more variants)
        private IActivity m_referenceActivity = null;
        private static Controller.TrailController m_controller = Controller.TrailController.Instance;

		public Trail()
        {
            m_radius = Data.Settings.DefaultRadius;
		}

        public Trail Copy(bool isEdit)
        {
            return Copy(isEdit, null);
        }
        public Trail Copy(bool isEdit, IActivity activity)
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
                else if (this.m_generated && activity != null && activity.Name != "")
                {
                    result.Name = activity.Name;
                }
                else if (this.m_generated && activity != null && activity.Location != "")
                {
                    result.Name = activity.Location;
                }
                else
                {
                    result.Name = this.Name + ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionCopy;
                }
            }
            //Do not copy "auto" attributes
            result.m_radius = this.m_radius;
            result.m_minDistance = this.m_minDistance;
            result.m_maxRequiredMisses = this.m_maxRequiredMisses;
            if (this.MatchAll && activity != null && this.TrailLocations.Count == 0)
            {
                //get all points
                IList<int> indexes;
                result.TrailLocations = Trail.TrailGpsPointsFromSplits(
                    activity, out indexes, false);
            }
            else
            {
                foreach (TrailGPSLocation t in this.TrailLocations)
                {
                    result.m_trailLocations.Add(new TrailGPSLocation(t.LatitudeDegrees, t.LongitudeDegrees, t.Name, t.Required));
                }
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
                m_gpsBounds = null;
            }
        }

		public float Radius {
			get {
				return m_radius;
			}
			set {
				m_radius = value;
                m_gpsBounds = null;
            }
		}
        //This property is not visible in the GUI
        public float MinDistance
        {
            get
            {
                return m_minDistance;
            }
            set
            {
                m_minDistance = value;
            }
        }
        //This property is not visible in the GUI
        public int MaxRequiredMisses
        {
            get
            {
                return m_maxRequiredMisses;
            }
            set
            {
                m_maxRequiredMisses = value;
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
        public int HighScore
        {
            get
            {
                return m_HighScore;
            }
            set
            {
                m_HighScore = value;
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
                    m_gpsBounds = null;
                }
            }
        }
        private bool checkReferenceChanged()
        {
            bool result = false;
            if (m_isReference)
            {
                IActivity refAct = m_controller.checkReferenceActivity(false);
                if ((m_referenceActivity == null || refAct != m_referenceActivity) && refAct != null && 
                    refAct.GPSRoute != null && refAct.GPSRoute.Count > 0)
                {
                    m_gpsBounds = null;
                    m_referenceActivity = refAct;
                    m_trailLocations = new List<TrailGPSLocation>();
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
            //Add start indexes for active laps and for first point for rest following active
            //A pause at the end of the lap is not considered
            return TrailGpsPointsFromSplits(activity, out indexes, true);
        }
        public static IList<Data.TrailGPSLocation> TrailGpsPointsFromSplits(IActivity activity,
            out IList<int> indexes, bool onlyActiveLaps)
        {
            IList<Data.TrailGPSLocation> results = new List<Data.TrailGPSLocation>();

            //Get around a problem with only Rest laps
            if (onlyActiveLaps)
            {
                onlyActiveLaps = false;
                for (int j = 0; j < activity.Laps.Count; j++)
                {
                    if (!activity.Laps[j].Rest)
                    {
                        onlyActiveLaps = true;
                    }
                }
            }
            int lastIndex = 0;
            indexes = new List<int>();
            IList<bool> required = new List<bool>();
            IList<string> names = new List<string>();
            if (null == activity || null == activity.Laps || 0 == activity.Laps.Count)
            {
                indexes.Add(0);
                required.Add(true);
                if (null != activity)
                {
                    names.Add(activity.Name);
                }
            }
            else
            {
                lastIndex = activity.GPSRoute.Count - 1;
                for (int j = 0; j < activity.Laps.Count; j++)
                {
                    ILapInfo l = activity.Laps[j];
                    for (int i = 0; i < activity.GPSRoute.Count; i++)
                    {
                        if (0 > l.StartTime.CompareTo(TrailResult.getDateTimeFromElapsedActivityStatic(activity, activity.GPSRoute[i]).AddSeconds(0.9)) &&
                            (indexes.Count == 0 || i > indexes[indexes.Count - 1]) &&
                            (!onlyActiveLaps || !l.Rest || j > 0 && !activity.Laps[j - 1].Rest))
                        {
                            indexes.Add(i);
                            required.Add(!l.Rest);
                            names.Add(l.Notes);
                            i++;
                            break;
                        }
                    }
                }
            }

            bool lastIsRestlap = false;
            if (null == activity ||
                null == activity.Laps || 
                0 == activity.Laps.Count ||
                activity.Laps[activity.Laps.Count-1].Rest)
            {
                lastIsRestlap = true;
            }
            if (indexes.Count == 0 || 
                activity.GPSRoute.Count - 1 > indexes[indexes.Count - 1] &&
                (!onlyActiveLaps || !lastIsRestlap))
            {
                indexes.Add(activity.GPSRoute.Count - 1);
                required.Add(!lastIsRestlap);
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
                name, required[i]));
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

        static public Trail FromXml(XmlNode node)
        {
			Trail trail = new Trail();
            if (node.Attributes[xmlTags.sId] == null)
            {
				trail.Id = System.Guid.NewGuid().ToString();
			} else {
				trail.Id = node.Attributes[xmlTags.sId].Value;
			}
            trail.Name = node.Attributes[xmlTags.sName].Value;
            //Hidden possibility to get trails matching everything while activities are seen
            if (trail.Name.EndsWith("MatchAll"))
            {
                trail.MatchAll = true;
            }
            if (node.Attributes[xmlTags.sRadius] != null)
            {
                trail.Radius = Settings.parseFloat(node.Attributes[xmlTags.sRadius].Value);
            }
            if (node.Attributes[xmlTags.sMinDistance] != null)
            {
                trail.MinDistance = (Int16)XmlConvert.ToInt16(node.Attributes[xmlTags.sMinDistance].Value);
            }
            if (node.Attributes[xmlTags.sMaxRequiredMisses] != null)
            {
                trail.MaxRequiredMisses = (Int16)XmlConvert.ToInt16(node.Attributes[xmlTags.sMaxRequiredMisses].Value);
            }
            trail.TrailLocations.Clear();
			foreach (XmlNode TrailGPSLocationNode in node.SelectNodes(xmlTags.sTrailGPSLocation)) {
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
            XmlNode trailNode = doc.CreateElement(xmlTags.sTrail);
            XmlAttribute a = doc.CreateAttribute(xmlTags.sId);
            a.Value = this.Id;
            trailNode.Attributes.Append(a);
            a = doc.CreateAttribute(xmlTags.sName);
            a.Value = this.Name;
            trailNode.Attributes.Append(a);
            a = doc.CreateAttribute(xmlTags.sRadius);
            a.Value = this.Radius.ToString();
            trailNode.Attributes.Append(a);
            //Undocumented non-GUI property
            if (this.MinDistance > 0)
            {
                a = doc.CreateAttribute(xmlTags.sMinDistance);
                a.Value = this.MinDistance.ToString();
                trailNode.Attributes.Append(a);
            }
            if (this.MaxRequiredMisses > 0)
            {
                a = doc.CreateAttribute(xmlTags.sMaxRequiredMisses);
                a.Value = this.MaxRequiredMisses.ToString();
                trailNode.Attributes.Append(a);
            }
            foreach (TrailGPSLocation point in this.TrailLocations)
            {
                trailNode.AppendChild(point.ToXml(doc));
            }
            return trailNode;
        }

        private class xmlTags
        {
            public const string sTrail = "Trail";
            public const string sId = "id";
            public const string sName = "name";
            public const string sRadius = "radius";
            public const string sMinDistance = "minDistance";
            public const string sMaxRequiredMisses = "maxRequiredMisses";
            public const string sTrailGPSLocation = "TrailGPSLocation";
        }

        public bool IsInBounds(IList<IActivity> acts)
        {
            bool result = false;
            foreach (IActivity activity in acts)
            {
                if (activity != null && activity.GPSRoute != null)
                {
                    IGPSBounds gpsBounds = GPSBounds.FromGPSRoute(activity.GPSRoute);
                    result = this.IsInBounds(gpsBounds);
                    if (result)
                    {
                        break;
                    }
                }
            }
            return result;
        }

        //The bounds to check for - smaller than real bounds
        IGPSBounds m_gpsBounds = null;
        private IGPSBounds GpsBounds
        {
            get
            {
                m_gpsBounds = null;
                if (m_gpsBounds == null)
                {
                    m_gpsBounds = TrailGPSLocation.getGPSBounds(this.TrailLocations, -10 * m_radius, true);
                }
                return m_gpsBounds;
            }
        }
		private bool IsInBounds(IGPSBounds activityBounds) {
            if (null == activityBounds || this.TrailLocations.Count == 0)
            {
                return false;
            }
            if (this.GpsBounds == null)
            {
                return false;
            }
            //Have to adjust as ST will not consider i.e. 11,9453<11,92103<11,92104
            IGPSBounds a2 = new GPSBounds(
                new GPSLocation(activityBounds.NorthLatitudeDegrees+0.01F, activityBounds.WestLongitudeDegrees-0.01F),
                new GPSLocation(activityBounds.SouthLatitudeDegrees-0.01F, activityBounds.EastLongitudeDegrees+0.01F));
            return a2.Contains(this.GpsBounds);
		}
	}
}

