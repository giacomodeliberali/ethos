using System;
using Ardalis.GuardClauses;
using Ethos.Domain.Common;

namespace Ethos.Domain.Entities
{
    public class ScheduleException : Entity
    {
        public RecurringSchedule Schedule { get; private set; }

        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

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

                return new ScheduleException()
                {
                    Id = id,
                    Schedule = schedule,
                    EndDate = endDate,
                    StartDate = startDate,
                };
            }

            public static ScheduleException FromSnapshot(
                Guid id,
                RecurringSchedule schedule,
                DateTime startDate,
                DateTime endDate)
            {
                return new ScheduleException()
                {
                    Id = id,
                    Schedule = schedule,
                    EndDate = endDate,
                    StartDate = startDate,
                };
            }
        }
    }
}
