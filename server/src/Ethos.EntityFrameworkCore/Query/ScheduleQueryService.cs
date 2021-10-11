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

        public async Task<IEnumerable<ScheduleProjection>> GetInRangeAsync(DateTime startDate, DateTime endDate)
        {
            var schedules =
                await (from schedule in ApplicationDbContext.Schedules.AsNoTracking()
                join user in ApplicationDbContext.Users.AsNoTracking() on schedule.OrganizerId equals user.Id
                where schedule.StartDate <= startDate
                where !schedule.EndDate.HasValue || schedule.EndDate >= endDate
                select new
                {
                    Schedule = schedule,
                    User = user,
                }).ToListAsync();

            return schedules.Select(item => new ScheduleProjection()
            {
                Id = item.Schedule.Id,
                Name = item.Schedule.Name,
                Description = item.Schedule.Description,
                ParticipantsMaxNumber = item.Schedule.ParticipantsMaxNumber,
                StartDate = item.Schedule.StartDate,
                EndDate = item.Schedule.EndDate,
                DurationInMinutes = item.Schedule.DurationInMinutes,
                RecurringExpression = item.Schedule.RecurringExpression,
                OrganizerId = item.User.Id,
                OrganizerFullName = item.User.FullName,
                OrganizerEmail = item.User.Email,
                OrganizerUserName = item.User.UserName,
            });
        }
    }
}
