using System;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Identity;
using Ethos.Domain.Repositories;

namespace Ethos.Application.Booking
{
    public class BookingApplicationService : IBookingApplicationService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ICurrentUser _currentUser;

        public BookingApplicationService(
            IBookingRepository bookingRepository,
            IScheduleRepository scheduleRepository,
            ICurrentUser currentUser)
        {
            _bookingRepository = bookingRepository;
            _scheduleRepository = scheduleRepository;
            _currentUser = currentUser;
        }

        public async Task<Guid> CreateAsync(CreateBookingRequestDto input)
        {
            var currentUser = await _currentUser.GetCurrentUser();

            var schedule = await _scheduleRepository.GetByIdAsync(input.ScheduleId);

            var booking = new Domain.Booking.Booking(
                schedule,
                currentUser,
                input.StartDate,
                input.EndDate);

            return await _bookingRepository.CreateAsync(booking);
        }
    }
}
