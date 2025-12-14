using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipeazi.Domain.src.Entities;

namespace Shipeazi.Infrastructure.src.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(rt => rt.ExpiresAt)
                .IsRequired();

            builder.Property(rt => rt.CreatedAt)
                .IsRequired();

            builder.Property(rt => rt.CreatedByIp)
                .IsRequired()
                .HasMaxLength(45);

            builder.Property(rt => rt.RevokedByIp)
                .HasMaxLength(45);

            builder.Property(rt => rt.ReplacedByToken)
                .HasMaxLength(500);

            // Indexes for performance
            builder.HasIndex(rt => rt.Token);
            builder.HasIndex(rt => rt.UserId);
            builder.HasIndex(rt => rt.ExpiresAt);
        }
    }
}
