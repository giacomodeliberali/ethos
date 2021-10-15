using System;
using Ethos.Application.Contracts.Schedule;
using Ethos.Domain.Entities;
using MediatR;

namespace Ethos.Application.Commands
{
    public class DeleteScheduleCommand : IRequest
    {
        public Guid Id { get; init; }

        /// <summary>
        /// Required only if the schedule is recurring.
        /// </summary>
        public RecurringScheduleOperationType? RecurringScheduleOperationType { get; init; }

        public DateTime InstanceStartDate { get; init; }

        public DateTime InstanceEndDate { get; init; }
    }
}
