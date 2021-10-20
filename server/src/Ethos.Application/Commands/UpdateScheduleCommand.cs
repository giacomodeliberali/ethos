using System;
using Ethos.Application.Contracts.Schedule;
using MediatR;

namespace Ethos.Application.Commands
{
    public class UpdateScheduleCommand : IRequest
    {
        public Guid Id { get; init; }

        public DateTime InstanceStartDate { get; init; }

        public DateTime InstanceEndDate { get; init; }

        public Schedule UpdatedSchedule { get; init; }

        public RecurringScheduleOperationType? RecurringScheduleOperationType { get; init; }

        public class Schedule
        {
            public string Name { get; init; }

            public string Description { get; init; }

            public DateTime StartDate { get; init; }

            public DateTime EndDate { get; init; }

            public int DurationInMinutes { get; init; }

            public string RecurringCronExpression { get; init; }

            public int ParticipantsMaxNumber { get; init; }

            public Guid OrganizerId { get; init; }
        }
    }
}
