using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.EntityFrameworkCore.Entities;
using Ethos.Query.Projections;
using Ethos.Query.Services;
using Microsoft.EntityFrameworkCore;

namespace Ethos.EntityFrameworkCore.Query
{
    public class ScheduleExceptionQueryService : BaseQueryService, IScheduleExceptionQueryService
    {
        private IQueryable<ScheduleExceptionData> ScheduleExceptions => ApplicationDbContext.ScheduleExceptions.AsNoTracking();

        public ScheduleExceptionQueryService(
            ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {
        }

        public async Task<List<ScheduleExceptionProjection>> GetScheduleExceptionsAsync(Guid recurringScheduleId, DateOnlyPeriod period)
        {
            var exceptions = await ScheduleExceptions
                .Where(e => e.ScheduleId == recurringScheduleId &&
                            e.Date >= period.StartDate &&
                            e.Date <= period.EndDate)
                .ToListAsync();

            return exceptions.Select(e => new ScheduleExceptionProjection()
            {
                Id = e.Id,
                ScheduleId = e.ScheduleId,
                Date = new DateOnly(e.Date.Year, e.Date.Month, e.Date.Day),
            }).ToList();
        }

        public async Task<List<ScheduleExceptionProjection>> GetScheduleExceptionsAsync(Guid recurringScheduleId)
        {
            var exceptions = await ScheduleExceptions
                .Where(e => e.ScheduleId == recurringScheduleId)
                .ToListAsync();

            return exceptions.Select(e => new ScheduleExceptionProjection()
            {
                Id = e.Id,
                ScheduleId = e.ScheduleId,
                Date = new DateOnly(e.Date.Year, e.Date.Month, e.Date.Day),
            }).ToList();
        }
    }
}
