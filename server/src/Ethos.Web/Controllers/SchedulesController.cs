using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Services;
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
        public async Task<CreateScheduleReplyDto> CreateAsync(CreateScheduleRequestDto input)
        {
            return await _scheduleApplicationService.CreateAsync(input);
        }

        /// <summary>
        /// Generate (in memory) all the schedules that are in the given interval.
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<GeneratedScheduleDto>> GetAllInRange([Required] DateTime? startDate, [Required] DateTime? endDate)
        {
            return await _scheduleApplicationService.GetSchedules(startDate!.Value, endDate!.Value);
        }

        /// <summary>
        /// Update an existing schedule.
        /// </summary>
        [HttpPut]
        public async Task UpdateAsync(UpdateScheduleRequestDto input)
        {
            await _scheduleApplicationService.UpdateAsync(input);
        }

        /// <summary>
        /// Delete an existing schedule.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _scheduleApplicationService.DeleteAsync(id);
        }
    }
}
