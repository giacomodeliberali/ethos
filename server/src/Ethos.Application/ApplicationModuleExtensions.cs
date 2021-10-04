using Application.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationModuleExtensions
    {
        public static void AddApplicationModule(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IIdentityService, IdentityService>();
        }
    }
}
