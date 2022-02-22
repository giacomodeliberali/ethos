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
        /// Update an existing single schedule.
        /// </summary>
        Task UpdateSingleAsync(UpdateSingleScheduleRequestDto input);

        /// <summary>
        /// Update an existing recurring schedule instance.
        /// </summary>
        Task UpdateRecurringInstanceAsync(UpdateRecurringScheduleInstanceRequestDto input);

        /// <summary>
        /// Delete an existing schedule.
        /// </summary>
        Task DeleteAsync(DeleteScheduleRequestDto input);

        /// <summary>
        /// Generate (in memory) all the schedules that are in the given interval.
        /// </summary>
        Task<IEnumerable<GeneratedScheduleDto>> GetSchedules(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Returns the list of all recurring schedules and their next executions.
        /// </summary>
        Task<IEnumerable<RecurringScheduleDto>> GetAllRecurring();
    }
}
