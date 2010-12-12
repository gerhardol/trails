/*
Copyright (C) 2008, 2009 Henrik Naess
Copyright (C) 2010 Gerhard Olsson

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ZoneFiveSoftware.Common.Data.Fitness;

//Adapted from Matrix plugin
namespace TrailsPlugin.Integration
{
    public class UniqueRoutes
    {
        //Note: namespace changed, compatibility namespace still used
        private const string UniqueRoutesSettingsClr = "SportTracksUniqueRoutesPlugin.Source.Settings";
        private const string UniqueRoutesClr = "SportTracksUniqueRoutesPlugin.Source.UniqueRoutes";
        //private const string UniqueRouteGuid = "5c630517-46c4-478d-89d6-a8a6ca6337db";
        private const string UniquePlugin = "UniqueRoutesPlugin";
        System.Version minVersion = new System.Version(1,9,0,0);

        private object _uniqueRoutes = null;

        public string CompabilityText
        {
            get
            {
                string result = string.Format(Properties.Resources.UniqueRoutesToInstall, minVersion.ToString(), UniquePlugin);
                Type type;
                System.Version version;
                try
                {
                    type = IntegrationUtility.GetType(UniqueRoutesClr, UniquePlugin, out version);
                    if (type != null)
                    {
                        if (version.CompareTo(minVersion) >= 0)
                        {
                            result = string.Format(Properties.Resources.OtherPluginVersion, version.ToString(), UniquePlugin) + " " +
                                Properties.Resources.UniqueRoutesCompatible;
                        }
                        else
                        {
                            result = string.Format(Properties.Resources.OtherPluginVersion, version.ToString(), UniquePlugin) + " " +
                                string.Format(Properties.Resources.UniqueRoutesToInstall, minVersion.ToString(), UniquePlugin);
                        }
                    }
                }
                catch (Exception)
                {
                }
                return result;
            }
        }

        public bool UniqueRouteIntegrationEnabled
        {
            get { return _uniqueRoutes != null; }
        }

        private static bool HasDirection()
        {
            System.Version version;
            Type type = IntegrationUtility.GetType(UniqueRoutesSettingsClr, UniquePlugin, out version);
            if (type != null)
                return (bool)type.GetMethod("get_HasDirection").Invoke(null, null);
            else
            {
                throw new Exception(string.Format(Properties.Resources.OtherPluginExceptionText,
                           UniquePlugin + ".dll", Properties.Resources.UniqueRoutesPluginName) + Environment.NewLine);
            }
        }

        private static void SetDirection(bool hasDirection)
        {
            System.Version version;
            Type type = IntegrationUtility.GetType(UniqueRoutesSettingsClr, UniquePlugin, out version);
            if (type != null)
                type.GetMethod("set_HasDirection").Invoke(hasDirection, new object[] {hasDirection});
            else
            {
                throw new Exception(string.Format(Properties.Resources.OtherPluginExceptionText,
        UniquePlugin + ".dll", Properties.Resources.UniqueRoutesPluginName) + Environment.NewLine);
            }
        }

        public IList<IActivity> GetUniqueRoutesForActivity(IActivity activity, System.Windows.Forms.ProgressBar progressBar)
        {
            IList<IActivity> results = null;
            Type type;

            try
            {
                bool hasDirection = HasDirection();
                if (hasDirection)
                    SetDirection(false);

                if (progressBar == null)
                    progressBar = new System.Windows.Forms.ProgressBar();

                System.Version version;
                type = IntegrationUtility.GetType(UniqueRoutesClr, UniquePlugin, out version);
                if (type != null)
                {
                    MethodInfo methodInfo = type.GetMethod("findSimilarRoutes");
                    object resultFromURPlugIn = methodInfo.Invoke(activity, new object[] { activity, progressBar });
                    results = (IList<IActivity>) resultFromURPlugIn;
                }

                if (hasDirection)
                    SetDirection(true);
            }
            catch (Exception e)
            {
                // Log error?
                _uniqueRoutes = null;
                throw new Exception(string.Format(Properties.Resources.OtherPluginExceptionText,
            UniquePlugin + ".dll", Properties.Resources.UniqueRoutesPluginName) + Environment.NewLine, e);
            }

            if (type == null)
            {
                throw new Exception(string.Format(Properties.Resources.OtherPluginExceptionText,
        UniquePlugin + ".dll", Properties.Resources.UniqueRoutesPluginName) + Environment.NewLine);
            }

            return results;
        }
    }
}