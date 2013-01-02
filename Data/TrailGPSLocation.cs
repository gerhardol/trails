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

//Used in both Trails and Matrix plugin

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using System.Xml;
using System;
using System.Collections.Generic;

namespace TrailsPlugin.Data
{
    public class TrailGPSLocation
    {
        public TrailGPSLocation(IGPSPoint gpsLoc, string name, bool required)
        {
            this._gpsPoint = gpsLoc;
            this._LatitudeDegrees = _gpsPoint.LatitudeDegrees;
            this._LongitudeDegrees = _gpsPoint.LongitudeDegrees;
            this._name = name;
            this._required = required;
        }
        public TrailGPSLocation(IGPSPoint gpsLoc) :
            this(gpsLoc, "", true)
        {
        }
        
        public static IGPSPoint getGpsLoc(ZoneFiveSoftware.Common.Data.Fitness.IActivity activity, DateTime time)
        {
            IGPSPoint result = null;
            if (activity != null && activity.GPSRoute != null)
            {
                ITimeValueEntry<IGPSPoint> p = activity.GPSRoute.GetInterpolatedValue(time);
                if (null != p)
                {
                    result = p.Value;
                }
            }
            return result;
        }

        public TrailGPSLocation(TrailGPSLocation trailLocation)
        {
            this._gpsPoint = trailLocation._gpsPoint;
            this._LatitudeDegrees = _gpsPoint.LatitudeDegrees;
            this._LongitudeDegrees = _gpsPoint.LongitudeDegrees;
            this._name = trailLocation._name;
            this._required = trailLocation._required;
            this._radius = trailLocation._radius;
        }

        public override string ToString()
        {
            return _name + " " + _required + " " + _gpsPoint;
        }

        private float _radius = 25;
        public float Radius
        {
            get
            {
                return this._radius;
            }
            set
            {
                this._radius = value;
            }
        }

        private IGPSPoint _gpsPoint;
        public IGPSPoint GpsPoint
        {
            get
            {
                return _gpsPoint;
            }
            set
            {
                _cosmean = invalidLatLon;
                _gpsPoint = value;
                this._LatitudeDegrees = _gpsPoint.LatitudeDegrees;
                this._LongitudeDegrees = _gpsPoint.LongitudeDegrees;
            }
        }

        //Note: Use fields to improve performance (trail detection with like 5%)
        //Setting the fields will not have any effect
        public float _LatitudeDegrees;
        public float LatitudeDegrees
        {
            get
            {
                return _gpsPoint.LatitudeDegrees;
            }
        }
        public float _LongitudeDegrees;
        public float LongitudeDegrees
        {
            get
            {
                return _gpsPoint.LongitudeDegrees;
            }
        }
        public float Elevation
        {
            get
            {
                return _gpsPoint.ElevationMeters;
            }
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                this._name = value;
            }
        }

        private bool _required;
        public bool Required
        {
            get
            {
                return _required;
            }
            set
            {
                this._required = value;
            }
        }

        //This code is shared in other plugins
#if TRAILSPLUGIN
        static public TrailGPSLocation FromXml(XmlNode node)
        {
            string name = "";
            if (null != node.Attributes[xmlTags.sName] &&
                null != node.Attributes[xmlTags.sName].Value)
            {
                name = node.Attributes[xmlTags.sName].Value.ToString();
            }

            bool required = true;
            if (null != node.Attributes[xmlTags.sRequired] &&
                null != node.Attributes[xmlTags.sRequired].Value)
            {
                required = XmlConvert.ToBoolean(node.Attributes[xmlTags.sRequired].Value);
            }

            float elevation = float.NaN;
            if (null != node.Attributes[xmlTags.sElevation] &&
                null != node.Attributes[xmlTags.sElevation].Value)
            {
                elevation = Settings.parseFloat(node.Attributes[xmlTags.sElevation].Value);
            }

            return new TrailGPSLocation(new GPSPoint(
                Settings.parseFloat(node.Attributes[xmlTags.sLatitude].Value),
                Settings.parseFloat(node.Attributes[xmlTags.sLongitude].Value),
                elevation),
                name, required
            );
        }
#endif

        public XmlNode ToXml(XmlDocument doc)
        {
			XmlNode TrailGPSLocationNode = doc.CreateElement(xmlTags.sTrailGPSLocation);

			XmlAttribute a;

            a = doc.CreateAttribute(xmlTags.sLatitude);
			a.Value = XmlConvert.ToString(this.LatitudeDegrees);
			TrailGPSLocationNode.Attributes.Append(a);

            a = doc.CreateAttribute(xmlTags.sLongitude);
            a.Value = XmlConvert.ToString(this.LongitudeDegrees);
            TrailGPSLocationNode.Attributes.Append(a);

            if (!float.IsNaN(this.Elevation))
            {
                a = doc.CreateAttribute(xmlTags.sElevation);
                a.Value = XmlConvert.ToString(this.Elevation);
                TrailGPSLocationNode.Attributes.Append(a);
            }

            a = doc.CreateAttribute(xmlTags.sName);
            a.Value = this.Name.ToString();
            TrailGPSLocationNode.Attributes.Append(a);

            a = doc.CreateAttribute(xmlTags.sRequired);
            a.Value = XmlConvert.ToString(this.Required);
            TrailGPSLocationNode.Attributes.Append(a);

            return TrailGPSLocationNode;
		}

        private class xmlTags
        {
            public const string sTrailGPSLocation = "TrailGPSLocation";
            public const string sLatitude = "latitude";
            public const string sLongitude = "longitude";
            public const string sElevation = "elevation";
            public const string sName = "name";
            public const string sRequired = "required";
        }

        /* Distance calculation */

        public float DistanceMetersToPoint(IGPSPoint point)
        {
            if (point == null)
            {
                return float.MaxValue;
            }
            return this._gpsPoint.DistanceMetersToPoint(point);
        }

        public const float DistanceToSquareScaling = RadToDeg / EarthRadius;

        //Squared scaled distance calculations, speedup calculations for trail detection
        //Use (quite accurate) aproximate scaled squared distance instead of real distance, improve performance
        //This is short distance, the result should be accurate
        //See http://en.wikipedia.org/wiki/Geographical_distance
        private const float invalidLatLon = 999;
        private float _cosmean = invalidLatLon;
        private const float DegToRad = (float)(Math.PI / 180.0);
        private const float RadToDeg = (float)(180.0 / Math.PI);
        private const float EarthRadius = 6371009;

        public static float DistanceMetersToPointSquared(TrailGPSLocation trailp, IGPSPoint point)
        {
            //Use the trailp lat instead of average lat
            if (trailp._cosmean == invalidLatLon) { trailp._cosmean = (float)Math.Cos(trailp._LatitudeDegrees * DegToRad); }
            float dlat = point.LatitudeDegrees - trailp._LatitudeDegrees;
            float dlon = point.LongitudeDegrees - trailp._LongitudeDegrees;
            float result = dlat * dlat + dlon * dlon * trailp._cosmean;

            return result;
        }

        public static float DistanceMetersToPointGpsSquared(IGPSPoint point, IGPSPoint point2)
        {
            float _cosmean = (float)Math.Cos(point2.LatitudeDegrees * DegToRad);
            float dlat = point.LatitudeDegrees - point2.LatitudeDegrees;
            float dlon = point.LongitudeDegrees - point2.LongitudeDegrees;
            float result = dlat * dlat + dlon * dlon * _cosmean;

            return result;
        }

        public static GPSBounds getGPSBounds(IList<TrailGPSLocation> list, float radius)
        {
            return getGPSBounds(list, radius, false);
        }

        //TBD: Called often - not affecting so much but could be optimised
        public static GPSBounds getGPSBounds(IList<TrailGPSLocation> list, float radius, bool requiredCheck)
        {
            float north = -85; //Limit in ST - No GPS outside
            float south = +85;
            float east = -180;
            float west = 180;
            bool enoughRequired = true;

            if (requiredCheck)
            {
                int noRequired = 0;
                const int minRequired = 1; //list.Count-trail.MaxRequiredMisses
                foreach (TrailGPSLocation g in list)
                {
                    //Check if there are too few required - then all points must be considered
                    if (g.Required)
                    {
                        noRequired++;
                    }
                }
                if (noRequired < minRequired)
                {
                    enoughRequired = false;
                }
            }

            foreach (TrailGPSLocation g in list)
            {
                if ((g.Required || !requiredCheck || !enoughRequired) && g.GpsPoint != null)
                {
                    float glat = g.GpsPoint.LatitudeDegrees;
                    float glon = g.GpsPoint.LongitudeDegrees;

                    north = Math.Max(north, glat);
                    south = Math.Min(south, glat);
                    east  = Math.Max(east, glon);
                    west  = Math.Min(west, glon);
                }
            }
            //Overlapping, could be an exception
            if (north < south || east < west)
            {
                return null;
            }
            //Radius could be less than 0, to get smaller bounds than actual
            if (radius < 100 && radius > 0)
            {
                radius = 100;
            }

            //Get approx degrees for the radius offset increasing/(decreasing) the bounds
            //The magic numbers are size of a degree at the equator
            //latitude increases about 1% at the poles
            //longitude is up to 40% longer than linear extension - compensate 20%
            float lat = radius / 110574 * 1.005F;
            float lng = (float)(radius / 111132 / Math.Cos(Math.Abs(north) * Math.PI / 180));
            north += lat;
            south -= lat;
            east  += lng;
            west  -= lng;

            //if radius is negative, area may have to be adjusted
            //With no required points, use center of trail
            if (north < south || requiredCheck && !enoughRequired)
            {
                float tmp = (north+south)/2;
                north = tmp;
                south = tmp;
            }
            if (east < west || requiredCheck && !enoughRequired)
            {
                float tmp = (west+east)/2;
                west = tmp;
                east = tmp;
            }

            return new GPSBounds(new GPSLocation(north, west), new GPSLocation(south, east));
        }

        // Some temporary handling, no bother proper
        public string getField(int subItemSelected)
        {
            string subItemText;
            switch (subItemSelected)
            {
                case 0:
                case 1:
                    subItemText = this.LongitudeDegrees.ToString();
                    break;
                case 2:
                    subItemText = this.LatitudeDegrees.ToString();
                    break;
                default:
                    subItemText = this.Name;
                    break;
            }
            return subItemText;
        }

        public TrailGPSLocation setField(int subItemSelected, string s)
        {
            float pos = float.NaN;
            int valid = 1;
            TrailGPSLocation result = this;
            if (subItemSelected <= 2)
            {
                //check valid numbers
                try
                {
                    if (!float.TryParse(s, out pos))
                    {
                        pos = float.NaN;
                    }
                }
                catch
                {
                    pos = float.NaN;
                }
                if (!float.IsNaN(pos)
                    || subItemSelected == 1 && 85 < Math.Abs(pos)
                    || subItemSelected == 2 && 180 < Math.Abs(pos)
                    )
                {
                    valid = 0;
                }
            }
            else if (subItemSelected == 3)
            {
                valid = 0;
            }

            if (valid == 0)
            {
                switch (subItemSelected)
                {
                    case 1:
                        _gpsPoint = new GPSPoint(this.LatitudeDegrees, pos, this.Elevation);
                        this._LongitudeDegrees = pos;
                        break;
                    case 2:
                        _gpsPoint = new GPSPoint(pos, this.LongitudeDegrees, this.Elevation);
                        this._LatitudeDegrees = pos;
                        break;
                    case 99:
                        //Not yet implemented
                        _gpsPoint = new GPSPoint(this.LatitudeDegrees, this.LongitudeDegrees, pos);
                        break;
                    default:
                        this.Name = s;
                        break;
                }
            }
            _cosmean = invalidLatLon;
            return result;
        }
	}
}
