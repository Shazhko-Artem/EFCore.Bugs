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
            options.UseSqlServer("Data Source=.; Initial Catalog=EFCore.Bugs; MultipleActiveResultSets=True;");
        }
    }
}