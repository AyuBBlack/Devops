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
        public DbSet<History> Tables { get; set; } = null!;
    }
}
