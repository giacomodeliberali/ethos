using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Domain.Entities;
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
        public async Task<List<RecurringScheduleProjection>> GetOverlappingRecurringSchedulesAsync(DateTime startDate, DateTime endDate)
        {
            var schedules =
                await (from schedule in ApplicationDbContext.Schedules.AsNoTracking()
                join recurringSchedule in ApplicationDbContext.RecurringSchedules.AsNoTracking() on schedule.Id equals recurringSchedule.ScheduleId
                join organizer in ApplicationDbContext.Users.AsNoTracking() on schedule.OrganizerId equals organizer.Id
                where recurringSchedule.StartDate <= endDate
                where (recurringSchedule.EndDate ?? DateTime.MaxValue) >= startDate
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
                StartDate = item.RecurringSchedule.StartDate,
                EndDate = item.RecurringSchedule.EndDate,
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

        public async Task<List<SingleScheduleProjection>> GetOverlappingSingleSchedulesAsync(DateTime startDate, DateTime endDate)
        {
            var schedules =
                await (
                    from schedule in ApplicationDbContext.Schedules.AsNoTracking()
                    join singleSchedule in ApplicationDbContext.SingleSchedules.AsNoTracking() on schedule.Id equals singleSchedule.ScheduleId
                    join organizer in ApplicationDbContext.Users.AsNoTracking() on schedule.OrganizerId equals organizer.Id
                    where singleSchedule.StartDate <= endDate
                    where singleSchedule.EndDate >= startDate
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
    }
}
