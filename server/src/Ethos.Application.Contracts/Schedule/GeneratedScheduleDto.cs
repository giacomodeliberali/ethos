using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Required]
        public IEnumerable<BookingDto> Bookings { get; set; }

        [Required]
        public int ParticipantsMaxNumber { get; set; }

        public class BookingDto
        {
            [Required]
            public Guid Id { get; set; }

            /// <summary>
            /// Populated only if the caller is admin.
            /// </summary>
            public UserDto User { get; set; }
        }

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
