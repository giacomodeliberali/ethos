using System;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Contracts.Schedule;

namespace Ethos.Application.Booking
{
    public interface IBookingApplicationService
    {
        Task<Guid> CreateAsync(CreateBookingRequestDto input);
    }
}
