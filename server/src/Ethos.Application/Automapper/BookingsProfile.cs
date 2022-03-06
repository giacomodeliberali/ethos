using AutoMapper;
using Ethos.Application.Contracts.Booking;
using Ethos.Domain.Entities;
using Ethos.Query.Projections;

namespace Ethos.Application.Automapper;

public class BookingsProfile : Profile
{
    public BookingsProfile()
    {
        CreateMap<Booking, BookingDto>();
        CreateMap<ApplicationUser, BookingDto.UserDto>();
        CreateMap<Schedule, BookingDto.ScheduleDto>();
        
        CreateMap<BookingProjection, BookingDto>()
            .ForMember(dest => dest.User, opt => opt.MapFrom((src => new BookingDto.UserDto()
            {
                Id = src.UserId,
                FullName = src.UserFullName,
            })))
            .ForMember(dest => dest.Schedule, opt => opt.MapFrom(src => new BookingDto.ScheduleDto()
            {
                Id = src.ScheduleId,
                Name = src.ScheduleName,
                Description = src.ScheduleDescription,
                DurationInMinutes = src.ScheduleDurationInMinutes,
                OrganizerFullName = src.ScheduleOrganizerFullName,
            }));
    }
}