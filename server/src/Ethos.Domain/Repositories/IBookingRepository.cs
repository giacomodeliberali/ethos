using System;
using System.Threading.Tasks;

namespace Ethos.Domain.Repositories
{
    public interface IBookingRepository
    {
        Task<Guid> CreateAsync(Booking.Booking booking);
    }
}
