using Microsoft.EntityFrameworkCore;
using Shipeazi.Application.src.Repositories;
using Shipeazi.Domain.src.Entities;
using Shipeazi.Infrastructure.src.Persistence;

namespace Shipeazi.Infrastructure.src.Persistence.Repositories
{
    public class OtpVerificationRepository(AppDbContext context) : IOtpVerificationRepository
    {
        public async Task<OtpVerification?> GetLatestByPhoneAsync(string phoneNumber, string countryCode, string purpose)
        {
            return await context.OtpVerifications
                .Where(o => o.PhoneNumber == phoneNumber 
                       && o.CountryCode == countryCode 
                       && o.Purpose == purpose)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<OtpVerification?> GetByIdAsync(Guid id)
        {
            return await context.OtpVerifications.FindAsync(id);
        }

        public async Task AddAsync(OtpVerification otp)
        {
            await context.OtpVerifications.AddAsync(otp);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OtpVerification otp)
        {
            context.OtpVerifications.Update(otp);
            await context.SaveChangesAsync();
        }

        public async Task<bool> HasValidOtpAsync(string phoneNumber, string countryCode, string purpose)
        {
            return await context.OtpVerifications
                .AnyAsync(o => o.PhoneNumber == phoneNumber 
                          && o.CountryCode == countryCode 
                          && o.Purpose == purpose
                          && o.IsVerified 
                          && o.ExpiresAt > DateTime.UtcNow);
        }
    }
}
