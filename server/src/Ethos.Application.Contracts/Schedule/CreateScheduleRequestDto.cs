using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ethos.Application.Contracts.Schedule
{
    public class CreateScheduleRequestDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Populated only if recurring.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// If recurring it represent the duration of the schedule.
        /// </summary>
        public int DurationInMinutes { get; set; }

        /// <summary>
        /// A CRON expression to indicate this schedule is recurring.
        /// </summary>
        public string RecurringCronExpression { get; set; }

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
