using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Domain.Entities;
using Ethos.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Ethos.IntegrationTest.Setup
{
    internal static class ServiceProviderExtension
    {
        internal static async Task<ChangeUserDisposable> WithUser(this IServiceScope serviceScope, string userName)
        {
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByNameAsync(userName);
            var roles = await userManager.GetRolesAsync(user);

            return new ChangeUserDisposable(serviceScope.ServiceProvider, user, roles);
        }

        internal static async Task<ChangeUserDisposable> WithNewUser(this IServiceScope serviceScope, string userName, string role = RoleConstants.User, string fullName = null, Guid? id = null)
        {
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var guid = Guid.NewGuid();

            var user = new ApplicationUser(id ?? guid, $"{guid}@ethos.test.it", userName, fullName ?? $"Utente demo {guid}");

            await userManager.CreateAsync(user, "P2ssw0rd!");

            await userManager.AddToRoleAsync(user, role);

            return new ChangeUserDisposable(serviceScope.ServiceProvider, user, new List<string>(){ role });
        }
    }
}
