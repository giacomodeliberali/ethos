using System;
using Ardalis.GuardClauses;
using Ethos.Domain.Extensions;

namespace Ethos.Domain.Entities
{
    public class SingleSchedule : Schedule
    {
        public DateTimeOffset StartDate { get; private set; }

        public DateTimeOffset EndDate => StartDate.AddMinutes(DurationInMinutes);

        private SingleSchedule(
            Guid id,
            ApplicationUser organizer,
            DateTimeOffset startDate,
            int duration,
            string name,
            string description,
            int participantsMaxNumber,
            TimeZoneInfo timeZone)
            : base(id, organizer, name, description, participantsMaxNumber, duration, timeZone)
        {
            StartDate = TimeZoneInfo.ConvertTime(startDate, TimeZone);
        }

        public void UpdateTime(DateTimeOffset startDate, int durationInMinutes)
        {
            Guard.Against.Default(startDate, nameof(startDate));
            Guard.Against.DifferentTimezone(startDate, TimeZone);
            Guard.Against.NegativeOrZero(durationInMinutes, nameof(durationInMinutes));

            StartDate = TimeZoneInfo.ConvertTime(startDate, TimeZone);
            DurationInMinutes = durationInMinutes;
        }

        public static class Factory
        {
            public static SingleSchedule Create(
                Guid id,
                ApplicationUser organizer,
                string name,
                string description,
                int participantsMaxNumber,
                DateTimeOffset startDate,
                int durationInMinutes,
                TimeZoneInfo timeZone)
            {
                Guard.Against.Null(organizer, nameof(organizer));
                Guard.Against.NullOrEmpty(name, nameof(name));
                Guard.Against.NullOrEmpty(description, nameof(description));
                Guard.Against.Default(startDate, nameof(startDate));
                Guard.Against.Null(timeZone, nameof(timeZone));
                Guard.Against.DifferentTimezone(startDate, timeZone);
                Guard.Against.NegativeOrZero(durationInMinutes, nameof(durationInMinutes));

                return new SingleSchedule(
                    id,
                    organizer,
                    startDate,
                    durationInMinutes,
                    name,
                    description,
                    participantsMaxNumber,
                    timeZone);
            }

            public static SingleSchedule FromSnapshot(
                Guid id,
                ApplicationUser organizer,
                DateTimeOffset startDate,
                int duration,
                string name,
                string description,
                int participantsMaxNumber,
                TimeZoneInfo timeZone)
            {
                return new SingleSchedule(
                    id,
                    organizer,
                    startDate,
                    duration,
                    name,
                    description,
                    participantsMaxNumber,
                    timeZone);
            }
        }
    }
}
