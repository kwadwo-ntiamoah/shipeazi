using Microsoft.EntityFrameworkCore;
using Shipeazi.Application.src.Repositories;
using Shipeazi.Domain.src.Entities;
using Shipeazi.Infrastructure.src.Persistence;

namespace Shipeazi.Infrastructure.src.Persistence.Repositories
{
    public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
    {
        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(string userId)
        {
            return await context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(rt => rt.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await context.RefreshTokens.AddAsync(refreshToken);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RefreshToken refreshToken)
        {
            context.RefreshTokens.Update(refreshToken);
            await context.SaveChangesAsync();
        }

        public async Task RevokeAllUserTokensAsync(string userId, string revokedByIp)
        {
            var tokens = await context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.Revoke(revokedByIp);
            }

            await context.SaveChangesAsync();
        }
    }
}
