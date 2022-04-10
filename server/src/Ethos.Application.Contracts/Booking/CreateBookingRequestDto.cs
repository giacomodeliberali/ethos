using System;
using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Booking
{
    public class CreateBookingRequestDto
    {
        [Required]
        public Guid ScheduleId { get; set; }

        [Required]
        public DateTimeOffset StartDate { get; set; }

        [Required]
        public DateTimeOffset EndDate { get; set; }
    }
}
