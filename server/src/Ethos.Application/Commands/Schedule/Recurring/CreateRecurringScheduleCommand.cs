using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Ethos.Application.Commands.Schedule.Recurring
{
    public class CreateRecurringScheduleCommand : IRequest<Guid>
    {
        public string Name { get; }

        public string Description { get; }

        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        [Required]
        public int DurationInMinutes { get; }

        /// <summary>
        /// A CRON expression to indicate this schedule is recurring.
        /// </summary>
        public string RecurringCronExpression { get; }

        /// <summary>
        /// Defaults to zero if no limit is required.
        /// </summary>
        public int ParticipantsMaxNumber { get; }

        /// <summary>
        /// The id of the organizer of this schedule.
        /// </summary>
        public Guid OrganizerId { get; }

        public CreateRecurringScheduleCommand(
            string name,
            string description,
            DateTime startDate,
            DateTime endDate,
            int durationInMinutes,
            string recurringCronExpression,
            int participantsMaxNumber,
            Guid organizerId)
        {
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            DurationInMinutes = durationInMinutes;
            RecurringCronExpression = recurringCronExpression;
            ParticipantsMaxNumber = participantsMaxNumber;
            OrganizerId = organizerId;
        }
    }
}