using System.Collections.Generic;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Fitness;
using System.Xml;

namespace TrailsPlugin.Data {
	public class Trail {
		public string Name;
		private IList<TrailGPSLocation> m_trailLocations = new List<TrailGPSLocation>();

		public IList<TrailGPSLocation> TrailLocations {
			get {
				return m_trailLocations;
			}
		}

		static public Trail FromXml(XmlNode node) {
			Trail trail = new Trail();
			trail.Name = node.Attributes["name"].Value;
			trail.TrailLocations.Clear();
			foreach (XmlNode TrailGPSLocationNode in node.ChildNodes) {
				trail.TrailLocations.Add(TrailGPSLocation.FromXml(TrailGPSLocationNode));
			}
			return trail;
		}

		public XmlNode ToXml(XmlDocument doc) {
			XmlNode trailNode = doc.CreateElement("Trail");
			XmlAttribute a = doc.CreateAttribute("name");
			a.Value = this.Name;
			trailNode.Attributes.Append(a);
			foreach (TrailGPSLocation point in this.TrailLocations) {
				trailNode.AppendChild(point.ToXml(doc));
			}
			return trailNode;
		}

		public bool IsInBounds(IGPSBounds gpsBounds) {
			foreach (TrailGPSLocation trailGPSLocation in this.TrailLocations) {
				if (!gpsBounds.Contains(trailGPSLocation)) {
					return false;
				}
			}
			return true;
		}

		public IList<TrailResult> Results(IActivity activity) {
			IList<TrailResult> resultsList = new List<TrailResult>();
			if (activity.GPSRoute == null || activity.GPSRoute.Count == 0) {
				return resultsList ;
			}			

			float radius = 45;
			int trailIndex = 0;
			int startIndex = -1, endIndex = -1;
			
			for (int routeIndex = 0; routeIndex < activity.GPSRoute.Count; routeIndex++) {
				IGPSPoint routePoint = activity.GPSRoute[routeIndex].Value;
				if (trailIndex != 0) {
					float distFromStartToPoint = this.TrailLocations[0].DistanceMetersToPoint(routePoint);
					if (distFromStartToPoint < radius) {
						trailIndex = 0;
					}
				}
				float distToPoint = this.TrailLocations[trailIndex].DistanceMetersToPoint(routePoint);
				if (distToPoint < radius) {
					for (int routeIndex2 = routeIndex+1; routeIndex2 < activity.GPSRoute.Count; routeIndex2++) {
						IGPSPoint routePoint2 = activity.GPSRoute[routeIndex2].Value;
						float distToPoint2 = this.TrailLocations[0].DistanceMetersToPoint(routePoint2);
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

					} else if (trailIndex == this.TrailLocations.Count - 1) {
						// found the end
						endIndex = routeIndex;
						TrailResult result = new TrailResult(activity, resultsList.Count + 1, startIndex, endIndex);
						resultsList.Add(result);
						result = null;
						trailIndex = 0;
					} else {
						// found a mid point
						trailIndex++;
					}
				}
			}
			return resultsList;


			/*
			SplitPoints.SplitPoint splitPoint1 = null;
			IGPSBounds igpsbounds = GPSBounds.FromGPSRoute(activity.GPSRoute);
			IList<SplitPoints.SplitPoint> ilist = new List<SplitPoints.SplitPoint>();
			int i = 0;
			foreach (SplitPoints.SplitPoint splitPoint2 in MySplitPoints) {
				if ((PluginBase.TheInstance.Payments.SumOfValidatedPayments <= 8.5F) && (i >= 1)) {
					MessageDialog.Show(Resources.CreateSplitsAction_UsingMoreThanSplitPointIsAvailableForDonatorsWho, PluginBase.TheInstance.Name, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					break;
				}
				if (igpsbounds.Contains(splitPoint2.GPSLocation))
					ilist.Add(splitPoint2);
				i++;
			}
			IList<ITimeValueEntry<IGPSPoint>> ilist1 = new List<ITimeValueEntry<IGPSPoint>>();
			using (IEnumerator<ITimeValueEntry<IGPSPoint>> ienumerator = activity.GPSRoute.GetEnumerator()) {
				while (ienumerator.MoveNext()) {
					ITimeValueEntry<IGPSPoint> itimeValueEntry = ienumerator.get_Current();
					IGPSPoint igpspoint = itimeValueEntry.get_Value();
					if ((splitPoint1 != null) && (Calculations.GetDistance(igpspoint, splitPoint1.GPSLocation) > (double)splitPoint1.RadiusMeters)) {
						ilist1.Add(itimeValueEntry);
						SetLapPointForPointNearest(ilist1, splitPoint1, activity.GPSRoute, actionResult);
						splitPoint1 = null;
						ilist1.Clear();
					}
					if (splitPoint1 != null) {
						ilist1.Add(itimeValueEntry);
					} else {
						using (IEnumerator<SplitPoints.SplitPoint> ienumerator2 = ilist.GetEnumerator()) {
							while (ienumerator2.MoveNext()) {
								SplitPoints.SplitPoint splitPoint3 = ienumerator2.get_Current();
								double d = Calculations.GetDistance(igpspoint, splitPoint3.GPSLocation);
								if (d <= (double)splitPoint3.RadiusMeters) {
									if ((PluginBase.TheInstance.Payments.SumOfValidatedPayments <= 8.5F) && (actionResult.Splits.get_Count() >= 2)) {
										MessageDialog.Show(Resources.CreateSplitsAction_AutomaticallyCreatingMoreThan2SplitsIsAvailableForDonatorsWho, PluginBase.TheInstance.Name, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
										return;
									}
									ilist1.Add(itimeValueEntry);
									splitPoint1 = splitPoint3;
									break;
								}
							}
						}
					}
				}
			}
			if (splitPoint1 != null)
			SetLapPointForPointNearest(ilist1, splitPoint1, activity.GPSRoute, actionResult);
			 * */
		}

	}
}

/*
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OldManBiking.AfterImportPlugin.Data;
using OldManBiking.AfterImportPlugin.Properties;
using OldManBiking.AfterImportPlugin.UI.Actions;
using OldManBiking.SporttracksPlugins;
using OldManBiking.SporttracksPlugins.Actions;
using OldManBiking.SporttracksPlugins.Geodesics;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Visuals;

namespace OldManBiking.AfterImportPlugin.Actions
{

    internal class CreateSplitsAction : ActionBase
    {

        public enum Mode
        {
            StopWhenSplitsExist,
            AddToExistingSplits,
            ReplaceExistingSplits
        }

        private class ActionResult
        {

            internal IActivity Activity;
            private SortedList<DateTime,IValueRange<DateTime>> MySplits;

            internal IList<IValueRange<DateTime>> Splits
            {
                get
                {
                    return MySplits.get_Values();
                }
            }

            internal ActionResult(IActivity activity)
            {
                Activity = activity;
                MySplits = new SortedList<DateTime,IValueRange<DateTime>>();
            }

            internal void AddSplit(DateTime startTime, float durationSeconds)
            {
                IValueRange<DateTime> ivalueRange = new ValueRange<DateTime>(startTime, !Single.IsNaN(durationSeconds) ? startTime.AddSeconds((double)durationSeconds) : DateTime.MaxValue);
                if (!MySplits.ContainsKey(startTime))
                    MySplits.Add(startTime, ivalueRange);
            }

        } // class ActionResult

        private CreateSplitsActionForm MyActionForm;
        private IList<CreateSplitsAction.ActionResult> MyActionResults;
        private CreateSplitsAction.Mode MyMode;
        private SplitPoints MySplitPoints;

        public SortedList<IActivity,int> SplitsForActivities
        {
            get
            {
                if (MyActionResults == null)
                    return null;
                SortedList<IActivity,int> sortedList = new SortedList<IActivity,int>();
                using (IEnumerator<CreateSplitsAction.ActionResult> ienumerator = MyActionResults.GetEnumerator())
                {
                    while (ienumerator.MoveNext())
                    {
                        CreateSplitsAction.ActionResult actionResult = ienumerator.get_Current();
                        if (actionResult.Splits != null)
                            sortedList.Add(actionResult.Activity, actionResult.Splits.get_Count());
                    }
                }
                if (!sortedList.get_Count())
                    return null;
                return sortedList;
            }
        }

        public override Image Image
        {
            get
            {
                return Resources.Cut;
            }
        }

        public override string Title
        {
            get
            {
                return Resources.CreateSplitsAction_AutomaticallyCreateSplits;
            }
        }

        public CreateSplitsAction(IActivity activity) : this(iactivityArr)
        {
            IActivity[] iactivityArr = new IActivity[] { activity };
        }

        public CreateSplitsAction(IList<IActivity> activities) : base(activities)
        {
            MyActivities = activities;
            MySplitPoints = SplitPoints.Instance;
            MyMode = AfterImportMiscSettings.TheInstance.CreateSplitsMode;
            MyActionResults = new List<CreateSplitsAction.ActionResult>();
        }

        private void CreateSplits(IActivity activity, CreateSplitsAction.ActionResult actionResult)
        {
            if ((activity.GPSRoute == null) || !activity.GPSRoute.get_Count())
                return;
            if ((MyMode == CreateSplitsAction.Mode.StopWhenSplitsExist) && (activity.Laps != null) && (activity.Laps.Count > 0))
                return;
            SplitPoints.SplitPoint splitPoint1 = null;
            IGPSBounds igpsbounds = GPSBounds.FromGPSRoute(activity.GPSRoute);
            IList<SplitPoints.SplitPoint> ilist = new List<SplitPoints.SplitPoint>();
            int i = 0;
            foreach (SplitPoints.SplitPoint splitPoint2 in MySplitPoints)
            {
                if ((PluginBase.TheInstance.Payments.SumOfValidatedPayments <= 8.5F) && (i >= 1))
                {
                    MessageDialog.Show(Resources.CreateSplitsAction_UsingMoreThanSplitPointIsAvailableForDonatorsWho, PluginBase.TheInstance.Name, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    break;
                }
                if (igpsbounds.Contains(splitPoint2.GPSLocation))
                    ilist.Add(splitPoint2);
                i++;
            }
            IList<ITimeValueEntry<IGPSPoint>> ilist1 = new List<ITimeValueEntry<IGPSPoint>>();
            using (IEnumerator<ITimeValueEntry<IGPSPoint>> ienumerator = activity.GPSRoute.GetEnumerator())
            {
                while (ienumerator.MoveNext())
                {
                    ITimeValueEntry<IGPSPoint> itimeValueEntry = ienumerator.get_Current();
                    IGPSPoint igpspoint = itimeValueEntry.get_Value();
                    if ((splitPoint1 != null) && (Calculations.GetDistance(igpspoint, splitPoint1.GPSLocation) > (double)splitPoint1.RadiusMeters))
                    {
                        ilist1.Add(itimeValueEntry);
                        SetLapPointForPointNearest(ilist1, splitPoint1, activity.GPSRoute, actionResult);
                        splitPoint1 = null;
                        ilist1.Clear();
                    }
                    if (splitPoint1 != null)
                    {
                        ilist1.Add(itimeValueEntry);
                    }
                    else
                    {
                        using (IEnumerator<SplitPoints.SplitPoint> ienumerator2 = ilist.GetEnumerator())
                        {
                            while (ienumerator2.MoveNext())
                            {
                                SplitPoints.SplitPoint splitPoint3 = ienumerator2.get_Current();
                                double d = Calculations.GetDistance(igpspoint, splitPoint3.GPSLocation);
                                if (d <= (double)splitPoint3.RadiusMeters)
                                {
                                    if ((PluginBase.TheInstance.Payments.SumOfValidatedPayments <= 8.5F) && (actionResult.Splits.get_Count() >= 2))
                                    {
                                        MessageDialog.Show(Resources.CreateSplitsAction_AutomaticallyCreatingMoreThan2SplitsIsAvailableForDonatorsWho, PluginBase.TheInstance.Name, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                        return;
                                    }
                                    ilist1.Add(itimeValueEntry);
                                    splitPoint1 = splitPoint3;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (splitPoint1 != null)
                SetLapPointForPointNearest(ilist1, splitPoint1, activity.GPSRoute, actionResult);
        }

        internal void SaveResults()
        {
            // trial
        }

        private void SetLapPointForPointNearest(IList<ITimeValueEntry<IGPSPoint>> trackPointsWithinCircle, SplitPoints.SplitPoint activeSplitPoint, IGPSRoute iGPSRoute, CreateSplitsAction.ActionResult actionResult)
        {
            // trial
        }

        public override void DoWork(IJobMonitor jobMonitor, out bool canBeCancelled, out bool leaveStatusText)
        {
            base.DoWork(jobMonitor, out canBeCancelled, out leaveStatusText);
            canBeCancelled = true;
            leaveStatusText = true;
            int i = 1;
            using (IEnumerator<IActivity> ienumerator = MyActivities.GetEnumerator())
            {
                while (ienumerator.MoveNext())
                {
                    IActivity iactivity = ienumerator.get_Current();
                    try
                    {
                        CreateSplitsAction.ActionResult actionResult = new CreateSplitsAction.ActionResult(iactivity);
                        DateTime dateTime = iactivity.StartTime;
                        string s = String.Format(Resources.CreateSplitsAction_ActivityStartingAt + " {0:d} {0:HH:mm}", dateTime.ToLocalTime());
                        jobMonitor.StatusText = s;
                        CreateSplits(iactivity, actionResult);
                        jobMonitor.StatusText = String.Format("{0}: " + Resources.CreateSplitsAction_Found, s, actionResult.Splits.get_Count(), actionResult.Splits.get_Count() == 1 ? Resources.General_Split : Resources.General_Splits);
                        MyActionResults.Add(actionResult);
                    }
                    finally
                    {
                        MyJobMonitor.PercentComplete = 100.0F * (float)i / (float)MyActivities.get_Count();
                        i++;
                    }
                }
            }
        }

        public override bool IsEnabled(IActivity activity)
        {
            if (activity.GPSRoute != null)
                return activity.GPSRoute.get_Count() > 0;
            return false;
        }

        public override void Run(Rectangle rectButton)
        {
            // trial
        }

    } // class CreateSplitsAction

}

*/