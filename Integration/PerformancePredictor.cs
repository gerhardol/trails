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
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Visuals.Fitness;
#if ST_2_1
using TrailsPlugin.Data;
#endif

//Similar file used in Matrix, Trails, Overlay
namespace TrailsPlugin.Integration
{
    public static class PerformancePredictor
    {
        private const string PerformancePredictorClr = "PerformancePredictor.Export.PerformancePredictor";
        private const string PerformancePredictorPlugin = "PerformancePredictorPlugin";
        private const string PerformancePredictorPopup = "PerformancePredictorControl";

        private static System.Version minVersion = new System.Version(2, 0, 339, 0);
        private static System.Version currVersion = new System.Version(0, 0, 0, 0);
        private static bool testedPerformancePredictor = false;

        private static Type _PerformancePredictor = null;
        private static Type GetPerformancePredictor
        {
            get
            {
                if (_PerformancePredictor == null && !testedPerformancePredictor)
                {
                    testedPerformancePredictor = true;

                    _PerformancePredictor = IntegrationUtility.GetType(PerformancePredictorClr, PerformancePredictorPlugin, out currVersion);
                }
                return _PerformancePredictor;
            }
        }

        public static string CompabilityText
        {
            get
            {
                return IntegrationUtility.CompabilityText(GetPerformancePredictor, PerformancePredictorToInstall, PerformancePredictorCompatible, PerformancePredictorPlugin, currVersion, minVersion);
            }
        }

        public static bool PerformancePredictorIntegrationEnabled
        {
            get { return GetPerformancePredictor != null; }
        }

        public class PerformancePredictorResult
        {
            public PerformancePredictorResult(IList<Object> o)
            {
                this.activity = (IActivity)o[0];
                this.selInfo = (IItemTrackSelectionInfo)o[1];
                this.tooltip = (string)o[2];
            }
            public IActivity activity;
            public IItemTrackSelectionInfo selInfo;
            public string tooltip;
        }

        public static void PerformancePredictorControl(IList<IActivity> activities, IDailyActivityView view, TimeSpan time, double distance, System.Windows.Forms.ProgressBar progressBar)
        {
            try
            {
                if (GetPerformancePredictor != null)
                {
                    MethodInfo methodInfo = GetPerformancePredictor.GetMethod("PerformancePredictorPopup");
                    object resultFromPlugIn = methodInfo.Invoke(null, new object[] { activities, view, time, distance, progressBar });
                }
            }
            catch (Exception e)
            {
                // Log error?
                throw new Exception(string.Format(IntegrationUtility.OtherPluginExceptionText,
            PerformancePredictorPlugin + ".dll", PerformancePredictorPluginName) + Environment.NewLine, e);
            }

            if (GetPerformancePredictor == null)
            {
                throw new Exception(string.Format(IntegrationUtility.OtherPluginExceptionText,
        PerformancePredictorPlugin + ".dll", PerformancePredictorPluginName) + Environment.NewLine);
            }
        }

        private static string PerformancePredictorPluginName
        {
            get
            {
                return 
#if GPSRUNNING_UNIQUEROUTES||GPSRUNNING_OVERLAY
                  GpsRunningPlugin.Util.StringResources
#else // MATRIXPLUGIN, TRAILSPLUGIN
                  Properties.Resources
#endif
                  .PerformancePredictorPluginName;
            }
        }

        private static string PerformancePredictorToInstall
        {
            get
            {
                return
#if GPSRUNNING_UNIQUEROUTES||GPSRUNNING_OVERLAY
                  GpsRunningPlugin.Util.StringResources
#else // MATRIXPLUGIN, TRAILSPLUGIN
                  Properties.Resources
#endif
                  .PerformancePredictorToInstall;
            }
        }

        private static string PerformancePredictorCompatible
        {
            get
            {
                return
#if GPSRUNNING_UNIQUEROUTES||GPSRUNNING_OVERLAY
                  GpsRunningPlugin.Util.StringResources
#else // MATRIXPLUGIN, TRAILSPLUGIN
                  Properties.Resources
#endif
                  .PerformancePredictorCompatible;
            }
        }
}
}