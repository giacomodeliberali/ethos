using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Domain.Entities;

namespace Ethos.Domain.Repositories
{
    public interface IScheduleRepository
    {
        Task<Guid> CreateAsync(SingleSchedule schedule);

        Task<Guid> CreateAsync(RecurringSchedule schedule);

        Task DeleteAsync(SingleSchedule schedule);

        Task DeleteAsync(RecurringSchedule schedule);

        Task<Schedule> GetByIdAsync(Guid id);
        
        Task<IEnumerable<Schedule>> GetByIdAsync(IEnumerable<Guid> ids);

        Task UpdateAsync(SingleSchedule schedule);

        Task UpdateAsync(RecurringSchedule schedule);
    }
}
