#!/bin/bash
if [ -f $/run/pinger/pinger.pid ] 
then 
    exec ./stop-pinger.sh
    exec ./pinger.sh 2>> /var/log/pinger/pinger.log &
    echo $! > /run/pinger/pinger.pid
else
    exec ./pinger.sh 2>> /var/log/pinger/pinger.log &
    echo $! > /run/pinger/pinger.pid
fi
