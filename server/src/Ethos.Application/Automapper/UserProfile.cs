using AutoMapper;
using Ethos.Application.Contracts.Identity;
using Ethos.Domain.Entities;
using Ethos.Query.Projections;

namespace Ethos.Application.Automapper;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<ApplicationUser, UserDto>();
        CreateMap<UserProjection, UserDto>();
    }
}