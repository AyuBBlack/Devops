##### Была задача: К приложению из предыдущего ДЗ добавить поддержку docker’а, и развернуть в нём. Базу MS-SQL  для приложения также развернут docker’е.:  

Первым делом я скачал Docker Desktop. 

После чего принялся добавлять поддержку Docker(a) в свой проект.

Реализовал я его следующим кодом в Dockerfile:

```
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

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

После чего в файле launchSettings.json я дописал конфигурацию для запуска в докере

```
    "Docker": {
      "commandName": "Docker",
      "launchBrowser": true,
      "launchUrl": "{Scheme}://{ServiceHost}:{ServicePort}/swagger",
      "publishAllPorts": true,
      "useSSL": true
    }

```

Далее нужно изменить коннекшен с локалхоста в appsettings. Поменял я его на свой сервер на Yandex облаке.

```
"ConnectionStrings": {
    "ConnectPostgre": "Host=IPМоегоСервера;Port=5432;Database=Bundle;Username=postgres;Password=root"
  },
```

Далее поднял сервер postgres на виртуальной машине.

```
docker pull postgres
```

```
docker run --name post -p 5432:5432 -e POSTGRES_PASSWORD=root -d postgres
```

На этом этапе я стал проверять работает ли докер имедж приложения с моего локального компа 

```
docker run --name myApp -p 8080:80 -d sqlbundle

```

И да, по localhost:8080 я смог отправить данные в свою базу данных.

Решил, что на этом останавливаться не стоит и пошел пушить свое приложение в свой приватный репозиторий на Docker Hub. 

Предварительно поменяв имя image  
```
docker image tag sqlbundle:latest ayub95/repo:sqlbundle

```

Далее я запушил это дело и уже на виртуальной машине сделал Pull этого имеджа.

И также поднял контейнер приложения, после чего у любого человека был доступ к записи в базу данных своих реверсированных слов если знать, что отправлять)


Теперь, что касатеся docker compose.

Я дописал еще один скрипт модуль для миграции базы. Ибо внутри докер compose базу скрипт описанный выше не мигрирурет и его надо мигрировать вручную.

Для решения этой проблемы был написан модуль PrebDB

```
using Microsoft.EntityFrameworkCore;

namespace SqlBundle.Models
{
    public static class PrebDB
    {
        public static WebApplicationBuilder PopulateDB(this WebApplicationBuilder builder)
        {
            using var serviceProvider = builder.Services.BuildServiceProvider(); //Возращает сервис провайдер который реализует интерфейс

            var context = serviceProvider.GetService<SqlContext>();

            if (context == null)
            {
               throw new Exception("Context не создан");
            }
            context.Database.Migrate();

            return builder;
        }
    }
}
```

Данный модуль позволяет нам мигруровать базу с помошью метода

```
context.Database.Migrate();
```
