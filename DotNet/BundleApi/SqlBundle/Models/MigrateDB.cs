namespace SqlBundle.Models
{
    public static class MigrateDB
    {
        public static WebApplicationBuilder PopulateDB(this WebApplicationBuilder builder)
        {
            using var serviceProvider = builder.Services.BuildServiceProvider(); //Возращает сервис провайдер который реализует интерфейс

            var context = serviceProvider.GetService<SqlContext>();

            if (context == null)
            {
                throw new Exception("Context не создан");
            }
            else
            {
                context.Database.EnsureCreated();
            }
            return builder;
        }
    }
}
