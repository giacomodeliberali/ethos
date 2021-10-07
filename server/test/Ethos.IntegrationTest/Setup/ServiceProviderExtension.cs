using System;
using System.Threading.Tasks;
using Ethos.Domain.Entities;
using Ethos.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Ethos.IntegrationTest.Setup
{
    internal static class ServiceProviderExtension
    {
        internal static ChangeUserDisposable WithUser(this IServiceScope serviceScope, ApplicationUser user)
        {
            return new ChangeUserDisposable(serviceScope.ServiceProvider, user);
        }

        internal static async Task<ChangeUserDisposable> WithUser(this IServiceScope serviceScope, string userName)
        {
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByNameAsync(userName);
            return new ChangeUserDisposable(serviceScope.ServiceProvider, user);
        }

        internal static async Task<ChangeUserDisposable> WithNewUser(this IServiceScope serviceScope, string userName, string role = RoleConstants.User, string fullName = null, Guid? id = null)
        {
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var guid = Guid.NewGuid();

            var user = new ApplicationUser()
            {
                Id = id ?? guid,
                Email = $"{guid}@ethos.test.it",
                FullName = fullName ?? $"Utente demo {guid}",
                UserName = userName,
            };

            await userManager.CreateAsync(user, "P2ssw0rd!");

            await userManager.AddToRoleAsync(user, role);

            return new ChangeUserDisposable(serviceScope.ServiceProvider, user);
        }
    }
}