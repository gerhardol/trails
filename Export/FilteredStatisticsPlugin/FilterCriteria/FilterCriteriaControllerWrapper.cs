/*
Copyright (C) 2010 Sylvain Gravel

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

//From STGearChart plugin

using System;
using System.IO;
using System.Reflection;

namespace TrailsPlugin.Export  //FilteredStatisticsPlugin
{
    class FilterCriteriaControllerWrapper
    {
        private FilterCriteriaControllerWrapper()
        {
        }

        private void Initialize()
        {
            try
            {
                DetectMethodsAndClasses();
                m_PluginInstalled = true;
            }
            catch (Exception e)
            {
                m_PluginInstalled = false;
                throw e;
            }
        }

        private void DetectMethodsAndClasses()
        {
            foreach (Assembly loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (loadedAssembly.FullName.StartsWith("FilteredStatistics"))
                {
                    Type filterCriteriaControllerType = loadedAssembly.GetType("FilteredStatistics.Common.Controller.FilterCriteriaController");

                    if (filterCriteriaControllerType != null)
                    {
                        PropertyInfo instanceProperty = filterCriteriaControllerType.GetProperty("Instance");

                        if (instanceProperty != null &&
                            instanceProperty.CanRead)
                        {
                            MethodInfo registerMethod = filterCriteriaControllerType.GetMethod("RegisterFilterCriteriaProvider", new Type[] { typeof(object) });

                            m_ControllerInstance = instanceProperty.GetValue(null, null);

                            if (registerMethod != null)
                            {
                                m_RegisterMethod = registerMethod;
                            }
                        }
                    }
                }
            }
        }

        public static FilterCriteriaControllerWrapper Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new FilterCriteriaControllerWrapper();
                    m_Instance.Initialize();
                }

                return m_Instance;
            }
        }

        public void RegisterFilterCriteriaProvider(object provider)
        {
            if (IsPluginInstalled && RegisterMethodAvailable)
            {
                m_RegisterMethod.Invoke(m_ControllerInstance, new object[] { provider });
            }
        }

        public bool IsPluginInstalled
        {
            get { return m_PluginInstalled; }
        }

        public bool RegisterMethodAvailable
        {
            get { return m_RegisterMethod != null; }
        }

        private static FilterCriteriaControllerWrapper m_Instance = null;
        private object m_ControllerInstance = null;
        private bool m_PluginInstalled = false;
        private MethodInfo m_RegisterMethod = null;
    }
}
