using System.Collections.Generic;
using ZoneFiveSoftware.Common.Data.Fitness;
using System.Xml;
using System.Xml.Serialization;
using ZoneFiveSoftware.Common.Data.GPS;

namespace TrailsPlugin.Data {
	public class TrailData {

		private SortedList<string, Data.Trail> m_AllTrails = new SortedList<string, Data.Trail>();

		/*
		public IList<string> TrailNames {
			get {
				SortedList<long, string> sortedNames = new SortedList<long, string>();
				IGPSBounds gpsBounds = GPSBounds.FromGPSRoute(m_activity.GPSRoute);
				foreach (Data.Trail trail in PluginMain.Data.AllTrails.Values) {
					if (trail.IsInBounds(gpsBounds)) {
						IList<Data.TrailResult> results = trail.Results(m_activity);
						if (results.Count > 0) {
							sortedNames.Add(results[0].StartTime.Ticks, trail.Name);
						} else
					}
				}
				IList<string> names = new List<string>();
				foreach (string name in sortedNames.Values) {
					names.Add(name);
				}
				return names;
			}
		}
		*/
		public SortedList<string, Data.Trail> AllTrails {
			get {
				return m_AllTrails;
			}
		}

		public IList<Data.Trail> TrailsInBounds(IActivity activity) {
			IList<Data.Trail> trails = new List<Data.Trail>();

			IGPSBounds gpsBounds = GPSBounds.FromGPSRoute(activity.GPSRoute);
			foreach (Data.Trail trail in PluginMain.Data.AllTrails.Values) {
				if (trail.IsInBounds(gpsBounds)) {
					trails.Add(trail);
				}
			}

			return trails;
		}


		public SortedList<long, Data.Trail> TrailsWithResults(IActivity activity) {
			SortedList<long, Data.Trail> trails = new SortedList<long, Data.Trail>();
			foreach (Data.Trail trail in TrailsInBounds(activity)) {
				IList<Data.TrailResult> results = trail.Results(activity);
				if (results.Count > 0) {
					trails.Add(results[0].StartTime.Ticks, trail);
				}
			}
			return trails;

		}
		public bool InsertTrail(Data.Trail trail) {
			if (m_AllTrails.ContainsKey(trail.Name)) {
				return false;
			} else {
				m_AllTrails.Add(trail.Name, trail);
				return true;
			}
		}
		public bool UpdateTrail(string oldKey, Data.Trail trail) {
			if (m_AllTrails.ContainsKey(oldKey)) {
				m_AllTrails.Remove(oldKey);
				m_AllTrails.Add(trail.Name, trail);
				return true;
			} else {
				return false;
			}
		}
		public bool DeleteTrail(Data.Trail trail) {
			if (m_AllTrails.ContainsKey(trail.Name)) {
				m_AllTrails.Remove(trail.Name);
				return true;
			} else {
				return false;
			}
		}

		public void FromXml(XmlNode pluginNode, XmlNamespaceManager nsmgr) {
			m_AllTrails.Clear();
			foreach (XmlNode node in pluginNode.SelectNodes("trail:Trails/trail:Trail", nsmgr)) {
				Data.Trail trail = Data.Trail.FromXml(node, nsmgr);
				m_AllTrails.Add(trail.Name, trail);
			}

		}

		public XmlNode ToXml(XmlDocument doc) {
			XmlNode trails = doc.CreateElement("Trails");
			foreach (Data.Trail trail in PluginMain.Data.AllTrails.Values) {
				trails.AppendChild(trail.ToXml(doc));
			}
			return trails;
		}
	}
}
