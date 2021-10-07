using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Services;
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
        public async Task<CreateBookingReplyDto> CreateAsync(CreateBookingRequestDto input)
        {
            return await _bookingApplicationService.CreateAsync(input);
        }

        /// <summary>
        /// Delete an existing booking.
        /// </summary>
        [HttpDelete]
        public async Task DeleteAsync([Required] Guid id)
        {
            await _bookingApplicationService.DeleteAsync(id);
        }

        /// <summary>
        /// Return the requested booking or null.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<BookingDto> GetByIdAsync([Required] Guid id)
        {
            return await _bookingApplicationService.GetByIdAsync(id);
        }
    }
}
