using System;

namespace Ethos.EntityFrameworkCore.Entities
{
    public class RecurringScheduleData
    {
        public Guid ScheduleId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string RecurringExpression { get; set; }
    }
}
