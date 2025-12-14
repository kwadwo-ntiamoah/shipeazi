using Shipeazi.Domain.src.Common;

namespace Shipeazi.Domain.src.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string UserId { get; private set; } = string.Empty;
        public string Token { get; private set; } = string.Empty;
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string? ReplacedByToken { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public string? RevokedByIp { get; private set; }
        public string CreatedByIp { get; private set; } = string.Empty;

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsRevoked && !IsExpired;

        // Private constructor for EF Core
        private RefreshToken() { }

        public RefreshToken(string userId, string token, DateTime expiresAt, string createdByIp)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Token = token ?? throw new ArgumentNullException(nameof(token));
            ExpiresAt = expiresAt;
            CreatedAt = DateTime.UtcNow;
            CreatedByIp = createdByIp ?? throw new ArgumentNullException(nameof(createdByIp));
        }

        public void Revoke(string revokedByIp, string? replacedByToken = null)
        {
            RevokedAt = DateTime.UtcNow;
            RevokedByIp = revokedByIp;
            ReplacedByToken = replacedByToken;
        }
    }
}
