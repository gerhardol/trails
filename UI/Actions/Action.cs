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
using System.ComponentModel;
using System.Drawing;
//using MatrixPlugin.Properties;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;

namespace TrailsPlugin.UI.Actions {
	internal class Action : IAction {
		private IList<IActivity> m_activities;
#pragma warning disable 67
        public event PropertyChangedEventHandler PropertyChanged;

		public Action(IList<IActivity> activities) {
			this.m_activities = activities;
		}

		public void Refresh() {
		}

		public void Run(Rectangle rectButton) {
			//new UI.Activity.ActivityDetailPageControl(m_activities);
			//new TableViewer(activities, true);
		}

		public bool Enabled {
			get { return false; }
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

        public IList<string> MenuPath
        {
            get
            {
                return new List<string>();
            }
        }
        public string Title
        {
			get {
				return Properties.Resources.UI_Action_Title;
			}
		}
        private bool firstRun = true; 
        public bool Visible
        {
            get
            {
                //Analyze menu must be Visible at first call, otherwise it is hidden
                //Could be done with listeners too
                if (true == firstRun) {firstRun=false; return true;}
                if (m_activities.Count == 0) return false;
                return true;
            }
        }
    }
}

