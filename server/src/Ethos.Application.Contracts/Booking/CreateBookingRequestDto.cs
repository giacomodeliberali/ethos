using System;
using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Booking
{
    public class CreateBookingRequestDto
    {
        [Required]
        public Guid ScheduleId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
