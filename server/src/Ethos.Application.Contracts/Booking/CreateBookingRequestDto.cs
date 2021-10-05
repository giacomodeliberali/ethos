using System;

namespace Ethos.Application.Contracts.Booking
{
    public class CreateBookingRequestDto
    {
        public Guid ScheduleId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
