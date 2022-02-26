using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Application.Commands.Booking;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Identity;
using Ethos.Application.Queries;
using Ethos.Domain.Repositories;
using MediatR;

namespace Ethos.Application.Services
{
    /// <summary>
    /// Contains the use cases for the web UI.
    /// </summary>
    public class BookingApplicationService : BaseApplicationService, IBookingApplicationService
    {
        private readonly ICurrentUser _currentUser;

        public BookingApplicationService(
            ICurrentUser currentUser,
            IUnitOfWork unitOfWork,
            IMediator mediator)
        : base(mediator, unitOfWork)
        {
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<CreateBookingReplyDto> CreateAsync(CreateBookingRequestDto input)
        {
            return await Mediator.Send(new CreateBookingCommand(
                input.ScheduleId,
                input.StartDate,
                input.EndDate));
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id)
        {
            await Mediator.Send(new DeleteBookingCommand(id));
        }

        /// <inheritdoc />
        public async Task<BookingDto> GetByIdAsync(Guid id)
        {
            return await Mediator.Send(new GetBookingByIdQuery(id));
        }

        public async Task<IEnumerable<BookingDto>> GetFutureBookings()
        {
            return await Mediator.Send(new GetFutureBookingsQuery(_currentUser.UserId()));
        }
    }
}
