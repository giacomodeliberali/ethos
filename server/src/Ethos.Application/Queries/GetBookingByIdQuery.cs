using System;
using Ethos.Application.Contracts.Booking;
using MediatR;

namespace Ethos.Application.Queries
{
    public class GetBookingByIdQuery : IRequest<BookingDto>
    {
        public Guid Id { get; }

        public GetBookingByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
