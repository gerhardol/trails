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

        public string Version {
            get { return GetType().Assembly.GetName().Version.ToString(4); }
        }

        public void ReadOptions(XmlDocument xmlDoc, XmlNamespaceManager nsmgr, XmlElement pluginNode)
        {
            String attr;
            attr = pluginNode.GetAttribute(xmlTags.Verbose);
            if (attr.Length > 0) { Verbose = XmlConvert.ToInt16(attr); }
            attr = pluginNode.GetAttribute(xmlTags.settingsVersion);
            if (attr.Length > 0) { settingsVersion = (Int16)XmlConvert.ToInt16(attr); }
            if (settingsVersion == 2)
            {
                trailsReadLogbook = false;
                //Setting migrate is not needed here, just one time override read
            }
            //Set version so Preferences are loaded next time
            if (settingsVersion <= 2)
            {
                settingsVersion = 3;
            }

            attr = pluginNode.GetAttribute(xmlTags.trailsInLogbook);
            if (attr.Length > 0) {
                trailsWriteLogbook = XmlConvert.ToBoolean(attr);
                trailsReadLogbook = trailsWriteLogbook;
            }
            attr = pluginNode.GetAttribute(xmlTags.trailsReadLogbook);
            if (attr.Length > 0) { trailsReadLogbook = XmlConvert.ToBoolean(attr); }//Override
            attr = pluginNode.GetAttribute(xmlTags.trailsMigrateLogbook);
            if (attr.Length > 0) {
                trailsWriteLogbook = !trailsWriteLogbook;
            }
            //TBD Cleanup at migration

            Data.Settings.ReadOptions(xmlDoc, nsmgr, pluginNode);
            ReadExtensionData();
            if (!trailsReadLogbook)
            {
                Data.TrailData.ReadOptions(xmlDoc, nsmgr, pluginNode);
                trailsAreRead = true;
            }
        }

        public void WriteOptions(XmlDocument xmlDoc, XmlElement pluginNode)
        {
            pluginNode.SetAttribute(xmlTags.Verbose, XmlConvert.ToString(Verbose));
            pluginNode.SetAttribute(xmlTags.settingsVersion, XmlConvert.ToString(settingsVersion));

            //pluginNode.SetAttribute(xmlTags.trailsReadLogbook, XmlConvert.ToString(trailsReadLogbook));
            pluginNode.SetAttribute(xmlTags.trailsInLogbook, XmlConvert.ToString(trailsWriteLogbook));
            //pluginNode.SetAttribute(xmlTags.trailsMigrateLogbook, XmlConvert.ToString(false));

            Data.Settings.WriteOptions(xmlDoc, pluginNode);
            if (!trailsWriteLogbook)
            {
                Data.TrailData.WriteOptions(xmlDoc, pluginNode);
            }
        }
        #endregion

        public static void ReadExtensionData()
        {
            if (!trailsAreRead && trailsReadLogbook && null != Plugin.GetApplication().Logbook)
            {
                XmlDocument doc = new XmlDocument();
                string xml = Plugin.GetApplication().Logbook.GetExtensionText(GUIDs.PluginMain);
                if (xml == "")
                {
                    xml = "<TrailsPlugin/>";
                }
                doc.LoadXml(xml);
                Data.TrailData.FromXml(doc.DocumentElement);
                trailsAreRead = true;
            }
        }

        public static void WriteExtensionData() 
        {
            if (trailsWriteLogbook && null != Plugin.GetApplication().Logbook)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<TrailsPlugin/>");
                XmlNode trails = Data.TrailData.ToXml(doc);
                doc.DocumentElement.AppendChild(trails);
                Plugin.GetApplication().Logbook.SetExtensionText(GUIDs.PluginMain, doc.OuterXml);
                Plugin.GetApplication().Logbook.Modified = true;
            }
        }

        public static IApplication GetApplication()
        {
            return m_App;
        }

        void AppPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e != null && e.PropertyName == "Logbook")
            {
                if (trailsReadLogbook)
                {
                    trailsAreRead = false;
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
            public const string trailsReadLogbook = "trailsReadLogbook"; //Override
            public const string trailsInLogbook = "trailsInLogbook";
            public const string trailsMigrateLogbook = "trailsMigrateLogbook";
            public const string Verbose = "Verbose";
        }
        private static IApplication m_App = null;
        private static int settingsVersion = 0;
        //Versions:
        //0 default when not existing
        //1 (pre 1.0?) old logbook, both settings/trails. No longer supported, handled as 0.
        // All Settings in Preferences since ver 2 (used in plugin version 1.2). Setting versions higher than 2 in 2.0.
        //2 Trails in Preferences
        //3 Use separate settings

        private const int settingsVersionCurrent = 4;
        private static bool trailsAreRead = false;
        private static bool trailsReadLogbook = true;
        private static bool trailsWriteLogbook = true;
        public static int Verbose = 0;  //Only changed in xml file
        private bool m_FilterCriteriaProviderRegistered = false;
    }
}
