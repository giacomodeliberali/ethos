using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Domain.Common;
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

        public async Task<Guid> CreateAsync(SingleSchedule schedule)
        {
            var scheduleData = new ScheduleData()
            {
                Id = schedule.Id,
                Name = schedule.Name,
                Description = schedule.Description,
                OrganizerId = schedule.Organizer.Id,
                DurationInMinutes = schedule.DurationInMinutes,
                ParticipantsMaxNumber = schedule.ParticipantsMaxNumber,
                TimeZone = schedule.TimeZone.Id,
            };

            await _applicationDbContext.Schedules.AddAsync(scheduleData);

            var singleScheduleData = new SingleScheduleData()
            {
                ScheduleId = schedule.Id,
                StartDate = schedule.StartDate.DateTime,
                EndDate = schedule.EndDate.DateTime,
            };

            await _applicationDbContext.SingleSchedules.AddAsync(singleScheduleData);

            return schedule.Id;
        }

        public async Task<Guid> CreateAsync(RecurringSchedule schedule)
        {
            var scheduleData = new ScheduleData()
            {
                Id = schedule.Id,
                Name = schedule.Name,
                Description = schedule.Description,
                OrganizerId = schedule.Organizer.Id,
                DurationInMinutes = schedule.DurationInMinutes,
                ParticipantsMaxNumber = schedule.ParticipantsMaxNumber,
                TimeZone = schedule.TimeZone.Id,
            };

            await _applicationDbContext.Schedules.AddAsync(scheduleData);

            var recurringScheduleData = new RecurringScheduleData()
            {
                ScheduleId = schedule.Id,
                StartDate = schedule.Period.StartDate,
                EndDate = schedule.Period.EndDate,
                RecurringExpression = schedule.RecurringCronExpressionString,
            };

            await _applicationDbContext.RecurringSchedules.AddAsync(recurringScheduleData);

            return schedule.Id;
        }

        public async Task DeleteAsync(SingleSchedule schedule)
        {
            var scheduleData = await _applicationDbContext.Schedules.SingleAsync(s => s.Id == schedule.Id);
            var singleScheduleData = await _applicationDbContext.SingleSchedules.SingleAsync(s => s.ScheduleId == schedule.Id);

            _applicationDbContext.SingleSchedules.Remove(singleScheduleData);
            _applicationDbContext.Schedules.Remove(scheduleData);
        }

        public async Task DeleteAsync(RecurringSchedule schedule)
        {
            var scheduleData = await _applicationDbContext.Schedules.SingleAsync(s => s.Id == schedule.Id);
            var recurringScheduleData = await _applicationDbContext.RecurringSchedules.SingleAsync(s => s.ScheduleId == schedule.Id);

            _applicationDbContext.RecurringSchedules.Remove(recurringScheduleData);
            _applicationDbContext.Schedules.Remove(scheduleData);
        }

        public async Task<Schedule> GetByIdAsync(Guid id)
        {
            var scheduleData = await _applicationDbContext.Schedules.SingleAsync(s => s.Id == id);
            var organizer = await _userManager.FindByIdAsync(scheduleData.OrganizerId.ToString());

            var singleScheduleData = await _applicationDbContext.SingleSchedules.SingleOrDefaultAsync(s => s.ScheduleId == id);

            if (singleScheduleData != null)
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(scheduleData.TimeZone);
                return SingleSchedule.Factory.FromSnapshot(
                    scheduleData.Id,
                    organizer,
                    singleScheduleData.StartDate.ToDateTimeOffset(timeZone),
                    scheduleData.DurationInMinutes,
                    scheduleData.Name,
                    scheduleData.Description,
                    scheduleData.ParticipantsMaxNumber,
                    timeZone);
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
                    scheduleData.ParticipantsMaxNumber,
                    TimeZoneInfo.FindSystemTimeZoneById(scheduleData.TimeZone));
            }

            throw new ArgumentException("Invalid schedule type");
        }

        public async Task<IEnumerable<Schedule>> GetByIdAsync(IEnumerable<Guid> ids)
        {
            var schedules = new List<Schedule>();

            foreach (var scheduleId in ids)
            {
                schedules.Add(await GetByIdAsync(scheduleId));
            }

            return schedules;
        }

        public async Task UpdateAsync(SingleSchedule schedule)
        {
            await UpdateInternalAsync(schedule);

            var singleScheduleData = await _applicationDbContext.SingleSchedules.SingleAsync(s => s.ScheduleId == schedule.Id);
            singleScheduleData.StartDate = schedule.StartDate.DateTime;
            singleScheduleData.EndDate = schedule.EndDate.DateTime;
        }

        public async Task UpdateAsync(RecurringSchedule schedule)
        {
            await UpdateInternalAsync(schedule);

            var recurringScheduleData = await _applicationDbContext.RecurringSchedules.SingleAsync(s => s.ScheduleId == schedule.Id);
            recurringScheduleData.StartDate = schedule.Period.StartDate;
            recurringScheduleData.EndDate = schedule.Period.EndDate;
            recurringScheduleData.RecurringExpression = schedule.RecurringCronExpressionString;
        }

        private async Task UpdateInternalAsync(Schedule schedule)
        {
            var scheduleData = await _applicationDbContext.Schedules.SingleAsync(s => s.Id == schedule.Id);
            scheduleData.Name = schedule.Name;
            scheduleData.Description = schedule.Description;
            scheduleData.DurationInMinutes = schedule.DurationInMinutes;
            scheduleData.ParticipantsMaxNumber = schedule.ParticipantsMaxNumber;
            scheduleData.OrganizerId = schedule.Organizer.Id;
            scheduleData.TimeZone = schedule.TimeZone.Id;
        }
    }
}
