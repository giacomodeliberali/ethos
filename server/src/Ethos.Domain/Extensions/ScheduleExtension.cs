using Ethos.Domain.Entities;

namespace Ethos.Domain.Extensions
{
    public static class ScheduleExtension
    {
        public static bool IsRecurring(this Schedule schedule)
        {
            return schedule is RecurringSchedule;
        }
    }
}
