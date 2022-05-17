Для начала я скачал образу графаны на вм как подсказывает [документация](https://grafana.com/docs/grafana/latest/installation/docker/) мне нужен образа `grafana/grafana-oss`

Запускаю контейнер из образа графаны используя следующие флаги: 

* --rm для удаление контейнера после остановки
* -d для запуска на фоне
* --name задаю имя контейнера 
* -p публикую порты 
* -network=host задаю хостовую сеть 

`docker run --rm -d --name=grafana -p 3000:3000 --network=host grafana/grafana-oss`

Далее нужно было зайти в по адресу машины на 3000 порт и настроить графану для сборки метрики из прометея.

Благо делается это не сложно в разделе в **configuration->data source**

![](https://i.imgur.com/cUWK2ML.png)

> Далее импортурю дашборд [node_exporterFull](https://grafana.com/grafana/dashboards/1860)

![](https://i.imgur.com/wgkMLwb.png)

> Под номером 1860

![](https://i.imgur.com/rCYlLSS.png)

> Собственно получаем вот такой дашборд, где собраны все метрики, что собирает наш node exporter в prometheus. Тут же я создал папку в разделе folder с именем bla, чтобы в дальнейшем добавить туда свой алерт.


![](https://i.imgur.com/q96U3jP.png)

> Далее интегрируем нашего телеграмм бота. На фоне я это уже сделал.

![](https://i.imgur.com/Nqz6VqO.png)


> После чего добавил сверху метрику, которую отслеживаю, и добавил Conditions на то, что если система использует памяти больше чем 70%, то срабатывает предупреждение.

![](https://i.imgur.com/LijE8Hx.png)
![](https://i.imgur.com/houdMfH.png)


> Вот можно увидеть алерт в папке bla

![](https://i.imgur.com/Wrwn55q.png)

> Ну дальше остается связать мой tel c алертом в  Notification policies

![](https://i.imgur.com/wKvFo7A.png)


 Ну для проверки я __изменил значения__ для оповещения __на более низкие__.


> Собственно вот ответы от бота:

![](https://i.imgur.com/pIGeFI4.png)