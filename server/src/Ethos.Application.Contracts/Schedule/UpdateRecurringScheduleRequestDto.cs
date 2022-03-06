using System;
using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Schedule
{
    public class UpdateRecurringScheduleRequestDto
    {
        [Required]
        public Guid Id { get; init; }

        [Required]
        public string Name { get; init; }

        [Required]
        public string Description { get; init; }

        [Required]
        public DateTime InstanceStartDate { get; init; }

        [Required]
        public DateTime InstanceEndDate { get; init; }

        [Required]
        public DateTime StartDate { get; init; }
        
        public DateTime EndDate { get; init; }

        [Required]
        public int DurationInMinutes { get; init; }

        [Required]
        public int ParticipantsMaxNumber { get; init; }

        [Required]
        public string RecurringCronExpression { get; set; }

        [Required]
        public Guid OrganizerId { get; init; }
        
        [Required]
        public RecurringScheduleOperationType RecurringScheduleOperationType { get; set; }
    }
}
