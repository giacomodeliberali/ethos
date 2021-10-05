using System;
using System.Data.Common;
using System.Linq;
using Ethos.Application.Seed;
using Ethos.EntityFrameworkCore;
using Ethos.Web.Host;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ethos.IntegrationTest.Setup
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {
        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var connection = CreateInMemoryDatabase();
            builder.ConfigureServices(services =>
            {
                var descriptor =
                    services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var descriptor2 = services.SingleOrDefault(d => d.ServiceType == typeof(ApplicationDbContext));

                if (descriptor2 != null)
                {
                    services.Remove(descriptor2);
                }

                services.AddEntityFrameworkSqlite();

                // Create a new service provider.
                var provider = services
                    .AddEntityFrameworkSqlite()
                    .BuildServiceProvider();

                // Add a database context (ApplicationDbContext) using an in-memory
                // database for testing.
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlite(connection);
                    options.UseInternalServiceProvider(provider);
                });

                // Build the service provider.
                var serviceProvider = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var loggerFactory = scopedServices.GetRequiredService<ILoggerFactory>();

                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();

                    // seed database
                    foreach (var dataSeedContributor in scope.ServiceProvider.GetServices<IDataSeedContributor>())
                    {
                        dataSeedContributor.SeedAsync().Wait();
                    }
                }
            });
        }
    }
}
