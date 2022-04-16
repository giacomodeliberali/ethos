using System;

namespace Ethos.Query.Projections
{
    public class BookingProjection : IProjection
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string UserFullName { get; set; }

        public string UserEmail { get; set; }

        public string UserName { get; set; }

        public Guid ScheduleId { get; set; }

        public string ScheduleName { get; set; }

        public string ScheduleDescription { get; set; }

        public int ScheduleDurationInMinutes { get; set; }

        public string ScheduleOrganizerFullName { get; set; }

        public int ParticipantsMaxNumber { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }
    }
}
