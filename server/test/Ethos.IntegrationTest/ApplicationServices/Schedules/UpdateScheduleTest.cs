using System;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Application.Contracts;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Services;
using Ethos.Common;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.IntegrationTest.Setup;
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

        public UpdateScheduleTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
            _scheduleRepository = Scope.ServiceProvider.GetRequiredService<IScheduleRepository>();
            _scheduleApplicationService = Scope.ServiceProvider.GetRequiredService<IScheduleApplicationService>();
        }

        [Fact]
        public async Task ShouldThrowError_WhenOrganizerIsNotAdmin()
        {
            var admin = await Scope.WithUser("admin");
            var demoUser = await CreateUser("demoUser", role: RoleConstants.User);

            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddHours(2);

            var scheduleReplyDto = await _scheduleApplicationService.CreateAsync(new CreateSingleScheduleRequestDto()
            {
                Name = "Test schedule",
                Description = "Description",
                StartDate = startDate,
                DurationInMinutes = 60,
                OrganizerId = admin.User.Id,
            });

            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await _scheduleApplicationService.UpdateSingleAsync(new UpdateSingleScheduleRequestDto()
                {
                    Id = scheduleReplyDto.Id,
                    Name = "Test schedule up",
                    Description = "Description up",
                    StartDate = DateTime.UtcNow,
                    DurationInMinutes = 120,
                    OrganizerId = demoUser.Id,
                });
            });
        }

        [Fact]
        public async Task Should_UpdateSingle()
        {
            using var admin = await Scope.WithUser("admin");

            var scheduleReplyDto = await _scheduleApplicationService.CreateAsync(new CreateSingleScheduleRequestDto()
            {
                Name = "Single schedule",
                Description = "Schedule every weekday at 9am",
                StartDate = DateTime.Parse("2021-10-01T08:00Z").ToUniversalTime(),
                DurationInMinutes = 120,
                OrganizerId = admin.User.Id,
            });

            var newUser = await CreateUser("admin2", role: RoleConstants.Admin);

            await _scheduleApplicationService.UpdateSingleAsync(new UpdateSingleScheduleRequestDto()
            {
                Id = scheduleReplyDto.Id,
                Name = "Name up",
                Description = "Description up",
                StartDate = DateTime.Parse("2021-10-02T10:00Z").ToUniversalTime(),
                DurationInMinutes = 60,
                OrganizerId = newUser.Id,
            });

            var updatedSchedule = await _scheduleRepository.GetByIdAsync(scheduleReplyDto.Id);

            var expectedSchedule = SingleSchedule.Factory.Create(
                scheduleReplyDto.Id,
                newUser,
                "Name up",
                "Description up",
                0,
                new Period(
                    DateTime.Parse("2021-10-02T10:00Z").ToUniversalTime(),
                    DateTime.Parse("2021-10-02T11:00Z").ToUniversalTime()));

            updatedSchedule.ShouldBeEquivalentTo(expectedSchedule);

            var schedulesCount = await ApplicationDbContext.Schedules.AsQueryable().CountAsync();
            schedulesCount.ShouldBe(1);
        }
        
        [Fact]
        public async Task Should_UpdateRecurringSingleInstance()
        {
            using var admin = await Scope.WithUser("admin");

            var scheduleReplyDto = await _scheduleApplicationService.CreateRecurringAsync(new CreateRecurringScheduleRequestDto()
            {
                Name = "Recurring schedule",
                Description = "Schedule every weekday at 9am",
                StartDate = DateTime.Parse("2021-01-01T00:00Z").ToUniversalTime(),
                EndDate = DateTime.Parse("2021-01-31T00:00Z").ToUniversalTime(),
                RecurringCronExpression = CronTestExpressions.EveryWeekDayAt9,
                DurationInMinutes = 60,
                OrganizerId = admin.User.Id,
                ParticipantsMaxNumber = 1,
            });

            var newUser = await CreateUser("admin2", role: RoleConstants.Admin);

            await _scheduleApplicationService.UpdateRecurringAsync(new UpdateRecurringScheduleRequestDto()
            {
                Id = scheduleReplyDto.Id,
                Name = "Edit for 4 jan",
                Description = "This day is different",
                StartDate = DateTime.Parse("2021-01-04T19:00Z").ToUniversalTime(),
                EndDate = DateTime.Parse("2021-01-04T21:00Z").ToUniversalTime(),
                RecurringCronExpression = CronTestExpressions.EveryWeekDayAt9,
                DurationInMinutes = 120,
                OrganizerId = newUser.Id,
                ParticipantsMaxNumber = 2,
                InstanceStartDate = DateTime.Parse("2021-01-05T09:00Z").ToUniversalTime(),
                InstanceEndDate = DateTime.Parse("2021-01-05T10:00Z").ToUniversalTime(),
                RecurringScheduleOperationType = RecurringScheduleOperationType.Instance,
            });

            var updatedSchedule = await _scheduleRepository.GetByIdAsync(scheduleReplyDto.Id);

            var expectedSchedule = RecurringSchedule.Factory.Create(
                scheduleReplyDto.Id,
                admin.User,
                "Recurring schedule",
                "Schedule every weekday at 9am",
                1,
                new Period(
                    DateTime.Parse("2021-01-01T08:00Z").ToUniversalTime(),
                    DateTime.Parse("2021-01-31T00:00Z").ToUniversalTime()),
                60,
                CronTestExpressions.EveryWeekDayAt9);

            updatedSchedule.ShouldBeEquivalentTo(expectedSchedule);

            var schedules = await _scheduleApplicationService.GetSchedules(
                DateTime.Parse("2021-01-01T00:00Z").ToUniversalTime(),
                DateTime.Parse("2021-01-31T00:00Z").ToUniversalTime());

            schedules.Count().ShouldBe(21);

            var recurring = schedules.ElementAt(0);
            recurring.ScheduleId.ShouldBe(scheduleReplyDto.Id);
            recurring.IsRecurring.ShouldBeTrue();
            recurring.StartDate.ShouldBe(DateTime.Parse("2021-01-01T09:00Z").ToUniversalTime());
            recurring.EndDate.ShouldBe(DateTime.Parse("2021-01-01T10:00Z").ToUniversalTime());

            var jan4Recurring = schedules.ElementAt(1);
            jan4Recurring.IsRecurring.ShouldBeTrue();
            jan4Recurring.StartDate.ShouldBe(DateTime.Parse("2021-01-04T09:00Z").ToUniversalTime());
            jan4Recurring.EndDate.ShouldBe(DateTime.Parse("2021-01-04T10:00Z").ToUniversalTime());

            var single = schedules.ElementAt(2);
            single.IsRecurring.ShouldBeFalse();
            single.Organizer.UserName.ShouldBe("admin2");
            single.StartDate.ShouldBe(DateTime.Parse("2021-01-04T19:00Z").ToUniversalTime());
            single.EndDate.ShouldBe(DateTime.Parse("2021-01-04T21:00Z").ToUniversalTime());
            single.DurationInMinutes.ShouldBe(120);
            single.ParticipantsMaxNumber.ShouldBe(2);
            single.Name.ShouldBe("Edit for 4 jan");
            single.Description.ShouldBe("This day is different");
            
            var jan6Recurring = schedules.ElementAt(3);
            jan6Recurring.IsRecurring.ShouldBeTrue();
            jan6Recurring.StartDate.ShouldBe(DateTime.Parse("2021-01-06T09:00Z").ToUniversalTime());
            jan6Recurring.EndDate.ShouldBe(DateTime.Parse("2021-01-06T10:00Z").ToUniversalTime());
        }
        
         [Fact]
        public async Task Should_UpdateRecurringInstanceAndFuture()
        {
            using var admin = await Scope.WithUser("admin");

            var scheduleReplyDto = await _scheduleApplicationService.CreateRecurringAsync(new CreateRecurringScheduleRequestDto()
            {
                Name = "Recurring schedule",
                Description = "Schedule every weekday at 9am",
                StartDate = DateTime.Parse("2021-01-01T00:00Z").ToUniversalTime(),
                EndDate = DateTime.Parse("2021-01-31T00:00Z").ToUniversalTime(),
                RecurringCronExpression = CronTestExpressions.EveryWeekDayAt9,
                DurationInMinutes = 60,
                OrganizerId = admin.User.Id,
                ParticipantsMaxNumber = 1,
            });

            var newUser = await CreateUser("admin2", role: RoleConstants.Admin);

            await _scheduleApplicationService.UpdateRecurringAsync(new UpdateRecurringScheduleRequestDto()
            {
                Id = scheduleReplyDto.Id,
                Name = "15th jan onwards",
                Description = "From mid month is different",
                StartDate = DateTime.Parse("2021-01-25T00:00Z").ToUniversalTime(),
                EndDate = DateTime.Parse("2021-01-31T00:00Z").ToUniversalTime(),
                RecurringCronExpression = CronTestExpressions.EveryWeekDayAt9,
                DurationInMinutes = 60,
                OrganizerId = newUser.Id,
                ParticipantsMaxNumber = 5,
                InstanceStartDate = DateTime.Parse("2021-01-15T09:00Z").ToUniversalTime(),
                InstanceEndDate = DateTime.Parse("2021-01-15T10:00Z").ToUniversalTime(),
                RecurringScheduleOperationType = RecurringScheduleOperationType.InstanceAndFuture,
            });

            var updatedSchedule = await _scheduleRepository.GetByIdAsync(scheduleReplyDto.Id);

            var expectedSchedule = RecurringSchedule.Factory.Create(
                scheduleReplyDto.Id,
                admin.User,
                "Recurring schedule",
                "Schedule every weekday at 9am",
                1,
                new Period(
                    DateTime.Parse("2021-01-01T08:00Z").ToUniversalTime(),
                    DateTime.Parse("2021-01-15T00:00Z").ToUniversalTime()),
                60,
                CronTestExpressions.EveryWeekDayAt9);

            updatedSchedule.ShouldBeEquivalentTo(expectedSchedule);

            var schedules = await _scheduleApplicationService.GetSchedules(
                DateTime.Parse("2021-01-01T00:00Z").ToUniversalTime(),
                DateTime.Parse("2021-01-31T00:00Z").ToUniversalTime());

            schedules.Count().ShouldBe(16);

            var recurring = await _scheduleApplicationService.GetAllRecurring();
            
            recurring.Count().ShouldBe(2);
        }
    }
}
