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

        public string OrganizerEmail { get; set; }

        public string OrganizerUserName { get; set; }

        public DateTime  StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int DurationInMinutes { get; set; }

        public string RecurringExpression { get; set; }
    }
}
