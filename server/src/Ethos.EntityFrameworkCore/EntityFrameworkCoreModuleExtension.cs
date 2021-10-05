using Ethos.Domain;
using Ethos.Domain.Repositories;
using Ethos.EntityFrameworkCore.Repositories;
using Ethos.EntityFrameworkCore.Schedule;
using Microsoft.Extensions.DependencyInjection;

namespace Ethos.EntityFrameworkCore
{
    public static class EntityFrameworkCoreModuleExtension
    {
        public static void AddRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IScheduleRepository, ScheduleRepository>();
            serviceCollection.AddTransient<IBookingRepository, BookingRepository>();
        }
    }
}
