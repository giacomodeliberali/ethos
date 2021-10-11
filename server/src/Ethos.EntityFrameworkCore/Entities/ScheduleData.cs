using System;

namespace Ethos.EntityFrameworkCore.Entities
{
    public class ScheduleData
    {
        public Guid Id { get; set; }

        public Guid OrganizerId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int DurationInMinutes { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string RecurringExpression { get; set; }

        public int ParticipantsMaxNumber { get; set; }
    }
}
