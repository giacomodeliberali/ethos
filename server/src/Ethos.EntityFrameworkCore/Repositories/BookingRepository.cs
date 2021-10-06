using System;
using System.Threading.Tasks;
using Ethos.Domain.Entities;
using Ethos.Domain.Repositories;
using Ethos.EntityFrameworkCore.Entities;
using Microsoft.AspNetCore.Identity;

namespace Ethos.EntityFrameworkCore.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public BookingRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Guid> CreateAsync(Booking booking)
        {
            var bookingData = new BookingData
            {
                Id = booking.Id,
                ScheduleId = booking.Schedule.Id,
                UserId = booking.User.Id,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
            };

            await _applicationDbContext.Bookings.AddAsync(bookingData);

            return bookingData.Id;
        }
    }
}
