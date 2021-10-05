using System;
using Ethos.Application.Contracts.Identity;

namespace Ethos.Application.Contracts.Schedule
{
    public class GeneratedScheduleDto
    {
        public UserDto Organizer { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
