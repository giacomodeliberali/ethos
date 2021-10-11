using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Query.Projections;
using Ethos.Query.Services;
using Microsoft.EntityFrameworkCore;

namespace Ethos.EntityFrameworkCore.Query
{
    public class BookingQueryService : BaseQueryService, IBookingQueryService
    {
        public BookingQueryService(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {
        }

        public async Task<List<BookingProjection>> GetAllBookingsInRange(Guid scheduleId, DateTime startDate, DateTime endDate)
        {
            var bookings = await (
                    from booking in ApplicationDbContext.Bookings.AsNoTracking()
                    join schedule in ApplicationDbContext.Schedules.AsNoTracking() on booking.ScheduleId equals schedule.Id
                    join user in ApplicationDbContext.Users.AsNoTracking() on booking.UserId equals user.Id
                    where booking.StartDate >= startDate
                    where booking.EndDate <= endDate
                    where booking.ScheduleId == scheduleId
                    select new
                    {
                        Booking = booking,
                        Schedule = schedule,
                        User = user,
                    }).ToListAsync();

            var bookingsResult = bookings
                .Select(item => new BookingProjection()
                {
                    Id = item.Booking.Id,
                    StartDate = item.Booking.StartDate,
                    EndDate = item.Booking.EndDate,
                    ScheduleId = item.Booking.ScheduleId,
                    UserId = item.Booking.UserId,
                    UserFullName = item.User.FullName,
                    UserEmail = item.User.Email,
                    UserName = item.User.UserName,
                }).ToList();

            return bookingsResult
                .OrderBy(b => b.StartDate)
                .ToList();
        }
    }
}
