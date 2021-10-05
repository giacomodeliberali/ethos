using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Schedule;

namespace Ethos.Application.Services
{
    public interface IScheduleApplicationService
    {
        /// <summary>
        /// Creates a new Schedule for the currently logged in organizer.
        /// </summary>
        Task<Guid> CreateAsync(CreateScheduleRequestDto input);

        /// <summary>
        /// Generates in memory all the schedules that are in the given interval.
        /// </summary>
        Task<IEnumerable<GeneratedScheduleDto>> GetSchedules(DateTime from, DateTime to);
    }
}
