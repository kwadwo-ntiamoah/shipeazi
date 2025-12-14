using Shipeazi.Domain.src.Entities;

namespace Shipeazi.Application.src.Repositories
{
    public interface IOtpVerificationRepository
    {
        Task<OtpVerification?> GetLatestByPhoneAsync(string phoneNumber, string countryCode, string purpose);
        Task<OtpVerification?> GetByIdAsync(Guid id);
        Task AddAsync(OtpVerification otp);
        Task UpdateAsync(OtpVerification otp);
        Task<bool> HasValidOtpAsync(string phoneNumber, string countryCode, string purpose);
    }
}
