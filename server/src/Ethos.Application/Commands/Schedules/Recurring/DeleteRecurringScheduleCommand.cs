using System;
using Ethos.Application.Contracts;
using Ethos.Application.Contracts.Schedule;
using MediatR;

namespace Ethos.Application.Commands.Schedules.Recurring
{
    public class DeleteRecurringScheduleCommand : IRequest<DeleteScheduleReplyDto>
    {
        public Guid Id { get; init; }

        public RecurringScheduleOperationType RecurringScheduleOperationType { get; init; }

        public DateTimeOffset InstanceStartDate { get; init; }

        public DateTimeOffset InstanceEndDate { get; init; }
    }
}
