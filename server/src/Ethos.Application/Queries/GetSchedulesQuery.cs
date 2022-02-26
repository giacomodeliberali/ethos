using System;
using System.Collections.Generic;
using Ethos.Application.Contracts.Schedule;
using MediatR;

namespace Ethos.Application.Queries
{
    public class GetSchedulesQuery : IRequest<IEnumerable<GeneratedScheduleDto>>
    {
        public DateTime StartDate { get; init; }

        public DateTime EndDate { get; init; }
    }
}
