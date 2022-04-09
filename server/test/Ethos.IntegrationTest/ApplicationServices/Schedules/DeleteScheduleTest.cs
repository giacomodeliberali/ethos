using System;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Application.Contracts;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Services;
using Ethos.Domain.Common;
using Ethos.Domain.Repositories;
using Ethos.IntegrationTest.Setup;
using Ethos.Web.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest.ApplicationServices.Schedules
{
    public class DeleteScheduleTest : BaseIntegrationTest
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleApplicationService _scheduleApplicationService;

        public DeleteScheduleTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
            _scheduleRepository = Scope.ServiceProvider.GetRequiredService<IScheduleRepository>();
            _scheduleApplicationService = Scope.ServiceProvider.GetRequiredService<IScheduleApplicationService>();
        }

        [Fact]
        public async Task Should_DeleteSingleSchedule()
        {
            using var admin = await Scope.WithUser("admin");

            var singleScheduleReply = await _scheduleApplicationService.CreateAsync(new CreateSingleScheduleRequestDto()
            {
                Name = "Single schedule",
                Description = "Schedule",
                StartDate = DateTime.Parse("2021-10-01T08:00"),
                TimeZone = TimeZones.Amsterdam.Id,
                DurationInMinutes = 60,
                OrganizerId = admin.User.Id,
            });

            await _scheduleApplicationService.DeleteAsync(new DeleteSingleScheduleRequestDto()
            {
                Id = singleScheduleReply.Id,
            });

            await Should.ThrowAsync<Exception>(async () =>
            {
                await _scheduleRepository.GetByIdAsync(singleScheduleReply.Id);
            });

            (await ApplicationDbContext.Schedules.AsQueryable().CountAsync()).ShouldBe(0);
            (await ApplicationDbContext.SingleSchedules.AsQueryable().CountAsync()).ShouldBe(0);
            (await ApplicationDbContext.RecurringSchedules.AsQueryable().CountAsync()).ShouldBe(0);
        }

        [Fact]
        public async Task Should_DeleteRecurringSchedule_When_DeletingTheFirstInstance()
        {
            using var admin = await Scope.WithUser("admin");

            var singleScheduleReply = await _scheduleApplicationService.CreateRecurringAsync(new CreateRecurringScheduleRequestDto()
            {
                Name = "Recurring schedule",
                Description = "Schedule",
                StartDate = DateTime.Parse("2021-10-01T00:00"),
                EndDate = DateTime.Parse("2021-12-01T00:00"),
                TimeZone = TimeZones.Amsterdam.Id,
                DurationInMinutes = 60,
                ParticipantsMaxNumber = 2,
                RecurringCronExpression = "0 09 * * MON-FRI",
                OrganizerId = admin.User.Id,
            });

            await _scheduleApplicationService.DeleteRecurringAsync(new DeleteRecurringScheduleRequestDto()
            {
                Id = singleScheduleReply.Id,
                RecurringScheduleOperationType = RecurringScheduleOperationType.InstanceAndFuture,
                InstanceStartDate = DateTime.Parse("2021-10-01T09:00:00"),
                InstanceEndDate = DateTime.Parse("2021-10-01T11:00:00"),
            });

            (await ApplicationDbContext.Schedules.AsQueryable().CountAsync()).ShouldBe(0);
            (await ApplicationDbContext.SingleSchedules.AsQueryable().CountAsync()).ShouldBe(0);
            (await ApplicationDbContext.RecurringSchedules.AsQueryable().CountAsync()).ShouldBe(0);
        }

        [Fact]
        public async Task Should_UpdateDateTime_WhenDeletingFutureRecurringSchedules()
        {
            var firstOctober = DateTime.Parse("2021-10-01T00:00");
            var lastOctober = DateTime.Parse("2021-10-31T00:00");

            using var admin = await Scope.WithUser("admin");
            var scheduleReplyDto = await _scheduleApplicationService.CreateRecurringAsync(new CreateRecurringScheduleRequestDto()
            {
                Name = "Test recurring schedule",
                Description = "Recurring schedule every weekday at 9am",
                StartDate = firstOctober,
                EndDate = lastOctober,
                TimeZone = TimeZones.Amsterdam.Id,
                DurationInMinutes = 120,
                RecurringCronExpression = CronTestExpressions.EveryWeekDayAt9,
                OrganizerId = admin.User.Id,
            });

            await Scope.WithUser("admin");

            await _scheduleApplicationService.DeleteRecurringAsync(new DeleteRecurringScheduleRequestDto()
            {
                Id = scheduleReplyDto.Id,
                InstanceStartDate = DateTime.Parse("2021-10-07T09:00:00"),
                InstanceEndDate = DateTime.Parse("2021-10-07T11:00:00"),
                RecurringScheduleOperationType = RecurringScheduleOperationType.InstanceAndFuture,
            });

            var result = (await _scheduleApplicationService.GetSchedules(firstOctober, lastOctober)).ToList();
            result.Count.ShouldBe(4);

            result.First().StartDate.DateTime.ShouldBe(DateTime.Parse("2021-10-01T09:00:00"));
            result.First().EndDate.DateTime.ShouldBe(DateTime.Parse("2021-10-01T11:00:00"));

            result.Last().StartDate.DateTime.ShouldBe(DateTime.Parse("2021-10-06T09:00:00"));
            result.Last().EndDate.DateTime.ShouldBe(DateTime.Parse("2021-10-06T11:00:00"));
        }

        [Fact]
        public async Task Should_ThrowError_When_InstanceDateTimeAreNotProvided()
        {
            using var admin = await Scope.WithUser("admin");
            var singleScheduleReply = await _scheduleApplicationService.CreateAsync(new CreateSingleScheduleRequestDto()
            {
                Name = "Single schedule",
                Description = "Schedule",
                StartDate = DateTime.Parse("2021-10-01T08:00"),
                TimeZone = TimeZones.Amsterdam.Id,
                DurationInMinutes = 60,
                OrganizerId = admin.User.Id,
            });

            await Should.ThrowAsync<Exception>(async () =>
            {
                // missing instance info
                await _scheduleApplicationService.DeleteAsync(new DeleteSingleScheduleRequestDto()
                {
                    Id = singleScheduleReply.Id,
                });
            });
        }
    }
}
