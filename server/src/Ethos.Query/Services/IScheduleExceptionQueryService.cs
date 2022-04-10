using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Domain.Common;
using Ethos.Query.Projections;

namespace Ethos.Query.Services
{
    public interface IScheduleExceptionQueryService
    {
        Task<List<ScheduleExceptionProjection>> GetScheduleExceptionsAsync(Guid recurringScheduleId, DateOnlyPeriod period);
        Task<List<ScheduleExceptionProjection>> GetScheduleExceptionsAsync(Guid recurringScheduleId);
    }
}
