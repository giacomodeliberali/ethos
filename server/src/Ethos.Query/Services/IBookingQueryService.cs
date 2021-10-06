using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Query.Projections;

namespace Ethos.Query.Services
{
    public interface IBookingQueryService
    {
        Task<IEnumerable<BookingProjection>> GetAllInScheduleInRange(Guid scheduleId, DateTime startDate, DateTime endDate);
    }
}
