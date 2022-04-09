using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Domain.Common;
using Ethos.Query.Projections;

namespace Ethos.Query.Services
{
    public interface IScheduleExceptionQueryService
    {
        Task<List<ScheduleExtensionProjection>> GetScheduleExceptionsAsync(Guid recurringScheduleId, DateOnlyPeriod period);
        Task<List<ScheduleExtensionProjection>> GetScheduleExceptionsAsync(DateOnlyPeriod period);
        Task<List<ScheduleExtensionProjection>> GetScheduleExceptionsAsync(Guid recurringScheduleId);
    }
}
