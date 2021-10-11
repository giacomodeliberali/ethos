using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Query.Projections;

namespace Ethos.Query.Services
{
    public interface IScheduleExceptionQueryService
    {
        Task<List<ScheduleExtensionProjection>> GetScheduleExceptionsAsync(Guid recurringScheduleId, DateTime startDate, DateTime endDate);
        Task<List<ScheduleExtensionProjection>> GetScheduleExceptionsAsync(DateTime startDate, DateTime endDate);
    }
}
