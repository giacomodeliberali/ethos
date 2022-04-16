using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Domain.Common;
using Ethos.Query.Projections;
using Ethos.Query.Services;
using Microsoft.EntityFrameworkCore;

// TODO @GDL: refactor to a single Query method with the needed params

namespace Ethos.EntityFrameworkCore.Query
{
    public class BookingQueryService : BaseQueryService, IBookingQueryService
    {
        public BookingQueryService(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {
        }

        public async Task<List<BookingProjection>> GetAllBookingsInRange(Guid scheduleId, DateOnlyPeriod period)
        {
            var bookings = await (
                from booking in ApplicationDbContext.Bookings.AsNoTracking()
                join schedule in ApplicationDbContext.Schedules.AsNoTracking() on booking.ScheduleId equals schedule.Id
                join user in ApplicationDbContext.Users.AsNoTracking() on booking.UserId equals user.Id
                join organizer in ApplicationDbContext.Users.AsNoTracking() on schedule.OrganizerId equals organizer.Id
                where booking.StartDate >= period.StartDate.ToDateTime(TimeOnly.MinValue)
                where booking.EndDate <= period.EndDate.ToDateTime(TimeOnly.MaxValue)
                where booking.ScheduleId == scheduleId
                select new
                {
                    Booking = booking,
                    Schedule = schedule,
                    User = user,
                    Organizer = organizer,
                }).ToListAsync();

            var bookingsResult = bookings
                .Select(item =>
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById(item.Schedule.TimeZone);
                    return new BookingProjection()
                    {
                        Id = item.Booking.Id,
                        StartDate = item.Booking.StartDate.ToDateTimeOffset(timeZone),
                        EndDate = item.Booking.EndDate.ToDateTimeOffset(timeZone),
                        ScheduleId = item.Booking.ScheduleId,
                        UserId = item.Booking.UserId,
                        UserFullName = item.User.FullName,
                        UserEmail = item.User.Email,
                        UserName = item.User.UserName,
                        ScheduleDescription = item.Schedule.Description,
                        ScheduleName = item.Schedule.Name,
                        ScheduleDurationInMinutes = item.Schedule.DurationInMinutes,
                        ScheduleOrganizerFullName = item.Organizer.FullName,
                        ParticipantsMaxNumber = item.Schedule.ParticipantsMaxNumber,
                    };
                }).ToList();

            return bookingsResult
                .OrderBy(b => b.StartDate)
                .ToList();
        }

        public async Task<List<BookingProjection>> GetAllBookings(Guid scheduleId)
        {
            var bookings = await (
                from booking in ApplicationDbContext.Bookings.AsNoTracking()
                join schedule in ApplicationDbContext.Schedules.AsNoTracking() on booking.ScheduleId equals schedule.Id
                join user in ApplicationDbContext.Users.AsNoTracking() on booking.UserId equals user.Id
                join organizer in ApplicationDbContext.Users.AsNoTracking() on schedule.OrganizerId equals organizer.Id
                where booking.ScheduleId == scheduleId
                select new
                {
                    Booking = booking,
                    Schedule = schedule,
                    User = user,
                    Organizer = organizer,
                }).ToListAsync();

            var bookingsResult = bookings
                .Select(item =>
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById(item.Schedule.TimeZone);
                    return new BookingProjection()
                    {
                        Id = item.Booking.Id,
                        StartDate = item.Booking.StartDate.ToDateTimeOffset(timeZone),
                        EndDate = item.Booking.EndDate.ToDateTimeOffset(timeZone),
                        ScheduleId = item.Booking.ScheduleId,
                        UserId = item.Booking.UserId,
                        UserFullName = item.User.FullName,
                        UserEmail = item.User.Email,
                        UserName = item.User.UserName,
                        ScheduleDescription = item.Schedule.Description,
                        ScheduleName = item.Schedule.Name,
                        ScheduleDurationInMinutes = item.Schedule.DurationInMinutes,
                        ScheduleOrganizerFullName = item.Organizer.FullName,
                        ParticipantsMaxNumber = item.Schedule.ParticipantsMaxNumber,
                    };
                }).ToList();

            return bookingsResult
                .OrderBy(b => b.StartDate)
                .ToList();
        }

        public async Task<List<BookingProjection>> GetAllBookingsByUserId(Guid userId, DateOnlyPeriod period)
        {
            var bookings = await (
                from booking in ApplicationDbContext.Bookings.AsNoTracking()
                join schedule in ApplicationDbContext.Schedules.AsNoTracking() on booking.ScheduleId equals schedule.Id
                join user in ApplicationDbContext.Users.AsNoTracking() on booking.UserId equals user.Id
                join organizer in ApplicationDbContext.Users.AsNoTracking() on schedule.OrganizerId equals organizer.Id
                where booking.UserId == userId
                where booking.StartDate >= period.StartDate.ToDateTime(TimeOnly.MinValue)
                where booking.EndDate <= period.EndDate.ToDateTime(TimeOnly.MaxValue)
                select new
                {
                    Booking = booking,
                    Schedule = schedule,
                    User = user,
                    Organizer = organizer,
                }).ToListAsync();

            var bookingsResult = bookings
                .Select(item =>
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById(item.Schedule.TimeZone);
                    return new BookingProjection()
                    {
                        Id = item.Booking.Id,
                        StartDate = item.Booking.StartDate.ToDateTimeOffset(timeZone),
                        EndDate = item.Booking.EndDate.ToDateTimeOffset(timeZone),
                        ScheduleId = item.Booking.ScheduleId,
                        UserId = item.Booking.UserId,
                        UserFullName = item.User.FullName,
                        UserEmail = item.User.Email,
                        UserName = item.User.UserName,
                        ScheduleDescription = item.Schedule.Description,
                        ScheduleName = item.Schedule.Name,
                        ScheduleDurationInMinutes = item.Schedule.DurationInMinutes,
                        ScheduleOrganizerFullName = item.Organizer.FullName,
                        ParticipantsMaxNumber = item.Schedule.ParticipantsMaxNumber,
                    };
                }).ToList();

            return bookingsResult
                .OrderBy(b => b.StartDate)
                .ToList();
        }
    }
}
