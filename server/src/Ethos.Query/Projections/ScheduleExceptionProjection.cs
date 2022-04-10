using System;

namespace Ethos.Query.Projections
{
    public class ScheduleExceptionProjection : IProjection
    {
        public Guid Id { get; set; }

        public Guid ScheduleId { get; set; }

        public DateOnly Date { get; set; }
    }
}
