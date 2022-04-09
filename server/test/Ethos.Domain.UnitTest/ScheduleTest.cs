using System;
using System.Linq;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Ethos.Domain.UnitTest
{
    public class ScheduleTest : BaseUnitTest
    {
        [Fact]
        public void ShouldCreate_NonRecurring_Schedule()
        {
            var user = new ApplicationUser(GuidGenerator.Create(), "ShouldCreate_NonRecurring_Schedule@test.com", "username", "Full Name");
            
            var startDate = DateTime.Parse("2022-04-09T10:00");

            var sut = SingleSchedule.Factory.Create(
                GuidGenerator.Create(),
                user,
                name: "Schedule",
                description: "Description",
                participantsMaxNumber: 10,
                startDate,
                60,
                TimeZones.Amsterdam);

            sut.ShouldNotBeNull();
            sut.Id.ShouldNotBe(Guid.Empty);
            sut.Organizer.Id.ShouldBe(user.Id);
            sut.Name.ShouldBe("Schedule");
            sut.Description.ShouldBe("Description");
            sut.ParticipantsMaxNumber.ShouldBe(10);
            
            sut.StartDate.ShouldBe(DateTime.Parse("2022-04-09T10:00"));
            
            sut.EndDate.ShouldBe(DateTime.Parse("2022-04-09T11:00"));
            
            sut.TimeZone.Id.ShouldBe("Europe/Amsterdam");
        }

        [Fact]
        public void ShouldCreate_Recurring_Schedule()
        {
            var user = new ApplicationUser(GuidGenerator.Create(), "ShouldCreate_Recurring_Schedule@test.com", "username", "Full Name");

            var startDate = DateTime.Parse("2022-01-01T10:00");
            var endDate = DateTime.Parse("2023-01-01T10:00");

            var sut = RecurringSchedule.Factory.Create(
                GuidGenerator.Create(),
                user,
                name: "Schedule recurring",
                description: "Recurring schedule with no end",
                participantsMaxNumber: 0,
                new DateOnlyPeriod(startDate, endDate),
                duration: 60,
                recurringExpression: CronTestExpressions.EveryWeekDayAt9,
                TimeZones.Amsterdam);

            sut.ShouldNotBeNull();
            sut.Organizer.Id.ShouldBe(user.Id);
            sut.Name.ShouldBe("Schedule recurring");
            sut.Description.ShouldBe("Recurring schedule with no end");
            sut.ParticipantsMaxNumber.ShouldBe(0);
            sut.RecurringCronExpressionString.ShouldBe("0 09 * * MON-FRI");

            sut.Period.StartDate.ShouldBe(new DateOnly(2022, 01, 01));
            sut.Period.EndDate.ShouldBe(new DateOnly(2023, 01, 01));

            var occurrencesBeforeDayLight = sut.GetOccurrences(
                new DateOnlyPeriod(
                    new DateOnly(2022, 03, 21),
                    new DateOnly(2022, 03, 25)),
                TimeZones.Amsterdam)
                .ToList();
            
            occurrencesBeforeDayLight.Count.ShouldBe(5);
            occurrencesBeforeDayLight.First().StartDate.Offset.ShouldBe(TimeSpan.FromHours(1));
            occurrencesBeforeDayLight.First().StartDate.ShouldBe(DateTime.Parse("2022-03-21T09:00"));
            occurrencesBeforeDayLight.First().EndDate.ShouldBe(DateTime.Parse("2022-03-21T10:00"));
            
            occurrencesBeforeDayLight.Last().StartDate.ShouldBe(DateTime.Parse("2022-03-25T09:00"));
            occurrencesBeforeDayLight.Last().EndDate.ShouldBe(DateTime.Parse("2022-03-25T10:00"));
            
            var occurrencesAfterDayLight = sut.GetOccurrences(
                    new DateOnlyPeriod(
                        new DateOnly(2022, 03, 28),
                        new DateOnly(2022, 04, 1)),
                    TimeZones.Amsterdam)
                .ToList();
            
            occurrencesAfterDayLight.Count.ShouldBe(5);
            
            occurrencesAfterDayLight.First().StartDate.Offset.ShouldBe(TimeSpan.FromHours(2));
            occurrencesAfterDayLight.First().StartDate.ShouldBe(DateTime.Parse("2022-03-28T09:00"));
            occurrencesAfterDayLight.First().EndDate.ShouldBe(DateTime.Parse("2022-03-28T10:00"));
            
            occurrencesAfterDayLight.Last().StartDate.ShouldBe(DateTime.Parse("2022-04-01T09:00"));
            occurrencesAfterDayLight.Last().EndDate.ShouldBe(DateTime.Parse("2022-04-01T10:00"));
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
                    new DateOnlyPeriod(startDate, startDate.AddMonths(1)),
                    duration: 60,
                    recurringExpression: "00 10 37 ? * SUN,MON",
                    TimeZoneInfo.Utc);
            });
        }

        [Fact]
        public void ShouldCreate_FromSnapshot()
        {
            var guid = GuidGenerator.Create();
            var user = new ApplicationUser(GuidGenerator.Create(), "ShouldCreate_FromSnapshot@test.com", "username", "Full Name");

            var startDate = new DateOnly(2022,01, 01);
            var endDate = startDate.AddMonths(1);

            var sut = RecurringSchedule.Factory.FromSnapshot(
                    guid,
                    user,
                    startDate,
                    endDate,
                    recurringExpression: "0 09 * * MON-FRI",
                    duration: 60,
                    name: "name",
                    description: "description",
                    participantsMaxNumber: 10,
                    TimeZoneInfo.Utc);

            sut.Id.ShouldBe(guid);
            sut.Organizer.ShouldBe(user);
            sut.Period.StartDate.ShouldBe(startDate);
            sut.Period.EndDate.ShouldBe(endDate);
            sut.Name.ShouldBe("name");
            sut.Description.ShouldBe("description");
            sut.ParticipantsMaxNumber.ShouldBe(10);
            sut.RecurringCronExpressionString.ShouldBe("0 09 * * MON-FRI");
        }
    }
}
