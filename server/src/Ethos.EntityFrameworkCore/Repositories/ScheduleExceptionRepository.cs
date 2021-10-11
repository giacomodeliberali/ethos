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
                StartDate = scheduleException.StartDate,
                EndDate = scheduleException.EndDate,
            };

            await _applicationDbContext.ScheduleExceptions.AddAsync(scheduleExceptionData);

            return scheduleException.Id;
        }

        public async Task DeleteAsync(ScheduleException scheduleException)
        {
            var scheduleExceptionData = await _applicationDbContext.ScheduleExceptions.SingleAsync(e => e.ScheduleId == scheduleException.Id);
            _applicationDbContext.Remove(scheduleExceptionData);
        }

        public async Task UpdateAsync(ScheduleException scheduleException)
        {
            var scheduleExceptionData = await _applicationDbContext.ScheduleExceptions.SingleAsync(e => e.ScheduleId == scheduleException.Id);

            scheduleExceptionData.StartDate = scheduleException.StartDate;
            scheduleExceptionData.EndDate = scheduleException.EndDate;
        }

        public async Task<ScheduleException> GetByIdAsync(Guid guid)
        {
            var scheduleExceptionData = await _applicationDbContext.ScheduleExceptions.SingleAsync(e => e.ScheduleId == guid);
            var recurringSchedule = await _scheduleRepository.GetByIdAsync(scheduleExceptionData.ScheduleId);

            return ScheduleException.Factory.FromSnapshot(
                scheduleExceptionData.Id,
                recurringSchedule as RecurringSchedule,
                scheduleExceptionData.StartDate,
                scheduleExceptionData.EndDate);
        }
    }
}
