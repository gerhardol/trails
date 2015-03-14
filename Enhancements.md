

## Currently implementing ##
  * ever continuing improvements

## Maybe ##
  * Use Trail activity category to store activities used as Trails. This would also be a simple way to get Trail Categories
  * Use "Unique Routes - Common Stretches" when resyncing differences ([Features#Resync\_Diff\_at\_Trail\_Points](Features#Resync_Diff_at_Trail_Points.md)) time/distance differences.

## Less likely ##
  * Summary line: subtrails
  * Stretches of sub trails in the result (to get other "start points")
  * Resize multi graphs
  * Custom algorithms for smoothing (in addition to ST averaging).
  * Thread calculations, speed up handling for multiple cores. Will not make much difference though after other optimizations.
  * Insert results - compare to others. The intention is that it should be easy to paste from result services. Check a few result services, but goteborgsvarvet.com uses "Start (0 km)	00:00:00	00:00	13:33:47" and stockholmmarathon.se "5K 	0.26.18 	26.18 	05.16 	11.41 	4990".
    * Parse textbox
    * Ignore lines where first character is # completely, truncate lines after #
    * Make sure no of non commentlines match trail points (or a multiple, add several at once)
    * Split fields using tab, semicolon, comma, space, trim space (in this order).
    * Regexp for time: (((\d{1,2})[.:]){1,2}(\d{1,2})([,.](\d{1,2})?)). Locale dependent?
    * Guess field number from data.
    * If there is no data for a point, do not set it. This means that if first and last is set, the speed will be constant and if 2 and 3 only is set, the speed is even between 2 and 3 only, no other changes.
    * Optional:
      * Configure name, field to parse? in # comment, using keywords?
    * Some easy way to export results as activities (can be done from Edit Trail now).
    * Also parse DateTime and lap times?

## Not so likely ##
  * Export/Import trails in files. Probably as GPX routes