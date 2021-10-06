using System;
using System.Data.Common;
using System.Linq;
using Ethos.Application.Identity;
using Ethos.Application.Seed;
using Ethos.Domain.Entities;
using Ethos.EntityFrameworkCore;
using Ethos.Web.Host;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Ethos.IntegrationTest.Setup
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // remove EF on MSSQL
                services.RemoveAll<ApplicationDbContext>();
                services.RemoveAll<DbContextOptions<ApplicationDbContext>>();

                // remove authentication from HttpContext
                services.RemoveAll<ICurrentUser>();
                var icu = Substitute.For<ICurrentUser>();
                icu.GetCurrentUser().Returns((ApplicationUser) null);

                services.AddTransient((_) => icu);

                // Create a new service provider.
                var provider = services
                    .AddEntityFrameworkSqlite()
                    .BuildServiceProvider();

                // Add a database context (ApplicationDbContext) using an in-memory
                // database for testing.
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlite(CreateInMemoryDatabase());
                    options.UseInternalServiceProvider(provider);
                });
            });
        }
    }
}
