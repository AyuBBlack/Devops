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
            throw new NotImplementedException();
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
namespace SqlBundle.Logging
{
    public static class DbLoggerExtensions
    {
        public static ILoggingBuilder AddDbLogger(this ILoggingBuilder builder)
        {
            return builder;
        }
    }
}
```
Далее в programm добавляем наш билдер.

```builder.Logging.AddDbLogger();```

