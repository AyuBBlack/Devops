#!/bin/bash
USE=`df / | tail -1 | awk '{print $5}' | sed 's/%//'`
if [ $USE -gt 60 ]; then   
    truncate -s 0 /var/log/syslog && 
    rm -f /var/log/syslog.* && 
    sudo apt-get clean
else
    echo "Рутовая партиция не заняла более 60% места"
fi

USED_NOW=`df / | tail -1 | awk '{print $5}' | sed 's/%//'`
FREE_NOW=$((100-$USED_NOW))

echo "Сейчас свободно $FREE_NOW% места на рутовой партишен"  > /var/log/clean_sh.log
