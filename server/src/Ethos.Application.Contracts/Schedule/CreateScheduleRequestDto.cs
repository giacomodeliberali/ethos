using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ethos.Application.Contracts.Schedule
{
    public class CreateScheduleRequestDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int DurationInMinutes { get; set; }

        public string RecurringCronExpression { get; set; }
    }
}
