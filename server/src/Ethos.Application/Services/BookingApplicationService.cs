using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Identity;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.Query.Services;
using Ethos.Shared;

namespace Ethos.Application.Services
{
    public class BookingApplicationService : BaseApplicationService, IBookingApplicationService
    {
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
            IBookingQueryService bookingQueryService)
        : base(unitOfWork, guidGenerator)
        {
            _bookingRepository = bookingRepository;
            _scheduleRepository = scheduleRepository;
            _currentUser = currentUser;
            _mapper = mapper;
            _bookingQueryService = bookingQueryService;
        }

        /// <inheritdoc />
        public async Task<CreateBookingReplyDto> CreateAsync(CreateBookingRequestDto input)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(input.ScheduleId);

            if (schedule is RecurringSchedule recurringSchedule)
            {
                if (input.StartDate < recurringSchedule.StartDate || input.EndDate > recurringSchedule.EndDate)
                {
                    throw new BusinessException("Invalid booking date/time.");
                }

                var bookingDuration = (int)(input.EndDate - input.StartDate).TotalMinutes;
                if (schedule.DurationInMinutes != bookingDuration)
                {
                    throw new BusinessException("Invalid booking duration.");
                }

                var nextOccurrences = recurringSchedule.RecurringCronExpression.GetOccurrences(
                    fromUtc: input.StartDate,
                    toUtc: input.EndDate,
                    fromInclusive: true,
                    toInclusive: true);

                if (!nextOccurrences.Any())
                {
                    throw new BusinessException("Invalid booking date/time for a recurring schedule");
                }
            }
            else if (schedule is SingleSchedule singleSchedule)
            {
                if (input.StartDate < singleSchedule.StartDate || input.EndDate > singleSchedule.EndDate)
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
                GuidGenerator.Create(),
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
