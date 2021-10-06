using System;
using Ethos.Domain.Entities;
using Shouldly;
using Xunit;

namespace Ethos.Domain.UnitTest
{
    public class ScheduleTest
    {
        [Fact]
        public void ShouldCreate_NonRecurring_Schedule()
        {
            var user = new ApplicationUser()
            {
                Id = Guid.NewGuid(),
            };

            var startDate = DateTime.Now;
            var endDate = startDate.AddMonths(1);

            var sut = Schedule.Factory.CreateNonRecurring(
                user,
                name: "Schedule",
                description: "Description",
                startDate,
                endDate);

            sut.ShouldNotBeNull();
            sut.Organizer.Id.ShouldBe(user.Id);
            sut.StartDate.ShouldBe(startDate);
            sut.EndDate.ShouldBe(endDate);
            sut.Name.ShouldBe("Schedule");
            sut.Description.ShouldBe("Description");
        }

        [Fact]
        public void ShouldCreate_Recurring_Schedule()
        {
            var user = new ApplicationUser()
            {
                Id = Guid.NewGuid(),
            };

            var startDate = DateTime.Now;

            var sut = Schedule.Factory.CreateRecurring(
                user,
                name: "Schedule recurring",
                description: "Recurring schedule with no end",
                startDate,
                endDate: null,
                duration: TimeSpan.FromHours(1),
                recurringExpression: "@weekly");

            sut.ShouldNotBeNull();
            sut.Organizer.Id.ShouldBe(user.Id);
            sut.StartDate.ShouldBe(startDate);
            sut.EndDate.ShouldBeNull();
            sut.Name.ShouldBe("Schedule recurring");
            sut.Description.ShouldBe("Recurring schedule with no end");
            sut.RecurringCronExpression.ShouldNotBeNull();
        }
    }
}
