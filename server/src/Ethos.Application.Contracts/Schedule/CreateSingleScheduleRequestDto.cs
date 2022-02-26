using System;
using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Schedule
{
    public class CreateSingleScheduleRequestDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public int DurationInMinutes { get; set; }

        /// <summary>
        /// Defaults to zero if no limit is required.
        /// </summary>
        [Required]
        public int ParticipantsMaxNumber { get; set; }

        /// <summary>
        /// The id of the organizer of this schedule.
        /// </summary>
        [Required]
        public Guid OrganizerId { get; set; }
    }
}