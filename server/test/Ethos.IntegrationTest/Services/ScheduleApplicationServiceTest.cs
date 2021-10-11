using System;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Services;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.IntegrationTest.Setup;
using Ethos.Shared;
using Ethos.Web.Host;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest.Services
{
    public class ScheduleApplicationServiceTest : BaseIntegrationTest
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
                OrganizerId = admin.User.Id,
            })).Id;

            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);

            schedule.Organizer.Id.ShouldBe(admin.User.Id);
        }

        [Fact]
        public async Task ShouldNotGenerateInmemorySchedules_WhenSchedulesAreNotRecurring()
        {
            var now = DateTime.Now;

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
        public async Task ShouldGenerateInmemorySchedules_WhenSchedulesAreRecurring()
        {
            var firstOctober = DateTime.Parse("2021-10-01T07:00:00").ToUniversalTime();
            var lastOctober = DateTime.Parse("2021-10-31T23:00:00").ToUniversalTime();

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
                (await _scheduleApplicationService.GetSchedules(firstOctober, lastOctober)).ToList();

            generatedSchedules.Count().ShouldBe(21);
        }

        [Fact]
        public async Task ShouldGenerateInmemorySchedules_WhenSchedulesAreRecurring_NoEndDate()
        {
            var firstOctober = DateTime.Parse("2021-10-01T07:00:00").ToUniversalTime();
            var lastOctober = DateTime.Parse("2021-10-31T23:00:00").ToUniversalTime();

            using var admin = await Scope.WithUser("admin");

            await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
            {
                Name = "Test recurring schedule",
                Description = "Recurring schedule every weekday at 9am",
                StartDate = firstOctober,
                EndDate = null,
                DurationInMinutes = 120,
                RecurringCronExpression = "0 09 * * MON-FRI", // every week day at 9am
                OrganizerId = admin.User.Id,
            });


            var generatedSchedules =
                await _scheduleApplicationService.GetSchedules(firstOctober, lastOctober);

            generatedSchedules.Count().ShouldBe(21);
        }

        [Fact]
        public async Task ShouldCreateABooking_ForTheGivenNonRecurringSchedule()
        {
            var startDate = DateTime.Now;
            var endDate = startDate.AddHours(2);

            Guid scheduleId;
            using (var admin = await Scope.WithUser("admin"))
            {
                scheduleId = (await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
                {
                    Name = "Test schedule",
                    Description = "Description",
                    StartDate = startDate,
                    EndDate = endDate,
                    OrganizerId = admin.User.Id,
                })).Id;
            }

            await Scope.WithNewUser("userDemo", fullName: "User Demo");

            await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
            {
                ScheduleId = scheduleId,
                StartDate = startDate,
                EndDate = endDate,
            });

            var generatedSchedules = (await _scheduleApplicationService.GetSchedules(startDate, endDate)).ToList();

            generatedSchedules.Count().ShouldBe(1);
            generatedSchedules.Select(s => s.Bookings.Count()).Sum().ShouldBe(1);
            generatedSchedules.Single().Bookings.Single().User.ShouldBeNull();
        }

        [Fact]
        public async Task ShouldThrowError_DuringCreation_WhenOrganizerIsNotAdmin()
        {
            var admin = await Scope.WithUser("admin");
            var demoUser = await CreateUser("demoUser", role: RoleConstants.User);

            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
                {
                    Name = "Test schedule",
                    Description = "Description",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(1),
                    OrganizerId = demoUser.Id,
                });
            });
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
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
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
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddMonths(1),
                        OrganizerId = demoUser.Id,
                    },
                });
            });
        }

        [Fact]
        public async Task ShouldUpdateDateTime_WhenDeletingFutureRecurringSchedules()
        {
            var firstOctober = DateTime.Parse("2021-10-01T07:00:00").ToUniversalTime();
            var lastOctober = DateTime.Parse("2021-10-31T23:00:00").ToUniversalTime();

            using var admin = await Scope.WithUser("admin");
            var scheduleReplyDto = await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
            {
                Name = "Test recurring schedule",
                Description = "Recurring schedule every weekday at 9am",
                StartDate = firstOctober,
                EndDate = lastOctober,
                DurationInMinutes = 120,
                RecurringCronExpression = "0 09 * * MON-FRI", // every week day at 9am
                OrganizerId = admin.User.Id,
            });


            CreateBookingReplyDto bookingReply;
            using (await Scope.WithNewUser("demo"))
            {
                bookingReply = await _bookingApplicationService.CreateAsync(new CreateBookingRequestDto()
                {
                    ScheduleId = scheduleReplyDto.Id,
                    StartDate = DateTime.Parse("2021-10-06T09:00:00").ToUniversalTime(),
                    EndDate = DateTime.Parse("2021-10-06T11:00:00").ToUniversalTime(),
                });
            }

            await Scope.WithUser("admin");

            await Should.ThrowAsync<BusinessException>(async () =>
            {
                // Non è possibile eliminare la schedulazione, sono già presenti 1 prenotazioni
                await _scheduleApplicationService.DeleteAsync(new DeleteScheduleRequestDto()
                {
                    Id = scheduleReplyDto.Id,
                    InstanceStartDate = DateTime.Parse("2021-10-06T07:00:00").ToUniversalTime(),
                    InstanceEndDate = DateTime.Parse("2021-10-06T07:00:00").ToUniversalTime(),
                    RecurringScheduleOperationType = RecurringScheduleOperationType.Future,
                });
            });

            await _bookingApplicationService.DeleteAsync(bookingReply.Id);

            await _scheduleApplicationService.DeleteAsync(new DeleteScheduleRequestDto()
            {
                Id = scheduleReplyDto.Id,
                InstanceStartDate = DateTime.Parse("2021-10-06T07:00:00").ToUniversalTime(),
                InstanceEndDate = DateTime.Parse("2021-10-06T07:00:00").ToUniversalTime(),
                RecurringScheduleOperationType = RecurringScheduleOperationType.Future,
            });

            var result = (await _scheduleApplicationService.GetSchedules(firstOctober, lastOctober)).ToList();
            result.Count.ShouldBe(3);

            result.First().StartDate.ShouldBe(DateTime.Parse("2021-10-01T09:00:00").ToUniversalTime());
            result.First().EndDate.ShouldBe(DateTime.Parse("2021-10-01T11:00:00").ToUniversalTime());

            result.Last().StartDate.ShouldBe(DateTime.Parse("2021-10-05T09:00:00").ToUniversalTime());
            result.Last().EndDate.ShouldBe(DateTime.Parse("2021-10-05T11:00:00").ToUniversalTime());
        }

        [Fact]
        public async Task ShouldUpdateSingleScheduleFiled_WhenUpdating()
        {
            using var admin = await Scope.WithUser("admin");

            var scheduleReplyDto = await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
            {
                Name = "Single schedule",
                Description = "Schedule every weekday at 9am",
                StartDate = DateTime.Parse("2021-10-01T07:00:00").ToUniversalTime(),
                EndDate = DateTime.Parse("2021-10-01T09:00:00").ToUniversalTime(),
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
                    StartDate = DateTime.Parse("2021-10-02T10:00:00").ToUniversalTime(),
                    EndDate = DateTime.Parse("2021-10-02T12:00:00").ToUniversalTime(),
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
                DateTime.Parse("2021-10-02T10:00:00").ToUniversalTime(),
                DateTime.Parse("2021-10-02T12:00:00").ToUniversalTime());

            updatedSchedule.ShouldBeEquivalentTo(expectedSchedule);
        }

        [Fact]
        public async Task ShouldConvertSingleScheduleIntoRecurring_WhenUpdating()
        {
            using var admin = await Scope.WithUser("admin");

            var scheduleReplyDto = await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
            {
                Name = "Single schedule",
                Description = "Schedule",
                StartDate = DateTime.Parse("2021-10-01T07:00:00").ToUniversalTime(),
                EndDate = DateTime.Parse("2021-10-01T09:00:00").ToUniversalTime(),
                OrganizerId = admin.User.Id,
            });

            var originalSchedule = await _scheduleRepository.GetByIdAsync(scheduleReplyDto.Id);

            originalSchedule.ShouldBeOfType<SingleSchedule>();

            var newUser = await CreateUser("admin2", role: RoleConstants.Admin);

            await _scheduleApplicationService.UpdateAsync(new UpdateScheduleRequestDto()
            {
                Id = scheduleReplyDto.Id,
                Schedule = new UpdateScheduleRequestDto.ScheduleDto()
                {
                    Name = "Name up",
                    Description = "Description up",
                    StartDate = DateTime.Parse("2021-10-02T10:00:00").ToUniversalTime(),
                    DurationInMinutes = 30,
                    ParticipantsMaxNumber = 1,
                    RecurringCronExpression = "0 09 * * MON-FRI",
                    OrganizerId = newUser.Id,
                },
            });

            var updatedSchedule = await _scheduleRepository.GetByIdAsync(scheduleReplyDto.Id);

            updatedSchedule.ShouldBeOfType<RecurringSchedule>();

            var expectedSchedule = RecurringSchedule.Factory.Create(
                scheduleReplyDto.Id,
                newUser,
                "Name up",
                "Description up",
                1,
                DateTime.Parse("2021-10-02T10:00:00").ToUniversalTime(),
                null,
                30,
                "0 09 * * MON-FRI");

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
                StartDate = DateTime.Parse("2021-10-01T00:00:00").ToUniversalTime(),
                EndDate = DateTime.Parse("2021-10-31T00:00:00").ToUniversalTime(),
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
                InstanceStartDate = DateTime.Parse("2021-10-15T09:00:00").ToUniversalTime(),
                InstanceEndDate = DateTime.Parse("2021-10-15T11:00:00").ToUniversalTime(),
                RecurringScheduleOperationType = RecurringScheduleOperationType.Future,
                Schedule = new UpdateScheduleRequestDto.ScheduleDto()
                {
                    Name = "Name up",
                    Description = "Description up",
                    StartDate = DateTime.Parse("2021-10-01T00:00:00").ToUniversalTime(),
                    EndDate = null,
                    DurationInMinutes = 30,
                    ParticipantsMaxNumber = 1,
                    RecurringCronExpression = "0 09 * * MON-FRI",
                    OrganizerId = newUser.Id,
                },
            });

            var updatedSchedule = await _scheduleRepository.GetByIdAsync(scheduleReplyDto.Id);

            updatedSchedule.ShouldBeOfType<RecurringSchedule>();

            var expectedSchedule = RecurringSchedule.Factory.Create(
                scheduleReplyDto.Id,
                admin.User,
                "Single schedule",
                "Schedule",
                2,
                DateTime.Parse("2021-10-01T00:00:00").ToUniversalTime(),
                DateTime.Parse("2021-10-15T09:00:00").ToUniversalTime(),
                60,
                "0 09 * * MON-FRI");

            updatedSchedule.ShouldBeEquivalentTo(expectedSchedule);
        }
    }
}
