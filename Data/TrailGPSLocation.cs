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
        //If this is related from a track, save the time
        //DateTime? m_dateTime;

        //public TrailGPSLocation(DateTime time, ITimeValueEntry<IGPSPoint> t, String name, bool required)
        //    : this(t.Value.LatitudeDegrees, t.Value.LongitudeDegrees, name, true)
        //{
        //    //m_dateTime = time;
        //}
        public TrailGPSLocation(IGPSLocation gpsLoc, string name, bool required)
        {
            this._gpsLocation = gpsLoc;
            this._name = name;
            this._required = required;
        }
        public TrailGPSLocation(IGPSLocation gpsLoc)
        {
            this._gpsLocation = gpsLoc;
            this._name = "";
            this._required = true;
        }
        
        public static IGPSLocation getGpsLoc(ZoneFiveSoftware.Common.Data.Fitness.IActivity activity, DateTime time)
        {
            IGPSLocation result = null;
            if (activity != null && activity.GPSRoute != null)
            {
                ITimeValueEntry<IGPSPoint> p = activity.GPSRoute.GetInterpolatedValue(time);
                if (null != p)
                {
                    result = p.Value;// new GPSLocation(p.Value.LatitudeDegrees, p.Value.LongitudeDegrees);
                }
            }
            return result;
        }
        //public TrailGPSLocation(float latitudeDegrees, float longitudeDegrees, string name, bool required, float radius)
        //    : this(latitudeDegrees, longitudeDegrees, name, required)
        //{
        //    this._radius = radius;
        //}
        public TrailGPSLocation(TrailGPSLocation trailLocation)
        {
            this._gpsLocation = trailLocation._gpsLocation;// new GPSLocation(trailLocation._gpsLocation.LatitudeDegrees, trailLocation._gpsLocation.LongitudeDegrees);
            this._name = trailLocation._name;
            this._required = trailLocation._required;
            this._radius = trailLocation._radius;
        }

        public override string ToString()
        {
            return _name + " " + _required + " " + _gpsLocation;
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

        private IGPSLocation _gpsLocation;
        public IGPSLocation GpsLocation
        {
            get
            {
                return _gpsLocation;
            }
            set
            {
                _gpsLocation = value;
            }
        }
        //TBD optimize cache location
        public float LatitudeDegrees
        {
            get
            {
                return _gpsLocation.LatitudeDegrees;
            }
        }
        public float LongitudeDegrees
        {
            get
            {
                return _gpsLocation.LongitudeDegrees;
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
        //public DateTime? DateTime
        //{
        //    get
        //    {
        //        return m_dateTime;
        //    }
        //}
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
            return new TrailGPSLocation(new GPSLocation(
                Settings.parseFloat(node.Attributes[xmlTags.sLatitude].Value),
                Settings.parseFloat(node.Attributes[xmlTags.sLongitude].Value)),
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
            public const string sName = "name";
            public const string sRequired = "required";
        }

		public float DistanceMetersToPoint(IGPSPoint point) {
			GPSPoint thisPoint = new GPSPoint(
				this.LatitudeDegrees,
				this.LongitudeDegrees,
				0);
            if (point == null)
            {
                return float.MaxValue;
            }
			return point.DistanceMetersToPoint(thisPoint);
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
                if ((g.Required || !requiredCheck || !enoughRequired) && g.GpsLocation != null)
                {
                    north = Math.Max(north, g.GpsLocation.LatitudeDegrees);
                    south = Math.Min(south, g.GpsLocation.LatitudeDegrees);
                    east = Math.Max(east, g.GpsLocation.LongitudeDegrees);
                    west = Math.Min(west, g.GpsLocation.LongitudeDegrees);
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
            east += lng;
            west -= lng;
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
                        _gpsLocation = new GPSLocation(this.LatitudeDegrees, pos);
                        break;
                    case 2:
                        _gpsLocation = new GPSLocation(pos, this.LongitudeDegrees);
                        break;
                    default:
                        this.Name = s;
                        break;
                }
            }
            return result;
        }
	}
}
