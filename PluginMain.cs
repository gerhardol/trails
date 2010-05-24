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

using System;
using System.ComponentModel;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using System.Xml;

namespace TrailsPlugin {
	class PluginMain : IPlugin {
		#region IPlugin Members

		public IApplication Application {
			set {
				if (m_App != null) {
					m_App.PropertyChanged -= new PropertyChangedEventHandler(AppPropertyChanged);
				}

				m_App = value;

				if (m_App != null) {
					m_App.PropertyChanged += new PropertyChangedEventHandler(AppPropertyChanged);
				}
			}
		}

		public System.Guid Id {
			get {
				return GUIDs.PluginMain;
			}
		}

		public string Name {
			get {
				return Properties.Resources.TrailsPluginName;
			}
		}

		public string Version {
			get { return GetType().Assembly.GetName().Version.ToString(4); }
		}

		public void ReadOptions(XmlDocument xmlDoc, XmlNamespaceManager nsmgr, XmlElement pluginNode) {
            String attr;
            attr = pluginNode.GetAttribute(xmlTags.Verbose);
            if (attr.Length > 0) { Verbose = XmlConvert.ToInt16(attr); }
            attr = pluginNode.GetAttribute(xmlTags.settingsVersion);
            if (attr.Length > 0) { settingsVersion = (Int16)XmlConvert.ToInt16(attr); }
            //If not found (or version lower), settings will be loaded dynamically (from logbook)
            if (settingsVersionCurrent <= settingsVersion)
            {
                settingsVersion = settingsVersionCurrent;
                m_settings = new TrailsPlugin.Data.Settings();
                m_data = new TrailsPlugin.Data.TrailData();
                TrailsPlugin.Data.Settings.ReadOptions(xmlDoc, nsmgr, pluginNode);
                TrailsPlugin.Data.TrailData.ReadOptions(xmlDoc, nsmgr, pluginNode);
            }
            //Set version so Preferences are loaded next time
            if (0 == settingsVersion)
            {
                settingsVersion = settingsVersionCurrent;
            }
        }

		public void WriteOptions(XmlDocument xmlDoc, XmlElement pluginNode) {
            pluginNode.SetAttribute(xmlTags.Verbose, XmlConvert.ToString(Verbose));
            pluginNode.SetAttribute(xmlTags.settingsVersion, XmlConvert.ToString(settingsVersion));
            TrailsPlugin.Data.Settings.WriteOptions(xmlDoc, pluginNode);
            TrailsPlugin.Data.TrailData.WriteOptions(xmlDoc, pluginNode);
        }

		#endregion

        public static void ReadExtensionData()
        {
            m_data = new TrailsPlugin.Data.TrailData();
            m_settings = new TrailsPlugin.Data.Settings();

            XmlDocument doc = new XmlDocument();
            string xml = PluginMain.GetApplication().Logbook.GetExtensionText(GUIDs.PluginMain);
            if (xml == "")
            {
                xml = "<TrailsPlugin/>";
            }
            doc.LoadXml(xml);
            m_data.FromXml(doc.DocumentElement);
            m_settings.FromXml(doc.DocumentElement);
        }

		public static void WriteExtensionData() 
        {
            //Normally not used
            if (settingsVersionCurrent > settingsVersion)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<TrailsPlugin/>");
                doc.DocumentElement.AppendChild(m_data.ToXml(doc));
                doc.DocumentElement.AppendChild(m_settings.ToXml(doc));
                PluginMain.GetApplication().Logbook.SetExtensionText(GUIDs.PluginMain, doc.OuterXml);
                PluginMain.GetApplication().Logbook.Modified = true;
            }
		}

		public static IApplication GetApplication() {
			return m_App;
		}

		void AppPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (settingsVersionCurrent > settingsVersion)
            {
                if (e != null && e.PropertyName == "Logbook")
                {
                    m_settings = null;
                    m_data = null;
                }
            }
		}

		private static IApplication m_App = null;
		private static Data.TrailData m_data = null;
		private static Data.Settings m_settings = null;
		public static Data.TrailData Data {
			get {
				if (m_data == null) {
                    if (null == PluginMain.GetApplication().Logbook)
                    {
                        //logbook is null if it could not be loaded, to avoid an exception that occurs at exit
                        m_settings = new TrailsPlugin.Data.Settings();
                        m_data = new TrailsPlugin.Data.TrailData();
                    }
                    else
                    {
                        PluginMain.ReadExtensionData();
                    }
				}
				return m_data;
			}
		}

		public static Data.Settings Settings {
			get {
				if (m_settings == null) {
					PluginMain.ReadExtensionData();
				}
				return m_settings;
			}
		}
        private class xmlTags
        {
            public const string settingsVersion = "settingsVersion";
            public const string Verbose = "Verbose";
        }
        private static int settingsVersion = 0; //default when not existing
        private const int settingsVersionCurrent = 2;
        public static int Verbose = 0;  //Only changed in xml file
	}
}
