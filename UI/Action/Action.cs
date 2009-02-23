﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
//using MatrixPlugin.Properties;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;

namespace TrailsPlugin {
	internal class Action : IAction, INotifyPropertyChanged {
		private IList<IActivity> m_activities;
		public event PropertyChangedEventHandler PropertyChanged;

		public Action(IList<IActivity> activities) {
			this.m_activities = activities;
		}

		public void Refresh() {
		}

		public void Run(Rectangle rectButton) {
			new TrailsViewer(m_activities);
			//new TableViewer(activities, true);
		}

		public bool Enabled {
			get { return true; }
		}

		public bool HasMenuArrow {
			get { return false; }
		}

		public System.Drawing.Image Image {
			get {
				return null;
				//return Resources.Windows_Table_16x16; 
			}
		}

		public string Title {
			get {
				if (m_activities.Count == 1)
					return "Matrix for one activity";
				else
					return "Matrix for " + m_activities.Count + " activities";
			}
		}
	}
}

