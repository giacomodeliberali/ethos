using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ethos.Domain.Repositories
{
    public interface IScheduleRepository
    {
        Task<Guid> CreateAsync(Schedule.Schedule schedule);

        Task DeleteAsync(Schedule.Schedule schedule);

        Task<Schedule.Schedule> GetByIdAsync(Guid id);
    }
}
