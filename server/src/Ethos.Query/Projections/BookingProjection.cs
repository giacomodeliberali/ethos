using System;

namespace Ethos.Query.Projections
{
    public class BookingProjection : IProjection
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string UserFullName { get; set; }

        public string UserEmail { get; set; }

        public string UserName { get; set; }

        public Guid ScheduleId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
