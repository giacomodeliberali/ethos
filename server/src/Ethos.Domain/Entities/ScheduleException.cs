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
            DateTime startDate,
            DateTime endDate)
        {
            Id = id;
            Schedule = schedule;
            StartDate = startDate;
            EndDate = endDate;
        }

        public RecurringSchedule Schedule { get; }

        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        public static class Factory
        {
            public static ScheduleException Create(
                Guid id,
                RecurringSchedule schedule,
                DateTime startDate,
                DateTime endDate)
            {
                Guard.Against.Default(id, nameof(id));
                Guard.Against.Null(schedule, nameof(schedule));
                Guard.Against.Default(startDate, nameof(startDate));
                Guard.Against.Default(endDate, nameof(endDate));

                return new ScheduleException(id, schedule, startDate, endDate);
            }

            public static ScheduleException FromSnapshot(
                Guid id,
                RecurringSchedule schedule,
                DateTime startDate,
                DateTime endDate)
            {
                return new ScheduleException(id, schedule, startDate, endDate);
            }
        }
    }
}
