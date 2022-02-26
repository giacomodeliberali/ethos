using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Domain.Common;
using Ethos.Query.Projections;

namespace Ethos.Query.Services
{
    public interface IScheduleQueryService
    {
        /// <summary>
        /// Return all the recurring schedules that overlaps with the provided interval.
        /// </summary>
        Task<List<RecurringScheduleProjection>> GetOverlappingRecurringSchedulesAsync(Period period, bool fromInclusive = true, bool toInclusive = true);

        /// <summary>
        /// Return all the single schedules that overlaps with the provided interval.
        /// </summary>
        Task<List<SingleScheduleProjection>> GetOverlappingSingleSchedulesAsync(Period period, bool fromInclusive = true, bool toInclusive = true);

        /// <summary>
        /// Returns the list of all recurring schedules.
        /// </summary>
        Task<List<RecurringScheduleProjection>> GetAllRecurringSchedulesAsync();
    }
}
