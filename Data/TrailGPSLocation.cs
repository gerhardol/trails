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

//Use ST distance calculations in trail calculations, slows down
//#define NO_SIMPLE_DISTANCE

//Instead of real distances, use "scaled squared" distance calculations. This requires that compare values and factors are adjusted too.
//The effect on performance is minimal though.
//#define SQUARE_DISTANCE
#if NO_SIMPLE_DISTANCE
#undef SQUARE_DISTANCE
#endif

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using System.Xml;
using System;
using System.Collections.Generic;

namespace TrailsPlugin.Data
{
    public class TrailGPSLocation : GPSPoint
    {
        public TrailGPSLocation(float lat, float lon, float ele, string name, bool required, float radius)
          : base(lat, lon, ele)
        {
            this._name = name;
            this._required = required;
            this._radius = radius;
        }
        public TrailGPSLocation(IGPSPoint gpsLoc, string name, bool required)
            : this(gpsLoc.LatitudeDegrees, gpsLoc.LongitudeDegrees, gpsLoc.ElevationMeters, name, required, defaultRadius)
        {
        }
        public TrailGPSLocation(IGPSPoint gpsLoc) :
            this(gpsLoc, "", true)
        {
        }
        public TrailGPSLocation(IGPSLocation gpsLoc) :
            this(gpsLoc.LatitudeDegrees, gpsLoc.LongitudeDegrees, float.NaN, "", true, defaultRadius)
        {
        }
        public TrailGPSLocation(TrailGPSLocation trailLocation)
            : this(trailLocation.LatitudeDegrees, trailLocation.LongitudeDegrees, trailLocation.ElevationMeters, trailLocation._name, trailLocation._required, trailLocation._radius)
        {
        }
        
        public override string ToString()
        {
            return _name + " " + _required + " " +base.ToString()/* _gpsPoint*/;
        }

        //IGPSPoint does not allow direct modification of points
        public IGPSPoint GpsPoint
        {
            set
            {
                if (value != null)
                {
                    if (!float.IsNaN(value.LatitudeDegrees))
                    {
                        this.latitudeDegrees = value.LatitudeDegrees;
                    }
                    if (!float.IsNaN(value.LongitudeDegrees))
                    {
                        this.longitudeDegrees = value.LongitudeDegrees;
                    }
                    if (!float.IsNaN(value.ElevationMeters))
                    {
                        this.elevationMeters = value.ElevationMeters;
                    }
                }
            }
        }

        public IGPSLocation GpsLoc
        {
            set
            {
                if (value != null)
                {
                    if (!float.IsNaN(value.LatitudeDegrees))
                    {
                        this.latitudeDegrees = value.LatitudeDegrees;
                    }
                    if (!float.IsNaN(value.LongitudeDegrees))
                    {
                        this.longitudeDegrees = value.LongitudeDegrees;
                    }
                }
            }
        }

        private const float defaultRadius = 25;
        private float _radius;
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

        //Special set functions - GPSLocation could be "new"
        public void SetLatitude(string s)
        {
            float pos = float.NaN;
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
            if (!float.IsNaN(pos) && 180 < Math.Abs(pos))
            {
                this.latitudeDegrees = pos;
                _cosmean = invalidLatLon;
            }
        }

        public void SetLongitude(string s)
        {
            float pos = float.NaN;
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
            if (!float.IsNaN(pos) && 85 < Math.Abs(pos))
            {
                this.longitudeDegrees = pos;
                _cosmean = invalidLatLon;
            }
        }

        public void SetElevation(string s)
        {
            float pos = (float)GpsRunningPlugin.Util.UnitUtil.Elevation.Parse(s);
            if (!float.IsNaN(pos))
            {

                this.elevationMeters = pos;
            }
        }

#if TRAILSPLUGIN
        //This code is shared in other plugins
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

            float lat = float.NaN;
            if (null != node.Attributes[xmlTags.sLatitude] &&
                null != node.Attributes[xmlTags.sLatitude].Value)
            {
                lat = Settings.parseFloat(node.Attributes[xmlTags.sLatitude].Value);
            }

            float lon = float.NaN;
            if (null != node.Attributes[xmlTags.sLongitude] &&
                null != node.Attributes[xmlTags.sLongitude].Value)
            {
                lon = Settings.parseFloat(node.Attributes[xmlTags.sLongitude].Value);
            }

            return new TrailGPSLocation(lat, lon, elevation, name, required, defaultRadius);
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

            if (!float.IsNaN(this.ElevationMeters))
            {
                a = doc.CreateAttribute(xmlTags.sElevation);
                a.Value = XmlConvert.ToString(this.ElevationMeters);
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

#if NO_SIMPLE_DISTANCE
#else
#if SQUARE_DISTANCE
        public const float DistanceToSquareScaling = RadToDeg / EarthRadius;
        private const float RadToDeg = (float)(180.0 / Math.PI);
#endif
        //Simple distance calculations, speedup calculations for trail detection
        //Do not override standard DistanceMetersToPoint(), slower and used in certain places
        //Use (quite accurate) aproximate scaled squared distance instead of real distance, improve performance
        //Actually, also the EarthRadius * DegToRad * (float)Math.Sqrt(result) could be eliminated,
        //but this requires that all compare values must be converted to "scaled squared format" too. Very small improvement.

        //Algorithm is for short distance, the result should be accurate
        //See http://en.wikipedia.org/wiki/Geographical_distance

        private const float invalidLatLon = 999;
        private float _cosmean = invalidLatLon;
        private const float DegToRad = (float)(Math.PI / 180.0);
        private const float EarthRadius = 6371009;
#endif

        //Note: Faster to keep this static then 
        public static float DistanceMetersToPointSimple(TrailGPSLocation trailp, IGPSPoint point)
        {
#if NO_SIMPLE_DISTANCE
            return trailp.DistanceMetersToPoint(point);
#else
            //Use the trailp lat instead of average lat
            if (trailp._cosmean == invalidLatLon) { trailp._cosmean = (float)Math.Cos(trailp.latitudeDegrees * DegToRad); }
            float dlat = point.LatitudeDegrees - trailp.latitudeDegrees;
            float dlon = point.LongitudeDegrees - trailp.longitudeDegrees;
            float result = dlat * dlat + dlon * dlon * trailp._cosmean;

#if SQUARE_DISTANCE
            return result;
#else
            return EarthRadius * DegToRad * (float)Math.Sqrt(result);
#endif
#endif
        }

        public static float DistanceMetersToPointGpsSimple(IGPSPoint point, IGPSPoint point2)
        {
#if NO_SIMPLE_DISTANCE
            return point.DistanceMetersToPoint(point2);
#else
            float _cosmean = (float)Math.Cos(point2.LatitudeDegrees * DegToRad);
            float dlat = point.LatitudeDegrees - point2.LatitudeDegrees;
            float dlon = point.LongitudeDegrees - point2.LongitudeDegrees;
            float result = dlat * dlat + dlon * dlon * _cosmean;

#if SQUARE_DISTANCE
            return result;
#else
            return EarthRadius * DegToRad * (float)Math.Sqrt(result);
#endif
#endif
        }

        public static float checkPass(float radius, IGPSPoint r1, float dt1, IGPSPoint r2, float dt2, TrailGPSLocation trailp, out float d)
        {
#if SQUARE_DISTANCE
            const int sqrt2 = 2;
#else
            //Optimise, accuracy is down to percent
            const float sqrt2 = 1.4142135623730950488016887242097F;
#endif
            d = float.MaxValue;
            float factor = -1;
            if (r1 == null || r2 == null || trailp == null) return factor;

            //Check if the line goes via the "circle" if the sign changes
            //Also need to check close points that fit in a 45 deg tilted "square" where sign may not change

            //Optimise for all conditions tested - property access takes some time
            float tplat = trailp.latitudeDegrees;
            float tplon = trailp.longitudeDegrees;
            float r1lat = r1.LatitudeDegrees;
            float r1lon = r1.LongitudeDegrees;
            float r2lat = r2.LatitudeDegrees;
            float r2lon = r2.LongitudeDegrees;
            if (r1lat > tplat && r2lat < tplat ||
                r1lat < tplat && r2lat > tplat ||
                r1lon > tplon && r2lon < tplon ||
                r1lon < tplon && r2lon > tplon ||
                dt1 < radius * sqrt2 && dt2 < radius * sqrt2)
            {
                //Law of cosines - get a1, angle at r1, the first point
                float d12 = TrailGPSLocation.DistanceMetersToPointGpsSimple(r1, r2);
#if SQUARE_DISTANCE
                float a1_0 = (float)((dt1 + d12 - dt2) / (2 * Math.Sqrt(dt1 * d12)));
#else
                float a1_0 = (dt1 * dt1 + d12 * d12 - dt2 * dt2) / (2 * dt1 * d12);
#endif

                //Point is in circle if closest point is between r1&r2 and it is in circle (neither r1 nor r2 is)
                //This means the angle a1 must be +/- 90 degrees : cos(a1)>=0
                if (a1_0 > -0.001)
                {
                    //Rounding errors w GPS measurements
                    a1_0 = Math.Min(a1_0, 1);
                    a1_0 = Math.Max(a1_0, -1);
                    float a1 = (float)Math.Acos(a1_0);
                    //x is closest point to t on r1-r2 
                    //Dist from r1 to x
                    float d1x = (float)Math.Abs(dt1 * a1_0); //a1_0 = (float)Math.Cos(a1);
                    //Dist from t1 to x
                    float dtx = dt1 * (float)Math.Sin(a1);
                    if (d1x < d12 && dtx < radius)
                    {
                        d = dtx;
                        factor = (float)(d1x / d12);
                        //Return factor, to return best aproximation from r1
                    }
                }
            }
            return factor;
        }

/*****************************************************************************/
        public static GPSBounds getGPSBounds(IList<TrailGPSLocation> list, float radius)
        {
            return getGPSBounds(list, radius, false);
        }

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
                if ((g.Required || !requiredCheck || !enoughRequired))
                {
                    float glat = g.latitudeDegrees;
                    float glon = g.longitudeDegrees;

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
    }
}
