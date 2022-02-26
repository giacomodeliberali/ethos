using System;
using System.Threading.Tasks;
using Ethos.Domain.Entities;

namespace Ethos.Domain.Repositories
{
    public interface IScheduleExceptionRepository
    {
        Task<Guid> CreateAsync(ScheduleException scheduleException);

        Task DeleteAsync(ScheduleException scheduleException);

        Task UpdateAsync(ScheduleException scheduleException);

        Task<ScheduleException> GetByIdAsync(Guid id);
    }
}
