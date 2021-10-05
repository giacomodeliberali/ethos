using System;
using System.Linq;
using Ethos.Application.Email;
using Ethos.Application.Identity;
using Ethos.Application.Seed;
using Ethos.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Ethos.Application
{
    /// <summary>
    /// Register the needed services in the DI container.
    /// </summary>
    public static class ApplicationModuleExtensions
    {
        /// <summary>
        /// Register the needed services in the DI container.
        /// </summary>
        /// <param name="serviceCollection">The service collection where the services will be added.</param>
        public static void AddApplicationModule(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IIdentityService, IdentityService>();
            serviceCollection.AddTransient<IEmailSender, EmailSender>();
            serviceCollection.AddTransient<ICurrentUser, CurrentUser>();
            serviceCollection.AddTransient<IScheduleApplicationService, ScheduleApplicationService>();

            serviceCollection.AddDataSeedContributors();
        }

        private static void AddDataSeedContributors(this IServiceCollection serviceCollection)
        {
            var dataSeedContributorInterface = typeof(IDataSeedContributor);
            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsClass && !p.IsAbstract && dataSeedContributorInterface.IsAssignableFrom(p));

            foreach (var type in types)
            {
                serviceCollection.AddTransient(dataSeedContributorInterface, type);
            }
        }
    }
}
