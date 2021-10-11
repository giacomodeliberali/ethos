using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<List<ScheduleExtensionProjection>> GetScheduleExceptionsAsync(Guid recurringScheduleId, DateTime startDate, DateTime endDate)
        {
            var exceptions = await ScheduleExceptions
                .Where(e => e.ScheduleId == recurringScheduleId && e.StartDate >= startDate && e.EndDate <= endDate)
                .ToListAsync();

            return exceptions.Select(e => new ScheduleExtensionProjection()
            {
                Id = e.Id,
                ScheduleId = e.ScheduleId,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
            }).ToList();
        }

        public async Task<List<ScheduleExtensionProjection>> GetScheduleExceptionsAsync(DateTime startDate, DateTime endDate)
        {
            var exceptions = await ScheduleExceptions
                .Where(e => e.StartDate >= startDate && e.EndDate <= endDate)
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
