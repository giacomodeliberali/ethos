using System;

namespace Ethos.Query.Projections
{
    public class RecurringScheduleProjection : ScheduleProjection
    {
        public DateTime  StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string RecurringExpression { get; set; }
    }
}
