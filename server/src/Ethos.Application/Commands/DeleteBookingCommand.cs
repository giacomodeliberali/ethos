using System;
using MediatR;

namespace Ethos.Application.Commands
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
