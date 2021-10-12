using System;
using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Schedule
{
    public class UpdateScheduleRequestDto
    {
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Required only if the schedule is recurring.
        /// </summary>
        public DateTime? InstanceStartDate { get; set; }

        /// <summary>
        /// Required only if the schedule is recurring.
        /// </summary>
        public DateTime? InstanceEndDate { get; set; }

        [Required]
        public ScheduleDto Schedule { get; set; }

        /// <summary>
        /// Required only if the schedule is recurring.
        /// </summary>
        public RecurringScheduleOperationType? RecurringScheduleOperationType { get; set; }

        public class ScheduleDto
        {
            [Required]
            public string Name { get; set; }

            [Required]
            public string Description { get; set; }

            [Required]
            public DateTime StartDate { get; set; }

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
}
