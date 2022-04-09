using System;

namespace Ethos.Query.Projections
{
    public class ScheduleExtensionProjection : IProjection
    {
        public Guid Id { get; set; }

        public Guid ScheduleId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }
    }
}
