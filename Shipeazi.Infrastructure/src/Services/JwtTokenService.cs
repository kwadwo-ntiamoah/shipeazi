using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shipeazi.Application.src.Services;

namespace Shipeazi.Infrastructure.src.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string AccessToken, DateTime AccessTokenExpiresAt) GenerateAccessToken(string userId, string phoneNumber, bool isProfileComplete)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("phone", phoneNumber),
                new Claim("profileComplete", isProfileComplete.ToString())
            };

            // Short-lived access token (15 minutes)
            var expiresAt = DateTime.UtcNow.AddMinutes(15);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return (tokenString, expiresAt);
        }

        public string GenerateRefreshToken()
        {
            // Generate a cryptographically secure random token
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public string? ValidateRefreshToken(string token)
        {
            // Basic validation - just check if it's a valid base64 string
            try
            {
                Convert.FromBase64String(token);
                return token;
            }
            catch
            {
                return null;
            }
        }
    }
}
