using System;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Booking;
using Ethos.Domain.Entities;

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
    }
}
