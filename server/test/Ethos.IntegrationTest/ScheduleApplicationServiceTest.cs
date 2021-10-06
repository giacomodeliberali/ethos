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
        private readonly IScheduleApplicationService _scheduleApplicationService;
        private readonly IBookingApplicationService _bookingApplicationService;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private static ApplicationUser _admin;

        public ScheduleApplicationServiceTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
            // TODO proper setup
            _admin = UserManager.FindByNameAsync(RoleConstants.Admin).Result;

            var currentUser = Substitute.For<ICurrentUser>();
            currentUser.GetCurrentUser().Returns(_admin);

            _scheduleRepository = Scope.ServiceProvider.GetRequiredService<IScheduleRepository>();
            var bookingRepository = Scope.ServiceProvider.GetRequiredService<IBookingRepository>();
            var scheduleQueryService = Scope.ServiceProvider.GetRequiredService<IScheduleQueryService>();
            var bookingQueryService = Scope.ServiceProvider.GetRequiredService<IBookingQueryService>();
            _unitOfWork = Scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            _scheduleApplicationService = new ScheduleApplicationService(
                _unitOfWork,
                _scheduleRepository,
                currentUser,
                scheduleQueryService,
                bookingQueryService);

            _bookingApplicationService = new BookingApplicationService(
                _unitOfWork,
                bookingRepository,
                _scheduleRepository,
                currentUser);
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

            schedule.Organizer.Id.ShouldBe(_admin.Id);
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

            var scheduleId = await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
            {
                Name = "Test recurring schedule",
                Description = "Recurring schedule every weekday at 9am",
                StartDate = firstOctober,
                EndDate = lastOctober,
                DurationInMinutes = 120,
                RecurringCronExpression = "0 09 * * MON-FRI" // every week day at 9am

            });

            var generatedSchedules = (await _scheduleApplicationService.GetSchedules(firstOctober, lastOctober)).ToList();

            generatedSchedules.Count().ShouldBe(21);
        }

        [Fact]
        public async Task ShouldCreateABooking_ForTheGivenNonRecurringSchedule()
        {
            var startDate = DateTime.Now;
            var endDate = startDate.AddHours(2);

            var scheduleId = await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
            {
                Name = "Test schedule",
                Description = "Description",
                StartDate = startDate,
                EndDate = endDate,
            });

            var bookingId = await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
            {
                ScheduleId = scheduleId,
                StartDate = startDate,
                EndDate = endDate,
            });

            var generatedSchedules = (await _scheduleApplicationService.GetSchedules(startDate, endDate)).ToList();

            generatedSchedules.Count().ShouldBe(1);
            generatedSchedules.Select(s => s.Bookings.Count()).Sum().ShouldBe(1);
        }
    }
}
