##### Задача: 

>В приложении из предыдущего ДЗ реализовать кастомный логгер, который будет писать лог в локальный файл, лежащий в директории приложения.  

1. Для начала я создал отдельную папку **Logging** внутри проекта, потом добавил в него пару классов.

Первый класс был **DbLoggerConfiguration** 

```
namespace SqlBundle.Logging
{
    public class DbLoggerConfiguration
    {
        public string DbconnectionString { get; set; }
    }
}
```

в котором описан DbconnectionString, который в свою очередь описан в appsettings 

```
  "Dblogging": {
    "DbconnectionString": ""
  }
```

2. На этом этапе я начал писать класс  **DbLogger**

```
namespace SqlBundle.Logging
{
    public class DbLogger : ILogger
    {
        private readonly string _loggerName;
        private readonly DbLoggerConfiguration _config;

        public DbLogger(string loggerName, DbLoggerConfiguration config)
        {
            _loggerName = loggerName;
            _config = config;
        }
        IDisposable ILogger.BeginScope<TState>(TState state) => default;

        bool ILogger.IsEnabled(LogLevel logLevel) => true;

        void ILogger.Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
           //Console.WriteLine(formatter(state, exception));
            string fileName = Path.Combine(Environment.CurrentDirectory, "output.txt");

            //Создаем пустой файл
            StreamWriter swBegin = new StreamWriter(fileName);
            swBegin.WriteLine("");
            swBegin.Close();

            //Дозаписываем файл
            StreamWriter sw = new StreamWriter(fileName, true);;
            sw.WriteLine(formatter(state, exception));
            sw.Close();

            //Для консоли
/*            try
            {
                StreamWriter sw = new StreamWriter(fileName);
                sw.WriteLine(formatter(state, exception));
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }*/
        }
    }
}


```

3. Далее идет реализация класса **DbLoggerProvider**

```
using System.Collections.Concurrent;
namespace SqlBundle.Logging
{
    [ProviderAlias ("DbLogging")]
    public class DbLoggerProvider : ILoggerProvider
    {
        // Делает запрос есть или нет под таким именем,
        //если он есть возвращает его, если нет то создает
        private readonly ConcurrentDictionary<string, DbLogger> _loggers = new(); 
        private readonly DbLoggerConfiguration _config;
        //Конструктор
        public DbLoggerProvider(DbLoggerConfiguration config) => _config = config;

        //При запросе логера ему передается имя категории
        public ILogger CreateLogger(string categoryName) => 
            _loggers.GetOrAdd(categoryName, name => new DbLogger(name, _config)); 
        public void Dispose() => _loggers.Clear();
    }
}
```

4. После чего нужно реализовать класс **DbLoggerExtensions**, в котором логер регистрируется.
```
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;


namespace SqlBundle.Logging
{
    public static class DbLoggerExtensions
    {
        public static ILoggingBuilder AddDbLogger(this ILoggingBuilder builder, Action<DbLoggerConfiguration> configure)
        {
            //Подтягивает конфиг
            builder.AddConfiguration(); 
            //Добавляем наш провайдер. Дискриптор создается только если он не существует. 
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, DbLoggerProvider>());
            
            //Регистрация конфига
            LoggerProviderOptions.RegisterProviderOptions<DbLoggerConfiguration, DbLoggerProvider>(builder.Services);

            builder.Services.Configure(configure);

            return builder;
        }
    }
}

```
Далее в programm добавляем наш билдер.

```
builder.Logging.ClearProviders()
    .AddDbLogger(configure => { });   
```

Проверил всё, файл в папку с проектом записывается:

```
Executed DbCommand (9ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
SELECT t."Id", t."Date", t."Parametrs", t."Results"
FROM "Tables" AS t
```