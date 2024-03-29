using System;
using Ardalis.GuardClauses;
using Ethos.Domain.Common;
using Ethos.Domain.Extensions;

namespace Ethos.Domain.Entities
{
    public class Booking : Entity
    {
        public ApplicationUser User { get; private set; }

        public DateTimeOffset StartDate { get; private set; }

        public DateTimeOffset EndDate { get; private set; }

        public Schedule Schedule { get; private set; }

        private Booking(
            Guid id,
            Schedule schedule,
            ApplicationUser user,
            DateTimeOffset startDate,
            DateTimeOffset endDate)
        {
            Id = id;
            User = user;
            StartDate = startDate;
            EndDate = endDate;
            Schedule = schedule;
        }

        public static class Factory
        {
            public static Booking Create(
                Guid id,
                Schedule schedule,
                ApplicationUser user,
                DateTimeOffset startDate,
                DateTimeOffset endDate)
            {
                Guard.Against.Default(id, nameof(id));
                Guard.Against.Null(user);
                Guard.Against.Null(schedule);
                Guard.Against.DifferentTimezone(startDate, schedule.TimeZone);
                Guard.Against.DifferentTimezone(endDate, schedule.TimeZone);
                
                return new Booking(id, schedule, user, startDate, endDate);
            }

            public static Booking CreateFromSnapshot(
                Guid id,
                Schedule schedule,
                ApplicationUser user,
                DateTimeOffset startDate,
                DateTimeOffset endDate)
            {
                return new Booking(id, schedule, user, startDate, endDate);
            }
        }
    }
}
