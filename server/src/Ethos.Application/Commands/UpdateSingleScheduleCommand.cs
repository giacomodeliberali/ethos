using System;
using MediatR;

namespace Ethos.Application.Commands
{
    public class UpdateSingleScheduleCommand : IRequest
    {
        public Guid Id { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public DateTime StartDate { get; init; }

        public int DurationInMinutes { get; init; }

        public int ParticipantsMaxNumber { get; init; }

        public Guid OrganizerId { get; init; }
    }
}
