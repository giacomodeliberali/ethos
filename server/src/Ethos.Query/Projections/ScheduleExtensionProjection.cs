using System;

namespace Ethos.Query.Projections
{
    public class ScheduleExtensionProjection : IProjection
    {
        public Guid Id { get; set; }

        public Guid ScheduleId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
