using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Common;
using Ethos.IntegrationTest.Setup;
using Ethos.Query.Services;
using Ethos.Web.Host;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest.Infrastructure
{
    public class ApplicationSeedTest : BaseIntegrationTest
    {
        public ApplicationSeedTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
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

        [Fact]
        public async Task Should_Return_SeededAdmin()
        {
            var userQuery = Scope.ServiceProvider.GetRequiredService<IUserQueryService>();
            var admins = (await userQuery.GetAllAdminsAsync()).ToList();

            admins.Count().ShouldBe(1);

            var admin = admins.Single();

            admin.UserName.ShouldBe("admin");
            admin.Roles.ShouldBeEquivalentTo(new List<string>()
            {
                RoleConstants.Admin,
            });
        }
    }
}
