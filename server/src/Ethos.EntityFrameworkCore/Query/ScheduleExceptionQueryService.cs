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
        
        private IQueryable<ScheduleData> Schedules => ApplicationDbContext.Schedules.AsNoTracking();

        public ScheduleExceptionQueryService(
            ApplicationDbContext applicationDbContext)
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

            var schedule = await Schedules.FirstAsync(s => s.Id == recurringScheduleId);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(schedule.TimeZone);

            // TODO: aggiungere timezone in tutti i writemodel dove ci sono datetime in modo da evitare di dover ogni volta
            // leggere lo Schedule padre. In ogni projection assicurarsi che sia ritornato il dattime con l'offset corretto
            // es. bookings
            
            return exceptions.Select(e => new ScheduleExtensionProjection()
            {
                Id = e.Id,
                ScheduleId = e.ScheduleId,
                StartDate = e.StartDate.ToDateTimeOffset(timeZone),
                EndDate = e.EndDate.ToDateTimeOffset(timeZone),
            }).ToList();
        }

        public async Task<List<ScheduleExtensionProjection>> GetScheduleExceptionsAsync(Guid recurringScheduleId)
        {
            var exceptions = await ScheduleExceptions
                .Where(e => e.ScheduleId == recurringScheduleId)
                .ToListAsync();

            var schedule = await Schedules.FirstAsync(s => s.Id == recurringScheduleId);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(schedule.TimeZone);

            return exceptions.Select(e => new ScheduleExtensionProjection()
            {
                Id = e.Id,
                ScheduleId = e.ScheduleId,
                StartDate = e.StartDate.ToDateTimeOffset(timeZone),
                EndDate = e.EndDate.ToDateTimeOffset(timeZone),
            }).ToList();
        }
    }
}
