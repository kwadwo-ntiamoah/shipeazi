using AutoMapper;
using Shipeazi.API.src.DTOs.Requests;
using Shipeazi.API.src.DTOs.Responses;
using Shipeazi.Application.src.Commands;
using Shipeazi.Domain.src.Entities;
using Shipeazi.Infrastructure.src.Identity;

namespace Shipeazi.API.src.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map AppUser to User using the Load factory method
            CreateMap<AppUser, User>()
                .ConstructUsing(appUser => User.Load(
                    Guid.Parse(appUser.Id),
                    appUser.Phone,
                    appUser.Email,
                    appUser.DisplayName,
                    appUser.Address,
                    appUser.ShipeaziAddress
                ));

            // Authenticate (unified login/register with OTP)
            CreateMap<AuthenticateRequest, AuthenticateCommand>();
            CreateMap<AuthenticateResult, AuthenticateResponse>()
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => 
                    src.IsNewUser ? "Account created successfully" : "Login successful"));
            
            CreateMap<RefreshTokenRequest, RefreshTokenCommand>();
            CreateMap<RefreshTokenResult, RefreshTokenResponse>();

            CreateMap<CompleteProfileRequest, CompleteProfileCommand>();
            CreateMap<CompleteProfileResult, CompleteProfileResponse>();

            CreateMap<RequestOtpRequest, RequestOtpCommand>();
            CreateMap<RequestOtpResult, RequestOtpResponse>();
        }
    }
}