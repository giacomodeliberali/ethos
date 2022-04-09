using System;
using Ethos.Application.Contracts.Booking;
using MediatR;

namespace Ethos.Application.Commands.Booking
{
    public class CreateBookingCommand : IRequest<CreateBookingReplyDto>
    {
        public Guid ScheduleId { get; }

        public DateTimeOffset StartDate { get; }

        public DateTimeOffset EndDate { get; }

        public CreateBookingCommand(Guid scheduleId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            ScheduleId = scheduleId;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
