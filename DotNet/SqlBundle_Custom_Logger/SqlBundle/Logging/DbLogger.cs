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
