using System;
using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Schedule
{
    public class DeleteSingleScheduleRequestDto
    {
        [Required]
        public Guid Id { get; set; }
    }
}
