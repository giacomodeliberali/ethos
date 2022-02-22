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

            var scheduleReplyDto = await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
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

            var scheduleReplyDto = await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
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

            var schedulesCount = await ApplicationDbContext.Schedules.CountAsync();
            schedulesCount.ShouldBe(1);
        }
    }
}
