namespace Shipeazi.API.src.DTOs.Responses
{
    public class AuthenticateResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiresAt { get; set; }
        public bool IsProfileComplete { get; set; }
        public string UserId { get; set; } = string.Empty;
        public bool IsNewUser { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
