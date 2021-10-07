using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ethos.IntegrationTest.Setup;
using Ethos.Shared;
using Ethos.Web.Host;
using Microsoft.AspNetCore.Http;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest
{
    public class SwaggerTest : BaseTest
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
