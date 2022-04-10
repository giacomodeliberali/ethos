using System;
using Ardalis.GuardClauses;
using Ethos.Domain.Common;

#pragma warning disable CA1711

namespace Ethos.Domain.Entities
{
    public class ScheduleException : Entity
    {
        private ScheduleException(
            Guid id,
            RecurringSchedule schedule,
            DateOnly date)
        {
            Id = id;
            Schedule = schedule;
            Date = date;
        }

        public RecurringSchedule Schedule { get; }

        public DateOnly Date { get; }

        public static class Factory
        {
            public static ScheduleException Create(
                Guid id,
                RecurringSchedule schedule,
                DateOnly date)
            {
                Guard.Against.Default(id, nameof(id));
                Guard.Against.Null(schedule, nameof(schedule));
                Guard.Against.Default(date, nameof(date));

                return new ScheduleException(id, schedule, date);
            }

            public static ScheduleException FromSnapshot(
                Guid id,
                RecurringSchedule schedule,
                DateOnly date)
            {
                return new ScheduleException(id, schedule, date);
            }
        }
    }
}
