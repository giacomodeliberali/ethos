using System;
using System.ComponentModel.DataAnnotations;
using Ethos.Application.Contracts.Identity;

namespace Ethos.Application.Contracts.Schedule
{
    public class GeneratedScheduleDto
    {
        [Required]
        public Guid ScheduleId { get; set; }

        [Required]
        public UserDto Organizer { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
