using System;

namespace Ethos.Query.Projections
{
    public abstract class ScheduleProjection : IProjection
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int ParticipantsMaxNumber { get; set; }

        public int DurationInMinutes { get; set; }

        public OrganizerProjection Organizer { get; set; }

        public class OrganizerProjection
        {
            public Guid Id { get; set; }

            public string Email { get; set; }

            public string FullName { get; set; }

            public string UserName { get; set; }
        }
    }
}
