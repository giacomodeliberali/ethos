using System;
using Ethos.Domain.Entities;
using Shouldly;
using Xunit;

namespace Ethos.Domain.UnitTest
{
    public class ScheduleTest : BaseUnitTest
    {
        [Fact]
        public void ShouldCreate_NonRecurring_Schedule()
        {
            var user = new ApplicationUser()
            {
                Id = GuidGenerator.Create(),
            };

            var startDate = DateTime.Now;
            var endDate = startDate.AddMonths(1);

            var id = GuidGenerator.Create();
            var sut = SingleSchedule.Factory.Create(
                id,
                user,
                name: "Schedule",
                description: "Description",
                participantsMaxNumber: 10,
                startDate,
                endDate);

            sut.ShouldNotBeNull();
            sut.Id.ShouldBe(id);
            sut.Organizer.Id.ShouldBe(user.Id);
            sut.StartDate.ShouldBe(startDate);
            sut.EndDate.ShouldBe(endDate);
            sut.Name.ShouldBe("Schedule");
            sut.Description.ShouldBe("Description");
            sut.ParticipantsMaxNumber.ShouldBe(10);
        }

        [Fact]
        public void ShouldCreate_Recurring_Schedule()
        {
            var user = new ApplicationUser()
            {
                Id = GuidGenerator.Create(),
            };

            var startDate = DateTime.Now;

            var sut = RecurringSchedule.Factory.Create(
                GuidGenerator.Create(),
                user,
                name: "Schedule recurring",
                description: "Recurring schedule with no end",
                participantsMaxNumber: 0,
                startDate,
                endDate: null,
                duration: 60,
                recurringExpression: "@weekly");

            sut.ShouldNotBeNull();
            sut.Organizer.Id.ShouldBe(user.Id);
            sut.StartDate.ShouldBe(startDate);
            sut.EndDate.ShouldBeNull();
            sut.Name.ShouldBe("Schedule recurring");
            sut.Description.ShouldBe("Recurring schedule with no end");
            sut.RecurringCronExpression.ShouldNotBeNull();
            sut.ParticipantsMaxNumber.ShouldBe(0);
        }

        [Fact]
        public void ShouldCreate_FromSnapshot()
        {
            var guid = GuidGenerator.Create();
            var user = new ApplicationUser()
            {
                Id = GuidGenerator.Create(),
            };

            var startDate = DateTime.Now;
            var endDate = startDate.AddMonths(2);

            var sut = RecurringSchedule.Factory.FromSnapshot(
                    guid,
                    user,
                    startDate,
                    endDate,
                    recurringExpression: "0 09 * * MON-FRI",
                    duration: 60,
                    name: "name",
                    description: "description",
                    participantsMaxNumber: 10);

            sut.Id.ShouldBe(guid);
            sut.Organizer.ShouldBe(user);
            sut.StartDate.ShouldBe(startDate);
            sut.EndDate.ShouldBe(endDate);
            sut.Name.ShouldBe("name");
            sut.Description.ShouldBe("description");
            sut.ParticipantsMaxNumber.ShouldBe(10);
            sut.RecurringCronExpression.ShouldNotBeNull();
            sut.RecurringCronExpressionString.ShouldBe("0 09 * * MON-FRI");
        }
    }
}
