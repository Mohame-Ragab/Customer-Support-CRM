using AutoMapper;
using CustomerSupportCRM.Application.Features.Users.DTOs;

namespace CustomerSupportCRM.Infrastructure.Identity.Mappings;

public sealed class UserProfileMappingProfile : Profile
{
    public UserProfileMappingProfile()
    {
        CreateMap<ApplicationUser, UserProfileDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Roles, opt => opt.Ignore()); // Set manually in service
    }
}
