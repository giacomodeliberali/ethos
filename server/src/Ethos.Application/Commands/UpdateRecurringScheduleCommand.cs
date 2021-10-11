using Ethos.Application.Contracts.Schedule;
using Ethos.Domain.Entities;
using MediatR;

namespace Ethos.Application.Commands
{
    public class UpdateRecurringScheduleCommand : IRequest
    {
        public UpdateScheduleRequestDto Input { get; }

        public RecurringSchedule Schedule { get; }

        public UpdateRecurringScheduleCommand(UpdateScheduleRequestDto input, RecurringSchedule schedule)
        {
            Input = input;
            Schedule = schedule;
        }
    }
}
