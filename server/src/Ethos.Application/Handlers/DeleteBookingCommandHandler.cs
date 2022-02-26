using System;
using System.Threading;
using System.Threading.Tasks;
using Ethos.Application.Commands.Booking;
using Ethos.Application.Identity;
using Ethos.Common;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using MediatR;

namespace Ethos.Application.Handlers
{
    public class DeleteBookingCommandHandler : AsyncRequestHandler<DeleteBookingCommand>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBookingCommandHandler(
            IBookingRepository bookingRepository,
            ICurrentUser currentUser,
            IUnitOfWork unitOfWork)
        {
            _bookingRepository = bookingRepository;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        protected override async Task Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.Id);

            if (booking.StartDate < DateTime.UtcNow)
            {
                throw new BusinessException("You can not delete a booking in the past");
            }

            if (booking.User.Id != _currentUser.UserId() &&
                !await _currentUser.IsInRole(RoleConstants.Admin))
            {
                throw new BusinessException("You can only delete your own bookings!");
            }

            await _bookingRepository.DeleteAsync(booking);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
