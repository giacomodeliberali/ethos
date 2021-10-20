using System;
using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Booking
{
    public class CreateBookingReplyDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public int CurrentParticipantsNumber { get; set; }
    }
}
