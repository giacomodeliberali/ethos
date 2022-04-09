using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Repositories;
using Ethos.EntityFrameworkCore.Entities;
using Ethos.Query.Projections;
using Ethos.Query.Services;
using Microsoft.EntityFrameworkCore;

namespace Ethos.EntityFrameworkCore.Query
{
    public class ScheduleExceptionQueryService : BaseQueryService, IScheduleExceptionQueryService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private IQueryable<ScheduleExceptionData> ScheduleExceptions => ApplicationDbContext.ScheduleExceptions.AsNoTracking();

        public ScheduleExceptionQueryService(
            ApplicationDbContext applicationDbContext,
            IScheduleRepository scheduleRepository)
            : base(applicationDbContext)
        {
            _scheduleRepository = scheduleRepository;
        }

        public async Task<List<ScheduleExtensionProjection>> GetScheduleExceptionsAsync(Guid recurringScheduleId, DateOnlyPeriod period)
        {
            var exceptions = await ScheduleExceptions
                .Where(e => e.ScheduleId == recurringScheduleId &&
                            e.StartDate >= period.StartDate.ToDateTime(TimeOnly.MinValue) &&
                            e.EndDate <= period.EndDate.ToDateTime(TimeOnly.MaxValue))
                .ToListAsync();

            var schedule = (RecurringSchedule)(await _scheduleRepository.GetByIdAsync(recurringScheduleId));

            return exceptions.Select(e => new ScheduleExtensionProjection()
            {
                Id = e.Id,
                ScheduleId = e.ScheduleId,
                StartDate = e.StartDate.ToDateTimeOffset(schedule.TimeZone),
                EndDate = e.EndDate.ToDateTimeOffset(schedule.TimeZone),
            }).ToList();
        }

        public async Task<List<ScheduleExtensionProjection>> GetScheduleExceptionsAsync(Guid recurringScheduleId)
        {
            var exceptions = await ScheduleExceptions
                .Where(e => e.ScheduleId == recurringScheduleId)
                .ToListAsync();

            var schedule = (RecurringSchedule)(await _scheduleRepository.GetByIdAsync(recurringScheduleId));
            
            return exceptions.Select(e => new ScheduleExtensionProjection()
            {
                Id = e.Id,
                ScheduleId = e.ScheduleId,
                StartDate = e.StartDate.ToDateTimeOffset(schedule.TimeZone),
                EndDate = e.EndDate.ToDateTimeOffset(schedule.TimeZone),
            }).ToList();
        }
    }
}
