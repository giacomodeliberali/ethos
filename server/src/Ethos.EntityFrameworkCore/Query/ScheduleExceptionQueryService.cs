using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Domain.Common;
using Ethos.EntityFrameworkCore.Entities;
using Ethos.Query.Projections;
using Ethos.Query.Services;
using Microsoft.EntityFrameworkCore;

namespace Ethos.EntityFrameworkCore.Query
{
    public class ScheduleExceptionQueryService : BaseQueryService, IScheduleExceptionQueryService
    {
        private IQueryable<ScheduleExceptionData> ScheduleExceptions => ApplicationDbContext.ScheduleExceptions.AsNoTracking();

        public ScheduleExceptionQueryService(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {
        }

        public async Task<List<ScheduleExtensionProjection>> GetScheduleExceptionsAsync(Guid recurringScheduleId, DateOnlyPeriod period)
        {
            var exceptions = await ScheduleExceptions
                .Where(e => e.ScheduleId == recurringScheduleId &&
                            e.StartDate >= period.StartDate.ToDateTime(TimeOnly.MinValue) &&
                            e.EndDate <= period.EndDate.ToDateTime(TimeOnly.MaxValue))
                .ToListAsync();

            return exceptions.Select(e => new ScheduleExtensionProjection()
            {
                Id = e.Id,
                ScheduleId = e.ScheduleId,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
            }).ToList();
        }

        public async Task<List<ScheduleExtensionProjection>> GetScheduleExceptionsAsync(DateOnlyPeriod period)
        {
            var exceptions = await ScheduleExceptions
                .Where(e => e.StartDate >= period.StartDate.ToDateTime(TimeOnly.MinValue) &&
                            e.EndDate <= period.EndDate.ToDateTime(TimeOnly.MaxValue))
                .ToListAsync();

            return exceptions.Select(e => new ScheduleExtensionProjection()
            {
                Id = e.Id,
                ScheduleId = e.ScheduleId,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
            }).ToList();
        }

        public async Task<List<ScheduleExtensionProjection>> GetScheduleExceptionsAsync(Guid recurringScheduleId)
        {
            var exceptions = await ScheduleExceptions
                .Where(e => e.ScheduleId == recurringScheduleId)
                .ToListAsync();

            return exceptions.Select(e => new ScheduleExtensionProjection()
            {
                Id = e.Id,
                ScheduleId = e.ScheduleId,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
            }).ToList();
        }
    }
}
