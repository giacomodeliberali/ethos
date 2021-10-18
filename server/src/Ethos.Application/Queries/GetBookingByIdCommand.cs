using System;
using Ethos.Application.Contracts.Booking;
using MediatR;

namespace Ethos.Application.Queries
{
    public class GetBookingByIdCommand : IRequest<BookingDto>
    {
        public Guid Id { get; }

        public GetBookingByIdCommand(Guid id)
        {
            Id = id;
        }
    }
}
