using System;
using Ardalis.GuardClauses;
using Ethos.Domain.Common;

namespace Ethos.Domain.Entities
{
    public class SingleSchedule : Schedule
    {
        public Period Period { get; private set; }

        private SingleSchedule(
            Guid id,
            ApplicationUser organizer,
            Period period,
            int duration,
            string name,
            string description,
            int participantsMaxNumber)
            : base(id, organizer, name, description, participantsMaxNumber, duration)
        {
            Period = period;
        }

        public void UpdatePeriod(Period period)
        {
            Guard.Against.Null(period, nameof(period));
            Period = period;
            DurationInMinutes = period.DurationInMinutes;
        }

        public static class Factory
        {
            public static SingleSchedule Create(
                Guid guid,
                ApplicationUser organizer,
                string name,
                string description,
                int participantsMaxNumber,
                Period period)
            {
                Guard.Against.Null(organizer, nameof(organizer));
                Guard.Against.NullOrEmpty(name, nameof(name));
                Guard.Against.NullOrEmpty(description, nameof(description));
                Guard.Against.Null(period, nameof(period));

                return new SingleSchedule(
                    guid,
                    organizer,
                    period,
                    period.DurationInMinutes,
                    name,
                    description,
                    participantsMaxNumber);
            }

            public static SingleSchedule FromSnapshot(
                Guid id,
                ApplicationUser organizer,
                DateTime startDate,
                DateTime endDate,
                int duration,
                string name,
                string description,
                int participantsMaxNumber)
            {
                return new SingleSchedule(
                    id,
                    organizer,
                    new Period(startDate, endDate),
                    duration,
                    name,
                    description,
                    participantsMaxNumber);
            }
        }
    }
}
