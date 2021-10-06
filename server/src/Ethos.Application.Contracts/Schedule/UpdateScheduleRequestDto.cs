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

        public int DurationInMinutes { get; set; }

        public string RecurringCronExpression { get; set; }
    }
}
