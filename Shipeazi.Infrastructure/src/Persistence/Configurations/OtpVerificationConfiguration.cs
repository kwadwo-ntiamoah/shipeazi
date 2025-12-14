using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipeazi.Domain.src.Entities;

namespace Shipeazi.Infrastructure.src.Persistence.Configurations
{
    public class OtpVerificationConfiguration : IEntityTypeConfiguration<OtpVerification>
    {
        public void Configure(EntityTypeBuilder<OtpVerification> builder)
        {
            builder.ToTable("OtpVerifications");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(o => o.CountryCode)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(o => o.OtpCode)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(o => o.Purpose)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.ExpiresAt)
                .IsRequired();

            builder.Property(o => o.CreatedAt)
                .IsRequired();

            builder.Property(o => o.IsVerified)
                .IsRequired();

            builder.Property(o => o.AttemptsCount)
                .IsRequired();

            // Indexes for performance
            builder.HasIndex(o => new { o.PhoneNumber, o.CountryCode });
            builder.HasIndex(o => o.ExpiresAt);
            builder.HasIndex(o => o.IsVerified);
        }
    }
}
