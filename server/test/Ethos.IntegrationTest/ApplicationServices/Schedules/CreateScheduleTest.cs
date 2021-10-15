using System;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Services;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.IntegrationTest.Setup;
using Ethos.Shared;
using Ethos.Web.Host;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest.ApplicationServices.Schedules
{
    public class CreateScheduleTest : BaseIntegrationTest
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleApplicationService _scheduleApplicationService;
        private readonly IBookingApplicationService _bookingApplicationService;

        public CreateScheduleTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
            _scheduleRepository = Scope.ServiceProvider.GetRequiredService<IScheduleRepository>();
            _scheduleApplicationService = Scope.ServiceProvider.GetRequiredService<IScheduleApplicationService>();
            _bookingApplicationService = Scope.ServiceProvider.GetRequiredService<IBookingApplicationService>();
        }

        [Fact]
        public async Task Should_SaveOrganizer()
        {
            var admin = await Scope.WithUser("admin");
            var scheduleId = (await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
            {
                Name = "Test schedule",
                Description = "Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                OrganizerId = admin.User.Id,
            })).Id;

            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);

            schedule.Organizer.Id.ShouldBe(admin.User.Id);
        }

        [Fact]
        public async Task ShouldThrowError_WhenOrganizerIsNotAdmin()
        {
            var admin = await Scope.WithUser("admin");
            var demoUser = await CreateUser("demoUser", role: RoleConstants.User);

            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await _scheduleApplicationService.CreateAsync(new CreateScheduleRequestDto()
                {
                    Name = "Test schedule",
                    Description = "Description",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(1),
                    OrganizerId = demoUser.Id,
                });
            });
        }
    }
}
