Для начала я установил сам промидиус (*который в дальнейшем возможно буду называть "прометей"*) из [документации](https://prometheus.io/docs/introduction/first_steps/#using-the-graphing-interface)

Скачал 

`wget https://github.com/prometheus/prometheus/releases/download/v2.35.0/prometheus-2.35.0.linux-amd64.tar.gz`

Распаковал, посмотрел конфиг файл prometheus.yml

```
  - job_name: "prometheus"

    # metrics_path defaults to '/metrics'
    # scheme defaults to 'http'.
    static_configs:
      - targets: ["127.0.0.1:9090"]
```
Вижу, что он собирает свои же метрики, но всё равно сюда же поставим и node_exporter.

---


Скачал нод экспортер

`wget https://github.com/prometheus/node_exporter/releases/download/v1.3.1/node_exporter-1.3.1.linux-amd64.tar.gz`

Далее развернул его на машине и запустил

`tar xvfz node_exporter-*.*-amd64.tar.gz`
`cd node_exporter-*.*-amd64`
`./node_exporter`

После чего посмотрел на одной из машине выдает ли он метрики по адресу машины :9100/metrics

И да, [метрики](http://178.154.204.217:9100/metrics) тут как тут:

![](https://i.imgur.com/7j4wl61.png)


После чего надо было править файл конфига для прометея: 

```
 - job_name: "node_exporter"
    static_configs:
      - targets:
          - "178.154.204.217:9100"
```

Захожу на свой прометей и смотрю есть там метрики 

`node_filesystem_avail_bytes /1024/1024` Просмотр загруженности диска.


![](https://i.imgur.com/zKWGoA3.png)

Как видно на скриншоте, они есть. 

```
df -h
Filesystem      Size  Used Avail Use% Mounted on
udev            1.9G     0  1.9G   0% /dev
tmpfs           394M  5.8M  388M   2% /run
/dev/vda2        15G  5.4G  8.8G  38% /
tmpfs           2.0G     0  2.0G   0% /dev/shm
tmpfs           5.0M     0  5.0M   0% /run/lock
tmpfs           2.0G     0  2.0G   0% /sys/fs/cgroup
tmpfs           394M     0  394M   0% /run/user/1000
```
И что самое главное они совпадают. 

Тот же процесс проделал и для другой [машины](http://51.250.69.110:9100/metrics)

