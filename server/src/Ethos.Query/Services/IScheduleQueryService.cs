using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Query.Projections;

namespace Ethos.Query.Services
{
    public interface IScheduleQueryService
    {
        Task<IEnumerable<ScheduleProjection>> GetInRangeAsync(DateTime startDate, DateTime endDate);
    }
}
