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

        protected BaseTest(CustomWebApplicationFactory<Startup> factory)
        {
            Factory = factory;
            Client = Factory.CreateClient();
        }
    }
}
