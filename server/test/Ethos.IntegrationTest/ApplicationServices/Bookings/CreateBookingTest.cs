using System;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Services;
using Ethos.Domain.Exceptions;
using Ethos.IntegrationTest.Setup;
using Ethos.Web.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest.ApplicationServices.Bookings
{
    public class CreateBookingTest : BaseIntegrationTest
    {
        private readonly IScheduleApplicationService _scheduleApplicationService;
        private readonly IBookingApplicationService _bookingApplicationService;

        public CreateBookingTest(CustomWebApplicationFactory<Startup> factory)
            : base(factory)
        {
            _scheduleApplicationService = Scope.ServiceProvider.GetRequiredService<IScheduleApplicationService>();
            _bookingApplicationService = Scope.ServiceProvider.GetRequiredService<IBookingApplicationService>();
        }

        [Fact]
        public async Task ShouldCreateABooking()
        {
            Guid scheduleId;
            using (var admin = await Scope.WithUser("admin"))
            {
                scheduleId = (await _scheduleApplicationService.CreateAsync(new CreateSingleScheduleRequestDto()
                {
                    Name = "Test schedule",
                    Description = "Description",
                    StartDate = DateTime.Parse("2031-10-01T10:00Z").ToUniversalTime(),
                    DurationInMinutes = 120,
                    OrganizerId = admin.User.Id,
                })).Id;
            }

            var userDemo = await Scope.WithNewUser("demo");
            await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
            {
                ScheduleId = scheduleId,
                StartDate = DateTime.Parse("2031-10-01T10:00Z").ToUniversalTime(),
                EndDate = DateTime.Parse("2031-10-01T12:00Z").ToUniversalTime(),
            });

            var booking = await ApplicationDbContext.Bookings.AsQueryable().SingleAsync();

            booking.UserId.ShouldBe(userDemo.User.Id);
            booking.ScheduleId.ShouldBe(scheduleId);
        }

        [Fact]
        public async Task ShouldThrow_WhenBookingIsNotValidDateTime_ForNonRecurringSchedule()
        {
            var startDate = DateTime.Parse("2021-10-01T07:00:00").ToUniversalTime();
            var endDate = DateTime.Parse("2021-10-01T09:00:00").ToUniversalTime();

            Guid scheduleId;
            using (var admin = await Scope.WithUser("admin"))
            {
                scheduleId = (await _scheduleApplicationService.CreateAsync(new CreateSingleScheduleRequestDto()
                {
                    Name = "Test schedule",
                    Description = "Description",
                    StartDate = startDate,
                    DurationInMinutes = 120,
                    OrganizerId = admin.User.Id,
                })).Id;
            }

            await Scope.WithNewUser("demo");

            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
                {
                    ScheduleId = scheduleId,
                    StartDate = DateTime.Parse("2021-10-01T06:00:00").ToUniversalTime(), // 1 hour before
                    EndDate = endDate,
                });
            });

            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
                {
                    ScheduleId = scheduleId,
                    StartDate = startDate,
                    EndDate = DateTime.Parse("2021-10-01T08:00:00").ToUniversalTime(), // 1 hour before,
                });
            });

            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
                {
                    ScheduleId = scheduleId,
                    StartDate = DateTime.Parse("2021-10-01T07:30:00").ToUniversalTime(), // 30 min after
                    EndDate = DateTime.Parse("2021-10-01T09:30:00").ToUniversalTime(), // 30 min after
                });
            });
        }

        [Fact]
        public async Task ShouldThrow_WhenBookingIsNotValidDateTime_ForRecurringSchedule()
        {
            var startDate = DateTime.Parse("2031-10-01T09:00:00Z").ToUniversalTime();
            var endDate = DateTime.Parse("2031-10-31T00:00:00Z").ToUniversalTime();

            Guid scheduleId;
            using (var admin = await Scope.WithUser("admin"))
            {
                scheduleId = (await _scheduleApplicationService.CreateRecurringAsync(new CreateRecurringScheduleRequestDto()
                {
                    Name = "Test schedule",
                    Description = "Description",
                    StartDate = startDate,
                    EndDate = endDate,
                    OrganizerId = admin.User.Id,
                    RecurringCronExpression = "0 09 * * MON-FRI", // every week day at 9am
                    DurationInMinutes = 120,
                    ParticipantsMaxNumber = 2,
                })).Id;
            }

            await Scope.WithNewUser("demo");

            await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
            {
                // ok
                ScheduleId = scheduleId,
                StartDate = DateTime.Parse("2031-10-01T09:00:00Z").ToUniversalTime(),
                EndDate = DateTime.Parse("2031-10-01T11:00:00Z").ToUniversalTime(),
            });

            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
                {
                    ScheduleId = scheduleId,
                    StartDate = DateTime.Parse("2031-10-01T08:00:00Z").ToUniversalTime(),
                    EndDate = DateTime.Parse("2031-10-01T09:00:00Z").ToUniversalTime(),
                });
            });

            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
                {
                    // a month after
                    ScheduleId = scheduleId,
                    StartDate = DateTime.Parse("2031-11-01T09:00:00Z").ToUniversalTime(),
                    EndDate = DateTime.Parse("2031-11-01T11:00:00Z").ToUniversalTime(),
                });
            });

            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
                {
                    // duration is incorrect
                    ScheduleId = scheduleId,
                    StartDate = DateTime.Parse("2031-10-01T09:00:00Z").ToUniversalTime(),
                    EndDate = DateTime.Parse("2031-10-01T10:00:00Z").ToUniversalTime(),
                });
            });


            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
                {
                    // interval is shifter +30min
                    ScheduleId = scheduleId,
                    StartDate = DateTime.Parse("2031-10-01T09:30:00Z").ToUniversalTime(),
                    EndDate = DateTime.Parse("2031-10-01T10:30:00Z").ToUniversalTime(),
                });
            });

            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
                {
                    // interval is shifted +10h
                    ScheduleId = scheduleId,
                    StartDate = DateTime.Parse("2031-10-01T19:00:00Z").ToUniversalTime(),
                    EndDate = DateTime.Parse("2031-10-01T21:00:00Z").ToUniversalTime(),
                });
            });
        }

        [Fact]
        public async Task ShouldThrow_WhenNumberOfParticipantsExceeds()
        {
            var startDate = DateTime.Parse("2031-10-01T07:00:00").ToUniversalTime();
            var endDate = DateTime.Parse("2031-10-31T00:00:00").ToUniversalTime();

            Guid scheduleId;
            using (var admin = await Scope.WithUser("admin"))
            {
                scheduleId = (await _scheduleApplicationService.CreateRecurringAsync(new CreateRecurringScheduleRequestDto()
                {
                    Name = "Test schedule",
                    Description = "Description",
                    StartDate = startDate,
                    EndDate = endDate,
                    OrganizerId = admin.User.Id,
                    RecurringCronExpression = "0 09 * * MON-FRI", // every week day at 9am
                    DurationInMinutes = 120,
                    ParticipantsMaxNumber = 2, // max 2
                })).Id;
            }

            using (await Scope.WithNewUser("demo"))
            {
                await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
                {
                    ScheduleId = scheduleId,
                    StartDate = DateTime.Parse("2031-10-01T09:00:00").ToUniversalTime(),
                    EndDate = DateTime.Parse("2031-10-01T11:00:00").ToUniversalTime(),
                });
            }

            Guid bookingDemo1;
            using (await Scope.WithNewUser("demo1"))
            {
                var reply = await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
                {
                    ScheduleId = scheduleId,
                    StartDate = DateTime.Parse("2031-10-01T09:00:00").ToUniversalTime(),
                    EndDate = DateTime.Parse("2031-10-01T11:00:00").ToUniversalTime(),
                });
                bookingDemo1 = reply.Id;
            }

            using (await Scope.WithNewUser("demo2"))
            {
                await Should.ThrowAsync<ParticipantsMaxNumberReachedException>(async () =>
                {
                    await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
                    {
                        // max participants reached
                        ScheduleId = scheduleId,
                        StartDate = DateTime.Parse("2031-10-01T09:00:00").ToUniversalTime(),
                        EndDate = DateTime.Parse("2031-10-01T11:00:00").ToUniversalTime(),
                    });
                });
            }

            using (await Scope.WithUser("demo1"))
            {
                await _bookingApplicationService.DeleteAsync(bookingDemo1);
            }

            using (await Scope.WithUser("demo2"))
            {
                await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
                {
                    ScheduleId = scheduleId,
                    StartDate = DateTime.Parse("2031-10-01T09:00:00").ToUniversalTime(),
                    EndDate = DateTime.Parse("2031-10-01T11:00:00").ToUniversalTime(),
                });
            }
        }


        [Fact]
        public async Task ShouldCreateABooking_ForTheGivenNonRecurringSchedule()
        {
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddHours(2);

            Guid scheduleId;
            using (var admin = await Scope.WithUser("admin"))
            {
                scheduleId = (await _scheduleApplicationService.CreateAsync(new CreateSingleScheduleRequestDto()
                {
                    Name = "Test schedule",
                    Description = "Description",
                    StartDate = startDate,
                    DurationInMinutes = 120,
                    OrganizerId = admin.User.Id,
                })).Id;
            }

            await Scope.WithNewUser("userDemo", fullName: "User Demo");

            await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
            {
                ScheduleId = scheduleId,
                StartDate = startDate.ToUniversalTime(),
                EndDate = endDate.ToUniversalTime(),
            });

            var generatedSchedules = (await _scheduleApplicationService.GetSchedules(startDate, endDate)).ToList();

            generatedSchedules.Count.ShouldBe(1);
            generatedSchedules.Select(s => s.Bookings.Count()).Sum().ShouldBe(1);
            generatedSchedules.Single().Bookings.Single().User.ShouldNotBeNull().UserName.ShouldBe("userDemo");
        }
    }
}
