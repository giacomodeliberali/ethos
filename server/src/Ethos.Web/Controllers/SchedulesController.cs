using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Services;
using Ethos.Common;
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
        /// Generate all (single and recurring) schedules that are in the given interval.
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<GeneratedScheduleDto>> GetAllSchedulesInRange([Required] DateTimeOffset? startDate, [Required] DateTimeOffset? endDate)
        {
            return await _scheduleApplicationService.GetSchedules(startDate!.Value, endDate!.Value);
        }
    }
}
