using System;

namespace Ethos.EntityFrameworkCore.Entities
{
    public class RecurringScheduleData
    {
        public Guid ScheduleId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string RecurringExpression { get; set; }
    }
}
