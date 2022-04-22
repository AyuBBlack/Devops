#!/bin/bash
while true; do
	date >> /var/log/pinger/pinger.log &
	sleep 1
done
