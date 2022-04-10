using System;
using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Schedule
{
    public class UpdateSingleScheduleRequestDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTimeOffset StartDate { get; set; }

        [Required]
        public int DurationInMinutes { get; set; }

        [Required]
        public int ParticipantsMaxNumber { get; set; }

        [Required]
        public Guid OrganizerId { get; set; }
    }
}
