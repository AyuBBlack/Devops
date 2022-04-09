using Microsoft.EntityFrameworkCore;
namespace SqlBundle.Models
{
    public class SqlContext : DbContext
    {
        public SqlContext(DbContextOptions<SqlContext> options)
    : base(options) { }
        public DbSet<History> Tables { get; set; } = null!;
        public DbSet<HistoryLogger> LogHistories { get; set; } = null!;
    }
}
