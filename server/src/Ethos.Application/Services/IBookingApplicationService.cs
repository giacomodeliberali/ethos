using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Booking;

namespace Ethos.Application.Services
{
    public interface IBookingApplicationService
    {
        /// <summary>
        /// Create a new booking for the current user.
        /// </summary>
        Task<CreateBookingReplyDto> CreateAsync(CreateBookingRequestDto input);

        Task DeleteAsync(Guid id);

        Task<BookingDto> GetByIdAsync(Guid id);

        Task<IEnumerable<BookingDto>> GetFutureBookings();
    }
}
