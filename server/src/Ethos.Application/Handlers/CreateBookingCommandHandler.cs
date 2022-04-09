using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ethos.Application.Commands.Booking;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Identity;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.Query.Services;
using MediatR;

namespace Ethos.Application.Handlers
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, CreateBookingReplyDto>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingQueryService _bookingQueryService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly IGuidGenerator _guidGenerator;

        public CreateBookingCommandHandler(
            IScheduleRepository scheduleRepository,
            IBookingRepository bookingRepository,
            IBookingQueryService bookingQueryService,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser,
            IGuidGenerator guidGenerator)
        {
            _scheduleRepository = scheduleRepository;
            _bookingRepository = bookingRepository;
            _bookingQueryService = bookingQueryService;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _guidGenerator = guidGenerator;
        }

        public async Task<CreateBookingReplyDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            if (request.StartDate < DateTime.UtcNow)
            {
                throw new BusinessException("You can not book in the past");
            }

            var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId);

            if (schedule is RecurringSchedule recurringSchedule)
            {
                var bookingDuration = (int)(request.EndDate - request.StartDate).TotalMinutes;
                if (schedule.DurationInMinutes != bookingDuration)
                {
                    throw new BusinessException("Invalid booking duration.");
                }

                var occurrences = recurringSchedule.GetOccurrences(
                    new DateOnlyPeriod(request.StartDate, request.EndDate),
                    schedule.TimeZone)
                    .ToList();

                if (occurrences.Count != 1)
                {
                    throw new BusinessException("Invalid booking date/time.");
                }
            }
            else if (schedule is SingleSchedule singleSchedule)
            {
                if (request.StartDate < singleSchedule.StartDate || request.EndDate > singleSchedule.EndDate)
                {
                    throw new BusinessException("Invalid booking date/time.");
                }

                var bookingDuration = (int)(request.EndDate - request.StartDate).TotalMinutes;
                if (schedule.DurationInMinutes != bookingDuration)
                {
                    throw new BusinessException("Invalid booking duration.");
                }
            }

            var currentBookings = await _bookingQueryService.GetAllBookingsInRange(
                schedule.Id,
                new DateOnlyPeriod(request.StartDate, request.EndDate));

            if (schedule.ParticipantsMaxNumber > 0 &&
                currentBookings.Count >= schedule.ParticipantsMaxNumber)
            {
                throw new ParticipantsMaxNumberReachedException(schedule.ParticipantsMaxNumber);
            }

            var currentUser = await _currentUser.GetCurrentUser();

            var booking = Booking.Factory.Create(
                _guidGenerator.Create(),
                schedule,
                currentUser,
                request.StartDate,
                request.EndDate);

            await _bookingRepository.CreateAsync(booking);

            await _unitOfWork.SaveChangesAsync();

            return new CreateBookingReplyDto()
            {
                Id = booking.Id,
                CurrentParticipantsNumber = currentBookings.Count + 1,
            };
        }
    }
}
