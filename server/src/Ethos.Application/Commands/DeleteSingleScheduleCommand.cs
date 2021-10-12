using System;
using Ethos.Application.Contracts.Schedule;
using Ethos.Domain.Entities;
using MediatR;

namespace Ethos.Application.Commands
{
    public class DeleteSingleScheduleCommand : IRequest
    {
        public SingleSchedule Schedule { get; }

        public DeleteSingleScheduleCommand(SingleSchedule schedule)
        {
            Schedule = schedule;
        }
    }
}
