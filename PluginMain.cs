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
using TrailsPlugin.Export;

namespace TrailsPlugin {
    class Plugin : IPlugin
    {
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
                return Properties.Resources.ApplicationName;
            }
        }

        public string Version
        {
            get { return GetType().Assembly.GetName().Version.ToString(4); }
        }

        public void ReadOptions(XmlDocument xmlDoc, XmlNamespaceManager nsmgr, XmlElement pluginNode)
        {
            String attr;
            attr = pluginNode.GetAttribute(xmlTags.Verbose);
            if (attr.Length > 0) { Verbose = XmlConvert.ToInt16(attr); }
            attr = pluginNode.GetAttribute(xmlTags.settingsVersion);
            if (attr.Length > 0) { preferencesSettingsVersion = (Int16)XmlConvert.ToInt16(attr); }

            if (preferencesSettingsVersion == 2)
            {
                //Stored "flat" under the main
                Data.Settings.ReadOptions(xmlDoc, nsmgr, pluginNode);

                //Trails stored as one attribute
                attr = pluginNode.GetAttribute(xmlTags.tTrails_ver2);
                XmlDocument doc = new XmlDocument();
                if (null == attr || 0 == attr.Length)
                {
                    attr = "<" + xmlTags.sTrails + "/>";
                }
                doc.LoadXml(attr);
                Data.TrailData.ReadOptions(doc, nsmgr, doc.DocumentElement);
                //Trails are read, must wait for logbook open to call WriteTrailData();
                trailsAreRead = true;
                Controller.TrailController.Instance.ReReadTrails();

                //In case user do not save logbook when exiting, save backup
                SettingsToFile();
            }
            else
            {
                foreach (XmlElement node in pluginNode.ChildNodes)
                {
                    //Workaround, no hit with SelectNodes(xmlTags.sSettings))
                    if (node.Name == xmlTags.sSettings)
                    {
                        Data.Settings.ReadOptions(xmlDoc, nsmgr, node);
                    }
                }
                //Read trails when logbook is loaded
            }
        }

        public void WriteOptions(XmlDocument xmlDoc, XmlElement pluginNode)
        {
            //Set version so Preferences are loaded next time
            if (preferencesSettingsVersion <= 2)
            {
                preferencesSettingsVersion = 4;
                //TrailData must be stored prior to this, logbook is likely closed already
            }
            pluginNode.SetAttribute(xmlTags.Verbose, XmlConvert.ToString(Verbose));
            pluginNode.SetAttribute(xmlTags.settingsVersion, XmlConvert.ToString(preferencesSettingsVersion));

            XmlElement settings = xmlDoc.CreateElement(xmlTags.sSettings);
            Data.Settings.WriteOptions(xmlDoc, settings);
            pluginNode.AppendChild(settings);
        }
        #endregion

        //Parse/Write the trails xml, (trails only) from logbook or (settings and trails) from separate file (user triggered)
        private static void ParseXml(XmlDocument xmlDoc, bool includeSettings)
        {
            String attr = xmlDoc.DocumentElement.GetAttribute(xmlTags.settingsVersion);
            if (attr.Length > 0) { logbookSettingsVersion = (Int16)XmlConvert.ToInt16(attr); }
            if (includeSettings)
            {
                foreach (XmlElement node in xmlDoc.DocumentElement.SelectNodes(xmlTags.sSettings))
                {
                    Data.Settings.ReadOptions(xmlDoc, new XmlNamespaceManager(xmlDoc.NameTable), node);
                }
            }
            foreach (XmlElement node in xmlDoc.DocumentElement.SelectNodes(xmlTags.sTrails))
            {
                //Should be only one Trails, but simple way to get the XmlElement
                Data.TrailData.ReadOptions(xmlDoc, new XmlNamespaceManager(xmlDoc.NameTable), node);
            }

            //Reinit the controller (control state should be updated too)
            Controller.TrailController.Instance.ReReadTrails();
        }

        private static XmlDocument WriteXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<" + xmlTags.sTrailsPlugin + "/>");

            logbookSettingsVersion = 4;
            doc.DocumentElement.SetAttribute(xmlTags.settingsVersion, XmlConvert.ToString(logbookSettingsVersion));

            //Settings data always written, normally not read from logbook
            XmlElement settings = doc.CreateElement(xmlTags.sSettings);
            Data.Settings.WriteOptions(doc, settings);
            doc.DocumentElement.AppendChild(settings);

            XmlElement trails = doc.CreateElement(xmlTags.sTrails);
            Data.TrailData.WriteOptions(doc, trails);
            doc.DocumentElement.AppendChild(trails);

            return doc;
        }

        private int ReadExtensionVersion()
        {
            int version = 0;
            if (null != Plugin.GetApplication().Logbook)
            {
                XmlDocument xmlDoc = new XmlDocument();
                string xml = Plugin.GetApplication().Logbook.GetExtensionText(GUIDs.PluginMain);
                if (xml == "")
                {
                    xml = "<" + xmlTags.sTrailsPlugin + "/>";
                }
                xmlDoc.LoadXml(xml);
                String attr = xmlDoc.DocumentElement.GetAttribute(xmlTags.settingsVersion);
                if (attr.Length > 0) { version = (Int16)XmlConvert.ToInt16(attr); }
            }
            return version;
        }

        private static void ReadExtensionData()
        {
            if (!trailsAreRead && null != Plugin.GetApplication().Logbook)
            {
                XmlDocument xmlDoc = new XmlDocument();
                string xml = Plugin.GetApplication().Logbook.GetExtensionText(GUIDs.PluginMain);
                if (xml == "")
                {
                    xml = "<" + xmlTags.sTrailsPlugin + "/>";
                }
                xmlDoc.LoadXml(xml);
                ParseXml(xmlDoc, false);
                trailsAreRead = true;
            }
        }

        private static void WriteExtensionData(XmlDocument xmlDoc)
        {
            if (null != Plugin.GetApplication().Logbook)
            {
                String prev = Plugin.GetApplication().Logbook.GetExtensionText(GUIDs.PluginMain);
                String upd = xmlDoc.OuterXml;
                if (prev != upd)
                {
                    Plugin.GetApplication().Logbook.SetExtensionText(GUIDs.PluginMain, upd);
                    Plugin.GetApplication().Logbook.Modified = true;
                }
            }
        }

        public static void WriteExtensionData()
        {
            if (null != Plugin.GetApplication().Logbook)
            {
                XmlDocument xmlDoc = WriteXml();
                WriteExtensionData(xmlDoc);
            }
        }

        //Both settings and trails
        public static void SettingsFromFile(string f)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(f);

            ParseXml(xmlDoc, true);
            trailsAreRead = true;
            WriteExtensionData(xmlDoc);
        }

        private void SettingsToFile()
        {
            string xml = System.IO.Path.Combine(GetApplication().Configuration.UserPluginsDataFolder, Name);
            System.IO.Directory.CreateDirectory(xml);
            xml = System.IO.Path.Combine(xml, "Backup-" + preferencesSettingsVersion + "-" + Version + "-" + DateTime.Now.ToString("o").Replace(':', '_') + ".xml");
            SettingsToFile(xml);
        }

        public static void SettingsToFile(string f)
        {
            XmlDocument doc = WriteXml();

            doc.Save(f);
        }

        public static IApplication GetApplication()
        {
            return m_App;
        }

        void AppPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e != null && e.PropertyName == "Logbook")
            {
                //Logbook is read (possibly changed)
                if (preferencesSettingsVersion != 2)
                {
                    trailsAreRead = false;
                    ReadExtensionData();
                }
                else
                {
                    //Migration, first startup on old preferences file
                    int version = ReadExtensionVersion();
                    if (version > preferencesSettingsVersion)
                    {
                        //Mostly unexpected:
                        // Trails read from logbook are newer, use those in logbook, save a backup
                        SettingsToFile();
                        trailsAreRead = false;
                        ReadExtensionData();
                    }
                    else
                    {
                        //Logbook is not available during startup, save migrated trails
                        WriteExtensionData();
                    }
                }

                // Register our filter criteria provider
                if (FilterCriteriaControllerWrapper.Instance.IsPluginInstalled &&
                    FilterCriteriaControllerWrapper.Instance.RegisterMethodAvailable &&
                    !m_FilterCriteriaProviderRegistered)
                {
                    FilterCriteriaControllerWrapper.Instance.RegisterFilterCriteriaProvider(new TrailsFilterCriteriasProvider());
                    m_FilterCriteriaProviderRegistered = true;
                }
            }
        }
        
        private class xmlTags
        {
            public const string settingsVersion = "settingsVersion";
            public const string Verbose = "Verbose";

            public const string sTrailsPlugin = "TrailsPlugin";
            public const string sSettings = "Settings";
            public const string sTrails = "Trails";
            public const string tTrails_ver2 = "tTrails";
        }
        private static IApplication m_App = null;
        private static int preferencesSettingsVersion = 0;
        private static int logbookSettingsVersion = 0;
        //Versions:
        //0 default when not existing
        //1 (pre 1.0?) old logbook, both settings/trails. No longer supported, handled as 0.
        // Preferences ver 2 (used in plugin version 1.2). Versions higher than 2 in 2.0.
        //2 Settings/Trails in Preferences
        //3 Reserved
        //4 Settings (structured) in preferences, Trails in Logbook

        private static bool trailsAreRead = false;
        public static int Verbose = 0;  //Only changed in xml file
        private bool m_FilterCriteriaProviderRegistered = false;
    }
}
