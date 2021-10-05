using System;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Schedule;
using Ethos.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ethos.Web.Controllers
{
    /// <summary>
    /// The Schedule controller.
    /// </summary>
    [ApiController]
    [Authorize]
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
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<Guid> CreateAsync(CreateScheduleRequestDto input)
        {
            return await _scheduleApplicationService.CreateAsync(input);
        }
    }
}
