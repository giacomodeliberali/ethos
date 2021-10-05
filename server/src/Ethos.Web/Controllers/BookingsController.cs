using System;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ethos.Web.Controllers
{
    /// <summary>
    /// The Booking controller.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingApplicationService _bookingApplicationService;

        public BookingsController(IBookingApplicationService bookingApplicationService)
        {
            _bookingApplicationService = bookingApplicationService;
        }

        /// <summary>
        /// Create a new booking for the current user.
        /// </summary>
        [HttpPost]
        public async Task<Guid> CreateAsync(CreateBookingRequestDto input)
        {
            return await _bookingApplicationService.CreateAsync(input);
        }
    }
}
