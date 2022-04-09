using System;
using Ardalis.GuardClauses;
using Ethos.Domain.Common;

namespace Ethos.Domain.Entities
{
    public class SingleSchedule : Schedule
    {
        public DateTime StartDate { get; private set; }

        public DateTime EndDate => StartDate.AddMinutes(DurationInMinutes);

        private SingleSchedule(
            Guid id,
            ApplicationUser organizer,
            DateTime startDate,
            int duration,
            string name,
            string description,
            int participantsMaxNumber)
            : base(id, organizer, name, description, participantsMaxNumber, duration)
        {
            StartDate = startDate;
        }

        public void UpdateTime(DateTime startDate, int durationInMinutes)
        {
            Guard.Against.Default(startDate, nameof(startDate));
            Guard.Against.NegativeOrZero(durationInMinutes, nameof(durationInMinutes));

            StartDate = startDate;
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
                DateTime startDate,
                int durationInMinutes)
            {
                Guard.Against.Null(organizer, nameof(organizer));
                Guard.Against.NullOrEmpty(name, nameof(name));
                Guard.Against.NullOrEmpty(description, nameof(description));
                Guard.Against.Default(startDate, nameof(startDate));
                Guard.Against.NegativeOrZero(durationInMinutes, nameof(durationInMinutes));

                return new SingleSchedule(
                    id,
                    organizer,
                    startDate,
                    durationInMinutes,
                    name,
                    description,
                    participantsMaxNumber);
            }

            public static SingleSchedule FromSnapshot(
                Guid id,
                ApplicationUser organizer,
                DateTime startDate,
                int duration,
                string name,
                string description,
                int participantsMaxNumber)
            {
                return new SingleSchedule(
                    id,
                    organizer,
                    startDate,
                    duration,
                    name,
                    description,
                    participantsMaxNumber);
            }
        }
    }
}
