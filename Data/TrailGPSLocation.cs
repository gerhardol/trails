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

using ZoneFiveSoftware.Common.Data.GPS;
using System.Xml;
using System;

namespace TrailsPlugin.Data {
	public class TrailGPSLocation : GPSLocation {
		public TrailGPSLocation(float latitudeDegrees, float longitudeDegrees, string name) : base(latitudeDegrees, longitudeDegrees)
        {
            this.name = name;
        }
		static public TrailGPSLocation FromXml(XmlNode node) {

            string name = "";
            if (null != node.Attributes["name"] &&
                null != node.Attributes["name"].Value)
            {
                name = node.Attributes["name"].Value;
            }
			return new TrailGPSLocation(
				float.Parse(node.Attributes["latitude"].Value), 
				float.Parse(node.Attributes["longitude"].Value),
                name
			);
        }
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                this.name = value;
            }
        }

		public XmlNode ToXml(XmlDocument doc) {
			XmlNode TrailGPSLocationNode = doc.CreateElement("TrailGPSLocation");
			XmlAttribute a = doc.CreateAttribute("latitude");
			a.Value = this.LatitudeDegrees.ToString();
			TrailGPSLocationNode.Attributes.Append(a);
            a = doc.CreateAttribute("longitude");
            a.Value = this.LongitudeDegrees.ToString();
            TrailGPSLocationNode.Attributes.Append(a);
            a = doc.CreateAttribute("name");
            a.Value = this.Name.ToString();
            TrailGPSLocationNode.Attributes.Append(a);
            return TrailGPSLocationNode;
		}

		public float DistanceMetersToPoint(IGPSPoint point) {
			GPSPoint thisPoint = new GPSPoint(
				this.LatitudeDegrees,
				this.LongitudeDegrees,
				0);
			return point.DistanceMetersToPoint(thisPoint);
		}
        // Some temporary handling, no bother proper
        public string getField(int subItemSelected)
        {
            string subItemText;
            switch (subItemSelected)
            {
                case 0:
                    subItemText = this.LongitudeDegrees.ToString();
                    break;
                case 1:
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
            if (subItemSelected < 2)
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
                    || subItemSelected == 0 && 180 < Math.Abs(pos)
                    || subItemSelected == 1 && 90 < Math.Abs(pos)
                    )
                {
                    valid = 0;
                }
            }

            if (valid > 0)
            {
                switch (subItemSelected)
                {
                    case 0:
                        result = new TrailGPSLocation(pos, this.LatitudeDegrees, this.name);
                        break;
                    case 1:
                        result = new TrailGPSLocation(this.LongitudeDegrees, pos, this.name);
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
