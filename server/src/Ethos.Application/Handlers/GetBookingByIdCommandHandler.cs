using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Queries;
using Ethos.Domain.Repositories;
using MediatR;

namespace Ethos.Application.Handlers
{
    public class GetBookingByIdCommandHandler : IRequestHandler<GetBookingByIdCommand, BookingDto>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public GetBookingByIdCommandHandler(
            IBookingRepository bookingRepository,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<BookingDto> Handle(GetBookingByIdCommand request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.Id);
            return _mapper.Map<BookingDto>(booking);
        }
    }
}
