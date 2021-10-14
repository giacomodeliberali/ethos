using System;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Services;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.IntegrationTest.Setup;
using Ethos.Shared;
using Ethos.Web.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest.ApplicationServices.Schedules
{
    public class UpdateScheduleTest : BaseIntegrationTest
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleApplicationService _scheduleApplicationService;
        private readonly IBookingApplicationService _bookingApplicationService;

        public UpdateScheduleTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
            _scheduleRepository = Scope.ServiceProvider.GetRequiredService<IScheduleRepository>();
            _scheduleApplicationService = Scope.ServiceProvider.GetRequiredService<IScheduleApplicationService>();
            _bookingApplicationService = Scope.ServiceProvider.GetRequiredService<IBookingApplicationService>();
        }


        [Fact]
        public async Task Should_GenerateSingleInmemorySchedules_WhenSchedulesAreNotRecurring()
        {
            var now = DateTime.UtcNow;

            var admin = await Scope.WithUser("admin");

            await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
            {
                Name = "Test schedule",
                Description = "Description",
                StartDate = now,
                EndDate = now.AddMonths(1),
                OrganizerId = admin.User.Id,
            });

            var schedules = await _scheduleApplicationService.GetSchedules(now, now.AddMonths(1));

            schedules.Count().ShouldBe(1);
        }

        [Fact]
        public async Task Should_GenerateInmemorySchedules_WhenSchedulesAreRecurring()
        {
            var firstOctober = DateTime.Parse("2021-10-01T00:00Z").ToUniversalTime();
            var lastOctober = DateTime.Parse("2021-10-31T00:00Z").ToUniversalTime();

            using var admin = await Scope.WithUser("admin");

            await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
            {
                Name = "Test recurring schedule",
                Description = "Recurring schedule every weekday at 9am",
                StartDate = firstOctober,
                EndDate = lastOctober,
                DurationInMinutes = 120,
                RecurringCronExpression = "0 09 * * MON-FRI", // every week day at 9am
                OrganizerId = admin.User.Id,
            });


            var generatedSchedules = (await _scheduleApplicationService.GetSchedules(firstOctober, lastOctober)).ToList();

            generatedSchedules.Count().ShouldBe(21);
        }

        [Fact]
        public async Task Should_GenerateInmemorySchedules_WhenSchedulesAreRecurring_AndWithoutEndDate()
        {
            var firstOctober = DateTime.Parse("2021-10-01T00:00Z").ToUniversalTime();
            var lastOctober = DateTime.Parse("2021-10-31T00:00Z").ToUniversalTime();

            using var admin = await Scope.WithUser("admin");

            await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
            {
                Name = "Test recurring schedule",
                Description = "Recurring schedule every weekday at 9am",
                StartDate = firstOctober,
                EndDate = lastOctober,
                DurationInMinutes = 120,
                RecurringCronExpression = "0 09 * * MON-FRI", // every week day at 9am
                OrganizerId = admin.User.Id,
            });


            var generatedSchedules =
                await _scheduleApplicationService.GetSchedules(firstOctober, lastOctober);

            generatedSchedules.Count().ShouldBe(21);
        }

        [Fact]
        public async Task ShouldThrowError_DuringUpdate_WhenOrganizerIsNotAdmin()
        {
            var admin = await Scope.WithUser("admin");
            var demoUser = await CreateUser("demoUser", role: RoleConstants.User);

            var scheduleReplyDto = await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
            {
                Name = "Test schedule",
                Description = "Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                OrganizerId = admin.User.Id,
            });

            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await _scheduleApplicationService.UpdateAsync(new UpdateScheduleRequestDto()
                {
                    Id = scheduleReplyDto.Id,
                    Schedule = new UpdateScheduleRequestDto.ScheduleDto()
                    {
                        Name = "Test schedule up",
                        Description = "Description up",
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(1),
                        OrganizerId = demoUser.Id,
                    },
                });
            });
        }

        [Fact]
        public async Task ShouldUpdateSingleScheduleFiled_WhenUpdating()
        {
            using var admin = await Scope.WithUser("admin");

            var scheduleReplyDto = await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
            {
                Name = "Single schedule",
                Description = "Schedule every weekday at 9am",
                StartDate = DateTime.Parse("2021-10-01").ToUniversalTime(),
                EndDate = DateTime.Parse("2021-10-01").ToUniversalTime(),
                OrganizerId = admin.User.Id,
            });

            var newUser = await CreateUser("admin2", role: RoleConstants.Admin);

            await _scheduleApplicationService.UpdateAsync(new UpdateScheduleRequestDto()
            {
                Id = scheduleReplyDto.Id,
                Schedule = new UpdateScheduleRequestDto.ScheduleDto()
                {
                    Name = "Name up",
                    Description = "Description up",
                    StartDate = DateTime.Parse("2021-10-02").ToUniversalTime(),
                    EndDate = DateTime.Parse("2021-10-02").ToUniversalTime(),
                    OrganizerId = newUser.Id,
                },
            });

            var updatedSchedule = await _scheduleRepository.GetByIdAsync(scheduleReplyDto.Id);

            var expectedSchedule = SingleSchedule.Factory.Create(
                scheduleReplyDto.Id,
                newUser,
                "Name up",
                "Description up",
                0,
                new Period(DateTime.Parse("2021-10-02").ToUniversalTime(), DateTime.Parse("2021-10-02").ToUniversalTime()));

            updatedSchedule.ShouldBeEquivalentTo(expectedSchedule);
        }

        [Fact]
        public async Task ShouldConvertSingleScheduleIntoRecurring_WhenUpdating()
        {
            using var admin = await Scope.WithUser("admin");

            var singleScheduleReply = await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
            {
                Name = "Single schedule",
                Description = "Schedule",
                StartDate = DateTime.Parse("2021-10-01T09:00Z").ToUniversalTime(),
                EndDate = DateTime.Parse("2021-10-01T11:00Z").ToUniversalTime(),
                OrganizerId = admin.User.Id,
            });

            var originalSchedule = await _scheduleRepository.GetByIdAsync(singleScheduleReply.Id);

            originalSchedule.ShouldBeOfType<SingleSchedule>();

            var newUser = await CreateUser("admin2", role: RoleConstants.Admin);

            await _scheduleApplicationService.UpdateAsync(new UpdateScheduleRequestDto()
            {
                Id = singleScheduleReply.Id,
                Schedule = new UpdateScheduleRequestDto.ScheduleDto()
                {
                    Name = "Name up",
                    Description = "Description up",
                    StartDate = DateTime.Parse("2021-12-01T00:00Z").ToUniversalTime(),
                    EndDate = DateTime.Parse("2022-12-01T00:00Z").ToUniversalTime(),
                    DurationInMinutes = 30,
                    ParticipantsMaxNumber = 1,
                    RecurringCronExpression = "0 09 * * MON-FRI",
                    OrganizerId = newUser.Id,
                },
            });

            var updatedSchedule = await _scheduleRepository.GetByIdAsync(singleScheduleReply.Id);

            updatedSchedule.ShouldBeOfType<RecurringSchedule>();

            var expectedSchedule = RecurringSchedule.Factory.FromSnapshot(
                singleScheduleReply.Id,
                newUser,
                DateTime.Parse("2021-12-01T00:00Z").ToUniversalTime(),
                DateTime.Parse("2022-12-01T00:00Z").AddDays(1).AddTicks(-1).ToUniversalTime(),
                "0 09 * * MON-FRI",
                30,
                "Name up",
                "Description up",
                1);

            updatedSchedule.ShouldBeEquivalentTo(expectedSchedule);
        }

        [Fact]
        public async Task ShouldUpdateAndKeep_RecurringSchedule()
        {
            using var admin = await Scope.WithUser("admin");

            var scheduleReplyDto = await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
            {
                Name = "Single schedule",
                Description = "Schedule",
                StartDate = DateTime.Parse("2021-10-01T00:00Z").ToUniversalTime(),
                EndDate = DateTime.Parse("2021-10-31T23:59Z").ToUniversalTime(),
                DurationInMinutes = 60,
                ParticipantsMaxNumber = 2,
                RecurringCronExpression = "0 09 * * MON-FRI",
                OrganizerId = admin.User.Id,
            });

            var originalSchedule = await _scheduleRepository.GetByIdAsync(scheduleReplyDto.Id);

            originalSchedule.ShouldBeOfType<RecurringSchedule>();

            var newUser = await CreateUser("admin2", role: RoleConstants.Admin);

            await _scheduleApplicationService.UpdateAsync(new UpdateScheduleRequestDto()
            {
                Id = scheduleReplyDto.Id,
                InstanceStartDate = DateTime.Parse("2021-10-15T09:00:00Z").ToUniversalTime(),
                InstanceEndDate = DateTime.Parse("2021-10-15T11:00:00Z").ToUniversalTime(),
                RecurringScheduleOperationType = RecurringScheduleOperationType.Future,
                Schedule = new UpdateScheduleRequestDto.ScheduleDto()
                {
                    Name = "Name up",
                    Description = "Description up",
                    StartDate = DateTime.Parse("2021-10-16T00:00Z").ToUniversalTime(),
                    EndDate = DateTime.Parse("2021-10-16T00:00Z").ToUniversalTime(),
                    DurationInMinutes = 30,
                    ParticipantsMaxNumber = 1,
                    RecurringCronExpression = "0 09 * * TUE-FRI",
                    OrganizerId = newUser.Id,
                },
            });

            var updatedSchedule = await _scheduleRepository.GetByIdAsync(scheduleReplyDto.Id);

            updatedSchedule.ShouldBeOfType<RecurringSchedule>();

            var expectedSchedule = RecurringSchedule.Factory.FromSnapshot(
                scheduleReplyDto.Id,
                admin.User,
                DateTime.Parse("2021-10-01T00:00Z").ToUniversalTime(),
                DateTime.Parse("2021-10-15T09:00Z").ToUniversalTime(),
                "0 09 * * MON-FRI",
                60,
                "Single schedule",
                "Schedule",
                2);

            updatedSchedule.ShouldBeEquivalentTo(expectedSchedule);

            var newScheduleData = (await ApplicationDbContext.Schedules.ToListAsync()).Last();
            var newSchedule = (await _scheduleRepository.GetByIdAsync(newScheduleData.Id) as RecurringSchedule);

            expectedSchedule = RecurringSchedule.Factory.FromSnapshot(
                newScheduleData.Id,
                newUser,
                DateTime.Parse("2021-10-16T00:00Z").ToUniversalTime(),
                DateTime.Parse("2021-10-16T00:00Z").AddDays(1).AddTicks(-1).ToUniversalTime(),
                "0 09 * * TUE-FRI",
                30,
                "Name up",
                "Description up",
                1);

            newSchedule.ShouldBeEquivalentTo(expectedSchedule);
        }
    }
}
