using System;
using System.Threading.Tasks;
using Ethos.Domain.Entities;
using Ethos.Domain.Repositories;
using Ethos.EntityFrameworkCore.Booking;
using Microsoft.AspNetCore.Identity;

namespace Ethos.EntityFrameworkCore.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingRepository(
            ApplicationDbContext applicationDbContext,
            UserManager<ApplicationUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        public async Task<Guid> CreateAsync(Domain.Entities.Booking booking)
        {
            var bookingData = new BookingData()
            {
                Id = booking.Id,
                ScheduleId = booking.Schedule.Id,
                UserId = booking.User.Id,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
            };

            await _applicationDbContext.Bookings.AddAsync(bookingData);

            await _applicationDbContext.SaveChangesAsync();

            return bookingData.Id;
        }
    }
}
