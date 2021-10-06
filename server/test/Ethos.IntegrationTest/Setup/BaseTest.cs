using System.Net.Http;
using Ethos.Application.Seed;
using Ethos.Domain.Entities;
using Ethos.EntityFrameworkCore;
using Ethos.Web.Host;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ethos.IntegrationTest.Setup
{
    public abstract class BaseTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        protected readonly CustomWebApplicationFactory<Startup> Factory;
        protected readonly HttpClient Client;
        protected readonly UserManager<ApplicationUser> UserManager;
        protected readonly IServiceScope Scope;
        protected readonly ApplicationDbContext ApplicationDbContext;

        protected BaseTest(CustomWebApplicationFactory<Startup> factory)
        {
            Factory = factory;
            Client = Factory.CreateClient();
            Scope = factory.Services.CreateScope();
            UserManager = Scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            ApplicationDbContext = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            ApplicationDbContext.Database.EnsureCreated();

            // seed database
            foreach (var dataSeedContributor in Scope.ServiceProvider.GetServices<IDataSeedContributor>())
            {
                dataSeedContributor.SeedAsync().Wait();
            }
        }
    }
}
