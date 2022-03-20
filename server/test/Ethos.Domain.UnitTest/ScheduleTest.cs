using System;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Shouldly;
using Xunit;

namespace Ethos.Domain.UnitTest
{
    public class ScheduleTest : BaseUnitTest
    {
        [Fact]
        public void ShouldCreate_NonRecurring_Schedule()
        {
            var user = new ApplicationUser(GuidGenerator.Create(), "ShouldCreate_NonRecurring_Schedule@test.com", "username", "Full Name");

            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddMonths(1);

            var id = GuidGenerator.Create();
            var sut = SingleSchedule.Factory.Create(
                id,
                user,
                name: "Schedule",
                description: "Description",
                participantsMaxNumber: 10,
                new Period(startDate, endDate));

            sut.ShouldNotBeNull();
            sut.Id.ShouldBe(id);
            sut.Organizer.Id.ShouldBe(user.Id);
            sut.Period.StartDate.ShouldBe(startDate.ToUniversalTime());
            sut.Period.EndDate.ShouldBe(endDate.ToUniversalTime());
            sut.Name.ShouldBe("Schedule");
            sut.Description.ShouldBe("Description");
            sut.ParticipantsMaxNumber.ShouldBe(10);
        }

        [Fact]
        public void ShouldCreate_Recurring_Schedule()
        {
            var user = new ApplicationUser(GuidGenerator.Create(), "ShouldCreate_Recurring_Schedule@test.com", "username", "Full Name");

            var startDate = DateTime.UtcNow;

            var sut = RecurringSchedule.Factory.Create(
                GuidGenerator.Create(),
                user,
                name: "Schedule recurring",
                description: "Recurring schedule with no end",
                participantsMaxNumber: 0,
                new Period(startDate, startDate.AddMonths(1)),
                duration: 60,
                recurringExpression: "@weekly");

            sut.ShouldNotBeNull();
            sut.Organizer.Id.ShouldBe(user.Id);
            sut.Period.StartDate.ShouldBe(startDate.Date.ToUniversalTime());
            sut.Period.EndDate.ShouldBe(startDate.AddMonths(1).Date.AddDays(1).AddTicks(-1));
            sut.Name.ShouldBe("Schedule recurring");
            sut.Description.ShouldBe("Recurring schedule with no end");
            sut.RecurringCronExpression.ShouldNotBeNull();
            sut.ParticipantsMaxNumber.ShouldBe(0);
        }

        [Fact]
        public void Should_Throw_WhenInvalidCron()
        {
            Should.Throw<BusinessException>(() =>
            {
                var user = new ApplicationUser(GuidGenerator.Create(), "Should_Throw_WhenInvalidCron@test.com", "username", "Full Name");

                var startDate = DateTime.UtcNow;

                RecurringSchedule.Factory.Create(
                    GuidGenerator.Create(),
                    user,
                    name: "Schedule recurring",
                    description: "Recurring schedule with no end",
                    participantsMaxNumber: 0,
                    new Period(startDate, startDate.AddMonths(1)),
                    duration: 60,
                    recurringExpression: "00 10 37 ? * SUN,MON");
            });
        }

        [Fact]
        public void ShouldCreate_FromSnapshot()
        {
            var guid = GuidGenerator.Create();
            var user = new ApplicationUser(GuidGenerator.Create(), "ShouldCreate_FromSnapshot@test.com", "username", "Full Name");

            var startDate = DateTime.UtcNow;
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
            sut.Period.StartDate.ShouldBe(startDate);
            sut.Period.EndDate.ShouldBe(endDate);
            sut.Name.ShouldBe("name");
            sut.Description.ShouldBe("description");
            sut.ParticipantsMaxNumber.ShouldBe(10);
            sut.RecurringCronExpression.ShouldNotBeNull();
            sut.RecurringCronExpressionString.ShouldBe("0 09 * * MON-FRI");
        }
    }
}
