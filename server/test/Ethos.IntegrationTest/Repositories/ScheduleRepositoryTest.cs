using System;
using System.Threading.Tasks;
using Ethos.Domain.Entities;
using Ethos.Domain.Repositories;
using Ethos.IntegrationTest.Setup;
using Ethos.Shared;
using Ethos.Web.Host;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest.Repositories
{
    public class ScheduleRepositoryTest : BaseIntegrationTest
    {
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleRepositoryTest(CustomWebApplicationFactory<Startup> factory)
            : base(factory)
        {
            _scheduleRepository = Scope.ServiceProvider.GetRequiredService<IScheduleRepository>();
        }

        [Fact]
        public async Task Should_Create_And_GetById()
        {
            var organizer = await CreateUser("organizerDemo", RoleConstants.Admin);

            var scheduleId = await _scheduleRepository.CreateAsync(GenerateScheduleFor(organizer));

            scheduleId.ShouldNotBe(Guid.Empty);

            await CurrentUnitOfWork.SaveChangesAsync();

            var createdSchedule = await _scheduleRepository.GetByIdAsync(scheduleId);

            var expected = RecurringSchedule.Factory.Create(
                scheduleId,
                organizer,
                "Test schedule",
                "Description",
                3,
                DateTime.Parse("2021-10-01T07:00:00").ToUniversalTime(),
                DateTime.Parse("2021-10-31T09:00:00").ToUniversalTime(),
                120,
                "0 09 * * MON-FRI"
            );

            createdSchedule.ShouldBeEquivalentTo(expected);
        }


        [Fact]
        public async Task Should_Update()
        {
            var organizer = await CreateUser("organizerDemo", RoleConstants.Admin);

            var scheduleId = await _scheduleRepository.CreateAsync(GenerateScheduleFor(organizer));

            await CurrentUnitOfWork.SaveChangesAsync();

            var originalSchedule = (await _scheduleRepository.GetByIdAsync(scheduleId)) as RecurringSchedule;

            var newStartDate = DateTime.Parse("2021-10-03T07:00:00").ToUniversalTime();
            var newEndDate = DateTime.Parse("2021-10-5T09:00:00").ToUniversalTime();

            originalSchedule.UpdateDateTime(newStartDate, newEndDate, 60, "0 09 * * MON-FRI");
            originalSchedule.UpdateParticipantsMaxNumber(3);
            originalSchedule.UpdateNameAndDescription("New name", "New description");

            await _scheduleRepository.UpdateAsync(originalSchedule);

            await CurrentUnitOfWork.SaveChangesAsync();

            var updatedSchedule = await _scheduleRepository.GetByIdAsync(scheduleId);

            var expected = RecurringSchedule.Factory.Create(
                scheduleId,
                organizer,
                "New name",
                "New description",
                3,
                newStartDate,
                newEndDate,
                60,
                "0 09 * * MON-FRI"
            );


            updatedSchedule.ShouldBeEquivalentTo(expected);
        }

        private Schedule GenerateScheduleFor(ApplicationUser organizer)
        {

            var startDate = DateTime.Parse("2021-10-01T07:00:00").ToUniversalTime();
            var endDate = DateTime.Parse("2021-10-31T09:00:00").ToUniversalTime();

            var schedule = RecurringSchedule.Factory.Create(
                GuidGenerator.Create(),
                organizer,
                "Test schedule",
                "Description",
                3,
                startDate,
                endDate,
                120,
                "0 09 * * MON-FRI"
            );

            return schedule;
        }
    }
}
