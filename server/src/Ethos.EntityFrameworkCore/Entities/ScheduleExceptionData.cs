using System;

namespace Ethos.EntityFrameworkCore.Entities
{
    public class ScheduleExceptionData
    {
        public Guid Id { get; set; }

        public Guid ScheduleId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
