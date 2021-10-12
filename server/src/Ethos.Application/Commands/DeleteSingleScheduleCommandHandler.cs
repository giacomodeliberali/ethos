using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.Query.Services;
using MediatR;

namespace Ethos.Application.Commands
{
    public class DeleteSingleScheduleCommandHandler : AsyncRequestHandler<DeleteSingleScheduleCommand>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookingQueryService _bookingQueryService;

        public DeleteSingleScheduleCommandHandler(
            IScheduleRepository scheduleRepository,
            IUnitOfWork unitOfWork,
            IBookingQueryService bookingQueryService)
        {
            _scheduleRepository = scheduleRepository;
            _unitOfWork = unitOfWork;
            _bookingQueryService = bookingQueryService;
        }

        protected override async Task Handle(DeleteSingleScheduleCommand request, CancellationToken cancellationToken)
        {
            var existingBookings = await _bookingQueryService.GetAllBookingsInRange(
                request.Schedule.Id,
                request.Schedule.StartDate,
                request.Schedule.EndDate);

            if (existingBookings.Any())
            {
                throw new BusinessException($"Non è possibile eliminare la schedulazione, sono presenti {existingBookings.Count} prenotazioni");
            }

            await _scheduleRepository.DeleteAsync(request.Schedule);
        }
    }
}
