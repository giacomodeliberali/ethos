using System.Net.Http;
using Ethos.EntityFrameworkCore;
using Ethos.Web.Host;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ethos.IntegrationTest.Setup
{
    public abstract class BaseTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        protected readonly CustomWebApplicationFactory<Startup> Factory;
        protected readonly HttpClient Client;
        protected readonly IServiceScope ServiceScope;
        protected readonly ApplicationDbContext DbContext;

        protected BaseTest(CustomWebApplicationFactory<Startup> factory)
        {
            Factory = factory;
            Client = Factory.CreateClient();

            var scope = Factory.Services.CreateScope();
            DbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            DbContext.Database.EnsureCreated();
            ServiceScope = scope;
        }
    }
}
