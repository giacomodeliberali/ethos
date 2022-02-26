using System;
using System.Linq;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Contracts.Identity;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Email;
using Ethos.Application.Identity;
using Ethos.Application.Seed;
using Ethos.Application.Services;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Query.Projections;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
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
            serviceCollection.AddTransient<IBookingApplicationService, BookingApplicationService>();

            // add guid generator
            serviceCollection.AddSingleton<IGuidGenerator, SequentialGuidGenerator>();

            serviceCollection.AddEthosAutoMapper();

            serviceCollection.AddDataSeedContributors();

            // add MediatoR
            serviceCollection.AddMediatR(typeof(IApplicationModuleAssemblyMarker));
            serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidateCommandsPipelineBehaviour<,>));

            // add fluent validation
            serviceCollection.AddFluentValidation();
            serviceCollection.AddValidatorsFromAssembly(typeof(IApplicationModuleAssemblyMarker).Assembly);
        }

        private static void AddDataSeedContributors(this IServiceCollection serviceCollection)
        {
            var dataSeedContributorInterface = typeof(IDataSeedContributor);
            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => a.FullName!.StartsWith("Ethos", StringComparison.InvariantCulture))
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsClass && !p.IsAbstract && dataSeedContributorInterface.IsAssignableFrom(p));

            foreach (var type in types)
            {
                serviceCollection.AddTransient(dataSeedContributorInterface, type);
            }
        }

        private static void AddEthosAutoMapper(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAutoMapper(options =>
            {
                options.CreateMap<Booking, BookingDto>();
                options.CreateMap<Schedule, BookingDto.ScheduleDto>();
                options.CreateMap<ApplicationUser, BookingDto.UserDto>();
                options.CreateMap<ApplicationUser, UserDto>();
                options.CreateMap<ApplicationUser, GeneratedScheduleDto.UserDto>();
                options.CreateMap<UserProjection, UserDto>();
            });
        }
    }
}
