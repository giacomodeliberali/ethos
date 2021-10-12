using System;
using Ethos.Application.Contracts.Schedule;
using Ethos.Domain.Entities;
using MediatR;

namespace Ethos.Application.Commands
{
    public class DeleteRecurringScheduleCommand : IRequest
    {
        public RecurringSchedule Schedule { get; }

        public RecurringScheduleOperationType OperationType { get; }

        public DateTime InstanceStartDate { get; }

        public DateTime InstanceEndDate { get; }

        public DeleteRecurringScheduleCommand(
            RecurringSchedule schedule,
            RecurringScheduleOperationType operationType,
            DateTime instanceStartDate,
            DateTime instanceEndDate)
        {
            Schedule = schedule;
            OperationType = operationType;
            InstanceStartDate = instanceStartDate;
            InstanceEndDate = instanceEndDate;
        }
    }
}
