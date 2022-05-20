### Предисловие.

В начале я захотел всё запихнуть в докер композ. Оказалась эта не самая простая задача, если не вникать как запускается web api в докер композ.

Почитав [документацию](https://docs.microsoft.com/ru-ru/dotnet/architecture/containerized-lifecycle/design-develop-containerized-apps/build-aspnet-core-applications-linux-containers-aks-kubernetes) я понял, что нужно добавить environment 


`ASPNETCORE_URLS=http://+:80`

`ASPNETCORE_ENVIRONMENT: "Development"`


```
  bundleapi:
    image: bundleapi
    container_name: bundleapi
    ports: 
      - "8080:80"
      - "60443:443"
    environment:
      DBserver: "postgresql"
      DBport: "5432"
      DBname: "DB"
      DBuser: "postgres"
      DBPassword: "123"
      ASPNETCORE_ENVIRONMENT: "Development"
      ASPNETCORE_URLS: "http://+:80"
    build:
      context: .
      dockerfile: Dockerfile
    networks:
      - mynetwork
    restart: unless-stopped
    depends_on:
      - postgresql
```


> И теперь я решил прикрутить node_exporter с помощью контейнера к тому, что есть из предыдущей дз 



```
  prometheus:
    image: prom/prometheus
    container_name: prometheus
    restart: unless-stopped
    ports:
      - 9090:9090
    volumes:
      - ./prometheus.yml:/etc/prometheus/prometheus.yml:ro

      # - ./data:/prometheus
    command:
      - '--config.file=/etc/prometheus/prometheus.yml'
      - '--storage.tsdb.path=/prometheus'
      - '--web.console.libraries=/etc/prometheus/console_libraries'
      - '--web.console.templates=/etc/prometheus/consoles'
      - '--web.enable-lifecycle'
    networks:
      - mynetwork

  node-exporter:
    image: prom/node-exporter:latest
    container_name: node-exporter
    restart: unless-stopped
    volumes:
      - /proc:/host/proc:ro
      - /sys:/host/sys:ro
      - /:/rootfs:ro
    command:
      - '--path.procfs=/host/proc'
      - '--path.rootfs=/rootfs'
      - '--path.sysfs=/host/sys'
      - '--collector.filesystem.mount-points-exclude=^/(sys|proc|dev|host|etc)($$|/)'
    ports:
      - 9100:9100
    networks: 
      - mynetwork
```

Также прикручиваю сюда же графану:

```
  grafana: 
    image: grafana/grafana
    container_name: grafana
    environment:
      - GF_SECURITY_ADMIN_PASSWORD=P@ssw0rd
    restart: unless-stopped
    ports:
      - 3000:3000
    networks:
      - mynetwork
```

> Из всего этого получается следующее:
> 
> Я могу посмотреть все метрики с моей машины, загруженность диска, цп и так далее. 

![](https://i.imgur.com/kgJsOTB.png)

> Также я могу получать метрики http запросов своего api приложения что мы ранее разрабатывали. 

![](https://i.imgur.com/P8Okq03.png)

> Да и много еще других метрик API

![](https://i.imgur.com/Jqtv0tL.png)

**На этом с предисловием наверно всё, теперь касательно домашки.**


---

> Что нужно сделать:
> 1. На одной из ВМ поднять (любым способом из документации) инстанс Loki
> 2. На каждой из машин поднять поднять (любым способом из документации) Promtail, который будет слать данные в Loki
> 3. На каждой из машин запустить ваше приложение (любым способом)
> 4. Ваше приложение должно писать логи (любым способом).
> 5. Настроить Promtail так, чтоб он подхватывал логи вашего приложения и пробрасывал в централизованное хранилище Loki.
> При этом, важно каким-то образом (например, через label-ы придумать механизм однозначной идентификации источника по логу)
> 6. В Grafana (которую вы поднимали в предыдущей домашке) настроить Loki как источник данных и убедиться, что выборка логов происходит успешно
---

Для начала качаю 2 файла конфига для протаила и локи

`wget https://raw.githubusercontent.com/grafana/loki/v2.5.0/cmd/loki/loki-local-config.yaml -O loki-config.yaml`

`wget https://raw.githubusercontent.com/grafana/loki/v2.5.0/clients/cmd/promtail/promtail-docker-config.yaml -O promtail-config.yaml`

Дальше я добавляю к своему докер-композ файлу еще 2 сервиса - локи и промтайл 

```
  loki:
    image: grafana/loki:2.5.0
    container_name: loki
    ports:
      - "3100:3100"
    command: -config.file=/etc/loki/local-config.yaml
    networks:
      - mynetwork

  promtail:
    image: grafana/promtail:2.5.0
    container_name: promtail
    volumes:
      - /var/log:/var/log
      - /var/lib/docker/containers:/var/lib/docker/containers
      - ./promtail-config.yml:/etc/promtail/promtail-config.yml
    command: -config.file=/etc/promtail/promtail-config.yml
    networks:
      - mynetwork
```

> Для промтеила конфиг файл пришлось отредактировать, чтобы он брал логи не только моей машины, но и брал логи со всех контейнеров. В итоге конфиг файл для промтейла получился вот такой:

```
server:
  http_listen_port: 9080
  grpc_listen_port: 0

positions:
  filename: /tmp/positions.yaml

clients:
  - url: http://loki:3100/loki/api/v1/push

scrape_configs:
- job_name: system
  static_configs:
  - targets:
      - localhost
    labels:
      job: varlogs
      __path__: /var/log/*log

- job_name: containers
  static_configs:
  - targets:
      - localhost
    labels:
      job: containerlogs
      __path__: /var/lib/docker/containers/*/*log

  # --log-opt tag="{{.ImageName}}|{{.Name}}|{{.ImageFullID}}|{{.FullID}}"
  pipeline_stages:

  - json:
      expressions:
        stream: stream
        attrs: attrs
        tag: attrs.tag

  - regex:
      expression: (?P<image_name>(?:[^|]*[^|])).(?P<container_name>(?:[^|]*[^|])).(?P<image_id>(?:[^|]*[^|])).(?P<container_id>(?:[^|]*[^|]))
      source: "tag"

  - labels:
      tag:
      stream:
      image_name:
      container_name:
      image_id:
      container_id:
```

Теперь проверяем всю эту красоту, получилось 7 контейнеров в докер-композ.

![](https://i.imgur.com/c5q30S0.png)

> Красным отметил мое API для получения логов мне пригодится его ID.
> 
> Теперь к графане также прикручиваю Локи

![](https://i.imgur.com/suB0qu2.png)


> Теперь вот я могу посмотреть логи моих контейнеров, но для начала сравню системные логи в локи и с логами на машине

![](https://i.imgur.com/OheVTGB.png)


Логи на машине:

tail -f /var/log/syslog

```
May 20 21:22:41 ubu systemd-networkd[468]: veth6d4d437: Gained IPv6LL
May 20 21:22:41 ubu systemd-networkd[468]: vethcd1534d: Gained IPv6LL
May 20 21:29:35 ubu systemd[1]: Starting Ubuntu Advantage Timer for running repeated jobs...
May 20 21:29:36 ubu systemd[1]: ua-timer.service: Succeeded.
May 20 21:29:36 ubu systemd[1]: Finished Ubuntu Advantage Timer for running repeated jobs.
May 20 21:33:32 ubu gitlab-runner[507]: #033[31;1mERROR: Runner https://gitlab.com/6VKyRk2MAazsCvYZv_F3 is not healthy, but will be checked!#033[0;m
May 20 21:34:32 ubu gitlab-runner[507]: #033[31;1mERROR: Checking for jobs... forbidden             #033[0;m  #033[31;1mrunner#033[0;m=6VKyRk2M
May 20 21:37:14 ubu gitlab-runner[507]: #033[31;1mERROR: Checking for jobs... forbidden             #033[0;m  #033[31;1mrunner#033[0;m=6VKyRk2M
May 20 21:40:45 ubu gitlab-runner[507]: #033[31;1mERROR: Checking for jobs... forbidden             #033[0;m  #033[31;1mrunner#033[0;m=6VKyRk2M
May 20 21:40:45 ubu gitlab-runner[507]: #033[31;1mERROR: Runner https://gitlab.com/6VKyRk2MAazsCvYZv_F3 is not healthy and will be disabled!#033[0;m
```
> А вот логи в локи, тут даже видно, что он успел подхватить даже более свежие логи.

![](https://i.imgur.com/1ziMJZi.png)

> Теперь проверяю логи в моего приложения, которое запущено рядом с остальными в докер композ.


> Вывожу последние 2 лога приложения.

`docker logs 14370b9721a4 -n 2`

Обратите внимание на **"EventId":20101"** позже я к нему буду ссылаться.

```
{"EventId":10403,"LogLevel":"Information","Category":"Microsoft.EntityFrameworkCore.Infrastructure","Message":"Entity Framework Core 6.0.5 initialized \u0027SqlContext\u0027 using provider \u0027Npgsql.EntityFrameworkCore.PostgreSQL:6.0.4\u002B6cb649128e3e7aa8eddd77dfa75b34bad51e6e94\u0027 with options: None","State":{"Message":"Entity Framework Core 6.0.5 initialized \u0027SqlContext\u0027 using provider \u0027Npgsql.EntityFrameworkCore.PostgreSQL:6.0.4\u002B6cb649128e3e7aa8eddd77dfa75b34bad51e6e94\u0027 with options: None","version":"6.0.5","contextType":"SqlContext","provider":"Npgsql.EntityFrameworkCore.PostgreSQL","providerVersion":"6.0.4\u002B6cb649128e3e7aa8eddd77dfa75b34bad51e6e94","options":"None","{OriginalFormat}":"Entity Framework Core {version} initialized \u0027{contextType}\u0027 using provider \u0027{provider}:{providerVersion}\u0027 with options: {options}"}}
{"EventId":20101,"LogLevel":"Information","Category":"Microsoft.EntityFrameworkCore.Database.Command","Message":"Executed DbCommand (3ms) [Parameters=[], CommandType=\u0027Text\u0027, CommandTimeout=\u002730\u0027]\nSELECT t.\u0022Id\u0022, t.\u0022Date\u0022, t.\u0022Parametrs\u0022, t.\u0022Results\u0022\nFROM \u0022Tables\u0022 AS t","State":{"Message":"Executed DbCommand (3ms) [Parameters=[], CommandType=\u0027Text\u0027, CommandTimeout=\u002730\u0027]\nSELECT t.\u0022Id\u0022, t.\u0022Date\u0022, t.\u0022Parametrs\u0022, t.\u0022Results\u0022\nFROM \u0022Tables\u0022 AS t","elapsed":"3","parameters":"","commandType":"Text","commandTimeout":30,"newLine":"\n","commandText":"SELECT t.\u0022Id\u0022, t.\u0022Date\u0022, t.\u0022Parametrs\u0022, t.\u0022Results\u0022\nFROM \u0022Tables\u0022 AS t","{OriginalFormat}":"Executed DbCommand ({elapsed}ms) [Parameters=[{parameters}], CommandType=\u0027{commandType}\u0027, CommandTimeout=\u0027{commandTimeout}\u0027]{newLine}{commandText}"}}
```

Ну теперь возвращаясь к отмечанному мной ранее container id я могу отфильтровать логи по конкретному контейнеру.

И получить те же самый логи, в этом можно убедиться по ивенту id

![](https://i.imgur.com/ow8zY1r.png)

#### На этом пожалуй всё.