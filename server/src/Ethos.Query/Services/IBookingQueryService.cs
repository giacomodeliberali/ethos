using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Domain.Common;
using Ethos.Query.Projections;

namespace Ethos.Query.Services
{
    public interface IBookingQueryService
    {
        Task<List<BookingProjection>> GetAllBookingsInRange(Guid scheduleId, DateTime startDate, DateTime endDate);
        Task<List<BookingProjection>> GetAllBookings(Guid scheduleId);
        Task<List<BookingProjection>> GetAllBookingsByUserId(Guid userId, Period period);
    }
}
