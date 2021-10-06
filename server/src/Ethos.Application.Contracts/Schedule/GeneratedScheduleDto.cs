using System;
using System.Collections.Generic;
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

        [Required]
        public IEnumerable<BookingDto> Bookings { get; set; }

        public class BookingDto
        {
            [Required]
            public Guid Id { get; set; }

            [Required]
            public string UserFullName { get; set; }
        }
    }
}
