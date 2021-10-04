using Ethos.Application.Identity;
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
        }
    }
}
