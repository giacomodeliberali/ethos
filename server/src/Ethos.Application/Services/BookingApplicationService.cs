using System;
using System.Threading.Tasks;
using AutoMapper;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Identity;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;

namespace Ethos.Application.Services
{
    public class BookingApplicationService : BaseApplicationService, IBookingApplicationService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;

        public BookingApplicationService(
            IUnitOfWork unitOfWork,
            IGuidGenerator guidGenerator,
            IBookingRepository bookingRepository,
            IScheduleRepository scheduleRepository,
            ICurrentUser currentUser,
            IMapper mapper)
        : base(unitOfWork, guidGenerator)
        {
            _bookingRepository = bookingRepository;
            _scheduleRepository = scheduleRepository;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<CreateBookingReplyDto> CreateAsync(CreateBookingRequestDto input)
        {
            var currentUser = await _currentUser.GetCurrentUser();

            var schedule = await _scheduleRepository.GetByIdAsync(input.ScheduleId);

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
            };
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking.User.Id != _currentUser.GetCurrentUserId())
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
