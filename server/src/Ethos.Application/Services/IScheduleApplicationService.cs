using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Schedule;

namespace Ethos.Application.Services
{
    public interface IScheduleApplicationService
    {
        /// <summary>
        /// Create a new Schedule for the currently logged in organizer.
        /// </summary>
        Task<CreateScheduleReplyDto> CreateAsync(CreateScheduleRequestDto input);

        /// <summary>
        /// Update an existing schedule.
        /// </summary>
        Task UpdateAsync(UpdateScheduleRequestDto input);

        /// <summary>
        /// Delete an existing schedule.
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Generate (in memory) all the schedules that are in the given interval.
        /// </summary>
        Task<IEnumerable<GeneratedScheduleDto>> GetSchedules(DateTime from, DateTime to);
    }
}
