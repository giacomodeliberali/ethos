using System;
using System.Collections;
using System.Collections.Generic;

namespace Ethos.Application.Contracts.Schedule
{
    public class CreateScheduleRequestDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public TimeSpan? Duration { get; set; }

        public string RecurringCronExpression { get; set; }
    }
}
