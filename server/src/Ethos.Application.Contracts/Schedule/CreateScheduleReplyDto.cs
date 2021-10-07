using System;
using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Schedule
{
    public class CreateScheduleReplyDto
    {
        [Required]
        public Guid Id { get; set; }
    }
}
