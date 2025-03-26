using AutoMapper;
using DotNg.Application.Models.Auth;
using DotNg.Application.Models.RoleDto;
using DotNg.Infrastructure.Authentication.Identity.Models;

namespace DotNg.Application.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<AppUser, CustomUserResponse>();
        CreateMap<Role, RoleResponse>();
    }
}