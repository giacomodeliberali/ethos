using System;
using System.Collections.Generic;
using Ethos.Application.Contracts.Booking;
using MediatR;

namespace Ethos.Application.Queries
{
    public class GetFutureBookingsQuery : IRequest<IEnumerable<BookingDto>>
    {
        public GetFutureBookingsQuery(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; }
    }
}
