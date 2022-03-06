using AutoMapper;
using Ethos.Application.Contracts.Booking;
using Ethos.Application.Contracts.Schedule;
using Ethos.Domain.Entities;

namespace Ethos.Application.Automapper;

public class ScheduleProfile : Profile
{
    public ScheduleProfile()
    {
        CreateMap<ApplicationUser, GeneratedScheduleDto.UserDto>();
    }
}