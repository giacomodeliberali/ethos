using System;
using System.ComponentModel.DataAnnotations;

namespace Ethos.EntityFrameworkCore.Schedule
{
    public class ScheduleData
    {
        public Guid Id { get; set; }

        public Guid OrganizerId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public TimeSpan Duration { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string RecurringExpression { get; set; }
    }
}
