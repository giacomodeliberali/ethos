using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Domain.Common;
using Ethos.Query.Projections;

namespace Ethos.Query.Services
{
    public interface IBookingQueryService
    {
        Task<List<BookingProjection>> GetAllBookingsInRange(Guid scheduleId, DateOnlyPeriod period);
        Task<List<BookingProjection>> GetAllBookings(Guid scheduleId);
        Task<List<BookingProjection>> GetAllBookingsByUserId(Guid userId, DateOnlyPeriod period);
    }
}
