using System.Collections.Generic;
using Ethos.Application.Contracts.Schedule;
using MediatR;

namespace Ethos.Application.Queries
{
    public class GetAllRecurringSchedulesQuery : IRequest<IEnumerable<RecurringScheduleDto>>
    {
    }
}
