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

namespace TrailsPlugin.Data
{
	public class Trail
    {
        //How the trail is to be determined
        //A RefTrail is calc from Points, a Points trail could present Splits
        //Also used when sorting trails
        public enum CalcType
        {
            TrailPoints, Splits, HighScore
        }

        public string Id = Guid.NewGuid().ToString();
        public string Name;
		private IList<TrailGPSLocation> m_trailLocations = new List<TrailGPSLocation>();
        private float m_radius;
        private float m_minDistance = 0;
        private int m_maxRequiredMisses = 0;
        private bool m_biDirectional = false;

        private CalcType m_CalcType = Trail.CalcType.TrailPoints;
        private bool m_splits = false;
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
            if (this.TrailType == CalcType.Splits && activity != null && this.TrailLocations.Count == 0)
            {
                //get all points - it is made to a CalcType.TrailPoint trail
                TrailResultInfo indexes;
                result.TrailLocations = Trail.TrailGpsPointsFromSplits(
                    activity, out indexes, false);
            }
            else
            {
                foreach (TrailGPSLocation t in this.TrailLocations)
                {
                    result.m_trailLocations.Add(t.Copy());
                }
            }
            return result;
        }

        public CalcType TrailType
        {
            get { return m_CalcType; }
            set { m_CalcType = value; }
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
                foreach (TrailGPSLocation t in this.m_trailLocations)
                {
                    t.Radius = this.m_radius;
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
                foreach (TrailGPSLocation t in this.m_trailLocations)
                {
                    t.Radius=value;
                }
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
        //This property is not visible in the GUI
        public bool Bidirectional
        {
            get
            {
                return m_biDirectional;
            }
            set
            {
                m_biDirectional = value;
            }
        }
        /// <summary>
        /// Present TrailPoints or Splits
        /// </summary>
        public bool IsSplits
        {
            get
            {
                return m_splits;
            }
            set
            {
                m_splits = value;
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

        public static IList<TrailGPSLocation> TrailGpsPointsFromGps(IList<IGPSLocation> gps)
        {
            IList<Data.TrailGPSLocation> results = new List<Data.TrailGPSLocation>();
            foreach (IGPSLocation g in gps)
            {
                results.Add(new TrailGPSLocation(g.LatitudeDegrees, g.LongitudeDegrees, ""));
            }
            return results;
        }

        public static IList<TrailGPSLocation> TrailGpsPointsFromSplits(IActivity activity)
        {
            TrailResultInfo indexes;

            return TrailGpsPointsFromSplits(activity, out indexes);
        }

        public static IList<TrailGPSLocation> TrailGpsPointsFromSplits(IActivity activity,
            out TrailResultInfo indexes)
        {
            //Add start indexes for active laps and for first point for rest following active
            //A pause at the end of the lap is not considered
            return TrailGpsPointsFromSplits(activity, out indexes, true);
        }

        public static IList<TrailGPSLocation> TrailGpsPointsFromSplits(IActivity activity,
            out TrailResultInfo indexes, bool onlyActiveLaps)
        {
            IList<Data.TrailGPSLocation> results = new List<Data.TrailGPSLocation>();
            indexes = new TrailResultInfo(activity, false);
            if (activity == null)
            {
                //summary result
                return results;
            }

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

            IList<bool> lapActive = new List<bool>();

            bool lastIsRestlap = false;
            if (null == activity.Laps || 0 == activity.Laps.Count)
            {
                //Use MovingDistanceMetersTrack rather than ActualDistanceMetersTrack, assume similar activities have similar pauses/slow parts
                IDistanceDataTrack track = ActivityInfoCache.Instance.GetInfo(activity).MovingDistanceMetersTrack;
                if (track != null && track.Max > 0)
                {
                    //Create some kind of points - could be dependent on length
                    const float cDist = 1000;
                    float dist = 0;
                    while (dist < track.Max)
                    {
                        DateTime time = track.GetTimeAtDistanceMeters(dist);
                        indexes.Points.Add(new TrailResultPoint(time,""));
                        lapActive.Add(true);
                        dist = Math.Min(track.Max, dist + cDist);
                    }
                }
                else
                {
                    indexes.Points.Add(new TrailResultPoint(ActivityInfoCache.Instance.GetInfo(activity).ActualTrackStart, activity.Name));
                    lapActive.Add(true);
                }
            }
            else
            {
                for (int j = 0; j < activity.Laps.Count; j++)
                {
                    ILapInfo l = activity.Laps[j];
                    if (!onlyActiveLaps || !l.Rest || j > 0 && !activity.Laps[j - 1].Rest)
                    {
                        string name = l.Notes;
                        if (string.IsNullOrEmpty(name))
                        {
                            name = "#" + (indexes.Points.Count + 1);
                        }
                        indexes.Points.Add(new TrailResultPoint(l.StartTime, name));
                        lapActive.Add(!l.Rest);
                    }
                }
                lastIsRestlap = activity.Laps[activity.Laps.Count - 1].Rest;
            }

            //Add end point, except if last is a rest lap (where last already is added)
            if (!onlyActiveLaps || !lastIsRestlap)
            {
                indexes.Points.Add(new TrailResultPoint(ActivityInfoCache.Instance.GetInfo(activity).ActualTrackEnd, activity.Name));
                lapActive.Add(!lastIsRestlap);
            }

            if (null != activity.GPSRoute && 0 < activity.GPSRoute.Count)
            {
                for (int i = 0; i < indexes.Points.Count; i++)
                {
                    TrailResultPoint p = indexes.Points[i];
                    try
                    {
                        ITimeValueEntry<IGPSPoint> g = activity.GPSRoute.GetInterpolatedValue(p.Time);
                        results.Add(new Data.TrailGPSLocation(
                          p.Time, g, p.Name, lapActive[i]));
                    }
                    catch { }
                }
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
            //if (trail.Name.EndsWith("MatchAll"))
            //{
            //    trail.IsSplits = true;
            //}
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
            if (node.Attributes[xmlTags.sBidirectional] != null)
            {
                trail.Bidirectional = XmlConvert.ToBoolean(node.Attributes[xmlTags.sBidirectional].Value);
            }
            trail.TrailLocations.Clear();
			foreach (XmlNode TrailGPSLocationNode in node.SelectNodes(xmlTags.sTrailGPSLocation)) {
				trail.TrailLocations.Add(TrailGPSLocation.FromXml(TrailGPSLocationNode));
                if (string.IsNullOrEmpty(trail.TrailLocations[trail.TrailLocations.Count-1].Name))
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
            if (this.Bidirectional)
            {
                a = doc.CreateAttribute(xmlTags.sBidirectional);
                a.Value = this.Bidirectional.ToString();
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
            public const string sBidirectional = "bidirectional";
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
            IGPSBounds a2 = activityBounds;
            if (this.GpsBounds.NorthLatitudeDegrees == this.GpsBounds.SouthLatitudeDegrees ||
                this.GpsBounds.WestLongitudeDegrees == this.GpsBounds.EastLongitudeDegrees)
            {
                a2 = new GPSBounds(
                new GPSLocation(activityBounds.NorthLatitudeDegrees + 0.01F, activityBounds.WestLongitudeDegrees - 0.01F),
                new GPSLocation(activityBounds.SouthLatitudeDegrees - 0.01F, activityBounds.EastLongitudeDegrees + 0.01F));
            }
            return a2.Contains(this.GpsBounds);
		}
        public override string ToString()
        {
            return this.Name.ToString() + " " + this.m_gpsBounds + " " + this.m_trailLocations.Count;
        }
	}
}

