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

//Used in Matrix, Trails, Overlay, UniqueRoutes plugin
namespace TrailsPlugin.Integration
{
    public class IntegrationUtility
    {
        public static Type GetType(string clrType, string PluginName, out System.Version version)
        {
            version = null;
            try
            {
                //List the assemblies in the current application domain
                AppDomain currentDomain = AppDomain.CurrentDomain;
                Assembly[] assems = currentDomain.GetAssemblies();

                foreach (Assembly assem in assems)
                {
                    AssemblyName assemName = new AssemblyName((assem.FullName));
                    if (assemName.Name.Equals(PluginName) || assemName.Name.Equals(PluginName + ".dll"))
                    {
                        version = assemName.Version;
                        return assem.GetType(clrType);
                    }
                }
            }
            catch (Exception) { }
            return null;
        }

        public static string CompabilityText(Type type, string PluginToInstall, string PluginCompatible, string UniquePlugin, System.Version currVersion, System.Version minVersion)
        {
            string result = string.Format(PluginToInstall, minVersion.ToString(), UniquePlugin);
            try
            {
                if (type != null)
                {
                    if (currVersion.CompareTo(minVersion) >= 0)
                    {
                        result = string.Format(OtherPluginVersion, currVersion.ToString(), UniquePlugin) + " " +
                            PluginCompatible;
                    }
                    else
                    {
                        result = string.Format(OtherPluginVersion, currVersion.ToString(), UniquePlugin) + " " +
                            string.Format(PluginToInstall, minVersion.ToString(), UniquePlugin);
                    }
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
        public static string OtherPluginVersion
        {
            get
            {
                return
#if GPSRUNNING_UNIQUEROUTES||GPSRUNNING_OVERLAY
 GpsRunningPlugin.Util.StringResources
#else // MATRIXPLUGIN, TRAILSPLUGIN
                  Properties.Resources
#endif
.OtherPluginVersion;
            }
        }

        public static string OtherPluginExceptionText
        {
            get
            {
                return
#if GPSRUNNING_UNIQUEROUTES||GPSRUNNING_OVERLAY
 GpsRunningPlugin.Util.StringResources
#else // MATRIXPLUGIN, TRAILSPLUGIN
                  Properties.Resources
#endif
.OtherPluginExceptionText;
            }
        }
    }
}