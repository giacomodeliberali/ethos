using System;
using System.Threading.Tasks;
using Ethos.Domain.Entities;
using Ethos.Domain.Repositories;
using Ethos.EntityFrameworkCore.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ethos.EntityFrameworkCore.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingRepository(
            ApplicationDbContext applicationDbContext,
            IScheduleRepository scheduleRepository,
            UserManager<ApplicationUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _scheduleRepository = scheduleRepository;
            _userManager = userManager;
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

        public async Task<Booking> GetByIdAsync(Guid id)
        {
            var bookingData = await _applicationDbContext.Bookings.SingleAsync(b => b.Id == id);
            var schedule = await _scheduleRepository.GetByIdAsync(bookingData.ScheduleId);
            var user = await _userManager.FindByIdAsync(bookingData.UserId.ToString());

            return Booking.Factory.CreateFromSnapshot(
                bookingData.Id,
                schedule,
                user,
                bookingData.StartDate,
                bookingData.EndDate);
        }

        public async Task DeleteAsync(Booking booking)
        {
            var bookingData = await _applicationDbContext.Bookings.SingleAsync(b => b.Id == booking.Id);
            _applicationDbContext.Bookings.Remove(bookingData);
        }

        public async Task DeleteAsync(Guid id)
        {
            var bookingData = await _applicationDbContext.Bookings.SingleAsync(b => b.Id == id);
            _applicationDbContext.Bookings.Remove(bookingData);
        }

        public async Task UpdateAsync(Booking booking)
        {
            var bookingData = await _applicationDbContext.Bookings.SingleAsync(b => b.Id == booking.Id);
            bookingData.StartDate = booking.StartDate;
            bookingData.EndDate = booking.EndDate;
            bookingData.ScheduleId = booking.Schedule.Id;
        }
    }
}
