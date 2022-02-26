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
    /// The recurring schedule controller.
    /// </summary>
    [ApiController]
    [Authorize(Roles = RoleConstants.Admin)]
    [Route("api/schedules/recurring")]
    public class RecurringSchedulesController : ControllerBase
    {
        private readonly IScheduleApplicationService _scheduleApplicationService;

        public RecurringSchedulesController(IScheduleApplicationService scheduleApplicationService)
        {
            _scheduleApplicationService = scheduleApplicationService;
        }

        /// <summary>
        /// Create a new recurring schedule.
        /// </summary>
        [HttpPost]
        public async Task<CreateScheduleReplyDto> CreateRecurringScheduleAsync([Required] CreateRecurringScheduleRequestDto input)
        {
            return await _scheduleApplicationService.CreateRecurringAsync(input);
        }

        /// <summary>
        /// Returns the list of all returning schedules and their next executions.
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<RecurringScheduleDto>> GetAllRecurring()
        {
            return await _scheduleApplicationService.GetAllRecurring();
        }

        /// <summary>
        /// Update an existing recurring schedule instance.
        /// </summary>
        [HttpPut]
        public async Task UpdateRecurringScheduleInstanceAsync([Required] UpdateRecurringScheduleInstanceRequestDto input)
        {
            await _scheduleApplicationService.UpdateRecurringInstanceAsync(input);
        }

        /// <summary>
        /// Delete an existing recurring schedule.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task DeleteRecurringScheduleAsync([Required] DeleteRecurringScheduleRequestDto input, Guid id)
        {
            if (input.Id != id)
            {
                throw new BusinessException("Invalid request: Ids mismatch URL/body.");
            }

            await _scheduleApplicationService.DeleteRecurringAsync(input);
        }
    }
}
