using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Query.Projections;

namespace Ethos.Query.Services
{
    public interface IScheduleQueryService
    {
        /// <summary>
        /// Return all the recurring schedules that overlaps with the provided interval.
        /// </summary>
        Task<List<RecurringScheduleProjection>> GetOverlappingRecurringSchedulesAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Return all the single schedules that overlaps with the provided interval.
        /// </summary>
        Task<List<SingleScheduleProjection>> GetOverlappingSingleSchedulesAsync(DateTime startDate, DateTime endDate);
    }
}
