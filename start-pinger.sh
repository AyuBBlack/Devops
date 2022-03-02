#!/bin/bash
if [ -f $/run/pinger/pinger.pid ] 
then 
   /usr/local/bin/stop-pinger.sh
   /usr/local/bin/pinger.sh >> /var/log/pinger/pinger.log &
   echo $! > /run/pinger/pinger.pid
else
   /usr/local/bin/pinger.sh >> /var/log/pinger/pinger.log &
   echo $! > /run/pinger/pinger.pid
fi
