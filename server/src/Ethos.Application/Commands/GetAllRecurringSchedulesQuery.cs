using System.Collections.Generic;
using Ethos.Application.Contracts.Schedule;
using MediatR;

namespace Ethos.Application.Commands
{
    public class GetAllRecurringSchedulesQuery : IRequest<IEnumerable<RecurringScheduleDto>>
    {
    }
}
