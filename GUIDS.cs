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

namespace TrailsPlugin {
    class GUIDs {
        //Trails views
        public static readonly Guid PluginMain = new Guid("d75393a2-4a95-4fe7-ace2-375ff7338b2c");
#if ST_2_1
        public static readonly Guid MapControlLayer = new Guid("2aa470c3-0a3c-42c7-bbee-a43073072ee7");
#else
        public static readonly Guid TrailPointsControlLayerProvider = new Guid("2aa470c3-0a3c-42c7-bbee-a43073072ee7");
#endif
        public static readonly Guid Settings = new Guid("c5d04e80-6108-11df-a08a-0800200c9a66");
        public static readonly Guid Activity = new Guid("0af379d0-5ebe-11df-a08a-0800200c9a66");

        //ST standard views - same for ST3, ST3.1
        public static readonly Guid DailyActivityView = new Guid("1dc82ca0-88aa-45a5-a6c6-c25f56ad1fc3");
        //public static readonly Guid EquipmentView = new Guid("92e1a9b4-de58-11db-9705-00e08161165f");
        //public static readonly Guid AthleteView = new Guid("709e607b-cb51-431c-ba3f-197ab4df3de0");
        //public static readonly Guid ReportView = new Guid("99498256-cf51-11db-9705-005056c00008");

        public static readonly Guid SettingsView = new Guid("df106ae5-c497-11db-96fe-005056c00008");
        //public static readonly Guid RoutesView = new Guid("e9a99ef8-c497-11db-96fe-005056c00008");
        //public static readonly Guid OnlineView = new Guid("f8a828fc-c497-11db-96fe-005056c00008");
        //public static readonly Guid CategoriesView = new Guid("2cfdc5ac-d8d0-11db-9705-005056c00008");

        //FilteredStatistics
        public static readonly Guid FilteredStatisticsTemplateFilter = new Guid("9274e230-2359-11e0-ac64-0800200c9a66");
        public static readonly Guid FilteredStatisticsFilter = new Guid("92750940-2359-11e0-ac64-0800200c9a66");

        //Generated Trails
        public static readonly Guid SplitsTrail = new Guid("30681520-b220-11e0-a00b-0800200c9a66");
        //public static readonly Guid SplitsTimeTrail = new Guid("00c0959e-181f-4923-9e83-1fcd9dad34ed");
        //public static readonly Guid SwimSplitsTrail = new Guid("c35b7cad-cb7a-4040-a498-11ad813aa0ef");
        public static readonly Guid ReferenceTrail = new Guid("30681521-b220-11e0-a00b-0800200c9a66");
        public static readonly Guid HighScoreTrail = new Guid("30681522-b220-11e0-a00b-0800200c9a66");
        public static readonly Guid UniqueRoutesTrail = new Guid("389f4602-6480-4e7c-9830-3ec04a48aa6d");
        public static readonly Guid ElevationPointsTrail = new Guid("e5e19350-89c3-11e2-9e96-0800200c9a66");

        //GUIDs for the plugins, to open settings
        public static readonly Guid HighScorePluginMain = new Guid("4B84E5C0-EC2B-4C0C-8B8E-3FAEB09F74C6");
        public static readonly Guid UniqueRoutesPluginMain = new Guid("5c630517-46c4-478d-89d6-a8a6ca6337db");
    }
}