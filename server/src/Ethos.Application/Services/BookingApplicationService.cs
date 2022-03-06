using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Application.Commands.Booking;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Identity;
using Ethos.Application.Queries;
using MediatR;

namespace Ethos.Application.Services
{
    /// <summary>
    /// Contains the use cases for the web UI.
    /// </summary>
    public class BookingApplicationService : IBookingApplicationService
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUser _currentUser;

        public BookingApplicationService(
            ICurrentUser currentUser,
            IMediator mediator)
        {
            _currentUser = currentUser;
            _mediator = mediator;
        }

        /// <inheritdoc />
        public async Task<CreateBookingReplyDto> CreateAsync(CreateBookingRequestDto input)
        {
            return await _mediator.Send(new CreateBookingCommand(
                input.ScheduleId,
                input.StartDate,
                input.EndDate));
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id)
        {
            await _mediator.Send(new DeleteBookingCommand(id));
        }

        /// <inheritdoc />
        public async Task<BookingDto> GetByIdAsync(Guid id)
        {
            return await _mediator.Send(new GetBookingByIdQuery(id));
        }

        public async Task<IEnumerable<BookingDto>> GetFutureBookings()
        {
            return await _mediator.Send(new GetFutureBookingsQuery(_currentUser.UserId()));
        }
    }
}
