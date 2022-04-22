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
