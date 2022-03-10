Для начала мне нужно было вывести сколько процентов заняла корневая директория / 

Сделал я это следующей командой:

`USE=df / | tail -1 | awk '{print $5}' | sed 's/%//'`

Вывод записал в переменную **USE**

После чего написал условие **if**

```
if [ $USED_NOW -gt 40 ]; then
    truncate -s 0 /var/log/syslog &&
    rm -f /var/log/syslog.* &&
    sudo apt-get clean
else
    echo "Рутовая партиция не заняла более 60% места"
fi
```

Либо альтернативный вариант:

```
USED_NOW=`df / | tail -1 | awk '{print $5}' | sed 's/%//'`
FREE_NOW=$((100-$USED_NOW))
if [ $FREE_NOW -lt 60 ]; then   
    truncate -s 0 /var/log/syslog && 
    rm -f /var/log/syslog.* && 
    sudo apt-get clean
else
    echo "Рутовая партиция не заняла более 60% места"
fi
```

Далее нужно было записать сколько свободного пространства осталось. Учитывая, что мы располагаем 100% пространства, я записал снова в переменную **USED_NOW** сколько мы используем и отнял его от 100 и записал всё это в переменную **FREE_NOW**

```
USED_NOW=`df / | tail -1 | awk '{print $5}' | sed 's/%//'`
FREE_NOW=$((100-$USED_NOW))
```

Далее сделал файл для **logrotate** 

```
/var/log/clean_sh.log {
daily
rotate 4
size 5M
compress
delaycompress
}
```

После чего записал **`crontab -e`** 

`/var/spool/cron/crontabs/`

Но потом спросив ребят записал его отдельно в `/etc/cron.d/clean_sh_cro`n как вроде бы надо было дз.

Далее запаковал всё в архив.

`tar -p -cvf clean.tar.gz /etc/cron.d/clean_sh_cron /etc/logrotate.d/clean /usr/local/bin/clean.sh /var/log/clean_sh.log`
