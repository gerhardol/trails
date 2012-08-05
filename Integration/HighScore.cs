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
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Visuals.Fitness;
#if ST_2_1
using TrailsPlugin.Data;
#endif

//Similar file used in Matrix, Trails, Overlay
namespace TrailsPlugin.Integration
{
    public static class HighScore
    {
        //Note: namespace changed, compatibility namespace still used
        private const string HighScoreClr = "HighScore.Export.HighScore";
        private const string HighScorePlugin = "HighScorePlugin";
        private const string _HighScorePopup = "HighScorePopup";
        private const string getHighScore = "getResults";

        private static readonly System.Version minVersion = new System.Version(2, 0, 357, 0);
        private static System.Version currVersion = new System.Version(0, 0, 0, 0);
        private static bool testedHighScore = false;

        private static Type _HighScore = null;
        private static Type GetHighScore
        {
            get
            {
                if (_HighScore == null && !testedHighScore)
                {
                    testedHighScore = true;

                    _HighScore = IntegrationUtility.GetType(HighScoreClr, HighScorePlugin, out currVersion);
                    if (currVersion == null || currVersion.CompareTo(minVersion) < 0)
                    {
                        _HighScore = null;
                    }
                }
                return _HighScore;
            }
        }

        public static string CompabilityText
        {
            get
            {
                return IntegrationUtility.CompabilityText(GetHighScore, HighScoreToInstall, HighScoreCompatible, HighScorePlugin, currVersion, minVersion);
            }
        }

        public static bool HighScoreIntegrationEnabled
        {
            get { return GetHighScore != null; }
        }

        public class HighScoreResult
        {
            public HighScoreResult(IList<Object> o)
            {
                this.activity = (IActivity)o[0];
                this.selInfo = (IItemTrackSelectionInfo)o[1];
                this.tooltip = (string)o[2];
            }
            public IActivity activity;
            public IItemTrackSelectionInfo selInfo;
            public string tooltip;
        }

        public static IList<HighScoreResult> GetHighScoreForActivity(IList<IActivity> activities, System.Windows.Forms.ProgressBar progressBar)
        {
            IList<HighScoreResult> results = null;

            try
            {
                if (GetHighScore != null)
                {
                    MethodInfo methodInfo = GetHighScore.GetMethod(getHighScore);
                    object resultFromPlugIn = methodInfo.Invoke(null, new object[] { activities, progressBar });
                    results = new List<HighScoreResult>();
                    foreach (IList<Object> o in (IList<IList<Object>>)resultFromPlugIn)
                    {
                        //Ignore null results
                        //could be used to sync results for the same HighScore Goal, separate activities
                        if (o != null)
                        {
                            results.Add(new HighScoreResult(o));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Log error?
                throw new Exception(string.Format(IntegrationUtility.OtherPluginExceptionText,
                  HighScorePlugin + ".dll", HighScorePluginName) + Environment.NewLine, e);
            }

            if (GetHighScore == null)
            {
                throw new Exception(string.Format(IntegrationUtility.OtherPluginExceptionText,
                  HighScorePlugin + ".dll", HighScorePluginName) + Environment.NewLine);
            }

            return results;
        }

        public static void HighScorePopup(IList<IActivity> activities, IList<IValueRangeSeries<DateTime>> pauses, IDailyActivityView view, System.Windows.Forms.ProgressBar progressBar)
        {
            try
            {
                if (GetHighScore != null)
                {
                    MethodInfo methodInfo = GetHighScore.GetMethod(_HighScorePopup);
                    object resultFromPlugIn = methodInfo.Invoke(null, new object[] { activities, pauses, view, progressBar });
                }
            }
            catch (Exception e)
            {
                // Log error?
                throw new Exception(string.Format(IntegrationUtility.OtherPluginExceptionText,
                  HighScorePlugin + ".dll", HighScorePluginName) + Environment.NewLine, e);
            }

            if (GetHighScore == null)
            {
                throw new Exception(string.Format(IntegrationUtility.OtherPluginExceptionText,
                  HighScorePlugin + ".dll", HighScorePluginName) + Environment.NewLine);
            }
        }

        private static string HighScorePluginName
        {
            get
            {
                return 
#if GPSRUNNING_UNIQUEROUTES||GPSRUNNING_OVERLAY
                  GpsRunningPlugin.Util.StringResources
#else // MATRIXPLUGIN, TRAILSPLUGIN
                  Properties.Resources
#endif
                  .HighScorePluginName;
            }
        }

        private static string HighScoreToInstall
        {
            get
            {
                return
#if GPSRUNNING_UNIQUEROUTES||GPSRUNNING_OVERLAY
                  GpsRunningPlugin.Util.StringResources
#else // MATRIXPLUGIN, TRAILSPLUGIN
                  Properties.Resources
#endif
                  .HighScoreToInstall;
            }
        }

        private static string HighScoreCompatible
        {
            get
            {
                return
#if GPSRUNNING_UNIQUEROUTES||GPSRUNNING_OVERLAY
                  GpsRunningPlugin.Util.StringResources
#else // MATRIXPLUGIN, TRAILSPLUGIN
                  Properties.Resources
#endif
                  .HighScoreCompatible;
            }
        }
}
}