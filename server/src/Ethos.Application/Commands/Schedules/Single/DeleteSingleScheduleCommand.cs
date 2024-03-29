using System;
using Ethos.Application.Contracts.Schedule;
using MediatR;

namespace Ethos.Application.Commands.Schedules.Single
{
    public class DeleteSingleScheduleCommand : IRequest<DeleteScheduleReplyDto>
    {
        public Guid Id { get; init; }
    }
}
