using System;
using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Schedule
{
    public class DeleteScheduleRequestDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Required only if the schedule is recurring.
        /// </summary>
        public RecurringScheduleOperationType? RecurringScheduleOperationType { get; set; }

        [Required]
        public DateTime InstanceStartDate { get; set; }

        [Required]
        public DateTime InstanceEndDate { get; set; }
    }
}
