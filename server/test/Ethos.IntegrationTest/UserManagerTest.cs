using System;
using System.Threading.Tasks;
using Ethos.Domain.Identity;
using Ethos.IntegrationTest.Setup;
using Ethos.Shared;
using Ethos.Web;
using Ethos.Web.Host;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        }
    }
}
