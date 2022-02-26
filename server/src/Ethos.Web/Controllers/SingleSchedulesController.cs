using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Services;
using Ethos.Common;
using Ethos.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Ethos.Web.Controllers
{
    /// <summary>
    /// The Schedule controller.
    /// </summary>
    [ApiController]
    [Authorize(Roles = RoleConstants.Admin)]
    [Route("api/schedules/single")]
    public class SingleSchedulesController : ControllerBase
    {
        private readonly IScheduleApplicationService _scheduleApplicationService;

        public SingleSchedulesController(IScheduleApplicationService scheduleApplicationService)
        {
            _scheduleApplicationService = scheduleApplicationService;
        }

        /// <summary>
        /// Create a new single schedule.
        /// </summary>
        [HttpPost]
        public async Task<CreateScheduleReplyDto> CreateSingleScheduleAsync([Required] CreateSingleScheduleRequestDto input)
        {
            return await _scheduleApplicationService.CreateAsync(input);
        }

        /// <summary>
        /// Update an existing single schedule.
        /// </summary>
        [HttpPut]
        public async Task UpdateSingleScheduleAsync([Required] UpdateSingleScheduleRequestDto input)
        {
            await _scheduleApplicationService.UpdateSingleAsync(input);
        }

        /// <summary>
        /// Delete an existing schedule.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task DeleteSingleScheduleAsync([Required] DeleteSingleScheduleRequestDto input, Guid id)
        {
            if (input.Id != id)
            {
                throw new BusinessException("Invalid request: Ids mismatch URL/body.");
            }

            await _scheduleApplicationService.DeleteAsync(input);
        }
    }
}
