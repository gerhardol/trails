See the Developer wiki for more information

Upload to Google Code:

#!/bin/bash

p=
s="Updated German translation"

util/googlecode_upload.py -p gps-running -u gerhard.nospam -l Featured -w $p -s "$s" `ls -rt *plugin| tail -1`
