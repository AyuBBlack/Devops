##### Были задача: К приложению из предыдущего ДЗ по вариантам подключить БД MS-SQL Express, таблицу хранящую историю запросов и результатов работы приложения (дату, входные параметры, результат), добавить CRUD операции для данной сущности.:  

Перед началом работы я скачал PostgreSQL 14 и pgAdmin 4 для работы с Базой данных.

>Для начала я добавил Nuget пакеты в мой web api.

Это Microsoft.EntityFrameworkCore и Npgsql.EntityFrameworkCore.PostgreSQ

Далее я создал папку models и создал в нем модуль для формирования таблицы.

Реализовал я его следующим кодом:

```
namespace SqlBundle.Models
{
    public class History
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Parametrs { get; set; }
        public string Results { get; set; }
    }

}

```

После чего начал писать следующий модуль но на этот раз ContextDB

```
using Microsoft.EntityFrameworkCore;
namespace SqlBundle.Models
{
    public class SqlContext : DbContext
    {
        public SqlContext(DbContextOptions<SqlContext> options)
    : base(options)
        {
        }
        //Cвязываем таблицу tables.cs с таблицей которую создастся в базе 
        public DbSet<Tables> Tables { get; set; } = null!;
    }
}

```

Далее нужно дописать связку в appsettings.

```
  "ConnectionStrings": {
    "ConnectPostgre": "Host=localhost;Port=5432;Database=Bundle;Username=postgres;Password=1234"
  },

```
После чего добавляем контекст в наш в programm, чтобы тот знал куда ему ссылаться.
```
builder.Services.AddDbContext<SqlContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("ConnectPostgre")));
```
Следующим шагом я сделал новый класс BundleRepo, к которому будет ссылаться контроллер, чтобы выполнить основные команды в базе данных.

```
namespace SqlBundle.Models
{
    public class BundleRepo
    {
        private SqlContext Context;

        public void Create(History tables) //Создание таблицы
        {
            Context.Tables.Add(tables);
            Context.SaveChanges();
        }

        public BundleRepo(SqlContext context)
        {
            Context = context;
        }
        public IEnumerable<History> Get() //Получение таблиц
        {
            return Context.Tables;
        }
        public History Get(int Id) 
        {
            return Context.Tables.Find(Id); //Поиск id в таблице
        }
        public void Update(History updatetables) //Обновление тело таблицы
        {
            History curretTables   = Get(updatetables.Id);
            curretTables.Date      = updatetables.Date;
            curretTables.Parametrs = updatetables.Parametrs;
            curretTables.Results   = updatetables.Results;

            Context.Tables.Update(curretTables);
            Context.SaveChanges();
        }
        public string Delete(int Id) //Удаление таблицы
        {
            History history = Context.Tables.Find(Id);
            if (history != null)
            {
                Context.Remove(history);
                Context.SaveChanges();
                return "Ok";
            }
            else
                return "Error";
        }
    }
}

```


Далее реализуется контроллер
```
using Microsoft.AspNetCore.Mvc;
using SqlBundle.Models;

namespace SqlBundle.Controllers

{
    [Route("[controller]")]
    public class BundleRun : Controller
    {
        BundleRepo bundleRep;
        public BundleRun(BundleRepo bundleRepos) => bundleRep = bundleRepos;

        [HttpGet("/GetAllItems")] //Получаем все элементы 

        public IEnumerable<History> Get()
        {
            return bundleRep.Get();
        }
        private static string Reverse(string s) //Метод для реверса строки
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        [HttpGet("/Create")] //Создание записи в бд
        public string Get(string param)
        {
            History tables = new History();
            tables.Date = DateTime.Now.ToString("dd/MM/yy");
            tables.Parametrs = param;
            string reverse = Reverse(param);
            tables.Results = reverse;
            bundleRep.Create(tables);
            return reverse;
        }

        [HttpPost("/Update")] //Обновление записи 
        public void Update([FromBody] History updateTables)
        {
            updateTables.Results = Reverse(updateTables.Parametrs);
            bundleRep.Update(updateTables);
        }
        [HttpDelete("/Delete")] // Удаление записи 
        public string Delete(int Id)
        {
            return bundleRep.Delete(Id);
        }      
    }
}

```



