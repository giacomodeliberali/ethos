using System;

namespace Ethos.EntityFrameworkCore.Entities
{
    public class SingleScheduleData
    {
        public Guid ScheduleId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
