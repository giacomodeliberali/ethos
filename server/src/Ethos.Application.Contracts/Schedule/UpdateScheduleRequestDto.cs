using System;
using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Schedule
{
    public class UpdateScheduleRequestDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        /// <summary>
        /// If not recurring this must be EndDate - StartDate.
        /// If recurring it represent the duration of the schedule.
        /// </summary>
        [Required]
        public int DurationInMinutes { get; set; }

        public string RecurringCronExpression { get; set; }

        [Required]
        public int ParticipantsMaxNumber { get; set; }

        [Required]
        public Guid OrganizerId { get; set; }
    }
}
