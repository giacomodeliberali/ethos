using System;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Identity;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Guards;
using Ethos.Domain.Repositories;
using Ethos.Query.Services;
using Ethos.Shared;
using MediatR;

namespace Ethos.Application.Services
{
    public class BookingApplicationService : BaseApplicationService, IBookingApplicationService
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly IBookingRepository _bookingRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly IBookingQueryService _bookingQueryService;

        public BookingApplicationService(
            IUnitOfWork unitOfWork,
            IGuidGenerator guidGenerator,
            IBookingRepository bookingRepository,
            IScheduleRepository scheduleRepository,
            ICurrentUser currentUser,
            IMapper mapper,
            IBookingQueryService bookingQueryService,
            IMediator mediator)
        : base(mediator, unitOfWork)
        {
            _guidGenerator = guidGenerator;
            _bookingRepository = bookingRepository;
            _scheduleRepository = scheduleRepository;
            _currentUser = currentUser;
            _mapper = mapper;
            _bookingQueryService = bookingQueryService;
        }

        /// <inheritdoc />
        public async Task<CreateBookingReplyDto> CreateAsync(CreateBookingRequestDto input)
        {
            Guard.Against.NotUtc(input.StartDate, nameof(input.StartDate));
            Guard.Against.NotUtc(input.EndDate, nameof(input.EndDate));

            var schedule = await _scheduleRepository.GetByIdAsync(input.ScheduleId);

            if (schedule is RecurringSchedule recurringSchedule)
            {
                var bookingDuration = (int)(input.EndDate - input.StartDate).TotalMinutes;
                if (schedule.DurationInMinutes != bookingDuration)
                {
                    throw new BusinessException("Invalid booking duration.");
                }

                var occurrences = recurringSchedule.RecurringCronExpression.GetOccurrences(
                    input.StartDate,
                    input.EndDate,
                    fromInclusive: true,
                    toInclusive: true).ToList();

                if (occurrences.Count != 1 || occurrences.Single() < recurringSchedule.Period.StartDate || occurrences.Single() > recurringSchedule.Period.EndDate)
                {
                    throw new BusinessException("Invalid booking date/time.");
                }
            }
            else if (schedule is SingleSchedule singleSchedule)
            {
                if (input.StartDate < singleSchedule.Period.StartDate || input.EndDate > singleSchedule.Period.EndDate)
                {
                    throw new BusinessException("Invalid booking date/time.");
                }

                var bookingDuration = (int)(input.EndDate - input.StartDate).TotalMinutes;
                if (schedule.DurationInMinutes != bookingDuration)
                {
                    throw new BusinessException("Invalid booking duration.");
                }
            }

            var currentBookings = await _bookingQueryService.GetAllBookingsInRange(schedule.Id, input.StartDate, input.EndDate);

            if (schedule.ParticipantsMaxNumber > 0 &&
                currentBookings.Count >= schedule.ParticipantsMaxNumber)
            {
                throw new ParticipantsMaxNumberReached(schedule.ParticipantsMaxNumber);
            }

            var currentUser = await _currentUser.GetCurrentUser();

            var booking = Booking.Factory.Create(
                _guidGenerator.Create(),
                schedule,
                currentUser,
                input.StartDate,
                input.EndDate);

            await _bookingRepository.CreateAsync(booking);

            await UnitOfWork.SaveChangesAsync();

            return new CreateBookingReplyDto()
            {
                Id = booking.Id,
                CurrentParticipantsNumber = currentBookings.Count + 1,
            };
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking.User.Id != _currentUser.GetCurrentUserId() &&
                !await _currentUser.IsInRole(RoleConstants.Admin))
            {
                throw new BusinessException("You can only delete your own bookings!");
            }

            await _bookingRepository.DeleteAsync(booking);
            await UnitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<BookingDto> GetByIdAsync(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            return _mapper.Map<BookingDto>(booking);
        }
    }
}
