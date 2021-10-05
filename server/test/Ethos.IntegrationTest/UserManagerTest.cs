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
        public async Task ShouldNot_HaveAnyUserWithAdminRole_WhenApplicationStarts()
        {
            var scope = Factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var response = await userManager.GetUsersInRoleAsync(RoleConstants.Admin);
            response.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Should_HaveAUserWithAdminRole_WhenCreated()
        {
            var scope = Factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var response = await userManager.GetUsersInRoleAsync(RoleConstants.Admin);

            response.Count.ShouldBe(0);

            var user = new ApplicationUser
            {
                Email = "demo@ethos.it",
                UserName = "demo"
            };

            await userManager.CreateAsync(user, "P2ssw0rd!");

            await userManager.AddToRoleAsync(user, RoleConstants.Admin);

            response = await userManager.GetUsersInRoleAsync(RoleConstants.Admin);

            response.Count.ShouldBe(1);
        }
    }
}
