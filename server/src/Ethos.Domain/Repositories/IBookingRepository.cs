using System;
using System.Threading.Tasks;
using Ethos.Domain.Entities;

namespace Ethos.Domain.Repositories
{
    public interface IBookingRepository
    {
        Task<Guid> CreateAsync(Booking booking);

        Task<Booking> GetByIdAsync(Guid id);

        Task DeleteAsync(Booking booking);

        Task UpdateAsync(Booking booking);
    }
}
