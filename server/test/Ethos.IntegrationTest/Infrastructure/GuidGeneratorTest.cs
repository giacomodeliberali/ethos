using System.Collections.Generic;
using System.Linq;
using Ethos.IntegrationTest.Setup;
using Ethos.Web.Host;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest.Infrastructure
{
    public class GuidGeneratorTest : BaseIntegrationTest
    {
        public GuidGeneratorTest(CustomWebApplicationFactory<Startup> factory)
            : base(factory)
        {
        }

        [Fact]
        public void ShouldCreateSequentialGuids()
        {
            var guids = new List<string>();

            for (var i = 0; i < 1000; i++)
            {
                // default guid sequential at end for sql server
                // https://docs.abp.io/en/abp/4.4/Guid-Generation#options
                guids.Add(GuidGenerator.Create().ToString().Split("-")[4]);
            }

            var expectedGuids = guids.OrderBy(id => id).ToList();

            guids.SequenceEqual(expectedGuids).ShouldBeTrue();
        }
    }
}
