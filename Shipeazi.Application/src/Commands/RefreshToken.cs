using ErrorOr;
using Orchestrix.Mediator;
using Orchestrix.Mediator.Cqrs;
using Shipeazi.Application.src.Repositories;
using Shipeazi.Application.src.Services;
using Shipeazi.Domain.src.Entities;

namespace Shipeazi.Application.src.Commands
{
    public class RefreshTokenCommand(string refreshToken) : ICommand<ErrorOr<RefreshTokenResult>>
    {
        public string RefreshToken { get; set; } = refreshToken;
    }

    public class RefreshTokenResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiresAt { get; set; }
        public bool IsProfileComplete { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IHttpContextService httpContextService) : ICommandHandler<RefreshTokenCommand, ErrorOr<RefreshTokenResult>>
    {
        public async ValueTask<ErrorOr<RefreshTokenResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate the refresh token format
                var validatedToken = jwtTokenService.ValidateRefreshToken(request.RefreshToken);
                if (validatedToken == null)
                {
                    return Error.Validation(description: "Invalid refresh token format");
                }

                // Get the refresh token from database
                var refreshToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
                
                if (refreshToken == null)
                {
                    return Error.NotFound(description: "Refresh token not found");
                }

                // Check if token is active (not expired and not revoked)
                if (!refreshToken.IsActive)
                {
                    return Error.Unauthorized(description: "Refresh token is no longer valid");
                }

                // Get the user
                var user = await userRepository.GetByIdAsync(Guid.Parse(refreshToken.UserId));
                if (user == null)
                {
                    return Error.NotFound(description: "User not found");
                }

                // Check if profile is complete
                var isProfileComplete = user.IsProfileComplete();

                // Generate new access token (15 minutes)
                var (accessToken, accessTokenExpiresAt) = jwtTokenService.GenerateAccessToken(
                    userId: user.Id.ToString(),
                    phoneNumber: user.Phone.Value,
                    isProfileComplete: isProfileComplete
                );

                // Generate new refresh token and revoke the old one (rotation for security)
                var newRefreshTokenString = jwtTokenService.GenerateRefreshToken();
                var newRefreshTokenExpiresAt = DateTime.UtcNow.AddDays(60);
                var clientIp = httpContextService.GetClientIpAddress();

                var newRefreshToken = new RefreshToken(
                    userId: user.Id.ToString(),
                    token: newRefreshTokenString,
                    expiresAt: newRefreshTokenExpiresAt,
                    createdByIp: clientIp
                );

                // Revoke old token and save new one
                refreshToken.Revoke(clientIp, newRefreshTokenString);
                await refreshTokenRepository.UpdateAsync(refreshToken);
                await refreshTokenRepository.AddAsync(newRefreshToken);

                return new RefreshTokenResult
                {
                    AccessToken = accessToken,
                    AccessTokenExpiresAt = accessTokenExpiresAt,
                    RefreshToken = newRefreshTokenString,
                    RefreshTokenExpiresAt = newRefreshTokenExpiresAt,
                    IsProfileComplete = isProfileComplete,
                    UserId = user.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: ex.Message);
            }
        }
    }
}
