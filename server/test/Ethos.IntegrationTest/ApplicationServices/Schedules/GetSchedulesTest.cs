using System;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Services;
using Ethos.Common;
using Ethos.Domain.Common;
using Ethos.IntegrationTest.Setup;
using Ethos.Web.Host;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest.ApplicationServices.Schedules
{
    public class GetSchedulesTest : BaseIntegrationTest
    {
        private readonly IScheduleApplicationService _scheduleApplicationService;
        private readonly IBookingApplicationService _bookingApplicationService;

        public GetSchedulesTest(CustomWebApplicationFactory<Startup> factory)
            : base(factory)
        {
            _scheduleApplicationService = Scope.ServiceProvider.GetRequiredService<IScheduleApplicationService>();
            _bookingApplicationService = Scope.ServiceProvider.GetRequiredService<IBookingApplicationService>();
        }

        [Fact]
        public async Task ShouldGenerateInmemorySchedules_AndNotIncludeParticipantsData_IfCaller_IsAdmin()
        {
            var startDate = DateTime.Parse("2031-10-01T07:00:00").ToDateTimeOffset(TimeZones.Amsterdam);

            CreateScheduleReplyDto reply;
            using (var admin = await Scope.WithUser("admin"))
            {
                reply = await _scheduleApplicationService.CreateAsync(new CreateSingleScheduleRequestDto()
                {
                    Name = "Name",
                    Description = "Description",
                    StartDate = startDate,
                    TimeZone = TimeZones.Amsterdam.Id,
                    DurationInMinutes = 120,
                    OrganizerId = admin.User.Id,
                });
            }

            using (await Scope.WithNewUser("userDemo", fullName: "User Demo"))
            {
                await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
                {
                    ScheduleId = reply.Id,
                    StartDate = DateTime.Parse("2031-10-01T07:00:00").ToDateTimeOffset(TimeZones.Amsterdam),
                    EndDate = DateTime.Parse("2031-10-01T09:00:00").ToDateTimeOffset(TimeZones.Amsterdam),
                });
            }

            using (await Scope.WithNewUser("userDemo2", fullName: "User Demo 2"))
            {
                var generatedSchedule = (await _scheduleApplicationService.GetSchedules(startDate, startDate)).ShouldHaveSingleItem();
                var booking = generatedSchedule.Bookings.Single();
                booking.User.ShouldBeNull(); // should not include user data of userDemo
            }
        }

        [Fact]
        public async Task ShouldGenerateInmemorySchedules_AndIncludeParticipantsData_IfCaller_IsUser()
        {
            var startDate = DateTime.Parse("2031-10-01T07:00:00").ToDateTimeOffset(TimeZones.Amsterdam);
            var endDate = DateTime.Parse("2031-10-31T09:00:00").ToDateTimeOffset(TimeZones.Amsterdam);

            CreateScheduleReplyDto reply;
            using (var admin = await Scope.WithUser("admin"))
            {
                reply = await _scheduleApplicationService.CreateAsync(new CreateSingleScheduleRequestDto()
                {
                    Name = "Name",
                    Description = "Description",
                    StartDate = startDate,
                    TimeZone = TimeZones.Amsterdam.Id,
                    DurationInMinutes = 120,
                    OrganizerId = admin.User.Id,
                });
            }

            using (await Scope.WithNewUser("userDemo", fullName: "User Demo", role: RoleConstants.Admin))
            {
                await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
                {
                    ScheduleId = reply.Id,
                    StartDate = DateTime.Parse("2031-10-01T07:00:00").ToDateTimeOffset(TimeZones.Amsterdam),
                    EndDate = DateTime.Parse("2031-10-01T09:00:00").ToDateTimeOffset(TimeZones.Amsterdam),
                });

                var generatedSchedule = (await _scheduleApplicationService.GetSchedules(startDate, endDate)).Single();
                var booking = generatedSchedule.Bookings.Single();

                booking.User.ShouldNotBeNull(); // should include user data
                booking.User.FullName.ShouldBe("User Demo");
            }
        }

        [Fact]
        public async Task ShouldGenerateInmemorySchedules_WhenSchedulesAreRecurring_AndReturnOverlappingSchedules()
        {
            var firstOctober = DateTime.Parse("2021-10-01T00:00:00");
            var lastOctober = DateTime.Parse("2021-10-31T23:59:59");

            using var admin = await Scope.WithUser("admin");

            await _scheduleApplicationService.CreateRecurringAsync(new CreateRecurringScheduleRequestDto()
            {
                Name = "Test recurring schedule",
                Description = "Recurring schedule every weekday at 9am",
                StartDate = firstOctober,
                EndDate = lastOctober,
                TimeZone = TimeZones.Amsterdam.Id,
                DurationInMinutes = 120,
                RecurringCronExpression = "0 09 * * MON-FRI", // every week day at 9am
                OrganizerId = admin.User.Id,
            });


            var generatedSchedules = (await _scheduleApplicationService.GetSchedules(
                    DateTime.Parse("2021-10-4T09:00:00"),
                    DateTime.Parse("2021-10-8T09:00:00"))).ToList();
            generatedSchedules.Count.ShouldBe(5);

            generatedSchedules = (await _scheduleApplicationService.GetSchedules(
                DateTime.Parse("2021-09-1T09:00:00"),
                DateTime.Parse("2021-10-3T09:00:00"))).ToList();
            generatedSchedules.Count.ShouldBe(1);
            generatedSchedules.Single().StartDate.DateTime.ShouldBe(DateTime.Parse("2021-10-1T09:00:00"));
            generatedSchedules.Single().EndDate.DateTime.ShouldBe(DateTime.Parse("2021-10-1T11:00:00"));
            generatedSchedules.Single().DurationInMinutes.ShouldBe(120);


            generatedSchedules = (await _scheduleApplicationService.GetSchedules(
                DateTime.Parse("2021-09-1T09:00:00"),
                DateTime.Parse("2021-10-3T09:00:00"))).ToList();
            generatedSchedules.Count.ShouldBe(1);
            generatedSchedules.Single().StartDate.DateTime.ShouldBe(DateTime.Parse("2021-10-1T09:00:00"));
            generatedSchedules.Single().EndDate.DateTime.ShouldBe(DateTime.Parse("2021-10-1T11:00:00"));
            generatedSchedules.Single().DurationInMinutes.ShouldBe(120);

            generatedSchedules = (await _scheduleApplicationService.GetSchedules(
                DateTime.Parse("2021-10-15T09:00:00"),
                DateTime.Parse("2021-11-15T09:00:00"))).ToList();
            generatedSchedules.Count.ShouldBe(11);
            generatedSchedules.First().StartDate.DateTime.ShouldBe(DateTime.Parse("2021-10-15T09:00:00"));
            generatedSchedules.First().EndDate.DateTime.ShouldBe(DateTime.Parse("2021-10-15T11:00:00"));
            generatedSchedules.Last().StartDate.DateTime.ShouldBe(DateTime.Parse("2021-10-29T09:00:00"));
            generatedSchedules.Last().EndDate.DateTime.ShouldBe(DateTime.Parse("2021-10-29T11:00:00"));
            generatedSchedules.Last().DurationInMinutes.ShouldBe(120);
        }
    }
}
