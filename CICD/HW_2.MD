Что нужно сделать:
1. Написать Dockerfile, который позволит собрать образ для запуска вашего проекта.
*Очевидно что в докерфайле будут примерно следующие шаги:
Подходящий базовый образ
Подготовка окружения
Копирование файлов сборки в образ
Старт сервера - главный процесс контейнера
2*. Локально протестируйте, что сборка проходит успешно.
Локально протестируйте, что запуск контейнера приводит к запуску сервера.
Проверьте, что при запущенном контейнере при попытке отправить запрос в контйнеризованный сервер получается увидеть ответ.
3. Сделайте коммит Dockerfile-а в ваш gitlab репозиторий
4. Напишите новую джобу (давайте называть ее: build-docker), которая будет проводить сборку вашего проекта в docker-образ


Так докер файл у меня уже с предыдущего дз по .Net этапы с .Net можно считать у меня выполнеными.

Собственно сам докер файл.

```
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SqlBundle/SqlBundle.csproj", "SqlBundle/"]
RUN dotnet restore "SqlBundle/SqlBundle.csproj"
COPY . .
WORKDIR "/src/SqlBundle"
RUN dotnet build "SqlBundle.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SqlBundle.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SqlBundle.dll"]
```

Далее нужно было сделать стэдж для сборки докер файла.

```
build-docker:   
  stage: build   
  script:
    - docker build -t ayub95/repo . 
  tags:
    - "dev"
  allow_failure: false
```

[Ссылка на пайплайн](https://gitlab.com/AyuBBlack/inno/-/pipelines/528853132)