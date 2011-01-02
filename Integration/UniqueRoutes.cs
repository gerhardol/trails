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
using ZoneFiveSoftware.Common.Visuals.Fitness;

//Similar file used in Matrix, Trails, Overlay
namespace TrailsPlugin.Integration
{
    public static class UniqueRoutes
    {
        //Note: namespace changed, compatibility namespace still used
        private const string UniqueRoutesClr = "UniqueRoutes.Export.UniqueRoutes";
        private const string UniquePlugin = "UniqueRoutesPlugin";
        private const string findSimilarRoutes = "findSimilarRoutes";
        private const string findCommonStretches = "findCommonStretches";

        private static System.Version minVersion = new System.Version(1, 9, 203, 0);
        private static System.Version currVersion = new System.Version(0, 0, 0, 0);
        private static bool testedUniqueRoutes = false;

        private static Type _uniqueRoutes = null;
        private static Type GetUniqueRoutes
        {
            get
            {
                if (_uniqueRoutes == null && !testedUniqueRoutes)
                {
                    testedUniqueRoutes = true;

                    _uniqueRoutes = IntegrationUtility.GetType(UniqueRoutesClr, UniquePlugin, out currVersion);
                }
                return _uniqueRoutes;
            }
        }

        public static string CompabilityText
        {
            get
            {
                return IntegrationUtility.CompabilityText(GetUniqueRoutes, UniqueRoutesToInstall, UniqueRoutesCompatible, UniquePlugin, currVersion, minVersion);
            }
        }

        public static bool UniqueRouteIntegrationEnabled
        {
            get { return GetUniqueRoutes != null; }
        }

        public static IList<IActivity> GetUniqueRoutesForActivity(IActivity activity, System.Windows.Forms.ProgressBar progressBar)
        {
            IList<IActivity> results = null;

            try
            {
                if (progressBar == null)
                    progressBar = new System.Windows.Forms.ProgressBar();

                if (GetUniqueRoutes != null)
                {
                    MethodInfo methodInfo = GetUniqueRoutes.GetMethod(findSimilarRoutes);
                    object resultFromURPlugIn = methodInfo.Invoke(activity, new object[] { activity, progressBar });
                    results = (IList<IActivity>)resultFromURPlugIn;
                }
            }
            catch (Exception e)
            {
                // Log error?
                throw new Exception(string.Format(IntegrationUtility.OtherPluginExceptionText,
            UniquePlugin + ".dll", UniqueRoutesPluginName) + Environment.NewLine, e);
            }

            if (GetUniqueRoutes == null)
            {
                throw new Exception(string.Format(IntegrationUtility.OtherPluginExceptionText,
        UniquePlugin + ".dll", UniqueRoutesPluginName) + Environment.NewLine);
            }

            return results;
        }

        public static IDictionary<IActivity, IItemTrackSelectionInfo[]> GetCommonStretchesForActivity(IActivity activity, IList<IActivity> activities, System.Windows.Forms.ProgressBar progressBar)
        {
            IDictionary<IActivity, IItemTrackSelectionInfo[]> results = null;

            try
            {
                if (progressBar == null)
                    progressBar = new System.Windows.Forms.ProgressBar();

                if (GetUniqueRoutes != null)
                {
                    MethodInfo methodInfo = GetUniqueRoutes.GetMethod(findCommonStretches);
                    object resultFromURPlugIn = methodInfo.Invoke(activity, new object[] { activity, activities, progressBar });
                    results = (IDictionary<IActivity, IItemTrackSelectionInfo[]>)resultFromURPlugIn;
                }
            }
            catch (Exception e)
            {
                // Log error?
                _uniqueRoutes = null;
                throw new Exception(string.Format(IntegrationUtility.OtherPluginExceptionText,
            UniquePlugin + ".dll", UniqueRoutesPluginName) + Environment.NewLine, e);
            }

            if (GetUniqueRoutes == null)
            {
                throw new Exception(string.Format(IntegrationUtility.OtherPluginExceptionText,
        UniquePlugin + ".dll", UniqueRoutesPluginName) + Environment.NewLine);
            }

            return results;
        }

        private static string UniqueRoutesPluginName
        {
            get
            {
                return 
#if GPSRUNNING_UNIQUEROUTES||GPSRUNNING_OVERLAY
                  GpsRunningPlugin.Util.StringResources
#else // MATRIXPLUGIN, TRAILSPLUGIN
                  Properties.Resources
#endif
                  .UniqueRoutesPluginName;
            }
        }

        private static string UniqueRoutesToInstall
        {
            get
            {
                return
#if GPSRUNNING_UNIQUEROUTES||GPSRUNNING_OVERLAY
                  GpsRunningPlugin.Util.StringResources
#else // MATRIXPLUGIN, TRAILSPLUGIN
                  Properties.Resources
#endif
                  .UniqueRoutesToInstall;
            }
        }

        private static string UniqueRoutesCompatible
        {
            get
            {
                return
#if GPSRUNNING_UNIQUEROUTES||GPSRUNNING_OVERLAY
                  GpsRunningPlugin.Util.StringResources
#else // MATRIXPLUGIN, TRAILSPLUGIN
                  Properties.Resources
#endif
                  .UniqueRoutesCompatible;
            }
        }
}
}