using System;

namespace Ethos.Application.Contracts.Booking
{
    public class BookingDto
    {
        public Guid Id { get; set; }

        public ScheduleDto Schedule { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public UserDto User { get; set; }

        public class ScheduleDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public string OrganizerFullName { get; set; }

            public int DurationInMinutes { get; set; }
        }

        public class UserDto
        {
            public Guid Id { get; set; }

            public string FullName { get; set; }
        }
    }
}
