using System;
using Ardalis.GuardClauses;
using Ethos.Domain.Common;

namespace Ethos.Domain.Entities
{
    public abstract class Schedule : Entity
    {
        /// <summary>
        /// The admin user that manages this schedule.
        /// </summary>
        public ApplicationUser Organizer { get; protected set; }

        /// <summary>
        /// The schedule friendly name.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// The schedule extended description.
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// The max number of people that can participate to each schedule.
        /// </summary>
        public int ParticipantsMaxNumber { get; protected set; }

        public int DurationInMinutes { get; protected set; }

        public TimeZoneInfo TimeZone { get; protected set; }

        protected Schedule(
            Guid id,
            ApplicationUser organizer,
            string name,
            string description,
            int participantsMaxNumber,
            int durationInMinutes,
            TimeZoneInfo timeZone)
        {
            Id = id;
            Organizer = organizer;
            Name = name;
            Description = description;
            ParticipantsMaxNumber = participantsMaxNumber;
            DurationInMinutes = durationInMinutes;
            TimeZone = timeZone;
        }

        public virtual void UpdateOrganizer(ApplicationUser organizer)
        {
            Guard.Against.Null(organizer, nameof(organizer));
            Organizer = organizer;
        }

        public virtual void UpdateNameAndDescription(string name, string description)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));
            Guard.Against.NullOrEmpty(description, nameof(description));
            Name = name;
            Description = description;
        }

        public virtual void UpdateParticipantsMaxNumber(int participantsMaxNumber)
        {
            Guard.Against.Negative(participantsMaxNumber, nameof(participantsMaxNumber));
            ParticipantsMaxNumber = participantsMaxNumber;
        }
    }
}
