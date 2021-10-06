using System;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Identity;
using Ethos.Domain.Entities;
using Ethos.Domain.Repositories;

namespace Ethos.Application.Services
{
    public class BookingApplicationService : BaseApplicationService, IBookingApplicationService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ICurrentUser _currentUser;

        public BookingApplicationService(
            IUnitOfWork unitOfWork,
            IBookingRepository bookingRepository,
            IScheduleRepository scheduleRepository,
            ICurrentUser currentUser)
        : base(unitOfWork)
        {
            _bookingRepository = bookingRepository;
            _scheduleRepository = scheduleRepository;
            _currentUser = currentUser;
        }

        public async Task<Guid> CreateAsync(CreateBookingRequestDto input)
        {
            var currentUser = await _currentUser.GetCurrentUser();

            var schedule = await _scheduleRepository.GetByIdAsync(input.ScheduleId);

            var booking = Booking.Factory.Create(
                schedule,
                currentUser,
                input.StartDate,
                input.EndDate);

            var bookingId = await _bookingRepository.CreateAsync(booking);

            await UnitOfWork.SaveChangesAsync();

            return bookingId;
        }
    }
}
