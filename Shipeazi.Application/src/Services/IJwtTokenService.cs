namespace Shipeazi.Application.src.Services
{
    public interface IJwtTokenService
    {
        (string AccessToken, DateTime AccessTokenExpiresAt) GenerateAccessToken(string userId, string phoneNumber);
        string GenerateRefreshToken();
        string? ValidateRefreshToken(string token);
    }
}
