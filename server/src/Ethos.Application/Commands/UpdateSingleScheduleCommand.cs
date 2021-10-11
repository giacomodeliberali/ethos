using Ethos.Application.Contracts.Schedule;
using Ethos.Domain.Entities;
using MediatR;

namespace Ethos.Application.Commands
{
    public class UpdateSingleScheduleCommand : IRequest
    {
        public UpdateScheduleRequestDto Input { get; }

        public SingleSchedule Schedule { get; }

        public UpdateSingleScheduleCommand(UpdateScheduleRequestDto input, SingleSchedule schedule)
        {
            Input = input;
            Schedule = schedule;
        }
    }
}
