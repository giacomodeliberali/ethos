using System;
using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Schedule
{
    public class DeleteRecurringScheduleRequestDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public RecurringScheduleOperationType RecurringScheduleOperationType { get; set; }

        [Required]
        public DateTime InstanceStartDate { get; set; }

        [Required]
        public DateTime InstanceEndDate { get; set; }
    }
}
