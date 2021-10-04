using System.Net;
using System.Threading.Tasks;
using Ethos.IntegrationTest.Setup;
using Ethos.Web.Host;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest
{
    public class ControllerTest : BaseTest
    {
        public ControllerTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task UnauthorizedRequest_ShouldReturn_AppropriateStatusCode()
        {
            // Arrange & Act
            var response = await Client.GetAsync("/WeatherForecast");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}
