

# Getting Started #
See separate page: [Getting Started](Tutorials.md)

# User Interface #

## Daily Activity View ##
![http://wiki.trails.googlecode.com/hg/images/overview.png](http://wiki.trails.googlecode.com/hg/images/overview.png)

## List of Trails ##
  * ![http://trails.googlecode.com/hg/Resources/square-green-16-ns.png](http://trails.googlecode.com/hg/Resources/square-green-16-ns.png) Green square means that you went on that trail - there are results (number of results listed). (If there are results [filtered](Features#Filter_with_UniqueRoutes.md), the number of filtered results will be listed too.)
  * ![http://trails.googlecode.com/hg/Resources/square-green-remove.png](http://trails.googlecode.com/hg/Resources/square-green-remove.png) All required points have not been hit for this trail, but there is still a result. See [Partial Trails](Features#Partial_Trails.md).
  * ![http://trails.googlecode.com/hg/Resources/square-green-check.png](http://trails.googlecode.com/hg/Resources/square-green-check.png) The result is for a generated Trail, no results has been calculated yet. See [Split Analysis](Features#Split_Analysis.md).
  * ![http://trails.googlecode.com/hg/Resources/square-green-plus.png](http://trails.googlecode.com/hg/Resources/square-green-plus.png) There are activities in bounds, but no results has been calculated yet, to avoid longer calculation times when viewing many activities. The number of activities in bounds are shown. See [Multiple Activities](Features#Multiple_Activities.md).
  * ![http://trails.googlecode.com/hg/Resources/square-red-plus.png](http://trails.googlecode.com/hg/Resources/square-red-plus.png) Red square with plus means that all trail points are included in the area for at least one activity and that at least one trail point matches (the list shows the number of partial matches and inbound activities). If you did go on this trail, try increasing the radius of the trail-points, or modify the trail-points slightly. You can list all activities inbound: [Add InBound activities as results](Features#Add_InBound_activities_as_results.md)
> > Note: Searches are pruned when possible, so if the first required point does not match, the icon will be a solid red square.
  * ![http://trails.googlecode.com/hg/Resources/square-red-16-ns.png](http://trails.googlecode.com/hg/Resources/square-red-16-ns.png) Red square means that no activity matches the trail, but all trail points are included in the area for at least one activity.
  * ![http://trails.googlecode.com/hg/Resources/square-blue-16-ns.png](http://trails.googlecode.com/hg/Resources/square-blue-16-ns.png) Blue square means the no selected activity is included in the trail area.
  * No icon means no information, like HighScore Trail when HighScore is not installed or the Elevation trail (see [Barometric Elevation Adjustment](Features#Barometric_Elevation_Adjustment.md)). This also applies to trails without any trail points or a 0 radius.


> ![http://wiki.trails.googlecode.com/hg/images/traillist.jpg](http://wiki.trails.googlecode.com/hg/images/traillist.jpg)

Note: The plugin tries to find the "best" match by time for a trail point, even if it is between two GPS points.

## Multiple Results ##
If the same trail matches several times several laps on the same activity or several activities (see [Multiple Activities](Features#Multiple_Activities.md)), there can be several result rows. To see more than one result in the chart, select in the result list with Ctrl-click or Shift-click. The results are visible in the graph, using the color listed in the result list.

![http://wiki.trails.googlecode.com/hg/images/trails_activities_pace.jpg](http://wiki.trails.googlecode.com/hg/images/trails_activities_pace.jpg)

## Multiple Trails ##
It is possible to compare the results for several trails at the same time, by opening the trail name list by right-click on the list. The name of the first added trail is suffixed with "(`*`)". If opening the the list, all selected trails can be seen and deselected.

## Summary List Result ##
The summary list has a summary row for all selected results. If only one or zero normal results is selected in addition to the summary row, all results are included in the summary.

The summary are also displayed in the chart, similar to the summary row. The charts are the merged results from the included results. The chart displays the summary with black bold lines.
This feature is similar to category average in [Overlay plugin](http://code.google.com/p/gps-running/wiki/Overlay#Category_average).

There is no display on the route for the summary line.

## Reference Result and Reference Activity ##
The plugin selects one of the results as the _reference result_, the activity for the result is the _reference activity_. The reference result is used when [comparing results](Features#Reference_Diff.md) or [Features#Filter\_with\_UniqueRoutes](Features#Filter_with_UniqueRoutes.md).

The plugin also uses the reference result and activity, trying to keep selected the activity when switching trail or set of activities.

## Special Trail Properties ##
Set the properties in Edit Trail Dialog, checkboxes at the bottom (see tooltip).

### Default Reference Activity ###
The [reference trail](Features#Reference_Result_and_Reference_Activity.md) result is by default automatically selected from the results. If you want to have a fixed reference result you can set a default reference activity that is always included when calculating the trail. (The activity is added to the selected activities, so it may match other Trails too.) It is still possible to select another reference, the default is selected when the trail is selected.

The reference result is selected from the activity. Note that by modifying the trail or the activity or the trail, the activity may no longer match the trail. The plugin then uses another result as the reference.

### Partial Trails ###
All points are not required for a trail. If you deselect the "required" check box for a trail-point, the point is not required for matching a result. So if you have trails A-B-C, A-D-C and A-E-C, where A-B-C and A-D-C are "equal", add Trail A-B-D-C and deselect required for B,D to get both results.

### Two-Way Trail ###
Selecting this check box (see ToolTip for bottom boxes), the trail matches both in forward and backword direction.
The "up-arrow" close by will resort the trail points.

### Match by Name ###
Selecting this check box (see ToolTip for bottom boxes), the trail matches on the activity name, in addition to the trail points. So it is possible to match on the name only (if it has no trail points), or to find activities that "should match" but some points are off.

### Match Complete Activity ###
Let the trail result match the part before the first and after the last trail point. This makes Trails report the summary like UniqueRoutes, to find the full activity for a track passing certain trail points. If a trail has several results, the results are merged to one.

### Filter with UniqueRoutes ###
Trails finds result by comparing the trail points only. There is control in what results that are selected with flexibility in where points are placed and the radius, as well as required/optional trail points. The GPS points in between trail points are not checked. For instance UniqueRoutes checks all points but has other limitations. See [UniqueRoutes Internals](https://code.google.com/p/gps-running/wiki/UniqueRoutes#Internals).

It is possible to detect trails with points, then apply [UR Route Snippets](https://code.google.com/p/gps-running/wiki/UniqueRoutes#Selected_Route_Snippet) to filter the results using the GPSRoute for the reference result, testing if the other results are similar. This will give something similar to Strava segments.
Note as the Reference results is used for the intermediate GPS points, the results will depend on the current reference result GPS trail. To get predictive results, you may select a [Default Reference Activity](Features#Default_Reference_Activity.md).

Note: The trail list lists the number of results that was filtered by UniqueRoutes.

### Trail Sort Priority ###
Each trail has a "sort priority", default 0. The trails with higher sort priority are listed first in the dropdown list, so "interesting" are listed first. The sort order also affects auto selection when selecting activities and at import.
Trails with sort priority lower than -1 are not calculated automatically when activities are updated.

A typical use case is to set special sections like warmup, climbs etc to lower priority that the main trail.

## List-Route-Chart interaction ##
The plugin implements interaction between the result list, the route (map) and the charts. For instance when a part of the trail is selected in the chart, the corresponding section in the chart is selected. If you select a chart or route, the corresponding entry in the list is scrolled so it is visible.

It is possible to select on the route track and get feedback on the charts. The behavior is slightly dependent on how the route is drawn:
  * Single activity routes drawn by ST behaves like standard ST: MouseDown-MoveToSelect-MouseUp. See [ST Help](http://www.zonefivesoftware.com/sporttracks/support/help/?topic=manual-exploring-activity-details#measuring-route-distance). An addition is that it is possible to multi-select, just select again. Clear selection by clicking in the chart.
  * Routes drawn by Trails behaves differently.
    * First click scrolls the activity so it is visible in the list (first time only), then shows the position in the chart (also subsequent loops).
    * Second click selects the track section, displays the section on the chart.
    * Multi section selection is possible, just continue clicking the same trail result track. First click shows the position on the chart, the second marks the section. As only one range can be shown on the chart, the chart marks the last selection.
    * To cancel a selection, click the chart.
    * Note that ST standard route and trail points have "click priority" over trail routes. You can only select outside them.
    * "Close section clicks" can be ignored. (There should be a couple of GPS points in between section start/end). If so, there is a status message in the chart (as a tooltip). The positioning is done similar to how Trail results are calculated, the positioning may differ slightly to the clicked position.

The plugin allows multi selection in the charts, using Ctrl-click.

## Smoothing ##
The charts can be smoothed directly without going to ST settings. The smoothing is done in the same way as in ST, see [ST FAQ](http://www.zonefivesoftware.com/SportTracks/Forums/viewtopic.php?p=30602) for more information.

Click the chart axis to smooth, then use PageUp to increase smoothing with 10 s or PageDown to decrease smoothing with 10 s. Use Home to set to default value and End to set to 0. See [Keyboard shortcuts](Features#Keyboard_shortcuts.md) for more shortcuts.

The current value is shown with a tooltip, the axis is marked too.

Standard ST charts are smoothed over the complete activity only. For instance when running intervals and start/stopping sharply, the normal smoothing does not work well. The speed in the last part of a rest split affects the chart for the first part of the active fast part. This setting controls how sections are smoothed. The setting is toggled from the graph menu or by the graph keyboard shortcut Ctrl-'a'.
  * All: Standard ST behavior
  * Unchanged: All sections not changed are smoothed. This means that "blocks" of Active/Rest sections are smoothed together. (So for instance if autolap is used with activities with warmup/cooldown, the active part is smoothed separately.) This is the default in Trails.
  * All: Individual smoothing in each section.
Note: It is possible to view the splits in isolation too.

## Offset ##
Normal trails are compared from the trail points and an offset is normally not relevant (i.e. the relevant parts of the Activities match at trail points). Splits trail do not have any trail points, and the activities start from zero. To compare relevant parts, offsets may be required, for instance if the sources were not started at the same time or the time of one activity is off. This is similar to offset in [Overlay](https://code.google.com/p/gps-running/wiki/Overlay) plugin.

Offset can be applied in several ways:
  * If the trail is Splits and a track overlap in time compared to the reference trail result, an offset is applied based on time automatically.
  * Chart keyboard shortcut Alt-'o': select any graph for a result and drag it in the direction you want to offset the result.
  * Chart keyboard shortcuts similar to [Features#Smoothing](Features#Smoothing.md) ('o', PageUp), except that offset is changed instead of smoothing. (PageUp etc changes offset if a diff graph is selected first.)

The offset is displayed as a tooltip for diff graphs (if it exists), similar to [Features#Smoothing](Features#Smoothing.md).

It is possible to apply the displayed offset for a result to the activity itself. This can be used to update activities that has incorrect start time. See [Keyboard shortcuts](Features#Keyboard_shortcuts.md) Ctrl-'o'.

## Trailpoint Synchronization ##
Even when taking the same route several times, there are factors that affect the tracks: Distance measurement deviations, detours etc. Therefore comparing activities will deteriorate over the distance, so the parts compared at a certain distance is from separate parts of the activity.
This could be overcome by comparing the subtrails individually, but this will not give an overview.

The trailpoint synchronisation adjusts the charts, so the displayed chart match in each trailpoint. If you have sufficient number of trailpoints, the charts will show the same sections. The match is of course better just after the trailpoints. With sparse trailpoints, the charts before the trailpoints may be cut (if the result is longer than the reference) or has no points (if the reference is longer).

The feature is activated in the graph banner menu or by selecting the graph and press 't'.
See [Keyboard shortcuts](Features#Keyboard_shortcuts.md) for more shortcuts.

Note: When activating this feature, the X-axis (distance or time) for all results except for the reference result could be incorrect, as all other results effectively are adjusted to the reference results. The chart-map interaction could therefore work unexpected around trailpoints. Another caveeat is that the result can look strange if the results have pauses and non-required trail points.

This feature is related to [Resync Diff at Trail Points](Features#Resync_Diff_at_Trail_Points.md), that resyncs the sections used in differences, if you want the diff calculation to be synced in trailpoints.

An example of the effects of the feature. Note that this example uses "Passive Laps are Rest". For instance one of the red track starts with a rest lap and is therefore offset from the others. The sync handles this too, adding the offset in the beginning at the trail point. Similarly, the activity has a rest lap at the top, where the distance differs a lot among activities. The sync aligns the tracks after the rest lap, as can be seen by the elevation jump.

![http://wiki.trails.googlecode.com/hg/images/elevation-trailsync.png](http://wiki.trails.googlecode.com/hg/images/elevation-trailsync.png)
![http://wiki.trails.googlecode.com/hg/images/elevation-nosync.png](http://wiki.trails.googlecode.com/hg/images/elevation-nosync.png)

## Chart Expansion ##
The graph in normal mode can show more than one chart type.
You can view more than graph with the expansion button on the chart banner. The multiple charts are shown instead of the route.
![http://wiki.trails.googlecode.com/hg/images/trails_multicharts.jpg](http://wiki.trails.googlecode.com/hg/images/trails_multicharts.jpg)

## Keyboard Shortcuts ##
### Result List ###
  * SPACE: Toggle SelectSimilarSplits, see [Subtrails and Splits](Features#Subtrails_and_Splits.md)
  * DEL: Delete selected results from list [Exclude result from list](Features#Exclude_result_from_list.md)
  * Shift-DEL: Delete all but selected results from list. Only parent results are affected, subtrails/splits are ignored.
  * 'F5': See 'r'
  * 'a': Handle activity rest laps as activity pauses. (Compare to 'n'.)
  * Shift-'a': Include rest laps in result (default).
  * Ctrl-'a': Select all results
  * 'c': [Mark Common Stretches with UniqueRoutes](Features#Mark_UniqueRoutes_Common_Stretches.md)
  * Shift-'c': Diff charts based on UniqueRoutes - Common Stretches calculations. The diff is only for the common parts, syncing at start of common stretches.
  * Ctrl-'c': Copy list to clipboard.
  * 'e': Toggle setting to get [diff](Features#Reference_Diff.md) to itself instead of to the reference result.
  * Shift-'e': Toggle setting to use normal elevation track (normally GPS elevation) or the device elevation track (normally barometric) for Grade Adjusted Pace and elevation change. If the device elevation track do not exist, normal (GPS) elevation is still used.
  * Alt-'e': Toggle setting to insert a device elevation track, from other devices with the same time. For each result that do not have a device elevation track (normally barometric), check if any of the result activities have a device elevation track for the result time. Secondary check if any of the activities in the logbook has a device elevation track for the result time. If the chart has set an offset using ['a' shortcut](Features#Graph.md), the device shortcut is offset too.
  * Ctrl-'e': Select all results with a separate elevation track.
  * Shift-Ctrl-'e': Adjust the Device Elevation tracks to [Elevation Points](Features#Elevation_Points.md).
  * Alt-'f': Apply the device elevation track (that may be from another device and possibly be adjusted) to the activity GPS track (this is the elevation used in ST).
  * 'i':
    * ST Default filter is My Activities and My Friend's Activities: Insert activities in the current category (Instead of selecting all activities in reports view, this can be done from Daily Activity).
    * ST Default filter is other than My Activities or My Friend's Activities: Insert activities in the selected category.
  * Ctrl-'i': Insert activities in the category below current topmost level. For instance if you use "My Activities:Running:Trail", insert activities in "My Activities:Running".
  * Shift-'i': Insert all activities, including both My Activities and My Friend's Activities.
  * 'n': Non-required trail point starts a pause for the result. (Compare to 'a'.)
  * Shift-'n': Non-required is not pause, included as result (default).
  * 'o': Toggle contents of Start Distance. Start Distance list field normally shows the distance the activity starts. Using this will instead put the distance from the first matching trail point to the first point in the GPS track. Shift-'o' sets back to normal. The setting is not stored when restarting. The rationale of this setting is to find activities with pauses at a trail point, for instance if the start button was pressed late (or the first match occurred at a pause/rest lap, see keyboard shortcut 'a').
  * Ctrl-'o': Toggle show Standard Deviation for Duration, Distance and Average Pace/Speed. (For Distance, the deviation is shown with Elevation units.) The standard deviation is a measure of how much the result differs. About 2/3 of the results is within +/- the standard deviation (the activity data is likely not distributed to conform with this, this is still a measure of the exactness).
  * Alt-'k': If the result has no cadence, try inserting a graph from an activity that overlaps and have cadence information. Compare to Alt-'e'.
  * 'q': Toggle algorithm for [Grade Adjusted Pace](Features#Grade_Adjusted_Pace.md).
  * Shift-'q': Reset [Grade Adjusted Pace](Features#Grade_Adjusted_Pace.md).
  * Alt-'q': Set [diff adjust split times](Features#Reference_Diff.md).
  * Ctrl-Alt-q (or AltGr-'q'): Set [Pandolf Terrain factor](Features#Grade_Adjusted_Pace.md).
  * 'r': Refresh display calculations for current trail
  * Shift-'r': Clear all data and calculate all trails
  * 's': Increase result list size
  * Shift-'s': Decrease result list size
  * Ctrl-'s': Show device reading for time, dist etc instead of track/trail based (to compare device calculations)
  * 't': Toggle show total instead of average in the summary line.
  * Shift-'t': Insert activities with same start time as reference activity if time overlaps. See [Multiple Data Sources](Tutorials#Multiple_Data_Sources.md) for an example to use this and for other effects of the shortcut.
  * Ctrl-Alt-'t': Toggle setting to use the pauses for the reference result for results that overlaps the reference result. See [Multiple Data Sources](Tutorials#Multiple_Data_Sources.md) for an example to use this. This settings is active until SportTracks is restarted, it is not stored in settings.
  * Shift-Ctrl-Alt-'t': Toggle setting to diff over time-of-day. For the results that overlaps the reference result, the distance-diff over time is based on date-time instead of elapsed time. See [Multiple Data Sources](Tutorials#Multiple_Data_Sources.md) for an example to use this. This settings is active until SportTracks is restarted, it is not stored in settings.
  * 'u': [Add similar with UniqueRoutes](Features#Add_similar_with_UniqueRoutes.md)
  * Ctrl-'z': Toggle Zoom to selection, normal is to set location only. Also, when selecting in chart, mark all results in the chart.
  * Shift-'z': Toggle show only marked results on the map (to avoid cluttering).

### Graph ###
For graphs, after selecting a chart axis by clicking it:
  * PageUp: Increase smoothing in bigger steps (normally 10s)
  * PageDown: Decrease smoothing in bigger steps (normally 10s)
  * Home: Reset to default value used in ST
  * End: Set to zero
Clicking an axis also marks the charts for the result.
This also applies to [Offset](Features#Offset.md) for the Diff graphs.

General:
  * Smooth data with keyboard shortcuts in chart. Decrease by combining with Shift.
    * 'c': cadence
    * 'e': elevation and grade
    * 'h': heart rate
    * 's': pace and speed
    * 'p': power
    * 'o': offset (result offset, handled similarly to smoothing)
  * Alt-o: Change the Offset for the result related to the selected graph, in the direction the selection was dragged.
  * Shift-Ctrl-'o': Apply the displayed offset for a result to the activity itself. This can be used to update activities that has incorrect time.
  * 'l': hide Trail Points, Shift-'l' to show them again (not saved in settings)
  * Ctrl-'r': toggle only reference result with right hand axis graphs (if there is more than one result). Use for instance Time difference to the left and use this to get less cluttered charts. The "ST standard" color is used for graphs, instead of the reference result color.
  * Shift-'r': For time and distance differences, the normal is to use the reference for the difference. This will toggle use of self for difference instead.
  * 't': activate [Trailpoint Synchronization](Features#Trailpoint_Synchronization.md).
  * Shift-'t': deactivate Trailpoint Synchronization.
  * 'a' to sync the graphs to the reference graphs, to for instance align elevation from one activity to another. The settings cycle through the sequence: None (Default), Start, End, Average, Min, Max. The setting (other than None) is displayed as a tooltip when refreshing.
  * Graph Keyboard Shortcut: Ctrl-'a' to toggle smoothing over sub result borders. See [Smoothing](Features#Smoothing.md).

# Comparing #

## Multiple Activities ##
To compare multiple activities, select week or month in Daily Activity view or a group in Reports view or just Ctrl-click the activities. (Before ST 3.0.4068 a "group" had to be selected to see distances.) It is also possible to use [Add similar with UniqueRoutes](Features#Add_similar_with_UniqueRoutes.md) to insert activities to use or [insert activities in the current category](Features#Result_List.md).

Each trail result can be displayed as described in [Multiple Results](Features#Multiple_Results.md)

Note: The plugin will only automatically calculate if a trail has "inbound" activities for potential matches (no trail result calculation) if there are many activities to wait times. When selecting the trail, the actual calculation is done, and the result may go from inbound to no results.

## Subtrails and Splits ##
The subtrails or splits can be expanded by clicking the + sign in the result list.
You can compare similar subtrails or splits with the same index number by right clicking and select _Mark other sub Trails or splits in list_. When you change selected results, similar are selected automatically.

![http://wiki.trails.googlecode.com/hg/images/trails_subtrails.jpg](http://wiki.trails.googlecode.com/hg/images/trails_subtrails.jpg)

## Split Analysis ##
The plugin is primarily designed to work with predefined Trails, but can be used to compare Activity Splits too, for activities with GPS data.
  * Select the generated trail _Reference Activity_ to compare Trails matching the reference activity. The trail points are placed at each split for active laps for the activity. Select reference activity by right clicking in the list for a result for the activity. (If the list have no result for the activity, use the generated trail _Splits_, or make the activity the single selection before switching to many activities.)
  * Select the generated trail _Splits_ for the splits of each included activity. There are no Trail points for this trail, all activities match. The split points for the reference result are marked in the graph.

## Differences ##
Time and distance differences are related to a reference result, in _italics_ in the result list. The plugin selects a default reference result (normally keeping the previous result), but you can set a _Reference Result_ by right clicking in the result list. Select Time or Distance diff charts in the banner menu.

### Reference Diff ###
The chart for the reference result itself would just be a straight line. Instead, this chart shows the difference to the average, how much before/after a constant speed effort the reference result actually was.
Other results can also be compared to itself instead of to the reference with the [Result List keyboard shortcut 'e'](Features#Result_List.md).

Instead of the average pace, the difference to itself can be compared to the [Grade Adjusted Pace](Features#Grade_Adjusted_Pace.md), to get a diff compared to running "virtually flat".

It is also possible to compare to "split" time, for instance if you want to run the first part of a race faster than the second part. The GUI is a little crude:
  * Press Alt-'q' to get a popup with a textbox.
  * Enter distance (in current unit) and time offset (in seconds) in pairs separated by semicolon. So a setup for a 30 km terrain race where the start is crowded and and you want a negative split could use for instance "4;30;20;-120". This means the first 4 km should go 0.5 min slower than ideal pace, time at 20 km be 2 min faster than average (so km 4-20 actually gains 2.5 min), and km 20-30 may loose 2 min to the average.
  * The split is active until ST is restarted or until the "offset string" is cleared.

### Resync Diff at Trail Points ###
The plugin by default compares the difference after a specific time or distance. In other words, it compares the time/distance after a specific distance/time. If there are detours or poor GPS accuracy, the diff may not be calculated for the same sections any more, so the diff is "out-of-sync". For instance, if the reference result for a uphill starts (with a trail point) after 6km and downhill starts at 7km, but for another result, the uphill start at 8km due to a detour, the diff chart by default shows the difference by distance i.e. 7km to 7km.

The _Resync Diff at Trail Points_ option compares the same sections. In the previous example, the uphill is compared to the uphill, i.e. the 6km mark is compared to the 8km mark, so you can see how you fared, hill by hill. The total diff may be incorrect though.

Enable the option in the graph banner menu. The option only affects diff charts, no other charts.

If you 'resync' the difference at trailpoints, the same section is compared in the diff chart, resync match at a each trail point. Detours and distance calculation accuracy are then reset in each trail point. This makes mostly sense for _Time over Distance_ differences. Another example than detours is if you have poor accuracy in one activity, the diff graph will display a "time ahead" for the poor accuracy result even if the results are similar in the trail points. With this option and sufficient trail points, the diff chart can be reasonable with minor "jumps" at the trailpoints.

For _Distance over Time_ differences, the chart normally looks like a "sawtooth", as it is adjusted at each point. The chart is mostly usable in the trailpoints only, it shows a better representation of the difference at each point than the difference lost per time. (Resync for _Distance over Time_ differences is useful when one activity is recorded with several devices and the results are compared.)

Compare this feature to [Trailpoint Synchronization](Features#Trailpoint_Synchronization.md), where the presented charts are synced (chart display changed), but not how the difference is calculated.

![http://wiki.trails.googlecode.com/hg/images/trails_singletrailpoint.jpg](http://wiki.trails.googlecode.com/hg/images/trails_singletrailpoint.jpg)

Note: If _Mark other subtrails or splits in list_ is selected, marking of tracks by clicking in the list is quirky, you may have to click two times or increase the list size.

### Device ###
Some devices have a separate distance calculation. Instead of cumulating the distance from point to point, the device also use other factors like GPS accuracy to determine the distance. For instance Garmin devices can assume it has done mistakes and decrease distances when standing still. There has been discussions in the ST forums about the possibility to estimate the distance track in other ways than just point to point.

Some devices like Polar and GlobalSat do not have separate distance tracks but speed tracks. The speed tracks can be used to estimate a distance track, but the result may differ from the device readings.

Note that the separate distance track is not updated if you modify the activity (moving points etc).

The plugin can show the information as a SpeedPace track too.
If the device has a separate Elevation track (normally barometric) imported to ST, the plugin can display that chart too. This can be used if the device has a partial elevation information, like GPS info for a part of the activity.

The distance track has to be imported separately. A short tutorial: [GPS point to device distance difference](Tutorial#GPS_point_to_device_distance_difference.md)

SportTracks normally handle elevation with GPS latitude/longitude, but it is possible to store a completely separate elevation track too, intended for activities without GPS information, for instance barometric devices. Devices with barometric elevation can record elevation without GPS information also for a part of a track and may record both tracks. This elevation track is normally not visible in SportTracks, ST prefers the GPS information. An usage example: [Separate Elevation Tracks](Tutorials#Separate_Elevation_Tracks.md).

The device information can also be used if the core activity did not have GPS information and a separate GPS track (with incorrect time) was imported, to show the SpeedPace and Elevation from the device and the GPS route on the map.

The "device" tracks are slightly purple, to distinguish them from "normal" tracks.

### Elevation Points ###
Trail Points do not have elevation by default, matching is only done using latitude/longitude. If elevation is set for a trail point, the elevation track can be adjusted from the elevation points. This can be used to adjust elevation from barometric devices.

Barometric devices give a much better representation of the elevation than GPS elevation. However, the barometric elevation must be calibrated when starting and the elevation may drift if the pressure changes over time.

The plugin can adjust the elevation on elevation tracks using trail points that has elevation set. This is meaningful for barometric elevation. The function is enabled for activities with a separate elevation track ([Device Elevation](Features#Device.md)) and for devices where the device name matches the a configured name in the plugin settings. See [Barometric Elevation Adjustment](Tutorials#Barometric_Elevation_Adjustment.md) for more information.

All trail points with elevation set can be viewed with the _Elevation_ trail.

### Chart Alignment ###
Graphs can be aligned to the reference graphs, to for instance align elevation from one activity to another. The settings cycle through the sequence: None (Default), Start, End, Average, Min, Max. The setting (other than None) is displayed as a tooltip when refreshing.
The alignment is done for the left chart axis.
The setting is toggled by graph keyboard shortcut Ctrl-'a'. If the setting is other than 'None', the current setting and offset is shown as a tooltip when the graph is refreshed.
See also [Keyboard shortcuts](Features#Keyboard_shortcuts.md) for more shortcuts.

## Grade Adjusted Pace ##
The plugin can estimate the time and pace as it would be if running on flat ground.
The estimation accuracy differs from person to person and may at least give an indication of the pace on flat.

The plugin implements several algorithms:
  * [MervynDavies](http://runningtimes.com/Article.aspx?ArticleID=10507) Every 1% of upgrade slows your pace 3.3% Every 1% of downgrade speeds your pace by 1.8%. Note that the formula do not work well for steep up (>15%) and down (<-10%). Downhill steeper than 8% uses Kay formula instead, as the MervynDavies formula assumes that speed constantly increases downhill, which do not work well with live data. The up/down factors can be tuned on the Trails settings page.
  * GregMaclin Based on MervynDavies, same link. More effect for hills late in an activity.
  * MervynDaviesSpeed Adjusting the speed instead of the pace. This gives a little more aggressive adjustment uphill, a little less downhill. Steeper than 15% uphill and 9.5% downhill uses Kay, similar as for adjusting pace.
  * [Kay](http://www.lboro.ac.uk/microsites/maths/research/preprints/papers11/11-38.pdf) An Analysis of Race Records, compared to flat races. The formula has the fastest horisontal speed at 8% downhill, steeper than 18% is slower than flat. (This should work well on longer slopes, may not be correct for short slopes, elevation must be smoothed.) This formula has quite consistent data for steep/up down.
  * [JackDaniels](http://www.letsrun.com/forum/flat_read.php?thread=197366&page=0#ixzz23qCNy3uo) (jtupper) 1 % up hill slows you about 15 sec per mile and 1% down gives you about 8 seconds per mile benefit. The up/down factors can be tuned on the Trails settings page.
  * [AlbertoMinetti](http://jap.physiology.org/content/93/3/1039.full) Energy based
  * [ACSM](http://www.edulife.com.br/dados%5CArtigos%5CEducacao%20Fisica%5CFisiologia%20do%20Exercicio%5CEnergy%20expenditure%20of%20walking%20and%20running%20comparison%20with%20prrediction%20equations.pdf) Energy based on VO2Max
  * [Pandolf](http://ftp.rta.nato.int/public/PubFullText/RTO/TR/RTO-TR-HFM-080/TR-HFM-080-03.pdf) A study on soldiers marching and running. Includes the weight and the carried equipment in the calculations. The terrain is also included. It is possible to change the Pandolf terrain factor with [Result List keyboard shortcut Ctrl-Alt-'q'](Features#Result_List.md). Set distance and factor pairs (separate both values and pairs with semicolon) to have a variable terrain factor. For instance"2;1.2;4;1" has factor 1 up to 2km, then factor 1.2 to 4km, then 1 for the remaining part of the result. The terrain factors are defined as follows: 1.0 = black topping road; 1.1 = dirt road; 1.2 = light brush; 1.5 = heavy brush; 1.8 = swampy bog; 2.1 = loose sand; 2.5 = soft snow 15 cm; 3.3 = soft snow 25 cm; 4.1 = soft snow 35 cm. The terrain factor is saved until ST restarts.

Some algorithms work better than other. The energy based algorithms often seem to underestimate the total time lost on a hilly course. Obviously, the algorithms work best for running. They are not expected to be usable for other activities.

To get a good estimation, elevation data is important. Barometric devices gives better information than elevation correction, that is better than GPS elevation. It is also important have proper elevation [Smoothing](Features#Smoothing.md).

Activate the functionality in the result list context menu (right click the list, Advanced, Grade option) or by [Keyboard Shortcuts](Features#Keyboard_Shortcuts.md). When changing the algorithm, the name is shown as a tooltip.

If the feature is active, it shows the following:
  * Result list columns for estimations for "time if flat" and "pace if flat". The columns are only visible if the functionality is activated.
  * The pace graph shows the "grade adjusted pace". Tips: you can often see the original pace for many devices by selecting the device pace. See [Device](Features#Device.md).
  * [Difference](Features#Difference.md) for the reference result is to the average "flat" pace. So if you run with the same effort, elevation data is good and the algorithm works perfectly you would see a flat line.

If you [export a trail to an activity](Features#Trail_Import_Export.md) with the functionality activated, the time on the GPS points is from the grade adjusted pace. See also [Grade Adjusted Pace Course](Tutorials#Grade_Adjusted_Pace_Course.md).

# Other #

## Activity Import ##
At activity import, the plugin will assign the best matching Trail name to the activity.

## Trail Import/Export ##
There is no direct way to import/export trails. If you want to use the same trails on several computers, you probably want to use the same settings on all computers. See the [SportTracks FAQ](http://www.zonefivesoftware.com/sporttracks/forums/viewtopic.php?p=35922#p35922) for information about the Preferences.System.xml where settings are stored.

There are some way to share Trails though. Trails can be exported to activities from the Edit Trail dialog. See the Export button in the lower left corner. The export creates an activity in My Activities that can be exported with normal ST export actions.

Normally, the activity information uses GPS points with time from the current reference activity. However, the time is adjusted if [Grade Adjusted Pace](Features#Grade_Adjusted_Pace.md) or [Reference Diff](Features#Reference_Diff.md) is activated.

Similarly, any activity with GPS can be used to create a trail. See [Create from Splits](Tutorials#Create_from_Splits.md).

# Notes #

## Performance ##
Handling many activities and many trail points can slow down ST. Some tips:
  * Avoid using Google maps, that are rendered using Internet Explorer embedded in ST. You may see one or more of the following symptoms:
    * It takes several seconds to draw tracks, the tracks "flashes" while drawing.
    * There may be popups "Script on this page is not responding".
    * Google Terrain maps background gets black.

The alternatives depends on your options of course, but consider using map None for many activities (this often makes it easier to distinguish between activity tracks too). For me it takes a couple of seconds to draw about 1000 activities with None and similar map-providers, but tens of minutes using Google Maps.

If you have to use Google Maps:
  * Make sure you have installed Internet Explorer 9 (or later) with sufficient updates, IE9 is considerably faster than IE7/IE8.
  * If handling gets slow, flip map-provider back and forward to clear ST internal caches.
  * Quickly stepping though the trail points in Edit Trail stresses the map handling, you may get out of memory (at least on 32-bit OS). Do not do that.

These problems are not really in the Trails plugin, it just that the plugin may stress the ST handling and ST is limited by terms of use for Google Maps. The change in the plugin would be to limit the number of tracks displayed, which depends on the data, hardware and user preferences. The plugin currently enforces any limit, do not use larger activity sets than you can handle.

Normally, displaying a chart takes much longer time than displaying a track. One reason to why results must be selected to be viewed in the chart.

## Ascent/Descent ##
The Ascent/Descent handling uses similar strategy to calculate Ascent/Descent as ST core, but the ST methods has some limitations when splitting up activities. Therefore, the Ascent/Descent is not identical even for Splits trail.

# Advanced #

The plugin can be used to limit the activities used for the calculations or to add activities using other plugins.

## Add InBound activities as results ##
Adds all activities in bound for the trail as results, similar to the Splits trail. This can be used to find why a trail does not match.
Note that the "Splits" result is added also for activities already with results to enable "lap" finding.

## Exclude result from list ##
Exclude the selected results from the result list. The activities are not modified.

## Limit Activities ##
The plugin can be used to limit activities selected, for instance to make processing in other plugins.
  * Select the trail you want to use.
  * Select the trail results in the result list
  * Right click, select _Limit activities to activities selected in list_
> The activity selection is now limited.

## Limit Activities with UniqueRoutes ##
Limits the used activities to activities matching the reference result GPS track, applying UniqueRoutes. This can be used to limit the result from Trails further, for instance to avoid results with detours.

## Add similar with UniqueRoutes ##
Add activities with similar GPS points as the current reference result to the list of "used" activities. This can be used to find similar activities from a single daily activity, instead of going to the reports view, select all, then limit activities. In a way, this "inserts" activities the plugin may use to select activities from.

See the [UniqueRoutes wiki](http://code.google.com/p/gps-running/wiki/UniqueRoutes#Selected_Route_Snippet) for details.

## Mark UniqueRoutes Common Stretches ##
For the selected results, mark the parts common with the reference result using  UniqueRoutes Common Stretches in both the route and the chart. The reference result itself is not marked. Note that the activities will not have to inserted with UniqueRoutes for this feature to work, this only shows the parts in common. Note that the marked regions in the chart can be difficult to see in the SportTracks charts if several results are displayed simultaneously. (With one result, the first chart is "filled" so this is easier to see.)

See [UniqueRoutes wiki](http://code.google.com/p/gps-running/wiki/UniqueRoutes#Map_related) for details.

## UniqueRoutes Trail ##
If [Unique Routes plugin](http://code.google.com/p/gps-running/) is installed, Trails can use the reference activity to get activities as results, similar to Unique Routes. The complete activities are added as trail results, just not the matching part, just as Unique Routes. The same settings as in Unique Routes itself are used. Note that this applies to selection too, if more than one activity is selected (for instance filtered in Reports View), UR uses the activity selection regardless of the Unique Routes category in settings.

The _Edit_ button opens the UniqueRoutes settings, to modify the selection criteria.

Compare to feature [Add similar with UniqueRoutes](Features#Add_similar_with_UniqueRoutes.md) that adds activities to use with the current trail. The UniqueRoutes trail is the same as selecting a single activity, select the Splits trail, [Add similar with UniqueRoutes](Features#Add_similar_with_UniqueRoutes.md).

It is also possible to [Filter Results with UniqueRoutes](Features#Filter_with_UniqueRoutes.md), using [UR Route Snippets](https://code.google.com/p/gps-running/wiki/UniqueRoutes#Selected_Route_Snippet).

## HighScore Trail ##
If [HighScore plugin](http://code.google.com/p/gps-running/) is installed, the plugin can use HighScore to calculate trail results from the used activities, using the settings in HighScore. The default setting is "Minimal Time per specified Distance", but many other settings are possible. See [HighScore wiki](http://code.google.com/p/gps-running/wiki/HighScore) for more information about HighScore.

The _Edit_ button opens the HighScore settings, to modify the selection criteria.

Trails can show HighScore for up to 10 activities for each goal. The "primary HighScore goal" is listed as primary result. The secondary results are added as subtrails (with information how they rank from 2 and up) in the Order column. (The primary result has order from position in the list.)

## Analyze in HighScore ##
Select Trail results to analyze them in HighScore plugin. Use the context menu in the list.
See [HighScore wiki](http://code.google.com/p/gps-running/wiki/HighScore) for more information about HighScore.

The difference to [HighScore Trail](Features#HighScore_Trail.md) is that only the result parts are included, not the complete activities used to find trails from. So using the Splits Trail, then Analyze in HighScore should give the same result as the HighScore trail. Analyze in HighScore is also a way to change the HighScore settings.

## Analyze in PerformancePredictor ##
Select Trail results to analyze them in PerformancePredictor plugin. Use the context menu in the list.
PerformancePredictor can also be used to get training advice from performances as well as to extrapolate results. See [PerformancePredictor wiki](http://code.google.com/p/gps-running/wiki/PerformancePredictor) for more information about PerformancePredictor.

It is also possible to predict the distance for one distance based on the trail result in the result list. The distance to predict for (default 10km) can be changed in the settings.

Similar, Trails can show the ideal time for a distance, similar to Ideal Time in Performance Predictor. However, Trails also includes the estimated impact from hills.

## ExcludeStoppedCategory ##
In ST, "include time below stooped speed" is global for all activities, not per category. This makes it difficult to exclude stopped time for cycling, but not for running, as GPS is unreliable at lower speeds. (The stopped speed could be set to 0 for running, but then you will not have a notion of stopped time at all.)

To have this dynamically per category, set the activity name (or part of the category name path) in Settings. The list is a semicolon separated list with Category names (or parent category names) that should not include stopped time. Example: "Cycling;Cykling"

## Hidden Options ##
Some options can only be set in Preferences.System.xml directly. For the experienced user.
  * MaxAutoCalcActivitiesTrails="500"
> > To avoid longer wait times if many trails and activities exist. The value is Trails times activities, if the value is lower, the results are calculated automatically. If the value is exceeded, only inbound information is calculated.
  * MaxAutoCalcActivitiesSingleTrail="200"
> > Similar to above. If the activity set is changed, for instance from one activity to many, this value is the max number of activities where a result is calculated automatically for the currently selected trail.

# Other plugins #
Trails can be used from other plugins.

## Calculated Fields ##
Trails can be used to calculate data in Calculated Fields. See the [Calculated Fields wiki](http://code.google.com/p/calculatedfields/wiki/Tutorial4).

## Filtered Statistics ##
Trails can be applied as a filter to view parts of activities. See [Filtered Statistics wiki](http://code.google.com/p/st-filteredstatistics/wiki/Welcome?tm=6) for more information. You can use Filtered Statistics to apply other "filters" to view parts of an activity, as well as displaying other types of charts.