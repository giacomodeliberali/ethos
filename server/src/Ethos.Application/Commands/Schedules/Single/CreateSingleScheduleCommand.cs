using System;
using MediatR;
#pragma warning disable CA1716

namespace Ethos.Application.Commands.Schedules.Single
{
    public class CreateSingleScheduleCommand : IRequest<Guid>
    {
        public string Name { get; }

        public string Description { get; }

        public DateTime StartDate { get; }

        public int DurationInMinutes { get; }

        public int ParticipantsMaxNumber { get; }

        /// <summary>
        /// The id of the organizer of this schedule.
        /// </summary>
        public Guid OrganizerId { get; }

        public CreateSingleScheduleCommand(
            string name,
            string description,
            DateTime startDate,
            int durationInMinutes,
            int participantsMaxNumber,
            Guid organizerId)
        {
            Name = name;
            Description = description;
            StartDate = startDate;
            DurationInMinutes = durationInMinutes;
            ParticipantsMaxNumber = participantsMaxNumber;
            OrganizerId = organizerId;
        }
    }
}
