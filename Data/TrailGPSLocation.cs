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

using ZoneFiveSoftware.Common.Data.GPS;
using System.Xml;
using System;
using System.Collections.Generic;

namespace TrailsPlugin.Data {
	public class TrailGPSLocation {
        public TrailGPSLocation(float latitudeDegrees, float longitudeDegrees, string name)
            : this(latitudeDegrees, longitudeDegrees, name, true)
        {
        }
		public TrailGPSLocation(float latitudeDegrees, float longitudeDegrees, string name, bool required)
        {
            this._gpsLocation = new GPSLocation(latitudeDegrees, longitudeDegrees);
            this._name = name;
            this._required = required;
        }

        public override string ToString()
        {
            return _name + " " + _required + " " + _gpsLocation;
        }

        private GPSLocation _gpsLocation;
        public GPSLocation GpsLocation
        {
            get
            {
                return _gpsLocation;
            }
        }
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

        //This code is shared in other plugins
#if TRAILSPLUGIN
        static public TrailGPSLocation FromXml(XmlNode node)
        {
            string name = "";
            if (null != node.Attributes[xmlTags.sName] &&
                null != node.Attributes[xmlTags.sName].Value)
            {
                name = node.Attributes[xmlTags.sName].Value;
            }
            bool required = true;
            if (null != node.Attributes[xmlTags.sRequired] &&
                null != node.Attributes[xmlTags.sRequired].Value)
            {
                required = XmlConvert.ToBoolean(node.Attributes[xmlTags.sRequired].Value);
            }
            return new TrailGPSLocation(
                Settings.parseFloat(node.Attributes[xmlTags.sLatitude].Value),
                Settings.parseFloat(node.Attributes[xmlTags.sLongitude].Value),
                name, required
            );
        }
#endif

        public XmlNode ToXml(XmlDocument doc)
        {
			XmlNode TrailGPSLocationNode = doc.CreateElement(xmlTags.sTrailGPSLocation);
			XmlAttribute a = doc.CreateAttribute(xmlTags.sLatitude);
			a.Value = this.LatitudeDegrees.ToString();
			TrailGPSLocationNode.Attributes.Append(a);
            a = doc.CreateAttribute(xmlTags.sLongitude);
            a.Value = this.LongitudeDegrees.ToString();
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
        public static GPSBounds getGPSBounds(IList<TrailGPSLocation> list, float radius, bool requiredCheck)
        {
            float north = -90;
            float south = +90;
            float east = -180;
            float west = 180;
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
            foreach (TrailGPSLocation g in list)
            {
                if (g.Required || !requiredCheck || noRequired < minRequired)
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
            if (north < south || noRequired < minRequired)
            {
                float tmp = (north+south)/2;
                north = tmp;
                south = tmp;
            }
            if (east < west || noRequired < minRequired)
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
                if (float.NaN == pos
                    || subItemSelected == 1 && 90 < Math.Abs(pos)
                    || subItemSelected == 2 && 180 < Math.Abs(pos)
                    )
                {
                    valid = 0;
                }
            }

            if (valid > 0)
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
