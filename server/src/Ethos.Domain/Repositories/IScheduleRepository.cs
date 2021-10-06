using System;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Domain.Entities;

namespace Ethos.Domain.Repositories
{
    public interface IScheduleRepository
    {
        Task<Guid> CreateAsync(Schedule schedule);

        Task DeleteAsync(Schedule schedule);

        Task<Schedule> GetByIdAsync(Guid id);

        Task UpdateAsync(Schedule schedule);
    }
}
