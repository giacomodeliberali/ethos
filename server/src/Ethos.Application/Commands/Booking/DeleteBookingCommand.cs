using System;
using MediatR;

namespace Ethos.Application.Commands.Booking
{
    public class DeleteBookingCommand : IRequest
    {
        public Guid Id { get; }

        public DeleteBookingCommand(Guid id)
        {
            Id = id;
        }
    }
}
