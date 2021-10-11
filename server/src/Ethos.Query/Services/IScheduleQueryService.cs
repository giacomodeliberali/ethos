using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Query.Projections;

namespace Ethos.Query.Services
{
    public interface IScheduleQueryService
    {
        /// <summary>
        /// Return all the schedules that overlaps with the provided interval.
        /// </summary>
        Task<List<ScheduleProjection>> GetOverlappingSchedulesAsync(DateTime startDate, DateTime endDate);
    }
}
