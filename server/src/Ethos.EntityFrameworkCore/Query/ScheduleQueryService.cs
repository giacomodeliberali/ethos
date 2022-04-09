using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Domain.Common;
using Ethos.Query.Projections;
using Ethos.Query.Services;
using Microsoft.EntityFrameworkCore;

namespace Ethos.EntityFrameworkCore.Query
{
    public class ScheduleQueryService : BaseQueryService, IScheduleQueryService
    {
        public ScheduleQueryService(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {
        }

        /// <inheritdoc />
        public async Task<List<RecurringScheduleProjection>> GetOverlappingRecurringSchedulesAsync(DateOnlyPeriod period, bool fromInclusive = true, bool toInclusive = true)
        {
            var schedules =
                await (from schedule in ApplicationDbContext.Schedules.AsNoTracking()
                join recurringSchedule in ApplicationDbContext.RecurringSchedules.AsNoTracking() on schedule.Id equals recurringSchedule.ScheduleId
                join organizer in ApplicationDbContext.Users.AsNoTracking() on schedule.OrganizerId equals organizer.Id
                where fromInclusive ? recurringSchedule.StartDate <= period.EndDate : recurringSchedule.EndDate < period.EndDate
                where toInclusive ? recurringSchedule.EndDate >= period.StartDate : recurringSchedule.EndDate > period.StartDate
                select new
                {
                    Schedule = schedule,
                    RecurringSchedule = recurringSchedule,
                    Organizer = organizer,
                }).ToListAsync();

            return schedules.Select(item => new RecurringScheduleProjection()
            {
                Id = item.Schedule.Id,
                Name = item.Schedule.Name,
                Description = item.Schedule.Description,
                ParticipantsMaxNumber = item.Schedule.ParticipantsMaxNumber,
                StartDate = item.RecurringSchedule.StartDate.ToDateTime(TimeOnly.MinValue),
                EndDate = item.RecurringSchedule.EndDate.ToDateTime(TimeOnly.MaxValue),
                DurationInMinutes = item.Schedule.DurationInMinutes,
                RecurringExpression = item.RecurringSchedule.RecurringExpression,
                Organizer = new ScheduleProjection.OrganizerProjection()
                {
                    Id = item.Organizer.Id,
                    Email = item.Organizer.Email,
                    FullName = item.Organizer.FullName,
                    UserName = item.Organizer.UserName,
                },
            }).ToList();
        }

        public async Task<List<SingleScheduleProjection>> GetOverlappingSingleSchedulesAsync(DateOnlyPeriod period, bool fromInclusive = true, bool toInclusive = true)
        {
            var schedules =
                await (
                    from schedule in ApplicationDbContext.Schedules.AsNoTracking()
                    join singleSchedule in ApplicationDbContext.SingleSchedules.AsNoTracking() on schedule.Id equals singleSchedule.ScheduleId
                    join organizer in ApplicationDbContext.Users.AsNoTracking() on schedule.OrganizerId equals organizer.Id
                    where fromInclusive ? singleSchedule.StartDate <= period.EndDate.ToDateTime(TimeOnly.MaxValue) : singleSchedule.StartDate < period.EndDate.ToDateTime(TimeOnly.MaxValue)
                    where toInclusive ? singleSchedule.EndDate >= period.StartDate.ToDateTime(TimeOnly.MinValue) : singleSchedule.EndDate > period.StartDate.ToDateTime(TimeOnly.MinValue)
                    select new
                    {
                        Schedule = schedule,
                        SingleSchedule = singleSchedule,
                        Organizer = organizer,
                    }).ToListAsync();

            return schedules.Select(item => new SingleScheduleProjection()
            {
                Id = item.Schedule.Id,
                Name = item.Schedule.Name,
                Description = item.Schedule.Description,
                ParticipantsMaxNumber = item.Schedule.ParticipantsMaxNumber,
                StartDate = item.SingleSchedule.StartDate,
                EndDate = item.SingleSchedule.EndDate,
                DurationInMinutes = item.Schedule.DurationInMinutes,
                Organizer = new ScheduleProjection.OrganizerProjection()
                {
                    Id = item.Organizer.Id,
                    Email = item.Organizer.Email,
                    FullName = item.Organizer.FullName,
                    UserName = item.Organizer.UserName,
                },
            }).ToList();
        }

        /// <inheritdoc />
        public async Task<List<RecurringScheduleProjection>> GetAllRecurringSchedulesAsync()
        {
            var schedules =
                await (from schedule in ApplicationDbContext.Schedules.AsNoTracking()
                    join recurringSchedule in ApplicationDbContext.RecurringSchedules.AsNoTracking() on schedule.Id equals recurringSchedule.ScheduleId
                    join organizer in ApplicationDbContext.Users.AsNoTracking() on schedule.OrganizerId equals organizer.Id
                    select new
                    {
                        Schedule = schedule,
                        RecurringSchedule = recurringSchedule,
                        Organizer = organizer,
                    }).ToListAsync();

            return schedules.Select(item => new RecurringScheduleProjection()
            {
                Id = item.Schedule.Id,
                Name = item.Schedule.Name,
                Description = item.Schedule.Description,
                ParticipantsMaxNumber = item.Schedule.ParticipantsMaxNumber,
                StartDate = item.RecurringSchedule.StartDate.ToDateTime(TimeOnly.MinValue),
                EndDate = item.RecurringSchedule.EndDate.ToDateTime(TimeOnly.MaxValue),
                DurationInMinutes = item.Schedule.DurationInMinutes,
                RecurringExpression = item.RecurringSchedule.RecurringExpression,
                Organizer = new ScheduleProjection.OrganizerProjection()
                {
                    Id = item.Organizer.Id,
                    Email = item.Organizer.Email,
                    FullName = item.Organizer.FullName,
                    UserName = item.Organizer.UserName,
                },
            }).ToList();
        }
    }
}
