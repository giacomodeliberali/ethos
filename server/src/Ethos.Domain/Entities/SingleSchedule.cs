using System;
using Ardalis.GuardClauses;

namespace Ethos.Domain.Entities
{
    public class SingleSchedule : Schedule
    {
        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        private SingleSchedule()
        {
        }

        public void UpdateDateTime(DateTime startDate, DateTime endDate)
        {
            Guard.Against.Null(endDate, nameof(endDate));

            StartDate = startDate;
            EndDate = endDate;
            DurationInMinutes = (int)(endDate - startDate).TotalMinutes;
        }

        public static class Factory
        {
            public static SingleSchedule Create(
                Guid guid,
                ApplicationUser organizer,
                string name,
                string description,
                int participantsMaxNumber,
                DateTime startDate,
                DateTime endDate)
            {
                Guard.Against.Null(organizer, nameof(organizer));
                Guard.Against.NullOrEmpty(name, nameof(name));
                Guard.Against.NullOrEmpty(description, nameof(description));

                return new SingleSchedule()
                {
                    Id = guid,
                    Organizer = organizer,
                    Name = name,
                    Description = description,
                    ParticipantsMaxNumber = participantsMaxNumber,
                    StartDate = startDate,
                    EndDate = endDate,
                    DurationInMinutes = (int)(endDate - startDate).TotalMinutes,
                };
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
                return new SingleSchedule()
                {
                    Id = id,
                    Organizer = organizer,
                    StartDate = startDate,
                    EndDate = endDate,
                    DurationInMinutes = duration,
                    Name = name,
                    Description = description,
                    ParticipantsMaxNumber = participantsMaxNumber,
                };
            }
        }
    }
}
