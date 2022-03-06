using System;
using Ethos.Application.Contracts;
using MediatR;

namespace Ethos.Application.Commands.Schedules.Recurring
{
    public record UpdateRecurringScheduleInstanceCommand(
        Guid Id, 
        string Name, 
        string Description,
        DateTime StartDate, 
        DateTime EndDate,
        int DurationInMinutes,
        string RecurringCronExpression,
        Guid OrganizerId, 
        int ParticipantsMaxNumber, 
        DateTime InstanceStartDate, 
        DateTime InstanceEndDate,
        RecurringScheduleOperationType RecurringScheduleOperationType) : IRequest
    {
    }
}
