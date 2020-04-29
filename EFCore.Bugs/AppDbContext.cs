using EFCore.Bugs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFCore.Bugs
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        private readonly ILoggerFactory loggerFactory;

        public AppDbContext() : this(new LoggerFactory())
        {
        }

        public AppDbContext(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseLoggerFactory(loggerFactory)
                .EnableSensitiveDataLogging();
            options.UseSqlServer("Data Source=SHAZHKO\\MSSQLSERVER16; Initial Catalog=EFCore.Bugs; User Id=dbadmin;Password=Pwd12345!; MultipleActiveResultSets=True;");
        }
    }
}