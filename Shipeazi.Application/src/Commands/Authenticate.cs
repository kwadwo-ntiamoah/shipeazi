using ErrorOr;
using Orchestrix.Mediator;
using Orchestrix.Mediator.Cqrs;
using Shipeazi.Application.src.EventHandlers;
using Shipeazi.Application.src.Repositories;
using Shipeazi.Application.src.Services;
using Shipeazi.Domain.src.Entities;

namespace Shipeazi.Application.src.Commands
{
    public class AuthenticateCommand(string phoneNumber, string countryCode, string otpCode) : ICommand<ErrorOr<AuthenticateResult>>
    {
        public string PhoneNumber { get; set; } = phoneNumber;
        public string CountryCode { get; set; } = countryCode;
        public string OtpCode { get; set; } = otpCode;
    }

    public class AuthenticateResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiresAt { get; set; }
        public bool IsProfileComplete { get; set; }
        public string UserId { get; set; } = string.Empty;
        public bool IsNewUser { get; set; } // true for registration, false for login
    }

    public class AuthenticateCommandHandler(
        IUserRepository userRepository, 
        IRefreshTokenRepository refreshTokenRepository,
        IOtpVerificationRepository otpRepository,
        IMediator mediator, 
        IJwtTokenService jwtTokenService,
        IHttpContextService httpContextService) : ICommandHandler<AuthenticateCommand, ErrorOr<AuthenticateResult>>
    {
        public async ValueTask<ErrorOr<AuthenticateResult>> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if user already exists
                var username = $"{request.CountryCode}{request.PhoneNumber}";
                var existingUser = await userRepository.GetByPhoneNumberAsync(username);
                
                // Determine the purpose based on whether user exists
                var purpose = existingUser == null ? "registration" : "login";

                // Get the latest OTP for verification
                var latestOtp = await otpRepository.GetLatestByPhoneAsync(
                    request.PhoneNumber,
                    request.CountryCode,
                    purpose
                );

                if (latestOtp == null)
                {
                    return Error.Validation(description: $"Please request an OTP for {purpose} first");
                }

                // Check if OTP has already been used
                if (latestOtp.IsUsed)
                {
                    return Error.Unauthorized(description: "This OTP has already been used. Please request a new one.");
                }

                // Verify the OTP
                var isValid = latestOtp.Verify(request.OtpCode);
                if (!isValid)
                {
                    if (latestOtp.IsExpired)
                    {
                        await otpRepository.UpdateAsync(latestOtp);
                        return Error.Unauthorized(description: "OTP has expired. Please request a new one.");
                    }
                    if (!latestOtp.CanRetry)
                    {
                        await otpRepository.UpdateAsync(latestOtp);
                        return Error.Unauthorized(description: "Too many failed attempts. Please request a new OTP.");
                    }
                    await otpRepository.UpdateAsync(latestOtp);
                    return Error.Unauthorized(description: "Invalid OTP code");
                }

                // Mark OTP as used to prevent reuse
                latestOtp.MarkAsUsed();
                await otpRepository.UpdateAsync(latestOtp);

                User user;
                bool isNewUser = false;

                if (existingUser == null)
                {
                    // Registration: Create new user
                    user = new User(
                        phone: new(countryCode: request.CountryCode, value: request.PhoneNumber)
                    );

                    await userRepository.AddAsync(user);
                    await mediator.Publish(new UserCreatedNotification(user), cancellationToken);
                    isNewUser = true;
                }
                else
                {
                    // Login: Use existing user
                    user = existingUser;
                }
                
                // Generate access token (15 minutes)
                var (accessToken, accessTokenExpiresAt) = jwtTokenService.GenerateAccessToken(
                    userId: user.Id.ToString(),
                    phoneNumber: user.Phone.Value,
                    isProfileComplete: user.IsProfileComplete()
                );

                // Generate refresh token (60 days)
                var refreshTokenString = jwtTokenService.GenerateRefreshToken();
                var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(60);
                var clientIp = httpContextService.GetClientIpAddress();

                var refreshToken = new RefreshToken(
                    userId: user.Id.ToString(),
                    token: refreshTokenString,
                    expiresAt: refreshTokenExpiresAt,
                    createdByIp: clientIp
                );

                await refreshTokenRepository.AddAsync(refreshToken);

                return new AuthenticateResult
                {
                    AccessToken = accessToken,
                    AccessTokenExpiresAt = accessTokenExpiresAt,
                    RefreshToken = refreshTokenString,
                    RefreshTokenExpiresAt = refreshTokenExpiresAt,
                    IsProfileComplete = user.IsProfileComplete(),
                    UserId = user.Id.ToString(),
                    IsNewUser = isNewUser
                };

            } catch (Exception ex)
            {
                return Error.Failure(description: ex.Message);
            }
        }
    }
}
