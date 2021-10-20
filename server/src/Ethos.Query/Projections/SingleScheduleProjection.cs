using System;

namespace Ethos.Query.Projections
{
    public class SingleScheduleProjection : ScheduleProjection
    {
        public DateTime  StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
