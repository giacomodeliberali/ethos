using System;
using Ethos.Application.Contracts.Booking;
using MediatR;

namespace Ethos.Application.Commands
{
    public class CreateBookingCommand : IRequest<CreateBookingReplyDto>
    {
        public Guid ScheduleId { get; }

        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        public CreateBookingCommand(Guid scheduleId, DateTime startDate, DateTime endDate)
        {
            ScheduleId = scheduleId;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
