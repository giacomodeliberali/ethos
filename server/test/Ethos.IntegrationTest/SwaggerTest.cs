using System.Threading.Tasks;
using Ethos.IntegrationTest.Setup;
using Ethos.Web.Host;
using Xunit;

namespace Ethos.IntegrationTest
{
    public class SwaggerTest : BaseTest
    {
        public SwaggerTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task ShouldReturnSwaggerDocument()
        {
            // Arrange & Act
            var response = await Client.GetAsync("/swagger/v1/swagger.json");

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
