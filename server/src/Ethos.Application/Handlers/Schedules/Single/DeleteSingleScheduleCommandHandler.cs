using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ethos.Application.Commands.Schedules.Single;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Exceptions;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.Query.Services;
using MediatR;

namespace Ethos.Application.Handlers.Schedules.Single
{
    public class DeleteSingleScheduleCommandHandler : IRequestHandler<DeleteSingleScheduleCommand, DeleteScheduleReplyDto>
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

        public async Task<DeleteScheduleReplyDto> Handle(DeleteSingleScheduleCommand request, CancellationToken cancellationToken)
        {
            var result = new DeleteScheduleReplyDto
            {
                AffectedUsers = new List<DeleteScheduleReplyDto.UserDto>(),
            };

            var schedule = await _scheduleRepository.GetByIdAsync(request.Id);

            if (schedule is RecurringSchedule recurringSchedule)
            {
                throw new BusinessException("Can not delete recurring");
            }

            if (schedule is SingleSchedule singleSchedule)
            {
                await DeleteSchedule(singleSchedule);
            }

            await _unitOfWork.SaveChangesAsync();

            return result;
        }

        private async Task DeleteSchedule(SingleSchedule schedule)
        {
            var existingBookings = await _bookingQueryService.GetAllBookingsInRange(
                schedule.Id,
                new DateOnlyPeriod(schedule.StartDate, schedule.EndDate));

            if (existingBookings.Any())
            {
                throw new CanNotDeleteScheduleWithExistingBookingsException(existingBookings.Count);
            }

            await _scheduleRepository.DeleteAsync(schedule);
        }
    }
}
