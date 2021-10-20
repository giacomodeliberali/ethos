using Ethos.Domain.Repositories;
using Ethos.EntityFrameworkCore.Query;
using Ethos.EntityFrameworkCore.Repositories;
using Ethos.Query.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Ethos.EntityFrameworkCore
{
    public static class EntityFrameworkCoreModuleExtension
    {
        public static void AddRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IUnitOfWork, UnitOfWork>();
            serviceCollection.AddTransient<IScheduleRepository, ScheduleRepository>();
            serviceCollection.AddTransient<IBookingRepository, BookingRepository>();
            serviceCollection.AddTransient<IScheduleExceptionRepository, ScheduleExceptionRepository>();
        }

        public static void AddQueries(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IUserQueryService, UserQueryService>();
            serviceCollection.AddTransient<IScheduleQueryService, ScheduleQueryService>();
            serviceCollection.AddTransient<IBookingQueryService, BookingQueryService>();
            serviceCollection.AddTransient<IScheduleExceptionQueryService, ScheduleExceptionQueryService>();
        }
    }
}
