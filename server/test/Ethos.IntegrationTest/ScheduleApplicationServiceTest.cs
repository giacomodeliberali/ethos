using System;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Identity;
using Ethos.Application.Services;
using Ethos.Domain.Entities;
using Ethos.Domain.Repositories;
using Ethos.IntegrationTest.Setup;
using Ethos.Query.Services;
using Ethos.Shared;
using Ethos.Web.Host;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest
{
    public class ScheduleApplicationServiceTest : BaseTest
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleApplicationService _scheduleApplicationService;
        private readonly IBookingApplicationService _bookingApplicationService;

        public ScheduleApplicationServiceTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
            _scheduleRepository = Scope.ServiceProvider.GetRequiredService<IScheduleRepository>();
            _scheduleApplicationService = Scope.ServiceProvider.GetRequiredService<IScheduleApplicationService>();
            _bookingApplicationService = Scope.ServiceProvider.GetRequiredService<IBookingApplicationService>();
        }

        [Fact]
        public async Task ShouldCreateSchedule_WithCurrentLoggedUser()
        {
            var admin = await Scope.WithUser("admin");
            var scheduleId = (await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
            {
                Name = "Test schedule",
                Description = "Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
            })).Id;

            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);

            schedule.Organizer.Id.ShouldBe(admin.User.Id);
        }

        [Fact]
        public async Task ShouldNotGenerateInmemorySchedules_WhenSchedulesAreNotRecurring()
        {
            var now = DateTime.Now;

            using (Scope.WithUser("admin"))
            {
                await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
                {
                    Name = "Test schedule",
                    Description = "Description",
                    StartDate = now,
                    EndDate = now.AddMonths(1),
                });
            }

            var schedules = await _scheduleApplicationService.GetSchedules(now, now.AddMonths(1));

            schedules.Count().ShouldBe(1);
        }

        [Fact]
        public async Task ShouldGenerateInmemorySchedules_WhenSchedulesAreRecurring()
        {
            var firstOctober = DateTime.Parse("2021-10-01T07:00:00").ToUniversalTime();
            var lastOctober = DateTime.Parse("2021-10-31T23:00:00").ToUniversalTime();

            using (Scope.WithUser("admin"))
            {
                await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
                {
                    Name = "Test recurring schedule",
                    Description = "Recurring schedule every weekday at 9am",
                    StartDate = firstOctober,
                    EndDate = lastOctober,
                    DurationInMinutes = 120,
                    RecurringCronExpression = "0 09 * * MON-FRI" // every week day at 9am
                });
            }

            var generatedSchedules =
                (await _scheduleApplicationService.GetSchedules(firstOctober, lastOctober)).ToList();

            generatedSchedules.Count().ShouldBe(21);
        }

        [Fact]
        public async Task ShouldCreateABooking_ForTheGivenNonRecurringSchedule()
        {
            var startDate = DateTime.Now;
            var endDate = startDate.AddHours(2);

            Guid scheduleId;
            using (Scope.WithUser("admin"))
            {
                scheduleId = (await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
                {
                    Name = "Test schedule",
                    Description = "Description",
                    StartDate = startDate,
                    EndDate = endDate,
                })).Id;
            }

            using (await Scope.WithNewUser("userDemo", fullName: "User Demo"))
            {
                await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
                {
                    ScheduleId = scheduleId,
                    StartDate = startDate,
                    EndDate = endDate,
                });
            }

            var generatedSchedules = (await _scheduleApplicationService.GetSchedules(startDate, endDate)).ToList();

            generatedSchedules.Count().ShouldBe(1);
            generatedSchedules.Select(s => s.Bookings.Count()).Sum().ShouldBe(1);
            generatedSchedules.Single().Bookings.Single().UserFullName.ShouldBe("User Demo");
        }
    }
}
