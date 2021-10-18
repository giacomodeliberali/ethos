using System.Collections.Generic;

namespace Ethos.Application.Contracts.Schedule
{
    public class DeleteScheduleReplyDto
    {
        public List<UserDto> AffectedUsers { get; set; }

        public class UserDto
        {
            public string FullName { get; set; }
        }
    }
}
