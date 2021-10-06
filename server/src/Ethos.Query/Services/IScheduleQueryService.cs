using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Domain.Entities;
using Ethos.Query.Projections;

namespace Ethos.Query
{
    public interface IScheduleQueryService
    {
        Task<IEnumerable<ScheduleProjection>> GetInRangeAsync(DateTime startDate, DateTime endDate);
    }
}
