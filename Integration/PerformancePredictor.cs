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

        private const string _PerformancePredictorPopup = "PerformancePredictorPopup";
        private const string _PerformancePredictorFields = "getResults";

        private static readonly System.Version minVersion = new System.Version(2, 0, 357, 0);
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
                    if (currVersion.CompareTo(minVersion) < 0)
                    {
                        _PerformancePredictor = null;
                    }
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
            public class Predicted
            {
                public double new_dist, new_time;
                public Predicted(double new_dist, double new_time)
                {
                    this.new_dist = new_dist;
                    this.new_time = new_time;
                }
            }
            public PerformancePredictorResult(IList<Object> o)
            {
                this.vo2max = (double)o[0];
                this.vdot = (double)o[1];
                this.predicted = new List<Predicted>();
                for(int i = 2; i < o.Count-1; i+=2)
                {
                    predicted.Add(new Predicted((double)o[i], (double)o[i+1]));
                }
            }
            public double vo2max, vdot;
            public IList<Predicted> predicted;
        }

        public static void PerformancePredictorPopup(IList<IActivity> activities, IDailyActivityView view, TimeSpan time, double distance, System.Windows.Forms.ProgressBar progressBar)
        {
            try
            {
                if (GetPerformancePredictor != null)
                {
                    MethodInfo methodInfo = GetPerformancePredictor.GetMethod(_PerformancePredictorPopup);
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

        //relatve fast call for 2150 activities, 0.03s calledindividually, 0.0016s called for all
        public static IList<PerformancePredictorResult> PerformancePredictorFields(IList<IActivity> activities, IList<double> times, IList<double> distances, IList<double> predictDistances, System.Windows.Forms.ProgressBar progressBar)
        {
            IList<PerformancePredictorResult> results = null;
            try
            {
                if (GetPerformancePredictor != null)
                {
                    MethodInfo methodInfo = GetPerformancePredictor.GetMethod("getResults");
                    object resultFromPlugIn = methodInfo.Invoke(null, new object[] { activities, times, distances, predictDistances, progressBar });
                    results = new List<PerformancePredictorResult>();
                    foreach (IList<Object> o in (IList<IList<Object>>)resultFromPlugIn)
                    {
                        //Ignore null results
                        //could be used to sync results for the same HighScore Goal, separate activities
                        if (o != null)
                        {
                            results.Add(new PerformancePredictorResult(o));
                        }
                    }
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

            return results;
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