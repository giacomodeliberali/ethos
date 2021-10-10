using System;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Services;
using Ethos.IntegrationTest.Setup;
using Ethos.Shared;
using Ethos.Web.Host;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest.Services
{
    public class GetSchedulesInRangeTest : BaseIntegrationTest
    {
        private readonly IScheduleApplicationService _scheduleApplicationService;
        private readonly IBookingApplicationService _bookingApplicationService;

        public GetSchedulesInRangeTest(CustomWebApplicationFactory<Startup> factory)
            : base(factory)
        {
            _scheduleApplicationService = Scope.ServiceProvider.GetRequiredService<IScheduleApplicationService>();
            _bookingApplicationService = Scope.ServiceProvider.GetRequiredService<IBookingApplicationService>();
        }

        [Fact]
        public async Task ShouldGenerateInmemorySchedules_AndNotIncludeParticipantsData_IfCaller_IsAdmin()
        {
            var startDate = DateTime.Parse("2021-10-01T07:00:00").ToUniversalTime();
            var endDate = DateTime.Parse("2021-10-31T09:00:00").ToUniversalTime();

            CreateScheduleReplyDto reply;
            using (var admin = await Scope.WithUser("admin"))
            {
                reply = await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
                {
                    Name = "Name",
                    Description = "Description",
                    StartDate = startDate,
                    EndDate = endDate,
                    DurationInMinutes = 120,
                    OrganizerId = admin.User.Id,
                });
            }

            using (await Scope.WithNewUser("userDemo", fullName: "User Demo"))
            {
                await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
                {
                    ScheduleId = reply.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                });

                var generatedSchedule = (await _scheduleApplicationService.GetSchedules(startDate, endDate)).Single();
                var booking = generatedSchedule.Bookings.Single();

                booking.User.ShouldBeNull(); // should not include user data
            }
        }

        [Fact]
        public async Task ShouldGenerateInmemorySchedules_AndIncludeParticipantsData_IfCaller_IsUser()
        {
            var startDate = DateTime.Parse("2021-10-01T07:00:00").ToUniversalTime();
            var endDate = DateTime.Parse("2021-10-31T09:00:00").ToUniversalTime();

            CreateScheduleReplyDto reply;
            using (var admin = await Scope.WithUser("admin"))
            {
                reply = await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
                {
                    Name = "Name",
                    Description = "Description",
                    StartDate = startDate,
                    EndDate = endDate,
                    DurationInMinutes = 120,
                    OrganizerId = admin.User.Id,
                });
            }

            using (await Scope.WithNewUser("userDemo", fullName: "User Demo", role: RoleConstants.Admin))
            {
                await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
                {
                    ScheduleId = reply.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                });

                var generatedSchedule = (await _scheduleApplicationService.GetSchedules(startDate, endDate)).Single();
                var booking = generatedSchedule.Bookings.Single();

                booking.User.ShouldNotBeNull(); // should include user data
                booking.User.FullName.ShouldBe("User Demo");
            }
        }
    }
}
