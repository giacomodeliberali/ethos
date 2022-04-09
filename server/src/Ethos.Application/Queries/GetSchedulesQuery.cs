using System;
using System.Collections.Generic;
using Ethos.Application.Contracts.Schedule;
using MediatR;

namespace Ethos.Application.Queries
{
    public class GetSchedulesQuery : IRequest<IEnumerable<GeneratedScheduleDto>>
    {
        public GetSchedulesQuery(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public DateTimeOffset StartDate { get; }

        public DateTimeOffset EndDate { get; }
    }
}
