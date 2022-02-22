using System;
using MediatR;

namespace Ethos.Application.Commands
{
    public class UpdateRecurringScheduleInstanceCommand : IRequest
    {
        public Guid Id { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public DateTime InstanceStartDate { get; set; }

        public DateTime InstanceEndDate { get; set; }

        public DateTime StartDate { get; init; }

        public int DurationInMinutes { get; init; }

        public int ParticipantsMaxNumber { get; init; }

        public Guid OrganizerId { get; init; }
    }
}
