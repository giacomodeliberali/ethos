using System;
using System.Threading.Tasks;
using Ethos.Domain.Entities;

namespace Ethos.Domain.Repositories
{
    public interface IBookingRepository
    {
        Task<Guid> CreateAsync(Booking booking);
    }
}
