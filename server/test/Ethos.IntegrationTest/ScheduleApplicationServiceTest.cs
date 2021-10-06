using System;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Identity;
using Ethos.Application.Services;
using Ethos.Domain.Entities;
using Ethos.Domain.Repositories;
using Ethos.IntegrationTest.Setup;
using Ethos.Query;
using Ethos.Query.Services;
using Ethos.Web.Host;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest
{
    public class ScheduleApplicationServiceTest : BaseTest
    {
        private readonly IScheduleApplicationService _scheduleApplicationService;
        private readonly IScheduleRepository _scheduleRepository;
        private static Guid _userId;

        public ScheduleApplicationServiceTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
            // TODO proper setup
            var admin = UserManager.FindByNameAsync("admin").Result;
            _userId = admin.Id;

            var currentUser = Substitute.For<ICurrentUser>();
            currentUser.GetCurrentUser().Returns(admin);

            _scheduleRepository = Scope.ServiceProvider.GetRequiredService<IScheduleRepository>();
            var queryService = Scope.ServiceProvider.GetRequiredService<IScheduleQueryService>();
            var unitOfWork = Scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            _scheduleApplicationService = new ScheduleApplicationService(unitOfWork, _scheduleRepository, currentUser, queryService);
        }

        [Fact]
        public async Task ShouldCreateABooking()
        {
            var scheduleId = await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
            {
                Name = "Test schedule",
                Description = "Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
            });

            scheduleId.ShouldNotBe(Guid.Empty);

            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);

            schedule.Organizer.Id.ShouldBe(_userId);
        }

        [Fact]
        public async Task ShouldNotGenerateInmemorySchedules_WhenSchedulesAreNotRecurring()
        {
            var now = DateTime.Now;

            var scheduleId = await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
            {
                Name = "Test schedule",
                Description = "Description",
                StartDate = now,
                EndDate = now.AddMonths(1),
            });

            var schedules = await _scheduleApplicationService.GetSchedules(now, now.AddMonths(1));

            schedules.Count().ShouldBe(1);
        }

        [Fact]
        public async Task ShouldGenerateInmemorySchedules_WhenSchedulesAreRecurring()
        {
            var firstOctober = DateTime.Parse("2021-10-01T07:00:00").ToUniversalTime();
            var lastOctober = DateTime.Parse("2021-10-31T23:00:00").ToUniversalTime();

            await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
            {
                Name = "Test recurring schedule",
                Description = "Recurring schedule every weekday at 9am",
                StartDate = firstOctober,
                EndDate = lastOctober,
                DurationInMinutes = 120,
                RecurringCronExpression = "0 09 * * MON-FRI" // every week day at 9am

            });

            var schedules = await _scheduleApplicationService.GetSchedules(firstOctober, lastOctober);

            schedules.Count().ShouldBe(21);
        }

        [Fact]
        public async Task ShouldCreateASchedule_WithTheGivenGuid()
        {
            var unitOfWork = Scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var admin = await UserManager.FindByNameAsync("admin");

            var schedule = Schedule.Factory.CreateNonRecurring(
                admin,
           "Name",
       "Description",
        DateTime.Now,
                DateTime.Now.AddHours(2));

            var guid = await _scheduleRepository.CreateAsync(schedule);

            await unitOfWork.SaveChangesAsync();

            var created = await _scheduleRepository.GetByIdAsync(guid);

            created.Organizer.Id.ShouldBe(admin.Id);

            await _scheduleRepository.CreateAsync(schedule);
        }
    }
}
