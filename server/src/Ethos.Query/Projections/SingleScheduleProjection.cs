using System;

namespace Ethos.Query.Projections
{
    public class SingleScheduleProjection : ScheduleProjection
    {
        public DateTimeOffset  StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }
    }
}
