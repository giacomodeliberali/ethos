using System;
using System.Threading.Tasks;
using Ethos.Application.Commands;
using Ethos.Application.Contracts.Booking;
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
        public BookingApplicationService(
            IUnitOfWork unitOfWork,
            IMediator mediator)
        : base(mediator, unitOfWork)
        {
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
            return await Mediator.Send(new GetBookingByIdCommand(id));
        }
    }
}
