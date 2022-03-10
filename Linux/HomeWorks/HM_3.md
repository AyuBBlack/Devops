Для начала я написал три скрипта:

* pinger.sh
```
#!/bin/bash
while true; do
	date >> /var/log/pinger/pinger.log &
	sleep 1
done
```
* start-pinger.sh
```
#!/bin/bash
if [ -f $/run/pinger/pinger.pid ] #Проверяет есть ли файл
then 
    exec ./stop-pinger.sh #Если он есть запускает остановку 
    exec ./pinger.sh 2>> /var/log/pinger/pinger.log &
    echo $! > /run/pinger/pinger.pid
else #Иначе же просто запускает скрипт pinger.sh
    exec ./pinger.sh 2>> /var/log/pinger/pinger.log &
    echo $! > /run/pinger/pinger.pid
fi
```

* stop-pinger.sh
#Удаляет 
`pkill -F /run/pinger/pinger.pid && rm -f /run/pinger/pinger.pid`

> Git [https://github.com/AyuBBlack/Devops]

Далее написал systemd "pinger.service"

```
[Unit]
Description=Pinger service
After=network.target network-online.target remote-fs.target

[Install]
WantedBy=multi-user.target

[Service]
Type=forking
User=pinger
Group=pinger
EnvironmentFile=-/etc/default/pinger
ExecStart=/usr/local/bin/pinger.sh
ExecStop=/usr/local/bin/stop-pinger.sh
LimitNOFILE=65536
```

После чего создал пользователя pinger 

`useradd -mU pinger`

Затем запустил `systemctl daemon-reload` для подрузки изменений в systemd

Далее проверил systemctl start pinger.service

И получил ошибку, что нет прав на запись. Выдал все права на директории и всё служба не запускается нормально после чего стал анализировать свои файлы и понял, что ошибка была в пути в start-pinger.sh.

Было 

    exec ./stop-pinger.sh #Если он есть запускает остановку 
    exec ./pinger.sh 2>> /var/log/pinger/pinger.log &
    echo $! > /run/pinger/pinger.pid
    
Изменил на:

```
   /usr/local/bin/stop-pinger.sh
   /usr/local/bin/pinger.sh >> /var/log/pinger/pinger.log &
   echo $! > /run/pinger/pinger.pid
```

И наконец-то всё заработало:

```
root@cloud-1:/opt/devops2# systemctl start pinger.service
root@cloud-1:/opt/devops2# ps aux | grep pinger
pinger    181851  0.0  0.1   9492  3284 ?        S    14:52   0:00 /bin/bash /usr/local/bin/pinger.sh
pinger    181904  0.0  0.0   8076   576 ?        S    14:53   0:00 sleep 1
root      181906  0.0  0.0   9032   724 pts/0    S+   14:53   0:00 grep --color=auto pinger
root@cloud-1:/opt/devops2# cat ./start-pinger.sh
```

Теперь нужно сделать logrotate

В можно настроить логротейт /etc/logrotate.conf
Однако по умолчанию там уже стоит logrotate.d
Так что мне достаточно добавить туда конфиг, который условно я назову "pinger". 


```
/var/log/pinger/pinger.log {
daily
rotate 4
size 5M
compress
delaycompress
}
```


Теперь весь результат можно запаковать в один архив.

`tar -p -cvf file.tar.gz /usr/local/bin/*.sh /etc/systemd/system/pinger.service /etc/logrotate.d/pinger`
