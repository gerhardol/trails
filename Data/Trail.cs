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
            TrailPoints, Splits, SplitsTime, SwimSplits, HighScore, UniqueRoutes, ElevationPoints
        }

        public Guid Id;
        public string Name;
        public IList<Trail> Children = new List<Trail>(); 

        //The default reference activity, only makes sense for non-auto trails
        private IActivity m_DefaultRefActivity = null;
        private string m_DefaultRefActivityString = null;
        //The activity defing the Reference trail.
        //Also used as an attempt to keep the selection for normal trails
        private IActivity m_referenceActivity = null;

        private IList<TrailGPSLocation> m_trailLocations = null;
        private float m_radius = Data.Settings.DefaultRadius;
        private float m_minDistance = 0;
        private int   m_maxRequiredMisses = 0;

        private bool m_biDirectional = false;
        private bool m_isReference = false;
        private bool m_isTemporary = false;
        private bool m_isNameMatch = false;
        private bool m_isCompleteActivity = false;
        private bool m_urFilter = false;

        private int m_trailPriority = 0;
        //private bool m_isAutoTryAll = true;

        private CalcType m_CalcType = Trail.CalcType.TrailPoints;
        private bool m_splits = false;
        private bool m_generated = false;

        public Trail()
            : this(System.Guid.NewGuid(), false)
        {
        }

        public Trail(Guid Id, bool generated)
        {
            this.Id = Id;
            this.m_generated = generated;
        }

        /// Copy a trail (same guid, but not auto attributes)
        public Trail Duplicate()
        {
            return Copy(this.Id);
        }

        /// Copy a trail (set name from activity)
        public Trail Copy(IActivity activity, bool addMode)
        {
            Trail result = Copy(this.Id);
            if (addMode)
            {
                result.Id = System.Guid.NewGuid();
            }

            if (!addMode && this.TrailType == CalcType.ElevationPoints)
            {
                //editing Elevation
                result.Name = this.Name;
                result.TrailType = this.TrailType;
                result.m_generated = this.m_generated;
            }
            else if (this.IsReference && m_referenceActivity != null && m_referenceActivity.Name != "")
            {
                result.Name = this.m_referenceActivity.Name;
            }
            else if (addMode && activity != null && activity.Name != "")
            {
                result.Name = activity.Name;
            }
            else if (addMode && activity != null && activity.Location != "")
            {
                result.Name = activity.Location;
            }
            else
            {
                result.Name = this.Name + ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionCopy;
            }

            if (this.m_generated && (addMode || this.TrailType != CalcType.ElevationPoints) && 
                activity != null && this.TrailLocations.Count == 0)
            {
                //get all points - it is made to a CalcType.TrailPoint trail
                result.TrailLocations = Trail.TrailGpsPointsFromSplits(activity, false);
            }

            return result;
        }

        private Trail Copy(Guid id)
        {
            Trail result = new Trail(id, false);
            result.Name = this.Name;

            //Do not copy "auto" attributes
            result.m_radius = this.Radius;
            result.MinDistance = this.MinDistance;
            result.MaxRequiredMisses = this.MaxRequiredMisses;
            result.BiDirectional = this.BiDirectional;
            result.IsNameMatch = this.IsNameMatch;
            result.IsCompleteActivity = this.IsCompleteActivity;
            result.IsURFilter = this.IsURFilter;
            //result.IsAutoTryAll = this.IsAutoTryAll;
            result.IsTemporary = this.IsTemporary;
            foreach(Trail t in this.Children)
            {
                result.Children.Add(t);
            }

            if (!this.m_generated)
            {
                result.TrailPriority = this.TrailPriority;
            }
            result.DefaultRefActivity = this.DefaultRefActivity;

            result.m_trailLocations = new List<TrailGPSLocation>();
            foreach (TrailGPSLocation t in this.TrailLocations)
            {
                TrailGPSLocation t2 = new TrailGPSLocation(t);
                result.m_trailLocations.Add(t2);
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
                if (this.IsReference)
                {
                    if (m_referenceActivity != null &&
                        (m_trailLocations == null || m_trailLocations.Count == 0))
                    {
                        m_trailLocations = TrailGpsPointsFromSplits(m_referenceActivity);
                        m_gpsBounds = null;
                    }
                }

                if (this.m_trailLocations == null)
                {
                    this.m_trailLocations = new List<TrailGPSLocation>();
                }

                return m_trailLocations;
            }
            set
            {
                m_trailLocations = value;
                m_gpsBounds = null;
            }
        }

        public float Radius
        {
            get
            {
                return m_radius;
            }
            set
            {
                m_radius = value;
                m_gpsBounds = null;
                if (this.TrailType != CalcType.ElevationPoints)
                {
                    //Normal trails all have the same radius, for now
                    foreach (TrailGPSLocation t in this.TrailLocations)
                    {
                        t.Radius = value;
                    }
                }
            }
        }

        public IActivity DefaultRefActivity
        {
            get
            {
                if (this.m_DefaultRefActivityString != null)
                {
                    foreach (IActivity act in TrailsPlugin.Plugin.GetApplication().Logbook.Activities)
                    {
                        if (this.m_DefaultRefActivityString.Equals(act.ReferenceId))
                        {
                            this.m_DefaultRefActivity = act;
                            break;
                        }
                    }
                    this.m_DefaultRefActivityString = null;
                }
                return this.m_DefaultRefActivity;
            }
            set
            {
                m_DefaultRefActivity = value;
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

        public bool IsCompleteActivity
        {
            get
            {
                return m_isCompleteActivity;
            }
            set
            {
                m_isCompleteActivity = value;
            }
        }

        public bool IsURFilter
        {
            get
            {
                return m_urFilter;
            }
            set
            {
                m_urFilter = value;
            }
        }

        //public bool IsAutoTryAll
        //{
        //    get
        //    {
        //        return m_isAutoTryAll;
        //    }
        //    set
        //    {
        //        m_isAutoTryAll = value;
        //    }
        //}

        public int TrailPriority
        {
            get
            {
                return m_trailPriority;
            }
            set
            {
                m_trailPriority = value;
            }
        }

        public IActivity ReferenceActivity
        {
            get
            {
                return m_referenceActivity;
            }
            set
            {
                if (m_referenceActivity != value)
                {
                    this.m_referenceActivity = value;
                    if (this.IsReference)
                    {
                        //Just reset, value is fetched when needed
                        this.m_gpsBounds = null;
                        this.m_trailLocations = null;
                    }
                }
            }
        }

        public bool IsNameParentTo(Trail t)
        {
            bool result;
            if (t.Name.StartsWith(this.Name + ":") ||
                t.Name.StartsWith(this.Name + " :"))
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public static IList<TrailGPSLocation> TrailGpsPointsFromGps(IList<IGPSLocation> gps, float radius)
        {
            IList<Data.TrailGPSLocation> results = new List<Data.TrailGPSLocation>();
            foreach (IGPSLocation g in gps)
            {
                if (g != null)
                {
                    results.Add(new TrailGPSLocation(g, radius));
                }
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
                result.Add(t);
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
                        IGPSPoint p = Utils.TrackUtil.getGpsLoc(activity, time);
                        if (p != null)
                        {
                            results.Points.Add(new TrailResultPoint(new TrailGPSLocation(p), time));
                        }
                        else
                        {
                            if (activity.GPSRoute == null || activity.GPSRoute.Count == 0)
                            {
                                results.Points.Add(new TrailResultPoint(new TrailGPSLocation(activity.Name, true), time, track.TotalElapsedSeconds));
                            }
                            else
                            {
                                System.Diagnostics.Debug.Assert(false, "out of GPS");
                                if (results.Points.Count > 0)
                                {
                                    //end insert
                                    break;
                                }
                            }
                        }
                        dist = Math.Min(track.Max, dist + cDist);
                    }
                }
                else
                {
                    DateTime time = ActivityInfoCache.Instance.GetInfo(activity).ActualTrackStart;
                    IGPSPoint p = Utils.TrackUtil.getGpsLoc(activity, time);
                    if (p != null)
                    {
                        results.Points.Add(new TrailResultPoint(new TrailGPSLocation(p), time));
                    }
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
                        if (activity.GPSRoute == null || activity.GPSRoute.Count == 0)
                        {
                            results.Points.Add(new TrailResultPoint(new TrailGPSLocation(name, !l.Rest), d, l.TotalTime, l));
                        }
                        else
                        {
                            IGPSPoint t = Utils.TrackUtil.getGpsLoc(activity, d);
                            if (t != null)
                            {
                                results.Points.Add(new TrailResultPoint(new TrailGPSLocation(t, name, !l.Rest), d, l.TotalTime, l));
                            }
                        }
                    }
                }
                lastIsRestlap = activity.Laps[activity.Laps.Count - 1].Rest;
            }

            //Add end point, except if last is a rest lap (where last already is added)
            if (!onlyActiveLaps || !lastIsRestlap)
            {
                DateTime d = ActivityInfoCache.Instance.GetInfo(activity).ActualTrackEnd;
                if (activity.GPSRoute == null || activity.GPSRoute.Count == 0)
                {
                    results.Points.Add(new TrailResultPoint(new TrailGPSLocation(activity.Name, !lastIsRestlap), d));
                }
                else
                {
                    IGPSPoint t = Utils.TrackUtil.getGpsLoc(activity, d);
                    if (t != null)
                    {
                        results.Points.Add(new TrailResultPoint(new TrailGPSLocation(t, activity.Name, !lastIsRestlap), d));
                    }
                }
            }

            //Special for activities without any GPS info
            if (results.Count == 0 && activity.HasStartTime)
            {
                results.Points.Add(new TrailResultPoint(new TrailGPSLocation(activity.Name, true), activity.StartTime));
                results.Points.Add(new TrailResultPoint(new TrailGPSLocation(activity.Name, true), activity.StartTime + activity.TotalTimeEntered));
            }

            //A trail created from splits should not define elevation points
            foreach (TrailGPSLocation t in results.Points)
            {
                t.SetElevation(float.NaN);
            }

            return results;
        }

        public static IList<TrailResultInfo> TrailResultSwimInfoFromSplits(IActivity activity, bool onlyActiveLaps, int subresultIndex)
        {
            IList<TrailResultInfo> results = new List<TrailResultInfo>();
            if (activity == null || null == activity.Laps || 0 == activity.Laps.Count)
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

            for (int j = 0; j < activity.Laps.Count; j++)
            {
                ILapInfo l = activity.Laps[j];
                if ((!onlyActiveLaps || !l.Rest || j > 0 && !activity.Laps[j - 1].Rest) &&
                   //All swim related have at least one PoolLength for each lap
                   l.PoolLengths != null && (l.PoolLengths.Count > 0))
                {
                    TrailResultInfo r = new TrailResultInfo(activity, false);
                    r.LapInfo = l;
                    results.Add(r);
                    foreach (IPoolLengthInfo p in l.PoolLengths)
                    {
                        DateTime d = p.StartTime;
                        IPoolLengthInfo p1 = PoolLengthInfo.GetPoolLength(p);
                        r.Points.Add(new TrailResultPoint(new TrailGPSLocation(null, !l.Rest), d, p.TotalTime, p1, subresultIndex));
                        subresultIndex++;
                    }
                    //Need (dummy) last point
                    IPoolLengthInfo p2 = r.Points[r.Points.Count - 1].PoolLengthInfo;
                    r.Points.Add(new TrailResultPoint(new TrailGPSLocation(null, !l.Rest), p2.StartTime + p2.TotalTime, TimeSpan.Zero, p2, subresultIndex));
                }
            }

            //A trail created from splits should not define elevation points
            foreach (TrailResultInfo r in results)
            {
                foreach (TrailGPSLocation t in r.Points)
                {
                    t.SetElevation(float.NaN);
                }
            }

            return results;
        }

        static public TrailResultInfo ResultInfoFromSelection(IActivity activity, ZoneFiveSoftware.Common.Visuals.Fitness.IItemTrackSelectionInfo selInfo)
        {
            TrailResultInfo indexes = new TrailResultInfo(activity, false);
            DateTime time = selInfo.MarkedTimes[0].Lower;
            IGPSPoint p = Utils.TrackUtil.getGpsLoc(activity, time);
            if (p != null)
            {
                indexes.Points.Add(new TrailResultPoint(new TrailGPSLocation(p), time));
            }
            time = selInfo.MarkedTimes[0].Upper;
            p = Utils.TrackUtil.getGpsLoc(activity, time);
            if (p != null)
            {
                indexes.Points.Add(new TrailResultPoint(new TrailGPSLocation(p), time));
            }
            return indexes;
        }

        static public IList<Data.TrailGPSLocation> MergeTrailLocations(IList<Data.TrailGPSLocation> t1, IList<Data.TrailGPSLocation> t2)
        {
            foreach (Data.TrailGPSLocation t in t2)
            {
                t1.Add(t);
            }
            foreach (Data.TrailGPSLocation t in t1)
            {
                //Do not set elevation automatically
                t.SetElevation(float.NaN);
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
            Trail trail = new Trail(Id, false);
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
            if (node.Attributes[xmlTags.sDefaultRefActivity] != null)
            {
                trail.m_DefaultRefActivityString = node.Attributes[xmlTags.sDefaultRefActivity].Value;
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
            if (node.Attributes[xmlTags.sCompleteActivity] != null)
            {
                trail.IsCompleteActivity = XmlConvert.ToBoolean(node.Attributes[xmlTags.sCompleteActivity].Value);
            }
            if (node.Attributes[xmlTags.sURFilter] != null)
            {
                trail.IsURFilter = XmlConvert.ToBoolean(node.Attributes[xmlTags.sURFilter].Value);
            }
            //if (node.Attributes[xmlTags.sAutoTryAll] != null)
            //{
            //    trail.IsAutoTryAll = XmlConvert.ToBoolean(node.Attributes[xmlTags.sAutoTryAll].Value);
            //}
            if (node.Attributes[xmlTags.sTrailPriority] != null)
            {
                trail.TrailPriority = (Int16)XmlConvert.ToInt16(node.Attributes[xmlTags.sTrailPriority].Value);
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
            //Set radius - not stored per trail point
            trail.Radius = trail.m_radius;

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
            if (this.DefaultRefActivity != null)
            {
                a = doc.CreateAttribute(xmlTags.sDefaultRefActivity);
                a.Value = this.DefaultRefActivity.ReferenceId;
                trailNode.Attributes.Append(a);
            }

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
            if (this.IsCompleteActivity)
            {
                a = doc.CreateAttribute(xmlTags.sCompleteActivity);
                a.Value = XmlConvert.ToString(this.IsCompleteActivity);
                trailNode.Attributes.Append(a);
            }
            if (this.IsURFilter)
            {
                a = doc.CreateAttribute(xmlTags.sURFilter);
                a.Value = XmlConvert.ToString(this.IsURFilter);
                trailNode.Attributes.Append(a);
            }
            //if (!this.IsAutoTryAll)
            //{
            //    a = doc.CreateAttribute(xmlTags.sAutoTryAll);
            //    a.Value = XmlConvert.ToString(this.IsAutoTryAll);
            //    trailNode.Attributes.Append(a);
            //}
            if (this.TrailPriority != 0)
            {
                a = doc.CreateAttribute(xmlTags.sTrailPriority);
                a.Value = XmlConvert.ToString(this.TrailPriority);
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
            public const string sDefaultRefActivity = "DefaultRefActivity";

            public const string sMinDistance = "minDistance";
            public const string sMaxRequiredMisses = "maxRequiredMisses";
            public const string sBiDirectional = "bidirectional";
            public const string sNameMatch = "namematch";
            public const string sCompleteActivity = "completeactivity";
            public const string sURFilter = "urfilter";
            public const string sTrailPriority = "trailpriority";
            //public const string sAutoTryAll = "autotryall";
            public const string sTrailGPSLocation = "TrailGPSLocation";
        }

        //The bounds to check for - smaller than real bounds
        IGPSBounds m_gpsBounds = null;
        private IGPSBounds GpsBounds
        {
            get
            {
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

        public bool IsInBounds(IActivity activity)
        {
            bool result = false;
            if (activity != null && activity.GPSRoute != null)
            {
                IGPSBounds gpsBounds = ActivityCache.GpsBoundsCache(activity);
                result = this.IsInBounds(gpsBounds);
            }
            return result;
        }

        public IList<IActivity> InBoundActivities(IList<IActivity> acts)
        {
            IList<IActivity> result = new List<IActivity>();
            foreach (IActivity activity in acts)
            {
                if (this.IsInBounds(activity))
                {
                    result.Add(activity);
                }
            }
            return result;
        }

        public override string ToString()
        {
            return this.Name.ToString() + " " + this.m_gpsBounds + " " + this.TrailLocations.Count;
        }
    }
}

