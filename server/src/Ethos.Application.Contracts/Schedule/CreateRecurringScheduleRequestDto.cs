using System;
using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Schedule
{
    public class CreateRecurringScheduleRequestDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public int DurationInMinutes { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string RecurringCronExpression { get; set; }

        [Required]
        public int ParticipantsMaxNumber { get; set; }

        [Required]
        public Guid OrganizerId { get; set; }

        [Required]
        public string TimeZone { get; set; }
    }
}
