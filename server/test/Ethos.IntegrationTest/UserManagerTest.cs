using System.Linq;
using System.Threading.Tasks;
using Ethos.IntegrationTest.Setup;
using Ethos.Shared;
using Ethos.Web.Host;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest
{
    public class UserManagerTest : BaseTest
    {
        public UserManagerTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task ShouldHave_UserWithAdminRole_WhenApplicationStarts()
        {
            var response = await UserManager.GetUsersInRoleAsync(RoleConstants.Admin);
            response.Count.ShouldBe(1);
            var admin = response.Single();
            admin.Email.ShouldBe("admin@ethos.it");
            admin.UserName.ShouldBe("admin");
        }
    }
}
