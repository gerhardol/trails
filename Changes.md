This page describes the changes between plugin releases. The change log may contain further details.

### Changes ###

1.3.2 2015-10-04 (unreleased)
  * Changed version numbering after source repository was moved to GitHub.
  * Average time-of-day for summary results (in list) with more than two activities. Can give indication when a ride is normally at a certain point.
  * Updated Spanish translation.
  * Save column size in list when changing. Previously the default size was reset when switching back/to the plugin or changing the displayed columns only.
  * Possibility to show Custom Data fields. They are for the activity, not trail, but some are useful.
  * Show Lap fields for Split trails.
  * Add support for column "Max (Avg) Grade".
  * Sort was slow with many hundreds results. For simple fields like duration, the sort is instant now. More complex fields like average HR that has to be determined from the track, there is not much difference.
  * Set Garmin fenix, 920xt, 910xt ss barometric devices by default.
  * Viewer for UniqueRoutes was occasionally not showing time and date.


1.2.971 2015-01-30 (Plugin Catalog)
  * Pressing '+' when viewing automatically generated trail will offer to add a new trail with selected points, not just creating a copy of the generated trail.
  * New interface to Performance Predictor: Predict from the best result, not the average. Requires Performance Predictor 2.0.467.


1.2.964 2014-11-29 (Plugin Catalog)
  * Possibility to [Filter Results with UniqueRoutes](Features#filter-with-uniqueroutes). The trails will then behave a little more like Strava segments.
  * Possibility to set a [Default Reference Activity](Features#default-reference-activity). The default reference activity can be used to get a fixed activity to compare to or more predictive filter results with UniqueRoutes.
  * Improvements to comparing [Multiple Data Sources](Tutorials#multiple-data-sources), especially when the sources has different sets of pauses. See documentation for details.
  * Graphs over Distance shorter than the total distance and the graph had an offset (not starting at 0 x), the graph was cut.
  * Trails painted the graphs the same as standard ST, i.e. left to right. This means that the fill graph is painted on top of the other graphs. The order is now changed to draw the graphs right to left so the fill graph is in the lowest layer, not hiding other graphs.


1.2.945 2014-11-04 (Plugin Catalog)
  * Graph

    * Regions/Ranges selected in the graph is preserved when recalculating, switching trails or activities.

      * If result for the same activity (another result, subtrail, trail etc) is selected, and the selection is in the result, the snippet on the trail is used.

      * In all other situations, time/distance selection as offset in the graph is used.

    * Click outside chart lines did not clear selected regions.

  * Changed logic for add and editing trails:

    * Modify Trail

      * For normal or the generated Elevation trail will open the edit dialog for the trail

      * For other generated trails, edit opens the settings page for the trail (Trails, UniqueRoutes or HighScore).

    * Add Trail

      * No activity selected creates new empty trail

      * If a generated trail or nothing is selected on the map, create a trail based on the reference trail

      * If a point is selected on the activity track on the map, a popup asks where to add the selected points:

  * Yes to add to the current trail

  * No to add a new trail with the selected points

  * Edit Trail: Deleting a trail point selects the previous point instead of the first

  * GradeAdjustedPace related, [Grade Adjusted Pace](Features#grade-adjusted-pace) for more details

    * MervynDavies, GregMaclin and JackDaniels formula previously adjusted speed, not pace. This resulted in a little more aggressive adjustment uphill, a little less downhill.

    * Added MervynDaviesSpeed formula, corresponds to previous MervynDavies.

    * Steep downhill (and steep uphill for MervynDaviesSpeed) for linear algorithms MervynDavies, GregMaclin and JackDaniels uses Kay algorithm to adjust the pace to avoid unreasonable effects like infinitive speed at more than 56% downhill. The Kay formula has the fastest horizontal speed at 8% downhill, steeper than 18% is slower than flat. (This should work well on longer slopes, may not be correct for short slopes, elevation must be smoothed.)

    * Factors for MervynDavies and JackDaniels algorithms can be changed in settings.

    * Remove time component for Kay - should not be used for adjustment, only prediction of "world class results".

    * Corrected Kay grade adjusted pace for steep up/down

    * Corrected ACSM grade adjusted pace formula

    * The Vdot/Energy based formulas AlbertoMinetti and ACSM, Pandolf adjusted too much, using linear conversion from energy to speed. Jack Daniels tables uses a smaller factor, not linear dependency energy`<->`speed. (At at least downhill more than 15%, the Minetti study indicates that the efficiency is not exponential either, so the formulas may be off here.)

    * Full implementation of Pandolf formula. It takes the weight of the athlete and equipment into account. The formula also has a terrain factor, that can be set to handle sections of an activity. See [Grade Adjusted Pace](Features#grade-adjusted-pace).


1.2.917 2014-10-10 (Plugin Catalog)
  * Grade Adjusted Pace: Add Kay calculations to the existing methods. Background [Pace and Critical Gradient for Hill Runners: An Analysis of Race Records](http://www.lboro.ac.uk/microsites/maths/research/preprints/papers11/11-38.pdf), hint from a [ST forum post](http://www.zonefivesoftware.com/sporttracks/forums/viewtopic.php?p=85774&sid=cac957fef0d213becd6b06f6140cda0d#p85774). The algoritm study compared hilly races to flat races, with a time based component too.
  * [UniqueRoutes Trail](Features#uniqueroutes-trail) - use Trails as a viewer for Unique Routes. This adds the activities matched by UR as results. The previous integration, to insert activities in the selection is not changed.
  * The "Modify" button for the built-in trails Splits, Reference Activity, High Score and Unique Routes will now go to the related plugin settings. As the generated trails cannot be modified directly, the Modify button previously had the same functionality as the add button, adding a copy of the activity as a new trail.


1.2.909 2014-07-29
  * Updated to SportTracks 3.1.5314, that has memory optimizations. This decreases memory consumption when comparing hundreds of results.
  * Two-way trails did not show the result for the "back part" for away and back activities.
  * Tweaking for trail point detection for the start/end point. Previously the closest point was always selected, now points closer than about 5m are preferred (that gives the shortest trail).
  * Smoothing in the graph is changed with 10 s with PageUp/PageDown. See [Smoothing](Features#smoothing) for details.
  * When changing smoothing in the chart, the zooming level is unchanged.
  * Result List keyboard shortcut Alt-K to get Cadence from other activities (compare to Alt-E for elevation).
  * [Offset](Features#offset) for results. Normal trails are compared based on the trail points. Splits trail do not have any trail points and an offset can be used to adjust comparison start. This is similar to offset in Overlay plugin. Offset is normally not relevant for normal trails, but can be applied to all trails. Offset can be applied in several ways:
    * If the trail is Splits and a track overlap in time compared to the reference trail result, an offset is applied based on time automatically.
    * Chart keyboard shortcuts similar to [Smoothing](Features#smoothing) ('o', PageUp), except that offset is changed instead of smoothing.
    * Chart keyboard shortcut Alt-'o': select any graph for a result and drag it in the direction you want to offset the result.
    * Chart keyboard shortcut Shift-Ctrl-'o': Apply the displayed offset for a result to the activity itself. This can be used to update activities that has incorrect time.
  * Selecting in the chart for results with pauses were slightly incorrect. Similarly for the speed tooltip for selected result.
  * Diff graphs for activities with pauses were incorrect.
  * Saving chart images: Use Trail name in suggested name, save last used directory.
  * Selecting in chart: Mark all charted results on map if max 6 selected results.
  * Selecting in chart: Mark the "ST mapped activity" on the map also if not in the charted results if max 6 selected results.

1.2.876 2013-09-22 (Plugin Catalog)
  * PerformancePredictor Integration: Show Ideal time
  * Activities without GPS tracks were not shown correctly
  * Activities without elevation tracks had incorrect diff tracks if GradeAdjustedPace was selected.
  * Minor corrections and performance tweaks

1.2.844 2013-05-03 (Plugin Catalog)
  * With only one result and one selected, show summary for child results
  * ['t' shortcut](Features#result-list): Toggle show total instead of average in the summary line.
  * Result List Summary: Ignore Not-a-Number for averages.
  * Better precision for start/end positions on the route. Previously only second resolution was used, even if the start was determined with sub-second precision.

1.2.826 2013-04-12
  * Graphs could be plotted only in trailpoints in some situations.

1.2.821 2013-04-07

  * Support for recording elevation from a separate device (for instance with a good barometric elevation device), then use or apply it to a another result/activity (for instance from a device with a good GPS). See [Tutorial](Tutorials#separate-elevation-tracks) for a short explanation. (The previous experimental implementation in 1.0.604 is replaced.) Result List Keyboard shortcuts are added:
    * Use device elevation track for some calculations [Shift-'e' shortcut](Features#result-list).
    * Use other activities with the same time for the device elevation track [Alt-'e' shortcut](Features#result-list).
    * Select all results with a separate elevation track [Ctrl-'e' shortcut](Features#result-list).
  * Support to view "adjusted" elevation, primarily for barometric devices. See [elevation-points) and  [Barometric Elevation Adjustment](Tutorials#barometric-elevation-adjustment) for more information.
  * Device elevation (possibly adjusted) can be applied to activities [result-list) or be applied at import. See [Apply Device Elevation to Activity](Tutorials#apply-device-elevation-to-activity).

  * Ascending Speed (VAM), the average vertical speed using the start/end elevation difference. Primarily of interest for cycling.
  * Result List keyboard shortcut Ctrl-'i': Insert activities in the current category below topmost level. For instance if you use "My Activities:Running:Trail", insert activities in "My Activities:Running". See [Ctrl-'i' shortcut](Features#result-list).
  * [Trail Sort Priority](Features#trail-sort-priority)

  * Summary was not updated immediately when selecting a trail.
  * Avoid Sync at Trailpoints if only one result.
  * Selecting from Route did not clear selections on other results in chart.
  * Ctrl-click in charts could be "stuck" if mouse leaving chart before releasing key.
  * When clicking in chart without selecting a series, reset color on axis.
  * Remove end points for grade adjusted pace, as these are often "unstable".
  * Fill colors for primary multi result in chart was too transparant so fill was not visible.
  * Better time precision for Splits trail.

1.2.721 2013-02-24 (Plugin Catalog)
  * Trail attribute [Match Complete Activity](Features#match-complete-activity). Let the trail result match the part before the first and after the last trail point.
  * Diff columns for ResultList and EditTrail. Shows the accuracy for the trailpoint, how close the track is to the center of the trail point.
  * Standard Deviation statistics for the summary result: [Ctrl-'o' shortcut](Features#result-list).
  * Pace/Duration format drop hours if zero. (so 00:40:12 is now displayed as 40:12).
  * Some tweaks to editing trails, for selection and zooming
  * Allow 0 radius, to disable a trail
  * ToolTip pace summary could show summary for part not visible in the chart (when selecting on an activity, it is possible to select outside the result).
  * Some tweaks to trail detection, to handle non-required points better.

1.2.676 2013-02-01
  * After editing a trail, clear selection on route.
  * A trail could only be edited once.
  * If adding a new trail, do not keep the existing as selected.

1.2.672 2013-01-29 (Plugin Catalog)

Note: Build version changed in this release. (The code repositary changed from Subversion to Mercurial, code revisions are counted differently.) There are over 50 commits in this update.

  * General performance updates for charts.
  * Chart multi-select by using ctrl-click.
  * Colors for activities updated:
    * More colors (15, was 10) to display more activities
    * Slightly better contrast between the colors, slightly better visible in chart
    * Reordered colors to give better differences by default sort
    * Fill colors for multi activities in chart similar line color
  * Changed Zoom on route handling:
    * Result list Keyboard shortcut 'z' to zoom to marked tracks.
    * If ZoomToSelection is not set (default), use EnsureVisible to ensure that tracks are visible on the map
    * Use EnsureVisible as the default action in most situations when marking, also when ZoomToSelection is active
  * Always show last selected in chart or route as a range, with the summary info.
  * When start selecting in chart, show on route. Similarly, when start selecting on route, show on chart. Previously a section had to be selected.
  * Selecting in chart (or clear by click) clears selection on route (both Trail route and standard ST)
  * Multi-selection in ST route, similar to trails route: just continue selecting. (Clear by selecting in chart.)
  * Multi selection on trails routes did not always work. In addition, a tooltip is shown in the chart if selection fails.
  * When clicking in the result list, only mark on the chart when selecting the second time in a row, i.e. ctrl-click to select/deselect/select. This is to avoid marking in chart if not needed.
  * Clicking on axis did not clear existing selections.
  * If selecting a single subsplit in the result list, show the average for the subsplit in the summary (instead of all activities)
  * Show speed/pace as tooltip when selecting. This avoids a ST bug with summaries, as well as overcomes that multiple selections has no average at all.
  * If no Trail match, auto select auto generated Splits trail rather than Reference Activity. This emulates Overlay somehow.

1.0.663 2013-01-05
  * Speedup inbound filtering with many activities (purging trails that are not inbound). This is done by caching GPS Bounds information. It is now significantly faster to switch to many activities, especially to switch back. In addition, when selecting trails, the number of activities in bounds are shown. The Trail activity page is just a little slower than the standard SportTracks Summary page.
  * Speedup trail calculation about five times by changing the trail calculation. This is mostly seen when switching to all activities. Result presentation requires more time normally.

  By default, all trails are calculated only if "trails\*activities < 10000". This limit is not configurable in the GUI and set to 10000 by defualt in System.Preferences.xml (see MaxAutoCalcActivitiesSingleTrail) (this limit was previously set to 200).
  * Reference result row is always in italics, also with one result only.
  * Reworked charts to look more like standard SportTracks charts. The primary chart (first selected, with left axis) is always drawn in fill mode. With one result only displayed, the ST standard colors with fill graph for the primary chart is used. With more than one result a more discreet fill mode is selected (slightly transparent to see overlayed charts).
  * Click on chart axis to mark charts for the axis.
  * Chart marking (like [Mark Unique Routes Common Stretches](Features#mark-unique-routes-common-stretches) was not always visible.
  * Using keyboard shortcut in the chart did not always update the state in the chart menu.
  * Changes when selecting/clicking result rows. Previously, both routes and chart (as well as zoom) was done if changing selection. The primary behavior is to mark results that are clicked and still selected.
    * Only mark the route that is clicked and selected.
    * Only mark the charts for a result clicked and selected if more than one result is selected.
    * If Zoom-to-Selection is used (see [Chart shortcut Ctrl-Z](Features#graph), only zoom to results clicked and selected. Note: To zoom to selection, you can select and deselect a result. (See also keyboard shortcut Shift-Z for displaying only selected list rows.)


1.0.627 2013-01-01
  * Possibility to select and compare more than one trail at the same time [Multiple Trails](Features#multiple-trails).
  * Revised handling when changing selected activities, to select explicitly selected trails.
  * Calculate one trail also when switching to all activities. The plugin previously calculated if any activities covered the area for a trail only (but no trail calculation) when selecting more than 200 activities.
  * Do not select trails with more than 250 results automatically. This is to void the slow handling in ST charts when displaying charts. Previously for instance using Splits trail, use "Mark other sub Trails or splits in list" and selecting all activities could cause ST to work for a longer time. It is still possible to explicitly select a trail to get all results (the number of results are visible in the trail list).
  * Progressbar in some more situations when calculating trails.
  * Zoom to Trail bounds when selecting activities/trail.
  * "Add similar with Unique Routes" replaced used activities instead of adding.
  * A trail could not be renamed.

1.0.606 2012-12-17
  * When "keeping selection" when inserting activities, first split was selected too.
  * If the trail changed, select reference result (normally for same activity as previous reference result).
  * ToolTips in EditTrail menu
  * Possibility to reverse the trail points in a trail
  * New attributes for trails:
    * TwoWay match: Match also reverse matches. See [Two-Way Trail](Features#two-way-trail).
    * Temporary Trail: Do not save the trail when exiting ST
    * Name match: In addition to trail point match, also match on activity name. See [Match by Name](Features#match-by-name).
  * In addition, option to not include trail in global matches. This is hidden in the GUI for now for now.

1.0.604 2012-12-05 - Plugin Catalog
  * When inserting activities, keep the result selection in the list (also for subresults).
  * Some support for recording elevation from  a separate device, then use or apply it to a another result/activity. This can be used to use the barometric elevation from a device for a GPS device.

  The implementation is unofficial, limited and not documented, may be changed in the future.
Use [Keyboard shortcut Shift-'t'](Features#result-list) to insert activities with similar time as the reference result. The inserted result device elevation tracks will then be used as the device elevation tracks for the reference activity.

  Some new Result List Keyboard shortcuts (note that this is changed in 1.2.821):
    * Alt-'e': Adjusts the separate elevation track (but not the GPS elevation) for the selected result, compared to the reference result. Use to adjust barometric elevation with an offset.
    * Ctrl-'e': Apply the elevation from the inserted activity to the reference activity. The elevation is then aligned in the same as [Chart 'a'](Features#graph).
    * Shift-'e': Toggle Use normal elevation and device elevation for Grade Adjusted Pace.

1.0.597 2012-10-28
  * [HighScore Trail](Features#highscore-trail) can show HighScore for more than one activity
  * Mark Reference result with italics
  * Set order column in result list (instead of order following an activity)

1.0.593 2012-09-24
  * Simple GUI to adjust split adjust times. See [Reference Diff](Features#reference-diff).
  * Split time diff can also be set if [Grade Adjusted Pace](Features#grade-adjusted-pace) is not set.

1.0.588 2012-09-22
  * Grade Adjusted Pace settings are saved in preferences.
  * Possibility to somehow adjust "racesplit" times when viewing differences. See [Grade Adjusted Pace Course](Tutorials#grade-adjusted-pace-course).

1.0.583 2012-09-21
  * Grade Adjusted Pace: Possibility to estimate time/pace as it would be running on "flat" surface. This includes diff and pace graphs too. See [Grade Adjusted Pace](Features#grade-adjusted-pace).
  * Make device charts slightly more red/purple to easier distinguish them from normal chart lines.
  * Smoothing over unchanged borders were off with one trail point.

1.0.566 2012-08-05
  * Editing Trails with drag&drop did not work with Google Maps due to limitation in ST API. Workaround implemented.
  * Integration with PerformancePredictor and HighScore Plugins.
    * Popups calculated the Trail Results (not necessarily the complete distance). See [analyze-in-highscore) and [Analyze in PerformancePredictor](Features#analyze-in-performancepredictor).
    * Predict Performance on one distance (default 10km) in the result list
  * Stopped Time can be excluded for certain activities only. See [ExcludeStoppedCategory](Features#excludestoppedcategory).

1.0.555 2012-07-20
  * Possibility to control smoothing over sub result borders. For instance when running intervals and start/stopping sharply, the normal smoothing does not work well. The setting can be controlled from the Graph menu or by the keyboard shortcut Ctrl-'a'. See [Smoothing](Features#smoothing) for further information.
  * The graphs for the left axis can be aligned to the reference graphs, to for instance align elevation from one activity to another. There are several ways to align the graphs: None (Default), Start, End, Average, Min, Max. The setting can be controlled with the keyboard shortcut 'a'. See [Chart Alignment](Features#chart-alignment) for further information.
  * Device information: SpeedPace, Elevation, DeviceDist (diff was previously Device only)
  * Device distance diff scale was flipped, i.e. device was behind did show positive numbers
  * If only one normal result is selected in the list, show summary result for all
  * Result reference could not be set to a subresult
  * Align list columns as in ST: Numbers right, text right
  * When selecting track marked by ST, the complete track from start/end was marked, including pauses (also inactive laps, if set to handled as pauses).
  * Edit Trails: Incomplete matches handled: All matching points shown
  * Show diff distances with elevation units (for instance meters instead of kilometers)
  * Activities with partial GPS info only showed the GPS part (no GPS activities can be seen with the Splits trail)
  * Infinite values (like zero speed in pace charts) gave blank charts
  * Share axis with the same information (like Speed and DeviceSpeed)
  * When adding points to trails by selecting in the activity track, a section had to be selected, single points were ignored.
  * When editing trails, never offer to replace current trail points
  * When editing trails, the check for no info selected could fail to find that no points was available to edit
  * If no points are selected when adding trail points, open a new trail dialog.
  * Any keypress in graph pane did refresh the graphs (Ctrl-r does that).
  * For pace graphs, smooth speed before converting to pace, as slow points close to fast points gives weird results.

1.0.519 2012-04-22 - Plugin Catalog
  * Multi select on map possible: Click twice on a "trail route" to select. Multi selection possible by continue clicking. See [List-Route-Chart interaction](Features#list-route-chart-interaction) for more details.
  * When editing an activity, update list without manual recalculation. Chart results requires update of charts and trail detection requires recalculation still, for performance reasons.
  * Algorithms to detect short trails improved. Previously, the start to match for next point was done with some margin to current match.
  * When marking sections of a trail, interpolate positions between GPS points, to show position more accurately.
  * Exception when switching trails and no reference had been set when Power columns were visible.
  * When selecting one point on the route, the chart displayed a 1 sec selection.

1.0.505 2012-02-26
  * Result List keyboard shortcut: Ctrl-'s': Show device reading for time, dist etc instead of track/trail based (to compare device calculations)
  * Add result column "Descending Average Grade", similar to "Ascending Average Grade".

1.0.500 2011-12-22
  * Show summary line also for 1 result, can be used to get summary for splits.
  * Auto-select similar spilts did not update the summary line
  * Result List keyboard shortcut: Shift-'z': Show only marked results on the map (to avoid cluttering).
  * Add result columns Ascending, Descending and "Ascending Average Grade". The Ascent/Descent handling uses similar strategy to calculate Ascent/Descent as ST core, but the ST methods has some limitations when splitting up activities. Therefore, the Ascent/Descent is not identical even for Splits trail.
  * Speed up display of many average graphs. Too many points were added without improving the accuracy.
  * Show Tooltip smooth value when clicking an axis
  * Show trailpoints for reference when selecting only summary line

1.0.488 2011-11-30 - In Plugin Catalog
  * Chart keyboard shortcut ctrl-'d' in chart changed to menu item. (History for 1.0.473 updated)
  * Null check when switching activities

1.0.483 2011-11-22
  * Summary line in list. Shows the activity averages from the selected activities. If none is selected, show average for all. If selected, show the average in the chart (similar to Overlay).
  * Edit trail: Name could not be changed.
  * Removed AvgGrade from the list. It showed the avg from the chart\*100. A proper value would have been sum of ascent/descent divided with distance, but ascent/descent is not currently available.

1.0.473 2011-11-19
  * Result List keyboard shortcut: Ctrl-'z' to toggle "Zoom to selection", normal is to center on location only. Also, when selecting in chart, mark all routes. [Ctrl-'z' shortcut](Features#result-list)
  * Result List keyboard shortcut Ctrl-'t': Diff over time: to see diff over time based on absolute time. If the results do not have overlapping times, the diff over time is based on elapsed time. See [Multiple Data Sources](Tutorials#multiple-data-sources) for an example to use this.
  * Result List keyboard shortcut: Shift-'t' to insert activities with overlapping times. See [Multiple Data Sources](Tutorials#multiple-data-sources) for an example to use this.
  * Chart for GPS distance compared to device distance (if distance track has been imported). See See [Compare Data Sources](Tutorials#compare-data-sources) for an example to use this.

1.0.458 2011-10-04 - In Plugin Catalog
  * Edit Trails:
    * The Edit Trail dialog is no longer modal, you can access ST while the window is open. The route view can be used to move around points and activities.
    It is possible to abuse the
    * Enable drag&drop for TrailPoints, TrailPoints can be moved on the map.
  * When selecting part of the trail or a single point, center map but never zoom in. Only Zoom out if no match.
  * Optimized trail detection time to one third of previous. To calculate all trails on all activities in my logbook previously took over 30 seconds, now about 11 seconds. Normally, the time to format data for the list map and graph requires more time. The limits for auto calculation is increased.
  * Optimizations to find the position where trail matches. Affected certain situations only (mostly start point), for instance where there were several points prior the best match.
  * Fastest pace/speed incorrect
  * For pace/speed columns: Show reference pace/speed instead of each Activity pace/speed (so same data when comparing for instance running/biking).
  * If meter were used for distance, pace were normally 00:00. Is instead min/km.
  * Italian translation update

1.0.439 2011-09-06 - In Plugin Catalog
  * Select a single point when adding to trail did not work

1.0.437 2011-08-28 - In Plugin Catalog
  * Chart keyboard shortcut: 'l' to hide Trail Points, Shift-'l' to show them again (not saved in settings)
  * If an activity has only rest laps and RestLapsIsPause is set, handle them as Active anyway
  * If Reference Activity has no GPSRoute, set as NotInbound directly (otherwise Reference activity shows up also if the reference had no GPS track and therefore no trail points)
  * Diff graphs for Splits trail could look strange
  * Diff Distance over Time always show difference to itself, not diff to reference

1.0.430 2011-08-21
  * Split handling of rest laps and non-required points: 'a' handles active laps only, new keyboard shortcut 'n' handles non-required only. See [Result List](Features#result-list).
  * When creating diff charts for subsplits, use the corresponding subsplit for the reference (previously the complete ref was used, so subsplits were compared to the beginning of the reference).

1.0.425 2011-08-07
  * Result List: Only increase size if very small, never decrease. Also increse size of border, so it is easier to change manually
  * Edit Trail: If not viewing activity, no trail points were shown. ST requires a selection with an activity to show a map though.
  * AddInbound did not always work

1.0.422 2011-08-06
  * When adding a trail point on the map, add both lower and upper limits, if they differ more than the radius.
  * Progressbar when calculating trails
  * For time/dist diff, use actual difference at last point in the track. This can give a "jump" but shows better differences. (This is regardless if "Resync at TrailPoints" is activated).
  * When selecting, the plugin did not always zoom on the map
  * Edit Trail: Possible to delete more than one point at once
  * Edit Trail: Map Zoom Control
  * Edit Trail: Improve incomplete match display, also show also reverse matches.
  * Naming trail points for Splits/Reference Activity (if Laps not have names)
  * Result list keyboard shortcut 'i' changed:
    * ST Default filter is My Activities and My Friend's Activities: Insert activities in the current category (Instead of selecting all activities in reports view, this can be done from Daily Activity).
    * ST Default filter is other than My Activities or My Friend's Activities: Insert activities in the selected category.
  * Result List: Keyboard shortcut to calculate all trails:  Shift-'r'. If there are many activities, only inbound will be calculated by default, to limit calc time.

1.0.408 2011-08-01 - In Plugin Catalog
  * Minor updates for German translation
  * Difference to reference result shows the difference to the average result instead of being ignored (or replaced with SpeedPace) for single results or being a straight line (for multiple results).
  * Workaround for limitation in Track Coloring plugin, that ignored tracks marked by Trails.
  * Increase priority for Marked tracks, so they appear on top of normal tracks.
  * Mark only line chart for line/ fill charts, to much clutter.
  * When selecting results not related to the Daily Activity, the chart was not updated (the chart should be selected, so it can be found).
  * Add SpeedPace column, that displays speed or pace depending on the activity.

1.0.397 2011-07-25
  * German translation by omb
  * French translation by meven (existed previously, but was not built correctly).
  * Some Spanish translation updates
  * Possibility to see distance from trail point to first GPS point. See [Result List](Features#result-list), keyboard shortcut 'i'.
  * ResultList shortcut Shift-'c': Use CommonStretches for Diff Time over Distance. See [Result List](Features#result-list).

1.0.389 2011-07-23 - In Plugin Catalog
  * Diff time and distance checks could be incorrect if the activities differed a lot (i.e. if the non ref activities had a long detour).
  * Possibility to override "Include Stopped" for certain categories. See [Hidden Options](Features#hidden-options).
  * For Splits trail: If an activity has no laps, create a trail point every kilometer, to have something to start with.
  * When selecting a large activity set, the trail was not calculated (as intended), but the GUI did show a selected trail anyway.

1.0.381 2011-07-19
  * Edit Trail: Always show activity time/distance (instead of result time/distance).
  * Edit Trail: Double click on Distance/Time will set TrailPoint.
  * Edit Trail: Show Distance/Time also for added point (before there is a result including the point).
  * Possible to show only Reference Result for graphs for right side axis. See [Result List](Features#result-list).
  * Add setting for Add Current Activity. Runs if single activities are selected. Previously only keyboard shortcut [Result List](Features#result-list), 'i'.
  * Do not draw track on route if there is a pause. (ST paints the track but marks where it start, but pause markers could be hiding info if there are many tracks).
  * Handle non-required points as start of pause laps, optionally handle them as pauses. See [Result List](Features#result-list).
  * Delta Time over Distance had incorrect offsets at pauses.
  * Selecting on map (for single activity) also selects for subsplits.
  * Graphs over time with pauses were incorrect
  * Internal restructuring to minimize memory usage and speed up handling. However, the pause handling for Delta and time graphs offsets this some.
  * Exception occurred if ST was started without logbook ([issue 23](/gerhardol/trails/issues/23)).

1.0.364 2011-06-24 - In Plugin Catalog
  * Edit Trail: Reference Time/Distance shown
  * Edit Trail Export: show reference result (if it exists)
  * Avoid popup "Do you want to add points" when editing trail, even if no points were selected, when ST had one null item selected
  * Trail detection handling rewritten. The handling had grown very complex over time and there were some corner cases where not the "best" point were selected.
  * Distance were off with a few meters for trails as ST distance calculations has some limitations.
  * When editing/deleting trails, only recalculate the edited trail

1.0.356 2011-06-06
  * Dont change ref Trail when selecting no activities
  * Resync diff time/distance at trail points
  * Option to sync chart at trail points in menu
  * Localtime for FilterStatistics integration
  * If any type of "marking" starts/ends after result bounds, use start/end
  * Time/Dist diff did not handle pauses correctly ("steps" in chart).
  * List keyboard shortcut 'i': Insert in current activity category only, not the category selected in ST.
  * When selecting Active laps only, hide subsplits shorter than a second
  * Add checks for results without distance or GPS points
  * If deleting results, route was not updated
  * Sizing of result list more sensible
  * When editing Trail, keep trail after update
  * Control Adjust Resync at Trail Points from menu

1.0.345 2011-04-25
  * If no activity has GPS data, set Reference Activity trail to NotInBound by default.
  * Some exception preventions, if Splits covers other time period than the activity tracks.
  * Keyboard shortcut in result list: Shift-'i': Insert all activities, including both My Activities and My Friends Activities.

1.0.342 2011-04-22 - In Plugin Catalog
  * Resync rounding updates
  * Do not resize the list so frequently

1.0.340 2011-04-22
  * Use fill chart for "left axis" when there is one result
  * Grade axis formatting
  * "Resync" distance start offset adjustment

1.0.338 2011-04-21
  * Release as Trails 1.0

0.8.337 2011-04-21
  * Extend GPS track when the estimated start/end has no GPS point.
  * If track starts with offset (for instance rest) compared to detected point, add offset in "resync" display.
  * Add keyboard list shortcut 'i' to insert activities from current category
  * Tweaked colors slightly for charts used for single activities, to better match ST default.

0.8.331 2011-04-15
  * resync chart at trail points. Use chart keyboard shortcut 't'. See the documentation: [Trailpoint_Synchronization](Features#trailpoint-synchronization)

0.8.328 2011-04-14
  * Non-GPS activities support
  * Trailpoints can match also between GPS points on a track, on the closest expected point.
  * Possible to view/edit trails also when no activities are selected
  * Further tweaking to pass-by checking (when track passes without a GPS point is in radius).

0.7.319 2011-04-12
  * Trail detection: Pass-by (when no points in radius) updates. http://www.zonefivesoftware.com/sporttracks/forums/viewtopic.php?f=76&t=10455
  * When in expanded mode, charts that previously was not displayed due to no data, were not reevaluated when switching activities.
  * FastestSpeed/MaxHR is derived from smoothed track instead of unsmoothed

0.7.314 2011-04-10
  * Mark axis when smoothing graphs
  * Elevation was not shown

0.7.311 2011-04-10
  * Smooth data with keyboard shortcuts in chart:
    * 'c' for cadence, 'e' for elevation and grade, 'h' for heartrate, 's' for pace and speed, 'p' for power.
    * Use Shift to decrease, Ctrl to set to 0, no modifier to increase smoothing.
    * The current value is shown with a tooltip.
  * Possibility to handle rest laps as pauses. No GUI, use keyboard shortcut 'a' in List to activate, shift-a to reset
  * Increase list size with keyboard shortcut 's'.
  * Incorrect distance if there were pauses in the activity

0.6.307 2011-03-22
  * Added French translation
  * Updated Spanish translation

0.6.302 2011-03-18
  * Multiple charts in graph
  * Power charts
  * Tweak single point trail selection

0.5.296 2011-03-10
  * Many minor changes:
    * Diff charts are automatically Time over distance or distance over time
    * Result list resizes better
    * When adding InBound activities, include splits
    * Add InBound also for reference results
    * Possibility to add a filter for very short subtrails, to avoid "close" matches. No GUI, add minDistance=&quot;100&quot; in Preferences.System.xml for the trail.
    * When a single activity is (originally) selected, use ST standard to mark the trail
    * Set one result line in the list by default
    * Various additional checks and tweaks

0.5.278 2011-03-01
  * Trails west of 90W and 90E would not be found

0.5.277 2011-02-28
  * Italian and Spanish translation update

0.5.274 2011-02-27
  * Spanish translation update

0.5.273 2011-02-26
  * Keyboard shortcuts for the list:
    * SPACE: Toggle SelectSimilarSplits
    * Ctrl-C: Copy list
    * u: Add similar with Unique Routes
    * c: Mark Common Stretches with Unique Routes
    * DEL: Delete selected results from list
    * Shift-DEL: Delete all but selected results from list
  * Add InBound activities as results (from List->Advanced menu). This can be used to find activities in a certain area or to find why an activity do not match a trail.
  * Export Trail as activity, to share Trail (From Edit Trail menu). (Trails can be created from activities.)
  * When pressing Add and a generated trail is used, never suggest to add to it
  * Copy Trail - save a copy of the trail instead of overwriting the existing Trail.
  * Change how trail is selected when switching activities, to avoid using generated trails by default
  * Show number of results where possible in the trail list
  * When checking if a trail is inbound of an activity, the area must be adjusted for the trail radius. A simplified algorithm were used that used an incorrect longitude.
  * Ignore non-required trail points in trail bounds check
  * When editing Splits Trail, include reference points (including rest laps as not required trail points).
  * For Splits generated Trail, include Rest laps

0.5.257 2011-02-10
  * Partial Trails: Add possibility to mark points as not required
  * When editing trail points, draw the point
  * Change icon in trail list if there is at least one match
  * Increased the area used when checking if a trail is in bounds.
  * In some cases with smaller radiuses, the Trail could not be found

0.5.248 2011-02-04
  * FilteredStatistics update. Only visible in MainView
  * Avoid exception when viewing diff in some situations

0.5.244 2011-01-29
  * Italian translation

0.5.243 2011-01-29
  * Handle Pauses and Stopped time better.
  * Pace in list could be incorrect if there were very slow parts.
  * Expanded charts could show empty charts instead of hiding them.
  * FilteredStatistics integration - use Trails to filter part of activities. The implementation is not complete.
  * Added StartDistance column
  * Show SpeedPace graph is Diff is selected ut the only chart is same as the reference.
  * Sort generated Trails in order: Reference Activity, Splits, HighScore (otherwise HighScore was often selected by default).

0.5.232 2011-01-16
  * Exception: http://www.zonefivesoftware.com/sporttracks/forums/viewtopic.php?f=11&t=9850

0.5.231 2011-01-15
  * Fixed Columns in table
  * Zero distance for activities with only Rest laps http://www.zonefivesoftware.com/sporttracks/forums/viewtopic.php?f=32&t=8093&p=55083#p55083
  * Color Selector - colors can be customized.
  * Zoom to trail when selecting
  * UR integration: Uses ref result rather than ref activity (requires updated UR)
  * Slightly darker color when selecting
  * Import could hang if name set at import
  * Show sort direction in table
  * TrailPoint edit: move points up/down in the list

0.4.221 2011-01-03
  * Unique Routes Integration: Mark Common Stretches in chart and Route
    * HighScore Integration: Generate a HighScore Trail if HighScore is installed. The HighScore settings are used. Display tooltip for Goal as in HighScore table. (tooltip enabled for other trails too).

0.4.215 2010-12-25
  * ST scrolls at selection, so marking is occasionally incorrect. Workaround that selects the reference result for subsplits. Click to mark still do not work.
  * If more than one result, use activity color.
  * A few fixes (like clicking outside the list, axis markers were not cleared)

0.4.212 2010-12-23
  * No selection change when clicking + in list
  * CultureInvariant preferences parsing (so the system settings must not be identical if Preferences shared)
  * If a trail passes the entry point before the second point is seen, restart
  * If a trail name exists already when adding a new trail, report the specific error
  * If a single chart to show is empty, show SpeedPace instead

0.4.209 2010-12-21
  * CalculatedFields ambiguity
    http://www.zonefivesoftware.com/sporttracks/forums/viewtopic.php?p=54397#p54397
  * When clicking and SelectSimilarResults is set, only mark the clicked row. Will not work when expanded, ST reports incorrect row selected.
  * When selecting reference result, use the "rightclick row" and show it in the menu

0.4.207 2010-12-19 - In Plugin Catalog
  * Exception: http://www.zonefivesoftware.com/sporttracks/forums/viewtopic.php?f=76&t=9669

0.4.206 2010-12-18
  * Select similar subtrails and laps is a setting, so activity comparision is stepped
  * Results can be excluded from the list
  * Set activity name from Trail when importing
  * Sorting changed for listed trails

0.4.200 2010-12-17
  * Matrix integration gave exception

0.4.197 2010-12-12
  * Unique Routes integration. Activities can be added based on the reference activity. Also in ST2
  * Selecting on map/chart will scroll list instead of select
  * Clicking track on map in multi mode will mark the result in chart
  * If Diff time/dist is selected but only reference result shown, show speedPace instead.
  * Only active laps in reference activity

0.4.188 2010-12-05
  * Save multichart settings
  * Avoid exceptions for short distance activities
  * Splits trailpoints must not be on the same index
  * Try to make workaround with trails cover the same area due to
    http://www.zonefivesoftware.com/sporttracks/forums/viewtopic.php?p=54017#p54017

0.4.184 2010-12-05
  * Diff charts for time/distance, to compare results
  * Marking in route view vs the list vs the charts are synchronized
  * Multiple results in charts
  * Add automatic Trail: Reference Activity, based on Splits for one activity (same Activity as Reference Result)
  * Set reference result for splits
  * Name for list. For sub-trails, it is the Split/Trailpoint name
  * Select similar splits in List. For instance, all splits with index no 1.
  * Multiple trails in charts, to compare trail results

0.3.160 2010-11-20
  * "Sub-trails" listed in summary

0.3.156  2010-11-17
  * Result List can be sorted
  * Color column
  * Doubleclick to go to activity
  * Limit selection from selected in lists. Allows handling in other plugins.
  * View Trails on route map also in Reports view

0.3.149 2010-11-14
  * Select on Routes view select in list
  * Multi selection in list, also to map

0.2.145 2010-11-06
  * TrailPoints are shown in charts.
  * Selecting in Charts and Route update each other, like in ST core charts. Similarly, selecting in the list selects in the Route.
  * New algorithm to select points in Trails:
    * First point: Closest in continously leaving middle
    * Middle points: Closest to center
    * Last: Closest when approaching
      There is some hysteresis for first/last to handle GPS errors. The algorithm handles trails with points that overlaps better than before, for instance out and back activities and can be used to analyze splits on a 400m track.
  * Support trails with one point
  * There is a kind of score based on errors when selecting the trail to show.
  * Avoid crashes with really big radius and close zoom

Changes that do not use the ST3 API also included in the ST2 version. In this case the chart/route interaction and possibility to add laps using laps is not available in ST2.

0.2.134 2010-10-30
  * Tuned way to find matches:
    * First: Closest in "stretch" leaving middle
    * Middle: Closest
    * Last: Closest approaching
  * Some hysteris for first/last to make sure the complete activity is included. A fix for last point, first in next trail may start before end to avoid missing trails
  * Support trails with one point
  * A possibility to score quality of hits, selecting best

0.2.133 2010-10-19
  * CalculatedFields integration
  * Spanish translation

0.2.119 2010-07-31
  * When editing points, zoom on the map
  * Add buttons when editing trails

0.2.119 2010-07-30
  * Enable hiding of ChartTools

0.2.115 2010-07-30
  * First version where all ST2 functionality was ported to ST3. This includes a lot of internal changes, a large part of the plugin was rewritten to work. From this on, there are ST3 specific functions added.
  * Many internal touchups and GUI updates.

0.2.84 2010-06
  * Matrix integration - Trails can be used for Matrix

0.2.70 2010-05-21
  * Save settings/Trails in System.Preferences instead of the logbook.

0.2.65 2010-05-16

gerhard's first release.
  * Expanded mode: Show more graphs instead of Route (map). Note: This functionality was available on Google Code since March 2009, but not included in a released version.

  * Installation did not work if previous version was not installed
  * Radius must be int
  * A few other exceptions

  * License changed to LGPL

  * The Trails in the logbook (if version prior to 0.2.70 was installed) are not removed automatically, it is kept as a backup. The data will removed in a later version.

0.1.5 2009-03-25

Brendan Doherty's last official version.

### Feedback ###
For patches, bugreports or feature suggestions, use the Google Code issue list.
For other feedback please use the SportTracks forum or this wiki.