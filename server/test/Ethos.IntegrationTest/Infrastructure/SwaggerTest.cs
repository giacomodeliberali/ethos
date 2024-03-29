using System.Net;
using System.Threading.Tasks;
using Ethos.IntegrationTest.Setup;
using Ethos.Web.Host;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest.Infrastructure
{
    public class SwaggerTest : BaseIntegrationTest
    {
        public SwaggerTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task ShouldReturn_SwaggerDocument()
        {
            var response = await Client.GetAsync("/swagger/v1/swagger.json");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}
