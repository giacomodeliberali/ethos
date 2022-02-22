using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Services;
using Ethos.Domain.Exceptions;
using Ethos.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Ethos.Web.Controllers
{
    /// <summary>
    /// The Schedule controller.
    /// </summary>
    [ApiController]
    [Authorize(Roles = RoleConstants.Admin)]
    [Route("api/schedules")]
    public class SchedulesController : ControllerBase
    {
        private readonly IScheduleApplicationService _scheduleApplicationService;

        public SchedulesController(IScheduleApplicationService scheduleApplicationService)
        {
            _scheduleApplicationService = scheduleApplicationService;
        }

        /// <summary>
        /// Create a new schedule to the current user.
        /// </summary>
        [HttpPost]
        public async Task<CreateScheduleReplyDto> CreateScheduleAsync([Required] CreateScheduleRequestDto input)
        {
            return await _scheduleApplicationService.CreateAsync(input);
        }

        /// <summary>
        /// Generate (in memory) all the schedules that are in the given interval.
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<GeneratedScheduleDto>> GetAllSchedulesInRange([Required] DateTime? startDate, [Required] DateTime? endDate)
        {
            return await _scheduleApplicationService.GetSchedules(startDate!.Value, endDate!.Value);
        }

        /// <summary>
        /// Update an existing single schedule.
        /// </summary>
        [HttpPut("single")]
        public async Task UpdateScheduleAsync([Required] UpdateSingleScheduleRequestDto input)
        {
            await _scheduleApplicationService.UpdateSingleAsync(input);
        }

        /// <summary>
        /// Update an existing recurring schedule instance.
        /// </summary>
        [HttpPut("recurring")]
        public async Task UpdateRecurringScheduleInstanceAsync([Required] UpdateRecurringScheduleInstanceRequestDto input)
        {
            await _scheduleApplicationService.UpdateRecurringInstanceAsync(input);
        }

        /// <summary>
        /// Delete an existing schedule.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task DeleteScheduleAsync([Required] DeleteScheduleRequestDto input, Guid id)
        {
            if (input.Id != id)
            {
                throw new BusinessException("Invalid request: Ids mismatch URL/body.");
            }

            await _scheduleApplicationService.DeleteAsync(input);
        }
    }
}
