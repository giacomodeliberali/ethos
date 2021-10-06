using System;
using System.Threading.Tasks;
using Ethos.Domain.Entities;
using Ethos.Domain.Repositories;
using Ethos.EntityFrameworkCore.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ethos.EntityFrameworkCore.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public ScheduleRepository(
            ApplicationDbContext applicationDbContext,
            UserManager<ApplicationUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        public async Task<Guid> CreateAsync(Schedule schedule)
        {
            var scheduleData = new ScheduleData
            {
                Id = schedule.Id,
                OrganizerId = schedule.Organizer.Id,
                StartDate = schedule.StartDate,
                EndDate = schedule.EndDate,
                RecurringExpression = schedule.RecurringCronExpressionString,
                Name = schedule.Name,
                Description = schedule.Description,
                Duration = schedule.Duration,
            };

            await _applicationDbContext.Schedules.AddAsync(scheduleData);

            return scheduleData.Id;
        }

        public Task DeleteAsync(Schedule schedule)
        {
            throw new NotImplementedException();
        }

        public async Task<Schedule> GetByIdAsync(Guid id)
        {
            var scheduleData = await _applicationDbContext.Schedules.SingleAsync(s => s.Id == id);

            var organizer = await _userManager.FindByIdAsync(scheduleData.OrganizerId.ToString());

            return Schedule.Factory.FromSnapshot(
                organizer,
                scheduleData.StartDate,
                scheduleData.EndDate,
                scheduleData.RecurringExpression,
                scheduleData.Duration,
                scheduleData.Name,
                scheduleData.Description);
        }
    }
}
