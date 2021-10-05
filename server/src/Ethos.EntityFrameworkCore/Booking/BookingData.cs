using System;

namespace Ethos.EntityFrameworkCore.Booking
{
    public class BookingData
    {
        public Guid Id { get; set; }

        public Guid ScheduleId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid UserId { get; set; }
    }
}
