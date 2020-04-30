using System;
using EFCore.Bugs.Entities;
using EFCore.Bugs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace EFCore.Bugs
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            CreateUsers(serviceProvider);
	        Issue(serviceProvider);
            Resolve_1(serviceProvider);
            Resolve_2(serviceProvider);
            Resolve_3(serviceProvider);
            Console.ReadKey();
        }

        private static void Issue(ServiceProvider serviceProvider)
        {
	        var logger = serviceProvider.GetService<ILogger<Program>>();
	        logger.LogInformation("[Issue] Running..."); 
	        using var db = serviceProvider.GetService<AppDbContext>();
            var takeValue = 2;
            var users = db.Users
                .Include(u => u.Address)
                .Take(takeValue)
                .Select(b => new UserModel()
                {
                    Id = b.Id,
                    IsValue1 = b.Address.SomeEnum == SomeEnum.Value2 // if SomeEnum.Value2 == takeValue then throw Exception
                })
                // .Where(u => u.IsValue1)
                .ToList();
        }

        /// <summary>
        /// Move 'Take' after 'Select'
        /// </summary>
        /// <param name="serviceProvider"></param>
        private static void Resolve_1(ServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetService<ILogger<Program>>();
            logger.LogInformation("[Resolve #1] Running...");
            using var db = serviceProvider.GetService<AppDbContext>();
	        var takeValue = 2;
	        var users = db.Users
		        .Include(u => u.Address)
		        .Select(b => new UserModel()
		        {
			        Id = b.Id,
			        IsValue1 = b.Address.SomeEnum == SomeEnum.Value2
		        })
                // .Where(u => u.IsValue1) // if add the 'Where' then EF will generate the same SQL as for Resolve #1, #2 and #3
                .Take(takeValue)
		        .ToList();
        }

        /// <summary>
        /// Thanks to the "enumValue" variable, the SQL query is better, so this option is preferred if enum without 'FlagsAttribute'.
        /// </summary>
        /// <param name="serviceProvider"></param>
        private static void Resolve_2(ServiceProvider serviceProvider)
        {
	        var logger = serviceProvider.GetService<ILogger<Program>>();
	        logger.LogInformation("[Resolve #2] Running...");
            using var db = serviceProvider.GetService<AppDbContext>();
            var takeValue = 2;
            var enumValue = SomeEnum.Value2;
            var users = db.Users
                .Include(u => u.Address)
                .Take(takeValue)
                .Select(b => new UserModel()
                {
                    Id = b.Id,
                    IsValue1 = b.Address.SomeEnum == enumValue
                })
                // .Where(u => u.IsValue1) // if add the 'Where' then EF will generate the same SQL as for Resolve #1, #2 and #3
                .ToList();
        }

        /// <summary>
        /// An alternative way to solve. But will be bitwise comparison instead direct comparison.
        /// </summary>
        /// <param name="serviceProvider"></param>
        private static void Resolve_3(ServiceProvider serviceProvider)
        {
	        var logger = serviceProvider.GetService<ILogger<Program>>();
	        logger.LogInformation("[Resolve #3] Running...");
	        using var db = serviceProvider.GetService<AppDbContext>();
            var takeValue = 2;
            var users = db.Users
                .Include(u => u.Address)
                .Take(takeValue)
                .Select(b => new UserModel()
                {
                    Id = b.Id,
                    IsValue1 = b.Address.SomeEnum.HasFlag(SomeEnum.Value2)
                })
                // .Where(u => u.IsValue1) // if add the 'Where' then EF will generate the same SQL as for Resolve #1, #2 and #3
                .ToList();
        }

        private static void CreateUsers(ServiceProvider serviceProvider)
        {
            using var db = serviceProvider.GetService<AppDbContext>();
            db.Users.RemoveRange(db.Users);
            db.Users.Add(new User() { Address = new Address() { SomeEnum = SomeEnum.Value0 } });
            db.Users.Add(new User() { Address = new Address() { SomeEnum = SomeEnum.Value1 } });
            db.Users.Add(new User() { Address = new Address() { SomeEnum = SomeEnum.Value2 } });
            db.Users.Add(new User() { Address = new Address() { SomeEnum = SomeEnum.Value3 } });
            db.SaveChanges();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole())
                .AddTransient<AppDbContext>();
        }
    }
}
