using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Queries;
using Ethos.Domain.Common;
using Ethos.Query.Services;
using MediatR;

namespace Ethos.Application.Handlers
{

    public class GetBookingsQueryHandler : IRequestHandler<GetFutureBookingsQuery, IEnumerable<BookingDto>>
    {
        private readonly IBookingQueryService _bookingQueryService;
        private readonly IMapper _mapper;

        public GetBookingsQueryHandler(
            IBookingQueryService bookingQueryService,
            IMapper mapper)
        {
            _bookingQueryService = bookingQueryService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookingDto>> Handle(GetFutureBookingsQuery request, CancellationToken cancellationToken)
        {
            var period = new Period(DateTime.UtcNow, DateTime.UtcNow.AddYears(1));
            var bookings = await _bookingQueryService.GetAllBookingsByUserId(request.UserId, period);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
    }
}
