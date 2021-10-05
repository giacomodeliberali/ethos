using System;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Booking;

namespace Ethos.Application.Services
{
    public interface IBookingApplicationService
    {
        Task<Guid> CreateAsync(CreateBookingRequestDto input);
    }
}
