using Shipeazi.Domain.src.Common;

namespace Shipeazi.Domain.src.Entities
{
    public class OtpVerification : BaseEntity
    {
        public string PhoneNumber { get; private set; } = string.Empty;
        public string CountryCode { get; private set; } = string.Empty;
        public string OtpCode { get; private set; } = string.Empty;
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsVerified { get; private set; }
        public DateTime? VerifiedAt { get; private set; }
        public bool IsUsed { get; private set; }
        public DateTime? UsedAt { get; private set; }
        public int AttemptsCount { get; private set; }
        public string Purpose { get; private set; } = string.Empty; // "registration", "login", "password_reset"

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsValid => IsVerified && !IsExpired && !IsUsed;
        public bool CanRetry => AttemptsCount < 5; // Max 5 attempts

        // Private constructor for EF Core
        private OtpVerification() { }

        public OtpVerification(string phoneNumber, string countryCode, string otpCode, string purpose)
        {
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            CountryCode = countryCode ?? throw new ArgumentNullException(nameof(countryCode));
            OtpCode = otpCode ?? throw new ArgumentNullException(nameof(otpCode));
            Purpose = purpose ?? throw new ArgumentNullException(nameof(purpose));
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = DateTime.UtcNow.AddMinutes(10); // OTP valid for 10 minutes
            IsVerified = false;
            AttemptsCount = 0;
        }

        public bool Verify(string otpCode)
        {
            AttemptsCount++;

            if (IsExpired)
            {
                return false;
            }

            if (!CanRetry)
            {
                return false;
            }

            if (OtpCode == otpCode)
            {
                IsVerified = true;
                VerifiedAt = DateTime.UtcNow;
                return true;
            }

            return false;
        }

        public void MarkAsUsed()
        {
            if (!IsVerified)
            {
                throw new InvalidOperationException("Cannot mark an unverified OTP as used");
            }

            IsUsed = true;
            UsedAt = DateTime.UtcNow;
        }
    }
}
