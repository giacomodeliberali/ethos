using System;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Services;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.IntegrationTest.Setup;
using Ethos.Shared;
using Ethos.Web.Host;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest.ApplicationServices.Schedules
{
    public class CreateScheduleTest : BaseIntegrationTest
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleApplicationService _scheduleApplicationService;

        public CreateScheduleTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
            _scheduleRepository = Scope.ServiceProvider.GetRequiredService<IScheduleRepository>();
            _scheduleApplicationService = Scope.ServiceProvider.GetRequiredService<IScheduleApplicationService>();
        }

        [Fact]
        public async Task Should_SaveOrganizer()
        {
            var admin = await Scope.WithUser("admin");
            var scheduleId = (await _scheduleApplicationService.CreateAsync(new CreateSingleScheduleRequestDto()
            {
                Name = "Test schedule",
                Description = "Description",
                StartDate = DateTime.UtcNow,
                DurationInMinutes = 120,
                OrganizerId = admin.User.Id,
            })).Id;

            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);

            schedule.Organizer.Id.ShouldBe(admin.User.Id);
            schedule.DurationInMinutes.ShouldBe(120);
        }

        [Fact]
        public async Task ShouldThrowError_WhenOrganizerIsNotAdmin()
        {
            var admin = await Scope.WithUser("admin");
            var demoUser = await CreateUser("demoUser", role: RoleConstants.User);

            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await _scheduleApplicationService.CreateAsync(new CreateSingleScheduleRequestDto()
                {
                    Name = "Test schedule",
                    Description = "Description",
                    StartDate = DateTime.UtcNow,
                    DurationInMinutes = 120,
                    OrganizerId = demoUser.Id,
                });
            });
        }

        [Fact]
        public async Task ShouldThrowError_WhenStartDateIsAfterOrEqualEndDate()
        {
            var admin = await Scope.WithUser("admin");
            var demoUser = await CreateUser("demoUser", role: RoleConstants.User);

            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await _scheduleApplicationService.CreateRecurringAsync(new CreateRecurringScheduleRequestDto()
                {
                    Name = "Test schedule",
                    Description = "Description",
                    StartDate = DateTime.UtcNow.AddMonths(1),
                    EndDate = DateTime.UtcNow,
                    OrganizerId = demoUser.Id,
                    DurationInMinutes = 60,
                    RecurringCronExpression = CronTestExpressions.EveryMondayAt14,
                    ParticipantsMaxNumber = 5,
                });
            });

            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await _scheduleApplicationService.CreateAsync(new CreateSingleScheduleRequestDto()
                {
                    Name = "Test schedule",
                    Description = "Description",
                    StartDate = DateTime.UtcNow,
                    OrganizerId = demoUser.Id,
                    DurationInMinutes = 60,
                });
            });
        }

        [Fact]
        public async Task Should_CreateSingleSchedule()
        {
            var admin = await Scope.WithUser("admin");
            var scheduleId = (await _scheduleApplicationService.CreateAsync(new CreateSingleScheduleRequestDto()
            {
                Name = "Test schedule",
                Description = "Description",
                StartDate = DateTime.UtcNow,
                DurationInMinutes = 120,
                OrganizerId = admin.User.Id,
            })).Id;

            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
            schedule.ShouldBeOfType<SingleSchedule>();
        }

        [Fact]
        public async Task Should_CreateRecurringSchedule()
        {
            var admin = await Scope.WithUser("admin");
            var scheduleId = (await _scheduleApplicationService.CreateRecurringAsync(new CreateRecurringScheduleRequestDto()
            {
                Name = "Test schedule",
                Description = "Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                OrganizerId = admin.User.Id,
                RecurringCronExpression = CronTestExpressions.EveryMondayAt14,
                DurationInMinutes = 60,
                ParticipantsMaxNumber = 10,
            })).Id;

            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
            schedule.ShouldBeOfType<RecurringSchedule>();
        }
    }
}
