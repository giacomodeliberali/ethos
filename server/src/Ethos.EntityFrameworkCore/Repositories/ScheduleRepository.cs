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
            var scheduleData = new ScheduleData()
            {
                Id = schedule.Id,
                Name = schedule.Name,
                Description = schedule.Description,
                OrganizerId = schedule.Organizer.Id,
                DurationInMinutes = schedule.DurationInMinutes,
                ParticipantsMaxNumber = schedule.ParticipantsMaxNumber,
            };

            await _applicationDbContext.Schedules.AddAsync(scheduleData);

            if (schedule is RecurringSchedule recurringSchedule)
            {
                var recurringScheduleData = new RecurringScheduleData()
                {
                    ScheduleId = schedule.Id,
                    StartDate = recurringSchedule.StartDate,
                    EndDate = recurringSchedule.EndDate,
                    RecurringExpression = recurringSchedule.RecurringCronExpressionString,
                };

                await _applicationDbContext.RecurringSchedules.AddAsync(recurringScheduleData);
            }
            else if (schedule is SingleSchedule singleSchedule)
            {
                var singleScheduleData = new SingleScheduleData()
                {
                    ScheduleId = schedule.Id,
                    StartDate = singleSchedule.StartDate,
                    EndDate = singleSchedule.EndDate,
                };

                await _applicationDbContext.SingleSchedules.AddAsync(singleScheduleData);
            }
            else
            {
                throw new ArgumentException("Invalid schedule type");
            }

            return schedule.Id;
        }

        public async Task DeleteAsync(Schedule schedule)
        {
            var scheduleData = await _applicationDbContext.Schedules.SingleAsync(s => s.Id == schedule.Id);

            var singleScheduleData = await _applicationDbContext.SingleSchedules.SingleOrDefaultAsync(s => s.ScheduleId == schedule.Id);
            if (singleScheduleData != null)
            {
                _applicationDbContext.SingleSchedules.Remove(singleScheduleData);
            }

            var recurringScheduleData = await _applicationDbContext.RecurringSchedules.SingleOrDefaultAsync(s => s.ScheduleId == schedule.Id);
            if (recurringScheduleData != null)
            {
                _applicationDbContext.RecurringSchedules.Remove(recurringScheduleData);
            }

            _applicationDbContext.Schedules.Remove(scheduleData);
        }

        public async Task<Schedule> GetByIdAsync(Guid id)
        {
            var scheduleData = await _applicationDbContext.Schedules.SingleAsync(s => s.Id == id);
            var organizer = await _userManager.FindByIdAsync(scheduleData.OrganizerId.ToString());

            var singleScheduleData = await _applicationDbContext.SingleSchedules.SingleOrDefaultAsync(s => s.ScheduleId == id);

            if (singleScheduleData != null)
            {
                return SingleSchedule.Factory.FromSnapshot(
                    scheduleData.Id,
                    organizer,
                    singleScheduleData.StartDate,
                    singleScheduleData.EndDate,
                    scheduleData.DurationInMinutes,
                    scheduleData.Name,
                    scheduleData.Description,
                    scheduleData.ParticipantsMaxNumber);
            }

            var recurringScheduleData = await _applicationDbContext.RecurringSchedules.SingleOrDefaultAsync(s => s.ScheduleId == id);
            if (recurringScheduleData != null)
            {
                return RecurringSchedule.Factory.FromSnapshot(
                    scheduleData.Id,
                    organizer,
                    recurringScheduleData.StartDate,
                    recurringScheduleData.EndDate,
                    recurringScheduleData.RecurringExpression,
                    scheduleData.DurationInMinutes,
                    scheduleData.Name,
                    scheduleData.Description,
                    scheduleData.ParticipantsMaxNumber);
            }

            throw new ArgumentException("Invalid schedule type");
        }

        public async Task UpdateAsync(Schedule schedule)
        {
            var scheduleData = await _applicationDbContext.Schedules.SingleAsync(s => s.Id == schedule.Id);
            scheduleData.Name = schedule.Name;
            scheduleData.Description = schedule.Description;
            scheduleData.DurationInMinutes = schedule.DurationInMinutes;
            scheduleData.ParticipantsMaxNumber = schedule.ParticipantsMaxNumber;
            scheduleData.OrganizerId = schedule.Organizer.Id;

            if (schedule is SingleSchedule singleSchedule)
            {
                var singleScheduleData = await _applicationDbContext.SingleSchedules.SingleOrDefaultAsync(s => s.ScheduleId == schedule.Id);
                singleScheduleData.StartDate = singleSchedule.StartDate;
                singleScheduleData.EndDate = singleSchedule.EndDate;
            }
            else if (schedule is RecurringSchedule recurringSchedule)
            {
                var recurringScheduleData = await _applicationDbContext.RecurringSchedules.SingleOrDefaultAsync(s => s.ScheduleId == schedule.Id);
                recurringScheduleData.StartDate = recurringSchedule.StartDate;
                recurringScheduleData.EndDate = recurringSchedule.EndDate;
                recurringScheduleData.RecurringExpression = recurringSchedule.RecurringCronExpressionString;
            }
            else
            {
                throw new ArgumentException("Invalid schedule type");
            }
        }
    }
}
