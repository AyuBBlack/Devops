using Microsoft.Extensions.Options;
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
        public DbLoggerProvider(IOptionsMonitor<DbLoggerConfiguration> config) => _config = config.CurrentValue;

        //При запросе логера ему передается имя категории
        public ILogger CreateLogger(string categoryName) => 
            _loggers.GetOrAdd(categoryName, name => new DbLogger(name, _config)); 
        public void Dispose() => _loggers.Clear();
    }
}
