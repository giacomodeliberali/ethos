using System;
using System.Threading.Tasks;
using Ethos.Domain.Entities;
using Ethos.Domain.Repositories;
using Ethos.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ethos.EntityFrameworkCore.Repositories
{
    public class ScheduleExceptionRepository : IScheduleExceptionRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleExceptionRepository(
            ApplicationDbContext applicationDbContext,
            IScheduleRepository scheduleRepository)
        {
            _applicationDbContext = applicationDbContext;
            _scheduleRepository = scheduleRepository;
        }

        public async Task<Guid> CreateAsync(ScheduleException scheduleException)
        {
            var scheduleExceptionData = new ScheduleExceptionData()
            {
                Id = scheduleException.Id,
                ScheduleId = scheduleException.Schedule.Id,
                Date = scheduleException.Date,
            };

            await _applicationDbContext.ScheduleExceptions.AddAsync(scheduleExceptionData);

            return scheduleException.Id;
        }

        public Task DeleteAsync(ScheduleException scheduleException)
        {
            return DeleteAsync(scheduleException.Id);
        }

        public async Task DeleteAsync(Guid scheduleExceptionId)
        {
            var scheduleExceptionData = await _applicationDbContext.ScheduleExceptions.SingleAsync(e => e.Id == scheduleExceptionId);
            _applicationDbContext.Remove(scheduleExceptionData);
        }

        public async Task UpdateAsync(ScheduleException scheduleException)
        {
            var scheduleExceptionData = await _applicationDbContext.ScheduleExceptions.SingleAsync(e => e.Id == scheduleException.Id);

            scheduleExceptionData.Date = scheduleException.Date;
        }

        public async Task<ScheduleException> GetByIdAsync(Guid id)
        {
            var scheduleExceptionData = await _applicationDbContext.ScheduleExceptions.SingleAsync(e => e.ScheduleId == id);
            var recurringSchedule = await _scheduleRepository.GetByIdAsync(scheduleExceptionData.ScheduleId);

            return ScheduleException.Factory.FromSnapshot(
                scheduleExceptionData.Id,
                (recurringSchedule as RecurringSchedule) !,
                new DateOnly(scheduleExceptionData.Date.Year, scheduleExceptionData.Date.Month, scheduleExceptionData.Date.Day));
        }
    }
}
