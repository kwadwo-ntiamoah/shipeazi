namespace Shipeazi.Application.src.Services
{
    public interface IJwtTokenService
    {
        (string AccessToken, DateTime AccessTokenExpiresAt) GenerateAccessToken(string userId, string phoneNumber, bool isProfileComplete);
        string GenerateRefreshToken();
        string? ValidateRefreshToken(string token);
    }
}
