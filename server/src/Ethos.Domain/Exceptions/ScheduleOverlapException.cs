using System;

namespace Ethos.Domain.Exceptions
{
    public class ScheduleOverlapException : BusinessException
    {
        public ScheduleOverlapException(DateTime startOverlappingDate, DateTime endOverlappingDate)
            : base($"The schedules overlaps with another! ({startOverlappingDate:U} - {endOverlappingDate:U})")
        {
        }
    }
}
