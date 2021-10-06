using System;

namespace Ethos.Query.Projections
{
    public class ScheduleProjection : IProjection
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid OrganizerId { get; set; }

        public string OrganizerFullName { get; set; }

        public DateTime  StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public TimeSpan Duration { get; set; }

        public string RecurringExpression { get; set; }
    }
}
