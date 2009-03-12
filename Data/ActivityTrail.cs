using System;
using System.Collections.Generic;
using System.Text;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;

namespace TrailsPlugin.Data {
	class ActivityTrail {
		private IActivity m_activity;
		private Data.Trail m_trail;
		private IList<Data.TrailResult> m_resultsList;

		public ActivityTrail(IActivity activity, Data.Trail trail) {
			m_activity = activity;
			m_trail = trail;

		}
		public IActivity Activity {
			get {
				return m_activity;
			}
		}
		public Data.Trail Trail {
			get {
				return m_trail;
			}
		}

		public IList<TrailResult> Results {
			get {
				if (m_resultsList == null) {
					m_resultsList = new List<TrailResult>();
					if (m_activity.GPSRoute != null && m_activity.GPSRoute.Count > 0) {

						int trailIndex = 0;
						int startIndex = -1, endIndex = -1;

						for (int routeIndex = 0; routeIndex < m_activity.GPSRoute.Count; routeIndex++) {
							IGPSPoint routePoint = m_activity.GPSRoute[routeIndex].Value;
							if (trailIndex != 0) {
								float distFromStartToPoint = this.m_trail.TrailLocations[0].DistanceMetersToPoint(routePoint);
								if (distFromStartToPoint < this.m_trail.Radius) {
									trailIndex = 0;
								}
							}

							float distToPoint = this.m_trail.TrailLocations[trailIndex].DistanceMetersToPoint(routePoint);
							if (distToPoint < this.Trail.Radius) {
								for (int routeIndex2 = routeIndex + 1; routeIndex2 < m_activity.GPSRoute.Count; routeIndex2++) {
									IGPSPoint routePoint2 = m_activity.GPSRoute[routeIndex2].Value;
									float distToPoint2 = this.m_trail.TrailLocations[0].DistanceMetersToPoint(routePoint2);
									if (distToPoint2 > distToPoint) {
										break;
									} else {
										distToPoint = distToPoint2;
										routeIndex = routeIndex2;
									}
								}
								if (trailIndex == 0) {
									// found the start						
									startIndex = routeIndex;
									trailIndex++;

								} else if (trailIndex == this.m_trail.TrailLocations.Count - 1) {
									// found the end
									endIndex = routeIndex;
									TrailResult result = new TrailResult(m_activity, m_resultsList.Count + 1, startIndex, endIndex);
									m_resultsList.Add(result);
									result = null;
									trailIndex = 0;
								} else {
									// found a mid point
									trailIndex++;
								}
							}
						}
					}
				}
				return m_resultsList;
			}
		}
	}
}
