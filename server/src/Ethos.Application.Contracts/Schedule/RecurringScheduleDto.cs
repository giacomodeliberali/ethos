using System;
using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Schedule
{
    public class RecurringScheduleDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public UserDto Organizer { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public double DurationInMinutes { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int ParticipantsMaxNumber { get; set; }

        [Required]
        public string RecurringCronExpression { get; set; }

        public class UserDto
        {
            [Required]
            public Guid Id { get; set; }

            [Required]
            public string Email { get; set; }

            [Required]
            public string UserName { get; set; }

            [Required]
            public string FullName { get; set; }
        }
    }
}
