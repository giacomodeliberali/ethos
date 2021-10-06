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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest
{
    public class BookingApplicationServiceTest : BaseTest
    {
        private readonly IScheduleApplicationService _scheduleApplicationService;
        private readonly IBookingApplicationService _bookingApplicationService;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private static ApplicationUser _admin;

        public BookingApplicationServiceTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
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

            var bookings = await ApplicationDbContext.Bookings.CountAsync();

            bookings.ShouldBeGreaterThan(0);
        }
    }
}
