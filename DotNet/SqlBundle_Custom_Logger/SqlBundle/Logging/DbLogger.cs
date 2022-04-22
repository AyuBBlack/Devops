using Microsoft.Data.SqlClient;
using SqlBundle.Models;

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
        BundleRepo DbLog;
        void ILogger.Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)

        {
/*            using var connection = new SqlConnection(_config.DbconnectionString);

            connection.Open();
            ;
            var command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.Text;*/

            HistoryLogger LogHis = new HistoryLogger();
            LogHis.Date = DateTime.Now.ToString("dd/MM/yy");
            LogHis.StateAndExeption = formatter(state, exception);
            DbLog.CreateLog(LogHis);



/*            //Console.WriteLine(formatter(state, exception));
            string fileName = Path.Combine(Environment.CurrentDirectory, "output.txt");

            //Создаем пустой файл
            StreamWriter swBegin = new StreamWriter(fileName);
            swBegin.WriteLine("");
            swBegin.Close();

            //Дозаписываем файл
            StreamWriter sw = new StreamWriter(fileName, true); ;
            sw.WriteLine(formatter(state, exception));
            sw.Close();*/
        }
    }
}
