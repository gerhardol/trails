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

        public Guid Id;
        public string Name;
		private IList<TrailGPSLocation> m_trailLocations = new List<TrailGPSLocation>();
        private float m_radius;
        private float m_minDistance = 0;
        private int   m_maxRequiredMisses = 0;
        private bool m_biDirectional = false;
        private bool m_isReference = false;
        private bool m_isTemporary = false;
        private bool m_isNameMatch = false;
        private bool m_isAutoTryAll = true;

        private CalcType m_CalcType = Trail.CalcType.TrailPoints;
        private bool m_splits = false;
        private bool m_generated = false;

        private IActivity m_referenceActivity = null;
        private static Controller.TrailController m_controller = Controller.TrailController.Instance;

        public Trail(Guid Id)
        {
            this.Id = Id;
            m_radius = Data.Settings.DefaultRadius;
		}

        public Trail Copy(bool isEdit)
        {
            return Copy(isEdit, null);
        }

        public Trail Copy(bool isEdit, IActivity activity)
        {
            Trail result;
            if (isEdit)
            {
                result = new Trail(this.Id);
                result.Name = this.Name;
            }
            else
            {
                result = new Trail(System.Guid.NewGuid());
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
            result.BiDirectional = this.BiDirectional;
            result.IsNameMatch = this.IsNameMatch;
            result.IsAutoTryAll = this.IsAutoTryAll;
            result.IsTemporary = this.IsTemporary;

            if (this.TrailType == CalcType.Splits && activity != null && this.TrailLocations.Count == 0)
            {
                //get all points - it is made to a CalcType.TrailPoint trail
                result.TrailLocations = Trail.TrailGpsPointsFromSplits(activity, false);
            }
            else
            {
                foreach (TrailGPSLocation t in this.TrailLocations)
                {
                    result.m_trailLocations.Add(new TrailGPSLocation(t));
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
        public bool BiDirectional
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

        public bool IsTemporary
        {
            get
            {
                return m_isTemporary;
            }
            set
            {
                m_isTemporary = value;
            }
        }

        public bool IsNameMatch
        {
            get
            {
                return m_isNameMatch;
            }
            set
            {
                m_isNameMatch = value;
            }
        }

        public bool IsAutoTryAll
        {
            get
            {
                return m_isAutoTryAll;
            }
            set
            {
                m_isAutoTryAll = value;
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
                results.Add(new TrailGPSLocation(g));
            }
            return results;
        }

        public static IList<TrailGPSLocation> TrailGpsPointsFromSplits(IActivity activity)
        {
            //Add start indexes for active laps and for first point for rest following active
            //A pause at the end of the lap is not considered
            return TrailGpsPointsFromSplits(activity, true);
        }

        public static IList<TrailGPSLocation> TrailGpsPointsFromSplits(IActivity activity, bool onlyActiveLaps)
        {
            TrailResultInfo info = TrailResultInfoFromSplits(activity, onlyActiveLaps);
            IList<TrailGPSLocation> result = new List<TrailGPSLocation>();
            foreach (TrailResultPoint t in info.Points)
            {
                if (t.GpsLocation != null)
                {
                    result.Add(t);
                }
                else
                {
                }
            }
            return result;
        }

        public static TrailResultInfo TrailResultInfoFromSplits(IActivity activity, bool onlyActiveLaps)
        {
            TrailResultInfo results = new TrailResultInfo(activity, false);
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
                        break;
                    }
                }
            }

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
                        results.Points.Add(new TrailResultPoint(new TrailGPSLocation(TrailGPSLocation.getGpsLoc(activity, time)), time));
                        dist = Math.Min(track.Max, dist + cDist);
                    }
                }
                else
                {
                    DateTime time = ActivityInfoCache.Instance.GetInfo(activity).ActualTrackStart;
                    results.Points.Add(new TrailResultPoint(new TrailGPSLocation(TrailGPSLocation.getGpsLoc(activity, time)), time));
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
                            name = "#" + (results.Points.Count + 1);
                        }
                        DateTime d = l.StartTime;
                        results.Points.Add(new TrailResultPoint(new TrailGPSLocation(TrailGPSLocation.getGpsLoc(activity, d), name, !l.Rest), d));
                    }
                }
                lastIsRestlap = activity.Laps[activity.Laps.Count - 1].Rest;
            }

            //Add end point, except if last is a rest lap (where last already is added)
            if (!onlyActiveLaps || !lastIsRestlap)
            {
                DateTime d = ActivityInfoCache.Instance.GetInfo(activity).ActualTrackEnd;
                results.Points.Add(new TrailResultPoint(new TrailGPSLocation(TrailGPSLocation.getGpsLoc(activity, d), activity.Name, !lastIsRestlap), d));
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
            Guid Id;
            if (node.Attributes[xmlTags.sId] == null || String.IsNullOrEmpty(node.Attributes[xmlTags.sId].Value))
            {
                Id = System.Guid.NewGuid();
            }
            else
            {
                try
                {
                    Id = new Guid(node.Attributes[xmlTags.sId].Value.ToString());
                }
                catch (Exception)
                {
                    Id = System.Guid.NewGuid();
                }
            }
            Trail trail = new Trail(Id);
            trail.Name = node.Attributes[xmlTags.sName].Value.ToString();
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
            if (node.Attributes[xmlTags.sBiDirectional] != null)
            {
                //Output prior to 1.0.604 is not xml parsable (only if trail modified manually)
                trail.BiDirectional = XmlConvert.ToBoolean(node.Attributes[xmlTags.sBiDirectional].Value.ToLower());
            }
            if (node.Attributes[xmlTags.sNameMatch] != null)
            {
                trail.IsNameMatch = XmlConvert.ToBoolean(node.Attributes[xmlTags.sNameMatch].Value);
            }
            if (node.Attributes[xmlTags.sAutoTryAll] != null)
            {
                trail.IsAutoTryAll = XmlConvert.ToBoolean(node.Attributes[xmlTags.sAutoTryAll].Value);
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
            a.Value = XmlConvert.ToString(this.Id);
            trailNode.Attributes.Append(a);
            a = doc.CreateAttribute(xmlTags.sName);
            a.Value = this.Name.ToString();
            trailNode.Attributes.Append(a);
            a = doc.CreateAttribute(xmlTags.sRadius);
            a.Value = XmlConvert.ToString(this.Radius);
            trailNode.Attributes.Append(a);
            //Undocumented non-GUI property
            if (this.MinDistance > 0)
            {
                a = doc.CreateAttribute(xmlTags.sMinDistance);
                a.Value = XmlConvert.ToString(this.MinDistance);
                trailNode.Attributes.Append(a);
            }
            if (this.MaxRequiredMisses > 0)
            {
                a = doc.CreateAttribute(xmlTags.sMaxRequiredMisses);
                a.Value = XmlConvert.ToString(this.MaxRequiredMisses);
                trailNode.Attributes.Append(a);
            }
            if (this.BiDirectional)
            {
                a = doc.CreateAttribute(xmlTags.sBiDirectional);
                a.Value = XmlConvert.ToString(this.BiDirectional);
                trailNode.Attributes.Append(a);
            }
            if (this.IsNameMatch)
            {
                a = doc.CreateAttribute(xmlTags.sNameMatch);
                a.Value = XmlConvert.ToString(this.IsNameMatch);
                trailNode.Attributes.Append(a);
            }
            if (!this.IsAutoTryAll)
            {
                a = doc.CreateAttribute(xmlTags.sAutoTryAll);
                a.Value = XmlConvert.ToString(this.IsAutoTryAll);
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
            public const string sBiDirectional = "bidirectional";
            public const string sNameMatch = "namematch";
            public const string sAutoTryAll = "autotryall";
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

		private bool IsInBounds(IGPSBounds activityBounds)
        {
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

